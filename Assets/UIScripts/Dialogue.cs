using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using System;


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
    [SerializeField] GameObject dialogTextLinePrefab;
    //[SerializeField] Text SpeakerName;
    Tween textween;

    Queue<DialogueLine> lines = new();
    [SerializeField] float textSpeed = 0.3f;
    public static Dialogue Instance { get; private set; }
    Action callback = null;
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
        Text dialogTextLine = Instantiate(dialogTextLinePrefab, dialogueBox.transform).GetComponent<Text>();
        dialogTextLine.text = "";//If we clean up the prefab this can be removed.
        textween = dialogTextLine.DOText(line.SpeakerAnnotation()+line.Text, textSpeed, true, ScrambleMode.None).OnComplete(() => FinishTween());
    }
    public void ShowCharacterWithText(List<DialogueLine> lines,Action callback=null)
    {
        Show();
        Utilities.PurgeChildren(dialogueBox.transform);
        this.lines = new(lines);
        this.callback = callback;
        NextLine();
    }
    private void SkipText()
    {
        if (textween != null) FinishTween();
        else NextLine();
    }
    private void NextLine()
    {
        if (lines.Count > 0)
        {
            ShowCharacterWithText(lines.Dequeue());
        }
        else {
            callback?.Invoke();
            Hide(); }
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
        lines.Clear();
        GameManager.Instance.inputActions.Player.Enable();
        GameManager.Instance.inputActions.Dialogue.Disable();
        gameObject.SetActive(false);
    }
}
