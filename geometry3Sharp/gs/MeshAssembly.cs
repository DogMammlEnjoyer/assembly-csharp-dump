using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class MeshAssembly
	{
		public MeshAssembly(DMesh3 sourceMesh)
		{
			this.SourceMesh = sourceMesh;
			this.ClosedSolids = new List<DMesh3>();
			this.OpenMeshes = new List<DMesh3>();
		}

		public void Decompose()
		{
			this.process();
		}

		private void process()
		{
			DMesh3 dmesh = this.SourceMesh;
			if (!dmesh.CachedIsClosed)
			{
				dmesh = new DMesh3(this.SourceMesh, false, true, true, true);
				new RemoveDuplicateTriangles(dmesh).Apply();
				new MergeCoincidentEdges(dmesh).Apply();
			}
			DMesh3[] array = MeshConnectedComponents.Separate(dmesh);
			List<DMesh3> list = new List<DMesh3>();
			foreach (DMesh3 dmesh2 in array)
			{
				if (!dmesh2.CachedIsClosed)
				{
					this.OpenMeshes.Add(dmesh2);
				}
				else
				{
					list.Add(dmesh2);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			if (list.Count == 1)
			{
				this.ClosedSolids = new List<DMesh3>
				{
					list[0]
				};
			}
			if (this.HasNoVoids)
			{
				this.ClosedSolids = this.process_solids_novoid(list);
				return;
			}
			this.ClosedSolids = this.process_solids(list);
		}

		private List<DMesh3> process_solids(List<DMesh3> solid_components)
		{
			DMesh3 dmesh = new DMesh3(this.SourceMesh.Components | MeshComponents.FaceGroups);
			MeshEditor meshEditor = new MeshEditor(dmesh);
			foreach (DMesh3 appendMesh in solid_components)
			{
				meshEditor.AppendMesh(appendMesh, dmesh.AllocateTriangleGroup());
			}
			return new List<DMesh3>
			{
				dmesh
			};
		}

		private List<DMesh3> process_solids_novoid(List<DMesh3> solid_components)
		{
			return solid_components;
		}

		public DMesh3 SourceMesh;

		public bool HasNoVoids;

		public List<DMesh3> ClosedSolids;

		public List<DMesh3> OpenMeshes;
	}
}
