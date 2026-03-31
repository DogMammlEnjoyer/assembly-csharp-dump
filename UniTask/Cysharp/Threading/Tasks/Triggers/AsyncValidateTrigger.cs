using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncValidateTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnValidate()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnValidateHandler GetOnValidateAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnValidateHandler GetOnValidateAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnValidateAsync()
		{
			return ((IAsyncOnValidateHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnValidateAsync();
		}

		public UniTask OnValidateAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnValidateHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnValidateAsync();
		}
	}
}
