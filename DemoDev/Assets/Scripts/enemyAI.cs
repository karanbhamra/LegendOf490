using UnityEngine;
using System.Collections;

public class enemyAI : MonoBehaviour {

  public float targetDistance;
  public float enemyLookDistance;
  public float attackDistance;
  public float enemyMovementSpeed;
  public float damping;
  public Transform target;
  public EnemyStats stats;

  // Use this for initialization
  void Start () {
    stats = GetComponent<EnemyStats>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
    targetDistance = Vector3.Distance(target.position, transform.position);
    if (targetDistance < enemyLookDistance) {
      lookAtPlayer();
      //print("look at player");
    }
    if(targetDistance < attackDistance) {
      attackPlayer();
     // print("attack player");
    }
    if(stats.curHp <= 0) {
      DestroyObject(this);
    }
	}

  void lookAtPlayer() {
    Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*damping);
  }

  void attackPlayer() {
    transform.position = Vector3.MoveTowards(transform.position, target.position, enemyMovementSpeed * Time.deltaTime);
  }
}
