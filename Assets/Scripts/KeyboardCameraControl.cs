
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

using System.Collections.Generic;

public class KeyboardCameraControl : MonoBehaviour
{
	public float MaxDimension = 17.0f;
	public float MaxDimensionEnvironment = 28.0f;
	public float Center = 0.0f;


	GameObject _Target;
	bool _TargetActive = true;

	static float SLookSpeed = 5.0f;
	float _MoveSpeed = 0.2f;
	Vector3 _TargetPosition;

	float _RotationX = 0.0f;
	float _RotationY = 0.0f;

	
	public bool LockCursor = false;

	CursorLockMode wantedMode;

	// Apply requested cursor state
	void SetCursorState()
	{
		Cursor.lockState = wantedMode;
		// Hide cursor when locking
		Cursor.visible = (CursorLockMode.Locked != wantedMode);
	}

	void Start()
	{
		_Target = this.transform.GetChild(0).gameObject;
		MaxDimension /= 2;
	}

	void Update()
	{
		if (LockCursor)
		{
			if (Input.GetKeyDown("escape"))
			{ 
				LockCursor = !LockCursor;
				Cursor.lockState = wantedMode = CursorLockMode.None;
			}


			_TargetPosition = _Target.transform.position;
			_RotationX += Input.GetAxis("Mouse X") * SLookSpeed;
			_RotationY += Input.GetAxis("Mouse Y") * SLookSpeed;
			_RotationY = Mathf.Clamp(_RotationY, -90, 90);

			transform.localRotation = Quaternion.AngleAxis(_RotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis(_RotationY, Vector3.left);

			transform.position += transform.forward * _MoveSpeed * Input.GetAxis("Vertical");
			transform.position += transform.right *   _MoveSpeed * Input.GetAxis("Horizontal");
			if (!IsWithinCube())
			{
				if (_TargetActive)
				{
					_TargetActive = false;
					_Target.SetActive(_TargetActive);
				}
			}
			else
			{
				if (!_TargetActive)
				{
					_TargetActive = true;
					_Target.SetActive(_TargetActive);
				}
			}

			if (!IsWithinEnvironMent())
			{
				transform.position -= transform.forward * _MoveSpeed * Input.GetAxis("Vertical");
				transform.position -= transform.right * _MoveSpeed * Input.GetAxis("Horizontal");
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (Input.mousePosition.x < (Screen.width * (0.7f)))
				{ 
					LockCursor = !LockCursor;
					wantedMode = CursorLockMode.Locked;
				}
			}
		}

			SetCursorState();



	}

	bool IsWithinCube()
	{
		return
		(
			((_TargetPosition.x > (Center - MaxDimension)) && (_TargetPosition.x < (Center + MaxDimension))) &&
			((_TargetPosition.y > (Center - MaxDimension)) && (_TargetPosition.y < (Center + MaxDimension))) &&
			((_TargetPosition.z > (Center - MaxDimension)) && (_TargetPosition.z < (Center + MaxDimension)))
		);
	}


	bool IsWithinEnvironMent()
	{
		return
		(
			((this.transform.position.x > (-MaxDimensionEnvironment)) && (this.transform.position.x < (MaxDimensionEnvironment))) &&
			((this.transform.position.y > (-MaxDimensionEnvironment)) && (this.transform.position.y < (MaxDimensionEnvironment))) &&
			((this.transform.position.z > (-MaxDimensionEnvironment)) && (this.transform.position.z < (MaxDimensionEnvironment)))
		);
	}
	


}