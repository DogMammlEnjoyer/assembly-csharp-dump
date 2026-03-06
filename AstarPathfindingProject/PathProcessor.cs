using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pathfinding
{
	public class PathProcessor
	{
		public event Action<Path> OnPathPreSearch;

		public event Action<Path> OnPathPostSearch;

		public event Action OnQueueUnblocked;

		public int NumThreads
		{
			get
			{
				return this.pathHandlers.Length;
			}
		}

		public bool IsUsingMultithreading
		{
			get
			{
				return this.threads != null;
			}
		}

		internal PathProcessor(AstarPath astar, PathReturnQueue returnQueue, int processors, bool multithreaded)
		{
			this.astar = astar;
			this.returnQueue = returnQueue;
			if (processors < 0)
			{
				throw new ArgumentOutOfRangeException("processors");
			}
			if (!multithreaded && processors != 1)
			{
				throw new Exception("Only a single non-multithreaded processor is allowed");
			}
			this.queue = new ThreadControlQueue(processors);
			this.pathHandlers = new PathHandler[processors];
			for (int i = 0; i < processors; i++)
			{
				this.pathHandlers[i] = new PathHandler(i, processors);
			}
			if (multithreaded)
			{
				this.profilingSampler = CustomSampler.Create("Calculating Path", false);
				this.threads = new Thread[processors];
				for (int j = 0; j < processors; j++)
				{
					PathHandler pathHandler = this.pathHandlers[j];
					this.threads[j] = new Thread(delegate()
					{
						this.CalculatePathsThreaded(pathHandler);
					});
					this.threads[j].Name = "Pathfinding Thread " + j.ToString();
					this.threads[j].IsBackground = true;
					this.threads[j].Start();
				}
				return;
			}
			this.threadCoroutine = this.CalculatePaths(this.pathHandlers[0]);
		}

		private int Lock(bool block)
		{
			this.queue.Block();
			if (block)
			{
				while (!this.queue.AllReceiversBlocked)
				{
					if (this.IsUsingMultithreading)
					{
						Thread.Sleep(1);
					}
					else
					{
						this.TickNonMultithreaded();
					}
				}
			}
			this.nextLockID++;
			this.locks.Add(this.nextLockID);
			return this.nextLockID;
		}

		private void Unlock(int id)
		{
			if (!this.locks.Remove(id))
			{
				throw new ArgumentException("This lock has already been released");
			}
			if (this.locks.Count == 0)
			{
				if (this.OnQueueUnblocked != null)
				{
					this.OnQueueUnblocked();
				}
				this.queue.Unblock();
			}
		}

		public PathProcessor.GraphUpdateLock PausePathfinding(bool block)
		{
			return new PathProcessor.GraphUpdateLock(this, block);
		}

		public void TickNonMultithreaded()
		{
			if (this.threadCoroutine != null)
			{
				try
				{
					this.threadCoroutine.MoveNext();
				}
				catch (Exception ex)
				{
					this.threadCoroutine = null;
					if (!(ex is ThreadControlQueue.QueueTerminationException))
					{
						Debug.LogException(ex);
						Debug.LogError("Unhandled exception during pathfinding. Terminating.");
						this.queue.TerminateReceivers();
						try
						{
							this.queue.PopNoBlock(false);
						}
						catch
						{
						}
					}
				}
			}
		}

		public void JoinThreads()
		{
			if (this.threads != null)
			{
				for (int i = 0; i < this.threads.Length; i++)
				{
					if (!this.threads[i].Join(200))
					{
						Debug.LogError("Could not terminate pathfinding thread[" + i.ToString() + "] in 200ms, trying Thread.Abort");
						this.threads[i].Abort();
					}
				}
			}
		}

		public void AbortThreads()
		{
			if (this.threads == null)
			{
				return;
			}
			for (int i = 0; i < this.threads.Length; i++)
			{
				if (this.threads[i] != null && this.threads[i].IsAlive)
				{
					this.threads[i].Abort();
				}
			}
		}

		public int GetNewNodeIndex()
		{
			if (this.nodeIndexPool.Count <= 0)
			{
				int num = this.nextNodeIndex;
				this.nextNodeIndex = num + 1;
				return num;
			}
			return this.nodeIndexPool.Pop();
		}

		public void InitializeNode(GraphNode node)
		{
			if (!this.queue.AllReceiversBlocked)
			{
				throw new Exception("Trying to initialize a node when it is not safe to initialize any nodes. Must be done during a graph update. See http://arongranberg.com/astar/docs/graph-updates.php#direct");
			}
			for (int i = 0; i < this.pathHandlers.Length; i++)
			{
				this.pathHandlers[i].InitializeNode(node);
			}
			this.astar.hierarchicalGraph.OnCreatedNode(node);
		}

		public void DestroyNode(GraphNode node)
		{
			if (node.NodeIndex == -1)
			{
				return;
			}
			this.nodeIndexPool.Push(node.NodeIndex);
			for (int i = 0; i < this.pathHandlers.Length; i++)
			{
				this.pathHandlers[i].DestroyNode(node);
			}
			this.astar.hierarchicalGraph.AddDirtyNode(node);
		}

		private void CalculatePathsThreaded(PathHandler pathHandler)
		{
			try
			{
				long num = 100000L;
				long targetTick = DateTime.UtcNow.Ticks + num;
				for (;;)
				{
					Path path = this.queue.Pop();
					IPathInternals pathInternals = path;
					pathInternals.PrepareBase(pathHandler);
					pathInternals.AdvanceState(PathState.Processing);
					if (this.OnPathPreSearch != null)
					{
						this.OnPathPreSearch(path);
					}
					long ticks = DateTime.UtcNow.Ticks;
					pathInternals.Prepare();
					if (path.CompleteState == PathCompleteState.NotCalculated)
					{
						this.astar.debugPathData = pathInternals.PathHandler;
						this.astar.debugPathID = path.pathID;
						pathInternals.Initialize();
						while (path.CompleteState == PathCompleteState.NotCalculated)
						{
							pathInternals.CalculateStep(targetTick);
							targetTick = DateTime.UtcNow.Ticks + num;
							if (this.queue.IsTerminating)
							{
								path.FailWithError("AstarPath object destroyed");
							}
						}
						path.duration = (float)(DateTime.UtcNow.Ticks - ticks) * 0.0001f;
					}
					pathInternals.Cleanup();
					if (path.immediateCallback != null)
					{
						path.immediateCallback(path);
					}
					if (this.OnPathPostSearch != null)
					{
						this.OnPathPostSearch(path);
					}
					this.returnQueue.Enqueue(path);
					pathInternals.AdvanceState(PathState.ReturnQueue);
				}
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException || ex is ThreadControlQueue.QueueTerminationException)
				{
					if (this.astar.logPathResults == PathLog.Heavy)
					{
						Debug.LogWarning("Shutting down pathfinding thread #" + pathHandler.threadID.ToString());
					}
					return;
				}
				Debug.LogException(ex);
				Debug.LogError("Unhandled exception during pathfinding. Terminating.");
				this.queue.TerminateReceivers();
			}
			finally
			{
				Profiler.EndThreadProfiling();
			}
			Debug.LogError("Error : This part should never be reached.");
			this.queue.ReceiverTerminated();
		}

		private IEnumerator CalculatePaths(PathHandler pathHandler)
		{
			long maxTicks = (long)(this.astar.maxFrameTime * 10000f);
			long targetTick = DateTime.UtcNow.Ticks + maxTicks;
			for (;;)
			{
				Path p = null;
				bool blockedBefore = false;
				while (p == null)
				{
					try
					{
						p = this.queue.PopNoBlock(blockedBefore);
						blockedBefore |= (p == null);
					}
					catch (ThreadControlQueue.QueueTerminationException)
					{
						yield break;
					}
					if (p == null)
					{
						yield return null;
					}
				}
				IPathInternals ip = p;
				maxTicks = (long)(this.astar.maxFrameTime * 10000f);
				ip.PrepareBase(pathHandler);
				ip.AdvanceState(PathState.Processing);
				Action<Path> onPathPreSearch = this.OnPathPreSearch;
				if (onPathPreSearch != null)
				{
					onPathPreSearch(p);
				}
				long ticks = DateTime.UtcNow.Ticks;
				long totalTicks = 0L;
				ip.Prepare();
				if (p.CompleteState == PathCompleteState.NotCalculated)
				{
					this.astar.debugPathData = ip.PathHandler;
					this.astar.debugPathID = p.pathID;
					ip.Initialize();
					while (p.CompleteState == PathCompleteState.NotCalculated)
					{
						ip.CalculateStep(targetTick);
						if (p.CompleteState != PathCompleteState.NotCalculated)
						{
							break;
						}
						totalTicks += DateTime.UtcNow.Ticks - ticks;
						yield return null;
						ticks = DateTime.UtcNow.Ticks;
						if (this.queue.IsTerminating)
						{
							p.FailWithError("AstarPath object destroyed");
						}
						targetTick = DateTime.UtcNow.Ticks + maxTicks;
					}
					totalTicks += DateTime.UtcNow.Ticks - ticks;
					p.duration = (float)totalTicks * 0.0001f;
				}
				ip.Cleanup();
				OnPathDelegate immediateCallback = p.immediateCallback;
				if (immediateCallback != null)
				{
					immediateCallback(p);
				}
				Action<Path> onPathPostSearch = this.OnPathPostSearch;
				if (onPathPostSearch != null)
				{
					onPathPostSearch(p);
				}
				this.returnQueue.Enqueue(p);
				ip.AdvanceState(PathState.ReturnQueue);
				if (DateTime.UtcNow.Ticks > targetTick)
				{
					yield return null;
					targetTick = DateTime.UtcNow.Ticks + maxTicks;
				}
				p = null;
				ip = null;
			}
			yield break;
		}

		internal readonly ThreadControlQueue queue;

		private readonly AstarPath astar;

		private readonly PathReturnQueue returnQueue;

		private readonly PathHandler[] pathHandlers;

		private readonly Thread[] threads;

		private IEnumerator threadCoroutine;

		private int nextNodeIndex = 1;

		private readonly Stack<int> nodeIndexPool = new Stack<int>();

		private readonly List<int> locks = new List<int>();

		private int nextLockID;

		private CustomSampler profilingSampler;

		public struct GraphUpdateLock
		{
			public GraphUpdateLock(PathProcessor pathProcessor, bool block)
			{
				this.pathProcessor = pathProcessor;
				this.id = pathProcessor.Lock(block);
			}

			public bool Held
			{
				get
				{
					return this.pathProcessor != null && this.pathProcessor.locks.Contains(this.id);
				}
			}

			public void Release()
			{
				this.pathProcessor.Unlock(this.id);
			}

			private PathProcessor pathProcessor;

			private int id;
		}
	}
}
