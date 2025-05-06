using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueGiver : MonoBehaviour
{
    [SerializeField]
    List<string> lines;
    [SerializeField]
    string charName;
    [SerializeField]
    Sprite sprite;
    [SerializeField] TextAsset DialogueJSON;

    private DialogueTreeNode dialogueTree;
    bool shown = false;

    /// <summary>
    /// Toggles which source it should prefer for dialogue -> lines in inspector or json file
    /// </summary>
    bool PREFERJSON = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DialogueJSON != null && DialogueJSON.text != "" && (PREFERJSON || (lines == null || lines.Count == 0)))//If json is valid and prefered or if the lines are empty.
        {
            dialogueTree = DialogueTreeNode.DeserializeTree(DialogueJSON);
            Debug.Log("I used json!");
        }
        else
        {
            Debug.Log("I used inspector lines!");
            List<DialogueLine> diaLines = new();
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
            const string path = "IHaveNoClueWhatThisWillDo.json";
            DialogueTreeNode.BuildSimpleTree(diaLines).SerializeTree(path, new() {
                    new(Speaker.Beth.Name,Speaker.Beth.TextColor.ToHexString()),
                    new(Speaker.Erik.Name,Speaker.Erik.TextColor.ToHexString())
                });//Application.dataPath
            dialogueTree = DialogueTreeNode.DeserializeTree(path);
        }
    }

    public void ShowDialogue()
    {
        if (!shown)
        {
            Dialogue.Instance.ShowCharacterWithText(dialogueTree);
            shown = true;
        }
    }
    
}
