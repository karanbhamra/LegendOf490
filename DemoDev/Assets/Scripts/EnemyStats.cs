using UnityEngine;
using System.Collections;
public class EnemyStats : MonoBehaviour {

  public float curHp;
  public float maxHp;

  // Use this for initialization
  void Start() {
    curHp = 100;
    maxHp = 100;
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
}
