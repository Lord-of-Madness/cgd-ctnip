using UnityEngine;

public class ErikSceneManager : MonoBehaviour
{
    [SerializeField] GameObject Lights;
    [SerializeField] TextAsset BloodInspectionJSON;
    public void BloodInspection()
    {
        Lights.SetActive(false);
        Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(BloodInspectionJSON));
    }
}
