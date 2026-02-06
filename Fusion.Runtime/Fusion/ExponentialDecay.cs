using System;

namespace Fusion
{
	internal class ExponentialDecay
	{
		public double Fraction { get; internal set; }

		public double Time { get; internal set; }

		public double TimeScale { get; internal set; }

		public double Rate
		{
			get
			{
				return Math.Log(this.Fraction) / (this.Time * this.TimeScale);
			}
		}

		public double Calculate(double elapsed)
		{
			return Math.Exp(this.Rate * elapsed);
		}

		public double CalculateLimit(double period)
		{
			double num = this.Calculate(period);
			return 1.0 / (1.0 - num);
		}

		public ExponentialDecay(double fraction, double time)
		{
			Assert.Check<bool>(fraction > 0.0, fraction <= 1.0);
			Assert.Check(time > 0.0);
			this.Fraction = fraction;
			this.Time = time;
			this.TimeScale = 1.0;
		}
	}
}
