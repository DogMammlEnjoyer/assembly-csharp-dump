using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 25)]
	public struct StateEvent : IInputEventTypeInfo
	{
		public uint stateSizeInBytes
		{
			get
			{
				return this.baseEvent.sizeInBytes - 24U;
			}
		}

		public unsafe void* state
		{
			get
			{
				fixed (byte* ptr = &this.stateData.FixedElementField)
				{
					return (void*)ptr;
				}
			}
		}

		public unsafe InputEventPtr ToEventPtr()
		{
			fixed (StateEvent* ptr = &this)
			{
				return new InputEventPtr((InputEvent*)ptr);
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return 1398030676;
			}
		}

		public TState GetState<TState>() where TState : struct, IInputStateTypeInfo
		{
			TState result = default(TState);
			if (this.stateFormat != result.format)
			{
				throw new InvalidOperationException(string.Format("Expected state format '{0}' but got '{1}' instead", result.format, this.stateFormat));
			}
			UnsafeUtility.MemCpy(UnsafeUtility.AddressOf<TState>(ref result), this.state, Math.Min((long)((ulong)this.stateSizeInBytes), (long)UnsafeUtility.SizeOf<TState>()));
			return result;
		}

		public unsafe static TState GetState<TState>(InputEventPtr ptr) where TState : struct, IInputStateTypeInfo
		{
			return StateEvent.From(ptr)->GetState<TState>();
		}

		public static int GetEventSizeWithPayload<TState>() where TState : struct
		{
			return UnsafeUtility.SizeOf<TState>() + 20 + 4;
		}

		public unsafe static StateEvent* From(InputEventPtr ptr)
		{
			if (!ptr.valid)
			{
				throw new ArgumentNullException("ptr");
			}
			if (!ptr.IsA<StateEvent>())
			{
				throw new InvalidCastException(string.Format("Cannot cast event with type '{0}' into StateEvent", ptr.type));
			}
			return StateEvent.FromUnchecked(ptr);
		}

		internal unsafe static StateEvent* FromUnchecked(InputEventPtr ptr)
		{
			return (StateEvent*)ptr.data;
		}

		public static NativeArray<byte> From(InputDevice device, out InputEventPtr eventPtr, Allocator allocator = Allocator.Temp)
		{
			return StateEvent.From(device, out eventPtr, allocator, false);
		}

		public static NativeArray<byte> FromDefaultStateFor(InputDevice device, out InputEventPtr eventPtr, Allocator allocator = Allocator.Temp)
		{
			return StateEvent.From(device, out eventPtr, allocator, true);
		}

		private unsafe static NativeArray<byte> From(InputDevice device, out InputEventPtr eventPtr, Allocator allocator, bool useDefaultState)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (!device.added)
			{
				throw new ArgumentException(string.Format("Device '{0}' has not been added to system", device), "device");
			}
			FourCC format = device.m_StateBlock.format;
			uint alignedSizeInBytes = device.m_StateBlock.alignedSizeInBytes;
			uint byteOffset = device.m_StateBlock.byteOffset;
			byte* source = (byte*)((useDefaultState ? device.defaultStatePtr : device.currentStatePtr) + byteOffset);
			uint num = 24U + alignedSizeInBytes;
			NativeArray<byte> nativeArray = new NativeArray<byte>((int)num.AlignToMultipleOf(4U), allocator, NativeArrayOptions.ClearMemory);
			StateEvent* unsafePtr = (StateEvent*)nativeArray.GetUnsafePtr<byte>();
			unsafePtr->baseEvent = new InputEvent(1398030676, (int)num, device.deviceId, InputRuntime.s_Instance.currentTime);
			unsafePtr->stateFormat = format;
			UnsafeUtility.MemCpy(unsafePtr->state, (void*)source, (long)((ulong)alignedSizeInBytes));
			eventPtr = unsafePtr->ToEventPtr();
			return nativeArray;
		}

		public const int Type = 1398030676;

		internal const int kStateDataSizeToSubtract = 1;

		[FieldOffset(0)]
		public InputEvent baseEvent;

		[FieldOffset(20)]
		public FourCC stateFormat;

		[FixedBuffer(typeof(byte), 1)]
		[FieldOffset(24)]
		internal StateEvent.<stateData>e__FixedBuffer stateData;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct <stateData>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
