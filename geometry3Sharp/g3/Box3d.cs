using System;
using System.Collections.Generic;

namespace g3
{
	public struct Box3d
	{
		public Box3d(Vector3d center)
		{
			this.Center = center;
			this.AxisX = Vector3d.AxisX;
			this.AxisY = Vector3d.AxisY;
			this.AxisZ = Vector3d.AxisZ;
			this.Extent = Vector3d.Zero;
		}

		public Box3d(Vector3d center, Vector3d x, Vector3d y, Vector3d z, Vector3d extent)
		{
			this.Center = center;
			this.AxisX = x;
			this.AxisY = y;
			this.AxisZ = z;
			this.Extent = extent;
		}

		public Box3d(Vector3d center, Vector3d extent)
		{
			this.Center = center;
			this.Extent = extent;
			this.AxisX = Vector3d.AxisX;
			this.AxisY = Vector3d.AxisY;
			this.AxisZ = Vector3d.AxisZ;
		}

		public Box3d(AxisAlignedBox3d aaBox)
		{
			this.Extent = new Vector3f(aaBox.Width * 0.5, aaBox.Height * 0.5, aaBox.Depth * 0.5);
			this.Center = aaBox.Center;
			this.AxisX = Vector3d.AxisX;
			this.AxisY = Vector3d.AxisY;
			this.AxisZ = Vector3d.AxisZ;
		}

		public Box3d(Frame3f frame, Vector3d extent)
		{
			this.Center = frame.Origin;
			this.AxisX = frame.X;
			this.AxisY = frame.Y;
			this.AxisZ = frame.Z;
			this.Extent = extent;
		}

		public Box3d(Segment3d seg)
		{
			this.Center = seg.Center;
			this.AxisZ = seg.Direction;
			Vector3d.MakePerpVectors(ref this.AxisZ, out this.AxisX, out this.AxisY);
			this.Extent = new Vector3d(0.0, 0.0, seg.Extent);
		}

		public Vector3d Axis(int i)
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

		public Vector3d[] ComputeVertices()
		{
			Vector3d[] array = new Vector3d[8];
			this.ComputeVertices(array);
			return array;
		}

		public void ComputeVertices(Vector3d[] vertex)
		{
			Vector3d v = this.Extent.x * this.AxisX;
			Vector3d v2 = this.Extent.y * this.AxisY;
			Vector3d v3 = this.Extent.z * this.AxisZ;
			vertex[0] = this.Center - v - v2 - v3;
			vertex[1] = this.Center + v - v2 - v3;
			vertex[2] = this.Center + v + v2 - v3;
			vertex[3] = this.Center - v + v2 - v3;
			vertex[4] = this.Center - v - v2 + v3;
			vertex[5] = this.Center + v - v2 + v3;
			vertex[6] = this.Center + v + v2 + v3;
			vertex[7] = this.Center - v + v2 + v3;
		}

		public IEnumerable<Vector3d> VerticesItr()
		{
			Vector3d extAxis0 = this.Extent.x * this.AxisX;
			Vector3d extAxis = this.Extent.y * this.AxisY;
			Vector3d extAxis2 = this.Extent.z * this.AxisZ;
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

		public AxisAlignedBox3d ToAABB()
		{
			Vector3d v = this.Extent.x * this.AxisX;
			Vector3d v2 = this.Extent.y * this.AxisY;
			Vector3d v3 = this.Extent.z * this.AxisZ;
			AxisAlignedBox3d result = new AxisAlignedBox3d(this.Center - v - v2 - v3);
			result.Contain(this.Center + v - v2 - v3);
			result.Contain(this.Center + v + v2 - v3);
			result.Contain(this.Center - v + v2 - v3);
			result.Contain(this.Center - v - v2 + v3);
			result.Contain(this.Center + v - v2 + v3);
			result.Contain(this.Center + v + v2 + v3);
			result.Contain(this.Center - v + v2 + v3);
			return result;
		}

		public Vector3d Corner(int i)
		{
			return this.Center + (((i & 1) != 0 ^ (i & 2) != 0) ? (this.Extent.x * this.AxisX) : (-this.Extent.x * this.AxisX)) + ((i / 2 % 2 == 0) ? (-this.Extent.y * this.AxisY) : (this.Extent.y * this.AxisY)) + ((i < 4) ? (-this.Extent.z * this.AxisZ) : (this.Extent.z * this.AxisZ));
		}

		public double MaxExtent
		{
			get
			{
				return Math.Max(this.Extent.x, Math.Max(this.Extent.y, this.Extent.z));
			}
		}

		public double MinExtent
		{
			get
			{
				return Math.Min(this.Extent.x, Math.Max(this.Extent.y, this.Extent.z));
			}
		}

		public Vector3d Diagonal
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
				return 2.0 * this.Extent.x * 2.0 * this.Extent.y * 2.0 * this.Extent.z;
			}
		}

		public void Contain(Vector3d v)
		{
			Vector3d vector3d = v - this.Center;
			for (int i = 0; i < 3; i++)
			{
				double num = vector3d.Dot(this.Axis(i));
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

		public void Contain(IEnumerable<Vector3d> points)
		{
			IEnumerator<Vector3d> enumerator = points.GetEnumerator();
			enumerator.MoveNext();
			Vector3d vector3d = enumerator.Current - this.Center;
			Vector3d vector3d2 = new Vector3d(vector3d.Dot(this.AxisX), vector3d.Dot(this.AxisY), vector3d.Dot(this.AxisZ));
			Vector3d vector3d3 = vector3d2;
			while (enumerator.MoveNext())
			{
				Vector3d v = enumerator.Current;
				vector3d = v - this.Center;
				double num = vector3d.Dot(this.AxisX);
				if (num < vector3d2[0])
				{
					vector3d2[0] = num;
				}
				else if (num > vector3d3[0])
				{
					vector3d3[0] = num;
				}
				double num2 = vector3d.Dot(this.AxisY);
				if (num2 < vector3d2[1])
				{
					vector3d2[1] = num2;
				}
				else if (num2 > vector3d3[1])
				{
					vector3d3[1] = num2;
				}
				double num3 = vector3d.Dot(this.AxisZ);
				if (num3 < vector3d2[2])
				{
					vector3d2[2] = num3;
				}
				else if (num3 > vector3d3[2])
				{
					vector3d3[2] = num3;
				}
			}
			for (int i = 0; i < 3; i++)
			{
				this.Center += 0.5 * (vector3d2[i] + vector3d3[i]) * this.Axis(i);
				this.Extent[i] = 0.5 * (vector3d3[i] - vector3d2[i]);
			}
		}

		public void Contain(Box3d o)
		{
			Vector3d[] array = o.ComputeVertices();
			for (int i = 0; i < 8; i++)
			{
				this.Contain(array[i]);
			}
		}

		public bool Contains(Vector3d v)
		{
			Vector3d vector3d = v - this.Center;
			return Math.Abs(vector3d.Dot(this.AxisX)) <= this.Extent.x && Math.Abs(vector3d.Dot(this.AxisY)) <= this.Extent.y && Math.Abs(vector3d.Dot(this.AxisZ)) <= this.Extent.z;
		}

		public void Expand(double f)
		{
			this.Extent += f;
		}

		public void Translate(Vector3d v)
		{
			this.Center += v;
		}

		public void Scale(Vector3d s)
		{
			this.Center *= s;
			this.Extent *= s;
			this.AxisX *= s;
			this.AxisX.Normalize(2.220446049250313E-16);
			this.AxisY *= s;
			this.AxisY.Normalize(2.220446049250313E-16);
			this.AxisZ *= s;
			this.AxisZ.Normalize(2.220446049250313E-16);
		}

		public void ScaleExtents(Vector3d s)
		{
			this.Extent *= s;
		}

		public double DistanceSquared(Vector3d v)
		{
			v -= this.Center;
			double num = 0.0;
			Vector3d vector3d = default(Vector3d);
			for (int i = 0; i < 3; i++)
			{
				vector3d[i] = this.Axis(i).Dot(ref v);
				if (vector3d[i] < -this.Extent[i])
				{
					double num2 = vector3d[i] + this.Extent[i];
					num += num2 * num2;
					vector3d[i] = -this.Extent[i];
				}
				else if (vector3d[i] > this.Extent[i])
				{
					double num2 = vector3d[i] - this.Extent[i];
					num += num2 * num2;
					vector3d[i] = this.Extent[i];
				}
			}
			return num;
		}

		public Vector3d ClosestPoint(Vector3d v)
		{
			v -= this.Center;
			double num = 0.0;
			Vector3d vector3d = default(Vector3d);
			for (int i = 0; i < 3; i++)
			{
				vector3d[i] = this.Axis(i).Dot(ref v);
				double num2 = this.Extent[i];
				if (vector3d[i] < -num2)
				{
					double num3 = vector3d[i] + num2;
					num += num3 * num3;
					vector3d[i] = -num2;
				}
				else if (vector3d[i] > num2)
				{
					double num3 = vector3d[i] - num2;
					num += num3 * num3;
					vector3d[i] = num2;
				}
			}
			return this.Center + vector3d.x * this.AxisX + vector3d.y * this.AxisY + vector3d.z * this.AxisZ;
		}

		public static Box3d Merge(ref Box3d box0, ref Box3d box1)
		{
			Box3d box3d = default(Box3d);
			box3d.Center = 0.5 * (box0.Center + box1.Center);
			Quaterniond q = default(Quaterniond);
			Quaterniond q2 = default(Quaterniond);
			Matrix3d matrix3d = new Matrix3d(ref box0.AxisX, ref box0.AxisY, ref box0.AxisZ, false);
			q.SetFromRotationMatrix(ref matrix3d);
			Matrix3d matrix3d2 = new Matrix3d(ref box1.AxisX, ref box1.AxisY, ref box1.AxisZ, false);
			q2.SetFromRotationMatrix(ref matrix3d2);
			if (q.Dot(q2) < 0.0)
			{
				q2 = -q2;
			}
			Quaterniond quaterniond = q + q2;
			double d = 1.0 / Math.Sqrt(quaterniond.Dot(quaterniond));
			quaterniond *= d;
			Matrix3d matrix3d3 = quaterniond.ToRotationMatrix();
			box3d.AxisX = matrix3d3.Column(0);
			box3d.AxisY = matrix3d3.Column(1);
			box3d.AxisZ = matrix3d3.Column(2);
			Vector3d[] array = new Vector3d[8];
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			box0.ComputeVertices(array);
			for (int i = 0; i < 8; i++)
			{
				Vector3d vector3d = array[i] - box3d.Center;
				for (int j = 0; j < 3; j++)
				{
					double num = box3d.Axis(j).Dot(ref vector3d);
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
			box1.ComputeVertices(array);
			for (int i = 0; i < 8; i++)
			{
				Vector3d vector3d2 = array[i] - box3d.Center;
				for (int j = 0; j < 3; j++)
				{
					double num = box3d.Axis(j).Dot(ref vector3d2);
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
			for (int j = 0; j < 3; j++)
			{
				box3d.Center += 0.5 * (zero2[j] + zero[j]) * box3d.Axis(j);
				box3d.Extent[j] = 0.5 * (zero2[j] - zero[j]);
			}
			return box3d;
		}

		public static implicit operator Box3d(Box3f v)
		{
			return new Box3d(v.Center, v.AxisX, v.AxisY, v.AxisZ, v.Extent);
		}

		public static explicit operator Box3f(Box3d v)
		{
			return new Box3f((Vector3f)v.Center, (Vector3f)v.AxisX, (Vector3f)v.AxisY, (Vector3f)v.AxisZ, (Vector3f)v.Extent);
		}

		public Vector3d Center;

		public Vector3d AxisX;

		public Vector3d AxisY;

		public Vector3d AxisZ;

		public Vector3d Extent;

		public static readonly Box3d Empty = new Box3d(Vector3d.Zero);

		public static readonly Box3d UnitZeroCentered = new Box3d(Vector3d.Zero, 0.5 * Vector3d.One);

		public static readonly Box3d UnitPositive = new Box3d(0.5 * Vector3d.One, 0.5 * Vector3d.One);
	}
}
