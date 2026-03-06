using System;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineOrbitalTransposer has been deprecated. Use CinemachineOrbitalFollow instead")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	public class CinemachineOrbitalTransposer : CinemachineTransposer, AxisState.IRequiresInput
	{
		protected override void OnValidate()
		{
			if (this.m_LegacyRadius != 3.4028235E+38f && this.m_LegacyHeightOffset != 3.4028235E+38f && this.m_LegacyHeadingBias != 3.4028235E+38f)
			{
				this.m_FollowOffset = new Vector3(0f, this.m_LegacyHeightOffset, -this.m_LegacyRadius);
				this.m_LegacyHeightOffset = (this.m_LegacyRadius = float.MaxValue);
				this.m_Heading.m_Bias = this.m_LegacyHeadingBias;
				this.m_XAxis.m_MaxSpeed = this.m_XAxis.m_MaxSpeed / 10f;
				this.m_XAxis.m_AccelTime = this.m_XAxis.m_AccelTime / 10f;
				this.m_XAxis.m_DecelTime = this.m_XAxis.m_DecelTime / 10f;
				this.m_LegacyHeadingBias = float.MaxValue;
				int definition = (int)this.m_Heading.m_Definition;
				if (this.m_RecenterToTargetHeading.LegacyUpgrade(ref definition, ref this.m_Heading.m_VelocityFilterStrength))
				{
					this.m_Heading.m_Definition = (CinemachineOrbitalTransposer.Heading.HeadingDefinition)definition;
				}
			}
			this.m_XAxis.Validate();
			this.m_RecenterToTargetHeading.Validate();
			base.OnValidate();
		}

		public float UpdateHeading(float deltaTime, Vector3 up, ref AxisState axis)
		{
			return this.UpdateHeading(deltaTime, up, ref axis, ref this.m_RecenterToTargetHeading, true);
		}

		public float UpdateHeading(float deltaTime, Vector3 up, ref AxisState axis, ref AxisState.Recentering recentering, bool isLive)
		{
			if (this.m_BindingMode == BindingMode.LazyFollow)
			{
				axis.m_MinValue = -180f;
				axis.m_MaxValue = 180f;
			}
			if (deltaTime < 0f || !base.VirtualCamera.PreviousStateIsValid || !isLive)
			{
				axis.Reset();
				recentering.CancelRecentering();
			}
			else if (axis.Update(deltaTime))
			{
				recentering.CancelRecentering();
			}
			if (this.m_BindingMode == BindingMode.LazyFollow)
			{
				float value = axis.Value;
				axis.Value = 0f;
				return value;
			}
			CameraState vcamState = base.VcamState;
			float targetHeading = this.GetTargetHeading(axis.Value, this.m_TargetTracker.GetReferenceOrientation(this, this.m_BindingMode, up, ref vcamState));
			recentering.DoRecentering(ref axis, deltaTime, targetHeading);
			return axis.Value;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_PreviousTarget = null;
			this.m_LastTargetPosition = Vector3.zero;
			this.UpdateInputAxisProvider();
		}

		bool AxisState.IRequiresInput.RequiresInput()
		{
			return true;
		}

		internal void UpdateInputAxisProvider()
		{
			this.m_XAxis.SetInputAxisProvider(0, null);
			if (!this.m_HeadingIsDriven && base.VirtualCamera != null)
			{
				AxisState.IInputAxisProvider component = base.VirtualCamera.GetComponent<AxisState.IInputAxisProvider>();
				if (component != null)
				{
					this.m_XAxis.SetInputAxisProvider(0, component);
				}
			}
		}

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			base.OnTargetObjectWarped(target, positionDelta);
			if (target == base.FollowTarget)
			{
				this.m_LastTargetPosition += positionDelta;
				this.m_LastCameraPosition += positionDelta;
			}
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			base.ForceCameraPosition(pos, rot);
			this.m_LastCameraPosition = pos;
			this.m_XAxis.Value = this.GetAxisClosestValue(pos, base.VirtualCamera.State.ReferenceUp);
		}

		public override bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			this.m_RecenterToTargetHeading.DoRecentering(ref this.m_XAxis, -1f, 0f);
			this.m_RecenterToTargetHeading.CancelRecentering();
			if (fromCam != null && this.m_BindingMode != BindingMode.LazyFollow && (base.VirtualCamera.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && !CinemachineCore.IsLiveInBlend(base.VirtualCamera))
			{
				this.m_XAxis.Value = this.GetAxisClosestValue(fromCam.State.RawPosition, worldUp);
				return true;
			}
			return false;
		}

		public float GetAxisClosestValue(Vector3 cameraPos, Vector3 up)
		{
			CameraState vcamState = base.VcamState;
			Quaternion quaternion = this.m_TargetTracker.GetReferenceOrientation(this, this.m_BindingMode, up, ref vcamState);
			if (!(quaternion * Vector3.forward).ProjectOntoPlane(up).AlmostZero() && base.FollowTarget != null)
			{
				float num = 0f;
				if (this.m_BindingMode != BindingMode.LazyFollow)
				{
					num += this.m_Heading.m_Bias;
				}
				quaternion *= Quaternion.AngleAxis(num, up);
				Vector3 followTargetPosition = base.FollowTargetPosition;
				Vector3 from = (followTargetPosition + quaternion * base.EffectiveOffset - followTargetPosition).ProjectOntoPlane(up);
				Vector3 to = (cameraPos - followTargetPosition).ProjectOntoPlane(up);
				return Vector3.SignedAngle(from, to, up);
			}
			return this.m_LastHeading;
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			this.m_TargetTracker.InitStateInfo(this, deltaTime, this.m_BindingMode, curState.ReferenceUp);
			if (base.FollowTarget != this.m_PreviousTarget)
			{
				this.m_PreviousTarget = base.FollowTarget;
				this.m_TargetRigidBody = ((this.m_PreviousTarget == null) ? null : this.m_PreviousTarget.GetComponent<Rigidbody>());
				this.m_LastTargetPosition = ((this.m_PreviousTarget == null) ? Vector3.zero : this.m_PreviousTarget.position);
				this.mHeadingTracker = null;
			}
			this.m_LastHeading = this.HeadingUpdater(this, deltaTime, curState.ReferenceUp);
			float num = this.m_LastHeading;
			if (this.IsValid)
			{
				if (this.m_BindingMode != BindingMode.LazyFollow)
				{
					num += this.m_Heading.m_Bias;
				}
				Quaternion rotation = Quaternion.AngleAxis(num, Vector3.up);
				Vector3 effectiveOffset = base.EffectiveOffset;
				Vector3 vector = rotation * effectiveOffset;
				Vector3 referenceUp = curState.ReferenceUp;
				Vector3 desiredCameraOffset = vector;
				TrackerSettings trackerSettings = base.TrackerSettings;
				Vector3 vector2;
				Quaternion rotation2;
				this.m_TargetTracker.TrackTarget(this, deltaTime, referenceUp, desiredCameraOffset, trackerSettings, ref curState, out vector2, out rotation2);
				vector = rotation2 * vector;
				curState.ReferenceUp = rotation2 * Vector3.up;
				Vector3 followTargetPosition = base.FollowTargetPosition;
				vector2 += this.m_TargetTracker.GetOffsetForMinimumTargetDistance(this, vector2, vector, curState.RawOrientation * Vector3.forward, curState.ReferenceUp, followTargetPosition);
				curState.RawPosition = vector2 + vector;
				if (deltaTime >= 0f && base.VirtualCamera.PreviousStateIsValid)
				{
					Vector3 b = followTargetPosition;
					if (base.LookAtTarget != null)
					{
						b = base.LookAtTargetPosition;
					}
					Vector3 v = this.m_LastCameraPosition - b;
					Vector3 v2 = curState.RawPosition - b;
					if (v.sqrMagnitude > 0.01f && v2.sqrMagnitude > 0.01f)
					{
						curState.RotationDampingBypass *= UnityVectorExtensions.SafeFromToRotation(v, v2, curState.ReferenceUp);
					}
				}
				this.m_LastTargetPosition = followTargetPosition;
				this.m_LastCameraPosition = curState.RawPosition;
			}
		}

		internal override Vector3 GetTargetCameraPosition(Vector3 worldUp)
		{
			if (!this.IsValid)
			{
				return Vector3.zero;
			}
			float num = this.m_LastHeading;
			if (this.m_BindingMode != BindingMode.LazyFollow)
			{
				num += this.m_Heading.m_Bias;
			}
			CameraState vcamState = base.VcamState;
			Quaternion quaternion = Quaternion.AngleAxis(num, Vector3.up);
			quaternion = this.m_TargetTracker.GetReferenceOrientation(this, this.m_BindingMode, worldUp, ref vcamState) * quaternion;
			return quaternion * base.EffectiveOffset + this.m_LastTargetPosition;
		}

		private float GetTargetHeading(float currentHeading, Quaternion targetOrientation)
		{
			if (this.m_BindingMode == BindingMode.LazyFollow)
			{
				return 0f;
			}
			if (base.FollowTarget == null)
			{
				return currentHeading;
			}
			CinemachineOrbitalTransposer.Heading.HeadingDefinition headingDefinition = this.m_Heading.m_Definition;
			if (headingDefinition == CinemachineOrbitalTransposer.Heading.HeadingDefinition.Velocity && this.m_TargetRigidBody == null)
			{
				headingDefinition = CinemachineOrbitalTransposer.Heading.HeadingDefinition.PositionDelta;
			}
			Vector3 vector = Vector3.zero;
			switch (headingDefinition)
			{
			case CinemachineOrbitalTransposer.Heading.HeadingDefinition.PositionDelta:
				vector = base.FollowTargetPosition - this.m_LastTargetPosition;
				goto IL_98;
			case CinemachineOrbitalTransposer.Heading.HeadingDefinition.Velocity:
				vector = this.m_TargetRigidBody.linearVelocity;
				goto IL_98;
			case CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward:
				vector = base.FollowTargetRotation * Vector3.forward;
				goto IL_98;
			}
			return 0f;
			IL_98:
			Vector3 vector2 = targetOrientation * Vector3.up;
			vector = vector.ProjectOntoPlane(vector2);
			if (headingDefinition != CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward)
			{
				int num = this.m_Heading.m_VelocityFilterStrength * 5;
				if (this.mHeadingTracker == null || this.mHeadingTracker.FilterSize != num)
				{
					this.mHeadingTracker = new HeadingTracker(num);
				}
				this.mHeadingTracker.DecayHistory();
				if (!vector.AlmostZero())
				{
					this.mHeadingTracker.Add(vector);
				}
				vector = this.mHeadingTracker.GetReliableHeading();
			}
			if (!vector.AlmostZero())
			{
				return UnityVectorExtensions.SignedAngle(targetOrientation * Vector3.forward, vector, vector2);
			}
			return currentHeading;
		}

		internal void UpgradeToCm3(CinemachineOrbitalFollow c)
		{
			c.TrackerSettings = base.TrackerSettings;
			c.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
			c.Radius = -this.m_FollowOffset.z;
			c.HorizontalAxis.Range = new Vector2(this.m_XAxis.m_MinValue, this.m_XAxis.m_MaxValue);
			c.HorizontalAxis.Wrap = this.m_XAxis.m_Wrap;
			c.HorizontalAxis.Center = c.HorizontalAxis.ClampValue(0f);
			c.HorizontalAxis.Value = c.HorizontalAxis.ClampValue(this.m_XAxis.Value);
			c.HorizontalAxis.Recentering = new InputAxis.RecenteringSettings
			{
				Enabled = this.m_RecenterToTargetHeading.m_enabled,
				Time = this.m_RecenterToTargetHeading.m_RecenteringTime,
				Wait = this.m_RecenterToTargetHeading.m_WaitTime
			};
			c.VerticalAxis.Center = (c.VerticalAxis.Value = this.m_FollowOffset.y);
			c.VerticalAxis.Range = new Vector2(c.VerticalAxis.Center, c.VerticalAxis.Center);
			c.RadialAxis.Range = Vector2.one;
			c.RadialAxis.Center = (c.HorizontalAxis.Value = 1f);
		}

		[Space]
		[Tooltip("The definition of Forward.  Camera will follow behind.")]
		public CinemachineOrbitalTransposer.Heading m_Heading = new CinemachineOrbitalTransposer.Heading(CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward, 4, 0f);

		[Tooltip("Automatic heading recentering.  The settings here defines how the camera will reposition itself in the absence of player input.")]
		public AxisState.Recentering m_RecenterToTargetHeading = new AxisState.Recentering(true, 1f, 2f);

		[Tooltip("Heading Control.  The settings here control the behaviour of the camera in response to the player's input.")]
		public AxisState m_XAxis = new AxisState(-180f, 180f, true, false, 300f, 0.1f, 0.1f, "Mouse X", true);

		private Tracker m_TargetTracker;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("m_Radius")]
		private float m_LegacyRadius = float.MaxValue;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("m_HeightOffset")]
		private float m_LegacyHeightOffset = float.MaxValue;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("m_HeadingBias")]
		private float m_LegacyHeadingBias = float.MaxValue;

		[FormerlySerializedAs("m_HeadingIsSlave")]
		[HideInInspector]
		[NoSaveDuringPlay]
		public bool m_HeadingIsDriven;

		internal CinemachineOrbitalTransposer.UpdateHeadingDelegate HeadingUpdater = (CinemachineOrbitalTransposer orbital, float deltaTime, Vector3 up) => orbital.UpdateHeading(deltaTime, up, ref orbital.m_XAxis, ref orbital.m_RecenterToTargetHeading, CinemachineCore.IsLive(orbital.VirtualCamera));

		private Vector3 m_LastTargetPosition = Vector3.zero;

		private HeadingTracker mHeadingTracker;

		private Rigidbody m_TargetRigidBody;

		private Transform m_PreviousTarget;

		private Vector3 m_LastCameraPosition;

		private float m_LastHeading;

		[Serializable]
		public struct Heading
		{
			public Heading(CinemachineOrbitalTransposer.Heading.HeadingDefinition def, int filterStrength, float bias)
			{
				this.m_Definition = def;
				this.m_VelocityFilterStrength = filterStrength;
				this.m_Bias = bias;
			}

			[FormerlySerializedAs("m_HeadingDefinition")]
			[Tooltip("How 'forward' is defined.  The camera will be placed by default behind the target.  PositionDelta will consider 'forward' to be the direction in which the target is moving.")]
			public CinemachineOrbitalTransposer.Heading.HeadingDefinition m_Definition;

			[Range(0f, 10f)]
			[Tooltip("Size of the velocity sampling window for target heading filter.  This filters out irregularities in the target's movement.  Used only if deriving heading from target's movement (PositionDelta or Velocity)")]
			public int m_VelocityFilterStrength;

			[Range(-180f, 180f)]
			[FormerlySerializedAs("m_HeadingBias")]
			[Tooltip("Where the camera is placed when the X-axis value is zero.  This is a rotation in degrees around the Y axis.  When this value is 0, the camera will be placed behind the target.  Nonzero offsets will rotate the zero position around the target.")]
			public float m_Bias;

			public enum HeadingDefinition
			{
				PositionDelta,
				Velocity,
				TargetForward,
				WorldForward
			}
		}

		internal delegate float UpdateHeadingDelegate(CinemachineOrbitalTransposer orbital, float deltaTime, Vector3 up);
	}
}
