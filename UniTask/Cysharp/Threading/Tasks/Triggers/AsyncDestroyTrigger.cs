using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncDestroyTrigger : MonoBehaviour
	{
		public CancellationToken CancellationToken
		{
			get
			{
				if (this.cancellationTokenSource == null)
				{
					this.cancellationTokenSource = new CancellationTokenSource();
				}
				if (!this.awakeCalled)
				{
					PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AsyncDestroyTrigger.AwakeMonitor(this));
				}
				return this.cancellationTokenSource.Token;
			}
		}

		private void Awake()
		{
			this.awakeCalled = true;
		}

		private void OnDestroy()
		{
			this.called = true;
			CancellationTokenSource cancellationTokenSource = this.cancellationTokenSource;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			CancellationTokenSource cancellationTokenSource2 = this.cancellationTokenSource;
			if (cancellationTokenSource2 == null)
			{
				return;
			}
			cancellationTokenSource2.Dispose();
		}

		public UniTask OnDestroyAsync()
		{
			if (this.called)
			{
				return UniTask.CompletedTask;
			}
			UniTaskCompletionSource uniTaskCompletionSource = new UniTaskCompletionSource();
			this.CancellationToken.RegisterWithoutCaptureExecutionContext(delegate(object state)
			{
				((UniTaskCompletionSource)state).TrySetResult();
			}, uniTaskCompletionSource);
			return uniTaskCompletionSource.Task;
		}

		private bool awakeCalled;

		private bool called;

		private CancellationTokenSource cancellationTokenSource;

		private class AwakeMonitor : IPlayerLoopItem
		{
			public AwakeMonitor(AsyncDestroyTrigger trigger)
			{
				this.trigger = trigger;
			}

			public bool MoveNext()
			{
				if (this.trigger.called)
				{
					return false;
				}
				if (this.trigger == null)
				{
					this.trigger.OnDestroy();
					return false;
				}
				return true;
			}

			private readonly AsyncDestroyTrigger trigger;
		}
	}
}
