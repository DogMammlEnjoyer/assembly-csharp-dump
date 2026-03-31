using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncMouseOverTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnMouseOver()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnMouseOverHandler GetOnMouseOverAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnMouseOverHandler GetOnMouseOverAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnMouseOverAsync()
		{
			return ((IAsyncOnMouseOverHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseOverAsync();
		}

		public UniTask OnMouseOverAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnMouseOverHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseOverAsync();
		}
	}
}
