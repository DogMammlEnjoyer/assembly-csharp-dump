using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncFixedUpdateTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void FixedUpdate()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncFixedUpdateHandler GetFixedUpdateAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncFixedUpdateHandler GetFixedUpdateAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask FixedUpdateAsync()
		{
			return ((IAsyncFixedUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).FixedUpdateAsync();
		}

		public UniTask FixedUpdateAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncFixedUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).FixedUpdateAsync();
		}
	}
}
