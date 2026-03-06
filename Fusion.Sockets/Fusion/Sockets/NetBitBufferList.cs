using System;

namespace Fusion.Sockets
{
	internal struct NetBitBufferList
	{
		public unsafe void AddFirst(NetBitBuffer* item)
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

		public unsafe void AddLast(NetBitBuffer* item)
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

		public unsafe NetBitBuffer* RemoveHead()
		{
			Assert.Check(this.Count > 0);
			Assert.Check(this.Head != null);
			Assert.Check(this.IsInList(this.Head));
			NetBitBuffer* head = this.Head;
			this.Remove(head);
			return head;
		}

		public unsafe void Remove(NetBitBuffer* item)
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

		private unsafe bool IsInList(NetBitBuffer* item)
		{
			for (NetBitBuffer* ptr = this.Head; ptr != null; ptr = ptr->Next)
			{
				bool flag = ptr == item;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public int Count;

		public unsafe NetBitBuffer* Head;

		public unsafe NetBitBuffer* Tail;
	}
}
