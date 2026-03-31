using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncPreRenderTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnPreRender()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnPreRenderHandler GetOnPreRenderAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnPreRenderHandler GetOnPreRenderAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnPreRenderAsync()
		{
			return ((IAsyncOnPreRenderHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnPreRenderAsync();
		}

		public UniTask OnPreRenderAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnPreRenderHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnPreRenderAsync();
		}
	}
}
