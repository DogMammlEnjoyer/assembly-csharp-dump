using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncTriggerEnter2DTrigger : AsyncTriggerBase<Collider2D>
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			base.RaiseEvent(other);
		}

		public IAsyncOnTriggerEnter2DHandler GetOnTriggerEnter2DAsyncHandler()
		{
			return new AsyncTriggerHandler<Collider2D>(this, false);
		}

		public IAsyncOnTriggerEnter2DHandler GetOnTriggerEnter2DAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<Collider2D>(this, cancellationToken, false);
		}

		public UniTask<Collider2D> OnTriggerEnter2DAsync()
		{
			return ((IAsyncOnTriggerEnter2DHandler)new AsyncTriggerHandler<Collider2D>(this, true)).OnTriggerEnter2DAsync();
		}

		public UniTask<Collider2D> OnTriggerEnter2DAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnTriggerEnter2DHandler)new AsyncTriggerHandler<Collider2D>(this, cancellationToken, true)).OnTriggerEnter2DAsync();
		}
	}
}
