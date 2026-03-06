using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;
using UnityEngineInternal.Input;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 20)]
	public struct InputEvent
	{
		public FourCC type
		{
			get
			{
				return new FourCC((int)this.m_Event.type);
			}
			set
			{
				this.m_Event.type = (NativeInputEventType)value;
			}
		}

		public uint sizeInBytes
		{
			get
			{
				return (uint)this.m_Event.sizeInBytes;
			}
			set
			{
				if (value > 65535U)
				{
					throw new ArgumentException("Maximum event size is " + ushort.MaxValue.ToString(), "value");
				}
				this.m_Event.sizeInBytes = (ushort)value;
			}
		}

		public int eventId
		{
			get
			{
				return (int)((long)this.m_Event.eventId & 2147483647L);
			}
			set
			{
				this.m_Event.eventId = (value | (int)((long)this.m_Event.eventId & (long)((ulong)int.MinValue)));
			}
		}

		public int deviceId
		{
			get
			{
				return (int)this.m_Event.deviceId;
			}
			set
			{
				this.m_Event.deviceId = (ushort)value;
			}
		}

		public double time
		{
			get
			{
				return this.m_Event.time - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
			set
			{
				this.m_Event.time = value + InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
		}

		internal double internalTime
		{
			get
			{
				return this.m_Event.time;
			}
			set
			{
				this.m_Event.time = value;
			}
		}

		public InputEvent(FourCC type, int sizeInBytes, int deviceId, double time = -1.0)
		{
			if (time < 0.0)
			{
				time = InputRuntime.s_Instance.currentTime;
			}
			this.m_Event.type = (NativeInputEventType)type;
			this.m_Event.sizeInBytes = (ushort)sizeInBytes;
			this.m_Event.deviceId = (ushort)deviceId;
			this.m_Event.time = time;
			this.m_Event.eventId = 0;
		}

		public bool handled
		{
			get
			{
				return ((long)this.m_Event.eventId & (long)((ulong)int.MinValue)) == (long)((ulong)int.MinValue);
			}
			set
			{
				if (value)
				{
					this.m_Event.eventId = (int)((long)this.m_Event.eventId | (long)((ulong)int.MinValue));
					return;
				}
				this.m_Event.eventId = (int)((long)this.m_Event.eventId & 2147483647L);
			}
		}

		public override string ToString()
		{
			return string.Format("id={0} type={1} device={2} size={3} time={4}", new object[]
			{
				this.eventId,
				this.type,
				this.deviceId,
				this.sizeInBytes,
				this.time
			});
		}

		internal unsafe static InputEvent* GetNextInMemory(InputEvent* currentPtr)
		{
			uint num = currentPtr->sizeInBytes.AlignToMultipleOf(4U);
			return currentPtr + num / (uint)sizeof(InputEvent);
		}

		internal unsafe static InputEvent* GetNextInMemoryChecked(InputEvent* currentPtr, ref InputEventBuffer buffer)
		{
			uint num = currentPtr->sizeInBytes.AlignToMultipleOf(4U);
			InputEvent* ptr = currentPtr + num / (uint)sizeof(InputEvent);
			if (!buffer.Contains(ptr))
			{
				throw new InvalidOperationException(string.Format("Event '{0}' is last event in given buffer with size {1}", new InputEventPtr(currentPtr), buffer.sizeInBytes));
			}
			return ptr;
		}

		public unsafe static bool Equals(InputEvent* first, InputEvent* second)
		{
			return first == second || (first != null && second != null && first->m_Event.sizeInBytes == second->m_Event.sizeInBytes && UnsafeUtility.MemCmp((void*)first, (void*)second, (long)((ulong)first->m_Event.sizeInBytes)) == 0);
		}

		private const uint kHandledMask = 2147483648U;

		private const uint kIdMask = 2147483647U;

		internal const int kBaseEventSize = 20;

		public const int InvalidEventId = 0;

		internal const int kAlignment = 4;

		[FieldOffset(0)]
		private NativeInputEvent m_Event;
	}
}
