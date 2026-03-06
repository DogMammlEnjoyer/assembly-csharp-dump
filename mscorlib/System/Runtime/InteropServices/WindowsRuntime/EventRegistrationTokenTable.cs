using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	/// <summary>Stores mappings between delegates and event tokens, to support the implementation of a Windows Runtime event in managed code.</summary>
	/// <typeparam name="T">The type of the event handler delegate for a particular event.</typeparam>
	public sealed class EventRegistrationTokenTable<T> where T : class
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable`1" /> class.</summary>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="T" /> is not a delegate type.</exception>
		public EventRegistrationTokenTable()
		{
			if (!typeof(Delegate).IsAssignableFrom(typeof(T)))
			{
				throw new InvalidOperationException(Environment.GetResourceString("Type '{0}' is not a delegate type.  EventTokenTable may only be used with delegate types.", new object[]
				{
					typeof(T)
				}));
			}
		}

		/// <summary>Gets or sets a delegate of type <paramref name="T" /> whose invocation list includes all the event handler delegates that have been added, and that have not yet been removed. Invoking this delegate invokes all the event handlers.</summary>
		/// <returns>A delegate of type <paramref name="T" /> that represents all the event handler delegates that are currently registered for an event.</returns>
		public T InvocationList
		{
			get
			{
				return this.m_invokeList;
			}
			set
			{
				Dictionary<EventRegistrationToken, T> tokens = this.m_tokens;
				lock (tokens)
				{
					this.m_tokens.Clear();
					this.m_invokeList = default(T);
					if (value != null)
					{
						this.AddEventHandlerNoLock(value);
					}
				}
			}
		}

		/// <summary>Adds the specified event handler to the table and to the invocation list, and returns a token that can be used to remove the event handler.</summary>
		/// <param name="handler">The event handler to add.</param>
		/// <returns>A token that can be used to remove the event handler from the table and the invocation list.</returns>
		public EventRegistrationToken AddEventHandler(T handler)
		{
			if (handler == null)
			{
				return new EventRegistrationToken(0UL);
			}
			Dictionary<EventRegistrationToken, T> tokens = this.m_tokens;
			EventRegistrationToken result;
			lock (tokens)
			{
				result = this.AddEventHandlerNoLock(handler);
			}
			return result;
		}

		private EventRegistrationToken AddEventHandlerNoLock(T handler)
		{
			EventRegistrationToken preferredToken = EventRegistrationTokenTable<T>.GetPreferredToken(handler);
			while (this.m_tokens.ContainsKey(preferredToken))
			{
				preferredToken = new EventRegistrationToken(preferredToken.Value + 1UL);
			}
			this.m_tokens[preferredToken] = handler;
			Delegate @delegate = (Delegate)((object)this.m_invokeList);
			@delegate = Delegate.Combine(@delegate, (Delegate)((object)handler));
			this.m_invokeList = (T)((object)@delegate);
			return preferredToken;
		}

		[FriendAccessAllowed]
		internal T ExtractHandler(EventRegistrationToken token)
		{
			T result = default(T);
			Dictionary<EventRegistrationToken, T> tokens = this.m_tokens;
			lock (tokens)
			{
				if (this.m_tokens.TryGetValue(token, out result))
				{
					this.RemoveEventHandlerNoLock(token);
				}
			}
			return result;
		}

		private static EventRegistrationToken GetPreferredToken(T handler)
		{
			Delegate[] invocationList = ((Delegate)((object)handler)).GetInvocationList();
			uint hashCode;
			if (invocationList.Length == 1)
			{
				hashCode = (uint)invocationList[0].Method.GetHashCode();
			}
			else
			{
				hashCode = (uint)handler.GetHashCode();
			}
			return new EventRegistrationToken((ulong)typeof(T).MetadataToken << 32 | (ulong)hashCode);
		}

		/// <summary>Removes the event handler that is associated with the specified token from the table and the invocation list.</summary>
		/// <param name="token">The token that was returned when the event handler was added.</param>
		public void RemoveEventHandler(EventRegistrationToken token)
		{
			if (token.Value == 0UL)
			{
				return;
			}
			Dictionary<EventRegistrationToken, T> tokens = this.m_tokens;
			lock (tokens)
			{
				this.RemoveEventHandlerNoLock(token);
			}
		}

		/// <summary>Removes the specified event handler delegate from the table and the invocation list.</summary>
		/// <param name="handler">The event handler to remove.</param>
		public void RemoveEventHandler(T handler)
		{
			if (handler == null)
			{
				return;
			}
			Dictionary<EventRegistrationToken, T> tokens = this.m_tokens;
			lock (tokens)
			{
				EventRegistrationToken preferredToken = EventRegistrationTokenTable<T>.GetPreferredToken(handler);
				T t;
				if (this.m_tokens.TryGetValue(preferredToken, out t) && t == handler)
				{
					this.RemoveEventHandlerNoLock(preferredToken);
				}
				else
				{
					foreach (KeyValuePair<EventRegistrationToken, T> keyValuePair in this.m_tokens)
					{
						if (keyValuePair.Value == (T)((object)handler))
						{
							this.RemoveEventHandlerNoLock(keyValuePair.Key);
							break;
						}
					}
				}
			}
		}

		private void RemoveEventHandlerNoLock(EventRegistrationToken token)
		{
			T t;
			if (this.m_tokens.TryGetValue(token, out t))
			{
				this.m_tokens.Remove(token);
				Delegate @delegate = (Delegate)((object)this.m_invokeList);
				@delegate = Delegate.Remove(@delegate, (Delegate)((object)t));
				this.m_invokeList = (T)((object)@delegate);
			}
		}

		/// <summary>Returns the specified event registration token table, if it is not <see langword="null" />; otherwise, returns a new event registration token table.</summary>
		/// <param name="refEventTable">An event registration token table, passed by reference.</param>
		/// <returns>The event registration token table that is specified by <paramref name="refEventTable" />, if it is not <see langword="null" />; otherwise, a new event registration token table.</returns>
		public static EventRegistrationTokenTable<T> GetOrCreateEventRegistrationTokenTable(ref EventRegistrationTokenTable<T> refEventTable)
		{
			if (refEventTable == null)
			{
				Interlocked.CompareExchange<EventRegistrationTokenTable<T>>(ref refEventTable, new EventRegistrationTokenTable<T>(), null);
			}
			return refEventTable;
		}

		private Dictionary<EventRegistrationToken, T> m_tokens = new Dictionary<EventRegistrationToken, T>();

		private volatile T m_invokeList;
	}
}
