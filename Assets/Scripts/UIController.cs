using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject infoPanel;
    public Button infoButton;
    public Button quitButton;
    public Text infoButtonLabel;

    void Start()
    {
        if (infoButton != null)
            infoButton.onClick.AddListener(ToggleInfo);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitApp);
    }

    public void ToggleInfo()
    {
        if (infoPanel == null) return;

        bool show = !infoPanel.activeSelf;
        infoPanel.SetActive(show);

        if (infoButtonLabel != null)
            infoButtonLabel.text = show ? "Sembunyikan Info" : "Tampilkan Info";
    }

    public void QuitApp()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
