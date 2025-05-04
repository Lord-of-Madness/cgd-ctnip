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
    [SerializeField] GameObject dialogueBox;
    [SerializeField] GameObject dialogTextLinePrefab;
    //[SerializeField] Text SpeakerName;
    Tween textween;

    Queue<DialogueLine> lines = new();
    [SerializeField] float textSpeed = 0.3f;
    public static Dialogue Instance { get; private set; }
    private InputActionsGen inputActions;
    private void Awake()
    {
        Instance = this;
        inputActions = new();
        inputActions.UI.Enable();
    }

    private void Start()
    {
        inputActions.UI.Skip.performed += ctx => SkipText();
        inputActions.UI.Cancel.performed += ctx => gameObject.SetActive(false);
        gameObject.SetActive(false);
	}

	void ShowCharacterWithText(DialogueLine line)
    {        
        //SpeakerName.text = line.Who;
        CharacterImage.sprite = line.Sprite;//Todo do this.
        if (textween != null) FinishTween();
        Text dialogTextLine = Instantiate(dialogTextLinePrefab, dialogueBox.transform).GetComponent<Text>();
        dialogTextLine.text = "";//If we clean up the prefab this can be removed.
        textween = dialogTextLine.DOText(line.Text, textSpeed, true, ScrambleMode.None).OnComplete(() => FinishTween());
    }
    public void ShowCharacterWithText(List<DialogueLine> lines)
    {
        Show();
        PurgeChildren(dialogueBox.transform);
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
        else Hide();
    }
    void FinishTween()
    {
        textween.Kill(true);
        textween = null;
    }
    void Show(){
        gameObject.SetActive(true);
        GameManager.Instance.inputActions.Player.Disable();
    }
    void Hide()
    {
        FinishTween();
        lines.Clear();
        GameManager.Instance.inputActions.Player.Enable();
        gameObject.SetActive(false);
    }
    void PurgeChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
