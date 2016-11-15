using UnityEngine;
using System.Collections;

public class MainMenuChase : MonoBehaviour {
    public float min;
    public float max;

    public Transform rightTarget;
    public Transform leftTarget;
    // Use this for initialization
    void Start () {
        min = transform.position.x;
        max = transform.position.x + 80;
    }
	
	// Update is called once per frame
	void Update () {
        if(transform.position.x == max)
        {
            transform.LookAt(leftTarget);
        } 
        if(transform.position.x == min)
        {
            transform.LookAt(rightTarget);
        }
        transform.position = new Vector3(Mathf.PingPong(Time.time * 20, max - min) + min, transform.position.y, transform.position.z);
       

    }
}
