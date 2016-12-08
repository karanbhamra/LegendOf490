using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
    DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void StartGame() {
    SceneManager.LoadScene("Prologue");
  }
}
