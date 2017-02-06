using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transforms : MonoBehaviour
{
	float growthRate = 0.005f;
	float radius = 5;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		transform.Rotate(Vector3.right * 1);
		//transform.localScale += new Vector3(growthRate, growthRate, growthRate);
		transform.position += new Vector3(radius * Mathf.Sin(0.005f), 0, 0);
		transform.position += new Vector3(0, radius * Mathf.Sin(0.005f), 0);




	}
}
