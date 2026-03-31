using System;
using System.Collections.Generic;

namespace Liv.Lck
{
	internal class LckEventErrorLogger : IDisposable
	{
		public LckEventErrorLogger(ILckEventBus eventBus, Action<ILckResult> logAction)
		{
			this._eventBus = eventBus;
			this._logAction = logAction;
		}

		public void Monitor<TEvent, TResult>() where TEvent : LckEvents.IEventWithResult<TResult> where TResult : ILckResult
		{
			LckEventErrorLogger.EventSubscription<TEvent> item = new LckEventErrorLogger.EventSubscription<TEvent>(this._eventBus, new Action<TEvent>(this.OnEventReceived<TEvent, TResult>));
			this._subscriptions.Add(item);
		}

		private void OnEventReceived<TEvent, TResult>(TEvent evt) where TEvent : LckEvents.IEventWithResult<TResult> where TResult : ILckResult
		{
			TResult result = evt.Result;
			if (!result.Success)
			{
				this._logAction(evt.Result);
			}
		}

		public void Dispose()
		{
			foreach (IDisposable disposable in this._subscriptions)
			{
				disposable.Dispose();
			}
			this._subscriptions.Clear();
		}

		private readonly ILckEventBus _eventBus;

		private readonly Action<ILckResult> _logAction;

		private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

		private class EventSubscription<TEvent> : IDisposable
		{
			public EventSubscription(ILckEventBus eventBus, Action<TEvent> callback)
			{
				this._eventBus = eventBus;
				this._callback = callback;
				this._eventBus.AddListener<TEvent>(this._callback);
			}

			public void Dispose()
			{
				this._eventBus.RemoveListener<TEvent>(this._callback);
			}

			private readonly ILckEventBus _eventBus;

			private readonly Action<TEvent> _callback;
		}
	}
}
