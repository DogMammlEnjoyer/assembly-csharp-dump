using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncBeforeTransformParentChangedTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnBeforeTransformParentChanged()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnBeforeTransformParentChangedHandler GetOnBeforeTransformParentChangedAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnBeforeTransformParentChangedHandler GetOnBeforeTransformParentChangedAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnBeforeTransformParentChangedAsync()
		{
			return ((IAsyncOnBeforeTransformParentChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnBeforeTransformParentChangedAsync();
		}

		public UniTask OnBeforeTransformParentChangedAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnBeforeTransformParentChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnBeforeTransformParentChangedAsync();
		}
	}
}
