using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using g3;

namespace gs
{
	public class RemoveOccludedTriangles
	{
		protected virtual bool Cancelled()
		{
			return this.Progress != null && this.Progress.Cancelled();
		}

		public RemoveOccludedTriangles(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public RemoveOccludedTriangles(DMesh3 mesh, DMeshAABBTree3 spatial)
		{
			this.Mesh = mesh;
			this.Spatial = spatial;
		}

		public virtual bool Apply()
		{
			DMesh3 dmesh = this.Mesh;
			if (this.InsideMode == RemoveOccludedTriangles.CalculationMode.RayParity)
			{
				MeshBoundaryLoops meshBoundaryLoops = new MeshBoundaryLoops(dmesh, true);
				if (meshBoundaryLoops.Count > 0)
				{
					dmesh = new DMesh3(this.Mesh, false, true, true, true);
					foreach (EdgeLoop loop in meshBoundaryLoops)
					{
						if (this.Cancelled())
						{
							return false;
						}
						new SimpleHoleFiller(dmesh, loop).Fill(-1);
					}
				}
			}
			DMeshAABBTree3 spatial = (this.Spatial != null && dmesh == this.Mesh) ? this.Spatial : new DMeshAABBTree3(dmesh, true);
			if (this.InsideMode == RemoveOccludedTriangles.CalculationMode.AnalyticWindingNumber)
			{
				spatial.WindingNumber(Vector3d.Zero);
			}
			else if (this.InsideMode == RemoveOccludedTriangles.CalculationMode.FastWindingNumber)
			{
				spatial.FastWindingNumber(Vector3d.Zero);
			}
			if (this.Cancelled())
			{
				return false;
			}
			List<Vector3d> ray_dirs = null;
			int NR = 0;
			if (this.InsideMode == RemoveOccludedTriangles.CalculationMode.SimpleOcclusionTest)
			{
				ray_dirs = new List<Vector3d>();
				ray_dirs.Add(Vector3d.AxisX);
				ray_dirs.Add(-Vector3d.AxisX);
				ray_dirs.Add(Vector3d.AxisY);
				ray_dirs.Add(-Vector3d.AxisY);
				ray_dirs.Add(Vector3d.AxisZ);
				ray_dirs.Add(-Vector3d.AxisZ);
				NR = ray_dirs.Count;
			}
			Func<Vector3d, bool> isOccludedF = delegate(Vector3d pt)
			{
				if (this.InsideMode == RemoveOccludedTriangles.CalculationMode.RayParity)
				{
					return spatial.IsInside(pt);
				}
				if (this.InsideMode == RemoveOccludedTriangles.CalculationMode.AnalyticWindingNumber)
				{
					return spatial.WindingNumber(pt) > this.WindingIsoValue;
				}
				if (this.InsideMode == RemoveOccludedTriangles.CalculationMode.FastWindingNumber)
				{
					return spatial.FastWindingNumber(pt) > this.WindingIsoValue;
				}
				for (int i = 0; i < NR; i++)
				{
					if (spatial.FindNearestHitTriangle(new Ray3d(pt, ray_dirs[i], false), 1.7976931348623157E+308) == -1)
					{
						return false;
					}
				}
				return true;
			};
			bool cancel = false;
			BitArray vertices = null;
			if (this.PerVertex)
			{
				vertices = new BitArray(this.Mesh.MaxVertexID);
				MeshNormals normals = null;
				if (!this.Mesh.HasVertexNormals)
				{
					normals = new MeshNormals(this.Mesh, MeshNormals.NormalsTypes.Vertex_OneRingFaceAverage_AreaWeighted);
					normals.Compute();
				}
				gParallel.ForEach<int>(this.Mesh.VertexIndices(), delegate(int vid)
				{
					if (cancel)
					{
						return;
					}
					if (vid % 10 == 0)
					{
						cancel = this.Cancelled();
					}
					Vector3d vector3d = this.Mesh.GetVertex(vid);
					Vector3d v = (normals == null) ? this.Mesh.GetVertexNormal(vid) : normals[vid];
					vector3d += v * this.NormalOffset;
					vertices[vid] = isOccludedF(vector3d);
				});
			}
			if (this.Cancelled())
			{
				return false;
			}
			this.RemovedT = new List<int>();
			SpinLock removeLock = default(SpinLock);
			gParallel.ForEach<int>(this.Mesh.TriangleIndices(), delegate(int tid)
			{
				if (cancel)
				{
					return;
				}
				if (tid % 10 == 0)
				{
					cancel = this.Cancelled();
				}
				bool flag2;
				if (this.PerVertex)
				{
					Index3i triangle = this.Mesh.GetTriangle(tid);
					flag2 = (vertices[triangle.a] || vertices[triangle.b] || vertices[triangle.c]);
				}
				else
				{
					Vector3d vector3d = this.Mesh.GetTriCentroid(tid);
					Vector3d triNormal = this.Mesh.GetTriNormal(tid);
					vector3d += triNormal * this.NormalOffset;
					flag2 = isOccludedF(vector3d);
				}
				if (flag2)
				{
					bool flag3 = false;
					removeLock.Enter(ref flag3);
					this.RemovedT.Add(tid);
					removeLock.Exit();
				}
			});
			if (this.Cancelled())
			{
				return false;
			}
			if (this.RemovedT.Count > 0)
			{
				bool flag = new MeshEditor(this.Mesh).RemoveTriangles(this.RemovedT, true);
				this.RemoveFailed = !flag;
			}
			return true;
		}

		public DMesh3 Mesh;

		public DMeshAABBTree3 Spatial;

		public List<int> RemovedT;

		public bool RemoveFailed;

		public bool PerVertex;

		public double NormalOffset = 1E-08;

		public double WindingIsoValue = 0.5;

		public RemoveOccludedTriangles.CalculationMode InsideMode;

		public ProgressCancel Progress;

		public enum CalculationMode
		{
			RayParity,
			AnalyticWindingNumber,
			FastWindingNumber,
			SimpleOcclusionTest
		}
	}
}
