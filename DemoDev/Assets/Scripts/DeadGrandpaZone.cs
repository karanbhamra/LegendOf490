using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadGrandpaZone : MonoBehaviour {
    GameObject player;

    // Use this for initialization
    void Start () {
         player = GameObject.FindGameObjectWithTag("Player");
       // VikingCrewTools.SpeechbubbleManager.Instance.AddSpeechbubble(player.transform, "TEST");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        
        // testing to see if the collider zone is being triggered
        if (other.tag == "Player")
        {
            Debug.Log("Oh Shit! Gramps is dead! DAMN YOU BEEEEEES!!!!!!!!");
            VikingCrewTools.SpeechbubbleManager.Instance.AddSpeechbubble(player.transform, "GRANDPA?!\nDAMN YOU BEES!");

        }
    }
}
