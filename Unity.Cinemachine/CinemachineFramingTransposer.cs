using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("")]
	[Obsolete("CinemachineFramingTransposer has been deprecated. Use CinemachinePositionComposer instead")]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	[SaveDuringPlay]
	public class CinemachineFramingTransposer : CinemachineComponentBase
	{
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

		private void OnValidate()
		{
			this.m_CameraDistance = Mathf.Max(this.m_CameraDistance, 0.01f);
			this.m_DeadZoneDepth = Mathf.Max(this.m_DeadZoneDepth, 0f);
			this.m_GroupFramingSize = Mathf.Max(0.001f, this.m_GroupFramingSize);
			this.m_MaxDollyIn = Mathf.Max(0f, this.m_MaxDollyIn);
			this.m_MaxDollyOut = Mathf.Max(0f, this.m_MaxDollyOut);
			this.m_MinimumDistance = Mathf.Max(0f, this.m_MinimumDistance);
			this.m_MaximumDistance = Mathf.Max(this.m_MinimumDistance, this.m_MaximumDistance);
			this.m_MinimumFOV = Mathf.Max(1f, this.m_MinimumFOV);
			this.m_MaximumFOV = Mathf.Clamp(this.m_MaximumFOV, this.m_MinimumFOV, 179f);
			this.m_MinimumOrthoSize = Mathf.Max(0.01f, this.m_MinimumOrthoSize);
			this.m_MaximumOrthoSize = Mathf.Max(this.m_MinimumOrthoSize, this.m_MaximumOrthoSize);
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

		public Vector3 TrackedPoint { get; private set; }

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
			this.m_PreviousCameraPosition = pos;
			this.m_prevRotation = rot;
		}

		public override float GetMaxDampTime()
		{
			return Mathf.Max(this.m_XDamping, Mathf.Max(this.m_YDamping, this.m_ZDamping));
		}

		public override bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			if (fromCam != null && (base.VirtualCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && !CinemachineCore.IsLiveInBlend(base.VirtualCamera))
			{
				this.m_PreviousCameraPosition = fromCam.State.RawPosition;
				this.m_prevRotation = fromCam.State.RawOrientation;
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

		public Bounds LastBounds { get; private set; }

		public Matrix4x4 LastBoundsMatrix { get; private set; }

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			LensSettings lens = curState.Lens;
			Vector3 vector = base.FollowTargetPosition + base.FollowTargetRotation * this.m_TrackedObjectOffset;
			bool flag = deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid;
			if (!flag || base.VirtualCamera.FollowTargetChanged)
			{
				this.m_Predictor.Reset();
			}
			if (!flag)
			{
				this.m_PreviousCameraPosition = curState.RawPosition;
				this.m_prevFOV = (lens.Orthographic ? lens.OrthographicSize : lens.FieldOfView);
				this.m_prevRotation = curState.RawOrientation;
				if (!this.m_InheritingPosition && this.m_CenterOnActivate)
				{
					this.m_PreviousCameraPosition = base.FollowTargetPosition + curState.RawOrientation * Vector3.back * this.m_CameraDistance;
				}
			}
			if (!this.IsValid)
			{
				this.m_InheritingPosition = false;
				return;
			}
			float fieldOfView = lens.FieldOfView;
			ICinemachineTargetGroup followTargetAsGroup = base.FollowTargetAsGroup;
			bool flag2 = followTargetAsGroup != null && followTargetAsGroup.IsValid && this.m_GroupFramingMode != CinemachineFramingTransposer.FramingMode.None && !followTargetAsGroup.IsEmpty;
			if (flag2)
			{
				vector = this.ComputeGroupBounds(followTargetAsGroup, ref curState);
			}
			this.TrackedPoint = vector;
			if (this.m_LookaheadTime > 0.0001f)
			{
				this.m_Predictor.Smoothing = this.m_LookaheadSmoothing;
				this.m_Predictor.AddPosition(vector, deltaTime);
				Vector3 vector2 = this.m_Predictor.PredictPositionDelta(this.m_LookaheadTime);
				if (this.m_LookaheadIgnoreY)
				{
					vector2 = vector2.ProjectOntoPlane(curState.ReferenceUp);
				}
				Vector3 trackedPoint = vector + vector2;
				if (flag2)
				{
					Bounds lastBounds = this.LastBounds;
					lastBounds.center += this.LastBoundsMatrix.MultiplyPoint3x4(vector2);
					this.LastBounds = lastBounds;
				}
				this.TrackedPoint = trackedPoint;
			}
			if (!curState.HasLookAt())
			{
				curState.ReferenceLookAt = vector;
			}
			float num = this.m_CameraDistance;
			bool orthographic = lens.Orthographic;
			float num2 = flag2 ? this.GetTargetHeight(this.LastBounds.size / this.m_GroupFramingSize) : 0f;
			num2 = Mathf.Max(num2, 0.01f);
			if (!orthographic && flag2)
			{
				float z = this.LastBounds.extents.z;
				float z2 = this.LastBounds.center.z;
				if (z2 > z)
				{
					num2 = Mathf.Lerp(0f, num2, (z2 - z) / z2);
				}
				if (this.m_AdjustmentMode != CinemachineFramingTransposer.AdjustmentMode.ZoomOnly)
				{
					num = num2 / (2f * Mathf.Tan(fieldOfView * 0.017453292f / 2f));
					num = Mathf.Clamp(num, this.m_MinimumDistance, this.m_MaximumDistance);
					float num3 = num - this.m_CameraDistance;
					num3 = Mathf.Clamp(num3, -this.m_MaxDollyIn, this.m_MaxDollyOut);
					num = this.m_CameraDistance + num3;
				}
			}
			Quaternion rawOrientation = curState.RawOrientation;
			if (flag && this.m_TargetMovementOnly)
			{
				Quaternion rotation = rawOrientation * Quaternion.Inverse(this.m_prevRotation);
				this.m_PreviousCameraPosition = this.TrackedPoint + rotation * (this.m_PreviousCameraPosition - this.TrackedPoint);
			}
			this.m_prevRotation = rawOrientation;
			Vector3 previousCameraPosition = this.m_PreviousCameraPosition;
			Quaternion rotation2 = Quaternion.Inverse(rawOrientation);
			Vector3 b = rotation2 * previousCameraPosition;
			Vector3 vector3 = rotation2 * this.TrackedPoint - b;
			Vector3 vector4 = vector3;
			Vector3 vector5 = Vector3.zero;
			float num4 = Mathf.Max(0.01f, num - this.m_DeadZoneDepth / 2f);
			float num5 = Mathf.Max(num4, num + this.m_DeadZoneDepth / 2f);
			float num6 = Mathf.Min(vector3.z, vector4.z);
			if (num6 < num4)
			{
				vector5.z = num6 - num4;
			}
			if (num6 > num5)
			{
				vector5.z = num6 - num5;
			}
			float orthoSize = lens.Orthographic ? lens.OrthographicSize : (Mathf.Tan(0.5f * fieldOfView * 0.017453292f) * (num6 - vector5.z));
			Rect rect = this.ScreenToOrtho(this.SoftGuideRect, orthoSize, lens.Aspect);
			if (!flag)
			{
				Rect screenRect = rect;
				if (this.m_CenterOnActivate && !this.m_InheritingPosition)
				{
					screenRect = new Rect(screenRect.center, Vector2.zero);
				}
				vector5 += this.OrthoOffsetToScreenBounds(vector3, screenRect);
			}
			else
			{
				vector5 += this.OrthoOffsetToScreenBounds(vector3, rect);
				vector5 = base.VirtualCamera.DetachedFollowTargetDamp(vector5, new Vector3(this.m_XDamping, this.m_YDamping, this.m_ZDamping), deltaTime);
				if (!this.m_UnlimitedSoftZone && (deltaTime < 0f || base.VirtualCamera.FollowTargetAttachment > 0.9999f))
				{
					Rect screenRect2 = this.ScreenToOrtho(this.HardGuideRect, orthoSize, lens.Aspect);
					Vector3 a = rotation2 * vector - b;
					vector5 += this.OrthoOffsetToScreenBounds(a - vector5, screenRect2);
				}
			}
			curState.RawPosition = previousCameraPosition + rawOrientation * vector5;
			this.m_PreviousCameraPosition = curState.RawPosition;
			if (flag2)
			{
				if (orthographic)
				{
					num2 = Mathf.Clamp(num2 / 2f, this.m_MinimumOrthoSize, this.m_MaximumOrthoSize);
					if (flag)
					{
						num2 = this.m_prevFOV + base.VirtualCamera.DetachedFollowTargetDamp(num2 - this.m_prevFOV, this.m_ZDamping, deltaTime);
					}
					this.m_prevFOV = num2;
					lens.OrthographicSize = Mathf.Clamp(num2, this.m_MinimumOrthoSize, this.m_MaximumOrthoSize);
					curState.Lens = lens;
				}
				else if (this.m_AdjustmentMode != CinemachineFramingTransposer.AdjustmentMode.DollyOnly)
				{
					float z3 = (Quaternion.Inverse(curState.RawOrientation) * (vector - curState.RawPosition)).z;
					float num7 = 179f;
					if (z3 > 0.0001f)
					{
						num7 = 2f * Mathf.Atan(num2 / (2f * z3)) * 57.29578f;
					}
					num7 = Mathf.Clamp(num7, this.m_MinimumFOV, this.m_MaximumFOV);
					if (flag)
					{
						num7 = this.m_prevFOV + base.VirtualCamera.DetachedFollowTargetDamp(num7 - this.m_prevFOV, this.m_ZDamping, deltaTime);
					}
					this.m_prevFOV = num7;
					lens.FieldOfView = num7;
					curState.Lens = lens;
				}
			}
			this.m_InheritingPosition = false;
		}

		private float GetTargetHeight(Vector2 boundsSize)
		{
			CameraState vcamState;
			switch (this.m_GroupFramingMode)
			{
			case CinemachineFramingTransposer.FramingMode.Horizontal:
			{
				float x = boundsSize.x;
				vcamState = base.VcamState;
				return x / vcamState.Lens.Aspect;
			}
			case CinemachineFramingTransposer.FramingMode.Vertical:
				return boundsSize.y;
			}
			float x2 = boundsSize.x;
			vcamState = base.VcamState;
			return Mathf.Max(x2 / vcamState.Lens.Aspect, boundsSize.y);
		}

		private Vector3 ComputeGroupBounds(ICinemachineTargetGroup group, ref CameraState curState)
		{
			Vector3 vector = curState.RawPosition;
			Vector3 a = curState.RawOrientation * Vector3.forward;
			this.LastBoundsMatrix = Matrix4x4.TRS(vector, curState.RawOrientation, Vector3.one);
			Bounds lastBounds = group.GetViewSpaceBoundingBox(this.LastBoundsMatrix, true);
			Vector3 a2 = this.LastBoundsMatrix.MultiplyPoint3x4(lastBounds.center);
			float z = lastBounds.extents.z;
			if (!curState.Lens.Orthographic)
			{
				float z2 = (Quaternion.Inverse(curState.RawOrientation) * (a2 - vector)).z;
				vector = a2 - a * (Mathf.Max(z2, z) + z);
				lastBounds = CinemachineFramingTransposer.GetScreenSpaceGroupBoundingBox(group, ref vector, curState.RawOrientation);
				this.LastBoundsMatrix = Matrix4x4.TRS(vector, curState.RawOrientation, Vector3.one);
				a2 = this.LastBoundsMatrix.MultiplyPoint3x4(lastBounds.center);
			}
			this.LastBounds = lastBounds;
			return a2 - a * z;
		}

		private static Bounds GetScreenSpaceGroupBoundingBox(ICinemachineTargetGroup group, ref Vector3 pos, Quaternion orientation)
		{
			Matrix4x4 observer = Matrix4x4.TRS(pos, orientation, Vector3.one);
			Vector2 vector;
			Vector2 vector2;
			Vector2 vector3;
			group.GetViewSpaceAngularBounds(observer, out vector, out vector2, out vector3);
			Vector2 rot = (vector + vector2) / 2f;
			Quaternion rotation = Quaternion.identity.ApplyCameraRotation(rot, Vector3.up);
			pos = rotation * new Vector3(0f, 0f, (vector3.y + vector3.x) / 2f);
			pos.z = 0f;
			pos = observer.MultiplyPoint3x4(pos);
			observer = Matrix4x4.TRS(pos, orientation, Vector3.one);
			group.GetViewSpaceAngularBounds(observer, out vector, out vector2, out vector3);
			float num = vector3.y + vector3.x;
			Vector2 vector4 = new Vector2(89.5f, 89.5f);
			if (vector3.x > 0f)
			{
				vector4 = Vector2.Max(vector2, vector.Abs());
				vector4 = Vector2.Min(vector4, new Vector2(89.5f, 89.5f));
			}
			vector4 *= 0.017453292f;
			return new Bounds(new Vector3(0f, 0f, num / 2f), new Vector3(Mathf.Tan(vector4.y) * num, Mathf.Tan(vector4.x) * num, vector3.y - vector3.x));
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
						Enabled = !this.m_UnlimitedSoftZone,
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

		internal void UpgradeToCm3(CinemachinePositionComposer c)
		{
			c.TargetOffset = this.m_TrackedObjectOffset;
			c.Lookahead = new LookaheadSettings
			{
				Enabled = (this.m_LookaheadTime > 0f),
				Time = this.m_LookaheadTime,
				Smoothing = this.m_LookaheadSmoothing,
				IgnoreY = this.m_LookaheadIgnoreY
			};
			c.CameraDistance = this.m_CameraDistance;
			c.DeadZoneDepth = this.m_DeadZoneDepth;
			c.Damping = new Vector3(this.m_XDamping, this.m_YDamping, this.m_ZDamping);
			c.Composition = this.Composition;
			c.CenterOnActivate = this.m_CenterOnActivate;
		}

		internal void UpgradeToCm3(CinemachineGroupFraming c)
		{
			c.FramingMode = (CinemachineGroupFraming.FramingModes)this.m_GroupFramingMode;
			c.FramingSize = this.m_GroupFramingSize;
			c.Damping = this.m_ZDamping;
			c.SizeAdjustment = (CinemachineGroupFraming.SizeAdjustmentModes)this.m_AdjustmentMode;
			c.LateralAdjustment = CinemachineGroupFraming.LateralAdjustmentModes.ChangePosition;
			c.DollyRange = new Vector2(-this.m_MaxDollyIn, this.m_MaxDollyOut);
			c.FovRange = new Vector2(this.m_MinimumFOV, this.m_MaximumFOV);
			c.OrthoSizeRange = new Vector2(this.m_MinimumOrthoSize, this.m_MaximumOrthoSize);
		}

		[Tooltip("Offset from the Follow Target object (in target-local co-ordinates).  The camera will attempt to frame the point which is the target's position plus this offset.  Use it to correct for cases when the target's origin is not the point of interest for the camera.")]
		public Vector3 m_TrackedObjectOffset;

		[Tooltip("This setting will instruct the composer to adjust its target offset based on the motion of the target.  The composer will look at a point where it estimates the target will be this many seconds into the future.  Note that this setting is sensitive to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly.")]
		[Range(0f, 1f)]
		[Space]
		public float m_LookaheadTime;

		[Tooltip("Controls the smoothness of the lookahead algorithm.  Larger values smooth out jittery predictions and also increase prediction lag")]
		[Range(0f, 30f)]
		public float m_LookaheadSmoothing;

		[Tooltip("If checked, movement along the Y axis will be ignored for lookahead calculations")]
		public bool m_LookaheadIgnoreY;

		[Space]
		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain the offset in the X-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_XDamping = 1f;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain the offset in the Y-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_YDamping = 1f;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain the offset in the Z-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera.  Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_ZDamping = 1f;

		[Tooltip("If set, damping will apply  only to target motion, but not to camera rotation changes.  Turn this on to get an instant response when the rotation changes.  ")]
		public bool m_TargetMovementOnly = true;

		[Space]
		[Range(-0.5f, 1.5f)]
		[Tooltip("Horizontal screen position for target. The camera will move to position the tracked object here.")]
		public float m_ScreenX = 0.5f;

		[Range(-0.5f, 1.5f)]
		[Tooltip("Vertical screen position for target, The camera will move to position the tracked object here.")]
		public float m_ScreenY = 0.5f;

		[Tooltip("The distance along the camera axis that will be maintained from the Follow target")]
		public float m_CameraDistance = 10f;

		[Space]
		[Range(0f, 2f)]
		[Tooltip("Camera will not move horizontally if the target is within this range of the position.")]
		public float m_DeadZoneWidth;

		[Range(0f, 2f)]
		[Tooltip("Camera will not move vertically if the target is within this range of the position.")]
		public float m_DeadZoneHeight;

		[Tooltip("The camera will not move along its z-axis if the Follow target is within this distance of the specified camera distance")]
		[FormerlySerializedAs("m_DistanceDeadZoneSize")]
		public float m_DeadZoneDepth;

		[Space]
		[Tooltip("If checked, then then soft zone will be unlimited in size.")]
		public bool m_UnlimitedSoftZone;

		[Range(0f, 2f)]
		[Tooltip("When target is within this region, camera will gradually move horizontally to re-align towards the desired position, depending on the damping speed.")]
		public float m_SoftZoneWidth = 0.8f;

		[Range(0f, 2f)]
		[Tooltip("When target is within this region, camera will gradually move vertically to re-align towards the desired position, depending on the damping speed.")]
		public float m_SoftZoneHeight = 0.8f;

		[Range(-0.5f, 0.5f)]
		[Tooltip("A non-zero bias will move the target position horizontally away from the center of the soft zone.")]
		public float m_BiasX;

		[Range(-0.5f, 0.5f)]
		[Tooltip("A non-zero bias will move the target position vertically away from the center of the soft zone.")]
		public float m_BiasY;

		[Tooltip("Force target to center of screen when this camera activates.  If false, will clamp target to the edges of the dead zone")]
		public bool m_CenterOnActivate = true;

		[Space]
		[Tooltip("What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both")]
		[FormerlySerializedAs("m_FramingMode")]
		public CinemachineFramingTransposer.FramingMode m_GroupFramingMode = CinemachineFramingTransposer.FramingMode.HorizontalAndVertical;

		[Tooltip("How to adjust the camera to get the desired framing.  You can zoom, dolly in/out, or do both.")]
		public CinemachineFramingTransposer.AdjustmentMode m_AdjustmentMode;

		[Tooltip("The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc.")]
		public float m_GroupFramingSize = 0.8f;

		[Tooltip("The maximum distance toward the target that this behaviour is allowed to move the camera.")]
		public float m_MaxDollyIn = 5000f;

		[Tooltip("The maximum distance away the target that this behaviour is allowed to move the camera.")]
		public float m_MaxDollyOut = 5000f;

		[Tooltip("Set this to limit how close to the target the camera can get.")]
		public float m_MinimumDistance = 1f;

		[Tooltip("Set this to limit how far from the target the camera can get.")]
		public float m_MaximumDistance = 5000f;

		[Range(1f, 179f)]
		[Tooltip("If adjusting FOV, will not set the FOV lower than this.")]
		public float m_MinimumFOV = 3f;

		[Range(1f, 179f)]
		[Tooltip("If adjusting FOV, will not set the FOV higher than this.")]
		public float m_MaximumFOV = 60f;

		[Tooltip("If adjusting Orthographic Size, will not set it lower than this.")]
		public float m_MinimumOrthoSize = 1f;

		[Tooltip("If adjusting Orthographic Size, will not set it higher than this.")]
		public float m_MaximumOrthoSize = 5000f;

		private const float kMinimumCameraDistance = 0.01f;

		private const float kMinimumGroupSize = 0.01f;

		private Vector3 m_PreviousCameraPosition = Vector3.zero;

		internal PositionPredictor m_Predictor;

		private bool m_InheritingPosition;

		private float m_prevFOV;

		private Quaternion m_prevRotation;

		public enum FramingMode
		{
			Horizontal,
			Vertical,
			HorizontalAndVertical,
			None
		}

		public enum AdjustmentMode
		{
			ZoomOnly,
			DollyOnly,
			DollyThenZoom
		}
	}
}
