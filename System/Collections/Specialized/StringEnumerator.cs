using System;
using Unity;

namespace System.Collections.Specialized
{
	/// <summary>Supports a simple iteration over a <see cref="T:System.Collections.Specialized.StringCollection" />.</summary>
	public class StringEnumerator
	{
		internal StringEnumerator(StringCollection mappings)
		{
			this._temp = mappings;
			this._baseEnumerator = this._temp.GetEnumerator();
		}

		/// <summary>Gets the current element in the collection.</summary>
		/// <returns>The current element in the collection.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
		public string Current
		{
			get
			{
				return (string)this._baseEnumerator.Current;
			}
		}

		/// <summary>Advances the enumerator to the next element of the collection.</summary>
		/// <returns>
		///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
		public bool MoveNext()
		{
			return this._baseEnumerator.MoveNext();
		}

		/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
		public void Reset()
		{
			this._baseEnumerator.Reset();
		}

		internal StringEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator _baseEnumerator;

		private IEnumerable _temp;
	}
}
