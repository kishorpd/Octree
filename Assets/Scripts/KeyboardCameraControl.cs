
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

using System.Collections.Generic;

//[AddComponentMenu("Camera-Control/Keyboard")]
public class KeyboardCameraControl : MonoBehaviour
{
	static float lookSpeed = 5.0f;
	float moveSpeed = 0.2f;


	public struct myVec3
	{
		public float _X,_Y, _Z;

		public myVec3(Vector3 vec)
		{
			this._X = vec.x;
			this._Y = vec.y;
			this._Z = vec.z;
		}
		// Override the ToString method:
		public override string ToString()
		{
			return (String.Format("({0},{1},{2})", _X, _Y, _Z));
		}
	};

	float rotationX = 0.0f;
	float rotationY = 0.0f;

	[DllImport("OctreeT", EntryPoint = "TestDivide")]
	public static extern float StraightFromDllTestDivide(float a, float b);

	[DllImport("OctreeT", EntryPoint = "TestGameObj")]
	public static extern float TestGameObj(System.IntPtr aa);
	
	[DllImport("OctreeT", EntryPoint = "AddOne")]
	public static extern myVec3 AddOne(myVec3 vec);

	[DllImport("OctreeT", EntryPoint = "stayPersistant")]
	public static extern int stayPersistant;

	
	
	//============================
	System.IntPtr pnt = Marshal.AllocHGlobal(4);
                  
	void Start()
	{
		//pnt = (System.IntPtr)4.5f;
		float straightFromDllDivideResult = StraightFromDllTestDivide(20, 5);
		// Print it out to the console
		Debug.Log("First output from dll:" + straightFromDllDivideResult);
		Vector3 ab = new Vector3(2,3,4);
		myVec3 mv = new myVec3(ab);
		Debug.Log("AddOne:" + AddOne(mv));
		Debug.Log("stayPersistant:" + stayPersistant++);
	//	Debug.Log("Null or not:" + TestGameObj(pnt));
		Debug.Log("stayPersistant:" + stayPersistant++);

	}

	void Update()
	{
		rotationX += Input.GetAxis("Mouse X") * lookSpeed;
		rotationY += Input.GetAxis("Mouse Y") * lookSpeed;
		rotationY = Mathf.Clamp(rotationY, -90, 90);

		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

		transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical");
		transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal");
	}


	//myVec3 vectorToMyVec3(Vector3 vec)
	//{ 
	//	myVec3 loc = new myVec3();
	//	loc._X = vec.x;
	//}
}