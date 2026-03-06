using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct RequestSyncCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('S', 'Y', 'N', 'C');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return RequestSyncCommand.Type;
			}
		}

		public static RequestSyncCommand Create()
		{
			return new RequestSyncCommand
			{
				baseCommand = new InputDeviceCommand(RequestSyncCommand.Type, 8)
			};
		}

		internal const int kSize = 8;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;
	}
}
