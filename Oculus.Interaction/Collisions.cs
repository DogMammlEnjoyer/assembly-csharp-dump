using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public static class Collisions
	{
		public static Vector3 ClosestPointToColliders(Vector3 point, Collider[] colliders)
		{
			Vector3 result = point;
			float num = float.PositiveInfinity;
			foreach (Collider collider in colliders)
			{
				Vector3 vector = Collisions.ClosestPointToCollider(point, collider);
				float num2 = Vector3.SqrMagnitude(vector - point);
				if (num2 <= 1E-45f)
				{
					return vector;
				}
				if (num2 < num)
				{
					num = num2;
					result = vector;
				}
			}
			return result;
		}

		public static Vector3 ClosestPointToCollider(Vector3 point, Collider collider)
		{
			MeshCollider meshCollider = collider as MeshCollider;
			if (meshCollider == null)
			{
				return Physics.ClosestPoint(point, collider, collider.transform.position, collider.transform.rotation);
			}
			if (!meshCollider.convex)
			{
				return meshCollider.ClosestPointOnBounds(point);
			}
			Vector3 vector = Physics.ClosestPoint(point, collider, collider.transform.position, collider.transform.rotation);
			if (Vector3.SqrMagnitude(vector - point) < collider.contactOffset * collider.contactOffset)
			{
				return point;
			}
			return vector;
		}

		[Obsolete("This method is not in use and will soon be deleted.")]
		public static bool IsCapsuleWithinColliderApprox(Vector3 p0, Vector3 p1, float radius, Collider collider)
		{
			int num = Mathf.CeilToInt((p1 - p0).magnitude / radius) * 2;
			if (num == 0)
			{
				return Collisions.IsSphereWithinCollider(p0, radius, collider);
			}
			float num2 = 1f / (float)num;
			for (int i = 0; i <= num; i++)
			{
				if (Collisions.IsSphereWithinCollider(Vector3.Lerp(p0, p1, num2 * (float)i), radius, collider))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsPointWithinCollider(Vector3 point, Collider collider)
		{
			return Vector3.SqrMagnitude(Collisions.ClosestPointToCollider(point, collider) - point) <= float.Epsilon;
		}

		public static bool IsSphereWithinCollider(Vector3 point, float radius, Collider collider)
		{
			return Vector3.SqrMagnitude(Collisions.ClosestPointToCollider(point, collider) - point) <= radius * radius;
		}
	}
}
