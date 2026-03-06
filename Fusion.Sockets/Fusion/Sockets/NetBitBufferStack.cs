using System;

namespace Fusion.Sockets
{
	internal struct NetBitBufferStack
	{
		public unsafe bool TryPop(NetBitBuffer** result)
		{
			Assert.Check(this.Count >= 0);
			bool flag = this.Count == 0;
			bool result2;
			if (flag)
			{
				result2 = false;
			}
			else
			{
				IntPtr stack = this.Stack;
				int num = this.Count - 1;
				this.Count = num;
				*(IntPtr*)result = *(stack + (IntPtr)num * (IntPtr)sizeof(NetBitBuffer*));
				result2 = true;
			}
			return result2;
		}

		public static NetBitBufferStack Create(int capacity)
		{
			return new NetBitBufferStack
			{
				_capacity = capacity,
				Stack = Native.MallocAndClearPtrArray<NetBitBuffer>(capacity)
			};
		}

		public static void Dispose(ref NetBitBufferStack stack)
		{
			Native.Free<NetBitBuffer>(ref stack.Stack);
			stack.Count = 0;
			stack._capacity = 0;
		}

		public unsafe void PushFromHead(NetBitBuffer* head)
		{
			while (head != null)
			{
				NetBitBuffer* next = head->Next;
				head->Next = null;
				head->Prev = null;
				Assert.Check(this.Count >= 0 && this.Count <= this._capacity);
				bool flag = this.Count == this._capacity;
				if (flag)
				{
					try
					{
						this.Stack = Native.DoublePtrArray<NetBitBuffer>(this.Stack, this._capacity);
					}
					catch (OutOfMemoryException)
					{
						LogStream logInfo = InternalLogStreams.LogInfo;
						if (logInfo != null)
						{
							logInfo.Log(string.Format("OOM resize to _capacity:{0}, Count:{1}", this._capacity, this.Count));
						}
						throw;
					}
					this._capacity *= 2;
				}
				ref IntPtr stack = ref *(IntPtr*)this.Stack;
				int count = this.Count;
				this.Count = count + 1;
				*(ref stack + (IntPtr)count * (IntPtr)sizeof(NetBitBuffer*)) = head;
				head = next;
			}
		}

		private int _capacity;

		public unsafe NetBitBuffer** Stack;

		public int Count;
	}
}
