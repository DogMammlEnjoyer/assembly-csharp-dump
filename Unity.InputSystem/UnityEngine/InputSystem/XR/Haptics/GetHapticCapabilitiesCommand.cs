using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.XR.Haptics
{
	[StructLayout(LayoutKind.Explicit, Size = 28)]
	public struct GetHapticCapabilitiesCommand : IInputDeviceCommandInfo
	{
		private static FourCC Type
		{
			get
			{
				return new FourCC('X', 'H', 'C', '0');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return GetHapticCapabilitiesCommand.Type;
			}
		}

		public HapticCapabilities capabilities
		{
			get
			{
				return new HapticCapabilities(this.numChannels, this.supportsImpulse, this.supportsBuffer, this.frequencyHz, this.maxBufferSize, this.optimalBufferSize);
			}
		}

		public static GetHapticCapabilitiesCommand Create()
		{
			return new GetHapticCapabilitiesCommand
			{
				baseCommand = new InputDeviceCommand(GetHapticCapabilitiesCommand.Type, 28)
			};
		}

		private const int kSize = 28;

		[FieldOffset(0)]
		private InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public uint numChannels;

		[FieldOffset(12)]
		public bool supportsImpulse;

		[FieldOffset(13)]
		public bool supportsBuffer;

		[FieldOffset(16)]
		public uint frequencyHz;

		[FieldOffset(20)]
		public uint maxBufferSize;

		[FieldOffset(24)]
		public uint optimalBufferSize;
	}
}
