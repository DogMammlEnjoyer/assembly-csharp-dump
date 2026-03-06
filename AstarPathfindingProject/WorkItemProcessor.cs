using System;
using UnityEngine;

namespace Pathfinding
{
	internal class WorkItemProcessor : IWorkItemContext
	{
		public bool workItemsInProgressRightNow { get; private set; }

		public bool anyQueued
		{
			get
			{
				return this.workItems.Count > 0;
			}
		}

		public bool workItemsInProgress { get; private set; }

		void IWorkItemContext.QueueFloodFill()
		{
			this.queuedWorkItemFloodFill = true;
		}

		void IWorkItemContext.SetGraphDirty(NavGraph graph)
		{
			this.anyGraphsDirty = true;
		}

		public void EnsureValidFloodFill()
		{
			if (this.queuedWorkItemFloodFill)
			{
				this.astar.hierarchicalGraph.RecalculateAll();
				return;
			}
			this.astar.hierarchicalGraph.RecalculateIfNecessary();
		}

		public WorkItemProcessor(AstarPath astar)
		{
			this.astar = astar;
		}

		public void OnFloodFill()
		{
			this.queuedWorkItemFloodFill = false;
		}

		public void AddWorkItem(AstarWorkItem item)
		{
			this.workItems.Enqueue(item);
		}

		public bool ProcessWorkItems(bool force)
		{
			if (this.workItemsInProgressRightNow)
			{
				throw new Exception("Processing work items recursively. Please do not wait for other work items to be completed inside work items. If you think this is not caused by any of your scripts, this might be a bug.");
			}
			Physics2D.SyncTransforms();
			this.workItemsInProgressRightNow = true;
			this.astar.data.LockGraphStructure(true);
			while (this.workItems.Count > 0)
			{
				if (!this.workItemsInProgress)
				{
					this.workItemsInProgress = true;
					this.queuedWorkItemFloodFill = false;
				}
				AstarWorkItem astarWorkItem = this.workItems[0];
				bool flag;
				try
				{
					if (astarWorkItem.init != null)
					{
						astarWorkItem.init();
						astarWorkItem.init = null;
					}
					if (astarWorkItem.initWithContext != null)
					{
						astarWorkItem.initWithContext(this);
						astarWorkItem.initWithContext = null;
					}
					this.workItems[0] = astarWorkItem;
					if (astarWorkItem.update != null)
					{
						flag = astarWorkItem.update(force);
					}
					else
					{
						flag = (astarWorkItem.updateWithContext == null || astarWorkItem.updateWithContext(this, force));
					}
				}
				catch
				{
					this.workItems.Dequeue();
					this.workItemsInProgressRightNow = false;
					this.astar.data.UnlockGraphStructure();
					throw;
				}
				if (!flag)
				{
					if (force)
					{
						Debug.LogError("Misbehaving WorkItem. 'force'=true but the work item did not complete.\nIf force=true is passed to a WorkItem it should always return true.");
					}
					this.workItemsInProgressRightNow = false;
					this.astar.data.UnlockGraphStructure();
					return false;
				}
				this.workItems.Dequeue();
			}
			this.EnsureValidFloodFill();
			if (this.anyGraphsDirty)
			{
				GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
			}
			this.anyGraphsDirty = false;
			this.workItemsInProgressRightNow = false;
			this.workItemsInProgress = false;
			this.astar.data.UnlockGraphStructure();
			return true;
		}

		private readonly AstarPath astar;

		private readonly WorkItemProcessor.IndexedQueue<AstarWorkItem> workItems = new WorkItemProcessor.IndexedQueue<AstarWorkItem>();

		private bool queuedWorkItemFloodFill;

		private bool anyGraphsDirty = true;

		private class IndexedQueue<T>
		{
			public T this[int index]
			{
				get
				{
					if (index < 0 || index >= this.Count)
					{
						throw new IndexOutOfRangeException();
					}
					return this.buffer[(this.start + index) % this.buffer.Length];
				}
				set
				{
					if (index < 0 || index >= this.Count)
					{
						throw new IndexOutOfRangeException();
					}
					this.buffer[(this.start + index) % this.buffer.Length] = value;
				}
			}

			public int Count { get; private set; }

			public void Enqueue(T item)
			{
				if (this.Count == this.buffer.Length)
				{
					T[] array = new T[this.buffer.Length * 2];
					for (int i = 0; i < this.Count; i++)
					{
						array[i] = this[i];
					}
					this.buffer = array;
					this.start = 0;
				}
				this.buffer[(this.start + this.Count) % this.buffer.Length] = item;
				int count = this.Count;
				this.Count = count + 1;
			}

			public T Dequeue()
			{
				if (this.Count == 0)
				{
					throw new InvalidOperationException();
				}
				T result = this.buffer[this.start];
				this.start = (this.start + 1) % this.buffer.Length;
				int count = this.Count;
				this.Count = count - 1;
				return result;
			}

			private T[] buffer = new T[4];

			private int start;
		}
	}
}
