using UnityEngine;

public class survivorDialogueManager : MonoBehaviour
{
    [SerializeField] TextAsset basement;
    [SerializeField] TextAsset basement_alt;
    [SerializeField] TextAsset basement_secret;
    [SerializeField] TextAsset basement_secret_alt;
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
