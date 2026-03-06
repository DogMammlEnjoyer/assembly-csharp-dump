using System;

namespace g3
{
	internal struct Triangle
	{
		public void clear()
		{
			this.nMaterialID = -1;
			this.nGroupID = -1;
			this.vIndices = (this.vNormals = (this.vUVs = new Index3i(-1, -1, -1)));
		}

		public void set_vertex(int j, int vi, int ni = -1, int ui = -1)
		{
			this.vIndices[j] = vi;
			if (ni != -1)
			{
				this.vNormals[j] = ni;
			}
			if (ui != -1)
			{
				this.vUVs[j] = ui;
			}
		}

		public void move_vertex(int jFrom, int jTo)
		{
			this.vIndices[jTo] = this.vIndices[jFrom];
			this.vNormals[jTo] = this.vNormals[jFrom];
			this.vUVs[jTo] = this.vUVs[jFrom];
		}

		public bool is_complex()
		{
			for (int i = 0; i < 3; i++)
			{
				if (this.vNormals[i] != -1 && this.vNormals[i] != this.vNormals[i])
				{
					return true;
				}
				if (this.vUVs[i] != -1 && this.vUVs[i] != this.vUVs[i])
				{
					return true;
				}
			}
			return false;
		}

		public const int InvalidMaterialID = -1;

		public const int InvalidGroupID = -1;

		public Index3i vIndices;

		public Index3i vNormals;

		public Index3i vUVs;

		public int nMaterialID;

		public int nGroupID;
	}
}
