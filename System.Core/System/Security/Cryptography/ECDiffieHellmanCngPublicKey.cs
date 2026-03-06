using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	/// <summary>Specifies an Elliptic Curve Diffie-Hellman (ECDH) public key for use with the <see cref="T:System.Security.Cryptography.ECDiffieHellmanCng" /> class.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class ECDiffieHellmanCngPublicKey : ECDiffieHellmanPublicKey
	{
		internal ECDiffieHellmanCngPublicKey()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the key BLOB format for a <see cref="T:System.Security.Cryptography.ECDiffieHellmanCngPublicKey" /> object.</summary>
		/// <returns>The format that the key BLOB is expressed in.</returns>
		public CngKeyBlobFormat BlobFormat
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Converts a byte array that contains a public key to a <see cref="T:System.Security.Cryptography.ECDiffieHellmanCngPublicKey" /> object according to the specified format.</summary>
		/// <param name="publicKeyBlob">A byte array that contains an Elliptic Curve Diffie-Hellman (ECDH) public key.</param>
		/// <param name="format">An object that specifies the format of the key BLOB.</param>
		/// <returns>An object that contains the ECDH public key that is serialized in the byte array.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="publicKeyBlob" /> or <paramref name="format" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="publicKeyBlob" /> parameter does not contain an <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> key. </exception>
		[SecuritySafeCritical]
		public static ECDiffieHellmanPublicKey FromByteArray(byte[] publicKeyBlob, CngKeyBlobFormat format)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Converts an XML string to an <see cref="T:System.Security.Cryptography.ECDiffieHellmanCngPublicKey" /> object.</summary>
		/// <param name="xml">An XML string that contains an Elliptic Curve Diffie-Hellman (ECDH) key.</param>
		/// <returns>An object that contains the ECDH public key that is specified by the given XML.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="xml" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="xml" /> parameter does not specify an <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> key.</exception>
		[SecuritySafeCritical]
		public static ECDiffieHellmanCngPublicKey FromXmlString(string xml)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Converts the <see cref="T:System.Security.Cryptography.ECDiffieHellmanCngPublicKey" /> object to a <see cref="T:System.Security.Cryptography.CngKey" /> object.</summary>
		/// <returns>An object that contains the key represented by the <see cref="T:System.Security.Cryptography.ECDiffieHellmanCngPublicKey" /> object.</returns>
		public CngKey Import()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
