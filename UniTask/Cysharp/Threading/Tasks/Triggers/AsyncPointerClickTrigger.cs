using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncPointerClickTrigger : AsyncTriggerBase<PointerEventData>, IPointerClickHandler, IEventSystemHandler
	{
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnPointerClickHandler GetOnPointerClickAsyncHandler()
		{
			return new AsyncTriggerHandler<PointerEventData>(this, false);
		}

		public IAsyncOnPointerClickHandler GetOnPointerClickAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
		}

		public UniTask<PointerEventData> OnPointerClickAsync()
		{
			return ((IAsyncOnPointerClickHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerClickAsync();
		}

		public UniTask<PointerEventData> OnPointerClickAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnPointerClickHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerClickAsync();
		}
	}
}
