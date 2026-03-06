using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 9)]
	public struct EnableIMECompositionCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('I', 'M', 'E', 'M');
			}
		}

		public bool imeEnabled
		{
			get
			{
				return this.m_ImeEnabled > 0;
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return EnableIMECompositionCommand.Type;
			}
		}

		public static EnableIMECompositionCommand Create(bool enabled)
		{
			return new EnableIMECompositionCommand
			{
				baseCommand = new InputDeviceCommand(EnableIMECompositionCommand.Type, 9),
				m_ImeEnabled = (enabled ? byte.MaxValue : 0)
			};
		}

		internal const int kSize = 12;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		private byte m_ImeEnabled;
	}
}
