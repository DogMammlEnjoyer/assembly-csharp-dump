using System;

namespace g3
{
	public struct Box2f
	{
		public Box2f(Vector2f center)
		{
			this.Center = center;
			this.AxisX = Vector2f.AxisX;
			this.AxisY = Vector2f.AxisY;
			this.Extent = Vector2f.Zero;
		}

		public Box2f(Vector2f center, Vector2f x, Vector2f y, Vector2f extent)
		{
			this.Center = center;
			this.AxisX = x;
			this.AxisY = y;
			this.Extent = extent;
		}

		public Box2f(Vector2f center, Vector2f extent)
		{
			this.Center = center;
			this.Extent = extent;
			this.AxisX = Vector2f.AxisX;
			this.AxisY = Vector2f.AxisY;
		}

		public Box2f(AxisAlignedBox2f aaBox)
		{
			this.Extent = 0.5f * aaBox.Diagonal;
			this.Center = aaBox.Min + this.Extent;
			this.AxisX = Vector2f.AxisX;
			this.AxisY = Vector2f.AxisY;
		}

		public Vector2f Axis(int i)
		{
			if (i != 0)
			{
				return this.AxisY;
			}
			return this.AxisX;
		}

		public Vector2f[] ComputeVertices()
		{
			Vector2f[] array = new Vector2f[4];
			this.ComputeVertices(array);
			return array;
		}

		public void ComputeVertices(Vector2f[] vertex)
		{
			Vector2f o = this.Extent.x * this.AxisX;
			Vector2f o2 = this.Extent.y * this.AxisY;
			vertex[0] = this.Center - o - o2;
			vertex[1] = this.Center + o - o2;
			vertex[2] = this.Center + o + o2;
			vertex[3] = this.Center - o + o2;
		}

		public double MaxExtent
		{
			get
			{
				return (double)Math.Max(this.Extent.x, this.Extent.y);
			}
		}

		public double MinExtent
		{
			get
			{
				return (double)Math.Min(this.Extent.x, this.Extent.y);
			}
		}

		public Vector2f Diagonal
		{
			get
			{
				return this.Extent.x * this.AxisX + this.Extent.y * this.AxisY - (-this.Extent.x * this.AxisX - this.Extent.y * this.AxisY);
			}
		}

		public double Area
		{
			get
			{
				return (double)(2f * this.Extent.x * 2f * this.Extent.y);
			}
		}

		public void Contain(Vector2f v)
		{
			Vector2f vector2f = v - this.Center;
			for (int i = 0; i < 2; i++)
			{
				double num = (double)vector2f.Dot(this.Axis(i));
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

		public void Contain(Box2f o)
		{
			Vector2f[] array = o.ComputeVertices();
			for (int i = 0; i < 4; i++)
			{
				this.Contain(array[i]);
			}
		}

		public bool Contains(Vector2f v)
		{
			Vector2f vector2f = v - this.Center;
			return Math.Abs(vector2f.Dot(this.AxisX)) <= this.Extent.x && Math.Abs(vector2f.Dot(this.AxisY)) <= this.Extent.y;
		}

		public void Expand(float f)
		{
			this.Extent += f;
		}

		public void Translate(Vector2f v)
		{
			this.Center += v;
		}

		public Vector2f Center;

		public Vector2f AxisX;

		public Vector2f AxisY;

		public Vector2f Extent;

		public static readonly Box2f Empty = new Box2f(Vector2f.Zero);
	}
}
