using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Position Control/Cinemachine Position Composer")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePositionComposer.html")]
	public class CinemachinePositionComposer : CinemachineComponentBase, CinemachineFreeLookModifier.IModifiablePositionDamping, CinemachineFreeLookModifier.IModifiableDistance, CinemachineFreeLookModifier.IModifiableComposition
	{
		internal ScreenComposerSettings GetEffectiveComposition
		{
			get
			{
				return this.m_PreviousComposition;
			}
		}

		private void Reset()
		{
			this.TargetOffset = Vector3.zero;
			this.Lookahead = default(LookaheadSettings);
			this.Damping = Vector3.one;
			this.CameraDistance = 10f;
			this.Composition = ScreenComposerSettings.Default;
			this.DeadZoneDepth = 0f;
			this.CenterOnActivate = true;
		}

		private void OnValidate()
		{
			this.Damping.x = Mathf.Max(0f, this.Damping.x);
			this.Damping.y = Mathf.Max(0f, this.Damping.y);
			this.Damping.z = Mathf.Max(0f, this.Damping.z);
			this.CameraDistance = Mathf.Max(0.01f, this.CameraDistance);
			this.DeadZoneDepth = Mathf.Max(0f, this.DeadZoneDepth);
			this.Composition.Validate();
		}

		ScreenComposerSettings CinemachineFreeLookModifier.IModifiableComposition.Composition
		{
			get
			{
				return this.Composition;
			}
			set
			{
				this.Composition = value;
			}
		}

		Vector3 CinemachineFreeLookModifier.IModifiablePositionDamping.PositionDamping
		{
			get
			{
				return this.Damping;
			}
			set
			{
				this.Damping = value;
			}
		}

		float CinemachineFreeLookModifier.IModifiableDistance.Distance
		{
			get
			{
				return this.CameraDistance;
			}
			set
			{
				this.CameraDistance = value;
			}
		}

		public override bool IsValid
		{
			get
			{
				return base.enabled && base.FollowTarget != null;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Body;
			}
		}

		public override bool BodyAppliesAfterAim
		{
			get
			{
				return true;
			}
		}

		internal Vector3 TrackedPoint { get; private set; }

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			base.OnTargetObjectWarped(target, positionDelta);
			if (target == base.FollowTarget)
			{
				this.m_PreviousCameraPosition += positionDelta;
				this.m_Predictor.ApplyTransformDelta(positionDelta);
			}
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			base.ForceCameraPosition(pos, rot);
			this.m_Predictor.ApplyRotationDelta(rot * Quaternion.Inverse(this.m_PreviousRotation));
			this.m_PreviousCameraPosition = pos;
			this.m_PreviousRotation = rot;
		}

		public override float GetMaxDampTime()
		{
			return Mathf.Max(this.Damping.x, Mathf.Max(this.Damping.y, this.Damping.z));
		}

		public override bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			if (fromCam != null && (base.VirtualCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && !CinemachineCore.IsLiveInBlend(base.VirtualCamera))
			{
				this.m_PreviousCameraPosition = fromCam.State.RawPosition;
				this.m_PreviousRotation = fromCam.State.RawOrientation;
				this.m_InheritingPosition = true;
				return true;
			}
			return false;
		}

		private Rect ScreenToOrtho(Rect rScreen, float orthoSize, float aspect)
		{
			return new Rect
			{
				yMax = 2f * orthoSize * (1f - rScreen.yMin - 0.5f),
				yMin = 2f * orthoSize * (1f - rScreen.yMax - 0.5f),
				xMin = 2f * orthoSize * aspect * (rScreen.xMin - 0.5f),
				xMax = 2f * orthoSize * aspect * (rScreen.xMax - 0.5f)
			};
		}

		private Vector3 OrthoOffsetToScreenBounds(Vector3 targetPos2D, Rect screenRect)
		{
			Vector3 zero = Vector3.zero;
			if (targetPos2D.x < screenRect.xMin)
			{
				zero.x += targetPos2D.x - screenRect.xMin;
			}
			if (targetPos2D.x > screenRect.xMax)
			{
				zero.x += targetPos2D.x - screenRect.xMax;
			}
			if (targetPos2D.y < screenRect.yMin)
			{
				zero.y += targetPos2D.y - screenRect.yMin;
			}
			if (targetPos2D.y > screenRect.yMax)
			{
				zero.y += targetPos2D.y - screenRect.yMax;
			}
			return zero;
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			LensSettings lens = curState.Lens;
			Vector3 vector = base.FollowTargetPosition + base.FollowTargetRotation * this.TargetOffset;
			bool flag = deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid;
			if (!flag || base.VirtualCamera.FollowTargetChanged)
			{
				this.m_Predictor.Reset();
			}
			if (!flag)
			{
				this.m_PreviousCameraPosition = curState.RawPosition;
				this.m_PreviousRotation = curState.RawOrientation;
				this.m_PreviousDesiredDistance = this.CameraDistance;
				this.m_PreviousComposition = this.Composition;
				if (!this.m_InheritingPosition && this.CenterOnActivate)
				{
					this.m_PreviousCameraPosition = base.FollowTargetPosition + curState.RawOrientation * Vector3.back * this.CameraDistance;
				}
			}
			if (!this.IsValid)
			{
				this.m_InheritingPosition = false;
				return;
			}
			float fieldOfView = lens.FieldOfView;
			this.TrackedPoint = vector;
			if (this.Lookahead.Enabled && this.Lookahead.Time > 0.0001f)
			{
				this.m_Predictor.Smoothing = this.Lookahead.Smoothing;
				this.m_Predictor.AddPosition(vector, deltaTime);
				Vector3 vector2 = this.m_Predictor.PredictPositionDelta(this.Lookahead.Time);
				if (this.Lookahead.IgnoreY)
				{
					vector2 = vector2.ProjectOntoPlane(curState.ReferenceUp);
				}
				this.TrackedPoint = vector + vector2;
			}
			if (!curState.HasLookAt() || curState.ReferenceLookAt == base.FollowTargetPosition)
			{
				curState.ReferenceLookAt = vector;
			}
			Quaternion rawOrientation = curState.RawOrientation;
			if (flag)
			{
				Vector3 b = rawOrientation * Quaternion.Inverse(this.m_PreviousRotation) * (this.m_PreviousCameraPosition - this.TrackedPoint);
				this.m_PreviousCameraPosition = this.TrackedPoint + b;
				float num = this.CameraDistance - this.m_PreviousDesiredDistance;
				if (Mathf.Abs(num) > 0.0001f)
				{
					this.m_PreviousCameraPosition += b.normalized * num;
				}
			}
			this.m_PreviousRotation = rawOrientation;
			Vector3 vector3 = this.m_PreviousCameraPosition;
			Quaternion rotation = Quaternion.Inverse(rawOrientation);
			Vector3 b2 = rotation * vector3;
			Vector3 vector4 = rotation * this.TrackedPoint - b2;
			Vector3 vector5 = vector4;
			Vector3 vector6 = Vector3.zero;
			float num2 = Mathf.Max(0.01f, this.CameraDistance - this.DeadZoneDepth / 2f);
			float num3 = Mathf.Max(num2, this.CameraDistance + this.DeadZoneDepth / 2f);
			float num4 = Mathf.Min(vector4.z, vector5.z);
			if (num4 < num2)
			{
				vector6.z = num4 - num2;
			}
			if (num4 > num3)
			{
				vector6.z = num4 - num3;
			}
			float num5 = lens.Orthographic ? lens.OrthographicSize : (Mathf.Tan(0.5f * fieldOfView * 0.017453292f) * (num4 - vector6.z));
			Rect rect = this.ScreenToOrtho(this.Composition.DeadZoneRect, num5, lens.Aspect);
			if (!flag)
			{
				Rect screenRect = rect;
				if (this.CenterOnActivate && !this.m_InheritingPosition)
				{
					screenRect = new Rect(screenRect.center, Vector2.zero);
				}
				vector6 += this.OrthoOffsetToScreenBounds(vector4, screenRect);
			}
			else
			{
				if (this.Composition.ScreenPosition != this.m_PreviousComposition.ScreenPosition)
				{
					Vector2 vector7 = this.Composition.ScreenPosition - this.m_PreviousComposition.ScreenPosition;
					Vector3 vector8 = new Vector3(-vector7.x * num5 * lens.Aspect * 2f, vector7.y * num5 * 2f, 0f);
					vector4 += vector8;
					vector3 += rawOrientation * vector8;
				}
				vector6 += this.OrthoOffsetToScreenBounds(vector4, rect);
				vector6 = base.VirtualCamera.DetachedFollowTargetDamp(vector6, this.Damping, deltaTime);
				if (this.Composition.HardLimits.Enabled && (deltaTime < 0f || base.VirtualCamera.FollowTargetAttachment > 0.9999f))
				{
					Rect screenRect2 = this.ScreenToOrtho(this.Composition.HardLimitsRect, num5, lens.Aspect);
					Vector3 a = rotation * vector - b2;
					vector6 += this.OrthoOffsetToScreenBounds(a - vector6, screenRect2);
				}
			}
			curState.RawPosition = vector3 + rawOrientation * vector6;
			this.m_PreviousCameraPosition = curState.RawPosition;
			this.m_PreviousComposition = this.Composition;
			this.m_PreviousDesiredDistance = this.CameraDistance;
			this.m_InheritingPosition = false;
		}

		[Header("Camera Position")]
		[Tooltip("The distance along the camera axis that will be maintained from the target")]
		public float CameraDistance = 10f;

		[Tooltip("The camera will not move along its z-axis if the target is within this distance of the specified camera distance")]
		public float DeadZoneDepth;

		[Header("Composition")]
		[HideFoldout]
		public ScreenComposerSettings Composition = ScreenComposerSettings.Default;

		[Tooltip("Force target to center of screen when this camera activates.  If false, will clamp target to the edges of the dead zone")]
		public bool CenterOnActivate = true;

		[Header("Target Tracking")]
		[Tooltip("Offset from the target object (in target-local co-ordinates).  The camera will attempt to frame the point which is the target's position plus this offset.  Use it to correct for cases when the target's origin is not the point of interest for the camera.")]
		[FormerlySerializedAs("TrackedObjectOffset")]
		public Vector3 TargetOffset;

		[Tooltip("How aggressively the camera tries to follow the target in the screen space. Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical and horizontal settings can yield a wide range of camera behaviors.")]
		public Vector3 Damping;

		[FoldoutWithEnabledButton("Enabled")]
		public LookaheadSettings Lookahead;

		private const float kMinimumCameraDistance = 0.01f;

		internal PositionPredictor m_Predictor;

		private Vector3 m_PreviousCameraPosition = Vector3.zero;

		private Quaternion m_PreviousRotation;

		private ScreenComposerSettings m_PreviousComposition;

		private float m_PreviousDesiredDistance;

		private bool m_InheritingPosition;
	}
}
