using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshProjectionTarget : IOrientedProjectionTarget, IProjectionTarget
	{
		public DMesh3 Mesh { get; set; }

		public ISpatial Spatial { get; set; }

		public MeshProjectionTarget()
		{
		}

		public MeshProjectionTarget(DMesh3 mesh, ISpatial spatial)
		{
			this.Mesh = mesh;
			this.Spatial = spatial;
			if (this.Spatial == null)
			{
				this.Spatial = new DMeshAABBTree3(mesh, true);
			}
		}

		public MeshProjectionTarget(DMesh3 mesh)
		{
			this.Mesh = mesh;
			this.Spatial = new DMeshAABBTree3(mesh, true);
		}

		public virtual Vector3d Project(Vector3d vPoint, int identifier = -1)
		{
			int tID = this.Spatial.FindNearestTriangle(vPoint, double.MaxValue);
			Triangle3d triangle3d = default(Triangle3d);
			this.Mesh.GetTriVertices(tID, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			Vector3d result;
			Vector3d vector3d;
			DistPoint3Triangle3.DistanceSqr(ref vPoint, ref triangle3d, out result, out vector3d);
			return result;
		}

		public virtual Vector3d Project(Vector3d vPoint, out Vector3d vProjectNormal, int identifier = -1)
		{
			int tID = this.Spatial.FindNearestTriangle(vPoint, double.MaxValue);
			Triangle3d triangle3d = default(Triangle3d);
			this.Mesh.GetTriVertices(tID, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
			Vector3d result;
			Vector3d vector3d;
			DistPoint3Triangle3.DistanceSqr(ref vPoint, ref triangle3d, out result, out vector3d);
			vProjectNormal = triangle3d.Normal;
			return result;
		}

		public static MeshProjectionTarget Auto(DMesh3 mesh, bool bForceCopy = true)
		{
			if (bForceCopy)
			{
				return new MeshProjectionTarget(new DMesh3(mesh, false, MeshComponents.None));
			}
			return new MeshProjectionTarget(mesh);
		}

		public static MeshProjectionTarget Auto(DMesh3 mesh, IEnumerable<int> triangles, int nExpandRings = 5)
		{
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(mesh);
			meshFaceSelection.Select(triangles);
			meshFaceSelection.ExpandToOneRingNeighbours(nExpandRings, null);
			return new MeshProjectionTarget(new DSubmesh3(mesh, meshFaceSelection, 0).SubMesh);
		}
	}
}
