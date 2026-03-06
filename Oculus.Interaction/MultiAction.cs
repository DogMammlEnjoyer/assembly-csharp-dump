using System;
using System.Collections.Generic;

namespace Oculus.Interaction
{
	public class MultiAction<T> : MAction<T>
	{
		public event Action<T> Action
		{
			add
			{
				this.actions.Add(value);
			}
			remove
			{
				this.actions.Remove(value);
			}
		}

		public void Invoke(T t)
		{
			foreach (Action<T> action in this.actions)
			{
				action(t);
			}
		}

		protected HashSet<Action<T>> actions = new HashSet<Action<T>>();
	}
}
