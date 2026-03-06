using System;

namespace System.ComponentModel.Composition
{
	/// <summary>Holds an exported value created by an <see cref="T:System.ComponentModel.Composition.ExportFactory`1" /> object and a reference to a method to release that object.</summary>
	/// <typeparam name="T">The type of the exported value.</typeparam>
	public sealed class ExportLifetimeContext<T> : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.ExportLifetimeContext`1" /> class.</summary>
		/// <param name="value">The exported value.</param>
		/// <param name="disposeAction">A reference to a method to release the object.</param>
		public ExportLifetimeContext(T value, Action disposeAction)
		{
			this._value = value;
			this._disposeAction = disposeAction;
		}

		/// <summary>Gets the exported value of a <see cref="T:System.ComponentModel.Composition.ExportFactory`1" /> object.</summary>
		/// <returns>The exported value.</returns>
		public T Value
		{
			get
			{
				return this._value;
			}
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.ComponentModel.Composition.ExportLifetimeContext`1" /> class, including its associated export.</summary>
		public void Dispose()
		{
			if (this._disposeAction != null)
			{
				this._disposeAction();
			}
		}

		private readonly T _value;

		private readonly Action _disposeAction;
	}
}
