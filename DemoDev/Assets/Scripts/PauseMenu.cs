using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public string levelToLoad;
	public bool paused = false;
	private string saveGameName = "";
	public string testSaveData = "";

	private void Start()
	{
		Time.timeScale = 1; //Set the timeScale back to 1 for Restart option to work
	}

	private void Update()
	{

		if (Input.GetKeyDown(KeyCode.Escape)) //check if Escape key/Back key is pressed
		{
			if (paused)
				paused = false;  //unpause the game if already paused
			else
				paused = true;  //pause the game if not paused
		}

		if (paused)
			Time.timeScale = 0;  //set the timeScale to 0 so that all the procedings are halted
		else
			Time.timeScale = 1;  //set it back to 1 on unpausing the game

	}

	private void OnGUI()
	{
		if (paused)
		{

			if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "RESUME"))
			{
				paused = false;
			}

			if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + 2 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "SAVE GAME"))
			{
				//print ("save functionality not yet implemented");
				SaveData newGameSave = new SaveData();
				saveGameName = SceneManager.GetActiveScene().ToString();
				newGameSave.saveGameName = saveGameName;
				newGameSave.testData = "testing"; // will include player coords when up and working
				SaveLoad.GameSave(newGameSave);


			}

			if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + 3 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "LOAD GAME"))
			{
				//print ("load functionality not yet implemented");

				SaveData loadedGame = SaveLoad.GameLoad(saveGameName);
				if (loadedGame != null)
				{
					testSaveData = loadedGame.testData;
				}
			}

			if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + 4 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "RESTART"))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
			if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + 5 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "MAIN MENU"))
			{
				SceneManager.LoadScene(0, LoadSceneMode.Single);
			}
		}
	}
}