using System;
using System.Collections;
using Unity;

namespace System.Runtime.Serialization
{
	/// <summary>Provides a formatter-friendly mechanism for parsing the data in <see cref="T:System.Runtime.Serialization.SerializationInfo" />. This class cannot be inherited.</summary>
	public sealed class SerializationInfoEnumerator : IEnumerator
	{
		internal SerializationInfoEnumerator(string[] members, object[] info, Type[] types, int numItems)
		{
			this._members = members;
			this._data = info;
			this._types = types;
			this._numItems = numItems - 1;
			this._currItem = -1;
			this._current = false;
		}

		/// <summary>Updates the enumerator to the next item.</summary>
		/// <returns>
		///   <see langword="true" /> if a new element is found; otherwise, <see langword="false" />.</returns>
		public bool MoveNext()
		{
			if (this._currItem < this._numItems)
			{
				this._currItem++;
				this._current = true;
			}
			else
			{
				this._current = false;
			}
			return this._current;
		}

		/// <summary>Gets the current item in the collection.</summary>
		/// <returns>A <see cref="T:System.Runtime.Serialization.SerializationEntry" /> that contains the current serialization data.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumeration has not started or has already ended.</exception>
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		/// <summary>Gets the item currently being examined.</summary>
		/// <returns>The item currently being examined.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator has not started enumerating items or has reached the end of the enumeration.</exception>
		public SerializationEntry Current
		{
			get
			{
				if (!this._current)
				{
					throw new InvalidOperationException("Enumeration has either not started or has already finished.");
				}
				return new SerializationEntry(this._members[this._currItem], this._data[this._currItem], this._types[this._currItem]);
			}
		}

		/// <summary>Resets the enumerator to the first item.</summary>
		public void Reset()
		{
			this._currItem = -1;
			this._current = false;
		}

		/// <summary>Gets the name for the item currently being examined.</summary>
		/// <returns>The item name.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator has not started enumerating items or has reached the end of the enumeration.</exception>
		public string Name
		{
			get
			{
				if (!this._current)
				{
					throw new InvalidOperationException("Enumeration has either not started or has already finished.");
				}
				return this._members[this._currItem];
			}
		}

		/// <summary>Gets the value of the item currently being examined.</summary>
		/// <returns>The value of the item currently being examined.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator has not started enumerating items or has reached the end of the enumeration.</exception>
		public object Value
		{
			get
			{
				if (!this._current)
				{
					throw new InvalidOperationException("Enumeration has either not started or has already finished.");
				}
				return this._data[this._currItem];
			}
		}

		/// <summary>Gets the type of the item currently being examined.</summary>
		/// <returns>The type of the item currently being examined.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator has not started enumerating items or has reached the end of the enumeration.</exception>
		public Type ObjectType
		{
			get
			{
				if (!this._current)
				{
					throw new InvalidOperationException("Enumeration has either not started or has already finished.");
				}
				return this._types[this._currItem];
			}
		}

		internal SerializationInfoEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly string[] _members;

		private readonly object[] _data;

		private readonly Type[] _types;

		private readonly int _numItems;

		private int _currItem;

		private bool _current;
	}
}
