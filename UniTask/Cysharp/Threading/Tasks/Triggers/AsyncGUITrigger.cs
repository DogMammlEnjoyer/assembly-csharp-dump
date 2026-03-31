using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncGUITrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnGUI()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnGUIHandler GetOnGUIAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnGUIHandler GetOnGUIAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnGUIAsync()
		{
			return ((IAsyncOnGUIHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnGUIAsync();
		}

		public UniTask OnGUIAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnGUIHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnGUIAsync();
		}
	}
}
