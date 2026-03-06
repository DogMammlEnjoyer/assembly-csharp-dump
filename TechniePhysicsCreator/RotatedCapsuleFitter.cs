using System;
using Technie.PhysicsCreator.Rigid;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class RotatedCapsuleFitter
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
			ConstructionPlane constructionPlane = this.FindBestCapsulePlane(array, hullIndices);
			RotatedCapsule capsule;
			ConstructionPlane plane;
			RotatedCapsuleFitter.Refine(RotatedCapsuleFitter.FitCapsule(constructionPlane, array), constructionPlane, array, out capsule, out plane);
			return RotatedCapsuleFitter.ToDef(capsule, plane);
		}

		public CapsuleDef Fit(Vector3[] hullVertices, int[] hullIndices)
		{
			if (hullVertices == null || hullVertices.Length == 0 || hullIndices == null || hullIndices.Length == 0)
			{
				return default(CapsuleDef);
			}
			ConstructionPlane constructionPlane = this.FindBestCapsulePlane(hullVertices, hullIndices);
			RotatedCapsule capsule;
			ConstructionPlane plane;
			RotatedCapsuleFitter.Refine(RotatedCapsuleFitter.FitCapsule(constructionPlane, hullVertices), constructionPlane, hullVertices, out capsule, out plane);
			return RotatedCapsuleFitter.ToDef(capsule, plane);
		}

		public ConstructionPlane FindBestCapsulePlane(Vector3[] hullVertices, int[] hullIndices)
		{
			BoxDef boxDef = new RotatedBoxFitter().Fit(hullVertices, hullIndices);
			Vector3 c = boxDef.boxPosition + boxDef.boxRotation * boxDef.collisionBox.center;
			ConstructionPlane result;
			if (boxDef.collisionBox.size.x > boxDef.collisionBox.size.y && boxDef.collisionBox.size.x > boxDef.collisionBox.size.z)
			{
				result = new ConstructionPlane(c, boxDef.boxRotation * Vector3.right, boxDef.boxRotation * Vector3.forward);
			}
			else if (boxDef.collisionBox.size.y > boxDef.collisionBox.size.z)
			{
				result = new ConstructionPlane(c, boxDef.boxRotation * Vector3.up, boxDef.boxRotation * Vector3.right);
			}
			else
			{
				result = new ConstructionPlane(c, boxDef.boxRotation * Vector3.forward, boxDef.boxRotation * Vector3.right);
			}
			return result;
		}

		public static CapsuleDef ToDef(RotatedCapsule capsule, ConstructionPlane plane)
		{
			return new CapsuleDef
			{
				capsuleCenter = Vector3.zero,
				capsuleDirection = CapsuleAxis.Z,
				capsuleRadius = capsule.radius,
				capsuleHeight = capsule.height,
				capsulePosition = plane.center,
				capsuleRotation = plane.rotation
			};
		}

		public static void Refine(RotatedCapsule inputCapule, ConstructionPlane inputPlane, Vector3[] hullVertices, out RotatedCapsule bestCapsule, out ConstructionPlane bestPlane)
		{
			bestPlane = inputPlane;
			bestCapsule = inputCapule;
			Random random = new Random(1234);
			int num = 1024;
			for (int i = 0; i < num; i++)
			{
				float magnitude = Mathf.Min(bestCapsule.height, bestCapsule.radius) * 0.01f;
				ConstructionPlane constructionPlane = new ConstructionPlane(bestPlane, new Vector3(RotatedCapsuleFitter.Jitter(magnitude, random), RotatedCapsuleFitter.Jitter(magnitude, random), RotatedCapsuleFitter.Jitter(magnitude, random)));
				RotatedCapsule rotatedCapsule = RotatedCapsuleFitter.FitCapsule(constructionPlane, hullVertices);
				if (rotatedCapsule.CalcVolume() < bestCapsule.CalcVolume())
				{
					bestCapsule = rotatedCapsule;
					bestPlane = constructionPlane;
				}
			}
		}

		private static float Jitter(float magnitude, Random random)
		{
			return (float)(random.NextDouble() * (double)(magnitude * 2f) - (double)magnitude);
		}

		public static RotatedCapsule FitCapsule(ConstructionPlane plane, Vector3[] points)
		{
			RotatedCapsule rotatedCapsule = default(RotatedCapsule);
			rotatedCapsule.center = plane.center;
			rotatedCapsule.dir = plane.normal;
			foreach (Vector3 vector in points)
			{
				Vector3 vector2 = RotatedCapsuleFitter.ProjectOntoAxis(plane, vector);
				float b = Vector3.Distance(vector2, vector);
				float num = Vector3.Distance(plane.center, vector2);
				rotatedCapsule.radius = Mathf.Max(rotatedCapsule.radius, b);
				rotatedCapsule.height = Mathf.Max(rotatedCapsule.height, num * 2f);
			}
			return rotatedCapsule;
		}

		private static Vector3 ProjectOntoAxis(ConstructionPlane plane, Vector3 point)
		{
			Vector3 rhs = point - plane.center;
			float d = Vector3.Dot(plane.normal, rhs);
			return plane.center + plane.normal * d;
		}

		public static Vector3 FindCenter(Vector3[] vertices)
		{
			if (vertices == null || vertices.Length == 0)
			{
				return Vector3.zero;
			}
			Vector3 a = Vector3.zero;
			for (int i = 0; i < vertices.Length; i++)
			{
				a += vertices[i];
			}
			return a / (float)vertices.Length;
		}
	}
}
