using UnityEngine;

public class ErikSceneManager : MonoBehaviour
{
    [SerializeField] GameObject Lights;
    [SerializeField] TextAsset BloodInspectionJSON;
    [SerializeField] SceneTransition sceneTransition;
    public void BloodInspection()
    {
        Lights.SetActive(false);
        Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(BloodInspectionJSON));
        sceneTransition.Enabled = true;
    }
}
