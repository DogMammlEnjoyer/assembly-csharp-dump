using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal abstract class RenderGraphResourcePool<Type> : IRenderGraphResourcePool where Type : class
	{
		protected abstract void ReleaseInternalResource(Type res);

		protected abstract string GetResourceName(in Type res);

		protected abstract long GetResourceSize(in Type res);

		protected abstract string GetResourceTypeName();

		protected abstract int GetSortIndex(Type res);

		public void ReleaseResource(int hash, Type resource, int currentFrameIndex)
		{
			SortedList<int, ValueTuple<Type, int>> sortedList;
			if (!this.m_ResourcePool.TryGetValue(hash, out sortedList))
			{
				sortedList = new SortedList<int, ValueTuple<Type, int>>();
				this.m_ResourcePool.Add(hash, sortedList);
			}
			sortedList.Add(this.GetSortIndex(resource), new ValueTuple<Type, int>(resource, currentFrameIndex));
		}

		public bool TryGetResource(int hashCode, out Type resource)
		{
			SortedList<int, ValueTuple<Type, int>> sortedList;
			if (this.m_ResourcePool.TryGetValue(hashCode, out sortedList) && sortedList.Count > 0)
			{
				int index = sortedList.Count - 1;
				resource = sortedList.Values[index].Item1;
				sortedList.RemoveAt(index);
				return true;
			}
			resource = default(Type);
			return false;
		}

		public override void Cleanup()
		{
			foreach (KeyValuePair<int, SortedList<int, ValueTuple<Type, int>>> keyValuePair in this.m_ResourcePool)
			{
				foreach (KeyValuePair<int, ValueTuple<Type, int>> keyValuePair2 in keyValuePair.Value)
				{
					this.ReleaseInternalResource(keyValuePair2.Value.Item1);
				}
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public void RegisterFrameAllocation(int hash, Type value)
		{
			if (RenderGraph.enableValidityChecks && hash != -1)
			{
				this.m_FrameAllocatedResources.Add(new ValueTuple<int, Type>(hash, value));
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public void UnregisterFrameAllocation(int hash, Type value)
		{
			if (RenderGraph.enableValidityChecks && hash != -1)
			{
				this.m_FrameAllocatedResources.Remove(new ValueTuple<int, Type>(hash, value));
			}
		}

		public override void CheckFrameAllocation(bool onException, int frameIndex)
		{
		}

		public override void LogResources(RenderGraphLogger logger)
		{
			List<RenderGraphResourcePool<Type>.ResourceLogInfo> list = new List<RenderGraphResourcePool<Type>.ResourceLogInfo>();
			foreach (KeyValuePair<int, SortedList<int, ValueTuple<Type, int>>> keyValuePair in this.m_ResourcePool)
			{
				foreach (KeyValuePair<int, ValueTuple<Type, int>> keyValuePair2 in keyValuePair.Value)
				{
					list.Add(new RenderGraphResourcePool<Type>.ResourceLogInfo
					{
						name = this.GetResourceName(keyValuePair2.Value.Item1),
						size = this.GetResourceSize(keyValuePair2.Value.Item1)
					});
				}
			}
			logger.LogLine("== " + this.GetResourceTypeName() + " Resources ==", Array.Empty<object>());
			list.Sort(delegate(RenderGraphResourcePool<Type>.ResourceLogInfo a, RenderGraphResourcePool<Type>.ResourceLogInfo b)
			{
				if (a.size >= b.size)
				{
					return -1;
				}
				return 1;
			});
			int num = 0;
			float num2 = 0f;
			foreach (RenderGraphResourcePool<Type>.ResourceLogInfo resourceLogInfo in list)
			{
				float num3 = (float)resourceLogInfo.size / 1048576f;
				num2 += num3;
				logger.LogLine(string.Format("[{0:D2}]\t[{1:0.00} MB]\t{2}", num++, num3, resourceLogInfo.name), Array.Empty<object>());
			}
			logger.LogLine(string.Format("\nTotal Size [{0:0.00}]", num2), Array.Empty<object>());
		}

		public override void PurgeUnusedResources(int currentFrameIndex)
		{
			foreach (KeyValuePair<int, SortedList<int, ValueTuple<Type, int>>> keyValuePair in this.m_ResourcePool)
			{
				RenderGraphResourcePool<Type>.s_ToRemoveList.Clear();
				SortedList<int, ValueTuple<Type, int>> value = keyValuePair.Value;
				IList<int> keys = value.Keys;
				IList<ValueTuple<Type, int>> values = value.Values;
				for (int i = 0; i < value.Count; i++)
				{
					ValueTuple<Type, int> valueTuple = values[i];
					int item = keys[i];
					if (valueTuple.Item2 + 10 < currentFrameIndex)
					{
						this.ReleaseInternalResource(valueTuple.Item1);
						RenderGraphResourcePool<Type>.s_ToRemoveList.Add(item);
					}
				}
				for (int j = 0; j < RenderGraphResourcePool<Type>.s_ToRemoveList.Count; j++)
				{
					value.Remove(RenderGraphResourcePool<Type>.s_ToRemoveList[j]);
				}
			}
		}

		[TupleElementNames(new string[]
		{
			"resource",
			"frameIndex"
		})]
		protected Dictionary<int, SortedList<int, ValueTuple<Type, int>>> m_ResourcePool = new Dictionary<int, SortedList<int, ValueTuple<Type, int>>>();

		private List<ValueTuple<int, Type>> m_FrameAllocatedResources = new List<ValueTuple<int, Type>>();

		private const int kStaleResourceLifetime = 10;

		private static List<int> s_ToRemoveList = new List<int>(32);

		private struct ResourceLogInfo
		{
			public string name;

			public long size;
		}
	}
}
