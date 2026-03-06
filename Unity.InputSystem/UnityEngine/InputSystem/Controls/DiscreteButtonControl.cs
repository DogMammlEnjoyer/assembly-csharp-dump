using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Controls
{
	public class DiscreteButtonControl : ButtonControl
	{
		protected override void FinishSetup()
		{
			base.FinishSetup();
			if (!base.stateBlock.format.IsIntegerFormat())
			{
				throw new NotSupportedException(string.Format("Non-integer format '{0}' is not supported for DiscreteButtonControl '{1}'", base.stateBlock.format, this));
			}
		}

		public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
		{
			int num = MemoryHelpers.ReadTwosComplementMultipleBitsAsInt((void*)((byte*)statePtr + this.m_StateBlock.byteOffset), this.m_StateBlock.bitOffset, this.m_StateBlock.sizeInBits);
			float value = 0f;
			if (this.minValue > this.maxValue)
			{
				if (this.wrapAtValue == this.nullValue)
				{
					this.wrapAtValue = this.minValue;
				}
				if ((num >= this.minValue && num <= this.wrapAtValue) || (num != this.nullValue && num <= this.maxValue))
				{
					value = 1f;
				}
			}
			else
			{
				value = ((num >= this.minValue && num <= this.maxValue) ? 1f : 0f);
			}
			return base.Preprocess(value);
		}

		public unsafe override void WriteValueIntoState(float value, void* statePtr)
		{
			if (this.writeMode == DiscreteButtonControl.WriteMode.WriteNullAndMaxValue)
			{
				void* ptr = (void*)((byte*)statePtr + this.m_StateBlock.byteOffset);
				int value2 = (value >= base.pressPointOrDefault) ? this.maxValue : this.nullValue;
				MemoryHelpers.WriteIntAsTwosComplementMultipleBits(ptr, this.m_StateBlock.bitOffset, this.m_StateBlock.sizeInBits, value2);
				return;
			}
			throw new NotSupportedException("Writing value states for DiscreteButtonControl is not supported as a single value may correspond to multiple states");
		}

		public int minValue;

		public int maxValue;

		public int wrapAtValue;

		public int nullValue;

		public DiscreteButtonControl.WriteMode writeMode;

		public enum WriteMode
		{
			WriteDisabled,
			WriteNullAndMaxValue
		}
	}
}
