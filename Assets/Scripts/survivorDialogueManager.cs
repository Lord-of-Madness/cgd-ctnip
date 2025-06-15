using UnityEngine;

public class SurvivorDialogueManager : MonoBehaviour
{
    [SerializeField] TextAsset basement;
    [SerializeField] TextAsset basement_alt;
    [SerializeField] TextAsset basement_secret;
    [SerializeField] TextAsset basement_secret_alt;
    [SerializeField] TextAsset helpJSON;
    [SerializeField] int howManyDocsThereAre;
    public void StartDialogue()
    {
        bool alt = 
            GameManager.Instance.bethPC.playerData.Codex.Count +
            GameManager.Instance.bethPC.playerData.Inventory.Count +
            GameManager.Instance.bethPC.playerData.Documents.Count +
            GameManager.Instance.erikPC.playerData.Codex.Count +
            GameManager.Instance.erikPC.playerData.Inventory.Count +
            GameManager.Instance.erikPC.playerData.Documents.Count == howManyDocsThereAre;
        bool secret = GameManager.Instance.secretActivated;


        Dialogue.Instance.dialogueEnded.AddListener(() =>
        {
            bool help = false;
            GameManager.APD.Codex.ForEach((doc) =>
            {
                if (doc.name == "A survivor, helped")
                {
                    help = true;
                }
            });
            Dialogue.Instance.dialogueEnded.RemoveAllListeners();
            if (help)
            {
                Dialogue.Instance.dialogueEnded.AddListener(GameManager.EndScreem);
                Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(helpJSON));
            }
            else GameManager.EndScreem();
        });

        if (alt)
        {
            if(secret)
            {
                Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(basement_secret_alt));
            }
            else
            {
                Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(basement_alt));
            }
        }
        else
        {
            if (secret)
            {
                Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(basement_secret));
            }
            else
            {
                Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(basement));
            }
        }
    }
}
