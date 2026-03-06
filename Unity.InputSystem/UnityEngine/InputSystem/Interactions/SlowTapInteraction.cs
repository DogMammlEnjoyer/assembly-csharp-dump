using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.InputSystem.Interactions
{
	[DisplayName("Long Tap")]
	public class SlowTapInteraction : IInputInteraction
	{
		private float durationOrDefault
		{
			get
			{
				if (this.duration <= 0f)
				{
					return InputSystem.settings.defaultSlowTapTime;
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

		public void Process(ref InputInteractionContext context)
		{
			if (context.isWaiting && context.ControlIsActuated(this.pressPointOrDefault))
			{
				this.m_SlowTapStartTime = context.time;
				context.Started();
				return;
			}
			if (context.isStarted && !context.ControlIsActuated(this.pressPointOrDefault))
			{
				if (context.time - this.m_SlowTapStartTime >= (double)this.durationOrDefault)
				{
					context.Performed();
					return;
				}
				context.Canceled();
			}
		}

		public void Reset()
		{
			this.m_SlowTapStartTime = 0.0;
		}

		public float duration;

		public float pressPoint;

		private double m_SlowTapStartTime;
	}
}
