using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class DocumentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Page;
    [SerializeField] private TextMeshProUGUI PageNumber;
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private Button NextPageButton;
    [SerializeField] private Button PreviousPageButton;
    [SerializeField] private Button CloseButton;
    int cp;
    int CurrentPage { get => cp; set { cp = math.clamp(value,0, document.pages.Count-1); PageNumber.text = $"{cp+1}/{document.pages.Count}"; } }
    Document document;

    private void Start()
    {
        foreach (Button button in new List<Button>() { NextPageButton, PreviousPageButton, CloseButton })
        {
            button.enabled = false;
        }
    }
    public void ShowDocument(Document document)
    {
        this.document = document;
        CurrentPage = 0;
        Page.text = document.pages[0].text;
        Title.text = document.name;
        foreach (Button button in new List<Button>(){NextPageButton,PreviousPageButton,CloseButton })
        {
            button.enabled = true;
        }
    }
    public void NextPage()
    {
        CurrentPage++;
        Page.text = document.pages[CurrentPage].text;
    }
    public void PreviousPage()
    {
        CurrentPage--;
        Page.text = document.pages[CurrentPage].text;
    }
    public void CloseDocument()
    {
        PageNumber.text = "";
        Page.text = "";
        Title.text = "";
        foreach (Button button in new List<Button>() { NextPageButton, PreviousPageButton, CloseButton })
        {
            button.enabled = false;
        }
    }
}
