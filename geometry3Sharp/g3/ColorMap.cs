using System;
using System.Collections.Generic;

namespace g3
{
	public class ColorMap
	{
		public ColorMap()
		{
			this.validRange = Interval1d.Empty;
		}

		public ColorMap(float[] t, Colorf[] c)
		{
			this.validRange = Interval1d.Empty;
			for (int i = 0; i < t.Length; i++)
			{
				this.AddPoint(t[i], c[i]);
			}
		}

		public void AddPoint(float t, Colorf c)
		{
			ColorMap.ColorPoint colorPoint = new ColorMap.ColorPoint
			{
				t = t,
				c = c
			};
			if (this.points.Count == 0)
			{
				this.points.Add(colorPoint);
				this.validRange.Contain((double)t);
				return;
			}
			if (t < this.points[0].t)
			{
				this.points.Insert(0, colorPoint);
				this.validRange.Contain((double)t);
				return;
			}
			for (int i = 0; i < this.points.Count; i++)
			{
				if (this.points[i].t == t)
				{
					this.points[i] = colorPoint;
					return;
				}
				if (this.points[i].t > t)
				{
					this.points.Insert(i, colorPoint);
					return;
				}
			}
			this.points.Add(colorPoint);
			this.validRange.Contain((double)t);
		}

		public Colorf Linear(float t)
		{
			if (t <= this.points[0].t)
			{
				return this.points[0].c;
			}
			int count = this.points.Count;
			if (t >= this.points[count - 1].t)
			{
				return this.points[count - 1].c;
			}
			for (int i = 1; i < this.points.Count; i++)
			{
				if (this.points[i].t > t)
				{
					ColorMap.ColorPoint colorPoint = this.points[i - 1];
					ColorMap.ColorPoint colorPoint2 = this.points[i];
					float num = (t - colorPoint.t) / (colorPoint2.t - colorPoint.t);
					return (1f - num) * colorPoint.c + num * colorPoint2.c;
				}
			}
			return this.points[count - 1].c;
		}

		private List<ColorMap.ColorPoint> points = new List<ColorMap.ColorPoint>();

		private Interval1d validRange;

		private struct ColorPoint
		{
			public float t;

			public Colorf c;
		}
	}
}
