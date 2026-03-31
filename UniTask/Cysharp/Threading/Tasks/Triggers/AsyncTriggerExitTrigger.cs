using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncTriggerExitTrigger : AsyncTriggerBase<Collider>
	{
		private void OnTriggerExit(Collider other)
		{
			base.RaiseEvent(other);
		}

		public IAsyncOnTriggerExitHandler GetOnTriggerExitAsyncHandler()
		{
			return new AsyncTriggerHandler<Collider>(this, false);
		}

		public IAsyncOnTriggerExitHandler GetOnTriggerExitAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<Collider>(this, cancellationToken, false);
		}

		public UniTask<Collider> OnTriggerExitAsync()
		{
			return ((IAsyncOnTriggerExitHandler)new AsyncTriggerHandler<Collider>(this, true)).OnTriggerExitAsync();
		}

		public UniTask<Collider> OnTriggerExitAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnTriggerExitHandler)new AsyncTriggerHandler<Collider>(this, cancellationToken, true)).OnTriggerExitAsync();
		}
	}
}
