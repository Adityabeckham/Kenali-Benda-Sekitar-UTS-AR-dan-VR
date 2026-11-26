using UnityEngine;

public class ContentVisibility : MonoBehaviour
{
    public GameObject content;

    public void Show()
    {
        if (content == null) return;
        try
        {
            content.SetActive(true);
            foreach (var r in content.GetComponentsInChildren<Renderer>(true))
            {
                if (r != null) r.enabled = true;
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
