using System;

namespace g3
{
	public static class SpatialFunctions
	{
		[Obsolete("NormalOffset is deprecated - is anybody using it? please lmk.")]
		public class NormalOffset
		{
			public Vector3d FindNearestAndOffset(Vector3d pos)
			{
				int num = this.Spatial.FindNearestTriangle(pos, double.MaxValue);
				DistPoint3Triangle3 distPoint3Triangle = MeshQueries.TriangleDistance(this.Mesh, num, pos);
				Vector3d v = (!this.UseFaceNormal && this.Mesh.HasVertexNormals) ? this.Mesh.GetTriBaryNormal(num, distPoint3Triangle.TriangleBaryCoords.x, distPoint3Triangle.TriangleBaryCoords.y, distPoint3Triangle.TriangleBaryCoords.z) : this.Mesh.GetTriNormal(num);
				return distPoint3Triangle.TriangleClosest + this.Distance * v;
			}

			public DMesh3 Mesh;

			public ISpatial Spatial;

			public double Distance = 0.01;

			public bool UseFaceNormal = true;
		}
	}
}
