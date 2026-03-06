using System;

namespace Fusion.Sockets
{
	public struct NetConfigSimulationOscillator
	{
		public double GetCurveValue(Random rng, double elapsedSecs)
		{
			bool flag = this.Period == 0.0 && this.Shape > NetConfigSimulationOscillator.WaveShape.Noise;
			double num;
			if (flag)
			{
				num = this.Min + (this.Max - this.Min) * 0.5;
			}
			else
			{
				bool flag2 = this.Min == this.Max;
				if (flag2)
				{
					num = this.Min;
				}
				else
				{
					double num2;
					switch (this.Shape)
					{
					case NetConfigSimulationOscillator.WaveShape.Noise:
						num2 = rng.NextDouble();
						break;
					case NetConfigSimulationOscillator.WaveShape.Sine:
						num2 = Math.Sin(elapsedSecs * 2.0 * 3.141592653589793 / this.Period) * 0.5 + 0.5;
						break;
					case NetConfigSimulationOscillator.WaveShape.Square:
						num2 = (double)((elapsedSecs / this.Period % 1.0 > 0.5) ? 1 : 0);
						break;
					case NetConfigSimulationOscillator.WaveShape.Triangle:
						num2 = Math.Abs(elapsedSecs / this.Period % 1.0 * 2.0 - 1.0);
						break;
					case NetConfigSimulationOscillator.WaveShape.Saw:
						num2 = elapsedSecs / this.Period % 1.0;
						break;
					case NetConfigSimulationOscillator.WaveShape.ReverseSaw:
						num2 = 1.0 - elapsedSecs / this.Period % 1.0;
						break;
					default:
						num2 = 0.0;
						break;
					}
					num2 = ((num2 > this.Threshold) ? num2 : 0.0);
					num = this.Min + (this.Max - this.Min) * num2;
				}
			}
			double num3 = this.Additional * (rng.NextDouble() - 0.5);
			return num + num3;
		}

		public NetConfigSimulationOscillator.WaveShape Shape;

		public double Min;

		public double Max;

		public double Period;

		public double Threshold;

		public double Additional;

		public enum WaveShape
		{
			Noise,
			Sine,
			Square,
			Triangle,
			Saw,
			ReverseSaw
		}
	}
}
