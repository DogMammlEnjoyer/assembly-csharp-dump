using System;
using UnityEngine;

namespace Liv.Lck
{
	public static class LckRigidbodyExtensions
	{
		public static void LookAtFromPivotPoint(this Rigidbody rigidbody, Vector3 pivot, Vector3 forward, Vector3 position, Quaternion currentRotation)
		{
			Quaternion quaternion = Quaternion.LookRotation(forward.normalized, Vector3.up);
			Quaternion rotation = quaternion * Quaternion.Inverse(currentRotation);
			Vector3 point = position - pivot;
			Vector3 b = rotation * point;
			Vector3 position2 = pivot + b;
			rigidbody.MovePosition(position2);
			rigidbody.MoveRotation(quaternion);
		}
	}
}
