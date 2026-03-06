using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	[JsonOptIn]
	[Preserve]
	public class PointGraph : NavGraph, IUpdatableGraph
	{
		public int nodeCount { get; protected set; }

		public override int CountNodes()
		{
			return this.nodeCount;
		}

		public override void GetNodes(Action<GraphNode> action)
		{
			if (this.nodes == null)
			{
				return;
			}
			int nodeCount = this.nodeCount;
			for (int i = 0; i < nodeCount; i++)
			{
				action(this.nodes[i]);
			}
		}

		public override NNInfoInternal GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			return this.GetNearestInternal(position, constraint, true);
		}

		public override NNInfoInternal GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			return this.GetNearestInternal(position, constraint, false);
		}

		private NNInfoInternal GetNearestInternal(Vector3 position, NNConstraint constraint, bool fastCheck)
		{
			if (this.nodes == null)
			{
				return default(NNInfoInternal);
			}
			Int3 @int = (Int3)position;
			if (!this.optimizeForSparseGraph)
			{
				float num = (constraint == null || constraint.constrainDistance) ? AstarPath.active.maxNearestNodeDistanceSqr : float.PositiveInfinity;
				num *= 1000000f;
				NNInfoInternal nninfoInternal = new NNInfoInternal(null);
				long num2 = long.MaxValue;
				long num3 = long.MaxValue;
				for (int i = 0; i < this.nodeCount; i++)
				{
					PointNode pointNode = this.nodes[i];
					long sqrMagnitudeLong = (@int - pointNode.position).sqrMagnitudeLong;
					if (sqrMagnitudeLong < num2)
					{
						num2 = sqrMagnitudeLong;
						nninfoInternal.node = pointNode;
					}
					if (sqrMagnitudeLong < num3 && (float)sqrMagnitudeLong < num && (constraint == null || constraint.Suitable(pointNode)))
					{
						num3 = sqrMagnitudeLong;
						nninfoInternal.constrainedNode = pointNode;
					}
				}
				if (!fastCheck)
				{
					nninfoInternal.node = nninfoInternal.constrainedNode;
				}
				nninfoInternal.UpdateInfo();
				return nninfoInternal;
			}
			if (this.nearestNodeDistanceMode == PointGraph.NodeDistanceMode.Node)
			{
				return new NNInfoInternal(this.lookupTree.GetNearest(@int, fastCheck ? null : constraint));
			}
			GraphNode nearestConnection = this.lookupTree.GetNearestConnection(@int, fastCheck ? null : constraint, this.maximumConnectionLength);
			if (nearestConnection == null)
			{
				return default(NNInfoInternal);
			}
			return this.FindClosestConnectionPoint(nearestConnection as PointNode, position);
		}

		private NNInfoInternal FindClosestConnectionPoint(PointNode node, Vector3 position)
		{
			Vector3 clampedPosition = (Vector3)node.position;
			Connection[] connections = node.connections;
			Vector3 vector = (Vector3)node.position;
			float num = float.PositiveInfinity;
			if (connections != null)
			{
				for (int i = 0; i < connections.Length; i++)
				{
					Vector3 lineEnd = ((Vector3)connections[i].node.position + vector) * 0.5f;
					Vector3 vector2 = VectorMath.ClosestPointOnSegment(vector, lineEnd, position);
					float sqrMagnitude = (vector2 - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						clampedPosition = vector2;
					}
				}
			}
			return new NNInfoInternal
			{
				node = node,
				clampedPosition = clampedPosition
			};
		}

		public PointNode AddNode(Int3 position)
		{
			return this.AddNode<PointNode>(new PointNode(this.active), position);
		}

		public T AddNode<T>(T node, Int3 position) where T : PointNode
		{
			if (this.nodes == null || this.nodeCount == this.nodes.Length)
			{
				PointNode[] array = new PointNode[(this.nodes != null) ? Math.Max(this.nodes.Length + 4, this.nodes.Length * 2) : 4];
				if (this.nodes != null)
				{
					this.nodes.CopyTo(array, 0);
				}
				this.nodes = array;
			}
			node.SetPosition(position);
			node.GraphIndex = this.graphIndex;
			node.Walkable = true;
			this.nodes[this.nodeCount] = node;
			int nodeCount = this.nodeCount;
			this.nodeCount = nodeCount + 1;
			if (this.optimizeForSparseGraph)
			{
				this.AddToLookup(node);
			}
			return node;
		}

		protected static int CountChildren(Transform tr)
		{
			int num = 0;
			foreach (object obj in tr)
			{
				Transform tr2 = (Transform)obj;
				num++;
				num += PointGraph.CountChildren(tr2);
			}
			return num;
		}

		protected void AddChildren(ref int c, Transform tr)
		{
			foreach (object obj in tr)
			{
				Transform transform = (Transform)obj;
				this.nodes[c].position = (Int3)transform.position;
				this.nodes[c].Walkable = true;
				this.nodes[c].gameObject = transform.gameObject;
				c++;
				this.AddChildren(ref c, transform);
			}
		}

		public void RebuildNodeLookup()
		{
			if (!this.optimizeForSparseGraph || this.nodes == null)
			{
				this.lookupTree = new PointKDTree();
			}
			else
			{
				PointKDTree pointKDTree = this.lookupTree;
				GraphNode[] array = this.nodes;
				pointKDTree.Rebuild(array, 0, this.nodeCount);
			}
			this.RebuildConnectionDistanceLookup();
		}

		public void RebuildConnectionDistanceLookup()
		{
			this.maximumConnectionLength = 0L;
			if (this.nearestNodeDistanceMode == PointGraph.NodeDistanceMode.Connection)
			{
				for (int i = 0; i < this.nodeCount; i++)
				{
					PointNode pointNode = this.nodes[i];
					Connection[] connections = pointNode.connections;
					if (connections != null)
					{
						for (int j = 0; j < connections.Length; j++)
						{
							long sqrMagnitudeLong = (pointNode.position - connections[j].node.position).sqrMagnitudeLong;
							this.RegisterConnectionLength(sqrMagnitudeLong);
						}
					}
				}
			}
		}

		private void AddToLookup(PointNode node)
		{
			this.lookupTree.Add(node);
		}

		public void RegisterConnectionLength(long sqrLength)
		{
			this.maximumConnectionLength = Math.Max(this.maximumConnectionLength, sqrLength);
		}

		protected virtual PointNode[] CreateNodes(int count)
		{
			PointNode[] array = new PointNode[count];
			for (int i = 0; i < this.nodeCount; i++)
			{
				array[i] = new PointNode(this.active);
			}
			return array;
		}

		protected override IEnumerable<Progress> ScanInternal()
		{
			yield return new Progress(0f, "Searching for GameObjects");
			if (this.root == null)
			{
				GameObject[] gos = (this.searchTag != null) ? GameObject.FindGameObjectsWithTag(this.searchTag) : null;
				if (gos == null)
				{
					this.nodes = new PointNode[0];
					this.nodeCount = 0;
				}
				else
				{
					yield return new Progress(0.1f, "Creating nodes");
					this.nodeCount = gos.Length;
					this.nodes = this.CreateNodes(this.nodeCount);
					for (int i = 0; i < gos.Length; i++)
					{
						this.nodes[i].position = (Int3)gos[i].transform.position;
						this.nodes[i].Walkable = true;
						this.nodes[i].gameObject = gos[i].gameObject;
					}
				}
				gos = null;
			}
			else
			{
				if (!this.recursive)
				{
					this.nodeCount = this.root.childCount;
					this.nodes = this.CreateNodes(this.nodeCount);
					int num = 0;
					using (IEnumerator enumerator = this.root.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							Transform transform = (Transform)obj;
							this.nodes[num].position = (Int3)transform.position;
							this.nodes[num].Walkable = true;
							this.nodes[num].gameObject = transform.gameObject;
							num++;
						}
						goto IL_24C;
					}
				}
				this.nodeCount = PointGraph.CountChildren(this.root);
				this.nodes = this.CreateNodes(this.nodeCount);
				int num2 = 0;
				this.AddChildren(ref num2, this.root);
			}
			IL_24C:
			yield return new Progress(0.15f, "Building node lookup");
			this.RebuildNodeLookup();
			foreach (Progress progress in this.ConnectNodesAsync())
			{
				yield return progress.MapTo(0.15f, 0.95f, null);
			}
			IEnumerator<Progress> enumerator2 = null;
			yield return new Progress(0.95f, "Building connection distances");
			this.RebuildConnectionDistanceLookup();
			yield break;
			yield break;
		}

		public void ConnectNodes()
		{
			IEnumerator<Progress> enumerator = this.ConnectNodesAsync().GetEnumerator();
			while (enumerator.MoveNext())
			{
			}
			this.RebuildConnectionDistanceLookup();
		}

		private IEnumerable<Progress> ConnectNodesAsync()
		{
			if (this.maxDistance >= 0f)
			{
				List<Connection> connections = new List<Connection>();
				List<GraphNode> candidateConnections = new List<GraphNode>();
				long maxSquaredRange;
				if (this.maxDistance == 0f && (this.limits.x == 0f || this.limits.y == 0f || this.limits.z == 0f))
				{
					maxSquaredRange = long.MaxValue;
				}
				else
				{
					maxSquaredRange = (long)(Mathf.Max(this.limits.x, Mathf.Max(this.limits.y, Mathf.Max(this.limits.z, this.maxDistance))) * 1000f) + 1L;
					maxSquaredRange *= maxSquaredRange;
				}
				int num3;
				for (int i = 0; i < this.nodeCount; i = num3 + 1)
				{
					if (i % 512 == 0)
					{
						yield return new Progress((float)i / (float)this.nodeCount, "Connecting nodes");
					}
					connections.Clear();
					PointNode pointNode = this.nodes[i];
					if (this.optimizeForSparseGraph)
					{
						candidateConnections.Clear();
						this.lookupTree.GetInRange(pointNode.position, maxSquaredRange, candidateConnections);
						for (int j = 0; j < candidateConnections.Count; j++)
						{
							PointNode pointNode2 = candidateConnections[j] as PointNode;
							float num;
							if (pointNode2 != pointNode && this.IsValidConnection(pointNode, pointNode2, out num))
							{
								connections.Add(new Connection(pointNode2, (uint)Mathf.RoundToInt(num * 1000f), byte.MaxValue));
							}
						}
					}
					else
					{
						for (int k = 0; k < this.nodeCount; k++)
						{
							if (i != k)
							{
								PointNode pointNode3 = this.nodes[k];
								float num2;
								if (this.IsValidConnection(pointNode, pointNode3, out num2))
								{
									connections.Add(new Connection(pointNode3, (uint)Mathf.RoundToInt(num2 * 1000f), byte.MaxValue));
								}
							}
						}
					}
					pointNode.connections = connections.ToArray();
					pointNode.SetConnectivityDirty();
					num3 = i;
				}
				connections = null;
				candidateConnections = null;
			}
			yield break;
		}

		public virtual bool IsValidConnection(GraphNode a, GraphNode b, out float dist)
		{
			dist = 0f;
			if (!a.Walkable || !b.Walkable)
			{
				return false;
			}
			Vector3 vector = (Vector3)(b.position - a.position);
			if ((!Mathf.Approximately(this.limits.x, 0f) && Mathf.Abs(vector.x) > this.limits.x) || (!Mathf.Approximately(this.limits.y, 0f) && Mathf.Abs(vector.y) > this.limits.y) || (!Mathf.Approximately(this.limits.z, 0f) && Mathf.Abs(vector.z) > this.limits.z))
			{
				return false;
			}
			dist = vector.magnitude;
			if (this.maxDistance != 0f && dist >= this.maxDistance)
			{
				return false;
			}
			if (!this.raycast)
			{
				return true;
			}
			Ray ray = new Ray((Vector3)a.position, vector);
			Ray ray2 = new Ray((Vector3)b.position, -vector);
			if (this.use2DPhysics)
			{
				if (this.thickRaycast)
				{
					return !Physics2D.CircleCast(ray.origin, this.thickRaycastRadius, ray.direction, dist, this.mask) && !Physics2D.CircleCast(ray2.origin, this.thickRaycastRadius, ray2.direction, dist, this.mask);
				}
				return !Physics2D.Linecast((Vector3)a.position, (Vector3)b.position, this.mask) && !Physics2D.Linecast((Vector3)b.position, (Vector3)a.position, this.mask);
			}
			else
			{
				if (this.thickRaycast)
				{
					return !Physics.SphereCast(ray, this.thickRaycastRadius, dist, this.mask) && !Physics.SphereCast(ray2, this.thickRaycastRadius, dist, this.mask);
				}
				return !Physics.Linecast((Vector3)a.position, (Vector3)b.position, this.mask) && !Physics.Linecast((Vector3)b.position, (Vector3)a.position, this.mask);
			}
		}

		GraphUpdateThreading IUpdatableGraph.CanUpdateAsync(GraphUpdateObject o)
		{
			return GraphUpdateThreading.UnityThread;
		}

		void IUpdatableGraph.UpdateAreaInit(GraphUpdateObject o)
		{
		}

		void IUpdatableGraph.UpdateAreaPost(GraphUpdateObject o)
		{
		}

		void IUpdatableGraph.UpdateArea(GraphUpdateObject guo)
		{
			if (this.nodes == null)
			{
				return;
			}
			for (int i = 0; i < this.nodeCount; i++)
			{
				PointNode pointNode = this.nodes[i];
				if (guo.bounds.Contains((Vector3)pointNode.position))
				{
					guo.WillUpdateNode(pointNode);
					guo.Apply(pointNode);
				}
			}
			if (guo.updatePhysics)
			{
				Bounds bounds = guo.bounds;
				if (this.thickRaycast)
				{
					bounds.Expand(this.thickRaycastRadius * 2f);
				}
				List<Connection> list = ListPool<Connection>.Claim();
				for (int j = 0; j < this.nodeCount; j++)
				{
					PointNode pointNode2 = this.nodes[j];
					Vector3 a = (Vector3)pointNode2.position;
					List<Connection> list2 = null;
					for (int k = 0; k < this.nodeCount; k++)
					{
						if (k != j)
						{
							Vector3 b = (Vector3)this.nodes[k].position;
							if (VectorMath.SegmentIntersectsBounds(bounds, a, b))
							{
								PointNode pointNode3 = this.nodes[k];
								bool flag = pointNode2.ContainsConnection(pointNode3);
								float num;
								bool flag2 = this.IsValidConnection(pointNode2, pointNode3, out num);
								if (list2 == null && flag != flag2)
								{
									list.Clear();
									list2 = list;
									list2.AddRange(pointNode2.connections);
								}
								if (!flag && flag2)
								{
									uint cost = (uint)Mathf.RoundToInt(num * 1000f);
									list2.Add(new Connection(pointNode3, cost, byte.MaxValue));
									this.RegisterConnectionLength((pointNode3.position - pointNode2.position).sqrMagnitudeLong);
								}
								else if (flag && !flag2)
								{
									for (int l = 0; l < list2.Count; l++)
									{
										if (list2[l].node == pointNode3)
										{
											list2.RemoveAt(l);
											break;
										}
									}
								}
							}
						}
					}
					if (list2 != null)
					{
						pointNode2.connections = list2.ToArray();
						pointNode2.SetConnectivityDirty();
					}
				}
				ListPool<Connection>.Release(ref list);
			}
		}

		protected override void PostDeserialization(GraphSerializationContext ctx)
		{
			this.RebuildNodeLookup();
		}

		public override void RelocateNodes(Matrix4x4 deltaMatrix)
		{
			base.RelocateNodes(deltaMatrix);
			this.RebuildNodeLookup();
		}

		protected override void DeserializeSettingsCompatibility(GraphSerializationContext ctx)
		{
			base.DeserializeSettingsCompatibility(ctx);
			this.root = (ctx.DeserializeUnityObject() as Transform);
			this.searchTag = ctx.reader.ReadString();
			this.maxDistance = ctx.reader.ReadSingle();
			this.limits = ctx.DeserializeVector3();
			this.raycast = ctx.reader.ReadBoolean();
			this.use2DPhysics = ctx.reader.ReadBoolean();
			this.thickRaycast = ctx.reader.ReadBoolean();
			this.thickRaycastRadius = ctx.reader.ReadSingle();
			this.recursive = ctx.reader.ReadBoolean();
			ctx.reader.ReadBoolean();
			this.mask = ctx.reader.ReadInt32();
			this.optimizeForSparseGraph = ctx.reader.ReadBoolean();
			ctx.reader.ReadBoolean();
		}

		protected override void SerializeExtraInfo(GraphSerializationContext ctx)
		{
			if (this.nodes == null)
			{
				ctx.writer.Write(-1);
			}
			ctx.writer.Write(this.nodeCount);
			for (int i = 0; i < this.nodeCount; i++)
			{
				if (this.nodes[i] == null)
				{
					ctx.writer.Write(-1);
				}
				else
				{
					ctx.writer.Write(0);
					this.nodes[i].SerializeNode(ctx);
				}
			}
		}

		protected override void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
			int num = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.nodes = null;
				return;
			}
			this.nodes = new PointNode[num];
			this.nodeCount = num;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (ctx.reader.ReadInt32() != -1)
				{
					this.nodes[i] = new PointNode(this.active);
					this.nodes[i].DeserializeNode(ctx);
				}
			}
		}

		[JsonMember]
		public Transform root;

		[JsonMember]
		public string searchTag;

		[JsonMember]
		public float maxDistance;

		[JsonMember]
		public Vector3 limits;

		[JsonMember]
		public bool raycast = true;

		[JsonMember]
		public bool use2DPhysics;

		[JsonMember]
		public bool thickRaycast;

		[JsonMember]
		public float thickRaycastRadius = 1f;

		[JsonMember]
		public bool recursive = true;

		[JsonMember]
		public LayerMask mask;

		[JsonMember]
		public bool optimizeForSparseGraph;

		private PointKDTree lookupTree = new PointKDTree();

		private long maximumConnectionLength;

		public PointNode[] nodes;

		[JsonMember]
		public PointGraph.NodeDistanceMode nearestNodeDistanceMode;

		public enum NodeDistanceMode
		{
			Node,
			Connection
		}
	}
}
