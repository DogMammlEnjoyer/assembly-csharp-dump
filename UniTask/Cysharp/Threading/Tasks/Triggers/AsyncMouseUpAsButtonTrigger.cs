using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncMouseUpAsButtonTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnMouseUpAsButton()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnMouseUpAsButtonHandler GetOnMouseUpAsButtonAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnMouseUpAsButtonHandler GetOnMouseUpAsButtonAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnMouseUpAsButtonAsync()
		{
			return ((IAsyncOnMouseUpAsButtonHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnMouseUpAsButtonAsync();
		}

		public UniTask OnMouseUpAsButtonAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnMouseUpAsButtonHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnMouseUpAsButtonAsync();
		}
	}
}
