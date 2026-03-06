using System;
using System.Linq;
using g3;

namespace gs
{
	public class MeshAutoRepair
	{
		protected virtual bool Cancelled()
		{
			return this.Progress != null && this.Progress.Cancelled();
		}

		public MeshAutoRepair(DMesh3 mesh3)
		{
			this.Mesh = mesh3;
		}

		public bool Apply()
		{
			bool flag = false;
			if (flag)
			{
				this.Mesh.CheckValidity(false, FailMode.Throw);
			}
			this.do_remove_inside();
			if (this.Cancelled())
			{
				return false;
			}
			int num = 0;
			for (;;)
			{
				this.repair_orientation(false);
				if (this.Cancelled())
				{
					break;
				}
				this.repair_cracks(true, this.RepairTolerance);
				if (this.Mesh.IsClosed())
				{
					goto IL_20D;
				}
				if (this.Cancelled())
				{
					return false;
				}
				this.collapse_all_degenerate_edges(this.RepairTolerance * 0.5, true);
				if (this.Cancelled())
				{
					return false;
				}
				this.repair_cracks(true, 2.0 * this.RepairTolerance);
				if (this.Cancelled())
				{
					return false;
				}
				this.repair_cracks(false, 2.0 * this.RepairTolerance);
				if (this.Cancelled())
				{
					return false;
				}
				if (this.Mesh.IsClosed())
				{
					goto IL_20D;
				}
				this.repair_orientation(false);
				if (this.Cancelled())
				{
					return false;
				}
				if (flag)
				{
					this.Mesh.CheckValidity(false, FailMode.Throw);
				}
				this.remove_loners();
				int num2 = 0;
				int num3;
				bool flag2;
				this.fill_trivial_holes(out num3, out flag2);
				if (this.Cancelled())
				{
					return false;
				}
				if (this.Mesh.IsClosed())
				{
					goto IL_20D;
				}
				this.fill_any_holes(out num3, out flag2);
				if (this.Cancelled())
				{
					return false;
				}
				if (flag2)
				{
					this.disconnect_bowties(out num2);
					this.fill_any_holes(out num3, out flag2);
				}
				if (this.Cancelled())
				{
					return false;
				}
				if (this.Mesh.IsClosed())
				{
					goto IL_20D;
				}
				this.disconnect_bowties(out num2);
				if (this.Cancelled())
				{
					return false;
				}
				if (num == 0 && !this.Mesh.IsClosed())
				{
					num++;
				}
				else
				{
					if (num > this.ErosionIterations || this.Mesh.IsClosed())
					{
						goto IL_20D;
					}
					num++;
					MeshFaceSelection meshFaceSelection = new MeshFaceSelection(this.Mesh);
					foreach (int eid in MeshIterators.BoundaryEdges(this.Mesh))
					{
						meshFaceSelection.SelectEdgeTris(eid);
					}
					MeshEditor.RemoveTriangles(this.Mesh, meshFaceSelection, true);
				}
			}
			return false;
			IL_20D:
			if (this.MinEdgeLengthTol > 0.0)
			{
				this.collapse_all_degenerate_edges(this.MinEdgeLengthTol, false);
			}
			if (this.Cancelled())
			{
				return false;
			}
			this.repair_orientation(true);
			if (this.Cancelled())
			{
				return false;
			}
			if (flag)
			{
				this.Mesh.CheckValidity(false, FailMode.Throw);
			}
			this.Mesh = new DMesh3(this.Mesh, true, true, true, true);
			MeshNormals.QuickCompute(this.Mesh);
			return true;
		}

		private void fill_trivial_holes(out int nRemaining, out bool saw_spans)
		{
			MeshBoundaryLoops meshBoundaryLoops = new MeshBoundaryLoops(this.Mesh, true);
			nRemaining = 0;
			saw_spans = meshBoundaryLoops.SawOpenSpans;
			foreach (EdgeLoop edgeLoop in meshBoundaryLoops)
			{
				if (this.Cancelled())
				{
					break;
				}
				bool flag = false;
				if (edgeLoop.VertexCount == 3)
				{
					flag = new SimpleHoleFiller(this.Mesh, edgeLoop).Fill(-1);
				}
				else if (edgeLoop.VertexCount == 4)
				{
					flag = new MinimalHoleFill(this.Mesh, edgeLoop).Apply();
					if (!flag)
					{
						flag = new SimpleHoleFiller(this.Mesh, edgeLoop).Fill(-1);
					}
				}
				if (!flag)
				{
					nRemaining++;
				}
			}
		}

		private void fill_any_holes(out int nRemaining, out bool saw_spans)
		{
			MeshBoundaryLoops meshBoundaryLoops = new MeshBoundaryLoops(this.Mesh, true);
			nRemaining = 0;
			saw_spans = meshBoundaryLoops.SawOpenSpans;
			foreach (EdgeLoop edgeLoop in meshBoundaryLoops)
			{
				if (this.Cancelled())
				{
					break;
				}
				if (!new MinimalHoleFill(this.Mesh, edgeLoop).Apply())
				{
					if (this.Cancelled())
					{
						break;
					}
					new SimpleHoleFiller(this.Mesh, edgeLoop).Fill(-1);
				}
			}
		}

		private bool repair_cracks(bool bUniqueOnly, double mergeDist)
		{
			bool result;
			try
			{
				result = new MergeCoincidentEdges(this.Mesh)
				{
					OnlyUniquePairs = bUniqueOnly,
					MergeDistance = mergeDist
				}.Apply();
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private bool remove_duplicate_faces(double vtxTolerance, out int nRemoved)
		{
			nRemoved = 0;
			bool result;
			try
			{
				RemoveDuplicateTriangles removeDuplicateTriangles = new RemoveDuplicateTriangles(this.Mesh);
				removeDuplicateTriangles.VertexTolerance = vtxTolerance;
				bool flag = removeDuplicateTriangles.Apply();
				nRemoved = removeDuplicateTriangles.Removed;
				result = flag;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private bool collapse_degenerate_edges(double minLength, bool bBoundaryOnly, out int collapseCount)
		{
			collapseCount = 0;
			foreach (int num in MathUtil.ModuloIteration(this.Mesh.MaxEdgeID, 31337))
			{
				if (this.Cancelled())
				{
					break;
				}
				if (this.Mesh.IsEdge(num))
				{
					bool flag = this.Mesh.IsBoundaryEdge(num);
					if (!bBoundaryOnly || flag)
					{
						Index2i edgeV = this.Mesh.GetEdgeV(num);
						Vector3d vertex = this.Mesh.GetVertex(edgeV.a);
						Vector3d vertex2 = this.Mesh.GetVertex(edgeV.b);
						if (vertex.Distance(vertex2) < minLength)
						{
							int num2 = this.Mesh.IsBoundaryVertex(edgeV.a) ? edgeV.a : edgeV.b;
							int vRemove = (num2 == edgeV.a) ? edgeV.b : edgeV.a;
							DMesh3.EdgeCollapseInfo edgeCollapseInfo;
							if (this.Mesh.CollapseEdge(num2, vRemove, out edgeCollapseInfo) == MeshResult.Ok)
							{
								collapseCount++;
								if (!this.Mesh.IsBoundaryVertex(num2) || flag)
								{
									this.Mesh.SetVertex(num2, (vertex + vertex2) * 0.5);
								}
							}
						}
					}
				}
			}
			return true;
		}

		private bool collapse_all_degenerate_edges(double minLength, bool bBoundaryOnly)
		{
			bool flag = true;
			while (flag && !this.Cancelled())
			{
				int num;
				this.collapse_degenerate_edges(minLength, bBoundaryOnly, out num);
				if (num == 0)
				{
					flag = false;
				}
			}
			return true;
		}

		private bool disconnect_bowties(out int nRemaining)
		{
			MeshEditor meshEditor = new MeshEditor(this.Mesh);
			nRemaining = meshEditor.DisconnectAllBowties(10);
			return true;
		}

		private void repair_orientation(bool bGlobal)
		{
			MeshRepairOrientation meshRepairOrientation = new MeshRepairOrientation(this.Mesh, null);
			meshRepairOrientation.OrientComponents();
			if (this.Cancelled())
			{
				return;
			}
			if (bGlobal)
			{
				meshRepairOrientation.SolveGlobalOrientation();
			}
		}

		private bool remove_interior(out int nRemoved)
		{
			RemoveOccludedTriangles removeOccludedTriangles = new RemoveOccludedTriangles(this.Mesh);
			removeOccludedTriangles.PerVertex = true;
			removeOccludedTriangles.InsideMode = RemoveOccludedTriangles.CalculationMode.FastWindingNumber;
			removeOccludedTriangles.Apply();
			nRemoved = removeOccludedTriangles.RemovedT.Count<int>();
			return true;
		}

		private bool remove_occluded(out int nRemoved)
		{
			RemoveOccludedTriangles removeOccludedTriangles = new RemoveOccludedTriangles(this.Mesh);
			removeOccludedTriangles.PerVertex = true;
			removeOccludedTriangles.InsideMode = RemoveOccludedTriangles.CalculationMode.SimpleOcclusionTest;
			removeOccludedTriangles.Apply();
			nRemoved = removeOccludedTriangles.RemovedT.Count<int>();
			return true;
		}

		private bool do_remove_inside()
		{
			int num = 0;
			if (this.RemoveMode == MeshAutoRepair.RemoveModes.Interior)
			{
				return this.remove_interior(out num);
			}
			return this.RemoveMode != MeshAutoRepair.RemoveModes.Occluded || this.remove_occluded(out num);
		}

		private bool remove_loners()
		{
			MeshEditor.RemoveIsolatedTriangles(this.Mesh);
			return true;
		}

		public double RepairTolerance = 9.999999974752427E-07;

		public double MinEdgeLengthTol = 0.0001;

		public int ErosionIterations = 5;

		public MeshAutoRepair.RemoveModes RemoveMode;

		public ProgressCancel Progress;

		public DMesh3 Mesh;

		public enum RemoveModes
		{
			None,
			Interior,
			Occluded
		}
	}
}
