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
    [SerializeField] string DialogueJSON ="";

    private readonly List<DialogueLine> diaLines = new();
    bool shown = false;

    public bool DEBUGGENERATEJSON = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DialogueJSON != "" && (lines==null || lines.Count==0))
        {
            lines = JsonUtility.FromJson<List<string>>(DialogueJSON);
        }
        initDialogueLines();
    }
    /// <summary>
    /// Only for testing purposes
    /// </summary>
    void initDialogueLines()
    {
        for (int i = 0; i < lines.Count; i++)//Testing explicit speakers
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
            DialogueTreeNode dialogueRoot = DialogueTreeNode.BuildSimpleTree(diaLines);
            if (DEBUGGENERATEJSON)
            {
                dialogueRoot.SerializeTree("IHaveNoClueWhatThisWillDo.json");//Application.dataPath
            }

            Dialogue.Instance.ShowCharacterWithText(dialogueRoot /*,() => {
                shown = true;
                GameManager.Instance.ActivePlayer.playerData.Documents.Add(new("TestDocument", "BEHOLD THIS IS THE TEXT DOCUMENT OF DOOM!"));
            }*/);//Horribly se to podìlá kdyby nìco zabilo NPCèko bìhem hovoru.
        }
    }
    
}
