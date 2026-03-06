using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	internal struct WarpMousePositionCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('W', 'P', 'M', 'S');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return WarpMousePositionCommand.Type;
			}
		}

		public static WarpMousePositionCommand Create(Vector2 position)
		{
			return new WarpMousePositionCommand
			{
				baseCommand = new InputDeviceCommand(WarpMousePositionCommand.Type, 16),
				warpPositionInPlayerDisplaySpace = position
			};
		}

		internal const int kSize = 16;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public Vector2 warpPositionInPlayerDisplaySpace;
	}
}
