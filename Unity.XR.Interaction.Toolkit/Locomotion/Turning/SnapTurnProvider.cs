using System;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning
{
	[AddComponentMenu("XR/Locomotion/Snap Turn Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning.SnapTurnProvider.html")]
	public class SnapTurnProvider : LocomotionProvider
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

		public override bool canStartMoving
		{
			get
			{
				return this.m_DelayTime <= 0f || Time.time - this.m_DelayStartTime >= this.m_DelayTime;
			}
		}

		public XRBodyYawRotation transformation { get; set; } = new XRBodyYawRotation();

		public XRInputValueReader<Vector2> leftHandTurnInput
		{
			get
			{
				return this.m_LeftHandTurnInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_LeftHandTurnInput, value, this);
			}
		}

		public XRInputValueReader<Vector2> rightHandTurnInput
		{
			get
			{
				return this.m_RightHandTurnInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_RightHandTurnInput, value, this);
			}
		}

		protected void OnEnable()
		{
			this.m_LeftHandTurnInput.EnableDirectActionIfModeUsed();
			this.m_RightHandTurnInput.EnableDirectActionIfModeUsed();
		}

		protected void OnDisable()
		{
			this.m_LeftHandTurnInput.DisableDirectActionIfModeUsed();
			this.m_RightHandTurnInput.DisableDirectActionIfModeUsed();
		}

		protected void Update()
		{
			if (this.m_TimeStarted > 0f && this.m_TimeStarted + this.m_DebounceTime < Time.time)
			{
				this.m_TimeStarted = 0f;
				return;
			}
			Vector2 vector = this.ReadInput();
			float turnAmount = this.GetTurnAmount(vector);
			if (Mathf.Abs(turnAmount) > 0f)
			{
				this.StartTurn(turnAmount);
			}
			else if (Mathf.Approximately(this.m_CurrentTurnAmount, 0f) && base.locomotionState == LocomotionState.Moving)
			{
				base.TryEndLocomotion();
			}
			if (base.locomotionState == LocomotionState.Moving && math.abs(this.m_CurrentTurnAmount) > 0f)
			{
				this.m_TimeStarted = Time.time;
				this.transformation.angleDelta = this.m_CurrentTurnAmount;
				base.TryQueueTransformation(this.transformation);
				this.m_CurrentTurnAmount = 0f;
				if (Mathf.Approximately(turnAmount, 0f))
				{
					base.TryEndLocomotion();
				}
			}
			if (vector == Vector2.zero)
			{
				this.m_TurnAroundActivated = false;
			}
		}

		private Vector2 ReadInput()
		{
			Vector2 a = this.m_LeftHandTurnInput.ReadValue();
			Vector2 b = this.m_RightHandTurnInput.ReadValue();
			return a + b;
		}

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
			if (Mathf.Approximately(amount, 180f))
			{
				this.m_TurnAroundActivated = true;
			}
			if (base.locomotionState == LocomotionState.Idle)
			{
				if (this.m_DelayTime > 0f)
				{
					if (base.TryPrepareLocomotion())
					{
						this.m_DelayStartTime = Time.time;
					}
				}
				else
				{
					base.TryStartLocomotionImmediately();
				}
			}
			if (math.abs(amount) > 0f)
			{
				this.m_CurrentTurnAmount = amount;
			}
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

		[SerializeField]
		[Tooltip("Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).")]
		private XRInputValueReader<Vector2> m_LeftHandTurnInput = new XRInputValueReader<Vector2>("Left Hand Snap Turn", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).")]
		private XRInputValueReader<Vector2> m_RightHandTurnInput = new XRInputValueReader<Vector2>("Right Hand Snap Turn", XRInputValueReader.InputSourceMode.InputActionReference);

		private float m_CurrentTurnAmount;

		private float m_TimeStarted;

		private float m_DelayStartTime;

		private bool m_TurnAroundActivated;
	}
}
