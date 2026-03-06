using System;

namespace g3
{
	public class MeshExtrudeLoop
	{
		public MeshExtrudeLoop(DMesh3 mesh, EdgeLoop loop)
		{
			this.Mesh = mesh;
			this.Loop = loop;
			this.PositionF = ((Vector3d pos, Vector3f normal, int idx) => pos + Vector3d.AxisY);
		}

		public virtual ValidationStatus Validate()
		{
			return MeshValidation.IsBoundaryLoop(this.Mesh, this.Loop);
		}

		public virtual bool Extrude(int group_id = -1)
		{
			int num = this.Loop.Vertices.Length;
			this.NewLoop = new EdgeLoop(this.Mesh);
			this.NewLoop.Vertices = new int[num];
			for (int i = 0; i < num; i++)
			{
				int fromVID = this.Loop.Vertices[i];
				this.NewLoop.Vertices[i] = this.Mesh.AppendVertex(this.Mesh, fromVID);
			}
			for (int j = 0; j < num; j++)
			{
				Vector3d vertex = this.Mesh.GetVertex(this.Loop.Vertices[j]);
				Vector3f vertexNormal = this.Mesh.GetVertexNormal(this.Loop.Vertices[j]);
				Vector3d vNewPos = this.PositionF(vertex, vertexNormal, this.Loop.Vertices[j]);
				this.Mesh.SetVertex(this.NewLoop.Vertices[j], vNewPos);
			}
			MeshEditor meshEditor = new MeshEditor(this.Mesh);
			this.NewTriangles = meshEditor.StitchLoop(this.Loop.Vertices, this.NewLoop.Vertices, group_id);
			return true;
		}

		public DMesh3 Mesh;

		public EdgeLoop Loop;

		public Func<Vector3d, Vector3f, int, Vector3d> PositionF;

		public int[] NewTriangles;

		public EdgeLoop NewLoop;
	}
}
