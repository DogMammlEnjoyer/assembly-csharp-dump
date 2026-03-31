using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncMouseExitTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnMouseExit()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnMouseExitHandler GetOnMouseExitAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnMouseExitHandler GetOnMouseExitAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnMouseExitAsync()
		{
			return ((IAsyncOnMouseExitHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseExitAsync();
		}

		public UniTask OnMouseExitAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnMouseExitHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseExitAsync();
		}
	}
}
