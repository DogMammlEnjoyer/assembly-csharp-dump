using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Obsolete("SnapTurnProviderBase has been deprecated in XRI 3.0.0 and will be removed in a future version of XRI. Please use SnapTurnProvider instead.", false)]
	public abstract class SnapTurnProviderBase : LocomotionProvider
	{
		public float turnAmount
		{
			get
			{
				return this.m_TurnAmount;
			}
			set
			{
				this.m_TurnAmount = value;
			}
		}

		public float debounceTime
		{
			get
			{
				return this.m_DebounceTime;
			}
			set
			{
				this.m_DebounceTime = value;
			}
		}

		public bool enableTurnLeftRight
		{
			get
			{
				return this.m_EnableTurnLeftRight;
			}
			set
			{
				this.m_EnableTurnLeftRight = value;
			}
		}

		public bool enableTurnAround
		{
			get
			{
				return this.m_EnableTurnAround;
			}
			set
			{
				this.m_EnableTurnAround = value;
			}
		}

		public float delayTime
		{
			get
			{
				return this.m_DelayTime;
			}
			set
			{
				this.m_DelayTime = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (base.system != null && this.m_DelayTime > 0f && this.m_DelayTime > base.system.timeout)
			{
				Debug.LogWarning(string.Format("Delay Time ({0}) is longer than the Locomotion System's Timeout ({1}).", this.m_DelayTime, base.system.timeout), this);
			}
		}

		protected void Update()
		{
			if (this.m_TimeStarted > 0f && this.m_TimeStarted + this.m_DebounceTime < Time.time)
			{
				this.m_TimeStarted = 0f;
				return;
			}
			if (base.locomotionPhase == LocomotionPhase.Done)
			{
				base.locomotionPhase = LocomotionPhase.Idle;
			}
			Vector2 vector = this.ReadInput();
			float turnAmount = this.GetTurnAmount(vector);
			if (Mathf.Abs(turnAmount) > 0f || base.locomotionPhase == LocomotionPhase.Started)
			{
				this.StartTurn(turnAmount);
			}
			else if (Mathf.Approximately(this.m_CurrentTurnAmount, 0f) && base.locomotionPhase == LocomotionPhase.Moving)
			{
				base.locomotionPhase = LocomotionPhase.Done;
			}
			if (base.locomotionPhase == LocomotionPhase.Moving && Math.Abs(this.m_CurrentTurnAmount) > 0f && base.BeginLocomotion())
			{
				XROrigin xrOrigin = base.system.xrOrigin;
				if (xrOrigin != null)
				{
					xrOrigin.RotateAroundCameraUsingOriginUp(this.m_CurrentTurnAmount);
				}
				else
				{
					base.locomotionPhase = LocomotionPhase.Done;
				}
				this.m_CurrentTurnAmount = 0f;
				base.EndLocomotion();
				if (Mathf.Approximately(turnAmount, 0f))
				{
					base.locomotionPhase = LocomotionPhase.Done;
				}
			}
			if (vector == Vector2.zero)
			{
				this.m_TurnAroundActivated = false;
			}
		}

		protected abstract Vector2 ReadInput();

		protected virtual float GetTurnAmount(Vector2 input)
		{
			if (input == Vector2.zero)
			{
				return 0f;
			}
			switch (CardinalUtility.GetNearestCardinal(input))
			{
			case Cardinal.South:
				if (this.m_EnableTurnAround && !this.m_TurnAroundActivated)
				{
					return 180f;
				}
				break;
			case Cardinal.East:
				if (this.m_EnableTurnLeftRight)
				{
					return this.m_TurnAmount;
				}
				break;
			case Cardinal.West:
				if (this.m_EnableTurnLeftRight)
				{
					return -this.m_TurnAmount;
				}
				break;
			}
			return 0f;
		}

		protected void StartTurn(float amount)
		{
			if (this.m_TimeStarted > 0f)
			{
				return;
			}
			if (!base.CanBeginLocomotion())
			{
				return;
			}
			if (Mathf.Approximately(amount, 180f))
			{
				this.m_TurnAroundActivated = true;
			}
			if (base.locomotionPhase == LocomotionPhase.Idle)
			{
				base.locomotionPhase = LocomotionPhase.Started;
				this.m_DelayStartTime = Time.time;
			}
			if (Math.Abs(amount) > 0f)
			{
				this.m_CurrentTurnAmount = amount;
			}
			if (this.m_DelayTime > 0f && Time.time - this.m_DelayStartTime < this.m_DelayTime)
			{
				return;
			}
			base.locomotionPhase = LocomotionPhase.Moving;
			this.m_TimeStarted = Time.time;
		}

		[SerializeField]
		[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
		private float m_TurnAmount = 45f;

		[SerializeField]
		[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
		private float m_DebounceTime = 0.5f;

		[SerializeField]
		[Tooltip("Controls whether to enable left & right snap turns.")]
		private bool m_EnableTurnLeftRight = true;

		[SerializeField]
		[Tooltip("Controls whether to enable 180° snap turns.")]
		private bool m_EnableTurnAround = true;

		[SerializeField]
		[Tooltip("The time (in seconds) to delay the first turn after receiving initial input for the turn.")]
		private float m_DelayTime;

		private float m_CurrentTurnAmount;

		private float m_TimeStarted;

		private float m_DelayStartTime;

		private bool m_TurnAroundActivated;
	}
}
