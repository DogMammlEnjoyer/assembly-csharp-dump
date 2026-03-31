using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncDisableTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnDisable()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnDisableHandler GetOnDisableAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnDisableHandler GetOnDisableAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnDisableAsync()
		{
			return ((IAsyncOnDisableHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnDisableAsync();
		}

		public UniTask OnDisableAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnDisableHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnDisableAsync();
		}
	}
}
