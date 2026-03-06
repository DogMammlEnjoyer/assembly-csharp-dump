using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct DisableDeviceCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('D', 'S', 'B', 'L');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return DisableDeviceCommand.Type;
			}
		}

		public static DisableDeviceCommand Create()
		{
			return new DisableDeviceCommand
			{
				baseCommand = new InputDeviceCommand(DisableDeviceCommand.Type, 8)
			};
		}

		internal const int kSize = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;
	}
}
