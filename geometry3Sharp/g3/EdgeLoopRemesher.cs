using System;
using System.Collections.Generic;

namespace g3
{
	public class EdgeLoopRemesher : Remesher
	{
		public EdgeLoopRemesher(DMesh3 m, EdgeLoop loop) : base(m)
		{
			this.UpdateLoop(loop);
			this.EnableFlips = false;
			this.CustomSmoothF = new Func<DMesh3, int, double, Vector3d>(this.loop_smooth_vertex);
		}

		public void UpdateLoop(EdgeLoop loop)
		{
			this.InputLoop = loop;
			this.OutputLoop = null;
			this.CurrentLoopE = new List<int>(loop.Edges);
			this.CurrentLoopV = new List<int>(loop.Vertices);
		}

		public override void Precompute()
		{
			base.Precompute();
		}

		protected override int start_edges()
		{
			this.RemainingE = new List<int>(this.CurrentLoopE.Count);
			int num = 31337;
			int num2 = 0;
			do
			{
				this.RemainingE.Add(this.CurrentLoopE[num2]);
				num2 = (num2 + num) % this.CurrentLoopE.Count;
			}
			while (num2 != 0);
			int result = this.RemainingE[this.RemainingE.Count - 1];
			this.RemainingE.RemoveAt(this.RemainingE.Count - 1);
			return result;
		}

		protected override int next_edge(int cur_eid, out bool bDone)
		{
			if (this.RemainingE.Count == 0)
			{
				bDone = true;
				return 0;
			}
			bDone = false;
			int result = this.RemainingE[this.RemainingE.Count - 1];
			this.RemainingE.RemoveAt(this.RemainingE.Count - 1);
			return result;
		}

		protected override void end_pass()
		{
			this.OutputLoop = new EdgeLoop(this.mesh, this.CurrentLoopV.ToArray(), this.CurrentLoopE.ToArray(), false);
		}

		protected override void begin_smooth()
		{
			base.begin_smooth();
			if (this.LocalSmoothingRings > 0)
			{
				this.smoothV.Clear();
				if (this.LocalSmoothingRings == 1)
				{
					for (int i = 0; i < this.CurrentLoopV.Count; i++)
					{
						this.smoothV.Add(this.CurrentLoopV[i]);
						foreach (int item in this.mesh.VtxVerticesItr(this.CurrentLoopV[i]))
						{
							this.smoothV.Add(item);
						}
					}
					return;
				}
				MeshVertexSelection meshVertexSelection = new MeshVertexSelection(this.mesh);
				meshVertexSelection.Select(this.CurrentLoopV);
				meshVertexSelection.ExpandToOneRingNeighbours(this.LocalSmoothingRings, null);
				foreach (int item2 in meshVertexSelection)
				{
					this.smoothV.Add(item2);
				}
			}
		}

		protected override IEnumerable<int> smooth_vertices()
		{
			if (this.LocalSmoothingRings > 0)
			{
				return this.smoothV;
			}
			return this.CurrentLoopV;
		}

		private Vector3d loop_smooth_vertex(DMesh3 mesh, int vid, double alpha)
		{
			if (this.LocalSmoothingRings > 0 && !this.CurrentLoopV.Contains(vid))
			{
				bool flag = false;
				return base.ComputeSmoothedVertexPos(vid, new Func<DMesh3, int, double, Vector3d>(MeshUtil.UniformSmooth), out flag);
			}
			int num = this.CurrentLoopV.FindIndex((int i) => i == vid);
			if (num < 0)
			{
				return mesh.GetVertex(vid);
			}
			int index = (num + this.CurrentLoopV.Count - 1) % this.CurrentLoopV.Count;
			int index2 = (num + 1) % this.CurrentLoopV.Count;
			Vector3d v = mesh.GetVertex(this.CurrentLoopV[index]) + mesh.GetVertex(this.CurrentLoopV[index2]);
			v *= 0.5;
			return (1.0 - alpha) * mesh.GetVertex(vid) + alpha * v;
		}

		protected override IEnumerable<int> project_vertices()
		{
			if (this.LocalSmoothingRings > 0)
			{
				return this.smoothV;
			}
			return this.CurrentLoopV;
		}

		protected override void OnEdgeSplit(int edgeID, int va, int vb, DMesh3.EdgeSplitInfo splitInfo)
		{
			int num = this.CurrentLoopV.FindIndex((int i) => i == va);
			int num2 = this.CurrentLoopV.FindIndex((int i) => i == vb);
			if (this.CurrentLoopE.FindIndex((int i) => i == edgeID) == this.CurrentLoopE.Count - 1)
			{
				this.CurrentLoopV.Add(splitInfo.vNew);
			}
			else if (num < num2)
			{
				this.CurrentLoopV.Insert(num2, splitInfo.vNew);
			}
			else
			{
				this.CurrentLoopV.Insert(num, splitInfo.vNew);
			}
			this.rebuild_edge_list();
		}

		protected override void OnEdgeCollapse(int edgeID, int va, int vb, DMesh3.EdgeCollapseInfo collapseInfo)
		{
			int num = this.CurrentLoopV.FindIndex((int i) => i == collapseInfo.vRemoved);
			this.CurrentLoopV.RemoveAt(num);
			int num2 = this.CurrentLoopE.FindIndex((int i) => i == edgeID);
			this.CurrentLoopE.RemoveAt(num2);
			if (num == 0 && num2 == this.CurrentLoopE.Count)
			{
				this.rebuild_edge_list();
			}
		}

		private bool check_loop()
		{
			for (int i = 0; i < this.CurrentLoopV.Count; i++)
			{
				this.mesh.FindEdge(this.CurrentLoopV[i], this.CurrentLoopV[(i + 1) % this.CurrentLoopV.Count]);
			}
			return true;
		}

		private void rebuild_edge_list()
		{
			this.CurrentLoopE.Clear();
			int count = this.CurrentLoopV.Count;
			for (int i = 0; i < count; i++)
			{
				this.CurrentLoopE.Add(this.mesh.FindEdge(this.CurrentLoopV[i], this.CurrentLoopV[(i + 1) % count]));
			}
		}

		public EdgeLoop InputLoop;

		public EdgeLoop OutputLoop;

		public int LocalSmoothingRings;

		private List<int> CurrentLoopE;

		private List<int> CurrentLoopV;

		private List<int> RemainingE;

		private const int nPrime = 31337;

		private HashSet<int> smoothV = new HashSet<int>();
	}
}
