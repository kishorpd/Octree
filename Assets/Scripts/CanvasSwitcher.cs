using UnityEngine;
using System.Collections;

public class CanvasSwitcher : MonoBehaviour {

	public GameObject InteractCanvasObject;
	public GameObject DataCanvasObject;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void InteractCanvas()
	{
		InteractCanvasObject.SetActive(true);
		DataCanvasObject.SetActive(false);
	}

	public void DataCanvas()
	{
		InteractCanvasObject.SetActive(false);
		DataCanvasObject.SetActive(true);
	}
}
