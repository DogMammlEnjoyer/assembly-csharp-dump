using System;
using System.Collections.Generic;

namespace g3
{
	public class ScalarMap
	{
		public ScalarMap()
		{
			this.validRange = Interval1d.Empty;
		}

		public void AddPoint(double t, double value)
		{
			ScalarMap.Sample sample = new ScalarMap.Sample
			{
				t = t,
				value = value
			};
			if (this.points.Count == 0)
			{
				this.points.Add(sample);
				this.validRange.Contain(t);
				return;
			}
			if (t < this.points[0].t)
			{
				this.points.Insert(0, sample);
				this.validRange.Contain(t);
				return;
			}
			for (int i = 0; i < this.points.Count; i++)
			{
				if (this.points[i].t == t)
				{
					this.points[i] = sample;
					return;
				}
				if (this.points[i].t > t)
				{
					this.points.Insert(i, sample);
					return;
				}
			}
			this.points.Add(sample);
			this.validRange.Contain(t);
		}

		public double Linear(double t)
		{
			if (t <= this.points[0].t)
			{
				return this.points[0].value;
			}
			int count = this.points.Count;
			if (t >= this.points[count - 1].t)
			{
				return this.points[count - 1].value;
			}
			for (int i = 1; i < this.points.Count; i++)
			{
				if (this.points[i].t > t)
				{
					ScalarMap.Sample sample = this.points[i - 1];
					ScalarMap.Sample sample2 = this.points[i];
					double num = (t - sample.t) / (sample2.t - sample.t);
					return (1.0 - num) * sample.value + num * sample2.value;
				}
			}
			return this.points[count - 1].value;
		}

		private List<ScalarMap.Sample> points = new List<ScalarMap.Sample>();

		private Interval1d validRange;

		private struct Sample
		{
			public double t;

			public double value;
		}
	}
}
