using System;

namespace g3
{
	public class ModifyVerticesMeshChange
	{
		public ModifyVerticesMeshChange(DMesh3 mesh, MeshComponents wantComponents = MeshComponents.All)
		{
			this.initialize_buffers(mesh, wantComponents);
		}

		public int AppendNewVertex(DMesh3 mesh, int vid)
		{
			int length = this.ModifiedV.Length;
			this.ModifiedV.Add(vid);
			this.OldPositions.Add(mesh.GetVertex(vid));
			this.NewPositions.Add(this.OldPositions[length]);
			if (this.NewNormals != null)
			{
				this.OldNormals.Add(mesh.GetVertexNormal(vid));
				this.NewNormals.Add(this.OldNormals[length]);
			}
			if (this.NewColors != null)
			{
				this.OldColors.Add(mesh.GetVertexColor(vid));
				this.NewColors.Add(this.OldColors[length]);
			}
			if (this.NewUVs != null)
			{
				this.OldUVs.Add(mesh.GetVertexUV(vid));
				this.NewUVs.Add(this.OldUVs[length]);
			}
			return length;
		}

		public void Apply(DMesh3 mesh)
		{
			int size = this.ModifiedV.size;
			for (int i = 0; i < size; i++)
			{
				int vID = this.ModifiedV[i];
				mesh.SetVertex(vID, this.NewPositions[i]);
				if (this.NewNormals != null)
				{
					mesh.SetVertexNormal(vID, this.NewNormals[i]);
				}
				if (this.NewColors != null)
				{
					mesh.SetVertexColor(vID, this.NewColors[i]);
				}
				if (this.NewUVs != null)
				{
					mesh.SetVertexUV(vID, this.NewUVs[i]);
				}
			}
			if (this.OnApplyF != null)
			{
				this.OnApplyF(this);
			}
		}

		public void Revert(DMesh3 mesh)
		{
			int size = this.ModifiedV.size;
			for (int i = 0; i < size; i++)
			{
				int vID = this.ModifiedV[i];
				mesh.SetVertex(vID, this.OldPositions[i]);
				if (this.NewNormals != null)
				{
					mesh.SetVertexNormal(vID, this.OldNormals[i]);
				}
				if (this.NewColors != null)
				{
					mesh.SetVertexColor(vID, this.OldColors[i]);
				}
				if (this.NewUVs != null)
				{
					mesh.SetVertexUV(vID, this.OldUVs[i]);
				}
			}
			if (this.OnRevertF != null)
			{
				this.OnRevertF(this);
			}
		}

		private void initialize_buffers(DMesh3 mesh, MeshComponents components)
		{
			this.ModifiedV = new DVector<int>();
			this.NewPositions = new DVector<Vector3d>();
			this.OldPositions = new DVector<Vector3d>();
			if (mesh.HasVertexNormals && (components & MeshComponents.VertexNormals) != MeshComponents.None)
			{
				this.NewNormals = new DVector<Vector3f>();
				this.OldNormals = new DVector<Vector3f>();
			}
			if (mesh.HasVertexColors && (components & MeshComponents.VertexColors) != MeshComponents.None)
			{
				this.NewColors = new DVector<Vector3f>();
				this.OldColors = new DVector<Vector3f>();
			}
			if (mesh.HasVertexUVs && (components & MeshComponents.VertexUVs) != MeshComponents.None)
			{
				this.NewUVs = new DVector<Vector2f>();
				this.OldUVs = new DVector<Vector2f>();
			}
		}

		public DVector<int> ModifiedV;

		public DVector<Vector3d> OldPositions;

		public DVector<Vector3d> NewPositions;

		public DVector<Vector3f> OldNormals;

		public DVector<Vector3f> NewNormals;

		public DVector<Vector3f> OldColors;

		public DVector<Vector3f> NewColors;

		public DVector<Vector2f> OldUVs;

		public DVector<Vector2f> NewUVs;

		public Action<ModifyVerticesMeshChange> OnApplyF;

		public Action<ModifyVerticesMeshChange> OnRevertF;
	}
}
