using System;

namespace g3
{
	public class SimpleHoleFiller
	{
		public SimpleHoleFiller(DMesh3 mesh, EdgeLoop loop)
		{
			this.Mesh = mesh;
			this.Loop = loop;
			this.NewVertex = -1;
			this.NewTriangles = null;
		}

		public virtual ValidationStatus Validate()
		{
			return MeshValidation.IsBoundaryLoop(this.Mesh, this.Loop);
		}

		public virtual bool Fill(int group_id = -1)
		{
			if (this.Loop.Vertices.Length < 3)
			{
				return false;
			}
			if (this.Loop.Vertices.Length == 3)
			{
				Index3i tv = new Index3i(this.Loop.Vertices[0], this.Loop.Vertices[2], this.Loop.Vertices[1]);
				int num = this.Mesh.AppendTriangle(tv, group_id);
				if (num < 0)
				{
					return false;
				}
				this.NewTriangles = new int[]
				{
					num
				};
				this.NewVertex = -1;
				return true;
			}
			else
			{
				Vector3d vector3d = Vector3d.Zero;
				for (int i = 0; i < this.Loop.Vertices.Length; i++)
				{
					vector3d += this.Mesh.GetVertex(this.Loop.Vertices[i]);
				}
				vector3d *= 1.0 / (double)this.Loop.Vertices.Length;
				this.NewVertex = this.Mesh.AppendVertex(vector3d);
				MeshEditor meshEditor = new MeshEditor(this.Mesh);
				try
				{
					this.NewTriangles = meshEditor.AddTriangleFan_OrderedVertexLoop(this.NewVertex, this.Loop.Vertices, group_id);
				}
				catch
				{
					this.NewTriangles = null;
				}
				if (this.NewTriangles == null)
				{
					this.Mesh.RemoveVertex(this.NewVertex, true, false);
					this.NewVertex = -1;
					return false;
				}
				return true;
			}
		}

		public DMesh3 Mesh;

		public EdgeLoop Loop;

		public int NewVertex;

		public int[] NewTriangles;
	}
}
