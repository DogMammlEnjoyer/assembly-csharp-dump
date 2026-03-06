using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshLocalParam
	{
		public MeshLocalParam(int nMaxID, Func<int, Vector3f> nodePositionF, Func<int, Vector3f> nodeNormalF, Func<int, IEnumerable<int>> neighboursF)
		{
			this.PositionF = nodePositionF;
			this.NormalF = nodeNormalF;
			this.NeighboursF = neighboursF;
			this.SparseQueue = new DynamicPriorityQueue<MeshLocalParam.GraphNode>();
			this.SparseNodes = new SparseObjectList<MeshLocalParam.GraphNode>(nMaxID, 0);
			this.SparseNodePool = new MemoryPool<MeshLocalParam.GraphNode>();
			this.max_graph_distance = float.MinValue;
			this.max_uv_distance = float.MinValue;
		}

		public void Reset()
		{
			this.SparseQueue.Clear(false);
			this.SparseNodes.Clear();
			this.SparseNodePool.ReturnAll();
			this.max_graph_distance = float.MinValue;
		}

		public void ComputeToMaxDistance(Frame3f seedFrame, Index3i seedNbrs, float fMaxGraphDistance)
		{
			this.SeedFrame = seedFrame;
			for (int i = 0; i < 3; i++)
			{
				int num = seedNbrs[i];
				MeshLocalParam.GraphNode graphNode = this.get_node(num, true);
				graphNode.uv = this.compute_local_uv(ref this.SeedFrame, this.PositionF(num));
				graphNode.graph_distance = graphNode.uv.Length;
				graphNode.frozen = true;
				this.SparseQueue.Enqueue(graphNode, graphNode.graph_distance);
			}
			while (this.SparseQueue.Count > 0)
			{
				MeshLocalParam.GraphNode graphNode2 = this.SparseQueue.Dequeue();
				this.max_graph_distance = Math.Max(graphNode2.graph_distance, this.max_graph_distance);
				if (this.max_graph_distance > fMaxGraphDistance)
				{
					return;
				}
				if (graphNode2.parent != null)
				{
					switch (this.UVMode)
					{
					case MeshLocalParam.UVModes.ExponentialMap:
						this.update_uv_expmap(graphNode2);
						break;
					case MeshLocalParam.UVModes.ExponentialMap_UpwindAvg:
						this.update_uv_upwind_expmap(graphNode2);
						break;
					case MeshLocalParam.UVModes.PlanarProjection:
						this.update_uv_planar(graphNode2);
						break;
					}
				}
				float lengthSquared = graphNode2.uv.LengthSquared;
				if (lengthSquared > this.max_uv_distance)
				{
					this.max_uv_distance = lengthSquared;
				}
				graphNode2.frozen = true;
				this.update_neighbours_sparse(graphNode2);
			}
			this.max_uv_distance = (float)Math.Sqrt((double)this.max_uv_distance);
		}

		public void TransformUV(float fScale, Vector2f vTranslate)
		{
			foreach (KeyValuePair<int, MeshLocalParam.GraphNode> keyValuePair in this.SparseNodes.NonZeroValues())
			{
				MeshLocalParam.GraphNode value = keyValuePair.Value;
				if (value.frozen)
				{
					value.uv = value.uv * fScale + vTranslate;
				}
			}
		}

		public float MaxGraphDistance
		{
			get
			{
				return this.max_graph_distance;
			}
		}

		public float MaxUVDistance
		{
			get
			{
				return this.max_uv_distance;
			}
		}

		public Vector2f GetUV(int id)
		{
			MeshLocalParam.GraphNode graphNode = this.SparseNodes[id];
			if (graphNode == null)
			{
				return MeshLocalParam.InvalidUV;
			}
			return graphNode.uv;
		}

		public void ApplyUVs(Action<int, Vector2f> applyF)
		{
			foreach (KeyValuePair<int, MeshLocalParam.GraphNode> keyValuePair in this.SparseNodes.NonZeroValues())
			{
				MeshLocalParam.GraphNode value = keyValuePair.Value;
				if (value.frozen)
				{
					applyF(value.id, value.uv);
				}
			}
		}

		private Vector2f compute_local_uv(ref Frame3f f, Vector3f pos)
		{
			pos -= f.Origin;
			return new Vector2f(pos.Dot(f.X), pos.Dot(f.Y));
		}

		private Vector2f propagate_uv(Vector3f pos, Vector2f nbrUV, ref Frame3f fNbr, ref Frame3f fSeed)
		{
			Vector2f v = this.compute_local_uv(ref fNbr, pos);
			Frame3f frame3f = fSeed;
			frame3f.AlignAxis(2, fNbr.Z);
			Vector3f x = frame3f.X;
			Vector3f x2 = fNbr.X;
			float num = x2.Dot(x);
			float num2 = 1f - num * num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			float num3 = (float)Math.Sqrt((double)num2);
			if (x2.Cross(x).Dot(fNbr.Z) < 0f)
			{
				num3 = -num3;
			}
			Matrix2f m = new Matrix2f(num, num3, -num3, num);
			return nbrUV + m * v;
		}

		private void update_uv_expmap(MeshLocalParam.GraphNode node)
		{
			int id = node.id;
			int id2 = node.parent.id;
			Vector3f origin = this.PositionF(id2);
			Frame3f frame3f = new Frame3f(origin, this.NormalF(id2));
			node.uv = this.propagate_uv(this.PositionF(id), node.parent.uv, ref frame3f, ref this.SeedFrame);
		}

		private void update_uv_upwind_expmap(MeshLocalParam.GraphNode node)
		{
			int id = node.id;
			Vector3f pos = this.PositionF(id);
			Vector2f vector2f = Vector2f.Zero;
			float num = 0f;
			int num2 = 0;
			foreach (int num3 in this.NeighboursF(node.id))
			{
				MeshLocalParam.GraphNode graphNode = this.get_node(num3, false);
				if (graphNode.frozen)
				{
					Vector3f vector3f = this.PositionF(num3);
					Frame3f frame3f = new Frame3f(vector3f, this.NormalF(num3));
					Vector2f a = this.propagate_uv(pos, graphNode.uv, ref frame3f, ref this.SeedFrame);
					float num4 = 1f / (pos.DistanceSquared(vector3f) + 1E-06f);
					vector2f += num4 * a;
					num += num4;
					num2++;
				}
			}
			vector2f /= num;
			node.uv = vector2f;
		}

		private void update_uv_planar(MeshLocalParam.GraphNode g)
		{
			g.uv = this.compute_local_uv(ref this.SeedFrame, this.PositionF(g.id));
		}

		private MeshLocalParam.GraphNode get_node(int id, bool bCreateIfMissing = true)
		{
			MeshLocalParam.GraphNode graphNode = this.SparseNodes[id];
			if (graphNode == null)
			{
				graphNode = this.SparseNodePool.Allocate();
				graphNode.id = id;
				graphNode.parent = null;
				graphNode.frozen = false;
				graphNode.uv = Vector2f.Zero;
				graphNode.graph_distance = float.MaxValue;
				this.SparseNodes[id] = graphNode;
			}
			return graphNode;
		}

		private void update_neighbours_sparse(MeshLocalParam.GraphNode parent)
		{
			Vector3f vector3f = this.PositionF(parent.id);
			float graph_distance = parent.graph_distance;
			foreach (int num in this.NeighboursF(parent.id))
			{
				MeshLocalParam.GraphNode graphNode = this.get_node(num, true);
				if (!graphNode.frozen)
				{
					float num2 = graph_distance + vector3f.Distance(this.PositionF(num));
					if (this.SparseQueue.Contains(graphNode))
					{
						if (num2 < graphNode.priority)
						{
							graphNode.parent = parent;
							graphNode.graph_distance = num2;
							this.SparseQueue.Update(graphNode, graphNode.graph_distance);
						}
					}
					else
					{
						graphNode.parent = parent;
						graphNode.graph_distance = num2;
						this.SparseQueue.Enqueue(graphNode, graphNode.graph_distance);
					}
				}
			}
		}

		public static readonly Vector2f InvalidUV = new Vector2f(float.MaxValue, float.MaxValue);

		public MeshLocalParam.UVModes UVMode = MeshLocalParam.UVModes.ExponentialMap_UpwindAvg;

		private DynamicPriorityQueue<MeshLocalParam.GraphNode> SparseQueue;

		private SparseObjectList<MeshLocalParam.GraphNode> SparseNodes;

		private MemoryPool<MeshLocalParam.GraphNode> SparseNodePool;

		private Func<int, Vector3f> PositionF;

		private Func<int, Vector3f> NormalF;

		private Func<int, IEnumerable<int>> NeighboursF;

		private Frame3f SeedFrame;

		private float max_graph_distance;

		private float max_uv_distance;

		public enum UVModes
		{
			ExponentialMap,
			ExponentialMap_UpwindAvg,
			PlanarProjection
		}

		private class GraphNode : DynamicPriorityQueueNode, IEquatable<MeshLocalParam.GraphNode>
		{
			public bool Equals(MeshLocalParam.GraphNode other)
			{
				return this.id == other.id;
			}

			public int id;

			public MeshLocalParam.GraphNode parent;

			public float graph_distance;

			public Vector2f uv;

			public bool frozen;
		}
	}
}
