using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct InputEventStream
	{
		public bool isOpen
		{
			get
			{
				return this.m_IsOpen;
			}
		}

		public int remainingEventCount
		{
			get
			{
				return this.m_RemainingNativeEventCount + this.m_RemainingAppendEventCount;
			}
		}

		public int numEventsRetainedInBuffer
		{
			get
			{
				return this.m_NumEventsRetainedInBuffer;
			}
		}

		public unsafe InputEvent* currentEventPtr
		{
			get
			{
				if (this.m_RemainingNativeEventCount > 0)
				{
					return this.m_CurrentNativeEventReadPtr;
				}
				if (this.m_RemainingAppendEventCount <= 0)
				{
					return null;
				}
				return this.m_CurrentAppendEventReadPtr;
			}
		}

		public unsafe uint numBytesRetainedInBuffer
		{
			get
			{
				return (uint)((long)((byte*)this.m_CurrentNativeEventWritePtr - (byte*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(this.m_NativeBuffer.data)));
			}
		}

		public unsafe InputEventStream(ref InputEventBuffer eventBuffer, int maxAppendedEvents)
		{
			this.m_CurrentNativeEventWritePtr = (this.m_CurrentNativeEventReadPtr = (InputEvent*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(eventBuffer.data));
			this.m_NativeBuffer = eventBuffer;
			this.m_RemainingNativeEventCount = this.m_NativeBuffer.eventCount;
			this.m_NumEventsRetainedInBuffer = 0;
			this.m_CurrentAppendEventReadPtr = (this.m_CurrentAppendEventWritePtr = null);
			this.m_AppendBuffer = default(InputEventBuffer);
			this.m_RemainingAppendEventCount = 0;
			this.m_MaxAppendedEvents = maxAppendedEvents;
			this.m_IsOpen = true;
		}

		public unsafe void Close(ref InputEventBuffer eventBuffer)
		{
			if (this.m_NumEventsRetainedInBuffer > 0)
			{
				void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(this.m_NativeBuffer.data);
				long num = (long)((byte*)this.m_CurrentNativeEventWritePtr - (byte*)unsafeBufferPointerWithoutChecks);
				this.m_NativeBuffer = new InputEventBuffer((InputEvent*)unsafeBufferPointerWithoutChecks, this.m_NumEventsRetainedInBuffer, (int)num, (int)this.m_NativeBuffer.capacityInBytes);
			}
			else
			{
				this.m_NativeBuffer.Reset();
			}
			if (this.m_AppendBuffer.data.IsCreated)
			{
				this.m_AppendBuffer.Dispose();
			}
			eventBuffer = this.m_NativeBuffer;
			this.m_IsOpen = false;
		}

		public void CleanUpAfterException()
		{
			if (!this.isOpen)
			{
				return;
			}
			this.m_NativeBuffer.Reset();
			if (this.m_AppendBuffer.data.IsCreated)
			{
				this.m_AppendBuffer.Dispose();
			}
			this.m_IsOpen = false;
		}

		public unsafe void Write(InputEvent* eventPtr)
		{
			if (this.m_AppendBuffer.eventCount >= this.m_MaxAppendedEvents)
			{
				Debug.LogError("Maximum number of queued events exceeded. Set the 'maxQueuedEventsPerUpdate' setting to a higher value if you need to queue more events than this. " + string.Format("Current limit is '{0}'.", this.m_MaxAppendedEvents));
				return;
			}
			bool isCreated = this.m_AppendBuffer.data.IsCreated;
			byte* data = (byte*)this.m_AppendBuffer.bufferPtr.data;
			this.m_AppendBuffer.AppendEvent(eventPtr, 2048, Allocator.Temp);
			if (!isCreated)
			{
				this.m_CurrentAppendEventWritePtr = (this.m_CurrentAppendEventReadPtr = (InputEvent*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(this.m_AppendBuffer.data));
			}
			else
			{
				byte* data2 = (byte*)this.m_AppendBuffer.bufferPtr.data;
				if (data != data2)
				{
					long num = (long)((byte*)this.m_CurrentAppendEventWritePtr - (byte*)data);
					long num2 = (long)((byte*)this.m_CurrentAppendEventReadPtr - (byte*)data);
					this.m_CurrentAppendEventWritePtr = (InputEvent*)(data2 + num);
					this.m_CurrentAppendEventReadPtr = (InputEvent*)(data2 + num2);
				}
			}
			this.m_RemainingAppendEventCount++;
		}

		public unsafe InputEvent* Advance(bool leaveEventInBuffer)
		{
			if (this.m_RemainingNativeEventCount > 0)
			{
				this.m_NativeBuffer.AdvanceToNextEvent(ref this.m_CurrentNativeEventReadPtr, ref this.m_CurrentNativeEventWritePtr, ref this.m_NumEventsRetainedInBuffer, ref this.m_RemainingNativeEventCount, leaveEventInBuffer);
			}
			else if (this.m_RemainingAppendEventCount > 0)
			{
				int num = 0;
				this.m_AppendBuffer.AdvanceToNextEvent(ref this.m_CurrentAppendEventReadPtr, ref this.m_CurrentAppendEventWritePtr, ref num, ref this.m_RemainingAppendEventCount, false);
			}
			return this.currentEventPtr;
		}

		public unsafe InputEvent* Peek()
		{
			if (this.m_RemainingNativeEventCount > 1)
			{
				return InputEvent.GetNextInMemory(this.m_CurrentNativeEventReadPtr);
			}
			if (this.m_RemainingNativeEventCount == 1)
			{
				if (this.m_RemainingAppendEventCount <= 0)
				{
					return null;
				}
				return this.m_CurrentAppendEventReadPtr;
			}
			else
			{
				if (this.m_RemainingAppendEventCount > 1)
				{
					return InputEvent.GetNextInMemory(this.m_CurrentAppendEventReadPtr);
				}
				return null;
			}
		}

		private InputEventBuffer m_NativeBuffer;

		private unsafe InputEvent* m_CurrentNativeEventReadPtr;

		private unsafe InputEvent* m_CurrentNativeEventWritePtr;

		private int m_RemainingNativeEventCount;

		private readonly int m_MaxAppendedEvents;

		private InputEventBuffer m_AppendBuffer;

		private unsafe InputEvent* m_CurrentAppendEventReadPtr;

		private unsafe InputEvent* m_CurrentAppendEventWritePtr;

		private int m_RemainingAppendEventCount;

		private int m_NumEventsRetainedInBuffer;

		private bool m_IsOpen;
	}
}
