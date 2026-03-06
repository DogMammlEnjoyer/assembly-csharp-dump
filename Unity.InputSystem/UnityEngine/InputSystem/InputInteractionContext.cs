using System;

namespace UnityEngine.InputSystem
{
	public struct InputInteractionContext
	{
		public InputAction action
		{
			get
			{
				return this.m_State.GetActionOrNull(ref this.m_TriggerState);
			}
		}

		public InputControl control
		{
			get
			{
				return this.m_State.GetControl(ref this.m_TriggerState);
			}
		}

		public InputActionPhase phase
		{
			get
			{
				return this.m_TriggerState.phase;
			}
		}

		public double time
		{
			get
			{
				return this.m_TriggerState.time;
			}
		}

		public double startTime
		{
			get
			{
				return this.m_TriggerState.startTime;
			}
		}

		public bool timerHasExpired
		{
			get
			{
				return (this.m_Flags & InputInteractionContext.Flags.TimerHasExpired) > (InputInteractionContext.Flags)0;
			}
			internal set
			{
				if (value)
				{
					this.m_Flags |= InputInteractionContext.Flags.TimerHasExpired;
					return;
				}
				this.m_Flags &= ~InputInteractionContext.Flags.TimerHasExpired;
			}
		}

		public bool isWaiting
		{
			get
			{
				return this.phase == InputActionPhase.Waiting;
			}
		}

		public bool isStarted
		{
			get
			{
				return this.phase == InputActionPhase.Started;
			}
		}

		public float ComputeMagnitude()
		{
			return this.m_TriggerState.magnitude;
		}

		public bool ControlIsActuated(float threshold = 0f)
		{
			return InputActionState.IsActuated(ref this.m_TriggerState, threshold);
		}

		public void Started()
		{
			this.m_TriggerState.startTime = this.time;
			this.m_State.ChangePhaseOfInteraction(InputActionPhase.Started, ref this.m_TriggerState, InputActionPhase.Waiting, InputActionPhase.Waiting, true);
		}

		public void Performed()
		{
			if (this.m_TriggerState.phase == InputActionPhase.Waiting)
			{
				this.m_TriggerState.startTime = this.time;
			}
			this.m_State.ChangePhaseOfInteraction(InputActionPhase.Performed, ref this.m_TriggerState, InputActionPhase.Waiting, InputActionPhase.Waiting, true);
		}

		public void PerformedAndStayStarted()
		{
			if (this.m_TriggerState.phase == InputActionPhase.Waiting)
			{
				this.m_TriggerState.startTime = this.time;
			}
			this.m_State.ChangePhaseOfInteraction(InputActionPhase.Performed, ref this.m_TriggerState, InputActionPhase.Started, InputActionPhase.Waiting, true);
		}

		public void PerformedAndStayPerformed()
		{
			if (this.m_TriggerState.phase == InputActionPhase.Waiting)
			{
				this.m_TriggerState.startTime = this.time;
			}
			this.m_State.ChangePhaseOfInteraction(InputActionPhase.Performed, ref this.m_TriggerState, InputActionPhase.Performed, InputActionPhase.Waiting, true);
		}

		public void Canceled()
		{
			if (this.m_TriggerState.phase != InputActionPhase.Canceled)
			{
				this.m_State.ChangePhaseOfInteraction(InputActionPhase.Canceled, ref this.m_TriggerState, InputActionPhase.Waiting, InputActionPhase.Waiting, true);
			}
		}

		public void Waiting()
		{
			if (this.m_TriggerState.phase != InputActionPhase.Waiting)
			{
				this.m_State.ChangePhaseOfInteraction(InputActionPhase.Waiting, ref this.m_TriggerState, InputActionPhase.Waiting, InputActionPhase.Waiting, true);
			}
		}

		public void SetTimeout(float seconds)
		{
			this.m_State.StartTimeout(seconds, ref this.m_TriggerState);
		}

		public void SetTotalTimeoutCompletionTime(float seconds)
		{
			if (seconds <= 0f)
			{
				throw new ArgumentException("Seconds must be a positive value", "seconds");
			}
			this.m_State.SetTotalTimeoutCompletionTime(seconds, ref this.m_TriggerState);
		}

		public TValue ReadValue<TValue>() where TValue : struct
		{
			return this.m_State.ReadValue<TValue>(this.m_TriggerState.bindingIndex, this.m_TriggerState.controlIndex, false);
		}

		internal int mapIndex
		{
			get
			{
				return this.m_TriggerState.mapIndex;
			}
		}

		internal int controlIndex
		{
			get
			{
				return this.m_TriggerState.controlIndex;
			}
		}

		internal int bindingIndex
		{
			get
			{
				return this.m_TriggerState.bindingIndex;
			}
		}

		internal int interactionIndex
		{
			get
			{
				return this.m_TriggerState.interactionIndex;
			}
		}

		internal InputActionState m_State;

		internal InputInteractionContext.Flags m_Flags;

		internal InputActionState.TriggerState m_TriggerState;

		[Flags]
		internal enum Flags
		{
			TimerHasExpired = 2
		}
	}
}
