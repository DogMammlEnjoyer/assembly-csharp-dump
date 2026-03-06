using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 29)]
	public struct DeltaStateEvent : IInputEventTypeInfo
	{
		public uint deltaStateSizeInBytes
		{
			get
			{
				return this.baseEvent.sizeInBytes - 28U;
			}
		}

		public unsafe void* deltaState
		{
			get
			{
				fixed (byte* ptr = &this.stateData.FixedElementField)
				{
					return (void*)ptr;
				}
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return 1145852993;
			}
		}

		public unsafe InputEventPtr ToEventPtr()
		{
			fixed (DeltaStateEvent* ptr = &this)
			{
				return new InputEventPtr((InputEvent*)ptr);
			}
		}

		public unsafe static DeltaStateEvent* From(InputEventPtr ptr)
		{
			if (!ptr.valid)
			{
				throw new ArgumentNullException("ptr");
			}
			if (!ptr.IsA<DeltaStateEvent>())
			{
				throw new InvalidCastException(string.Format("Cannot cast event with type '{0}' into DeltaStateEvent", ptr.type));
			}
			return DeltaStateEvent.FromUnchecked(ptr);
		}

		internal unsafe static DeltaStateEvent* FromUnchecked(InputEventPtr ptr)
		{
			return (DeltaStateEvent*)ptr.data;
		}

		public unsafe static NativeArray<byte> From(InputControl control, out InputEventPtr eventPtr, Allocator allocator = Allocator.Temp)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputDevice device = control.device;
			if (!device.added)
			{
				throw new ArgumentException(string.Format("Device for control '{0}' has not been added to system", control), "control");
			}
			ref InputStateBlock ptr = ref device.m_StateBlock;
			ref InputStateBlock ptr2 = ref control.m_StateBlock;
			FourCC format = ptr.format;
			uint num;
			if (ptr2.bitOffset != 0U)
			{
				num = (ptr2.bitOffset + ptr2.sizeInBits + 7U) / 8U;
			}
			else
			{
				num = ptr2.alignedSizeInBytes;
			}
			uint byteOffset = ptr2.byteOffset;
			byte* source = (byte*)control.currentStatePtr + byteOffset;
			uint num2 = 28U + num;
			NativeArray<byte> nativeArray = new NativeArray<byte>((int)num2.AlignToMultipleOf(4U), allocator, NativeArrayOptions.ClearMemory);
			DeltaStateEvent* unsafePtr = (DeltaStateEvent*)nativeArray.GetUnsafePtr<byte>();
			unsafePtr->baseEvent = new InputEvent(1145852993, (int)num2, device.deviceId, InputRuntime.s_Instance.currentTime);
			unsafePtr->stateFormat = format;
			unsafePtr->stateOffset = ptr2.byteOffset - ptr.byteOffset;
			UnsafeUtility.MemCpy(unsafePtr->deltaState, (void*)source, (long)((ulong)num));
			eventPtr = unsafePtr->ToEventPtr();
			return nativeArray;
		}

		public const int Type = 1145852993;

		[FieldOffset(0)]
		public InputEvent baseEvent;

		[FieldOffset(20)]
		public FourCC stateFormat;

		[FieldOffset(24)]
		public uint stateOffset;

		[FixedBuffer(typeof(byte), 1)]
		[FieldOffset(28)]
		internal DeltaStateEvent.<stateData>e__FixedBuffer stateData;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct <stateData>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
