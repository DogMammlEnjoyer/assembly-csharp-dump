using System;
using System.Security.Permissions;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	/// <summary>Represents an X.509 store, which is a physical store where certificates are persisted and managed. This class cannot be inherited.</summary>
	public sealed class X509Store : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" /> class using the personal certificates of the current user store.</summary>
		public X509Store() : this("MY", StoreLocation.CurrentUser)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" /> class using the specified store name.</summary>
		/// <param name="storeName">A string value that represents the store name. See <see cref="T:System.Security.Cryptography.X509Certificates.StoreName" /> for more information.</param>
		public X509Store(string storeName) : this(storeName, StoreLocation.CurrentUser)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" /> class using the specified <see cref="T:System.Security.Cryptography.X509Certificates.StoreName" /> value.</summary>
		/// <param name="storeName">One of the enumeration values that specifies the name of the X.509 certificate store.</param>
		public X509Store(StoreName storeName) : this(storeName, StoreLocation.CurrentUser)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" /> class using the specified <see cref="T:System.Security.Cryptography.X509Certificates.StoreLocation" /> value.</summary>
		/// <param name="storeLocation">One of the enumeration values that specifies the location of the X.509 certificate store.</param>
		public X509Store(StoreLocation storeLocation) : this("MY", storeLocation)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" /> class using the specified <see cref="T:System.Security.Cryptography.X509Certificates.StoreName" /> and <see cref="T:System.Security.Cryptography.X509Certificates.StoreLocation" /> values.</summary>
		/// <param name="storeName">One of the enumeration values that specifies the name of the X.509 certificate store.</param>
		/// <param name="storeLocation">One of the enumeration values that specifies the location of the X.509 certificate store.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="storeLocation" /> is not a valid location or <paramref name="storeName" /> is not a valid name.</exception>
		public X509Store(StoreName storeName, StoreLocation storeLocation)
		{
			if (storeName < StoreName.AddressBook || storeName > StoreName.TrustedPublisher)
			{
				throw new ArgumentException("storeName");
			}
			if (storeLocation < StoreLocation.CurrentUser || storeLocation > StoreLocation.LocalMachine)
			{
				throw new ArgumentException("storeLocation");
			}
			if (storeName == StoreName.CertificateAuthority)
			{
				this._name = "CA";
			}
			else
			{
				this._name = storeName.ToString();
			}
			this._location = storeLocation;
		}

		public X509Store(StoreName storeName, StoreLocation storeLocation, OpenFlags openFlags) : this(storeName, storeLocation)
		{
			this._flags = openFlags;
		}

		public X509Store(string storeName, StoreLocation storeLocation, OpenFlags openFlags) : this(storeName, storeLocation)
		{
			this._flags = openFlags;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" /> class using an Intptr handle to an <see langword="HCERTSTORE" /> store.</summary>
		/// <param name="storeHandle">A handle to an <see langword="HCERTSTORE" /> store.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="storeHandle" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="storeHandle" /> parameter points to an invalid context.</exception>
		[MonoTODO("Mono's stores are fully managed. All handles are invalid.")]
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public X509Store(IntPtr storeHandle)
		{
			if (storeHandle == IntPtr.Zero)
			{
				throw new ArgumentNullException("storeHandle");
			}
			throw new CryptographicException("Invalid handle.");
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" /> class using a string that represents a value from the <see cref="T:System.Security.Cryptography.X509Certificates.StoreName" /> enumeration and a value from the <see cref="T:System.Security.Cryptography.X509Certificates.StoreLocation" /> enumeration.</summary>
		/// <param name="storeName">A string that represents a value from the <see cref="T:System.Security.Cryptography.X509Certificates.StoreName" /> enumeration.</param>
		/// <param name="storeLocation">One of the enumeration values that specifies the location of the X.509 certificate store.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="storeLocation" /> contains invalid values.</exception>
		public X509Store(string storeName, StoreLocation storeLocation)
		{
			if (storeLocation < StoreLocation.CurrentUser || storeLocation > StoreLocation.LocalMachine)
			{
				throw new ArgumentException("storeLocation");
			}
			this._name = storeName;
			this._location = storeLocation;
		}

		/// <summary>Returns a collection of certificates located in an X.509 certificate store.</summary>
		/// <returns>A collection of certificates.</returns>
		public X509Certificate2Collection Certificates
		{
			get
			{
				if (this.list == null)
				{
					this.list = new X509Certificate2Collection();
				}
				else if (this.store == null)
				{
					this.list.Clear();
				}
				return this.list;
			}
		}

		/// <summary>Gets the location of the X.509 certificate store.</summary>
		/// <returns>The location of the certificate store.</returns>
		public StoreLocation Location
		{
			get
			{
				return this._location;
			}
		}

		/// <summary>Gets the name of the X.509 certificate store.</summary>
		/// <returns>The name of the certificate store.</returns>
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		private X509Stores Factory
		{
			get
			{
				if (this._location == StoreLocation.CurrentUser)
				{
					return X509StoreManager.CurrentUser;
				}
				return X509StoreManager.LocalMachine;
			}
		}

		public bool IsOpen
		{
			get
			{
				return this.store != null;
			}
		}

		private bool IsReadOnly
		{
			get
			{
				return (this._flags & OpenFlags.ReadWrite) == OpenFlags.ReadOnly;
			}
		}

		internal X509Store Store
		{
			get
			{
				return this.store;
			}
		}

		/// <summary>Gets an <see cref="T:System.IntPtr" /> handle to an <see langword="HCERTSTORE" /> store.</summary>
		/// <returns>A handle to an <see langword="HCERTSTORE" /> store.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The store is not open.</exception>
		[MonoTODO("Mono's stores are fully managed. Always returns IntPtr.Zero.")]
		public IntPtr StoreHandle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		/// <summary>Adds a certificate to an X.509 certificate store.</summary>
		/// <param name="certificate">The certificate to add.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="certificate" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The certificate could not be added to the store.</exception>
		public void Add(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (!this.IsOpen)
			{
				throw new CryptographicException(Locale.GetText("Store isn't opened."));
			}
			if (this.IsReadOnly)
			{
				throw new CryptographicException(Locale.GetText("Store is read-only."));
			}
			if (!this.Exists(certificate))
			{
				try
				{
					this.store.Import(new X509Certificate(certificate.RawData));
				}
				finally
				{
					this.Certificates.Add(certificate);
				}
			}
		}

		/// <summary>Adds a collection of certificates to an X.509 certificate store.</summary>
		/// <param name="certificates">The collection of certificates to add.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="certificates" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		[MonoTODO("Method isn't transactional (like documented)")]
		public void AddRange(X509Certificate2Collection certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			if (certificates.Count == 0)
			{
				return;
			}
			if (!this.IsOpen)
			{
				throw new CryptographicException(Locale.GetText("Store isn't opened."));
			}
			if (this.IsReadOnly)
			{
				throw new CryptographicException(Locale.GetText("Store is read-only."));
			}
			foreach (X509Certificate2 x509Certificate in certificates)
			{
				if (!this.Exists(x509Certificate))
				{
					try
					{
						this.store.Import(new X509Certificate(x509Certificate.RawData));
					}
					finally
					{
						this.Certificates.Add(x509Certificate);
					}
				}
			}
		}

		/// <summary>Closes an X.509 certificate store.</summary>
		public void Close()
		{
			this.store = null;
			if (this.list != null)
			{
				this.list.Clear();
			}
		}

		/// <summary>Releases the resources used by this <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" />.</summary>
		public void Dispose()
		{
			this.Close();
		}

		/// <summary>Opens an X.509 certificate store or creates a new store, depending on <see cref="T:System.Security.Cryptography.X509Certificates.OpenFlags" /> flag settings.</summary>
		/// <param name="flags">A bitwise combination of enumeration values that specifies the way to open the X.509 certificate store.</param>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The store is unreadable.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">The store contains invalid values.</exception>
		public void Open(OpenFlags flags)
		{
			if (string.IsNullOrEmpty(this._name))
			{
				throw new CryptographicException(Locale.GetText("Invalid store name (null or empty)."));
			}
			string storeName;
			if (this._name == "Root")
			{
				storeName = "Trust";
			}
			else
			{
				storeName = this._name;
			}
			bool create = (flags & OpenFlags.OpenExistingOnly) != OpenFlags.OpenExistingOnly;
			this.store = this.Factory.Open(storeName, create);
			if (this.store == null)
			{
				throw new CryptographicException(Locale.GetText("Store {0} doesn't exists.", new object[]
				{
					this._name
				}));
			}
			this._flags = flags;
			foreach (X509Certificate x509Certificate in this.store.Certificates)
			{
				X509Certificate2 x509Certificate2 = new X509Certificate2(x509Certificate.RawData);
				x509Certificate2.Impl.PrivateKey = x509Certificate.RSA;
				this.Certificates.Add(x509Certificate2);
			}
		}

		/// <summary>Removes a certificate from an X.509 certificate store.</summary>
		/// <param name="certificate">The certificate to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="certificate" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public void Remove(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (!this.IsOpen)
			{
				throw new CryptographicException(Locale.GetText("Store isn't opened."));
			}
			if (!this.Exists(certificate))
			{
				return;
			}
			if (this.IsReadOnly)
			{
				throw new CryptographicException(Locale.GetText("Store is read-only."));
			}
			try
			{
				this.store.Remove(new X509Certificate(certificate.RawData));
			}
			finally
			{
				this.Certificates.Remove(certificate);
			}
		}

		/// <summary>Removes a range of certificates from an X.509 certificate store.</summary>
		/// <param name="certificates">A range of certificates to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="certificates" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		[MonoTODO("Method isn't transactional (like documented)")]
		public void RemoveRange(X509Certificate2Collection certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			if (certificates.Count == 0)
			{
				return;
			}
			if (!this.IsOpen)
			{
				throw new CryptographicException(Locale.GetText("Store isn't opened."));
			}
			bool flag = false;
			foreach (X509Certificate2 certificate in certificates)
			{
				if (this.Exists(certificate))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			if (this.IsReadOnly)
			{
				throw new CryptographicException(Locale.GetText("Store is read-only."));
			}
			try
			{
				foreach (X509Certificate2 x509Certificate in certificates)
				{
					this.store.Remove(new X509Certificate(x509Certificate.RawData));
				}
			}
			finally
			{
				this.Certificates.RemoveRange(certificates);
			}
		}

		private bool Exists(X509Certificate2 certificate)
		{
			if (this.store == null || this.list == null || certificate == null)
			{
				return false;
			}
			foreach (X509Certificate2 other in this.list)
			{
				if (certificate.Equals(other))
				{
					return true;
				}
			}
			return false;
		}

		private string _name;

		private StoreLocation _location;

		private X509Certificate2Collection list;

		private OpenFlags _flags;

		private X509Store store;
	}
}
