using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSData : MonoBehaviour  
{
	float deltaTime = 0.0f;
	public Text FPSDataText;
	

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}
 
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		FPSDataText.text = text;
		//GUI.Label(rect, text, style);
	}
}