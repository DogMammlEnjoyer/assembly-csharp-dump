using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
	[BurstCompile]
	[AddComponentMenu("XR/XR Transform Stabilizer", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.XRTransformStabilizer.html")]
	[DefaultExecutionOrder(-29985)]
	public class XRTransformStabilizer : MonoBehaviour
	{
		public Transform targetTransform
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				this.m_Target = value;
			}
		}

		public IXRRayProvider aimTarget
		{
			get
			{
				return this.m_AimTarget;
			}
			set
			{
				this.m_AimTarget = value;
				this.m_AimTargetObject = (value as Object);
			}
		}

		public bool useLocalSpace
		{
			get
			{
				return this.m_UseLocalSpace;
			}
			set
			{
				this.m_UseLocalSpace = value;
			}
		}

		public float angleStabilization
		{
			get
			{
				return this.m_AngleStabilization;
			}
			set
			{
				this.m_AngleStabilization = value;
			}
		}

		public float positionStabilization
		{
			get
			{
				return this.m_PositionStabilization;
			}
			set
			{
				this.m_PositionStabilization = value;
			}
		}

		protected void Awake()
		{
			this.m_ThisTransform = base.transform;
			if (this.m_AimTarget == null)
			{
				this.m_AimTarget = (this.m_AimTargetObject as IXRRayProvider);
			}
		}

		protected void OnEnable()
		{
			if (this.m_AimTarget == null)
			{
				this.m_AimTarget = (this.m_AimTargetObject as IXRRayProvider);
			}
			if (this.m_Target == null)
			{
				return;
			}
			if (this.m_UseLocalSpace)
			{
				this.m_ThisTransform.SetLocalPose(this.m_Target.GetLocalPose());
				return;
			}
			this.m_ThisTransform.SetWorldPose(this.m_Target.GetWorldPose());
		}

		protected void Update()
		{
			if (this.m_Target == null)
			{
				return;
			}
			if (this.m_AimTarget != null && this.m_AimTargetObject == null && this.m_AimTarget == this.m_AimTargetObject)
			{
				Debug.LogWarning("The reference assigned to Aim Target Object has been destroyed, clearing property on XR Transform Stabilizer.", this);
				this.aimTarget = null;
			}
			XRTransformStabilizer.ApplyStabilization(ref this.m_ThisTransform, this.m_Target, this.m_AimTarget, this.m_PositionStabilization, this.m_AngleStabilization, Time.deltaTime, this.m_UseLocalSpace);
		}

		public static void ApplyStabilization(ref Transform toStabilize, in Transform target, float positionStabilization, float angleStabilization, float deltaTime, bool useLocalSpace = false)
		{
			Pose currentPose;
			Pose targetPose;
			XRTransformStabilizer.CalculatePoses(toStabilize, target, useLocalSpace, out currentPose, out targetPose);
			float localScale = XRTransformStabilizer.CalculateScaleFactor(toStabilize, useLocalSpace);
			XRTransformStabilizer.ProcessStabilizationWithoutAimTarget(currentPose, targetPose, positionStabilization, angleStabilization, deltaTime, localScale, toStabilize, useLocalSpace);
		}

		public static void ApplyStabilization(ref Transform toStabilize, in Transform target, in float3 targetEndpoint, float positionStabilization, float angleStabilization, float deltaTime, bool useLocalSpace = false)
		{
			Pose currentPose;
			Pose targetPose;
			XRTransformStabilizer.CalculatePoses(toStabilize, target, useLocalSpace, out currentPose, out targetPose);
			float localScale = XRTransformStabilizer.CalculateScaleFactor(toStabilize, useLocalSpace);
			XRTransformStabilizer.ProcessStabilization(currentPose, targetPose, targetEndpoint, positionStabilization, angleStabilization, deltaTime, localScale, toStabilize, useLocalSpace);
		}

		public static void ApplyStabilization(ref Transform toStabilize, in Transform target, in IXRRayProvider aimTarget, float positionStabilization, float angleStabilization, float deltaTime, bool useLocalSpace = false)
		{
			if (aimTarget == null)
			{
				XRTransformStabilizer.ApplyStabilization(ref toStabilize, target, positionStabilization, angleStabilization, deltaTime, useLocalSpace);
				return;
			}
			float3 @float = aimTarget.rayEndPoint;
			XRTransformStabilizer.ApplyStabilization(ref toStabilize, target, @float, positionStabilization, angleStabilization, deltaTime, false);
		}

		private static void ProcessStabilization(Pose currentPose, Pose targetPose, Vector3 targetEndpoint, float positionStabilization, float angleStabilization, float deltaTime, float localScale, Transform toStabilize, bool useLocalSpace)
		{
			float3 @float = currentPose.position;
			quaternion quaternion = currentPose.rotation;
			float3 float2 = targetPose.position;
			quaternion quaternion2 = targetPose.rotation;
			float invScale = 1f / localScale;
			float3 v;
			XRTransformStabilizer.StabilizePosition(@float, float2, deltaTime, positionStabilization * localScale, out v);
			float3 float3 = toStabilize.forward;
			float3 float4 = toStabilize.up;
			float3 float5 = targetEndpoint;
			quaternion quaternion3;
			float scaleFactor;
			float alternateStabilization;
			XRTransformStabilizer.CalculateRotationParams(@float, v, float3, float4, float5, invScale, angleStabilization, out quaternion3, out scaleFactor, out alternateStabilization);
			quaternion q;
			XRTransformStabilizer.StabilizeOptimalRotation(quaternion, quaternion2, quaternion3, deltaTime, angleStabilization, alternateStabilization, scaleFactor, out q);
			Pose pose = new Pose(v, q);
			if (useLocalSpace)
			{
				toStabilize.SetLocalPose(pose);
				return;
			}
			toStabilize.SetWorldPose(pose);
		}

		private static void ProcessStabilizationWithoutAimTarget(Pose currentPose, Pose targetPose, float positionStabilization, float angleStabilization, float deltaTime, float localScale, Transform toStabilize, bool useLocalSpace)
		{
			float3 @float = currentPose.position;
			quaternion quaternion = currentPose.rotation;
			float3 float2 = targetPose.position;
			quaternion quaternion2 = targetPose.rotation;
			float3 v;
			quaternion q;
			XRTransformStabilizer.StabilizeTransform(@float, quaternion, float2, quaternion2, deltaTime, positionStabilization * localScale, angleStabilization, out v, out q);
			Pose pose = new Pose(v, q);
			if (useLocalSpace)
			{
				toStabilize.SetLocalPose(pose);
				return;
			}
			toStabilize.SetWorldPose(pose);
		}

		private static void CalculatePoses(Transform toStabilize, Transform target, bool useLocalSpace, out Pose currentPose, out Pose targetPose)
		{
			currentPose = (useLocalSpace ? toStabilize.GetLocalPose() : toStabilize.GetWorldPose());
			targetPose = (useLocalSpace ? target.GetLocalPose() : target.GetWorldPose());
		}

		private static float CalculateScaleFactor(Transform toStabilize, bool useLocalSpace)
		{
			float num = useLocalSpace ? toStabilize.lossyScale.x : 1f;
			if (Mathf.Abs(num) >= 0.01f)
			{
				return num;
			}
			return 0.01f;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRTransformStabilizer.StabilizeTransform_000011D4$PostfixBurstDelegate))]
		private static void StabilizeTransform(in float3 startPos, in quaternion startRot, in float3 targetPos, in quaternion targetRot, float deltaTime, float positionStabilization, float angleStabilization, out float3 resultPos, out quaternion resultRot)
		{
			XRTransformStabilizer.StabilizeTransform_000011D4$BurstDirectCall.Invoke(startPos, startRot, targetPos, targetRot, deltaTime, positionStabilization, angleStabilization, out resultPos, out resultRot);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRTransformStabilizer.StabilizePosition_000011D5$PostfixBurstDelegate))]
		private static void StabilizePosition(in float3 startPos, in float3 targetPos, float deltaTime, float positionStabilization, out float3 resultPos)
		{
			XRTransformStabilizer.StabilizePosition_000011D5$BurstDirectCall.Invoke(startPos, targetPos, deltaTime, positionStabilization, out resultPos);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRTransformStabilizer.StabilizeOptimalRotation_000011D6$PostfixBurstDelegate))]
		private static void StabilizeOptimalRotation(in quaternion startRot, in quaternion targetRot, in quaternion alternateStartRot, float deltaTime, float angleStabilization, float alternateStabilization, float scaleFactor, out quaternion resultRot)
		{
			XRTransformStabilizer.StabilizeOptimalRotation_000011D6$BurstDirectCall.Invoke(startRot, targetRot, alternateStartRot, deltaTime, angleStabilization, alternateStabilization, scaleFactor, out resultRot);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRTransformStabilizer.CalculateStabilizedLerp_000011D7$PostfixBurstDelegate))]
		private static float CalculateStabilizedLerp(float distance, float timeSlice)
		{
			return XRTransformStabilizer.CalculateStabilizedLerp_000011D7$BurstDirectCall.Invoke(distance, timeSlice);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRTransformStabilizer.CalculateRotationParams_000011D8$PostfixBurstDelegate))]
		private static void CalculateRotationParams(in float3 currentPosition, in float3 resultPosition, in float3 forward, in float3 up, in float3 rayEnd, float invScale, float angleStabilization, out quaternion antiRotation, out float scaleFactor, out float targetAngleScale)
		{
			XRTransformStabilizer.CalculateRotationParams_000011D8$BurstDirectCall.Invoke(currentPosition, resultPosition, forward, up, rayEnd, invScale, angleStabilization, out antiRotation, out scaleFactor, out targetAngleScale);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void StabilizeTransform$BurstManaged(in float3 startPos, in quaternion startRot, in float3 targetPos, in quaternion targetRot, float deltaTime, float positionStabilization, float angleStabilization, out float3 resultPos, out quaternion resultRot)
		{
			if (positionStabilization > 0f)
			{
				float t = XRTransformStabilizer.CalculateStabilizedLerp(math.length(targetPos - startPos) / positionStabilization, deltaTime);
				resultPos = math.lerp(startPos, targetPos, t);
			}
			else
			{
				resultPos = targetPos;
			}
			if (angleStabilization > 0f)
			{
				float num;
				BurstMathUtility.Angle(targetRot, startRot, out num);
				float t2 = XRTransformStabilizer.CalculateStabilizedLerp(num / angleStabilization, deltaTime);
				resultRot = math.slerp(startRot, targetRot, t2);
				return;
			}
			resultRot = targetRot;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void StabilizePosition$BurstManaged(in float3 startPos, in float3 targetPos, float deltaTime, float positionStabilization, out float3 resultPos)
		{
			if (positionStabilization > 0f)
			{
				float t = XRTransformStabilizer.CalculateStabilizedLerp(math.length(targetPos - startPos) / positionStabilization, deltaTime);
				resultPos = math.lerp(startPos, targetPos, t);
				return;
			}
			resultPos = targetPos;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void StabilizeOptimalRotation$BurstManaged(in quaternion startRot, in quaternion targetRot, in quaternion alternateStartRot, float deltaTime, float angleStabilization, float alternateStabilization, float scaleFactor, out quaternion resultRot)
		{
			if (angleStabilization <= 0f)
			{
				resultRot = targetRot;
				return;
			}
			float num;
			BurstMathUtility.Angle(targetRot, startRot, out num);
			float num2 = num / angleStabilization;
			float num3;
			BurstMathUtility.Angle(targetRot, alternateStartRot, out num3);
			float num4 = num3 / alternateStabilization;
			if (num4 < num2)
			{
				num4 = XRTransformStabilizer.CalculateStabilizedLerp(num4, deltaTime * scaleFactor);
				resultRot = math.slerp(alternateStartRot, targetRot, num4);
				return;
			}
			num2 = XRTransformStabilizer.CalculateStabilizedLerp(num2, deltaTime * scaleFactor);
			resultRot = math.slerp(startRot, targetRot, num2);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float CalculateStabilizedLerp$BurstManaged(float distance, float timeSlice)
		{
			if (distance >= 1f)
			{
				return 1f;
			}
			if (distance <= 0f)
			{
				return 0f;
			}
			float num = distance - distance * distance;
			float num2 = num * num;
			float num3 = timeSlice / 0.011111111f;
			float num4 = math.clamp(num3, 0f, 1f);
			float num5 = math.clamp(num3 - 1f, 0f, 1f);
			float num6 = math.clamp(num3 - 2f, 0f, 1f);
			return distance * num4 + num * num5 + num2 * num6;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CalculateRotationParams$BurstManaged(in float3 currentPosition, in float3 resultPosition, in float3 forward, in float3 up, in float3 rayEnd, float invScale, float angleStabilization, out quaternion antiRotation, out float scaleFactor, out float targetAngleScale)
		{
			float num = math.length(rayEnd - currentPosition);
			float3 lhs = currentPosition + forward * num;
			antiRotation = quaternion.LookRotationSafe(lhs - resultPosition, up);
			scaleFactor = 1f + math.log(math.max(num * invScale, 1f));
			targetAngleScale = angleStabilization * math.clamp(scaleFactor, 1f, 3f);
		}

		private const float k_90FPS = 0.011111111f;

		[SerializeField]
		[Tooltip("The Transform component whose position and rotation will be matched and stabilized.")]
		private Transform m_Target;

		[SerializeField]
		[RequireInterface(typeof(IXRRayProvider))]
		[Tooltip("Optional - When provided a ray, the stabilizer will calculate the rotation that keeps a ray's endpoint stable.")]
		private Object m_AimTargetObject;

		private IXRRayProvider m_AimTarget;

		[SerializeField]
		[Tooltip("If enabled, will read the target and apply stabilization in local space. Otherwise, in world space.")]
		private bool m_UseLocalSpace;

		[Header("Stabilization Parameters")]
		[SerializeField]
		[Tooltip("Maximum distance (in degrees) that stabilization will be applied.")]
		private float m_AngleStabilization = 20f;

		[SerializeField]
		[Tooltip("Maximum distance (in meters) that stabilization will be applied.")]
		private float m_PositionStabilization = 0.25f;

		private Transform m_ThisTransform;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void StabilizeTransform_000011D4$PostfixBurstDelegate(in float3 startPos, in quaternion startRot, in float3 targetPos, in quaternion targetRot, float deltaTime, float positionStabilization, float angleStabilization, out float3 resultPos, out quaternion resultRot);

		internal static class StabilizeTransform_000011D4$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRTransformStabilizer.StabilizeTransform_000011D4$BurstDirectCall.Pointer == 0)
				{
					XRTransformStabilizer.StabilizeTransform_000011D4$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRTransformStabilizer.StabilizeTransform_000011D4$PostfixBurstDelegate>(new XRTransformStabilizer.StabilizeTransform_000011D4$PostfixBurstDelegate(XRTransformStabilizer.StabilizeTransform)).Value;
				}
				A_0 = XRTransformStabilizer.StabilizeTransform_000011D4$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRTransformStabilizer.StabilizeTransform_000011D4$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 startPos, in quaternion startRot, in float3 targetPos, in quaternion targetRot, float deltaTime, float positionStabilization, float angleStabilization, out float3 resultPos, out quaternion resultRot)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRTransformStabilizer.StabilizeTransform_000011D4$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.quaternion&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&,System.Single,System.Single,System.Single,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&), ref startPos, ref startRot, ref targetPos, ref targetRot, deltaTime, positionStabilization, angleStabilization, ref resultPos, ref resultRot, functionPointer);
						return;
					}
				}
				XRTransformStabilizer.StabilizeTransform$BurstManaged(startPos, startRot, targetPos, targetRot, deltaTime, positionStabilization, angleStabilization, out resultPos, out resultRot);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void StabilizePosition_000011D5$PostfixBurstDelegate(in float3 startPos, in float3 targetPos, float deltaTime, float positionStabilization, out float3 resultPos);

		internal static class StabilizePosition_000011D5$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRTransformStabilizer.StabilizePosition_000011D5$BurstDirectCall.Pointer == 0)
				{
					XRTransformStabilizer.StabilizePosition_000011D5$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRTransformStabilizer.StabilizePosition_000011D5$PostfixBurstDelegate>(new XRTransformStabilizer.StabilizePosition_000011D5$PostfixBurstDelegate(XRTransformStabilizer.StabilizePosition)).Value;
				}
				A_0 = XRTransformStabilizer.StabilizePosition_000011D5$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRTransformStabilizer.StabilizePosition_000011D5$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 startPos, in float3 targetPos, float deltaTime, float positionStabilization, out float3 resultPos)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRTransformStabilizer.StabilizePosition_000011D5$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,System.Single,Unity.Mathematics.float3&), ref startPos, ref targetPos, deltaTime, positionStabilization, ref resultPos, functionPointer);
						return;
					}
				}
				XRTransformStabilizer.StabilizePosition$BurstManaged(startPos, targetPos, deltaTime, positionStabilization, out resultPos);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void StabilizeOptimalRotation_000011D6$PostfixBurstDelegate(in quaternion startRot, in quaternion targetRot, in quaternion alternateStartRot, float deltaTime, float angleStabilization, float alternateStabilization, float scaleFactor, out quaternion resultRot);

		internal static class StabilizeOptimalRotation_000011D6$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRTransformStabilizer.StabilizeOptimalRotation_000011D6$BurstDirectCall.Pointer == 0)
				{
					XRTransformStabilizer.StabilizeOptimalRotation_000011D6$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRTransformStabilizer.StabilizeOptimalRotation_000011D6$PostfixBurstDelegate>(new XRTransformStabilizer.StabilizeOptimalRotation_000011D6$PostfixBurstDelegate(XRTransformStabilizer.StabilizeOptimalRotation)).Value;
				}
				A_0 = XRTransformStabilizer.StabilizeOptimalRotation_000011D6$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRTransformStabilizer.StabilizeOptimalRotation_000011D6$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in quaternion startRot, in quaternion targetRot, in quaternion alternateStartRot, float deltaTime, float angleStabilization, float alternateStabilization, float scaleFactor, out quaternion resultRot)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRTransformStabilizer.StabilizeOptimalRotation_000011D6$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.quaternion&,Unity.Mathematics.quaternion&,Unity.Mathematics.quaternion&,System.Single,System.Single,System.Single,System.Single,Unity.Mathematics.quaternion&), ref startRot, ref targetRot, ref alternateStartRot, deltaTime, angleStabilization, alternateStabilization, scaleFactor, ref resultRot, functionPointer);
						return;
					}
				}
				XRTransformStabilizer.StabilizeOptimalRotation$BurstManaged(startRot, targetRot, alternateStartRot, deltaTime, angleStabilization, alternateStabilization, scaleFactor, out resultRot);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate float CalculateStabilizedLerp_000011D7$PostfixBurstDelegate(float distance, float timeSlice);

		internal static class CalculateStabilizedLerp_000011D7$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRTransformStabilizer.CalculateStabilizedLerp_000011D7$BurstDirectCall.Pointer == 0)
				{
					XRTransformStabilizer.CalculateStabilizedLerp_000011D7$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRTransformStabilizer.CalculateStabilizedLerp_000011D7$PostfixBurstDelegate>(new XRTransformStabilizer.CalculateStabilizedLerp_000011D7$PostfixBurstDelegate(XRTransformStabilizer.CalculateStabilizedLerp)).Value;
				}
				A_0 = XRTransformStabilizer.CalculateStabilizedLerp_000011D7$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRTransformStabilizer.CalculateStabilizedLerp_000011D7$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static float Invoke(float distance, float timeSlice)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRTransformStabilizer.CalculateStabilizedLerp_000011D7$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Single(System.Single,System.Single), distance, timeSlice, functionPointer);
					}
				}
				return XRTransformStabilizer.CalculateStabilizedLerp$BurstManaged(distance, timeSlice);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateRotationParams_000011D8$PostfixBurstDelegate(in float3 currentPosition, in float3 resultPosition, in float3 forward, in float3 up, in float3 rayEnd, float invScale, float angleStabilization, out quaternion antiRotation, out float scaleFactor, out float targetAngleScale);

		internal static class CalculateRotationParams_000011D8$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRTransformStabilizer.CalculateRotationParams_000011D8$BurstDirectCall.Pointer == 0)
				{
					XRTransformStabilizer.CalculateRotationParams_000011D8$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRTransformStabilizer.CalculateRotationParams_000011D8$PostfixBurstDelegate>(new XRTransformStabilizer.CalculateRotationParams_000011D8$PostfixBurstDelegate(XRTransformStabilizer.CalculateRotationParams)).Value;
				}
				A_0 = XRTransformStabilizer.CalculateRotationParams_000011D8$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRTransformStabilizer.CalculateRotationParams_000011D8$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 currentPosition, in float3 resultPosition, in float3 forward, in float3 up, in float3 rayEnd, float invScale, float angleStabilization, out quaternion antiRotation, out float scaleFactor, out float targetAngleScale)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRTransformStabilizer.CalculateRotationParams_000011D8$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,System.Single,Unity.Mathematics.quaternion&,System.Single&,System.Single&), ref currentPosition, ref resultPosition, ref forward, ref up, ref rayEnd, invScale, angleStabilization, ref antiRotation, ref scaleFactor, ref targetAngleScale, functionPointer);
						return;
					}
				}
				XRTransformStabilizer.CalculateRotationParams$BurstManaged(currentPosition, resultPosition, forward, up, rayEnd, invScale, angleStabilization, out antiRotation, out scaleFactor, out targetAngleScale);
			}

			private static IntPtr Pointer;
		}
	}
}
