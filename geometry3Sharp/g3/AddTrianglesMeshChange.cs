using System;
using System.Collections.Generic;

namespace g3
{
	public class AddTrianglesMeshChange
	{
		public void InitializeFromExisting(DMesh3 mesh, IEnumerable<int> added_v, IEnumerable<int> added_t)
		{
			this.initialize_buffers(mesh);
			bool hasTriangleGroups = mesh.HasTriangleGroups;
			if (added_v != null)
			{
				foreach (int vid in added_v)
				{
					this.append_vertex(mesh, vid);
				}
			}
			foreach (int num in added_t)
			{
				Index3i triangle = mesh.GetTriangle(num);
				Index4i value = new Index4i(triangle.a, triangle.b, triangle.c, hasTriangleGroups ? mesh.GetTriangleGroup(num) : -1);
				this.AddedT.Add(num);
				this.Triangles.Add(value);
			}
		}

		public void Apply(DMesh3 mesh)
		{
			int size = this.AddedV.size;
			if (size > 0)
			{
				NewVertexInfo newVertexInfo = new NewVertexInfo(this.Positions[0]);
				mesh.BeginUnsafeVerticesInsert();
				for (int i = 0; i < size; i++)
				{
					int vid = this.AddedV[i];
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
						throw new Exception("AddTrianglesMeshChange.Revert: error in InsertVertex(" + vid.ToString() + "): " + meshResult.ToString());
					}
				}
				mesh.EndUnsafeVerticesInsert();
			}
			int size2 = this.AddedT.size;
			if (size2 > 0)
			{
				mesh.BeginUnsafeTrianglesInsert();
				for (int j = 0; j < size2; j++)
				{
					int tid = this.AddedT[j];
					Index4i index4i = this.Triangles[j];
					Index3i tv = new Index3i(index4i.a, index4i.b, index4i.c);
					MeshResult meshResult2 = mesh.InsertTriangle(tid, tv, index4i.d, true);
					if (meshResult2 != MeshResult.Ok)
					{
						throw new Exception("AddTrianglesMeshChange.Revert: error in InsertTriangle(" + tid.ToString() + "): " + meshResult2.ToString());
					}
				}
				mesh.EndUnsafeTrianglesInsert();
			}
			if (this.OnApplyF != null)
			{
				this.OnApplyF(this.AddedV, this.AddedT);
			}
		}

		public void Revert(DMesh3 mesh)
		{
			int size = this.AddedT.size;
			for (int i = 0; i < size; i++)
			{
				int num = this.AddedT[i];
				MeshResult meshResult = mesh.RemoveTriangle(this.AddedT[i], true, false);
				if (meshResult != MeshResult.Ok)
				{
					throw new Exception("AddTrianglesMeshChange.Apply: error in RemoveTriangle(" + num.ToString() + "): " + meshResult.ToString());
				}
			}
			if (this.OnRevertF != null)
			{
				this.OnRevertF(this.AddedV, this.AddedT);
			}
		}

		private void append_vertex(DMesh3 mesh, int vid)
		{
			this.AddedV.Add(vid);
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
		}

		private void initialize_buffers(DMesh3 mesh)
		{
			this.AddedV = new DVector<int>();
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
			this.AddedT = new DVector<int>();
			this.Triangles = new DVector<Index4i>();
		}

		protected DVector<int> AddedV;

		protected DVector<Vector3d> Positions;

		protected DVector<Vector3f> Normals;

		protected DVector<Vector3f> Colors;

		protected DVector<Vector2f> UVs;

		protected DVector<int> AddedT;

		protected DVector<Index4i> Triangles;

		public Action<IEnumerable<int>, IEnumerable<int>> OnApplyF;

		public Action<IEnumerable<int>, IEnumerable<int>> OnRevertF;
	}
}
