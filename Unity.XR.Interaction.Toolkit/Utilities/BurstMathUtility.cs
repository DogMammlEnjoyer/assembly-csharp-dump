using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	[BurstCompile]
	public static class BurstMathUtility
	{
		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.OrthogonalUpVector_0000034D$PostfixBurstDelegate))]
		public static void OrthogonalUpVector(in Vector3 forward, in Vector3 referenceUp, out Vector3 orthogonalUp)
		{
			BurstMathUtility.OrthogonalUpVector_0000034D$BurstDirectCall.Invoke(forward, referenceUp, out orthogonalUp);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.OrthogonalUpVector_0000034E$PostfixBurstDelegate))]
		public static void OrthogonalUpVector(in float3 forward, in float3 referenceUp, out float3 orthogonalUp)
		{
			BurstMathUtility.OrthogonalUpVector_0000034E$BurstDirectCall.Invoke(forward, referenceUp, out orthogonalUp);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.OrthogonalLookRotation_0000034F$PostfixBurstDelegate))]
		public static void OrthogonalLookRotation(in Vector3 forward, in Vector3 referenceUp, out Quaternion lookRotation)
		{
			BurstMathUtility.OrthogonalLookRotation_0000034F$BurstDirectCall.Invoke(forward, referenceUp, out lookRotation);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.OrthogonalLookRotation_00000350$PostfixBurstDelegate))]
		public static void OrthogonalLookRotation(in float3 forward, in float3 referenceUp, out quaternion lookRotation)
		{
			BurstMathUtility.OrthogonalLookRotation_00000350$BurstDirectCall.Invoke(forward, referenceUp, out lookRotation);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.ProjectOnPlane_00000351$PostfixBurstDelegate))]
		public static void ProjectOnPlane(in float3 vector, in float3 planeNormal, out float3 projectedVector)
		{
			BurstMathUtility.ProjectOnPlane_00000351$BurstDirectCall.Invoke(vector, planeNormal, out projectedVector);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.ProjectOnPlane_00000352$PostfixBurstDelegate))]
		public static void ProjectOnPlane(in Vector3 vector, in Vector3 planeNormal, out Vector3 projectedVector)
		{
			BurstMathUtility.ProjectOnPlane_00000352$BurstDirectCall.Invoke(vector, planeNormal, out projectedVector);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$PostfixBurstDelegate))]
		public static void LookRotationWithForwardProjectedOnPlane(in float3 forward, in float3 planeNormal, out quaternion lookRotation)
		{
			BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$BurstDirectCall.Invoke(forward, planeNormal, out lookRotation);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$PostfixBurstDelegate))]
		public static void LookRotationWithForwardProjectedOnPlane(in Vector3 forward, in Vector3 planeNormal, out Quaternion lookRotation)
		{
			BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$BurstDirectCall.Invoke(forward, planeNormal, out lookRotation);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.Angle_00000355$PostfixBurstDelegate))]
		public static void Angle(in quaternion a, in quaternion b, out float angle)
		{
			BurstMathUtility.Angle_00000355$BurstDirectCall.Invoke(a, b, out angle);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.Angle_00000356$PostfixBurstDelegate))]
		public static void Angle(in Vector3 a, in Vector3 b, out float angle)
		{
			BurstMathUtility.Angle_00000356$BurstDirectCall.Invoke(a, b, out angle);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.FastVectorEquals_00000357$PostfixBurstDelegate))]
		public static bool FastVectorEquals(in float3 a, in float3 b, float tolerance = 0.0001f)
		{
			return BurstMathUtility.FastVectorEquals_00000357$BurstDirectCall.Invoke(a, b, tolerance);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.FastVectorEquals_00000358$PostfixBurstDelegate))]
		public static bool FastVectorEquals(in Vector3 a, in Vector3 b, float tolerance = 0.0001f)
		{
			return BurstMathUtility.FastVectorEquals_00000358$BurstDirectCall.Invoke(a, b, tolerance);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.FastSafeDivide_00000359$PostfixBurstDelegate))]
		public static void FastSafeDivide(in Vector3 a, in Vector3 b, out Vector3 result, float tolerance = 1E-06f)
		{
			BurstMathUtility.FastSafeDivide_00000359$BurstDirectCall.Invoke(a, b, out result, tolerance);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.FastSafeDivide_0000035A$PostfixBurstDelegate))]
		public static void FastSafeDivide(in float3 a, in float3 b, out float3 result, float tolerance = 1E-06f)
		{
			BurstMathUtility.FastSafeDivide_0000035A$BurstDirectCall.Invoke(a, b, out result, tolerance);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.Scale_0000035B$PostfixBurstDelegate))]
		public static void Scale(in float3 a, in float3 b, out float3 result)
		{
			BurstMathUtility.Scale_0000035B$BurstDirectCall.Invoke(a, b, out result);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.Scale_0000035C$PostfixBurstDelegate))]
		public static void Scale(in Vector3 a, in Vector3 b, out Vector3 result)
		{
			BurstMathUtility.Scale_0000035C$BurstDirectCall.Invoke(a, b, out result);
		}

		public static Vector3 Orthogonal(Vector3 input)
		{
			float3 @float = input;
			float3 v;
			BurstMathUtility.Orthogonal(@float, out v);
			return v;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstMathUtility.Orthogonal_0000035E$PostfixBurstDelegate))]
		public static void Orthogonal(in float3 input, out float3 result)
		{
			BurstMathUtility.Orthogonal_0000035E$BurstDirectCall.Invoke(input, out result);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OrthogonalUpVector$BurstManaged(in Vector3 forward, in Vector3 referenceUp, out Vector3 orthogonalUp)
		{
			float3 @float = forward;
			float3 float2 = referenceUp;
			float3 v;
			BurstMathUtility.OrthogonalUpVector(@float, float2, out v);
			orthogonalUp = v;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OrthogonalUpVector$BurstManaged(in float3 forward, in float3 referenceUp, out float3 orthogonalUp)
		{
			float3 y = -math.cross(forward, referenceUp);
			orthogonalUp = math.cross(forward, y);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OrthogonalLookRotation$BurstManaged(in Vector3 forward, in Vector3 referenceUp, out Quaternion lookRotation)
		{
			float3 @float = forward;
			float3 float2 = referenceUp;
			quaternion q;
			BurstMathUtility.OrthogonalLookRotation(@float, float2, out q);
			lookRotation = q;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OrthogonalLookRotation$BurstManaged(in float3 forward, in float3 referenceUp, out quaternion lookRotation)
		{
			float3 up;
			BurstMathUtility.OrthogonalUpVector(forward, referenceUp, out up);
			lookRotation = quaternion.LookRotation(forward, up);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ProjectOnPlane$BurstManaged(in float3 vector, in float3 planeNormal, out float3 projectedVector)
		{
			float num = math.dot(planeNormal, planeNormal);
			if (num < 1.1920929E-07f)
			{
				projectedVector = vector;
				return;
			}
			float num2 = math.dot(vector, planeNormal);
			projectedVector = new float3(vector.x - planeNormal.x * num2 / num, vector.y - planeNormal.y * num2 / num, vector.z - planeNormal.z * num2 / num);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ProjectOnPlane$BurstManaged(in Vector3 vector, in Vector3 planeNormal, out Vector3 projectedVector)
		{
			float3 @float = vector;
			float3 float2 = planeNormal;
			float3 v;
			BurstMathUtility.ProjectOnPlane(@float, float2, out v);
			projectedVector = v;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LookRotationWithForwardProjectedOnPlane$BurstManaged(in float3 forward, in float3 planeNormal, out quaternion lookRotation)
		{
			float3 forward2;
			BurstMathUtility.ProjectOnPlane(forward, planeNormal, out forward2);
			lookRotation = quaternion.LookRotation(forward2, planeNormal);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void LookRotationWithForwardProjectedOnPlane$BurstManaged(in Vector3 forward, in Vector3 planeNormal, out Quaternion lookRotation)
		{
			float3 @float = forward;
			float3 float2 = planeNormal;
			quaternion q;
			BurstMathUtility.LookRotationWithForwardProjectedOnPlane(@float, float2, out q);
			lookRotation = q;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Angle$BurstManaged(in quaternion a, in quaternion b, out float angle)
		{
			float num = math.min(math.abs(math.dot(a, b)), 1f);
			angle = ((num > 0.999999f) ? 0f : (math.acos(num) * 2f * 57.29578f));
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Angle$BurstManaged(in Vector3 a, in Vector3 b, out float angle)
		{
			Vector3 vector = a;
			float sqrMagnitude = vector.sqrMagnitude;
			vector = b;
			float num = math.sqrt(sqrMagnitude * vector.sqrMagnitude);
			if (num < 1E-15f)
			{
				angle = 0f;
				return;
			}
			float x = math.clamp(math.dot(a, b) / num, -1f, 1f);
			angle = math.acos(x) * 57.29578f;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool FastVectorEquals$BurstManaged(in float3 a, in float3 b, float tolerance = 0.0001f)
		{
			return math.abs(a.x - b.x) < tolerance && math.abs(a.y - b.y) < tolerance && math.abs(a.z - b.z) < tolerance;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool FastVectorEquals$BurstManaged(in Vector3 a, in Vector3 b, float tolerance = 0.0001f)
		{
			return math.abs(a.x - b.x) < tolerance && math.abs(a.y - b.y) < tolerance && math.abs(a.z - b.z) < tolerance;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FastSafeDivide$BurstManaged(in Vector3 a, in Vector3 b, out Vector3 result, float tolerance = 1E-06f)
		{
			float3 @float = a;
			float3 float2 = b;
			float3 v;
			BurstMathUtility.FastSafeDivide(@float, float2, out v, tolerance);
			result = v;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FastSafeDivide$BurstManaged(in float3 a, in float3 b, out float3 result, float tolerance = 1E-06f)
		{
			result = default(float3);
			if (math.abs(a.x - b.x) > tolerance)
			{
				result.x = a.x / b.x;
			}
			if (math.abs(a.y - b.y) > tolerance)
			{
				result.y = a.y / b.y;
			}
			if (math.abs(a.z - b.z) > tolerance)
			{
				result.z = a.z / b.z;
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Scale$BurstManaged(in float3 a, in float3 b, out float3 result)
		{
			result = new float3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Scale$BurstManaged(in Vector3 a, in Vector3 b, out Vector3 result)
		{
			result = new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Orthogonal$BurstManaged(in float3 input, out float3 result)
		{
			if (math.abs(input.x) < math.abs(input.y) && math.abs(input.x) < math.abs(input.z))
			{
				result = math.cross(input, new float3(1f, 0f, 0f));
				return;
			}
			if (math.abs(input.y) < math.abs(input.z))
			{
				result = math.cross(input, new float3(0f, 1f, 0f));
				return;
			}
			result = math.cross(input, new float3(0f, 0f, 1f));
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void OrthogonalUpVector_0000034D$PostfixBurstDelegate(in Vector3 forward, in Vector3 referenceUp, out Vector3 orthogonalUp);

		internal static class OrthogonalUpVector_0000034D$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.OrthogonalUpVector_0000034D$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.OrthogonalUpVector_0000034D$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.OrthogonalUpVector_0000034D$PostfixBurstDelegate>(new BurstMathUtility.OrthogonalUpVector_0000034D$PostfixBurstDelegate(BurstMathUtility.OrthogonalUpVector)).Value;
				}
				A_0 = BurstMathUtility.OrthogonalUpVector_0000034D$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.OrthogonalUpVector_0000034D$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 forward, in Vector3 referenceUp, out Vector3 orthogonalUp)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.OrthogonalUpVector_0000034D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&), ref forward, ref referenceUp, ref orthogonalUp, functionPointer);
						return;
					}
				}
				BurstMathUtility.OrthogonalUpVector$BurstManaged(forward, referenceUp, out orthogonalUp);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void OrthogonalUpVector_0000034E$PostfixBurstDelegate(in float3 forward, in float3 referenceUp, out float3 orthogonalUp);

		internal static class OrthogonalUpVector_0000034E$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.OrthogonalUpVector_0000034E$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.OrthogonalUpVector_0000034E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.OrthogonalUpVector_0000034E$PostfixBurstDelegate>(new BurstMathUtility.OrthogonalUpVector_0000034E$PostfixBurstDelegate(BurstMathUtility.OrthogonalUpVector)).Value;
				}
				A_0 = BurstMathUtility.OrthogonalUpVector_0000034E$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.OrthogonalUpVector_0000034E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 forward, in float3 referenceUp, out float3 orthogonalUp)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.OrthogonalUpVector_0000034E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&), ref forward, ref referenceUp, ref orthogonalUp, functionPointer);
						return;
					}
				}
				BurstMathUtility.OrthogonalUpVector$BurstManaged(forward, referenceUp, out orthogonalUp);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void OrthogonalLookRotation_0000034F$PostfixBurstDelegate(in Vector3 forward, in Vector3 referenceUp, out Quaternion lookRotation);

		internal static class OrthogonalLookRotation_0000034F$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.OrthogonalLookRotation_0000034F$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.OrthogonalLookRotation_0000034F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.OrthogonalLookRotation_0000034F$PostfixBurstDelegate>(new BurstMathUtility.OrthogonalLookRotation_0000034F$PostfixBurstDelegate(BurstMathUtility.OrthogonalLookRotation)).Value;
				}
				A_0 = BurstMathUtility.OrthogonalLookRotation_0000034F$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.OrthogonalLookRotation_0000034F$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 forward, in Vector3 referenceUp, out Quaternion lookRotation)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.OrthogonalLookRotation_0000034F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Quaternion&), ref forward, ref referenceUp, ref lookRotation, functionPointer);
						return;
					}
				}
				BurstMathUtility.OrthogonalLookRotation$BurstManaged(forward, referenceUp, out lookRotation);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void OrthogonalLookRotation_00000350$PostfixBurstDelegate(in float3 forward, in float3 referenceUp, out quaternion lookRotation);

		internal static class OrthogonalLookRotation_00000350$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.OrthogonalLookRotation_00000350$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.OrthogonalLookRotation_00000350$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.OrthogonalLookRotation_00000350$PostfixBurstDelegate>(new BurstMathUtility.OrthogonalLookRotation_00000350$PostfixBurstDelegate(BurstMathUtility.OrthogonalLookRotation)).Value;
				}
				A_0 = BurstMathUtility.OrthogonalLookRotation_00000350$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.OrthogonalLookRotation_00000350$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 forward, in float3 referenceUp, out quaternion lookRotation)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.OrthogonalLookRotation_00000350$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&), ref forward, ref referenceUp, ref lookRotation, functionPointer);
						return;
					}
				}
				BurstMathUtility.OrthogonalLookRotation$BurstManaged(forward, referenceUp, out lookRotation);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ProjectOnPlane_00000351$PostfixBurstDelegate(in float3 vector, in float3 planeNormal, out float3 projectedVector);

		internal static class ProjectOnPlane_00000351$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.ProjectOnPlane_00000351$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.ProjectOnPlane_00000351$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.ProjectOnPlane_00000351$PostfixBurstDelegate>(new BurstMathUtility.ProjectOnPlane_00000351$PostfixBurstDelegate(BurstMathUtility.ProjectOnPlane)).Value;
				}
				A_0 = BurstMathUtility.ProjectOnPlane_00000351$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.ProjectOnPlane_00000351$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 vector, in float3 planeNormal, out float3 projectedVector)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.ProjectOnPlane_00000351$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&), ref vector, ref planeNormal, ref projectedVector, functionPointer);
						return;
					}
				}
				BurstMathUtility.ProjectOnPlane$BurstManaged(vector, planeNormal, out projectedVector);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ProjectOnPlane_00000352$PostfixBurstDelegate(in Vector3 vector, in Vector3 planeNormal, out Vector3 projectedVector);

		internal static class ProjectOnPlane_00000352$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.ProjectOnPlane_00000352$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.ProjectOnPlane_00000352$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.ProjectOnPlane_00000352$PostfixBurstDelegate>(new BurstMathUtility.ProjectOnPlane_00000352$PostfixBurstDelegate(BurstMathUtility.ProjectOnPlane)).Value;
				}
				A_0 = BurstMathUtility.ProjectOnPlane_00000352$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.ProjectOnPlane_00000352$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 vector, in Vector3 planeNormal, out Vector3 projectedVector)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.ProjectOnPlane_00000352$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&), ref vector, ref planeNormal, ref projectedVector, functionPointer);
						return;
					}
				}
				BurstMathUtility.ProjectOnPlane$BurstManaged(vector, planeNormal, out projectedVector);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void LookRotationWithForwardProjectedOnPlane_00000353$PostfixBurstDelegate(in float3 forward, in float3 planeNormal, out quaternion lookRotation);

		internal static class LookRotationWithForwardProjectedOnPlane_00000353$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$PostfixBurstDelegate>(new BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$PostfixBurstDelegate(BurstMathUtility.LookRotationWithForwardProjectedOnPlane)).Value;
				}
				A_0 = BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 forward, in float3 planeNormal, out quaternion lookRotation)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000353$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&), ref forward, ref planeNormal, ref lookRotation, functionPointer);
						return;
					}
				}
				BurstMathUtility.LookRotationWithForwardProjectedOnPlane$BurstManaged(forward, planeNormal, out lookRotation);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void LookRotationWithForwardProjectedOnPlane_00000354$PostfixBurstDelegate(in Vector3 forward, in Vector3 planeNormal, out Quaternion lookRotation);

		internal static class LookRotationWithForwardProjectedOnPlane_00000354$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$PostfixBurstDelegate>(new BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$PostfixBurstDelegate(BurstMathUtility.LookRotationWithForwardProjectedOnPlane)).Value;
				}
				A_0 = BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 forward, in Vector3 planeNormal, out Quaternion lookRotation)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.LookRotationWithForwardProjectedOnPlane_00000354$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Quaternion&), ref forward, ref planeNormal, ref lookRotation, functionPointer);
						return;
					}
				}
				BurstMathUtility.LookRotationWithForwardProjectedOnPlane$BurstManaged(forward, planeNormal, out lookRotation);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void Angle_00000355$PostfixBurstDelegate(in quaternion a, in quaternion b, out float angle);

		internal static class Angle_00000355$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.Angle_00000355$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.Angle_00000355$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.Angle_00000355$PostfixBurstDelegate>(new BurstMathUtility.Angle_00000355$PostfixBurstDelegate(BurstMathUtility.Angle)).Value;
				}
				A_0 = BurstMathUtility.Angle_00000355$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.Angle_00000355$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in quaternion a, in quaternion b, out float angle)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.Angle_00000355$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.quaternion&,Unity.Mathematics.quaternion&,System.Single&), ref a, ref b, ref angle, functionPointer);
						return;
					}
				}
				BurstMathUtility.Angle$BurstManaged(a, b, out angle);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void Angle_00000356$PostfixBurstDelegate(in Vector3 a, in Vector3 b, out float angle);

		internal static class Angle_00000356$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.Angle_00000356$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.Angle_00000356$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.Angle_00000356$PostfixBurstDelegate>(new BurstMathUtility.Angle_00000356$PostfixBurstDelegate(BurstMathUtility.Angle)).Value;
				}
				A_0 = BurstMathUtility.Angle_00000356$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.Angle_00000356$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 a, in Vector3 b, out float angle)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.Angle_00000356$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,System.Single&), ref a, ref b, ref angle, functionPointer);
						return;
					}
				}
				BurstMathUtility.Angle$BurstManaged(a, b, out angle);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool FastVectorEquals_00000357$PostfixBurstDelegate(in float3 a, in float3 b, float tolerance = 0.0001f);

		internal static class FastVectorEquals_00000357$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.FastVectorEquals_00000357$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.FastVectorEquals_00000357$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.FastVectorEquals_00000357$PostfixBurstDelegate>(new BurstMathUtility.FastVectorEquals_00000357$PostfixBurstDelegate(BurstMathUtility.FastVectorEquals)).Value;
				}
				A_0 = BurstMathUtility.FastVectorEquals_00000357$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.FastVectorEquals_00000357$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(in float3 a, in float3 b, float tolerance = 0.0001f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.FastVectorEquals_00000357$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single), ref a, ref b, tolerance, functionPointer);
					}
				}
				return BurstMathUtility.FastVectorEquals$BurstManaged(a, b, tolerance);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool FastVectorEquals_00000358$PostfixBurstDelegate(in Vector3 a, in Vector3 b, float tolerance = 0.0001f);

		internal static class FastVectorEquals_00000358$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.FastVectorEquals_00000358$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.FastVectorEquals_00000358$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.FastVectorEquals_00000358$PostfixBurstDelegate>(new BurstMathUtility.FastVectorEquals_00000358$PostfixBurstDelegate(BurstMathUtility.FastVectorEquals)).Value;
				}
				A_0 = BurstMathUtility.FastVectorEquals_00000358$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.FastVectorEquals_00000358$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(in Vector3 a, in Vector3 b, float tolerance = 0.0001f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.FastVectorEquals_00000358$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(UnityEngine.Vector3&,UnityEngine.Vector3&,System.Single), ref a, ref b, tolerance, functionPointer);
					}
				}
				return BurstMathUtility.FastVectorEquals$BurstManaged(a, b, tolerance);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void FastSafeDivide_00000359$PostfixBurstDelegate(in Vector3 a, in Vector3 b, out Vector3 result, float tolerance = 1E-06f);

		internal static class FastSafeDivide_00000359$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.FastSafeDivide_00000359$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.FastSafeDivide_00000359$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.FastSafeDivide_00000359$PostfixBurstDelegate>(new BurstMathUtility.FastSafeDivide_00000359$PostfixBurstDelegate(BurstMathUtility.FastSafeDivide)).Value;
				}
				A_0 = BurstMathUtility.FastSafeDivide_00000359$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.FastSafeDivide_00000359$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 a, in Vector3 b, out Vector3 result, float tolerance = 1E-06f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.FastSafeDivide_00000359$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&,System.Single), ref a, ref b, ref result, tolerance, functionPointer);
						return;
					}
				}
				BurstMathUtility.FastSafeDivide$BurstManaged(a, b, out result, tolerance);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void FastSafeDivide_0000035A$PostfixBurstDelegate(in float3 a, in float3 b, out float3 result, float tolerance = 1E-06f);

		internal static class FastSafeDivide_0000035A$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.FastSafeDivide_0000035A$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.FastSafeDivide_0000035A$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.FastSafeDivide_0000035A$PostfixBurstDelegate>(new BurstMathUtility.FastSafeDivide_0000035A$PostfixBurstDelegate(BurstMathUtility.FastSafeDivide)).Value;
				}
				A_0 = BurstMathUtility.FastSafeDivide_0000035A$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.FastSafeDivide_0000035A$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 a, in float3 b, out float3 result, float tolerance = 1E-06f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.FastSafeDivide_0000035A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single), ref a, ref b, ref result, tolerance, functionPointer);
						return;
					}
				}
				BurstMathUtility.FastSafeDivide$BurstManaged(a, b, out result, tolerance);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void Scale_0000035B$PostfixBurstDelegate(in float3 a, in float3 b, out float3 result);

		internal static class Scale_0000035B$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.Scale_0000035B$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.Scale_0000035B$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.Scale_0000035B$PostfixBurstDelegate>(new BurstMathUtility.Scale_0000035B$PostfixBurstDelegate(BurstMathUtility.Scale)).Value;
				}
				A_0 = BurstMathUtility.Scale_0000035B$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.Scale_0000035B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 a, in float3 b, out float3 result)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.Scale_0000035B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&), ref a, ref b, ref result, functionPointer);
						return;
					}
				}
				BurstMathUtility.Scale$BurstManaged(a, b, out result);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void Scale_0000035C$PostfixBurstDelegate(in Vector3 a, in Vector3 b, out Vector3 result);

		internal static class Scale_0000035C$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.Scale_0000035C$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.Scale_0000035C$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.Scale_0000035C$PostfixBurstDelegate>(new BurstMathUtility.Scale_0000035C$PostfixBurstDelegate(BurstMathUtility.Scale)).Value;
				}
				A_0 = BurstMathUtility.Scale_0000035C$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.Scale_0000035C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 a, in Vector3 b, out Vector3 result)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.Scale_0000035C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&), ref a, ref b, ref result, functionPointer);
						return;
					}
				}
				BurstMathUtility.Scale$BurstManaged(a, b, out result);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void Orthogonal_0000035E$PostfixBurstDelegate(in float3 input, out float3 result);

		internal static class Orthogonal_0000035E$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstMathUtility.Orthogonal_0000035E$BurstDirectCall.Pointer == 0)
				{
					BurstMathUtility.Orthogonal_0000035E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstMathUtility.Orthogonal_0000035E$PostfixBurstDelegate>(new BurstMathUtility.Orthogonal_0000035E$PostfixBurstDelegate(BurstMathUtility.Orthogonal)).Value;
				}
				A_0 = BurstMathUtility.Orthogonal_0000035E$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstMathUtility.Orthogonal_0000035E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 input, out float3 result)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstMathUtility.Orthogonal_0000035E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&), ref input, ref result, functionPointer);
						return;
					}
				}
				BurstMathUtility.Orthogonal$BurstManaged(input, out result);
			}

			private static IntPtr Pointer;
		}
	}
}
