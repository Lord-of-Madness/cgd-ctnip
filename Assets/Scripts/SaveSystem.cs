using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class SaveSystem:MonoBehaviour
{
	static string completeGenericSavePath;
	static string completeSceneSavePath;
	static AllSavedData savedSceneData;
	static AllSavedData savedGenData;
	static List<ISaveable> allSceneSaveables = new();
	static List<ISaveable> allGenSaveables = new();

	bool firstUpdate = true;
	private void Awake()
	{
		allSceneSaveables.Clear();
	}
	private void Start()
	{
		if (!Directory.Exists(GlobalConstants.savePath))
		{
			Directory.CreateDirectory(GlobalConstants.savePath);
		}

		if (!allGenSaveables.Contains(GameManager.Instance))
			AddGeneralSaveable(GameManager.Instance);

		UpdateSceneSavePath();
		
	}

	public static void RemoveGenSaveable(ISaveable saveable)
	{
		allGenSaveables.Remove(saveable);
	}

	static void UpdateSceneSavePath()
	{
		completeSceneSavePath = GlobalConstants.savePath + "/" + SceneManager.GetActiveScene().name + ".json";
		completeGenericSavePath = GlobalConstants.savePath + "/generalSave.json";
	}

	public static void DeleteCurrentGenSave()
	{
		UpdateSceneSavePath();
		File.Delete(completeSceneSavePath);
	}

	public static void SaveAll()
	{
		SaveGenData();
		SaveSceneData();
	}

	public static void LoadAll()
	{
		LoadGenData();
		LoadSceneData();
	}

	public static void SaveSceneData()
	{
		UpdateSceneSavePath();
		savedSceneData = new AllSavedData();
		foreach (ISaveable s in allSceneSaveables)
			s.SaveSceneSpecific(savedSceneData);
		string json = JsonConvert.SerializeObject(savedSceneData);
		File.WriteAllText(completeSceneSavePath, json); 
		Debug.Log("Data saved to " + completeSceneSavePath);
		savedSceneData = null; //Save memory
	}

	public static void LoadSceneData()
	{
		UpdateSceneSavePath();
		if (File.Exists(completeSceneSavePath))
		{
			string json = System.IO.File.ReadAllText(completeSceneSavePath);
			savedSceneData = JsonConvert.DeserializeObject<AllSavedData>(json);

			foreach (ISaveable s in allSceneSaveables)
				s.LoadSceneSpecific(savedSceneData);
		}
		else
		{
			SaveSceneData();
			Debug.LogWarning("No save file found -> saving...");
		}

		savedSceneData = null; //Save memory

	}

	public static void AddSceneSaveable(ISaveable s) => allSceneSaveables.Add(s);

	public static void DeleteCurrentSceneSave()
	{
		UpdateSceneSavePath();
		File.Delete(completeGenericSavePath);
	}

	public static void SaveGenData()
	{
		UpdateSceneSavePath();
		savedGenData = new AllSavedData();
		foreach (ISaveable s in allGenSaveables)
			s.SaveGeneric(savedGenData);
		string json = JsonConvert.SerializeObject(savedGenData);
		File.WriteAllText(completeGenericSavePath, json);
		Debug.Log("Data saved to " + completeGenericSavePath);
		savedGenData = null; //Save memory
	}

	public static void LoadGenData()
	{
		UpdateSceneSavePath();
		if (File.Exists(completeGenericSavePath))
		{
			string json = System.IO.File.ReadAllText(completeGenericSavePath);
			savedGenData = JsonConvert.DeserializeObject<AllSavedData>(json);

			foreach (ISaveable s in allGenSaveables)
				s.LoadGeneric(savedGenData);
		}
		else
		{
			SaveGenData();
			Debug.LogWarning("No save file found -> saving...");
		}

		savedGenData = null; //Save memory

	}


	public static void AddGeneralSaveable(ISaveable s) => allGenSaveables.Add(s);



	public interface ISaveable
	{
		public void SaveGeneric(AllSavedData dataHolder);
		public void LoadGeneric(AllSavedData data);
		public void SaveSceneSpecific(AllSavedData dataHolder);
		public void LoadSceneSpecific(AllSavedData data);

	}

	[Serializable]
	public class AllSavedData
	{
		public Dictionary<string, CharacterSceneData> charSceneData = new();
		public Dictionary<string, CharacterGenData> charGenData = new();
		public Dictionary<string, EnemyData> enemyData = new();
		public Dictionary<string, DoorData> doorData = new();
		public GameManagerData gameManagerData;
		public MansionLevelData mansionLevelData;
		public GramophoneLevelData gramophoneLevelData;
	}


	[Serializable]
	public class CharacterSceneData
	{
		public string name;
		public Vector3JsonFriendly pos;
	}

	[Serializable]
	public class CharacterGenData
	{

		public ToolData revolverData;
		public ToolData cameraData;


		public List<Document> Documents = new();
		public List<Document> Codex = new();
		public List<Document> Inventory = new();
	}

	[Serializable]
	public class ToolData
	{
		public string name;
		public int stashedAmmo;
		public int loadedAmmo;
	}

	[Serializable]
	public class EnemyData
	{
		public int hp;
		public Vector3JsonFriendly pos;
		public bool following;
		public bool aggroed;

	}

	[Serializable]
	public class DoorData
	{
		public bool isOpen;
	}

	[Serializable]
	public class GameManagerData
	{
		public int activePlayer;
	}

	[Serializable]
	public class MansionLevelData
	{
		public bool keyPickedUp;
	}
	
	[Serializable]
	public class GramophoneLevelData
	{
		public bool generatorFixed;
	}


}

public struct Vector3JsonFriendly
{
	public float x;
	public float y;
	public float z;

	public Vector3JsonFriendly(Vector3 vec)
	{
		x = vec.x;
		y = vec.y;
		z = vec.z;
	}
	
	public Vector3 GetVector3()
	{
		return new Vector3(x, y, z);
	}
}