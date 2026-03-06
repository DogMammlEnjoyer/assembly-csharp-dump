using System;

namespace Fusion
{
	internal struct TimerDelta
	{
		public bool IsRunning
		{
			get
			{
				return this._timer.IsRunning;
			}
		}

		public double Consume()
		{
			double elapsedInSeconds = this._timer.ElapsedInSeconds;
			double result = Math.Max(elapsedInSeconds - this._timerLast, 0.0);
			this._timerLast = elapsedInSeconds;
			return result;
		}

		public double Peek()
		{
			return Math.Max(this._timer.ElapsedInSeconds - this._timerLast, 0.0);
		}

		public static TimerDelta StartNew()
		{
			return new TimerDelta
			{
				_timer = Timer.StartNew()
			};
		}

		private Timer _timer;

		private double _timerLast;
	}
}
