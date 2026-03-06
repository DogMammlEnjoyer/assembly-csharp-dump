using System;

namespace g3
{
	public class DMeshIntersectionTarget : IIntersectionTarget
	{
		public DMesh3 Mesh { get; set; }

		public ISpatial Spatial { get; set; }

		public DMeshIntersectionTarget()
		{
		}

		public DMeshIntersectionTarget(DMesh3 mesh, ISpatial spatial)
		{
			this.Mesh = mesh;
			this.Spatial = spatial;
		}

		public bool HasNormal
		{
			get
			{
				return true;
			}
		}

		public bool RayIntersect(Ray3d ray, out Vector3d vHit, out Vector3d vHitNormal)
		{
			vHit = Vector3d.Zero;
			vHitNormal = Vector3d.AxisX;
			int num = this.Spatial.FindNearestHitTriangle(ray, double.MaxValue);
			if (num == -1)
			{
				return false;
			}
			IntrRay3Triangle3 intrRay3Triangle = MeshQueries.TriangleIntersection(this.Mesh, num, ray);
			vHit = ray.PointAt(intrRay3Triangle.RayParameter);
			if (!this.UseFaceNormal && this.Mesh.HasVertexNormals)
			{
				vHitNormal = this.Mesh.GetTriBaryNormal(num, intrRay3Triangle.TriangleBaryCoords.x, intrRay3Triangle.TriangleBaryCoords.y, intrRay3Triangle.TriangleBaryCoords.z);
			}
			else
			{
				vHitNormal = this.Mesh.GetTriNormal(num);
			}
			return true;
		}

		public bool UseFaceNormal = true;
	}
}
