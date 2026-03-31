using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncLateUpdateTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void LateUpdate()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncLateUpdateHandler GetLateUpdateAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncLateUpdateHandler GetLateUpdateAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask LateUpdateAsync()
		{
			return ((IAsyncLateUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).LateUpdateAsync();
		}

		public UniTask LateUpdateAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncLateUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).LateUpdateAsync();
		}
	}
}
