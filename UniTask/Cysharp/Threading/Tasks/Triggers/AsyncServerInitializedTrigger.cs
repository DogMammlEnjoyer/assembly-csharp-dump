using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncServerInitializedTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnServerInitialized()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnServerInitializedHandler GetOnServerInitializedAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnServerInitializedHandler GetOnServerInitializedAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnServerInitializedAsync()
		{
			return ((IAsyncOnServerInitializedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnServerInitializedAsync();
		}

		public UniTask OnServerInitializedAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnServerInitializedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnServerInitializedAsync();
		}
	}
}
