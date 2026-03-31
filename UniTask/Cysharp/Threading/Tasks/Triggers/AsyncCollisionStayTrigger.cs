using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncCollisionStayTrigger : AsyncTriggerBase<Collision>
	{
		private void OnCollisionStay(Collision coll)
		{
			base.RaiseEvent(coll);
		}

		public IAsyncOnCollisionStayHandler GetOnCollisionStayAsyncHandler()
		{
			return new AsyncTriggerHandler<Collision>(this, false);
		}

		public IAsyncOnCollisionStayHandler GetOnCollisionStayAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<Collision>(this, cancellationToken, false);
		}

		public UniTask<Collision> OnCollisionStayAsync()
		{
			return ((IAsyncOnCollisionStayHandler)new AsyncTriggerHandler<Collision>(this, true)).OnCollisionStayAsync();
		}

		public UniTask<Collision> OnCollisionStayAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnCollisionStayHandler)new AsyncTriggerHandler<Collision>(this, cancellationToken, true)).OnCollisionStayAsync();
		}
	}
}
