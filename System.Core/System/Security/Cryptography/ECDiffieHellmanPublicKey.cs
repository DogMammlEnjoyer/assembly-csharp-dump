using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	/// <summary>Provides an abstract base class from which all <see cref="T:System.Security.Cryptography.ECDiffieHellmanCngPublicKey" /> implementations must inherit.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public abstract class ECDiffieHellmanPublicKey : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.ECDiffieHellmanPublicKey" /> class.</summary>
		protected ECDiffieHellmanPublicKey()
		{
			this.m_keyBlob = new byte[0];
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.ECDiffieHellmanPublicKey" /> class.</summary>
		/// <param name="keyBlob">A byte array that represents an <see cref="T:System.Security.Cryptography.ECDiffieHellmanPublicKey" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="keyBlob" /> is <see langword="null" />.</exception>
		protected ECDiffieHellmanPublicKey(byte[] keyBlob)
		{
			if (keyBlob == null)
			{
				throw new ArgumentNullException("keyBlob");
			}
			this.m_keyBlob = (keyBlob.Clone() as byte[]);
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> class and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///       <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>Serializes the <see cref="T:System.Security.Cryptography.ECDiffieHellmanPublicKey" /> key BLOB to a byte array.</summary>
		/// <returns>A byte array that contains the serialized Elliptic Curve Diffie-Hellman (ECDH) public key.</returns>
		public virtual byte[] ToByteArray()
		{
			return this.m_keyBlob.Clone() as byte[];
		}

		/// <summary>Serializes the <see cref="T:System.Security.Cryptography.ECDiffieHellmanPublicKey" /> public key to an XML string.</summary>
		/// <returns>An XML string that contains the serialized Elliptic Curve Diffie-Hellman (ECDH) public key.</returns>
		public virtual string ToXmlString()
		{
			throw new NotImplementedException(SR.GetString("Method not supported. Derived class must override."));
		}

		/// <summary>When overridden in a derived class, exports the named or explicit <see cref="T:System.Security.Cryptography.ECParameters" /> for an <see cref="T:System.Security.Cryptography.ECCurve" /> object.  </summary>
		/// <returns>An object that represents the point on the curve for this key.</returns>
		/// <exception cref="T:System.NotSupportedException">A derived class must override this method.</exception>
		public virtual ECParameters ExportParameters()
		{
			throw new NotSupportedException(SR.GetString("Method not supported. Derived class must override."));
		}

		/// <summary>When overridden in a derived class, exports the explicit <see cref="T:System.Security.Cryptography.ECParameters" /> for an <see cref="T:System.Security.Cryptography.ECCurve" /> object.  </summary>
		/// <returns>An object that represents the point on the curve for this key, using the explicit curve format. </returns>
		/// <exception cref="T:System.NotSupportedException">A derived class must override this method.</exception>
		public virtual ECParameters ExportExplicitParameters()
		{
			throw new NotSupportedException(SR.GetString("Method not supported. Derived class must override."));
		}

		private byte[] m_keyBlob;
	}
}
