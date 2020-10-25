using UnityEngine;
using TMPro;

public class LoadTxt : MonoBehaviour
{
    [SerializeField] string m_txtFile = "credits";
    string txtContents;
    // Start is called before the first frame update
    void Start()
    {
        TextAsset txtAssets = (TextAsset)Resources.Load(m_txtFile);
        txtContents = txtAssets.text;

        TextMeshProUGUI textmeshPro = GetComponent<TextMeshProUGUI>();
        textmeshPro.SetText(txtContents);
    }
}
