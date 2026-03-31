using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncPointerEnterTrigger : AsyncTriggerBase<PointerEventData>, IPointerEnterHandler, IEventSystemHandler
	{
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnPointerEnterHandler GetOnPointerEnterAsyncHandler()
		{
			return new AsyncTriggerHandler<PointerEventData>(this, false);
		}

		public IAsyncOnPointerEnterHandler GetOnPointerEnterAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
		}

		public UniTask<PointerEventData> OnPointerEnterAsync()
		{
			return ((IAsyncOnPointerEnterHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerEnterAsync();
		}

		public UniTask<PointerEventData> OnPointerEnterAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnPointerEnterHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerEnterAsync();
		}
	}
}
