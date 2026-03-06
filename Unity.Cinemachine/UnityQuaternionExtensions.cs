using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public static class UnityQuaternionExtensions
	{
		public static Quaternion SlerpWithReferenceUp(Quaternion qA, Quaternion qB, float t, Vector3 up)
		{
			Vector3 vector = (qA * Vector3.forward).ProjectOntoPlane(up);
			Vector3 v = (qB * Vector3.forward).ProjectOntoPlane(up);
			if (vector.AlmostZero() || v.AlmostZero())
			{
				return Quaternion.Slerp(qA, qB, t);
			}
			Quaternion quaternion = Quaternion.LookRotation(vector, up);
			Quaternion lhs = Quaternion.Inverse(quaternion);
			Quaternion quaternion2 = lhs * qA;
			Quaternion quaternion3 = lhs * qB;
			Vector3 eulerAngles = quaternion2.eulerAngles;
			Vector3 eulerAngles2 = quaternion3.eulerAngles;
			return quaternion * Quaternion.Euler(Mathf.LerpAngle(eulerAngles.x, eulerAngles2.x, t), Mathf.LerpAngle(eulerAngles.y, eulerAngles2.y, t), Mathf.LerpAngle(eulerAngles.z, eulerAngles2.z, t));
		}

		public static Vector2 GetCameraRotationToTarget(this Quaternion orient, Vector3 lookAtDir, Vector3 worldUp)
		{
			if (lookAtDir.AlmostZero())
			{
				return Vector2.zero;
			}
			Quaternion rotation = Quaternion.Inverse(orient);
			Vector3 vector = rotation * worldUp;
			lookAtDir = rotation * lookAtDir;
			float num = 0f;
			Vector3 vector2 = lookAtDir.ProjectOntoPlane(vector);
			if (!vector2.AlmostZero())
			{
				Vector3 vector3 = Vector3.forward.ProjectOntoPlane(vector);
				if (vector3.AlmostZero())
				{
					vector3 = Vector3.up.ProjectOntoPlane(vector);
				}
				num = UnityVectorExtensions.SignedAngle(vector3, vector2, vector);
			}
			Quaternion rotation2 = Quaternion.AngleAxis(num, vector);
			return new Vector2(UnityVectorExtensions.SignedAngle(rotation2 * Vector3.forward, lookAtDir, rotation2 * Vector3.right), num);
		}

		public static Quaternion ApplyCameraRotation(this Quaternion orient, Vector2 rot, Vector3 worldUp)
		{
			if (rot.sqrMagnitude < 0.0001f)
			{
				return orient;
			}
			Quaternion rhs = Quaternion.AngleAxis(rot.x, Vector3.right);
			return Quaternion.AngleAxis(rot.y, worldUp) * orient * rhs;
		}
	}
}
