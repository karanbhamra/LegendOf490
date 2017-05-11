using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FootSteps : MonoBehaviour
{
	public AudioSource sound;

	private void Start()
	{

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W) ||
		   Input.GetKeyDown(KeyCode.A) ||
		   Input.GetKeyDown(KeyCode.S) ||
		   Input.GetKeyDown(KeyCode.D))
		{
			sound.loop = true;
			sound.Play();

		}
		if (Input.GetKeyUp(KeyCode.W) ||
			Input.GetKeyUp(KeyCode.A) ||
			Input.GetKeyUp(KeyCode.S) ||
			Input.GetKeyUp(KeyCode.D))
		{
			sound.loop = false;
		}
	}
}