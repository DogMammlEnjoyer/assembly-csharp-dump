using System;
using UnityEngine;

namespace Oculus.Interaction.Throw
{
	[Obsolete]
	public class VelocityCalculatorUtilMethods
	{
		public static Vector3 ToLinearVelocity(Vector3 startPosition, Vector3 destinationPosition, float deltaTime)
		{
			if (Mathf.Abs(deltaTime) <= Mathf.Epsilon)
			{
				return Vector3.zero;
			}
			return (destinationPosition - startPosition) / deltaTime;
		}

		public static Vector3 ToAngularVelocity(Quaternion startQuaternion, Quaternion destinationQuaternion, float deltaTime)
		{
			if (startQuaternion.Equals(destinationQuaternion) || deltaTime == 0f)
			{
				return Vector3.zero;
			}
			return VelocityCalculatorUtilMethods.DeltaRotationToAngularVelocity(destinationQuaternion * Quaternion.Inverse(startQuaternion), deltaTime);
		}

		public static Quaternion AngularVelocityToQuat(Vector3 angularVelocity)
		{
			return Quaternion.AngleAxis(angularVelocity.magnitude, angularVelocity.normalized);
		}

		public static ValueTuple<float, Vector3> QuatToAngleAxis(Quaternion inputQuat)
		{
			float num;
			Vector3 zero;
			inputQuat.ToAngleAxis(out num, out zero);
			if (float.IsInfinity(zero.x))
			{
				zero = Vector3.zero;
				num = 0f;
			}
			if (num > 180f)
			{
				num -= 360f;
			}
			return new ValueTuple<float, Vector3>(num, zero);
		}

		public static Vector3 QuatToAngularVeloc(Quaternion inputQuat)
		{
			Vector3 zero = Vector3.zero;
			ValueTuple<float, Vector3> valueTuple = VelocityCalculatorUtilMethods.QuatToAngleAxis(inputQuat);
			float item = valueTuple.Item1;
			return valueTuple.Item2 * item;
		}

		public static Vector3 DeltaRotationToAngularVelocity(Quaternion deltaRotation, float deltaTime)
		{
			ValueTuple<float, Vector3> valueTuple = VelocityCalculatorUtilMethods.QuatToAngleAxis(deltaRotation);
			float item = valueTuple.Item1;
			Vector3 item2 = valueTuple.Item2;
			if (Mathf.Abs(deltaTime) <= Mathf.Epsilon)
			{
				return Vector3.zero;
			}
			return item2 * item * 0.017453292f / deltaTime;
		}

		public static ValueTuple<Vector3, Vector3> GetVelocityAndAngularVelocity(TransformSample startSample, TransformSample endSample, float duration)
		{
			return new ValueTuple<Vector3, Vector3>(VelocityCalculatorUtilMethods.ToLinearVelocity(startSample.Position, endSample.Position, duration), VelocityCalculatorUtilMethods.ToAngularVelocity(startSample.Rotation, endSample.Rotation, duration));
		}
	}
}
