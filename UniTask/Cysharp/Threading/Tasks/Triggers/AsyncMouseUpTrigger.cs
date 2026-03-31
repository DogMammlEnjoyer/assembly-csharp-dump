using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncMouseUpTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnMouseUp()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnMouseUpHandler GetOnMouseUpAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnMouseUpHandler GetOnMouseUpAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnMouseUpAsync()
		{
			return ((IAsyncOnMouseUpHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseUpAsync();
		}

		public UniTask OnMouseUpAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnMouseUpHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseUpAsync();
		}
	}
}
