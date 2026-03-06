using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.InputSystem.Interactions
{
	[DisplayName("Tap")]
	public class TapInteraction : IInputInteraction
	{
		private float durationOrDefault
		{
			get
			{
				if ((double)this.duration <= 0.0)
				{
					return InputSystem.settings.defaultTapTime;
				}
				return this.duration;
			}
		}

		private float pressPointOrDefault
		{
			get
			{
				if (this.pressPoint <= 0f)
				{
					return ButtonControl.s_GlobalDefaultButtonPressPoint;
				}
				return this.pressPoint;
			}
		}

		private float releasePointOrDefault
		{
			get
			{
				return this.pressPointOrDefault * ButtonControl.s_GlobalDefaultButtonReleaseThreshold;
			}
		}

		public void Process(ref InputInteractionContext context)
		{
			if (context.timerHasExpired)
			{
				context.Canceled();
				this.canceledFromTimerExpired = true;
				return;
			}
			if (context.isWaiting && context.ControlIsActuated(this.pressPointOrDefault) && !this.canceledFromTimerExpired)
			{
				this.m_TapStartTime = context.time;
				context.Started();
				context.SetTimeout(this.durationOrDefault + 1E-05f);
				return;
			}
			if (context.isStarted && !context.ControlIsActuated(this.releasePointOrDefault))
			{
				if (context.time - this.m_TapStartTime <= (double)this.durationOrDefault)
				{
					context.Performed();
				}
				else
				{
					context.Canceled();
				}
			}
			if (!context.ControlIsActuated(this.releasePointOrDefault))
			{
				this.canceledFromTimerExpired = false;
			}
		}

		public void Reset()
		{
			this.m_TapStartTime = 0.0;
		}

		public float duration;

		public float pressPoint;

		private double m_TapStartTime;

		private bool canceledFromTimerExpired;
	}
}
