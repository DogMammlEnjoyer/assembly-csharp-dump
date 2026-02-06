using System;

namespace Fusion
{
	internal struct NetworkObjectHeaderSnapshotList
	{
		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public NetworkObjectHeaderSnapshot Oldest
		{
			get
			{
				return this._tail;
			}
		}

		public NetworkObjectHeaderSnapshot Latest
		{
			get
			{
				return this._head;
			}
		}

		public void AddFirst(NetworkObjectHeaderSnapshot item)
		{
			Assert.Check(!this.IsInList(item));
			item.Next = this._head;
			item.Prev = null;
			bool flag = this._head != null;
			if (flag)
			{
				this._head.Prev = item;
				this._head = item;
			}
			else
			{
				this._head = item;
				this._tail = item;
			}
			this._count++;
		}

		public void AddLast(NetworkObjectHeaderSnapshot item)
		{
			Assert.Check(!this.IsInList(item));
			item.Next = null;
			item.Prev = this._tail;
			bool flag = this._tail != null;
			if (flag)
			{
				this._tail.Next = item;
				this._tail = item;
			}
			else
			{
				this._head = item;
				this._tail = item;
			}
			this._count++;
		}

		public void AddBefore(NetworkObjectHeaderSnapshot before, NetworkObjectHeaderSnapshot item)
		{
			Assert.Check(this._count > 0);
			Assert.Check(this.IsInList(before));
			Assert.Check(!this.IsInList(item));
			bool flag = before == this._head;
			if (flag)
			{
				this.AddFirst(item);
			}
			else
			{
				item.Next = before;
				item.Prev = before.Prev;
				before.Prev.Next = item;
				before.Prev = item;
				this._count++;
			}
			Assert.Check(this.IsInList(before));
			Assert.Check(this.IsInList(item));
		}

		public void AddAfter(NetworkObjectHeaderSnapshot after, NetworkObjectHeaderSnapshot item)
		{
			Assert.Check(this._count > 0);
			Assert.Check(this.IsInList(after));
			Assert.Check(!this.IsInList(item));
			bool flag = after == this._tail;
			if (flag)
			{
				this.AddLast(item);
			}
			else
			{
				item.Next = after.Next;
				item.Prev = after;
				after.Next.Prev = item;
				after.Next = item;
				this._count++;
			}
			Assert.Check(this.IsInList(after));
			Assert.Check(this.IsInList(item));
		}

		public NetworkObjectHeaderSnapshot RemoveOldest()
		{
			Assert.Check(this._count > 0);
			Assert.Check(this._tail != null);
			Assert.Check(this.IsInList(this._tail));
			NetworkObjectHeaderSnapshot tail = this._tail;
			this.Remove(tail);
			return tail;
		}

		public NetworkObjectHeaderSnapshot RemoveLatest()
		{
			Assert.Check(this._count > 0);
			Assert.Check(this._head != null);
			Assert.Check(this.IsInList(this._head));
			NetworkObjectHeaderSnapshot head = this._head;
			this.Remove(head);
			return head;
		}

		public void Remove(NetworkObjectHeaderSnapshot item)
		{
			Assert.Check(this.IsInList(item));
			bool flag = item.Prev != null;
			if (flag)
			{
				item.Prev.Next = item.Next;
			}
			bool flag2 = item.Next != null;
			if (flag2)
			{
				item.Next.Prev = item.Prev;
			}
			bool flag3 = item == this._tail;
			if (flag3)
			{
				this._tail = item.Prev;
			}
			bool flag4 = item == this._head;
			if (flag4)
			{
				this._head = item.Next;
			}
			item.Prev = null;
			item.Next = null;
			this._count--;
		}

		private bool IsInList(NetworkObjectHeaderSnapshot item)
		{
			for (NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot = this._head; networkObjectHeaderSnapshot != null; networkObjectHeaderSnapshot = networkObjectHeaderSnapshot.Next)
			{
				bool flag = networkObjectHeaderSnapshot == item;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public const int SIZE = 24;

		public const int ALIGNMENT = 8;

		private int _count;

		private NetworkObjectHeaderSnapshot _tail;

		private NetworkObjectHeaderSnapshot _head;
	}
}
