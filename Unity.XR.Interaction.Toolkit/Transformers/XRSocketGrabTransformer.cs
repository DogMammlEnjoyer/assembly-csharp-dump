using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Transformers
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Transformers.XRSocketGrabTransformer.html")]
	[BurstCompile]
	public class XRSocketGrabTransformer : IXRGrabTransformer
	{
		public bool canProcess { get; set; } = true;

		public float socketSnappingRadius { get; set; }

		public SocketScaleMode scaleMode { get; set; }

		internal bool scaleOnlyMode { get; set; }

		public float3 fixedScale { get; set; } = new float3(1f, 1f, 1f);

		public float3 targetBoundsSize { get; set; } = new float3(1f, 1f, 1f);

		public IXRInteractor socketInteractor { get; set; }

		public void OnLink(XRGrabInteractable grabInteractable)
		{
		}

		public void OnGrab(XRGrabInteractable grabInteractable)
		{
		}

		public void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
		{
			this.RegisterInteractableScale(grabInteractable, localScale);
		}

		public void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
		{
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic || updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
			{
				if (this.scaleMode == SocketScaleMode.None)
				{
					if (!this.scaleOnlyMode)
					{
						XRSocketGrabTransformer.UpdateTargetWithoutScale(grabInteractable, this.socketInteractor, this.socketSnappingRadius, ref targetPose);
						return;
					}
				}
				else
				{
					float3 @float = this.m_InitialScale[grabInteractable];
					float3 v = this.ComputeSocketTargetScale(grabInteractable, @float);
					if (!this.scaleOnlyMode)
					{
						float3 float2 = this.m_InteractableBoundsSize[grabInteractable];
						XRSocketGrabTransformer.UpdateTargetWithScale(grabInteractable, this.socketInteractor, this.socketSnappingRadius, @float, float2, v, ref targetPose, ref localScale);
						return;
					}
					localScale = v;
				}
			}
		}

		private static void UpdateTargetWithoutScale(XRGrabInteractable grabInteractable, IXRInteractor interactor, float snappingRadius, ref Pose targetPose)
		{
			Pose pose;
			if (!XRSocketGrabTransformer.GetTargetPoseForInteractable(grabInteractable, interactor, out pose))
			{
				return;
			}
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			if (ixrselectInteractor == null || !ixrselectInteractor.IsSelecting(grabInteractable))
			{
				float3 @float = targetPose.position;
				float3 float2 = pose.position;
				if (!XRSocketGrabTransformer.IsWithinRadius(@float, float2, snappingRadius))
				{
					return;
				}
			}
			targetPose = pose;
		}

		private static void UpdateTargetWithScale(XRGrabInteractable grabInteractable, IXRInteractor interactor, float innerRadius, in float3 initialScale, in float3 initialBounds, in float3 targetScale, ref Pose targetPose, ref Vector3 localScale)
		{
			Pose pose;
			if (!XRSocketGrabTransformer.GetTargetPoseForInteractable(grabInteractable, interactor, out pose))
			{
				return;
			}
			Vector3 position = grabInteractable.transform.position;
			bool flag = BurstMathUtility.FastVectorEquals(position, pose.position, 0.01f);
			float num = XRSocketGrabTransformer.FastCalculateRadiusOffset(initialScale, targetScale, initialBounds, innerRadius);
			float radius = flag ? num : innerRadius;
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			if (ixrselectInteractor == null || !ixrselectInteractor.IsSelecting(grabInteractable))
			{
				float3 @float = targetPose.position;
				float3 float2 = pose.position;
				if (!XRSocketGrabTransformer.IsWithinRadius(@float, float2, radius))
				{
					localScale = initialScale;
					return;
				}
			}
			targetPose = pose;
			if (flag)
			{
				localScale = targetScale;
			}
		}

		public void OnUnlink(XRGrabInteractable grabInteractable)
		{
			float3 v;
			if (this.m_InitialScale.TryGetValue(grabInteractable, out v))
			{
				grabInteractable.SetTargetLocalScale(v);
				this.m_InitialScale.Remove(grabInteractable);
				this.m_InteractableBoundsSize.Remove(grabInteractable);
			}
		}

		private bool RegisterInteractableScale(IXRInteractable targetInteractable, Vector3 scale)
		{
			if (!this.m_InitialScale.TryAdd(targetInteractable, scale))
			{
				return false;
			}
			Transform transform = targetInteractable.transform;
			Pose worldPose = transform.GetWorldPose();
			transform.SetWorldPose(Pose.identity);
			this.m_InteractableBoundsSize[targetInteractable] = BoundsUtils.GetBounds(targetInteractable.transform).size;
			transform.SetWorldPose(worldPose);
			return true;
		}

		private float3 ComputeSocketTargetScale(IXRInteractable interactable, in float3 initialInteractableScale)
		{
			switch (this.scaleMode)
			{
			case SocketScaleMode.Fixed:
			{
				float3 @float = this.fixedScale;
				float3 result;
				BurstMathUtility.Scale(initialInteractableScale, @float, out result);
				return result;
			}
			case SocketScaleMode.StretchedToFitSize:
			{
				float3 float2;
				if (!this.m_InteractableBoundsSize.TryGetValue(interactable, out float2))
				{
					return initialInteractableScale;
				}
				float3 @float = this.targetBoundsSize;
				float3 result2;
				XRSocketGrabTransformer.CalculateScaleToFit(float2, @float, initialInteractableScale, Mathf.Epsilon, out result2);
				return result2;
			}
			}
			return initialInteractableScale;
		}

		private static bool GetTargetPoseForInteractable(IXRInteractable interactable, IXRInteractor interactor, out Pose targetPose)
		{
			targetPose = Pose.identity;
			XRGrabInteractable xrgrabInteractable = interactable as XRGrabInteractable;
			if (xrgrabInteractable == null)
			{
				return false;
			}
			Transform attachTransform = interactor.GetAttachTransform(xrgrabInteractable);
			Transform transform = xrgrabInteractable.transform;
			Transform attachTransform2 = xrgrabInteractable.GetAttachTransform(interactor);
			Vector3 vector = transform.position - attachTransform2.position;
			if (xrgrabInteractable.trackRotation)
			{
				Vector3 v = attachTransform2.InverseTransformDirection(vector);
				float3 @float = attachTransform.position;
				quaternion quaternion = attachTransform.rotation;
				float3 float2 = v;
				quaternion quaternion2 = transform.rotation;
				quaternion quaternion3 = attachTransform2.rotation;
				float3 v2;
				quaternion q;
				XRSocketGrabTransformer.FastComputeNewTrackedPose(@float, quaternion, float2, quaternion2, quaternion3, out v2, out q);
				targetPose.position = v2;
				targetPose.rotation = q;
			}
			else
			{
				targetPose.position = vector + attachTransform.position;
			}
			return true;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$PostfixBurstDelegate))]
		private static float FastCalculateRadiusOffset(in float3 initialScale, in float3 targetScale, in float3 initialBoundsSize, float innerRadius)
		{
			return XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$BurstDirectCall.Invoke(initialScale, targetScale, initialBoundsSize, innerRadius);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$PostfixBurstDelegate))]
		private static void FastComputeNewTrackedPose(in float3 interactorAttachPos, in quaternion interactorAttachRot, in float3 positionOffset, in quaternion interactableRot, in quaternion interactableAttachRot, out float3 targetPos, out quaternion targetRot)
		{
			XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$BurstDirectCall.Invoke(interactorAttachPos, interactorAttachRot, positionOffset, interactableRot, interactableAttachRot, out targetPos, out targetRot);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRSocketGrabTransformer.IsWithinRadius_00000930$PostfixBurstDelegate))]
		private static bool IsWithinRadius(in float3 a, in float3 b, float radius)
		{
			return XRSocketGrabTransformer.IsWithinRadius_00000930$BurstDirectCall.Invoke(a, b, radius);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRSocketGrabTransformer.CalculateScaleToFit_00000931$PostfixBurstDelegate))]
		private static void CalculateScaleToFit(in float3 boundsSize, in float3 fixedSize, in float3 initialScale, float epsilon, out float3 newScale)
		{
			XRSocketGrabTransformer.CalculateScaleToFit_00000931$BurstDirectCall.Invoke(boundsSize, fixedSize, initialScale, epsilon, out newScale);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float FastCalculateRadiusOffset$BurstManaged(in float3 initialScale, in float3 targetScale, in float3 initialBoundsSize, float innerRadius)
		{
			float x = math.max(math.max(initialBoundsSize.x, initialBoundsSize.y), initialBoundsSize.z);
			float3 @float;
			BurstMathUtility.FastSafeDivide(targetScale, initialScale, out @float, 1E-06f);
			float x2 = @float.x * initialBoundsSize.x;
			float y = @float.y * initialBoundsSize.y;
			float y2 = @float.z * initialBoundsSize.z;
			float y3 = math.max(math.max(x2, y), y2);
			float num = math.max(x, y3);
			return innerRadius + num / 2f;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FastComputeNewTrackedPose$BurstManaged(in float3 interactorAttachPos, in quaternion interactorAttachRot, in float3 positionOffset, in quaternion interactableRot, in quaternion interactableAttachRot, out float3 targetPos, out quaternion targetRot)
		{
			quaternion b = math.inverse(math.mul(math.inverse(interactableRot), interactableAttachRot));
			targetPos = math.mul(interactorAttachRot, positionOffset) + interactorAttachPos;
			targetRot = math.mul(interactorAttachRot, b);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsWithinRadius$BurstManaged(in float3 a, in float3 b, float radius)
		{
			return math.lengthsq(a - b) < radius * radius;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CalculateScaleToFit$BurstManaged(in float3 boundsSize, in float3 fixedSize, in float3 initialScale, float epsilon, out float3 newScale)
		{
			float x = boundsSize.x / (fixedSize.x + epsilon);
			float y = boundsSize.y / (fixedSize.y + epsilon);
			float y2 = boundsSize.z / (fixedSize.z + epsilon);
			float rhs = math.max(math.max(x, y), y2);
			newScale = initialScale / rhs;
		}

		private const float k_SocketSnappingAxisTolerance = 0.01f;

		private readonly Dictionary<IXRInteractable, float3> m_InitialScale = new Dictionary<IXRInteractable, float3>();

		private readonly Dictionary<IXRInteractable, float3> m_InteractableBoundsSize = new Dictionary<IXRInteractable, float3>();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate float FastCalculateRadiusOffset_0000092E$PostfixBurstDelegate(in float3 initialScale, in float3 targetScale, in float3 initialBoundsSize, float innerRadius);

		internal static class FastCalculateRadiusOffset_0000092E$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$BurstDirectCall.Pointer == 0)
				{
					XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$PostfixBurstDelegate>(new XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$PostfixBurstDelegate(XRSocketGrabTransformer.FastCalculateRadiusOffset)).Value;
				}
				A_0 = XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static float Invoke(in float3 initialScale, in float3 targetScale, in float3 initialBoundsSize, float innerRadius)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRSocketGrabTransformer.FastCalculateRadiusOffset_0000092E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Single(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single), ref initialScale, ref targetScale, ref initialBoundsSize, innerRadius, functionPointer);
					}
				}
				return XRSocketGrabTransformer.FastCalculateRadiusOffset$BurstManaged(initialScale, targetScale, initialBoundsSize, innerRadius);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void FastComputeNewTrackedPose_0000092F$PostfixBurstDelegate(in float3 interactorAttachPos, in quaternion interactorAttachRot, in float3 positionOffset, in quaternion interactableRot, in quaternion interactableAttachRot, out float3 targetPos, out quaternion targetRot);

		internal static class FastComputeNewTrackedPose_0000092F$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$BurstDirectCall.Pointer == 0)
				{
					XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$PostfixBurstDelegate>(new XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$PostfixBurstDelegate(XRSocketGrabTransformer.FastComputeNewTrackedPose)).Value;
				}
				A_0 = XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 interactorAttachPos, in quaternion interactorAttachRot, in float3 positionOffset, in quaternion interactableRot, in quaternion interactableAttachRot, out float3 targetPos, out quaternion targetRot)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRSocketGrabTransformer.FastComputeNewTrackedPose_0000092F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.quaternion&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&,Unity.Mathematics.quaternion&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&), ref interactorAttachPos, ref interactorAttachRot, ref positionOffset, ref interactableRot, ref interactableAttachRot, ref targetPos, ref targetRot, functionPointer);
						return;
					}
				}
				XRSocketGrabTransformer.FastComputeNewTrackedPose$BurstManaged(interactorAttachPos, interactorAttachRot, positionOffset, interactableRot, interactableAttachRot, out targetPos, out targetRot);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate bool IsWithinRadius_00000930$PostfixBurstDelegate(in float3 a, in float3 b, float radius);

		internal static class IsWithinRadius_00000930$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRSocketGrabTransformer.IsWithinRadius_00000930$BurstDirectCall.Pointer == 0)
				{
					XRSocketGrabTransformer.IsWithinRadius_00000930$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRSocketGrabTransformer.IsWithinRadius_00000930$PostfixBurstDelegate>(new XRSocketGrabTransformer.IsWithinRadius_00000930$PostfixBurstDelegate(XRSocketGrabTransformer.IsWithinRadius)).Value;
				}
				A_0 = XRSocketGrabTransformer.IsWithinRadius_00000930$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRSocketGrabTransformer.IsWithinRadius_00000930$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static bool Invoke(in float3 a, in float3 b, float radius)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRSocketGrabTransformer.IsWithinRadius_00000930$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single), ref a, ref b, radius, functionPointer);
					}
				}
				return XRSocketGrabTransformer.IsWithinRadius$BurstManaged(a, b, radius);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CalculateScaleToFit_00000931$PostfixBurstDelegate(in float3 boundsSize, in float3 fixedSize, in float3 initialScale, float epsilon, out float3 newScale);

		internal static class CalculateScaleToFit_00000931$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRSocketGrabTransformer.CalculateScaleToFit_00000931$BurstDirectCall.Pointer == 0)
				{
					XRSocketGrabTransformer.CalculateScaleToFit_00000931$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRSocketGrabTransformer.CalculateScaleToFit_00000931$PostfixBurstDelegate>(new XRSocketGrabTransformer.CalculateScaleToFit_00000931$PostfixBurstDelegate(XRSocketGrabTransformer.CalculateScaleToFit)).Value;
				}
				A_0 = XRSocketGrabTransformer.CalculateScaleToFit_00000931$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRSocketGrabTransformer.CalculateScaleToFit_00000931$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 boundsSize, in float3 fixedSize, in float3 initialScale, float epsilon, out float3 newScale)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRSocketGrabTransformer.CalculateScaleToFit_00000931$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&), ref boundsSize, ref fixedSize, ref initialScale, epsilon, ref newScale, functionPointer);
						return;
					}
				}
				XRSocketGrabTransformer.CalculateScaleToFit$BurstManaged(boundsSize, fixedSize, initialScale, epsilon, out newScale);
			}

			private static IntPtr Pointer;
		}
	}
}
