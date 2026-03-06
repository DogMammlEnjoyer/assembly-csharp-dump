using System;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Controls
{
	public class DoubleControl : InputControl<double>
	{
		public DoubleControl()
		{
			this.m_StateBlock.format = InputStateBlock.FormatDouble;
		}

		public unsafe override double ReadUnprocessedValueFromState(void* statePtr)
		{
			return this.m_StateBlock.ReadDouble(statePtr);
		}

		public unsafe override void WriteValueIntoState(double value, void* statePtr)
		{
			this.m_StateBlock.WriteDouble(statePtr, value);
		}
	}
}
