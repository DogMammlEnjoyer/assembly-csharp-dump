using System;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates
{
	/// <summary>Represents a chain-building engine for <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> certificates.</summary>
	public class X509Chain : IDisposable
	{
		internal X509ChainImpl Impl
		{
			get
			{
				X509Helper2.ThrowIfContextInvalid(this.impl);
				return this.impl;
			}
		}

		internal bool IsValid
		{
			get
			{
				return X509Helper2.IsValid(this.impl);
			}
		}

		internal void ThrowIfContextInvalid()
		{
			X509Helper2.ThrowIfContextInvalid(this.impl);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> class.</summary>
		public X509Chain() : this(false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> class specifying a value that indicates whether the machine context should be used.</summary>
		/// <param name="useMachineContext">
		///   <see langword="true" /> to use the machine context; <see langword="false" /> to use the current user context.</param>
		public X509Chain(bool useMachineContext)
		{
			this.impl = X509Helper2.CreateChainImpl(useMachineContext);
		}

		internal X509Chain(X509ChainImpl impl)
		{
			X509Helper2.ThrowIfContextInvalid(impl);
			this.impl = impl;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> class using an <see cref="T:System.IntPtr" /> handle to an X.509 chain.</summary>
		/// <param name="chainContext">An <see cref="T:System.IntPtr" /> handle to an X.509 chain.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="chainContext" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="chainContext" /> parameter points to an invalid context.</exception>
		[MonoTODO("Mono's X509Chain is fully managed. All handles are invalid.")]
		public X509Chain(IntPtr chainContext)
		{
			throw new NotSupportedException();
		}

		/// <summary>Gets a handle to an X.509 chain.</summary>
		/// <returns>An <see cref="T:System.IntPtr" /> handle to an X.509 chain.</returns>
		[MonoTODO("Mono's X509Chain is fully managed. Always returns IntPtr.Zero.")]
		public IntPtr ChainContext
		{
			get
			{
				if (this.impl != null && this.impl.IsValid)
				{
					return this.impl.Handle;
				}
				return IntPtr.Zero;
			}
		}

		/// <summary>Gets a collection of <see cref="T:System.Security.Cryptography.X509Certificates.X509ChainElement" /> objects.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.X509Certificates.X509ChainElementCollection" /> object.</returns>
		public X509ChainElementCollection ChainElements
		{
			get
			{
				return this.Impl.ChainElements;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.X509Certificates.X509ChainPolicy" /> to use when building an X.509 certificate chain.</summary>
		/// <returns>The <see cref="T:System.Security.Cryptography.X509Certificates.X509ChainPolicy" /> object associated with this X.509 chain.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value being set for this property is <see langword="null" />.</exception>
		public X509ChainPolicy ChainPolicy
		{
			get
			{
				return this.Impl.ChainPolicy;
			}
			set
			{
				this.Impl.ChainPolicy = value;
			}
		}

		/// <summary>Gets the status of each element in an <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> object.</summary>
		/// <returns>An array of <see cref="T:System.Security.Cryptography.X509Certificates.X509ChainStatus" /> objects.</returns>
		public X509ChainStatus[] ChainStatus
		{
			get
			{
				return this.Impl.ChainStatus;
			}
		}

		/// <summary>Gets a safe handle for this <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> instance.</summary>
		/// <returns>The safe handle for this <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> instance.</returns>
		public SafeX509ChainHandle SafeHandle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Builds an X.509 chain using the policy specified in <see cref="T:System.Security.Cryptography.X509Certificates.X509ChainPolicy" />.</summary>
		/// <param name="certificate">An <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the X.509 certificate is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="certificate" /> is not a valid certificate or is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="certificate" /> is unreadable.</exception>
		[MonoTODO("Not totally RFC3280 compliant, but neither is MS implementation...")]
		public bool Build(X509Certificate2 certificate)
		{
			return this.Impl.Build(certificate);
		}

		/// <summary>Clears the current <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> object.</summary>
		public void Reset()
		{
			this.Impl.Reset();
		}

		/// <summary>Creates an <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> object after querying for the mapping defined in the CryptoConfig file, and maps the chain to that mapping.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" /> object.</returns>
		public static X509Chain Create()
		{
			return (X509Chain)CryptoConfig.CreateFromName("X509Chain");
		}

		/// <summary>Releases all of the resources used by this <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" />.</summary>
		[SecuritySafeCritical]
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by this <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" />, and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.impl != null)
			{
				this.impl.Dispose();
				this.impl = null;
			}
		}

		~X509Chain()
		{
			this.Dispose(false);
		}

		private X509ChainImpl impl;
	}
}
