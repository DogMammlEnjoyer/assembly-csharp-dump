using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncCancelTrigger : AsyncTriggerBase<BaseEventData>, ICancelHandler, IEventSystemHandler
	{
		void ICancelHandler.OnCancel(BaseEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnCancelHandler GetOnCancelAsyncHandler()
		{
			return new AsyncTriggerHandler<BaseEventData>(this, false);
		}

		public IAsyncOnCancelHandler GetOnCancelAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, false);
		}

		public UniTask<BaseEventData> OnCancelAsync()
		{
			return ((IAsyncOnCancelHandler)new AsyncTriggerHandler<BaseEventData>(this, true)).OnCancelAsync();
		}

		public UniTask<BaseEventData> OnCancelAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnCancelHandler)new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, true)).OnCancelAsync();
		}
	}
}
