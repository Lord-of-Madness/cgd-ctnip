using UnityEngine;
using UnityEngine.SceneManagement;

public class ExteriorSceneManager : MonoBehaviour
{
    [SerializeField] TextAsset MissionBrief;
    [SerializeField] TextAsset Outside;
    private DialogueTreeNode MissionBriefDT;
    private DialogueTreeNode OutsideDT;
    void Start()
    {
        MissionBriefDT = DialogueTreeNode.DeserializeTree(MissionBrief);
        OutsideDT = DialogueTreeNode.DeserializeTree(Outside);
        StartCoroutine(Utilities.CallAfterSomeTime(() => Dialogue.Instance.ShowCharacterWithText(MissionBriefDT),1f));
    }
    public void EnterMansion()
    {
        SceneManager.LoadScene("GramofonScene");
    }
    public void ShowOutsideDialogue()
    {
        Dialogue.Instance.ShowCharacterWithText(OutsideDT);
    }
}
