using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class MeshExtrudeMesh
	{
		public MeshExtrudeMesh(DMesh3 mesh)
		{
			this.Mesh = mesh;
			this.ExtrudedPositionF = ((Vector3d pos, Vector3f normal, int idx) => pos + normal);
		}

		public virtual ValidationStatus Validate()
		{
			return ValidationStatus.Ok;
		}

		public virtual bool Extrude()
		{
			MeshNormals meshNormals = null;
			bool hasVertexNormals = this.Mesh.HasVertexNormals;
			if (!hasVertexNormals)
			{
				meshNormals = new MeshNormals(this.Mesh, MeshNormals.NormalsTypes.Vertex_OneRingFaceAverage_AreaWeighted);
				meshNormals.Compute();
			}
			this.InitialLoops = new MeshBoundaryLoops(this.Mesh, true);
			this.InitialTriangles = this.Mesh.TriangleIndices().ToArray<int>();
			this.InitialVertices = this.Mesh.VertexIndices().ToArray<int>();
			this.InitialToOffsetMapV = new IndexMap(this.Mesh.MaxVertexID, this.Mesh.MaxVertexID);
			this.OffsetGroupID = this.OffsetGroup.GetGroupID(this.Mesh);
			MeshEditor meshEditor = new MeshEditor(this.Mesh);
			this.OffsetTriangles = meshEditor.DuplicateTriangles(this.InitialTriangles, ref this.InitialToOffsetMapV, this.OffsetGroupID);
			foreach (int num in this.InitialVertices)
			{
				int vID = this.InitialToOffsetMapV[num];
				if (this.Mesh.IsVertex(vID))
				{
					Vector3d vertex = this.Mesh.GetVertex(num);
					Vector3f arg = hasVertexNormals ? this.Mesh.GetVertexNormal(num) : ((Vector3f)meshNormals.Normals[num]);
					Vector3d vNewPos = this.ExtrudedPositionF(vertex, arg, num);
					this.Mesh.SetVertex(vID, vNewPos);
				}
			}
			if (this.IsPositiveOffset)
			{
				meshEditor.ReverseTriangles(this.InitialTriangles, true);
			}
			else
			{
				meshEditor.ReverseTriangles(this.OffsetTriangles, true);
			}
			this.NewLoops = new EdgeLoop[this.InitialLoops.Count];
			this.StitchTriangles = new int[this.InitialLoops.Count][];
			this.StitchGroupIDs = new int[this.InitialLoops.Count];
			int num2 = 0;
			foreach (EdgeLoop edgeLoop in this.InitialLoops)
			{
				int[] array = new int[edgeLoop.VertexCount];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = this.InitialToOffsetMapV[edgeLoop.Vertices[j]];
				}
				this.StitchGroupIDs[num2] = this.StitchGroups.GetGroupID(this.Mesh);
				if (this.IsPositiveOffset)
				{
					this.StitchTriangles[num2] = meshEditor.StitchLoop(array, edgeLoop.Vertices, this.StitchGroupIDs[num2]);
				}
				else
				{
					this.StitchTriangles[num2] = meshEditor.StitchLoop(edgeLoop.Vertices, array, this.StitchGroupIDs[num2]);
				}
				this.NewLoops[num2] = EdgeLoop.FromVertices(this.Mesh, array);
				num2++;
			}
			return true;
		}

		public DMesh3 Mesh;

		public SetGroupBehavior OffsetGroup = SetGroupBehavior.AutoGenerate;

		public SetGroupBehavior StitchGroups = SetGroupBehavior.AutoGenerate;

		public Func<Vector3d, Vector3f, int, Vector3d> ExtrudedPositionF;

		public bool IsPositiveOffset = true;

		public MeshBoundaryLoops InitialLoops;

		public int[] InitialTriangles;

		public int[] InitialVertices;

		public IndexMap InitialToOffsetMapV;

		private List<int> OffsetTriangles;

		public int OffsetGroupID;

		public EdgeLoop[] NewLoops;

		public int[][] StitchTriangles;

		public int[] StitchGroupIDs;
	}
}
