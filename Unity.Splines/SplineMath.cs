using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	public static class SplineMath
	{
		public static float RayLineParameter(float3 ro, float3 rd, float3 lineOrigin, float3 lineDir)
		{
			float3 x = ro - lineOrigin;
			float3 y = math.cross(rd, math.cross(rd, lineDir));
			return math.dot(x, y) / math.dot(lineDir, y);
		}

		public static float3 RayLineDistance(float3 ro, float3 rd, float3 a, float3 b)
		{
			ValueTuple<float3, float3> valueTuple = SplineMath.RayLineNearestPoint(ro, rd, a, b);
			return valueTuple.Item2 - valueTuple.Item1;
		}

		[return: TupleElementNames(new string[]
		{
			"rayPoint",
			"linePoint"
		})]
		public static ValueTuple<float3, float3> RayLineNearestPoint(float3 ro, float3 rd, float3 a, float3 b)
		{
			float num;
			float num2;
			return SplineMath.RayLineNearestPoint(ro, rd, a, b, out num, out num2);
		}

		[return: TupleElementNames(new string[]
		{
			"rayPoint",
			"linePoint"
		})]
		public static ValueTuple<float3, float3> RayLineNearestPoint(float3 ro, float3 rd, float3 a, float3 b, out float rayParam, out float lineParam)
		{
			float3 @float = b - a;
			lineParam = SplineMath.RayLineParameter(ro, rd, a, @float);
			float3 float2 = a + @float * math.saturate(lineParam);
			rayParam = math.dot(rd, float2 - ro);
			return new ValueTuple<float3, float3>(ro + rd * rayParam, float2);
		}

		public static float3 PointLineNearestPoint(float3 p, float3 a, float3 b, out float lineParam)
		{
			float3 @float = b - a;
			float num = math.length(@float);
			float3 float2 = math.select(0f, @float * (1f / num), num > 1.1754944E-38f);
			lineParam = math.dot(float2, p - a);
			return a + float2 * math.clamp(lineParam, 0f, num);
		}

		public static float DistancePointLine(float3 p, float3 a, float3 b)
		{
			float num;
			return math.length(SplineMath.PointLineNearestPoint(p, a, b, out num) - p);
		}

		internal static float GetUnitCircleTangentLength()
		{
			return 4f * (math.sqrt(2f) - 1f) / 3f;
		}
	}
}
