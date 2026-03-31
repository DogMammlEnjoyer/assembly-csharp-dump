using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncEnableTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnEnable()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnEnableHandler GetOnEnableAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnEnableHandler GetOnEnableAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnEnableAsync()
		{
			return ((IAsyncOnEnableHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnEnableAsync();
		}

		public UniTask OnEnableAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnEnableHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnEnableAsync();
		}
	}
}
