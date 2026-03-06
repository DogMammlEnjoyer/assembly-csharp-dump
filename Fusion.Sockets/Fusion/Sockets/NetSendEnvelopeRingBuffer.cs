using System;

namespace Fusion.Sockets
{
	internal struct NetSendEnvelopeRingBuffer
	{
		public bool IsFull
		{
			get
			{
				return this.Count == this._itemsCapacity;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Count == 0;
			}
		}

		public unsafe void Push(NetSendEnvelope envelope)
		{
			bool flag = this.Count == this._itemsCapacity;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			this._items[this.Head] = envelope;
			this.Head = (this.Head + 1) % this._itemsCapacity;
			this.Count++;
		}

		public unsafe bool TryPush(NetSendEnvelope envelope)
		{
			bool flag = this.Count == this._itemsCapacity;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this._items[this.Head] = envelope;
				this.Head = (this.Head + 1) % this._itemsCapacity;
				this.Count++;
				result = true;
			}
			return result;
		}

		public unsafe NetSendEnvelope Peek()
		{
			Assert.Check(this.Count > 0);
			return this._items[this.Tail];
		}

		public void Pop()
		{
			Assert.Check(this.Count > 0);
			this.Tail = (this.Tail + 1) % this._itemsCapacity;
			this.Count--;
		}

		public void Reset()
		{
			this.Head = 0;
			this.Tail = 0;
			this.Count = 0;
		}

		public void Dispose()
		{
			Native.Free<NetSendEnvelope>(ref this._items);
		}

		public static NetSendEnvelopeRingBuffer Create(int capacity)
		{
			NetSendEnvelopeRingBuffer result;
			result.Head = 0;
			result.Tail = 0;
			result.Count = 0;
			result._itemsCapacity = capacity;
			result._items = Native.MallocAndClearArray<NetSendEnvelope>(capacity);
			return result;
		}

		private unsafe NetSendEnvelope* _items;

		private int _itemsCapacity;

		public int Head;

		public int Tail;

		public int Count;
	}
}
