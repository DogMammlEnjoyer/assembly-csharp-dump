using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	/// <summary>The <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientEnumerator" /> class provides enumeration functionality for the <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientCollection" /> collection. <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientEnumerator" /> implements the <see cref="T:System.Collections.IEnumerator" /> interface.</summary>
	public sealed class CmsRecipientEnumerator : IEnumerator
	{
		internal CmsRecipientEnumerator(CmsRecipientCollection recipients)
		{
			this._recipients = recipients;
			this._current = -1;
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.CmsRecipientEnumerator.Current" /> property retrieves the current <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipient" /> object from the <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientCollection" /> collection.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipient" /> object that represents the current recipient in the <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientCollection" /> collection.</returns>
		public CmsRecipient Current
		{
			get
			{
				return this._recipients[this._current];
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.CmsRecipientEnumerator.System#Collections#IEnumerator#Current" /> property retrieves the current <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipient" /> object from the <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientCollection" /> collection.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipient" /> object that represents the current recipient in the <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientCollection" /> collection.</returns>
		object IEnumerator.Current
		{
			get
			{
				return this._recipients[this._current];
			}
		}

		/// <summary>The <see cref="M:System.Security.Cryptography.Pkcs.CmsRecipientEnumerator.MoveNext" /> method advances the enumeration to the next <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipient" /> object in the <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientCollection" /> collection.</summary>
		/// <returns>
		///   <see langword="true" /> if the enumeration successfully moved to the next <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipient" /> object; <see langword="false" /> if the enumeration moved past the last item in the enumeration.</returns>
		public bool MoveNext()
		{
			if (this._current >= this._recipients.Count - 1)
			{
				return false;
			}
			this._current++;
			return true;
		}

		/// <summary>The <see cref="M:System.Security.Cryptography.Pkcs.CmsRecipientEnumerator.Reset" /> method resets the enumeration to the first <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipient" /> object in the <see cref="T:System.Security.Cryptography.Pkcs.CmsRecipientCollection" /> collection.</summary>
		public void Reset()
		{
			this._current = -1;
		}

		internal CmsRecipientEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly CmsRecipientCollection _recipients;

		private int _current;
	}
}
