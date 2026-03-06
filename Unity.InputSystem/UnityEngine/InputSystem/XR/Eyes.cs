using System;

namespace UnityEngine.InputSystem.XR
{
	public struct Eyes
	{
		public Vector3 leftEyePosition
		{
			get
			{
				return this.m_LeftEyePosition;
			}
			set
			{
				this.m_LeftEyePosition = value;
			}
		}

		public Quaternion leftEyeRotation
		{
			get
			{
				return this.m_LeftEyeRotation;
			}
			set
			{
				this.m_LeftEyeRotation = value;
			}
		}

		public Vector3 rightEyePosition
		{
			get
			{
				return this.m_RightEyePosition;
			}
			set
			{
				this.m_RightEyePosition = value;
			}
		}

		public Quaternion rightEyeRotation
		{
			get
			{
				return this.m_RightEyeRotation;
			}
			set
			{
				this.m_RightEyeRotation = value;
			}
		}

		public Vector3 fixationPoint
		{
			get
			{
				return this.m_FixationPoint;
			}
			set
			{
				this.m_FixationPoint = value;
			}
		}

		public float leftEyeOpenAmount
		{
			get
			{
				return this.m_LeftEyeOpenAmount;
			}
			set
			{
				this.m_LeftEyeOpenAmount = value;
			}
		}

		public float rightEyeOpenAmount
		{
			get
			{
				return this.m_RightEyeOpenAmount;
			}
			set
			{
				this.m_RightEyeOpenAmount = value;
			}
		}

		public Vector3 m_LeftEyePosition;

		public Quaternion m_LeftEyeRotation;

		public Vector3 m_RightEyePosition;

		public Quaternion m_RightEyeRotation;

		public Vector3 m_FixationPoint;

		public float m_LeftEyeOpenAmount;

		public float m_RightEyeOpenAmount;
	}
}
