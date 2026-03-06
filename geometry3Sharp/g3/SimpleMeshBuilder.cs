using System;
using System.Collections.Generic;

namespace g3
{
	public class SimpleMeshBuilder : IMeshBuilder
	{
		public SimpleMeshBuilder()
		{
			this.Meshes = new List<SimpleMesh>();
			this.Materials = new List<GenericMaterial>();
			this.MaterialAssignment = new List<int>();
			this.nActiveMesh = -1;
		}

		public int AppendNewMesh(bool bHaveVtxNormals, bool bHaveVtxColors, bool bHaveVtxUVs, bool bHaveFaceGroups)
		{
			int count = this.Meshes.Count;
			SimpleMesh simpleMesh = new SimpleMesh();
			simpleMesh.Initialize(bHaveVtxNormals, bHaveVtxColors, bHaveVtxUVs, bHaveFaceGroups);
			this.Meshes.Add(simpleMesh);
			this.MaterialAssignment.Add(-1);
			this.nActiveMesh = count;
			return count;
		}

		public int AppendNewMesh(DMesh3 existingMesh)
		{
			int count = this.Meshes.Count;
			SimpleMesh item = new SimpleMesh(existingMesh);
			this.Meshes.Add(item);
			this.MaterialAssignment.Add(-1);
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
			return this.Meshes[this.nActiveMesh].AppendTriangle(i, j, k, -1);
		}

		public int AppendTriangle(int i, int j, int k, int g)
		{
			return this.Meshes[this.nActiveMesh].AppendTriangle(i, j, k, g);
		}

		public int AppendVertex(double x, double y, double z)
		{
			return this.Meshes[this.nActiveMesh].AppendVertex(x, y, z);
		}

		public int AppendVertex(NewVertexInfo info)
		{
			return this.Meshes[this.nActiveMesh].AppendVertex(info);
		}

		public bool SupportsMetaData
		{
			get
			{
				return false;
			}
		}

		public void AppendMetaData(string identifier, object data)
		{
			throw new NotImplementedException("SimpleMeshBuilder: metadata not supported");
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

		public void SetVertexUV(int vID, Vector2f UV)
		{
			this.Meshes[this.nActiveMesh].SetVertexUV(vID, UV);
		}

		public List<SimpleMesh> Meshes;

		public List<GenericMaterial> Materials;

		public List<int> MaterialAssignment;

		private int nActiveMesh;
	}
}
