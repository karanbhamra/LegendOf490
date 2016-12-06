using UnityEngine;
using System.Collections;

public class enemyAI : MonoBehaviour {

  public float targetDistance;
  public float enemyLookDistance;
  public float attackDistance;
  public float enemyMovementSpeed;
  public float damping;
  public Transform target;
  private Rigidbody theRigidBody;
  //Renderer myRenderer;


  // Use this for initialization
  void Start () {
   // myRenderer.GetComponent<Renderer>();
    theRigidBody = GetComponent<Rigidbody>();
    if(theRigidBody != null){
      print("rigid body not null");
    }
	}
	
	// Update is called once per frame
	void FixedUpdate () {

    targetDistance = Vector3.Distance(target.position, transform.position);
    if (targetDistance < enemyLookDistance) {
      lookAtPlayer();
      print("look at player");
    }
    if(targetDistance < attackDistance) {
      attackPlayer();
      print("attack player");
    } else {
      //myRenderer
    }
	}

  void lookAtPlayer() {
    Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*damping);
  }

  void attackPlayer() {
    theRigidBody.AddForce(transform.forward * enemyMovementSpeed);
  }
}
