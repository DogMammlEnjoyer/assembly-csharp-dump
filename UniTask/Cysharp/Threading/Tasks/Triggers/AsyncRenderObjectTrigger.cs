using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncRenderObjectTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnRenderObject()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnRenderObjectHandler GetOnRenderObjectAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnRenderObjectHandler GetOnRenderObjectAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnRenderObjectAsync()
		{
			return ((IAsyncOnRenderObjectHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnRenderObjectAsync();
		}

		public UniTask OnRenderObjectAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnRenderObjectHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnRenderObjectAsync();
		}
	}
}
