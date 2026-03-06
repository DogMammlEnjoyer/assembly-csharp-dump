using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pathfinding
{
	internal class GraphUpdateProcessor
	{
		public event Action OnGraphsUpdated;

		public bool IsAnyGraphUpdateQueued
		{
			get
			{
				return this.graphUpdateQueue.Count > 0;
			}
		}

		public bool IsAnyGraphUpdateInProgress
		{
			get
			{
				return this.anyGraphUpdateInProgress;
			}
		}

		public GraphUpdateProcessor(AstarPath astar)
		{
			this.astar = astar;
		}

		public AstarWorkItem GetWorkItem()
		{
			return new AstarWorkItem(new Action(this.QueueGraphUpdatesInternal), new Func<bool, bool>(this.ProcessGraphUpdates));
		}

		public void EnableMultithreading()
		{
			if (this.graphUpdateThread == null || !this.graphUpdateThread.IsAlive)
			{
				this.asyncUpdateProfilingSampler = CustomSampler.Create("Graph Update", false);
				this.graphUpdateThread = new Thread(new ThreadStart(this.ProcessGraphUpdatesAsync));
				this.graphUpdateThread.IsBackground = true;
				this.graphUpdateThread.Priority = ThreadPriority.Lowest;
				this.graphUpdateThread.Start();
			}
		}

		public void DisableMultithreading()
		{
			if (this.graphUpdateThread != null && this.graphUpdateThread.IsAlive)
			{
				this.exitAsyncThread.Set();
				if (!this.graphUpdateThread.Join(5000))
				{
					Debug.LogError("Graph update thread did not exit in 5 seconds");
				}
				this.graphUpdateThread = null;
			}
		}

		public void AddToQueue(GraphUpdateObject ob)
		{
			this.graphUpdateQueue.Enqueue(ob);
		}

		private void QueueGraphUpdatesInternal()
		{
			while (this.graphUpdateQueue.Count > 0)
			{
				GraphUpdateObject graphUpdateObject = this.graphUpdateQueue.Dequeue();
				if (graphUpdateObject.internalStage != -2)
				{
					Debug.LogError("Expected remaining graph updates to be pending");
				}
				else
				{
					graphUpdateObject.internalStage = 0;
					foreach (object obj in this.astar.data.GetUpdateableGraphs())
					{
						IUpdatableGraph updatableGraph = (IUpdatableGraph)obj;
						NavGraph graph = updatableGraph as NavGraph;
						if (graphUpdateObject.nnConstraint == null || graphUpdateObject.nnConstraint.SuitableGraph(this.astar.data.GetGraphIndex(graph), graph))
						{
							GraphUpdateProcessor.GUOSingle item = default(GraphUpdateProcessor.GUOSingle);
							item.order = GraphUpdateProcessor.GraphUpdateOrder.GraphUpdate;
							item.obj = graphUpdateObject;
							item.graph = updatableGraph;
							graphUpdateObject.internalStage++;
							this.graphUpdateQueueRegular.Enqueue(item);
						}
					}
				}
			}
			GraphModifier.TriggerEvent(GraphModifier.EventType.PreUpdate);
			this.anyGraphUpdateInProgress = true;
		}

		private bool ProcessGraphUpdates(bool force)
		{
			if (force)
			{
				this.asyncGraphUpdatesComplete.WaitOne();
			}
			else if (!this.asyncGraphUpdatesComplete.WaitOne(0))
			{
				return false;
			}
			this.ProcessPostUpdates();
			if (!this.ProcessRegularUpdates(force))
			{
				return false;
			}
			GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
			if (this.OnGraphsUpdated != null)
			{
				this.OnGraphsUpdated();
			}
			this.anyGraphUpdateInProgress = false;
			return true;
		}

		private bool ProcessRegularUpdates(bool force)
		{
			while (this.graphUpdateQueueRegular.Count > 0)
			{
				GraphUpdateProcessor.GUOSingle guosingle = this.graphUpdateQueueRegular.Peek();
				GraphUpdateThreading graphUpdateThreading = guosingle.graph.CanUpdateAsync(guosingle.obj);
				if (force || !Application.isPlaying || this.graphUpdateThread == null || !this.graphUpdateThread.IsAlive)
				{
					graphUpdateThreading &= (GraphUpdateThreading)(-2);
				}
				if ((graphUpdateThreading & GraphUpdateThreading.UnityInit) != GraphUpdateThreading.UnityThread)
				{
					if (this.StartAsyncUpdatesIfQueued())
					{
						return false;
					}
					guosingle.graph.UpdateAreaInit(guosingle.obj);
				}
				if ((graphUpdateThreading & GraphUpdateThreading.SeparateThread) != GraphUpdateThreading.UnityThread)
				{
					this.graphUpdateQueueRegular.Dequeue();
					this.graphUpdateQueueAsync.Enqueue(guosingle);
					if ((graphUpdateThreading & GraphUpdateThreading.UnityPost) != GraphUpdateThreading.UnityThread && this.StartAsyncUpdatesIfQueued())
					{
						return false;
					}
				}
				else
				{
					if (this.StartAsyncUpdatesIfQueued())
					{
						return false;
					}
					this.graphUpdateQueueRegular.Dequeue();
					try
					{
						guosingle.graph.UpdateArea(guosingle.obj);
					}
					catch (Exception ex)
					{
						string str = "Error while updating graphs\n";
						Exception ex2 = ex;
						Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
					}
					if ((graphUpdateThreading & GraphUpdateThreading.UnityPost) != GraphUpdateThreading.UnityThread)
					{
						guosingle.graph.UpdateAreaPost(guosingle.obj);
					}
					guosingle.obj.internalStage--;
				}
			}
			return !this.StartAsyncUpdatesIfQueued();
		}

		private bool StartAsyncUpdatesIfQueued()
		{
			if (this.graphUpdateQueueAsync.Count > 0)
			{
				this.asyncGraphUpdatesComplete.Reset();
				this.graphUpdateAsyncEvent.Set();
				return true;
			}
			return false;
		}

		private void ProcessPostUpdates()
		{
			while (this.graphUpdateQueuePost.Count > 0)
			{
				GraphUpdateProcessor.GUOSingle guosingle = this.graphUpdateQueuePost.Dequeue();
				if ((guosingle.graph.CanUpdateAsync(guosingle.obj) & GraphUpdateThreading.UnityPost) != GraphUpdateThreading.UnityThread)
				{
					try
					{
						guosingle.graph.UpdateAreaPost(guosingle.obj);
					}
					catch (Exception ex)
					{
						string str = "Error while updating graphs (post step)\n";
						Exception ex2 = ex;
						Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
					}
				}
				guosingle.obj.internalStage--;
			}
		}

		private void ProcessGraphUpdatesAsync()
		{
			AutoResetEvent[] array = new AutoResetEvent[]
			{
				this.graphUpdateAsyncEvent,
				this.exitAsyncThread
			};
			for (;;)
			{
				WaitHandle[] waitHandles = array;
				if (WaitHandle.WaitAny(waitHandles) == 1)
				{
					break;
				}
				while (this.graphUpdateQueueAsync.Count > 0)
				{
					GraphUpdateProcessor.GUOSingle guosingle = this.graphUpdateQueueAsync.Dequeue();
					try
					{
						if (guosingle.order != GraphUpdateProcessor.GraphUpdateOrder.GraphUpdate)
						{
							throw new NotSupportedException(guosingle.order.ToString() ?? "");
						}
						guosingle.graph.UpdateArea(guosingle.obj);
						this.graphUpdateQueuePost.Enqueue(guosingle);
					}
					catch (Exception ex)
					{
						string str = "Exception while updating graphs:\n";
						Exception ex2 = ex;
						Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
					}
				}
				this.asyncGraphUpdatesComplete.Set();
			}
			while (this.graphUpdateQueueAsync.Count > 0)
			{
				this.graphUpdateQueueAsync.Dequeue().obj.internalStage = -3;
			}
			this.asyncGraphUpdatesComplete.Set();
			Profiler.EndThreadProfiling();
		}

		private readonly AstarPath astar;

		private Thread graphUpdateThread;

		private bool anyGraphUpdateInProgress;

		private CustomSampler asyncUpdateProfilingSampler;

		private readonly Queue<GraphUpdateObject> graphUpdateQueue = new Queue<GraphUpdateObject>();

		private readonly Queue<GraphUpdateProcessor.GUOSingle> graphUpdateQueueAsync = new Queue<GraphUpdateProcessor.GUOSingle>();

		private readonly Queue<GraphUpdateProcessor.GUOSingle> graphUpdateQueuePost = new Queue<GraphUpdateProcessor.GUOSingle>();

		private readonly Queue<GraphUpdateProcessor.GUOSingle> graphUpdateQueueRegular = new Queue<GraphUpdateProcessor.GUOSingle>();

		private readonly ManualResetEvent asyncGraphUpdatesComplete = new ManualResetEvent(true);

		private readonly AutoResetEvent graphUpdateAsyncEvent = new AutoResetEvent(false);

		private readonly AutoResetEvent exitAsyncThread = new AutoResetEvent(false);

		private enum GraphUpdateOrder
		{
			GraphUpdate
		}

		private struct GUOSingle
		{
			public GraphUpdateProcessor.GraphUpdateOrder order;

			public IUpdatableGraph graph;

			public GraphUpdateObject obj;
		}
	}
}
