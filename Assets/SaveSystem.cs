using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows;
using Newtonsoft.Json;

public class SaveSystem:MonoBehaviour
{
	static AllSavedData savedData;
	static List<ISaveable> allSaveables = new();
	public static void Save()
	{
		savedData = new AllSavedData();
		foreach (ISaveable s in allSaveables)
			s.Save(savedData);
		string json = JsonConvert.SerializeObject(savedData);
		System.IO.File.WriteAllText(GlobalConstants.savePath, json);
		Debug.Log("Data saved to " + GlobalConstants.savePath);
	}

	public static void Load()
	{
		if (File.Exists(GlobalConstants.savePath))
		{
			string json = System.IO.File.ReadAllText(GlobalConstants.savePath);
			savedData = JsonConvert.DeserializeObject<AllSavedData>(json);

			foreach (ISaveable s in allSaveables)
				s.Load(savedData);
		}
		else
		{
			Debug.LogWarning("No save file found");
		}
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
		public GameManagerData gameManagerData;
		public MansionLevelData mansionLevelData;
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

	}

	[Serializable]
	public class GameManagerData
	{

	}

	[Serializable]
	public class MansionLevelData
	{
		public bool keyPickedUp;
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