using System;

namespace Fusion
{
	internal class FixedFeedback : IFeedbackController
	{
		public FixedFeedback(double outputMin, double outputMax, double deadzoneMin, double deadzoneMax)
		{
			Assert.Check(!double.IsNaN(outputMin));
			Assert.Check(!double.IsNaN(outputMax));
			Assert.Check(!double.IsInfinity(outputMin));
			Assert.Check(!double.IsInfinity(outputMax));
			Assert.Check(outputMin <= outputMax);
			this._outputMin = outputMin;
			this._outputMax = outputMax;
			this._deadzoneMin = deadzoneMin;
			this._deadzoneMax = deadzoneMax;
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
			double num = target - sample;
			bool flag = num > this._deadzoneMax;
			if (flag)
			{
				this._output = this._outputMax;
			}
			else
			{
				bool flag2 = num < this._deadzoneMin;
				if (flag2)
				{
					this._output = this._outputMin;
				}
				else
				{
					this._output = 0.0;
				}
			}
		}

		void IFeedbackController.Reset()
		{
			this._output = 0.0;
		}

		void IFeedbackController.ResetOutput()
		{
			this._output = 0.0;
		}

		private readonly double _outputMin;

		private readonly double _outputMax;

		private readonly double _deadzoneMin;

		private readonly double _deadzoneMax;

		private double _output;
	}
}
