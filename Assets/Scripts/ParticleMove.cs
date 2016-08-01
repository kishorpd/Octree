using UnityEngine;
using System.Collections;

public class ParticleMove : MonoBehaviour
{

	public int radius = 5;
	public Color color = Color.blue;
	public int XRangeMin = -8;
	public int YRangeMin = -8;
	public int ZRangeMin = -8;
	public int XRangeMax =  8;
	public int YRangeMax =  8;
	public int ZRangeMax =  8;
	 
	public bool goUp = true;
	public bool goRight = true;
	public bool goIn = true;

	public float MinSpeed = 0.001f;
	public float MaxSpeed = 0.2f;
	float positionX = 1.5f;
	float positionY = 1.5f;
	float positionZ = 1.5f;
	float speed = 0.0f;
	float stepX = 0.0f;
	float stepY = 0.0f;
	float stepZ = 0.0f;

	static bool _S_ToMove = false;
	public Vector3 position;
	Vector3 scale;

	// Use this for initialization
	void Start()
	{
		// setPosition();
		//setRadius();
		setDirection();

	}

	static public void ToggleMove()
	{
		_S_ToMove = !_S_ToMove;
	}

	// Update is called once per frame
	void Update()
	{
		if (_S_ToMove)
			move();
	}

	// Update is called once per frame
	void setPosition()
	{
		//set random position
		position.x = (float)Random.Range(XRangeMin, XRangeMax);
		position.y = (float)Random.Range(YRangeMin, YRangeMax);
		position.z = (float)Random.Range(ZRangeMin, ZRangeMax);
		transform.position = position;
	}

	void setRadius()
	{
		//set scale
		scale.x = radius;
		scale.y = radius;
		scale.z = radius;
		transform.localScale = scale;
	}

	void setDirection()
	{
		//set random direction
		int angle = Random.Range(0, 360);
		speed = Random.Range(MinSpeed, MaxSpeed);

		stepX = Mathf.Abs(speed * Mathf.Sin(angle));
		stepY = Mathf.Abs(speed * Mathf.Cos(angle));
		stepZ = (stepX + stepY) / 2;// Mathf.Abs(speed * Mathf.Tan(angle));

	}

	/*
	void setColor()
	{
		//set random color
		//Create random color
		Color col1 = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
		Mesh mesh1 = GetComponent<MeshFilter>().mesh;

		//Change colors of meshes
		Vector3[] vertices = mesh1.vertices;
		Color[] colors = new Color[vertices.Length];

		for (int i = 0; i < vertices.Length; i++)
		{
			colors[i] = col1;
			colors[i] = color;
		}
		mesh1.colors = colors; //Set new colors of vertices
	}
	 */

	void move()
	{
		positionX = transform.position.x;
		positionY = transform.position.y;
		positionZ = transform.position.z;

		//handle X
		if (positionX > XRangeMax)
		{
			goRight = false;
		}

		if (positionX < XRangeMin)
		{
			goRight = true;
		}

		if (goRight == true)
		{
			position.x = stepX;
		}
		else
		{
			position.x = -stepX;
		}

		//handle Y
		if (positionY > YRangeMax)
		{
			goUp = false;
		}

		if (positionY < YRangeMin)
		{
			goUp = true;
		}

		if (goUp == true)
		{
			position.y = stepY;
		}
		else
		{
			position.y = -stepY;
		}


		//handle Z
		if (positionZ > ZRangeMax)
		{
			goIn = false;
		}

		if (positionZ < ZRangeMin)
		{
			goIn = true;
		}

		if (goIn == true)
		{
			position.z = stepZ;
		}
		else
		{
			position.z = -stepZ;
		}

		transform.position += position;
	}
}
