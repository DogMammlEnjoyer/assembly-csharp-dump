using System;

namespace g3
{
	public class DenseUVMesh
	{
		public DenseUVMesh()
		{
			this.UVs = new DVector<Vector2f>();
			this.TriangleUVs = new DVector<Index3i>();
		}

		public int AppendUV(Vector2f uv)
		{
			int length = this.UVs.Length;
			this.UVs.Add(uv);
			return length;
		}

		public DVector<Vector2f> UVs;

		public DVector<Index3i> TriangleUVs;
	}
}
