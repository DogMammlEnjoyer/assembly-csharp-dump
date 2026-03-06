using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct RequestResetCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('R', 'S', 'E', 'T');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return RequestResetCommand.Type;
			}
		}

		public static RequestResetCommand Create()
		{
			return new RequestResetCommand
			{
				baseCommand = new InputDeviceCommand(RequestResetCommand.Type, 8)
			};
		}

		internal const int kSize = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;
	}
}
