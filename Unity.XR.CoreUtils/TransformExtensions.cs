using System;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class TransformExtensions
	{
		public static Pose GetLocalPose(this Transform transform)
		{
			Vector3 position;
			Quaternion rotation;
			transform.GetLocalPositionAndRotation(out position, out rotation);
			return new Pose(position, rotation);
		}

		public static Pose GetWorldPose(this Transform transform)
		{
			Vector3 position;
			Quaternion rotation;
			transform.GetPositionAndRotation(out position, out rotation);
			return new Pose(position, rotation);
		}

		public static void SetLocalPose(this Transform transform, Pose pose)
		{
			transform.SetLocalPositionAndRotation(pose.position, pose.rotation);
		}

		public static void SetWorldPose(this Transform transform, Pose pose)
		{
			transform.SetPositionAndRotation(pose.position, pose.rotation);
		}

		public static Pose TransformPose(this Transform transform, Pose pose)
		{
			return pose.GetTransformedBy(transform);
		}

		public static Pose InverseTransformPose(this Transform transform, Pose pose)
		{
			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}
			return new Pose
			{
				position = transform.InverseTransformPoint(pose.position),
				rotation = Quaternion.Inverse(transform.rotation) * pose.rotation
			};
		}

		public static Ray InverseTransformRay(this Transform transform, Ray ray)
		{
			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}
			return new Ray(transform.InverseTransformPoint(ray.origin), transform.InverseTransformDirection(ray.direction));
		}
	}
}
