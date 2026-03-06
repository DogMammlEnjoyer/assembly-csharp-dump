using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public struct DVector2
	{
		public static DVector2 Subtract(DVector2 a, DVector2 b)
		{
			return new DVector2(a.x - b.x, a.y - b.y);
		}

		public DVector2(double xx, double yy)
		{
			this.x = xx;
			this.y = yy;
		}

		public DVector2(DVector2 r)
		{
			this.x = r.x;
			this.y = r.y;
		}

		public Vector2 GetVector2()
		{
			return new Vector2((float)this.x, (float)this.y);
		}

		public bool IsContainedIn(DRect r)
		{
			return this.x >= r.x && this.y >= r.y && this.x <= r.x + r.width && this.y <= r.y + r.height;
		}

		public bool IsContainedInWithMargin(DRect r)
		{
			return this.x >= r.x - DVector2.epsilon && this.y >= r.y - DVector2.epsilon && this.x <= r.x + r.width + DVector2.epsilon && this.y <= r.y + r.height + DVector2.epsilon;
		}

		public override string ToString()
		{
			return string.Format("({0},{1})", this.x, this.y);
		}

		public string ToString(string formatS)
		{
			return string.Format("({0},{1})", this.x.ToString(formatS), this.y.ToString(formatS));
		}

		public static double Distance(DVector2 a, DVector2 b)
		{
			double num = b.x - a.x;
			double num2 = b.y - a.y;
			return Math.Sqrt(num * num + num2 * num2);
		}

		private static double epsilon = 1E-05;

		public double x;

		public double y;
	}
}
