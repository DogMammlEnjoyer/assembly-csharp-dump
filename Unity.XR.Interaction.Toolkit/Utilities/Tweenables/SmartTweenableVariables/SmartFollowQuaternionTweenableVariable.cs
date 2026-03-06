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
	public class SmartFollowQuaternionTweenableVariable : QuaternionTweenableVariable
	{
		public float minAngleAllowed { get; set; }

		public float maxAngleAllowed { get; set; }

		public float minToMaxDelaySeconds { get; set; }

		public SmartFollowQuaternionTweenableVariable(float minAngleAllowed = 0.1f, float maxAngleAllowed = 5f, float minToMaxDelaySeconds = 3f)
		{
			this.minAngleAllowed = minAngleAllowed;
			this.maxAngleAllowed = maxAngleAllowed;
			this.minToMaxDelaySeconds = minToMaxDelaySeconds;
		}

		public bool IsNewTargetWithinThreshold(Quaternion newTarget)
		{
			float num = Quaternion.Angle(base.target, newTarget);
			float num2 = Time.unscaledTime - this.m_LastUpdateTime;
			float num3 = Mathf.Lerp(this.minAngleAllowed, this.maxAngleAllowed, Mathf.Clamp01(num2 / this.minToMaxDelaySeconds));
			return num > num3;
		}

		public bool SetTargetWithinThreshold(Quaternion newTarget)
		{
			bool flag = this.IsNewTargetWithinThreshold(newTarget);
			if (flag)
			{
				base.target = newTarget;
			}
			return flag;
		}

		protected override void OnTargetChanged(Quaternion newTarget)
		{
			this.m_LastUpdateTime = Time.unscaledTime;
		}

		public void HandleSmartTween(float deltaTime, float lowerSpeed, float upperSpeed)
		{
			float angleOffsetDeg = Quaternion.Angle(base.target, base.Value);
			float tweenTarget;
			SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget(deltaTime, angleOffsetDeg, this.maxAngleAllowed, lowerSpeed, upperSpeed, out tweenTarget);
			base.HandleTween(tweenTarget);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$PostfixBurstDelegate))]
		private static void ComputeNewTweenTarget(float deltaTime, float angleOffsetDeg, float maxAngleAllowed, float lowerSpeed, float upperSpeed, out float newTweenTarget)
		{
			SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$BurstDirectCall.Invoke(deltaTime, angleOffsetDeg, maxAngleAllowed, lowerSpeed, upperSpeed, out newTweenTarget);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ComputeNewTweenTarget$BurstManaged(float deltaTime, float angleOffsetDeg, float maxAngleAllowed, float lowerSpeed, float upperSpeed, out float newTweenTarget)
		{
			float num = 1f - math.clamp(angleOffsetDeg / maxAngleAllowed, 0f, 1f);
			newTweenTarget = deltaTime * math.clamp(num * upperSpeed, lowerSpeed, upperSpeed);
		}

		private float m_LastUpdateTime;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ComputeNewTweenTarget_00000416$PostfixBurstDelegate(float deltaTime, float angleOffsetDeg, float maxAngleAllowed, float lowerSpeed, float upperSpeed, out float newTweenTarget);

		internal static class ComputeNewTweenTarget_00000416$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$BurstDirectCall.Pointer == 0)
				{
					SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$PostfixBurstDelegate>(new SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$PostfixBurstDelegate(SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget)).Value;
				}
				A_0 = SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(float deltaTime, float angleOffsetDeg, float maxAngleAllowed, float lowerSpeed, float upperSpeed, out float newTweenTarget)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget_00000416$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Single,System.Single,System.Single,System.Single,System.Single,System.Single&), deltaTime, angleOffsetDeg, maxAngleAllowed, lowerSpeed, upperSpeed, ref newTweenTarget, functionPointer);
						return;
					}
				}
				SmartFollowQuaternionTweenableVariable.ComputeNewTweenTarget$BurstManaged(deltaTime, angleOffsetDeg, maxAngleAllowed, lowerSpeed, upperSpeed, out newTweenTarget);
			}

			private static IntPtr Pointer;
		}
	}
}
