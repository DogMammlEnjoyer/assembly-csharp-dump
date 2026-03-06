using System;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class ConstructionPlane
	{
		public ConstructionPlane(Vector3 c)
		{
			this.center = c;
			this.normal = Vector3.forward;
			this.tangent = Vector3.up;
			this.Init();
		}

		public ConstructionPlane(Vector3 c, Vector3 n, Vector3 t)
		{
			this.center = c;
			this.normal = n;
			this.tangent = t;
			this.Init();
		}

		public ConstructionPlane(ConstructionPlane basePlane, float angle)
		{
			Vector3 vector = Quaternion.AngleAxis(angle, basePlane.normal) * basePlane.tangent;
			this.center = basePlane.center;
			this.normal = basePlane.normal;
			this.tangent = vector;
			this.Init();
		}

		public ConstructionPlane(ConstructionPlane basePlane, Vector3 positionOffset)
		{
			this.center = basePlane.center + positionOffset;
			this.normal = basePlane.normal;
			this.tangent = basePlane.tangent;
			this.Init();
		}

		private void Init()
		{
			if (this.normal.magnitude < 0.01f)
			{
				Debug.LogError("!");
			}
			this.rotation = Quaternion.LookRotation(this.normal, this.tangent);
			this.planeToWorld = Matrix4x4.TRS(this.center, this.rotation, Vector3.one);
			this.worldToPlane = this.planeToWorld.inverse;
		}

		public Vector3 center;

		public Vector3 normal;

		public Vector3 tangent;

		public Quaternion rotation;

		public Matrix4x4 planeToWorld;

		public Matrix4x4 worldToPlane;
	}
}
