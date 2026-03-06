using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
	[BurstCompile]
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/Interactors/Interaction Attach Controller", 22)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Attachment.InteractionAttachController.html")]
	public class InteractionAttachController : MonoBehaviour, IInteractionAttachController
	{
		public Transform transformToFollow
		{
			get
			{
				return this.m_TransformToFollow;
			}
			set
			{
				this.m_TransformToFollow = value;
			}
		}

		public MotionStabilizationMode motionStabilizationMode
		{
			get
			{
				return this.m_MotionStabilizationMode;
			}
			set
			{
				this.m_MotionStabilizationMode = value;
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

		public bool smoothOffset
		{
			get
			{
				return this.m_SmoothOffset;
			}
			set
			{
				this.m_SmoothOffset = value;
			}
		}

		public float smoothingSpeed
		{
			get
			{
				return this.m_SmoothingSpeed;
			}
			set
			{
				this.m_SmoothingSpeed = Mathf.Clamp(value, 1f, 30f);
			}
		}

		public bool useDistanceBasedVelocityScaling
		{
			get
			{
				return this.m_UseDistanceBasedVelocityScaling;
			}
			set
			{
				this.m_UseDistanceBasedVelocityScaling = value;
			}
		}

		public bool useMomentum
		{
			get
			{
				return this.m_UseMomentum;
			}
			set
			{
				this.m_UseMomentum = value;
			}
		}

		public float momentumDecayScale
		{
			get
			{
				return this.m_MomentumDecayScale;
			}
			set
			{
				this.m_MomentumDecayScale = Mathf.Clamp(value, 0f, 10f);
			}
		}

		public float momentumDecayScaleFromInput
		{
			get
			{
				return this.m_MomentumDecayScaleFromInput;
			}
			set
			{
				this.m_MomentumDecayScaleFromInput = Mathf.Clamp(value, 0f, 10f);
			}
		}

		public float zVelocityRampThreshold
		{
			get
			{
				return this.m_ZVelocityRampThreshold;
			}
			set
			{
				this.m_ZVelocityRampThreshold = Mathf.Clamp(value, 0f, 5f);
			}
		}

		public float pullVelocityBias
		{
			get
			{
				return this.m_PullVelocityBias;
			}
			set
			{
				this.m_PullVelocityBias = Mathf.Clamp(value, 0f, 2f);
			}
		}

		public float pushVelocityBias
		{
			get
			{
				return this.m_PushVelocityBias;
			}
			set
			{
				this.m_PushVelocityBias = Mathf.Clamp(value, 0f, 2f);
			}
		}

		public float minAdditionalVelocityScalar
		{
			get
			{
				return this.m_MinAdditionalVelocityScalar;
			}
			set
			{
				this.m_MinAdditionalVelocityScalar = Mathf.Clamp(value, 0f, 2f);
			}
		}

		public float maxAdditionalVelocityScalar
		{
			get
			{
				return this.m_MaxAdditionalVelocityScalar;
			}
			set
			{
				this.m_MaxAdditionalVelocityScalar = Mathf.Clamp(value, 0f, 5f);
			}
		}

		public bool useManipulationInput
		{
			get
			{
				return this.m_UseManipulationInput;
			}
			set
			{
				this.m_UseManipulationInput = value;
			}
		}

		public XRInputValueReader<Vector2> manipulationInput
		{
			get
			{
				return this.m_ManipulationInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_ManipulationInput, value, this);
			}
		}

		public InteractionAttachController.ManipulationXAxisMode manipulationXAxisMode
		{
			get
			{
				return this.m_ManipulationXAxisMode;
			}
			set
			{
				this.m_ManipulationXAxisMode = value;
			}
		}

		public InteractionAttachController.ManipulationYAxisMode manipulationYAxisMode
		{
			get
			{
				return this.m_ManipulationYAxisMode;
			}
			set
			{
				this.m_ManipulationYAxisMode = value;
			}
		}

		public bool combineManipulationAxes
		{
			get
			{
				return this.m_CombineManipulationAxes;
			}
			set
			{
				this.m_CombineManipulationAxes = value;
			}
		}

		public float manipulationTranslateSpeed
		{
			get
			{
				return this.m_ManipulationTranslateSpeed;
			}
			set
			{
				this.m_ManipulationTranslateSpeed = value;
			}
		}

		public float manipulationRotateSpeed
		{
			get
			{
				return this.m_ManipulationRotateSpeed;
			}
			set
			{
				this.m_ManipulationRotateSpeed = value;
			}
		}

		public Transform manipulationRotateReferenceFrame
		{
			get
			{
				return this.m_ManipulationRotateReferenceFrame;
			}
			set
			{
				this.m_ManipulationRotateReferenceFrame = value;
			}
		}

		public bool enableDebugLines
		{
			get
			{
				return this.m_EnableDebugLines;
			}
			set
			{
				this.m_EnableDebugLines = value;
			}
		}

		public bool hasOffset
		{
			get
			{
				return this.m_HasOffset;
			}
		}

		public event Action attachUpdated;

		private Transform GetXROriginTransform()
		{
			if (!this.InitializeXROrigin())
			{
				return null;
			}
			return this.m_XROrigin.Origin.transform;
		}

		private bool InitializeXROrigin()
		{
			if (this.m_XROrigin == null)
			{
				ComponentLocatorUtility<XROrigin>.TryFindComponent(out this.m_XROrigin);
			}
			this.m_HasXROrigin = (this.m_XROrigin != null);
			return this.m_HasXROrigin;
		}

		protected virtual void OnValidate()
		{
			float minAdditionalVelocityScalar = Mathf.Min(this.m_MinAdditionalVelocityScalar, this.m_MaxAdditionalVelocityScalar);
			float maxAdditionalVelocityScalar = Mathf.Max(this.m_MinAdditionalVelocityScalar, this.m_MaxAdditionalVelocityScalar);
			this.m_MinAdditionalVelocityScalar = minAdditionalVelocityScalar;
			this.m_MaxAdditionalVelocityScalar = maxAdditionalVelocityScalar;
			if (this.m_TransformToFollow == null)
			{
				this.m_TransformToFollow = base.transform;
			}
		}

		protected virtual void Awake()
		{
			if (this.m_TransformToFollow == null)
			{
				this.m_TransformToFollow = base.transform;
			}
		}

		protected virtual void OnEnable()
		{
			if (!this.InitializeXROrigin() && this.m_UseDistanceBasedVelocityScaling)
			{
				Debug.LogWarning(string.Format("Missing XR Origin. Disabling distance-based velocity scaling on this {0}.", this), this);
				this.m_UseDistanceBasedVelocityScaling = false;
			}
			this.m_HasSelectInteractor = base.TryGetComponent<IXRSelectInteractor>(out this.m_SelectInteractor);
			if (this.m_AnchorParent != null)
			{
				this.m_AnchorParent.gameObject.SetActive(true);
			}
			this.m_ManipulationInput.EnableDirectActionIfModeUsed();
		}

		protected virtual void OnDisable()
		{
			if (this.m_AnchorParent != null)
			{
				this.m_AnchorParent.gameObject.SetActive(false);
			}
			this.m_ManipulationInput.DisableDirectActionIfModeUsed();
		}

		private void SyncAnchorParent()
		{
			if (this.m_TransformToFollow == null)
			{
				this.m_TransformToFollow = base.transform;
			}
			this.m_AnchorParent.SetWorldPose(this.m_TransformToFollow.GetWorldPose());
		}

		Transform IInteractionAttachController.GetOrCreateAnchorTransform(bool updateTransform)
		{
			if (this.m_AnchorParent == null)
			{
				Transform xroriginTransform = this.GetXROriginTransform();
				string name = base.GetType().Name;
				string text = "";
				IXRInteractor ixrinteractor;
				if (base.TryGetComponent<IXRInteractor>(out ixrinteractor))
				{
					text = ixrinteractor.handedness.ToString();
				}
				this.m_AnchorParent = new GameObject(string.Concat(new string[]
				{
					"[",
					text,
					" ",
					name,
					"] Attach"
				})).transform;
				this.m_AnchorParent.SetParent(xroriginTransform, false);
				this.m_AnchorParent.SetLocalPose(Pose.identity);
				if (this.m_AnchorChild == null)
				{
					this.m_AnchorChild = new GameObject(string.Concat(new string[]
					{
						"[",
						text,
						" ",
						name,
						"] Attach Child"
					})).transform;
					this.m_AnchorChild.SetParent(this.m_AnchorParent, false);
					this.m_AnchorChild.SetLocalPose(Pose.identity);
				}
			}
			if (updateTransform)
			{
				this.SyncAnchorParent();
			}
			return this.m_AnchorChild;
		}

		void IInteractionAttachController.MoveTo(Vector3 targetWorldPosition)
		{
			this.SyncAnchorParent();
			this.MoveToPosition(targetWorldPosition);
		}

		private void SyncOffset()
		{
			this.MoveToPosition(this.m_AnchorChild.position);
		}

		private void MoveToPosition(Vector3 targetWorldPosition)
		{
			this.m_AnchorChild.position = targetWorldPosition;
			Vector3 direction = targetWorldPosition - this.m_AnchorParent.position;
			this.m_StartLocalOffset = this.m_AnchorParent.InverseTransformDirection(direction);
			this.m_StartLocalOffsetLength = this.m_StartLocalOffset.magnitude;
			this.m_StartLocalOffsetNormalized = ((this.m_StartLocalOffsetLength > 1E-05f) ? (this.m_StartLocalOffset / this.m_StartLocalOffsetLength) : Vector3.zero);
			this.m_TargetLocalOffsetNormalized = this.m_StartLocalOffsetNormalized;
			this.m_LastTargetLocalPosition = this.m_AnchorChild.localPosition;
			if (this.m_HasXROrigin)
			{
				this.m_LastTargetOriginSpacePosition = this.m_XROrigin.Origin.transform.InverseTransformPoint(this.m_AnchorChild.position);
			}
			this.m_Pivot = this.m_StartLocalOffsetLength;
			this.m_HasOffset = (this.m_StartLocalOffsetLength > 0f);
			this.m_Momentum = 0f;
			this.m_FirstMovementFrame = true;
			this.m_WasVelocityScalingBlocked = false;
			this.m_VelocityTracker.ResetVelocityTracking();
		}

		void IInteractionAttachController.ApplyLocalPositionOffset(Vector3 offset)
		{
			this.SyncAnchorParent();
			this.MoveToPosition(this.m_AnchorChild.position + this.m_AnchorParent.TransformDirection(offset));
		}

		void IInteractionAttachController.ApplyLocalRotationOffset(Quaternion localRotation)
		{
			this.m_AnchorChild.localRotation *= localRotation;
		}

		public void ResetOffset()
		{
			this.m_FirstMovementFrame = true;
			this.m_HasOffset = false;
			this.m_WasVelocityScalingBlocked = false;
			this.m_Momentum = 0f;
			this.m_AnchorChild.SetLocalPose(Pose.identity);
			this.SyncAnchorParent();
		}

		void IInteractionAttachController.DoUpdate(float deltaTime)
		{
			if (!this.m_HasXROrigin)
			{
				return;
			}
			Transform transform = this.m_XROrigin.Origin.transform;
			Vector3 up = transform.up;
			if (this.m_MotionStabilizationMode == MotionStabilizationMode.Never || (this.m_MotionStabilizationMode == MotionStabilizationMode.WithPositionOffset && !this.m_HasOffset))
			{
				this.SyncAnchorParent();
			}
			else if (!this.m_HasOffset)
			{
				XRTransformStabilizer.ApplyStabilization(ref this.m_AnchorParent, this.m_TransformToFollow, this.m_PositionStabilization, this.m_AngleStabilization, deltaTime, false);
			}
			else
			{
				float z = this.m_AnchorChild.localPosition.z;
				float num = 1f + z;
				float positionStabilization = num * this.m_PositionStabilization;
				float angleStabilization = num * this.m_AngleStabilization;
				Vector3 position = this.m_AnchorParent.position;
				Vector3 vector = this.m_AnchorChild.position - position;
				Vector3 b = (Vector3.Angle(vector.normalized, up) > 45f) ? Vector3.ProjectOnPlane(vector, up) : vector;
				Vector3 v = position + b;
				float3 @float = v;
				XRTransformStabilizer.ApplyStabilization(ref this.m_AnchorParent, this.m_TransformToFollow, @float, positionStabilization, angleStabilization, deltaTime, false);
			}
			if (!this.m_HasOffset)
			{
				Action action = this.attachUpdated;
				if (action == null)
				{
					return;
				}
				action();
				return;
			}
			else
			{
				if (this.m_UseDistanceBasedVelocityScaling)
				{
					this.m_VelocityTracker.UpdateAttachPointVelocityData(this.m_TransformToFollow, transform);
				}
				if ((this.m_UseDistanceBasedVelocityScaling || this.m_UseManipulationInput) && !this.UpdateVelocityScalingBlock())
				{
					this.DoPositionUpdate(deltaTime);
				}
				else
				{
					this.UpdatePosition(this.m_StartLocalOffset, deltaTime);
				}
				bool flag = this.m_ManipulationYAxisMode == InteractionAttachController.ManipulationYAxisMode.VerticalRotation;
				bool flag2 = this.m_ManipulationXAxisMode == InteractionAttachController.ManipulationXAxisMode.HorizontalRotation;
				Vector2 vector2;
				if (this.m_UseManipulationInput && (flag || flag2) && this.m_ManipulationInput.TryReadValue(out vector2))
				{
					vector2 = this.FilterManipulationInput(vector2);
					float num2 = this.m_ManipulationRotateSpeed * deltaTime;
					float x = flag ? (vector2.y * num2) : 0f;
					float y = flag2 ? (vector2.x * num2) : 0f;
					Quaternion quaternion = (this.m_ManipulationRotateReferenceFrame != null) ? this.m_ManipulationRotateReferenceFrame.rotation : this.m_AnchorParent.rotation;
					this.m_AnchorChild.rotation = quaternion * Quaternion.Euler(x, y, 0f) * Quaternion.Inverse(quaternion) * this.m_AnchorChild.rotation;
				}
				Action action2 = this.attachUpdated;
				if (action2 == null)
				{
					return;
				}
				action2();
				return;
			}
		}

		private void DoPositionUpdate(float deltaTime)
		{
			float3 @float = this.m_SmoothOffset ? this.m_LastTargetLocalPosition : this.m_AnchorChild.localPosition;
			Vector3 direction = Vector3.zero;
			float3 lhs;
			if (this.m_FirstMovementFrame)
			{
				lhs = float3.zero;
				this.m_FirstMovementFrame = false;
			}
			else if (this.m_UseDistanceBasedVelocityScaling)
			{
				Transform transform = this.m_XROrigin.Origin.transform;
				direction = this.m_VelocityTracker.GetAttachPointVelocity(transform);
				lhs = this.m_AnchorParent.InverseTransformDirection(direction);
			}
			else
			{
				lhs = float3.zero;
			}
			bool applyMomentum = this.m_UseMomentum;
			Vector2 vector;
			if (this.m_UseManipulationInput && this.m_ManipulationYAxisMode == InteractionAttachController.ManipulationYAxisMode.Translate && this.m_ManipulationInput.TryReadValue(out vector))
			{
				vector = this.FilterManipulationInput(vector);
				float rhs = vector.y * this.m_ManipulationTranslateSpeed;
				lhs += new float3(this.m_TargetLocalOffsetNormalized.x, this.m_TargetLocalOffsetNormalized.y, this.m_TargetLocalOffsetNormalized.z) * rhs;
				applyMomentum = false;
				this.m_MomentumDecayFromInput = true;
			}
			float momentumDecayScale = this.m_MomentumDecayFromInput ? this.m_MomentumDecayScaleFromInput : this.m_MomentumDecayScale;
			float3 float2 = this.m_StartLocalOffsetNormalized;
			float startLocalOffsetLength = this.m_StartLocalOffsetLength;
			float3 float3 = this.m_TargetLocalOffsetNormalized;
			float3 float4;
			InteractionAttachController.ComputeAmplifiedOffset(lhs, float2, startLocalOffsetLength, float3, @float, this.m_MinAdditionalVelocityScalar, this.m_MaxAdditionalVelocityScalar, this.m_PushVelocityBias, this.m_PullVelocityBias, this.m_ZVelocityRampThreshold, this.m_UseMomentum, applyMomentum, momentumDecayScale, ref this.m_Momentum, ref this.m_Pivot, deltaTime, out float4);
			if (math.abs(this.m_Momentum) < 0.001f)
			{
				this.m_MomentumDecayFromInput = false;
			}
			if (math.dot(math.normalize(float4), this.m_StartLocalOffsetNormalized) < 0.05f)
			{
				this.ResetOffset();
				return;
			}
			this.UpdatePosition(float4, deltaTime);
		}

		private bool UpdateVelocityScalingBlock()
		{
			if (!this.m_HasSelectInteractor)
			{
				return false;
			}
			bool flag = false;
			if (this.m_SelectInteractor.hasSelection)
			{
				IXRSelectInteractable ixrselectInteractable = this.m_SelectInteractor.interactablesSelected[0];
				if (ixrselectInteractable != null && ixrselectInteractable.interactorsSelecting.Count > 1)
				{
					flag = true;
				}
			}
			if (flag && !this.m_WasVelocityScalingBlocked)
			{
				this.SyncOffset();
			}
			this.m_WasVelocityScalingBlocked = flag;
			return flag;
		}

		private void UpdatePosition(Vector3 targetLocalPosition, float deltaTime)
		{
			if (!this.m_SmoothOffset || !this.m_HasXROrigin)
			{
				this.m_AnchorChild.localPosition = targetLocalPosition;
				this.m_LastTargetLocalPosition = targetLocalPosition;
				return;
			}
			Transform transform = this.m_XROrigin.Origin.transform;
			Vector3 vector = transform.TransformPoint(this.m_LastTargetOriginSpacePosition);
			Vector3 vector2 = this.m_AnchorParent.TransformPoint(targetLocalPosition);
			Vector3 position = BurstLerpUtility.BezierLerp(vector, vector2, this.m_SmoothingSpeed * deltaTime, 0.5f);
			this.m_AnchorChild.position = position;
			this.m_LastTargetOriginSpacePosition = transform.InverseTransformPoint(position);
			this.m_LastTargetLocalPosition = targetLocalPosition;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(InteractionAttachController.ComputeAmplifiedOffset_00001157$PostfixBurstDelegate))]
		private static void ComputeAmplifiedOffset(in float3 velocityLocal, in float3 startLocalOffsetNormalized, float startLocalOffsetLength, in float3 targetLocalOffsetNormalized, in float3 currentLocalOffset, float minAdditionalVelocityScalar, float maxAdditionalVelocityScalar, float pushVelocityBias, float pullVelocityBias, float zVelocityRampThreshold, bool calculateMomentum, bool applyMomentum, float momentumDecayScale, ref float momentum, ref float pivot, float deltaTime, out float3 newOffset)
		{
			InteractionAttachController.ComputeAmplifiedOffset_00001157$BurstDirectCall.Invoke(velocityLocal, startLocalOffsetNormalized, startLocalOffsetLength, targetLocalOffsetNormalized, currentLocalOffset, minAdditionalVelocityScalar, maxAdditionalVelocityScalar, pushVelocityBias, pullVelocityBias, zVelocityRampThreshold, calculateMomentum, applyMomentum, momentumDecayScale, ref momentum, ref pivot, deltaTime, out newOffset);
		}

		private Vector2 FilterManipulationInput(in Vector2 input)
		{
			if (this.m_CombineManipulationAxes || this.m_ManipulationXAxisMode == InteractionAttachController.ManipulationXAxisMode.None || this.m_ManipulationYAxisMode == InteractionAttachController.ManipulationYAxisMode.None)
			{
				return input;
			}
			if (Mathf.Abs(input.y) < Mathf.Abs(input.x))
			{
				return new Vector2(input.x, 0f);
			}
			return new Vector2(0f, input.y);
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ComputeAmplifiedOffset$BurstManaged(in float3 velocityLocal, in float3 startLocalOffsetNormalized, float startLocalOffsetLength, in float3 targetLocalOffsetNormalized, in float3 currentLocalOffset, float minAdditionalVelocityScalar, float maxAdditionalVelocityScalar, float pushVelocityBias, float pullVelocityBias, float zVelocityRampThreshold, bool calculateMomentum, bool applyMomentum, float momentumDecayScale, ref float momentum, ref float pivot, float deltaTime, out float3 newOffset)
		{
			float3 x;
			if (math.abs(math.dot(math.normalize(velocityLocal), targetLocalOffsetNormalized)) > 0.866f)
			{
				x = math.project(velocityLocal, targetLocalOffsetNormalized);
			}
			else
			{
				x = float3.zero;
			}
			float start = minAdditionalVelocityScalar * pivot;
			float end = maxAdditionalVelocityScalar * pivot;
			float num = math.length(currentLocalOffset);
			float num2 = math.sign(math.dot(math.normalize(x), startLocalOffsetNormalized));
			bool flag = num2 > 0f;
			float num3 = math.length(x) * num2;
			float t = math.clamp(math.abs(num) / pivot * (flag ? pushVelocityBias : pullVelocityBias), 0f, 1f);
			float num4 = BurstLerpUtility.BezierLerp(start, end, t, 0.5f);
			float num5 = (zVelocityRampThreshold > 0f) ? math.clamp(math.abs(num3) / zVelocityRampThreshold, 0f, 1f) : 1f;
			float num6 = num3 * num5 * (1f + num4) * deltaTime;
			if (calculateMomentum)
			{
				float num7 = math.abs(momentum);
				float num8 = math.abs(num6);
				bool flag2 = num5 >= 1f;
				if ((int)math.sign(momentum) != (int)math.sign(num6) && math.abs(num7 - num8) > 0.001f)
				{
					if (flag2)
					{
						momentum = num6 * 0.5f;
					}
					else if (num5 > 0.25f)
					{
						momentum = 0f;
					}
				}
				else if (flag2)
				{
					momentum = math.max(num7, num8 / 2f) * math.sign(num6);
				}
				if (math.abs(momentum) < 0.001f)
				{
					momentum = 0f;
				}
				else
				{
					momentum *= 1f - momentumDecayScale * deltaTime;
				}
			}
			else
			{
				momentum = 0f;
			}
			float num9 = num + num6;
			if (applyMomentum)
			{
				num9 += momentum;
			}
			newOffset = startLocalOffsetNormalized * num9;
			if (num9 > startLocalOffsetLength)
			{
				pivot = num9;
				return;
			}
			pivot = math.lerp(pivot, (startLocalOffsetLength + num9) / 2f, deltaTime * num4);
		}

		[SerializeField]
		private Transform m_TransformToFollow;

		[SerializeField]
		private MotionStabilizationMode m_MotionStabilizationMode = MotionStabilizationMode.WithPositionOffset;

		[SerializeField]
		private float m_PositionStabilization = 0.25f;

		[SerializeField]
		private float m_AngleStabilization = 20f;

		[SerializeField]
		private bool m_SmoothOffset;

		[SerializeField]
		[Range(1f, 30f)]
		private float m_SmoothingSpeed = 10f;

		[SerializeField]
		private bool m_UseDistanceBasedVelocityScaling = true;

		[SerializeField]
		private bool m_UseMomentum = true;

		[SerializeField]
		[Range(0f, 10f)]
		private float m_MomentumDecayScale = 1.25f;

		[SerializeField]
		[Range(0f, 10f)]
		private float m_MomentumDecayScaleFromInput = 5.5f;

		[SerializeField]
		[Range(0f, 5f)]
		private float m_ZVelocityRampThreshold = 0.3f;

		[SerializeField]
		[Range(0f, 2f)]
		private float m_PullVelocityBias = 1f;

		[SerializeField]
		[Range(0f, 2f)]
		private float m_PushVelocityBias = 1.25f;

		[SerializeField]
		[Range(0f, 2f)]
		private float m_MinAdditionalVelocityScalar = 0.05f;

		[SerializeField]
		[Range(0f, 5f)]
		private float m_MaxAdditionalVelocityScalar = 1.5f;

		[SerializeField]
		private bool m_UseManipulationInput;

		[SerializeField]
		private XRInputValueReader<Vector2> m_ManipulationInput = new XRInputValueReader<Vector2>("Manipulation", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private InteractionAttachController.ManipulationXAxisMode m_ManipulationXAxisMode = InteractionAttachController.ManipulationXAxisMode.HorizontalRotation;

		[SerializeField]
		private InteractionAttachController.ManipulationYAxisMode m_ManipulationYAxisMode = InteractionAttachController.ManipulationYAxisMode.Translate;

		[SerializeField]
		private bool m_CombineManipulationAxes;

		[SerializeField]
		private float m_ManipulationTranslateSpeed = 1f;

		[SerializeField]
		private float m_ManipulationRotateSpeed = 180f;

		[SerializeField]
		private Transform m_ManipulationRotateReferenceFrame;

		[SerializeField]
		private bool m_EnableDebugLines;

		private bool m_FirstMovementFrame;

		private bool m_HasOffset;

		private float m_StartLocalOffsetLength;

		private Vector3 m_StartLocalOffset;

		private Vector3 m_StartLocalOffsetNormalized;

		private Vector3 m_TargetLocalOffsetNormalized;

		private float m_Pivot;

		private float m_Momentum;

		private bool m_MomentumDecayFromInput;

		private bool m_WasVelocityScalingBlocked;

		private bool m_HasSelectInteractor;

		private IXRSelectInteractor m_SelectInteractor;

		private bool m_HasXROrigin;

		private XROrigin m_XROrigin;

		private Transform m_AnchorParent;

		private Transform m_AnchorChild;

		private Vector3 m_LastTargetLocalPosition;

		private Vector3 m_LastTargetOriginSpacePosition;

		private readonly AttachPointVelocityTracker m_VelocityTracker = new AttachPointVelocityTracker();

		public enum ManipulationXAxisMode
		{
			None,
			HorizontalRotation
		}

		public enum ManipulationYAxisMode
		{
			None,
			VerticalRotation,
			Translate
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void ComputeAmplifiedOffset_00001157$PostfixBurstDelegate(in float3 velocityLocal, in float3 startLocalOffsetNormalized, float startLocalOffsetLength, in float3 targetLocalOffsetNormalized, in float3 currentLocalOffset, float minAdditionalVelocityScalar, float maxAdditionalVelocityScalar, float pushVelocityBias, float pullVelocityBias, float zVelocityRampThreshold, bool calculateMomentum, bool applyMomentum, float momentumDecayScale, ref float momentum, ref float pivot, float deltaTime, out float3 newOffset);

		internal static class ComputeAmplifiedOffset_00001157$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (InteractionAttachController.ComputeAmplifiedOffset_00001157$BurstDirectCall.Pointer == 0)
				{
					InteractionAttachController.ComputeAmplifiedOffset_00001157$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<InteractionAttachController.ComputeAmplifiedOffset_00001157$PostfixBurstDelegate>(new InteractionAttachController.ComputeAmplifiedOffset_00001157$PostfixBurstDelegate(InteractionAttachController.ComputeAmplifiedOffset)).Value;
				}
				A_0 = InteractionAttachController.ComputeAmplifiedOffset_00001157$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				InteractionAttachController.ComputeAmplifiedOffset_00001157$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in float3 velocityLocal, in float3 startLocalOffsetNormalized, float startLocalOffsetLength, in float3 targetLocalOffsetNormalized, in float3 currentLocalOffset, float minAdditionalVelocityScalar, float maxAdditionalVelocityScalar, float pushVelocityBias, float pullVelocityBias, float zVelocityRampThreshold, bool calculateMomentum, bool applyMomentum, float momentumDecayScale, ref float momentum, ref float pivot, float deltaTime, out float3 newOffset)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = InteractionAttachController.ComputeAmplifiedOffset_00001157$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,Unity.Mathematics.float3&,Unity.Mathematics.float3&,System.Single,System.Single,System.Single,System.Single,System.Single,System.Boolean,System.Boolean,System.Single,System.Single&,System.Single&,System.Single,Unity.Mathematics.float3&), ref velocityLocal, ref startLocalOffsetNormalized, startLocalOffsetLength, ref targetLocalOffsetNormalized, ref currentLocalOffset, minAdditionalVelocityScalar, maxAdditionalVelocityScalar, pushVelocityBias, pullVelocityBias, zVelocityRampThreshold, calculateMomentum, applyMomentum, momentumDecayScale, ref momentum, ref pivot, deltaTime, ref newOffset, functionPointer);
						return;
					}
				}
				InteractionAttachController.ComputeAmplifiedOffset$BurstManaged(velocityLocal, startLocalOffsetNormalized, startLocalOffsetLength, targetLocalOffsetNormalized, currentLocalOffset, minAdditionalVelocityScalar, maxAdditionalVelocityScalar, pushVelocityBias, pullVelocityBias, zVelocityRampThreshold, calculateMomentum, applyMomentum, momentumDecayScale, ref momentum, ref pivot, deltaTime, out newOffset);
			}

			private static IntPtr Pointer;
		}
	}
}
