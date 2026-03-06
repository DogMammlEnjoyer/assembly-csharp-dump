using System;
using System.Collections.Generic;

namespace Liv.Lck
{
	internal class LckPublicApiEventBridge : IDisposable
	{
		internal LckPublicApiEventBridge(ILckEventBus eventBus)
		{
			this._eventBus = eventBus;
		}

		public void Forward<TEvent, TResult>(Action<TResult> publicEventInvoker) where TEvent : LckEvents.IEventWithResult<TResult> where TResult : ILckResult
		{
			this._forwarders.Add(new LckEventForwarder<TEvent, TResult>(this._eventBus, (TEvent evt) => evt.Result, publicEventInvoker));
		}

		public void Forward<TEvent, TResult>(Func<TEvent, TResult> selector, Action<TResult> publicEventInvoker)
		{
			this._forwarders.Add(new LckEventForwarder<TEvent, TResult>(this._eventBus, selector, publicEventInvoker));
		}

		public void Dispose()
		{
			foreach (IDisposable disposable in this._forwarders)
			{
				disposable.Dispose();
			}
			this._forwarders.Clear();
		}

		private readonly ILckEventBus _eventBus;

		private readonly List<IDisposable> _forwarders = new List<IDisposable>();
	}
}
