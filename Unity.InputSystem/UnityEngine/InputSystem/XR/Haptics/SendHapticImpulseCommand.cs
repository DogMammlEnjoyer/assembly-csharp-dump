using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.XR.Haptics
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct SendHapticImpulseCommand : IInputDeviceCommandInfo
	{
		private static FourCC Type
		{
			get
			{
				return new FourCC('X', 'H', 'I', '0');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return SendHapticImpulseCommand.Type;
			}
		}

		public static SendHapticImpulseCommand Create(int motorChannel, float motorAmplitude, float motorDuration)
		{
			return new SendHapticImpulseCommand
			{
				baseCommand = new InputDeviceCommand(SendHapticImpulseCommand.Type, 20),
				channel = motorChannel,
				amplitude = motorAmplitude,
				duration = motorDuration
			};
		}

		private const int kSize = 20;

		[FieldOffset(0)]
		private InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		private int channel;

		[FieldOffset(12)]
		private float amplitude;

		[FieldOffset(16)]
		private float duration;
	}
}
