using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ThisLookAtCamera();
	}

	void ThisLookAtCamera()
	{

			{
				this.transform.LookAt(Camera.main.transform);
				//Debug.Log("TYPE: " + GetComponent<Pipe>().GetPipeType().ToString());

				//corrects the flipped text
				this.transform.Rotate(Vector3.up - new Vector3(0, 180, 0));

			}

	}
}
