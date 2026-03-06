using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	public static class CurveUtility
	{
		public static float3 EvaluatePosition(BezierCurve curve, float t)
		{
			t = math.clamp(t, 0f, 1f);
			float num = t * t;
			float num2 = num * t;
			return curve.P0 * (-1f * num2 + 3f * num - 3f * t + 1f) + curve.P1 * (3f * num2 - 6f * num + 3f * t) + curve.P2 * (-3f * num2 + 3f * num) + curve.P3 * num2;
		}

		public static float3 EvaluateTangent(BezierCurve curve, float t)
		{
			t = math.clamp(t, 0f, 1f);
			float num = t * t;
			return curve.P0 * (-3f * num + 6f * t - 3f) + curve.P1 * (9f * num - 12f * t + 3f) + curve.P2 * (-9f * num + 6f * t) + curve.P3 * (3f * num);
		}

		public static float3 EvaluateAcceleration(BezierCurve curve, float t)
		{
			t = math.clamp(t, 0f, 1f);
			return curve.P0 * (-6f * t + 6f) + curve.P1 * (18f * t - 12f) + curve.P2 * (-18f * t + 6f) + curve.P3 * (6f * t);
		}

		public static float EvaluateCurvature(BezierCurve curve, float t)
		{
			t = math.clamp(t, 0f, 1f);
			float3 x = CurveUtility.EvaluateTangent(curve, t);
			float3 @float = CurveUtility.EvaluateAcceleration(curve, t);
			float num = math.lengthsq(x);
			float num2 = math.lengthsq(@float);
			float num3 = math.dot(x, @float);
			return math.sqrt(num * num2 - num3 * num3) / (num * math.length(x));
		}

		private static float3 DeCasteljau(BezierCurve curve, float t)
		{
			float3 p = curve.P0;
			float3 p2 = curve.P1;
			float3 p3 = curve.P2;
			float3 p4 = curve.P3;
			float3 start = math.lerp(p, p2, t);
			float3 @float = math.lerp(p2, p3, t);
			float3 end = math.lerp(p3, p4, t);
			float3 start2 = math.lerp(start, @float, t);
			float3 end2 = math.lerp(@float, end, t);
			return math.lerp(start2, end2, t);
		}

		public static void Split(BezierCurve curve, float t, out BezierCurve left, out BezierCurve right)
		{
			t = math.clamp(t, 0f, 1f);
			float3 @float = math.lerp(curve.P0, curve.P1, t);
			float3 float2 = math.lerp(curve.P1, curve.P2, t);
			float3 float3 = math.lerp(curve.P2, curve.P3, t);
			float3 float4 = math.lerp(@float, float2, t);
			float3 float5 = math.lerp(float2, float3, t);
			float3 float6 = math.lerp(float4, float5, t);
			left = new BezierCurve(curve.P0, @float, float4, float6);
			right = new BezierCurve(float6, float5, float3, curve.P3);
		}

		public static float CalculateLength(BezierCurve curve, int resolution = 30)
		{
			float num = 0f;
			float3 rhs = CurveUtility.EvaluatePosition(curve, 0f);
			for (int i = 1; i < resolution; i++)
			{
				float3 @float = CurveUtility.EvaluatePosition(curve, (float)i / ((float)resolution - 1f));
				float3 x = @float - rhs;
				num += math.length(x);
				rhs = @float;
			}
			return num;
		}

		public static void CalculateCurveLengths(BezierCurve curve, DistanceToInterpolation[] lookupTable)
		{
			NativeArray<DistanceToInterpolation> lookupTable2 = new NativeArray<DistanceToInterpolation>(lookupTable, Allocator.Temp);
			CurveUtility.CalculateCurveLengths(curve, lookupTable2);
			lookupTable2.CopyTo(lookupTable);
		}

		public static void CalculateCurveLengths(BezierCurve curve, NativeArray<DistanceToInterpolation> lookupTable)
		{
			int length = lookupTable.Length;
			float num = 0f;
			float3 rhs = CurveUtility.EvaluatePosition(curve, 0f);
			lookupTable[0] = new DistanceToInterpolation
			{
				Distance = 0f,
				T = 0f
			};
			for (int i = 1; i < length; i++)
			{
				float t = (float)i / ((float)length - 1f);
				float3 @float = CurveUtility.EvaluatePosition(curve, t);
				float3 x = @float - rhs;
				num += math.length(x);
				lookupTable[i] = new DistanceToInterpolation
				{
					Distance = num,
					T = t
				};
				rhs = @float;
			}
		}

		private static bool Approximately(float a, float b)
		{
			return math.abs(b - a) < math.max(1E-06f * math.max(math.abs(a), math.abs(b)), 0.0008f);
		}

		public static float ApproximateLength(BezierCurve curve)
		{
			float num = math.length(curve.P3 - curve.P0);
			return (math.length(curve.P0 - curve.P1) + math.length(curve.P2 - curve.P1) + math.length(curve.P3 - curve.P2) + num) / 2f;
		}

		internal static void EvaluateUpVectors(BezierCurve curve, float3 startUp, float3 endUp, NativeArray<float3> upVectors)
		{
			upVectors[0] = startUp;
			upVectors[upVectors.Length - 1] = endUp;
			for (int i = 1; i < upVectors.Length - 1; i++)
			{
				float t = (float)i / (float)(upVectors.Length - 1);
				upVectors[i] = CurveUtility.EvaluateUpVector(curve, t, upVectors[0], endUp, true);
			}
		}

		internal static float3 EvaluateUpVector(BezierCurve curve, float t, float3 startUp, float3 endUp, bool fixEndUpMismatch = true)
		{
			float rhs = math.length(SplineUtility.GetExplicitLinearTangent(curve.P0, curve.P3));
			float3 rhs2 = math.normalize(curve.P3 - curve.P0) * rhs;
			if (CurveUtility.Approximately(math.length(curve.P1 - curve.P0), 0f))
			{
				curve.P1 = curve.P0 + rhs2;
			}
			if (CurveUtility.Approximately(math.length(curve.P2 - curve.P3), 0f))
			{
				curve.P2 = curve.P3 - rhs2;
			}
			NativeArray<float3> nativeArray = new NativeArray<float3>(16, Allocator.Temp, NativeArrayOptions.ClearMemory);
			CurveUtility.FrenetFrame nextRotationMinimizingFrame;
			nextRotationMinimizingFrame.origin = curve.P0;
			nextRotationMinimizingFrame.tangent = curve.P1 - curve.P0;
			nextRotationMinimizingFrame.normal = startUp;
			nextRotationMinimizingFrame.binormal = math.normalize(math.cross(nextRotationMinimizingFrame.tangent, nextRotationMinimizingFrame.normal));
			if (float.IsNaN(nextRotationMinimizingFrame.binormal.x))
			{
				return float3.zero;
			}
			nativeArray[0] = nextRotationMinimizingFrame.normal;
			float num = 0.06666667f;
			float num2 = num;
			float num3 = 0f;
			float3 result = float3.zero;
			for (int i = 1; i < 16; i++)
			{
				CurveUtility.FrenetFrame frenetFrame = nextRotationMinimizingFrame;
				nextRotationMinimizingFrame = CurveUtility.GetNextRotationMinimizingFrame(curve, frenetFrame, num2);
				nativeArray[i] = nextRotationMinimizingFrame.normal;
				if (num3 <= t && num2 >= t)
				{
					float t2 = (t - num3) / num;
					result = Vector3.Slerp(frenetFrame.normal, nextRotationMinimizingFrame.normal, t2);
				}
				num3 = num2;
				num2 += num;
			}
			if (!fixEndUpMismatch)
			{
				return result;
			}
			if (num3 <= t && num2 >= t)
			{
				result = endUp;
			}
			float3 @float = nativeArray[15];
			float num4 = math.acos(math.clamp(math.dot(@float, endUp), -1f, 1f));
			if (num4 == 0f)
			{
				return result;
			}
			float3 axis = math.normalize(nextRotationMinimizingFrame.tangent);
			quaternion q = quaternion.AxisAngle(axis, num4);
			quaternion q2 = quaternion.AxisAngle(axis, -num4);
			float num5 = math.acos(math.clamp(math.dot(math.rotate(q, endUp), @float), -1f, 1f));
			float num6 = math.acos(math.clamp(math.dot(math.rotate(q2, endUp), @float), -1f, 1f));
			if (num5 > num6)
			{
				num4 *= -1f;
			}
			num2 = num;
			num3 = 0f;
			for (int j = 1; j < nativeArray.Length; j++)
			{
				float3 v = nativeArray[j];
				float num7 = math.lerp(0f, num4, num2);
				float3 value = math.rotate(quaternion.AxisAngle(math.normalize(CurveUtility.EvaluateTangent(curve, num2)), -num7), v);
				nativeArray[j] = value;
				if (num3 <= t && num2 >= t)
				{
					float t3 = (t - num3) / num;
					return Vector3.Slerp(nativeArray[j - 1], nativeArray[j], t3);
				}
				num3 = num2;
				num2 += num;
			}
			return endUp;
		}

		private static CurveUtility.FrenetFrame GetNextRotationMinimizingFrame(BezierCurve curve, CurveUtility.FrenetFrame previousRMFrame, float nextRMFrameT)
		{
			CurveUtility.FrenetFrame frenetFrame;
			frenetFrame.origin = CurveUtility.EvaluatePosition(curve, nextRMFrameT);
			frenetFrame.tangent = CurveUtility.EvaluateTangent(curve, nextRMFrameT);
			float3 @float = frenetFrame.origin - previousRMFrame.origin;
			float rhs = math.dot(@float, @float);
			float3 float2 = previousRMFrame.binormal - @float * 2f / rhs * math.dot(@float, previousRMFrame.binormal);
			float3 rhs2 = previousRMFrame.tangent - @float * 2f / rhs * math.dot(@float, previousRMFrame.tangent);
			float3 float3 = frenetFrame.tangent - rhs2;
			float rhs3 = math.dot(float3, float3);
			frenetFrame.binormal = math.normalize(float2 - float3 * 2f / rhs3 * math.dot(float3, float2));
			frenetFrame.normal = math.normalize(math.cross(frenetFrame.binormal, frenetFrame.tangent));
			return frenetFrame;
		}

		public static float GetDistanceToInterpolation(BezierCurve curve, float distance)
		{
			CurveUtility.CalculateCurveLengths(curve, CurveUtility.k_DistanceLUT);
			return CurveUtility.GetDistanceToInterpolation<DistanceToInterpolation[]>(CurveUtility.k_DistanceLUT, distance);
		}

		public static float GetDistanceToInterpolation<T>(T lut, float distance) where T : IReadOnlyList<DistanceToInterpolation>
		{
			if (lut == null || lut.Count < 1 || distance <= 0f)
			{
				return 0f;
			}
			int count = lut.Count;
			float distance2 = lut[count - 1].Distance;
			if (distance >= distance2)
			{
				return 1f;
			}
			DistanceToInterpolation distanceToInterpolation = lut[0];
			for (int i = 1; i < count; i++)
			{
				DistanceToInterpolation distanceToInterpolation2 = lut[i];
				if (distance < distanceToInterpolation2.Distance)
				{
					return math.lerp(distanceToInterpolation.T, distanceToInterpolation2.T, (distance - distanceToInterpolation.Distance) / (distanceToInterpolation2.Distance - distanceToInterpolation.Distance));
				}
				distanceToInterpolation = distanceToInterpolation2;
			}
			return 1f;
		}

		public static float3 GetNearestPoint(BezierCurve curve, Ray ray, int resolution = 16)
		{
			float3 result;
			float num;
			CurveUtility.GetNearestPoint(curve, ray, out result, out num, resolution);
			return result;
		}

		public static float GetNearestPoint(BezierCurve curve, Ray ray, out float3 position, out float interpolation, int resolution = 16)
		{
			float num = float.PositiveInfinity;
			float num2 = 0f;
			interpolation = 0f;
			position = float3.zero;
			float3 a = CurveUtility.EvaluatePosition(curve, 0f);
			float3 ro = ray.origin;
			float3 rd = ray.direction;
			for (int i = 1; i < resolution; i++)
			{
				float t = (float)i / ((float)resolution - 1f);
				float3 @float = CurveUtility.EvaluatePosition(curve, t);
				float num3;
				float num4;
				ValueTuple<float3, float3> valueTuple = SplineMath.RayLineNearestPoint(ro, rd, a, @float, out num3, out num4);
				float3 item = valueTuple.Item1;
				float3 item2 = valueTuple.Item2;
				float num5 = math.lengthsq(item2 - item);
				if (num5 < num)
				{
					position = item2;
					num = num5;
					num2 = num4;
					interpolation = (float)(i - 1) / ((float)resolution - 1f);
				}
				a = @float;
			}
			interpolation += num2 * (1f / ((float)resolution - 1f));
			return math.sqrt(num);
		}

		private const int k_NormalsPerCurve = 16;

		private const float k_Epsilon = 0.0001f;

		private static readonly DistanceToInterpolation[] k_DistanceLUT = new DistanceToInterpolation[24];

		private struct FrenetFrame
		{
			public float3 origin;

			public float3 tangent;

			public float3 normal;

			public float3 binormal;
		}
	}
}
