using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Controls
{
	[InputControlLayout(hideInUI = true)]
	public class TouchPressControl : ButtonControl
	{
		protected override void FinishSetup()
		{
			base.FinishSetup();
			if (!base.stateBlock.format.IsIntegerFormat())
			{
				throw new NotSupportedException(string.Format("Non-integer format '{0}' is not supported for TouchButtonControl '{1}'", base.stateBlock.format, this));
			}
		}

		public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
		{
			TouchPhase touchPhase = (TouchPhase)MemoryHelpers.ReadMultipleBitsAsUInt((void*)((byte*)statePtr + this.m_StateBlock.byteOffset), this.m_StateBlock.bitOffset, this.m_StateBlock.sizeInBits);
			float value = 0f;
			if (touchPhase == TouchPhase.Began || touchPhase == TouchPhase.Stationary || touchPhase == TouchPhase.Moved)
			{
				value = 1f;
			}
			return base.Preprocess(value);
		}

		public unsafe override void WriteValueIntoState(float value, void* statePtr)
		{
			throw new NotSupportedException();
		}
	}
}
