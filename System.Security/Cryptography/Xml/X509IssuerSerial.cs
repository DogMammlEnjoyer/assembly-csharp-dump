using System;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the &lt;<see langword="X509IssuerSerial" />&gt; element of an XML digital signature.</summary>
	public struct X509IssuerSerial
	{
		internal X509IssuerSerial(string issuerName, string serialNumber)
		{
			this = default(X509IssuerSerial);
			this.IssuerName = issuerName;
			this.SerialNumber = serialNumber;
		}

		/// <summary>Gets or sets an X.509 certificate issuer's distinguished name.</summary>
		/// <returns>An X.509 certificate issuer's distinguished name.</returns>
		public string IssuerName { readonly get; set; }

		/// <summary>Gets or sets an X.509 certificate issuer's serial number.</summary>
		/// <returns>An X.509 certificate issuer's serial number.</returns>
		public string SerialNumber { readonly get; set; }
	}
}
