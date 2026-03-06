using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.XR.Haptics
{
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct GetCurrentHapticStateCommand : IInputDeviceCommandInfo
	{
		private static FourCC Type
		{
			get
			{
				return new FourCC('X', 'H', 'S', '0');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return GetCurrentHapticStateCommand.Type;
			}
		}

		public HapticState currentState
		{
			get
			{
				return new HapticState(this.samplesQueued, this.samplesAvailable);
			}
		}

		public static GetCurrentHapticStateCommand Create()
		{
			return new GetCurrentHapticStateCommand
			{
				baseCommand = new InputDeviceCommand(GetCurrentHapticStateCommand.Type, 16)
			};
		}

		private const int kSize = 16;

		[FieldOffset(0)]
		private InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public uint samplesQueued;

		[FieldOffset(12)]
		public uint samplesAvailable;
	}
}
