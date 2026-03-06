using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.InputSystem.Interactions
{
	[DisplayName("Hold")]
	public class HoldInteraction : IInputInteraction
	{
		private float durationOrDefault
		{
			get
			{
				if ((double)this.duration <= 0.0)
				{
					return InputSystem.settings.defaultHoldTime;
				}
				return this.duration;
			}
		}

		private float pressPointOrDefault
		{
			get
			{
				if ((double)this.pressPoint <= 0.0)
				{
					return ButtonControl.s_GlobalDefaultButtonPressPoint;
				}
				return this.pressPoint;
			}
		}

		public void Process(ref InputInteractionContext context)
		{
			if (context.timerHasExpired)
			{
				context.PerformedAndStayPerformed();
				return;
			}
			switch (context.phase)
			{
			case InputActionPhase.Waiting:
				if (context.ControlIsActuated(this.pressPointOrDefault))
				{
					this.m_TimePressed = context.time;
					context.Started();
					context.SetTimeout(this.durationOrDefault);
					return;
				}
				break;
			case InputActionPhase.Started:
				if (context.time - this.m_TimePressed >= (double)this.durationOrDefault)
				{
					context.PerformedAndStayPerformed();
				}
				if (!context.ControlIsActuated(0f))
				{
					context.Canceled();
					return;
				}
				break;
			case InputActionPhase.Performed:
				if (!context.ControlIsActuated(this.pressPointOrDefault))
				{
					context.Canceled();
				}
				break;
			default:
				return;
			}
		}

		public void Reset()
		{
			this.m_TimePressed = 0.0;
		}

		public float duration;

		public float pressPoint;

		private double m_TimePressed;
	}
}
