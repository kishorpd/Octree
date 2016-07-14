using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridOverlay : MonoBehaviour
{
	
	// x ht, y , z
	public const float Center = 22.0f;
	public const float MaxDimension = 17.0f;
	public Vector3 CubeDimension = new Vector3(MaxDimension/2,MaxDimension/2,MaxDimension/2);

	
	private Material lineMaterial;
	public MainInstance _MainInstance;

	private Color mainColor = new Color(0f, 1f, 0f, 1f);
	private Color subColor = new Color(0f, 0.5f, 0f, 1f);

	public Vector3 CubeCenter = new Vector3(Center, Center, Center);
	public Vector3 PartitionerCenter = new Vector3(Center, Center, Center);
	public Vector3 CubeWidth;
	int i = 0;
	bool completed = true;

	public bool ShowPartitions = true;
	public bool ShowParticleDebugLines = true;

	void Start()
	{
		CubeWidth = CubeDimension;
	}


	void DrawLine(Vector3 center, Vector3 width)
	{
		GL.Vertex(center + width);
		GL.Vertex(center - width);
	}

	void DrawPlaneXZ(Vector3 center, Vector3 width)
	{
		Vector3 tempWidth = new Vector3();
		tempWidth.z += width.z;
		center.x += width.x;
		DrawLine(center, tempWidth);
		center.x -= 2 * width.x;
		DrawLine(center, tempWidth);

		tempWidth.z = 0;
		center.x += width.x;


		tempWidth.x += width.x;
		center.z += width.z;
		DrawLine(center, tempWidth);
		center.z -= 2 * width.z;
		DrawLine(center, tempWidth);

		tempWidth.x = 0;

	}

	void DrawPlaneXY(Vector3 center, Vector3 width)
	{
		Vector3 tempWidth = new Vector3();
		tempWidth.y += width.y;
		center.x += width.x;
		DrawLine(center, tempWidth);
		center.x -= 2 * width.x;
		DrawLine(center, tempWidth);
		tempWidth.y = 0;
		center.x += width.x;


		tempWidth.x += width.x;
		center.y += width.y;
		DrawLine(center, tempWidth);
		center.y -= 2 * width.y;
		DrawLine(center, tempWidth);
	}

	void DrawPlaneYZ(Vector3 center, Vector3 width)
	{
		Vector3 tempWidth = new Vector3();
		tempWidth.y += width.y;
		center.z += width.z;
		DrawLine(center, tempWidth);
		center.z -= 2 * width.z;
		DrawLine(center, tempWidth);
		tempWidth.y = 0;
		center.z += width.z;


		tempWidth.z += width.z;
		center.y += width.y;
		DrawLine(center, tempWidth);
		center.y -= 2 * width.y;
		DrawLine(center, tempWidth);
	}

	void DrawPlanePartitions(Vector3 center, Vector3 width)
	{
		DrawPlaneXZ(center, width);
		DrawPlaneXY(center, width);
		DrawPlaneYZ(center, width);
	}

	public void DrawCube(Vector3 center, Vector3 width)
	{

		Vector3 tempCenter = new Vector3();
		tempCenter = center;
		tempCenter.y += width.y;
		DrawPlaneXZ(tempCenter, width);
		tempCenter.y -= 2*width.y;
		DrawPlaneXZ(tempCenter, width);
		tempCenter.y = center.y;

		tempCenter.z += width.z;
		DrawPlaneXY(tempCenter, width);
		tempCenter.z -= 2 * width.z;
		DrawPlaneXY(tempCenter, width);
		tempCenter.z = center.z;
		
		tempCenter.x += width.x;
		DrawPlaneYZ(tempCenter, width);
		tempCenter.x -= 2 * width.x;
		DrawPlaneYZ(tempCenter, width);
	}

	public void DrawPartitioners(Vector3 center, Vector3 width)
	{
		DrawLine(center, new Vector3(0, width.y, 0));
		DrawLine(center, new Vector3(0, 0, width.z));
		DrawLine(center, new Vector3(width.x, 0, 0));
		DrawPlanePartitions(center, width);
	}

	void Update()
	{
	}

	void CreateLineMaterial()
	{

		if (!lineMaterial)
		{
			lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
				"SubShader { Pass { " +
				"    Blend SrcAlpha OneMinusSrcAlpha " +
				"    ZWrite Off Cull Off Fog { Mode Off } " +
				"    BindChannels {" +
				"      Bind \"vertex\", vertex Bind \"color\", color }" +
				"} } }");
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public void ParticleDebugLines()
	{
		ShowParticleDebugLines = !ShowParticleDebugLines;
	}

	void OnPostRender()
	{
		CreateLineMaterial();
		// set the current material
		lineMaterial.SetPass(0);

		GL.Begin(GL.LINES);

		GL.Color(mainColor);

		//Debug.Log("OnPostRender-> Center : " + PartitionerCenter + " HalfWidth :" + CubeWidth);
		//DrawPartitioners(PartitionerCenter, CubeWidth);
		DrawCube(CubeCenter, CubeWidth);
		
		if (ShowParticleDebugLines)
			_MainInstance.DrawParticlesDebug();

		if (ShowPartitions)
		{
 			//draw partitions for octrees
			_MainInstance.DrawOctreePartitions();
		}

		GL.End();
	}
}
