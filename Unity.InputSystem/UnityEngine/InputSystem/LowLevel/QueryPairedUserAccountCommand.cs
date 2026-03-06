using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 1040)]
	public struct QueryPairedUserAccountCommand : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('P', 'A', 'C', 'C');
			}
		}

		public unsafe string id
		{
			get
			{
				fixed (byte* ptr = &this.idBuffer.FixedElementField)
				{
					return StringHelpers.ReadStringFromBuffer(new IntPtr((void*)ptr), 256);
				}
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length > 256)
				{
					throw new ArgumentException(string.Format("ID '{0}' exceeds maximum supported length of {1} characters", value, 256), "value");
				}
				fixed (byte* ptr = &this.idBuffer.FixedElementField)
				{
					byte* value2 = ptr;
					StringHelpers.WriteStringToBuffer(value, new IntPtr((void*)value2), 256);
				}
			}
		}

		public unsafe string name
		{
			get
			{
				fixed (byte* ptr = &this.nameBuffer.FixedElementField)
				{
					return StringHelpers.ReadStringFromBuffer(new IntPtr((void*)ptr), 256);
				}
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length > 256)
				{
					throw new ArgumentException(string.Format("Name '{0}' exceeds maximum supported length of {1} characters", value, 256), "value");
				}
				fixed (byte* ptr = &this.nameBuffer.FixedElementField)
				{
					byte* value2 = ptr;
					StringHelpers.WriteStringToBuffer(value, new IntPtr((void*)value2), 256);
				}
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return QueryPairedUserAccountCommand.Type;
			}
		}

		public static QueryPairedUserAccountCommand Create()
		{
			return new QueryPairedUserAccountCommand
			{
				baseCommand = new InputDeviceCommand(QueryPairedUserAccountCommand.Type, 1040)
			};
		}

		internal const int kMaxNameLength = 256;

		internal const int kMaxIdLength = 256;

		internal const int kSize = 1040;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public ulong handle;

		[FixedBuffer(typeof(byte), 512)]
		[FieldOffset(16)]
		internal QueryPairedUserAccountCommand.<nameBuffer>e__FixedBuffer nameBuffer;

		[FixedBuffer(typeof(byte), 512)]
		[FieldOffset(528)]
		internal QueryPairedUserAccountCommand.<idBuffer>e__FixedBuffer idBuffer;

		[Flags]
		public enum Result : long
		{
			DevicePairedToUserAccount = 2L,
			UserAccountSelectionInProgress = 4L,
			UserAccountSelectionComplete = 8L,
			UserAccountSelectionCanceled = 16L
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 512)]
		public struct <idBuffer>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 512)]
		public struct <nameBuffer>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
