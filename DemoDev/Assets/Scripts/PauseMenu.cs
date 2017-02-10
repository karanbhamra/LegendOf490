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
	public Vector3 playerPos;

	private void Start()
	{
		Time.timeScale = 1; //Set the timeScale back to 1 for Restart option to work
		saveGameName = SceneManager.GetActiveScene().name;  // get scene name for filename

		if (PlayerPrefs.HasKey ("newgame") && PlayerPrefs.GetString ("newgame") == "false")
		{
			PositionUpdate ();
			
		}
		else{
			PlayerPrefs.SetFloat ("PlayerX", -20);
			PlayerPrefs.SetFloat ("PlayerY", 0);
			PlayerPrefs.SetFloat ("PlayerZ", -89);
		}
	}

	public void PositionUpdate()
	{
		SaveData loadedGame = SaveLoad.GameLoad(saveGameName);
		if (loadedGame != null)
		{
			playerPos.x = PlayerPrefs.GetFloat ("PlayerX");//loadedGame.playerPositionX;
			playerPos.y = PlayerPrefs.GetFloat ("PlayerY");//loadedGame.playerPositionY;
			playerPos.z = PlayerPrefs.GetFloat ("PlayerZ");//loadedGame.playerPositionZ;

			GameObject.FindGameObjectWithTag("Player").transform.position = playerPos;
			testSaveData = loadedGame.testData;
		}
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

			// game save
			if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + 2 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "SAVE GAME"))
			{
				SaveData newGameSave = new SaveData();
				//saveGameName = SceneManager.GetActiveScene().name;  // get scene name for filename
				newGameSave.saveGameName = saveGameName;
				// player xyz coords
				Vector3 playerCoords = GameObject.FindGameObjectWithTag("Player").transform.position;
				newGameSave.playerPositionX = playerCoords.x;
				newGameSave.playerPositionY = playerCoords.y;
				newGameSave.playerPositionZ = playerCoords.z;
				newGameSave.testData = "testing"; // will include player coords when up and working
				SaveLoad.GameSave(newGameSave);


			}

			// game load
			if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + 3 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "LOAD GAME"))
			{

//				SaveData loadedGame = SaveLoad.GameLoad(saveGameName);
//				if (loadedGame != null)
//				{
//					playerPos.x = loadedGame.playerPositionX;
//					playerPos.y = loadedGame.playerPositionY;
//					playerPos.z = loadedGame.playerPositionZ;
//
//					GameObject.FindGameObjectWithTag("Player").transform.position = playerPos;
//					testSaveData = loadedGame.testData;
//				}
				PositionUpdate ();
				playerPos.x = PlayerPrefs.GetFloat ("PlayerX");//loadedGame.playerPositionX;
				playerPos.y = PlayerPrefs.GetFloat ("PlayerY");//loadedGame.playerPositionY;
				playerPos.z = PlayerPrefs.GetFloat ("PlayerZ");//loadedGame.playerPositionZ;
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