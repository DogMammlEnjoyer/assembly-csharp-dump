using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncBecameInvisibleTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnBecameInvisible()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnBecameInvisibleHandler GetOnBecameInvisibleAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnBecameInvisibleHandler GetOnBecameInvisibleAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnBecameInvisibleAsync()
		{
			return ((IAsyncOnBecameInvisibleHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnBecameInvisibleAsync();
		}

		public UniTask OnBecameInvisibleAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnBecameInvisibleHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnBecameInvisibleAsync();
		}
	}
}
