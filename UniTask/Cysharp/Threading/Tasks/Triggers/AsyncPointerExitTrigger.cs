using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncPointerExitTrigger : AsyncTriggerBase<PointerEventData>, IPointerExitHandler, IEventSystemHandler
	{
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnPointerExitHandler GetOnPointerExitAsyncHandler()
		{
			return new AsyncTriggerHandler<PointerEventData>(this, false);
		}

		public IAsyncOnPointerExitHandler GetOnPointerExitAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
		}

		public UniTask<PointerEventData> OnPointerExitAsync()
		{
			return ((IAsyncOnPointerExitHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnPointerExitAsync();
		}

		public UniTask<PointerEventData> OnPointerExitAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnPointerExitHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnPointerExitAsync();
		}
	}
}
