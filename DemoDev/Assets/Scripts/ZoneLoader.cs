using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E))
		{
			print("Player is in loading zone.");
            print("Loading next level now...");
            SceneManager.LoadScene("Village");

            /*
			if (Input.GetKeyDown(KeyCode.E))
			{
				print("Loading next level now...");
				SceneManager.LoadScene("Village");
				
			}
            */
        }
		
	}
}
