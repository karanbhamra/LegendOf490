using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {
    public float forwardSpeed = 12.0f;
    public float turnSpeed = 150.0f;
    public float strafeSpeed = 12.0f;
	public float jumpHeight;
	private Rigidbody rbody;
	private bool onGround;

    public float dashAmount = 120.0f;
    public float dashDamping = 2.0f;
    private int dashCount = 0;
    private float dashCooldown = 0f;

	private Slider healthSlider ;
	private bool gainHealth;
    
 
    //this runs once 
    void Start() {
		Application.targetFrameRate = 60;   // set max fps of 60
		onGround = true;
		jumpHeight = 8f;
		rbody = GetComponent<Rigidbody> ();
		healthSlider = GameObject.FindGameObjectWithTag("health").GetComponent<Slider>();


		gainHealth = false;
    }

    //this runs every frame
    void Update() {
        bool forwardPressed = Input.GetKey(KeyCode.W);
		bool attackPressed = Input.GetMouseButton(0);
		bool spacePressed = Input.GetKey (KeyCode.Space);
		bool backPressed = Input.GetKey (KeyCode.S);


		if (gainHealth && healthSlider.value < 100)
		{
			healthSlider.value += 0.25f;
			gainHealth = true;

		}
		else {
			gainHealth = false;
		}
    

		if (onGround) {
			// make char jump when space bar is pressed and char is on ground
			if (spacePressed) 
			{
				rbody.velocity = new Vector3 (0f, jumpHeight, 0f);
				onGround = false;
			}
		}

		if (forwardPressed || backPressed) {
            GameObject.Find("Player").GetComponent<Animation>().Play("Walk");

        }
		else if (attackPressed) { 
			GameObject.Find("Player").GetComponent<Animation>().Play("Attack");
			// showcase attack using mana or getting damaged by changing healthbar value
			//healthSlider.value -= 0.1f;
		}
		else {
            GameObject.Find("Player").GetComponent<Animation>().Play("Wait");
        }

        //Double tap e to dodge right, and double tap q to dodge left
        if (Input.GetKeyDown(KeyCode.E)) {
            if(dashCooldown > 0 && dashCount >= 1) {
                transform.Translate(dashAmount,0 ,0);
            } else {
                dashCooldown = 0.5f;
                dashCount += 1;
            }
        } else if(Input.GetKeyDown(KeyCode.Q)) {
            if(dashCooldown > 0 && dashCount >= 1) {
                 transform.Translate(-dashAmount, 0, 0);
            } else {
                dashCooldown = 0.5f;
                dashCount += 1;
            }
        }

        //Code to handle the reseting of the cooldown on the dash
        if(dashCooldown > 0) {
            dashCooldown -= 1 * Time.deltaTime;
        } else {
            dashCooldown = 0;
        }

        //Code to move the player forward, back,left,right,and strafe
        var rotationMovement = Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed;
        var forwardMovement = Input.GetAxis("Vertical") * Time.deltaTime * forwardSpeed;
        var horizontalMovement = Input.GetAxis("Strafe") * Time.deltaTime * forwardSpeed;

        transform.Rotate(0, rotationMovement, 0);
        transform.Translate(horizontalMovement, 0, forwardMovement);
    }

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag ("ground")) {
			onGround = true;
		}
		if (other.gameObject.tag == "enemy")
		{
			healthSlider.value -= 1.0f;
			gainHealth = false;
		}
	}

	void OnCollisionStay(Collision other) {
		if (other.gameObject.tag == "enemy")
		{
			healthSlider.value -= 1.0f;
			gainHealth = false;
		}	
	}

	void OnCollisionExit(Collision other) {
		if (other.gameObject.tag == "enemy")
		{
			healthSlider.value -= 1.0f;
			gainHealth = true;
		}
	}
}

