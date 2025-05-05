using TMPro;
using UnityEngine;

public class DocumentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Page;
    [SerializeField] private TextMeshProUGUI PageNumber;
    [SerializeField] private TextMeshProUGUI Title;

    void Start()
    {
        
    }

    public void ShowDocument(Document document)
    {
        Page.text = document.pages[0].text;
        PageNumber.text = document.pages.Count.ToString();
        Title.text = document.name;
    }
}
