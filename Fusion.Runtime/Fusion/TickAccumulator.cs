using System;

namespace Fusion
{
	public struct TickAccumulator
	{
		public int Pending
		{
			get
			{
				return this._ticks;
			}
		}

		public double Remainder
		{
			get
			{
				return this._time;
			}
		}

		public bool Running
		{
			get
			{
				return this._running;
			}
		}

		public double TimeScale
		{
			get
			{
				return this._scale;
			}
			set
			{
				Assert.Check(value > 0.0);
				this._scale = value;
			}
		}

		public float Alpha(double step)
		{
			return (float)Maths.Clamp01((double)this._ticks / step);
		}

		public void AddTicks(int ticks)
		{
			this._ticks += ticks;
		}

		public void AddTime(double dt, double step, int? maxTicks = null)
		{
			Assert.Check(dt >= 0.0);
			Assert.Check(step > 0.0);
			bool flag = !this._running;
			if (!flag)
			{
				Assert.Check(this._scale > 0.0);
				Assert.Check(!double.IsInfinity(dt));
				this._time += dt * this._scale;
				while (this._time >= step)
				{
					this._time -= step;
					this._ticks++;
					bool flag2 = maxTicks != null && this._ticks >= maxTicks.Value;
					if (flag2)
					{
						this._time = 0.0;
						this._ticks = maxTicks.Value;
						break;
					}
				}
			}
		}

		public void Stop()
		{
			this._running = false;
		}

		public void Start()
		{
			this._running = true;
		}

		public bool ConsumeTick(out bool last)
		{
			Assert.Check(this._ticks >= 0);
			bool flag = this._ticks > 0;
			bool result;
			if (flag)
			{
				this._ticks--;
				last = (this._ticks == 0);
				result = true;
			}
			else
			{
				last = false;
				result = false;
			}
			return result;
		}

		public static TickAccumulator StartNew()
		{
			TickAccumulator result = default(TickAccumulator);
			result.TimeScale = 1.0;
			result.Start();
			return result;
		}

		private double _time;

		private double _scale;

		private int _ticks;

		private bool _running;
	}
}
