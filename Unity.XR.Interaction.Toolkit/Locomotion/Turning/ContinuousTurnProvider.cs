using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning
{
	[AddComponentMenu("XR/Locomotion/Continuous Turn Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning.ContinuousTurnProvider.html")]
	public class ContinuousTurnProvider : LocomotionProvider
	{
		public float turnSpeed
		{
			get
			{
				return this.m_TurnSpeed;
			}
			set
			{
				this.m_TurnSpeed = value;
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

		public XRBodyYawRotation transformation { get; set; } = new XRBodyYawRotation();

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
			this.m_IsTurningXROrigin = false;
			Vector2 vector = this.ReadInput();
			float turnAmount = this.GetTurnAmount(vector);
			this.TurnRig(turnAmount);
			if (!this.m_IsTurningXROrigin)
			{
				base.TryEndLocomotion();
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
			switch (CardinalUtility.GetNearestCardinal(input))
			{
			case Cardinal.South:
				if (this.m_EnableTurnAround && !this.m_TurnAroundActivated)
				{
					return 180f;
				}
				break;
			case Cardinal.East:
			case Cardinal.West:
				if (this.m_EnableTurnLeftRight)
				{
					return input.magnitude * (Mathf.Sign(input.x) * this.m_TurnSpeed * Time.deltaTime);
				}
				break;
			}
			return 0f;
		}

		protected void TurnRig(float turnAmount)
		{
			if (Mathf.Approximately(turnAmount, 0f))
			{
				return;
			}
			if (Mathf.Approximately(turnAmount, 180f))
			{
				this.m_TurnAroundActivated = true;
			}
			base.TryStartLocomotionImmediately();
			if (base.locomotionState != LocomotionState.Moving)
			{
				return;
			}
			this.m_IsTurningXROrigin = true;
			this.transformation.angleDelta = turnAmount;
			base.TryQueueTransformation(this.transformation);
		}

		[SerializeField]
		[Tooltip("The number of degrees/second clockwise to rotate when turning clockwise.")]
		private float m_TurnSpeed = 60f;

		[SerializeField]
		[Tooltip("Controls whether to enable left & right continuous turns.")]
		private bool m_EnableTurnLeftRight = true;

		[SerializeField]
		[Tooltip("Controls whether to enable 180° snap turns on the South direction.")]
		private bool m_EnableTurnAround;

		[SerializeField]
		[Tooltip("Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).")]
		private XRInputValueReader<Vector2> m_LeftHandTurnInput = new XRInputValueReader<Vector2>("Left Hand Turn", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).")]
		private XRInputValueReader<Vector2> m_RightHandTurnInput = new XRInputValueReader<Vector2>("Right Hand Turn", XRInputValueReader.InputSourceMode.InputActionReference);

		private bool m_IsTurningXROrigin;

		private bool m_TurnAroundActivated;
	}
}
