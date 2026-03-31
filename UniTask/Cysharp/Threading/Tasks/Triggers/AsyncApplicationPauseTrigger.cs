using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncApplicationPauseTrigger : AsyncTriggerBase<bool>
	{
		private void OnApplicationPause(bool pauseStatus)
		{
			base.RaiseEvent(pauseStatus);
		}

		public IAsyncOnApplicationPauseHandler GetOnApplicationPauseAsyncHandler()
		{
			return new AsyncTriggerHandler<bool>(this, false);
		}

		public IAsyncOnApplicationPauseHandler GetOnApplicationPauseAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<bool>(this, cancellationToken, false);
		}

		public UniTask<bool> OnApplicationPauseAsync()
		{
			return ((IAsyncOnApplicationPauseHandler)new AsyncTriggerHandler<bool>(this, true)).OnApplicationPauseAsync();
		}

		public UniTask<bool> OnApplicationPauseAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnApplicationPauseHandler)new AsyncTriggerHandler<bool>(this, cancellationToken, true)).OnApplicationPauseAsync();
		}
	}
}
