using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct EnableDeviceCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('E', 'N', 'B', 'L');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return EnableDeviceCommand.Type;
			}
		}

		public static EnableDeviceCommand Create()
		{
			return new EnableDeviceCommand
			{
				baseCommand = new InputDeviceCommand(EnableDeviceCommand.Type, 8)
			};
		}

		internal const int kSize = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;
	}
}
