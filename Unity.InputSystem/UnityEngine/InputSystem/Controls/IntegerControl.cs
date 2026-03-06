using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Controls
{
	public class IntegerControl : InputControl<int>
	{
		public IntegerControl()
		{
			this.m_StateBlock.format = InputStateBlock.FormatInt;
		}

		public unsafe override int ReadUnprocessedValueFromState(void* statePtr)
		{
			if (this.m_OptimizedControlDataType == 1229870112)
			{
				return *(int*)((byte*)statePtr + this.m_StateBlock.byteOffset);
			}
			return this.m_StateBlock.ReadInt(statePtr);
		}

		public unsafe override void WriteValueIntoState(int value, void* statePtr)
		{
			if (this.m_OptimizedControlDataType == 1229870112)
			{
				*(int*)((byte*)statePtr + this.m_StateBlock.byteOffset) = value;
				return;
			}
			this.m_StateBlock.WriteInt(statePtr, value);
		}

		protected override FourCC CalculateOptimizedControlDataType()
		{
			if (this.m_StateBlock.format == InputStateBlock.FormatInt && this.m_StateBlock.sizeInBits == 32U && this.m_StateBlock.bitOffset == 0U)
			{
				return InputStateBlock.FormatInt;
			}
			return InputStateBlock.FormatInvalid;
		}
	}
}
