using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Windows;
using UnityEditor.Overlays;
using System.Xml.Serialization;

public class SaveSystem
{
	static AllSavedData savedData;
	static List<ISaveable> allSaveables;
	public static void Save()
	{
		savedData = new AllSavedData();
		foreach (ISaveable s in allSaveables)
			s.Save(savedData);
		string json = JsonUtility.ToJson(savedData);
		System.IO.File.WriteAllText(GlobalConstants.savePath, json);
	}

	public static void Load()
	{
		if (File.Exists(GlobalConstants.savePath))
		{
			string json = System.IO.File.ReadAllText(GlobalConstants.savePath);
			savedData = JsonUtility.FromJson<AllSavedData>(json);

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
		public Vector3 pos;
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
		public Vector3 pos;
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