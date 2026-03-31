using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncTransformChildrenChangedTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnTransformChildrenChanged()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnTransformChildrenChangedHandler GetOnTransformChildrenChangedAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnTransformChildrenChangedHandler GetOnTransformChildrenChangedAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnTransformChildrenChangedAsync()
		{
			return ((IAsyncOnTransformChildrenChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnTransformChildrenChangedAsync();
		}

		public UniTask OnTransformChildrenChangedAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnTransformChildrenChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnTransformChildrenChangedAsync();
		}
	}
}
