using System;
using UnityEngine;

namespace Pathfinding.Voxels
{
	internal struct VoxelPolygonClipper
	{
		public VoxelPolygonClipper(int capacity)
		{
			this.x = new float[capacity];
			this.y = new float[capacity];
			this.z = new float[capacity];
			this.n = 0;
		}

		public Vector3 this[int i]
		{
			set
			{
				this.x[i] = value.x;
				this.y[i] = value.y;
				this.z[i] = value.z;
			}
		}

		public void ClipPolygonAlongX(ref VoxelPolygonClipper result, float multi, float offset)
		{
			int num = 0;
			float num2 = multi * this.x[this.n - 1] + offset;
			int i = 0;
			int num3 = this.n - 1;
			while (i < this.n)
			{
				float num4 = multi * this.x[i] + offset;
				bool flag = num2 >= 0f;
				bool flag2 = num4 >= 0f;
				if (flag != flag2)
				{
					float num5 = num2 / (num2 - num4);
					result.x[num] = this.x[num3] + (this.x[i] - this.x[num3]) * num5;
					result.y[num] = this.y[num3] + (this.y[i] - this.y[num3]) * num5;
					result.z[num] = this.z[num3] + (this.z[i] - this.z[num3]) * num5;
					num++;
				}
				if (flag2)
				{
					result.x[num] = this.x[i];
					result.y[num] = this.y[i];
					result.z[num] = this.z[i];
					num++;
				}
				num2 = num4;
				num3 = i;
				i++;
			}
			result.n = num;
		}

		public void ClipPolygonAlongZWithYZ(ref VoxelPolygonClipper result, float multi, float offset)
		{
			int num = 0;
			float num2 = multi * this.z[this.n - 1] + offset;
			int i = 0;
			int num3 = this.n - 1;
			while (i < this.n)
			{
				float num4 = multi * this.z[i] + offset;
				bool flag = num2 >= 0f;
				bool flag2 = num4 >= 0f;
				if (flag != flag2)
				{
					float num5 = num2 / (num2 - num4);
					result.y[num] = this.y[num3] + (this.y[i] - this.y[num3]) * num5;
					result.z[num] = this.z[num3] + (this.z[i] - this.z[num3]) * num5;
					num++;
				}
				if (flag2)
				{
					result.y[num] = this.y[i];
					result.z[num] = this.z[i];
					num++;
				}
				num2 = num4;
				num3 = i;
				i++;
			}
			result.n = num;
		}

		public void ClipPolygonAlongZWithY(ref VoxelPolygonClipper result, float multi, float offset)
		{
			int num = 0;
			float num2 = multi * this.z[this.n - 1] + offset;
			int i = 0;
			int num3 = this.n - 1;
			while (i < this.n)
			{
				float num4 = multi * this.z[i] + offset;
				bool flag = num2 >= 0f;
				bool flag2 = num4 >= 0f;
				if (flag != flag2)
				{
					float num5 = num2 / (num2 - num4);
					result.y[num] = this.y[num3] + (this.y[i] - this.y[num3]) * num5;
					num++;
				}
				if (flag2)
				{
					result.y[num] = this.y[i];
					num++;
				}
				num2 = num4;
				num3 = i;
				i++;
			}
			result.n = num;
		}

		public float[] x;

		public float[] y;

		public float[] z;

		public int n;
	}
}
