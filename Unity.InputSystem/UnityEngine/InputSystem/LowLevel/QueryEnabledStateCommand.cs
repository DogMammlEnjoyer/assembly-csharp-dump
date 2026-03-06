using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 9)]
	public struct QueryEnabledStateCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('Q', 'E', 'N', 'B');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return QueryEnabledStateCommand.Type;
			}
		}

		public static QueryEnabledStateCommand Create()
		{
			return new QueryEnabledStateCommand
			{
				baseCommand = new InputDeviceCommand(QueryEnabledStateCommand.Type, 9)
			};
		}

		internal const int kSize = 9;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public bool isEnabled;
	}
}
