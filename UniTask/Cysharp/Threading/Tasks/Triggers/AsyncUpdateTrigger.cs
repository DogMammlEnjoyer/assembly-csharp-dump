using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncUpdateTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void Update()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncUpdateHandler GetUpdateAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncUpdateHandler GetUpdateAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask UpdateAsync()
		{
			return ((IAsyncUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).UpdateAsync();
		}

		public UniTask UpdateAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncUpdateHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).UpdateAsync();
		}
	}
}
