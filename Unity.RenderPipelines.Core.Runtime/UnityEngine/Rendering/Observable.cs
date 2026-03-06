using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public struct Observable<T>
	{
		public event Action<T> onValueChanged;

		public T value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				if (!EqualityComparer<T>.Default.Equals(value, this.m_Value))
				{
					this.m_Value = value;
					Action<T> action = this.onValueChanged;
					if (action == null)
					{
						return;
					}
					action(value);
				}
			}
		}

		public Observable(T newValue)
		{
			this.m_Value = newValue;
			this.onValueChanged = null;
		}

		private T m_Value;
	}
}
