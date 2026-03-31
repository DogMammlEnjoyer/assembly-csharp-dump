using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cysharp.Threading.Tasks.Triggers
{
	[DisallowMultipleComponent]
	public sealed class AsyncMoveTrigger : AsyncTriggerBase<AxisEventData>, IMoveHandler, IEventSystemHandler
	{
		void IMoveHandler.OnMove(AxisEventData eventData)
		{
			base.RaiseEvent(eventData);
		}

		public IAsyncOnMoveHandler GetOnMoveAsyncHandler()
		{
			return new AsyncTriggerHandler<AxisEventData>(this, false);
		}

		public IAsyncOnMoveHandler GetOnMoveAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<AxisEventData>(this, cancellationToken, false);
		}

		public UniTask<AxisEventData> OnMoveAsync()
		{
			return ((IAsyncOnMoveHandler)new AsyncTriggerHandler<AxisEventData>(this, true)).OnMoveAsync();
		}

		public UniTask<AxisEventData> OnMoveAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnMoveHandler)new AsyncTriggerHandler<AxisEventData>(this, cancellationToken, true)).OnMoveAsync();
		}
	}
}
