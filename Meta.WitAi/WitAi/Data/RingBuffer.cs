using System;
using UnityEngine;

namespace Meta.WitAi.Data
{
	public class RingBuffer<T>
	{
		public int Capacity
		{
			get
			{
				return this.buffer.Length;
			}
		}

		public int GetBufferArrayIndex(long bufferDataIndex)
		{
			if (this.bufferDataLength <= bufferDataIndex)
			{
				return -1;
			}
			if (this.bufferDataLength - bufferDataIndex > (long)this.buffer.Length)
			{
				return -1;
			}
			long num = this.bufferDataLength - bufferDataIndex;
			long num2 = (long)this.bufferIndex - num;
			if (num2 < 0L)
			{
				num2 = (long)this.buffer.Length + num2;
			}
			return (int)num2;
		}

		public T this[long bufferDataIndex]
		{
			get
			{
				return this.buffer[this.GetBufferArrayIndex(bufferDataIndex)];
			}
		}

		public void Clear(bool eraseData = false)
		{
			this.bufferIndex = 0;
			this.bufferDataLength = 0L;
			if (eraseData)
			{
				for (int i = 0; i < this.buffer.Length; i++)
				{
					this.buffer[i] = default(T);
				}
			}
		}

		public RingBuffer(int capacity)
		{
			this.buffer = new T[capacity];
		}

		private int CopyToBuffer(T[] data, int offset, int length, int newBufferIndex)
		{
			if (length > this.buffer.Length)
			{
				throw new ArgumentException("Push data exceeds buffer size.");
			}
			if (newBufferIndex + length < this.buffer.Length)
			{
				Array.Copy(data, offset, this.buffer, newBufferIndex, length);
				return newBufferIndex + length;
			}
			int num = Mathf.Min(length, this.buffer.Length);
			int num2 = this.buffer.Length - newBufferIndex;
			int num3 = num - num2;
			int result;
			try
			{
				Array.Copy(data, offset, this.buffer, newBufferIndex, num2);
				Array.Copy(data, offset + num2, this.buffer, 0, num3);
				result = num3;
			}
			catch (ArgumentException ex)
			{
				throw ex;
			}
			return result;
		}

		public void WriteFromBuffer(RingBuffer<T>.ByteDataWriter writer, long newBufferIndex, int length)
		{
			T[] obj = this.buffer;
			lock (obj)
			{
				if (newBufferIndex + (long)length < (long)this.buffer.Length)
				{
					writer(this.buffer, (int)newBufferIndex, length);
				}
				else
				{
					if ((long)length > this.bufferDataLength)
					{
						length = (int)(this.bufferDataLength - newBufferIndex);
					}
					if (length > this.buffer.Length)
					{
						length = this.buffer.Length;
					}
					int num = Math.Min(this.buffer.Length, length);
					int num2 = (int)((long)this.buffer.Length - newBufferIndex);
					int length2 = num - num2;
					writer(this.buffer, (int)newBufferIndex, num2);
					writer(this.buffer, 0, length2);
				}
			}
		}

		private int CopyFromBuffer(T[] data, int offset, int length, int newBufferIndex)
		{
			if (length > this.buffer.Length)
			{
				throw new ArgumentException(string.Format("Push data exceeds buffer size {0} < {1}", length, this.buffer.Length));
			}
			if (newBufferIndex + length < this.buffer.Length)
			{
				Array.Copy(this.buffer, newBufferIndex, data, offset, length);
				return newBufferIndex + length;
			}
			int num = Mathf.Min(this.buffer.Length, length);
			int num2 = this.buffer.Length - newBufferIndex;
			int num3 = num - num2;
			Array.Copy(this.buffer, newBufferIndex, data, offset, num2);
			Array.Copy(this.buffer, 0, data, offset + num2, num3);
			return num3;
		}

		public void Push(T[] data, int offset, int length)
		{
			T[] obj = this.buffer;
			lock (obj)
			{
				this.bufferIndex = this.CopyToBuffer(data, offset, length, this.bufferIndex);
				this.bufferDataLength += (long)length;
				RingBuffer<T>.OnDataAdded onDataAddedEvent = this.OnDataAddedEvent;
				if (onDataAddedEvent != null)
				{
					onDataAddedEvent(data, offset, length);
				}
			}
		}

		public void Push(T data)
		{
			T[] obj = this.buffer;
			lock (obj)
			{
				T[] array = this.buffer;
				int num = this.bufferIndex;
				this.bufferIndex = num + 1;
				array[num] = data;
				if (this.bufferIndex >= this.buffer.Length)
				{
					this.bufferIndex = 0;
				}
				this.bufferDataLength += 1L;
			}
		}

		public int Read(T[] data, int offset, int length, long bufferDataIndex)
		{
			if (this.bufferIndex == 0 && this.bufferDataLength == 0L)
			{
				return 0;
			}
			T[] obj = this.buffer;
			int result;
			lock (obj)
			{
				int num = (int)(Math.Min(bufferDataIndex + (long)length, this.bufferDataLength) - bufferDataIndex);
				int num2 = this.bufferIndex - (int)(this.bufferDataLength - bufferDataIndex);
				if (num2 < 0)
				{
					num2 = this.buffer.Length + num2;
				}
				this.CopyFromBuffer(data, offset, length, num2);
				result = num;
			}
			return result;
		}

		public RingBuffer<T>.Marker CreateMarker(int offset = 0)
		{
			long num = this.bufferDataLength + (long)offset;
			if (num < 0L)
			{
				num = 0L;
			}
			int num2 = this.bufferIndex + offset;
			if (num2 < 0)
			{
				num2 = this.buffer.Length + num2;
			}
			if (num2 > this.buffer.Length)
			{
				num2 -= this.buffer.Length;
			}
			return new RingBuffer<T>.Marker(this, num, num2);
		}

		public RingBuffer<T>.OnDataAdded OnDataAddedEvent;

		private readonly T[] buffer;

		private int bufferIndex;

		private long bufferDataLength;

		public delegate void OnDataAdded(T[] data, int offset, int length);

		public delegate void ByteDataWriter(T[] buffer, int offset, int length);

		public class Marker
		{
			public RingBuffer<T> RingBuffer
			{
				get
				{
					return this.ringBuffer;
				}
			}

			public Marker(RingBuffer<T> ringBuffer, long markerPosition, int bufIndex)
			{
				this.ringBuffer = ringBuffer;
				this.bufferDataIndex = markerPosition;
				this.index = bufIndex;
			}

			public bool IsValid
			{
				get
				{
					return this.ringBuffer.bufferDataLength - this.bufferDataIndex <= (long)this.ringBuffer.Capacity;
				}
			}

			public long AvailableByteCount
			{
				get
				{
					return Math.Min((long)this.ringBuffer.Capacity, this.RequestedByteCount);
				}
			}

			public long RequestedByteCount
			{
				get
				{
					return this.ringBuffer.bufferDataLength - this.bufferDataIndex;
				}
			}

			public long CurrentBufferDataIndex
			{
				get
				{
					return this.bufferDataIndex;
				}
			}

			public int Read(T[] buffer, int offset, int length, bool skipToNextValid = false)
			{
				int num = -1;
				if (!this.IsValid && skipToNextValid && this.ringBuffer.bufferDataLength > (long)this.ringBuffer.Capacity)
				{
					this.bufferDataIndex = this.ringBuffer.bufferDataLength - (long)this.ringBuffer.Capacity;
				}
				if (this.IsValid)
				{
					num = this.ringBuffer.Read(buffer, offset, length, this.bufferDataIndex);
					this.bufferDataIndex += (long)num;
					this.index += num;
					if (this.index > buffer.Length)
					{
						this.index -= buffer.Length;
					}
				}
				return num;
			}

			public void ReadIntoWriters(params RingBuffer<T>.ByteDataWriter[] writers)
			{
				if (!this.IsValid && this.ringBuffer.bufferDataLength > (long)this.ringBuffer.Capacity)
				{
					this.bufferDataIndex = this.ringBuffer.bufferDataLength - (long)this.ringBuffer.Capacity;
				}
				this.index = this.ringBuffer.GetBufferArrayIndex(this.bufferDataIndex);
				int num = (int)(this.ringBuffer.bufferDataLength - this.bufferDataIndex);
				if (this.IsValid && num > 0)
				{
					for (int i = 0; i < writers.Length; i++)
					{
						this.ringBuffer.WriteFromBuffer(writers[i], (long)this.index, num);
					}
				}
				this.bufferDataIndex += (long)num;
				this.index = this.ringBuffer.GetBufferArrayIndex(this.bufferDataIndex);
			}

			public RingBuffer<T>.Marker Clone()
			{
				return new RingBuffer<T>.Marker(this.ringBuffer, this.bufferDataIndex, this.index);
			}

			public void Offset(int amount)
			{
				this.bufferDataIndex += (long)amount;
				if (this.bufferDataIndex < 0L)
				{
					this.bufferDataIndex = 0L;
				}
				if (this.bufferDataIndex > this.ringBuffer.bufferDataLength)
				{
					this.bufferDataIndex = this.ringBuffer.bufferDataLength;
				}
				this.index = this.ringBuffer.GetBufferArrayIndex(this.bufferDataIndex);
			}

			private long bufferDataIndex;

			private int index;

			private readonly RingBuffer<T> ringBuffer;
		}
	}
}
