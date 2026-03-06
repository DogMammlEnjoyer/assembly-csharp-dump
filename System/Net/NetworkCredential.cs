using System;
using System.Security;

namespace System.Net
{
	/// <summary>Provides credentials for password-based authentication schemes such as basic, digest, NTLM, and Kerberos authentication.</summary>
	public class NetworkCredential : ICredentials, ICredentialsByHost
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.NetworkCredential" /> class.</summary>
		public NetworkCredential() : this(string.Empty, string.Empty, string.Empty)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.NetworkCredential" /> class with the specified user name and password.</summary>
		/// <param name="userName">The user name associated with the credentials.</param>
		/// <param name="password">The password for the user name associated with the credentials.</param>
		public NetworkCredential(string userName, string password) : this(userName, password, string.Empty)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.NetworkCredential" /> class with the specified user name and password.</summary>
		/// <param name="userName">The user name associated with the credentials.</param>
		/// <param name="password">The password for the user name associated with the credentials.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Security.SecureString" /> class is not supported on this platform.</exception>
		public NetworkCredential(string userName, SecureString password) : this(userName, password, string.Empty)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.NetworkCredential" /> class with the specified user name, password, and domain.</summary>
		/// <param name="userName">The user name associated with the credentials.</param>
		/// <param name="password">The password for the user name associated with the credentials.</param>
		/// <param name="domain">The domain associated with these credentials.</param>
		public NetworkCredential(string userName, string password, string domain)
		{
			this.UserName = userName;
			this.Password = password;
			this.Domain = domain;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.NetworkCredential" /> class with the specified user name, password, and domain.</summary>
		/// <param name="userName">The user name associated with the credentials.</param>
		/// <param name="password">The password for the user name associated with the credentials.</param>
		/// <param name="domain">The domain associated with these credentials.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Security.SecureString" /> class is not supported on this platform.</exception>
		public NetworkCredential(string userName, SecureString password, string domain)
		{
			this.UserName = userName;
			this.SecurePassword = password;
			this.Domain = domain;
		}

		/// <summary>Gets or sets the user name associated with the credentials.</summary>
		/// <returns>The user name associated with the credentials.</returns>
		public string UserName
		{
			get
			{
				return this.InternalGetUserName();
			}
			set
			{
				if (value == null)
				{
					this.m_userName = string.Empty;
					return;
				}
				this.m_userName = value;
			}
		}

		/// <summary>Gets or sets the password for the user name associated with the credentials.</summary>
		/// <returns>The password associated with the credentials. If this <see cref="T:System.Net.NetworkCredential" /> instance was initialized with the <paramref name="password" /> parameter set to <see langword="null" />, then the <see cref="P:System.Net.NetworkCredential.Password" /> property will return an empty string.</returns>
		public string Password
		{
			get
			{
				return this.InternalGetPassword();
			}
			set
			{
				this.m_password = UnsafeNclNativeMethods.SecureStringHelper.CreateSecureString(value);
			}
		}

		/// <summary>Gets or sets the password as a <see cref="T:System.Security.SecureString" /> instance.</summary>
		/// <returns>The password for the user name associated with the credentials.</returns>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Security.SecureString" /> class is not supported on this platform.</exception>
		public SecureString SecurePassword
		{
			get
			{
				return this.InternalGetSecurePassword().Copy();
			}
			set
			{
				if (value == null)
				{
					this.m_password = new SecureString();
					return;
				}
				this.m_password = value.Copy();
			}
		}

		/// <summary>Gets or sets the domain or computer name that verifies the credentials.</summary>
		/// <returns>The name of the domain associated with the credentials.</returns>
		public string Domain
		{
			get
			{
				return this.InternalGetDomain();
			}
			set
			{
				if (value == null)
				{
					this.m_domain = string.Empty;
					return;
				}
				this.m_domain = value;
			}
		}

		internal string InternalGetUserName()
		{
			return this.m_userName;
		}

		internal string InternalGetPassword()
		{
			return UnsafeNclNativeMethods.SecureStringHelper.CreateString(this.m_password);
		}

		internal SecureString InternalGetSecurePassword()
		{
			return this.m_password;
		}

		internal string InternalGetDomain()
		{
			return this.m_domain;
		}

		internal string InternalGetDomainUserName()
		{
			string text = this.InternalGetDomain();
			if (text.Length != 0)
			{
				text += "\\";
			}
			return text + this.InternalGetUserName();
		}

		/// <summary>Returns an instance of the <see cref="T:System.Net.NetworkCredential" /> class for the specified Uniform Resource Identifier (URI) and authentication type.</summary>
		/// <param name="uri">The URI that the client provides authentication for.</param>
		/// <param name="authType">The type of authentication requested, as defined in the <see cref="P:System.Net.IAuthenticationModule.AuthenticationType" /> property.</param>
		/// <returns>A <see cref="T:System.Net.NetworkCredential" /> object.</returns>
		public NetworkCredential GetCredential(Uri uri, string authType)
		{
			return this;
		}

		/// <summary>Returns an instance of the <see cref="T:System.Net.NetworkCredential" /> class for the specified host, port, and authentication type.</summary>
		/// <param name="host">The host computer that authenticates the client.</param>
		/// <param name="port">The port on the <paramref name="host" /> that the client communicates with.</param>
		/// <param name="authenticationType">The type of authentication requested, as defined in the <see cref="P:System.Net.IAuthenticationModule.AuthenticationType" /> property.</param>
		/// <returns>A <see cref="T:System.Net.NetworkCredential" /> for the specified host, port, and authentication protocol, or <see langword="null" /> if there are no credentials available for the specified host, port, and authentication protocol.</returns>
		public NetworkCredential GetCredential(string host, int port, string authenticationType)
		{
			return this;
		}

		private string m_domain;

		private string m_userName;

		private SecureString m_password;
	}
}
