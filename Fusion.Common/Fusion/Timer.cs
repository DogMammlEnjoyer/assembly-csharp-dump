using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public struct Timer
	{
		public static Timer StartNew()
		{
			Timer result = default(Timer);
			result.Start();
			return result;
		}

		public long ElapsedInTicks
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this._running == 1) ? (this._elapsed + this.GetDelta()) : this._elapsed;
			}
		}

		public double ElapsedInMilliseconds
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.ElapsedInSeconds * 1000.0;
			}
		}

		public double ElapsedInSeconds
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (double)this.ElapsedInTicks / (double)Stopwatch.Frequency;
			}
		}

		public bool IsRunning
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._running == 1;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Start()
		{
			bool flag = this._running == 0;
			if (flag)
			{
				this._start = Stopwatch.GetTimestamp();
				this._running = 1;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Stop()
		{
			long delta = this.GetDelta();
			bool flag = this._running == 1;
			if (flag)
			{
				this._elapsed += delta;
				this._running = 0;
				bool flag2 = this._elapsed < 0L;
				if (flag2)
				{
					this._elapsed = 0L;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			this._elapsed = 0L;
			this._running = 0;
			this._start = 0L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Restart()
		{
			this._elapsed = 0L;
			this._running = 1;
			this._start = Stopwatch.GetTimestamp();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private long GetDelta()
		{
			return Stopwatch.GetTimestamp() - this._start;
		}

		private long _start;

		private long _elapsed;

		private byte _running;
	}
}
