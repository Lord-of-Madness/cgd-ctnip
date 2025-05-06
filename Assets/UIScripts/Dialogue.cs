using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using System;
using NUnit.Framework.Interfaces;
using System.Linq;


public class DialogueTreeNode
{
    public List<DialogueTreeNode> children = new();
    public DialogueLine Line { get; set; }
    public bool IsLeaf => children.Count == 0;
    public bool IsChoice => children.Count > 1;
    public Action callback;
    private const int FUNNY_NUMBER = 69420; //Haha funny. Once you stop being utterly hilarious you can replace it with -1 or something.
    private int SerializationID = FUNNY_NUMBER;
    public DialogueTreeNode(DialogueLine line)
    {
        Line = line;
    }

    internal void AddChild(DialogueTreeNode dialogueTreeNode)
    {
        children.Add(dialogueTreeNode);
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
            foreach (var child in children)
            {
                if (child.IsLeaf) return child;
                else return child.GetFirstLeaf();
            }
        }
        return null;
    }
    public DialogueTreeNode DeserializeTree(string JSONPATH )
    {
        List<DialogueNodeJSON> list = JsonUtility.FromJson<DialogueWrapper>(Resources.Load<TextAsset>(JSONPATH).text).nodes;
        Dictionary<int, DialogueTreeNode> nodeDict = new();
        foreach (var node in list)
        {
            nodeDict.Add(node.id, new(new(node.Text,node.Who,Resources.Load<Sprite>($"CharacterPortraits/{node.Who}"),node.Hex) ));
        }
        DialogueTreeNode root = nodeDict.ContainsKey(0) ? nodeDict[0] : null;
        if(root == null) Debug.LogError($"No root node found in {JSONPATH}");
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
            Hex = Line.Hex,
            children = new List<int>()
        };
        SerializationID = nodeJSON.id;
        nodeList.Add(nodeJSON.id, nodeJSON);
        foreach (var child in children)
        {
            nodeJSON.children.Add(child.SerializeNodeRecursion(nodeList));
        }

        return SerializationID;
    }
    public void SerializeTree(string path)
    {
        Dictionary<int, DialogueNodeJSON> nodeList = new();
        SerializeNodeRecursion(nodeList);
        //WARNING/TODO: the entire tree is dirty after this and cannot be serialized again - Need to set the SerializationID back to FUNNY_NUMBER
        Debug.Log(nodeList.Count);
        string json = JsonUtility.ToJson(new DialogueWrapper(nodeList.Values.ToList()));
        Debug.Log(json);
        System.IO.File.WriteAllText(path, json);
    }
}
[System.Serializable]
public class DialogueWrapper
{
    public List<DialogueNodeJSON> nodes;
    public DialogueWrapper(List<DialogueNodeJSON> nodes)
    {
        this.nodes = nodes;
    }
}
[System.Serializable]
public class DialogueNodeJSON
{
    public int id;
    public string Text;
    public string Who;
    //public string SpritePath { get; set; } //Lets just use Who and a concrete folder in Resources.
    public string Hex;
    public List<int> children;
}

public class DialogueLine
{
    public string Text { get; set; }
    public string Who { get; set; }
    public Sprite Sprite { get; set; }
    public string Hex { get; set; }
    public DialogueLine(string text, string who, Sprite sprite, string colorHex)
    {
        Text = text;
        Who = who;
        Sprite = sprite;
        Hex = colorHex;
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
        return $"<color=#{Hex}>{Who}: </color>";
    }
}
public class Speaker
{
    public string Name { get; set; }
    public Sprite Portrait { get; set; }
    public Color TextColor { get; set; } = Color.gray;
    public Speaker(string name, Sprite portrait)
    {
        Name = name;
        Portrait = portrait;
    }
    public static Speaker Beth => new("Elisabeth", Resources.Load<Sprite>("Placeholders/fbFoto")) {TextColor = new(1f, 0.513725f, 0.513725f) };
    public static Speaker Erik => new("Erik", Resources.Load<Sprite>("Placeholders/Portrait mistodrzici")) { TextColor = new(0.5294f, 6.941f, 1f) };//TODO add portraits
}

public class Dialogue : MonoBehaviour
{
    [SerializeField]Image CharacterImage;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] Text dialogTextLinePrefab;
    [SerializeField] Button dialogOptButtonPrefab;
    Tween textween;

    DialogueTreeNode lines;
    [SerializeField] float textSpeed = 0.3f;
    public static Dialogue Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.inputActions.Dialogue.Skip.performed += ctx => SkipText();
        GameManager.Instance.inputActions.Dialogue.Cancel.performed += ctx => Hide();
        gameObject.SetActive(false);
	}

	void ShowCharacterWithText(DialogueLine line)
    {        
        CharacterImage.sprite = line.Sprite;
        if (line.Sprite==null)Debug.LogWarning($"Speaker: {line.Who} has no SpritePortrait");
        if (textween != null) FinishTween();
        Text dialogTextLine = Instantiate(dialogTextLinePrefab, dialogueBox.transform);
        dialogTextLine.text = "";//If we clean up the prefab this can be removed.
        textween = dialogTextLine.DOText(line.SpeakerAnnotation()+line.Text, textSpeed, true, ScrambleMode.None).OnComplete(() => FinishTween());
    }
    public void ShowCharacterWithText(DialogueTreeNode lines)
    {
        Show();
        Utilities.PurgeChildren(dialogueBox.transform);
        this.lines = lines;
        NextLine();
    }
    private void SkipText()
    {
        if (textween != null) FinishTween();
        else NextLine();
    }
    private void NextLine()
    {
        if (!lines.IsLeaf)
        {
            if (lines.IsChoice)
            {
                Debug.Log("TODOChoice");//TODO
            }
            else {
                ShowCharacterWithText(lines.Line);
                lines.callback?.Invoke();
                lines = lines.children[0];
            }
        }
        else
        {
            Hide();
        }
    }
    void FinishTween()
    {
        textween.Kill(true);
        textween = null;
    }
    void Show(){
        gameObject.SetActive(true);
        GameManager.Instance.inputActions.Player.Disable();
        GameManager.Instance.inputActions.Dialogue.Enable();
    }
    void Hide()
    {
        FinishTween();
        lines=null;
        GameManager.Instance.inputActions.Player.Enable();
        GameManager.Instance.inputActions.Dialogue.Disable();
        gameObject.SetActive(false);
    }
}
