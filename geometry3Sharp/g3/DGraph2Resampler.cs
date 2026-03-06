using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace g3
{
	public class DGraph2Resampler
	{
		public DGraph2Resampler(DGraph2 graph)
		{
			this.Graph = graph;
		}

		public void SplitToMaxEdgeLength(double fMaxLen)
		{
			List<int> list = new List<int>();
			int maxEdgeID = this.Graph.MaxEdgeID;
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (this.Graph.IsEdge(i) && !this.FixedEdgeFilterF(i))
				{
					Index2i edgeV = this.Graph.GetEdgeV(i);
					double num = this.Graph.GetVertex(edgeV.a).Distance(this.Graph.GetVertex(edgeV.b));
					DGraph.EdgeSplitInfo edgeSplitInfo;
					if (num > fMaxLen && this.Graph.SplitEdge(i, out edgeSplitInfo) == MeshResult.Ok && num > 2.0 * fMaxLen)
					{
						list.Add(i);
						list.Add(edgeSplitInfo.eNewBN);
					}
				}
			}
			while (list.Count > 0)
			{
				int num2 = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				if (this.Graph.IsEdge(num2))
				{
					Index2i edgeV2 = this.Graph.GetEdgeV(num2);
					double num3 = this.Graph.GetVertex(edgeV2.a).Distance(this.Graph.GetVertex(edgeV2.b));
					DGraph.EdgeSplitInfo edgeSplitInfo2;
					if (num3 > fMaxLen && this.Graph.SplitEdge(num2, out edgeSplitInfo2) == MeshResult.Ok && num3 > 2.0 * fMaxLen)
					{
						list.Add(num2);
						list.Add(edgeSplitInfo2.eNewBN);
					}
				}
			}
		}

		public void CollapseFlatVertices(double fMaxDeviationDeg = 5.0)
		{
			bool flag = false;
			int num = 200;
			int num2 = 0;
			IL_10F:
			while (!flag && num2++ < num)
			{
				flag = true;
				int maxVertexID = this.Graph.MaxVertexID;
				int num3 = 0;
				for (;;)
				{
					int num4 = num3;
					num3 = (num3 + 31337) % maxVertexID;
					if (this.Graph.IsVertex(num4) && this.Graph.GetVtxEdgeCount(num4) == 2 && Math.Abs(this.Graph.OpeningAngle(num4, 1.7976931348623157E+308)) >= 180.0 - fMaxDeviationDeg)
					{
						ReadOnlyCollection<int> vtxEdges = this.Graph.GetVtxEdges(num4);
						int num5 = vtxEdges.First<int>();
						int arg = vtxEdges.Last<int>();
						if (!this.FixedEdgeFilterF(num5) && !this.FixedEdgeFilterF(arg))
						{
							Index2i edgeV = this.Graph.GetEdgeV(num5);
							int vKeep = (edgeV.a == num4) ? edgeV.b : edgeV.a;
							DGraph.EdgeCollapseInfo edgeCollapseInfo;
							if (this.Graph.CollapseEdge(vKeep, num4, out edgeCollapseInfo) != MeshResult.Ok)
							{
								break;
							}
							flag = false;
						}
					}
					if (num3 == 0)
					{
						goto IL_10F;
					}
				}
				throw new Exception("DGraph2Resampler.CollapseFlatVertices: failed!");
			}
		}

		public void CollapseDegenerateEdges(double fDegenLenThresh = 1.1920928955078125E-07)
		{
			bool flag = false;
			int num = 100;
			int num2 = 0;
			while (!flag && num2++ < num)
			{
				flag = true;
				int maxEdgeID = this.Graph.MaxEdgeID;
				for (int i = 0; i < maxEdgeID; i++)
				{
					if (this.Graph.IsEdge(i) && !this.FixedEdgeFilterF(i))
					{
						Index2i edgeV = this.Graph.GetEdgeV(i);
						Vector2d vertex = this.Graph.GetVertex(edgeV.a);
						Vector2d vertex2 = this.Graph.GetVertex(edgeV.b);
						if (vertex.Distance(vertex2) < fDegenLenThresh)
						{
							int a = edgeV.a;
							int b = edgeV.b;
							DGraph.EdgeCollapseInfo edgeCollapseInfo;
							if (this.Graph.CollapseEdge(a, b, out edgeCollapseInfo) == MeshResult.Ok)
							{
								flag = false;
							}
						}
					}
				}
			}
		}

		public void CollapseToMinEdgeLength(double fMinLen)
		{
			double num = 140.0;
			double num2 = fMinLen * fMinLen;
			bool flag = false;
			int num3 = 100;
			int num4 = 0;
			while (!flag && num4++ < num3)
			{
				flag = true;
				int maxEdgeID = this.Graph.MaxEdgeID;
				int num5 = 0;
				do
				{
					int num6 = num5;
					num5 = (num5 + 31337) % maxEdgeID;
					if (this.Graph.IsEdge(num6) && !this.FixedEdgeFilterF(num6))
					{
						Index2i edgeV = this.Graph.GetEdgeV(num6);
						Vector2d vertex = this.Graph.GetVertex(edgeV.a);
						Vector2d vertex2 = this.Graph.GetVertex(edgeV.b);
						if (vertex.DistanceSquared(vertex2) < num2)
						{
							int num7 = -1;
							int vtxEdgeCount = this.Graph.GetVtxEdgeCount(edgeV.a);
							int vtxEdgeCount2 = this.Graph.GetVtxEdgeCount(edgeV.b);
							if (vtxEdgeCount == 2 || vtxEdgeCount2 == 2)
							{
								if (vtxEdgeCount != 2)
								{
									num7 = 0;
								}
								else if (vtxEdgeCount2 != 2)
								{
									num7 = 1;
								}
								if (num7 == -1)
								{
									double num8 = Math.Abs(this.Graph.OpeningAngle(edgeV.a, double.MaxValue));
									double num9 = Math.Abs(this.Graph.OpeningAngle(edgeV.b, double.MaxValue));
									if (num8 < num && num9 < num)
									{
										goto IL_1D6;
									}
									if (num8 < num)
									{
										num7 = 0;
									}
									else if (num9 < num)
									{
										num7 = 1;
									}
								}
								Vector2d vNewPos = (num7 == -1) ? (0.5 * (vertex + vertex2)) : ((num7 == 0) ? vertex : vertex2);
								int vKeep = edgeV.a;
								int vRemove = edgeV.b;
								if (num7 == 1)
								{
									vRemove = edgeV.a;
									vKeep = edgeV.b;
								}
								DGraph.EdgeCollapseInfo edgeCollapseInfo;
								if (this.Graph.CollapseEdge(vKeep, vRemove, out edgeCollapseInfo) == MeshResult.Ok)
								{
									this.Graph.SetVertex(edgeCollapseInfo.vKept, vNewPos);
									flag = false;
								}
							}
						}
					}
					IL_1D6:;
				}
				while (num5 != 0);
			}
		}

		public DGraph2 Graph;

		public Func<int, bool> FixedEdgeFilterF = (int eid) => false;
	}
}
