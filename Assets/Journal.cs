using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    Image Backcover;
    PlayerCharacter currentCharacter;
    [SerializeField] DocumentUI documentUI;
    [SerializeField] Transform LabelBox;
    [SerializeField] Button JournalLabelPrefab;
    void Start()
    {
        Backcover = GameObject.Find("Backcover").GetComponent<Image>();
        GameObject.Find("BethPortrait").GetComponent<Button>().onClick.AddListener(() => { if(currentCharacter != PlayerCharacter.Beth) SwitchCharacter(PlayerCharacter.Beth); });
        GameObject.Find("ErikPortrait").GetComponent<Button>().onClick.AddListener(() => { if(currentCharacter != PlayerCharacter.Erik) SwitchCharacter(PlayerCharacter.Erik); });
        SwitchCharacter(PlayerCharacter.Beth);
        GameManager.Instance.inputActions.Player.Journal.performed += ctx => Show();
        GameManager.Instance.inputActions.Journal.Cancel.performed += ctx => Hide();
        GameManager.Instance.inputActions.Journal.JournalExit.performed += ctx => Hide();
        Hide();
    }
    public void Show()
    {
        //TODO pause game
        GameManager.Instance.inputActions.Player.Disable();
        GameManager.Instance.inputActions.Journal.Enable();
        gameObject.SetActive(true);
        HUD.Instance.Hide();
    }
    public void Hide()
    {
        GameManager.Instance.inputActions.Player.Enable();
        gameObject.SetActive(false);
        HUD.Instance.Show();
    }
    public void SwitchCharacter(PlayerCharacter character) {
        currentCharacter = character;
        Debug.Log("Switching Character");
    }
    public void OnNotesPressed()
    {
        OnBookmarkPressed();
        Debug.Log("Dokumenty");
        FillLabels(GameManager.APD.Documents);

    }
    public void OnInventoryPressed()
    {
        OnBookmarkPressed();
        Debug.Log("Inventráè");
        FillLabels(GameManager.APD.Inventory);
    }
    public void OnCodexPressed()
    {
        OnBookmarkPressed();
        Debug.Log("Ne nefixnu to typo nahoøe");
        FillLabels(GameManager.APD.Codex);
    }
    void OnBookmarkPressed()
    {
        Utilities.PurgeChildren(LabelBox);
        Debug.Log("Záložkavìc");
    }
    void FillLabels(List<Document> documents)
    {
        foreach (var document in documents)
        {
             Instantiate(JournalLabelPrefab, LabelBox).onClick.AddListener(() => documentUI.ShowDocument(document));
        }
    }
}
