using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class MainInstance : MonoBehaviour {

	//____public
	public GameObject ParticlePrefab;
	public GameObject VertexPrefab;
	public GameObject ParticleSpawner;
	public Camera GLCamera;
	public Text TextBox;
	public Text TextBoxCurrentMaxDepth;
	public Text TextBoxCurrentSplitAt;
	public InputField InputMaxDepth;
	public InputField InputMaxSplitAt;
	public Slider InputMaxDepthSlider;
	public Slider InputSplitAtSlider;


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
	private bool _UpdateDepth = false;
	private bool _UpdateSplitAt = false;
	private bool _Clear = false;


	private int _NewMaxDepth = 0;
	private int _NewSplitAt = 0;


	enum CursorMode
	{
		NORMAL,
		SPAWN_MULTIPLE
	}

	CursorMode _CursorMode = CursorMode.NORMAL;

	KeyboardCameraControl _KeyboardCameraControl;
	GridOverlay _GridOverlay;
	Ray myRay;      // initializing the ray
	RaycastHit hit; // initializing the raycasthit
	bool _DisplayVertices = true;
	bool _ShowTextMesh = false;

	// Use this for initialization
	void Start () {
		_Particles = new List<GameObject>();
		_Vertices = new List<GameObject>();
		_GridOverlay = GLCamera.GetComponent<GridOverlay>();
		_KeyboardCameraControl = GLCamera.GetComponent<KeyboardCameraControl>();
		_GridOverlay._MainInstance = this;
		CubeCenter = _GridOverlay.CubeCenter;
		CubeWidth = _GridOverlay.CubeDimension;
		//RootOcTree = new Octree(CubeCenter, CubeWidth, _FParticleRadius);
		RootOcTree = null;
		float _FParticleRadius = ParticlePrefab.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x / 2;
		_VParticleRadius = new Vector3(_FParticleRadius, _FParticleRadius, _FParticleRadius);
		TextBoxCurrentMaxDepth.text = "Depth : " + Octree.SMaxDepthToStopAt;
		TextBoxCurrentSplitAt.text = "Split after : " + Octree.SMaxChildren;
		InputMaxDepth.text = Octree.SMaxDepthToStopAt.ToString();
		InputMaxSplitAt.text = Octree.SMaxChildren.ToString();

		ParticlePrefab.transform.GetChild(2).gameObject.SetActive(_ShowTextMesh);
		ParticlePrefab.transform.GetChild(3).gameObject.SetActive(_ShowTextMesh);

	}
	
	// Update is called once per frame
	void Update () {

		float _FParticleRadius = ParticlePrefab.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x / 2;

		if (RootOcTree != null)
		{
			RootOcTree.Clear();
		}

		RootOcTree = null;

		if (_Clear)
		{
			_Clear = false;
			foreach (GameObject particleObj in _Particles)
			{ 
				Destroy(particleObj);
			}

			_Particles.Clear();
		}

		RootOcTree = new Octree(CubeCenter, CubeWidth, _FParticleRadius, this);

		if (_UpdateDepth)
		{
			_UpdateDepth = false;
			Octree.SMaxDepthToStopAt = Convert.ToInt32(InputMaxDepthSlider.value * _NewMaxDepth);
			TextBoxCurrentMaxDepth.text = "Depth : " + Octree.SMaxDepthToStopAt;
		}

		if (_UpdateSplitAt)
		{
			_UpdateSplitAt = false;
			_NewSplitAt -= 1;
			Octree.SMaxChildren = Convert.ToInt32(InputSplitAtSlider.value * _NewSplitAt) + 1;
			TextBoxCurrentSplitAt.text = "Split after : " + (Octree.SMaxChildren);
			//Octree.SMaxChildren += 1;
		}

		foreach (GameObject obj in _Particles)
		{
			RootOcTree.Insert(obj);
		}

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
							_CursorMode = CursorMode.SPAWN_MULTIPLE;
						}
					break;
				}

			case CursorMode.SPAWN_MULTIPLE:
				{
					if ((_Paint) ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
					{
						//Debug.Log("SPAWNed IT!!!!");
						//instantiate and add in the list
						Vector3 positionToSpawn = ParticleSpawner.transform.position;
						GameObject ParticleObject;
						float tempWidth = CubeWidth.x;
						for (int i = -10; i < 10; ++i)
						{
							ParticleObject = Instantiate(ParticlePrefab, new Vector3( UnityEngine.Random.Range(-tempWidth, tempWidth), UnityEngine.Random.Range(-tempWidth, tempWidth), UnityEngine.Random.Range(-tempWidth, tempWidth)), Quaternion.identity) as GameObject;
							_Particles.Add(ParticleObject);
							RootOcTree.Insert(ParticleObject);
						}

					}

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
		long size = 0;
		
		using (Stream s = new MemoryStream())
		{
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(s, RootOcTree);
			size = s.Length;
		}

		TextBox.text = " Total Particles:" + RootOcTree.TotalChildren +
			"\n Total Size : " + size +
			"\n Maximum Depth : " + Octree.SMaxDepthReached +
			"\n Particles on boundary : " + Octree.STotalOverLapping +
			"\n Split at : " + Octree.SMaxChildren;

		RootOcTree.DisplayInfoOfEachParticle();
	}

	public void Clear()
	{
		_Clear = true;
		//Application.LoadLevel(0);
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
		
		//foreach (GameObject particleObj in _Particles)
		//{
		//	_GridOverlay.DrawCube(particleObj.transform.position, _VParticleRadius);
		//	_GridOverlay.DrawPartitioners(particleObj.transform.position, _VParticleRadius);
		//}
	}

	public void DrawOctreeHierarchy()
	{
		_GridOverlay.SetColor(Color.green);
		RootOcTree.DrawTree(_GridOverlay);
		_GridOverlay.SetColor(Color.blue);
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
		_DisplayVertices = !_DisplayVertices;
		foreach (GameObject particleObj in _Vertices)
		{
			particleObj.gameObject.SetActive(_DisplayVertices);
		}
	}

	public void DisplayPartitions()
	{
		_GridOverlay.ShowPartitions = !_GridOverlay.ShowPartitions;
	}

	public void DisplayHierarchy()
	{
		_GridOverlay.ShowHierarchy = !_GridOverlay.ShowHierarchy;

	}

	public void DisplayMeshText()
	{
		_ShowTextMesh = !_ShowTextMesh;
		
		foreach (GameObject particleObj in _Particles)
		{

			particleObj.transform.GetChild(2).gameObject.SetActive(_ShowTextMesh);
			particleObj.transform.GetChild(3).gameObject.SetActive(_ShowTextMesh);
		}
		ParticlePrefab.transform.GetChild(2).gameObject.SetActive(_ShowTextMesh);
		ParticlePrefab.transform.GetChild(3).gameObject.SetActive(_ShowTextMesh);
	}


	public void MoveParticles()
	{
		ParticleMove.ToggleMove();
	}

	public void ChangeMaxDepth()
	{
		_UpdateDepth = true;
		_NewMaxDepth = Convert.ToInt32(InputMaxDepth.text);
		if (_NewMaxDepth < 0)
		{
			_NewMaxDepth = 0;
			InputMaxDepth.text = "" + _NewMaxDepth;
		}
	}

	public void ChangeSplitAt()
	{
		_UpdateSplitAt = true;
		_NewSplitAt = Convert.ToInt32(InputMaxSplitAt.text);
		InputMaxSplitAt.text = "" + (_NewSplitAt + 1);

		if (_NewSplitAt < 1)
		{
			_NewSplitAt = 2;
			InputMaxSplitAt.text = "" + _NewSplitAt;
		}

	}
	
	public void ChangeSplitWithSlider()
	{
		_UpdateSplitAt = true;
		_NewSplitAt = Convert.ToInt32(InputMaxSplitAt.text);

		if (_NewSplitAt < 1)
		{
			_NewSplitAt = 2;
			InputMaxSplitAt.text = "" + _NewSplitAt;
		}

	}
}
