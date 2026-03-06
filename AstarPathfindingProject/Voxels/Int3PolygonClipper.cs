using System;

namespace Pathfinding.Voxels
{
	internal struct Int3PolygonClipper
	{
		public void Init()
		{
			if (this.clipPolygonCache == null)
			{
				this.clipPolygonCache = new float[21];
				this.clipPolygonIntCache = new int[21];
			}
		}

		public int ClipPolygon(Int3[] vIn, int n, Int3[] vOut, int multi, int offset, int axis)
		{
			this.Init();
			int[] array = this.clipPolygonIntCache;
			for (int i = 0; i < n; i++)
			{
				array[i] = multi * vIn[i][axis] + offset;
			}
			int num = 0;
			int j = 0;
			int num2 = n - 1;
			while (j < n)
			{
				bool flag = array[num2] >= 0;
				bool flag2 = array[j] >= 0;
				if (flag != flag2)
				{
					double rhs = (double)array[num2] / (double)(array[num2] - array[j]);
					vOut[num] = vIn[num2] + (vIn[j] - vIn[num2]) * rhs;
					num++;
				}
				if (flag2)
				{
					vOut[num] = vIn[j];
					num++;
				}
				num2 = j;
				j++;
			}
			return num;
		}

		private float[] clipPolygonCache;

		private int[] clipPolygonIntCache;
	}
}
