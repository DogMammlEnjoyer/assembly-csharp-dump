using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 9)]
	public struct QueryCanRunInBackground : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('Q', 'R', 'I', 'B');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return QueryCanRunInBackground.Type;
			}
		}

		public static QueryCanRunInBackground Create()
		{
			return new QueryCanRunInBackground
			{
				baseCommand = new InputDeviceCommand(QueryCanRunInBackground.Type, 9),
				canRunInBackground = false
			};
		}

		internal const int kSize = 9;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public bool canRunInBackground;
	}
}
