using UnityEngine;

public class RemovableObject : MonoBehaviour, SaveSystem.ISaveable
{

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        SaveSystem.AddSceneSaveable(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void LoadGeneric(SaveSystem.AllSavedData data)
	{
		throw new System.NotImplementedException();
	}

	public void LoadSceneSpecific(SaveSystem.AllSavedData data)
	{
		if (data.remObjData.ContainsKey(Utilities.GetFullPathName(gameObject)))
		{
			SaveSystem.RemovableObjectData myData = data.remObjData[Utilities.GetFullPathName(gameObject)];
			if (myData.isRemoved) gameObject.SetActive(false);
			else gameObject.SetActive(true);
		}
		else
			gameObject.SetActive(false);
	}

	public void SaveGeneric(SaveSystem.AllSavedData dataHolder)
	{
		throw new System.NotImplementedException();
	}

	public void SaveSceneSpecific(SaveSystem.AllSavedData dataHolder)
	{
		dataHolder.remObjData[Utilities.GetFullPathName(gameObject)] = new SaveSystem.RemovableObjectData() { isRemoved = !gameObject.activeSelf };
	}
}
