using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using g3;

namespace gs
{
	public class RemesherPro : Remesher
	{
		public RemesherPro(DMesh3 m) : base(m)
		{
		}

		protected IEnumerable<int> EdgesIterator()
		{
			int cur_eid = this.start_edges();
			bool flag = false;
			do
			{
				yield return cur_eid;
				cur_eid = this.next_edge(cur_eid, out flag);
			}
			while (!flag);
			yield break;
		}

		private void queue_one_ring_safe(int vid)
		{
			if (this.mesh.IsVertex(vid))
			{
				bool flag = false;
				this.modified_edges_lock.Enter(ref flag);
				foreach (int item in this.mesh.VtxEdgesItr(vid))
				{
					this.modified_edges.Add(item);
				}
				this.modified_edges_lock.Exit();
			}
		}

		private void queue_one_ring(int vid)
		{
			if (this.mesh.IsVertex(vid))
			{
				foreach (int item in this.mesh.VtxEdgesItr(vid))
				{
					this.modified_edges.Add(item);
				}
			}
		}

		private void queue_edge_safe(int eid)
		{
			bool flag = false;
			this.modified_edges_lock.Enter(ref flag);
			this.modified_edges.Add(eid);
			this.modified_edges_lock.Exit();
		}

		private void queue_edge(int eid)
		{
			this.modified_edges.Add(eid);
		}

		protected override void OnEdgeSplit(int edgeID, int va, int vb, DMesh3.EdgeSplitInfo splitInfo)
		{
			if (this.SplitF != null)
			{
				this.SplitF(edgeID, va, vb, splitInfo.vNew);
			}
		}

		public void FastestRemesh(int nMaxIterations = 25, bool bDoFastSplits = true)
		{
			this.ResetQueue();
			int num = 0;
			if (bDoFastSplits)
			{
				if (this.Cancelled())
				{
					return;
				}
				bool flag = true;
				while (flag)
				{
					double num2 = (double)this.FastSplitIteration();
					if (num++ > nMaxIterations)
					{
						flag = false;
					}
					if (num2 / (double)this.mesh.EdgeCount < 0.01)
					{
						flag = false;
					}
					if (this.Cancelled())
					{
						return;
					}
				}
				this.ResetQueue();
			}
			Remesher.TargetProjectionMode projectionMode = this.ProjectionMode;
			int num3 = 0;
			while (num3 < nMaxIterations - 1 && !this.Cancelled())
			{
				this.ProjectionMode = ((num3 % 2 == 0) ? Remesher.TargetProjectionMode.NoProjection : projectionMode);
				this.RemeshIteration();
				num3++;
			}
			this.ProjectionMode = projectionMode;
			if (this.Cancelled())
			{
				return;
			}
			this.RemeshIteration();
		}

		public void SharpEdgeReprojectionRemesh(int nRemeshIterations, int nTuneIterations, bool bDoFastSplits = true)
		{
			if (base.ProjectionTarget == null || !(base.ProjectionTarget is IOrientedProjectionTarget))
			{
				throw new Exception("RemesherPro.SharpEdgeReprojectionRemesh: cannot call this without a ProjectionTarget that has normals");
			}
			this.ResetQueue();
			int num = 0;
			if (bDoFastSplits)
			{
				if (this.Cancelled())
				{
					return;
				}
				bool flag = true;
				while (flag)
				{
					double num2 = (double)this.FastSplitIteration();
					if (num++ > nRemeshIterations)
					{
						flag = false;
					}
					if (num2 / (double)this.mesh.EdgeCount < 0.01)
					{
						flag = false;
					}
					if (this.Cancelled())
					{
						return;
					}
				}
				this.ResetQueue();
			}
			bool useFaceAlignedProjection = this.UseFaceAlignedProjection;
			this.UseFaceAlignedProjection = true;
			this.FaceProjectionPassesPerIteration = 1;
			double smoothSpeedT = this.SmoothSpeedT;
			int num3 = 0;
			while (num3 < nRemeshIterations && !this.Cancelled())
			{
				this.RemeshIteration();
				if (num3 > nRemeshIterations / 2)
				{
					this.SmoothSpeedT *= 0.8999999761581421;
				}
				num3++;
			}
			int num4 = 0;
			while (num4 < nTuneIterations && !this.Cancelled())
			{
				this.TrackedFaceProjectionPass();
				num4++;
			}
			this.SmoothSpeedT = smoothSpeedT;
			this.UseFaceAlignedProjection = useFaceAlignedProjection;
		}

		public void ResetQueue()
		{
			if (this.modified_edges != null)
			{
				this.modified_edges.Clear();
				this.modified_edges = null;
			}
		}

		public int FastSplitIteration()
		{
			if (this.mesh.TriangleCount == 0)
			{
				return 0;
			}
			this.PushState();
			this.EnableFlips = (this.EnableCollapses = (this.EnableSmoothing = false));
			this.ProjectionMode = Remesher.TargetProjectionMode.NoProjection;
			this.begin_pass();
			this.begin_ops();
			IEnumerable<int> enumerable = this.EdgesIterator();
			if (this.modified_edges == null)
			{
				this.modified_edges = new HashSet<int>();
			}
			else
			{
				this.edges_buffer.Clear();
				this.edges_buffer.AddRange(this.modified_edges);
				enumerable = this.edges_buffer;
				this.modified_edges.Clear();
			}
			int edgeCount = base.Mesh.EdgeCount;
			int num = 0;
			double max_edge_len_sqr = this.MaxEdgeLength * this.MaxEdgeLength;
			this.SplitF = delegate(int edgeID, int a, int b, int vNew)
			{
				Vector3d vertex = this.Mesh.GetVertex(vNew);
				foreach (int num4 in this.Mesh.VtxEdgesItr(vNew))
				{
					Index2i edgeV = this.Mesh.GetEdgeV(num4);
					int vID = (edgeV.a == vNew) ? edgeV.b : edgeV.a;
					if (this.mesh.GetVertex(vID).DistanceSquared(ref vertex) > max_edge_len_sqr)
					{
						this.queue_edge(num4);
					}
				}
			};
			this.ModifiedEdgesLastPass = 0;
			int num2 = 0;
			foreach (int num3 in enumerable)
			{
				if (this.Cancelled())
				{
					goto IL_15E;
				}
				if (this.mesh.IsEdge(num3))
				{
					this.mesh.GetEdgeV(num3);
					this.mesh.GetEdgeOpposingV(num3);
					num2++;
					if (this.ProcessEdge(num3) == Remesher.ProcessResult.Ok_Split)
					{
						this.ModifiedEdgesLastPass++;
						num++;
					}
				}
			}
			this.end_ops();
			IL_15E:
			this.SplitF = null;
			this.PopState();
			this.end_pass();
			return num;
		}

		public virtual void RemeshIteration()
		{
			if (this.mesh.TriangleCount == 0)
			{
				return;
			}
			this.begin_pass();
			this.begin_ops();
			IEnumerable<int> enumerable = this.EdgesIterator();
			if (this.modified_edges == null)
			{
				this.modified_edges = new HashSet<int>();
			}
			else
			{
				this.edges_buffer.Clear();
				this.edges_buffer.AddRange(this.modified_edges);
				enumerable = this.edges_buffer;
				this.modified_edges.Clear();
			}
			int edgeCount = base.Mesh.EdgeCount;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.ModifiedEdgesLastPass = 0;
			int num4 = 0;
			foreach (int num5 in enumerable)
			{
				if (this.Cancelled())
				{
					return;
				}
				if (this.mesh.IsEdge(num5))
				{
					Index2i edgeV = this.mesh.GetEdgeV(num5);
					Index2i edgeOpposingV = this.mesh.GetEdgeOpposingV(num5);
					num4++;
					Remesher.ProcessResult processResult = this.ProcessEdge(num5);
					if (processResult == Remesher.ProcessResult.Ok_Collapsed)
					{
						this.queue_one_ring(edgeV.a);
						this.queue_one_ring(edgeV.b);
						this.queue_one_ring(edgeOpposingV.a);
						this.queue_one_ring(edgeOpposingV.b);
						this.ModifiedEdgesLastPass++;
						num3++;
					}
					else if (processResult == Remesher.ProcessResult.Ok_Split)
					{
						this.queue_one_ring(edgeV.a);
						this.queue_one_ring(edgeV.b);
						this.queue_one_ring(edgeOpposingV.a);
						this.queue_one_ring(edgeOpposingV.b);
						this.ModifiedEdgesLastPass++;
						num2++;
					}
					else if (processResult == Remesher.ProcessResult.Ok_Flipped)
					{
						this.queue_one_ring(edgeV.a);
						this.queue_one_ring(edgeV.b);
						this.queue_one_ring(edgeOpposingV.a);
						this.queue_one_ring(edgeOpposingV.b);
						this.ModifiedEdgesLastPass++;
						num++;
					}
				}
			}
			this.end_ops();
			if (this.Cancelled())
			{
				return;
			}
			this.begin_smooth();
			if (this.EnableSmoothing && this.SmoothSpeedT > 0.0)
			{
				this.TrackedSmoothPass(this.EnableParallelSmooth);
				this.DoDebugChecks();
			}
			this.end_smooth();
			if (this.Cancelled())
			{
				return;
			}
			this.begin_project();
			if (base.ProjectionTarget != null && this.ProjectionMode == Remesher.TargetProjectionMode.AfterRefinement)
			{
				if (this.UseFaceAlignedProjection)
				{
					for (int i = 0; i < this.FaceProjectionPassesPerIteration; i++)
					{
						this.TrackedFaceProjectionPass();
					}
				}
				else
				{
					this.TrackedProjectionPass(this.EnableParallelProjection);
				}
				this.DoDebugChecks();
			}
			this.end_project();
			this.end_pass();
		}

		protected virtual void TrackedSmoothPass(bool bParallel)
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
				Vector3d vertex = this.Mesh.GetVertex(vID);
				bool flag = false;
				Vector3d value = this.ComputeSmoothedVertexPos(vID, smoothFunc, out flag);
				if (flag)
				{
					this.vModifiedV[vID] = true;
					this.vBufferV[vID] = value;
					foreach (int num in this.mesh.VtxEdgesItr(vID))
					{
						Index2i edgeV = this.Mesh.GetEdgeV(num);
						int vID2 = (edgeV.a == vID) ? edgeV.b : edgeV.a;
						Vector3d vertex2 = this.mesh.GetVertex(vID2);
						vertex.Distance(vertex2);
						double num2 = value.Distance(vertex2);
						if (num2 < this.MinEdgeLength || num2 > this.MaxEdgeLength)
						{
							this.queue_edge_safe(num);
						}
					}
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

		protected virtual void TrackedProjectionPass(bool bParallel)
		{
			this.InitializeVertexBufferForPass();
			Action<int> action = delegate(int vID)
			{
				Vector3d vertex = base.Mesh.GetVertex(vID);
				bool flag = false;
				Vector3d vector3d = this.ComputeProjectedVertexPos(vID, out flag);
				if (vertex.EpsilonEqual(vector3d, 9.999999974752427E-07))
				{
					flag = false;
				}
				if (flag)
				{
					this.vModifiedV[vID] = true;
					this.vBufferV[vID] = vector3d;
					foreach (int num in this.mesh.VtxEdgesItr(vID))
					{
						Index2i edgeV = base.Mesh.GetEdgeV(num);
						int vID2 = (edgeV.a == vID) ? edgeV.b : edgeV.a;
						Vector3d vertex2 = this.mesh.GetVertex(vID2);
						vertex.Distance(vertex2);
						double num2 = vector3d.Distance(vertex2);
						if (num2 < this.MinEdgeLength || num2 > this.MaxEdgeLength)
						{
							this.queue_edge_safe(num);
						}
					}
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

		protected virtual Vector3d ComputeProjectedVertexPos(int vID, out bool bModified)
		{
			bModified = false;
			if (base.vertex_is_constrained(vID))
			{
				return base.Mesh.GetVertex(vID);
			}
			if (this.VertexControlF != null && (this.VertexControlF(vID) & Remesher.VertexControl.NoProject) != Remesher.VertexControl.AllowAll)
			{
				return base.Mesh.GetVertex(vID);
			}
			Vector3d vertex = this.mesh.GetVertex(vID);
			Vector3d result = base.ProjectionTarget.Project(vertex, vID);
			bModified = true;
			return result;
		}

		protected virtual void InitializeBuffersForFacePass()
		{
			base.InitializeVertexBufferForPass();
			if (this.vBufferVWeights.size < this.vBufferV.size)
			{
				this.vBufferVWeights.resize(this.vBufferV.size);
			}
			int maxVertexID = this.mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				this.vBufferV[i] = Vector3d.Zero;
				this.vBufferVWeights[i] = 0.0;
			}
		}

		protected virtual void TrackedFaceProjectionPass()
		{
			IOrientedProjectionTarget normalTarget = base.ProjectionTarget as IOrientedProjectionTarget;
			if (normalTarget == null)
			{
				throw new Exception("RemesherPro.TrackedFaceProjectionPass: projection target does not have normals!");
			}
			this.InitializeBuffersForFacePass();
			SpinLock buffer_lock = default(SpinLock);
			Action<int> body = delegate(int tid)
			{
				Vector3d setZ;
				double num;
				Vector3d vector3d;
				this.mesh.GetTriInfo(tid, out setZ, out num, out vector3d);
				Vector3d vector3d2;
				Vector3d v = normalTarget.Project(vector3d, out vector3d2, -1);
				Index3i triangle = this.mesh.GetTriangle(tid);
				Vector3d v2 = this.mesh.GetVertex(triangle.a);
				Vector3d v3 = this.mesh.GetVertex(triangle.b);
				Vector3d v4 = this.mesh.GetVertex(triangle.c);
				Frame3f frame3f = new Frame3f(vector3d, setZ);
				v2 = frame3f.ToFrameP(ref v2);
				v3 = frame3f.ToFrameP(ref v3);
				v4 = frame3f.ToFrameP(ref v4);
				frame3f.AlignAxis(2, (Vector3f)vector3d2);
				frame3f.Origin = (Vector3f)v;
				v2 = frame3f.FromFrameP(ref v2);
				v3 = frame3f.FromFrameP(ref v3);
				v4 = frame3f.FromFrameP(ref v4);
				double num2 = setZ.Dot(vector3d2);
				num2 = MathUtil.Clamp(num2, 0.0, 1.0);
				double num3 = num * (num2 * num2 * num2);
				bool flag = false;
				buffer_lock.Enter(ref flag);
				DVector<Vector3d> vBufferV = this.vBufferV;
				int i = triangle.a;
				vBufferV[i] += num3 * v2;
				DVector<double> dvector = this.vBufferVWeights;
				i = triangle.a;
				dvector[i] += num3;
				vBufferV = this.vBufferV;
				i = triangle.b;
				vBufferV[i] += num3 * v3;
				dvector = this.vBufferVWeights;
				i = triangle.b;
				dvector[i] += num3;
				vBufferV = this.vBufferV;
				i = triangle.c;
				vBufferV[i] += num3 * v4;
				dvector = this.vBufferVWeights;
				i = triangle.c;
				dvector[i] += num3;
				buffer_lock.Exit();
			};
			gParallel.ForEach<int>(this.mesh.TriangleIndices(), body);
			gParallel.ForEach<int>(this.mesh.VertexIndices(), delegate(int vID)
			{
				this.vModifiedV[vID] = false;
				if (this.vBufferVWeights[vID] < 1E-08)
				{
					return;
				}
				if (this.vertex_is_constrained(vID))
				{
					return;
				}
				if (this.VertexControlF != null && (this.VertexControlF(vID) & Remesher.VertexControl.NoProject) != Remesher.VertexControl.AllowAll)
				{
					return;
				}
				Vector3d vertex = this.mesh.GetVertex(vID);
				Vector3d vector3d = this.vBufferV[vID] / this.vBufferVWeights[vID];
				if (vertex.EpsilonEqual(vector3d, 9.999999974752427E-07))
				{
					return;
				}
				this.vModifiedV[vID] = true;
				this.vBufferV[vID] = vector3d;
				foreach (int num in this.mesh.VtxEdgesItr(vID))
				{
					Index2i edgeV = this.Mesh.GetEdgeV(num);
					int vID2 = (edgeV.a == vID) ? edgeV.b : edgeV.a;
					Vector3d vertex2 = this.mesh.GetVertex(vID2);
					vertex.Distance(vertex2);
					double num2 = vector3d.Distance(vertex2);
					if (num2 < this.MinEdgeLength || num2 > this.MaxEdgeLength)
					{
						this.queue_edge_safe(num);
					}
				}
			});
			this.ApplyVertexBuffer(true);
		}

		public void PushState()
		{
			RemesherPro.SettingState item = new RemesherPro.SettingState
			{
				EnableFlips = this.EnableFlips,
				EnableCollapses = this.EnableCollapses,
				EnableSplits = this.EnableSplits,
				EnableSmoothing = this.EnableSmoothing,
				MinEdgeLength = this.MinEdgeLength,
				MaxEdgeLength = this.MaxEdgeLength,
				SmoothSpeedT = this.SmoothSpeedT,
				SmoothType = this.SmoothType,
				ProjectionMode = this.ProjectionMode
			};
			this.stateStack.Add(item);
		}

		public void PopState()
		{
			RemesherPro.SettingState settingState = this.stateStack.Last<RemesherPro.SettingState>();
			this.stateStack.RemoveAt(this.stateStack.Count - 1);
			this.EnableFlips = settingState.EnableFlips;
			this.EnableCollapses = settingState.EnableCollapses;
			this.EnableSplits = settingState.EnableSplits;
			this.EnableSmoothing = settingState.EnableSmoothing;
			this.MinEdgeLength = settingState.MinEdgeLength;
			this.MaxEdgeLength = settingState.MaxEdgeLength;
			this.SmoothSpeedT = settingState.SmoothSpeedT;
			this.SmoothType = settingState.SmoothType;
			this.ProjectionMode = settingState.ProjectionMode;
		}

		public bool UseFaceAlignedProjection;

		public int FaceProjectionPassesPerIteration = 1;

		private HashSet<int> modified_edges;

		private SpinLock modified_edges_lock;

		private Action<int, int, int, int> SplitF;

		private List<int> edges_buffer = new List<int>();

		protected DVector<double> vBufferVWeights = new DVector<double>();

		private List<RemesherPro.SettingState> stateStack = new List<RemesherPro.SettingState>();

		private struct SettingState
		{
			public bool EnableFlips;

			public bool EnableCollapses;

			public bool EnableSplits;

			public bool EnableSmoothing;

			public double MinEdgeLength;

			public double MaxEdgeLength;

			public double SmoothSpeedT;

			public Remesher.SmoothTypes SmoothType;

			public Remesher.TargetProjectionMode ProjectionMode;
		}
	}
}
