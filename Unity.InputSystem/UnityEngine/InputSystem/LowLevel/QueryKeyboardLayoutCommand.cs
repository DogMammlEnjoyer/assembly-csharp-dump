using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 264)]
	public struct QueryKeyboardLayoutCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('K', 'B', 'L', 'T');
			}
		}

		public unsafe string ReadLayoutName()
		{
			fixed (QueryKeyboardLayoutCommand* ptr = &this)
			{
				return StringHelpers.ReadStringFromBuffer(new IntPtr((void*)(&ptr->nameBuffer.FixedElementField)), 256);
			}
		}

		public unsafe void WriteLayoutName(string name)
		{
			fixed (QueryKeyboardLayoutCommand* ptr = &this)
			{
				QueryKeyboardLayoutCommand* ptr2 = ptr;
				StringHelpers.WriteStringToBuffer(name, new IntPtr((void*)(&ptr2->nameBuffer.FixedElementField)), 256);
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return QueryKeyboardLayoutCommand.Type;
			}
		}

		public static QueryKeyboardLayoutCommand Create()
		{
			return new QueryKeyboardLayoutCommand
			{
				baseCommand = new InputDeviceCommand(QueryKeyboardLayoutCommand.Type, 264)
			};
		}

		internal const int kMaxNameLength = 256;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FixedBuffer(typeof(byte), 256)]
		[FieldOffset(8)]
		public QueryKeyboardLayoutCommand.<nameBuffer>e__FixedBuffer nameBuffer;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 256)]
		public struct <nameBuffer>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
