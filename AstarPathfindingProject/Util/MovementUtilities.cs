using System;
using UnityEngine;

namespace Pathfinding.Util
{
	public static class MovementUtilities
	{
		public static Vector2 ClampVelocity(Vector2 velocity, float maxSpeed, float slowdownFactor, bool slowWhenNotFacingTarget, Vector2 forward)
		{
			float num = maxSpeed * slowdownFactor;
			if (slowWhenNotFacingTarget && (forward.x != 0f || forward.y != 0f))
			{
				float num2;
				Vector2 vector = VectorMath.Normalize(velocity, out num2);
				float num3 = Vector2.Dot(vector, forward);
				float num4 = Mathf.Clamp(num3 + 0.707f, 0.2f, 1f);
				num *= num4;
				num2 = Mathf.Min(num2, num);
				float f = Mathf.Min(Mathf.Acos(Mathf.Clamp(num3, -1f, 1f)), (20f + 180f * (1f - slowdownFactor * slowdownFactor)) * 0.017453292f);
				float num5 = Mathf.Sin(f);
				float num6 = Mathf.Cos(f);
				num5 *= Mathf.Sign(vector.x * forward.y - vector.y * forward.x);
				return new Vector2(forward.x * num6 + forward.y * num5, forward.y * num6 - forward.x * num5) * num2;
			}
			return Vector2.ClampMagnitude(velocity, num);
		}

		public static Vector2 CalculateAccelerationToReachPoint(Vector2 deltaPosition, Vector2 targetVelocity, Vector2 currentVelocity, float forwardsAcceleration, float rotationSpeed, float maxSpeed, Vector2 forwardsVector)
		{
			if (forwardsAcceleration <= 0f)
			{
				return Vector2.zero;
			}
			float magnitude = currentVelocity.magnitude;
			float num = magnitude * rotationSpeed * 0.017453292f;
			num = Mathf.Max(num, forwardsAcceleration);
			deltaPosition = VectorMath.ComplexMultiplyConjugate(deltaPosition, forwardsVector);
			targetVelocity = VectorMath.ComplexMultiplyConjugate(targetVelocity, forwardsVector);
			currentVelocity = VectorMath.ComplexMultiplyConjugate(currentVelocity, forwardsVector);
			float num2 = 1f / (forwardsAcceleration * forwardsAcceleration);
			float num3 = 1f / (num * num);
			if (targetVelocity == Vector2.zero)
			{
				float num4 = 0.01f;
				float num5 = 10f;
				while (num5 - num4 > 0.01f)
				{
					float num6 = (num5 + num4) * 0.5f;
					Vector2 vector = (6f * deltaPosition - 4f * num6 * currentVelocity) / (num6 * num6);
					Vector2 a = 6f * (num6 * currentVelocity - 2f * deltaPosition) / (num6 * num6 * num6);
					Vector2 vector2 = vector + a * num6;
					if (vector.x * vector.x * num2 + vector.y * vector.y * num3 > 1f || vector2.x * vector2.x * num2 + vector2.y * vector2.y * num3 > 1f)
					{
						num4 = num6;
					}
					else
					{
						num5 = num6;
					}
				}
				Vector2 vector3 = (6f * deltaPosition - 4f * num5 * currentVelocity) / (num5 * num5);
				vector3.y *= 2f;
				float num7 = vector3.x * vector3.x * num2 + vector3.y * vector3.y * num3;
				if (num7 > 1f)
				{
					vector3 /= Mathf.Sqrt(num7);
				}
				return VectorMath.ComplexMultiply(vector3, forwardsVector);
			}
			float num8;
			Vector2 a2 = VectorMath.Normalize(targetVelocity, out num8);
			float magnitude2 = deltaPosition.magnitude;
			Vector2 vector4 = ((deltaPosition - a2 * Math.Min(0.5f * magnitude2 * num8 / (magnitude + num8), maxSpeed * 1.5f)).normalized * maxSpeed - currentVelocity) * 10f;
			float num9 = vector4.x * vector4.x * num2 + vector4.y * vector4.y * num3;
			if (num9 > 1f)
			{
				vector4 /= Mathf.Sqrt(num9);
			}
			return VectorMath.ComplexMultiply(vector4, forwardsVector);
		}
	}
}
