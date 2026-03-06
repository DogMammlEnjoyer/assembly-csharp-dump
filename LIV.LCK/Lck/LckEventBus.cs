using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckEventBus : ILckEventBus
	{
		[Preserve]
		public LckEventBus()
		{
		}

		public void AddListener<T>(Action<T> listener)
		{
			if (!this._delegates.ContainsKey(typeof(T)))
			{
				this._delegates[typeof(T)] = null;
			}
			this._delegates[typeof(T)] = (Action<T>)Delegate.Combine((Action<T>)this._delegates[typeof(T)], listener);
		}

		public void RemoveListener<T>(Action<T> listener)
		{
			Delegate @delegate;
			if (this._delegates.TryGetValue(typeof(T), out @delegate))
			{
				Action<T> action = (Action<T>)Delegate.Remove((Action<T>)@delegate, listener);
				if (action == null)
				{
					this._delegates.Remove(typeof(T));
					return;
				}
				this._delegates[typeof(T)] = action;
			}
		}

		public void Trigger<T>(T eventData)
		{
			Delegate @delegate;
			if (this._delegates.TryGetValue(typeof(T), out @delegate))
			{
				Action<T> action = @delegate as Action<T>;
				if (action == null)
				{
					return;
				}
				action(eventData);
			}
		}

		private readonly Dictionary<Type, Delegate> _delegates = new Dictionary<Type, Delegate>();
	}
}
