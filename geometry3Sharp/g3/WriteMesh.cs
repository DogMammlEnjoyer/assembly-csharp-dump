using System;
using System.Collections.Generic;

namespace g3
{
	public struct WriteMesh
	{
		public WriteMesh(IMesh mesh, string name = "")
		{
			this.Mesh = mesh;
			this.Name = name;
			this.UVs = null;
			this.Materials = null;
			this.TriToMaterialMap = null;
		}

		public IMesh Mesh;

		public string Name;

		public List<GenericMaterial> Materials;

		public IIndexMap TriToMaterialMap;

		public DenseUVMesh UVs;
	}
}
