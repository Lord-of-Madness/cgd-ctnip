using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class SaveSystem:MonoBehaviour
{
	static string completeSavePath;
	static AllSavedData savedData;
	static List<ISaveable> allSaveables = new();

	bool firstUpdate = true;
	private void Awake()
	{
		allSaveables.Clear();
	}
	private void Start()
	{
		if (!Directory.Exists(GlobalConstants.savePath))
		{
			Directory.CreateDirectory(GlobalConstants.savePath);
		}

		AddSaveable(GameManager.Instance);

		UpdateSavePath();
		
	}


	static void UpdateSavePath()
	{
		completeSavePath = GlobalConstants.savePath + "/" + SceneManager.GetActiveScene().name + ".json";
	}

	public static void DeleteCurrentSave()
	{
		UpdateSavePath();
		File.Delete(completeSavePath);
	}

	public static void Save()
	{
		UpdateSavePath();
		savedData = new AllSavedData();
		foreach (ISaveable s in allSaveables)
			s.Save(savedData);
		string json = JsonConvert.SerializeObject(savedData);
		File.WriteAllText(completeSavePath, json); 
		Debug.Log("Data saved to " + completeSavePath);
		savedData = null; //Save memory
	}

	public static void Load()
	{
		UpdateSavePath();
		if (File.Exists(completeSavePath))
		{
			string json = System.IO.File.ReadAllText(completeSavePath);
			savedData = JsonConvert.DeserializeObject<AllSavedData>(json);

			foreach (ISaveable s in allSaveables)
				s.Load(savedData);
		}
		else
		{
			Save();
			Debug.LogWarning("No save file found -> saving...");
		}

		savedData = null; //Save memory

	}

	public static void AddSaveable(ISaveable s) => allSaveables.Add(s);



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