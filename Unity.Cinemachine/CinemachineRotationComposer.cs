using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Rotation Control/Cinemachine Rotation Composer")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Aim)]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.LookAt)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineRotationComposer.html")]
	public class CinemachineRotationComposer : CinemachineComponentBase, CinemachineFreeLookModifier.IModifiableComposition
	{
		private void Reset()
		{
			this.TargetOffset = Vector3.zero;
			this.Lookahead = default(LookaheadSettings);
			this.Damping = new Vector2(0.5f, 0.5f);
			this.Composition = ScreenComposerSettings.Default;
			this.CenterOnActivate = true;
		}

		private void OnValidate()
		{
			this.Damping.x = Mathf.Max(0f, this.Damping.x);
			this.Damping.y = Mathf.Max(0f, this.Damping.y);
			this.Composition.Validate();
		}

		public override bool IsValid
		{
			get
			{
				return base.enabled && base.LookAtTarget != null;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Aim;
			}
		}

		internal override bool CameraLooksAtTarget
		{
			get
			{
				return true;
			}
		}

		internal Vector3 TrackedPoint { get; private set; }

		internal ScreenComposerSettings GetEffectiveComposition
		{
			get
			{
				return this.m_CompositionLastFrame;
			}
		}

		private Vector3 GetLookAtPointAndSetTrackedPoint(Vector3 lookAt, Vector3 up, float deltaTime)
		{
			Vector3 vector = lookAt;
			if (base.LookAtTarget != null)
			{
				vector += base.LookAtTargetRotation * this.TargetOffset;
			}
			if (!this.Lookahead.Enabled || this.Lookahead.Time < 0.0001f)
			{
				this.TrackedPoint = vector;
			}
			else
			{
				bool flag = base.VirtualCamera.LookAtTargetChanged || !base.VirtualCamera.PreviousStateIsValid;
				this.m_Predictor.Smoothing = this.Lookahead.Smoothing;
				this.m_Predictor.AddPosition(vector, flag ? -1f : deltaTime);
				Vector3 vector2 = this.m_Predictor.PredictPositionDelta(this.Lookahead.Time);
				if (this.Lookahead.IgnoreY)
				{
					vector2 = vector2.ProjectOntoPlane(up);
				}
				this.TrackedPoint = vector + vector2;
			}
			return this.TrackedPoint;
		}

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			base.OnTargetObjectWarped(target, positionDelta);
			if (target == base.LookAtTarget)
			{
				this.m_CameraPosPrevFrame += positionDelta;
				this.m_LookAtPrevFrame += positionDelta;
				this.m_Predictor.ApplyTransformDelta(positionDelta);
			}
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			base.ForceCameraPosition(pos, rot);
			this.m_Predictor.ApplyRotationDelta(rot * Quaternion.Inverse(this.m_CameraOrientationPrevFrame));
			this.m_CameraPosPrevFrame = pos;
			this.m_CameraOrientationPrevFrame = rot;
		}

		public override float GetMaxDampTime()
		{
			return Mathf.Max(this.Damping.x, this.Damping.y);
		}

		public override void PrePipelineMutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (this.IsValid && curState.HasLookAt())
			{
				curState.ReferenceLookAt = this.GetLookAtPointAndSetTrackedPoint(curState.ReferenceLookAt, curState.ReferenceUp, deltaTime);
			}
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (!this.IsValid || !curState.HasLookAt())
			{
				return;
			}
			if (!(this.TrackedPoint - curState.ReferenceLookAt).AlmostZero())
			{
				Vector3 b = Vector3.Lerp(curState.GetCorrectedPosition(), curState.ReferenceLookAt, 0.5f);
				Vector3 lhs = curState.ReferenceLookAt - b;
				Vector3 rhs = this.TrackedPoint - b;
				if (Vector3.Dot(lhs, rhs) < 0f)
				{
					float t = Vector3.Distance(curState.ReferenceLookAt, b) / Vector3.Distance(curState.ReferenceLookAt, this.TrackedPoint);
					this.TrackedPoint = Vector3.Lerp(curState.ReferenceLookAt, this.TrackedPoint, t);
				}
			}
			float magnitude = (this.TrackedPoint - curState.GetCorrectedPosition()).magnitude;
			if (magnitude < 0.0001f)
			{
				if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
				{
					curState.RawOrientation = this.m_CameraOrientationPrevFrame;
				}
				return;
			}
			this.m_Cache.UpdateCache(ref curState.Lens, this.Composition.DeadZoneRect, this.Composition.HardLimitsRect, magnitude);
			Quaternion quaternion = curState.RawOrientation;
			if (deltaTime < 0f || !base.VirtualCamera.PreviousStateIsValid)
			{
				quaternion = Quaternion.LookRotation(quaternion * Vector3.forward, curState.ReferenceUp);
				Rect fovSoftGuideRect = this.m_Cache.FovSoftGuideRect;
				if (this.CenterOnActivate)
				{
					fovSoftGuideRect = new Rect(fovSoftGuideRect.center, Vector2.zero);
				}
				this.RotateToScreenBounds(ref curState, fovSoftGuideRect, curState.ReferenceLookAt, ref quaternion, this.m_Cache.Fov, -1f);
			}
			else
			{
				Vector3 vector = this.m_LookAtPrevFrame - this.m_CameraPosPrevFrame;
				if (vector.AlmostZero())
				{
					quaternion = Quaternion.LookRotation(this.m_CameraOrientationPrevFrame * Vector3.forward, curState.ReferenceUp);
				}
				else
				{
					vector = curState.RotationDampingBypass * vector;
					if (this.Composition.ScreenPosition != this.m_CompositionLastFrame.ScreenPosition)
					{
						Vector3 v = this.m_Cache.DirectionFromScreen(this.m_CompositionLastFrame.ScreenPosition);
						Vector3 v2 = this.m_Cache.DirectionFromScreen(this.Composition.ScreenPosition);
						Quaternion quaternion2 = Quaternion.identity.ApplyCameraRotation(this.m_ScreenOffsetPrevFrame, Vector3.up);
						quaternion2 *= UnityVectorExtensions.SafeFromToRotation(v, v2, Vector3.up);
						this.m_ScreenOffsetPrevFrame = Quaternion.identity.GetCameraRotationToTarget(quaternion2 * Vector3.forward, Vector3.up);
					}
					quaternion = Quaternion.LookRotation(vector, curState.ReferenceUp);
					quaternion = quaternion.ApplyCameraRotation(-this.m_ScreenOffsetPrevFrame, curState.ReferenceUp);
				}
				this.RotateToScreenBounds(ref curState, this.m_Cache.FovSoftGuideRect, this.TrackedPoint, ref quaternion, this.m_Cache.Fov, deltaTime);
				if (this.Composition.HardLimits.Enabled && (deltaTime < 0f || base.VirtualCamera.LookAtTargetAttachment > 0.9999f))
				{
					this.RotateToScreenBounds(ref curState, this.m_Cache.FovHardGuideRect, curState.ReferenceLookAt, ref quaternion, this.m_Cache.Fov, -1f);
				}
			}
			this.m_CameraPosPrevFrame = curState.GetCorrectedPosition();
			this.m_LookAtPrevFrame = this.TrackedPoint;
			this.m_CameraOrientationPrevFrame = quaternion.normalized;
			this.m_ScreenOffsetPrevFrame = this.m_CameraOrientationPrevFrame.GetCameraRotationToTarget(this.m_LookAtPrevFrame - this.m_CameraPosPrevFrame, curState.ReferenceUp);
			this.m_CompositionLastFrame = this.Composition;
			curState.RawOrientation = this.m_CameraOrientationPrevFrame;
		}

		private void RotateToScreenBounds(ref CameraState state, Rect screenRect, Vector3 trackedPoint, ref Quaternion rigOrientation, Vector2 fov, float deltaTime)
		{
			Vector3 vector = trackedPoint - state.GetCorrectedPosition();
			Vector2 cameraRotationToTarget = rigOrientation.GetCameraRotationToTarget(vector, state.ReferenceUp);
			CinemachineRotationComposer.ClampVerticalBounds(ref screenRect, vector, state.ReferenceUp, fov.y);
			float num = (screenRect.yMin - 0.5f) * fov.y;
			float num2 = (screenRect.yMax - 0.5f) * fov.y;
			if (cameraRotationToTarget.x < num)
			{
				cameraRotationToTarget.x -= num;
			}
			else if (cameraRotationToTarget.x > num2)
			{
				cameraRotationToTarget.x -= num2;
			}
			else
			{
				cameraRotationToTarget.x = 0f;
			}
			num = (screenRect.xMin - 0.5f) * fov.x;
			num2 = (screenRect.xMax - 0.5f) * fov.x;
			if (cameraRotationToTarget.y < num)
			{
				cameraRotationToTarget.y -= num;
			}
			else if (cameraRotationToTarget.y > num2)
			{
				cameraRotationToTarget.y -= num2;
			}
			else
			{
				cameraRotationToTarget.y = 0f;
			}
			if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
			{
				cameraRotationToTarget.x = base.VirtualCamera.DetachedLookAtTargetDamp(cameraRotationToTarget.x, this.Damping.y, deltaTime);
				cameraRotationToTarget.y = base.VirtualCamera.DetachedLookAtTargetDamp(cameraRotationToTarget.y, this.Damping.x, deltaTime);
			}
			rigOrientation = rigOrientation.ApplyCameraRotation(cameraRotationToTarget, state.ReferenceUp);
		}

		private static bool ClampVerticalBounds(ref Rect r, Vector3 dir, Vector3 up, float fov)
		{
			float num = UnityVectorExtensions.Angle(dir, up);
			float num2 = fov / 2f + 1f;
			if (num < num2)
			{
				float num3 = 1f - (num2 - num) / fov;
				if (r.yMax > num3)
				{
					r.yMin = Mathf.Min(r.yMin, num3);
					r.yMax = Mathf.Min(r.yMax, num3);
					return true;
				}
			}
			if (num > 180f - num2)
			{
				float num4 = (num - (180f - num2)) / fov;
				if (num4 > r.yMin)
				{
					r.yMin = Mathf.Max(r.yMin, num4);
					r.yMax = Mathf.Max(r.yMax, num4);
					return true;
				}
			}
			return false;
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

		[Header("Composition")]
		[HideFoldout]
		public ScreenComposerSettings Composition = ScreenComposerSettings.Default;

		[Tooltip("Force target to center of screen when this camera activates.  If false, will clamp target to the edges of the dead zone")]
		public bool CenterOnActivate = true;

		[Header("Target Tracking")]
		[Tooltip("Target offset from the target object's center in target-local space. Use this to fine-tune the tracking target position when the desired area is not the tracked object's center.")]
		[FormerlySerializedAs("TrackedObjectOffset")]
		public Vector3 TargetOffset;

		[Tooltip("How aggressively the camera tries to follow the target in the screen space. Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical and horizontal settings can yield a wide range of camera behaviors.")]
		public Vector2 Damping;

		[FoldoutWithEnabledButton("Enabled")]
		public LookaheadSettings Lookahead;

		private Vector3 m_CameraPosPrevFrame = Vector3.zero;

		private Vector3 m_LookAtPrevFrame = Vector3.zero;

		private Vector2 m_ScreenOffsetPrevFrame = Vector2.zero;

		private Quaternion m_CameraOrientationPrevFrame = Quaternion.identity;

		internal PositionPredictor m_Predictor;

		private ScreenComposerSettings m_CompositionLastFrame;

		private CinemachineRotationComposer.FovCache m_Cache;

		private struct FovCache
		{
			public void UpdateCache(ref LensSettings lens, Rect softGuide, Rect hardGuide, float targetDistance)
			{
				bool flag = this.m_Aspect != lens.Aspect || softGuide != this.m_DeadZoneRect || hardGuide != this.m_HardLimitRect || this.m_ScreenBounds == Vector2.zero;
				if (lens.Orthographic)
				{
					float num = Mathf.Abs(lens.OrthographicSize / targetDistance);
					if (this.m_OrthoSizeOverDistance == 0f || Mathf.Abs(num - this.m_OrthoSizeOverDistance) / this.m_OrthoSizeOverDistance > this.m_OrthoSizeOverDistance * 0.01f)
					{
						flag = true;
					}
					if (flag)
					{
						this.m_HalfFovRad = new Vector2(Mathf.Atan(lens.Aspect * num), Mathf.Atan(num));
						this.Fov = new Vector2(114.59156f * this.m_HalfFovRad.x, 114.59156f * this.m_HalfFovRad.y);
						this.m_OrthoSizeOverDistance = num;
					}
				}
				else
				{
					float fieldOfView = lens.FieldOfView;
					if (this.Fov.y != fieldOfView)
					{
						flag = true;
					}
					if (flag)
					{
						float num2 = fieldOfView * 0.017453292f * 0.5f;
						this.m_HalfFovRad = new Vector2((float)Math.Atan(Math.Tan((double)num2) * (double)lens.Aspect), num2);
						this.Fov = new Vector2(57.29578f * this.m_HalfFovRad.x * 2f, fieldOfView);
						this.m_OrthoSizeOverDistance = 0f;
					}
				}
				if (flag)
				{
					this.m_Aspect = lens.Aspect;
					this.m_ScreenBounds = new Vector2(Mathf.Tan(this.m_HalfFovRad.x), Mathf.Tan(this.m_HalfFovRad.y));
					this.m_DeadZoneRect = softGuide;
					this.m_HardLimitRect = hardGuide;
					this.FovSoftGuideRect = new Rect
					{
						min = this.ScreenToAngle(softGuide.min),
						max = this.ScreenToAngle(softGuide.max)
					};
					this.FovHardGuideRect = new Rect
					{
						min = this.ScreenToAngle(hardGuide.min),
						max = this.ScreenToAngle(hardGuide.max)
					};
				}
			}

			private readonly Vector2 ScreenToAngle(Vector2 p)
			{
				return new Vector2((this.m_HalfFovRad.x + Mathf.Atan(2f * (p.x - 0.5f) * this.m_ScreenBounds.x)) / (2f * this.m_HalfFovRad.x), (this.m_HalfFovRad.y + Mathf.Atan(2f * (p.y - 0.5f) * this.m_ScreenBounds.y)) / (2f * this.m_HalfFovRad.y));
			}

			public readonly Vector3 DirectionFromScreen(Vector2 p)
			{
				return new Vector3(2f * (p.x - 0.5f) * this.m_ScreenBounds.x, -2f * (p.y - 0.5f) * this.m_ScreenBounds.y, 1f);
			}

			public Rect FovSoftGuideRect;

			public Rect FovHardGuideRect;

			public Vector2 Fov;

			private float m_OrthoSizeOverDistance;

			private float m_Aspect;

			private Rect m_DeadZoneRect;

			private Rect m_HardLimitRect;

			private Vector2 m_ScreenBounds;

			private Vector2 m_HalfFovRad;
		}
	}
}
