using UnityEngine;
using System.Collections;

public class MainMenuChase : MonoBehaviour {
    private float min;
    private float max;

    public Transform rightTarget;
    public Transform leftTarget;
    // Use this for initialization
    void Start () {
        min = transform.position.x;
        max = transform.position.x + 100;
    }
	
	// Update is called once per frame
	void Update () {
      
        if (transform.position.x >90)
        {
            transform.LookAt(leftTarget);
        } 
        if(transform.position.x < 10)
        {
            transform.LookAt(rightTarget);
        }

        transform.position = new Vector3(Mathf.PingPong(Time.time * 10, max-min), transform.position.y, transform.position.z);

    }
}
