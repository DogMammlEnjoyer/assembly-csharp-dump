using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct InputDeviceCommand : IInputDeviceCommandInfo
	{
		public int payloadSizeInBytes
		{
			get
			{
				return this.sizeInBytes - 8;
			}
		}

		public unsafe void* payloadPtr
		{
			get
			{
				fixed (InputDeviceCommand* ptr = &this)
				{
					void* ptr2 = (void*)ptr;
					return (void*)((byte*)ptr2 + 8);
				}
			}
		}

		public InputDeviceCommand(FourCC type, int sizeInBytes = 8)
		{
			this.type = type;
			this.sizeInBytes = sizeInBytes;
		}

		public unsafe static NativeArray<byte> AllocateNative(FourCC type, int payloadSize)
		{
			int length = payloadSize + 8;
			NativeArray<byte> nativeArray = new NativeArray<byte>(length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			InputDeviceCommand* unsafePtr = (InputDeviceCommand*)nativeArray.GetUnsafePtr<byte>();
			unsafePtr->type = type;
			unsafePtr->sizeInBytes = length;
			return nativeArray;
		}

		public FourCC typeStatic
		{
			get
			{
				return default(FourCC);
			}
		}

		internal const int kBaseCommandSize = 8;

		public const int BaseCommandSize = 8;

		public const long GenericFailure = -1L;

		public const long GenericSuccess = 1L;

		[FieldOffset(0)]
		public FourCC type;

		[FieldOffset(4)]
		public int sizeInBytes;
	}
}
