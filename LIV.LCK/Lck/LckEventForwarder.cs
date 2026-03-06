using System;

namespace Liv.Lck
{
	internal class LckEventForwarder<TEvent, TResult> : IDisposable
	{
		internal LckEventForwarder(ILckEventBus eventBus, Func<TEvent, TResult> selector, Action<TResult> forwardingAction)
		{
			this._eventBus = eventBus;
			this._selector = selector;
			this._forwardingAction = forwardingAction;
			this._eventBus.AddListener<TEvent>(new Action<TEvent>(this.OnEventReceived));
		}

		private void OnEventReceived(TEvent evt)
		{
			TResult obj = this._selector(evt);
			this._forwardingAction(obj);
		}

		public void Dispose()
		{
			this._eventBus.RemoveListener<TEvent>(new Action<TEvent>(this.OnEventReceived));
		}

		private readonly ILckEventBus _eventBus;

		private readonly Func<TEvent, TResult> _selector;

		private readonly Action<TResult> _forwardingAction;
	}
}
