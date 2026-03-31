using System;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncAwakeTrigger : AsyncTriggerBase<AsyncUnit>
	{
		public UniTask AwakeAsync()
		{
			if (this.calledAwake)
			{
				return UniTask.CompletedTask;
			}
			return ((IAsyncOneShotTrigger)new AsyncTriggerHandler<AsyncUnit>(this, true)).OneShotAsync();
		}
	}
}
