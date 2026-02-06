using System;

namespace Fusion
{
	internal class VariableFeedback : IFeedbackController
	{
		public VariableFeedback(double Kp, double Ki, double Kd, double outputMin, double outputMax)
		{
			Assert.Check(!double.IsNaN(Kp));
			Assert.Check(!double.IsInfinity(Kp));
			Assert.Check(!double.IsNaN(Ki));
			Assert.Check(!double.IsInfinity(Ki));
			Assert.Check(!double.IsNaN(Kd));
			Assert.Check(!double.IsInfinity(Kd));
			Assert.Check(!double.IsNaN(outputMin));
			Assert.Check(!double.IsNaN(outputMax));
			Assert.Check(outputMin <= outputMax);
			this._Kp = Kp;
			this._Ki = this._Kp * Ki;
			this._Kd = this._Kp * Kd;
			this._outputMin = outputMin;
			this._outputMax = outputMax;
			this._lastSample = 0.0;
			this._sum = 0.0;
			this._output = 0.0;
		}

		double IFeedbackController.Output()
		{
			return this._output;
		}

		void IFeedbackController.Update(double sample, double target, double dt)
		{
			Assert.Check(!double.IsNaN(sample));
			Assert.Check(!double.IsInfinity(sample));
			Assert.Check(!double.IsNaN(target));
			Assert.Check(!double.IsInfinity(target));
			Assert.Check(!double.IsNaN(dt));
			Assert.Check(!double.IsInfinity(dt));
			Assert.Check(dt >= double.Epsilon);
			double num = target - sample;
			double num2 = sample - this._lastSample;
			this._output = this._Kp * num;
			bool flag = this._output > this._outputMax;
			if (flag)
			{
				this._output = this._outputMax;
			}
			else
			{
				bool flag2 = this._output < this._outputMin;
				if (flag2)
				{
					this._output = this._outputMin;
				}
			}
			this._sum += this._Ki * dt * num - this._Kd / dt * num2;
			this._output += this._sum;
			bool flag3 = this._output > this._outputMax;
			if (flag3)
			{
				this._sum -= this._output - this._outputMax;
				this._output = this._outputMax;
			}
			else
			{
				bool flag4 = this._output < this._outputMin;
				if (flag4)
				{
					this._sum += this._outputMin - this._output;
					this._output = this._outputMin;
				}
			}
		}

		void IFeedbackController.Reset()
		{
			this._lastSample = 0.0;
			this._sum = 0.0;
			this._output = 0.0;
		}

		void IFeedbackController.ResetOutput()
		{
			this._output = 0.0;
		}

		private readonly double _Kp;

		private readonly double _Ki;

		private readonly double _Kd;

		private readonly double _outputMin;

		private readonly double _outputMax;

		private double _lastSample;

		private double _sum;

		private double _output;
	}
}
