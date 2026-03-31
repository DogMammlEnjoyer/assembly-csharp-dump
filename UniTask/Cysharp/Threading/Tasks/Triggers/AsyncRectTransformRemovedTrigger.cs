using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncRectTransformRemovedTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnRectTransformRemoved()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnRectTransformRemovedHandler GetOnRectTransformRemovedAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnRectTransformRemovedHandler GetOnRectTransformRemovedAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnRectTransformRemovedAsync()
		{
			return ((IAsyncOnRectTransformRemovedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnRectTransformRemovedAsync();
		}

		public UniTask OnRectTransformRemovedAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnRectTransformRemovedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnRectTransformRemovedAsync();
		}
	}
}
