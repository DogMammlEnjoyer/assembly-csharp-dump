using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	[BurstCompile]
	public static class BurstPhysicsUtils
	{
		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$PostfixBurstDelegate))]
		public static void GetSphereOverlapParameters(in Vector3 overlapStart, in Vector3 overlapEnd, out Vector3 normalizedOverlapVector, out float overlapSqrMagnitude, out float overlapDistance)
		{
			BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$BurstDirectCall.Invoke(overlapStart, overlapEnd, out normalizedOverlapVector, out overlapSqrMagnitude, out overlapDistance);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstPhysicsUtils.GetConecastParameters_00000360$PostfixBurstDelegate))]
		public static void GetConecastParameters(float angleRadius, float offset, float maxOffset, in Vector3 direction, out Vector3 originOffset, out float radius, out float castMax)
		{
			BurstPhysicsUtils.GetConecastParameters_00000360$BurstDirectCall.Invoke(angleRadius, offset, maxOffset, direction, out originOffset, out radius, out castMax);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$PostfixBurstDelegate))]
		internal static void GetMultiSegmentConecastParameters(float angleRadius, float segmentOffset, float offsetFromOrigin, float maxOffset, in Vector3 direction, out Vector3 originOffset, out float radius, out float castMax)
		{
			BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$BurstDirectCall.Invoke(angleRadius, segmentOffset, offsetFromOrigin, maxOffset, direction, out originOffset, out radius, out castMax);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstPhysicsUtils.GetConecastOffset_00000362$PostfixBurstDelegate))]
		public static void GetConecastOffset(in float3 origin, in float3 conePoint, in float3 direction, out float coneOffset)
		{
			BurstPhysicsUtils.GetConecastOffset_00000362$BurstDirectCall.Invoke(origin, conePoint, direction, out coneOffset);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetSphereOverlapParameters$BurstManaged(in Vector3 overlapStart, in Vector3 overlapEnd, out Vector3 normalizedOverlapVector, out float overlapSqrMagnitude, out float overlapDistance)
		{
			Vector3 a = overlapEnd - overlapStart;
			overlapSqrMagnitude = math.distancesq(overlapStart, overlapEnd);
			overlapDistance = math.sqrt(overlapSqrMagnitude);
			normalizedOverlapVector = a / overlapDistance;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetConecastParameters$BurstManaged(float angleRadius, float offset, float maxOffset, in Vector3 direction, out Vector3 originOffset, out float radius, out float castMax)
		{
			castMax = math.clamp(offset, 0.125f, maxOffset);
			radius = angleRadius * (offset + castMax);
			originOffset = direction * (offset - radius);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetMultiSegmentConecastParameters$BurstManaged(float angleRadius, float segmentOffset, float offsetFromOrigin, float maxOffset, in Vector3 direction, out Vector3 originOffset, out float radius, out float castMax)
		{
			castMax = math.clamp(segmentOffset, 0.125f, maxOffset);
			if (segmentOffset + castMax > maxOffset)
			{
				castMax = math.clamp(maxOffset - segmentOffset, 0.125f, castMax);
			}
			radius = angleRadius * (offsetFromOrigin + castMax);
			originOffset = direction * (segmentOffset - radius);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetConecastOffset$BurstManaged(in float3 origin, in float3 conePoint, in float3 direction, out float coneOffset)
		{
			float3 @float = conePoint - origin;
			float rhs = math.dot(@float, direction);
			float3 x = @float - direction * rhs;
			coneOffset = math.length(x);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetSphereOverlapParameters_0000035F$PostfixBurstDelegate(in Vector3 overlapStart, in Vector3 overlapEnd, out Vector3 normalizedOverlapVector, out float overlapSqrMagnitude, out float overlapDistance);

		internal static class GetSphereOverlapParameters_0000035F$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$BurstDirectCall.Pointer == 0)
				{
					BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$PostfixBurstDelegate>(new BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$PostfixBurstDelegate(BurstPhysicsUtils.GetSphereOverlapParameters)).Value;
				}
				A_0 = BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 overlapStart, in Vector3 overlapEnd, out Vector3 normalizedOverlapVector, out float overlapSqrMagnitude, out float overlapDistance)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstPhysicsUtils.GetSphereOverlapParameters_0000035F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&,System.Single&,System.Single&), ref overlapStart, ref overlapEnd, ref normalizedOverlapVector, ref overlapSqrMagnitude, ref overlapDistance, functionPointer);
						return;
					}
				}
				BurstPhysicsUtils.GetSphereOverlapParameters$BurstManaged(overlapStart, overlapEnd, out normalizedOverlapVector, out overlapSqrMagnitude, out overlapDistance);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetConecastParameters_00000360$PostfixBurstDelegate(float angleRadius, float offset, float maxOffset, in Vector3 direction, out Vector3 originOffset, out float radius, out float castMax);

		internal static class GetConecastParameters_00000360$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstPhysicsUtils.GetConecastParameters_00000360$BurstDirectCall.Pointer == 0)
				{
					BurstPhysicsUtils.GetConecastParameters_00000360$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstPhysicsUtils.GetConecastParameters_00000360$PostfixBurstDelegate>(new BurstPhysicsUtils.GetConecastParameters_00000360$PostfixBurstDelegate(BurstPhysicsUtils.GetConecastParameters)).Value;
				}
				A_0 = BurstPhysicsUtils.GetConecastParameters_00000360$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstPhysicsUtils.GetConecastParameters_00000360$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(float angleRadius, float offset, float maxOffset, in Vector3 direction, out Vector3 originOffset, out float radius, out float castMax)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstPhysicsUtils.GetConecastParameters_00000360$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Single,System.Single,System.Single,UnityEngine.Vector3&,UnityEngine.Vector3&,System.Single&,System.Single&), angleRadius, offset, maxOffset, ref direction, ref originOffset, ref radius, ref castMax, functionPointer);
						return;
					}
				}
				BurstPhysicsUtils.GetConecastParameters$BurstManaged(angleRadius, offset, maxOffset, direction, out originOffset, out radius, out castMax);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetMultiSegmentConecastParameters_00000361$PostfixBurstDelegate(float angleRadius, float segmentOffset, float offsetFromOrigin, float maxOffset, in Vector3 direction, out Vector3 originOffset, out float radius, out float castMax);

		internal static class GetMultiSegmentConecastParameters_00000361$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$BurstDirectCall.Pointer == 0)
				{
					BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$PostfixBurstDelegate>(new BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$PostfixBurstDelegate(BurstPhysicsUtils.GetMultiSegmentConecastParameters)).Value;
				}
				A_0 = BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(float angleRadius, float segmentOffset, float offsetFromOrigin, float maxOffset, in Vector3 direction, out Vector3 originOffset, out float radius, out float castMax)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstPhysicsUtils.GetMultiSegmentConecastParameters_00000361$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Single,System.Single,System.Single,System.Single,UnityEngine.Vector3&,UnityEngine.Vector3&,System.Single&,System.Single&), angleRadius, segmentOffset, offsetFromOrigin, maxOffset, ref direction, ref originOffset, ref radius, ref castMax, functionPointer);
						return;
					}
				}
				BurstPhysicsUtils.GetMultiSegmentConecastParameters$BurstManaged(angleRadius, segmentOffset, offsetFromOrigin, maxOffset, direction, out originOffset, out radius, out castMax);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GetConecastOffset_00000362$PostfixBurstDelegate(in float3 origin, in float3 conePoint, in float3 direction, out float coneOffset);

		internal static class GetConecastOffset_00000362$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstPhysicsUtils.GetConecastOffset_00000362$BurstDirectCall.Pointer == 0)
				{
					BurstPhysicsUtils.GetConecastOffset_00000362$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstPhysicsUtils.GetConecastOffset_00000362$PostfixBurstDelegate>(new BurstPhysicsUtils.GetConecastOffset_00000362$PostfixBurstDelegate(BurstPhysicsUtils.GetConecastOffset)).Value;
				}
				A_0 = BurstPhysicsUtils.GetConecastOffset_00000362$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstPhysicsUtils.GetConecastOffset_00000362$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 origin, in float3 conePoint, in float3 direction, out float coneOffset)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstPhysicsUtils.GetConecastOffset_00000362$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single&), ref origin, ref conePoint, ref direction, ref coneOffset, functionPointer);
						return;
					}
				}
				BurstPhysicsUtils.GetConecastOffset$BurstManaged(origin, conePoint, direction, out coneOffset);
			}

			private static IntPtr Pointer;
		}
	}
}
