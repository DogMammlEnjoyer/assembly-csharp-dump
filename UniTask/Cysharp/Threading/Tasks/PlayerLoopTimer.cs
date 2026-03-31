using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public abstract class PlayerLoopTimer : IDisposable, IPlayerLoopItem
	{
		protected PlayerLoopTimer(bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object> timerCallback, object state)
		{
			this.periodic = periodic;
			this.playerLoopTiming = playerLoopTiming;
			this.cancellationToken = cancellationToken;
			this.timerCallback = timerCallback;
			this.state = state;
		}

		public static PlayerLoopTimer Create(TimeSpan interval, bool periodic, DelayType delayType, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object> timerCallback, object state)
		{
			switch (delayType)
			{
			case DelayType.UnscaledDeltaTime:
				return new IgnoreTimeScalePlayerLoopTimer(interval, periodic, playerLoopTiming, cancellationToken, timerCallback, state);
			case DelayType.Realtime:
				return new RealtimePlayerLoopTimer(interval, periodic, playerLoopTiming, cancellationToken, timerCallback, state);
			}
			return new DeltaTimePlayerLoopTimer(interval, periodic, playerLoopTiming, cancellationToken, timerCallback, state);
		}

		public static PlayerLoopTimer StartNew(TimeSpan interval, bool periodic, DelayType delayType, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object> timerCallback, object state)
		{
			PlayerLoopTimer playerLoopTimer = PlayerLoopTimer.Create(interval, periodic, delayType, playerLoopTiming, cancellationToken, timerCallback, state);
			playerLoopTimer.Restart();
			return playerLoopTimer;
		}

		public void Restart()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(null);
			}
			this.ResetCore(null);
			if (!this.isRunning)
			{
				this.isRunning = true;
				PlayerLoopHelper.AddAction(this.playerLoopTiming, this);
			}
			this.tryStop = false;
		}

		public void Restart(TimeSpan interval)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(null);
			}
			this.ResetCore(new TimeSpan?(interval));
			if (!this.isRunning)
			{
				this.isRunning = true;
				PlayerLoopHelper.AddAction(this.playerLoopTiming, this);
			}
			this.tryStop = false;
		}

		public void Stop()
		{
			this.tryStop = true;
		}

		protected abstract void ResetCore(TimeSpan? newInterval);

		public void Dispose()
		{
			this.isDisposed = true;
		}

		bool IPlayerLoopItem.MoveNext()
		{
			if (this.isDisposed)
			{
				this.isRunning = false;
				return false;
			}
			if (this.tryStop)
			{
				this.isRunning = false;
				return false;
			}
			if (this.cancellationToken.IsCancellationRequested)
			{
				this.isRunning = false;
				return false;
			}
			if (this.MoveNextCore())
			{
				return true;
			}
			this.timerCallback(this.state);
			if (this.periodic)
			{
				this.ResetCore(null);
				return true;
			}
			this.isRunning = false;
			return false;
		}

		protected abstract bool MoveNextCore();

		private readonly CancellationToken cancellationToken;

		private readonly Action<object> timerCallback;

		private readonly object state;

		private readonly PlayerLoopTiming playerLoopTiming;

		private readonly bool periodic;

		private bool isRunning;

		private bool tryStop;

		private bool isDisposed;
	}
}
