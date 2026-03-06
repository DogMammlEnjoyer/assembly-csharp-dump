using System;

namespace System.ComponentModel
{
	/// <summary>Provides a simple list of delegates. This class cannot be inherited.</summary>
	public sealed class EventHandlerList : IDisposable
	{
		internal EventHandlerList(Component parent)
		{
			this._parent = parent;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.EventHandlerList" /> class.</summary>
		public EventHandlerList()
		{
		}

		/// <summary>Gets or sets the delegate for the specified object.</summary>
		/// <param name="key">An object to find in the list.</param>
		/// <returns>The delegate for the specified key, or <see langword="null" /> if a delegate does not exist.</returns>
		public Delegate this[object key]
		{
			get
			{
				EventHandlerList.ListEntry listEntry = null;
				if (this._parent == null || this._parent.CanRaiseEventsInternal)
				{
					listEntry = this.Find(key);
				}
				if (listEntry == null)
				{
					return null;
				}
				return listEntry._handler;
			}
			set
			{
				EventHandlerList.ListEntry listEntry = this.Find(key);
				if (listEntry != null)
				{
					listEntry._handler = value;
					return;
				}
				this._head = new EventHandlerList.ListEntry(key, value, this._head);
			}
		}

		/// <summary>Adds a delegate to the list.</summary>
		/// <param name="key">The object that owns the event.</param>
		/// <param name="value">The delegate to add to the list.</param>
		public void AddHandler(object key, Delegate value)
		{
			EventHandlerList.ListEntry listEntry = this.Find(key);
			if (listEntry != null)
			{
				listEntry._handler = Delegate.Combine(listEntry._handler, value);
				return;
			}
			this._head = new EventHandlerList.ListEntry(key, value, this._head);
		}

		/// <summary>Adds a list of delegates to the current list.</summary>
		/// <param name="listToAddFrom">The list to add.</param>
		public void AddHandlers(EventHandlerList listToAddFrom)
		{
			for (EventHandlerList.ListEntry listEntry = listToAddFrom._head; listEntry != null; listEntry = listEntry._next)
			{
				this.AddHandler(listEntry._key, listEntry._handler);
			}
		}

		/// <summary>Disposes the delegate list.</summary>
		public void Dispose()
		{
			this._head = null;
		}

		private EventHandlerList.ListEntry Find(object key)
		{
			EventHandlerList.ListEntry listEntry = this._head;
			while (listEntry != null && listEntry._key != key)
			{
				listEntry = listEntry._next;
			}
			return listEntry;
		}

		/// <summary>Removes a delegate from the list.</summary>
		/// <param name="key">The object that owns the event.</param>
		/// <param name="value">The delegate to remove from the list.</param>
		public void RemoveHandler(object key, Delegate value)
		{
			EventHandlerList.ListEntry listEntry = this.Find(key);
			if (listEntry != null)
			{
				listEntry._handler = Delegate.Remove(listEntry._handler, value);
			}
		}

		private EventHandlerList.ListEntry _head;

		private Component _parent;

		private sealed class ListEntry
		{
			public ListEntry(object key, Delegate handler, EventHandlerList.ListEntry next)
			{
				this._next = next;
				this._key = key;
				this._handler = handler;
			}

			internal EventHandlerList.ListEntry _next;

			internal object _key;

			internal Delegate _handler;
		}
	}
}
