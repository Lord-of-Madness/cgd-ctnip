using UnityEngine;

public class ExteriorSceneManager : MonoBehaviour
{
    [SerializeField] TextAsset MissionBrief;
    [SerializeField] TextAsset Outside;
    private DialogueTreeNode MissionBriefDT;
    private DialogueTreeNode OutsideDT;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip ambientMusic;
    void Start()
    {
        MissionBriefDT = DialogueTreeNode.DeserializeTree(MissionBrief);
        audioSource.clip = ambientMusic;
        audioSource.Play();
        OutsideDT = DialogueTreeNode.DeserializeTree(Outside);
        StartCoroutine(Utilities.CallAfterSomeTime(
            () =>
        {

            Dialogue.Instance.dialogueEnded.AddListener(() =>
            {
                foreach (Document document in GameManager.Instance.ActivePlayer.playerData.Documents)
                    if ("Controls" == document.name)
                    {
                        if (GameManager.Instance.erikPC != null)
                            GameManager.Instance.erikPC.playerData.Documents.Add(document);
                        break;
                    }
            });
            Dialogue.Instance.ShowCharacterWithText(MissionBriefDT);
            

        }, 1f)
            );
    }
    public void EnterMansion()
    {
        SceneTransitionManager.LoadNewScene("GramofonScene");
    }
    public void ShowOutsideDialogue()
    {
        Dialogue.Instance.ShowCharacterWithText(OutsideDT);
    }
}
