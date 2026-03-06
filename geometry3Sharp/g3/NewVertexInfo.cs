using System;

namespace g3
{
	public struct NewVertexInfo
	{
		public NewVertexInfo(Vector3d v)
		{
			this.v = v;
			this.n = (this.c = Vector3f.Zero);
			this.uv = Vector2f.Zero;
			this.bHaveN = (this.bHaveC = (this.bHaveUV = false));
		}

		public NewVertexInfo(Vector3d v, Vector3f n)
		{
			this.v = v;
			this.n = n;
			this.c = Vector3f.Zero;
			this.uv = Vector2f.Zero;
			this.bHaveN = true;
			this.bHaveC = (this.bHaveUV = false);
		}

		public NewVertexInfo(Vector3d v, Vector3f n, Vector3f c)
		{
			this.v = v;
			this.n = n;
			this.c = c;
			this.uv = Vector2f.Zero;
			this.bHaveN = (this.bHaveC = true);
			this.bHaveUV = false;
		}

		public NewVertexInfo(Vector3d v, Vector3f n, Vector3f c, Vector2f uv)
		{
			this.v = v;
			this.n = n;
			this.c = c;
			this.uv = uv;
			this.bHaveN = (this.bHaveC = (this.bHaveUV = true));
		}

		public Vector3d v;

		public Vector3f n;

		public Vector3f c;

		public Vector2f uv;

		public bool bHaveN;

		public bool bHaveUV;

		public bool bHaveC;
	}
}
