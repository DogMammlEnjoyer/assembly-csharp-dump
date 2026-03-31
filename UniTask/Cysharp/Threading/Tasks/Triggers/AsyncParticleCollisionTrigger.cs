using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncParticleCollisionTrigger : AsyncTriggerBase<GameObject>
	{
		private void OnParticleCollision(GameObject other)
		{
			base.RaiseEvent(other);
		}

		public IAsyncOnParticleCollisionHandler GetOnParticleCollisionAsyncHandler()
		{
			return new AsyncTriggerHandler<GameObject>(this, false);
		}

		public IAsyncOnParticleCollisionHandler GetOnParticleCollisionAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<GameObject>(this, cancellationToken, false);
		}

		public UniTask<GameObject> OnParticleCollisionAsync()
		{
			return ((IAsyncOnParticleCollisionHandler)new AsyncTriggerHandler<GameObject>(this, true)).OnParticleCollisionAsync();
		}

		public UniTask<GameObject> OnParticleCollisionAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnParticleCollisionHandler)new AsyncTriggerHandler<GameObject>(this, cancellationToken, true)).OnParticleCollisionAsync();
		}
	}
}
