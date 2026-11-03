using UnityEngine;

public class PikachuYellow : MonoBehaviour
{
    public Color yellow = new Color(1f, 0.92f, 0.02f, 1f);

    void Start()
    {
        foreach (var r in GetComponentsInChildren<Renderer>(true))
        {
            foreach (var mat in r.materials)
            {
                mat.mainTexture = null;
                mat.color = yellow;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", yellow);
                if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", null);
                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", yellow * 0.25f);
                }
            }
        }
    }
}
