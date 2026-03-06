using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class MeshExtrudeFaces
	{
		public MeshExtrudeFaces(DMesh3 mesh, int[] triangles, bool bForceCopyArray = false)
		{
			this.Mesh = mesh;
			if (bForceCopyArray)
			{
				this.Triangles = (int[])triangles.Clone();
			}
			else
			{
				this.Triangles = triangles;
			}
			this.ExtrudedPositionF = ((Vector3d pos, Vector3f normal, int idx) => pos + Vector3d.AxisY);
		}

		public MeshExtrudeFaces(DMesh3 mesh, IEnumerable<int> triangles)
		{
			this.Mesh = mesh;
			this.Triangles = triangles.ToArray<int>();
			this.ExtrudedPositionF = ((Vector3d pos, Vector3f normal, int idx) => pos + Vector3d.AxisY);
		}

		public virtual ValidationStatus Validate()
		{
			return ValidationStatus.Ok;
		}

		public virtual bool Extrude()
		{
			MeshEditor meshEditor = new MeshEditor(this.Mesh);
			meshEditor.SeparateTriangles(this.Triangles, true, out this.EdgePairs);
			MeshNormals meshNormals = null;
			bool hasVertexNormals = this.Mesh.HasVertexNormals;
			if (!hasVertexNormals)
			{
				meshNormals = new MeshNormals(this.Mesh, MeshNormals.NormalsTypes.Vertex_OneRingFaceAverage_AreaWeighted);
				meshNormals.Compute();
			}
			this.ExtrudeVertices = new MeshVertexSelection(this.Mesh);
			this.ExtrudeVertices.SelectTriangleVertices(this.Triangles);
			Vector3d[] array = new Vector3d[this.ExtrudeVertices.Count];
			int num = 0;
			foreach (int num2 in this.ExtrudeVertices)
			{
				Vector3d vertex = this.Mesh.GetVertex(num2);
				Vector3f arg = hasVertexNormals ? this.Mesh.GetVertexNormal(num2) : ((Vector3f)meshNormals.Normals[num2]);
				array[num++] = this.ExtrudedPositionF(vertex, arg, num2);
			}
			num = 0;
			foreach (int vID in this.ExtrudeVertices)
			{
				this.Mesh.SetVertex(vID, array[num++]);
			}
			this.JoinGroupID = this.Group.GetGroupID(this.Mesh);
			this.JoinTriangles = meshEditor.StitchUnorderedEdges(this.EdgePairs, this.JoinGroupID, false, out this.JoinIncomplete);
			return this.JoinTriangles != null && !this.JoinIncomplete;
		}

		public DMesh3 Mesh;

		public int[] Triangles;

		public SetGroupBehavior Group = SetGroupBehavior.AutoGenerate;

		public Func<Vector3d, Vector3f, int, Vector3d> ExtrudedPositionF;

		public List<Index2i> EdgePairs;

		public MeshVertexSelection ExtrudeVertices;

		public int[] JoinTriangles;

		public int JoinGroupID;

		public bool JoinIncomplete;
	}
}
