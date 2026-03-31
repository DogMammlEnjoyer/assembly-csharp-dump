using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
	internal sealed class DeltaTimePlayerLoopTimer : PlayerLoopTimer
	{
		public DeltaTimePlayerLoopTimer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object> timerCallback, object state) : base(periodic, playerLoopTiming, cancellationToken, timerCallback, state)
		{
			this.ResetCore(new TimeSpan?(interval));
		}

		protected override bool MoveNextCore()
		{
			if (this.elapsed == 0f && this.initialFrame == Time.frameCount)
			{
				return true;
			}
			this.elapsed += Time.deltaTime;
			return this.elapsed < this.interval;
		}

		protected override void ResetCore(TimeSpan? interval)
		{
			this.elapsed = 0f;
			this.initialFrame = (PlayerLoopHelper.IsMainThread ? Time.frameCount : -1);
			if (interval != null)
			{
				this.interval = (float)interval.Value.TotalSeconds;
			}
		}

		private int initialFrame;

		private float elapsed;

		private float interval;
	}
}
