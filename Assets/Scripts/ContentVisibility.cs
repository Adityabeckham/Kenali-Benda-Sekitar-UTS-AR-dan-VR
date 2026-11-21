using UnityEngine;

public class ContentVisibility : MonoBehaviour
{
    public GameObject content;

    public void Show()
    {
        if (content == null) return;
        content.SetActive(true);
        foreach (var r in content.GetComponentsInChildren<Renderer>(true))
            r.enabled = true;
    }

    public void Hide()
    {
        if (content == null) return;
        content.SetActive(false);
    }
}
