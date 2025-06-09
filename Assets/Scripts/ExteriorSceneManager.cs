using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
