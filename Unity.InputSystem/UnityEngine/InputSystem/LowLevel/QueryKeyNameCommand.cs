using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 268)]
	public struct QueryKeyNameCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('K', 'Y', 'C', 'F');
			}
		}

		public unsafe string ReadKeyName()
		{
			fixed (QueryKeyNameCommand* ptr = &this)
			{
				return StringHelpers.ReadStringFromBuffer(new IntPtr((void*)(&ptr->nameBuffer.FixedElementField)), 256);
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return QueryKeyNameCommand.Type;
			}
		}

		public static QueryKeyNameCommand Create(Key key)
		{
			return new QueryKeyNameCommand
			{
				baseCommand = new InputDeviceCommand(QueryKeyNameCommand.Type, 268),
				scanOrKeyCode = (int)key
			};
		}

		internal const int kMaxNameLength = 256;

		internal const int kSize = 268;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public int scanOrKeyCode;

		[FixedBuffer(typeof(byte), 256)]
		[FieldOffset(12)]
		public QueryKeyNameCommand.<nameBuffer>e__FixedBuffer nameBuffer;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 256)]
		public struct <nameBuffer>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
