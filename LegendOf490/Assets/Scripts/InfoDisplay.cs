using UnityEngine;
using System.Collections;
using UnityEditor;

public class InfoDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
	private Vector3 camObject;

	void Start()
	{
		//camObject = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
	}

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	// used to draw text elements on screen
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		camObject = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;


		// fps info
		GUIStyle style = new GUIStyle();
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		if (msec < 1) // otherwise fps output gets larger and larger // dirty fix
			fps = 0;
		string text = string.Format("{0:0.0} ms ({1:0.0} fps)", msec, fps);
		GUI.Label(rect, text, style);

		// player position info
		GUIStyle position = new GUIStyle ();
		Rect rect2 = new Rect(0,0,w,h*2/100);
		style.alignment = TextAnchor.UpperRight;
		style.fontSize = h*2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		string posText = "Player position:\t" + transform.position.ToString () + "\n" + "Camera position:\t" + camObject.ToString () ;
		GUI.Label (rect2,posText,style);

	}
}