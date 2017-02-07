using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class SaveLoad
{


	public static void GameSave(SaveData saveGame)
	{

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.dataPath + saveGame.saveGameName + ".sav");
		bf.Serialize(file, saveGame);
		file.Close();
		Debug.Log("Saved Game: " + saveGame.testData);

	}

	public static SaveData GameLoad(string gameToLoad)
	{
		SaveData loadedGame = null;
		if (File.Exists(Application.dataPath + gameToLoad + ".sav"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.dataPath + gameToLoad + ".sav", FileMode.Open);
			loadedGame = (SaveData)bf.Deserialize(file);
			file.Close();
			Debug.Log("Loaded Game: " + loadedGame.testData);
			return loadedGame;
		}
		else
		{
			Debug.Log("File doesn't exist!");
			return loadedGame;
		}

	}

}
