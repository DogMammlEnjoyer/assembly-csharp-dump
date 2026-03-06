using System;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.InputSystem.Interactions
{
	public class MultiTapInteraction : IInputInteraction<float>, IInputInteraction
	{
		private float tapTimeOrDefault
		{
			get
			{
				if ((double)this.tapTime <= 0.0)
				{
					return InputSystem.settings.defaultTapTime;
				}
				return this.tapTime;
			}
		}

		internal float tapDelayOrDefault
		{
			get
			{
				if ((double)this.tapDelay <= 0.0)
				{
					return InputSystem.settings.multiTapDelayTime;
				}
				return this.tapDelay;
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
				return;
			}
			switch (this.m_CurrentTapPhase)
			{
			case MultiTapInteraction.TapPhase.None:
				if (context.ControlIsActuated(this.pressPointOrDefault))
				{
					this.m_CurrentTapPhase = MultiTapInteraction.TapPhase.WaitingForNextRelease;
					this.m_CurrentTapStartTime = context.time;
					context.Started();
					float tapTimeOrDefault = this.tapTimeOrDefault;
					float tapDelayOrDefault = this.tapDelayOrDefault;
					context.SetTimeout(tapTimeOrDefault);
					context.SetTotalTimeoutCompletionTime(tapTimeOrDefault * (float)this.tapCount + (float)(this.tapCount - 1) * tapDelayOrDefault);
					return;
				}
				break;
			case MultiTapInteraction.TapPhase.WaitingForNextRelease:
				if (!context.ControlIsActuated(this.releasePointOrDefault))
				{
					if (context.time - this.m_CurrentTapStartTime > (double)this.tapTimeOrDefault)
					{
						context.Canceled();
						return;
					}
					this.m_CurrentTapCount++;
					if (this.m_CurrentTapCount >= this.tapCount)
					{
						context.Performed();
						return;
					}
					this.m_CurrentTapPhase = MultiTapInteraction.TapPhase.WaitingForNextPress;
					this.m_LastTapReleaseTime = context.time;
					context.SetTimeout(this.tapDelayOrDefault);
					return;
				}
				break;
			case MultiTapInteraction.TapPhase.WaitingForNextPress:
				if (context.ControlIsActuated(this.pressPointOrDefault))
				{
					if (context.time - this.m_LastTapReleaseTime <= (double)this.tapDelayOrDefault)
					{
						this.m_CurrentTapPhase = MultiTapInteraction.TapPhase.WaitingForNextRelease;
						this.m_CurrentTapStartTime = context.time;
						context.SetTimeout(this.tapTimeOrDefault);
						return;
					}
					context.Canceled();
				}
				break;
			default:
				return;
			}
		}

		public void Reset()
		{
			this.m_CurrentTapPhase = MultiTapInteraction.TapPhase.None;
			this.m_CurrentTapCount = 0;
			this.m_CurrentTapStartTime = 0.0;
			this.m_LastTapReleaseTime = 0.0;
		}

		[Tooltip("The maximum time (in seconds) allowed to elapse between pressing and releasing a control for it to register as a tap.")]
		public float tapTime;

		[Tooltip("The maximum delay (in seconds) allowed between each tap. If this time is exceeded, the multi-tap is canceled.")]
		public float tapDelay;

		[Tooltip("How many taps need to be performed in succession. Two means double-tap, three means triple-tap, and so on.")]
		public int tapCount = 2;

		public float pressPoint;

		private MultiTapInteraction.TapPhase m_CurrentTapPhase;

		private int m_CurrentTapCount;

		private double m_CurrentTapStartTime;

		private double m_LastTapReleaseTime;

		private enum TapPhase
		{
			None,
			WaitingForNextRelease,
			WaitingForNextPress
		}
	}
}
