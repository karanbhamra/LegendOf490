using UnityEngine;
using UnityEngine.UI;// we need this namespace in order to access UI elements within our script
using System.Collections;
using UnityEngine.SceneManagement; // neded in order to load scenes
using System.IO;
using System.Collections.Generic;
using System;

public class MainMenu : MonoBehaviour
{


	void Start()

	{

	}



	public void LoadLatestSaveFile()
	{
		var currentDirectory = System.IO.Directory.GetParent (Application.dataPath).ToString ();
		Debug.Log ("Current Directory: " + currentDirectory );
		var listOfSaves = Directory.GetFiles (currentDirectory, "*.bee");
		foreach(var save in listOfSaves)
		{
			Debug.Log (save.ToString ());
			if (save.Contains ("Prologue"))
			{
				//SaveData loadedGame = SaveLoad.GameLoad ("Prologue");
				SceneManager.LoadScene ("Prologue");
				Debug.Log ("Loaded LATEST SAVE");

				SaveData loadedGame = SaveLoad.GameLoad ("Prologue");
			}
		}

		
	}
		

	public void StartLevel() //this function will be used on our Play button

	{
		Debug.Log("Prologue loaded");
		SceneManager.LoadScene("Prologue"); //this will load our first level from our build settings. "1" is the second scene in our game


	}

	public void ExitGame() //This function will be used on our "Yes" button in our Quit menu

	{
		Application.Quit(); //this will quit our game. Note this will only work after building the game

	}

}