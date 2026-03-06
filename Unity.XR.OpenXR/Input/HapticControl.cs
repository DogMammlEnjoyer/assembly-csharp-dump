using System;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace UnityEngine.XR.OpenXR.Input
{
	[Preserve]
	public class HapticControl : InputControl<Haptic>
	{
		public HapticControl()
		{
			this.m_StateBlock.sizeInBits = 1U;
			this.m_StateBlock.bitOffset = 0U;
			this.m_StateBlock.byteOffset = 0U;
		}

		public unsafe override Haptic ReadUnprocessedValueFromState(void* statePtr)
		{
			return default(Haptic);
		}
	}
}
