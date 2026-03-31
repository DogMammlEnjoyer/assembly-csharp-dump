using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncPreCullTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnPreCull()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnPreCullHandler GetOnPreCullAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnPreCullHandler GetOnPreCullAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnPreCullAsync()
		{
			return ((IAsyncOnPreCullHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnPreCullAsync();
		}

		public UniTask OnPreCullAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnPreCullHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnPreCullAsync();
		}
	}
}
