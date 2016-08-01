using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawFrustum : MonoBehaviour {


	public List<GameObject> FarPlane;
	public List<GameObject> NearPlane;
	static public List<GameObject> AllPoints;
	static public List<Vector3> STraversalLines;
	static public List<Vector3> SSnapLines;
	static List<GameObject> SFarPlane;
	static List<GameObject> SNearPlane;
	public float Offset = 0.3f;
	Vector3 tempPos;

	// Use this for initialization
	void Start () {
		AllPoints = new List<GameObject>();
		STraversalLines = new List<Vector3>();
		SSnapLines = new List<Vector3>();
		tempPos = new Vector3();
		SFarPlane = FarPlane;
		SNearPlane = NearPlane;
		foreach (GameObject point in SNearPlane)
			AllPoints.Add(point);
		foreach (GameObject point in SFarPlane)
			AllPoints.Add(point);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.R))
		{
			for (int i = 0; i < 4; ++i)
			{
				tempPos = SFarPlane[i].transform.localPosition;
				tempPos.z += Offset;
				SFarPlane[i].transform.localPosition = tempPos;
			}
		}

		if (Input.GetKeyDown(KeyCode.T))
		{
			for (int i = 0; i < 4; ++i)
			{
				tempPos = SFarPlane[i].transform.localPosition;
				tempPos.z -= Offset;
				SFarPlane[i].transform.localPosition = tempPos;
			}
		}

	}

	static public void DrawWireframe()
	{
		GL.Color(Color.white);
		for (int i = 0; i < 4; ++i)
		{
			GL.Vertex(SNearPlane[i].transform.position);
			GL.Vertex(SNearPlane[i + 1].transform.position);
			GL.Vertex(SFarPlane[i].transform.position);
			GL.Vertex(SFarPlane[i + 1].transform.position);

			//lines in between of two planes
			GL.Vertex(SNearPlane[i].transform.position);
			GL.Vertex(SFarPlane[i].transform.position);
		}

		if (MainInstance.SShowTraversal)
			DrawHierarchy();
		
		if (MainInstance.SShowSnap)
			DrawSnap();

		STraversalLines.Clear();
		SSnapLines.Clear();
	}

	static void DrawHierarchy()
	{
		GL.Color(Color.black);
		foreach (Vector3 point in STraversalLines)
		{
			GL.Vertex(point);
		}
	}

	static void DrawSnap()
	{
		GL.Color(Color.yellow);
		foreach (Vector3 point in SSnapLines)
		{
			GL.Vertex(point);
		}
	}
}
