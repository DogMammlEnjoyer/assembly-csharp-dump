using System;

namespace g3
{
	public class CachingMeshSDFImplicit : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public CachingMeshSDFImplicit(CachingMeshSDF sdf)
		{
			this.SDF = sdf;
			this.GridOrigin = sdf.GridOrigin;
			this.CellSize = (double)sdf.CellSize;
		}

		public AxisAlignedBox3d Bounds()
		{
			return new AxisAlignedBox3d(this.GridOrigin.x, this.GridOrigin.y, this.GridOrigin.z, this.GridOrigin.x + this.CellSize * (double)this.SDF.Grid.ni, this.GridOrigin.y + this.CellSize * (double)this.SDF.Grid.nj, this.GridOrigin.z + this.CellSize * (double)this.SDF.Grid.nk);
		}

		public double Value(ref Vector3d pt)
		{
			Vector3d vector3d = new Vector3d((pt.x - this.GridOrigin.x) / this.CellSize, (pt.y - this.GridOrigin.y) / this.CellSize, (pt.z - this.GridOrigin.z) / this.CellSize);
			int num = (int)vector3d.x;
			int num2 = (int)vector3d.y;
			int num3 = num2 + 1;
			int num4 = (int)vector3d.z;
			int num5 = num4 + 1;
			if (num < 0 || num + 1 >= this.SDF.Grid.ni || num2 < 0 || num3 >= this.SDF.Grid.nj || num4 < 0 || num5 >= this.SDF.Grid.nk)
			{
				return this.Outside;
			}
			double num6 = vector3d.x - (double)num;
			double num7 = vector3d.y - (double)num2;
			double num8 = vector3d.z - (double)num4;
			double num9 = 1.0 - num6;
			double num10;
			double num11;
			this.get_value_pair(num, num2, num4, out num10, out num11);
			double num12 = (1.0 - num7) * (1.0 - num8);
			double num13 = (num9 * num10 + num6 * num11) * num12;
			this.get_value_pair(num, num2, num5, out num10, out num11);
			num12 = (1.0 - num7) * num8;
			double num14 = num13 + (num9 * num10 + num6 * num11) * num12;
			this.get_value_pair(num, num3, num4, out num10, out num11);
			num12 = num7 * (1.0 - num8);
			double num15 = num14 + (num9 * num10 + num6 * num11) * num12;
			this.get_value_pair(num, num3, num5, out num10, out num11);
			num12 = num7 * num8;
			return num15 + (num9 * num10 + num6 * num11) * num12;
		}

		private void get_value_pair(int i, int j, int k, out double a, out double b)
		{
			a = (double)this.SDF.GetValue(new Vector3i(i, j, k));
			b = (double)this.SDF.GetValue(new Vector3i(i + 1, j, k));
		}

		public Vector3d Gradient(ref Vector3d pt)
		{
			Vector3d vector3d = new Vector3d((pt.x - this.GridOrigin.x) / this.CellSize, (pt.y - this.GridOrigin.y) / this.CellSize, (pt.z - this.GridOrigin.z) / this.CellSize);
			if (vector3d.x < 0.0 || vector3d.x >= (double)(this.SDF.Grid.ni - 1) || vector3d.y < 0.0 || vector3d.y >= (double)(this.SDF.Grid.nj - 1) || vector3d.z < 0.0 || vector3d.z >= (double)(this.SDF.Grid.nk - 1))
			{
				return Vector3d.Zero;
			}
			int num = (int)vector3d.x;
			int num2 = (int)vector3d.y;
			int j = num2 + 1;
			int num3 = (int)vector3d.z;
			int k = num3 + 1;
			double num4 = vector3d.x - (double)num;
			double num5 = vector3d.y - (double)num2;
			double num6 = vector3d.z - (double)num3;
			double num7;
			double num8;
			this.get_value_pair(num, num2, num3, out num7, out num8);
			double num9;
			double num10;
			this.get_value_pair(num, j, num3, out num9, out num10);
			double num11;
			double num12;
			this.get_value_pair(num, num2, k, out num11, out num12);
			double num13;
			double num14;
			this.get_value_pair(num, j, k, out num13, out num14);
			double x = -num7 * (1.0 - num5) * (1.0 - num6) + -num11 * (1.0 - num5) * num6 + -num9 * num5 * (1.0 - num6) + -num13 * num5 * num6 + num8 * (1.0 - num5) * (1.0 - num6) + num12 * (1.0 - num5) * num6 + num10 * num5 * (1.0 - num6) + num14 * num5 * num6;
			double y = -num7 * (1.0 - num4) * (1.0 - num6) + -num11 * (1.0 - num4) * num6 + num9 * (1.0 - num4) * (1.0 - num6) + num13 * (1.0 - num4) * num6 + -num8 * num4 * (1.0 - num6) + -num12 * num4 * num6 + num10 * num4 * (1.0 - num6) + num14 * num4 * num6;
			double z = -num7 * (1.0 - num4) * (1.0 - num5) + num11 * (1.0 - num4) * (1.0 - num5) + -num9 * (1.0 - num4) * num5 + num13 * (1.0 - num4) * num5 + -num8 * num4 * (1.0 - num5) + num12 * num4 * (1.0 - num5) + -num10 * num4 * num5 + num14 * num4 * num5;
			return new Vector3d(x, y, z);
		}

		public CachingMeshSDF SDF;

		public double CellSize;

		public Vector3d GridOrigin;

		public double Outside = Math.Sqrt(Math.Sqrt(double.MaxValue));
	}
}
