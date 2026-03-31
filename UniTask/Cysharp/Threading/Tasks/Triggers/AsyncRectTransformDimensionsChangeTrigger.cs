using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncRectTransformDimensionsChangeTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnRectTransformDimensionsChange()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnRectTransformDimensionsChangeHandler GetOnRectTransformDimensionsChangeAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnRectTransformDimensionsChangeHandler GetOnRectTransformDimensionsChangeAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnRectTransformDimensionsChangeAsync()
		{
			return ((IAsyncOnRectTransformDimensionsChangeHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnRectTransformDimensionsChangeAsync();
		}

		public UniTask OnRectTransformDimensionsChangeAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnRectTransformDimensionsChangeHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnRectTransformDimensionsChangeAsync();
		}
	}
}
