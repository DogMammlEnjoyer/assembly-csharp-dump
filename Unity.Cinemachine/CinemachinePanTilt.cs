using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Rotation Control/Cinemachine Pan Tilt")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Aim)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePanTilt.html")]
	public class CinemachinePanTilt : CinemachineComponentBase, IInputAxisOwner, IInputAxisResetSource, CinemachineFreeLookModifier.IModifierValueSource
	{
		private void OnValidate()
		{
			this.PanAxis.Validate();
			this.TiltAxis.Range.x = Mathf.Clamp(this.TiltAxis.Range.x, -90f, 90f);
			this.TiltAxis.Range.y = Mathf.Clamp(this.TiltAxis.Range.y, -90f, 90f);
			this.TiltAxis.Validate();
		}

		private void Reset()
		{
			this.PanAxis = CinemachinePanTilt.DefaultPan;
			this.TiltAxis = CinemachinePanTilt.DefaultTilt;
			this.ReferenceFrame = CinemachinePanTilt.ReferenceFrames.ParentObject;
			this.RecenterTarget = CinemachinePanTilt.RecenterTargetModes.AxisCenter;
		}

		private static InputAxis DefaultPan
		{
			get
			{
				return new InputAxis
				{
					Value = 0f,
					Range = new Vector2(-180f, 180f),
					Wrap = true,
					Center = 0f,
					Recentering = InputAxis.RecenteringSettings.Default
				};
			}
		}

		private static InputAxis DefaultTilt
		{
			get
			{
				return new InputAxis
				{
					Value = 0f,
					Range = new Vector2(-70f, 70f),
					Wrap = false,
					Center = 0f,
					Recentering = InputAxis.RecenteringSettings.Default
				};
			}
		}

		void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
		{
			axes.Add(new IInputAxisOwner.AxisDescriptor
			{
				DrivenAxis = (() => ref this.PanAxis),
				Name = "Look X (Pan)",
				Hint = IInputAxisOwner.AxisDescriptor.Hints.X
			});
			axes.Add(new IInputAxisOwner.AxisDescriptor
			{
				DrivenAxis = (() => ref this.TiltAxis),
				Name = "Look Y (Tilt)",
				Hint = IInputAxisOwner.AxisDescriptor.Hints.Y
			});
		}

		void IInputAxisResetSource.RegisterResetHandler(Action handler)
		{
			this.m_ResetHandler = (Action)Delegate.Combine(this.m_ResetHandler, handler);
		}

		void IInputAxisResetSource.UnregisterResetHandler(Action handler)
		{
			this.m_ResetHandler = (Action)Delegate.Remove(this.m_ResetHandler, handler);
		}

		float CinemachineFreeLookModifier.IModifierValueSource.NormalizedModifierValue
		{
			get
			{
				float num = this.TiltAxis.Range.y - this.TiltAxis.Range.x;
				return (this.TiltAxis.Value - this.TiltAxis.Range.x) / ((num > 0.001f) ? num : 1f) * 2f - 1f;
			}
		}

		bool IInputAxisResetSource.HasResetHandler
		{
			get
			{
				return this.m_ResetHandler != null;
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

		public override void PrePipelineMutateCameraState(ref CameraState state, float deltaTime)
		{
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (!this.IsValid)
			{
				return;
			}
			if (deltaTime < 0f || !base.VirtualCamera.PreviousStateIsValid || !CinemachineCore.IsLive(base.VirtualCamera))
			{
				Action resetHandler = this.m_ResetHandler;
				if (resetHandler != null)
				{
					resetHandler();
				}
			}
			Quaternion quaternion = this.GetReferenceFrame(curState.ReferenceUp) * Quaternion.Euler(this.TiltAxis.Value, this.PanAxis.Value, 0f);
			curState.RawOrientation = quaternion;
			if (base.VirtualCamera.PreviousStateIsValid)
			{
				curState.RotationDampingBypass *= UnityVectorExtensions.SafeFromToRotation(this.m_PreviousCameraRotation * Vector3.forward, quaternion * Vector3.forward, curState.ReferenceUp);
			}
			this.m_PreviousCameraRotation = quaternion;
			bool flag = this.PanAxis.TrackValueChange();
			bool flag2 = this.TiltAxis.TrackValueChange();
			if (this.PanAxis.Recentering.Time == this.TiltAxis.Recentering.Time)
			{
				flag = (flag || flag2);
				flag2 = (flag2 || flag);
			}
			Vector2 recenterTarget = this.GetRecenterTarget();
			this.PanAxis.UpdateRecentering(deltaTime, flag, recenterTarget.x);
			this.TiltAxis.UpdateRecentering(deltaTime, flag2, recenterTarget.y);
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			this.SetAxesForRotation(rot);
		}

		public override bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			Action resetHandler = this.m_ResetHandler;
			if (resetHandler != null)
			{
				resetHandler();
			}
			if (fromCam != null && (base.VirtualCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && !CinemachineCore.IsLiveInBlend(base.VirtualCamera))
			{
				this.SetAxesForRotation(fromCam.State.RawOrientation);
				return true;
			}
			return false;
		}

		private void SetAxesForRotation(Quaternion targetRot)
		{
			Action resetHandler = this.m_ResetHandler;
			if (resetHandler != null)
			{
				resetHandler();
			}
			Vector3 referenceUp = base.VcamState.ReferenceUp;
			Vector3 vector = this.GetReferenceFrame(referenceUp) * Vector3.forward;
			this.PanAxis.Value = 0f;
			Vector3 vector2 = targetRot * Vector3.forward;
			Vector3 vector3 = vector.ProjectOntoPlane(referenceUp);
			Vector3 vector4 = vector2.ProjectOntoPlane(referenceUp);
			if (!vector3.AlmostZero() && !vector4.AlmostZero())
			{
				this.PanAxis.Value = Vector3.SignedAngle(vector3, vector4, referenceUp);
			}
			this.TiltAxis.Value = 0f;
			vector = Quaternion.AngleAxis(this.PanAxis.Value, referenceUp) * vector;
			Vector3 vector5 = Vector3.Cross(referenceUp, vector);
			if (!vector5.AlmostZero())
			{
				this.TiltAxis.Value = Vector3.SignedAngle(vector, vector2, vector5);
			}
		}

		private Quaternion GetReferenceFrame(Vector3 up)
		{
			Transform transform = null;
			switch (this.ReferenceFrame)
			{
			case CinemachinePanTilt.ReferenceFrames.ParentObject:
				transform = base.VirtualCamera.transform.parent;
				break;
			case CinemachinePanTilt.ReferenceFrames.TrackingTarget:
				transform = base.FollowTarget;
				break;
			case CinemachinePanTilt.ReferenceFrames.LookAtTarget:
				transform = base.LookAtTarget;
				break;
			}
			if (!(transform != null))
			{
				return Quaternion.FromToRotation(Vector3.up, up);
			}
			return transform.rotation;
		}

		public Vector2 GetRecenterTarget()
		{
			Transform transform = null;
			CinemachinePanTilt.RecenterTargetModes recenterTarget = this.RecenterTarget;
			if (recenterTarget != CinemachinePanTilt.RecenterTargetModes.TrackingTargetForward)
			{
				if (recenterTarget == CinemachinePanTilt.RecenterTargetModes.LookAtTargetForward)
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
				return new Vector2(CinemachinePanTilt.<GetRecenterTarget>g__NormalizeAngle|31_0(eulerAngles.y), CinemachinePanTilt.<GetRecenterTarget>g__NormalizeAngle|31_0(eulerAngles.x));
			}
			return new Vector2(this.PanAxis.Center, this.TiltAxis.Center);
		}

		[CompilerGenerated]
		internal static float <GetRecenterTarget>g__NormalizeAngle|31_0(float angle)
		{
			return (angle + 180f) % 360f - 180f;
		}

		public CinemachinePanTilt.ReferenceFrames ReferenceFrame;

		public CinemachinePanTilt.RecenterTargetModes RecenterTarget;

		[Tooltip("Axis representing the current horizontal rotation.  Value is in degrees and represents a rotation about the Y axis.")]
		public InputAxis PanAxis = CinemachinePanTilt.DefaultPan;

		[Tooltip("Axis representing the current vertical rotation.  Value is in degrees and represents a rotation about the X axis.")]
		public InputAxis TiltAxis = CinemachinePanTilt.DefaultTilt;

		private Quaternion m_PreviousCameraRotation;

		private Action m_ResetHandler;

		public enum ReferenceFrames
		{
			ParentObject,
			World,
			TrackingTarget,
			LookAtTarget
		}

		public enum RecenterTargetModes
		{
			AxisCenter,
			TrackingTargetForward,
			LookAtTargetForward
		}
	}
}
