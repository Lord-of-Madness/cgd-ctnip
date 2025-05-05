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

    private readonly List<DialogueLine> diaLines = new();
    bool shown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initDialogueLines();
    }

    void initDialogueLines()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (i % 2 == 0)
            {
                diaLines.Add(new DialogueLine(lines[i], Speaker.Beth));
            }
            else
            {
                diaLines.Add(new DialogueLine(lines[i], Speaker.Erik));
            }
        }
        foreach (var line in lines)
        {
            diaLines.Add(new DialogueLine(line, charName, sprite, Color.red));
        }
    }

    public void ShowDialogue()
    {
        if (!shown)
        {
            Dialogue.Instance.ShowCharacterWithText(diaLines,() => {
                shown = true;
                GameManager.Instance.ActivePlayer.playerData.Documents.Add(new("TestDocument", "BEHOLD THIS IS THE TEXT DOCUMENT OF DOOM!"));
            });//Horribly se to podìlá kdyby nìco zabilo NPCèko bìhem hovoru.
        }
    }
}
