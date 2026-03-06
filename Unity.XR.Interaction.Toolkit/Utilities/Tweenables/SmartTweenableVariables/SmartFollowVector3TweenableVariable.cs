using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.SmartTweenableVariables
{
	[BurstCompile]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class SmartFollowVector3TweenableVariable : Vector3TweenableVariable
	{
		public float minDistanceAllowed { get; set; }

		public float maxDistanceAllowed
		{
			get
			{
				return this.m_MaxDistanceAllowed;
			}
			set
			{
				this.m_MaxDistanceAllowed = value;
				this.m_SqrMaxDistanceAllowed = this.m_MaxDistanceAllowed * this.m_MaxDistanceAllowed;
			}
		}

		public float minToMaxDelaySeconds { get; set; }

		public SmartFollowVector3TweenableVariable(float minDistanceAllowed = 0.01f, float maxDistanceAllowed = 0.3f, float minToMaxDelaySeconds = 3f)
		{
			this.minDistanceAllowed = minDistanceAllowed;
			this.maxDistanceAllowed = maxDistanceAllowed;
			this.minToMaxDelaySeconds = minToMaxDelaySeconds;
		}

		public bool IsNewTargetWithinThreshold(float3 newTarget)
		{
			float3 value = base.Value;
			return SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold(value, newTarget, this.minDistanceAllowed, this.m_MaxDistanceAllowed, Time.unscaledTime - this.m_LastUpdateTime, this.minToMaxDelaySeconds);
		}

		public bool SetTargetWithinThreshold(float3 newTarget)
		{
			bool flag = this.IsNewTargetWithinThreshold(newTarget);
			if (flag)
			{
				base.target = newTarget;
			}
			return flag;
		}

		protected override void OnTargetChanged(float3 newTarget)
		{
			base.OnTargetChanged(newTarget);
			this.m_LastUpdateTime = Time.unscaledTime;
		}

		public void HandleSmartTween(float deltaTime, float lowerSpeed, float upperSpeed)
		{
			float3 value = base.Value;
			float3 target = base.target;
			float tweenTarget;
			SmartFollowVector3TweenableVariable.ComputeNewTweenTarget(value, target, this.m_SqrMaxDistanceAllowed, deltaTime, lowerSpeed, upperSpeed, out tweenTarget);
			base.HandleTween(tweenTarget);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$PostfixBurstDelegate))]
		private static void ComputeNewTweenTarget(in float3 currentValue, in float3 targetValue, float sqrMaxDistanceAllowed, float deltaTime, float lowerSpeed, float upperSpeed, out float newTweenTarget)
		{
			SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$BurstDirectCall.Invoke(currentValue, targetValue, sqrMaxDistanceAllowed, deltaTime, lowerSpeed, upperSpeed, out newTweenTarget);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$PostfixBurstDelegate))]
		private static bool IsNewTargetWithinThreshold(in float3 currentValue, in float3 targetValue, float minDistanceAllowed, float maxDistanceAllowed, float timeSinceLastUpdate, float minToMaxDelaySeconds)
		{
			return SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$BurstDirectCall.Invoke(currentValue, targetValue, minDistanceAllowed, maxDistanceAllowed, timeSinceLastUpdate, minToMaxDelaySeconds);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ComputeNewTweenTarget$BurstManaged(in float3 currentValue, in float3 targetValue, float sqrMaxDistanceAllowed, float deltaTime, float lowerSpeed, float upperSpeed, out float newTweenTarget)
		{
			float num = math.distancesq(currentValue, targetValue);
			float num2 = math.clamp((1f - math.clamp(num / sqrMaxDistanceAllowed, 0f, 1f)) * upperSpeed, lowerSpeed, upperSpeed);
			newTweenTarget = deltaTime * num2;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsNewTargetWithinThreshold$BurstManaged(in float3 currentValue, in float3 targetValue, float minDistanceAllowed, float maxDistanceAllowed, float timeSinceLastUpdate, float minToMaxDelaySeconds)
		{
			float num = math.distancesq(currentValue, targetValue);
			float num2 = math.lerp(minDistanceAllowed, maxDistanceAllowed, math.clamp(timeSinceLastUpdate / minToMaxDelaySeconds, 0f, 1f));
			return num > num2 * num2;
		}

		private float m_MaxDistanceAllowed;

		private float m_SqrMaxDistanceAllowed;

		private float m_LastUpdateTime;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ComputeNewTweenTarget_00000422$PostfixBurstDelegate(in float3 currentValue, in float3 targetValue, float sqrMaxDistanceAllowed, float deltaTime, float lowerSpeed, float upperSpeed, out float newTweenTarget);

		internal static class ComputeNewTweenTarget_00000422$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$BurstDirectCall.Pointer == 0)
				{
					SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$PostfixBurstDelegate>(new SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$PostfixBurstDelegate(SmartFollowVector3TweenableVariable.ComputeNewTweenTarget)).Value;
				}
				A_0 = SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 currentValue, in float3 targetValue, float sqrMaxDistanceAllowed, float deltaTime, float lowerSpeed, float upperSpeed, out float newTweenTarget)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = SmartFollowVector3TweenableVariable.ComputeNewTweenTarget_00000422$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,System.Single,System.Single,System.Single,System.Single&), ref currentValue, ref targetValue, sqrMaxDistanceAllowed, deltaTime, lowerSpeed, upperSpeed, ref newTweenTarget, functionPointer);
						return;
					}
				}
				SmartFollowVector3TweenableVariable.ComputeNewTweenTarget$BurstManaged(currentValue, targetValue, sqrMaxDistanceAllowed, deltaTime, lowerSpeed, upperSpeed, out newTweenTarget);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool IsNewTargetWithinThreshold_00000423$PostfixBurstDelegate(in float3 currentValue, in float3 targetValue, float minDistanceAllowed, float maxDistanceAllowed, float timeSinceLastUpdate, float minToMaxDelaySeconds);

		internal static class IsNewTargetWithinThreshold_00000423$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$BurstDirectCall.Pointer == 0)
				{
					SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$PostfixBurstDelegate>(new SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$PostfixBurstDelegate(SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold)).Value;
				}
				A_0 = SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(in float3 currentValue, in float3 targetValue, float minDistanceAllowed, float maxDistanceAllowed, float timeSinceLastUpdate, float minToMaxDelaySeconds)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold_00000423$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,System.Single,System.Single,System.Single), ref currentValue, ref targetValue, minDistanceAllowed, maxDistanceAllowed, timeSinceLastUpdate, minToMaxDelaySeconds, functionPointer);
					}
				}
				return SmartFollowVector3TweenableVariable.IsNewTargetWithinThreshold$BurstManaged(currentValue, targetValue, minDistanceAllowed, maxDistanceAllowed, timeSinceLastUpdate, minToMaxDelaySeconds);
			}

			private static IntPtr Pointer;
		}
	}
}
