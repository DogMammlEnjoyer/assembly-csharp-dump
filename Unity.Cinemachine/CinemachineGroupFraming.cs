using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Group Framing")]
	[ExecuteAlways]
	[SaveDuringPlay]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.GroupLookAt)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineGroupFraming.html")]
	public class CinemachineGroupFraming : CinemachineExtension
	{
		private void OnValidate()
		{
			this.FramingSize = Mathf.Max(0.01f, this.FramingSize);
			this.Damping = Mathf.Max(0f, this.Damping);
			this.DollyRange.y = Mathf.Max(this.DollyRange.x, this.DollyRange.y);
			this.FovRange.y = Mathf.Clamp(this.FovRange.y, 1f, 179f);
			this.FovRange.x = Mathf.Clamp(this.FovRange.x, 1f, this.FovRange.y);
			this.OrthoSizeRange.x = Mathf.Max(0.01f, this.OrthoSizeRange.x);
			this.OrthoSizeRange.y = Mathf.Max(this.OrthoSizeRange.x, this.OrthoSizeRange.y);
		}

		private void Reset()
		{
			this.FramingMode = CinemachineGroupFraming.FramingModes.HorizontalAndVertical;
			this.SizeAdjustment = CinemachineGroupFraming.SizeAdjustmentModes.DollyThenZoom;
			this.LateralAdjustment = CinemachineGroupFraming.LateralAdjustmentModes.ChangePosition;
			this.FramingSize = 0.8f;
			this.CenterOffset = Vector2.zero;
			this.Damping = 2f;
			this.DollyRange = new Vector2(-100f, 100f);
			this.FovRange = new Vector2(1f, 100f);
			this.OrthoSizeRange = new Vector2(1f, 1000f);
		}

		public override float GetMaxDampTime()
		{
			return this.Damping;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			CinemachineGroupFraming.VcamExtraState extraState = base.GetExtraState<CinemachineGroupFraming.VcamExtraState>(vcam);
			if (!vcam.PreviousStateIsValid)
			{
				extraState.Reset();
			}
			if (extraState.Stage == CinemachineCore.Stage.Finalize || !Application.isPlaying)
			{
				if (vcam.TryGetComponent<CinemachineConfiner2D>(out extraState.Confiner))
				{
					extraState.Stage = CinemachineCore.Stage.Body;
				}
				else
				{
					extraState.Stage = CinemachineCore.Stage.Aim;
					CinemachineCamera cinemachineCamera = vcam as CinemachineCamera;
					if (cinemachineCamera != null)
					{
						CinemachineComponentBase cinemachineComponent = cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
						if (cinemachineComponent != null && cinemachineComponent.BodyAppliesAfterAim)
						{
							extraState.Stage = CinemachineCore.Stage.Body;
						}
					}
				}
			}
			if (stage != extraState.Stage)
			{
				return;
			}
			ICinemachineTargetGroup cinemachineTargetGroup = vcam.LookAtTargetAsGroup;
			if (cinemachineTargetGroup == null)
			{
				cinemachineTargetGroup = vcam.FollowTargetAsGroup;
			}
			if (cinemachineTargetGroup == null || !cinemachineTargetGroup.IsValid)
			{
				return;
			}
			if (state.Lens.Orthographic)
			{
				this.OrthoFraming(vcam, cinemachineTargetGroup, extraState, ref state, deltaTime);
			}
			else
			{
				this.PerspectiveFraming(vcam, cinemachineTargetGroup, extraState, ref state, deltaTime);
			}
			if (extraState.Confiner != null && Mathf.Abs(extraState.PreviousOrthoSize - state.Lens.OrthographicSize) > 0.0001f)
			{
				extraState.Confiner.InvalidateLensCache();
				extraState.PreviousOrthoSize = state.Lens.OrthographicSize;
			}
		}

		private void OrthoFraming(CinemachineVirtualCameraBase vcam, ICinemachineTargetGroup group, CinemachineGroupFraming.VcamExtraState extra, ref CameraState state, float deltaTime)
		{
			float dampTime = (vcam.PreviousStateIsValid && deltaTime >= 0f) ? this.Damping : 0f;
			Vector3 correctedPosition = state.GetCorrectedPosition();
			Quaternion correctedOrientation = state.GetCorrectedOrientation();
			this.GroupBoundsMatrix = Matrix4x4.TRS(correctedPosition, correctedOrientation, Vector3.one);
			this.GroupBounds = group.GetViewSpaceBoundingBox(this.GroupBoundsMatrix, true);
			Vector3 center = this.GroupBounds.center;
			center.z = Mathf.Min(0f, center.z - this.GroupBounds.extents.z);
			LensSettings lens = state.Lens;
			float num = Mathf.Clamp(this.GetFrameHeight(this.GroupBounds.size / this.FramingSize, lens.Aspect) * 0.5f, this.OrthoSizeRange.x, this.OrthoSizeRange.y) - lens.OrthographicSize;
			extra.FovAdjustment += vcam.DetachedFollowTargetDamp(num - extra.FovAdjustment, dampTime, deltaTime);
			lens.OrthographicSize += extra.FovAdjustment;
			center.x -= this.CenterOffset.x * lens.OrthographicSize / lens.Aspect;
			center.y -= this.CenterOffset.y * lens.OrthographicSize;
			extra.PosAdjustment += vcam.DetachedFollowTargetDamp(center - extra.PosAdjustment, dampTime, deltaTime);
			state.PositionCorrection += correctedOrientation * extra.PosAdjustment;
			state.Lens = lens;
		}

		private void PerspectiveFraming(CinemachineVirtualCameraBase vcam, ICinemachineTargetGroup group, CinemachineGroupFraming.VcamExtraState extra, ref CameraState state, float deltaTime)
		{
			float dampTime = (vcam.PreviousStateIsValid && deltaTime >= 0f) ? this.Damping : 0f;
			Vector3 correctedPosition = state.GetCorrectedPosition();
			Quaternion correctedOrientation = state.GetCorrectedOrientation();
			Vector3 vector = correctedPosition;
			Quaternion quaternion = correctedOrientation;
			Vector3 vector2 = quaternion * Vector3.up;
			float fieldOfView = state.Lens.FieldOfView;
			bool flag = this.SizeAdjustment > CinemachineGroupFraming.SizeAdjustmentModes.ZoomOnly;
			Vector2 vector3 = flag ? this.DollyRange : Vector2.zero;
			Matrix4x4 observer = Matrix4x4.TRS(vector, quaternion, Vector3.one);
			Bounds viewSpaceBoundingBox = group.GetViewSpaceBoundingBox(observer, flag);
			bool flag2 = this.LateralAdjustment == CinemachineGroupFraming.LateralAdjustmentModes.ChangePosition;
			if (!flag2)
			{
				Vector3 vector4 = observer.MultiplyPoint3x4(viewSpaceBoundingBox.center) - vector;
				if (!Vector3.Cross(vector4, vector2).AlmostZero())
				{
					quaternion = Quaternion.LookRotation(vector4, vector2);
				}
			}
			float z = Mathf.Clamp(Mathf.Min(0f, viewSpaceBoundingBox.center.z) - viewSpaceBoundingBox.extents.z - 5f, vector3.x, vector3.y);
			vector += quaternion * new Vector3(0f, 0f, z);
			this.ComputeCameraViewGroupBounds(group, ref vector, ref quaternion, flag2);
			this.AdjustSize(group, state.Lens.Aspect, ref vector, ref quaternion, ref fieldOfView, ref z);
			LensSettings lens = state.Lens;
			float num = fieldOfView - lens.FieldOfView;
			extra.FovAdjustment += vcam.DetachedFollowTargetDamp(num - extra.FovAdjustment, dampTime, deltaTime);
			lens.FieldOfView += extra.FovAdjustment;
			state.Lens = lens;
			Vector2 cameraRotationToTarget = correctedOrientation.GetCameraRotationToTarget(quaternion * Vector3.forward, vector2);
			extra.RotAdjustment.x = extra.RotAdjustment.x + vcam.DetachedFollowTargetDamp(cameraRotationToTarget.x - extra.RotAdjustment.x, dampTime, deltaTime);
			extra.RotAdjustment.y = extra.RotAdjustment.y + vcam.DetachedFollowTargetDamp(cameraRotationToTarget.y - extra.RotAdjustment.y, dampTime, deltaTime);
			state.OrientationCorrection *= Quaternion.identity.ApplyCameraRotation(extra.RotAdjustment, vector2);
			correctedOrientation = state.GetCorrectedOrientation();
			Vector3 a = Quaternion.Inverse(correctedOrientation) * (vector - correctedPosition);
			extra.PosAdjustment += vcam.DetachedFollowTargetDamp(a - extra.PosAdjustment, dampTime, deltaTime);
			state.PositionCorrection += correctedOrientation * extra.PosAdjustment;
			if (Mathf.Abs(this.CenterOffset.x) > 0.01f || Mathf.Abs(this.CenterOffset.y) > 0.01f)
			{
				float num2 = 0.5f * state.Lens.FieldOfView;
				if (flag2)
				{
					float num3 = this.GroupBounds.center.z - this.GroupBounds.extents.z;
					state.PositionCorrection -= correctedOrientation * new Vector3(this.CenterOffset.x * Mathf.Tan(num2 * 0.017453292f * state.Lens.Aspect) * num3, this.CenterOffset.y * Mathf.Tan(num2 * 0.017453292f) * num3, 0f);
					return;
				}
				Vector2 rot = new Vector2(this.CenterOffset.y * num2, this.CenterOffset.x * num2 / state.Lens.Aspect);
				state.OrientationCorrection *= Quaternion.identity.ApplyCameraRotation(rot, state.ReferenceUp);
			}
		}

		private void AdjustSize(ICinemachineTargetGroup group, float aspect, ref Vector3 camPos, ref Quaternion camRot, ref float fov, ref float dollyAmount)
		{
			if (this.SizeAdjustment != CinemachineGroupFraming.SizeAdjustmentModes.ZoomOnly)
			{
				float frameHeight = this.GetFrameHeight(this.GroupBounds.size / this.FramingSize, aspect);
				float num = this.GroupBounds.center.z - this.GroupBounds.extents.z;
				float num2 = frameHeight / (2f * Mathf.Tan(fov * 0.017453292f / 2f));
				float num3 = num - num2;
				num3 = Mathf.Clamp(num3 + dollyAmount, this.DollyRange.x, this.DollyRange.y) - dollyAmount;
				dollyAmount += num3;
				camPos += camRot * new Vector3(0f, 0f, num3);
				this.ComputeCameraViewGroupBounds(group, ref camPos, ref camRot, true);
			}
			if (this.SizeAdjustment != CinemachineGroupFraming.SizeAdjustmentModes.DollyOnly)
			{
				float frameHeight2 = this.GetFrameHeight(this.GroupBounds.size / this.FramingSize, aspect);
				float num4 = this.GroupBounds.center.z - this.GroupBounds.extents.z;
				if (num4 > 0.0001f)
				{
					fov = 2f * Mathf.Atan(frameHeight2 / (2f * num4)) * 57.29578f;
				}
				fov = Mathf.Clamp(fov, this.FovRange.x, this.FovRange.y);
			}
		}

		private void ComputeCameraViewGroupBounds(ICinemachineTargetGroup group, ref Vector3 camPos, ref Quaternion camRot, bool moveCamera)
		{
			this.GroupBoundsMatrix = Matrix4x4.TRS(camPos, camRot, Vector3.one);
			if (moveCamera)
			{
				this.GroupBounds = group.GetViewSpaceBoundingBox(this.GroupBoundsMatrix, false);
				Vector3 center = this.GroupBounds.center;
				center.z = 0f;
				camPos = this.GroupBoundsMatrix.MultiplyPoint3x4(center);
				this.GroupBoundsMatrix = Matrix4x4.TRS(camPos, camRot, Vector3.one);
			}
			Vector2 vector;
			Vector2 vector2;
			Vector2 vector3;
			group.GetViewSpaceAngularBounds(this.GroupBoundsMatrix, out vector, out vector2, out vector3);
			Vector2 vector4 = (vector + vector2) / 2f;
			Quaternion quaternion = Quaternion.identity.ApplyCameraRotation(vector4, Vector3.up);
			if (moveCamera)
			{
				Vector3 vector5 = quaternion * Vector3.forward;
				float d;
				new Plane(Vector3.forward, new Vector3(0f, 0f, vector3.x)).Raycast(new Ray(Vector3.zero, vector5), out d);
				camPos = vector5 * d;
				camPos.z = 0f;
				camPos = this.GroupBoundsMatrix.MultiplyPoint3x4(camPos);
				this.GroupBoundsMatrix.SetColumn(3, camPos);
				group.GetViewSpaceAngularBounds(this.GroupBoundsMatrix, out vector, out vector2, out vector3);
			}
			else
			{
				camRot *= quaternion;
				this.GroupBoundsMatrix = Matrix4x4.TRS(camPos, camRot, Vector3.one);
				vector -= vector4;
				vector2 -= vector4;
			}
			Vector2 vector6 = new Vector2(89.5f, 89.5f);
			if (vector3.x > 0f)
			{
				vector6 = Vector2.Max(vector2, vector.Abs());
				vector6 = Vector2.Min(vector6, new Vector2(89.5f, 89.5f));
			}
			float num = vector3.x * 2f;
			vector6 *= 0.017453292f;
			this.GroupBounds = new Bounds(new Vector3(0f, 0f, (vector3.x + vector3.y) * 0.5f), new Vector3(Mathf.Tan(vector6.y) * num, Mathf.Tan(vector6.x) * num, vector3.y - vector3.x));
		}

		private float GetFrameHeight(Vector2 boundsSize, float aspect)
		{
			float a;
			switch (this.FramingMode)
			{
			case CinemachineGroupFraming.FramingModes.Horizontal:
				a = Mathf.Max(0.0001f, boundsSize.x) / aspect;
				goto IL_6B;
			case CinemachineGroupFraming.FramingModes.Vertical:
				a = Mathf.Max(0.0001f, boundsSize.y);
				goto IL_6B;
			}
			a = Mathf.Max(Mathf.Max(0.0001f, boundsSize.x) / aspect, Mathf.Max(0.0001f, boundsSize.y));
			IL_6B:
			return Mathf.Max(a, 0.01f);
		}

		[Tooltip("What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both")]
		public CinemachineGroupFraming.FramingModes FramingMode = CinemachineGroupFraming.FramingModes.HorizontalAndVertical;

		[Tooltip("The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc.")]
		[Range(0f, 2f)]
		public float FramingSize = 0.8f;

		[Tooltip("A nonzero value will offset the group in the camera frame.")]
		public Vector2 CenterOffset = Vector2.zero;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to frame the group. Small numbers are more responsive, rapidly adjusting the camera to keep the group in the frame.  Larger numbers give a heavier more slowly responding camera.")]
		public float Damping = 2f;

		[Tooltip("How to adjust the camera to get the desired framing size.  You can zoom, dolly in/out, or do both.")]
		public CinemachineGroupFraming.SizeAdjustmentModes SizeAdjustment = CinemachineGroupFraming.SizeAdjustmentModes.DollyThenZoom;

		[Tooltip("How to adjust the camera to get the desired horizontal and vertical framing.")]
		public CinemachineGroupFraming.LateralAdjustmentModes LateralAdjustment;

		[Tooltip("Allowable FOV range, if adjusting FOV.")]
		[MinMaxRangeSlider(1f, 179f)]
		public Vector2 FovRange = new Vector2(1f, 100f);

		[Tooltip("Allowable range for the camera to move.  0 is the undollied position.  Negative values move the camera closer to the target.")]
		[Vector2AsRange]
		public Vector2 DollyRange = new Vector2(-100f, 100f);

		[Tooltip("Allowable orthographic size range, if adjusting orthographic size.")]
		[Vector2AsRange]
		public Vector2 OrthoSizeRange = new Vector2(1f, 1000f);

		private const float k_MinimumGroupSize = 0.01f;

		internal Bounds GroupBounds;

		internal Matrix4x4 GroupBoundsMatrix;

		public enum FramingModes
		{
			Horizontal,
			Vertical,
			HorizontalAndVertical
		}

		public enum SizeAdjustmentModes
		{
			ZoomOnly,
			DollyOnly,
			DollyThenZoom
		}

		public enum LateralAdjustmentModes
		{
			ChangePosition,
			ChangeRotation
		}

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public void Reset()
			{
				this.PosAdjustment = Vector3.zero;
				this.RotAdjustment = Vector2.zero;
				this.FovAdjustment = 0f;
				this.Stage = CinemachineCore.Stage.Finalize;
			}

			public Vector3 PosAdjustment;

			public Vector2 RotAdjustment;

			public float FovAdjustment;

			public CinemachineCore.Stage Stage = CinemachineCore.Stage.Finalize;

			public CinemachineConfiner2D Confiner;

			public float PreviousOrthoSize;
		}
	}
}
