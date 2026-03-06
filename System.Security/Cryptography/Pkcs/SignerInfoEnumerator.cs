using System;
using System.Collections;

namespace System.Security.Cryptography.Pkcs
{
	/// <summary>The <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoEnumerator" /> class provides enumeration functionality for the <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoCollection" /> collection. <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoEnumerator" /> implements the <see cref="T:System.Collections.IEnumerator" /> interface.</summary>
	public sealed class SignerInfoEnumerator : IEnumerator
	{
		private SignerInfoEnumerator()
		{
		}

		internal SignerInfoEnumerator(SignerInfoCollection signerInfos)
		{
			this._signerInfos = signerInfos;
			this._position = -1;
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.SignerInfoEnumerator.Current" /> property retrieves the current <see cref="T:System.Security.Cryptography.Pkcs.SignerInfo" /> object from the <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoCollection" /> collection.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Pkcs.SignerInfo" /> object that represents the current signer information structure in the <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoCollection" /> collection.</returns>
		public SignerInfo Current
		{
			get
			{
				return this._signerInfos[this._position];
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.SignerInfoEnumerator.System#Collections#IEnumerator#Current" /> property retrieves the current <see cref="T:System.Security.Cryptography.Pkcs.SignerInfo" /> object from the <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoCollection" /> collection.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Pkcs.SignerInfo" /> object that represents the current signer information structure in the <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoCollection" /> collection.</returns>
		object IEnumerator.Current
		{
			get
			{
				return this._signerInfos[this._position];
			}
		}

		/// <summary>The <see cref="M:System.Security.Cryptography.Pkcs.SignerInfoEnumerator.MoveNext" /> method advances the enumeration to the next   <see cref="T:System.Security.Cryptography.Pkcs.SignerInfo" /> object in the <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoCollection" /> collection.</summary>
		/// <returns>This method returns a bool value that specifies whether the enumeration successfully advanced. If the enumeration successfully moved to the next <see cref="T:System.Security.Cryptography.Pkcs.SignerInfo" /> object, the method returns <see langword="true" />. If the enumeration moved past the last item in the enumeration, it returns <see langword="false" />.</returns>
		public bool MoveNext()
		{
			int num = this._position + 1;
			if (num >= this._signerInfos.Count)
			{
				return false;
			}
			this._position = num;
			return true;
		}

		/// <summary>The <see cref="M:System.Security.Cryptography.Pkcs.SignerInfoEnumerator.Reset" /> method resets the enumeration to the first <see cref="T:System.Security.Cryptography.Pkcs.SignerInfo" /> object in the <see cref="T:System.Security.Cryptography.Pkcs.SignerInfoCollection" /> collection.</summary>
		public void Reset()
		{
			this._position = -1;
		}

		private readonly SignerInfoCollection _signerInfos;

		private int _position;
	}
}
