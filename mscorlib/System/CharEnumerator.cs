using System;
using System.Collections;
using System.Collections.Generic;
using Unity;

namespace System
{
	/// <summary>Supports iterating over a <see cref="T:System.String" /> object and reading its individual characters. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class CharEnumerator : IEnumerator, IEnumerator<char>, IDisposable, ICloneable
	{
		internal CharEnumerator(string str)
		{
			this._str = str;
			this._index = -1;
		}

		/// <summary>Creates a copy of the current <see cref="T:System.CharEnumerator" /> object.</summary>
		/// <returns>An <see cref="T:System.Object" /> that is a copy of the current <see cref="T:System.CharEnumerator" /> object.</returns>
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Increments the internal index of the current <see cref="T:System.CharEnumerator" /> object to the next character of the enumerated string.</summary>
		/// <returns>
		///   <see langword="true" /> if the index is successfully incremented and within the enumerated string; otherwise, <see langword="false" />.</returns>
		public bool MoveNext()
		{
			if (this._index < this._str.Length - 1)
			{
				this._index++;
				this._currentElement = this._str[this._index];
				return true;
			}
			this._index = this._str.Length;
			return false;
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.CharEnumerator" /> class.</summary>
		public void Dispose()
		{
			if (this._str != null)
			{
				this._index = this._str.Length;
			}
			this._str = null;
		}

		/// <summary>Gets the currently referenced character in the string enumerated by this <see cref="T:System.CharEnumerator" /> object. For a description of this member, see <see cref="P:System.Collections.IEnumerator.Current" />.</summary>
		/// <returns>The boxed Unicode character currently referenced by this <see cref="T:System.CharEnumerator" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">Enumeration has not started.  
		///  -or-  
		///  Enumeration has ended.</exception>
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		/// <summary>Gets the currently referenced character in the string enumerated by this <see cref="T:System.CharEnumerator" /> object.</summary>
		/// <returns>The Unicode character currently referenced by this <see cref="T:System.CharEnumerator" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The index is invalid; that is, it is before the first or after the last character of the enumerated string.</exception>
		public char Current
		{
			get
			{
				if (this._index == -1)
				{
					throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
				}
				if (this._index >= this._str.Length)
				{
					throw new InvalidOperationException("Enumeration already finished.");
				}
				return this._currentElement;
			}
		}

		/// <summary>Initializes the index to a position logically before the first character of the enumerated string.</summary>
		public void Reset()
		{
			this._currentElement = '\0';
			this._index = -1;
		}

		internal CharEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string _str;

		private int _index;

		private char _currentElement;
	}
}
