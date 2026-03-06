using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	internal struct QuerySamplingFrequencyCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('S', 'M', 'P', 'L');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return QuerySamplingFrequencyCommand.Type;
			}
		}

		public static QuerySamplingFrequencyCommand Create()
		{
			return new QuerySamplingFrequencyCommand
			{
				baseCommand = new InputDeviceCommand(QuerySamplingFrequencyCommand.Type, 12)
			};
		}

		internal const int kSize = 12;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public float frequency;
	}
}
