using System;

namespace Unity.Cinemachine
{
	internal struct Point64
	{
		public Point64(Point64 pt)
		{
			this.X = pt.X;
			this.Y = pt.Y;
		}

		public Point64(long x, long y)
		{
			this.X = x;
			this.Y = y;
		}

		public Point64(double x, double y)
		{
			this.X = (long)Math.Round(x);
			this.Y = (long)Math.Round(y);
		}

		public Point64(PointD pt)
		{
			this.X = (long)Math.Round(pt.x);
			this.Y = (long)Math.Round(pt.y);
		}

		public Point64(Point64 pt, double scale)
		{
			this.X = (long)Math.Round((double)pt.X * scale);
			this.Y = (long)Math.Round((double)pt.Y * scale);
		}

		public Point64(PointD pt, double scale)
		{
			this.X = (long)Math.Round(pt.x * scale);
			this.Y = (long)Math.Round(pt.y * scale);
		}

		public static bool operator ==(Point64 lhs, Point64 rhs)
		{
			return lhs.X == rhs.X && lhs.Y == rhs.Y;
		}

		public static bool operator !=(Point64 lhs, Point64 rhs)
		{
			return lhs.X != rhs.X || lhs.Y != rhs.Y;
		}

		public static Point64 operator +(Point64 lhs, Point64 rhs)
		{
			return new Point64(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}

		public static Point64 operator -(Point64 lhs, Point64 rhs)
		{
			return new Point64(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}

		public override string ToString()
		{
			return string.Format("{0},{1} ", this.X, this.Y);
		}

		public override bool Equals(object obj)
		{
			if (obj is Point64)
			{
				Point64 rhs = (Point64)obj;
				return this == rhs;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public long X;

		public long Y;
	}
}
