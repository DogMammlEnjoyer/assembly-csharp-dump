using System;
using UnityEngine.InputSystem;

namespace UnityEngine.Rendering
{
	internal class DebugActionState
	{
		internal bool runningAction { get; private set; }

		internal float actionState { get; private set; }

		private void Trigger(int triggerCount, float state)
		{
			this.actionState = state;
			this.runningAction = true;
			this.m_Timer = 0f;
			this.m_TriggerPressedUp = new bool[triggerCount];
			for (int i = 0; i < this.m_TriggerPressedUp.Length; i++)
			{
				this.m_TriggerPressedUp[i] = false;
			}
		}

		public void TriggerWithButton(InputAction action, float state)
		{
			this.inputAction = action;
			this.Trigger(action.bindings.Count, state);
		}

		private void Reset()
		{
			this.runningAction = false;
			this.m_Timer = 0f;
			this.m_TriggerPressedUp = null;
		}

		public void Update(DebugActionDesc desc)
		{
			this.actionState = 0f;
			if (this.m_TriggerPressedUp != null)
			{
				this.m_Timer += Time.deltaTime;
				for (int i = 0; i < this.m_TriggerPressedUp.Length; i++)
				{
					if (this.inputAction != null)
					{
						this.m_TriggerPressedUp[i] |= Mathf.Approximately(this.inputAction.ReadValue<float>(), 0f);
					}
				}
				bool flag = true;
				foreach (bool flag2 in this.m_TriggerPressedUp)
				{
					flag = (flag && flag2);
				}
				if (flag || (this.m_Timer > desc.repeatDelay && desc.repeatMode == DebugActionRepeatMode.Delay))
				{
					this.Reset();
				}
			}
		}

		private DebugActionState.DebugActionKeyType m_Type;

		private InputAction inputAction;

		private bool[] m_TriggerPressedUp;

		private float m_Timer;

		private enum DebugActionKeyType
		{
			Button,
			Axis,
			Key
		}
	}
}
