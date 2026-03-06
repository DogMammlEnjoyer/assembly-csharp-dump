using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Obsolete("The ContinuousMoveProviderBase has been deprecated in XRI 3.0.0 and will be removed in a future version of XRI. Please use ContinuousTurnProvider instead.", false)]
	public abstract class ContinuousTurnProviderBase : LocomotionProvider
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

		protected void Update()
		{
			this.m_IsTurningXROrigin = false;
			Vector2 input = this.ReadInput();
			float turnAmount = this.GetTurnAmount(input);
			this.TurnRig(turnAmount);
			switch (base.locomotionPhase)
			{
			case LocomotionPhase.Idle:
			case LocomotionPhase.Started:
				if (this.m_IsTurningXROrigin)
				{
					base.locomotionPhase = LocomotionPhase.Moving;
					return;
				}
				break;
			case LocomotionPhase.Moving:
				if (!this.m_IsTurningXROrigin)
				{
					base.locomotionPhase = LocomotionPhase.Done;
					return;
				}
				break;
			case LocomotionPhase.Done:
				base.locomotionPhase = (this.m_IsTurningXROrigin ? LocomotionPhase.Moving : LocomotionPhase.Idle);
				break;
			default:
				return;
			}
		}

		protected abstract Vector2 ReadInput();

		protected virtual float GetTurnAmount(Vector2 input)
		{
			if (input == Vector2.zero)
			{
				return 0f;
			}
			Cardinal nearestCardinal = CardinalUtility.GetNearestCardinal(input);
			if (nearestCardinal > Cardinal.South && nearestCardinal - Cardinal.East <= 1)
			{
				return input.magnitude * (Mathf.Sign(input.x) * this.m_TurnSpeed * Time.deltaTime);
			}
			return 0f;
		}

		protected void TurnRig(float turnAmount)
		{
			if (Mathf.Approximately(turnAmount, 0f))
			{
				return;
			}
			if (base.CanBeginLocomotion() && base.BeginLocomotion())
			{
				XROrigin xrOrigin = base.system.xrOrigin;
				if (xrOrigin != null)
				{
					this.m_IsTurningXROrigin = true;
					xrOrigin.RotateAroundCameraUsingOriginUp(turnAmount);
				}
				base.EndLocomotion();
			}
		}

		[SerializeField]
		[Tooltip("The number of degrees/second clockwise to rotate when turning clockwise.")]
		private float m_TurnSpeed = 60f;

		private bool m_IsTurningXROrigin;
	}
}
