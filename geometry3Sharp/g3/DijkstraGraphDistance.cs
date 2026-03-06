using System;
using System.Collections.Generic;

namespace g3
{
	public class DijkstraGraphDistance
	{
		public DijkstraGraphDistance(int nMaxID, bool bSparse, Func<int, bool> nodeFilterF, Func<int, int, float> nodeDistanceF, Func<int, IEnumerable<int>> neighboursF, IEnumerable<Vector2d> seeds = null)
		{
			this.NodeFilterF = nodeFilterF;
			this.NodeDistanceF = nodeDistanceF;
			this.NeighboursF = neighboursF;
			if (bSparse)
			{
				this.SparseQueue = new DynamicPriorityQueue<DijkstraGraphDistance.GraphNode>();
				this.SparseNodes = new SparseObjectList<DijkstraGraphDistance.GraphNode>(nMaxID, 0);
				this.SparseNodePool = new MemoryPool<DijkstraGraphDistance.GraphNode>();
			}
			else
			{
				this.DenseQueue = new IndexPriorityQueue(nMaxID);
				this.DenseNodes = new DijkstraGraphDistance.GraphNodeStruct[nMaxID];
			}
			this.Seeds = new List<int>();
			this.max_value = float.MinValue;
			if (seeds != null)
			{
				foreach (Vector2d vector2d in seeds)
				{
					this.AddSeed((int)vector2d.x, (float)vector2d.y);
				}
			}
		}

		public static DijkstraGraphDistance MeshVertices(DMesh3 mesh, bool bSparse = false)
		{
			if (!bSparse)
			{
				return new DijkstraGraphDistance(mesh.MaxVertexID, false, (int id) => true, (int a, int b) => (float)mesh.GetVertex(a).Distance(mesh.GetVertex(b)), new Func<int, IEnumerable<int>>(mesh.VtxVerticesItr), null);
			}
			return new DijkstraGraphDistance(mesh.MaxVertexID, true, (int id) => mesh.IsVertex(id), (int a, int b) => (float)mesh.GetVertex(a).Distance(mesh.GetVertex(b)), new Func<int, IEnumerable<int>>(mesh.VtxVerticesItr), null);
		}

		public static DijkstraGraphDistance MeshTriangles(DMesh3 mesh, bool bSparse = false)
		{
			Func<int, int, float> nodeDistanceF = (int a, int b) => (float)mesh.GetTriCentroid(a).Distance(mesh.GetTriCentroid(b));
			if (!bSparse)
			{
				return new DijkstraGraphDistance(mesh.MaxTriangleID, false, (int id) => true, nodeDistanceF, new Func<int, IEnumerable<int>>(mesh.TriTrianglesItr), null);
			}
			return new DijkstraGraphDistance(mesh.MaxTriangleID, true, (int id) => mesh.IsTriangle(id), nodeDistanceF, new Func<int, IEnumerable<int>>(mesh.TriTrianglesItr), null);
		}

		public void Reset()
		{
			if (this.SparseNodes != null)
			{
				this.SparseQueue.Clear(false);
				this.SparseNodes.Clear();
				this.SparseNodePool.ReturnAll();
			}
			else
			{
				this.DenseQueue.Clear(false);
				Array.Clear(this.DenseNodes, 0, this.DenseNodes.Length);
			}
			this.Seeds = new List<int>();
			this.max_value = float.MinValue;
		}

		public void AddSeed(int id, float seed_dist)
		{
			if (this.SparseNodes != null)
			{
				DijkstraGraphDistance.GraphNode node = this.get_node(id);
				this.SparseQueue.Enqueue(node, seed_dist);
			}
			else
			{
				this.enqueue_node_dense(id, seed_dist, -1);
			}
			this.Seeds.Add(id);
		}

		public bool IsSeed(int id)
		{
			return this.Seeds.Contains(id);
		}

		public void Compute()
		{
			if (this.TrackOrder)
			{
				this.order = new List<int>();
			}
			if (this.SparseNodes != null)
			{
				this.Compute_Sparse();
				return;
			}
			this.Compute_Dense();
		}

		protected void Compute_Sparse()
		{
			while (this.SparseQueue.Count > 0)
			{
				DijkstraGraphDistance.GraphNode graphNode = this.SparseQueue.Dequeue();
				this.max_value = Math.Max(graphNode.priority, this.max_value);
				graphNode.frozen = true;
				if (this.TrackOrder)
				{
					this.order.Add(graphNode.id);
				}
				this.update_neighbours_sparse(graphNode);
			}
		}

		protected void Compute_Dense()
		{
			while (this.DenseQueue.Count > 0)
			{
				float firstPriority = this.DenseQueue.FirstPriority;
				int num = this.DenseQueue.Dequeue();
				DijkstraGraphDistance.GraphNodeStruct graphNodeStruct = this.DenseNodes[num];
				graphNodeStruct.frozen = true;
				if (this.TrackOrder)
				{
					this.order.Add(graphNodeStruct.id);
				}
				graphNodeStruct.distance = this.max_value;
				this.DenseNodes[num] = graphNodeStruct;
				this.max_value = Math.Max(firstPriority, this.max_value);
				this.update_neighbours_dense(graphNodeStruct.id);
			}
		}

		public void ComputeToMaxDistance(float fMaxDistance)
		{
			if (this.TrackOrder)
			{
				this.order = new List<int>();
			}
			if (this.SparseNodes != null)
			{
				this.ComputeToMaxDistance_Sparse(fMaxDistance);
				return;
			}
			this.ComputeToMaxDistance_Dense(fMaxDistance);
		}

		protected void ComputeToMaxDistance_Sparse(float fMaxDistance)
		{
			while (this.SparseQueue.Count > 0)
			{
				DijkstraGraphDistance.GraphNode graphNode = this.SparseQueue.Dequeue();
				this.max_value = Math.Max(graphNode.priority, this.max_value);
				if (this.max_value > fMaxDistance)
				{
					return;
				}
				graphNode.frozen = true;
				if (this.TrackOrder)
				{
					this.order.Add(graphNode.id);
				}
				this.update_neighbours_sparse(graphNode);
			}
		}

		protected void ComputeToMaxDistance_Dense(float fMaxDistance)
		{
			while (this.DenseQueue.Count > 0)
			{
				float firstPriority = this.DenseQueue.FirstPriority;
				this.max_value = Math.Max(firstPriority, this.max_value);
				if (this.max_value > fMaxDistance)
				{
					return;
				}
				int num = this.DenseQueue.Dequeue();
				DijkstraGraphDistance.GraphNodeStruct graphNodeStruct = this.DenseNodes[num];
				graphNodeStruct.frozen = true;
				if (this.TrackOrder)
				{
					this.order.Add(graphNodeStruct.id);
				}
				graphNodeStruct.distance = this.max_value;
				this.DenseNodes[num] = graphNodeStruct;
				this.update_neighbours_dense(graphNodeStruct.id);
			}
		}

		public void ComputeToNode(int node_id, float fMaxDistance = 3.4028235E+38f)
		{
			if (this.TrackOrder)
			{
				this.order = new List<int>();
			}
			if (this.SparseNodes != null)
			{
				this.ComputeToNode_Sparse(node_id, fMaxDistance);
				return;
			}
			this.ComputeToNode_Dense(node_id, fMaxDistance);
		}

		protected void ComputeToNode_Sparse(int node_id, float fMaxDistance)
		{
			while (this.SparseQueue.Count > 0)
			{
				DijkstraGraphDistance.GraphNode graphNode = this.SparseQueue.Dequeue();
				this.max_value = Math.Max(graphNode.priority, this.max_value);
				if (this.max_value > fMaxDistance)
				{
					return;
				}
				graphNode.frozen = true;
				if (this.TrackOrder)
				{
					this.order.Add(graphNode.id);
				}
				if (graphNode.id == node_id)
				{
					return;
				}
				this.update_neighbours_sparse(graphNode);
			}
		}

		protected void ComputeToNode_Dense(int node_id, float fMaxDistance)
		{
			while (this.DenseQueue.Count > 0)
			{
				float firstPriority = this.DenseQueue.FirstPriority;
				this.max_value = Math.Max(firstPriority, this.max_value);
				if (this.max_value > fMaxDistance)
				{
					return;
				}
				int num = this.DenseQueue.Dequeue();
				DijkstraGraphDistance.GraphNodeStruct graphNodeStruct = this.DenseNodes[num];
				graphNodeStruct.frozen = true;
				if (this.TrackOrder)
				{
					this.order.Add(graphNodeStruct.id);
				}
				graphNodeStruct.distance = this.max_value;
				this.DenseNodes[num] = graphNodeStruct;
				if (graphNodeStruct.id == node_id)
				{
					return;
				}
				this.update_neighbours_dense(graphNodeStruct.id);
			}
		}

		public int ComputeToNode(Func<int, bool> terminatingNodeF, float fMaxDistance = 3.4028235E+38f)
		{
			if (this.TrackOrder)
			{
				this.order = new List<int>();
			}
			if (this.SparseNodes != null)
			{
				return this.ComputeToNode_Sparse(terminatingNodeF, fMaxDistance);
			}
			return this.ComputeToNode_Dense(terminatingNodeF, fMaxDistance);
		}

		protected int ComputeToNode_Sparse(Func<int, bool> terminatingNodeF, float fMaxDistance)
		{
			while (this.SparseQueue.Count > 0)
			{
				DijkstraGraphDistance.GraphNode graphNode = this.SparseQueue.Dequeue();
				this.max_value = Math.Max(graphNode.priority, this.max_value);
				if (this.max_value > fMaxDistance)
				{
					return -1;
				}
				graphNode.frozen = true;
				if (this.TrackOrder)
				{
					this.order.Add(graphNode.id);
				}
				if (terminatingNodeF(graphNode.id))
				{
					return graphNode.id;
				}
				this.update_neighbours_sparse(graphNode);
			}
			return -1;
		}

		protected int ComputeToNode_Dense(Func<int, bool> terminatingNodeF, float fMaxDistance)
		{
			while (this.DenseQueue.Count > 0)
			{
				float firstPriority = this.DenseQueue.FirstPriority;
				this.max_value = Math.Max(firstPriority, this.max_value);
				if (this.max_value > fMaxDistance)
				{
					return -1;
				}
				int num = this.DenseQueue.Dequeue();
				DijkstraGraphDistance.GraphNodeStruct graphNodeStruct = this.DenseNodes[num];
				graphNodeStruct.frozen = true;
				if (this.TrackOrder)
				{
					this.order.Add(graphNodeStruct.id);
				}
				graphNodeStruct.distance = this.max_value;
				this.DenseNodes[num] = graphNodeStruct;
				if (terminatingNodeF(graphNodeStruct.id))
				{
					return graphNodeStruct.id;
				}
				this.update_neighbours_dense(graphNodeStruct.id);
			}
			return -1;
		}

		public float MaxDistance
		{
			get
			{
				return this.max_value;
			}
		}

		public float GetDistance(int id)
		{
			if (this.SparseNodes != null)
			{
				DijkstraGraphDistance.GraphNode graphNode = this.SparseNodes[id];
				if (graphNode == null)
				{
					return float.MaxValue;
				}
				return graphNode.priority;
			}
			else
			{
				DijkstraGraphDistance.GraphNodeStruct graphNodeStruct = this.DenseNodes[id];
				if (!graphNodeStruct.frozen)
				{
					return float.MaxValue;
				}
				return graphNodeStruct.distance;
			}
		}

		public List<int> GetOrder()
		{
			if (!this.TrackOrder)
			{
				throw new InvalidOperationException("DijkstraGraphDistance.GetOrder: Must set TrackOrder = true");
			}
			return this.order;
		}

		public bool GetPathToSeed(int fromv, List<int> path)
		{
			if (this.SparseNodes != null)
			{
				DijkstraGraphDistance.GraphNode graphNode = this.get_node(fromv);
				if (!graphNode.frozen)
				{
					return false;
				}
				path.Add(fromv);
				while (graphNode.parent != null)
				{
					path.Add(graphNode.parent.id);
					graphNode = graphNode.parent;
				}
				return true;
			}
			else
			{
				DijkstraGraphDistance.GraphNodeStruct graphNodeStruct = this.DenseNodes[fromv];
				if (!graphNodeStruct.frozen)
				{
					return false;
				}
				path.Add(fromv);
				while (graphNodeStruct.parent != -1)
				{
					path.Add(graphNodeStruct.parent);
					graphNodeStruct = this.DenseNodes[graphNodeStruct.parent];
				}
				return true;
			}
		}

		private DijkstraGraphDistance.GraphNode get_node(int id)
		{
			DijkstraGraphDistance.GraphNode graphNode = this.SparseNodes[id];
			if (graphNode == null)
			{
				graphNode = this.SparseNodePool.Allocate();
				graphNode.id = id;
				graphNode.parent = null;
				graphNode.frozen = false;
				this.SparseNodes[id] = graphNode;
			}
			return graphNode;
		}

		private void update_neighbours_sparse(DijkstraGraphDistance.GraphNode parent)
		{
			float priority = parent.priority;
			foreach (int num in this.NeighboursF(parent.id))
			{
				if (this.NodeFilterF(num))
				{
					DijkstraGraphDistance.GraphNode graphNode = this.get_node(num);
					if (!graphNode.frozen)
					{
						float num2 = this.NodeDistanceF(parent.id, num) + priority;
						if (num2 != 3.4028235E+38f)
						{
							if (this.SparseQueue.Contains(graphNode))
							{
								if (num2 < graphNode.priority)
								{
									graphNode.parent = parent;
									this.SparseQueue.Update(graphNode, num2);
								}
							}
							else
							{
								graphNode.parent = parent;
								this.SparseQueue.Enqueue(graphNode, num2);
							}
						}
					}
				}
			}
		}

		private void enqueue_node_dense(int id, float dist, int parent_id)
		{
			DijkstraGraphDistance.GraphNodeStruct graphNodeStruct = new DijkstraGraphDistance.GraphNodeStruct(id, parent_id, dist);
			this.DenseNodes[id] = graphNodeStruct;
			this.DenseQueue.Insert(id, dist);
		}

		private void update_neighbours_dense(int parent_id)
		{
			float distance = this.DenseNodes[parent_id].distance;
			foreach (int num in this.NeighboursF(parent_id))
			{
				if (this.NodeFilterF(num))
				{
					DijkstraGraphDistance.GraphNodeStruct graphNodeStruct = this.DenseNodes[num];
					if (!graphNodeStruct.frozen)
					{
						float num2 = this.NodeDistanceF(parent_id, num) + distance;
						if (num2 != 3.4028235E+38f)
						{
							if (this.DenseQueue.Contains(num))
							{
								if (num2 < graphNodeStruct.distance)
								{
									graphNodeStruct.parent = parent_id;
									this.DenseQueue.Update(num, num2);
									this.DenseNodes[num] = graphNodeStruct;
								}
							}
							else
							{
								this.enqueue_node_dense(num, num2, parent_id);
							}
						}
					}
				}
			}
		}

		public const float InvalidValue = 3.4028235E+38f;

		public bool TrackOrder;

		private DynamicPriorityQueue<DijkstraGraphDistance.GraphNode> SparseQueue;

		private SparseObjectList<DijkstraGraphDistance.GraphNode> SparseNodes;

		private MemoryPool<DijkstraGraphDistance.GraphNode> SparseNodePool;

		private IndexPriorityQueue DenseQueue;

		private DijkstraGraphDistance.GraphNodeStruct[] DenseNodes;

		private Func<int, bool> NodeFilterF;

		private Func<int, int, float> NodeDistanceF;

		private Func<int, IEnumerable<int>> NeighboursF;

		private List<int> Seeds;

		private float max_value;

		private List<int> order;

		private class GraphNode : DynamicPriorityQueueNode, IEquatable<DijkstraGraphDistance.GraphNode>
		{
			public bool Equals(DijkstraGraphDistance.GraphNode other)
			{
				return this.id == other.id;
			}

			public int id;

			public DijkstraGraphDistance.GraphNode parent;

			public bool frozen;
		}

		private struct GraphNodeStruct : IEquatable<DijkstraGraphDistance.GraphNodeStruct>
		{
			public GraphNodeStruct(int id, int parent, float distance)
			{
				this.id = id;
				this.parent = parent;
				this.distance = distance;
				this.frozen = false;
			}

			public bool Equals(DijkstraGraphDistance.GraphNodeStruct other)
			{
				return this.id == other.id;
			}

			public int id;

			public int parent;

			public bool frozen;

			public float distance;

			public static readonly DijkstraGraphDistance.GraphNodeStruct Zero = new DijkstraGraphDistance.GraphNodeStruct
			{
				id = -1,
				parent = -1,
				distance = float.MaxValue,
				frozen = false
			};
		}
	}
}
