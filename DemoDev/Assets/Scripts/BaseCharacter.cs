using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour {
  private string name;
  private string description;
  private float health;
  private float stamina;

  public string NAME {
    get { return this.name; }
    set { this.name = value; }
  }
  public string DESCRIPTION {
    get { return this.description; }
    set { this.description = value; }
  }
  public float HEALTH {
    get { return this.health; }
    set { this.health = value; }
  }
  public float STAMINA {
    get { return this.stamina; }
    set { this.stamina = value; }
  }
  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
