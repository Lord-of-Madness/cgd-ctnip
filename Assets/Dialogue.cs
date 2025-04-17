using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;


public class DialogueLine
{
    public string Text { get; set; }
    public string Who { get; set; }
    public Sprite Sprite { get; set; }
    public DialogueLine(string text, string who, Sprite sprite)
    {
        Text = text;
        Who = who;
        Sprite = sprite;
    }
}

public class Dialogue : MonoBehaviour
{
    [SerializeField]Image CharacterImage;
    [SerializeField]Text dialogueText;
    [SerializeField] Text SpeakerName;
    Tween textween;

    Queue<DialogueLine> lines = new();
    [SerializeField] float textSpeed = 0.5f;
    public static Dialogue Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
		InputSystem.actions["Skip"].performed += ctx => SkipText();
		InputSystem.actions["Cancel"].performed += ctx => gameObject.SetActive(false);
        gameObject.SetActive(false);
	}

	public void ShowCharacterWithText(DialogueLine line)
    {
		gameObject.SetActive(true);
        SpeakerName.text = line.Who;
        CharacterImage.sprite = line.Sprite;
        if (textween != null) FinishTween();
        dialogueText.text = "";
        textween = dialogueText.DOText(line.Text, textSpeed, true, ScrambleMode.None);
    }
    public void ShowCharacterWithText(List<DialogueLine> lines)
    {
        gameObject.SetActive(true);
        this.lines = new(lines);
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
        else
        {
            gameObject.SetActive(false);
        }
    }
    void FinishTween()
    {
        textween.Kill(true);
        textween = null;
    }

	private void OnEnable()
	{
		InputSystem.actions["Jump"].actionMap.Disable();
	}

	private void OnDisable()
	{
        FinishTween();
        lines.Clear();
		InputSystem.actions["Jump"].actionMap.Enable();
	}
}
