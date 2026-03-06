using System;
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
	[AddComponentMenu("XR/Transformers/XR General Grab Transformer", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Transformers.XRGeneralGrabTransformer.html")]
	[BurstCompile]
	public class XRGeneralGrabTransformer : XRBaseGrabTransformer
	{
		public XRGeneralGrabTransformer.ManipulationAxes permittedDisplacementAxes
		{
			get
			{
				return this.m_PermittedDisplacementAxes;
			}
			set
			{
				this.m_PermittedDisplacementAxes = value;
			}
		}

		public XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode constrainedAxisDisplacementMode
		{
			get
			{
				return this.m_ConstrainedAxisDisplacementMode;
			}
			set
			{
				this.m_ConstrainedAxisDisplacementMode = value;
			}
		}

		public XRGeneralGrabTransformer.TwoHandedRotationMode allowTwoHandedRotation
		{
			get
			{
				return this.m_TwoHandedRotationMode;
			}
			set
			{
				this.m_TwoHandedRotationMode = value;
			}
		}

		public bool allowOneHandedScaling
		{
			get
			{
				return this.m_AllowOneHandedScaling;
			}
			set
			{
				this.m_AllowOneHandedScaling = value;
			}
		}

		public bool allowTwoHandedScaling
		{
			get
			{
				return this.m_AllowTwoHandedScaling;
			}
			set
			{
				this.m_AllowTwoHandedScaling = value;
			}
		}

		public float oneHandedScaleSpeed
		{
			get
			{
				return this.m_OneHandedScaleSpeed;
			}
			set
			{
				this.m_OneHandedScaleSpeed = Mathf.Max(value, 0f);
			}
		}

		public float thresholdMoveRatioForScale
		{
			get
			{
				return this.m_ThresholdMoveRatioForScale;
			}
			set
			{
				this.m_ThresholdMoveRatioForScale = value;
			}
		}

		public bool clampScaling
		{
			get
			{
				return this.m_ClampScaling;
			}
			set
			{
				this.m_ClampScaling = value;
			}
		}

		public float minimumScaleRatio
		{
			get
			{
				return this.m_MinimumScaleRatio;
			}
			set
			{
				this.m_MinimumScaleRatio = Mathf.Min(1f, value);
				this.m_MinimumScale = this.m_InitialScale * this.m_MinimumScaleRatio;
			}
		}

		public float maximumScaleRatio
		{
			get
			{
				return this.m_MaximumScaleRatio;
			}
			set
			{
				this.m_MaximumScaleRatio = Mathf.Max(1f, value);
				this.m_MaximumScale = this.m_InitialScale * this.m_MaximumScaleRatio;
			}
		}

		public float scaleMultiplier
		{
			get
			{
				return this.m_ScaleMultiplier;
			}
			set
			{
				this.m_ScaleMultiplier = value;
			}
		}

		protected override XRBaseGrabTransformer.RegistrationMode registrationMode
		{
			get
			{
				return XRBaseGrabTransformer.RegistrationMode.SingleAndMultiple;
			}
		}

		protected void Awake()
		{
		}

		public override void OnLink(XRGrabInteractable grabInteractable)
		{
			base.OnLink(grabInteractable);
			this.m_InitialScale = grabInteractable.transform.localScale;
			float num = Mathf.Max(new float[]
			{
				Mathf.Abs(this.m_InitialScale.x),
				Mathf.Abs(this.m_InitialScale.y),
				Mathf.Abs(this.m_InitialScale.z)
			});
			this.m_InitialScaleProportions = this.m_InitialScale.SafeDivide(new Vector3(num, num, num));
		}

		public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
		{
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic || updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
			{
				this.UpdateTarget(grabInteractable, ref targetPose, ref localScale);
			}
		}

		public override void OnGrab(XRGrabInteractable grabInteractable)
		{
			base.OnGrab(grabInteractable);
			IXRSelectInteractor ixrselectInteractor = grabInteractable.interactorsSelecting[0];
			Transform transform = grabInteractable.transform;
			Transform attachTransform = grabInteractable.GetAttachTransform(ixrselectInteractor);
			this.m_ScaleValueProvider = (ixrselectInteractor as IXRScaleValueProvider);
			this.m_HasScaleValueProvider = (this.m_ScaleValueProvider != null);
			this.m_OriginalObjectPose = transform.GetWorldPose();
			this.m_OriginalInteractorPose = ixrselectInteractor.GetAttachTransform(grabInteractable).GetWorldPose();
			this.m_OriginalInteractor = ixrselectInteractor;
			this.m_LastGrabCount = 1;
			Vector3 value = Vector3.zero;
			Quaternion lhs = Quaternion.identity;
			Quaternion rotation = this.m_OriginalObjectPose.rotation;
			if (grabInteractable.trackRotation)
			{
				rotation = this.m_OriginalInteractorPose.rotation;
				lhs = Quaternion.Inverse(Quaternion.Inverse(this.m_OriginalObjectPose.rotation) * attachTransform.rotation);
			}
			Vector3 position = this.m_OriginalObjectPose.position;
			if (grabInteractable.trackPosition)
			{
				position = this.m_OriginalInteractorPose.position;
				Vector3 vector = this.m_OriginalObjectPose.position - attachTransform.position;
				value = (grabInteractable.trackRotation ? attachTransform.InverseTransformDirection(vector) : vector);
			}
			this.m_ConstrainedAxisDisplacementModeOnGrab = this.m_ConstrainedAxisDisplacementMode;
			this.m_PermittedDisplacementAxesOnGrab = this.m_PermittedDisplacementAxes;
			position = XRGeneralGrabTransformer.AdjustPositionForPermittedAxes(position, this.m_OriginalObjectPose, this.m_PermittedDisplacementAxesOnGrab, this.m_ConstrainedAxisDisplacementModeOnGrab);
			this.m_OriginalObjectPose = new Pose(position, rotation);
			Vector3 localScale = transform.localScale;
			this.TranslateSetup(this.m_OriginalInteractorPose, this.m_OriginalInteractorPose.position, this.m_OriginalObjectPose, localScale);
			Quaternion rotation2 = lhs * Quaternion.Inverse(this.m_OriginalInteractorPose.rotation) * this.m_OriginalObjectPose.rotation;
			Vector3 position2 = value.Divide(localScale);
			this.m_OffsetPose = new Pose(position2, rotation2);
		}

		public override void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
		{
			base.OnGrabCountChanged(grabInteractable, targetPose, localScale);
			int count = grabInteractable.interactorsSelecting.Count;
			if (count == 1)
			{
				if (grabInteractable.interactorsSelecting[0] != this.m_OriginalInteractor || count < this.m_LastGrabCount)
				{
					this.OnGrab(grabInteractable);
				}
			}
			else if (count > 1)
			{
				IXRInteractor ixrinteractor = grabInteractable.interactorsSelecting[0];
				IXRSelectInteractor interactor = grabInteractable.interactorsSelecting[1];
				Transform attachTransform = ixrinteractor.GetAttachTransform(grabInteractable);
				Transform attachTransform2 = grabInteractable.GetAttachTransform(interactor);
				this.m_ScaleAtGrabStart = localScale;
				this.m_StartHandleBar = attachTransform.InverseTransformPoint(attachTransform2.position);
				this.m_StartHandleBarNormalized = this.m_StartHandleBar.normalized;
				this.m_StartHandleBarLookRotation = Quaternion.LookRotation(this.m_StartHandleBarNormalized, BurstMathUtility.Orthogonal(this.m_StartHandleBarNormalized));
				this.m_StartHandleBarUp = this.m_StartHandleBarLookRotation * Vector3.up;
				this.m_InverseStartHandleBarLookRotation = Quaternion.Inverse(this.m_StartHandleBarLookRotation);
				this.m_LastHandleBarLocalRotation = this.m_StartHandleBarLookRotation;
				this.m_FirstFrameSinceTwoHandedGrab = true;
			}
			this.m_LastGrabCount = count;
			this.m_MinimumScale = this.m_InitialScale * this.m_MinimumScaleRatio;
			this.m_MaximumScale = this.m_InitialScale * this.m_MaximumScaleRatio;
		}

		private void ComputeAdjustedInteractorPose(XRGrabInteractable grabInteractable, out Vector3 newHandleBar, out Vector3 adjustedInteractorPosition, out Quaternion adjustedInteractorRotation)
		{
			if (grabInteractable.interactorsSelecting.Count == 1 || this.m_TwoHandedRotationMode == XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandOnly)
			{
				newHandleBar = this.m_StartHandleBar;
				Pose worldPose = grabInteractable.interactorsSelecting[0].GetAttachTransform(grabInteractable).GetWorldPose();
				adjustedInteractorPosition = worldPose.position;
				adjustedInteractorRotation = worldPose.rotation;
				return;
			}
			if (grabInteractable.interactorsSelecting.Count > 1)
			{
				IXRInteractor ixrinteractor = grabInteractable.interactorsSelecting[0];
				IXRSelectInteractor ixrselectInteractor = grabInteractable.interactorsSelecting[1];
				Transform attachTransform = ixrinteractor.GetAttachTransform(grabInteractable);
				Transform attachTransform2 = ixrselectInteractor.GetAttachTransform(grabInteractable);
				newHandleBar = attachTransform.InverseTransformPoint(attachTransform2.position);
				Quaternion quaternion2;
				if (this.m_TwoHandedRotationMode == XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand)
				{
					Vector3 normalized = newHandleBar.normalized;
					Vector3 vector = this.m_LastHandleBarLocalRotation * Vector3.up;
					float num = Vector3.Dot(this.m_StartHandleBarUp, vector);
					Vector3 upwards = vector;
					if (num > 0f)
					{
						float num2 = num * 0.5f;
						float t = num2 * num2;
						upwards = Vector3.Lerp(vector, this.m_StartHandleBarUp, t);
					}
					Quaternion quaternion = Quaternion.LookRotation(normalized, upwards);
					this.m_LastHandleBarLocalRotation = quaternion;
					Quaternion rhs = quaternion * this.m_InverseStartHandleBarLookRotation;
					quaternion2 = attachTransform.rotation * rhs;
				}
				else if (this.m_TwoHandedRotationMode == XRGeneralGrabTransformer.TwoHandedRotationMode.TwoHandedAverage)
				{
					Vector3 normalized2 = (attachTransform2.position - attachTransform.position).normalized;
					Vector3 rhs2 = Vector3.Slerp(attachTransform.right, attachTransform2.right, 0.5f);
					Vector3 vector2 = Vector3.Slerp(attachTransform.up, attachTransform2.up, 0.5f);
					Vector3 a = Vector3.Cross(normalized2, rhs2);
					float num3 = Mathf.PingPong(Vector3.Angle(vector2, normalized2), 90f);
					vector2 = Vector3.Slerp(a, vector2, num3 / 90f);
					Vector3 rhs3 = Vector3.Cross(vector2, normalized2);
					vector2 = Vector3.Cross(normalized2, rhs3);
					if (this.m_FirstFrameSinceTwoHandedGrab)
					{
						this.m_FirstFrameSinceTwoHandedGrab = false;
					}
					else if (Vector3.Dot(vector2, this.m_LastTwoHandedUp) <= 0f)
					{
						vector2 = -vector2;
					}
					this.m_LastTwoHandedUp = vector2;
					quaternion2 = Quaternion.LookRotation(normalized2, vector2) * Quaternion.Inverse(this.m_OffsetPose.rotation);
				}
				else
				{
					quaternion2 = attachTransform.rotation;
				}
				adjustedInteractorPosition = attachTransform.position;
				adjustedInteractorRotation = quaternion2;
				return;
			}
			newHandleBar = this.m_StartHandleBar;
			adjustedInteractorPosition = Vector3.zero;
			adjustedInteractorRotation = Quaternion.identity;
		}

		private void TranslateSetup(Pose interactorCentroidPose, Vector3 grabCentroid, Pose objectPose, Vector3 objectScale)
		{
			Quaternion rotation = Quaternion.Inverse(interactorCentroidPose.rotation);
			this.m_InteractorLocalGrabPoint = rotation * (grabCentroid - interactorCentroidPose.position);
			this.m_ObjectLocalGrabPoint = Quaternion.Inverse(objectPose.rotation) * (grabCentroid - objectPose.position);
			this.m_ObjectLocalGrabPoint = this.m_ObjectLocalGrabPoint.Divide(objectScale);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$PostfixBurstDelegate))]
		private static void ComputeNewObjectPosition(in float3 interactorPosition, in quaternion interactorRotation, in quaternion objectRotation, in float3 objectScale, bool trackRotation, in float3 offsetPosition, in float3 objectLocalGrabPoint, in float3 interactorLocalGrabPoint, out Vector3 newPosition)
		{
			XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$BurstDirectCall.Invoke(interactorPosition, interactorRotation, objectRotation, objectScale, trackRotation, offsetPosition, objectLocalGrabPoint, interactorLocalGrabPoint, out newPosition);
		}

		private static float3 Scale(float3 a, float3 b)
		{
			return new float3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		private Quaternion ComputeNewObjectRotation(in Quaternion interactorRotation, bool trackRotation)
		{
			if (!trackRotation)
			{
				return this.m_OriginalObjectPose.rotation;
			}
			return interactorRotation * this.m_OffsetPose.rotation;
		}

		private static Vector3 AdjustPositionForPermittedAxes(in Vector3 targetPosition, in Pose originalObjectPose, XRGeneralGrabTransformer.ManipulationAxes permittedAxes, XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode axisDisplacementMode)
		{
			bool flag = (permittedAxes & XRGeneralGrabTransformer.ManipulationAxes.X) > (XRGeneralGrabTransformer.ManipulationAxes)0;
			bool flag2 = (permittedAxes & XRGeneralGrabTransformer.ManipulationAxes.Y) > (XRGeneralGrabTransformer.ManipulationAxes)0;
			bool flag3 = (permittedAxes & XRGeneralGrabTransformer.ManipulationAxes.Z) > (XRGeneralGrabTransformer.ManipulationAxes)0;
			if (flag && flag2 && flag3)
			{
				return targetPosition;
			}
			if (!flag && !flag2 && !flag3)
			{
				return originalObjectPose.position;
			}
			Vector3 result;
			XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst(targetPosition, originalObjectPose, axisDisplacementMode, flag, flag2, flag3, out result);
			return result;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$PostfixBurstDelegate))]
		private static void AdjustPositionForPermittedAxesBurst(in Vector3 targetPosition, in Pose originalObjectPose, XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode axisDisplacementMode, bool hasX, bool hasY, bool hasZ, out Vector3 adjustedTargetPosition)
		{
			XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$BurstDirectCall.Invoke(targetPosition, originalObjectPose, axisDisplacementMode, hasX, hasY, hasZ, out adjustedTargetPosition);
		}

		private Vector3 ComputeNewScale(in XRGrabInteractable grabInteractable, in Vector3 startScale, in Vector3 currentScale, in Vector3 startHandleBar, in Vector3 newHandleBar, bool trackScale)
		{
			int count = grabInteractable.interactorsSelecting.Count;
			if (trackScale && count == 1 && this.m_AllowOneHandedScaling && this.m_HasScaleValueProvider && this.m_ScaleValueProvider.scaleMode == ScaleMode.ScaleOverTime)
			{
				float scaleValue = this.m_ScaleValueProvider.scaleValue;
				if (Mathf.Approximately(scaleValue, 0f))
				{
					return currentScale;
				}
				Vector3 result;
				XRGeneralGrabTransformer.ComputeNewOneHandedScale(currentScale, this.m_InitialScaleProportions, this.m_ClampScaling, this.m_MinimumScale, this.m_MaximumScale, scaleValue, Time.deltaTime, this.m_OneHandedScaleSpeed, out result);
				return result;
			}
			else
			{
				if (trackScale && count > 1 && this.m_AllowTwoHandedScaling)
				{
					Vector3 result2;
					XRGeneralGrabTransformer.ComputeNewTwoHandedScale(startScale, currentScale, startHandleBar, newHandleBar, this.m_ClampScaling, this.m_ScaleMultiplier, this.m_ThresholdMoveRatioForScale, this.m_MinimumScale, this.m_MaximumScale, out result2);
					return result2;
				}
				return currentScale;
			}
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$PostfixBurstDelegate))]
		private static void ComputeNewOneHandedScale(in Vector3 currentScale, in Vector3 initialScaleProportions, bool clampScale, in Vector3 minScale, in Vector3 maxScale, float scaleInput, float deltaTime, float scaleSpeed, out Vector3 newScale)
		{
			XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$BurstDirectCall.Invoke(currentScale, initialScaleProportions, clampScale, minScale, maxScale, scaleInput, deltaTime, scaleSpeed, out newScale);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$PostfixBurstDelegate))]
		private static void ComputeNewTwoHandedScale(in Vector3 startScale, in Vector3 currentScale, in Vector3 startHandleBar, in Vector3 newHandleBar, bool clampScale, float scaleMultiplier, float thresholdMoveRatioForScale, in Vector3 minScale, in Vector3 maxScale, out Vector3 newScale)
		{
			XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$BurstDirectCall.Invoke(startScale, currentScale, startHandleBar, newHandleBar, clampScale, scaleMultiplier, thresholdMoveRatioForScale, minScale, maxScale, out newScale);
		}

		private void UpdateTarget(XRGrabInteractable grabInteractable, ref Pose targetPose, ref Vector3 localScale)
		{
			Vector3 vector;
			Vector3 v;
			Quaternion q;
			this.ComputeAdjustedInteractorPose(grabInteractable, out vector, out v, out q);
			localScale = this.ComputeNewScale(grabInteractable, this.m_ScaleAtGrabStart, localScale, this.m_StartHandleBar, vector, grabInteractable.trackScale);
			targetPose.rotation = this.ComputeNewObjectRotation(q, grabInteractable.trackRotation);
			float3 @float = v;
			quaternion quaternion = q;
			quaternion quaternion2 = targetPose.rotation;
			float3 float2 = localScale;
			bool trackRotation = grabInteractable.trackRotation;
			float3 float3 = this.m_OffsetPose.position;
			float3 float4 = this.m_ObjectLocalGrabPoint;
			float3 float5 = this.m_InteractorLocalGrabPoint;
			Vector3 vector2;
			XRGeneralGrabTransformer.ComputeNewObjectPosition(@float, quaternion, quaternion2, float2, trackRotation, float3, float4, float5, out vector2);
			targetPose.position = XRGeneralGrabTransformer.AdjustPositionForPermittedAxes(vector2, this.m_OriginalObjectPose, this.m_PermittedDisplacementAxesOnGrab, this.m_ConstrainedAxisDisplacementModeOnGrab);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ComputeNewObjectPosition$BurstManaged(in float3 interactorPosition, in quaternion interactorRotation, in quaternion objectRotation, in float3 objectScale, bool trackRotation, in float3 offsetPosition, in float3 objectLocalGrabPoint, in float3 interactorLocalGrabPoint, out Vector3 newPosition)
		{
			float3 @float = XRGeneralGrabTransformer.Scale(offsetPosition, objectScale);
			float3 float2 = math.mul(interactorRotation, @float);
			float3 rhs = trackRotation ? float2 : @float;
			float3 rhs2 = interactorPosition + rhs;
			float3 v = XRGeneralGrabTransformer.Scale(objectLocalGrabPoint, objectScale);
			float3 float3 = interactorLocalGrabPoint;
			float3 = math.mul(interactorRotation, float3);
			float3 rhs3 = math.mul(objectRotation, v);
			newPosition = float3 - rhs3 + rhs2;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void AdjustPositionForPermittedAxesBurst$BurstManaged(in Vector3 targetPosition, in Pose originalObjectPose, XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode axisDisplacementMode, bool hasX, bool hasY, bool hasZ, out Vector3 adjustedTargetPosition)
		{
			float3 lhs = float3.zero;
			float3 rhs = float3.zero;
			float3 rhs2 = float3.zero;
			float3 @float = new float3(1f, 0f, 0f);
			float3 float2 = new float3(0f, 1f, 0f);
			float3 float3 = new float3(0f, 0f, 1f);
			float3 a = targetPosition - originalObjectPose.position;
			float3 rhs3 = float3.zero;
			float3 lhs2 = originalObjectPose.position;
			quaternion q = originalObjectPose.rotation;
			if (axisDisplacementMode == XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode.WorldAxisRelative)
			{
				if (hasX)
				{
					lhs = math.project(a, @float);
				}
				if (hasY)
				{
					rhs = math.project(a, float2);
				}
				if (hasZ)
				{
					rhs2 = math.project(a, float3);
				}
				rhs3 = lhs + rhs + rhs2;
			}
			else if (axisDisplacementMode == XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode.ObjectRelative)
			{
				if (hasX)
				{
					float3 ontoB = math.mul(q, @float);
					lhs = math.project(a, ontoB);
				}
				if (hasY)
				{
					float3 ontoB2 = math.mul(q, float2);
					rhs = math.project(a, ontoB2);
				}
				if (hasZ)
				{
					float3 ontoB3 = math.mul(q, float3);
					rhs2 = math.project(a, ontoB3);
				}
				rhs3 = lhs + rhs + rhs2;
			}
			else if (axisDisplacementMode == XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode.ObjectRelativeWithLockedWorldUp)
			{
				if (hasX && hasZ)
				{
					BurstMathUtility.ProjectOnPlane(a, float2, out rhs3);
				}
				else
				{
					float3 rhs4 = Vector3.zero;
					if (hasX)
					{
						float3 ontoB4 = math.mul(q, @float);
						lhs = math.project(a, ontoB4);
					}
					if (hasY)
					{
						float3 ontoB5 = math.mul(q, float2);
						rhs = math.project(a, ontoB5);
						rhs4 = math.project(a, float2);
					}
					if (hasZ)
					{
						float3 ontoB6 = math.mul(q, float3);
						rhs2 = math.project(a, ontoB6);
					}
					float3 float4 = lhs + rhs + rhs2;
					float3 lhs3;
					BurstMathUtility.ProjectOnPlane(float4, float2, out lhs3);
					rhs3 = lhs3 + rhs4;
				}
			}
			adjustedTargetPosition = lhs2 + rhs3;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ComputeNewOneHandedScale$BurstManaged(in Vector3 currentScale, in Vector3 initialScaleProportions, bool clampScale, in Vector3 minScale, in Vector3 maxScale, float scaleInput, float deltaTime, float scaleSpeed, out Vector3 newScale)
		{
			newScale = currentScale;
			float num = scaleInput * deltaTime * scaleSpeed;
			float3 @float = new float3(num, num, num);
			float3 float2 = initialScaleProportions;
			float3 rhs;
			BurstMathUtility.Scale(@float, float2, out rhs);
			float3 float3 = currentScale + rhs;
			if (!clampScale)
			{
				newScale = math.max(float3, float3.zero);
				return;
			}
			if (num > 0f)
			{
				newScale = ((math.abs(float3.x) > math.abs(maxScale.x) || math.abs(float3.y) > math.abs(maxScale.y) || math.abs(float3.z) > math.abs(maxScale.z)) ? maxScale : float3);
				return;
			}
			if (num < 0f)
			{
				newScale = ((math.abs(float3.x) < math.abs(minScale.x) || math.abs(float3.y) < math.abs(minScale.y) || math.abs(float3.z) < math.abs(minScale.z)) ? minScale : float3);
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ComputeNewTwoHandedScale$BurstManaged(in Vector3 startScale, in Vector3 currentScale, in Vector3 startHandleBar, in Vector3 newHandleBar, bool clampScale, float scaleMultiplier, float thresholdMoveRatioForScale, in Vector3 minScale, in Vector3 maxScale, out Vector3 newScale)
		{
			newScale = currentScale;
			float num = math.length(newHandleBar) / math.length(startHandleBar);
			if (num <= 1f)
			{
				if (num < 1f)
				{
					float num2 = (1f / num - 1f) * scaleMultiplier - thresholdMoveRatioForScale;
					if (num2 < 0f)
					{
						return;
					}
					float num3 = 1f + num2;
					Vector3 vector = 1f / num3 * startScale;
					bool flag = math.abs(vector.x) < math.abs(minScale.x) || math.abs(vector.y) < math.abs(minScale.y) || math.abs(vector.z) < math.abs(minScale.z);
					newScale = ((flag && clampScale) ? minScale : vector);
				}
				return;
			}
			float num4 = (num - 1f) * scaleMultiplier - thresholdMoveRatioForScale;
			if (num4 < 0f)
			{
				return;
			}
			Vector3 vector2 = (1f + num4) * startScale;
			bool flag2 = math.abs(vector2.x) > math.abs(maxScale.x) || math.abs(vector2.y) > math.abs(maxScale.y) || math.abs(vector2.z) > math.abs(maxScale.z);
			newScale = ((flag2 && clampScale) ? maxScale : vector2);
		}

		[Header("Translation Constraints")]
		[SerializeField]
		[Tooltip("Permitted axes for translation displacement relative to the object's initial rotation.")]
		private XRGeneralGrabTransformer.ManipulationAxes m_PermittedDisplacementAxes = XRGeneralGrabTransformer.ManipulationAxes.All;

		[SerializeField]
		[Tooltip("Determines how the constrained axis displacement mode is computed.")]
		private XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode m_ConstrainedAxisDisplacementMode = XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode.ObjectRelativeWithLockedWorldUp;

		[Header("Rotation Constraints")]
		[SerializeField]
		[Tooltip("Determines how rotation is calculated when using two hands for the grab interaction.")]
		private XRGeneralGrabTransformer.TwoHandedRotationMode m_TwoHandedRotationMode = XRGeneralGrabTransformer.TwoHandedRotationMode.FirstHandDirectedTowardsSecondHand;

		[Header("Scaling Constraints")]
		[SerializeField]
		[Tooltip("Allow one handed scaling using the scale value provider if available.")]
		private bool m_AllowOneHandedScaling = true;

		[SerializeField]
		[Tooltip("Allow scaling when using multi-grab interaction.")]
		private bool m_AllowTwoHandedScaling;

		[SerializeField]
		[Tooltip("Scaling speed over time for one handed scaling based on the scale value provider.")]
		[Range(0f, 32f)]
		private float m_OneHandedScaleSpeed = 0.5f;

		[SerializeField]
		[Tooltip("(Two Handed Scaling) Percentage as a measure of 0 to 1 of scaled relative hand displacement required to trigger scale operation.\nIf this value is 0f, scaling happens the moment both grab interactors move closer or further away from each other.\nOtherwise, this percentage is used as a threshold before any scaling happens.")]
		[Range(0f, 1f)]
		private float m_ThresholdMoveRatioForScale = 0.05f;

		[Space]
		[SerializeField]
		[Tooltip("If enabled, scaling will abide by ratio ranges defined below.")]
		private bool m_ClampScaling = true;

		[SerializeField]
		[Tooltip("Minimum scale multiplier applied to the initial scale captured on start.")]
		[Range(0.01f, 1f)]
		private float m_MinimumScaleRatio = 0.25f;

		[SerializeField]
		[Tooltip("Maximum scale multiplier applied to the initial scale captured on start.")]
		[Range(1f, 10f)]
		private float m_MaximumScaleRatio = 2f;

		[Space]
		[SerializeField]
		[Range(0.1f, 5f)]
		[Tooltip("Scales the distance of displacement between interactors needed to modify the scale interactable.")]
		private float m_ScaleMultiplier = 0.25f;

		private Pose m_OriginalObjectPose;

		private Pose m_OffsetPose;

		private Pose m_OriginalInteractorPose;

		private Vector3 m_InteractorLocalGrabPoint;

		private Vector3 m_ObjectLocalGrabPoint;

		private IXRInteractor m_OriginalInteractor;

		private int m_LastGrabCount;

		private Vector3 m_StartHandleBar;

		private Vector3 m_StartHandleBarNormalized;

		private Vector3 m_StartHandleBarUp;

		private Quaternion m_StartHandleBarLookRotation;

		private Quaternion m_InverseStartHandleBarLookRotation;

		private Quaternion m_LastHandleBarLocalRotation;

		private Vector3 m_ScaleAtGrabStart;

		private bool m_FirstFrameSinceTwoHandedGrab;

		private Vector3 m_LastTwoHandedUp;

		private Vector3 m_InitialScale;

		private Vector3 m_InitialScaleProportions;

		private Vector3 m_MinimumScale;

		private Vector3 m_MaximumScale;

		private XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode m_ConstrainedAxisDisplacementModeOnGrab;

		private XRGeneralGrabTransformer.ManipulationAxes m_PermittedDisplacementAxesOnGrab;

		private IXRScaleValueProvider m_ScaleValueProvider;

		private bool m_HasScaleValueProvider;

		[Flags]
		public enum ManipulationAxes
		{
			X = 1,
			Y = 2,
			Z = 4,
			All = 7
		}

		public enum ConstrainedAxisDisplacementMode
		{
			ObjectRelative,
			ObjectRelativeWithLockedWorldUp,
			WorldAxisRelative
		}

		public enum TwoHandedRotationMode
		{
			FirstHandOnly,
			FirstHandDirectedTowardsSecondHand,
			TwoHandedAverage
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ComputeNewObjectPosition_00000905$PostfixBurstDelegate(in float3 interactorPosition, in quaternion interactorRotation, in quaternion objectRotation, in float3 objectScale, bool trackRotation, in float3 offsetPosition, in float3 objectLocalGrabPoint, in float3 interactorLocalGrabPoint, out Vector3 newPosition);

		internal static class ComputeNewObjectPosition_00000905$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$BurstDirectCall.Pointer == 0)
				{
					XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$PostfixBurstDelegate>(new XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$PostfixBurstDelegate(XRGeneralGrabTransformer.ComputeNewObjectPosition)).Value;
				}
				A_0 = XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 interactorPosition, in quaternion interactorRotation, in quaternion objectRotation, in float3 objectScale, bool trackRotation, in float3 offsetPosition, in float3 objectLocalGrabPoint, in float3 interactorLocalGrabPoint, out Vector3 newPosition)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRGeneralGrabTransformer.ComputeNewObjectPosition_00000905$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.quaternion&,Unity.Mathematics.quaternion&,Unity.Mathematics.float3&,System.Boolean,Unity.Mathematics.float3&,Unity.Mathematics.float3&,Unity.Mathematics.float3&,UnityEngine.Vector3&), ref interactorPosition, ref interactorRotation, ref objectRotation, ref objectScale, trackRotation, ref offsetPosition, ref objectLocalGrabPoint, ref interactorLocalGrabPoint, ref newPosition, functionPointer);
						return;
					}
				}
				XRGeneralGrabTransformer.ComputeNewObjectPosition$BurstManaged(interactorPosition, interactorRotation, objectRotation, objectScale, trackRotation, offsetPosition, objectLocalGrabPoint, interactorLocalGrabPoint, out newPosition);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void AdjustPositionForPermittedAxesBurst_00000909$PostfixBurstDelegate(in Vector3 targetPosition, in Pose originalObjectPose, XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode axisDisplacementMode, bool hasX, bool hasY, bool hasZ, out Vector3 adjustedTargetPosition);

		internal static class AdjustPositionForPermittedAxesBurst_00000909$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$BurstDirectCall.Pointer == 0)
				{
					XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$PostfixBurstDelegate>(new XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$PostfixBurstDelegate(XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst)).Value;
				}
				A_0 = XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 targetPosition, in Pose originalObjectPose, XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode axisDisplacementMode, bool hasX, bool hasY, bool hasZ, out Vector3 adjustedTargetPosition)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst_00000909$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Pose&,UnityEngine.XR.Interaction.Toolkit.Transformers.XRGeneralGrabTransformer/ConstrainedAxisDisplacementMode,System.Boolean,System.Boolean,System.Boolean,UnityEngine.Vector3&), ref targetPosition, ref originalObjectPose, axisDisplacementMode, hasX, hasY, hasZ, ref adjustedTargetPosition, functionPointer);
						return;
					}
				}
				XRGeneralGrabTransformer.AdjustPositionForPermittedAxesBurst$BurstManaged(targetPosition, originalObjectPose, axisDisplacementMode, hasX, hasY, hasZ, out adjustedTargetPosition);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ComputeNewOneHandedScale_0000090B$PostfixBurstDelegate(in Vector3 currentScale, in Vector3 initialScaleProportions, bool clampScale, in Vector3 minScale, in Vector3 maxScale, float scaleInput, float deltaTime, float scaleSpeed, out Vector3 newScale);

		internal static class ComputeNewOneHandedScale_0000090B$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$BurstDirectCall.Pointer == 0)
				{
					XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$PostfixBurstDelegate>(new XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$PostfixBurstDelegate(XRGeneralGrabTransformer.ComputeNewOneHandedScale)).Value;
				}
				A_0 = XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 currentScale, in Vector3 initialScaleProportions, bool clampScale, in Vector3 minScale, in Vector3 maxScale, float scaleInput, float deltaTime, float scaleSpeed, out Vector3 newScale)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRGeneralGrabTransformer.ComputeNewOneHandedScale_0000090B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,System.Boolean,UnityEngine.Vector3&,UnityEngine.Vector3&,System.Single,System.Single,System.Single,UnityEngine.Vector3&), ref currentScale, ref initialScaleProportions, clampScale, ref minScale, ref maxScale, scaleInput, deltaTime, scaleSpeed, ref newScale, functionPointer);
						return;
					}
				}
				XRGeneralGrabTransformer.ComputeNewOneHandedScale$BurstManaged(currentScale, initialScaleProportions, clampScale, minScale, maxScale, scaleInput, deltaTime, scaleSpeed, out newScale);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ComputeNewTwoHandedScale_0000090C$PostfixBurstDelegate(in Vector3 startScale, in Vector3 currentScale, in Vector3 startHandleBar, in Vector3 newHandleBar, bool clampScale, float scaleMultiplier, float thresholdMoveRatioForScale, in Vector3 minScale, in Vector3 maxScale, out Vector3 newScale);

		internal static class ComputeNewTwoHandedScale_0000090C$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$BurstDirectCall.Pointer == 0)
				{
					XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$PostfixBurstDelegate>(new XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$PostfixBurstDelegate(XRGeneralGrabTransformer.ComputeNewTwoHandedScale)).Value;
				}
				A_0 = XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in Vector3 startScale, in Vector3 currentScale, in Vector3 startHandleBar, in Vector3 newHandleBar, bool clampScale, float scaleMultiplier, float thresholdMoveRatioForScale, in Vector3 minScale, in Vector3 maxScale, out Vector3 newScale)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRGeneralGrabTransformer.ComputeNewTwoHandedScale_0000090C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&,System.Boolean,System.Single,System.Single,UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Vector3&), ref startScale, ref currentScale, ref startHandleBar, ref newHandleBar, clampScale, scaleMultiplier, thresholdMoveRatioForScale, ref minScale, ref maxScale, ref newScale, functionPointer);
						return;
					}
				}
				XRGeneralGrabTransformer.ComputeNewTwoHandedScale$BurstManaged(startScale, currentScale, startHandleBar, newHandleBar, clampScale, scaleMultiplier, thresholdMoveRatioForScale, minScale, maxScale, out newScale);
			}

			private static IntPtr Pointer;
		}
	}
}
