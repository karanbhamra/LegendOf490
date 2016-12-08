using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

  public Animator animator;
  private Vector3 moveDirection;

  public float speed = 0.0f;
  public float h = 0.0f;
  public float v = 0.0f;
  public float tiltAngleY;

  bool upPressed;
  bool downPressed;
  bool rightPressed;
  bool leftPressed;
  bool attackPressed;
  KeyCode currentStroke;

  public bool attack1 = false;  // used for attack mode 1
  public bool attack2 = false;  // used for attack mode 2
  public bool attack3 = false;  // used for attack mode 3

  public bool jump = false;     // used for jumping
  public bool die = false;      // are we alive?

  // Use this for initialization
  void Start() {
    this.animator = GetComponent<Animator>() as Animator;
    moveDirection = Vector3.zero;
  }

  // Update is called once per frame
  void Update() {
    if (speed > 0.05f) {
      speed -= 0.05f;
    }
    if(speed > 35.0f) {
      speed = 35.0f;
    }
    if(speed <= 0.05f){
      speed = 0.0f;
    }
  }

  void FixedUpdate() {
    if(upPressed = Input.GetKey(KeyCode.W)) {
      v = 1.0f;
      tiltAngleY = 0.0f;
    }
    if (downPressed = Input.GetKey(KeyCode.S)) {
      v = -1.0f;
      tiltAngleY = 180.0f;
    }
    if(rightPressed = Input.GetKey(KeyCode.D)) {
      h = 1.0f;
      tiltAngleY = 90.0f;
    }
    if(leftPressed = Input.GetKey(KeyCode.A)) {
      h = -1.0f;
      tiltAngleY = 270.0f;
    }
    if(attackPressed = Input.GetMouseButton(0)) {
      speed -= 0.1f;
    }

    // The Inputs are defined in the Input Manager
    if(h != 0.0f || v != 0.0f && speed < 35.0f) {
      speed += 0.2f;
    }

    transform.rotation = Quaternion.Euler(0.0f, tiltAngleY, 0.0f);
    transform.position += transform.forward * Time.deltaTime * speed;

    animator.SetFloat("Speed", speed);
    animator.SetFloat("Horizontal", h);
    animator.SetFloat("Vertical", v);
    animator.SetBool("MeleeAttack", attackPressed);

    h = 0.0f;
    v = 0.0f;
  }

}
