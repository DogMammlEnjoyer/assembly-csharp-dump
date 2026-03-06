using System;
using System.Collections.Generic;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;

namespace UnityEngine.XR.LegacyInputHelpers
{
	public class ArmModel : BasePoseProvider
	{
		public override PoseDataFlags GetPoseFromProvider(out Pose output)
		{
			if (this.OnControllerInputUpdated())
			{
				output = this.finalPose;
				return PoseDataFlags.Position | PoseDataFlags.Rotation;
			}
			output = Pose.identity;
			return PoseDataFlags.NoData;
		}

		public Pose finalPose
		{
			get
			{
				return this.m_FinalPose;
			}
			set
			{
				this.m_FinalPose = value;
			}
		}

		public XRNode poseSource
		{
			get
			{
				return this.m_PoseSource;
			}
			set
			{
				this.m_PoseSource = value;
			}
		}

		public XRNode headGameObject
		{
			get
			{
				return this.m_HeadPoseSource;
			}
			set
			{
				this.m_HeadPoseSource = value;
			}
		}

		public Vector3 elbowRestPosition
		{
			get
			{
				return this.m_ElbowRestPosition;
			}
			set
			{
				this.m_ElbowRestPosition = value;
			}
		}

		public Vector3 wristRestPosition
		{
			get
			{
				return this.m_WristRestPosition;
			}
			set
			{
				this.m_WristRestPosition = value;
			}
		}

		public Vector3 controllerRestPosition
		{
			get
			{
				return this.m_ControllerRestPosition;
			}
			set
			{
				this.m_ControllerRestPosition = value;
			}
		}

		public Vector3 armExtensionOffset
		{
			get
			{
				return this.m_ArmExtensionOffset;
			}
			set
			{
				this.m_ArmExtensionOffset = value;
			}
		}

		public float elbowBendRatio
		{
			get
			{
				return this.m_ElbowBendRatio;
			}
			set
			{
				this.m_ElbowBendRatio = value;
			}
		}

		public bool isLockedToNeck
		{
			get
			{
				return this.m_IsLockedToNeck;
			}
			set
			{
				this.m_IsLockedToNeck = value;
			}
		}

		public Vector3 neckPosition
		{
			get
			{
				return this.m_NeckPosition;
			}
		}

		public Vector3 shoulderPosition
		{
			get
			{
				return this.m_NeckPosition + this.m_TorsoRotation * Vector3.Scale(ArmModel.SHOULDER_POSITION, this.m_HandedMultiplier);
			}
		}

		public Quaternion shoulderRotation
		{
			get
			{
				return this.m_TorsoRotation;
			}
		}

		public Vector3 elbowPosition
		{
			get
			{
				return this.m_ElbowPosition;
			}
		}

		public Quaternion elbowRotation
		{
			get
			{
				return this.m_ElbowRotation;
			}
		}

		public Vector3 wristPosition
		{
			get
			{
				return this.m_WristPosition;
			}
		}

		public Quaternion wristRotation
		{
			get
			{
				return this.m_WristRotation;
			}
		}

		public Vector3 controllerPosition
		{
			get
			{
				return this.m_ControllerPosition;
			}
		}

		public Quaternion controllerRotation
		{
			get
			{
				return this.m_ControllerRotation;
			}
		}

		protected virtual void OnEnable()
		{
			this.UpdateTorsoDirection(true);
			this.OnControllerInputUpdated();
		}

		protected virtual void OnDisable()
		{
		}

		public virtual bool OnControllerInputUpdated()
		{
			this.UpdateHandedness();
			return this.UpdateTorsoDirection(false) && this.UpdateNeckPosition() && this.ApplyArmModel();
		}

		protected virtual void UpdateHandedness()
		{
			this.m_HandedMultiplier.Set(0f, 1f, 1f);
			if (this.m_PoseSource == XRNode.RightHand || this.m_PoseSource == XRNode.TrackingReference)
			{
				this.m_HandedMultiplier.x = 1f;
				return;
			}
			if (this.m_PoseSource == XRNode.LeftHand)
			{
				this.m_HandedMultiplier.x = -1f;
			}
		}

		protected virtual bool UpdateTorsoDirection(bool forceImmediate)
		{
			Vector3 vector = default(Vector3);
			if (this.TryGetForwardVector(this.m_HeadPoseSource, out vector))
			{
				vector.y = 0f;
				vector.Normalize();
				Vector3 vector2;
				if (forceImmediate)
				{
					this.m_TorsoDirection = vector;
				}
				else if (this.TryGetAngularAcceleration(this.poseSource, out vector2))
				{
					float t = Mathf.Clamp((vector2.magnitude - 0.2f) / 45f, 0f, 0.1f);
					this.m_TorsoDirection = Vector3.Slerp(this.m_TorsoDirection, vector, t);
				}
				this.m_TorsoRotation = Quaternion.FromToRotation(Vector3.forward, this.m_TorsoDirection);
				return true;
			}
			return false;
		}

		protected virtual bool UpdateNeckPosition()
		{
			if (this.m_IsLockedToNeck && this.TryGetPosition(this.m_HeadPoseSource, out this.m_NeckPosition))
			{
				return this.ApplyInverseNeckModel(this.m_NeckPosition, out this.m_NeckPosition);
			}
			this.m_NeckPosition = Vector3.zero;
			return true;
		}

		protected virtual bool ApplyArmModel()
		{
			this.SetUntransformedJointPositions();
			Quaternion controllerOrientation;
			Quaternion xyRotation;
			float xAngle;
			if (this.GetControllerRotation(out controllerOrientation, out xyRotation, out xAngle))
			{
				float extensionRatio = this.CalculateExtensionRatio(xAngle);
				this.ApplyExtensionOffset(extensionRatio);
				Quaternion lerpRotation = this.CalculateLerpRotation(xyRotation, extensionRatio);
				this.CalculateFinalJointRotations(controllerOrientation, xyRotation, lerpRotation);
				this.ApplyRotationToJoints();
				this.m_FinalPose.position = this.m_ControllerPosition;
				this.m_FinalPose.rotation = this.m_ControllerRotation;
				return true;
			}
			return false;
		}

		protected virtual void SetUntransformedJointPositions()
		{
			this.m_ElbowPosition = Vector3.Scale(this.m_ElbowRestPosition, this.m_HandedMultiplier);
			this.m_WristPosition = Vector3.Scale(this.m_WristRestPosition, this.m_HandedMultiplier);
			this.m_ControllerPosition = Vector3.Scale(this.m_ControllerRestPosition, this.m_HandedMultiplier);
		}

		protected virtual float CalculateExtensionRatio(float xAngle)
		{
			return Mathf.Clamp((xAngle - 7f) / 53f, 0f, 1f);
		}

		protected virtual void ApplyExtensionOffset(float extensionRatio)
		{
			Vector3 a = Vector3.Scale(this.m_ArmExtensionOffset, this.m_HandedMultiplier);
			this.m_ElbowPosition += a * extensionRatio;
		}

		protected virtual Quaternion CalculateLerpRotation(Quaternion xyRotation, float extensionRatio)
		{
			float num = Quaternion.Angle(xyRotation, Quaternion.identity);
			float num2 = 1f - Mathf.Pow(num / 180f, 6f);
			float num3 = 1f - this.m_ElbowBendRatio + this.m_ElbowBendRatio * extensionRatio * 0.4f;
			num3 *= num2;
			return Quaternion.Lerp(Quaternion.identity, xyRotation, num3);
		}

		protected virtual void CalculateFinalJointRotations(Quaternion controllerOrientation, Quaternion xyRotation, Quaternion lerpRotation)
		{
			this.m_ElbowRotation = this.m_TorsoRotation * Quaternion.Inverse(lerpRotation) * xyRotation;
			this.m_WristRotation = this.m_ElbowRotation * lerpRotation;
			this.m_ControllerRotation = this.m_TorsoRotation * controllerOrientation;
		}

		protected virtual void ApplyRotationToJoints()
		{
			this.m_ElbowPosition = this.m_NeckPosition + this.m_TorsoRotation * this.m_ElbowPosition;
			this.m_WristPosition = this.m_ElbowPosition + this.m_ElbowRotation * this.m_WristPosition;
			this.m_ControllerPosition = this.m_WristPosition + this.m_WristRotation * this.m_ControllerPosition;
		}

		protected virtual bool ApplyInverseNeckModel(Vector3 headPosition, out Vector3 calculatedPosition)
		{
			Quaternion rotation = default(Quaternion);
			if (this.TryGetRotation(this.m_HeadPoseSource, out rotation))
			{
				Vector3 b = rotation * ArmModel.NECK_OFFSET - ArmModel.NECK_OFFSET.y * Vector3.up;
				headPosition -= b;
				calculatedPosition = headPosition;
				return true;
			}
			calculatedPosition = Vector3.zero;
			return false;
		}

		protected bool TryGetForwardVector(XRNode node, out Vector3 forward)
		{
			Pose pose = default(Pose);
			if (this.TryGetRotation(node, out pose.rotation) && this.TryGetPosition(node, out pose.position))
			{
				forward = pose.forward;
				return true;
			}
			forward = Vector3.zero;
			return false;
		}

		protected bool TryGetRotation(XRNode node, out Quaternion rotation)
		{
			InputTracking.GetNodeStates(this.xrNodeStateListOrientation);
			int count = this.xrNodeStateListOrientation.Count;
			for (int i = 0; i < count; i++)
			{
				XRNodeState xrnodeState = this.xrNodeStateListOrientation[i];
				if (xrnodeState.nodeType == node && xrnodeState.TryGetRotation(out rotation))
				{
					return true;
				}
			}
			rotation = Quaternion.identity;
			return false;
		}

		protected bool TryGetPosition(XRNode node, out Vector3 position)
		{
			InputTracking.GetNodeStates(this.xrNodeStateListPosition);
			int count = this.xrNodeStateListPosition.Count;
			for (int i = 0; i < count; i++)
			{
				XRNodeState xrnodeState = this.xrNodeStateListPosition[i];
				if (xrnodeState.nodeType == node && xrnodeState.TryGetPosition(out position))
				{
					return true;
				}
			}
			position = Vector3.zero;
			return false;
		}

		protected bool TryGetAngularAcceleration(XRNode node, out Vector3 angularAccel)
		{
			InputTracking.GetNodeStates(this.xrNodeStateListAngularAcceleration);
			int count = this.xrNodeStateListAngularAcceleration.Count;
			for (int i = 0; i < count; i++)
			{
				XRNodeState xrnodeState = this.xrNodeStateListAngularAcceleration[i];
				if (xrnodeState.nodeType == node && xrnodeState.TryGetAngularAcceleration(out angularAccel))
				{
					return true;
				}
			}
			angularAccel = Vector3.zero;
			return false;
		}

		protected bool TryGetAngularVelocity(XRNode node, out Vector3 angVel)
		{
			InputTracking.GetNodeStates(this.xrNodeStateListAngularVelocity);
			int count = this.xrNodeStateListAngularVelocity.Count;
			for (int i = 0; i < count; i++)
			{
				XRNodeState xrnodeState = this.xrNodeStateListAngularVelocity[i];
				if (xrnodeState.nodeType == node && xrnodeState.TryGetAngularVelocity(out angVel))
				{
					return true;
				}
			}
			angVel = Vector3.zero;
			return false;
		}

		protected bool GetControllerRotation(out Quaternion rotation, out Quaternion xyRotation, out float xAngle)
		{
			if (this.TryGetRotation(this.poseSource, out rotation))
			{
				rotation = Quaternion.Inverse(this.m_TorsoRotation) * rotation;
				Vector3 vector = rotation * Vector3.forward;
				xAngle = 90f - Vector3.Angle(vector, Vector3.up);
				xyRotation = Quaternion.FromToRotation(Vector3.forward, vector);
				return true;
			}
			rotation = Quaternion.identity;
			xyRotation = Quaternion.identity;
			xAngle = 0f;
			return false;
		}

		private Pose m_FinalPose;

		[SerializeField]
		private XRNode m_PoseSource = XRNode.LeftHand;

		[SerializeField]
		private XRNode m_HeadPoseSource = XRNode.CenterEye;

		[SerializeField]
		private Vector3 m_ElbowRestPosition = ArmModel.DEFAULT_ELBOW_REST_POSITION;

		[SerializeField]
		private Vector3 m_WristRestPosition = ArmModel.DEFAULT_WRIST_REST_POSITION;

		[SerializeField]
		private Vector3 m_ControllerRestPosition = ArmModel.DEFAULT_CONTROLLER_REST_POSITION;

		[SerializeField]
		private Vector3 m_ArmExtensionOffset = ArmModel.DEFAULT_ARM_EXTENSION_OFFSET;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_ElbowBendRatio = 0.6f;

		[SerializeField]
		private bool m_IsLockedToNeck = true;

		protected Vector3 m_NeckPosition;

		protected Vector3 m_ElbowPosition;

		protected Quaternion m_ElbowRotation;

		protected Vector3 m_WristPosition;

		protected Quaternion m_WristRotation;

		protected Vector3 m_ControllerPosition;

		protected Quaternion m_ControllerRotation;

		protected Vector3 m_HandedMultiplier;

		protected Vector3 m_TorsoDirection;

		protected Quaternion m_TorsoRotation;

		protected static readonly Vector3 DEFAULT_ELBOW_REST_POSITION = new Vector3(0.195f, -0.5f, 0.005f);

		protected static readonly Vector3 DEFAULT_WRIST_REST_POSITION = new Vector3(0f, 0f, 0.25f);

		protected static readonly Vector3 DEFAULT_CONTROLLER_REST_POSITION = new Vector3(0f, 0f, 0.05f);

		protected static readonly Vector3 DEFAULT_ARM_EXTENSION_OFFSET = new Vector3(-0.13f, 0.14f, 0.08f);

		protected const float DEFAULT_ELBOW_BEND_RATIO = 0.6f;

		protected const float EXTENSION_WEIGHT = 0.4f;

		protected static readonly Vector3 SHOULDER_POSITION = new Vector3(0.17f, -0.2f, -0.03f);

		protected static readonly Vector3 NECK_OFFSET = new Vector3(0f, 0.075f, 0.08f);

		protected const float MIN_EXTENSION_ANGLE = 7f;

		protected const float MAX_EXTENSION_ANGLE = 60f;

		private List<XRNodeState> xrNodeStateListOrientation = new List<XRNodeState>();

		private List<XRNodeState> xrNodeStateListPosition = new List<XRNodeState>();

		private List<XRNodeState> xrNodeStateListAngularAcceleration = new List<XRNodeState>();

		private List<XRNodeState> xrNodeStateListAngularVelocity = new List<XRNodeState>();
	}
}
