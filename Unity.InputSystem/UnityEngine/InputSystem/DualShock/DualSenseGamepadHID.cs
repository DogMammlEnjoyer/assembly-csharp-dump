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
	[InputControlLayout(stateType = typeof(DualSenseHIDInputReport), displayName = "DualSense HID")]
	public class DualSenseGamepadHID : DualShockGamepad, IEventMerger, IEventPreProcessor, IInputStateCallbackReceiver
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
			if (this.m_LowFrequencyMotorSpeed == null && this.m_HighFrequenceyMotorSpeed == null)
			{
				return;
			}
			this.SetMotorSpeedsAndLightBarColor(new float?(0f), new float?(0f), this.m_LightBarColor);
		}

		public override void ResetHaptics()
		{
			if (this.m_LowFrequencyMotorSpeed == null && this.m_HighFrequenceyMotorSpeed == null)
			{
				return;
			}
			this.m_HighFrequenceyMotorSpeed = null;
			this.m_LowFrequencyMotorSpeed = null;
			this.SetMotorSpeedsAndLightBarColor(this.m_LowFrequencyMotorSpeed, this.m_HighFrequenceyMotorSpeed, this.m_LightBarColor);
		}

		public override void ResumeHaptics()
		{
			if (this.m_LowFrequencyMotorSpeed == null && this.m_HighFrequenceyMotorSpeed == null)
			{
				return;
			}
			this.SetMotorSpeedsAndLightBarColor(this.m_LowFrequencyMotorSpeed, this.m_HighFrequenceyMotorSpeed, this.m_LightBarColor);
		}

		public override void SetLightBarColor(Color color)
		{
			this.m_LightBarColor = new Color?(color);
			this.SetMotorSpeedsAndLightBarColor(this.m_LowFrequencyMotorSpeed, this.m_HighFrequenceyMotorSpeed, this.m_LightBarColor);
		}

		public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
		{
			this.m_LowFrequencyMotorSpeed = new float?(lowFrequency);
			this.m_HighFrequenceyMotorSpeed = new float?(highFrequency);
			this.SetMotorSpeedsAndLightBarColor(this.m_LowFrequencyMotorSpeed, this.m_HighFrequenceyMotorSpeed, this.m_LightBarColor);
		}

		public bool SetMotorSpeedsAndLightBarColor(float? lowFrequency, float? highFrequency, Color? color)
		{
			float value = (lowFrequency != null) ? lowFrequency.Value : 0f;
			float value2 = (highFrequency != null) ? highFrequency.Value : 0f;
			Color color2 = (color != null) ? color.Value : Color.black;
			DualSenseHIDUSBOutputReport dualSenseHIDUSBOutputReport = DualSenseHIDUSBOutputReport.Create(new DualSenseHIDOutputReportPayload
			{
				enableFlags1 = 3,
				enableFlags2 = 4,
				lowFrequencyMotorSpeed = (byte)NumberHelpers.NormalizedFloatToUInt(value, 0U, 255U),
				highFrequencyMotorSpeed = (byte)NumberHelpers.NormalizedFloatToUInt(value2, 0U, 255U),
				redColor = (byte)NumberHelpers.NormalizedFloatToUInt(color2.r, 0U, 255U),
				greenColor = (byte)NumberHelpers.NormalizedFloatToUInt(color2.g, 0U, 255U),
				blueColor = (byte)NumberHelpers.NormalizedFloatToUInt(color2.b, 0U, 255U)
			}, base.hidDescriptor.outputReportSize);
			return base.ExecuteCommand<DualSenseHIDUSBOutputReport>(ref dualSenseHIDUSBOutputReport) >= 0L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool MergeForward(DualSenseGamepadHID.DualSenseHIDUSBInputReport* currentState, DualSenseGamepadHID.DualSenseHIDUSBInputReport* nextState)
		{
			return currentState->buttons0 == nextState->buttons0 && currentState->buttons1 == nextState->buttons1 && currentState->buttons2 == nextState->buttons2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool MergeForward(DualSenseGamepadHID.DualSenseHIDBluetoothInputReport* currentState, DualSenseGamepadHID.DualSenseHIDBluetoothInputReport* nextState)
		{
			return currentState->buttons0 == nextState->buttons0 && currentState->buttons1 == nextState->buttons1 && currentState->buttons2 == nextState->buttons2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool MergeForward(DualSenseGamepadHID.DualSenseHIDMinimalInputReport* currentState, DualSenseGamepadHID.DualSenseHIDMinimalInputReport* nextState)
		{
			return currentState->buttons0 == nextState->buttons0 && currentState->buttons1 == nextState->buttons1 && currentState->buttons2 == nextState->buttons2;
		}

		unsafe bool IEventMerger.MergeForward(InputEventPtr currentEventPtr, InputEventPtr nextEventPtr)
		{
			if (currentEventPtr.type != 1398030676 || nextEventPtr.type != 1398030676)
			{
				return false;
			}
			StateEvent* ptr = StateEvent.FromUnchecked(currentEventPtr);
			StateEvent* ptr2 = StateEvent.FromUnchecked(nextEventPtr);
			if (ptr->stateFormat != DualSenseGamepadHID.DualSenseHIDGenericInputReport.Format || ptr2->stateFormat != DualSenseGamepadHID.DualSenseHIDGenericInputReport.Format)
			{
				return false;
			}
			if (ptr->stateSizeInBytes != ptr2->stateSizeInBytes)
			{
				return false;
			}
			DualSenseGamepadHID.DualSenseHIDGenericInputReport* state = (DualSenseGamepadHID.DualSenseHIDGenericInputReport*)ptr->state;
			DualSenseGamepadHID.DualSenseHIDGenericInputReport* state2 = (DualSenseGamepadHID.DualSenseHIDGenericInputReport*)ptr2->state;
			if (state->reportId != state2->reportId)
			{
				return false;
			}
			if (state->reportId == 1)
			{
				if ((ulong)ptr->stateSizeInBytes == (ulong)((long)DualSenseGamepadHID.DualSenseHIDMinimalInputReport.ExpectedSize1) || (ulong)ptr->stateSizeInBytes == (ulong)((long)DualSenseGamepadHID.DualSenseHIDMinimalInputReport.ExpectedSize2))
				{
					DualSenseGamepadHID.DualSenseHIDMinimalInputReport* state3 = (DualSenseGamepadHID.DualSenseHIDMinimalInputReport*)ptr->state;
					DualSenseGamepadHID.DualSenseHIDMinimalInputReport* state4 = (DualSenseGamepadHID.DualSenseHIDMinimalInputReport*)ptr2->state;
					return DualSenseGamepadHID.MergeForward(state3, state4);
				}
				DualSenseGamepadHID.DualSenseHIDUSBInputReport* state5 = (DualSenseGamepadHID.DualSenseHIDUSBInputReport*)ptr->state;
				DualSenseGamepadHID.DualSenseHIDUSBInputReport* state6 = (DualSenseGamepadHID.DualSenseHIDUSBInputReport*)ptr2->state;
				return DualSenseGamepadHID.MergeForward(state5, state6);
			}
			else
			{
				if (state->reportId == 49)
				{
					DualSenseGamepadHID.DualSenseHIDBluetoothInputReport* state7 = (DualSenseGamepadHID.DualSenseHIDBluetoothInputReport*)ptr->state;
					DualSenseGamepadHID.DualSenseHIDBluetoothInputReport* state8 = (DualSenseGamepadHID.DualSenseHIDBluetoothInputReport*)ptr2->state;
					return DualSenseGamepadHID.MergeForward(state7, state8);
				}
				return false;
			}
		}

		unsafe bool IEventPreProcessor.PreProcessEvent(InputEventPtr eventPtr)
		{
			if (eventPtr.type != 1398030676)
			{
				return eventPtr.type != 1145852993;
			}
			StateEvent* ptr = StateEvent.FromUnchecked(eventPtr);
			if (ptr->stateFormat == DualSenseHIDInputReport.Format)
			{
				return true;
			}
			uint stateSizeInBytes = ptr->stateSizeInBytes;
			if (ptr->stateFormat != DualSenseGamepadHID.DualSenseHIDGenericInputReport.Format || (ulong)stateSizeInBytes < (ulong)((long)sizeof(DualSenseHIDInputReport)))
			{
				return false;
			}
			DualSenseGamepadHID.DualSenseHIDGenericInputReport* state = (DualSenseGamepadHID.DualSenseHIDGenericInputReport*)ptr->state;
			if (state->reportId == 1)
			{
				if ((ulong)ptr->stateSizeInBytes == (ulong)((long)DualSenseGamepadHID.DualSenseHIDMinimalInputReport.ExpectedSize1) || (ulong)ptr->stateSizeInBytes == (ulong)((long)DualSenseGamepadHID.DualSenseHIDMinimalInputReport.ExpectedSize2))
				{
					DualSenseHIDInputReport dualSenseHIDInputReport = ((DualSenseGamepadHID.DualSenseHIDMinimalInputReport*)ptr->state)->ToHIDInputReport();
					*(DualSenseHIDInputReport*)ptr->state = dualSenseHIDInputReport;
				}
				else
				{
					DualSenseHIDInputReport dualSenseHIDInputReport2 = ((DualSenseGamepadHID.DualSenseHIDUSBInputReport*)ptr->state)->ToHIDInputReport();
					*(DualSenseHIDInputReport*)ptr->state = dualSenseHIDInputReport2;
				}
				ptr->stateFormat = DualSenseHIDInputReport.Format;
				return true;
			}
			if (state->reportId == 49)
			{
				DualSenseHIDInputReport dualSenseHIDInputReport3 = ((DualSenseGamepadHID.DualSenseHIDBluetoothInputReport*)ptr->state)->ToHIDInputReport();
				*(DualSenseHIDInputReport*)ptr->state = dualSenseHIDInputReport3;
				ptr->stateFormat = DualSenseHIDInputReport.Format;
				return true;
			}
			return false;
		}

		public void OnNextUpdate()
		{
		}

		public unsafe void OnStateEvent(InputEventPtr eventPtr)
		{
			if (eventPtr.type == 1398030676 && eventPtr.stateFormat == DualSenseHIDInputReport.Format)
			{
				DualSenseHIDInputReport* ptr = (DualSenseHIDInputReport*)((byte*)base.currentStatePtr + this.m_StateBlock.byteOffset);
				DualSenseHIDInputReport* state = (DualSenseHIDInputReport*)StateEvent.FromUnchecked(eventPtr)->state;
				if (state->leftStickX >= 120 && state->leftStickX <= 135 && state->leftStickY >= 120 && state->leftStickY <= 135 && state->rightStickX >= 120 && state->rightStickX <= 135 && state->rightStickY >= 120 && state->rightStickY <= 135 && state->leftTrigger == ptr->leftTrigger && state->rightTrigger == ptr->rightTrigger && state->buttons0 == ptr->buttons0 && state->buttons1 == ptr->buttons1 && state->buttons2 == ptr->buttons2)
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

		protected Color? m_LightBarColor;

		private byte outputSequenceId;

		internal const byte JitterMaskLow = 120;

		internal const byte JitterMaskHigh = 135;

		[StructLayout(LayoutKind.Explicit)]
		internal struct DualSenseHIDGenericInputReport
		{
			public static FourCC Format
			{
				get
				{
					return new FourCC('H', 'I', 'D', ' ');
				}
			}

			[FieldOffset(0)]
			public byte reportId;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct DualSenseHIDUSBInputReport
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public DualSenseHIDInputReport ToHIDInputReport()
			{
				return new DualSenseHIDInputReport
				{
					leftStickX = this.leftStickX,
					leftStickY = this.leftStickY,
					rightStickX = this.rightStickX,
					rightStickY = this.rightStickY,
					leftTrigger = this.leftTrigger,
					rightTrigger = this.rightTrigger,
					buttons0 = this.buttons0,
					buttons1 = this.buttons1,
					buttons2 = (this.buttons2 & 7)
				};
			}

			public const int ExpectedReportId = 1;

			[FieldOffset(0)]
			public byte reportId;

			[FieldOffset(1)]
			public byte leftStickX;

			[FieldOffset(2)]
			public byte leftStickY;

			[FieldOffset(3)]
			public byte rightStickX;

			[FieldOffset(4)]
			public byte rightStickY;

			[FieldOffset(5)]
			public byte leftTrigger;

			[FieldOffset(6)]
			public byte rightTrigger;

			[FieldOffset(8)]
			public byte buttons0;

			[FieldOffset(9)]
			public byte buttons1;

			[FieldOffset(10)]
			public byte buttons2;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct DualSenseHIDBluetoothInputReport
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public DualSenseHIDInputReport ToHIDInputReport()
			{
				return new DualSenseHIDInputReport
				{
					leftStickX = this.leftStickX,
					leftStickY = this.leftStickY,
					rightStickX = this.rightStickX,
					rightStickY = this.rightStickY,
					leftTrigger = this.leftTrigger,
					rightTrigger = this.rightTrigger,
					buttons0 = this.buttons0,
					buttons1 = this.buttons1,
					buttons2 = (this.buttons2 & 7)
				};
			}

			public const int ExpectedReportId = 49;

			[FieldOffset(0)]
			public byte reportId;

			[FieldOffset(2)]
			public byte leftStickX;

			[FieldOffset(3)]
			public byte leftStickY;

			[FieldOffset(4)]
			public byte rightStickX;

			[FieldOffset(5)]
			public byte rightStickY;

			[FieldOffset(6)]
			public byte leftTrigger;

			[FieldOffset(7)]
			public byte rightTrigger;

			[FieldOffset(9)]
			public byte buttons0;

			[FieldOffset(10)]
			public byte buttons1;

			[FieldOffset(11)]
			public byte buttons2;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct DualSenseHIDMinimalInputReport
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public DualSenseHIDInputReport ToHIDInputReport()
			{
				return new DualSenseHIDInputReport
				{
					leftStickX = this.leftStickX,
					leftStickY = this.leftStickY,
					rightStickX = this.rightStickX,
					rightStickY = this.rightStickY,
					leftTrigger = this.leftTrigger,
					rightTrigger = this.rightTrigger,
					buttons0 = this.buttons0,
					buttons1 = this.buttons1,
					buttons2 = (this.buttons2 & 3)
				};
			}

			public static int ExpectedSize1 = 10;

			public static int ExpectedSize2 = 78;

			[FieldOffset(0)]
			public byte reportId;

			[FieldOffset(1)]
			public byte leftStickX;

			[FieldOffset(2)]
			public byte leftStickY;

			[FieldOffset(3)]
			public byte rightStickX;

			[FieldOffset(4)]
			public byte rightStickY;

			[FieldOffset(5)]
			public byte buttons0;

			[FieldOffset(6)]
			public byte buttons1;

			[FieldOffset(7)]
			public byte buttons2;

			[FieldOffset(8)]
			public byte leftTrigger;

			[FieldOffset(9)]
			public byte rightTrigger;
		}
	}
}
