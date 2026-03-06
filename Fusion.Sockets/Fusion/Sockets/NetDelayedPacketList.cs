using System;

namespace Fusion.Sockets
{
	internal struct NetDelayedPacketList
	{
		public unsafe void AddFirst(NetDelayedPacket* item)
		{
			Assert.Check(!this.IsInList(item));
			item->Next = this.Head;
			item->Prev = null;
			bool flag = this.Head != null;
			if (flag)
			{
				this.Head->Prev = item;
				this.Head = item;
			}
			else
			{
				this.Head = item;
				this.Tail = item;
			}
			this.Count++;
		}

		public unsafe void AddLast(NetDelayedPacket* item)
		{
			Assert.Check(!this.IsInList(item));
			item->Next = null;
			item->Prev = this.Tail;
			bool flag = this.Tail != null;
			if (flag)
			{
				this.Tail->Next = item;
				this.Tail = item;
			}
			else
			{
				this.Head = item;
				this.Tail = item;
			}
			this.Count++;
		}

		public unsafe NetDelayedPacket* RemoveHead()
		{
			Assert.Check(this.Count > 0);
			Assert.Check(this.Head != null);
			Assert.Check(this.IsInList(this.Head));
			NetDelayedPacket* head = this.Head;
			this.Remove(head);
			return head;
		}

		public unsafe void Remove(NetDelayedPacket* item)
		{
			Assert.Check(this.IsInList(item));
			bool flag = item->Prev != null;
			if (flag)
			{
				item->Prev->Next = item->Next;
			}
			bool flag2 = item->Next != null;
			if (flag2)
			{
				item->Next->Prev = item->Prev;
			}
			bool flag3 = item == this.Tail;
			if (flag3)
			{
				this.Tail = item->Prev;
			}
			bool flag4 = item == this.Head;
			if (flag4)
			{
				this.Head = item->Next;
			}
			item->Prev = null;
			item->Next = null;
			this.Count--;
		}

		private unsafe bool IsInList(NetDelayedPacket* item)
		{
			for (NetDelayedPacket* ptr = this.Head; ptr != null; ptr = ptr->Next)
			{
				bool flag = ptr == item;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public unsafe void Dispose()
		{
			while (this.Count > 0)
			{
				NetDelayedPacket* ptr = this.RemoveHead();
				Native.Free<NetDelayedPacket>(ref ptr);
			}
			Assert.Check(this.Head == null);
			Assert.Check(this.Tail == null);
		}

		public int Count;

		public unsafe NetDelayedPacket* Head;

		public unsafe NetDelayedPacket* Tail;
	}
}
