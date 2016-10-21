using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 12.0f;
    public float turnSpeed = 150.0f;
	private float jumpPressure;
	private float minJump;
	private float maxJumpPressure;
	private Rigidbody rbody;

	private bool onGround;
    
 
    //this runs once 
    void Start()
    {
		onGround = true;
		jumpPressure = 0f;
		minJump = 8f;
		maxJumpPressure = 10f;
		rbody = GetComponent<Rigidbody> ();

    }

    //this runs every frame
    void Update()
    {
        bool forwardPressed = Input.GetKeyDown(KeyCode.W);
        bool attackPressed = Input.GetMouseButtonDown(0);

		if (onGround) {
			// holding space bar
			if (Input.GetButton ("Jump"))
			{
				if (jumpPressure < maxJumpPressure)
				{
					jumpPressure += Time.deltaTime * 10f;
				}
				else {
					jumpPressure = maxJumpPressure;
				}
				//print (jumpPressure);
			}
			else 	// not holding space bar
			{
				if (jumpPressure > 0f)
				{
					jumpPressure = jumpPressure + minJump;
					rbody.velocity = new Vector3 (0f, jumpPressure, 0f);
					jumpPressure = 0;
					onGround = false;
				}
				
			}
		}

        if (forwardPressed)
        {
            GameObject.Find("Player").GetComponent<Animation>().Play("Walk");
        } else if (attackPressed)
        {
            GameObject.Find("Player").GetComponent<Animation>().Play("Attack");
        }
        else {
            //play wait animation

        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * forwardSpeed;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag ("ground"))
		{
			onGround = true;
		}
	}

}

