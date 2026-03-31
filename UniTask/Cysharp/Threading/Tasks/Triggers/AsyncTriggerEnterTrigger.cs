using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncTriggerEnterTrigger : AsyncTriggerBase<Collider>
	{
		private void OnTriggerEnter(Collider other)
		{
			base.RaiseEvent(other);
		}

		public IAsyncOnTriggerEnterHandler GetOnTriggerEnterAsyncHandler()
		{
			return new AsyncTriggerHandler<Collider>(this, false);
		}

		public IAsyncOnTriggerEnterHandler GetOnTriggerEnterAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<Collider>(this, cancellationToken, false);
		}

		public UniTask<Collider> OnTriggerEnterAsync()
		{
			return ((IAsyncOnTriggerEnterHandler)new AsyncTriggerHandler<Collider>(this, true)).OnTriggerEnterAsync();
		}

		public UniTask<Collider> OnTriggerEnterAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnTriggerEnterHandler)new AsyncTriggerHandler<Collider>(this, cancellationToken, true)).OnTriggerEnterAsync();
		}
	}
}
