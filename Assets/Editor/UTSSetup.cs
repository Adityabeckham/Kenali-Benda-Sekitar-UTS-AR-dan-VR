using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class UTSSetup
{
    const string OldScenePath = "Assets/Scenes/SampleScene.unity";
    const string NewScenePath = "Assets/Scenes/MainAR.unity";

    [MenuItem("UTS/1. Setup MainAR Scene")]
    public static void Setup()
    {
        var scene = EditorSceneManager.OpenScene(OldScenePath, OpenSceneMode.Single);

        var imageTarget = GameObject.Find("ImageTarget");
        if (imageTarget == null)
        {
            Debug.LogError("[UTS] ImageTarget not found");
            return;
        }

        var pikachu = GameObject.Find("pikachu");
        if (pikachu == null)
        {
            Debug.LogError("[UTS] pikachu not found");
            return;
        }

        if (pikachu.GetComponent<ObjectRotator>() == null)
            pikachu.AddComponent<ObjectRotator>();

        CreateNameLabel(imageTarget.transform, pikachu.transform, "Pikachu");
        CreateUI();

        EditorSceneManager.SaveScene(scene, NewScenePath);
        if (File.Exists(OldScenePath))
        {
            AssetDatabase.DeleteAsset(OldScenePath);
        }

        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(NewScenePath, true) };

        ConfigurePlayerSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[UTS] Setup complete: MainAR scene ready");
    }

    [MenuItem("UTS/2. Add Second Image Target (run after Vuforia import)")]
    public static void AddSecondTarget()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (!scene.path.EndsWith("MainAR.unity"))
        {
            Debug.LogError("[UTS] Open MainAR scene first (run menu 1)");
            return;
        }

        var databaseXmls = Directory.GetFiles("Assets/StreamingAssets/Vuforia", "*.xml");
        if (databaseXmls.Length == 0)
        {
            Debug.LogError("[UTS] No dataset in StreamingAssets/Vuforia");
            return;
        }

        var firstTarget = GameObject.Find("ImageTarget");
        if (firstTarget == null)
        {
            Debug.LogError("[UTS] ImageTarget not found");
            return;
        }

        var existingNames = Object.FindObjectsByType<Vuforia.ImageTargetBehaviour>()
            .Select(t => t.TargetName)
            .ToList();

        string secondTargetName = null;
        string secondDatasetPath = null;

        foreach (var xmlPath in databaseXmls)
        {
            var text = File.ReadAllText(xmlPath);
            var matches = System.Text.RegularExpressions.Regex.Matches(text, "name=\"([^\"]+)\"");
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                var n = m.Groups[1].Value;
                if (!existingNames.Contains(n))
                {
                    secondTargetName = n;
                    secondDatasetPath = "Vuforia/" + Path.GetFileName(xmlPath);
                    break;
                }
            }
            if (secondTargetName != null) break;
        }

        if (secondTargetName == null)
        {
            Debug.LogWarning("[UTS] No unused target found in dataset. Upload the jam image to Vuforia Target Manager, download dataset, replace files in Assets/StreamingAssets/Vuforia, then run this menu again.");
            return;
        }

        var it2 = Object.Instantiate(firstTarget, firstTarget.transform.parent);
        it2.name = "ImageTarget_" + secondTargetName;

        var behaviour = it2.GetComponent<Vuforia.ImageTargetBehaviour>();
        var so = new SerializedObject(behaviour);
        so.FindProperty("mTrackableName").stringValue = secondTargetName;
        so.FindProperty("mDataSetPath").stringValue = secondDatasetPath;
        so.ApplyModifiedPropertiesWithoutUndo();

        var preview = it2.GetComponent("ImageTargetPreview");
        if (preview != null)
        {
            var pso = new SerializedObject(preview);
            var tn = pso.FindProperty("mTargetName");
            var dn = pso.FindProperty("mDatasetName");
            if (tn != null) tn.stringValue = secondTargetName;
            if (dn != null) dn.stringValue = secondDatasetPath;
            pso.ApplyModifiedPropertiesWithoutUndo();
        }

        foreach (Transform child in it2.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        var rot = it2.GetComponent<ObjectRotator>();
        if (rot != null) Object.DestroyImmediate(rot);

        var holder = new GameObject("Content_" + secondTargetName);
        holder.transform.SetParent(it2.transform, false);
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;
        holder.transform.localScale = Vector3.one;

        var label = CreateTextMesh(holder.transform, "Jam Casio", 0.04f);
        label.transform.localPosition = new Vector3(0f, 0.15f, 0f);

        CreateInfoPanelForSecondTarget();

        EditorSceneManager.MarkSceneDirty(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("[UTS] Second ImageTarget added: " + secondTargetName + " (" + secondDatasetPath + ")");
    }

    static void CreateNameLabel(Transform imageTarget, Transform model, string name)
    {
        if (imageTarget.Find("Label_" + name) != null) return;

        var rends = model.GetComponentsInChildren<Renderer>();
        float topY = 0.1f;
        if (rends.Length > 0)
        {
            var b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
            topY = imageTarget.InverseTransformPoint(new Vector3(b.center.x, b.max.y, b.center.z)).y;
        }

        var label = CreateTextMesh(imageTarget, name, 0.05f);
        label.name = "Label_" + name;
        label.transform.localPosition = new Vector3(0f, topY + 0.08f, 0f);
        label.transform.localRotation = Quaternion.identity;
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

    static void CreateUI()
    {
        if (GameObject.Find("Canvas") != null) return;

        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        var infoPanel = CreatePanel(canvasGo.transform, "InfoPanel");
        var infoText = CreateText(infoPanel.transform, "InfoText",
            "Pikachu adalah Pokemon bertipe listrik berbentuk tikus.\n" +
            "HP 60 | No. 025 | Mouse Pokemon\n\n" +
            "Objek 3D berputar otomatis saat target terdeteksi.\n" +
            "Arahkan kamera ke gambar target untuk melihat model.",
            32, TextAnchor.UpperLeft);
        Stretch(infoText.rectTransform, 40f, 40f, 40f, 40f);

        var infoBtn = CreateButton(canvasGo.transform, "BtnInfo", "Tampilkan Info", font);
        var quitBtn = CreateButton(canvasGo.transform, "BtnQuit", "Keluar", font);

        AnchorTopRight(quitBtn.transform as RectTransform, new Vector2(-20f, -20f), new Vector2(320f, 100f));
        AnchorTopLeft(infoBtn.transform as RectTransform, new Vector2(20f, -20f), new Vector2(480f, 100f));

        var ui = canvasGo.AddComponent<UIController>();
        ui.infoPanel = infoPanel.gameObject;
        ui.infoButton = infoBtn;
        ui.quitButton = quitBtn;
        ui.infoButtonLabel = infoBtn.GetComponentInChildren<Text>();

        if (infoPanel.gameObject.activeSelf == false)
            infoPanel.gameObject.SetActive(true);
        if (ui.infoButtonLabel != null)
            ui.infoButtonLabel.text = "Sembunyikan Info";

         if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }
    }

    static void CreateInfoPanelForSecondTarget()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null) return;
        var panel = canvas.transform.Find("InfoPanel");
        if (panel == null) return;
        var t = panel.GetComponentInChildren<Text>();
        if (t != null && !t.text.Contains("Casio"))
        {
            t.text += "\n\nTarget 2: Jam Casio\nJam tangan digital klasik dengan display LCD.";
        }
    }

    static Image CreatePanel(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        var img = go.GetComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.72f);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, -80f);
        rt.sizeDelta = new Vector2(920f, 640f);

        return img;
    }

    static Text CreateText(Transform parent, string name, string content, int fontSize, TextAnchor anchor)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        var t = go.GetComponent<Text>();
        t.text = content;
        t.fontSize = fontSize;
        t.alignment = anchor;
        t.color = Color.white;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (t.font == null)
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        return t;
    }

    static Button CreateButton(Transform parent, string name, string label, Font font)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var img = go.GetComponent<Image>();
        img.color = new Color(0.12f, 0.45f, 0.9f, 0.92f);

        var btn = go.GetComponent<Button>();

        var textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(go.transform, false);
        Stretch(textGo.GetComponent<RectTransform>(), 8f, 8f, 8f, 8f);

        var t = textGo.GetComponent<Text>();
        t.text = label;
        t.fontSize = 34;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.font = font;
        if (t.font == null)
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return btn;
    }

    static void Stretch(RectTransform rt, float left, float right, float top, float bottom)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    static void AnchorTopRight(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static void AnchorTopLeft(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static void ConfigurePlayerSettings()
    {
        PlayerSettings.companyName = "UTB";
        PlayerSettings.productName = "AR_ADITYA BECKHAM_24552011066";

        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.utb.arnama.nim");
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.iOS), "com.utb.arnama.nim");

        PlayerSettings.iOS.cameraUsageDescription = "Kamera diperlukan untuk fitur Augmented Reality mendeteksi image target.";
        PlayerSettings.iOS.microphoneUsageDescription = "";

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.Android.forceSDCardPermission = false;
        PlayerSettings.Android.forceInternetPermission = false;

        Debug.Log("[UTS] Player settings configured (AR_ADITYA BECKHAM_24552011066)");
    }
}
