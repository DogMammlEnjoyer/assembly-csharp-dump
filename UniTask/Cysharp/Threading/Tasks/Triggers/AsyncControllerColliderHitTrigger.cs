using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncControllerColliderHitTrigger : AsyncTriggerBase<ControllerColliderHit>
	{
		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			base.RaiseEvent(hit);
		}

		public IAsyncOnControllerColliderHitHandler GetOnControllerColliderHitAsyncHandler()
		{
			return new AsyncTriggerHandler<ControllerColliderHit>(this, false);
		}

		public IAsyncOnControllerColliderHitHandler GetOnControllerColliderHitAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<ControllerColliderHit>(this, cancellationToken, false);
		}

		public UniTask<ControllerColliderHit> OnControllerColliderHitAsync()
		{
			return ((IAsyncOnControllerColliderHitHandler)new AsyncTriggerHandler<ControllerColliderHit>(this, true)).OnControllerColliderHitAsync();
		}

		public UniTask<ControllerColliderHit> OnControllerColliderHitAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnControllerColliderHitHandler)new AsyncTriggerHandler<ControllerColliderHit>(this, cancellationToken, true)).OnControllerColliderHitAsync();
		}
	}
}
