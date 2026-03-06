using System;

namespace System.Threading
{
	/// <summary>Represents ambient data that is local to a given asynchronous control flow, such as an asynchronous method.</summary>
	/// <typeparam name="T">The type of the ambient data.</typeparam>
	public sealed class AsyncLocal<T> : IAsyncLocal
	{
		/// <summary>Instantiates an <see cref="T:System.Threading.AsyncLocal`1" /> instance that does not receive change notifications.</summary>
		public AsyncLocal()
		{
		}

		/// <summary>Instantiates an <see cref="T:System.Threading.AsyncLocal`1" /> local instance that receives change notifications.</summary>
		/// <param name="valueChangedHandler">The delegate that is called whenever the current value changes on any thread.</param>
		public AsyncLocal(Action<AsyncLocalValueChangedArgs<T>> valueChangedHandler)
		{
			this.m_valueChangedHandler = valueChangedHandler;
		}

		/// <summary>Gets or sets the value of the ambient data.</summary>
		/// <returns>The value of the ambient data. If no value has been set, the returned value is default(T).</returns>
		public T Value
		{
			get
			{
				object localValue = ExecutionContext.GetLocalValue(this);
				if (localValue != null)
				{
					return (T)((object)localValue);
				}
				return default(T);
			}
			set
			{
				ExecutionContext.SetLocalValue(this, value, this.m_valueChangedHandler != null);
			}
		}

		void IAsyncLocal.OnValueChanged(object previousValueObj, object currentValueObj, bool contextChanged)
		{
			T previousValue = (previousValueObj == null) ? default(T) : ((T)((object)previousValueObj));
			T currentValue = (currentValueObj == null) ? default(T) : ((T)((object)currentValueObj));
			this.m_valueChangedHandler(new AsyncLocalValueChangedArgs<T>(previousValue, currentValue, contextChanged));
		}

		private readonly Action<AsyncLocalValueChangedArgs<T>> m_valueChangedHandler;
	}
}
