using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	[BurstCompile]
	public static class BurstLerpUtility
	{
		public static Vector3 BezierLerp(in Vector3 start, in Vector3 end, float t, float controlHeightFactor = 0.5f)
		{
			float3 @float = start;
			float3 float2 = end;
			float3 v;
			BurstLerpUtility.BezierLerp(@float, float2, t, out v, controlHeightFactor);
			return v;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstLerpUtility.BezierLerp_00000343$PostfixBurstDelegate))]
		public static void BezierLerp(in float3 start, in float3 end, float t, out float3 result, float controlHeightFactor = 0.5f)
		{
			BurstLerpUtility.BezierLerp_00000343$BurstDirectCall.Invoke(start, end, t, out result, controlHeightFactor);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstLerpUtility.BezierLerp_00000344$PostfixBurstDelegate))]
		public static float BezierLerp(float start, float end, float t, float controlHeightFactor = 0.5f)
		{
			return BurstLerpUtility.BezierLerp_00000344$BurstDirectCall.Invoke(start, end, t, controlHeightFactor);
		}

		public static Vector3 BounceOutLerp(Vector3 start, Vector3 end, float t, float speed = 1f)
		{
			float3 @float = start;
			float3 float2 = end;
			float3 v;
			BurstLerpUtility.BounceOutLerp(@float, float2, t, out v, speed);
			return v;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstLerpUtility.BounceOutLerp_00000346$PostfixBurstDelegate))]
		public static void BounceOutLerp(in float3 start, in float3 end, float t, out float3 result, float speed = 1f)
		{
			BurstLerpUtility.BounceOutLerp_00000346$BurstDirectCall.Invoke(start, end, t, out result, speed);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstLerpUtility.BounceOutLerp_00000347$PostfixBurstDelegate))]
		public static float BounceOutLerp(float start, float end, float t, float speed = 1f)
		{
			return BurstLerpUtility.BounceOutLerp_00000347$BurstDirectCall.Invoke(start, end, t, speed);
		}

		private static float EaseOutBounce(float t, float speed = 1f)
		{
			t = Mathf.Clamp01(t * speed);
			if (t < 0.36363637f)
			{
				return 7.5625f * t * t;
			}
			if (t < 0.72727275f)
			{
				t -= 0.54545456f;
				return 7.5625f * t * t + 0.75f;
			}
			if ((double)t < 0.9090909090909091)
			{
				t -= 0.8181818f;
				return 7.5625f * t * t + 0.9375f;
			}
			t -= 0.95454544f;
			return 7.5625f * t * t + 0.984375f;
		}

		public static Vector3 SingleBounceOutLerp(Vector3 start, Vector3 end, float t, float speed = 1f)
		{
			float3 @float = start;
			float3 float2 = end;
			float3 v;
			BurstLerpUtility.BounceOutLerp(@float, float2, t, out v, speed);
			return v;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstLerpUtility.SingleBounceOutLerp_0000034A$PostfixBurstDelegate))]
		public static void SingleBounceOutLerp(in float3 start, in float3 end, float t, out float3 result, float speed = 1f)
		{
			BurstLerpUtility.SingleBounceOutLerp_0000034A$BurstDirectCall.Invoke(start, end, t, out result, speed);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(BurstLerpUtility.SingleBounceOutLerp_0000034B$PostfixBurstDelegate))]
		public static float SingleBounceOutLerp(float start, float end, float t, float speed = 1f)
		{
			return BurstLerpUtility.SingleBounceOutLerp_0000034B$BurstDirectCall.Invoke(start, end, t, speed);
		}

		private static float EaseOutBounceSingle(float t, float speed = 1f)
		{
			t = Mathf.Clamp01(t * speed);
			if (t < 0.36363637f)
			{
				return 7.5625f * t * t;
			}
			if (t < 0.72727275f)
			{
				t -= 0.54545456f;
				return 7.5625f * t * t + 0.75f;
			}
			t -= 0.8181818f;
			return 7.5625f * t * t + 0.9375f;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void BezierLerp$BurstManaged(in float3 start, in float3 end, float t, out float3 result, float controlHeightFactor = 0.5f)
		{
			result = math.lerp(start, end, BurstLerpUtility.BezierLerp(0f, 1f, t, controlHeightFactor));
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float BezierLerp$BurstManaged(float start, float end, float t, float controlHeightFactor = 0.5f)
		{
			float num = (start + end) / 2f + controlHeightFactor * (end - start);
			float num2 = 1f - t;
			return num2 * (num2 * start + t * num) + t * (num2 * num + t * end);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void BounceOutLerp$BurstManaged(in float3 start, in float3 end, float t, out float3 result, float speed = 1f)
		{
			result = math.lerp(start, end, BurstLerpUtility.EaseOutBounce(t, speed));
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float BounceOutLerp$BurstManaged(float start, float end, float t, float speed = 1f)
		{
			return math.lerp(start, end, BurstLerpUtility.EaseOutBounce(t, speed));
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SingleBounceOutLerp$BurstManaged(in float3 start, in float3 end, float t, out float3 result, float speed = 1f)
		{
			result = math.lerp(start, end, BurstLerpUtility.EaseOutBounceSingle(t, speed));
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float SingleBounceOutLerp$BurstManaged(float start, float end, float t, float speed = 1f)
		{
			return math.lerp(start, end, BurstLerpUtility.EaseOutBounceSingle(t, speed));
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void BezierLerp_00000343$PostfixBurstDelegate(in float3 start, in float3 end, float t, out float3 result, float controlHeightFactor = 0.5f);

		internal static class BezierLerp_00000343$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstLerpUtility.BezierLerp_00000343$BurstDirectCall.Pointer == 0)
				{
					BurstLerpUtility.BezierLerp_00000343$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstLerpUtility.BezierLerp_00000343$PostfixBurstDelegate>(new BurstLerpUtility.BezierLerp_00000343$PostfixBurstDelegate(BurstLerpUtility.BezierLerp)).Value;
				}
				A_0 = BurstLerpUtility.BezierLerp_00000343$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstLerpUtility.BezierLerp_00000343$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 start, in float3 end, float t, out float3 result, float controlHeightFactor = 0.5f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstLerpUtility.BezierLerp_00000343$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&,System.Single), ref start, ref end, t, ref result, controlHeightFactor, functionPointer);
						return;
					}
				}
				BurstLerpUtility.BezierLerp$BurstManaged(start, end, t, out result, controlHeightFactor);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate float BezierLerp_00000344$PostfixBurstDelegate(float start, float end, float t, float controlHeightFactor = 0.5f);

		internal static class BezierLerp_00000344$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstLerpUtility.BezierLerp_00000344$BurstDirectCall.Pointer == 0)
				{
					BurstLerpUtility.BezierLerp_00000344$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstLerpUtility.BezierLerp_00000344$PostfixBurstDelegate>(new BurstLerpUtility.BezierLerp_00000344$PostfixBurstDelegate(BurstLerpUtility.BezierLerp)).Value;
				}
				A_0 = BurstLerpUtility.BezierLerp_00000344$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstLerpUtility.BezierLerp_00000344$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static float Invoke(float start, float end, float t, float controlHeightFactor = 0.5f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstLerpUtility.BezierLerp_00000344$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Single(System.Single,System.Single,System.Single,System.Single), start, end, t, controlHeightFactor, functionPointer);
					}
				}
				return BurstLerpUtility.BezierLerp$BurstManaged(start, end, t, controlHeightFactor);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void BounceOutLerp_00000346$PostfixBurstDelegate(in float3 start, in float3 end, float t, out float3 result, float speed = 1f);

		internal static class BounceOutLerp_00000346$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstLerpUtility.BounceOutLerp_00000346$BurstDirectCall.Pointer == 0)
				{
					BurstLerpUtility.BounceOutLerp_00000346$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstLerpUtility.BounceOutLerp_00000346$PostfixBurstDelegate>(new BurstLerpUtility.BounceOutLerp_00000346$PostfixBurstDelegate(BurstLerpUtility.BounceOutLerp)).Value;
				}
				A_0 = BurstLerpUtility.BounceOutLerp_00000346$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstLerpUtility.BounceOutLerp_00000346$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 start, in float3 end, float t, out float3 result, float speed = 1f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstLerpUtility.BounceOutLerp_00000346$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&,System.Single), ref start, ref end, t, ref result, speed, functionPointer);
						return;
					}
				}
				BurstLerpUtility.BounceOutLerp$BurstManaged(start, end, t, out result, speed);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate float BounceOutLerp_00000347$PostfixBurstDelegate(float start, float end, float t, float speed = 1f);

		internal static class BounceOutLerp_00000347$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstLerpUtility.BounceOutLerp_00000347$BurstDirectCall.Pointer == 0)
				{
					BurstLerpUtility.BounceOutLerp_00000347$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstLerpUtility.BounceOutLerp_00000347$PostfixBurstDelegate>(new BurstLerpUtility.BounceOutLerp_00000347$PostfixBurstDelegate(BurstLerpUtility.BounceOutLerp)).Value;
				}
				A_0 = BurstLerpUtility.BounceOutLerp_00000347$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstLerpUtility.BounceOutLerp_00000347$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static float Invoke(float start, float end, float t, float speed = 1f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstLerpUtility.BounceOutLerp_00000347$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Single(System.Single,System.Single,System.Single,System.Single), start, end, t, speed, functionPointer);
					}
				}
				return BurstLerpUtility.BounceOutLerp$BurstManaged(start, end, t, speed);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void SingleBounceOutLerp_0000034A$PostfixBurstDelegate(in float3 start, in float3 end, float t, out float3 result, float speed = 1f);

		internal static class SingleBounceOutLerp_0000034A$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstLerpUtility.SingleBounceOutLerp_0000034A$BurstDirectCall.Pointer == 0)
				{
					BurstLerpUtility.SingleBounceOutLerp_0000034A$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstLerpUtility.SingleBounceOutLerp_0000034A$PostfixBurstDelegate>(new BurstLerpUtility.SingleBounceOutLerp_0000034A$PostfixBurstDelegate(BurstLerpUtility.SingleBounceOutLerp)).Value;
				}
				A_0 = BurstLerpUtility.SingleBounceOutLerp_0000034A$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstLerpUtility.SingleBounceOutLerp_0000034A$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 start, in float3 end, float t, out float3 result, float speed = 1f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstLerpUtility.SingleBounceOutLerp_0000034A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&,System.Single), ref start, ref end, t, ref result, speed, functionPointer);
						return;
					}
				}
				BurstLerpUtility.SingleBounceOutLerp$BurstManaged(start, end, t, out result, speed);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate float SingleBounceOutLerp_0000034B$PostfixBurstDelegate(float start, float end, float t, float speed = 1f);

		internal static class SingleBounceOutLerp_0000034B$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (BurstLerpUtility.SingleBounceOutLerp_0000034B$BurstDirectCall.Pointer == 0)
				{
					BurstLerpUtility.SingleBounceOutLerp_0000034B$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstLerpUtility.SingleBounceOutLerp_0000034B$PostfixBurstDelegate>(new BurstLerpUtility.SingleBounceOutLerp_0000034B$PostfixBurstDelegate(BurstLerpUtility.SingleBounceOutLerp)).Value;
				}
				A_0 = BurstLerpUtility.SingleBounceOutLerp_0000034B$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				BurstLerpUtility.SingleBounceOutLerp_0000034B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static float Invoke(float start, float end, float t, float speed = 1f)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = BurstLerpUtility.SingleBounceOutLerp_0000034B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Single(System.Single,System.Single,System.Single,System.Single), start, end, t, speed, functionPointer);
					}
				}
				return BurstLerpUtility.SingleBounceOutLerp$BurstManaged(start, end, t, speed);
			}

			private static IntPtr Pointer;
		}
	}
}
