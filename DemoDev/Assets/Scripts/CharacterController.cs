using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

  public Animator animator;
  private Vector3 moveDirection;

  public float speed = 35.0f;
  public float h = 0.0f;
  public float v = 0.0f;

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

  }

  void FixedUpdate() {
    // The Inputs are defined in the Input Manager
    h = Input.GetAxis("Horizontal") * -1.0f; // get value for horizontal axis
    v = Input.GetAxis("Vertical") * -1.0f;   // get value for vertical axis

    transform.Translate(new Vector3(h * speed * Time.deltaTime, 0.0f, v * speed * Time.deltaTime));

    animator.SetFloat("Speed", speed);
    animator.SetFloat("Horizontal", h);
    animator.SetFloat("Vertical", v);

  }

}
