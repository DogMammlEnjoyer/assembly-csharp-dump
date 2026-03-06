using System;
using UnityEngine;

namespace GTMathUtil
{
	internal class WithinBounds
	{
		public static bool PointWithinBoxColliderBounds(Vector3 point, BoxCollider boxCollider)
		{
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			Vector3 vector2 = boxCollider.size / 2f;
			Vector3 center = boxCollider.center;
			return vector.x < vector2.x + center.x && vector.x > -vector2.x + center.x && vector.y < vector2.y + center.y && vector.y > -vector2.y + center.y && vector.z < vector2.z + center.z && vector.z > -vector2.z + center.z;
		}

		public static bool PointWithinCapsuleColliderBounds(Vector3 point, CapsuleCollider capsuleCollider)
		{
			Vector3 vector = capsuleCollider.transform.InverseTransformPoint(point);
			Vector3 center = capsuleCollider.center;
			float radius = capsuleCollider.radius;
			float height = capsuleCollider.height;
			Vector3 vector2 = Vector3.up;
			switch (capsuleCollider.direction)
			{
			case 0:
				vector2 = Vector3.right;
				break;
			case 1:
				vector2 = Vector3.up;
				break;
			case 2:
				vector2 = Vector3.forward;
				break;
			}
			float num = Vector3.Dot(vector, vector2);
			Vector3 vector3 = num * vector2 + center;
			Vector3 vector4 = vector3 - center;
			return ((vector - vector3).magnitude < radius && vector4.magnitude <= height) || (num >= 0f && (vector - (center + vector2 * height / 2f)).magnitude < radius) || (num < 0f && (vector - (center - vector2 * height / 2f)).magnitude < radius);
		}

		public static bool PointWithinSphereColliderBounds(Vector3 point, SphereCollider sphereCollider)
		{
			Vector3 a = sphereCollider.transform.InverseTransformPoint(point);
			Vector3 center = sphereCollider.center;
			float radius = sphereCollider.radius;
			return (a - center).magnitude < radius;
		}
	}
}
