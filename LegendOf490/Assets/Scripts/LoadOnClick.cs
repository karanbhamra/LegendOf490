using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {

	public void LoadScene(int level) {
		//Application.LoadLevel (level);
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}

	public void ExitGame() {

		// Quit is ignored in the Editor, works in binary
		Application.Quit ();
	}
}
