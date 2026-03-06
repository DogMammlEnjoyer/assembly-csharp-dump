using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography
{
	/// <summary>Provides enumeration functionality for the <see cref="T:System.Security.Cryptography.CryptographicAttributeObjectCollection" /> collection. This class cannot be inherited.</summary>
	public sealed class CryptographicAttributeObjectEnumerator : IEnumerator
	{
		internal CryptographicAttributeObjectEnumerator(CryptographicAttributeObjectCollection attributes)
		{
			this._attributes = attributes;
			this._current = -1;
		}

		/// <summary>Gets the current <see cref="T:System.Security.Cryptography.CryptographicAttributeObject" /> object from the <see cref="T:System.Security.Cryptography.CryptographicAttributeObjectCollection" /> collection.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.CryptographicAttributeObject" /> object that represents the current cryptographic attribute in the <see cref="T:System.Security.Cryptography.CryptographicAttributeObjectCollection" /> collection.</returns>
		public CryptographicAttributeObject Current
		{
			get
			{
				return this._attributes[this._current];
			}
		}

		/// <summary>Gets the current <see cref="T:System.Security.Cryptography.CryptographicAttributeObject" /> object from the <see cref="T:System.Security.Cryptography.CryptographicAttributeObjectCollection" /> collection.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.CryptographicAttributeObject" /> object that represents the current cryptographic attribute in the <see cref="T:System.Security.Cryptography.CryptographicAttributeObjectCollection" /> collection.</returns>
		object IEnumerator.Current
		{
			get
			{
				return this._attributes[this._current];
			}
		}

		/// <summary>Advances the enumeration to the next <see cref="T:System.Security.Cryptography.CryptographicAttributeObject" /> object in the <see cref="T:System.Security.Cryptography.CryptographicAttributeObjectCollection" /> collection.</summary>
		/// <returns>
		///   <see langword="true" /> if the enumeration successfully moved to the next <see cref="T:System.Security.Cryptography.CryptographicAttributeObject" /> object; <see langword="false" /> if the enumerator is at the end of the enumeration.</returns>
		public bool MoveNext()
		{
			if (this._current >= this._attributes.Count - 1)
			{
				return false;
			}
			this._current++;
			return true;
		}

		/// <summary>Resets the enumeration to the first <see cref="T:System.Security.Cryptography.CryptographicAttributeObject" /> object in the <see cref="T:System.Security.Cryptography.CryptographicAttributeObjectCollection" /> collection.</summary>
		public void Reset()
		{
			this._current = -1;
		}

		internal CryptographicAttributeObjectEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly CryptographicAttributeObjectCollection _attributes;

		private int _current;
	}
}
