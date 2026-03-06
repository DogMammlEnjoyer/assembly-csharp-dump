using System;
using System.Collections.Generic;

namespace g3
{
	public class DMesh3Builder : IMeshBuilder
	{
		public DMesh3Builder()
		{
			this.Meshes = new List<DMesh3>();
			this.Materials = new List<GenericMaterial>();
			this.MaterialAssignment = new List<int>();
			this.Metadata = new List<Dictionary<string, object>>();
			this.nActiveMesh = -1;
		}

		public int AppendNewMesh(bool bHaveVtxNormals, bool bHaveVtxColors, bool bHaveVtxUVs, bool bHaveFaceGroups)
		{
			int count = this.Meshes.Count;
			DMesh3 item = new DMesh3(bHaveVtxNormals, bHaveVtxColors, bHaveVtxUVs, bHaveFaceGroups);
			this.Meshes.Add(item);
			this.MaterialAssignment.Add(-1);
			this.Metadata.Add(new Dictionary<string, object>());
			this.nActiveMesh = count;
			return count;
		}

		public int AppendNewMesh(DMesh3 existingMesh)
		{
			int count = this.Meshes.Count;
			this.Meshes.Add(existingMesh);
			this.MaterialAssignment.Add(-1);
			this.Metadata.Add(new Dictionary<string, object>());
			this.nActiveMesh = count;
			return count;
		}

		public void SetActiveMesh(int id)
		{
			if (id >= 0 && id < this.Meshes.Count)
			{
				this.nActiveMesh = id;
				return;
			}
			throw new ArgumentOutOfRangeException("active mesh id is out of range");
		}

		public int AppendTriangle(int i, int j, int k)
		{
			return this.AppendTriangle(i, j, k, -1);
		}

		public int AppendTriangle(int i, int j, int k, int g)
		{
			int num = this.Meshes[this.nActiveMesh].FindTriangle(i, j, k);
			if (num != -1)
			{
				if (this.DuplicateTriBehavior == DMesh3Builder.AddTriangleFailBehaviors.DuplicateAllVertices)
				{
					return this.append_duplicate_triangle(i, j, k, g);
				}
				return num;
			}
			else
			{
				int num2 = this.Meshes[this.nActiveMesh].AppendTriangle(i, j, k, g);
				if (num2 != -2)
				{
					return num2;
				}
				if (this.NonManifoldTriBehavior == DMesh3Builder.AddTriangleFailBehaviors.DuplicateAllVertices)
				{
					return this.append_duplicate_triangle(i, j, k, g);
				}
				return -2;
			}
		}

		private int append_duplicate_triangle(int i, int j, int k, int g)
		{
			NewVertexInfo info = default(NewVertexInfo);
			this.Meshes[this.nActiveMesh].GetVertex(i, ref info, true, true, true);
			int v = this.Meshes[this.nActiveMesh].AppendVertex(info);
			this.Meshes[this.nActiveMesh].GetVertex(j, ref info, true, true, true);
			int v2 = this.Meshes[this.nActiveMesh].AppendVertex(info);
			this.Meshes[this.nActiveMesh].GetVertex(k, ref info, true, true, true);
			int v3 = this.Meshes[this.nActiveMesh].AppendVertex(info);
			return this.Meshes[this.nActiveMesh].AppendTriangle(v, v2, v3, g);
		}

		public int AppendVertex(double x, double y, double z)
		{
			return this.Meshes[this.nActiveMesh].AppendVertex(new Vector3d(x, y, z));
		}

		public int AppendVertex(NewVertexInfo info)
		{
			return this.Meshes[this.nActiveMesh].AppendVertex(info);
		}

		public bool SupportsMetaData
		{
			get
			{
				return true;
			}
		}

		public void AppendMetaData(string identifier, object data)
		{
			this.Metadata[this.nActiveMesh].Add(identifier, data);
		}

		public void SetVertexUV(int vID, Vector2f UV)
		{
			this.Meshes[this.nActiveMesh].SetVertexUV(vID, UV);
		}

		public int BuildMaterial(GenericMaterial m)
		{
			int count = this.Materials.Count;
			this.Materials.Add(m);
			return count;
		}

		public void AssignMaterial(int materialID, int meshID)
		{
			if (meshID >= this.MaterialAssignment.Count || materialID >= this.Materials.Count)
			{
				throw new ArgumentOutOfRangeException("[SimpleMeshBuilder::AssignMaterial] meshID or materialID are out-of-range");
			}
			this.MaterialAssignment[meshID] = materialID;
		}

		public static DMesh3 Build<VType, TType, NType>(IEnumerable<VType> Vertices, IEnumerable<TType> Triangles, IEnumerable<NType> Normals = null, IEnumerable<int> TriGroups = null)
		{
			DMesh3 dmesh = new DMesh3(Normals != null, false, false, TriGroups != null);
			Vector3d[] array = BufferUtil.ToVector3d<VType>(Vertices);
			for (int i = 0; i < array.Length; i++)
			{
				dmesh.AppendVertex(array[i]);
			}
			if (Normals != null)
			{
				Vector3f[] array2 = BufferUtil.ToVector3f<NType>(Normals);
				if (array2.Length != array.Length)
				{
					throw new Exception("DMesh3Builder.Build: incorrect number of normals provided");
				}
				for (int j = 0; j < array2.Length; j++)
				{
					dmesh.SetVertexNormal(j, array2[j]);
				}
			}
			Index3i[] array3 = BufferUtil.ToIndex3i<TType>(Triangles);
			for (int k = 0; k < array3.Length; k++)
			{
				dmesh.AppendTriangle(array3[k], -1);
			}
			if (TriGroups != null)
			{
				List<int> list = new List<int>(TriGroups);
				if (list.Count != array3.Length)
				{
					throw new Exception("DMesh3Builder.Build: incorect number of triangle groups");
				}
				for (int l = 0; l < array3.Length; l++)
				{
					dmesh.SetTriangleGroup(l, list[l]);
				}
			}
			return dmesh;
		}

		public DMesh3Builder.AddTriangleFailBehaviors NonManifoldTriBehavior = DMesh3Builder.AddTriangleFailBehaviors.DuplicateAllVertices;

		public DMesh3Builder.AddTriangleFailBehaviors DuplicateTriBehavior;

		public List<DMesh3> Meshes;

		public List<GenericMaterial> Materials;

		public List<int> MaterialAssignment;

		public List<Dictionary<string, object>> Metadata;

		private int nActiveMesh;

		public enum AddTriangleFailBehaviors
		{
			DiscardTriangle,
			DuplicateAllVertices
		}
	}
}
