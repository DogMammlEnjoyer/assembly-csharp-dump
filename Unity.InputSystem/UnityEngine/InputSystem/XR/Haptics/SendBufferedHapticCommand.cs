using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.XR.Haptics
{
	[StructLayout(LayoutKind.Explicit, Size = 1040)]
	public struct SendBufferedHapticCommand : IInputDeviceCommandInfo
	{
		private static FourCC Type
		{
			get
			{
				return new FourCC('X', 'H', 'U', '0');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return SendBufferedHapticCommand.Type;
			}
		}

		public unsafe static SendBufferedHapticCommand Create(byte[] rumbleBuffer)
		{
			if (rumbleBuffer == null)
			{
				throw new ArgumentNullException("rumbleBuffer");
			}
			int num = Mathf.Min(1024, rumbleBuffer.Length);
			SendBufferedHapticCommand result = new SendBufferedHapticCommand
			{
				baseCommand = new InputDeviceCommand(SendBufferedHapticCommand.Type, 1040),
				bufferSize = num
			};
			SendBufferedHapticCommand* ptr = &result;
			fixed (byte[] array = rumbleBuffer)
			{
				byte* ptr2;
				if (rumbleBuffer == null || array.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array[0];
				}
				for (int i = 0; i < num; i++)
				{
					*(ref ptr->buffer.FixedElementField + i) = ptr2[i];
				}
			}
			return result;
		}

		private const int kMaxHapticBufferSize = 1024;

		private const int kSize = 1040;

		[FieldOffset(0)]
		private InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		private int channel;

		[FieldOffset(12)]
		private int bufferSize;

		[FixedBuffer(typeof(byte), 1024)]
		[FieldOffset(16)]
		private SendBufferedHapticCommand.<buffer>e__FixedBuffer buffer;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 1024)]
		public struct <buffer>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
