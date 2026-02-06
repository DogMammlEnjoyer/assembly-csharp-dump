using System;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit)]
	public struct TickTimer : INetworkStruct
	{
		public static TickTimer None
		{
			get
			{
				return default(TickTimer);
			}
		}

		public bool IsRunning
		{
			get
			{
				return this._target > 0;
			}
		}

		public int? TargetTick
		{
			get
			{
				return new int?((this._target > 0) ? this._target : 0);
			}
		}

		public bool Expired(NetworkRunner runner)
		{
			return BehaviourUtils.IsAlive(runner) && runner.IsRunning && this._target > 0 && this._target <= runner.Simulation.Tick;
		}

		public bool ExpiredOrNotRunning(NetworkRunner runner)
		{
			return this._target == 0 || !runner.IsRunning || this.Expired(runner);
		}

		public int? RemainingTicks(NetworkRunner runner)
		{
			bool flag = BehaviourUtils.IsNotAlive(runner) || !runner.IsRunning;
			int? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool isRunning = this.IsRunning;
				if (isRunning)
				{
					result = new int?(Math.Max(0, this._target - runner.Simulation.Tick));
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public float? RemainingTime(NetworkRunner runner)
		{
			int? num = this.RemainingTicks(runner);
			bool flag = num != null;
			float? result;
			if (flag)
			{
				result = new float?((float)num.Value * runner.DeltaTime);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static TickTimer CreateFromSeconds(NetworkRunner runner, float delayInSeconds)
		{
			bool flag = BehaviourUtils.IsNotAlive(runner) || !runner.IsRunning;
			TickTimer result;
			if (flag)
			{
				result = default(TickTimer);
			}
			else
			{
				TickTimer tickTimer;
				tickTimer._target = runner.Simulation.Tick + (int)Math.Ceiling((double)(delayInSeconds / runner.DeltaTime));
				result = tickTimer;
			}
			return result;
		}

		public static TickTimer CreateFromTicks(NetworkRunner runner, int ticks)
		{
			bool flag = BehaviourUtils.IsNotAlive(runner) || !runner.IsRunning;
			TickTimer result;
			if (flag)
			{
				result = default(TickTimer);
			}
			else
			{
				TickTimer tickTimer;
				tickTimer._target = runner.Simulation.Tick + ticks;
				result = tickTimer;
			}
			return result;
		}

		public override string ToString()
		{
			return this._target.ToString();
		}

		[FieldOffset(0)]
		private int _target;
	}
}
