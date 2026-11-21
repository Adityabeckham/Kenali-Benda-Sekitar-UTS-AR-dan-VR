using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class UTSFinalSetup
{
    const string ScenePath = "Assets/Scenes/MainAR.unity";
    const string PackagePath = "AR_DATABASE.unitypackage";
    const string FbxPath = "Assets/Art/Models/casio watch improved.fbx";
    const string SecondTargetName = "image-target-2-jamcasio";
    const string SecondDatasetPath = "Vuforia/AR_DATABASE.xml";

    [MenuItem("UTS/Final Setup (2 targets + visibility)")]
    public static void Run()
    {
        ImportDatabase();
        OpenScene();
        FixVuforiaMaxTargets();
        SetupPokemonContent();
        SetupSecondTarget();
        UpdateInfoPanel();
        SaveScene();
        Debug.Log("[UTS] Final setup complete");
    }

    static void ImportDatabase()
    {
        var abs = Path.GetFullPath(PackagePath);
        if (!File.Exists(abs))
        {
            Debug.LogError("[UTS] Package not found: " + abs);
            return;
        }

        if (File.Exists("Assets/StreamingAssets/Vuforia/AR_DATABASE.xml"))
        {
            Debug.Log("[UTS] AR_DATABASE already imported");
            return;
        }

        AssetDatabase.ImportPackage(abs, false);
        AssetDatabase.Refresh();
        Debug.Log("[UTS] AR_DATABASE package imported");
    }

    static void OpenScene()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
    }

    static void FixVuforiaMaxTargets()
    {
        var path = "Assets/Resources/VuforiaConfiguration.asset";
        var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if (asset == null)
        {
            Debug.LogWarning("[UTS] VuforiaConfiguration not found at " + path);
            return;
        }

        var so = new SerializedObject(asset);
        var prop = so.FindProperty("vuforia.maxSimultaneousImageTargets");
        if (prop == null)
        {
            Debug.LogWarning("[UTS] maxSimultaneousImageTargets property not found");
            return;
        }

        if (prop.intValue < 2)
        {
            prop.intValue = 2;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            Debug.Log("[UTS] maxSimultaneousImageTargets -> 2");
        }
    }

    static void SetupPokemonContent()
    {
        var target = GameObject.Find("ImageTarget");
        if (target == null)
        {
            Debug.LogError("[UTS] ImageTarget not found");
            return;
        }

        var handler = target.GetComponent<DefaultObserverEventHandler>();
        if (handler == null)
        {
            Debug.LogError("[UTS] DefaultObserverEventHandler not found on ImageTarget");
            return;
        }

        handler.StatusFilter = DefaultObserverEventHandler.TrackingStatusFilter.Tracked;

        var content = target.transform.Find("Content_Pokemon");
        if (content == null)
        {
            var go = new GameObject("Content_Pokemon");
            go.transform.SetParent(target.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            content = go.transform;

            var pikachu = GameObject.Find("pikachu");
            if (pikachu != null && pikachu.transform.parent == target.transform)
                pikachu.transform.SetParent(content, true);

            var label = target.transform.Find("Label_Pikachu");
            if (label != null)
                label.SetParent(content, true);

            go.SetActive(false);
        }

        var vis = target.GetComponent<ContentVisibility>();
        if (vis == null)
            vis = target.AddComponent<ContentVisibility>();
        vis.content = content.gameObject;

        WireEvents(handler, vis);
        Debug.Log("[UTS] Pokemon content visibility wired");
    }

    static void SetupSecondTarget()
    {
        var existing = Object.FindObjectsByType<Vuforia.ImageTargetBehaviour>(FindObjectsSortMode.None)
            .FirstOrDefault(t => t.TargetName == SecondTargetName);
        if (existing != null)
        {
            Debug.Log("[UTS] Second target already exists");
            return;
        }

        var first = GameObject.Find("ImageTarget");
        if (first == null)
        {
            Debug.LogError("[UTS] ImageTarget not found");
            return;
        }

        if (!File.Exists("Assets/StreamingAssets/Vuforia/AR_DATABASE.xml"))
        {
            Debug.LogError("[UTS] AR_DATABASE.xml missing — import package first");
            return;
        }

        var it2 = Object.Instantiate(first, first.transform.parent);
        it2.name = "ImageTarget_JamCasio";

        var behaviour = it2.GetComponent<Vuforia.ImageTargetBehaviour>();
        var so = new SerializedObject(behaviour);
        so.FindProperty("mTrackableName").stringValue = SecondTargetName;
        so.FindProperty("mDataSetPath").stringValue = SecondDatasetPath;
        so.FindProperty("mWidth").floatValue = 10f;
        so.FindProperty("mHeight").floatValue = 12.5f;
        so.ApplyModifiedPropertiesWithoutUndo();

        var preview = it2.GetComponent("ImageTargetPreview");
        if (preview != null)
        {
            var pso = new SerializedObject(preview);
            var tn = pso.FindProperty("mTargetName");
            var dn = pso.FindProperty("mDatasetName");
            if (tn != null) tn.stringValue = SecondTargetName;
            if (dn != null) dn.stringValue = SecondDatasetPath;
            pso.ApplyModifiedPropertiesWithoutUndo();
        }

        foreach (Transform child in it2.transform)
            Object.DestroyImmediate(child.gameObject);

        var yellow = it2.GetComponent<PikachuYellow>();
        if (yellow != null) Object.DestroyImmediate(yellow);
        var rotOnTarget = it2.GetComponent<ObjectRotator>();
        if (rotOnTarget != null) Object.DestroyImmediate(rotOnTarget);

        it2.transform.localPosition = new Vector3(0f, 0f, 0f);
        it2.transform.localRotation = Quaternion.identity;
        it2.transform.localScale = Vector3.one;

        var content = new GameObject("Content_Jam");
        content.transform.SetParent(it2.transform, false);

        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(FbxPath);
        if (fbx == null)
        {
            Debug.LogError("[UTS] FBX not found at " + FbxPath);
        }
        else
        {
            var model = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
            model.name = "JamCasio";
            model.transform.SetParent(content.transform, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;

            FitModelToBounds(model, targetHeight: 5f);

            if (model.GetComponent<ObjectRotator>() == null)
                model.AddComponent<ObjectRotator>();
        }

        var label = CreateTextMesh(content.transform, "Casio Watch", 0.35f);
        label.name = "Label_Jam";
        label.transform.localPosition = new Vector3(0f, 3.5f, 0f);

        content.SetActive(false);

        var handler = it2.GetComponent<DefaultObserverEventHandler>();
        if (handler == null)
            handler = it2.AddComponent<DefaultObserverEventHandler>();
        handler.StatusFilter = DefaultObserverEventHandler.TrackingStatusFilter.Tracked;

        var vis = it2.GetComponent<ContentVisibility>();
        if (vis == null)
            vis = it2.AddComponent<ContentVisibility>();
        vis.content = content;

        WireEvents(handler, vis);
        Debug.Log("[UTS] Second ImageTarget added: " + SecondTargetName);
    }

    static void WireEvents(DefaultObserverEventHandler handler, ContentVisibility vis)
    {
        handler.OnTargetFound.RemoveAllListeners();
        handler.OnTargetLost.RemoveAllListeners();
        UnityEventTools.AddVoidPersistentListener(handler.OnTargetFound, vis.Show);
        UnityEventTools.AddVoidPersistentListener(handler.OnTargetLost, vis.Hide);
    }

    static void FitModelToBounds(GameObject model, float targetHeight)
    {
        var rends = model.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) return;

        var b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
            b.Encapsulate(rends[i].bounds);

        if (b.size.y < 0.0001f) return;

        float scale = targetHeight / b.size.y;
        var root = model.transform;
        root.localScale = root.localScale * scale;

        rends = model.GetComponentsInChildren<Renderer>();
        b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
            b.Encapsulate(rends[i].bounds);

        var parent = model.transform.parent;
        if (parent != null)
        {
            var worldCenter = b.center;
            var localCenter = parent.InverseTransformPoint(worldCenter);
            model.transform.localPosition -= localCenter;
            model.transform.localPosition = new Vector3(
                model.transform.localPosition.x,
                model.transform.localPosition.y + targetHeight * 0.5f,
                model.transform.localPosition.z);
        }
    }

    static TextMesh CreateTextMesh(Transform parent, string text, float characterSize)
    {
        var go = new GameObject("Text3D_" + text);
        go.transform.SetParent(parent, false);

        var tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 48;
        tm.characterSize = characterSize;
        tm.anchor = TextAnchor.LowerCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font != null)
        {
            tm.font = font;
            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = font.material;
        }

        return tm;
    }

    static void UpdateInfoPanel()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null) return;
        var panel = canvas.transform.Find("InfoPanel");
        if (panel == null) return;
        var t = panel.GetComponentInChildren<UnityEngine.UI.Text>();
        if (t == null) return;

        t.text =
            "Target 1 — Pikachu\n" +
            "Pokemon bertipe listrik. HP 60 | No. 025\n\n" +
            "Target 2 — Jam Casio\n" +
            "Jam tangan digital klasik dengan display LCD.\n\n" +
            "Objek 3D berputar otomatis saat target terdeteksi.\n" +
            "Arahkan kamera ke gambar target untuk melihat model.";
    }

    static void SaveScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
