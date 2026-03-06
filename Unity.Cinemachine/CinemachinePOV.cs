using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachinePOV has been deprecated. Use CinemachinePanTilt instead")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[CameraPipeline(CinemachineCore.Stage.Aim)]
	public class CinemachinePOV : CinemachineComponentBase, CinemachineFreeLookModifier.IModifierValueSource, AxisState.IRequiresInput
	{
		float CinemachineFreeLookModifier.IModifierValueSource.NormalizedModifierValue
		{
			get
			{
				float num = this.m_VerticalAxis.m_MaxValue - this.m_VerticalAxis.m_MinValue;
				return (this.m_VerticalAxis.Value - this.m_VerticalAxis.m_MinValue) / ((num > 0.001f) ? num : 1f) * 2f - 1f;
			}
		}

		public override bool IsValid
		{
			get
			{
				return base.enabled;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Aim;
			}
		}

		private void OnValidate()
		{
			this.m_VerticalAxis.Validate();
			this.m_VerticalRecentering.Validate();
			this.m_HorizontalAxis.Validate();
			this.m_HorizontalRecentering.Validate();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateInputAxisProvider();
		}

		bool AxisState.IRequiresInput.RequiresInput()
		{
			return true;
		}

		internal void UpdateInputAxisProvider()
		{
			this.m_HorizontalAxis.SetInputAxisProvider(0, null);
			this.m_VerticalAxis.SetInputAxisProvider(1, null);
			if (base.VirtualCamera != null)
			{
				AxisState.IInputAxisProvider component = base.VirtualCamera.GetComponent<AxisState.IInputAxisProvider>();
				if (component != null)
				{
					this.m_HorizontalAxis.SetInputAxisProvider(0, component);
					this.m_VerticalAxis.SetInputAxisProvider(1, component);
				}
			}
		}

		public override void PrePipelineMutateCameraState(ref CameraState state, float deltaTime)
		{
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (!this.IsValid)
			{
				return;
			}
			if (deltaTime >= 0f && (!base.VirtualCamera.PreviousStateIsValid || !CinemachineCore.IsLive(base.VirtualCamera)))
			{
				deltaTime = -1f;
			}
			if (deltaTime >= 0f)
			{
				if (this.m_HorizontalAxis.Update(deltaTime))
				{
					this.m_HorizontalRecentering.CancelRecentering();
				}
				if (this.m_VerticalAxis.Update(deltaTime))
				{
					this.m_VerticalRecentering.CancelRecentering();
				}
			}
			Vector2 recenterTarget = this.GetRecenterTarget();
			this.m_HorizontalRecentering.DoRecentering(ref this.m_HorizontalAxis, deltaTime, recenterTarget.x);
			this.m_VerticalRecentering.DoRecentering(ref this.m_VerticalAxis, deltaTime, recenterTarget.y);
			Quaternion quaternion = Quaternion.Euler(this.m_VerticalAxis.Value, this.m_HorizontalAxis.Value, 0f);
			Transform parent = base.VirtualCamera.transform.parent;
			if (parent != null)
			{
				quaternion = parent.rotation * quaternion;
			}
			else
			{
				quaternion = Quaternion.FromToRotation(Vector3.up, curState.ReferenceUp) * quaternion;
			}
			curState.RawOrientation = quaternion;
			if (base.VirtualCamera.PreviousStateIsValid)
			{
				curState.RotationDampingBypass *= UnityVectorExtensions.SafeFromToRotation(this.m_PreviousCameraRotation * Vector3.forward, quaternion * Vector3.forward, curState.ReferenceUp);
			}
			this.m_PreviousCameraRotation = quaternion;
		}

		public Vector2 GetRecenterTarget()
		{
			Transform transform = null;
			CinemachinePOV.RecenterTargetMode recenterTarget = this.m_RecenterTarget;
			if (recenterTarget != CinemachinePOV.RecenterTargetMode.FollowTargetForward)
			{
				if (recenterTarget == CinemachinePOV.RecenterTargetMode.LookAtTargetForward)
				{
					transform = base.VirtualCamera.LookAt;
				}
			}
			else
			{
				transform = base.VirtualCamera.Follow;
			}
			if (transform != null)
			{
				Vector3 vector = transform.forward;
				Transform parent = base.VirtualCamera.transform.parent;
				if (parent != null)
				{
					vector = parent.rotation * vector;
				}
				Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.forward, vector).eulerAngles;
				return new Vector2(CinemachinePOV.NormalizeAngle(eulerAngles.y), CinemachinePOV.NormalizeAngle(eulerAngles.x));
			}
			return Vector2.zero;
		}

		private static float NormalizeAngle(float angle)
		{
			return (angle + 180f) % 360f - 180f;
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			this.SetAxesForRotation(rot);
		}

		public override bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			this.m_HorizontalRecentering.DoRecentering(ref this.m_HorizontalAxis, -1f, 0f);
			this.m_VerticalRecentering.DoRecentering(ref this.m_VerticalAxis, -1f, 0f);
			this.m_HorizontalRecentering.CancelRecentering();
			this.m_VerticalRecentering.CancelRecentering();
			if (fromCam != null && (base.VirtualCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && !CinemachineCore.IsLiveInBlend(base.VirtualCamera))
			{
				this.SetAxesForRotation(fromCam.State.RawOrientation);
				return true;
			}
			return false;
		}

		private void SetAxesForRotation(Quaternion targetRot)
		{
			Vector3 referenceUp = base.VcamState.ReferenceUp;
			Vector3 vector = Vector3.forward;
			Transform parent = base.VirtualCamera.transform.parent;
			if (parent != null)
			{
				vector = parent.rotation * vector;
			}
			this.m_HorizontalAxis.Value = 0f;
			this.m_HorizontalAxis.Reset();
			Vector3 vector2 = targetRot * Vector3.forward;
			Vector3 vector3 = vector.ProjectOntoPlane(referenceUp);
			Vector3 vector4 = vector2.ProjectOntoPlane(referenceUp);
			if (!vector3.AlmostZero() && !vector4.AlmostZero())
			{
				this.m_HorizontalAxis.Value = Vector3.SignedAngle(vector3, vector4, referenceUp);
			}
			this.m_VerticalAxis.Value = 0f;
			this.m_VerticalAxis.Reset();
			vector = Quaternion.AngleAxis(this.m_HorizontalAxis.Value, referenceUp) * vector;
			Vector3 vector5 = Vector3.Cross(referenceUp, vector);
			if (!vector5.AlmostZero())
			{
				this.m_VerticalAxis.Value = Vector3.SignedAngle(vector, vector2, vector5);
			}
		}

		internal void UpgradeToCm3(CinemachinePanTilt c)
		{
			c.ReferenceFrame = CinemachinePanTilt.ReferenceFrames.ParentObject;
			c.RecenterTarget = (CinemachinePanTilt.RecenterTargetModes)this.m_RecenterTarget;
			c.PanAxis.Range = new Vector2(this.m_HorizontalAxis.m_MinValue, this.m_HorizontalAxis.m_MaxValue);
			c.PanAxis.Center = 0f;
			c.PanAxis.Recentering = new InputAxis.RecenteringSettings
			{
				Enabled = this.m_HorizontalRecentering.m_enabled,
				Time = this.m_HorizontalRecentering.m_RecenteringTime,
				Wait = this.m_HorizontalRecentering.m_WaitTime
			};
			c.TiltAxis.Range = new Vector2(this.m_VerticalAxis.m_MinValue, this.m_VerticalAxis.m_MaxValue);
			c.TiltAxis.Center = 0f;
			c.TiltAxis.Recentering = new InputAxis.RecenteringSettings
			{
				Enabled = this.m_VerticalRecentering.m_enabled,
				Time = this.m_VerticalRecentering.m_RecenteringTime,
				Wait = this.m_VerticalRecentering.m_WaitTime
			};
		}

		public CinemachinePOV.RecenterTargetMode m_RecenterTarget;

		[Tooltip("The Vertical axis.  Value is -90..90. Controls the vertical orientation")]
		public AxisState m_VerticalAxis = new AxisState(-70f, 70f, false, false, 300f, 0.1f, 0.1f, "Mouse Y", true);

		[Tooltip("Controls how automatic recentering of the Vertical axis is accomplished")]
		public AxisState.Recentering m_VerticalRecentering = new AxisState.Recentering(false, 1f, 2f);

		[Tooltip("The Horizontal axis.  Value is -180..180.  Controls the horizontal orientation")]
		public AxisState m_HorizontalAxis = new AxisState(-180f, 180f, true, false, 300f, 0.1f, 0.1f, "Mouse X", false);

		[Tooltip("Controls how automatic recentering of the Horizontal axis is accomplished")]
		public AxisState.Recentering m_HorizontalRecentering = new AxisState.Recentering(false, 1f, 2f);

		[HideInInspector]
		[Tooltip("Obsolete - no longer used")]
		public bool m_ApplyBeforeBody;

		private Quaternion m_PreviousCameraRotation;

		public enum RecenterTargetMode
		{
			None,
			FollowTargetForward,
			LookAtTargetForward
		}
	}
}
