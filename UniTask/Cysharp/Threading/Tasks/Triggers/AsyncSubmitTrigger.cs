using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncSubmitTrigger : AsyncTriggerBase<BaseEventData>, ISubmitHandler, IEventSystemHandler
	{
		void ISubmitHandler.OnSubmit(BaseEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnSubmitHandler GetOnSubmitAsyncHandler()
		{
			return new AsyncTriggerHandler<BaseEventData>(this, false);
		}

		public IAsyncOnSubmitHandler GetOnSubmitAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, false);
		}

		public UniTask<BaseEventData> OnSubmitAsync()
		{
			return ((IAsyncOnSubmitHandler)new AsyncTriggerHandler<BaseEventData>(this, true)).OnSubmitAsync();
		}

		public UniTask<BaseEventData> OnSubmitAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnSubmitHandler)new AsyncTriggerHandler<BaseEventData>(this, cancellationToken, true)).OnSubmitAsync();
		}
	}
}
