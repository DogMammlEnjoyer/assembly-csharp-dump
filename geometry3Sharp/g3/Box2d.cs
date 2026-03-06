using System;

namespace g3
{
	public struct Box2d
	{
		public Box2d(Vector2d center)
		{
			this.Center = center;
			this.AxisX = Vector2d.AxisX;
			this.AxisY = Vector2d.AxisY;
			this.Extent = Vector2d.Zero;
		}

		public Box2d(Vector2d center, Vector2d x, Vector2d y, Vector2d extent)
		{
			this.Center = center;
			this.AxisX = x;
			this.AxisY = y;
			this.Extent = extent;
		}

		public Box2d(Vector2d center, Vector2d extent)
		{
			this.Center = center;
			this.Extent = extent;
			this.AxisX = Vector2d.AxisX;
			this.AxisY = Vector2d.AxisY;
		}

		public Box2d(AxisAlignedBox2d aaBox)
		{
			this.Extent = 0.5 * aaBox.Diagonal;
			this.Center = aaBox.Min + this.Extent;
			this.AxisX = Vector2d.AxisX;
			this.AxisY = Vector2d.AxisY;
		}

		public Box2d(Segment2d seg)
		{
			this.Center = seg.Center;
			this.AxisX = seg.Direction;
			this.AxisY = seg.Direction.Perp;
			this.Extent = new Vector2d(seg.Extent, 0.0);
		}

		public Vector2d Axis(int i)
		{
			if (i != 0)
			{
				return this.AxisY;
			}
			return this.AxisX;
		}

		public Vector2d[] ComputeVertices()
		{
			Vector2d[] array = new Vector2d[4];
			this.ComputeVertices(array);
			return array;
		}

		public void ComputeVertices(Vector2d[] vertex)
		{
			Vector2d o = this.Extent.x * this.AxisX;
			Vector2d o2 = this.Extent.y * this.AxisY;
			vertex[0] = this.Center - o - o2;
			vertex[1] = this.Center + o - o2;
			vertex[2] = this.Center + o + o2;
			vertex[3] = this.Center - o + o2;
		}

		public void ComputeVertices(ref Vector2dTuple4 vertex)
		{
			Vector2d o = this.Extent.x * this.AxisX;
			Vector2d o2 = this.Extent.y * this.AxisY;
			vertex[0] = this.Center - o - o2;
			vertex[1] = this.Center + o - o2;
			vertex[2] = this.Center + o + o2;
			vertex[3] = this.Center - o + o2;
		}

		public double MaxExtent
		{
			get
			{
				return Math.Max(this.Extent.x, this.Extent.y);
			}
		}

		public double MinExtent
		{
			get
			{
				return Math.Min(this.Extent.x, this.Extent.y);
			}
		}

		public Vector2d Diagonal
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
				return 2.0 * this.Extent.x * 2.0 * this.Extent.y;
			}
		}

		public void Contain(Vector2d v)
		{
			Vector2d vector2d = v - this.Center;
			for (int i = 0; i < 2; i++)
			{
				double num = vector2d.Dot(this.Axis(i));
				if (Math.Abs(num) > this.Extent[i])
				{
					double num2 = -this.Extent[i];
					double num3 = this.Extent[i];
					if (num < num2)
					{
						num2 = num;
					}
					else if (num > num3)
					{
						num3 = num;
					}
					this.Extent[i] = (num3 - num2) * 0.5;
					this.Center += (num3 + num2) * 0.5 * this.Axis(i);
				}
			}
		}

		public void Contain(Box2d o)
		{
			Vector2d[] array = o.ComputeVertices();
			for (int i = 0; i < 4; i++)
			{
				this.Contain(array[i]);
			}
		}

		public bool Contains(Vector2d v)
		{
			Vector2d vector2d = v - this.Center;
			return Math.Abs(vector2d.Dot(this.AxisX)) <= this.Extent.x && Math.Abs(vector2d.Dot(this.AxisY)) <= this.Extent.y;
		}

		public void Expand(double f)
		{
			this.Extent += f;
		}

		public void Translate(Vector2d v)
		{
			this.Center += v;
		}

		public void RotateAxes(Matrix2d m)
		{
			this.AxisX = m * this.AxisX;
			this.AxisY = m * this.AxisY;
		}

		public double DistanceSquared(Vector2d v)
		{
			v -= this.Center;
			double num = 0.0;
			for (int i = 0; i < 2; i++)
			{
				double num2;
				double num3;
				if (i == 0)
				{
					num2 = v.Dot(this.AxisX);
					num3 = this.Extent.x;
				}
				else
				{
					num2 = v.Dot(this.AxisY);
					num3 = this.Extent.y;
				}
				if (num2 < -num3)
				{
					double num4 = num2 + num3;
					num += num4 * num4;
				}
				else if (num2 > num3)
				{
					double num4 = num2 - num3;
					num += num4 * num4;
				}
			}
			return num;
		}

		public Vector2d ClosestPoint(Vector2d v)
		{
			Vector2d vector2d = v - this.Center;
			double num = 0.0;
			Vector2d vector2d2 = default(Vector2d);
			for (int i = 0; i < 2; i++)
			{
				vector2d2[i] = vector2d.Dot((i == 0) ? this.AxisX : this.AxisY);
				double num2 = (i == 0) ? this.Extent.x : this.Extent.y;
				if (vector2d2[i] < -num2)
				{
					double num3 = vector2d2[i] + num2;
					num += num3 * num3;
					vector2d2[i] = -num2;
				}
				else if (vector2d2[i] > num2)
				{
					double num3 = vector2d2[i] - num2;
					num += num3 * num3;
					vector2d2[i] = num2;
				}
			}
			return this.Center + vector2d2.x * this.AxisX + vector2d2.y * this.AxisY;
		}

		public static Box2d Merge(ref Box2d box0, ref Box2d box1)
		{
			Box2d box2d = default(Box2d);
			box2d.Center = 0.5 * (box0.Center + box1.Center);
			if (box0.AxisX.Dot(box1.AxisX) >= 0.0)
			{
				box2d.AxisX = 0.5 * (box0.AxisX + box1.AxisX);
				box2d.AxisX.Normalize(2.220446049250313E-16);
			}
			else
			{
				box2d.AxisX = 0.5 * (box0.AxisX - box1.AxisX);
				box2d.AxisX.Normalize(2.220446049250313E-16);
			}
			box2d.AxisY = -box2d.AxisX.Perp;
			Vector2d vector2d = default(Vector2d);
			Vector2d zero = Vector2d.Zero;
			Vector2d zero2 = Vector2d.Zero;
			Vector2dTuple4 vector2dTuple = default(Vector2dTuple4);
			box0.ComputeVertices(ref vector2dTuple);
			for (int i = 0; i < 4; i++)
			{
				vector2d = vector2dTuple[i] - box2d.Center;
				for (int j = 0; j < 2; j++)
				{
					double num = vector2d.Dot(box2d.Axis(j));
					if (num > zero2[j])
					{
						zero2[j] = num;
					}
					else if (num < zero[j])
					{
						zero[j] = num;
					}
				}
			}
			box1.ComputeVertices(ref vector2dTuple);
			for (int i = 0; i < 4; i++)
			{
				vector2d = vector2dTuple[i] - box2d.Center;
				for (int j = 0; j < 2; j++)
				{
					double num = vector2d.Dot(box2d.Axis(j));
					if (num > zero2[j])
					{
						zero2[j] = num;
					}
					else if (num < zero[j])
					{
						zero[j] = num;
					}
				}
			}
			box2d.Extent[0] = 0.5 * (zero2[0] - zero[0]);
			box2d.Extent[1] = 0.5 * (zero2[1] - zero[1]);
			box2d.Center += box2d.AxisX * (0.5 * (zero2[0] + zero[0]));
			box2d.Center += box2d.AxisY * (0.5 * (zero2[1] + zero[1]));
			return box2d;
		}

		public static implicit operator Box2d(Box2f v)
		{
			return new Box2d(v.Center, v.AxisX, v.AxisY, v.Extent);
		}

		public static explicit operator Box2f(Box2d v)
		{
			return new Box2f((Vector2f)v.Center, (Vector2f)v.AxisX, (Vector2f)v.AxisY, (Vector2f)v.Extent);
		}

		public Vector2d Center;

		public Vector2d AxisX;

		public Vector2d AxisY;

		public Vector2d Extent;

		public static readonly Box2d Empty = new Box2d(Vector2d.Zero);
	}
}
