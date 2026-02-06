using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal class NetworkObjectPriorityList
	{
		public NetworkObjectConnectionDataList GetLevelList(int level)
		{
			return this.Levels[level];
		}

		public void IncreasePriorities()
		{
			for (int i = 1; i < this.Levels.Length - 1; i++)
			{
				bool flag = this.Levels[i + 1].Head == null;
				if (!flag)
				{
					for (NetworkObjectConnectionData networkObjectConnectionData = this.Levels[i + 1].Head; networkObjectConnectionData != null; networkObjectConnectionData = networkObjectConnectionData.Next)
					{
						networkObjectConnectionData.PriorityLevel = NetworkObjectMeta.EncodePriorityLevel(i);
					}
					bool flag2 = this.Levels[i].Head == null;
					if (flag2)
					{
						this.Levels[i] = this.Levels[i + 1];
					}
					else
					{
						this.Levels[i + 1].Head.Prev = this.Levels[i].Tail;
						this.Levels[i].Tail.Next = this.Levels[i + 1].Head;
						this.Levels[i].Tail = this.Levels[i + 1].Tail;
						NetworkObjectConnectionDataList[] levels = this.Levels;
						int num = i;
						levels[num].Count = levels[num].Count + this.Levels[i + 1].Count;
					}
					this.Levels[i + 1] = default(NetworkObjectConnectionDataList);
				}
			}
		}

		public void SetIdle(NetworkObjectConnectionData item)
		{
			bool flag = NetworkObjectMeta.IsIdle(item.PriorityLevel);
			if (!flag)
			{
				bool flag2 = NetworkObjectMeta.IsActive(item.PriorityLevel);
				if (flag2)
				{
					this.Levels[NetworkObjectMeta.DecodePriorityLevel(item.PriorityLevel)].Remove(item);
				}
				item.PriorityLevel = -32768;
				this.Idle.AddLast(item);
				Assert.Check(NetworkObjectMeta.IsIdle(item.PriorityLevel));
			}
		}

		public void SetActive(NetworkObjectConnectionData item, NetworkObjectMeta meta)
		{
			bool flag = NetworkObjectMeta.IsActive(item.PriorityLevel);
			if (!flag)
			{
				bool flag2 = NetworkObjectMeta.IsIdle(item.PriorityLevel);
				if (flag2)
				{
					this.Idle.Remove(item);
				}
				item.PriorityLevel = meta.GetPriority(this.Player);
				Assert.Check(NetworkObjectMeta.IsActive(item.PriorityLevel));
				this.Levels[NetworkObjectMeta.DecodePriorityLevel(item.PriorityLevel)].AddLast(item);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(NetworkObjectConnectionData item)
		{
			Assert.Check(NetworkObjectMeta.IsActive(item.PriorityLevel));
			this.Levels[NetworkObjectMeta.DecodePriorityLevel(item.PriorityLevel)].AddLast(item);
		}

		public void RemoveSent(NetworkObjectConnectionData item)
		{
			Assert.Check(item.PriorityLevel != -32768);
			Assert.Check(NetworkObjectMeta.IsActive(item.PriorityLevel));
			this.Levels[NetworkObjectMeta.DecodePriorityLevel(item.PriorityLevel)].Remove(item);
		}

		public void Remove(NetworkObjectConnectionData item)
		{
			bool flag = item.PriorityLevel == -32768;
			if (flag)
			{
				this.Idle.Remove(item);
			}
			else
			{
				bool flag2 = NetworkObjectMeta.IsActive(item.PriorityLevel);
				if (flag2)
				{
					this.Levels[NetworkObjectMeta.DecodePriorityLevel(item.PriorityLevel)].Remove(item);
				}
			}
		}

		public PlayerRef Player;

		public NetworkObjectConnectionDataList Idle;

		private NetworkObjectConnectionDataList[] Levels = new NetworkObjectConnectionDataList[5];
	}
}
