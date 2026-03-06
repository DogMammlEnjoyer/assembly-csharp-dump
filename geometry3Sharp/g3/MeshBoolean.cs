using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshBoolean
	{
		public bool Compute()
		{
			this.cutTargetOp = new MeshMeshCut
			{
				Target = new DMesh3(this.Target, false, true, true, true),
				CutMesh = this.Tool,
				VertexSnapTol = this.VertexSnapTol
			};
			this.cutTargetOp.Compute();
			this.cutTargetOp.RemoveContained();
			this.cutTargetMesh = this.cutTargetOp.Target;
			this.cutToolOp = new MeshMeshCut
			{
				Target = new DMesh3(this.Tool, false, true, true, true),
				CutMesh = this.Target,
				VertexSnapTol = this.VertexSnapTol
			};
			this.cutToolOp.Compute();
			this.cutToolOp.RemoveContained();
			this.cutToolMesh = this.cutToolOp.Target;
			this.resolve_vtx_pairs();
			this.Result = this.cutToolMesh;
			MeshEditor.Append(this.Result, this.cutTargetMesh);
			return true;
		}

		private void resolve_vtx_pairs()
		{
			HashSet<int> hashSet = new HashSet<int>(MeshIterators.BoundaryVertices(this.cutTargetMesh));
			HashSet<int> hashSet2 = new HashSet<int>(MeshIterators.BoundaryVertices(this.cutToolMesh));
			this.split_missing(this.cutTargetOp, this.cutToolOp, this.cutTargetMesh, this.cutToolMesh, hashSet, hashSet2);
			this.split_missing(this.cutToolOp, this.cutTargetOp, this.cutToolMesh, this.cutTargetMesh, hashSet2, hashSet);
		}

		private void split_missing(MeshMeshCut fromOp, MeshMeshCut toOp, DMesh3 fromMesh, DMesh3 toMesh, HashSet<int> fromVerts, HashSet<int> toVerts)
		{
			List<int> list = new List<int>();
			foreach (int num in fromVerts)
			{
				Vector3d vertex = fromMesh.GetVertex(num);
				if (this.find_nearest_vertex(toMesh, vertex, toVerts) == -1)
				{
					list.Add(num);
				}
			}
			foreach (int vID in list)
			{
				Vector3d vertex2 = fromMesh.GetVertex(vID);
				int num2 = this.find_nearest_edge(toMesh, vertex2, toVerts);
				DMesh3.EdgeSplitInfo edgeSplitInfo;
				if (num2 == -1)
				{
					Console.WriteLine("could not find edge to split?");
				}
				else if (toMesh.SplitEdge(num2, out edgeSplitInfo, 0.5) != MeshResult.Ok)
				{
					Console.WriteLine("edge split failed");
				}
				else
				{
					toMesh.SetVertex(edgeSplitInfo.vNew, vertex2);
					toVerts.Add(edgeSplitInfo.vNew);
				}
			}
		}

		private int find_nearest_vertex(DMesh3 mesh, Vector3d v, HashSet<int> vertices)
		{
			int result = -1;
			double num = this.VertexSnapTol * this.VertexSnapTol;
			foreach (int num2 in vertices)
			{
				double num3 = mesh.GetVertex(num2).DistanceSquared(ref v);
				if (num3 < num)
				{
					result = num2;
					num = num3;
				}
			}
			return result;
		}

		private int find_nearest_edge(DMesh3 mesh, Vector3d v, HashSet<int> vertices)
		{
			int result = -1;
			double num = this.VertexSnapTol * this.VertexSnapTol;
			foreach (int num2 in mesh.BoundaryEdgeIndices())
			{
				Index2i edgeV = mesh.GetEdgeV(num2);
				if (vertices.Contains(edgeV.a) && vertices.Contains(edgeV.b))
				{
					Segment3d segment3d = new Segment3d(mesh.GetVertex(edgeV.a), mesh.GetVertex(edgeV.b));
					double num3 = segment3d.DistanceSquared(v);
					if (num3 < num)
					{
						result = num2;
						num = num3;
					}
				}
			}
			return result;
		}

		public DMesh3 Target;

		public DMesh3 Tool;

		public double VertexSnapTol = 1E-05;

		public DMesh3 Result;

		private MeshMeshCut cutTargetOp;

		private MeshMeshCut cutToolOp;

		private DMesh3 cutTargetMesh;

		private DMesh3 cutToolMesh;
	}
}
