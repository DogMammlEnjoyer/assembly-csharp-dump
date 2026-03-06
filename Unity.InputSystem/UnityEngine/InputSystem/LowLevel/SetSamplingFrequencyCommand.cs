using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	public struct SetSamplingFrequencyCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('S', 'S', 'P', 'L');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return SetSamplingFrequencyCommand.Type;
			}
		}

		public static SetSamplingFrequencyCommand Create(float frequency)
		{
			return new SetSamplingFrequencyCommand
			{
				baseCommand = new InputDeviceCommand(SetSamplingFrequencyCommand.Type, 12),
				frequency = frequency
			};
		}

		internal const int kSize = 12;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public float frequency;
	}
}
