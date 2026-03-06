using System;

namespace Unity.Cinemachine
{
	internal struct PointD
	{
		public PointD(PointD pt)
		{
			this.x = pt.x;
			this.y = pt.y;
		}

		public PointD(Point64 pt)
		{
			this.x = (double)pt.X;
			this.y = (double)pt.Y;
		}

		public PointD(PointD pt, double scale)
		{
			this.x = pt.x * scale;
			this.y = pt.y * scale;
		}

		public PointD(Point64 pt, double scale)
		{
			this.x = (double)pt.X * scale;
			this.y = (double)pt.Y * scale;
		}

		public PointD(long x, long y)
		{
			this.x = (double)x;
			this.y = (double)y;
		}

		public PointD(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return string.Format("{0:F},{1:F} ", this.x, this.y);
		}

		private static bool IsAlmostZero(double value)
		{
			return Math.Abs(value) <= 1E-15;
		}

		public static bool operator ==(PointD lhs, PointD rhs)
		{
			return PointD.IsAlmostZero(lhs.x - rhs.x) && PointD.IsAlmostZero(lhs.y - rhs.y);
		}

		public static bool operator !=(PointD lhs, PointD rhs)
		{
			return !PointD.IsAlmostZero(lhs.x - rhs.x) || !PointD.IsAlmostZero(lhs.y - rhs.y);
		}

		public override bool Equals(object obj)
		{
			if (obj is PointD)
			{
				PointD rhs = (PointD)obj;
				return this == rhs;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public double x;

		public double y;
	}
}
