using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Linq;
using System.IO;
using TMPro;
using UnityEngine.Events;


public class DialogueTreeNode
{
    public List<DialogueTreeNode> Children { get; private set; } = new();
    public DialogueLine Line { get; private set; }
    public bool IsLeaf => Children.Count == 0;
    public bool IsChoice => Children.Count > 1;
    public Action callback;
    private const int FUNNY_NUMBER = 69420; //Haha funny. Once you stop being utterly hilarious you can replace it with -1 or something.
    private int SerializationID = FUNNY_NUMBER;
    public DialogueTreeNode(DialogueLine line)
    {
        Line = line;
        if(line.Document!=null)
        {
            callback += () => {
                if (line.Document.type == Document.DocumentType.Documents) GameManager.APD.Documents.Add(line.Document);
                else if (line.Document.type == Document.DocumentType.Codex) GameManager.APD.Codex.Add(line.Document);
                else if (line.Document.type == Document.DocumentType.Inventory) GameManager.APD.Inventory.Add(line.Document);
                else
                {
                    Debug.LogWarning($"Document type {line.Document.type} not recognized. Adding to Documents.");
                    GameManager.APD.Documents.Add(line.Document);
                }
                
            };
        }
        if (line.voiceline != null)
        {
            callback += () => GameManager.Instance.ActivePlayer.VoiceSource.PlayOneShot(line.voiceline);
        }
    }

    internal void AddChild(DialogueTreeNode dialogueTreeNode)
    {
        Children.Add(dialogueTreeNode);
    }
    public static DialogueTreeNode BuildSimpleTree(List<DialogueLine> lines)
    {
        DialogueTreeNode root = new(lines[0]);
        DialogueTreeNode workingNode = root;
        for (int i = 1; i < lines.Count; i++)
        {
            DialogueTreeNode dialogueTreeNode = new(lines[i]);
            workingNode.AddChild(dialogueTreeNode);
            workingNode = dialogueTreeNode;
        }
        return root;
    }
    public DialogueTreeNode GetFirstLeaf()
    {
        if (IsLeaf) return this;
        else
        {
            foreach (var child in Children)
            {
                if (child.IsLeaf) return child;
                else return child.GetFirstLeaf();
            }
        }
        return null;
    }
    public static Dictionary<string,string> SpeakerHexPairToDict(List<SpeakerHexPair> speakers)
    {
        Dictionary<string, string> dict = new();
        foreach (var speaker in speakers)
        {
            dict.Add(speaker.Speaker, speaker.Hex);
        }
        return dict;
    }
    public static DialogueTreeNode DeserializeTree(TextAsset json)
    {
        return DesTree(json.text);
    }
    public static DialogueTreeNode DeserializeTree(string JSONPATH )
    {
        string json;
        try
        {
            json = File.ReadAllText(JSONPATH);
        }
        catch (Exception e)
        {
            Debug.LogError("Deb�lku, �patn� jsi tam ten JSON dal. �u�aj takovou ekcep�nu to hodilo: "+e);
            return null;
        }
        return DesTree(json);
    }
    static DialogueTreeNode DesTree(string json)
    {
        DialogueWrapper dialogueData = JsonUtility.FromJson<DialogueWrapper>(json);
        List<DialogueNodeJSON> list = dialogueData.nodes;
        Dictionary<string, string> speakers = SpeakerHexPairToDict(dialogueData.speakers);
        Dictionary<int, DialogueTreeNode> nodeDict = new();
        foreach (var node in list)
        {
            string hex = speakers.ContainsKey(node.Who) ? speakers[node.Who] : GameManager.SpeakerGlobalData.ContainsKey(node.Who) ? GameManager.SpeakerGlobalData[node.Who] : Color.gray.ToHexString();
            string imagePath = (node.Who!="")? $"CharacterPortraits/{node.Who}": "CharacterPortraits/blank";
            string documentPath = $"Documents/{node.DocumentName}";
            string voicePath = $"Voiceover/Dialogues/{node.DocumentName}";
            nodeDict.Add(node.id, new(new(node.Text, node.Who, Resources.Load<Sprite>(imagePath), hex,Resources.Load<TextAsset>(documentPath), Resources.Load<AudioClip>(voicePath))));
        }
        DialogueTreeNode root = nodeDict.ContainsKey(0) ? nodeDict[0] : null;
        if (root == null) Debug.LogError($"No root node found in json");
        foreach (var node in list)
        {
            DialogueTreeNode currentNode = nodeDict[node.id];
            foreach (var childId in node.children)
            {
                if (nodeDict.ContainsKey(childId))
                {
                    currentNode.AddChild(nodeDict[childId]);
                }
                else
                {
                    Debug.LogError($"Child node {childId} not found for parent {node.id}");
                }
            }

        }
        return root;
    }

    int SerializeNodeRecursion(Dictionary<int,DialogueNodeJSON> nodeList)
    {
        if (SerializationID != FUNNY_NUMBER) return SerializationID;
        DialogueNodeJSON nodeJSON = new()
        {
            id = nodeList.Count,
            Text = Line.Text,
            Who = Line.Who,
            DocumentName = Line.Document?.name,
            children = new List<int>()
        };
        SerializationID = nodeJSON.id;
        nodeList.Add(nodeJSON.id, nodeJSON);
        foreach (var child in Children)
        {
            nodeJSON.children.Add(child.SerializeNodeRecursion(nodeList));
        }

        return SerializationID;
    }
    public void SerializeTree(string path,List<SpeakerHexPair> speakerHexes)
    {
        Dictionary<int, DialogueNodeJSON> nodeList = new();
        SerializeNodeRecursion(nodeList);
        //WARNING/TODO: the entire tree is dirty after this and cannot be serialized again - Need to set the SerializationID back to FUNNY_NUMBER
        string json = JsonUtility.ToJson(new DialogueWrapper(nodeList.Values.ToList(), speakerHexes));
        File.WriteAllText(path, json);
    }
}

[System.Serializable]
public class SpeakerHexPair
{
    public string Speaker;
    public string Hex;
    public SpeakerHexPair(string speaker, string hex)
    {
        Speaker = speaker;
        Hex = hex;
    }
}
[System.Serializable]
public class DialogueWrapper
{
    public List<DialogueNodeJSON> nodes;
    public List<SpeakerHexPair> speakers;
    public DialogueWrapper(List<DialogueNodeJSON> nodes, List<SpeakerHexPair> speakers)
    {
        this.nodes = nodes;
        this.speakers = speakers;
    }
}
[System.Serializable]
public class SpeakerGlobalSettings
{
    public List<SpeakerHexPair> speakers;
    public SpeakerGlobalSettings(List<DialogueNodeJSON> nodes, List<SpeakerHexPair> speakers)
    {
        this.speakers = speakers;
    }
}
[System.Serializable]
public class DialogueNodeJSON
{
    public int id;
    public string Text;
    public string Who;
    /// <summary>
    /// If the dialogue node is supposed to give a document the path will be here. Will be loaded as such: Resources.Load<TextAsset>(Documents/DocumentName) to load it.
    /// </summary>
    public string DocumentName;
    public List<int> children;
}

public class DialogueLine
{
    public string Text { get; set; }
    public string Who { get; set; }
    public Sprite Sprite { get; set; }
    public string Hex { get; set; }
    public Document Document { get; set; }
    public AudioClip voiceline { get; set; }
    public DialogueLine(string text, string who, Sprite sprite, string colorHex, TextAsset document,AudioClip audioClip)// TODO
    {
        Text = text;
        Who = who;
        Sprite = sprite;
        Hex = colorHex;
        Document = document != null ? JsonUtility.FromJson<Document>(document.text ): null;
        voiceline = audioClip;
    }
    public DialogueLine(string text, string who, Sprite sprite, Color color)
    {
        Text = text;
        Who = who;
        Sprite = sprite;
        Hex = color.ToHexString();
    }
    public DialogueLine(string text, Speaker speaker)
    {
        Text = text;
        Who = speaker.Name;
        Sprite = speaker.Portrait;
        Hex = speaker.TextColor.ToHexString();
    }
    public string SpeakerAnnotation()
    {
        if(Hex.Length==8 && Hex[6]=='0'&& Hex[7] == '0') return "";
        return $"<color=#{Hex}>{Who}: </color>";
    }
}
public class Speaker
{
    public string Name { get; set; }
    public Sprite Portrait { get; set; }
    public Color TextColor { get; set; } = Color.gray;
    public Speaker(string name)
    {
        Name = name;
        Portrait = Resources.Load<Sprite>($"CharacterPortraits/{name}");
    }
    public static Speaker Beth => new("ELISABETH") {TextColor = new(1f, 0.513725f, 0.513725f) };
    public static Speaker Erik => new("ERIK") { TextColor = new(0.5294f, 6.941f, 1f) };
}

public class Dialogue : MonoBehaviour
{
    [SerializeField] Image CharacterImage;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextMeshProUGUI dialogTextLinePrefab;
    [SerializeField] Button dialogOptButtonPrefab;
    [SerializeField] VerticalLayoutGroup dialogOptBoxPrefab;
    //Tween textween;

    DialogueTreeNode lines;
    //[SerializeField] float textSpeed = 0.3f;
    public static Dialogue Instance { get; private set; }
    public UnityEvent dialogueEnded;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.inputActions.Dialogue.Skip.performed += ctx => NextLine();
        //GameManager.Instance.inputActions.Dialogue.Cancel.performed += ctx => Hide();//Perhaps remove - callbacks would get called multiple times.
        gameObject.SetActive(false);
	}

	void ShowCharacterWithText(DialogueLine line)
    {        
        CharacterImage.sprite = line.Sprite;
        if (line.Sprite==null)Debug.LogWarning($"Speaker: {line.Who} has no SpritePortrait");
        TextMeshProUGUI dialogTextLine = Instantiate(dialogTextLinePrefab, dialogueBox.transform);
        dialogTextLine.text = line.SpeakerAnnotation() + line.Text;
    }
    public void ShowCharacterWithText(DialogueTreeNode lines)
    {
        Show();
        Utilities.PurgeChildren(dialogueBox.transform);
        this.lines = lines;
        NextLine();
    }
    private void NextLine()
    {
        if (!lines.IsLeaf)
        {
            if (lines.IsChoice)
            {
                GameManager.Instance.inputActions.Dialogue.Skip.Disable();
                ShowCharacterWithText(lines.Line);
                if (lines.callback != null) Debug.Log("Callback is not null");
                lines.callback?.Invoke();
                List<Button> buttons = new();
                VerticalLayoutGroup optbox = Instantiate(dialogOptBoxPrefab, dialogueBox.transform);
                for (int i = 0; i < lines.Children.Count; i++)
                {
                    DialogueTreeNode child = lines.Children[i];
                    Button button = Instantiate(dialogOptButtonPrefab, optbox.transform);
                    buttons.Add(button);
                    button.GetComponent<TextMeshProUGUI>().text = (1+i).ToString()+ " � " + child.Line.Text;
                    button.onClick.AddListener(() => {
                        buttons.ForEach((c) => Destroy(c.gameObject));
                        child.callback?.Invoke();
                        lines = child;
                        GameManager.Instance.inputActions.Dialogue.Skip.Enable();
                        NextLine();
                    });
                }
            }
            else {
                ShowCharacterWithText(lines.Line);
                lines.callback?.Invoke();
                lines = lines.Children.First();
            }
        }
        else
        {
            Hide();
            dialogueEnded?.Invoke();
        }
    }
    void Show(){
        gameObject.SetActive(true);
        GameManager.Instance.inputActions.Player.Disable();
        GameManager.Instance.inputActions.Dialogue.Enable();
    }
    void Hide()
    {
        lines=null;
        GameManager.Instance.inputActions.Player.Enable();
        GameManager.Instance.inputActions.Dialogue.Disable();
        gameObject.SetActive(false);
    }
}
