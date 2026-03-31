using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncMouseEnterTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnMouseEnter()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnMouseEnterHandler GetOnMouseEnterAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnMouseEnterHandler GetOnMouseEnterAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnMouseEnterAsync()
		{
			return ((IAsyncOnMouseEnterHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseEnterAsync();
		}

		public UniTask OnMouseEnterAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnMouseEnterHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseEnterAsync();
		}
	}
}
