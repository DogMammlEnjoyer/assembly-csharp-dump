using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncCollisionEnterTrigger : AsyncTriggerBase<Collision>
	{
		private void OnCollisionEnter(Collision coll)
		{
			base.RaiseEvent(coll);
		}

		public IAsyncOnCollisionEnterHandler GetOnCollisionEnterAsyncHandler()
		{
			return new AsyncTriggerHandler<Collision>(this, false);
		}

		public IAsyncOnCollisionEnterHandler GetOnCollisionEnterAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<Collision>(this, cancellationToken, false);
		}

		public UniTask<Collision> OnCollisionEnterAsync()
		{
			return ((IAsyncOnCollisionEnterHandler)new AsyncTriggerHandler<Collision>(this, true)).OnCollisionEnterAsync();
		}

		public UniTask<Collision> OnCollisionEnterAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnCollisionEnterHandler)new AsyncTriggerHandler<Collision>(this, cancellationToken, true)).OnCollisionEnterAsync();
		}
	}
}
