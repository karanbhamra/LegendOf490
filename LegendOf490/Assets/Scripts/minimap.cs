using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class minimap : MonoBehaviour {

	public GameObject target = null;
	public float camHeight = 200.0f;
	//Vector3 pos;

	// Use this for initialization
	void Start () {
		//pos = Vector3.zero;

	}

	// Update is called once per frame
	void Update () {
		//pos = target.transform.position;
		transform.position = new Vector3 (target.transform.position.x, camHeight, target.transform.position.z);


	}
}
