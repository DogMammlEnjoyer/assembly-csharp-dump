using System;
using Unity;

namespace System.Text
{
	/// <summary>Provides basic information about an encoding.</summary>
	[Serializable]
	public sealed class EncodingInfo
	{
		internal EncodingInfo(int codePage, string name, string displayName)
		{
			this.iCodePage = codePage;
			this.strEncodingName = name;
			this.strDisplayName = displayName;
		}

		/// <summary>Gets the code page identifier of the encoding.</summary>
		/// <returns>The code page identifier of the encoding.</returns>
		public int CodePage
		{
			get
			{
				return this.iCodePage;
			}
		}

		/// <summary>Gets the name registered with the Internet Assigned Numbers Authority (IANA) for the encoding.</summary>
		/// <returns>The IANA name for the encoding. For more information about the IANA, see www.iana.org.</returns>
		public string Name
		{
			get
			{
				return this.strEncodingName;
			}
		}

		/// <summary>Gets the human-readable description of the encoding.</summary>
		/// <returns>The human-readable description of the encoding.</returns>
		public string DisplayName
		{
			get
			{
				return this.strDisplayName;
			}
		}

		/// <summary>Returns a <see cref="T:System.Text.Encoding" /> object that corresponds to the current <see cref="T:System.Text.EncodingInfo" /> object.</summary>
		/// <returns>A <see cref="T:System.Text.Encoding" /> object that corresponds to the current <see cref="T:System.Text.EncodingInfo" /> object.</returns>
		public Encoding GetEncoding()
		{
			return Encoding.GetEncoding(this.iCodePage);
		}

		/// <summary>Gets a value indicating whether the specified object is equal to the current <see cref="T:System.Text.EncodingInfo" /> object.</summary>
		/// <param name="value">An object to compare to the current <see cref="T:System.Text.EncodingInfo" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is a <see cref="T:System.Text.EncodingInfo" /> object and is equal to the current <see cref="T:System.Text.EncodingInfo" /> object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object value)
		{
			EncodingInfo encodingInfo = value as EncodingInfo;
			return encodingInfo != null && this.CodePage == encodingInfo.CodePage;
		}

		/// <summary>Returns the hash code for the current <see cref="T:System.Text.EncodingInfo" /> object.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.CodePage;
		}

		internal EncodingInfo()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private int iCodePage;

		private string strEncodingName;

		private string strDisplayName;
	}
}
