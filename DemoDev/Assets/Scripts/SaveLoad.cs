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
		//FileStream file = File.Create(Application.dataPath + saveGame.saveGameName + ".sav");
		FileStream file = File.Create(saveGame.saveGameName + ".sav");
		bf.Serialize(file, saveGame);
		file.Close();
		Debug.Log("Saved Game: " + saveGame.saveGameName);

	}

	public static SaveData GameLoad(string gameToLoad)
	{
		SaveData loadedGame;
		if (File.Exists(gameToLoad + ".sav"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			//FileStream file = File.Open(Application.dataPath + gameToLoad + ".sav", FileMode.Open);
			FileStream file = File.Open(gameToLoad + ".sav", FileMode.Open);
			loadedGame = (SaveData)bf.Deserialize(file);
			file.Close();
			Debug.Log("Loaded Game: " + loadedGame.saveGameName);
			return loadedGame;
		}
		else
		{
			Debug.Log("File doesn't exist!");
			return null;
		}

	}

}
