using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Curves
{
	[BurstCompile]
	internal static class CurveUtility
	{
		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.SampleQuadraticBezierPoint_0000043F$PostfixBurstDelegate))]
		public static void SampleQuadraticBezierPoint(in float3 p0, in float3 p1, in float3 p2, float t, out float3 point)
		{
			CurveUtility.SampleQuadraticBezierPoint_0000043F$BurstDirectCall.Invoke(p0, p1, p2, t, out point);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.SampleCubicBezierPoint_00000440$PostfixBurstDelegate))]
		public static void SampleCubicBezierPoint(in float3 p0, in float3 p1, in float3 p2, in float3 p3, float t, out float3 point)
		{
			CurveUtility.SampleCubicBezierPoint_00000440$BurstDirectCall.Invoke(p0, p1, p2, p3, t, out point);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.ElevateQuadraticToCubicBezier_00000441$PostfixBurstDelegate))]
		public static void ElevateQuadraticToCubicBezier(in float3 p0, in float3 p1, in float3 p2, out float3 c0, out float3 c1, out float3 c2, out float3 c3)
		{
			CurveUtility.ElevateQuadraticToCubicBezier_00000441$BurstDirectCall.Invoke(p0, p1, p2, out c0, out c1, out c2, out c3);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.GenerateCubicBezierCurve_00000442$PostfixBurstDelegate))]
		public static void GenerateCubicBezierCurve(int numTargetPoints, float curveRatio, in float3 lineOrigin, in float3 lineDirection, in float3 endPoint, ref NativeArray<float3> targetPoints)
		{
			CurveUtility.GenerateCubicBezierCurve_00000442$BurstDirectCall.Invoke(numTargetPoints, curveRatio, lineOrigin, lineDirection, endPoint, ref targetPoints);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.TryGenerateCubicBezierCurve_00000443$PostfixBurstDelegate))]
		public static bool TryGenerateCubicBezierCurve(int numTargetPoints, float curveRatio, in float3 curveOrigin, in float3 curveDirection, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f)
		{
			return CurveUtility.TryGenerateCubicBezierCurve_00000443$BurstDirectCall.Invoke(numTargetPoints, curveRatio, curveOrigin, curveDirection, endPoint, ref targetPoints, minLineLength, startOffset, endOffset);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.TryGenerateCubicBezierCurve_00000444$PostfixBurstDelegate))]
		public static bool TryGenerateCubicBezierCurve(int numTargetPoints, in float3 curveOrigin, in float3 midPoint, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f)
		{
			return CurveUtility.TryGenerateCubicBezierCurve_00000444$BurstDirectCall.Invoke(numTargetPoints, curveOrigin, midPoint, endPoint, ref targetPoints, minLineLength, startOffset, endOffset);
		}

		private static bool TryGenerateCubicBezierCurveCore(int numTargetPoints, in float3 curveOrigin, in float3 midPoint, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f)
		{
			float3 @float;
			float3 float2;
			float3 float3;
			float3 float4;
			CurveUtility.ElevateQuadraticToCubicBezier(curveOrigin, midPoint, endPoint, out @float, out float2, out float3, out float4);
			bool flag = startOffset > 0f;
			bool flag2 = endOffset > 0f;
			float num = 0f;
			float num2 = 1f;
			if (flag || flag2)
			{
				float num3 = startOffset + endOffset;
				float num4 = CurveUtility.ApproximateCubicBezierLength(@float, float2, float3, float4, math.max(numTargetPoints / 2, 4));
				if (num3 > num4 || num4 - num3 < minLineLength)
				{
					return false;
				}
				if (flag)
				{
					num = startOffset / num4;
				}
				if (flag2)
				{
					num2 = (num4 - endOffset) / num4;
				}
			}
			float num5 = (num2 - num) / (float)(numTargetPoints - 1);
			for (int i = 0; i < numTargetPoints; i++)
			{
				float t = num + (float)i * num5;
				float3 value;
				CurveUtility.SampleCubicBezierPoint(@float, float2, float3, float4, t, out value);
				targetPoints[i] = value;
			}
			return true;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.ApproximateCubicBezierLength_00000446$PostfixBurstDelegate))]
		public static float ApproximateCubicBezierLength(in float3 p0, in float3 p1, in float3 p2, in float3 p3, int subdivisions)
		{
			return CurveUtility.ApproximateCubicBezierLength_00000446$BurstDirectCall.Invoke(p0, p1, p2, p3, subdivisions);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.SampleProjectilePoint_00000447$PostfixBurstDelegate))]
		public static void SampleProjectilePoint(in float3 initialPosition, in float3 initialVelocity, in float3 constantAcceleration, float time, out float3 point)
		{
			CurveUtility.SampleProjectilePoint_00000447$BurstDirectCall.Invoke(initialPosition, initialVelocity, constantAcceleration, time, out point);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(CurveUtility.CalculateProjectileFlightTime_00000448$PostfixBurstDelegate))]
		public static void CalculateProjectileFlightTime(float velocityMagnitude, float gravityAcceleration, float angleRad, float height, float extraFlightTime, out float flightTime)
		{
			CurveUtility.CalculateProjectileFlightTime_00000448$BurstDirectCall.Invoke(velocityMagnitude, gravityAcceleration, angleRad, height, extraFlightTime, out flightTime);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SampleQuadraticBezierPoint$BurstManaged(in float3 p0, in float3 p1, in float3 p2, float t, out float3 point)
		{
			float num = 1f - t;
			float lhs = num * num;
			float lhs2 = t * t;
			point = lhs * p0 + 2f * num * t * p1 + lhs2 * p2;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SampleCubicBezierPoint$BurstManaged(in float3 p0, in float3 p1, in float3 p2, in float3 p3, float t, out float3 point)
		{
			float num = 1f - t;
			float num2 = num * num;
			float lhs = num2 * num;
			float num3 = t * t;
			float lhs2 = num3 * t;
			point = lhs * p0 + 3f * num2 * t * p1 + 3f * num * num3 * p2 + lhs2 * p3;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ElevateQuadraticToCubicBezier$BurstManaged(in float3 p0, in float3 p1, in float3 p2, out float3 c0, out float3 c1, out float3 c2, out float3 c3)
		{
			c0 = p0;
			c1 = p0 + 0.6666667f * (p1 - p0);
			c2 = p2 + 0.6666667f * (p1 - p2);
			c3 = p2;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GenerateCubicBezierCurve$BurstManaged(int numTargetPoints, float curveRatio, in float3 lineOrigin, in float3 lineDirection, in float3 endPoint, ref NativeArray<float3> targetPoints)
		{
			float rhs = math.length(endPoint - lineOrigin);
			float3 @float = lineOrigin + lineDirection * rhs * curveRatio;
			float3 float2;
			float3 float3;
			float3 float4;
			float3 float5;
			CurveUtility.ElevateQuadraticToCubicBezier(lineOrigin, @float, endPoint, out float2, out float3, out float4, out float5);
			targetPoints[0] = lineOrigin;
			float num = 1f / (float)(numTargetPoints - 1);
			for (int i = 1; i < numTargetPoints; i++)
			{
				float t = (float)i * num;
				float3 value;
				CurveUtility.SampleCubicBezierPoint(float2, float3, float4, float5, t, out value);
				targetPoints[i] = value;
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool TryGenerateCubicBezierCurve$BurstManaged(int numTargetPoints, float curveRatio, in float3 curveOrigin, in float3 curveDirection, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f)
		{
			float num = math.length(endPoint - curveOrigin);
			float num2 = startOffset + endOffset;
			if (num2 > num || num - num2 < minLineLength)
			{
				return false;
			}
			float3 @float;
			if (curveRatio > 0f)
			{
				@float = curveOrigin + curveDirection * num * curveRatio;
			}
			else
			{
				@float = math.lerp(curveOrigin, endPoint, 0.5f);
			}
			return CurveUtility.TryGenerateCubicBezierCurveCore(numTargetPoints, curveOrigin, @float, endPoint, ref targetPoints, minLineLength, startOffset, endOffset);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool TryGenerateCubicBezierCurve$BurstManaged(int numTargetPoints, in float3 curveOrigin, in float3 midPoint, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f)
		{
			float num = math.length(endPoint - curveOrigin);
			float num2 = startOffset + endOffset;
			return num2 <= num && num - num2 >= minLineLength && CurveUtility.TryGenerateCubicBezierCurveCore(numTargetPoints, curveOrigin, midPoint, endPoint, ref targetPoints, minLineLength, startOffset, endOffset);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float ApproximateCubicBezierLength$BurstManaged(in float3 p0, in float3 p1, in float3 p2, in float3 p3, int subdivisions)
		{
			float num = 0f;
			float3 y = p0;
			for (int i = 1; i <= subdivisions; i++)
			{
				float t = (float)i / (float)subdivisions;
				float3 @float;
				CurveUtility.SampleCubicBezierPoint(p0, p1, p2, p3, t, out @float);
				num += math.distance(@float, y);
				y = @float;
			}
			return num;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SampleProjectilePoint$BurstManaged(in float3 initialPosition, in float3 initialVelocity, in float3 constantAcceleration, float time, out float3 point)
		{
			point = initialPosition + initialVelocity * time + constantAcceleration * (0.5f * time * time);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CalculateProjectileFlightTime$BurstManaged(float velocityMagnitude, float gravityAcceleration, float angleRad, float height, float extraFlightTime, out float flightTime)
		{
			float num = velocityMagnitude * angleRad;
			if (height < 0f)
			{
				flightTime = 0f;
			}
			else if (math.abs(height) < 9.536743E-07f)
			{
				flightTime = 2f * num / gravityAcceleration;
			}
			else
			{
				flightTime = (num + math.sqrt(num * num + 2f * gravityAcceleration * height)) / gravityAcceleration;
			}
			flightTime = math.max(flightTime + extraFlightTime, 0f);
		}

		private const float k_EightEpsilon = 9.536743E-07f;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void SampleQuadraticBezierPoint_0000043F$PostfixBurstDelegate(in float3 p0, in float3 p1, in float3 p2, float t, out float3 point);

		internal static class SampleQuadraticBezierPoint_0000043F$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.SampleQuadraticBezierPoint_0000043F$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.SampleQuadraticBezierPoint_0000043F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.SampleQuadraticBezierPoint_0000043F$PostfixBurstDelegate>(new CurveUtility.SampleQuadraticBezierPoint_0000043F$PostfixBurstDelegate(CurveUtility.SampleQuadraticBezierPoint)).Value;
				}
				A_0 = CurveUtility.SampleQuadraticBezierPoint_0000043F$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.SampleQuadraticBezierPoint_0000043F$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 p0, in float3 p1, in float3 p2, float t, out float3 point)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.SampleQuadraticBezierPoint_0000043F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&), ref p0, ref p1, ref p2, t, ref point, functionPointer);
						return;
					}
				}
				CurveUtility.SampleQuadraticBezierPoint$BurstManaged(p0, p1, p2, t, out point);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void SampleCubicBezierPoint_00000440$PostfixBurstDelegate(in float3 p0, in float3 p1, in float3 p2, in float3 p3, float t, out float3 point);

		internal static class SampleCubicBezierPoint_00000440$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.SampleCubicBezierPoint_00000440$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.SampleCubicBezierPoint_00000440$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.SampleCubicBezierPoint_00000440$PostfixBurstDelegate>(new CurveUtility.SampleCubicBezierPoint_00000440$PostfixBurstDelegate(CurveUtility.SampleCubicBezierPoint)).Value;
				}
				A_0 = CurveUtility.SampleCubicBezierPoint_00000440$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.SampleCubicBezierPoint_00000440$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 p0, in float3 p1, in float3 p2, in float3 p3, float t, out float3 point)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.SampleCubicBezierPoint_00000440$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&), ref p0, ref p1, ref p2, ref p3, t, ref point, functionPointer);
						return;
					}
				}
				CurveUtility.SampleCubicBezierPoint$BurstManaged(p0, p1, p2, p3, t, out point);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ElevateQuadraticToCubicBezier_00000441$PostfixBurstDelegate(in float3 p0, in float3 p1, in float3 p2, out float3 c0, out float3 c1, out float3 c2, out float3 c3);

		internal static class ElevateQuadraticToCubicBezier_00000441$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.ElevateQuadraticToCubicBezier_00000441$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.ElevateQuadraticToCubicBezier_00000441$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.ElevateQuadraticToCubicBezier_00000441$PostfixBurstDelegate>(new CurveUtility.ElevateQuadraticToCubicBezier_00000441$PostfixBurstDelegate(CurveUtility.ElevateQuadraticToCubicBezier)).Value;
				}
				A_0 = CurveUtility.ElevateQuadraticToCubicBezier_00000441$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.ElevateQuadraticToCubicBezier_00000441$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 p0, in float3 p1, in float3 p2, out float3 c0, out float3 c1, out float3 c2, out float3 c3)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.ElevateQuadraticToCubicBezier_00000441$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&), ref p0, ref p1, ref p2, ref c0, ref c1, ref c2, ref c3, functionPointer);
						return;
					}
				}
				CurveUtility.ElevateQuadraticToCubicBezier$BurstManaged(p0, p1, p2, out c0, out c1, out c2, out c3);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GenerateCubicBezierCurve_00000442$PostfixBurstDelegate(int numTargetPoints, float curveRatio, in float3 lineOrigin, in float3 lineDirection, in float3 endPoint, ref NativeArray<float3> targetPoints);

		internal static class GenerateCubicBezierCurve_00000442$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.GenerateCubicBezierCurve_00000442$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.GenerateCubicBezierCurve_00000442$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.GenerateCubicBezierCurve_00000442$PostfixBurstDelegate>(new CurveUtility.GenerateCubicBezierCurve_00000442$PostfixBurstDelegate(CurveUtility.GenerateCubicBezierCurve)).Value;
				}
				A_0 = CurveUtility.GenerateCubicBezierCurve_00000442$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.GenerateCubicBezierCurve_00000442$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(int numTargetPoints, float curveRatio, in float3 lineOrigin, in float3 lineDirection, in float3 endPoint, ref NativeArray<float3> targetPoints)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.GenerateCubicBezierCurve_00000442$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Int32,System.Single,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Collections.NativeArray`1<Unity.Mathematics.float3>&), numTargetPoints, curveRatio, ref lineOrigin, ref lineDirection, ref endPoint, ref targetPoints, functionPointer);
						return;
					}
				}
				CurveUtility.GenerateCubicBezierCurve$BurstManaged(numTargetPoints, curveRatio, lineOrigin, lineDirection, endPoint, ref targetPoints);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool TryGenerateCubicBezierCurve_00000443$PostfixBurstDelegate(int numTargetPoints, float curveRatio, in float3 curveOrigin, in float3 curveDirection, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f);

		internal static class TryGenerateCubicBezierCurve_00000443$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.TryGenerateCubicBezierCurve_00000443$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.TryGenerateCubicBezierCurve_00000443$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.TryGenerateCubicBezierCurve_00000443$PostfixBurstDelegate>(new CurveUtility.TryGenerateCubicBezierCurve_00000443$PostfixBurstDelegate(CurveUtility.TryGenerateCubicBezierCurve)).Value;
				}
				A_0 = CurveUtility.TryGenerateCubicBezierCurve_00000443$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.TryGenerateCubicBezierCurve_00000443$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(int numTargetPoints, float curveRatio, in float3 curveOrigin, in float3 curveDirection, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.TryGenerateCubicBezierCurve_00000443$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(System.Int32,System.Single,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Collections.NativeArray`1<Unity.Mathematics.float3>&,System.Single,System.Single,System.Single), numTargetPoints, curveRatio, ref curveOrigin, ref curveDirection, ref endPoint, ref targetPoints, minLineLength, startOffset, endOffset, functionPointer);
					}
				}
				return CurveUtility.TryGenerateCubicBezierCurve$BurstManaged(numTargetPoints, curveRatio, curveOrigin, curveDirection, endPoint, ref targetPoints, minLineLength, startOffset, endOffset);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool TryGenerateCubicBezierCurve_00000444$PostfixBurstDelegate(int numTargetPoints, in float3 curveOrigin, in float3 midPoint, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f);

		internal static class TryGenerateCubicBezierCurve_00000444$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.TryGenerateCubicBezierCurve_00000444$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.TryGenerateCubicBezierCurve_00000444$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.TryGenerateCubicBezierCurve_00000444$PostfixBurstDelegate>(new CurveUtility.TryGenerateCubicBezierCurve_00000444$PostfixBurstDelegate(CurveUtility.TryGenerateCubicBezierCurve)).Value;
				}
				A_0 = CurveUtility.TryGenerateCubicBezierCurve_00000444$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.TryGenerateCubicBezierCurve_00000444$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(int numTargetPoints, in float3 curveOrigin, in float3 midPoint, in float3 endPoint, ref NativeArray<float3> targetPoints, float minLineLength = 0.005f, float startOffset = 0f, float endOffset = 0f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.TryGenerateCubicBezierCurve_00000444$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(System.Int32,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Collections.NativeArray`1<Unity.Mathematics.float3>&,System.Single,System.Single,System.Single), numTargetPoints, ref curveOrigin, ref midPoint, ref endPoint, ref targetPoints, minLineLength, startOffset, endOffset, functionPointer);
					}
				}
				return CurveUtility.TryGenerateCubicBezierCurve$BurstManaged(numTargetPoints, curveOrigin, midPoint, endPoint, ref targetPoints, minLineLength, startOffset, endOffset);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate float ApproximateCubicBezierLength_00000446$PostfixBurstDelegate(in float3 p0, in float3 p1, in float3 p2, in float3 p3, int subdivisions);

		internal static class ApproximateCubicBezierLength_00000446$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.ApproximateCubicBezierLength_00000446$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.ApproximateCubicBezierLength_00000446$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.ApproximateCubicBezierLength_00000446$PostfixBurstDelegate>(new CurveUtility.ApproximateCubicBezierLength_00000446$PostfixBurstDelegate(CurveUtility.ApproximateCubicBezierLength)).Value;
				}
				A_0 = CurveUtility.ApproximateCubicBezierLength_00000446$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.ApproximateCubicBezierLength_00000446$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static float Invoke(in float3 p0, in float3 p1, in float3 p2, in float3 p3, int subdivisions)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.ApproximateCubicBezierLength_00000446$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Single(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Int32), ref p0, ref p1, ref p2, ref p3, subdivisions, functionPointer);
					}
				}
				return CurveUtility.ApproximateCubicBezierLength$BurstManaged(p0, p1, p2, p3, subdivisions);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void SampleProjectilePoint_00000447$PostfixBurstDelegate(in float3 initialPosition, in float3 initialVelocity, in float3 constantAcceleration, float time, out float3 point);

		internal static class SampleProjectilePoint_00000447$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.SampleProjectilePoint_00000447$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.SampleProjectilePoint_00000447$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.SampleProjectilePoint_00000447$PostfixBurstDelegate>(new CurveUtility.SampleProjectilePoint_00000447$PostfixBurstDelegate(CurveUtility.SampleProjectilePoint)).Value;
				}
				A_0 = CurveUtility.SampleProjectilePoint_00000447$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.SampleProjectilePoint_00000447$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 initialPosition, in float3 initialVelocity, in float3 constantAcceleration, float time, out float3 point)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.SampleProjectilePoint_00000447$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&), ref initialPosition, ref initialVelocity, ref constantAcceleration, time, ref point, functionPointer);
						return;
					}
				}
				CurveUtility.SampleProjectilePoint$BurstManaged(initialPosition, initialVelocity, constantAcceleration, time, out point);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateProjectileFlightTime_00000448$PostfixBurstDelegate(float velocityMagnitude, float gravityAcceleration, float angleRad, float height, float extraFlightTime, out float flightTime);

		internal static class CalculateProjectileFlightTime_00000448$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (CurveUtility.CalculateProjectileFlightTime_00000448$BurstDirectCall.Pointer == 0)
				{
					CurveUtility.CalculateProjectileFlightTime_00000448$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<CurveUtility.CalculateProjectileFlightTime_00000448$PostfixBurstDelegate>(new CurveUtility.CalculateProjectileFlightTime_00000448$PostfixBurstDelegate(CurveUtility.CalculateProjectileFlightTime)).Value;
				}
				A_0 = CurveUtility.CalculateProjectileFlightTime_00000448$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				CurveUtility.CalculateProjectileFlightTime_00000448$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(float velocityMagnitude, float gravityAcceleration, float angleRad, float height, float extraFlightTime, out float flightTime)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = CurveUtility.CalculateProjectileFlightTime_00000448$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Single,System.Single,System.Single,System.Single,System.Single,System.Single&), velocityMagnitude, gravityAcceleration, angleRad, height, extraFlightTime, ref flightTime, functionPointer);
						return;
					}
				}
				CurveUtility.CalculateProjectileFlightTime$BurstManaged(velocityMagnitude, gravityAcceleration, angleRad, height, extraFlightTime, out flightTime);
			}

			private static IntPtr Pointer;
		}
	}
}
