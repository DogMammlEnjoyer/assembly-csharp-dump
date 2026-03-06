using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Controls
{
	[InputControlLayout(hideInUI = true)]
	public class TouchPhaseControl : InputControl<TouchPhase>
	{
		public TouchPhaseControl()
		{
			this.m_StateBlock.format = InputStateBlock.FormatInt;
		}

		public unsafe override TouchPhase ReadUnprocessedValueFromState(void* statePtr)
		{
			return (TouchPhase)base.stateBlock.ReadInt(statePtr);
		}

		public unsafe override void WriteValueIntoState(TouchPhase value, void* statePtr)
		{
			*(int*)((byte*)statePtr + this.m_StateBlock.byteOffset) = (int)value;
		}
	}
}
