using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	public abstract class Path : IPathInternals
	{
		public PathState PipelineState { get; private set; }

		public PathCompleteState CompleteState
		{
			get
			{
				return this.completeState;
			}
			protected set
			{
				object obj = this.stateLock;
				lock (obj)
				{
					if (this.completeState != PathCompleteState.Error)
					{
						this.completeState = value;
					}
				}
			}
		}

		public bool error
		{
			get
			{
				return this.CompleteState == PathCompleteState.Error;
			}
		}

		public string errorLog { get; private set; }

		public int searchedNodes { get; protected set; }

		bool IPathInternals.Pooled { get; set; }

		[Obsolete("Has been renamed to 'Pooled' to use more widely underestood terminology", true)]
		internal bool recycled
		{
			get
			{
				return false;
			}
		}

		public ushort pathID { get; private set; }

		public int[] tagPenalties
		{
			get
			{
				return this.manualTagPenalties;
			}
			set
			{
				if (value == null || value.Length != 32)
				{
					this.manualTagPenalties = null;
					this.internalTagPenalties = Path.ZeroTagPenalties;
					return;
				}
				this.manualTagPenalties = value;
				this.internalTagPenalties = value;
			}
		}

		public virtual bool FloodingPath
		{
			get
			{
				return false;
			}
		}

		public float GetTotalLength()
		{
			if (this.vectorPath == null)
			{
				return float.PositiveInfinity;
			}
			float num = 0f;
			for (int i = 0; i < this.vectorPath.Count - 1; i++)
			{
				num += Vector3.Distance(this.vectorPath[i], this.vectorPath[i + 1]);
			}
			return num;
		}

		public IEnumerator WaitForPath()
		{
			if (this.PipelineState == PathState.Created)
			{
				throw new InvalidOperationException("This path has not been started yet");
			}
			while (this.PipelineState != PathState.Returned)
			{
				yield return null;
			}
			yield break;
		}

		public void BlockUntilCalculated()
		{
			AstarPath.BlockUntilCalculated(this);
		}

		internal uint CalculateHScore(GraphNode node)
		{
			switch (this.heuristic)
			{
			case Heuristic.Manhattan:
			{
				Int3 position = node.position;
				uint num = (uint)((float)(Math.Abs(this.hTarget.x - position.x) + Math.Abs(this.hTarget.y - position.y) + Math.Abs(this.hTarget.z - position.z)) * this.heuristicScale);
				if (this.hTargetNode != null)
				{
					num = Math.Max(num, AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex));
				}
				return num;
			}
			case Heuristic.DiagonalManhattan:
			{
				Int3 @int = this.GetHTarget() - node.position;
				@int.x = Math.Abs(@int.x);
				@int.y = Math.Abs(@int.y);
				@int.z = Math.Abs(@int.z);
				int num2 = Math.Min(@int.x, @int.z);
				int num3 = Math.Max(@int.x, @int.z);
				uint num = (uint)((float)(14 * num2 / 10 + (num3 - num2) + @int.y) * this.heuristicScale);
				if (this.hTargetNode != null)
				{
					num = Math.Max(num, AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex));
				}
				return num;
			}
			case Heuristic.Euclidean:
			{
				uint num = (uint)((float)(this.GetHTarget() - node.position).costMagnitude * this.heuristicScale);
				if (this.hTargetNode != null)
				{
					num = Math.Max(num, AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex));
				}
				return num;
			}
			default:
				return 0U;
			}
		}

		public uint GetTagPenalty(int tag)
		{
			return (uint)this.internalTagPenalties[tag];
		}

		protected Int3 GetHTarget()
		{
			return this.hTarget;
		}

		public bool CanTraverse(GraphNode node)
		{
			if (this.traversalProvider != null)
			{
				return this.traversalProvider.CanTraverse(this, node);
			}
			return node.Walkable && (this.enabledTags >> (int)node.Tag & 1) != 0;
		}

		public uint GetTraversalCost(GraphNode node)
		{
			if (this.traversalProvider != null)
			{
				return this.traversalProvider.GetTraversalCost(this, node);
			}
			return this.GetTagPenalty((int)node.Tag) + node.Penalty;
		}

		public virtual uint GetConnectionSpecialCost(GraphNode a, GraphNode b, uint currentCost)
		{
			return currentCost;
		}

		public bool IsDone()
		{
			return this.PipelineState > PathState.Processing;
		}

		void IPathInternals.AdvanceState(PathState s)
		{
			object obj = this.stateLock;
			lock (obj)
			{
				this.PipelineState = (PathState)Math.Max((int)this.PipelineState, (int)s);
			}
		}

		[Obsolete("Use the 'PipelineState' property instead")]
		public PathState GetState()
		{
			return this.PipelineState;
		}

		public void FailWithError(string msg)
		{
			this.Error();
			if (this.errorLog != "")
			{
				this.errorLog = this.errorLog + "\n" + msg;
				return;
			}
			this.errorLog = msg;
		}

		[Obsolete("Use FailWithError instead")]
		protected void LogError(string msg)
		{
			this.Log(msg);
		}

		[Obsolete("Use FailWithError instead")]
		protected void Log(string msg)
		{
			this.errorLog += msg;
		}

		public void Error()
		{
			this.CompleteState = PathCompleteState.Error;
		}

		private void ErrorCheck()
		{
			if (!this.hasBeenReset)
			{
				this.FailWithError("Please use the static Construct function for creating paths, do not use the normal constructors.");
			}
			if (((IPathInternals)this).Pooled)
			{
				this.FailWithError("The path is currently in a path pool. Are you sending the path for calculation twice?");
			}
			if (this.pathHandler == null)
			{
				this.FailWithError("Field pathHandler is not set. Please report this bug.");
			}
			if (this.PipelineState > PathState.Processing)
			{
				this.FailWithError("This path has already been processed. Do not request a path with the same path object twice.");
			}
		}

		protected virtual void OnEnterPool()
		{
			if (this.vectorPath != null)
			{
				ListPool<Vector3>.Release(ref this.vectorPath);
			}
			if (this.path != null)
			{
				ListPool<GraphNode>.Release(ref this.path);
			}
			this.callback = null;
			this.immediateCallback = null;
			this.traversalProvider = null;
			this.pathHandler = null;
		}

		protected virtual void Reset()
		{
			if (AstarPath.active == null)
			{
				throw new NullReferenceException("No AstarPath object found in the scene. Make sure there is one or do not create paths in Awake");
			}
			this.hasBeenReset = true;
			this.PipelineState = PathState.Created;
			this.releasedNotSilent = false;
			this.pathHandler = null;
			this.callback = null;
			this.immediateCallback = null;
			this.errorLog = "";
			this.completeState = PathCompleteState.NotCalculated;
			this.path = ListPool<GraphNode>.Claim();
			this.vectorPath = ListPool<Vector3>.Claim();
			this.currentR = null;
			this.duration = 0f;
			this.searchedNodes = 0;
			this.nnConstraint = PathNNConstraint.Default;
			this.next = null;
			this.heuristic = AstarPath.active.heuristic;
			this.heuristicScale = AstarPath.active.heuristicScale;
			this.enabledTags = -1;
			this.tagPenalties = null;
			this.pathID = AstarPath.active.GetNextPathID();
			this.hTarget = Int3.zero;
			this.hTargetNode = null;
			this.traversalProvider = null;
		}

		public void Claim(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException("o");
			}
			for (int i = 0; i < this.claimed.Count; i++)
			{
				if (this.claimed[i] == o)
				{
					throw new ArgumentException("You have already claimed the path with that object (" + ((o != null) ? o.ToString() : null) + "). Are you claiming the path with the same object twice?");
				}
			}
			this.claimed.Add(o);
		}

		[Obsolete("Use Release(o, true) instead")]
		internal void ReleaseSilent(object o)
		{
			this.Release(o, true);
		}

		public void Release(object o, bool silent = false)
		{
			if (o == null)
			{
				throw new ArgumentNullException("o");
			}
			for (int i = 0; i < this.claimed.Count; i++)
			{
				if (this.claimed[i] == o)
				{
					this.claimed.RemoveAt(i);
					if (!silent)
					{
						this.releasedNotSilent = true;
					}
					if (this.claimed.Count == 0 && this.releasedNotSilent)
					{
						PathPool.Pool(this);
					}
					return;
				}
			}
			if (this.claimed.Count == 0)
			{
				throw new ArgumentException("You are releasing a path which is not claimed at all (most likely it has been pooled already). Are you releasing the path with the same object (" + ((o != null) ? o.ToString() : null) + ") twice?\nCheck out the documentation on path pooling for help.");
			}
			throw new ArgumentException("You are releasing a path which has not been claimed with this object (" + ((o != null) ? o.ToString() : null) + "). Are you releasing the path with the same object twice?\nCheck out the documentation on path pooling for help.");
		}

		protected virtual void Trace(PathNode from)
		{
			PathNode pathNode = from;
			int num = 0;
			while (pathNode != null)
			{
				pathNode = pathNode.parent;
				num++;
				if (num > 16384)
				{
					Debug.LogWarning("Infinite loop? >16384 node path. Remove this message if you really have that long paths (Path.cs, Trace method)");
					break;
				}
			}
			if (this.path.Capacity < num)
			{
				this.path.Capacity = num;
			}
			if (this.vectorPath.Capacity < num)
			{
				this.vectorPath.Capacity = num;
			}
			pathNode = from;
			for (int i = 0; i < num; i++)
			{
				this.path.Add(pathNode.node);
				pathNode = pathNode.parent;
			}
			int num2 = num / 2;
			for (int j = 0; j < num2; j++)
			{
				GraphNode value = this.path[j];
				this.path[j] = this.path[num - j - 1];
				this.path[num - j - 1] = value;
			}
			for (int k = 0; k < num; k++)
			{
				this.vectorPath.Add((Vector3)this.path[k].position);
			}
		}

		protected void DebugStringPrefix(PathLog logMode, StringBuilder text)
		{
			text.Append(this.error ? "Path Failed : " : "Path Completed : ");
			text.Append("Computation Time ");
			text.Append(this.duration.ToString((logMode == PathLog.Heavy) ? "0.000 ms " : "0.00 ms "));
			text.Append("Searched Nodes ").Append(this.searchedNodes);
			if (!this.error)
			{
				text.Append(" Path Length ");
				text.Append((this.path == null) ? "Null" : this.path.Count.ToString());
			}
		}

		protected void DebugStringSuffix(PathLog logMode, StringBuilder text)
		{
			if (this.error)
			{
				text.Append("\nError: ").Append(this.errorLog);
			}
			if (logMode == PathLog.Heavy && !AstarPath.active.IsUsingMultithreading)
			{
				text.Append("\nCallback references ");
				if (this.callback != null)
				{
					text.Append(this.callback.Target.GetType().FullName).AppendLine();
				}
				else
				{
					text.AppendLine("NULL");
				}
			}
			text.Append("\nPath Number ").Append(this.pathID).Append(" (unique id)");
		}

		protected virtual string DebugString(PathLog logMode)
		{
			if (logMode == PathLog.None || (!this.error && logMode == PathLog.OnlyErrors))
			{
				return "";
			}
			StringBuilder debugStringBuilder = this.pathHandler.DebugStringBuilder;
			debugStringBuilder.Length = 0;
			this.DebugStringPrefix(logMode, debugStringBuilder);
			this.DebugStringSuffix(logMode, debugStringBuilder);
			return debugStringBuilder.ToString();
		}

		protected virtual void ReturnPath()
		{
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		protected void PrepareBase(PathHandler pathHandler)
		{
			if (pathHandler.PathID > this.pathID)
			{
				pathHandler.ClearPathIDs();
			}
			this.pathHandler = pathHandler;
			pathHandler.InitializeForPath(this);
			if (this.internalTagPenalties == null || this.internalTagPenalties.Length != 32)
			{
				this.internalTagPenalties = Path.ZeroTagPenalties;
			}
			try
			{
				this.ErrorCheck();
			}
			catch (Exception ex)
			{
				this.FailWithError(ex.Message);
			}
		}

		protected abstract void Prepare();

		protected virtual void Cleanup()
		{
		}

		protected abstract void Initialize();

		protected abstract void CalculateStep(long targetTick);

		PathHandler IPathInternals.PathHandler
		{
			get
			{
				return this.pathHandler;
			}
		}

		void IPathInternals.OnEnterPool()
		{
			this.OnEnterPool();
		}

		void IPathInternals.Reset()
		{
			this.Reset();
		}

		void IPathInternals.ReturnPath()
		{
			this.ReturnPath();
		}

		void IPathInternals.PrepareBase(PathHandler handler)
		{
			this.PrepareBase(handler);
		}

		void IPathInternals.Prepare()
		{
			this.Prepare();
		}

		void IPathInternals.Cleanup()
		{
			this.Cleanup();
		}

		void IPathInternals.Initialize()
		{
			this.Initialize();
		}

		void IPathInternals.CalculateStep(long targetTick)
		{
			this.CalculateStep(targetTick);
		}

		string IPathInternals.DebugString(PathLog logMode)
		{
			return this.DebugString(logMode);
		}

		protected PathHandler pathHandler;

		public OnPathDelegate callback;

		public OnPathDelegate immediateCallback;

		private object stateLock = new object();

		public ITraversalProvider traversalProvider;

		protected PathCompleteState completeState;

		public List<GraphNode> path;

		public List<Vector3> vectorPath;

		protected PathNode currentR;

		public float duration;

		protected bool hasBeenReset;

		public NNConstraint nnConstraint = PathNNConstraint.Default;

		internal Path next;

		public Heuristic heuristic;

		public float heuristicScale = 1f;

		protected GraphNode hTargetNode;

		protected Int3 hTarget;

		public int enabledTags = -1;

		private static readonly int[] ZeroTagPenalties = new int[32];

		protected int[] internalTagPenalties;

		protected int[] manualTagPenalties;

		private List<object> claimed = new List<object>();

		private bool releasedNotSilent;
	}
}
