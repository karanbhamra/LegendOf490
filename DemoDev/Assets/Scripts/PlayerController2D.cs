using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerController2D : MonoBehaviour {

    public float movementSpeed = 12.0f;
  

    private Rigidbody rbody;
   
    public float dashAmount = 120.0f;
    public float dashDamping = 2.0f;
    private int dashCount = 0;
    private float dashCooldown = 0f;

    private bool canPause;
    private Slider healthSlider;
    private bool gainHealth;
	  EnemyStats enemyStatsScript;
    bool upPressed;
    bool downPressed;
    bool rightPressed;
    bool leftPressed ;
    bool attackPressed;
	  KeyCode currentStroke;


    //this runs once 
    void Start()
    {
		currentStroke = KeyCode.UpArrow;

        Application.targetFrameRate = 60;   // set max fps of 60
		QualitySettings.antiAliasing = 4;	// antialiasing multisampling set to 4x
    
        rbody = GetComponent<Rigidbody>();
        healthSlider = GameObject.FindGameObjectWithTag("HealthUI").GetComponent<Slider>();

        canPause = true;

        gainHealth = false;
    }

    //this runs every frame
    void Update()
    {
        upPressed = Input.GetKey(KeyCode.W);
        downPressed = Input.GetKey(KeyCode.S);
        rightPressed = Input.GetKey(KeyCode.D);
        leftPressed = Input.GetKey(KeyCode.A);
        attackPressed = Input.GetMouseButton(0);
 
//        if (gainHealth && healthSlider.value < 100)
//        {
//            healthSlider.value += 0.25f;
//            gainHealth = true;
//
//        }
//        else
//        {
//            gainHealth = false;
//        }

        if (upPressed || downPressed || rightPressed || leftPressed)
        {
            GameObject.Find("Player").GetComponent<Animation>().Play("Walk");

        }
        else if (attackPressed)
        {
            GameObject.Find("Player").GetComponent<Animation>().Play("Attack");
            if (GameObject.Find("Slime") != null) {
                float distance = Vector3.Distance(GameObject.Find("Player").transform.position, GameObject.Find("Slime").transform.position);
                Vector3 targetDir = GameObject.Find("Slime").transform.position - GameObject.Find("Player").transform.position;
                Vector3 forward = GameObject.Find("Player").transform.forward;
                float angle = Vector3.Angle(targetDir, forward);
                if (distance < 40.0 && angle < 60.0)
                {
                    BasicAttack();
                }
                // showcase attack using mana or getting damaged by changing healthbar value
                //healthSlider.value -= 0.1f;
            }

        }
        else
        {
            GameObject.Find("Player").GetComponent<Animation>().Play("Wait");
        }


        // when escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // pause
            if (canPause)
            {
                print("paused");
                Time.timeScale = 0;
                canPause = false;
            }
            else
            {  // unpause
                Time.timeScale = 1;
                canPause = true;
            }
        }

        basicDash();
      
        //Code to move the player forward, back,left,right

        float moveHorizontal = Input.GetAxisRaw("Horizontal") * Time.deltaTime * movementSpeed;
        float moveVertical = Input.GetAxisRaw("Vertical") * Time.deltaTime * movementSpeed; 

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (movement != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement.normalized), 0.2f);

        transform.Translate(movement , Space.World);
    }//end update
    
    void basicDash()
    {
        //Double tap e to dodge right, and double tap q to dodge left
        if (Input.GetKeyDown(KeyCode.D))
        {
            if(currentStroke != KeyCode.D)
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

    void BasicAttack()
    {
        enemyStatsScript = GameObject.Find("Slime").GetComponent<EnemyStats>();
        enemyStatsScript.ReceiveDamage(10);
    }
//
//    void OnCollisionEnter(Collision other)
//    {
//      
//        if (other.gameObject.tag == "enemy")
//        {
//            healthSlider.value -= 1.0f;
//            gainHealth = false;
//        }
//    }
//
//    void OnCollisionStay(Collision other)
//    {
//        if (other.gameObject.tag == "enemy")
//        {
//            healthSlider.value -= 1.0f;
//            gainHealth = false;
//        }
//    }
//
//    void OnCollisionExit(Collision other)
//    {
//        if (other.gameObject.tag == "enemy")
//        {
//            healthSlider.value -= 1.0f;
//            gainHealth = true;
//        }
//    }
}
