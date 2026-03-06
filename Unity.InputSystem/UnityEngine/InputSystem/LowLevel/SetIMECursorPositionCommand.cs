using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct SetIMECursorPositionCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('I', 'M', 'E', 'P');
			}
		}

		public Vector2 position
		{
			get
			{
				return this.m_Position;
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return SetIMECursorPositionCommand.Type;
			}
		}

		public static SetIMECursorPositionCommand Create(Vector2 cursorPosition)
		{
			return new SetIMECursorPositionCommand
			{
				baseCommand = new InputDeviceCommand(SetIMECursorPositionCommand.Type, 16),
				m_Position = cursorPosition
			};
		}

		internal const int kSize = 16;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		private Vector2 m_Position;
	}
}
