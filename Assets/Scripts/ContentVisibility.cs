using UnityEngine;

public class ContentVisibility : MonoBehaviour
{
    public GameObject content;

    static Material sSafeMat;
    public void Show()
    {
        if (content == null) return;
        try
        {
            content.SetActive(true);
            foreach (var r in content.GetComponentsInChildren<Renderer>(true))
            {
                if (r == null) continue;
                r.enabled = true;
                var mat = r.sharedMaterial;
                if (mat == null || mat.shader == null || mat.shader.name.Contains("Standard"))
                {
                    if (sSafeMat == null)
                    {
                        var sh = Shader.Find("Universal Render Pipeline/Lit");
                        if (sh != null) sSafeMat = new Material(sh);
                    }
                    if (sSafeMat != null) r.sharedMaterial = sSafeMat;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[ContentVisibility] Show failed: " + e);
        }
    }

    public void Hide()
    {
        if (content == null) return;
        content.SetActive(false);
    }
}
