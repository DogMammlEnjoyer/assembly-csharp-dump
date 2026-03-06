using System;
using Technie.PhysicsCreator.Rigid;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class AlignedCapsuleFitter
	{
		public CapsuleDef Fit(Hull hull, Vector3[] meshVertices, int[] meshIndices)
		{
			Vector3[] array;
			int[] hullIndices;
			hull.FindConvexHull(meshVertices, meshIndices, out array, out hullIndices, false);
			if (array == null || array.Length == 0)
			{
				return default(CapsuleDef);
			}
			return this.Fit(array, hullIndices);
		}

		public CapsuleDef Fit(Vector3[] hullVertices, int[] hullIndices)
		{
			if (hullVertices == null || hullVertices.Length == 0 || hullIndices == null || hullIndices.Length == 0)
			{
				return default(CapsuleDef);
			}
			RotatedBox rotatedBox = RotatedBoxFitter.FindTightestBox(new ConstructionPlane(Vector3.zero), hullVertices);
			ConstructionPlane constructionPlane;
			CapsuleAxis capsuleDirection;
			if (rotatedBox.size.x > rotatedBox.size.y && rotatedBox.size.x > rotatedBox.size.z)
			{
				constructionPlane = new ConstructionPlane(rotatedBox.center, Vector3.right, Vector3.forward);
				capsuleDirection = CapsuleAxis.X;
			}
			else if (rotatedBox.size.y > rotatedBox.size.z)
			{
				constructionPlane = new ConstructionPlane(rotatedBox.center, Vector3.up, Vector3.right);
				capsuleDirection = CapsuleAxis.Y;
			}
			else
			{
				constructionPlane = new ConstructionPlane(rotatedBox.center, Vector3.forward, Vector3.right);
				capsuleDirection = CapsuleAxis.Z;
			}
			RotatedCapsule rotatedCapsule;
			ConstructionPlane constructionPlane2;
			RotatedCapsuleFitter.Refine(RotatedCapsuleFitter.FitCapsule(constructionPlane, hullVertices), constructionPlane, hullVertices, out rotatedCapsule, out constructionPlane2);
			return new CapsuleDef
			{
				capsuleDirection = capsuleDirection,
				capsuleRadius = rotatedCapsule.radius,
				capsuleHeight = rotatedCapsule.height,
				capsuleCenter = rotatedCapsule.center,
				capsulePosition = Vector3.zero,
				capsuleRotation = Quaternion.identity
			};
		}
	}
}
