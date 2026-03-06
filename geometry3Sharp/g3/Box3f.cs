using System;
using System.Collections.Generic;

namespace g3
{
	public struct Box3f
	{
		public Box3f(Vector3f center)
		{
			this.Center = center;
			this.AxisX = Vector3f.AxisX;
			this.AxisY = Vector3f.AxisY;
			this.AxisZ = Vector3f.AxisZ;
			this.Extent = Vector3f.Zero;
		}

		public Box3f(Vector3f center, Vector3f x, Vector3f y, Vector3f z, Vector3f extent)
		{
			this.Center = center;
			this.AxisX = x;
			this.AxisY = y;
			this.AxisZ = z;
			this.Extent = extent;
		}

		public Box3f(Vector3f center, Vector3f extent)
		{
			this.Center = center;
			this.Extent = extent;
			this.AxisX = Vector3f.AxisX;
			this.AxisY = Vector3f.AxisY;
			this.AxisZ = Vector3f.AxisZ;
		}

		public Box3f(AxisAlignedBox3f aaBox)
		{
			this.Extent = new Vector3f(aaBox.Width * 0.5f, aaBox.Height * 0.5f, aaBox.Depth * 0.5f);
			this.Center = aaBox.Center;
			this.AxisX = Vector3f.AxisX;
			this.AxisY = Vector3f.AxisY;
			this.AxisZ = Vector3f.AxisZ;
		}

		public Vector3f Axis(int i)
		{
			if (i == 0)
			{
				return this.AxisX;
			}
			if (i != 1)
			{
				return this.AxisZ;
			}
			return this.AxisY;
		}

		public Vector3f[] ComputeVertices()
		{
			Vector3f[] array = new Vector3f[8];
			this.ComputeVertices(array);
			return array;
		}

		public void ComputeVertices(Vector3f[] vertex)
		{
			Vector3f v = this.Extent.x * this.AxisX;
			Vector3f v2 = this.Extent.y * this.AxisY;
			Vector3f v3 = this.Extent.z * this.AxisZ;
			vertex[0] = this.Center - v - v2 - v3;
			vertex[1] = this.Center + v - v2 - v3;
			vertex[2] = this.Center + v + v2 - v3;
			vertex[3] = this.Center - v + v2 - v3;
			vertex[4] = this.Center - v - v2 + v3;
			vertex[5] = this.Center + v - v2 + v3;
			vertex[6] = this.Center + v + v2 + v3;
			vertex[7] = this.Center - v + v2 + v3;
		}

		public IEnumerable<Vector3f> VerticesItr()
		{
			Vector3f extAxis0 = this.Extent.x * this.AxisX;
			Vector3f extAxis = this.Extent.y * this.AxisY;
			Vector3f extAxis2 = this.Extent.z * this.AxisZ;
			yield return this.Center - extAxis0 - extAxis - extAxis2;
			yield return this.Center + extAxis0 - extAxis - extAxis2;
			yield return this.Center + extAxis0 + extAxis - extAxis2;
			yield return this.Center - extAxis0 + extAxis - extAxis2;
			yield return this.Center - extAxis0 - extAxis + extAxis2;
			yield return this.Center + extAxis0 - extAxis + extAxis2;
			yield return this.Center + extAxis0 + extAxis + extAxis2;
			yield return this.Center - extAxis0 + extAxis + extAxis2;
			yield break;
		}

		public AxisAlignedBox3f ToAABB()
		{
			Vector3f v = this.Extent.x * this.AxisX;
			Vector3f v2 = this.Extent.y * this.AxisY;
			Vector3f v3 = this.Extent.z * this.AxisZ;
			AxisAlignedBox3f result = new AxisAlignedBox3f(this.Center - v - v2 - v3);
			result.Contain(this.Center + v - v2 - v3);
			result.Contain(this.Center + v + v2 - v3);
			result.Contain(this.Center - v + v2 - v3);
			result.Contain(this.Center - v - v2 + v3);
			result.Contain(this.Center + v - v2 + v3);
			result.Contain(this.Center + v + v2 + v3);
			result.Contain(this.Center - v + v2 + v3);
			return result;
		}

		public double MaxExtent
		{
			get
			{
				return (double)Math.Max(this.Extent.x, Math.Max(this.Extent.y, this.Extent.z));
			}
		}

		public double MinExtent
		{
			get
			{
				return (double)Math.Min(this.Extent.x, Math.Max(this.Extent.y, this.Extent.z));
			}
		}

		public Vector3f Diagonal
		{
			get
			{
				return this.Extent.x * this.AxisX + this.Extent.y * this.AxisY + this.Extent.z * this.AxisZ - (-this.Extent.x * this.AxisX - this.Extent.y * this.AxisY - this.Extent.z * this.AxisZ);
			}
		}

		public double Volume
		{
			get
			{
				return (double)(2f * this.Extent.x * 2f * this.Extent.y * 2f * this.Extent.z);
			}
		}

		public void Contain(Vector3f v)
		{
			Vector3f vector3f = v - this.Center;
			for (int i = 0; i < 3; i++)
			{
				double num = (double)vector3f.Dot(this.Axis(i));
				if (Math.Abs(num) > (double)this.Extent[i])
				{
					double num2 = (double)(-(double)this.Extent[i]);
					double num3 = (double)this.Extent[i];
					if (num < num2)
					{
						num2 = num;
					}
					else if (num > num3)
					{
						num3 = num;
					}
					this.Extent[i] = (float)(num3 - num2) * 0.5f;
					this.Center += (float)(num3 + num2) * 0.5f * this.Axis(i);
				}
			}
		}

		public void Contain(Box3f o)
		{
			Vector3f[] array = o.ComputeVertices();
			for (int i = 0; i < 8; i++)
			{
				this.Contain(array[i]);
			}
		}

		public bool Contains(Vector3f v)
		{
			Vector3f vector3f = v - this.Center;
			return Math.Abs(vector3f.Dot(this.AxisX)) <= this.Extent.x && Math.Abs(vector3f.Dot(this.AxisY)) <= this.Extent.y && Math.Abs(vector3f.Dot(this.AxisZ)) <= this.Extent.z;
		}

		public void Expand(float f)
		{
			this.Extent += f;
		}

		public void Translate(Vector3f v)
		{
			this.Center += v;
		}

		public void Scale(Vector3f s)
		{
			this.Center *= s;
			this.Extent *= s;
			this.AxisX *= s;
			this.AxisX.Normalize(1.1920929E-07f);
			this.AxisY *= s;
			this.AxisY.Normalize(1.1920929E-07f);
			this.AxisZ *= s;
			this.AxisZ.Normalize(1.1920929E-07f);
		}

		public void ScaleExtents(Vector3f s)
		{
			this.Extent *= s;
		}

		public Vector3f Center;

		public Vector3f AxisX;

		public Vector3f AxisY;

		public Vector3f AxisZ;

		public Vector3f Extent;

		public static readonly Box3f Empty = new Box3f(Vector3f.Zero);
	}
}
