using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

[Serializable]
public class PlayerAgent : MonoBehaviour
{

  public PC playerCharacterData;

  void Awake()
  {
    PC tmp = new PC();
    //tmp.TAG = this.transform.gameObject.tag;
    //tmp.characterGO = this.transform.gameObject;
    tmp.NAME = "Player";
    tmp.HEALTH = 100.0f;
    tmp.STAMINA = 100.0f;
    tmp.DESCRIPTION = "Main Character";

    this.playerCharacterData = tmp;
  }

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (this.playerCharacterData.HEALTH < 0.0f)
    {
      this.playerCharacterData.HEALTH = 0.0f;

      //this.transform.GetComponent<CharacterController>().die = true;
    }
  }
}
