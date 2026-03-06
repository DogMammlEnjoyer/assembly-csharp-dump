using System;

namespace g3
{
	public class SetVerticesMeshChange
	{
		public void Apply(DMesh3 mesh)
		{
			if (this.NewPositions != null)
			{
				mesh.VerticesBuffer.copy(this.NewPositions);
			}
			if (mesh.HasVertexNormals && this.NewNormals != null)
			{
				mesh.NormalsBuffer.copy(this.NewNormals);
			}
			if (mesh.HasVertexColors && this.NewColors != null)
			{
				mesh.ColorsBuffer.copy(this.NewColors);
			}
			if (mesh.HasVertexUVs && this.NewUVs != null)
			{
				mesh.UVBuffer.copy(this.NewUVs);
			}
			if (this.OnApplyF != null)
			{
				this.OnApplyF(this);
			}
		}

		public void Revert(DMesh3 mesh)
		{
			if (this.OldPositions != null)
			{
				mesh.VerticesBuffer.copy(this.OldPositions);
			}
			if (mesh.HasVertexNormals && this.OldNormals != null)
			{
				mesh.NormalsBuffer.copy(this.OldNormals);
			}
			if (mesh.HasVertexColors && this.OldColors != null)
			{
				mesh.ColorsBuffer.copy(this.OldColors);
			}
			if (mesh.HasVertexUVs && this.OldUVs != null)
			{
				mesh.UVBuffer.copy(this.OldUVs);
			}
			if (this.OnRevertF != null)
			{
				this.OnRevertF(this);
			}
		}

		public DVector<double> OldPositions;

		public DVector<double> NewPositions;

		public DVector<float> OldNormals;

		public DVector<float> NewNormals;

		public DVector<float> OldColors;

		public DVector<float> NewColors;

		public DVector<float> OldUVs;

		public DVector<float> NewUVs;

		public Action<SetVerticesMeshChange> OnApplyF;

		public Action<SetVerticesMeshChange> OnRevertF;
	}
}
