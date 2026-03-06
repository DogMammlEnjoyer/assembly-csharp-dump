using System;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	public static class AnimationRuntimeUtils
	{
		public static void SolveTwoBoneIK(AnimationStream stream, ReadWriteTransformHandle root, ReadWriteTransformHandle mid, ReadWriteTransformHandle tip, ReadOnlyTransformHandle target, ReadOnlyTransformHandle hint, float posWeight, float rotWeight, float hintWeight, AffineTransform targetOffset)
		{
			Vector3 position = root.GetPosition(stream);
			Vector3 position2 = mid.GetPosition(stream);
			Vector3 position3 = tip.GetPosition(stream);
			Vector3 a;
			Quaternion lhs;
			target.GetGlobalTR(stream, out a, out lhs);
			Vector3 a2 = Vector3.Lerp(position3, a + targetOffset.translation, posWeight);
			Quaternion rotation = Quaternion.Lerp(tip.GetRotation(stream), lhs * targetOffset.rotation, rotWeight);
			bool flag = hint.IsValid(stream) && hintWeight > 0f;
			Vector3 vector = position2 - position;
			Vector3 rhs = position3 - position2;
			Vector3 vector2 = position3 - position;
			Vector3 vector3 = a2 - position;
			float magnitude = vector.magnitude;
			float magnitude2 = rhs.magnitude;
			float magnitude3 = vector2.magnitude;
			float magnitude4 = vector3.magnitude;
			float num = AnimationRuntimeUtils.TriangleAngle(magnitude3, magnitude, magnitude2);
			float num2 = AnimationRuntimeUtils.TriangleAngle(magnitude4, magnitude, magnitude2);
			Vector3 vector4 = Vector3.Cross(vector, rhs);
			if (vector4.sqrMagnitude < 1E-08f)
			{
				vector4 = (flag ? Vector3.Cross(hint.GetPosition(stream) - position, rhs) : Vector3.zero);
				if (vector4.sqrMagnitude < 1E-08f)
				{
					vector4 = Vector3.Cross(vector3, rhs);
				}
				if (vector4.sqrMagnitude < 1E-08f)
				{
					vector4 = Vector3.up;
				}
			}
			vector4 = Vector3.Normalize(vector4);
			float f = 0.5f * (num - num2);
			float num3 = Mathf.Sin(f);
			float w = Mathf.Cos(f);
			Quaternion lhs2 = new Quaternion(vector4.x * num3, vector4.y * num3, vector4.z * num3, w);
			mid.SetRotation(stream, lhs2 * mid.GetRotation(stream));
			vector2 = tip.GetPosition(stream) - position;
			root.SetRotation(stream, QuaternionExt.FromToRotation(vector2, vector3) * root.GetRotation(stream));
			if (flag)
			{
				float sqrMagnitude = vector2.sqrMagnitude;
				if (sqrMagnitude > 0f)
				{
					position2 = mid.GetPosition(stream);
					Vector3 position4 = tip.GetPosition(stream);
					vector = position2 - position;
					vector2 = position4 - position;
					Vector3 vector5 = vector2 / Mathf.Sqrt(sqrMagnitude);
					Vector3 vector6 = hint.GetPosition(stream) - position;
					Vector3 from = vector - vector5 * Vector3.Dot(vector, vector5);
					Vector3 to = vector6 - vector5 * Vector3.Dot(vector6, vector5);
					float num4 = magnitude + magnitude2;
					if (from.sqrMagnitude > num4 * num4 * 0.001f && to.sqrMagnitude > 0f)
					{
						Quaternion quaternion = QuaternionExt.FromToRotation(from, to);
						quaternion.x *= hintWeight;
						quaternion.y *= hintWeight;
						quaternion.z *= hintWeight;
						quaternion = QuaternionExt.NormalizeSafe(quaternion);
						root.SetRotation(stream, quaternion * root.GetRotation(stream));
					}
				}
			}
			tip.SetRotation(stream, rotation);
		}

		public static void InverseSolveTwoBoneIK(AnimationStream stream, ReadOnlyTransformHandle root, ReadOnlyTransformHandle mid, ReadOnlyTransformHandle tip, ReadWriteTransformHandle target, ReadWriteTransformHandle hint, float posWeight, float rotWeight, float hintWeight, AffineTransform targetOffset)
		{
			Vector3 position = root.GetPosition(stream);
			Vector3 position2 = mid.GetPosition(stream);
			Vector3 tipPosition;
			Quaternion tipRotation;
			tip.GetGlobalTR(stream, out tipPosition, out tipRotation);
			Vector3 position3;
			Quaternion rotation;
			target.GetGlobalTR(stream, out position3, out rotation);
			bool flag = hint.IsValid(stream);
			Vector3 position4 = Vector3.zero;
			if (flag)
			{
				position4 = hint.GetPosition(stream);
			}
			AnimationRuntimeUtils.InverseSolveTwoBoneIK(position, position2, tipPosition, tipRotation, ref position3, ref rotation, ref position4, flag, posWeight, rotWeight, hintWeight, targetOffset);
			target.SetPosition(stream, position3);
			target.SetRotation(stream, rotation);
			hint.SetPosition(stream, position4);
		}

		public static void InverseSolveTwoBoneIK(Vector3 rootPosition, Vector3 midPosition, Vector3 tipPosition, Quaternion tipRotation, ref Vector3 targetPosition, ref Quaternion targetRotation, ref Vector3 hintPosition, bool isHintValid, float posWeight, float rotWeight, float hintWeight, AffineTransform targetOffset)
		{
			targetPosition = ((posWeight > 0f) ? (tipPosition + targetOffset.translation) : targetPosition);
			targetRotation = ((rotWeight > 0f) ? (tipRotation * targetOffset.rotation) : targetRotation);
			if (isHintValid)
			{
				Vector3 vector = tipPosition - rootPosition;
				Vector3 a = midPosition - rootPosition;
				Vector3 vector2 = tipPosition - midPosition;
				float magnitude = a.magnitude;
				float magnitude2 = vector2.magnitude;
				float num = Vector3.Dot(vector, vector);
				Vector3 vector3 = rootPosition;
				if (num > 1E-08f)
				{
					vector3 += Vector3.Dot(a / num, vector) * vector;
				}
				Vector3 vector4 = midPosition - vector3;
				float d = magnitude + magnitude2;
				hintPosition = ((hintWeight > 0f) ? (vector3 + vector4.normalized * d) : hintPosition);
			}
		}

		private static float TriangleAngle(float aLen, float aLen1, float aLen2)
		{
			return Mathf.Acos(Mathf.Clamp((aLen1 * aLen1 + aLen2 * aLen2 - aLen * aLen) / (aLen1 * aLen2) / 2f, -1f, 1f));
		}

		public static bool SolveFABRIK(ref NativeArray<Vector3> linkPositions, ref NativeArray<float> linkLengths, Vector3 target, float tolerance, float maxReach, int maxIterations)
		{
			Vector3 vector = target - linkPositions[0];
			if (vector.sqrMagnitude > AnimationRuntimeUtils.Square(maxReach))
			{
				Vector3 normalized = vector.normalized;
				for (int i = 1; i < linkPositions.Length; i++)
				{
					linkPositions[i] = linkPositions[i - 1] + normalized * linkLengths[i - 1];
				}
				return true;
			}
			int num = linkPositions.Length - 1;
			float num2 = AnimationRuntimeUtils.Square(tolerance);
			if (AnimationRuntimeUtils.SqrDistance(linkPositions[num], target) > num2)
			{
				Vector3 value = linkPositions[0];
				int num3 = 0;
				do
				{
					linkPositions[num] = target;
					for (int j = num - 1; j > -1; j--)
					{
						linkPositions[j] = linkPositions[j + 1] + (linkPositions[j] - linkPositions[j + 1]).normalized * linkLengths[j];
					}
					linkPositions[0] = value;
					for (int k = 1; k < linkPositions.Length; k++)
					{
						linkPositions[k] = linkPositions[k - 1] + (linkPositions[k] - linkPositions[k - 1]).normalized * linkLengths[k - 1];
					}
				}
				while (AnimationRuntimeUtils.SqrDistance(linkPositions[num], target) > num2 && ++num3 < maxIterations);
				return true;
			}
			return false;
		}

		public static float SqrDistance(Vector3 lhs, Vector3 rhs)
		{
			return (rhs - lhs).sqrMagnitude;
		}

		public static float Square(float value)
		{
			return value * value;
		}

		public static Vector3 Lerp(Vector3 a, Vector3 b, Vector3 t)
		{
			return Vector3.Scale(a, Vector3.one - t) + Vector3.Scale(b, t);
		}

		public static float Select(float a, float b, float c)
		{
			if (c <= 0f)
			{
				return a;
			}
			return b;
		}

		public static Vector3 Select(Vector3 a, Vector3 b, Vector3 c)
		{
			return new Vector3(AnimationRuntimeUtils.Select(a.x, b.x, c.x), AnimationRuntimeUtils.Select(a.y, b.y, c.y), AnimationRuntimeUtils.Select(a.z, b.z, c.z));
		}

		public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
		{
			float num = Vector3.Dot(planeNormal, planeNormal);
			float num2 = Vector3.Dot(vector, planeNormal);
			return new Vector3(vector.x - planeNormal.x * num2 / num, vector.y - planeNormal.y * num2 / num, vector.z - planeNormal.z * num2 / num);
		}

		internal static float Sum(AnimationJobCache cache, CacheIndex index, int count)
		{
			if (count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				num += cache.GetRaw(index, i);
			}
			return num;
		}

		public static float Sum(NativeArray<float> floatBuffer)
		{
			if (floatBuffer.Length == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < floatBuffer.Length; i++)
			{
				num += floatBuffer[i];
			}
			return num;
		}

		public static void PassThrough(AnimationStream stream, ReadWriteTransformHandle handle)
		{
			Vector3 position;
			Quaternion rotation;
			Vector3 scale;
			handle.GetLocalTRS(stream, out position, out rotation, out scale);
			handle.SetLocalTRS(stream, position, rotation, scale, false);
		}

		private const float k_SqrEpsilon = 1E-08f;
	}
}
