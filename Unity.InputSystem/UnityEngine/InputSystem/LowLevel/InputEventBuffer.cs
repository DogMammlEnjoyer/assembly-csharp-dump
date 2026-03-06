using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public struct InputEventBuffer : IEnumerable<InputEventPtr>, IEnumerable, IDisposable, ICloneable
	{
		public int eventCount
		{
			get
			{
				return this.m_EventCount;
			}
		}

		public long sizeInBytes
		{
			get
			{
				return this.m_SizeInBytes;
			}
		}

		public long capacityInBytes
		{
			get
			{
				if (!this.m_Buffer.IsCreated)
				{
					return 0L;
				}
				return (long)this.m_Buffer.Length;
			}
		}

		public NativeArray<byte> data
		{
			get
			{
				return this.m_Buffer;
			}
		}

		public unsafe InputEventPtr bufferPtr
		{
			get
			{
				return (InputEvent*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(this.m_Buffer);
			}
		}

		public unsafe InputEventBuffer(InputEvent* eventPtr, int eventCount, int sizeInBytes = -1, int capacityInBytes = -1)
		{
			this = default(InputEventBuffer);
			if (eventPtr == null && eventCount != 0)
			{
				throw new ArgumentException("eventPtr is NULL but eventCount is != 0", "eventCount");
			}
			if (capacityInBytes != 0 && capacityInBytes < sizeInBytes)
			{
				throw new ArgumentException(string.Format("capacity({0}) cannot be smaller than size({1})", capacityInBytes, sizeInBytes), "capacityInBytes");
			}
			if (eventPtr != null)
			{
				if (capacityInBytes < 0)
				{
					capacityInBytes = sizeInBytes;
				}
				this.m_Buffer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)eventPtr, (capacityInBytes > 0) ? capacityInBytes : 0, Allocator.None);
				this.m_SizeInBytes = ((sizeInBytes >= 0) ? ((long)sizeInBytes) : -1L);
				this.m_EventCount = eventCount;
				this.m_WeOwnTheBuffer = false;
			}
		}

		public InputEventBuffer(NativeArray<byte> buffer, int eventCount, int sizeInBytes = -1, bool transferNativeArrayOwnership = false)
		{
			if (eventCount > 0 && !buffer.IsCreated)
			{
				throw new ArgumentException("buffer has no data but eventCount is > 0", "eventCount");
			}
			if (sizeInBytes > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("sizeInBytes");
			}
			this.m_Buffer = buffer;
			this.m_WeOwnTheBuffer = transferNativeArrayOwnership;
			this.m_SizeInBytes = (long)((sizeInBytes >= 0) ? sizeInBytes : buffer.Length);
			this.m_EventCount = eventCount;
		}

		public unsafe void AppendEvent(InputEvent* eventPtr, int capacityIncrementInBytes = 2048, Allocator allocator = Allocator.Persistent)
		{
			if (eventPtr == null)
			{
				throw new ArgumentNullException("eventPtr");
			}
			uint sizeInBytes = eventPtr->sizeInBytes;
			UnsafeUtility.MemCpy((void*)this.AllocateEvent((int)sizeInBytes, capacityIncrementInBytes, allocator), (void*)eventPtr, (long)((ulong)sizeInBytes));
		}

		public unsafe InputEvent* AllocateEvent(int sizeInBytes, int capacityIncrementInBytes = 2048, Allocator allocator = Allocator.Persistent)
		{
			if (sizeInBytes < 20)
			{
				throw new ArgumentException(string.Format("sizeInBytes must be >= sizeof(InputEvent) == {0} (was {1})", 20, sizeInBytes), "sizeInBytes");
			}
			int num = sizeInBytes.AlignToMultipleOf(4);
			long num2 = this.m_SizeInBytes + (long)num;
			if (this.capacityInBytes < num2)
			{
				long num3 = num2.AlignToMultipleOf((long)capacityIncrementInBytes);
				if (num3 > 2147483647L)
				{
					throw new NotImplementedException("NativeArray long support");
				}
				NativeArray<byte> nativeArray = new NativeArray<byte>((int)num3, allocator, NativeArrayOptions.ClearMemory);
				if (this.m_Buffer.IsCreated)
				{
					UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr<byte>(), NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(this.m_Buffer), this.sizeInBytes);
					if (this.m_WeOwnTheBuffer)
					{
						this.m_Buffer.Dispose();
					}
				}
				this.m_Buffer = nativeArray;
				this.m_WeOwnTheBuffer = true;
			}
			InputEvent* ptr = (InputEvent*)((byte*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(this.m_Buffer) + this.m_SizeInBytes);
			ptr->sizeInBytes = (uint)sizeInBytes;
			this.m_SizeInBytes += (long)num;
			this.m_EventCount++;
			return ptr;
		}

		public unsafe bool Contains(InputEvent* eventPtr)
		{
			if (eventPtr == null)
			{
				return false;
			}
			if (this.sizeInBytes == 0L)
			{
				return false;
			}
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(this.data);
			return eventPtr >= (InputEvent*)unsafeBufferPointerWithoutChecks && (this.sizeInBytes == -1L || eventPtr < (InputEvent*)((byte*)unsafeBufferPointerWithoutChecks + this.sizeInBytes));
		}

		public void Reset()
		{
			this.m_EventCount = 0;
			if (this.m_SizeInBytes != -1L)
			{
				this.m_SizeInBytes = 0L;
			}
		}

		internal unsafe void AdvanceToNextEvent(ref InputEvent* currentReadPos, ref InputEvent* currentWritePos, ref int numEventsRetainedInBuffer, ref int numRemainingEvents, bool leaveEventInBuffer)
		{
			InputEvent* ptr = currentReadPos;
			if (numRemainingEvents > 1)
			{
				ptr = InputEvent.GetNextInMemory(currentReadPos);
			}
			if (leaveEventInBuffer)
			{
				uint sizeInBytes = currentReadPos.sizeInBytes;
				if (currentReadPos != currentWritePos)
				{
					UnsafeUtility.MemMove(currentWritePos, currentReadPos, (long)((ulong)sizeInBytes));
				}
				currentWritePos += (IntPtr)((UIntPtr)sizeInBytes.AlignToMultipleOf(4U));
				numEventsRetainedInBuffer++;
			}
			currentReadPos = ptr;
			numRemainingEvents--;
		}

		public IEnumerator<InputEventPtr> GetEnumerator()
		{
			return new InputEventBuffer.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Dispose()
		{
			if (!this.m_WeOwnTheBuffer)
			{
				return;
			}
			this.m_Buffer.Dispose();
			this.m_WeOwnTheBuffer = false;
			this.m_SizeInBytes = 0L;
			this.m_EventCount = 0;
		}

		public InputEventBuffer Clone()
		{
			InputEventBuffer result = default(InputEventBuffer);
			if (this.m_Buffer.IsCreated)
			{
				result.m_Buffer = new NativeArray<byte>(this.m_Buffer.Length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				result.m_Buffer.CopyFrom(this.m_Buffer);
				result.m_WeOwnTheBuffer = true;
			}
			result.m_SizeInBytes = this.m_SizeInBytes;
			result.m_EventCount = this.m_EventCount;
			return result;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public const long BufferSizeUnknown = -1L;

		private NativeArray<byte> m_Buffer;

		private long m_SizeInBytes;

		private int m_EventCount;

		private bool m_WeOwnTheBuffer;

		private struct Enumerator : IEnumerator<InputEventPtr>, IEnumerator, IDisposable
		{
			public Enumerator(InputEventBuffer buffer)
			{
				this.m_Buffer = buffer.bufferPtr;
				this.m_EventCount = buffer.m_EventCount;
				this.m_CurrentEvent = null;
				this.m_CurrentIndex = 0;
			}

			public bool MoveNext()
			{
				if (this.m_CurrentIndex == this.m_EventCount)
				{
					return false;
				}
				if (this.m_CurrentEvent == null)
				{
					this.m_CurrentEvent = this.m_Buffer;
					return this.m_CurrentEvent != null;
				}
				this.m_CurrentIndex++;
				if (this.m_CurrentIndex == this.m_EventCount)
				{
					return false;
				}
				this.m_CurrentEvent = InputEvent.GetNextInMemory(this.m_CurrentEvent);
				return true;
			}

			public void Reset()
			{
				this.m_CurrentEvent = null;
				this.m_CurrentIndex = 0;
			}

			public void Dispose()
			{
			}

			public InputEventPtr Current
			{
				get
				{
					return this.m_CurrentEvent;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private unsafe readonly InputEvent* m_Buffer;

			private readonly int m_EventCount;

			private unsafe InputEvent* m_CurrentEvent;

			private int m_CurrentIndex;
		}
	}
}
