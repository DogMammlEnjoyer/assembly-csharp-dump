using System;
using System.Collections.Generic;

namespace g3
{
	public class GraphSplitter2d
	{
		public GraphSplitter2d(DGraph2 graph)
		{
			this.Graph = graph;
		}

		public void InsertLine(Line2d line, int insert_edges_id = -1)
		{
			if (insert_edges_id == -1)
			{
				insert_edges_id = this.InsertedEdgesID;
			}
			this.do_split(line, true, insert_edges_id);
		}

		protected virtual void do_split(Line2d line, bool insert_edges, int insert_gid)
		{
			if (this.EdgeSigns.Length < this.Graph.MaxVertexID)
			{
				this.EdgeSigns.resize(this.Graph.MaxVertexID);
			}
			foreach (int num in this.Graph.VertexIndices())
			{
				this.EdgeSigns[num] = line.WhichSide(this.Graph.GetVertex(num), this.OnVertexTol);
			}
			this.hits.Clear();
			foreach (int num2 in this.Graph.EdgeIndices())
			{
				Index2i edgeV = this.Graph.GetEdgeV(num2);
				Index2i index2i = new Index2i(this.EdgeSigns[edgeV.a], this.EdgeSigns[edgeV.b]);
				if (index2i.a * index2i.b <= 0)
				{
					GraphSplitter2d.edge_hit item = new GraphSplitter2d.edge_hit
					{
						hit_eid = num2,
						vtx_signs = index2i,
						hit_vid = -1
					};
					Vector2d vertex = this.Graph.GetVertex(edgeV.a);
					Vector2d vertex2 = this.Graph.GetVertex(edgeV.b);
					if (index2i.a == index2i.b)
					{
						if (vertex.DistanceSquared(vertex2) > 2.220446049250313E-16)
						{
							item.hit_vid = edgeV.a;
							item.line_t = line.Project(vertex);
							this.hits.Add(item);
							item.hit_vid = edgeV.b;
							item.line_t = line.Project(vertex2);
							this.hits.Add(item);
						}
						else
						{
							index2i.b = 1;
						}
					}
					if (index2i.a == 0)
					{
						item.hit_pos = vertex;
						item.hit_vid = edgeV.a;
						item.line_t = line.Project(vertex);
					}
					else if (index2i.b == 0)
					{
						item.hit_pos = vertex2;
						item.hit_vid = edgeV.b;
						item.line_t = line.Project(vertex2);
					}
					else
					{
						IntrLine2Segment2 intrLine2Segment = new IntrLine2Segment2(line, new Segment2d(vertex, vertex2));
						if (!intrLine2Segment.Find())
						{
							throw new Exception("GraphSplitter2d.Split: signs are different but ray did not it?");
						}
						if (!intrLine2Segment.IsSimpleIntersection)
						{
							throw new Exception("GraphSplitter2d.Split: got parallel edge case!");
						}
						item.hit_pos = intrLine2Segment.Point;
						item.line_t = intrLine2Segment.Parameter;
					}
					this.hits.Add(item);
				}
			}
			this.hits.Sort((GraphSplitter2d.edge_hit hit0, GraphSplitter2d.edge_hit hit1) => hit0.line_t.CompareTo(hit1.line_t));
			int count = this.hits.Count;
			for (int i = 0; i < count - 1; i++)
			{
				int index = i + 1;
				if (this.hits[i].line_t != this.hits[index].line_t && this.hits[i].hit_eid != this.hits[index].hit_eid)
				{
					int num3 = this.hits[i].hit_vid;
					int num4 = this.hits[index].hit_vid;
					if ((num3 != num4 || num3 < 0) && (num3 < 0 || num4 < 0 || this.Graph.FindEdge(num3, num4) < 0))
					{
						if (num3 == -1)
						{
							DGraph.EdgeSplitInfo edgeSplitInfo;
							if (this.Graph.SplitEdge(this.hits[i].hit_eid, out edgeSplitInfo) != MeshResult.Ok)
							{
								throw new Exception("GraphSplitter2d.Split: first edge split failed!");
							}
							num3 = edgeSplitInfo.vNew;
							this.Graph.SetVertex(num3, this.hits[i].hit_pos);
							GraphSplitter2d.edge_hit value = this.hits[i];
							value.hit_vid = num3;
							this.hits[i] = value;
						}
						if (num4 == -1)
						{
							DGraph.EdgeSplitInfo edgeSplitInfo2;
							if (this.Graph.SplitEdge(this.hits[index].hit_eid, out edgeSplitInfo2) != MeshResult.Ok)
							{
								throw new Exception("GraphSplitter2d.Split: second edge split failed!");
							}
							num4 = edgeSplitInfo2.vNew;
							this.Graph.SetVertex(num4, this.hits[index].hit_pos);
							GraphSplitter2d.edge_hit value2 = this.hits[index];
							value2.hit_vid = num4;
							this.hits[index] = value2;
						}
						if (this.InsideTestF != null)
						{
							Vector2d arg = 0.5 * (this.Graph.GetVertex(num3) + this.Graph.GetVertex(num4));
							if (!this.InsideTestF(arg))
							{
								goto IL_4EF;
							}
						}
						if (insert_edges)
						{
							this.Graph.AppendEdge(num3, num4, insert_gid);
						}
					}
				}
				IL_4EF:;
			}
		}

		public DGraph2 Graph;

		public double OnVertexTol = 1.1920928955078125E-07;

		public int InsertedEdgesID = 1;

		public Func<Vector2d, bool> InsideTestF;

		private DVector<int> EdgeSigns = new DVector<int>();

		private List<GraphSplitter2d.edge_hit> hits = new List<GraphSplitter2d.edge_hit>();

		private struct edge_hit
		{
			public int hit_eid;

			public Index2i vtx_signs;

			public int hit_vid;

			public Vector2d hit_pos;

			public double line_t;
		}
	}
}
