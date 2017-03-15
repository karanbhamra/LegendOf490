using UnityEngine;
using System.Collections;
public class EnemyStats : MonoBehaviour {

  public float curHp;
  public float maxHp;
    public float damage;

  // Use this for initialization
  void Start() {
    curHp = 100;
    maxHp = 100;
    damage = 10;
  }

  // Update is called once per frame
  void Update() {
    if (curHp <= 0) {
      GameObject.Destroy(gameObject);
    }
  }

  public void ReceiveDamage(float dmg) {
    curHp -= dmg;

    print("damage done = " + dmg);
    print("enemy hp = " + curHp);
  }

  public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
//            this.curHp -= other.GetComponent<EnemyStats>().damage; // this is throwing a nullpointerexception
        }
        if(curHp < 1.0f)
        {
            DestroyObject(this);
        }
    }
}
