using System;

namespace Fusion
{
	internal class SimulationHistoryEntryList
	{
		public void AddFirst(Simulation.History.Entry item)
		{
			bool flag = item == null;
			if (!flag)
			{
				Assert.Check(!this.IsInList(item));
				item.Next = this.Head;
				item.Prev = null;
				bool flag2 = this.Head != null;
				if (flag2)
				{
					this.Head.Prev = item;
					this.Head = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}
		}

		public void AddLast(Simulation.History.Entry item)
		{
			bool flag = item == null;
			if (!flag)
			{
				Assert.Check(!this.IsInList(item));
				item.Next = null;
				item.Prev = this.Tail;
				bool flag2 = this.Tail != null;
				if (flag2)
				{
					this.Tail.Next = item;
					this.Tail = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}
		}

		public void AddBefore(Simulation.History.Entry item, Simulation.History.Entry before)
		{
			bool flag = item == null;
			if (!flag)
			{
				bool flag2 = before == null;
				if (!flag2)
				{
					Assert.Check(this.Count > 0);
					Assert.Check(this.IsInList(before));
					Assert.Check(!this.IsInList(item));
					bool flag3 = before == this.Head;
					if (flag3)
					{
						this.AddFirst(item);
					}
					else
					{
						Assert.Check(this.Count > 1);
						Assert.Check(before.Prev != null);
						item.Next = before;
						item.Prev = before.Prev;
						before.Prev.Next = item;
						before.Prev = item;
						this.Count++;
					}
					Assert.Check(this.IsInList(before));
					Assert.Check(this.IsInList(item));
				}
			}
		}

		public void AddAfter(Simulation.History.Entry item, Simulation.History.Entry after)
		{
			bool flag = item == null;
			if (!flag)
			{
				bool flag2 = after == null;
				if (!flag2)
				{
					Assert.Check(this.Count > 0);
					Assert.Check(this.IsInList(after));
					Assert.Check(!this.IsInList(item));
					bool flag3 = after == this.Tail;
					if (flag3)
					{
						this.AddLast(item);
					}
					else
					{
						Assert.Check(this.Count > 1);
						Assert.Check(after.Next != null);
						item.Next = after.Next;
						item.Prev = after;
						after.Next.Prev = item;
						after.Next = item;
						this.Count++;
					}
					Assert.Check(this.IsInList(after));
					Assert.Check(this.IsInList(item));
				}
			}
		}

		public Simulation.History.Entry RemoveHead()
		{
			Assert.Check(this.Count > 0);
			Assert.Check(this.Head != null);
			Assert.Check(this.IsInList(this.Head));
			Simulation.History.Entry head = this.Head;
			this.Remove(head);
			return head;
		}

		public void Remove(Simulation.History.Entry item)
		{
			bool flag = item == null;
			if (!flag)
			{
				Assert.Check(this.IsInList(item));
				bool flag2 = item.Prev != null;
				if (flag2)
				{
					item.Prev.Next = item.Next;
				}
				bool flag3 = item.Next != null;
				if (flag3)
				{
					item.Next.Prev = item.Prev;
				}
				bool flag4 = item == this.Tail;
				if (flag4)
				{
					this.Tail = item.Prev;
				}
				bool flag5 = item == this.Head;
				if (flag5)
				{
					this.Head = item.Next;
				}
				item.Prev = null;
				item.Next = null;
				this.Count--;
			}
		}

		private bool IsInList(Simulation.History.Entry item)
		{
			bool flag = item == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (Simulation.History.Entry entry = this.Head; entry != null; entry = entry.Next)
				{
					bool flag2 = entry == item;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public SimulationHistoryEntryList RemoveAll()
		{
			this.Head = null;
			this.Tail = null;
			this.Count = 0;
			return this;
		}

		public void Concat(SimulationHistoryEntryList other)
		{
			bool flag = other.Count == 0;
			if (!flag)
			{
				bool flag2 = this.Count == 0;
				if (flag2)
				{
					this.Count = other.Count;
					this.Head = other.Head;
					this.Tail = other.Tail;
				}
				else
				{
					Assert.Check(!this.IsInList(other.Head));
					Assert.Check(this.Tail != null);
					Assert.Check(this.Tail.Next == null);
					Assert.Check(other.Head != null);
					Assert.Check(other.Head.Prev == null);
					this.Tail.Next = other.Head;
					other.Head.Prev = this.Tail;
					this.Tail = other.Tail;
					this.Count += other.Count;
				}
			}
		}

		public int Count;

		public Simulation.History.Entry Head;

		public Simulation.History.Entry Tail;
	}
}
