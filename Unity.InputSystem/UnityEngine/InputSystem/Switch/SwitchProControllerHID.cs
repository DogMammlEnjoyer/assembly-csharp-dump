using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Switch
{
	[InputControlLayout(stateType = typeof(SwitchProControllerHIDInputState), displayName = "Switch Pro Controller")]
	public class SwitchProControllerHID : Gamepad, IInputStateCallbackReceiver, IEventPreProcessor
	{
		[InputControl(name = "capture", displayName = "Capture")]
		public ButtonControl captureButton { get; protected set; }

		[InputControl(name = "home", displayName = "Home")]
		public ButtonControl homeButton { get; protected set; }

		protected override void OnAdded()
		{
			base.OnAdded();
			this.captureButton = base.GetChildControl<ButtonControl>("capture");
			this.homeButton = base.GetChildControl<ButtonControl>("home");
			this.HandshakeRestart();
		}

		private void HandshakeRestart()
		{
			this.m_HandshakeStepIndex = -1;
			this.m_HandshakeTimer = InputRuntime.s_Instance.currentTime;
		}

		private void HandshakeTick()
		{
			double currentTime = InputRuntime.s_Instance.currentTime;
			if (currentTime >= this.m_LastUpdateTimeInternal + 2.0 && currentTime >= this.m_HandshakeTimer + 2.0)
			{
				this.m_HandshakeStepIndex = 0;
			}
			else
			{
				if (this.m_HandshakeStepIndex + 1 >= SwitchProControllerHID.s_HandshakeSequence.Length)
				{
					return;
				}
				if (currentTime <= this.m_HandshakeTimer + 0.1)
				{
					return;
				}
				this.m_HandshakeStepIndex++;
			}
			this.m_HandshakeTimer = currentTime;
			SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType type = SwitchProControllerHID.s_HandshakeSequence[this.m_HandshakeStepIndex];
			SwitchProControllerHID.SwitchMagicOutputHIDBluetooth switchMagicOutputHIDBluetooth = SwitchProControllerHID.SwitchMagicOutputHIDBluetooth.Create(type);
			if (base.ExecuteCommand<SwitchProControllerHID.SwitchMagicOutputHIDBluetooth>(ref switchMagicOutputHIDBluetooth) > 0L)
			{
				return;
			}
			SwitchProControllerHID.SwitchMagicOutputHIDUSB switchMagicOutputHIDUSB = SwitchProControllerHID.SwitchMagicOutputHIDUSB.Create(type);
			base.ExecuteCommand<SwitchProControllerHID.SwitchMagicOutputHIDUSB>(ref switchMagicOutputHIDUSB);
		}

		public void OnNextUpdate()
		{
			this.HandshakeTick();
		}

		public unsafe void OnStateEvent(InputEventPtr eventPtr)
		{
			if (eventPtr.type == 1398030676 && eventPtr.stateFormat == SwitchProControllerHIDInputState.Format)
			{
				SwitchProControllerHIDInputState* ptr = (SwitchProControllerHIDInputState*)((byte*)base.currentStatePtr + this.m_StateBlock.byteOffset);
				SwitchProControllerHIDInputState* state = (SwitchProControllerHIDInputState*)StateEvent.FromUnchecked(eventPtr)->state;
				if (state->leftStickX >= 120 && state->leftStickX <= 135 && state->leftStickY >= 120 && state->leftStickY <= 135 && state->rightStickX >= 120 && state->rightStickX <= 135 && state->rightStickY >= 120 && state->rightStickY <= 135 && state->buttons1 == ptr->buttons1 && state->buttons2 == ptr->buttons2)
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

		public unsafe bool PreProcessEvent(InputEventPtr eventPtr)
		{
			if (eventPtr.type == 1145852993)
			{
				return DeltaStateEvent.FromUnchecked(eventPtr)->stateFormat == SwitchProControllerHIDInputState.Format;
			}
			if (eventPtr.type != 1398030676)
			{
				return true;
			}
			StateEvent* ptr = StateEvent.FromUnchecked(eventPtr);
			uint stateSizeInBytes = ptr->stateSizeInBytes;
			if (ptr->stateFormat == SwitchProControllerHIDInputState.Format)
			{
				return true;
			}
			if (ptr->stateFormat != SwitchProControllerHID.SwitchHIDGenericInputReport.Format || (ulong)stateSizeInBytes < (ulong)((long)sizeof(SwitchProControllerHID.SwitchHIDGenericInputReport)))
			{
				return false;
			}
			SwitchProControllerHID.SwitchHIDGenericInputReport* state = (SwitchProControllerHID.SwitchHIDGenericInputReport*)ptr->state;
			if (state->reportId == 63 && stateSizeInBytes >= 12U)
			{
				SwitchProControllerHIDInputState switchProControllerHIDInputState = ((SwitchProControllerHID.SwitchSimpleInputReport*)ptr->state)->ToHIDInputReport();
				*(SwitchProControllerHIDInputState*)ptr->state = switchProControllerHIDInputState;
				ptr->stateFormat = SwitchProControllerHIDInputState.Format;
				return true;
			}
			if (state->reportId == 48 && stateSizeInBytes >= 25U)
			{
				SwitchProControllerHIDInputState switchProControllerHIDInputState2 = ((SwitchProControllerHID.SwitchFullInputReport*)ptr->state)->ToHIDInputReport();
				*(SwitchProControllerHIDInputState*)ptr->state = switchProControllerHIDInputState2;
				ptr->stateFormat = SwitchProControllerHIDInputState.Format;
				return true;
			}
			if (stateSizeInBytes == 8U || stateSizeInBytes == 9U)
			{
				int num = (stateSizeInBytes == 9U) ? 1 : 0;
				SwitchProControllerHIDInputState switchProControllerHIDInputState3 = ((SwitchProControllerHID.SwitchInputOnlyReport*)((byte*)ptr->state + num))->ToHIDInputReport();
				*(SwitchProControllerHIDInputState*)ptr->state = switchProControllerHIDInputState3;
				ptr->stateFormat = SwitchProControllerHIDInputState.Format;
				return true;
			}
			return false;
		}

		private static readonly SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType[] s_HandshakeSequence = new SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType[]
		{
			SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType.Status,
			SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType.Handshake,
			SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType.Highspeed,
			SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType.Handshake,
			SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType.ForceUSB
		};

		private int m_HandshakeStepIndex;

		private double m_HandshakeTimer;

		internal const byte JitterMaskLow = 120;

		internal const byte JitterMaskHigh = 135;

		[StructLayout(LayoutKind.Explicit, Size = 7)]
		private struct SwitchInputOnlyReport
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public SwitchProControllerHIDInputState ToHIDInputReport()
			{
				SwitchProControllerHIDInputState result = new SwitchProControllerHIDInputState
				{
					leftStickX = this.leftX,
					leftStickY = this.leftY,
					rightStickX = this.rightX,
					rightStickY = this.rightY
				};
				result.Set(SwitchProControllerHIDInputState.Button.West, (this.buttons0 & 1) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.South, (this.buttons0 & 2) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.East, (this.buttons0 & 4) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.North, (this.buttons0 & 8) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.L, (this.buttons0 & 16) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.R, (this.buttons0 & 32) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.ZL, (this.buttons0 & 64) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.ZR, (this.buttons0 & 128) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Minus, (this.buttons1 & 1) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Plus, (this.buttons1 & 2) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.StickL, (this.buttons1 & 4) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.StickR, (this.buttons1 & 8) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Home, (this.buttons1 & 16) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Capture, (this.buttons1 & 32) > 0);
				bool state = false;
				bool state2 = false;
				bool state3 = false;
				bool state4 = false;
				switch (this.hat)
				{
				case 0:
					state2 = true;
					break;
				case 1:
					state2 = true;
					state3 = true;
					break;
				case 2:
					state3 = true;
					break;
				case 3:
					state4 = true;
					state3 = true;
					break;
				case 4:
					state4 = true;
					break;
				case 5:
					state4 = true;
					state = true;
					break;
				case 6:
					state = true;
					break;
				case 7:
					state2 = true;
					state = true;
					break;
				}
				result.Set(SwitchProControllerHIDInputState.Button.Left, state);
				result.Set(SwitchProControllerHIDInputState.Button.Up, state2);
				result.Set(SwitchProControllerHIDInputState.Button.Right, state3);
				result.Set(SwitchProControllerHIDInputState.Button.Down, state4);
				return result;
			}

			public const int kSize = 7;

			[FieldOffset(0)]
			public byte buttons0;

			[FieldOffset(1)]
			public byte buttons1;

			[FieldOffset(2)]
			public byte hat;

			[FieldOffset(3)]
			public byte leftX;

			[FieldOffset(4)]
			public byte leftY;

			[FieldOffset(5)]
			public byte rightX;

			[FieldOffset(6)]
			public byte rightY;
		}

		[StructLayout(LayoutKind.Explicit, Size = 12)]
		private struct SwitchSimpleInputReport
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public SwitchProControllerHIDInputState ToHIDInputReport()
			{
				byte leftStickX = (byte)NumberHelpers.RemapUIntBitsToNormalizeFloatToUIntBits((uint)this.leftX, 16U, 8U);
				byte leftStickY = (byte)NumberHelpers.RemapUIntBitsToNormalizeFloatToUIntBits((uint)this.leftY, 16U, 8U);
				byte rightStickX = (byte)NumberHelpers.RemapUIntBitsToNormalizeFloatToUIntBits((uint)this.rightX, 16U, 8U);
				byte rightStickY = (byte)NumberHelpers.RemapUIntBitsToNormalizeFloatToUIntBits((uint)this.rightY, 16U, 8U);
				SwitchProControllerHIDInputState result = new SwitchProControllerHIDInputState
				{
					leftStickX = leftStickX,
					leftStickY = leftStickY,
					rightStickX = rightStickX,
					rightStickY = rightStickY
				};
				result.Set(SwitchProControllerHIDInputState.Button.South, (this.buttons0 & 1) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.East, (this.buttons0 & 2) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.West, (this.buttons0 & 4) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.North, (this.buttons0 & 8) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.L, (this.buttons0 & 16) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.R, (this.buttons0 & 32) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.ZL, (this.buttons0 & 64) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.ZR, (this.buttons0 & 128) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Minus, (this.buttons1 & 1) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Plus, (this.buttons1 & 2) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.StickL, (this.buttons1 & 4) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.StickR, (this.buttons1 & 8) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Home, (this.buttons1 & 16) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Capture, (this.buttons1 & 32) > 0);
				bool state = false;
				bool state2 = false;
				bool state3 = false;
				bool state4 = false;
				switch (this.hat)
				{
				case 0:
					state2 = true;
					break;
				case 1:
					state2 = true;
					state3 = true;
					break;
				case 2:
					state3 = true;
					break;
				case 3:
					state4 = true;
					state3 = true;
					break;
				case 4:
					state4 = true;
					break;
				case 5:
					state4 = true;
					state = true;
					break;
				case 6:
					state = true;
					break;
				case 7:
					state2 = true;
					state = true;
					break;
				}
				result.Set(SwitchProControllerHIDInputState.Button.Left, state);
				result.Set(SwitchProControllerHIDInputState.Button.Up, state2);
				result.Set(SwitchProControllerHIDInputState.Button.Right, state3);
				result.Set(SwitchProControllerHIDInputState.Button.Down, state4);
				return result;
			}

			public const int kSize = 12;

			public const byte ExpectedReportId = 63;

			[FieldOffset(0)]
			public byte reportId;

			[FieldOffset(1)]
			public byte buttons0;

			[FieldOffset(2)]
			public byte buttons1;

			[FieldOffset(3)]
			public byte hat;

			[FieldOffset(4)]
			public ushort leftX;

			[FieldOffset(6)]
			public ushort leftY;

			[FieldOffset(8)]
			public ushort rightX;

			[FieldOffset(10)]
			public ushort rightY;
		}

		[StructLayout(LayoutKind.Explicit, Size = 25)]
		private struct SwitchFullInputReport
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public SwitchProControllerHIDInputState ToHIDInputReport()
			{
				uint value = (uint)((int)this.left0 | (int)(this.left1 & 15) << 8);
				uint value2 = (uint)((this.left1 & 240) >> 4 | (int)this.left2 << 4);
				uint value3 = (uint)((int)this.right0 | (int)(this.right1 & 15) << 8);
				uint value4 = (uint)((this.right1 & 240) >> 4 | (int)this.right2 << 4);
				byte leftStickX = (byte)NumberHelpers.RemapUIntBitsToNormalizeFloatToUIntBits(value, 12U, 8U);
				byte leftStickY = byte.MaxValue - (byte)NumberHelpers.RemapUIntBitsToNormalizeFloatToUIntBits(value2, 12U, 8U);
				byte rightStickX = (byte)NumberHelpers.RemapUIntBitsToNormalizeFloatToUIntBits(value3, 12U, 8U);
				byte rightStickY = byte.MaxValue - (byte)NumberHelpers.RemapUIntBitsToNormalizeFloatToUIntBits(value4, 12U, 8U);
				SwitchProControllerHIDInputState result = new SwitchProControllerHIDInputState
				{
					leftStickX = leftStickX,
					leftStickY = leftStickY,
					rightStickX = rightStickX,
					rightStickY = rightStickY
				};
				result.Set(SwitchProControllerHIDInputState.Button.West, (this.buttons0 & 1) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.North, (this.buttons0 & 2) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.South, (this.buttons0 & 4) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.East, (this.buttons0 & 8) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.R, (this.buttons0 & 64) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.ZR, (this.buttons0 & 128) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Minus, (this.buttons1 & 1) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Plus, (this.buttons1 & 2) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.StickR, (this.buttons1 & 4) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.StickL, (this.buttons1 & 8) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Home, (this.buttons1 & 16) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Capture, (this.buttons1 & 32) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Down, (this.buttons2 & 1) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Up, (this.buttons2 & 2) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Right, (this.buttons2 & 4) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.Left, (this.buttons2 & 8) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.L, (this.buttons2 & 64) > 0);
				result.Set(SwitchProControllerHIDInputState.Button.ZL, (this.buttons2 & 128) > 0);
				return result;
			}

			public const int kSize = 25;

			public const byte ExpectedReportId = 48;

			[FieldOffset(0)]
			public byte reportId;

			[FieldOffset(3)]
			public byte buttons0;

			[FieldOffset(4)]
			public byte buttons1;

			[FieldOffset(5)]
			public byte buttons2;

			[FieldOffset(6)]
			public byte left0;

			[FieldOffset(7)]
			public byte left1;

			[FieldOffset(8)]
			public byte left2;

			[FieldOffset(9)]
			public byte right0;

			[FieldOffset(10)]
			public byte right1;

			[FieldOffset(11)]
			public byte right2;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SwitchHIDGenericInputReport
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

		[StructLayout(LayoutKind.Explicit, Size = 49)]
		internal struct SwitchMagicOutputReport
		{
			public const int kSize = 49;

			public const byte ExpectedReplyInputReportId = 129;

			[FieldOffset(0)]
			public byte reportType;

			[FieldOffset(1)]
			public byte commandId;

			internal enum ReportType
			{
				Magic = 128
			}

			public enum CommandIdType
			{
				Status = 1,
				Handshake,
				Highspeed,
				ForceUSB
			}
		}

		[StructLayout(LayoutKind.Explicit, Size = 57)]
		internal struct SwitchMagicOutputHIDBluetooth : IInputDeviceCommandInfo
		{
			public static FourCC Type
			{
				get
				{
					return new FourCC('H', 'I', 'D', 'O');
				}
			}

			public FourCC typeStatic
			{
				get
				{
					return SwitchProControllerHID.SwitchMagicOutputHIDBluetooth.Type;
				}
			}

			public static SwitchProControllerHID.SwitchMagicOutputHIDBluetooth Create(SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType type)
			{
				return new SwitchProControllerHID.SwitchMagicOutputHIDBluetooth
				{
					baseCommand = new InputDeviceCommand(SwitchProControllerHID.SwitchMagicOutputHIDBluetooth.Type, 57),
					report = new SwitchProControllerHID.SwitchMagicOutputReport
					{
						reportType = 128,
						commandId = (byte)type
					}
				};
			}

			public const int kSize = 57;

			[FieldOffset(0)]
			public InputDeviceCommand baseCommand;

			[FieldOffset(8)]
			public SwitchProControllerHID.SwitchMagicOutputReport report;
		}

		[StructLayout(LayoutKind.Explicit, Size = 72)]
		internal struct SwitchMagicOutputHIDUSB : IInputDeviceCommandInfo
		{
			public static FourCC Type
			{
				get
				{
					return new FourCC('H', 'I', 'D', 'O');
				}
			}

			public FourCC typeStatic
			{
				get
				{
					return SwitchProControllerHID.SwitchMagicOutputHIDUSB.Type;
				}
			}

			public static SwitchProControllerHID.SwitchMagicOutputHIDUSB Create(SwitchProControllerHID.SwitchMagicOutputReport.CommandIdType type)
			{
				return new SwitchProControllerHID.SwitchMagicOutputHIDUSB
				{
					baseCommand = new InputDeviceCommand(SwitchProControllerHID.SwitchMagicOutputHIDUSB.Type, 72),
					report = new SwitchProControllerHID.SwitchMagicOutputReport
					{
						reportType = 128,
						commandId = (byte)type
					}
				};
			}

			public const int kSize = 72;

			[FieldOffset(0)]
			public InputDeviceCommand baseCommand;

			[FieldOffset(8)]
			public SwitchProControllerHID.SwitchMagicOutputReport report;
		}
	}
}
