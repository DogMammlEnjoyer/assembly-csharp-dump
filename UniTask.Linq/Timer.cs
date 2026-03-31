using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class Timer : IUniTaskAsyncEnumerable<AsyncUnit>
	{
		public Timer(TimeSpan dueTime, TimeSpan? period, PlayerLoopTiming updateTiming, bool ignoreTimeScale)
		{
			this.updateTiming = updateTiming;
			this.dueTime = dueTime;
			this.period = period;
			this.ignoreTimeScale = ignoreTimeScale;
		}

		public IUniTaskAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Timer._Timer(this.dueTime, this.period, this.updateTiming, this.ignoreTimeScale, cancellationToken);
		}

		private readonly PlayerLoopTiming updateTiming;

		private readonly TimeSpan dueTime;

		private readonly TimeSpan? period;

		private readonly bool ignoreTimeScale;

		private class _Timer : MoveNextSource, IUniTaskAsyncEnumerator<AsyncUnit>, IUniTaskAsyncDisposable, IPlayerLoopItem
		{
			public _Timer(TimeSpan dueTime, TimeSpan? period, PlayerLoopTiming updateTiming, bool ignoreTimeScale, CancellationToken cancellationToken)
			{
				this.dueTime = (float)dueTime.TotalSeconds;
				this.period = ((period == null) ? null : new float?((float)period.Value.TotalSeconds));
				if (this.dueTime <= 0f)
				{
					this.dueTime = 0f;
				}
				if (this.period != null)
				{
					float? num = this.period;
					float num2 = 0f;
					if (num.GetValueOrDefault() <= num2 & num != null)
					{
						this.period = new float?((float)1);
					}
				}
				this.initialFrame = (PlayerLoopHelper.IsMainThread ? Time.frameCount : -1);
				this.dueTimePhase = true;
				this.updateTiming = updateTiming;
				this.ignoreTimeScale = ignoreTimeScale;
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
				this.elapsed = 0f;
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
					if (this.elapsed == 0f && this.initialFrame == Time.frameCount)
					{
						return true;
					}
					this.elapsed += (this.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
					if (this.elapsed >= this.dueTime)
					{
						this.dueTimePhase = false;
						this.completionSource.TrySetResult(true);
					}
				}
				else
				{
					if (this.period == null)
					{
						this.completed = true;
						this.completionSource.TrySetResult(false);
						return false;
					}
					this.elapsed += (this.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
					float num = this.elapsed;
					float? num2 = this.period;
					if (num >= num2.GetValueOrDefault() & num2 != null)
					{
						this.completionSource.TrySetResult(true);
					}
				}
				return true;
			}

			private readonly float dueTime;

			private readonly float? period;

			private readonly PlayerLoopTiming updateTiming;

			private readonly bool ignoreTimeScale;

			private CancellationToken cancellationToken;

			private int initialFrame;

			private float elapsed;

			private bool dueTimePhase;

			private bool completed;

			private bool disposed;
		}
	}
}
