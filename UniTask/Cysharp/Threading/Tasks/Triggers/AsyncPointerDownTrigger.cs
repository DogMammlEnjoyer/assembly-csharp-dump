using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncPointerDownTrigger : AsyncTriggerBase<PointerEventData>, IPointerDownHandler, IEventSystemHandler
	{
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnPointerDownHandler GetOnPointerDownAsyncHandler()
		{
			return new AsyncTriggerHandler<PointerEventData>(this, false);
		}

		public IAsyncOnPointerDownHandler GetOnPointerDownAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
		}

		public UniTask<PointerEventData> OnPointerDownAsync()
		{
			return ((IAsyncOnPointerDownHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerDownAsync();
		}

		public UniTask<PointerEventData> OnPointerDownAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnPointerDownHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerDownAsync();
		}
	}
}
