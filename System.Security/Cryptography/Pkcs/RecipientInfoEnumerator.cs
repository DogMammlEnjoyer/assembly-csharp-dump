using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	/// <summary>The <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoEnumerator" /> class provides enumeration functionality for the <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoCollection" /> collection. <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoEnumerator" /> implements the <see cref="T:System.Collections.IEnumerator" /> interface.</summary>
	public sealed class RecipientInfoEnumerator : IEnumerator
	{
		internal RecipientInfoEnumerator(RecipientInfoCollection RecipientInfos)
		{
			this._recipientInfos = RecipientInfos;
			this._current = -1;
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.RecipientInfoEnumerator.Current" /> property retrieves the current <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfo" /> object from the <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoCollection" /> collection.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfo" /> object that represents the current recipient information structure in the <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoCollection" /> collection.</returns>
		public RecipientInfo Current
		{
			get
			{
				return this._recipientInfos[this._current];
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.RecipientInfoEnumerator.System#Collections#IEnumerator#Current" /> property retrieves the current <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfo" /> object from the <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoCollection" /> collection.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfo" /> object that represents the current recipient information structure in the <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoCollection" /> collection.</returns>
		object IEnumerator.Current
		{
			get
			{
				return this._recipientInfos[this._current];
			}
		}

		/// <summary>The <see cref="M:System.Security.Cryptography.Pkcs.RecipientInfoEnumerator.MoveNext" /> method advances the enumeration to the next <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfo" /> object in the <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoCollection" /> collection.</summary>
		/// <returns>This method returns a bool that specifies whether the enumeration successfully advanced. If the enumeration successfully moved to the next <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfo" /> object, the method returns <see langword="true" />. If the enumeration moved past the last item in the enumeration, it returns <see langword="false" />.</returns>
		public bool MoveNext()
		{
			if (this._current >= this._recipientInfos.Count - 1)
			{
				return false;
			}
			this._current++;
			return true;
		}

		/// <summary>The <see cref="M:System.Security.Cryptography.Pkcs.RecipientInfoEnumerator.Reset" /> method resets the enumeration to the first <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfo" /> object in the <see cref="T:System.Security.Cryptography.Pkcs.RecipientInfoCollection" /> collection.</summary>
		public void Reset()
		{
			this._current = -1;
		}

		internal RecipientInfoEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly RecipientInfoCollection _recipientInfos;

		private int _current;
	}
}
