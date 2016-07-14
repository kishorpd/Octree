

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Assets.Scripts
{
	public class Octree
	{
		//           Quadrants considered
																		  
		//	    +-----------+       +-----------+	   |+Y
		//	   /_|________/ |	   /_|________/ |	   |
		//	  /| |      / | |	  /| |      / | |	   |
		//	 +----------+ | |	 +----------+ | |	   |
		//	 | | |      | | |	 | | |      | | |	   |  /+Z
		//	 | | +----2-|-|-+	 | | +----1-|-|-+	   | /
		//	 | |/_______|_|/ 	 | |/_______|_|/ ______|/_____+x
		//	 |/    3    |/   	 |/    0    |/   	  /|
		//	 +---------+	   	 +---------+	   -z/ |
		//											/  |
		//											   |
		//	    +-----------+       +-----------+	   |
		//	   /_|________/ |	   /_|________/ |	   |
		//	  /| |      / | |	  /| |      / | |	   | -Y
		//	 +----------+ | |	 +----------+ | |	
		//	 | | |      | | |	 | | |      | | |	
		//	 | | +----6-|-|-+	 | | +----5-|-|-+	
		//	 | |/_______|_|/ 	 | |/_______|_|/ 	
		//	 |/    7    |/   	 |/    4    |/   	
		//	 +---------+	   	 +---------+	   


		float a;

		Vector3 Center ;//= new Vector3();
		Vector3 HalfWidth;// = new Vector3();
		Octree Parent = null;
		Dictionary<int, Octree> _Nodes = new Dictionary<int, Octree>();
		List<GameObject> _Children = new List<GameObject>();
		Dictionary<int, List<GameObject>> _OverLappingParticles = new Dictionary<int, List<GameObject>>();
		static float SRadius = 0;
		static float SRadiusDiagonal = 0;
		static float SRadiusSquare = 0;
		static int SMaxDepthToStopAt = 10;
		static float SQUARE_ROOT_THREE_BY_TWO = Mathf.Sqrt(3)/2;

		public static int STotalOverLapping = 0;
		public static int SMaxChildren = 1;
		public static int SMaxDepthReached = 0;
		public static bool SDebugVertices { get; set; }


		bool _OverFlow = false;
		public int TotalChildren = 0;
		int _CurrentDepth = 0;

		public static MainInstance SMainInstance { get; set; }

		enum OctantEnums : byte
		{
			O0 = 0	,//= 0x001, // 00000001
			O1		,//= 0x002, // 00000010
			O2		,//= 0x004, // 00000100
			O3		,//= 0x008, // 00001000
			O4		,//= 0x010, // 00010000
			O5		,//= 0x020, // 00100000
			O6		,//= 0x040, // 01000000
			O7		 //= 0x080, // 10000000
		}


		public Octree(Vector3 center, Vector3 halfWidth, float particleRadius, MainInstance main)
		{
			//constructor of root 
			Center = center;
			HalfWidth = halfWidth;
			SRadius = particleRadius;
			SRadiusSquare = SRadius * SRadius;
			SRadiusDiagonal = SRadius / Mathf.Sqrt(2);
			Debug.Log("SRadius : " + SRadius);
			SMainInstance = main;
			SDebugVertices = false;
		}


		public Octree(Vector3 center, Vector3 halfWidth, float particleRadius)
		{
			//constructor of root 
			Center = center;
			HalfWidth = halfWidth;
			SRadius = particleRadius;
			SRadiusSquare = SRadius * SRadius;
			SRadiusDiagonal = SRadius / Mathf.Sqrt(2);

		}

		Octree(Vector3 center, Vector3 halfWidth, Octree parent)
		{
			Center = center;
			HalfWidth = halfWidth;
			Parent = parent;
			_CurrentDepth = parent._CurrentDepth + 1;

			if (_CurrentDepth > SMaxDepthReached)
				SMaxDepthReached = _CurrentDepth;

		}

		public bool Insert(GameObject particleObject)
		{
			int octantOfParticleObj = GetOctant(particleObject.transform.position);
			if (_OverFlow) //total children > SMaxChildren
			{
				if (_Children.Count == 0)
				{
					//insert in children
					
					if (!_Nodes.ContainsKey(octantOfParticleObj))
						_Nodes[octantOfParticleObj] = new Octree(GetCenterOfOctant(GetOctant(particleObject.transform.position)), HalfWidth / 2, this);

					_Nodes[octantOfParticleObj].Insert(particleObject);
				}
				else 
				{
					//create new octrees from private constructor maybe
					foreach( GameObject obj in _Children)
					{
						octantOfParticleObj = GetOctant(obj.transform.position);

						if (!_Nodes.ContainsKey(octantOfParticleObj))
							_Nodes[octantOfParticleObj] = new Octree(GetCenterOfOctant(GetOctant(particleObject.transform.position)), HalfWidth / 2, this);

						_Nodes[octantOfParticleObj].Insert(obj);

				
					}
						_Children.Clear();
				}
				++TotalChildren;
			}
			else 
			{
				_Children.Add(particleObject);
				//Debug.Log(" _Children.Count : " + _Children.Count + " _CurrentDepth : " + _CurrentDepth);
				++TotalChildren;
				if (TotalChildren == SMaxChildren)
				{
					_OverFlow = true;
				}
			}

			//get octant
			//particleObject.get sprite and radius from it
			//check for overlapping octants
			//drop in given overlapping octant
			OctantsOverlapping(particleObject);
			return false;
		}

		int GetOctant(Vector3 position)
		{
			if (SDebugVertices) SMainInstance.SpawnVertex(position);

			return ((position.y < Center.y) ? (InQuadrantXZ(position) + 4) : (InQuadrantXZ(position)));
		}



		int InQuadrantXZ(Vector3 position)
		{
			//    +z
			// ___ ___
			//| 2 | 1 |
			//|___|___| + X
			//| 3 | 0 |
			//|___|___|
			// The quadrants are numbered in this manner.

			return (position.z < Center.z) ?					//if (object is below X axis) {execute first parenthesis} else {second}
					((position.x < Center.x) ? 3 : 0) : 		//if (the object is to left of y axis and below X axis) return 3 else 2
						((position.x < Center.x) ? 2 : 1);		//if (the object is to left of y axis and above X axis) return 0 else 1

		}

		byte OctantsOverlapping(GameObject particleObj)
		{
			Vector3 particlePosition = particleObj.transform.position;
			// This is a way to check the overlapping of sphere in octants
			// But total number of the vertices to check are 22!

			//             +XY								  +YZ
			//
			//         , - ~ 0 ~ - ,			        , - ~ - ~ - ,
			//     7 '               1 ,		    6 '               0 ,
			//   ,                       ,		  ,                       ,
			//  ,                         ,		 ,                         ,
			// ,                           ,	,                           ,
			// 6        Total = 8          2	5        Total = 6          1
			// ,                           ,	,                           ,
			//  ,                         ,		 ,                         ,
			//   ,                       ,		  ,                       ,
			//     5                  , 3		    4                  , 2
			//       ' - , _ 4 _ ,  '			      ' - , _ _ _ ,  '
			//
			//  Total points to check for orthogonal positions = 8 + 6 = 14.


			//             45 degree points from center						
			//
			//         , - ~ - ~ - ,		
			//     3 '               0,		
			//   ,                       ,	
			//  ,                         ,	
			// ,                           ,
			// |        Total = 4          |
			// ,                           ,
			//  ,                         ,	
			//   ,                       ,	
			//     2                  , 1	
			//       ' - , _ _ _ ,  '		
			//
			// Total = 4*2 = 8;
			//  Total points to check = 8 + 6 + 8 = 22.    :(

			//				|~ - ~ - ,		
			//				|          0,		
			//				|             ,	
			//				|  /           ,	
			//				| /             ,
			//				|/ 45 degree    |		x = y = (r/1.414).
			//---------------------------------------




			//Now I went through many implementations including the one up above...which I am cancelling...but finally decided the one below.
			//Following is a more optimized way to achieve the same output. 

			//Check if the distance between the center of the partition and center of the particle is less than 
			//the radius.
			// if it is then the particle lies in all the octants
			//		so add the particle in all the octants
			//else
			// {
			//		vec3 diagonal;
			//		if(center.x,y,z is less than particle.x,y,z)
			//			{ 
			//				check octant for (particle.x,y,z - radius)
			//				(diagonal.x,y,z  =  particle.x,y,z - radius/1.414)
			//			}
			//		else
			//			{ 
			//				check octant for (particle.x + radius)
			//				(diagonal.x,y,z  =  particle.x,y,z + radius/1.414)
			//			}
			//
			//		now check the octant for the diagonal point of each axis
			//		i.e. check overlapping for (diagonal) 
			//	}
			SDebugVertices = true;
			byte octants = Convert.ToByte("00000000", 2);
			//Debug.Log("___octants.12_1: " + octants);
			if (ExistsInAllQuadrants(particlePosition))
			{
				octants = Convert.ToByte("11111111", 2);// 255;
			}
			else 
			{
				Vector3 diagonal = particlePosition;
				int particleOctant = GetOctant(particlePosition);
				//check for x
				if (Center.x < particlePosition.x)
				{
					particlePosition.x -= SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition), octants);
					particlePosition.x += SRadius;

					diagonal.x = particlePosition.x - (SRadiusDiagonal);
				}
				else
				{
					particlePosition.x += SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition), octants);
					particlePosition.x -= SRadius;

					diagonal.x = particlePosition.x + (SRadiusDiagonal);
				}
	
				// check for y
				if (Center.y < particlePosition.y)
				{
					particlePosition.y -= SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition), octants);
					particlePosition.y += SRadius;

					diagonal.y = particlePosition.y - (SRadiusDiagonal);
				}
				else
				{
					particlePosition.y += SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition), octants);
					particlePosition.y -= SRadius;

					diagonal.y = particlePosition.y + (SRadiusDiagonal);
				}

				//check for the XY plane's diagonal
				//Debug.Log("__diagonal.xy_: " + diagonal);
				if (GetOctant(diagonal) != particleOctant)
					octants |= OctantsToByte(GetOctant(diagonal), octants);
				
				
				//check for z
				if (Center.z < particlePosition.z)
				{
					particlePosition.z -= SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition), octants);
					particlePosition.z += SRadius;

					diagonal.z = particlePosition.z - (SRadiusDiagonal);
				}
				else
				{
					particlePosition.z += SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition), octants);
					particlePosition.z -= SRadius;

					diagonal.z = particlePosition.z + (SRadiusDiagonal);
				}

				float tempVal = diagonal.x;
				diagonal.x = particlePosition.x;
				//check for the YZ plane's diagonal
				//Debug.Log("__diagonal.yz_: " + diagonal);
				if (GetOctant(diagonal) != particleOctant)
					octants |= OctantsToByte(GetOctant(diagonal), octants);

				diagonal.x = tempVal;
				tempVal = diagonal.y;
				diagonal.y = particlePosition.y;
				//check for the XZ plane's diagonal
				//Debug.Log("__diagonal.xz_: " + diagonal);
				if (GetOctant(diagonal) != particleOctant)
					octants |= OctantsToByte(GetOctant(diagonal), octants);

			}


			//Debug.Log("__+++++++++++++++++++++++++++++++++++++++++++++++++++_");
			////if (octants.ToString() == OctantEnums.O0.ToString())
			for (int i = 0; i < 8; ++i)
			{
				byte tempOctant = OctantsToByte(i);
				if (tempOctant == (octants & tempOctant))
				{ 
					//Debug.Log("i: " + i + ". OctantsToByte: " + Convert.ToString(tempOctant, 2));
					//insert the object into that specific object
					if (!_OverLappingParticles.ContainsKey(i))
					{
						++STotalOverLapping;
						_OverLappingParticles.Add(i, new List<GameObject>());
					}
					_OverLappingParticles[i].Add(particleObj);
				}
			}

			//Debug.Log("___octants.10 : " + Convert.ToString(octants, 2));
			//Debug.Log("___octants.12_2: " + octants);

			//SDebugVertices = false;

			return octants;
		}

		byte OctantsToByte(int octant)
		{
			byte octantByte = 128;
			///octantByte <<= unchecked((int)(octant));
			octantByte >>= octant;

			return octantByte;
		}

		byte OctantsToByte(int octant, byte octantsFilled)						
		{
			byte octantByte = 128;
			///octantByte <<= unchecked((int)(octant));
			octantByte >>= octant;
			//Debug.Log("_OctantsToByte(,)__octantByte: " + Convert.ToString(octantByte, 2));
			//Debug.Log("_OctantsToByte(,)__octantsFilled: " + Convert.ToString(octantsFilled, 2));

			if ((octantsFilled | octant) == octant)
				return octantByte;
			else
				return 0;

		}

		
		bool ExistsInAllQuadrants(Vector3 particleCenter)
		{
			//keeping readability
			float x = Center.x - particleCenter.x;
			float y = Center.y - particleCenter.y;
			float z = Center.z - particleCenter.z;

			//Debug.Log("SRadiusSquare: " + SRadiusSquare);
			//Debug.Log("(((x * x) + (y * y) + (z * z)): " + (((x * x) + (y * y) + (z * z))));

			return (((x * x) + (y * y) + (z * z)) <= SRadiusSquare);
		}

		Vector3 GetCenterOfOctant(int octant)
		{ 
			OctantEnums octantCase = (OctantEnums)octant;
			///there are two ways one is calculating with few if else other is using switch

			//switch is more readable so going with switch

			float x = HalfWidth.x / 2;
			float y = HalfWidth.y / 2;
			float z = HalfWidth.z / 2;



			//           Quadrants considered

			//	    +-----------+       +-----------+		   |+Y
			//	   /_|________/ |	   /_|________/ |		   |
			//	  /| |      / | |	  /| |      / | |		   |
			//	 +----------+ | |	 +----------+ | |		   |
			//	 | | |      | | |	 | | |      | | |		   |  /+Z
			//	 | | +----2-|-|-+	 | | +----1-|-|-+	 -++2  | /  1  +++
			//	 | |/_______|_|/ 	 | |/_______|_|/	-x_____|/_____+x
			//	 |/    3    |/   	 |/    0    |/	   3 -+-  /|  0 ++-
			//	 +---------+	   	 +---------+		   -z/ |  
			//										 6 --+	/  |  5 +-+
			//												   |   
			//	    +-----------+       +-----------+  7 ---   |  4 +--
			//	   /_|________/ |	   /_|________/ |		   |
			//	  /| |      / | |	  /| |      / | |		   | -Y
			//	 +----------+ | |	 +----------+ | |		
			//	 | | |      | | |	 | | |      | | |		
			//	 | | +----6-|-|-+	 | | +----5-|-|-+		
			//	 | |/_______|_|/ 	 | |/_______|_|/ 	
			//	 |/    7    |/   	 |/    4    |/   	
			//	 +---------+	   	 +---------+	   


			Debug.Log("_CurrentDepth" + _CurrentDepth + "Center" + Center + "octant" + octant);

			switch (octantCase)
			{

				case OctantEnums.O0: return new /*++-*/ Vector3(Center.x + x, Center.y + y, Center.z - z);
				case OctantEnums.O1: return new /*+++*/ Vector3(Center.x + x, Center.y + y, Center.z + z);
				case OctantEnums.O2: return new /*-++*/ Vector3(Center.x - x, Center.y + y, Center.z + z);
				case OctantEnums.O3: return new /*-+-*/ Vector3(Center.x - x, Center.y + y, Center.z - z);
				case OctantEnums.O4: return new /*+--*/ Vector3(Center.x + x, Center.y - y, Center.z - z);
				case OctantEnums.O5: return new /*+-+*/ Vector3(Center.x + x, Center.y - y, Center.z + z);
				case OctantEnums.O6: return new /*--+*/ Vector3(Center.x - x, Center.y - y, Center.z + z);
				case OctantEnums.O7: return new /*---*/ Vector3(Center.x - x, Center.y - y, Center.z - z);

			}

			return new Vector3();

		}

		public void DrawPartitions(GridOverlay gridOverlay)
		{
			if (TotalChildren > 0)
			{
				//Debug.Log(" Center : " + Center + " HalfWidth :" + HalfWidth);
				///Debug.Log(" _CurrentDepth : " + _CurrentDepth + " HalfWidth :" + HalfWidth);
				//Debug.Log(" _TotalChildren : " + _TotalChildren + "_CurrentDepth" + _CurrentDepth);
				//Debug.Log(" sQuarterDiagonal : " + sQuarterDiagonal);
				gridOverlay.DrawPartitioners(Center, HalfWidth);
				for (int i = 0; i < 8; ++i )
				{
					if(_Nodes.ContainsKey(i))
						_Nodes[i].DrawPartitions(gridOverlay);
				}
			}
		}

	}
}

