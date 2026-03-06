using System;

namespace UnityEngine.XR.LegacyInputHelpers
{
	public class SwingArmModel : ArmModel
	{
		public float shoulderRotationRatio
		{
			get
			{
				return this.m_ShoulderRotationRatio;
			}
			set
			{
				this.m_ShoulderRotationRatio = value;
			}
		}

		public float elbowRotationRatio
		{
			get
			{
				return this.m_ElbowRotationRatio;
			}
			set
			{
				this.m_ElbowRotationRatio = value;
			}
		}

		public float wristRotationRatio
		{
			get
			{
				return this.m_WristRotationRatio;
			}
			set
			{
				this.m_WristRotationRatio = value;
			}
		}

		public float minJointShiftAngle
		{
			get
			{
				return this.m_JointShiftAngle.x;
			}
			set
			{
				this.m_JointShiftAngle.x = value;
			}
		}

		public float maxJointShiftAngle
		{
			get
			{
				return this.m_JointShiftAngle.y;
			}
			set
			{
				this.m_JointShiftAngle.y = value;
			}
		}

		public float jointShiftExponent
		{
			get
			{
				return this.m_JointShiftExponent;
			}
			set
			{
				this.m_JointShiftExponent = value;
			}
		}

		public float shiftedShoulderRotationRatio
		{
			get
			{
				return this.m_ShiftedShoulderRotationRatio;
			}
			set
			{
				this.m_ShiftedShoulderRotationRatio = value;
			}
		}

		public float shiftedElbowRotationRatio
		{
			get
			{
				return this.m_ShiftedElbowRotationRatio;
			}
			set
			{
				this.m_ShiftedElbowRotationRatio = value;
			}
		}

		public float shiftedWristRotationRatio
		{
			get
			{
				return this.m_ShiftedWristRotationRatio;
			}
			set
			{
				this.m_ShiftedWristRotationRatio = value;
			}
		}

		protected override void CalculateFinalJointRotations(Quaternion controllerOrientation, Quaternion xyRotation, Quaternion lerpRotation)
		{
			float num = Quaternion.Angle(xyRotation, Quaternion.identity);
			float num2 = this.maxJointShiftAngle - this.minJointShiftAngle;
			float t = Mathf.Pow(Mathf.Clamp01((num - this.minJointShiftAngle) / num2), this.m_JointShiftExponent);
			float t2 = Mathf.Lerp(this.m_ShoulderRotationRatio, this.m_ShiftedShoulderRotationRatio, t);
			float t3 = Mathf.Lerp(this.m_ElbowRotationRatio, this.m_ShiftedElbowRotationRatio, t);
			float t4 = Mathf.Lerp(this.m_WristRotationRatio, this.m_ShiftedWristRotationRatio, t);
			Quaternion rhs = Quaternion.Lerp(Quaternion.identity, xyRotation, t2);
			Quaternion rhs2 = Quaternion.Lerp(Quaternion.identity, xyRotation, t3);
			Quaternion rhs3 = Quaternion.Lerp(Quaternion.identity, xyRotation, t4);
			Quaternion quaternion = this.m_TorsoRotation * rhs;
			this.m_ElbowRotation = quaternion * rhs2;
			this.m_WristRotation = base.elbowRotation * rhs3;
			this.m_ControllerRotation = this.m_TorsoRotation * controllerOrientation;
			this.m_TorsoRotation = quaternion;
		}

		[Tooltip("Portion of controller rotation applied to the shoulder joint.")]
		[SerializeField]
		[Range(0f, 1f)]
		private float m_ShoulderRotationRatio = 0.5f;

		[Tooltip("Portion of controller rotation applied to the elbow joint.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_ElbowRotationRatio = 0.3f;

		[Tooltip("Portion of controller rotation applied to the wrist joint.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_WristRotationRatio = 0.2f;

		[SerializeField]
		private Vector2 m_JointShiftAngle = new Vector2(160f, 180f);

		[Tooltip("Exponent applied to the joint shift ratio to control the curve of the shift.")]
		[Range(1f, 20f)]
		[SerializeField]
		private float m_JointShiftExponent = 6f;

		[Tooltip("Portion of controller rotation applied to the shoulder joint when the controller is backwards.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_ShiftedShoulderRotationRatio = 0.1f;

		[Tooltip("Portion of controller rotation applied to the elbow joint when the controller is backwards.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_ShiftedElbowRotationRatio = 0.4f;

		[Tooltip("Portion of controller rotation applied to the wrist joint when the controller is backwards.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float m_ShiftedWristRotationRatio = 0.5f;
	}
}
