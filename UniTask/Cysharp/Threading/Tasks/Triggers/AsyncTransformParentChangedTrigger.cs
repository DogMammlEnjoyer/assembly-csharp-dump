using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncTransformParentChangedTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnTransformParentChanged()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnTransformParentChangedHandler GetOnTransformParentChangedAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnTransformParentChangedHandler GetOnTransformParentChangedAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnTransformParentChangedAsync()
		{
			return ((IAsyncOnTransformParentChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnTransformParentChangedAsync();
		}

		public UniTask OnTransformParentChangedAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnTransformParentChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnTransformParentChangedAsync();
		}
	}
}
