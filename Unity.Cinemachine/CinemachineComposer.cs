using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineComposer has been deprecated. Use CinemachineRotationComposer instead")]
	[CameraPipeline(CinemachineCore.Stage.Aim)]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineComposer.html")]
	public class CinemachineComposer : CinemachineComponentBase
	{
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

		public Vector3 TrackedPoint { get; private set; }

		protected virtual Vector3 GetLookAtPointAndSetTrackedPoint(Vector3 lookAt, Vector3 up, float deltaTime)
		{
			Vector3 vector = lookAt;
			if (base.LookAtTarget != null)
			{
				vector += base.LookAtTargetRotation * this.m_TrackedObjectOffset;
			}
			if (this.m_LookaheadTime < 0.0001f)
			{
				this.TrackedPoint = vector;
			}
			else
			{
				bool flag = base.VirtualCamera.LookAtTargetChanged || !base.VirtualCamera.PreviousStateIsValid;
				this.m_Predictor.Smoothing = this.m_LookaheadSmoothing;
				this.m_Predictor.AddPosition(vector, flag ? -1f : deltaTime);
				Vector3 vector2 = this.m_Predictor.PredictPositionDelta(this.m_LookaheadTime);
				if (this.m_LookaheadIgnoreY)
				{
					vector2 = vector2.ProjectOntoPlane(up);
				}
				this.TrackedPoint = vector + vector2;
			}
			return vector;
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
			this.m_CameraPosPrevFrame = pos;
			this.m_CameraOrientationPrevFrame = rot;
		}

		public override float GetMaxDampTime()
		{
			return Mathf.Max(this.m_HorizontalDamping, this.m_VerticalDamping);
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
			this.mCache.UpdateCache(curState.Lens, this.SoftGuideRect, this.HardGuideRect, magnitude);
			Quaternion quaternion = curState.RawOrientation;
			if (deltaTime < 0f || !base.VirtualCamera.PreviousStateIsValid)
			{
				quaternion = Quaternion.LookRotation(quaternion * Vector3.forward, curState.ReferenceUp);
				Rect mFovSoftGuideRect = this.mCache.mFovSoftGuideRect;
				if (this.m_CenterOnActivate)
				{
					mFovSoftGuideRect = new Rect(mFovSoftGuideRect.center, Vector2.zero);
				}
				this.RotateToScreenBounds(ref curState, mFovSoftGuideRect, curState.ReferenceLookAt, ref quaternion, this.mCache.mFov, this.mCache.mFovH, -1f);
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
					quaternion = Quaternion.LookRotation(vector, curState.ReferenceUp);
					quaternion = quaternion.ApplyCameraRotation(-this.m_ScreenOffsetPrevFrame, curState.ReferenceUp);
				}
				this.RotateToScreenBounds(ref curState, this.mCache.mFovSoftGuideRect, this.TrackedPoint, ref quaternion, this.mCache.mFov, this.mCache.mFovH, deltaTime);
				if (deltaTime < 0f || base.VirtualCamera.LookAtTargetAttachment > 0.9999f)
				{
					this.RotateToScreenBounds(ref curState, this.mCache.mFovHardGuideRect, curState.ReferenceLookAt, ref quaternion, this.mCache.mFov, this.mCache.mFovH, -1f);
				}
			}
			this.m_CameraPosPrevFrame = curState.GetCorrectedPosition();
			this.m_LookAtPrevFrame = this.TrackedPoint;
			this.m_CameraOrientationPrevFrame = quaternion.normalized;
			this.m_ScreenOffsetPrevFrame = this.m_CameraOrientationPrevFrame.GetCameraRotationToTarget(this.m_LookAtPrevFrame - curState.GetCorrectedPosition(), curState.ReferenceUp);
			curState.RawOrientation = this.m_CameraOrientationPrevFrame;
		}

		internal Rect SoftGuideRect
		{
			get
			{
				return new Rect(this.m_ScreenX - this.m_DeadZoneWidth / 2f, this.m_ScreenY - this.m_DeadZoneHeight / 2f, this.m_DeadZoneWidth, this.m_DeadZoneHeight);
			}
			set
			{
				this.m_DeadZoneWidth = Mathf.Clamp(value.width, 0f, 2f);
				this.m_DeadZoneHeight = Mathf.Clamp(value.height, 0f, 2f);
				this.m_ScreenX = Mathf.Clamp(value.x + this.m_DeadZoneWidth / 2f, -0.5f, 1.5f);
				this.m_ScreenY = Mathf.Clamp(value.y + this.m_DeadZoneHeight / 2f, -0.5f, 1.5f);
				this.m_SoftZoneWidth = Mathf.Max(this.m_SoftZoneWidth, this.m_DeadZoneWidth);
				this.m_SoftZoneHeight = Mathf.Max(this.m_SoftZoneHeight, this.m_DeadZoneHeight);
			}
		}

		internal Rect HardGuideRect
		{
			get
			{
				Rect result = new Rect(this.m_ScreenX - this.m_SoftZoneWidth / 2f, this.m_ScreenY - this.m_SoftZoneHeight / 2f, this.m_SoftZoneWidth, this.m_SoftZoneHeight);
				result.position += new Vector2(this.m_BiasX * (this.m_SoftZoneWidth - this.m_DeadZoneWidth), this.m_BiasY * (this.m_SoftZoneHeight - this.m_DeadZoneHeight));
				return result;
			}
			set
			{
				this.m_SoftZoneWidth = Mathf.Clamp(value.width, 0f, 2f);
				this.m_SoftZoneHeight = Mathf.Clamp(value.height, 0f, 2f);
				this.m_DeadZoneWidth = Mathf.Min(this.m_DeadZoneWidth, this.m_SoftZoneWidth);
				this.m_DeadZoneHeight = Mathf.Min(this.m_DeadZoneHeight, this.m_SoftZoneHeight);
			}
		}

		private void RotateToScreenBounds(ref CameraState state, Rect screenRect, Vector3 trackedPoint, ref Quaternion rigOrientation, float fov, float fovH, float deltaTime)
		{
			Vector3 vector = trackedPoint - state.GetCorrectedPosition();
			Vector2 cameraRotationToTarget = rigOrientation.GetCameraRotationToTarget(vector, state.ReferenceUp);
			this.ClampVerticalBounds(ref screenRect, vector, state.ReferenceUp, fov);
			float num = (screenRect.yMin - 0.5f) * fov;
			float num2 = (screenRect.yMax - 0.5f) * fov;
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
			num = (screenRect.xMin - 0.5f) * fovH;
			num2 = (screenRect.xMax - 0.5f) * fovH;
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
				cameraRotationToTarget.x = base.VirtualCamera.DetachedLookAtTargetDamp(cameraRotationToTarget.x, this.m_VerticalDamping, deltaTime);
				cameraRotationToTarget.y = base.VirtualCamera.DetachedLookAtTargetDamp(cameraRotationToTarget.y, this.m_HorizontalDamping, deltaTime);
			}
			rigOrientation = rigOrientation.ApplyCameraRotation(cameraRotationToTarget, state.ReferenceUp);
		}

		private bool ClampVerticalBounds(ref Rect r, Vector3 dir, Vector3 up, float fov)
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

		internal ScreenComposerSettings Composition
		{
			get
			{
				return new ScreenComposerSettings
				{
					ScreenPosition = new Vector2(this.m_ScreenX, this.m_ScreenY) - new Vector2(0.5f, 0.5f),
					DeadZone = new ScreenComposerSettings.DeadZoneSettings
					{
						Enabled = true,
						Size = new Vector2(this.m_DeadZoneWidth, this.m_DeadZoneHeight)
					},
					HardLimits = new ScreenComposerSettings.HardLimitSettings
					{
						Enabled = true,
						Size = new Vector2(this.m_SoftZoneWidth, this.m_SoftZoneHeight),
						Offset = new Vector2(this.m_BiasX, this.m_BiasY) * 2f
					}
				};
			}
			set
			{
				this.m_ScreenX = value.ScreenPosition.x + 0.5f;
				this.m_ScreenY = value.ScreenPosition.y + 0.5f;
				this.m_DeadZoneWidth = value.DeadZone.Size.x;
				this.m_DeadZoneHeight = value.DeadZone.Size.y;
				this.m_SoftZoneWidth = value.HardLimits.Size.x;
				this.m_SoftZoneHeight = value.HardLimits.Size.y;
				this.m_BiasX = value.HardLimits.Offset.x / 2f;
				this.m_BiasY = value.HardLimits.Offset.y / 2f;
			}
		}

		internal void UpgradeToCm3(CinemachineRotationComposer c)
		{
			c.TargetOffset = this.m_TrackedObjectOffset;
			c.Lookahead = new LookaheadSettings
			{
				Enabled = (this.m_LookaheadTime > 0f),
				Time = this.m_LookaheadTime,
				Smoothing = this.m_LookaheadSmoothing,
				IgnoreY = this.m_LookaheadIgnoreY
			};
			c.Damping = new Vector2(this.m_HorizontalDamping, this.m_VerticalDamping);
			c.Composition = this.Composition;
			c.CenterOnActivate = this.m_CenterOnActivate;
		}

		[Tooltip("Target offset from the target object's center in target-local space. Use this to fine-tune the tracking target position when the desired area is not the tracked object's center.")]
		public Vector3 m_TrackedObjectOffset = Vector3.zero;

		[Space]
		[Tooltip("This setting will instruct the composer to adjust its target offset based on the motion of the target.  The composer will look at a point where it estimates the target will be this many seconds into the future.  Note that this setting is sensitive to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly.")]
		[Range(0f, 1f)]
		public float m_LookaheadTime;

		[Tooltip("Controls the smoothness of the lookahead algorithm.  Larger values smooth out jittery predictions and also increase prediction lag")]
		[Range(0f, 30f)]
		public float m_LookaheadSmoothing;

		[Tooltip("If checked, movement along the Y axis will be ignored for lookahead calculations")]
		public bool m_LookaheadIgnoreY;

		[Space]
		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to follow the target in the screen-horizontal direction. Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical and horizontal settings can yield a wide range of camera behaviors.")]
		public float m_HorizontalDamping = 0.5f;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to follow the target in the screen-vertical direction. Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical and horizontal settings can yield a wide range of camera behaviors.")]
		public float m_VerticalDamping = 0.5f;

		[Space]
		[Range(-0.5f, 1.5f)]
		[Tooltip("Horizontal screen position for target. The camera will rotate to position the tracked object here.")]
		public float m_ScreenX = 0.5f;

		[Range(-0.5f, 1.5f)]
		[Tooltip("Vertical screen position for target, The camera will rotate to position the tracked object here.")]
		public float m_ScreenY = 0.5f;

		[Range(0f, 2f)]
		[Tooltip("Camera will not rotate horizontally if the target is within this range of the position.")]
		public float m_DeadZoneWidth;

		[Range(0f, 2f)]
		[Tooltip("Camera will not rotate vertically if the target is within this range of the position.")]
		public float m_DeadZoneHeight;

		[Range(0f, 2f)]
		[Tooltip("When target is within this region, camera will gradually rotate horizontally to re-align towards the desired position, depending on the damping speed.")]
		public float m_SoftZoneWidth = 0.8f;

		[Range(0f, 2f)]
		[Tooltip("When target is within this region, camera will gradually rotate vertically to re-align towards the desired position, depending on the damping speed.")]
		public float m_SoftZoneHeight = 0.8f;

		[Range(-0.5f, 0.5f)]
		[Tooltip("A non-zero bias will move the target position horizontally away from the center of the soft zone.")]
		public float m_BiasX;

		[Range(-0.5f, 0.5f)]
		[Tooltip("A non-zero bias will move the target position vertically away from the center of the soft zone.")]
		public float m_BiasY;

		[Tooltip("Force target to center of screen when this camera activates.  If false, will clamp target to the edges of the dead zone")]
		public bool m_CenterOnActivate = true;

		private Vector3 m_CameraPosPrevFrame = Vector3.zero;

		private Vector3 m_LookAtPrevFrame = Vector3.zero;

		private Vector2 m_ScreenOffsetPrevFrame = Vector2.zero;

		private Quaternion m_CameraOrientationPrevFrame = Quaternion.identity;

		internal PositionPredictor m_Predictor;

		private CinemachineComposer.FovCache mCache;

		private struct FovCache
		{
			public void UpdateCache(LensSettings lens, Rect softGuide, Rect hardGuide, float targetDistance)
			{
				bool flag = this.mAspect != lens.Aspect || softGuide != this.mSoftGuideRect || hardGuide != this.mHardGuideRect;
				if (lens.Orthographic)
				{
					float num = Mathf.Abs(lens.OrthographicSize / targetDistance);
					if (this.mOrthoSizeOverDistance == 0f || Mathf.Abs(num - this.mOrthoSizeOverDistance) / this.mOrthoSizeOverDistance > this.mOrthoSizeOverDistance * 0.01f)
					{
						flag = true;
					}
					if (flag)
					{
						this.mFov = 114.59156f * Mathf.Atan(num);
						this.mFovH = 114.59156f * Mathf.Atan(lens.Aspect * num);
						this.mOrthoSizeOverDistance = num;
					}
				}
				else
				{
					float fieldOfView = lens.FieldOfView;
					if (this.mFov != fieldOfView)
					{
						flag = true;
					}
					if (flag)
					{
						this.mFov = fieldOfView;
						double num2 = 2.0 * Math.Atan(Math.Tan((double)(this.mFov * 0.017453292f / 2f)) * (double)lens.Aspect);
						this.mFovH = (float)(57.295780181884766 * num2);
						this.mOrthoSizeOverDistance = 0f;
					}
				}
				if (flag)
				{
					this.mFovSoftGuideRect = this.ScreenToFOV(softGuide, this.mFov, this.mFovH, lens.Aspect);
					this.mSoftGuideRect = softGuide;
					this.mFovHardGuideRect = this.ScreenToFOV(hardGuide, this.mFov, this.mFovH, lens.Aspect);
					this.mHardGuideRect = hardGuide;
					this.mAspect = lens.Aspect;
				}
			}

			private Rect ScreenToFOV(Rect rScreen, float fov, float fovH, float aspect)
			{
				Rect result = new Rect(rScreen);
				Matrix4x4 inverse = Matrix4x4.Perspective(fov, aspect, 0.0001f, 2f).inverse;
				Vector3 vector = inverse.MultiplyPoint(new Vector3(0f, result.yMin * 2f - 1f, 0.5f));
				vector.z = -vector.z;
				float num = UnityVectorExtensions.SignedAngle(Vector3.forward, vector, Vector3.left);
				result.yMin = (fov / 2f + num) / fov;
				vector = inverse.MultiplyPoint(new Vector3(0f, result.yMax * 2f - 1f, 0.5f));
				vector.z = -vector.z;
				num = UnityVectorExtensions.SignedAngle(Vector3.forward, vector, Vector3.left);
				result.yMax = (fov / 2f + num) / fov;
				vector = inverse.MultiplyPoint(new Vector3(result.xMin * 2f - 1f, 0f, 0.5f));
				vector.z = -vector.z;
				num = UnityVectorExtensions.SignedAngle(Vector3.forward, vector, Vector3.up);
				result.xMin = (fovH / 2f + num) / fovH;
				vector = inverse.MultiplyPoint(new Vector3(result.xMax * 2f - 1f, 0f, 0.5f));
				vector.z = -vector.z;
				num = UnityVectorExtensions.SignedAngle(Vector3.forward, vector, Vector3.up);
				result.xMax = (fovH / 2f + num) / fovH;
				return result;
			}

			public Rect mFovSoftGuideRect;

			public Rect mFovHardGuideRect;

			public float mFovH;

			public float mFov;

			private float mOrthoSizeOverDistance;

			private float mAspect;

			private Rect mSoftGuideRect;

			private Rect mHardGuideRect;
		}
	}
}
