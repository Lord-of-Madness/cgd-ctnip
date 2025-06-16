using UnityEngine;

public class ErikSceneManager : MonoBehaviour, SaveSystem.ISaveable
{
    [SerializeField] GameObject Lights;
    [SerializeField] TextAsset BloodInspectionJSON;
    [SerializeField] SceneTransition sceneTransition;

	private void Start()
	{
        SaveSystem.AddGeneralSaveable(this);
	}

	public void BloodInspection()
    {
        Lights.SetActive(false);
        Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(BloodInspectionJSON));
        sceneTransition.Enabled = true;
		GameManager.Instance.ErikInspectedBlood = true;
    }

	public void SaveGeneric(SaveSystem.AllSavedData dataHolder)
	{
		return; //Blood inspection is save in GameManager
	}

	public void LoadGeneric(SaveSystem.AllSavedData data)
	{

		Lights.SetActive(!data.erikLevelData.inspectedBlood);
		sceneTransition.Enabled = data.erikLevelData.inspectedBlood;

	}

	public void SaveSceneSpecific(SaveSystem.AllSavedData dataHolder)
	{
		throw new System.NotImplementedException();
	}

	public void LoadSceneSpecific(SaveSystem.AllSavedData data)
	{
		throw new System.NotImplementedException();
	}
}
