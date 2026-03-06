using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography.X509Certificates
{
	/// <summary>Provides details about the time stamp that was applied to an Authenticode signature for a manifest. </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class TimestampInformation
	{
		internal TimestampInformation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the hash algorithm used to compute the time stamp signature.</summary>
		/// <returns>The hash algorithm used to compute the time stamp signature.</returns>
		public string HashAlgorithm
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the HRESULT value that results from verifying the signature.</summary>
		/// <returns>The HRESULT value that results from verifying the signature.</returns>
		public int HResult
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets a value indicating whether the time stamp of the signature is valid.</summary>
		/// <returns>
		///     <see langword="true" /> if the time stamp is valid; otherwise, <see langword="false" />. </returns>
		public bool IsValid
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		/// <summary>Gets the chain of certificates used to verify the time stamp of the signature.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> object that represents the certificate chain.</returns>
		public X509Chain SignatureChain
		{
			[SecuritySafeCritical]
			[StorePermission(SecurityAction.Demand, OpenStore = true, EnumerateCertificates = true)]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the certificate that signed the time stamp.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> object that represents the certificate.</returns>
		public X509Certificate2 SigningCertificate
		{
			[SecuritySafeCritical]
			[StorePermission(SecurityAction.Demand, OpenStore = true, EnumerateCertificates = true)]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the time stamp that was applied to the signature.</summary>
		/// <returns>A <see cref="T:System.DateTime" /> object that represents the time stamp.</returns>
		public DateTime Timestamp
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(DateTime);
			}
		}

		/// <summary>Gets the result of verifying the time stamp signature.</summary>
		/// <returns>One of the <see cref="T:System.Security.Cryptography.SignatureVerificationResult" /> values.</returns>
		public SignatureVerificationResult VerificationResult
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return SignatureVerificationResult.Valid;
			}
		}
	}
}
