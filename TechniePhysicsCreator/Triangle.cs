using System;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class Triangle
	{
		public Triangle(Vector3 p0, Vector3 p1, Vector3 p2)
		{
			Vector3 lhs = p1 - p0;
			Vector3 rhs = p2 - p0;
			Vector3 vector = Vector3.Cross(lhs, rhs);
			this.area = vector.magnitude * 0.5f;
			this.normal = vector.normalized;
			this.center = (p0 + p1 + p2) / 3f;
		}

		public Vector3 normal;

		public float area;

		public Vector3 center;
	}
}
