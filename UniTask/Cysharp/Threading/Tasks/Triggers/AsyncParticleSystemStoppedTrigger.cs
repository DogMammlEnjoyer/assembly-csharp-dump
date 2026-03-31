using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncParticleSystemStoppedTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void OnParticleSystemStopped()
		{
			base.RaiseEvent(AsyncUnit.Default);
		}

		public IAsyncOnParticleSystemStoppedHandler GetOnParticleSystemStoppedAsyncHandler()
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, false);
		}

		public IAsyncOnParticleSystemStoppedHandler GetOnParticleSystemStoppedAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, false);
		}

		public UniTask OnParticleSystemStoppedAsync()
		{
			return ((IAsyncOnParticleSystemStoppedHandler)new AsyncTriggerHandler<AsyncUnit>(this, true)).OnParticleSystemStoppedAsync();
		}

		public UniTask OnParticleSystemStoppedAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnParticleSystemStoppedHandler)new AsyncTriggerHandler<AsyncUnit>(this, cancellationToken, true)).OnParticleSystemStoppedAsync();
		}
	}
}
