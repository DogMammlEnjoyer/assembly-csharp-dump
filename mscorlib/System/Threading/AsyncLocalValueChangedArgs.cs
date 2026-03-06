using System;

namespace System.Threading
{
	/// <summary>The class that provides data change information to <see cref="T:System.Threading.AsyncLocal`1" /> instances that register for change notifications.</summary>
	/// <typeparam name="T">The type of the data.</typeparam>
	public readonly struct AsyncLocalValueChangedArgs<T>
	{
		/// <summary>Gets the data's previous value.</summary>
		/// <returns>The data's previous value.</returns>
		public T PreviousValue { get; }

		/// <summary>Gets the data's current value.</summary>
		/// <returns>The data's current value.</returns>
		public T CurrentValue { get; }

		/// <summary>Returns a value that indicates whether the value changes because of a change of execution context.</summary>
		/// <returns>
		///   <see langword="true" /> if the value changed because of a change of execution context; otherwise, <see langword="false" />.</returns>
		public bool ThreadContextChanged { get; }

		internal AsyncLocalValueChangedArgs(T previousValue, T currentValue, bool contextChanged)
		{
			this = default(AsyncLocalValueChangedArgs<T>);
			this.PreviousValue = previousValue;
			this.CurrentValue = currentValue;
			this.ThreadContextChanged = contextChanged;
		}
	}
}
