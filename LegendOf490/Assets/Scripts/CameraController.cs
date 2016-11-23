using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float turnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
    public float zoomSpeed = 4.0f;

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private Vector3 offset;
    private bool isRotating;    // Is the camera being rotated?
    private bool isZooming;        //are we zooming?
    private float scrollValue;
    public GameObject target;   //the player
	public float cameraDistance = 30f;

    void Start() {
		target = GameObject.FindGameObjectWithTag ("user");
     
    }

    void Update() {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        isRotating = Input.GetMouseButton(1);
        scrollValue = Input.GetAxis("Mouse ScrollWheel");


        if (scrollValue != 0)
            isZooming = true;
        // Disable movements on button release
        if (scrollValue == 0)
            isZooming = false;

        if (!forwardPressed) {
            // Get the right mouse button
            if (Input.GetMouseButtonDown(1)) {
                // Get mouse origin
                mouseOrigin = Input.mousePosition;
                isRotating = true;
            }

            // Rotate camera along X and Y axis
            if (isRotating) {
                rotateAroundPlayer();
            }
            if (isZooming) {
                zoomFromPlayer();
            }
        } else {
            cameraFollowPlayer();
        }
        if ((!isRotating && !isZooming)) {
            cameraFollowPlayer();
        }
       
    }

    public void rotateAroundPlayer() {
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

        transform.RotateAround(target.transform.position, transform.right, -pos.y * turnSpeed);
        transform.RotateAround(target.transform.position, Vector3.up, pos.x * turnSpeed);
        
    }

    public void zoomFromPlayer() {
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

        if (scrollValue > 0) {
            Vector3 move = pos.y * zoomSpeed * transform.forward;
            transform.Translate(move, Space.World);
        }
        else {
            Vector3 move = pos.y * zoomSpeed * transform.forward * -1;
            transform.Translate(move, Space.World);
        }
    }

    public void cameraFollowPlayer() {
		float distance = cameraDistance;
        // the height we want the camera to be above the target
        float height = 20.0f;

        float heightDamping = 2.0f;
        float rotationDamping = 2.0f;


        // Calculate the current rotation angles
        float wantedRotationAngle = target.transform.eulerAngles.y;
        float wantedHeight = target.transform.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
       
        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = target.transform.position;
        transform.position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        Camera.main.transform.LookAt(target.transform.GetChild(0));

    }
}