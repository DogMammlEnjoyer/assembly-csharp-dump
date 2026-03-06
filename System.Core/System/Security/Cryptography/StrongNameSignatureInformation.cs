using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	/// <summary>Holds the strong name signature information for a manifest.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class StrongNameSignatureInformation
	{
		internal StrongNameSignatureInformation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the hash algorithm that is used to calculate the strong name signature.</summary>
		/// <returns>The name of the hash algorithm that is used to calculate the strong name signature.</returns>
		public string HashAlgorithm
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the HRESULT value of the result code.</summary>
		/// <returns>The HRESULT value of the result code.</returns>
		public int HResult
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Gets a value indicating whether the strong name signature is valid.</summary>
		/// <returns>
		///     <see langword="true" /> if the strong name signature is valid; otherwise, <see langword="false" />.</returns>
		public bool IsValid
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		/// <summary>Gets the public key that is used to verify the signature.</summary>
		/// <returns>The public key that is used to verify the signature. </returns>
		public AsymmetricAlgorithm PublicKey
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the results of verifying the strong name signature.</summary>
		/// <returns>The result codes for signature verification.</returns>
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
