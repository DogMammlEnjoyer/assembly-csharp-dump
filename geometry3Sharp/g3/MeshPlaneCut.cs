using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshPlaneCut
	{
		public MeshPlaneCut(DMesh3 mesh, Vector3d origin, Vector3d normal)
		{
			this.Mesh = mesh;
			this.PlaneOrigin = origin;
			this.PlaneNormal = normal;
		}

		public virtual ValidationStatus Validate()
		{
			return ValidationStatus.Ok;
		}

		public virtual bool Cut()
		{
			double invalidDist = double.MinValue;
			MeshEdgeSelection meshEdgeSelection = null;
			MeshVertexSelection meshVertexSelection = null;
			if (this.CutFaceSet != null)
			{
				meshEdgeSelection = new MeshEdgeSelection(this.Mesh, this.CutFaceSet, 1);
				meshVertexSelection = new MeshVertexSelection(this.Mesh, meshEdgeSelection);
			}
			int maxVertexID = this.Mesh.MaxVertexID;
			double[] signs = new double[maxVertexID];
			gParallel.ForEach<int>(Interval1i.Range(maxVertexID), delegate(int vid)
			{
				if (this.Mesh.IsVertex(vid))
				{
					Vector3d vertex = this.Mesh.GetVertex(vid);
					signs[vid] = (vertex - this.PlaneOrigin).Dot(this.PlaneNormal);
					return;
				}
				signs[vid] = invalidDist;
			});
			HashSet<int> ZeroEdges = new HashSet<int>();
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> OnCutEdges = new HashSet<int>();
			int maxEdgeID = this.Mesh.MaxEdgeID;
			HashSet<int> hashSet2 = new HashSet<int>();
			IEnumerable<int> enumerable = Interval1i.Range(maxEdgeID);
			if (meshEdgeSelection != null)
			{
				enumerable = meshEdgeSelection;
			}
			foreach (int num in enumerable)
			{
				if (this.Mesh.IsEdge(num) && num < maxEdgeID && !hashSet2.Contains(num))
				{
					Index2i edgeV = this.Mesh.GetEdgeV(num);
					double num2 = signs[edgeV.a];
					double num3 = signs[edgeV.b];
					int num4 = (Math.Abs(num2) < 2.220446049250313E-16) ? 1 : 0;
					int num5 = (Math.Abs(num3) < 2.220446049250313E-16) ? 1 : 0;
					if (num4 + num5 > 0)
					{
						if (num4 + num5 == 2)
						{
							ZeroEdges.Add(num);
						}
						else
						{
							hashSet.Add((num4 == 1) ? edgeV[0] : edgeV[1]);
						}
					}
					else if (num2 * num3 <= 0.0)
					{
						DMesh3.EdgeSplitInfo edgeSplitInfo;
						if (this.Mesh.SplitEdge(num, out edgeSplitInfo, 0.5) != MeshResult.Ok)
						{
							throw new Exception("MeshPlaneCut.Cut: failed in SplitEdge");
						}
						double num6 = num2 / (num2 - num3);
						Vector3d vNewPos = (1.0 - num6) * this.Mesh.GetVertex(edgeV.a) + num6 * this.Mesh.GetVertex(edgeV.b);
						this.Mesh.SetVertex(edgeSplitInfo.vNew, vNewPos);
						hashSet2.Add(edgeSplitInfo.eNewBN);
						hashSet2.Add(edgeSplitInfo.eNewCN);
						OnCutEdges.Add(edgeSplitInfo.eNewCN);
						if (edgeSplitInfo.eNewDN != -1)
						{
							hashSet2.Add(edgeSplitInfo.eNewDN);
							OnCutEdges.Add(edgeSplitInfo.eNewDN);
						}
					}
				}
			}
			IEnumerable<int> enumerable2 = Interval1i.Range(maxVertexID);
			if (meshVertexSelection != null)
			{
				enumerable2 = meshVertexSelection;
			}
			foreach (int num7 in enumerable2)
			{
				if (signs[num7] > 0.0 && this.Mesh.IsVertex(num7))
				{
					this.Mesh.RemoveVertex(num7, true, false);
				}
			}
			if (this.CollapseDegenerateEdgesOnCut)
			{
				this.collapse_degenerate_edges(OnCutEdges, ZeroEdges);
			}
			Func<int, bool> edgeFilterF = (int eid) => OnCutEdges.Contains(eid) || ZeroEdges.Contains(eid);
			try
			{
				MeshBoundaryLoops meshBoundaryLoops = new MeshBoundaryLoops(this.Mesh, false);
				meshBoundaryLoops.EdgeFilterF = edgeFilterF;
				meshBoundaryLoops.Compute();
				this.CutLoops = meshBoundaryLoops.Loops;
				this.CutSpans = meshBoundaryLoops.Spans;
				this.CutLoopsFailed = false;
				this.FoundOpenSpans = (this.CutSpans.Count > 0);
			}
			catch
			{
				this.CutLoops = new List<EdgeLoop>();
				this.CutLoopsFailed = true;
			}
			return true;
		}

		protected void collapse_degenerate_edges(HashSet<int> OnCutEdges, HashSet<int> ZeroEdges)
		{
			HashSet<int>[] array = new HashSet<int>[]
			{
				OnCutEdges,
				ZeroEdges
			};
			double num = this.DegenerateEdgeTol * this.DegenerateEdgeTol;
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			int num2 = 0;
			do
			{
				num2 = 0;
				HashSet<int>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					foreach (int eID in array2[i])
					{
						if (this.Mesh.IsEdge(eID))
						{
							this.Mesh.GetEdgeV(eID, ref zero, ref zero2);
							if (zero.DistanceSquared(zero2) <= num)
							{
								Index2i edgeV = this.Mesh.GetEdgeV(eID);
								DMesh3.EdgeCollapseInfo edgeCollapseInfo;
								if (this.Mesh.CollapseEdge(edgeV.a, edgeV.b, out edgeCollapseInfo) == MeshResult.Ok)
								{
									num2++;
								}
							}
						}
					}
				}
			}
			while (num2 != 0);
		}

		public bool FillHoles(int constantGroupID = -1)
		{
			bool result = true;
			this.LoopFillTriangles = new List<int[]>(this.CutLoops.Count);
			foreach (EdgeLoop loop in this.CutLoops)
			{
				SimpleHoleFiller simpleHoleFiller = new SimpleHoleFiller(this.Mesh, loop);
				int group_id = (constantGroupID >= 0) ? constantGroupID : this.Mesh.AllocateTriangleGroup();
				if (simpleHoleFiller.Fill(group_id))
				{
					result = false;
					this.LoopFillTriangles.Add(simpleHoleFiller.NewTriangles);
				}
				else
				{
					this.LoopFillTriangles.Add(null);
				}
			}
			return result;
		}

		public DMesh3 Mesh;

		public Vector3d PlaneOrigin;

		public Vector3d PlaneNormal;

		public bool CollapseDegenerateEdgesOnCut = true;

		public double DegenerateEdgeTol = 9.999999974752427E-07;

		public MeshFaceSelection CutFaceSet;

		public List<EdgeLoop> CutLoops;

		public List<EdgeSpan> CutSpans;

		public bool CutLoopsFailed;

		public bool FoundOpenSpans;

		public List<int[]> LoopFillTriangles;
	}
}
