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
    [SerializeField] GameObject BethPortrait;
    [SerializeField] GameObject ErikPortrait;
    [SerializeField] GameObject JournalPaper;
    [SerializeField] List<AudioClip> PageRustleSound;
    bool SwapWasEnabled;
    void Start()
    {
        //Backcover = GameObject.Find("Backcover").GetComponent<Image>();
        BethPortrait.GetComponent<Button>().onClick.AddListener(() => { if(GameManager.Instance.activeChar != PlayerCharacter.Beth) SwitchCharacter(); });
        ErikPortrait.GetComponent<Button>().onClick.AddListener(() => { if(GameManager.Instance.activeChar != PlayerCharacter.Erik) SwitchCharacter(); });
        GameManager.Instance.inputActions.Player.Journal.performed += ctx => Show();
        GameManager.Instance.inputActions.Player.Controls.performed += ctx => Show("Controls");
        GameManager.Instance.inputActions.Journal.Cancel.performed += ctx => Hide();
        GameManager.Instance.inputActions.Journal.JournalExit.performed += ctx => Hide();
        Hide();
    }
    public void Show(string docname = "")
    {
		//Pause Game
		Time.timeScale = 0;


		GameManager.Instance.ActivePlayer.VoiceSource.PlayOneShot(PageRustleSound[Random.Range(0,PageRustleSound.Count)]);
        SwapWasEnabled = GameManager.Instance.inputActions.Player.SwapCharacters.enabled && GameManager.Instance.OtherPlayer!=null;
        if (!SwapWasEnabled )
        {
            if (GameManager.Instance.activeChar == PlayerCharacter.Beth)
            {
                ErikPortrait.SetActive(false);
            }
            else
            {
                BethPortrait.SetActive(false);
            }
        }
        else
        {
            BethPortrait.SetActive(true);
            ErikPortrait.SetActive(true);
        }
        SetCharLabelOrder();
        if(docname != "")
        {
            OnNotesPressed();
            foreach (Document document in GameManager.Instance.ActivePlayer.playerData.Documents)
                if(docname == document.name)
                {
                    documentUI.ShowDocument(document);
                    break;
                }
        }
        else
            switch (GameManager.APD.lastTypeAdded)
            {
                case Document.DocumentType.Inventory:
                    OnInventoryPressed();
                    break;
                case Document.DocumentType.Codex:
                    OnCodexPressed();
                    break;
                case Document.DocumentType.Documents:
                default:
                    OnNotesPressed();
                    break;
            }
        GameManager.Instance.inputActions.Player.Disable();
        GameManager.Instance.inputActions.Journal.Enable();
        gameObject.SetActive(true);
        HUD.Instance.Hide();
    }
    public void Hide()
    {
		//ÜnPause Game
		Time.timeScale = 1;
	
        GameManager.Instance.inputActions.Player.Enable();
        gameObject.SetActive(false);
        HUD.Instance.Show();
    }
    public void SwitchCharacter() {
        if (!SwapWasEnabled) return;
        GameManager.Instance.ActivePlayer.VoiceSource.PlayOneShot(PageRustleSound[Random.Range(0, PageRustleSound.Count)]);
        GameManager.Instance.SwapCharacters();
        SetCharLabelOrder();
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
        GameManager.Instance.ActivePlayer.VoiceSource.PlayOneShot(PageRustleSound[Random.Range(0, PageRustleSound.Count)]);
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
            b.onClick.AddListener(() => { GameManager.Instance.ActivePlayer.VoiceSource.PlayOneShot(PageRustleSound[Random.Range(0, PageRustleSound.Count)]); documentUI.ShowDocument(document); });
            b.GetComponentInChildren<TextMeshProUGUI>().text = document.name;
        }
    }
    void SetCharLabelOrder()
    {
        if (GameManager.Instance.activeChar == PlayerCharacter.Beth)
        {
            ErikPortrait.transform.SetParent(JournalPaper.transform.parent);
            ErikPortrait.transform.SetSiblingIndex(JournalPaper.transform.GetSiblingIndex()-1);
            BethPortrait.transform.SetParent(JournalPaper.transform);
        }
        else
        {
            BethPortrait.transform.SetParent(JournalPaper.transform.parent);
            BethPortrait.transform.SetSiblingIndex(JournalPaper.transform.GetSiblingIndex()-1);
            ErikPortrait.transform.SetParent(JournalPaper.transform);
        }
    }
}
