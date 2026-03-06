using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineGroupTransposer has been deprecated. Use CinemachineRotationComposer and CinemachineGroupFraming instead")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[CameraPipeline(CinemachineCore.Stage.Aim)]
	public class CinemachineGroupComposer : CinemachineComposer
	{
		private void OnValidate()
		{
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

		private void Reset()
		{
			this.m_GroupFramingSize = 0.8f;
			this.m_FramingMode = CinemachineGroupComposer.FramingMode.HorizontalAndVertical;
			this.m_FrameDamping = 2f;
			this.m_AdjustmentMode = CinemachineGroupComposer.AdjustmentMode.ZoomOnly;
			this.m_MaxDollyIn = 5000f;
			this.m_MaxDollyOut = 5000f;
			this.m_MinimumDistance = 1f;
			this.m_MaximumDistance = 5000f;
			this.m_MinimumFOV = 3f;
			this.m_MaximumFOV = 60f;
			this.m_MinimumOrthoSize = 1f;
			this.m_MaximumOrthoSize = 5000f;
		}

		public Bounds LastBounds { get; private set; }

		public Matrix4x4 LastBoundsMatrix { get; private set; }

		public override float GetMaxDampTime()
		{
			return Mathf.Max(base.GetMaxDampTime(), this.m_FrameDamping);
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			ICinemachineTargetGroup lookAtTargetAsGroup = base.LookAtTargetAsGroup;
			if (lookAtTargetAsGroup == null || !lookAtTargetAsGroup.IsValid)
			{
				base.MutateCameraState(ref curState, deltaTime);
				return;
			}
			if (!this.IsValid || !curState.HasLookAt())
			{
				this.m_prevFramingDistance = 0f;
				this.m_prevFOV = 0f;
				return;
			}
			bool orthographic = curState.Lens.Orthographic;
			bool flag = !orthographic && this.m_AdjustmentMode > CinemachineGroupComposer.AdjustmentMode.ZoomOnly;
			Vector3 referenceUp = curState.ReferenceUp;
			Vector3 vector = curState.RawPosition;
			Vector3 vector2 = lookAtTargetAsGroup.Sphere.position;
			Vector3 vector3 = vector2 - vector;
			float magnitude = vector3.magnitude;
			if (magnitude < 0.0001f)
			{
				return;
			}
			vector3 /= magnitude;
			this.LastBoundsMatrix = Matrix4x4.TRS(vector, Quaternion.LookRotation(vector3, referenceUp), Vector3.one);
			Bounds lastBounds;
			if (orthographic)
			{
				lastBounds = lookAtTargetAsGroup.GetViewSpaceBoundingBox(this.LastBoundsMatrix, true);
				vector2 = this.LastBoundsMatrix.MultiplyPoint3x4(lastBounds.center);
				vector3 = (vector2 - vector).normalized;
				this.LastBoundsMatrix = Matrix4x4.TRS(vector, Quaternion.LookRotation(vector3, referenceUp), Vector3.one);
				lastBounds = lookAtTargetAsGroup.GetViewSpaceBoundingBox(this.LastBoundsMatrix, true);
				this.LastBounds = lastBounds;
			}
			else
			{
				lastBounds = CinemachineGroupComposer.GetScreenSpaceGroupBoundingBox(lookAtTargetAsGroup, this.LastBoundsMatrix, out vector3);
				this.LastBoundsMatrix = Matrix4x4.TRS(vector, Quaternion.LookRotation(vector3, referenceUp), Vector3.one);
				this.LastBounds = lastBounds;
				vector2 = vector + vector3 * lastBounds.center.z;
			}
			float z = lastBounds.extents.z;
			float num = this.GetTargetHeight(lastBounds.size / this.m_GroupFramingSize);
			if (orthographic)
			{
				num = Mathf.Clamp(num / 2f, this.m_MinimumOrthoSize, this.m_MaximumOrthoSize);
				if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
				{
					num = this.m_prevFOV + base.VirtualCamera.DetachedLookAtTargetDamp(num - this.m_prevFOV, this.m_FrameDamping, deltaTime);
				}
				this.m_prevFOV = num;
				LensSettings lens = curState.Lens;
				lens.OrthographicSize = Mathf.Clamp(num, this.m_MinimumOrthoSize, this.m_MaximumOrthoSize);
				curState.Lens = lens;
			}
			else
			{
				float z2 = lastBounds.center.z;
				if (z2 > z)
				{
					num = Mathf.Lerp(0f, num, (z2 - z) / z2);
				}
				if (flag)
				{
					float num2 = Mathf.Clamp(z + num / (2f * Mathf.Tan(curState.Lens.FieldOfView * 0.017453292f / 2f)), z + this.m_MinimumDistance, z + this.m_MaximumDistance) - Vector3.Distance(curState.RawPosition, vector2);
					num2 = Mathf.Clamp(num2, -this.m_MaxDollyIn, this.m_MaxDollyOut);
					if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
					{
						float num3 = num2 - this.m_prevFramingDistance;
						num3 = base.VirtualCamera.DetachedLookAtTargetDamp(num3, this.m_FrameDamping, deltaTime);
						num2 = this.m_prevFramingDistance + num3;
					}
					this.m_prevFramingDistance = num2;
					curState.PositionCorrection -= vector3 * num2;
					vector -= vector3 * num2;
				}
				if (this.m_AdjustmentMode != CinemachineGroupComposer.AdjustmentMode.DollyOnly)
				{
					float num4 = (vector2 - vector).magnitude - z;
					float num5 = 179f;
					if (num4 > 0.0001f)
					{
						num5 = 2f * Mathf.Atan(num / (2f * num4)) * 57.29578f;
					}
					num5 = Mathf.Clamp(num5, this.m_MinimumFOV, this.m_MaximumFOV);
					if (deltaTime >= 0f && this.m_prevFOV != 0f && base.VirtualCamera.PreviousStateIsValid)
					{
						num5 = this.m_prevFOV + base.VirtualCamera.DetachedLookAtTargetDamp(num5 - this.m_prevFOV, this.m_FrameDamping, deltaTime);
					}
					this.m_prevFOV = num5;
					LensSettings lens2 = curState.Lens;
					lens2.FieldOfView = num5;
					curState.Lens = lens2;
				}
			}
			curState.ReferenceLookAt = this.GetLookAtPointAndSetTrackedPoint(vector2, curState.ReferenceUp, deltaTime);
			base.MutateCameraState(ref curState, deltaTime);
		}

		private float GetTargetHeight(Vector2 boundsSize)
		{
			CameraState vcamState;
			switch (this.m_FramingMode)
			{
			case CinemachineGroupComposer.FramingMode.Horizontal:
			{
				float num = Mathf.Max(0.0001f, boundsSize.x);
				vcamState = base.VcamState;
				return num / vcamState.Lens.Aspect;
			}
			case CinemachineGroupComposer.FramingMode.Vertical:
				return Mathf.Max(0.0001f, boundsSize.y);
			}
			float num2 = Mathf.Max(0.0001f, boundsSize.x);
			vcamState = base.VcamState;
			return Mathf.Max(num2 / vcamState.Lens.Aspect, Mathf.Max(0.0001f, boundsSize.y));
		}

		private static Bounds GetScreenSpaceGroupBoundingBox(ICinemachineTargetGroup group, Matrix4x4 observer, out Vector3 newFwd)
		{
			Vector2 a;
			Vector2 vector;
			Vector2 vector2;
			group.GetViewSpaceAngularBounds(observer, out a, out vector, out vector2);
			Vector2 vector3 = (a + vector) / 2f;
			newFwd = Quaternion.identity.ApplyCameraRotation(vector3, Vector3.up) * Vector3.forward;
			newFwd = observer.MultiplyVector(newFwd);
			float num = vector2.y + vector2.x;
			Vector2 vector4 = Vector2.Min(vector - vector3, new Vector2(89.5f, 89.5f)) * 0.017453292f;
			return new Bounds(new Vector3(0f, 0f, num / 2f), new Vector3(Mathf.Tan(vector4.y) * num, Mathf.Tan(vector4.x) * num, vector2.y - vector2.x));
		}

		internal void UpgradeToCm3(CinemachineGroupFraming c)
		{
			c.FramingMode = (CinemachineGroupFraming.FramingModes)this.m_FramingMode;
			c.FramingSize = this.m_GroupFramingSize;
			c.Damping = this.m_FrameDamping;
			c.SizeAdjustment = (CinemachineGroupFraming.SizeAdjustmentModes)this.m_AdjustmentMode;
			c.LateralAdjustment = CinemachineGroupFraming.LateralAdjustmentModes.ChangeRotation;
			c.DollyRange = new Vector2(-this.m_MaxDollyIn, this.m_MaxDollyOut);
			c.FovRange = new Vector2(this.m_MinimumFOV, this.m_MaximumFOV);
			c.OrthoSizeRange = new Vector2(this.m_MinimumOrthoSize, this.m_MaximumOrthoSize);
		}

		[Tooltip("The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc.")]
		public float m_GroupFramingSize = 0.8f;

		[Tooltip("What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both")]
		public CinemachineGroupComposer.FramingMode m_FramingMode = CinemachineGroupComposer.FramingMode.HorizontalAndVertical;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to frame the group. Small numbers are more responsive, rapidly adjusting the camera to keep the group in the frame.  Larger numbers give a heavier more slowly responding camera.")]
		public float m_FrameDamping = 2f;

		[Tooltip("How to adjust the camera to get the desired framing.  You can zoom, dolly in/out, or do both.")]
		public CinemachineGroupComposer.AdjustmentMode m_AdjustmentMode;

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

		private float m_prevFramingDistance;

		private float m_prevFOV;

		public enum FramingMode
		{
			Horizontal,
			Vertical,
			HorizontalAndVertical
		}

		public enum AdjustmentMode
		{
			ZoomOnly,
			DollyOnly,
			DollyThenZoom
		}
	}
}
