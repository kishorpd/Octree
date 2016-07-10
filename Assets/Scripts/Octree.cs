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
		int TotalChildren = 0;
		Octree Parent = null;
		List<Octree> children = new List<Octree>();
		Dictionary<int, List<Octree>> OverLappingParticles = new Dictionary<int, List<Octree>>();
		static float SRadius = 0;
		static float SRadiusDiagonal = 0;
		static float SRadiusSquare = 0;


		enum OctantEnums : byte
		{
			O0= 0x001, // 00000001
			O1= 0x002, // 00000010
			O2= 0x004, // 00000100
			O3= 0x008, // 00001000
			O4= 0x010, // 00010000
			O5= 0x020, // 00100000
			O6= 0x040, // 01000000
			O7= 0x080, // 10000000
		}


		public Octree(Vector3 center, Vector3 halfWidth, float particleRadius)
		{
			//constructor of root 
			Center = center;
			HalfWidth = halfWidth;
			SRadius = particleRadius;
			SRadiusSquare = SRadius * SRadius;
			SRadiusDiagonal = SRadius / Mathf.Sqrt(2);
			Debug.Log("SRadius : " + SRadius);
		}

		public bool Insert(GameObject particleObject)
		{
			//get octant
			OctantsOverlapping(particleObject.transform.position);
			//particleObject.get sprite and radius from it
			//check for overlapping octants
			//drop in given overlapping octant
			return false;
		}

		int GetOctant(Vector3 position)
		{
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

		byte OctantsOverlapping(Vector3 particlePosition)
		{

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





			//Now I went through many implementations up above...which I am cancelling...but finally decided the one below

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

			byte octants = Convert.ToByte("00000000", 2);

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
						if ((octants | OctantsToByte(GetOctant(particlePosition))) != octants)
						octants |= OctantsToByte(GetOctant(particlePosition));
					particlePosition.x += SRadius;

					diagonal.x = particlePosition.x - (SRadiusDiagonal);
				}
				else
				{
					particlePosition.x += SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition));
					particlePosition.x -= SRadius;

					diagonal.x = particlePosition.x + (SRadiusDiagonal);
				}
	
				// check for y
				if (Center.y < particlePosition.y)
				{
					particlePosition.y -= SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition));
					particlePosition.y += SRadius;

					diagonal.y = particlePosition.y - (SRadiusDiagonal);
				}
				else
				{
					particlePosition.y += SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition));
					particlePosition.y -= SRadius;

					diagonal.y = particlePosition.y + (SRadiusDiagonal);
				}

				//check for the XY plane's diagonal
				if (GetOctant(diagonal) != particleOctant)
					octants |= OctantsToByte(GetOctant(diagonal));
				
				
				//check for z
				if (Center.z < particlePosition.z)
				{
					particlePosition.z -= SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition));
					particlePosition.z += SRadius;

					diagonal.z = particlePosition.z - (SRadiusDiagonal);
				}
				else
				{
					particlePosition.z += SRadius;
					if (GetOctant(particlePosition) != particleOctant)
						octants |= OctantsToByte(GetOctant(particlePosition));
					particlePosition.z -= SRadius;

					diagonal.z = particlePosition.z + (SRadiusDiagonal);
				}

				float tempVal = diagonal.x;
				diagonal.x = 0;
				//check for the YZ plane's diagonal
				if (GetOctant(diagonal) != particleOctant)
					octants |= OctantsToByte(GetOctant(diagonal));

				diagonal.x = tempVal;
				tempVal = diagonal.y;
				diagonal.y = 0;
				//check for the XZ plane's diagonal
				if (GetOctant(diagonal) != particleOctant)
				octants |= OctantsToByte(GetOctant(diagonal));

			}


			Debug.Log("__+++++++++++++++++++++++++++++++++++++++++++++++++++_");
			////if (octants.ToString() == OctantEnums.O0.ToString())
			for (int i = 0; i < 8; ++i)
			{
				byte tempOctant = OctantsToByte(i);
				if (tempOctant == (octants & tempOctant))
					Debug.Log("i: " + i + ". OctantsToByte: " + Convert.ToString(tempOctant, 2));
			}

			Debug.Log("___octants: " + Convert.ToString(octants, 2));


			return octants;
		}

		byte OctantsToByte(int octant, byte octantsFilled)
		{
			byte octantByte = 128;// (1 << 1) & 0xFF;
			///octantByte <<= unchecked((int)(octant));
			octantByte >>= octant;
			Debug.Log("___octantByte: " + Convert.ToString(octantByte, 2));

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

			Debug.Log("(((x * x) + (y * y) + (z * z)): " + (((x * x) + (y * y) + (z * z))));
			Debug.Log("SRadiusSquare: " + SRadiusSquare);

			return (((x * x) + (y * y) + (z * z)) <= SRadiusSquare);
		}

	}
}

