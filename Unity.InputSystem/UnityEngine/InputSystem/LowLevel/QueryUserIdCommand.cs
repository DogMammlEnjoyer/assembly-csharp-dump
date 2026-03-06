using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 520)]
	internal struct QueryUserIdCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('U', 'S', 'E', 'R');
			}
		}

		public unsafe string ReadId()
		{
			fixed (QueryUserIdCommand* ptr = &this)
			{
				return StringHelpers.ReadStringFromBuffer(new IntPtr((void*)(&ptr->idBuffer.FixedElementField)), 256);
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return QueryUserIdCommand.Type;
			}
		}

		public static QueryUserIdCommand Create()
		{
			return new QueryUserIdCommand
			{
				baseCommand = new InputDeviceCommand(QueryUserIdCommand.Type, 520)
			};
		}

		public const int kMaxIdLength = 256;

		internal const int kSize = 520;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FixedBuffer(typeof(byte), 512)]
		[FieldOffset(8)]
		public QueryUserIdCommand.<idBuffer>e__FixedBuffer idBuffer;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 512)]
		public struct <idBuffer>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
