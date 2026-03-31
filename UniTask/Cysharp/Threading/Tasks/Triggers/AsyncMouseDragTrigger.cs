using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncMouseDragTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnMouseDrag()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnMouseDragHandler GetOnMouseDragAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnMouseDragHandler GetOnMouseDragAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnMouseDragAsync()
		{
			return ((IAsyncOnMouseDragHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseDragAsync();
		}

		public UniTask OnMouseDragAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnMouseDragHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseDragAsync();
		}
	}
}
