using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;


public class MainInstance : MonoBehaviour {

	//____public
	public GameObject ParticlePrefab;
	public GameObject VertexPrefab;
	public GameObject ParticleSpawner;
	public Camera GLCamera;
	public Text TextBox;


	Vector3 _VParticleRadius;
	float Center = 0.0f;
	float MaxDimension = 17.0f;
	Vector3 CubeCenter;
	Vector3 PartitionerCenter;
	Vector3 CubeWidth;

	public Octree RootOcTree;

	//___private
	private List<GameObject> _Particles;
	private List<GameObject> _Vertices;
	private bool _Paint = false;

	enum CursorMode
	{
		NORMAL,
		DRAGGING
	}

	CursorMode _CursorMode = CursorMode.NORMAL;

	KeyboardCameraControl _KeyboardCameraControl;
	GridOverlay _GridOverlay;
	Ray myRay;      // initializing the ray
	RaycastHit hit; // initializing the raycasthit
	bool DisplayVertices = true;

	// Use this for initialization
	void Start () {
		_Particles = new List<GameObject>();
		_Vertices = new List<GameObject>();
		_GridOverlay = GLCamera.GetComponent<GridOverlay>();
		_KeyboardCameraControl = GLCamera.GetComponent<KeyboardCameraControl>();
		_GridOverlay._MainInstance = this;
		CubeCenter = _GridOverlay.CubeCenter;
		CubeWidth = _GridOverlay.CubeDimension;
		float _FParticleRadius = ParticlePrefab.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x / 2;
		RootOcTree = new Octree(CubeCenter, CubeWidth, _FParticleRadius, this);
		//RootOcTree = new Octree(CubeCenter, CubeWidth, _FParticleRadius);
		_VParticleRadius = new Vector3(_FParticleRadius, _FParticleRadius, _FParticleRadius);

	}
	
	// Update is called once per frame
	void Update () {
		if (_KeyboardCameraControl.LockCursor)
		{ 
			if (ParticleSpawner.activeSelf)
			SpawnParticleOnClick();
		}

		UpdateText();
	}


	void SpawnParticleOnClick()
	{
		//if (Input.GetKey(KeyCode.Mouse0))
		switch (_CursorMode)
		{

			case CursorMode.NORMAL:
				{
						if ((_Paint) ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
						{
							//Debug.Log("SPAWNed IT!!!!");
							//instantiate and add in the list
							Vector3 positionToSpawn = ParticleSpawner.transform.position;
							GameObject ParticleObject;
							ParticleObject = Instantiate(ParticlePrefab, new Vector3(positionToSpawn.x, positionToSpawn.y, positionToSpawn.z), Quaternion.identity) as GameObject;
							//Debug.Log("ParticleObject.transform.position: " + ParticleObject.transform.position);
							_Particles.Add(ParticleObject);

							RootOcTree.Insert(ParticleObject);
						
						}
						if (Input.GetMouseButtonUp(1))
						{
							//TODO: fix right click at appropriate time
							//ClearQuadtree();
							//_QuadTree.ParticleUnderCursor(hit.point);
							// 
							Debug.Log("CHANGEDS!!!!");
							_CursorMode = CursorMode.DRAGGING;
						}
					break;
				}

			case CursorMode.DRAGGING:
				{
					

					if (Input.GetMouseButtonUp(1))
					{
						// 
						_CursorMode = CursorMode.NORMAL;

					}

					break;
				}

		}//switch end
	}


	void UpdateText()
	{
		TextBox.text = " Total Particles:" + RootOcTree.TotalChildren +
			"\n Maximum Depth : " + Octree.SMaxDepthReached +
			"\n Particles on boundary : " + Octree.STotalOverLapping +
			"\n Split at : " + Octree.SMaxChildren;

		RootOcTree.DisplayInfoOfEachParticle();
	}

	public void Clear()
	{
		Application.LoadLevel(0);
	}

	public void SpawnVertex(Vector3 positionToSpawn)
	{
		//Instantiate(VertexPrefab, new Vector3(positionToSpawn.x, positionToSpawn.y, positionToSpawn.z), Quaternion.identity);
		
		_Vertices.Add(Instantiate(VertexPrefab, new Vector3(positionToSpawn.x, positionToSpawn.y, positionToSpawn.z), Quaternion.identity) as GameObject);
	}

	public void DrawParticlesDebug()
	{
		foreach (GameObject particleObj in _Particles)
		{
			_GridOverlay.DrawCube(particleObj.transform.position, _VParticleRadius);
			_GridOverlay.DrawPartitioners(particleObj.transform.position, _VParticleRadius);
		}
	}

	public void DrawOctreePartitions()
	{
		_GridOverlay.SetColor(Color.cyan);
		RootOcTree.DrawPartitions(_GridOverlay);
		_GridOverlay.SetColor(Color.green);
		RootOcTree.DrawTree(_GridOverlay);
		_GridOverlay.SetColor(Color.blue);

		//foreach (GameObject particleObj in _Particles)
		//{
		//	_GridOverlay.DrawCube(particleObj.transform.position, _VParticleRadius);
		//	_GridOverlay.DrawPartitioners(particleObj.transform.position, _VParticleRadius);
		//}
	}

	public void SwapSphereVsSprite()
	{
		bool isSprite = _Particles[0].transform.GetChild(0).gameObject.activeSelf;
		foreach (GameObject particleObj in _Particles)
		{

			particleObj.transform.GetChild(0).gameObject.SetActive(!isSprite);
			particleObj.transform.GetChild(1).gameObject.SetActive(isSprite);
		}
		ParticlePrefab.transform.GetChild(0).gameObject.SetActive(!isSprite);
		ParticlePrefab.transform.GetChild(1).gameObject.SetActive(isSprite);
	}	

	public void ToggleVertices()
	{
		DisplayVertices = !DisplayVertices;
		foreach (GameObject particleObj in _Vertices)
		{
			particleObj.gameObject.SetActive(DisplayVertices);
		}
	}

	public void DisplayPartitions()
	{
		_GridOverlay.ShowPartitions = !_GridOverlay.ShowPartitions;
	}

}
