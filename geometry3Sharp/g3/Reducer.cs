using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace g3
{
	public class Reducer : MeshRefinerBase
	{
		private bool EnableInlineProjection
		{
			get
			{
				return this.ProjectionMode == Reducer.TargetProjectionMode.Inline;
			}
		}

		public Reducer(DMesh3 m) : base(m)
		{
		}

		protected Reducer()
		{
		}

		public void SetProjectionTarget(IProjectionTarget target)
		{
			this.target = target;
		}

		public virtual void DoReduce()
		{
			if (this.mesh.TriangleCount == 0)
			{
				return;
			}
			this.begin_pass();
			this.begin_setup();
			this.Precompute(false);
			if (this.Cancelled())
			{
				return;
			}
			this.InitializeVertexQuadrics();
			if (this.Cancelled())
			{
				return;
			}
			this.InitializeQueue();
			if (this.Cancelled())
			{
				return;
			}
			this.end_setup();
			this.begin_ops();
			this.begin_collapse();
			while (this.EdgeQueue.Count > 0)
			{
				if (this.ReduceMode == Reducer.TargetModes.VertexCount)
				{
					if (this.mesh.VertexCount <= this.TargetCount)
					{
						break;
					}
				}
				else if (this.mesh.TriangleCount <= this.TargetCount)
				{
					break;
				}
				this.COUNT_ITERATIONS++;
				int num = this.EdgeQueue.Dequeue();
				if (this.mesh.IsEdge(num))
				{
					if (this.Cancelled())
					{
						return;
					}
					int num2;
					if (this.CollapseEdge(num, this.EdgeQuadrics[num].collapse_pt, out num2) == Reducer.ProcessResult.Ok_Collapsed)
					{
						this.vertQuadrics[num2] = this.EdgeQuadrics[num].q;
						this.UpdateNeighbours(num2);
					}
				}
			}
			this.end_collapse();
			this.end_ops();
			if (this.Cancelled())
			{
				return;
			}
			this.Reproject();
			this.end_pass();
		}

		public virtual void ReduceToTriangleCount(int nCount)
		{
			this.ReduceMode = Reducer.TargetModes.TriangleCount;
			this.TargetCount = Math.Max(1, nCount);
			this.MinEdgeLength = double.MaxValue;
			this.DoReduce();
		}

		public virtual void ReduceToVertexCount(int nCount)
		{
			this.ReduceMode = Reducer.TargetModes.VertexCount;
			this.TargetCount = Math.Max(3, nCount);
			this.MinEdgeLength = double.MaxValue;
			this.DoReduce();
		}

		public virtual void ReduceToEdgeLength(double minEdgeLen)
		{
			this.ReduceMode = Reducer.TargetModes.MinEdgeLength;
			this.TargetCount = 1;
			this.MinEdgeLength = minEdgeLen;
			this.DoReduce();
		}

		public virtual void FastCollapsePass(double fMinEdgeLength, int nRounds = 1, bool MeshIsClosedHint = false)
		{
			if (this.mesh.TriangleCount == 0)
			{
				return;
			}
			this.MinEdgeLength = fMinEdgeLength;
			double num = this.MinEdgeLength * this.MinEdgeLength;
			this.HaveBoundary = false;
			this.begin_pass();
			this.begin_setup();
			this.Precompute(MeshIsClosedHint);
			if (this.Cancelled())
			{
				return;
			}
			this.end_setup();
			this.begin_ops();
			this.begin_collapse();
			int maxEdgeID = this.mesh.MaxEdgeID;
			for (int i = 0; i < nRounds; i++)
			{
				int num2 = 0;
				Vector3d zero = Vector3d.Zero;
				Vector3d zero2 = Vector3d.Zero;
				for (int j = 0; j < maxEdgeID; j++)
				{
					if (this.mesh.IsEdge(j) && !this.mesh.IsBoundaryEdge(j))
					{
						if (this.Cancelled())
						{
							return;
						}
						this.mesh.GetEdgeV(j, ref zero, ref zero2);
						if (zero.DistanceSquared(ref zero2) <= num)
						{
							this.COUNT_ITERATIONS++;
							Vector3d vNewPos = (zero + zero2) * 0.5;
							int num3;
							if (this.CollapseEdge(j, vNewPos, out num3) == Reducer.ProcessResult.Ok_Collapsed)
							{
								num2++;
							}
						}
					}
				}
				if (num2 == 0)
				{
					break;
				}
			}
			this.end_collapse();
			this.end_ops();
			if (this.Cancelled())
			{
				return;
			}
			this.Reproject();
			this.end_pass();
		}

		protected virtual void InitializeVertexQuadrics()
		{
			int maxTriangleID = this.mesh.MaxTriangleID;
			QuadricError[] triQuadrics = new QuadricError[maxTriangleID];
			double[] triAreas = new double[maxTriangleID];
			gParallel.BlockStartEnd(0, this.mesh.MaxTriangleID - 1, delegate(int start_tid, int end_tid)
			{
				for (int i = start_tid; i <= end_tid; i++)
				{
					if (this.mesh.IsTriangle(i))
					{
						Vector3d vector3d;
						Vector3d vector3d2;
						this.mesh.GetTriInfo(i, out vector3d, out triAreas[i], out vector3d2);
						triQuadrics[i] = new QuadricError(ref vector3d, ref vector3d2);
					}
				}
			}, -1, false);
			int maxVertexID = this.mesh.MaxVertexID;
			this.vertQuadrics = new QuadricError[maxVertexID];
			gParallel.BlockStartEnd(0, this.mesh.MaxVertexID - 1, delegate(int start_vid, int end_vid)
			{
				for (int i = start_vid; i <= end_vid; i++)
				{
					this.vertQuadrics[i] = QuadricError.Zero;
					if (this.mesh.IsVertex(i))
					{
						foreach (int num in this.mesh.VtxTrianglesItr(i))
						{
							this.vertQuadrics[i].Add(triAreas[num], ref triQuadrics[num]);
						}
					}
				}
			}, -1, false);
		}

		protected virtual void InitializeQueue()
		{
			int edgeCount = this.mesh.EdgeCount;
			int maxEdgeID = this.mesh.MaxEdgeID;
			this.EdgeQuadrics = new Reducer.QEdge[maxEdgeID];
			this.EdgeQueue = new IndexPriorityQueue(maxEdgeID);
			float[] edgeErrors = new float[maxEdgeID];
			gParallel.BlockStartEnd(0, maxEdgeID - 1, delegate(int start_eid, int end_eid)
			{
				for (int k = start_eid; k <= end_eid; k++)
				{
					if (this.mesh.IsEdge(k))
					{
						Index2i edgeV = this.mesh.GetEdgeV(k);
						QuadricError quadricError = new QuadricError(ref this.vertQuadrics[edgeV.a], ref this.vertQuadrics[edgeV.b]);
						Vector3d vector3d = this.OptimalPoint(k, ref quadricError, edgeV.a, edgeV.b);
						edgeErrors[k] = (float)quadricError.Evaluate(ref vector3d);
						this.EdgeQuadrics[k] = new Reducer.QEdge(k, ref quadricError, ref vector3d);
					}
				}
			}, -1, false);
			int[] array = new int[maxEdgeID];
			for (int i = 0; i < maxEdgeID; i++)
			{
				array[i] = i;
			}
			Array.Sort<float, int>(edgeErrors, array);
			for (int j = 0; j < edgeErrors.Length; j++)
			{
				int num = array[j];
				if (this.mesh.IsEdge(num))
				{
					Reducer.QEdge qedge = this.EdgeQuadrics[num];
					this.EdgeQueue.Insert(qedge.eid, edgeErrors[j]);
				}
			}
		}

		protected Vector3d OptimalPoint(int eid, ref QuadricError q, int ea, int eb)
		{
			if (this.HaveBoundary && this.PreserveBoundaryShape)
			{
				if (this.mesh.IsBoundaryEdge(eid))
				{
					return (this.mesh.GetVertex(ea) + this.mesh.GetVertex(eb)) * 0.5;
				}
				if (this.IsBoundaryV(ea))
				{
					return this.mesh.GetVertex(ea);
				}
				if (this.IsBoundaryV(eb))
				{
					return this.mesh.GetVertex(eb);
				}
			}
			if (!this.MinimizeQuadricPositionError)
			{
				return this.project((this.mesh.GetVertex(ea) + this.mesh.GetVertex(eb)) * 0.5);
			}
			Vector3d zero = Vector3d.Zero;
			if (q.OptimalPoint(ref zero))
			{
				return this.project(zero);
			}
			Vector3d vertex = this.mesh.GetVertex(ea);
			Vector3d vertex2 = this.mesh.GetVertex(eb);
			Vector3d result = this.project((vertex + vertex2) * 0.5);
			double num = q.Evaluate(ref vertex);
			double num2 = q.Evaluate(ref vertex2);
			double c = q.Evaluate(ref result);
			double num3 = MathUtil.Min(num, num2, c);
			if (num3 == num)
			{
				return vertex;
			}
			if (num3 == num2)
			{
				return vertex2;
			}
			return result;
		}

		private Vector3d project(Vector3d pos)
		{
			if (this.EnableInlineProjection && this.target != null)
			{
				return this.target.Project(pos, -1);
			}
			return pos;
		}

		protected virtual void UpdateNeighbours(int vid)
		{
			foreach (int num in this.mesh.VtxEdgesItr(vid))
			{
				Index2i edgeV = this.mesh.GetEdgeV(num);
				QuadricError quadricError = new QuadricError(ref this.vertQuadrics[edgeV.a], ref this.vertQuadrics[edgeV.b]);
				Vector3d vector3d = this.OptimalPoint(num, ref quadricError, edgeV.a, edgeV.b);
				double num2 = quadricError.Evaluate(ref vector3d);
				this.EdgeQuadrics[num] = new Reducer.QEdge(num, ref quadricError, ref vector3d);
				if (this.EdgeQueue.Contains(num))
				{
					this.EdgeQueue.Update(num, (float)num2);
				}
				else
				{
					this.EdgeQueue.Insert(num, (float)num2);
				}
			}
		}

		protected virtual void Reproject()
		{
			this.begin_project();
			if (this.target != null && this.ProjectionMode == Reducer.TargetProjectionMode.AfterRefinement)
			{
				this.FullProjectionPass();
				this.DoDebugChecks();
			}
			this.end_project();
		}

		protected virtual void Precompute(bool bMeshIsClosed = false)
		{
			this.HaveBoundary = false;
			this.IsBoundaryVtxCache = new bool[this.mesh.MaxVertexID];
			if (!bMeshIsClosed)
			{
				foreach (int eID in this.mesh.BoundaryEdgeIndices())
				{
					Index2i edgeV = this.mesh.GetEdgeV(eID);
					this.IsBoundaryVtxCache[edgeV.a] = true;
					this.IsBoundaryVtxCache[edgeV.b] = true;
					this.HaveBoundary = true;
				}
			}
		}

		protected bool IsBoundaryV(int vid)
		{
			return this.IsBoundaryVtxCache[vid];
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

		protected virtual IEnumerable<int> project_vertices()
		{
			return this.mesh.VertexIndices();
		}

		protected virtual Reducer.ProcessResult CollapseEdge(int edgeID, Vector3d vNewPos, out int collapseToV)
		{
			collapseToV = -1;
			EdgeConstraint edgeConstraint = (this.constraints == null) ? EdgeConstraint.Unconstrained : this.constraints.GetEdgeConstraint(edgeID);
			if (edgeConstraint.NoModifications)
			{
				return Reducer.ProcessResult.Ignored_EdgeIsFullyConstrained;
			}
			if (!edgeConstraint.CanCollapse)
			{
				return Reducer.ProcessResult.Ignored_EdgeIsFullyConstrained;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (!this.mesh.GetEdge(edgeID, ref num, ref num2, ref num3, ref num4))
			{
				return Reducer.ProcessResult.Failed_NotAnEdge;
			}
			bool flag = num4 == -1;
			Index3i triangle = this.mesh.GetTriangle(num3);
			int c = IndexUtil.find_tri_other_vtx(num, num2, triangle);
			Index3i tri_verts = flag ? DMesh3.InvalidTriangle : this.mesh.GetTriangle(num4);
			int d = flag ? -1 : IndexUtil.find_tri_other_vtx(num, num2, tri_verts);
			Vector3d vertex = this.mesh.GetVertex(num);
			Vector3d vertex2 = this.mesh.GetVertex(num2);
			if ((vertex - vertex2).LengthSquared > this.MinEdgeLength * this.MinEdgeLength)
			{
				return Reducer.ProcessResult.Ignored_EdgeTooLong;
			}
			this.begin_collapse();
			int num5 = -1;
			if (!base.can_collapse_constraints(edgeID, num, num2, c, d, num3, num4, out num5))
			{
				return Reducer.ProcessResult.Ignored_Constrained;
			}
			if (this.PreserveBoundaryShape && this.HaveBoundary)
			{
				if (num5 != -1 && ((this.IsBoundaryV(num2) && num5 != num2) || (this.IsBoundaryV(num) && num5 != num)))
				{
					return Reducer.ProcessResult.Ignored_Constrained;
				}
				if (this.IsBoundaryV(num2))
				{
					num5 = num2;
				}
				else if (this.IsBoundaryV(num))
				{
					num5 = num;
				}
			}
			Reducer.ProcessResult result = Reducer.ProcessResult.Failed_OpNotSuccessful;
			int num6 = num2;
			int num7 = num;
			if (num5 == num2)
			{
				vNewPos = vertex2;
			}
			else if (num5 == num)
			{
				num6 = num;
				num7 = num2;
				vNewPos = vertex;
			}
			else
			{
				vNewPos = this.get_projected_collapse_position(num6, vNewPos);
			}
			if (base.collapse_creates_flip_or_invalid(num, num2, ref vNewPos, num3, num4) || base.collapse_creates_flip_or_invalid(num2, num, ref vNewPos, num3, num4))
			{
				result = Reducer.ProcessResult.Ignored_CreatesFlip;
			}
			else
			{
				this.COUNT_COLLAPSES++;
				DMesh3.EdgeCollapseInfo edgeCollapseInfo;
				if (this.mesh.CollapseEdge(num6, num7, out edgeCollapseInfo) == MeshResult.Ok)
				{
					collapseToV = num6;
					this.mesh.SetVertex(num6, vNewPos);
					if (this.constraints != null)
					{
						this.constraints.ClearEdgeConstraint(edgeID);
						this.constraints.ClearEdgeConstraint(edgeCollapseInfo.eRemoved0);
						if (edgeCollapseInfo.eRemoved1 != -1)
						{
							this.constraints.ClearEdgeConstraint(edgeCollapseInfo.eRemoved1);
						}
						this.constraints.ClearVertexConstraint(num7);
					}
					this.OnEdgeCollapse(edgeID, num6, num7, edgeCollapseInfo);
					this.DoDebugChecks();
					result = Reducer.ProcessResult.Ok_Collapsed;
				}
			}
			this.end_collapse();
			return result;
		}

		protected void project_vertex(int vID, IProjectionTarget targetIn)
		{
			Vector3d vertex = this.mesh.GetVertex(vID);
			Vector3d vNewPos = targetIn.Project(vertex, vID);
			this.mesh.SetVertex(vID, vNewPos);
		}

		protected Vector3d get_projected_collapse_position(int vid, Vector3d vNewPos)
		{
			if (this.constraints == null)
			{
				return vNewPos;
			}
			VertexConstraint vertexConstraint = this.constraints.GetVertexConstraint(vid);
			if (vertexConstraint.Target != null)
			{
				return vertexConstraint.Target.Project(vNewPos, vid);
			}
			bool @fixed = vertexConstraint.Fixed;
			return vNewPos;
		}

		protected virtual void FullProjectionPass()
		{
			Action<int> body = delegate(int vID)
			{
				if (base.vertex_is_constrained(vID))
				{
					return;
				}
				Vector3d vertex = this.mesh.GetVertex(vID);
				Vector3d vNewPos = this.target.Project(vertex, vID);
				this.mesh.SetVertex(vID, vNewPos);
			};
			gParallel.ForEach<int>(this.project_vertices(), body);
		}

		[Conditional("DEBUG")]
		protected virtual void RuntimeDebugCheck(int eid)
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

		protected virtual void DebugCheckVertexConstraints()
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
				this.COUNT_COLLAPSES = 0;
				this.COUNT_ITERATIONS = 0;
				this.AllOpsW = new Stopwatch();
				this.SetupW = new Stopwatch();
				this.ProjectW = new Stopwatch();
				this.CollapseW = new Stopwatch();
			}
		}

		protected virtual void end_pass()
		{
			if (this.ENABLE_PROFILING)
			{
				Console.WriteLine(string.Format("ReducePass: T {0} V {1} collapses {2}  iterations {3}", new object[]
				{
					this.mesh.TriangleCount,
					this.mesh.VertexCount,
					this.COUNT_COLLAPSES,
					this.COUNT_ITERATIONS
				}));
				Console.WriteLine(string.Format("           Timing1: setup {0} ops {1} project {2}", Util.ToSecMilli(this.SetupW.Elapsed), Util.ToSecMilli(this.AllOpsW.Elapsed), Util.ToSecMilli(this.ProjectW.Elapsed)));
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

		protected virtual void begin_setup()
		{
			if (this.ENABLE_PROFILING)
			{
				this.SetupW.Start();
			}
		}

		protected virtual void end_setup()
		{
			if (this.ENABLE_PROFILING)
			{
				this.SetupW.Stop();
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

		protected IProjectionTarget target;

		public bool MinimizeQuadricPositionError = true;

		public bool PreserveBoundaryShape = true;

		public List<int> DebugEdges = new List<int>();

		public Reducer.TargetProjectionMode ProjectionMode = Reducer.TargetProjectionMode.AfterRefinement;

		public bool ENABLE_PROFILING;

		protected double MinEdgeLength = double.MaxValue;

		protected int TargetCount = int.MaxValue;

		protected Reducer.TargetModes ReduceMode;

		protected QuadricError[] vertQuadrics;

		protected Reducer.QEdge[] EdgeQuadrics;

		protected IndexPriorityQueue EdgeQueue;

		protected bool HaveBoundary;

		protected bool[] IsBoundaryVtxCache;

		private const int nPrime = 31337;

		private int nMaxEdgeID;

		public bool ENABLE_DEBUG_CHECKS;

		private int COUNT_COLLAPSES;

		private int COUNT_ITERATIONS;

		private Stopwatch AllOpsW;

		private Stopwatch SetupW;

		private Stopwatch ProjectW;

		private Stopwatch CollapseW;

		public enum TargetProjectionMode
		{
			NoProjection,
			AfterRefinement,
			Inline
		}

		protected enum TargetModes
		{
			TriangleCount,
			VertexCount,
			MinEdgeLength
		}

		protected struct QEdge
		{
			public QEdge(int edge_id, ref QuadricError qin, ref Vector3d pt)
			{
				this.eid = edge_id;
				this.q = qin;
				this.collapse_pt = pt;
			}

			public int eid;

			public QuadricError q;

			public Vector3d collapse_pt;
		}

		protected enum ProcessResult
		{
			Ok_Collapsed,
			Ignored_CannotCollapse,
			Ignored_EdgeIsFullyConstrained,
			Ignored_EdgeTooLong,
			Ignored_Constrained,
			Ignored_CreatesFlip,
			Failed_OpNotSuccessful,
			Failed_NotAnEdge
		}
	}
}
