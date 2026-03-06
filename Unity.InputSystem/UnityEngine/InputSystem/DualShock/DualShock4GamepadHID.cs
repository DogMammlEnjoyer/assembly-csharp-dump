using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock
{
	[InputControlLayout(stateType = typeof(DualShock4HIDInputReport), hideInUI = true, isNoisy = true)]
	public class DualShock4GamepadHID : DualShockGamepad, IEventPreProcessor, IInputStateCallbackReceiver
	{
		public ButtonControl leftTriggerButton { get; protected set; }

		public ButtonControl rightTriggerButton { get; protected set; }

		public ButtonControl playStationButton { get; protected set; }

		protected override void FinishSetup()
		{
			this.leftTriggerButton = base.GetChildControl<ButtonControl>("leftTriggerButton");
			this.rightTriggerButton = base.GetChildControl<ButtonControl>("rightTriggerButton");
			this.playStationButton = base.GetChildControl<ButtonControl>("systemButton");
			base.FinishSetup();
		}

		public override void PauseHaptics()
		{
			if (this.m_LowFrequencyMotorSpeed == null && this.m_HighFrequenceyMotorSpeed == null && this.m_LightBarColor == null)
			{
				return;
			}
			DualShockHIDOutputReport dualShockHIDOutputReport = DualShockHIDOutputReport.Create(base.hidDescriptor.outputReportSize);
			dualShockHIDOutputReport.SetMotorSpeeds(0f, 0f);
			if (this.m_LightBarColor != null)
			{
				dualShockHIDOutputReport.SetColor(Color.black);
			}
			base.ExecuteCommand<DualShockHIDOutputReport>(ref dualShockHIDOutputReport);
		}

		public override void ResetHaptics()
		{
			if (this.m_LowFrequencyMotorSpeed == null && this.m_HighFrequenceyMotorSpeed == null && this.m_LightBarColor == null)
			{
				return;
			}
			DualShockHIDOutputReport dualShockHIDOutputReport = DualShockHIDOutputReport.Create(base.hidDescriptor.outputReportSize);
			dualShockHIDOutputReport.SetMotorSpeeds(0f, 0f);
			if (this.m_LightBarColor != null)
			{
				dualShockHIDOutputReport.SetColor(Color.black);
			}
			base.ExecuteCommand<DualShockHIDOutputReport>(ref dualShockHIDOutputReport);
			this.m_HighFrequenceyMotorSpeed = null;
			this.m_LowFrequencyMotorSpeed = null;
			this.m_LightBarColor = null;
		}

		public override void ResumeHaptics()
		{
			if (this.m_LowFrequencyMotorSpeed == null && this.m_HighFrequenceyMotorSpeed == null && this.m_LightBarColor == null)
			{
				return;
			}
			DualShockHIDOutputReport dualShockHIDOutputReport = DualShockHIDOutputReport.Create(base.hidDescriptor.outputReportSize);
			if (this.m_LowFrequencyMotorSpeed != null || this.m_HighFrequenceyMotorSpeed != null)
			{
				dualShockHIDOutputReport.SetMotorSpeeds(this.m_LowFrequencyMotorSpeed.Value, this.m_HighFrequenceyMotorSpeed.Value);
			}
			if (this.m_LightBarColor != null)
			{
				dualShockHIDOutputReport.SetColor(this.m_LightBarColor.Value);
			}
			base.ExecuteCommand<DualShockHIDOutputReport>(ref dualShockHIDOutputReport);
		}

		public override void SetLightBarColor(Color color)
		{
			DualShockHIDOutputReport dualShockHIDOutputReport = DualShockHIDOutputReport.Create(base.hidDescriptor.outputReportSize);
			dualShockHIDOutputReport.SetColor(color);
			base.ExecuteCommand<DualShockHIDOutputReport>(ref dualShockHIDOutputReport);
			this.m_LightBarColor = new Color?(color);
		}

		public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
		{
			DualShockHIDOutputReport dualShockHIDOutputReport = DualShockHIDOutputReport.Create(base.hidDescriptor.outputReportSize);
			dualShockHIDOutputReport.SetMotorSpeeds(lowFrequency, highFrequency);
			base.ExecuteCommand<DualShockHIDOutputReport>(ref dualShockHIDOutputReport);
			this.m_LowFrequencyMotorSpeed = new float?(lowFrequency);
			this.m_HighFrequenceyMotorSpeed = new float?(highFrequency);
		}

		public bool SetMotorSpeedsAndLightBarColor(float lowFrequency, float highFrequency, Color color)
		{
			DualShockHIDOutputReport dualShockHIDOutputReport = DualShockHIDOutputReport.Create(base.hidDescriptor.outputReportSize);
			dualShockHIDOutputReport.SetMotorSpeeds(lowFrequency, highFrequency);
			dualShockHIDOutputReport.SetColor(color);
			long num = base.ExecuteCommand<DualShockHIDOutputReport>(ref dualShockHIDOutputReport);
			this.m_LowFrequencyMotorSpeed = new float?(lowFrequency);
			this.m_HighFrequenceyMotorSpeed = new float?(highFrequency);
			this.m_LightBarColor = new Color?(color);
			return num >= 0L;
		}

		unsafe bool IEventPreProcessor.PreProcessEvent(InputEventPtr eventPtr)
		{
			if (eventPtr.type != 1398030676)
			{
				return eventPtr.type != 1145852993;
			}
			StateEvent* ptr = StateEvent.FromUnchecked(eventPtr);
			if (ptr->stateFormat == DualShock4HIDInputReport.Format)
			{
				return true;
			}
			uint stateSizeInBytes = ptr->stateSizeInBytes;
			if (ptr->stateFormat != DualShock4GamepadHID.DualShock4HIDGenericInputReport.Format || (ulong)stateSizeInBytes < (ulong)((long)sizeof(DualShock4GamepadHID.DualShock4HIDGenericInputReport)))
			{
				return false;
			}
			byte* state = (byte*)ptr->state;
			byte b = *state;
			if (b != 1)
			{
				if (b - 17 > 8)
				{
					return false;
				}
				if ((state[1] & 128) == 0)
				{
					return false;
				}
				if ((ulong)stateSizeInBytes < (ulong)((long)(sizeof(DualShock4GamepadHID.DualShock4HIDGenericInputReport) + 3)))
				{
					return false;
				}
				DualShock4HIDInputReport dualShock4HIDInputReport = ((DualShock4GamepadHID.DualShock4HIDGenericInputReport*)(state + 3))->ToHIDInputReport();
				*(DualShock4HIDInputReport*)ptr->state = dualShock4HIDInputReport;
				ptr->stateFormat = DualShock4HIDInputReport.Format;
				return true;
			}
			else
			{
				if ((ulong)stateSizeInBytes < (ulong)((long)(sizeof(DualShock4GamepadHID.DualShock4HIDGenericInputReport) + 1)))
				{
					return false;
				}
				DualShock4HIDInputReport dualShock4HIDInputReport2 = ((DualShock4GamepadHID.DualShock4HIDGenericInputReport*)(state + 1))->ToHIDInputReport();
				*(DualShock4HIDInputReport*)ptr->state = dualShock4HIDInputReport2;
				ptr->stateFormat = DualShock4HIDInputReport.Format;
				return true;
			}
		}

		public void OnNextUpdate()
		{
		}

		public unsafe void OnStateEvent(InputEventPtr eventPtr)
		{
			if (eventPtr.type == 1398030676 && eventPtr.stateFormat == DualShock4HIDInputReport.Format)
			{
				DualShock4HIDInputReport* ptr = (DualShock4HIDInputReport*)((byte*)base.currentStatePtr + this.m_StateBlock.byteOffset);
				DualShock4HIDInputReport* state = (DualShock4HIDInputReport*)StateEvent.FromUnchecked(eventPtr)->state;
				if (state->leftStickX >= 120 && state->leftStickX <= 135 && state->leftStickY >= 120 && state->leftStickY <= 135 && state->rightStickX >= 120 && state->rightStickX <= 135 && state->rightStickY >= 120 && state->rightStickY <= 135 && state->leftTrigger == ptr->leftTrigger && state->rightTrigger == ptr->rightTrigger && state->buttons1 == ptr->buttons1 && state->buttons2 == ptr->buttons2 && state->buttons3 == ptr->buttons3)
				{
					InputSystem.s_Manager.DontMakeCurrentlyUpdatingDeviceCurrent();
				}
			}
			InputState.Change(this, eventPtr, InputUpdateType.None);
		}

		public bool GetStateOffsetForEvent(InputControl control, InputEventPtr eventPtr, ref uint offset)
		{
			return false;
		}

		private float? m_LowFrequencyMotorSpeed;

		private float? m_HighFrequenceyMotorSpeed;

		private Color? m_LightBarColor;

		internal const byte JitterMaskLow = 120;

		internal const byte JitterMaskHigh = 135;

		[StructLayout(LayoutKind.Explicit)]
		internal struct DualShock4HIDGenericInputReport
		{
			public static FourCC Format
			{
				get
				{
					return new FourCC('H', 'I', 'D', ' ');
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public DualShock4HIDInputReport ToHIDInputReport()
			{
				return new DualShock4HIDInputReport
				{
					leftStickX = this.leftStickX,
					leftStickY = this.leftStickY,
					rightStickX = this.rightStickX,
					rightStickY = this.rightStickY,
					leftTrigger = this.leftTrigger,
					rightTrigger = this.rightTrigger,
					buttons1 = this.buttons0,
					buttons2 = this.buttons1,
					buttons3 = this.buttons2
				};
			}

			[FieldOffset(0)]
			public byte leftStickX;

			[FieldOffset(1)]
			public byte leftStickY;

			[FieldOffset(2)]
			public byte rightStickX;

			[FieldOffset(3)]
			public byte rightStickY;

			[FieldOffset(4)]
			public byte buttons0;

			[FieldOffset(5)]
			public byte buttons1;

			[FieldOffset(6)]
			public byte buttons2;

			[FieldOffset(7)]
			public byte leftTrigger;

			[FieldOffset(8)]
			public byte rightTrigger;
		}
	}
}
