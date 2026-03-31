using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncInitializePotentialDragTrigger : AsyncTriggerBase<PointerEventData>, IInitializePotentialDragHandler, IEventSystemHandler
	{
		void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnInitializePotentialDragHandler GetOnInitializePotentialDragAsyncHandler()
		{
			return new AsyncTriggerHandler<PointerEventData>(this, false);
		}

		public IAsyncOnInitializePotentialDragHandler GetOnInitializePotentialDragAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, false);
		}

		public UniTask<PointerEventData> OnInitializePotentialDragAsync()
		{
			return ((IAsyncOnInitializePotentialDragHandler)new AsyncTriggerHandler<PointerEventData>(this, true)).OnInitializePotentialDragAsync();
		}

		public UniTask<PointerEventData> OnInitializePotentialDragAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnInitializePotentialDragHandler)new AsyncTriggerHandler<PointerEventData>(this, cancellationToken, true)).OnInitializePotentialDragAsync();
		}
	}
}
