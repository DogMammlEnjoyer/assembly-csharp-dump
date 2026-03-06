using System;

namespace Technie.PhysicsCreator.QHull
{
	public class Vector3d
	{
		public Vector3d()
		{
		}

		public Vector3d(Vector3d v)
		{
			this.set(v);
		}

		public Vector3d(double x, double y, double z)
		{
			this.set(x, y, z);
		}

		public double get(int i)
		{
			switch (i)
			{
			case 0:
				return this.x;
			case 1:
				return this.y;
			case 2:
				return this.z;
			default:
				throw new IndexOutOfRangeException(i.ToString() ?? "");
			}
		}

		public void set(int i, double value)
		{
			switch (i)
			{
			case 0:
				this.x = value;
				return;
			case 1:
				this.y = value;
				return;
			case 2:
				this.z = value;
				return;
			default:
				throw new IndexOutOfRangeException(i.ToString() ?? "");
			}
		}

		public void set(Vector3d v1)
		{
			this.x = v1.x;
			this.y = v1.y;
			this.z = v1.z;
		}

		public void add(Vector3d v1, Vector3d v2)
		{
			this.x = v1.x + v2.x;
			this.y = v1.y + v2.y;
			this.z = v1.z + v2.z;
		}

		public void add(Vector3d v1)
		{
			this.x += v1.x;
			this.y += v1.y;
			this.z += v1.z;
		}

		public void sub(Vector3d v1, Vector3d v2)
		{
			this.x = v1.x - v2.x;
			this.y = v1.y - v2.y;
			this.z = v1.z - v2.z;
		}

		public void sub(Vector3d v1)
		{
			this.x -= v1.x;
			this.y -= v1.y;
			this.z -= v1.z;
		}

		public void scale(double s)
		{
			this.x = s * this.x;
			this.y = s * this.y;
			this.z = s * this.z;
		}

		public void scale(double s, Vector3d v1)
		{
			this.x = s * v1.x;
			this.y = s * v1.y;
			this.z = s * v1.z;
		}

		public double norm()
		{
			return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
		}

		public double normSquared()
		{
			return this.x * this.x + this.y * this.y + this.z * this.z;
		}

		public double distance(Vector3d v)
		{
			double num = this.x - v.x;
			double num2 = this.y - v.y;
			double num3 = this.z - v.z;
			return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
		}

		public double distanceSquared(Vector3d v)
		{
			double num = this.x - v.x;
			double num2 = this.y - v.y;
			double num3 = this.z - v.z;
			return num * num + num2 * num2 + num3 * num3;
		}

		public double dot(Vector3d v1)
		{
			return this.x * v1.x + this.y * v1.y + this.z * v1.z;
		}

		public void normalize()
		{
			double num = this.x * this.x + this.y * this.y + this.z * this.z;
			double num2 = num - 1.0;
			if (num2 > 4.440892098500626E-16 || num2 < -4.440892098500626E-16)
			{
				double num3 = Math.Sqrt(num);
				this.x /= num3;
				this.y /= num3;
				this.z /= num3;
			}
		}

		public void setZero()
		{
			this.x = 0.0;
			this.y = 0.0;
			this.z = 0.0;
		}

		public void set(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public void cross(Vector3d v1, Vector3d v2)
		{
			double num = v1.y * v2.z - v1.z * v2.y;
			double num2 = v1.z * v2.x - v1.x * v2.z;
			double num3 = v1.x * v2.y - v1.y * v2.x;
			this.x = num;
			this.y = num2;
			this.z = num3;
		}

		protected void setRandom(double lower, double upper, Random generator)
		{
			double num = upper - lower;
			this.x = generator.NextDouble() * num + lower;
			this.y = generator.NextDouble() * num + lower;
			this.z = generator.NextDouble() * num + lower;
		}

		public string toString()
		{
			return string.Concat(new string[]
			{
				this.x.ToString(),
				" ",
				this.y.ToString(),
				" ",
				this.z.ToString()
			});
		}

		private const double DOUBLE_PREC = 2.220446049250313E-16;

		public double x;

		public double y;

		public double z;
	}
}
