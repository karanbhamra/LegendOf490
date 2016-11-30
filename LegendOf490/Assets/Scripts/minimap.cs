using UnityEngine;
using System.Collections;

public class minimap: MonoBehaviour
{

    // target will be the user selected in the inspector panel
    public GameObject target;// = GameObject.FindGameObjectWithTag ("user");
    // camheight is how high the camera is looking down from
    public float camHeight = 200.0f;

    // Use this for initialization
    void Start()
    {
        //target = GameObject.FindGameObjectWithTag ("user");
        transform.position = new Vector3(target.transform.position.x, camHeight, target.transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        // follow player on the x and z pos with a static height, no rotations
        transform.position = new Vector3(target.transform.position.x, camHeight, target.transform.position.z);


    }
}
