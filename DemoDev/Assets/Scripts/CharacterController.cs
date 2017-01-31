using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

  public Animator animator;
  private Vector3 moveDirection;
  public GameObject enemy;

  public float dashAmount = 15.0f;
  public float dashDamping = 2.0f;
  private int dashCount = 0;
  private float dashCooldown = 0f;

  public float speed = 0.0f;
  public float h = 0.0f;
  public float v = 0.0f;
  public float tiltAngleY;
  public float damage = 20.0f;

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
      if (GameObject.FindWithTag("enemy") != null) {
        float distance = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, GameObject.FindWithTag("enemy").transform.position);
        Vector3 targetDir = GameObject.FindWithTag("enemy").transform.position - GameObject.FindWithTag("Player").transform.position;
        Vector3 forward = GameObject.FindWithTag("Player").transform.forward;
        float angle = Vector3.Angle(targetDir, forward);
        if (distance < 5.0 && angle < 60.0) {
          //was distance 40.0. This allowed long range with sword. Not practical
          Attack(damage);
        }
        // showcase attack using mana or getting damaged by changing healthbar value
        //healthSlider.value -= 0.1f;
      }
    }
    basicDash();
    // The Inputs are defined in the Input Manager
    if(h != 0.0f || v != 0.0f) {
      speed += 0.2f;
    }else {
      speed = 0.0f;
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
  void Attack(float damage) {
    enemy = GameObject.FindWithTag("enemy");
    EnemyStats stats = enemy.GetComponent<EnemyStats>();
    //stats.ReceiveDamage(damage);
    DestroyObject(enemy);
  }

    void basicDash()
    {
        //Double tap e to dodge right, and double tap q to dodge left
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentStroke != KeyCode.D)
            {
                currentStroke = KeyCode.D;
                dashCount = 0;
            }
            if (dashCooldown > 0 && dashCount >= 1)
            {
                transform.position = new Vector3(transform.position.x + dashAmount, transform.position.y, transform.position.z);
            }
            else
            {
                dashCooldown = 0.5f;
                dashCount += 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentStroke != KeyCode.A)
            {
                currentStroke = KeyCode.A;
                dashCount = 0;
            }
            if (dashCooldown > 0 && dashCount >= 1)
            {
                transform.position = new Vector3(transform.position.x - dashAmount, transform.position.y, transform.position.z);
            }
            else
            {
                dashCooldown = 0.5f;
                dashCount += 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentStroke != KeyCode.S)
            {
                currentStroke = KeyCode.S;
                dashCount = 0;
            }
            if (dashCooldown > 0 && dashCount >= 1)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - dashAmount);
            }
            else
            {
                dashCooldown = 0.5f;
                dashCount += 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentStroke != KeyCode.W)
            {
                currentStroke = KeyCode.W;
                dashCount = 0;
            }
            if (dashCooldown > 0 && dashCount >= 1)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + dashAmount);
            }
            else
            {
                dashCooldown = 0.5f;
                dashCount += 1;
            }
        }

        //Code to handle the reseting of the cooldown on the dash
        if (dashCooldown > 0)
        {
            dashCooldown -= 1 * Time.deltaTime;
        }
        else
        {
            dashCooldown = 0;
        }
    }
}
