using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class TimerFrame : IUniTaskAsyncEnumerable<AsyncUnit>
	{
		public TimerFrame(int dueTimeFrameCount, int? periodFrameCount, PlayerLoopTiming updateTiming)
		{
			this.updateTiming = updateTiming;
			this.dueTimeFrameCount = dueTimeFrameCount;
			this.periodFrameCount = periodFrameCount;
		}

		public IUniTaskAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new TimerFrame._TimerFrame(this.dueTimeFrameCount, this.periodFrameCount, this.updateTiming, cancellationToken);
		}

		private readonly PlayerLoopTiming updateTiming;

		private readonly int dueTimeFrameCount;

		private readonly int? periodFrameCount;

		private class _TimerFrame : MoveNextSource, IUniTaskAsyncEnumerator<AsyncUnit>, IUniTaskAsyncDisposable, IPlayerLoopItem
		{
			public _TimerFrame(int dueTimeFrameCount, int? periodFrameCount, PlayerLoopTiming updateTiming, CancellationToken cancellationToken)
			{
				if (dueTimeFrameCount <= 0)
				{
					dueTimeFrameCount = 0;
				}
				if (periodFrameCount != null)
				{
					int? num = periodFrameCount;
					int num2 = 0;
					if (num.GetValueOrDefault() <= num2 & num != null)
					{
						periodFrameCount = new int?(1);
					}
				}
				this.initialFrame = (PlayerLoopHelper.IsMainThread ? Time.frameCount : -1);
				this.dueTimePhase = true;
				this.dueTimeFrameCount = dueTimeFrameCount;
				this.periodFrameCount = periodFrameCount;
				this.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(updateTiming, this);
			}

			public AsyncUnit Current
			{
				get
				{
					return default(AsyncUnit);
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				if (this.disposed || this.cancellationToken.IsCancellationRequested || this.completed)
				{
					return CompletedTasks.False;
				}
				this.currentFrame = 0;
				this.completionSource.Reset();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			public UniTask DisposeAsync()
			{
				if (!this.disposed)
				{
					this.disposed = true;
				}
				return default(UniTask);
			}

			public bool MoveNext()
			{
				if (this.disposed || this.cancellationToken.IsCancellationRequested)
				{
					this.completionSource.TrySetResult(false);
					return false;
				}
				if (this.dueTimePhase)
				{
					if (this.currentFrame == 0)
					{
						if (this.dueTimeFrameCount == 0)
						{
							this.dueTimePhase = false;
							this.completionSource.TrySetResult(true);
							return true;
						}
						if (this.initialFrame == Time.frameCount)
						{
							return true;
						}
					}
					int num = this.currentFrame + 1;
					this.currentFrame = num;
					if (num >= this.dueTimeFrameCount)
					{
						this.dueTimePhase = false;
						this.completionSource.TrySetResult(true);
					}
				}
				else
				{
					if (this.periodFrameCount == null)
					{
						this.completed = true;
						this.completionSource.TrySetResult(false);
						return false;
					}
					int num = this.currentFrame + 1;
					this.currentFrame = num;
					int num2 = num;
					int? num3 = this.periodFrameCount;
					if (num2 >= num3.GetValueOrDefault() & num3 != null)
					{
						this.completionSource.TrySetResult(true);
					}
				}
				return true;
			}

			private readonly int dueTimeFrameCount;

			private readonly int? periodFrameCount;

			private CancellationToken cancellationToken;

			private int initialFrame;

			private int currentFrame;

			private bool dueTimePhase;

			private bool completed;

			private bool disposed;
		}
	}
}
