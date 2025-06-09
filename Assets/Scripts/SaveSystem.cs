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
	static List<ISaveable> allSceneSaveables = new();

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

		AddSaveable(GameManager.Instance);

		UpdateSceneSavePath();
		
	}


	static void UpdateSceneSavePath()
	{
		completeSceneSavePath = GlobalConstants.savePath + "/" + SceneManager.GetActiveScene().name + ".json";
		completeGenericSavePath = GlobalConstants.savePath + "/generalSave.json";
	}

	public static void DeleteCurrentSceneSave()
	{
		UpdateSceneSavePath();
		File.Delete(completeSceneSavePath);
	}

	public static void SaveSceneData()
	{
		UpdateSceneSavePath();
		savedSceneData = new AllSavedData();
		foreach (ISaveable s in allSceneSaveables)
			s.Save(savedSceneData);
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
				s.Load(savedSceneData);
		}
		else
		{
			SaveSceneData();
			Debug.LogWarning("No save file found -> saving...");
		}

		savedSceneData = null; //Save memory

	}

	public static void AddSaveable(ISaveable s) => allSceneSaveables.Add(s);



	public interface ISaveable
	{
		public void Save(AllSavedData dataHolder);
		public void Load(AllSavedData data);

	}

	[Serializable]
	public class AllSavedData
	{
		public Dictionary<string, CharacterData> charData = new();
		public Dictionary<string, EnemyData> enemyData = new();
		public Dictionary<string, DoorData> doorData = new();
		public GameManagerData gameManagerData;
		public MansionLevelData mansionLevelData;
		public GramophoneLevelData gramophoneLevelData;
	}


	[Serializable]
	public class CharacterData
	{
		public string name;
		public Vector3JsonFriendly pos;
		
		
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