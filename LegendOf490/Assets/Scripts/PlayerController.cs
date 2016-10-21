using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 12.0f;
    public float turnSpeed = 150.0f;
    
 
    //this runs once 
    void Start()
    {

    }

    //this runs every frame
    void Update()
    {
        bool forwardPressed = Input.GetKeyDown(KeyCode.W);
        bool attackPressed = Input.GetMouseButtonDown(0);

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

}

