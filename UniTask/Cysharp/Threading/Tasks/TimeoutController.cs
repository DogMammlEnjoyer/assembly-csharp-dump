using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public sealed class TimeoutController : IDisposable
	{
		private static void CancelCancellationTokenSourceState(object state)
		{
			((CancellationTokenSource)state).Cancel();
		}

		public TimeoutController(DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update)
		{
			this.timeoutSource = new CancellationTokenSource();
			this.originalLinkCancellationTokenSource = null;
			this.linkedSource = null;
			this.delayType = delayType;
			this.delayTiming = delayTiming;
		}

		public TimeoutController(CancellationTokenSource linkCancellationTokenSource, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update)
		{
			this.timeoutSource = new CancellationTokenSource();
			this.originalLinkCancellationTokenSource = linkCancellationTokenSource;
			this.linkedSource = CancellationTokenSource.CreateLinkedTokenSource(this.timeoutSource.Token, linkCancellationTokenSource.Token);
			this.delayType = delayType;
			this.delayTiming = delayTiming;
		}

		public CancellationToken Timeout(int millisecondsTimeout)
		{
			return this.Timeout(TimeSpan.FromMilliseconds((double)millisecondsTimeout));
		}

		public CancellationToken Timeout(TimeSpan timeout)
		{
			if (this.originalLinkCancellationTokenSource != null && this.originalLinkCancellationTokenSource.IsCancellationRequested)
			{
				return this.originalLinkCancellationTokenSource.Token;
			}
			if (this.timeoutSource.IsCancellationRequested)
			{
				this.timeoutSource.Dispose();
				this.timeoutSource = new CancellationTokenSource();
				if (this.linkedSource != null)
				{
					this.linkedSource.Cancel();
					this.linkedSource.Dispose();
					this.linkedSource = CancellationTokenSource.CreateLinkedTokenSource(this.timeoutSource.Token, this.originalLinkCancellationTokenSource.Token);
				}
				PlayerLoopTimer playerLoopTimer = this.timer;
				if (playerLoopTimer != null)
				{
					playerLoopTimer.Dispose();
				}
				this.timer = null;
			}
			CancellationToken token = ((this.linkedSource != null) ? this.linkedSource : this.timeoutSource).Token;
			if (this.timer == null)
			{
				this.timer = PlayerLoopTimer.StartNew(timeout, false, this.delayType, this.delayTiming, token, TimeoutController.CancelCancellationTokenSourceStateDelegate, this.timeoutSource);
			}
			else
			{
				this.timer.Restart(timeout);
			}
			return token;
		}

		public bool IsTimeout()
		{
			return this.timeoutSource.IsCancellationRequested;
		}

		public void Reset()
		{
			PlayerLoopTimer playerLoopTimer = this.timer;
			if (playerLoopTimer == null)
			{
				return;
			}
			playerLoopTimer.Stop();
		}

		public void Dispose()
		{
			if (this.isDisposed)
			{
				return;
			}
			try
			{
				PlayerLoopTimer playerLoopTimer = this.timer;
				if (playerLoopTimer != null)
				{
					playerLoopTimer.Dispose();
				}
				this.timeoutSource.Cancel();
				this.timeoutSource.Dispose();
				if (this.linkedSource != null)
				{
					this.linkedSource.Cancel();
					this.linkedSource.Dispose();
				}
			}
			finally
			{
				this.isDisposed = true;
			}
		}

		private static readonly Action<object> CancelCancellationTokenSourceStateDelegate = new Action<object>(TimeoutController.CancelCancellationTokenSourceState);

		private CancellationTokenSource timeoutSource;

		private CancellationTokenSource linkedSource;

		private PlayerLoopTimer timer;

		private bool isDisposed;

		private readonly DelayType delayType;

		private readonly PlayerLoopTiming delayTiming;

		private readonly CancellationTokenSource originalLinkCancellationTokenSource;
	}
}
