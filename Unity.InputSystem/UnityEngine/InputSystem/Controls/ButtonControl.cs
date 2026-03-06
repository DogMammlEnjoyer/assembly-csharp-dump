using System;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Controls
{
	public class ButtonControl : AxisControl
	{
		internal bool needsToCheckFramePress { get; private set; }

		public float pressPointOrDefault
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

		public ButtonControl()
		{
			this.m_StateBlock.format = InputStateBlock.FormatBit;
			this.m_MinValue = 0f;
			this.m_MaxValue = 1f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public new bool IsValueConsideredPressed(float value)
		{
			return value >= this.pressPointOrDefault;
		}

		public unsafe bool isPressed
		{
			get
			{
				if (!this.needsToCheckFramePress)
				{
					return this.IsValueConsideredPressed(*base.value);
				}
				return this.m_LastUpdateWasPress;
			}
		}

		private void BeginTestingForFramePresses(bool currentlyPressed, bool pressedLastFrame)
		{
			this.needsToCheckFramePress = true;
			base.device.m_ButtonControlsCheckingPressState.Add(this);
			this.m_LastUpdateWasPress = currentlyPressed;
			if (currentlyPressed && !pressedLastFrame)
			{
				this.m_UpdateCountLastPressed = base.device.m_CurrentUpdateStepCount;
				return;
			}
			if (pressedLastFrame && !currentlyPressed)
			{
				this.m_UpdateCountLastReleased = base.device.m_CurrentUpdateStepCount;
			}
		}

		public unsafe bool wasPressedThisFrame
		{
			get
			{
				if (!this.needsToCheckFramePress)
				{
					bool flag = this.IsValueConsideredPressed(*base.value);
					bool flag2 = this.IsValueConsideredPressed(base.ReadValueFromPreviousFrame());
					this.BeginTestingForFramePresses(flag, flag2);
					return base.device.wasUpdatedThisFrame && flag && !flag2;
				}
				return InputUpdate.s_UpdateStepCount == this.m_UpdateCountLastPressed;
			}
		}

		public unsafe bool wasReleasedThisFrame
		{
			get
			{
				if (!this.needsToCheckFramePress)
				{
					bool flag = this.IsValueConsideredPressed(*base.value);
					bool flag2 = this.IsValueConsideredPressed(base.ReadValueFromPreviousFrame());
					this.BeginTestingForFramePresses(flag, flag2);
					return base.device.wasUpdatedThisFrame && !flag && flag2;
				}
				return InputUpdate.s_UpdateStepCount == this.m_UpdateCountLastReleased;
			}
		}

		internal unsafe void UpdateWasPressed()
		{
			bool flag = this.IsValueConsideredPressed(*base.value);
			if (this.m_LastUpdateWasPress != flag)
			{
				if (flag)
				{
					this.m_UpdateCountLastPressed = base.device.m_CurrentUpdateStepCount;
				}
				else
				{
					this.m_UpdateCountLastReleased = base.device.m_CurrentUpdateStepCount;
				}
				this.m_LastUpdateWasPress = flag;
			}
		}

		private bool m_NeedsToCheckFramePress;

		private uint m_UpdateCountLastPressed = uint.MaxValue;

		private uint m_UpdateCountLastReleased = uint.MaxValue;

		private bool m_LastUpdateWasPress;

		public float pressPoint = -1f;

		internal static float s_GlobalDefaultButtonPressPoint;

		internal static float s_GlobalDefaultButtonReleaseThreshold;

		internal const float kMinButtonPressPoint = 0.0001f;
	}
}
