using System;

namespace g3
{
	public class MeshTriInfoCache
	{
		public MeshTriInfoCache(DMesh3 mesh)
		{
			MeshTriInfoCache <>4__this = this;
			int triangleCount = mesh.TriangleCount;
			this.Centroids = new DVector<Vector3d>();
			this.Centroids.resize(triangleCount);
			this.Normals = new DVector<Vector3d>();
			this.Normals.resize(triangleCount);
			this.Areas = new DVector<double>();
			this.Areas.resize(triangleCount);
			gParallel.ForEach<int>(mesh.TriangleIndices(), delegate(int tid)
			{
				Vector3d value;
				double value2;
				Vector3d value3;
				mesh.GetTriInfo(tid, out value, out value2, out value3);
				<>4__this.Centroids[tid] = value3;
				<>4__this.Normals[tid] = value;
				<>4__this.Areas[tid] = value2;
			});
		}

		public void GetTriInfo(int tid, ref Vector3d n, ref double a, ref Vector3d c)
		{
			c = this.Centroids[tid];
			n = this.Normals[tid];
			a = this.Areas[tid];
		}

		public DVector<Vector3d> Centroids;

		public DVector<Vector3d> Normals;

		public DVector<double> Areas;
	}
}
