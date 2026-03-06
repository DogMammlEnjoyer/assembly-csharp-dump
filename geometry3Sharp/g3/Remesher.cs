using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace g3
{
	public class Remesher : MeshRefinerBase
	{
		private bool EnableInlineProjection
		{
			get
			{
				return this.ProjectionMode == Remesher.TargetProjectionMode.Inline;
			}
		}

		public Remesher(DMesh3 m) : base(m)
		{
		}

		protected Remesher()
		{
		}

		public IProjectionTarget ProjectionTarget
		{
			get
			{
				return this.target;
			}
		}

		public void SetProjectionTarget(IProjectionTarget target)
		{
			this.target = target;
		}

		public void SetTargetEdgeLength(double fLength)
		{
			this.MinEdgeLength = fLength * 0.66;
			this.MaxEdgeLength = fLength * 1.33;
		}

		public virtual void Precompute()
		{
			this.MeshIsClosed = true;
			foreach (int eid in this.mesh.EdgeIndices())
			{
				if (this.mesh.IsBoundaryEdge(eid))
				{
					this.MeshIsClosed = false;
					break;
				}
			}
		}

		public virtual void BasicRemeshPass()
		{
			if (this.mesh.TriangleCount == 0)
			{
				return;
			}
			this.begin_pass();
			this.begin_ops();
			int num = this.start_edges();
			bool flag = false;
			this.ModifiedEdgesLastPass = 0;
			for (;;)
			{
				if (this.mesh.IsEdge(num))
				{
					Remesher.ProcessResult processResult = this.ProcessEdge(num);
					if (processResult == Remesher.ProcessResult.Ok_Collapsed || processResult == Remesher.ProcessResult.Ok_Flipped || processResult == Remesher.ProcessResult.Ok_Split)
					{
						this.ModifiedEdgesLastPass++;
					}
				}
				if (this.Cancelled())
				{
					break;
				}
				num = this.next_edge(num, out flag);
				if (flag)
				{
					goto Block_6;
				}
			}
			return;
			Block_6:
			this.end_ops();
			if (this.Cancelled())
			{
				return;
			}
			this.begin_smooth();
			if (this.EnableSmoothing && this.SmoothSpeedT > 0.0)
			{
				if (this.EnableSmoothInPlace)
				{
					this.FullSmoothPass_InPlace(this.EnableParallelSmooth);
				}
				else
				{
					this.FullSmoothPass_Buffer(this.EnableParallelSmooth);
				}
				this.DoDebugChecks();
			}
			this.end_smooth();
			if (this.Cancelled())
			{
				return;
			}
			this.begin_project();
			if (this.target != null && this.ProjectionMode == Remesher.TargetProjectionMode.AfterRefinement)
			{
				this.FullProjectionPass();
				this.DoDebugChecks();
			}
			this.end_project();
			if (this.Cancelled())
			{
				return;
			}
			this.end_pass();
		}

		protected virtual void OnEdgeSplit(int edgeID, int va, int vb, DMesh3.EdgeSplitInfo splitInfo)
		{
		}

		protected virtual void OnEdgeCollapse(int edgeID, int va, int vb, DMesh3.EdgeCollapseInfo collapseInfo)
		{
		}

		protected virtual int start_edges()
		{
			this.nMaxEdgeID = this.mesh.MaxEdgeID;
			return 0;
		}

		protected virtual int next_edge(int cur_eid, out bool bDone)
		{
			int num = (cur_eid + 31337) % this.nMaxEdgeID;
			bDone = (num == 0);
			return num;
		}

		protected virtual IEnumerable<int> smooth_vertices()
		{
			return this.mesh.VertexIndices();
		}

		protected virtual IEnumerable<int> project_vertices()
		{
			return this.mesh.VertexIndices();
		}

		protected virtual Remesher.ProcessResult ProcessEdge(int edgeID)
		{
			EdgeConstraint edgeConstraint = (this.constraints == null) ? EdgeConstraint.Unconstrained : this.constraints.GetEdgeConstraint(edgeID);
			if (edgeConstraint.NoModifications)
			{
				return Remesher.ProcessResult.Ignored_EdgeIsFullyConstrained;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (!this.mesh.GetEdge(edgeID, ref num, ref num2, ref num3, ref num4))
			{
				return Remesher.ProcessResult.Failed_NotAnEdge;
			}
			bool flag = num4 == -1;
			Index2i edgeOpposingV = this.mesh.GetEdgeOpposingV(edgeID);
			int a = edgeOpposingV.a;
			int b = edgeOpposingV.b;
			Vector3d vertex = this.mesh.GetVertex(num);
			Vector3d vertex2 = this.mesh.GetVertex(num2);
			double num5 = vertex.DistanceSquared(vertex2);
			this.begin_collapse();
			int num6 = -1;
			bool flag2 = this.EnableCollapses && edgeConstraint.CanCollapse && num5 < this.MinEdgeLength * this.MinEdgeLength && base.can_collapse_constraints(edgeID, num, num2, a, b, num3, num4, out num6);
			bool flag3 = false;
			if (flag2)
			{
				int num7 = num2;
				int num8 = num;
				Vector3d vNewPos = (vertex + vertex2) * 0.5;
				if (num6 == num2)
				{
					vNewPos = vertex2;
				}
				else if (num6 == num)
				{
					num7 = num;
					num8 = num2;
					vNewPos = vertex;
				}
				else
				{
					vNewPos = this.get_projected_collapse_position(num7, vNewPos);
				}
				if (!this.PreventNormalFlips || (!base.collapse_creates_flip_or_invalid(num, num2, ref vNewPos, num3, num4) && !base.collapse_creates_flip_or_invalid(num2, num, ref vNewPos, num3, num4)))
				{
					this.COUNT_COLLAPSES++;
					DMesh3.EdgeCollapseInfo edgeCollapseInfo;
					if (this.mesh.CollapseEdge(num7, num8, out edgeCollapseInfo) == MeshResult.Ok)
					{
						this.mesh.SetVertex(num7, vNewPos);
						if (this.constraints != null)
						{
							this.constraints.ClearEdgeConstraint(edgeID);
							this.constraints.ClearEdgeConstraint(edgeCollapseInfo.eRemoved0);
							if (edgeCollapseInfo.eRemoved1 != -1)
							{
								this.constraints.ClearEdgeConstraint(edgeCollapseInfo.eRemoved1);
							}
							this.constraints.ClearVertexConstraint(num8);
						}
						this.OnEdgeCollapse(edgeID, num7, num8, edgeCollapseInfo);
						this.DoDebugChecks();
						return Remesher.ProcessResult.Ok_Collapsed;
					}
					flag3 = true;
				}
			}
			this.end_collapse();
			this.begin_flip();
			bool flag4 = false;
			if (this.EnableFlips && edgeConstraint.CanFlip && !flag)
			{
				bool flag5 = !this.MeshIsClosed && (flag || this.mesh.IsBoundaryVertex(num));
				bool flag6 = !this.MeshIsClosed && (flag || this.mesh.IsBoundaryVertex(num2));
				bool flag7 = !this.MeshIsClosed && this.mesh.IsBoundaryVertex(a);
				bool flag8 = !this.MeshIsClosed && this.mesh.IsBoundaryVertex(b);
				int vtxEdgeCount = this.mesh.GetVtxEdgeCount(num);
				int vtxEdgeCount2 = this.mesh.GetVtxEdgeCount(num2);
				int vtxEdgeCount3 = this.mesh.GetVtxEdgeCount(a);
				int vtxEdgeCount4 = this.mesh.GetVtxEdgeCount(b);
				int num9 = flag5 ? vtxEdgeCount : 6;
				int num10 = flag6 ? vtxEdgeCount2 : 6;
				int num11 = flag7 ? vtxEdgeCount3 : 6;
				int num12 = flag8 ? vtxEdgeCount4 : 6;
				int num13 = Math.Abs(vtxEdgeCount - num9) + Math.Abs(vtxEdgeCount2 - num10) + Math.Abs(vtxEdgeCount3 - num11) + Math.Abs(vtxEdgeCount4 - num12);
				bool flag9 = Math.Abs(vtxEdgeCount - 1 - num9) + Math.Abs(vtxEdgeCount2 - 1 - num10) + Math.Abs(vtxEdgeCount3 + 1 - num11) + Math.Abs(vtxEdgeCount4 + 1 - num12) < num13;
				if (flag9 && this.PreventNormalFlips && base.flip_inverts_normals(num, num2, a, b, num3))
				{
					flag9 = false;
				}
				if (flag9)
				{
					this.COUNT_FLIPS++;
					DMesh3.EdgeFlipInfo edgeFlipInfo;
					if (this.mesh.FlipEdge(edgeID, out edgeFlipInfo) == MeshResult.Ok)
					{
						this.DoDebugChecks();
						return Remesher.ProcessResult.Ok_Flipped;
					}
					flag4 = true;
				}
			}
			this.end_flip();
			this.begin_split();
			bool flag10 = false;
			if (this.EnableSplits && edgeConstraint.CanSplit && num5 > this.MaxEdgeLength * this.MaxEdgeLength)
			{
				this.COUNT_SPLITS++;
				DMesh3.EdgeSplitInfo splitInfo;
				if (this.mesh.SplitEdge(edgeID, out splitInfo, 0.5) == MeshResult.Ok)
				{
					this.update_after_split(edgeID, num, num2, ref splitInfo);
					this.OnEdgeSplit(edgeID, num, num2, splitInfo);
					this.DoDebugChecks();
					return Remesher.ProcessResult.Ok_Split;
				}
				flag10 = true;
			}
			this.end_split();
			if (flag4 || flag10 || flag3)
			{
				return Remesher.ProcessResult.Failed_OpNotSuccessful;
			}
			return Remesher.ProcessResult.Ignored_EdgeIsFine;
		}

		protected virtual void update_after_split(int edgeID, int va, int vb, ref DMesh3.EdgeSplitInfo splitInfo)
		{
			bool flag = false;
			if (this.constraints != null && this.constraints.HasEdgeConstraint(edgeID))
			{
				this.constraints.SetOrUpdateEdgeConstraint(splitInfo.eNewBN, this.constraints.GetEdgeConstraint(edgeID));
				VertexConstraint vertexConstraint = this.constraints.GetVertexConstraint(va);
				VertexConstraint vertexConstraint2 = this.constraints.GetVertexConstraint(vb);
				if (vertexConstraint.Fixed && vertexConstraint2.Fixed)
				{
					int setID = (vertexConstraint.FixedSetID > 0 && vertexConstraint.FixedSetID == vertexConstraint2.FixedSetID) ? vertexConstraint.FixedSetID : -1;
					this.constraints.SetOrUpdateVertexConstraint(splitInfo.vNew, new VertexConstraint(true, setID));
					flag = true;
				}
				if (vertexConstraint.Target != null || vertexConstraint2.Target != null)
				{
					IProjectionTarget projectionTarget = this.constraints.GetEdgeConstraint(edgeID).Target;
					IProjectionTarget projectionTarget2 = null;
					if (vertexConstraint.Target == vertexConstraint2.Target && vertexConstraint.Target == projectionTarget)
					{
						projectionTarget2 = projectionTarget;
					}
					else if (vertexConstraint.Target == projectionTarget && vertexConstraint2.Fixed)
					{
						projectionTarget2 = projectionTarget;
					}
					else if (vertexConstraint2.Target == projectionTarget && vertexConstraint.Fixed)
					{
						projectionTarget2 = projectionTarget;
					}
					if (projectionTarget2 != null)
					{
						this.constraints.SetOrUpdateVertexConstraint(splitInfo.vNew, new VertexConstraint(projectionTarget2));
						this.project_vertex(splitInfo.vNew, projectionTarget2);
						flag = true;
					}
				}
			}
			if (this.EnableInlineProjection && !flag && this.target != null)
			{
				this.project_vertex(splitInfo.vNew, this.target);
			}
		}

		protected virtual void project_vertex(int vID, IProjectionTarget targetIn)
		{
			Vector3d vertex = this.mesh.GetVertex(vID);
			Vector3d vNewPos = targetIn.Project(vertex, vID);
			this.mesh.SetVertex(vID, vNewPos);
		}

		protected virtual Vector3d get_projected_collapse_position(int vid, Vector3d vNewPos)
		{
			if (this.constraints != null)
			{
				VertexConstraint vertexConstraint = this.constraints.GetVertexConstraint(vid);
				if (vertexConstraint.Target != null)
				{
					return vertexConstraint.Target.Project(vNewPos, vid);
				}
				if (vertexConstraint.Fixed)
				{
					return vNewPos;
				}
			}
			if (this.EnableInlineProjection && this.target != null && (this.VertexControlF == null || (this.VertexControlF(vid) & Remesher.VertexControl.NoProject) == Remesher.VertexControl.AllowAll))
			{
				return this.target.Project(vNewPos, vid);
			}
			return vNewPos;
		}

		protected virtual void FullSmoothPass_InPlace(bool bParallel)
		{
			Func<DMesh3, int, double, Vector3d> smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.UniformSmooth);
			if (this.CustomSmoothF != null)
			{
				smoothFunc = this.CustomSmoothF;
			}
			else if (this.SmoothType == Remesher.SmoothTypes.MeanValue)
			{
				smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.MeanValueSmooth);
			}
			else if (this.SmoothType == Remesher.SmoothTypes.Cotan)
			{
				smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.CotanSmooth);
			}
			Action<int> action = delegate(int vID)
			{
				bool flag = false;
				Vector3d vNewPos = this.ComputeSmoothedVertexPos(vID, smoothFunc, out flag);
				if (flag)
				{
					this.mesh.SetVertex(vID, vNewPos);
				}
			};
			if (bParallel)
			{
				gParallel.ForEach<int>(this.smooth_vertices(), action);
				return;
			}
			foreach (int obj in this.smooth_vertices())
			{
				action(obj);
			}
		}

		protected virtual void FullSmoothPass_Buffer(bool bParallel)
		{
			this.InitializeVertexBufferForPass();
			Func<DMesh3, int, double, Vector3d> smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.UniformSmooth);
			if (this.CustomSmoothF != null)
			{
				smoothFunc = this.CustomSmoothF;
			}
			else if (this.SmoothType == Remesher.SmoothTypes.MeanValue)
			{
				smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.MeanValueSmooth);
			}
			else if (this.SmoothType == Remesher.SmoothTypes.Cotan)
			{
				smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.CotanSmooth);
			}
			Action<int> action = delegate(int vID)
			{
				bool flag = false;
				Vector3d value = this.ComputeSmoothedVertexPos(vID, smoothFunc, out flag);
				if (flag)
				{
					this.vModifiedV[vID] = true;
					this.vBufferV[vID] = value;
				}
			};
			if (bParallel)
			{
				gParallel.ForEach<int>(this.smooth_vertices(), action);
			}
			else
			{
				foreach (int obj in this.smooth_vertices())
				{
					action(obj);
				}
			}
			this.ApplyVertexBuffer(bParallel);
		}

		protected virtual void InitializeVertexBufferForPass()
		{
			if (this.vBufferV.size < this.mesh.MaxVertexID)
			{
				this.vBufferV.resize(this.mesh.MaxVertexID + this.mesh.MaxVertexID / 5);
			}
			if (this.vModifiedV.Length < this.mesh.MaxVertexID)
			{
				this.vModifiedV = new BitArray(2 * this.mesh.MaxVertexID);
				return;
			}
			this.vModifiedV.SetAll(false);
		}

		protected virtual void ApplyVertexBuffer(bool bParallel)
		{
			if (bParallel)
			{
				gParallel.BlockStartEnd(0, this.mesh.MaxVertexID - 1, delegate(int a, int b)
				{
					for (int i = a; i <= b; i++)
					{
						if (this.vModifiedV[i])
						{
							base.Mesh.SetVertex(i, this.vBufferV[i]);
						}
					}
				}, -1, false);
				return;
			}
			foreach (int num in this.mesh.VertexIndices())
			{
				if (this.vModifiedV[num])
				{
					base.Mesh.SetVertex(num, this.vBufferV[num]);
				}
			}
		}

		protected virtual Vector3d ComputeSmoothedVertexPos(int vID, Func<DMesh3, int, double, Vector3d> smoothFunc, out bool bModified)
		{
			bModified = false;
			VertexConstraint unconstrained = VertexConstraint.Unconstrained;
			bool flag = base.get_vertex_constraint(vID, ref unconstrained);
			if (unconstrained.Fixed)
			{
				return base.Mesh.GetVertex(vID);
			}
			Remesher.VertexControl vertexControl = (this.VertexControlF == null) ? Remesher.VertexControl.AllowAll : this.VertexControlF(vID);
			if ((vertexControl & Remesher.VertexControl.NoSmooth) != Remesher.VertexControl.AllowAll)
			{
				return base.Mesh.GetVertex(vID);
			}
			Vector3d vector3d = smoothFunc(this.mesh, vID, this.SmoothSpeedT);
			if (unconstrained.Target != null)
			{
				vector3d = unconstrained.Target.Project(vector3d, vID);
			}
			else if (this.EnableInlineProjection && this.target != null && (vertexControl & Remesher.VertexControl.NoProject) == Remesher.VertexControl.AllowAll)
			{
				vector3d = this.target.Project(vector3d, vID);
			}
			bModified = true;
			return vector3d;
		}

		protected virtual void FullProjectionPass()
		{
			Action<int> action = delegate(int vID)
			{
				if (base.vertex_is_constrained(vID))
				{
					return;
				}
				if (this.VertexControlF != null && (this.VertexControlF(vID) & Remesher.VertexControl.NoProject) != Remesher.VertexControl.AllowAll)
				{
					return;
				}
				Vector3d vertex = this.mesh.GetVertex(vID);
				Vector3d vNewPos = this.target.Project(vertex, vID);
				this.mesh.SetVertex(vID, vNewPos);
			};
			if (this.EnableParallelProjection)
			{
				gParallel.ForEach<int>(this.project_vertices(), action);
				return;
			}
			foreach (int obj in this.project_vertices())
			{
				action(obj);
			}
		}

		[Conditional("DEBUG")]
		private void RuntimeDebugCheck(int eid)
		{
			if (this.DebugEdges.Contains(eid))
			{
				Debugger.Break();
			}
		}

		protected virtual void DoDebugChecks()
		{
			if (!this.ENABLE_DEBUG_CHECKS)
			{
				return;
			}
			this.DebugCheckVertexConstraints();
		}

		private void DebugCheckVertexConstraints()
		{
			if (this.constraints == null)
			{
				return;
			}
			foreach (object obj in this.constraints.VertexConstraintsItr())
			{
				KeyValuePair<int, VertexConstraint> keyValuePair = (KeyValuePair<int, VertexConstraint>)obj;
				int key = keyValuePair.Key;
				if (keyValuePair.Value.Target != null)
				{
					Vector3d vertex = this.mesh.GetVertex(key);
					Vector3d v = keyValuePair.Value.Target.Project(vertex, key);
					if (vertex.DistanceSquared(v) > 9.999999747378752E-05)
					{
						Util.gBreakToDebugger();
					}
				}
			}
		}

		protected virtual void begin_pass()
		{
			if (this.ENABLE_PROFILING)
			{
				this.COUNT_SPLITS = (this.COUNT_COLLAPSES = (this.COUNT_FLIPS = 0));
				this.AllOpsW = new Stopwatch();
				this.SmoothW = new Stopwatch();
				this.ProjectW = new Stopwatch();
				this.FlipW = new Stopwatch();
				this.SplitW = new Stopwatch();
				this.CollapseW = new Stopwatch();
			}
		}

		protected virtual void end_pass()
		{
			if (this.ENABLE_PROFILING)
			{
				Console.WriteLine(string.Format("RemeshPass: T {0} V {1} splits {2} flips {3} collapses {4}", new object[]
				{
					this.mesh.TriangleCount,
					this.mesh.VertexCount,
					this.COUNT_SPLITS,
					this.COUNT_FLIPS,
					this.COUNT_COLLAPSES
				}));
				Console.WriteLine(string.Format("           Timing1:  ops {0} smooth {1} project {2}", Util.ToSecMilli(this.AllOpsW.Elapsed), Util.ToSecMilli(this.SmoothW.Elapsed), Util.ToSecMilli(this.ProjectW.Elapsed)));
				Console.WriteLine(string.Format("           Timing2:  collapse {0} flip {1} split {2}", Util.ToSecMilli(this.CollapseW.Elapsed), Util.ToSecMilli(this.FlipW.Elapsed), Util.ToSecMilli(this.SplitW.Elapsed)));
			}
		}

		protected virtual void begin_ops()
		{
			if (this.ENABLE_PROFILING)
			{
				this.AllOpsW.Start();
			}
		}

		protected virtual void end_ops()
		{
			if (this.ENABLE_PROFILING)
			{
				this.AllOpsW.Stop();
			}
		}

		protected virtual void begin_smooth()
		{
			if (this.ENABLE_PROFILING)
			{
				this.SmoothW.Start();
			}
		}

		protected virtual void end_smooth()
		{
			if (this.ENABLE_PROFILING)
			{
				this.SmoothW.Stop();
			}
		}

		protected virtual void begin_project()
		{
			if (this.ENABLE_PROFILING)
			{
				this.ProjectW.Start();
			}
		}

		protected virtual void end_project()
		{
			if (this.ENABLE_PROFILING)
			{
				this.ProjectW.Stop();
			}
		}

		protected virtual void begin_collapse()
		{
			if (this.ENABLE_PROFILING)
			{
				this.CollapseW.Start();
			}
		}

		protected virtual void end_collapse()
		{
			if (this.ENABLE_PROFILING)
			{
				this.CollapseW.Stop();
			}
		}

		protected virtual void begin_flip()
		{
			if (this.ENABLE_PROFILING)
			{
				this.FlipW.Start();
			}
		}

		protected virtual void end_flip()
		{
			if (this.ENABLE_PROFILING)
			{
				this.FlipW.Stop();
			}
		}

		protected virtual void begin_split()
		{
			if (this.ENABLE_PROFILING)
			{
				this.SplitW.Start();
			}
		}

		protected virtual void end_split()
		{
			if (this.ENABLE_PROFILING)
			{
				this.SplitW.Stop();
			}
		}

		private IProjectionTarget target;

		public bool EnableFlips = true;

		public bool EnableCollapses = true;

		public bool EnableSplits = true;

		public bool EnableSmoothing = true;

		public bool PreventNormalFlips;

		public double MinEdgeLength = 0.0010000000474974513;

		public double MaxEdgeLength = 0.10000000149011612;

		public double SmoothSpeedT = 0.10000000149011612;

		public Remesher.SmoothTypes SmoothType;

		public Func<DMesh3, int, double, Vector3d> CustomSmoothF;

		public Func<int, Remesher.VertexControl> VertexControlF;

		public List<int> DebugEdges = new List<int>();

		public Remesher.TargetProjectionMode ProjectionMode = Remesher.TargetProjectionMode.AfterRefinement;

		public bool EnableParallelProjection = true;

		public bool EnableParallelSmooth = true;

		public bool EnableSmoothInPlace;

		public bool ENABLE_PROFILING;

		private bool MeshIsClosed;

		public int ModifiedEdgesLastPass;

		private const int nPrime = 31337;

		private int nMaxEdgeID;

		protected DVector<Vector3d> vBufferV = new DVector<Vector3d>();

		protected BitArray vModifiedV = new BitArray(4096);

		public bool ENABLE_DEBUG_CHECKS;

		private int COUNT_SPLITS;

		private int COUNT_COLLAPSES;

		private int COUNT_FLIPS;

		private Stopwatch AllOpsW;

		private Stopwatch SmoothW;

		private Stopwatch ProjectW;

		private Stopwatch FlipW;

		private Stopwatch SplitW;

		private Stopwatch CollapseW;

		public enum SmoothTypes
		{
			Uniform,
			Cotan,
			MeanValue
		}

		[Flags]
		public enum VertexControl
		{
			AllowAll = 0,
			NoSmooth = 1,
			NoProject = 2,
			NoMovement = 3
		}

		public enum TargetProjectionMode
		{
			NoProjection,
			AfterRefinement,
			Inline
		}

		protected enum ProcessResult
		{
			Ok_Collapsed,
			Ok_Flipped,
			Ok_Split,
			Ignored_EdgeIsFine,
			Ignored_EdgeIsFullyConstrained,
			Failed_OpNotSuccessful,
			Failed_NotAnEdge
		}
	}
}
