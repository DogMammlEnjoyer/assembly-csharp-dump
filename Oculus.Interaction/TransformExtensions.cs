using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public static class TransformExtensions
	{
		public static Vector3 InverseTransformPointUnscaled(this Transform transform, Vector3 position)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(position);
		}

		public static Vector3 TransformPointUnscaled(this Transform transform, Vector3 position)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).MultiplyPoint3x4(position);
		}

		public static Bounds TransformBounds(this Transform transform, in Bounds bounds)
		{
			Bounds result = default(Bounds);
			Bounds bounds2 = bounds;
			Vector3 min = bounds2.min;
			bounds2 = bounds;
			Vector3 max = bounds2.max;
			Vector3 position = transform.position;
			Vector3 position2 = transform.position;
			Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					float num = localToWorldMatrix[i, j] * min[j];
					float num2 = localToWorldMatrix[i, j] * max[j];
					ref Vector3 ptr = ref position;
					int index = i;
					ptr[index] += ((num < num2) ? num : num2);
					ptr = ref position2;
					index = i;
					ptr[index] += ((num < num2) ? num2 : num);
				}
			}
			result.SetMinMax(position, position2);
			return result;
		}

		public static Transform FindChildRecursive(this Transform parent, string name)
		{
			return parent.FindChildRecursive((Transform child) => child.name.Contains(name));
		}

		public static Transform FindChildRecursive(this Transform parent, Predicate<Transform> predicate)
		{
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (predicate(transform))
				{
					return transform;
				}
				Transform transform2 = transform.FindChildRecursive(predicate);
				if (transform2 != null)
				{
					return transform2;
				}
			}
			return null;
		}
	}
}
