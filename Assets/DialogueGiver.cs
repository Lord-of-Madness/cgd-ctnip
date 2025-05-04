using System.Collections.Generic;
using UnityEngine;

public class DialogueGiver : MonoBehaviour
{
    [SerializeField]
    List<string> lines;
    [SerializeField]
    string charName;
    [SerializeField]
    Sprite sprite;

    List<DialogueLine> diaLines = new();
    bool shown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initDialogueLines();
    }

    void initDialogueLines()
    {
		foreach (var line in lines)
		{
			diaLines.Add(new DialogueLine(line, charName, sprite));
		}
	}

    public void ShowDialogue()
    {
        if (!shown) { 
            Dialogue.Instance.ShowCharacterWithText(diaLines);
        shown = true;}
    }
}
