using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncOnCanvasGroupChangedTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnCanvasGroupChanged()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnCanvasGroupChangedHandler GetOnCanvasGroupChangedAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnCanvasGroupChangedHandler GetOnCanvasGroupChangedAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnCanvasGroupChangedAsync()
		{
			return ((IAsyncOnCanvasGroupChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnCanvasGroupChangedAsync();
		}

		public UniTask OnCanvasGroupChangedAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnCanvasGroupChangedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnCanvasGroupChangedAsync();
		}
	}
}
