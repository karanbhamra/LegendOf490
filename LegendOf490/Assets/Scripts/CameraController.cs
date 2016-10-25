using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    //
    // VARIABLES
    //

    public float turnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
    public float zoomSpeed = 4.0f;

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isRotating;    // Is the camera being rotated?
    private bool isZooming;        //are we zooming?
    private float scrollValue;
    public GameObject target;
    //
    // UPDATE
    //

    void Update()
    {
        scrollValue = Input.GetAxis("Mouse ScrollWheel"); 

        // Get the right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }

        if(scrollValue != 0)
            isZooming = true;
        

        // Disable movements on button release

        if (!Input.GetMouseButton(1))
            isRotating = false;

        if(scrollValue == 0)
            isZooming = false;
        

        // Rotate camera along X and Y axis
        if (isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            transform.RotateAround(target.transform.position, transform.right, -pos.y * turnSpeed);
            transform.RotateAround(target.transform.position, Vector3.up, pos.x * turnSpeed);
        }
        if (isZooming)
        {
           
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            if (scrollValue > 0)
            {
                Vector3 move = pos.y * zoomSpeed * transform.forward;
                transform.Translate(move, Space.World);
            } else
            {
                Vector3 move = pos.y * zoomSpeed * transform.forward *-1 ;
                transform.Translate(move, Space.World);
            }
           
        }

        
    }
}