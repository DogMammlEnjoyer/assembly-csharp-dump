using System;

namespace UnityEngine.Rendering.Universal
{
	internal struct IntPoint
	{
		public IntPoint(long X, long Y)
		{
			this.X = X;
			this.Y = Y;
			this.NX = 0.0;
			this.NY = 0.0;
			this.N = -1L;
			this.D = 0L;
		}

		public IntPoint(double x, double y)
		{
			this.X = (long)x;
			this.Y = (long)y;
			this.NX = 0.0;
			this.NY = 0.0;
			this.N = -1L;
			this.D = 0L;
		}

		public IntPoint(IntPoint pt)
		{
			this.X = pt.X;
			this.Y = pt.Y;
			this.NX = pt.NX;
			this.NY = pt.NY;
			this.N = pt.N;
			this.D = pt.D;
		}

		public static bool operator ==(IntPoint a, IntPoint b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(IntPoint a, IntPoint b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is IntPoint)
			{
				IntPoint intPoint = (IntPoint)obj;
				return this.X == intPoint.X && this.Y == intPoint.Y;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public long N;

		public long X;

		public long Y;

		public long D;

		public double NX;

		public double NY;
	}
}
