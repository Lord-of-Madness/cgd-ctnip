using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    //Image Backcover;
    [SerializeField] DocumentUI documentUI;
    [SerializeField] Transform LabelBox;
    [SerializeField] Button JournalLabelPrefab;
    bool SwapWasEnabled;
    void Start()
    {
        //Backcover = GameObject.Find("Backcover").GetComponent<Image>();
        GameObject.Find("BethPortrait").GetComponent<Button>().onClick.AddListener(() => { if(GameManager.Instance.activeChar != PlayerCharacter.Beth) SwitchCharacter(); });
        GameObject.Find("ErikPortrait").GetComponent<Button>().onClick.AddListener(() => { if(GameManager.Instance.activeChar != PlayerCharacter.Erik) SwitchCharacter(); });
        GameManager.Instance.inputActions.Player.Journal.performed += ctx => Show();
        GameManager.Instance.inputActions.Journal.Cancel.performed += ctx => Hide();
        GameManager.Instance.inputActions.Journal.JournalExit.performed += ctx => Hide();
        Hide();
    }
    public void Show()
    {
        //TODO pause game
        SwapWasEnabled = GameManager.Instance.inputActions.Player.SwapCharacters.enabled;
        GameManager.Instance.inputActions.Player.Disable();
        GameManager.Instance.inputActions.Journal.Enable();
        Debug.Log(GameManager.Instance.inputActions.Journal.enabled);
        gameObject.SetActive(true);
        HUD.Instance.Hide();
    }
    public void Hide()
    {
        GameManager.Instance.inputActions.Player.Enable();
        gameObject.SetActive(false);
        HUD.Instance.Show();
    }
    public void SwitchCharacter() {
        if (!SwapWasEnabled) return;
        GameManager.Instance.SwapCharacters();
        OnNotesPressed();
        //Debug.Log("Switching Character");
    }
    public void OnNotesPressed()
    {
        OnBookmarkPressed();
        //Debug.Log("Dokumenty");
        FillLabels(GameManager.APD.Documents);

    }
    public void OnInventoryPressed()
    {
        OnBookmarkPressed();
        //Debug.Log("Inventráè");
        FillLabels(GameManager.APD.Inventory);
    }
    public void OnCodexPressed()
    {
        OnBookmarkPressed();
        //Debug.Log("Ne nefixnu to typo nahoøe");
        FillLabels(GameManager.APD.Codex);
    }
    void OnBookmarkPressed()
    {
        Utilities.PurgeChildren(LabelBox);
        documentUI.CloseDocument();/*
        if(GameManager.Instance.activeChar == PlayerCharacter.Beth)
        {
            Backcover.color = Speaker.Beth.TextColor;
        }
        else
        {
            Backcover.color = Speaker.Erik.TextColor;
        }*/
        //Debug.Log("Záložkavìc");
    }
    void FillLabels(List<Document> documents)
    {
        foreach (var document in documents)
        {
            Button b = Instantiate(JournalLabelPrefab, LabelBox);
            b.onClick.AddListener(() => documentUI.ShowDocument(document));
            b.GetComponentInChildren<TextMeshProUGUI>().text = document.name;
        }
    }
}
