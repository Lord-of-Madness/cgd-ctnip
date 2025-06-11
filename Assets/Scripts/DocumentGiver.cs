using System.Collections.Generic;
using UnityEngine;

public class DocumentGiver : MonoBehaviour
{
    Document document;
    [SerializeField] List<Page> pages;
    [SerializeField] string documentName;
    [SerializeField] Document.DocumentType documentType;
    [SerializeField] bool UseJSON = false;
    [SerializeField] TextAsset DocumentJSON;
    AudioClip backpack;
    private void Start()
    {
        backpack = Resources.Load<AudioClip>("Sounds\\Journal\\backpack");
        if (UseJSON)
        {
            document = JsonUtility.FromJson<Document>(DocumentJSON.text);
        }
        else document = new(documentName, pages, documentType);
    }
    public void GiveDocument() {
        GameManager.APD.lastTypeAdded = document.type;
        GameManager.Instance.ActivePlayer.VoiceSource.PlayOneShot(backpack);
        if (document.type == Document.DocumentType.Documents) GameManager.APD.Documents.Add(document);
        else if (document.type == Document.DocumentType.Codex) GameManager.APD.Codex.Add(document);
        else if (document.type == Document.DocumentType.Inventory) GameManager.APD.Inventory.Add(document);
        else
        {
            Debug.LogWarning($"Document type {document.type} not recognized. Adding to Documents.");
            GameManager.APD.Documents.Add(document);
        }
    }
}
