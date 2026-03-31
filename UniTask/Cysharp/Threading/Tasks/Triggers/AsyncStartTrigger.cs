using System;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncStartTrigger : AsyncTriggerBase<AsyncUnit>
	{
		private void Start()
		{
			this.called = true;
			base.RaiseEvent(AsyncUnit.Default);
		}

		public UniTask StartAsync()
		{
			if (this.called)
			{
				return UniTask.CompletedTask;
			}
			return ((IAsyncOneShotTrigger)new AsyncTriggerHandler<AsyncUnit>(this, true)).OneShotAsync();
		}

		private bool called;
	}
}
