using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncDeselectTrigger : AsyncTriggerBase<BaseEventData>, IDeselectHandler, IEventSystemHandler
	{
		void IDeselectHandler.OnDeselect(BaseEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnDeselectHandler GetOnDeselectAsyncHandler()
		{
			return new AsyncTriggerHandler<BaseEventData>(this, false);
		}

		public IAsyncOnDeselectHandler GetOnDeselectAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, false);
		}

		public UniTask<BaseEventData> OnDeselectAsync()
		{
			return ((IAsyncOnDeselectHandler)new AsyncTriggerHandler<BaseEventData>(this, true)).OnDeselectAsync();
		}

		public UniTask<BaseEventData> OnDeselectAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnDeselectHandler)new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, true)).OnDeselectAsync();
		}
	}
}
