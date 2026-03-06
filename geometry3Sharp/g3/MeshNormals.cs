using System;
using System.Threading;

namespace g3
{
	public class MeshNormals
	{
		public MeshNormals(IMesh mesh, MeshNormals.NormalsTypes eType = MeshNormals.NormalsTypes.Vertex_OneRingFaceAverage_AreaWeighted)
		{
			this.Mesh = mesh;
			this.NormalType = eType;
			this.Normals = new DVector<Vector3d>();
			this.VertexF = new Func<int, Vector3d>(this.Mesh.GetVertex);
		}

		public void Compute()
		{
			this.Compute_FaceAvg_AreaWeighted();
		}

		public Vector3d this[int vid]
		{
			get
			{
				return this.Normals[vid];
			}
		}

		public void CopyTo(DMesh3 SetMesh)
		{
			if (SetMesh.MaxVertexID < this.Mesh.MaxVertexID)
			{
				throw new Exception("MeshNormals.Set: SetMesh does not have enough vertices!");
			}
			if (!SetMesh.HasVertexNormals)
			{
				SetMesh.EnableVertexNormals(Vector3f.AxisY);
			}
			int maxVertexID = this.Mesh.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (this.Mesh.IsVertex(i) && SetMesh.IsVertex(i))
				{
					SetMesh.SetVertexNormal(i, (Vector3f)this.Normals[i]);
				}
			}
		}

		private void Compute_FaceAvg_AreaWeighted()
		{
			int maxVertexID = this.Mesh.MaxVertexID;
			if (maxVertexID != this.Normals.size)
			{
				this.Normals.resize(maxVertexID);
			}
			for (int i = 0; i < maxVertexID; i++)
			{
				this.Normals[i] = Vector3d.Zero;
			}
			SpinLock Normals_lock = default(SpinLock);
			gParallel.ForEach<int>(this.Mesh.TriangleIndices(), delegate(int ti)
			{
				Index3i triangle = this.Mesh.GetTriangle(ti);
				Vector3d vertex = this.Mesh.GetVertex(triangle.a);
				Vector3d vertex2 = this.Mesh.GetVertex(triangle.b);
				Vector3d vertex3 = this.Mesh.GetVertex(triangle.c);
				Vector3d v = MathUtil.Normal(ref vertex, ref vertex2, ref vertex3);
				double f = MathUtil.Area(ref vertex, ref vertex2, ref vertex3);
				bool flag = false;
				Normals_lock.Enter(ref flag);
				DVector<Vector3d> normals = this.Normals;
				int i2 = triangle.a;
				normals[i2] += f * v;
				normals = this.Normals;
				i2 = triangle.b;
				normals[i2] += f * v;
				normals = this.Normals;
				i2 = triangle.c;
				normals[i2] += f * v;
				Normals_lock.Exit();
			});
			gParallel.BlockStartEnd(0, maxVertexID - 1, delegate(int vi_start, int vi_end)
			{
				for (int j = vi_start; j <= vi_end; j++)
				{
					if (this.Normals[j].LengthSquared > 9.999999974752427E-07)
					{
						this.Normals[j] = this.Normals[j].Normalized;
					}
				}
			}, -1, false);
		}

		public static void QuickCompute(DMesh3 mesh)
		{
			MeshNormals meshNormals = new MeshNormals(mesh, MeshNormals.NormalsTypes.Vertex_OneRingFaceAverage_AreaWeighted);
			meshNormals.Compute();
			meshNormals.CopyTo(mesh);
		}

		public static Vector3d QuickCompute(DMesh3 mesh, int vid, MeshNormals.NormalsTypes type = MeshNormals.NormalsTypes.Vertex_OneRingFaceAverage_AreaWeighted)
		{
			Vector3d v = Vector3d.Zero;
			foreach (int tID in mesh.VtxTrianglesItr(vid))
			{
				Vector3d v2;
				double f;
				Vector3d vector3d;
				mesh.GetTriInfo(tID, out v2, out f, out vector3d);
				v += f * v2;
			}
			return v.Normalized;
		}

		public IMesh Mesh;

		public DVector<Vector3d> Normals;

		public Func<int, Vector3d> VertexF;

		public MeshNormals.NormalsTypes NormalType;

		public enum NormalsTypes
		{
			Vertex_OneRingFaceAverage_AreaWeighted
		}
	}
}
