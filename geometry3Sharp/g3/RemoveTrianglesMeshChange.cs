using System;
using System.Collections.Generic;

namespace g3
{
	public class RemoveTrianglesMeshChange
	{
		public void InitializeFromApply(DMesh3 mesh, IEnumerable<int> triangles)
		{
			this.initialize_buffers(mesh);
			bool hasTriangleGroups = mesh.HasTriangleGroups;
			foreach (int num in triangles)
			{
				if (mesh.IsTriangle(num))
				{
					Index3i triangle = mesh.GetTriangle(num);
					this.save_vertex(mesh, triangle.a, false);
					this.save_vertex(mesh, triangle.b, false);
					this.save_vertex(mesh, triangle.c, false);
					Index4i value = new Index4i(triangle.a, triangle.b, triangle.c, hasTriangleGroups ? mesh.GetTriangleGroup(num) : -1);
					this.RemovedT.Add(num);
					this.Triangles.Add(value);
					MeshResult meshResult = mesh.RemoveTriangle(num, true, false);
					if (meshResult != MeshResult.Ok)
					{
						throw new Exception("RemoveTrianglesMeshChange.Initialize: exception in RemoveTriangle(" + num.ToString() + "): " + meshResult.ToString());
					}
				}
			}
		}

		public void InitializeFromExisting(DMesh3 mesh, IEnumerable<int> remove_t)
		{
			this.initialize_buffers(mesh);
			bool hasTriangleGroups = mesh.HasTriangleGroups;
			HashSet<int> hashSet = new HashSet<int>(remove_t);
			HashSet<int> hashSet2 = new HashSet<int>();
			IndexUtil.TrianglesToVertices(mesh, remove_t, hashSet2);
			List<int> list = new List<int>();
			foreach (int num in hashSet2)
			{
				bool flag = true;
				foreach (int item in mesh.VtxTrianglesItr(num))
				{
					if (!hashSet.Contains(item))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list.Add(num);
				}
			}
			foreach (int vid in list)
			{
				this.save_vertex(mesh, vid, true);
			}
			foreach (int num2 in remove_t)
			{
				Index3i triangle = mesh.GetTriangle(num2);
				Index4i value = new Index4i(triangle.a, triangle.b, triangle.c, hasTriangleGroups ? mesh.GetTriangleGroup(num2) : -1);
				this.RemovedT.Add(num2);
				this.Triangles.Add(value);
			}
		}

		public void Apply(DMesh3 mesh)
		{
			int size = this.RemovedT.size;
			for (int i = 0; i < size; i++)
			{
				int num = this.RemovedT[i];
				MeshResult meshResult = mesh.RemoveTriangle(this.RemovedT[i], true, false);
				if (meshResult != MeshResult.Ok)
				{
					throw new Exception("RemoveTrianglesMeshChange.Apply: error in RemoveTriangle(" + num.ToString() + "): " + meshResult.ToString());
				}
			}
			if (this.OnApplyF != null)
			{
				this.OnApplyF(this.RemovedV, this.RemovedT);
			}
		}

		public void Revert(DMesh3 mesh)
		{
			int size = this.RemovedV.size;
			if (size > 0)
			{
				NewVertexInfo newVertexInfo = new NewVertexInfo(this.Positions[0]);
				mesh.BeginUnsafeVerticesInsert();
				for (int i = 0; i < size; i++)
				{
					int vid = this.RemovedV[i];
					newVertexInfo.v = this.Positions[i];
					if (this.Normals != null)
					{
						newVertexInfo.bHaveN = true;
						newVertexInfo.n = this.Normals[i];
					}
					if (this.Colors != null)
					{
						newVertexInfo.bHaveC = true;
						newVertexInfo.c = this.Colors[i];
					}
					if (this.UVs != null)
					{
						newVertexInfo.bHaveUV = true;
						newVertexInfo.uv = this.UVs[i];
					}
					MeshResult meshResult = mesh.InsertVertex(vid, ref newVertexInfo, true);
					if (meshResult != MeshResult.Ok)
					{
						throw new Exception("RemoveTrianglesMeshChange.Revert: error in InsertVertex(" + vid.ToString() + "): " + meshResult.ToString());
					}
				}
				mesh.EndUnsafeVerticesInsert();
			}
			int size2 = this.RemovedT.size;
			if (size2 > 0)
			{
				mesh.BeginUnsafeTrianglesInsert();
				for (int j = 0; j < size2; j++)
				{
					int tid = this.RemovedT[j];
					Index4i index4i = this.Triangles[j];
					Index3i tv = new Index3i(index4i.a, index4i.b, index4i.c);
					MeshResult meshResult2 = mesh.InsertTriangle(tid, tv, index4i.d, true);
					if (meshResult2 != MeshResult.Ok)
					{
						throw new Exception("RemoveTrianglesMeshChange.Revert: error in InsertTriangle(" + tid.ToString() + "): " + meshResult2.ToString());
					}
				}
				mesh.EndUnsafeTrianglesInsert();
			}
			if (this.OnRevertF != null)
			{
				this.OnRevertF(this.RemovedV, this.RemovedT);
			}
		}

		private bool save_vertex(DMesh3 mesh, int vid, bool force = false)
		{
			if (force || mesh.VerticesRefCounts.refCount(vid) == 2)
			{
				this.RemovedV.Add(vid);
				this.Positions.Add(mesh.GetVertex(vid));
				if (this.Normals != null)
				{
					this.Normals.Add(mesh.GetVertexNormal(vid));
				}
				if (this.Colors != null)
				{
					this.Colors.Add(mesh.GetVertexColor(vid));
				}
				if (this.UVs != null)
				{
					this.UVs.Add(mesh.GetVertexUV(vid));
				}
				return false;
			}
			return true;
		}

		private void initialize_buffers(DMesh3 mesh)
		{
			this.RemovedV = new DVector<int>();
			this.Positions = new DVector<Vector3d>();
			if (mesh.HasVertexNormals)
			{
				this.Normals = new DVector<Vector3f>();
			}
			if (mesh.HasVertexColors)
			{
				this.Colors = new DVector<Vector3f>();
			}
			if (mesh.HasVertexUVs)
			{
				this.UVs = new DVector<Vector2f>();
			}
			this.RemovedT = new DVector<int>();
			this.Triangles = new DVector<Index4i>();
		}

		protected DVector<int> RemovedV;

		protected DVector<Vector3d> Positions;

		protected DVector<Vector3f> Normals;

		protected DVector<Vector3f> Colors;

		protected DVector<Vector2f> UVs;

		protected DVector<int> RemovedT;

		protected DVector<Index4i> Triangles;

		public Action<IEnumerable<int>, IEnumerable<int>> OnApplyF;

		public Action<IEnumerable<int>, IEnumerable<int>> OnRevertF;
	}
}
