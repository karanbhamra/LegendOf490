using UnityEngine;
using System.Collections;

public class InfoDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    private Vector3 camObject;
    private string playerObject;
    private bool enabled;

    void Start()
    {
        camObject = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        enabled = false;

    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        if (Input.GetKeyDown("p"))  // if user pressed P, toggle the debug info //can be changed later - Karan
        {
            enabled = !enabled;
        }


    }

    // used to draw text elements on screen
    void OnGUI()
    {
        if (enabled)
        {
            int w = Screen.width, h = Screen.height;
            camObject = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
            playerObject = GameObject.FindGameObjectWithTag("Player").transform.position.ToString();

            // fps info
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = h * 2 / 100;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.magenta;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            if (msec < 1) // otherwise fps output gets larger and larger // dirty fix
                fps = 0;
            string text = string.Format("{0:0.0} ms ({1:0.0} fps)", msec, fps);
            GUI.Label(rect, text, style);

            // player position info
            //		GUIStyle position = new GUIStyle ();
            Rect rect2 = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = h * 2 / 100;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.magenta;
            string posText = "\nPlayer position:\t" + playerObject + "\n" + "Camera position:\t" + camObject.ToString();
            GUI.Label(rect2, posText, style);
        }

    }
}