using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncApplicationQuitTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnApplicationQuit()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnApplicationQuitHandler GetOnApplicationQuitAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnApplicationQuitHandler GetOnApplicationQuitAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnApplicationQuitAsync()
		{
			return ((IAsyncOnApplicationQuitHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnApplicationQuitAsync();
		}

		public UniTask OnApplicationQuitAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnApplicationQuitHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnApplicationQuitAsync();
		}
	}
}
