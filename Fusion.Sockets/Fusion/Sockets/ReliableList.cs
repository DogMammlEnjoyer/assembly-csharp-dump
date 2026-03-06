using System;

namespace Fusion.Sockets
{
	public struct ReliableList
	{
		public unsafe void AddFirst(ReliableHeader* item)
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

		public unsafe void AddLast(ReliableHeader* item)
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

		public unsafe void AddBefore(ReliableHeader* before, ReliableHeader* item)
		{
			Assert.Check(this.Count > 0);
			Assert.Check(this.IsInList(before));
			Assert.Check(!this.IsInList(item));
			bool flag = before == this.Head;
			if (flag)
			{
				this.AddFirst(item);
			}
			else
			{
				item->Next = before;
				item->Prev = before->Prev;
				before->Prev->Next = item;
				before->Prev = item;
				this.Count++;
			}
			Assert.Check(this.IsInList(before));
			Assert.Check(this.IsInList(item));
		}

		public unsafe void AddAfter(ReliableHeader* after, ReliableHeader* item)
		{
			Assert.Check(this.Count > 0);
			Assert.Check(this.IsInList(after));
			Assert.Check(!this.IsInList(item));
			bool flag = after == this.Tail;
			if (flag)
			{
				this.AddLast(item);
			}
			else
			{
				item->Next = after->Next;
				item->Prev = after;
				after->Next->Prev = item;
				after->Next = item;
				this.Count++;
			}
			Assert.Check(this.IsInList(after));
			Assert.Check(this.IsInList(item));
		}

		public unsafe ReliableHeader* RemoveHead()
		{
			Assert.Check(this.Count > 0);
			Assert.Check(this.Head != null);
			Assert.Check(this.IsInList(this.Head));
			ReliableHeader* head = this.Head;
			this.Remove(head);
			return head;
		}

		public unsafe void Remove(ReliableHeader* item)
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

		private unsafe bool IsInList(ReliableHeader* item)
		{
			for (ReliableHeader* ptr = this.Head; ptr != null; ptr = ptr->Next)
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
				ReliableHeader* ptr = this.RemoveHead();
				Native.Free<ReliableHeader>(ref ptr);
			}
			Assert.Check(this.Head == null);
			Assert.Check(this.Tail == null);
		}

		public int Count;

		public unsafe ReliableHeader* Head;

		public unsafe ReliableHeader* Tail;
	}
}
