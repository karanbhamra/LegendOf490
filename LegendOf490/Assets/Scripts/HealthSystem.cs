using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {

	// in spector attach the object
	public GameObject healthObject;

	public float health = 100.0f;

	// TODO: on damage, adjust healthbar

	// Use this for initialization
	void Start () {
	
	}

	public float getHealth()
	{
		return health;
	}

	public void setHealth(float value)
	{
		health = value;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		string type = healthObject.tag;
		print (type + " health at: " + getHealth ());
	}
}
