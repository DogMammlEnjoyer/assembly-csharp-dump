using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncApplicationFocusTrigger : AsyncTriggerBase<bool>
	{
		private void OnApplicationFocus(bool hasFocus)
		{
			base.RaiseEvent(hasFocus);
		}

		public IAsyncOnApplicationFocusHandler GetOnApplicationFocusAsyncHandler()
		{
			return new AsyncTriggerHandler<bool>(this, false);
		}

		public IAsyncOnApplicationFocusHandler GetOnApplicationFocusAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<bool>(this, cancellationToken, false);
		}

		public UniTask<bool> OnApplicationFocusAsync()
		{
			return ((IAsyncOnApplicationFocusHandler)new AsyncTriggerHandler<bool>(this, true)).OnApplicationFocusAsync();
		}

		public UniTask<bool> OnApplicationFocusAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnApplicationFocusHandler)new AsyncTriggerHandler<bool>(this, cancellationToken, true)).OnApplicationFocusAsync();
		}
	}
}
