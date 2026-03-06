using System;

namespace System
{
	/// <summary>Provides a custom constructor for uniform resource identifiers (URIs) and modifies URIs for the <see cref="T:System.Uri" /> class.</summary>
	public class UriBuilder
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.UriBuilder" /> class.</summary>
		public UriBuilder()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.UriBuilder" /> class with the specified URI.</summary>
		/// <param name="uri">A URI string.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="uri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UriFormatException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.FormatException" />, instead.  
		///
		///
		///     <paramref name="uri" /> is a zero length string or contains only spaces.  
		///  -or-  
		///  The parsing routine detected a scheme in an invalid form.  
		///  -or-  
		///  The parser detected more than two consecutive slashes in a URI that does not use the "file" scheme.  
		///  -or-  
		///  <paramref name="uri" /> is not a valid URI.</exception>
		public UriBuilder(string uri)
		{
			Uri uri2 = new Uri(uri, UriKind.RelativeOrAbsolute);
			if (uri2.IsAbsoluteUri)
			{
				this.Init(uri2);
				return;
			}
			uri = Uri.UriSchemeHttp + Uri.SchemeDelimiter + uri;
			this.Init(new Uri(uri));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.UriBuilder" /> class with the specified <see cref="T:System.Uri" /> instance.</summary>
		/// <param name="uri">An instance of the <see cref="T:System.Uri" /> class.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="uri" /> is <see langword="null" />.</exception>
		public UriBuilder(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			this.Init(uri);
		}

		private void Init(Uri uri)
		{
			this._fragment = uri.Fragment;
			this._query = uri.Query;
			this._host = uri.Host;
			this._path = uri.AbsolutePath;
			this._port = uri.Port;
			this._scheme = uri.Scheme;
			this._schemeDelimiter = (uri.HasAuthority ? Uri.SchemeDelimiter : ":");
			string userInfo = uri.UserInfo;
			if (!string.IsNullOrEmpty(userInfo))
			{
				int num = userInfo.IndexOf(':');
				if (num != -1)
				{
					this._password = userInfo.Substring(num + 1);
					this._username = userInfo.Substring(0, num);
				}
				else
				{
					this._username = userInfo;
				}
			}
			this.SetFieldsFromUri(uri);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.UriBuilder" /> class with the specified scheme and host.</summary>
		/// <param name="schemeName">An Internet access protocol.</param>
		/// <param name="hostName">A DNS-style domain name or IP address.</param>
		public UriBuilder(string schemeName, string hostName)
		{
			this.Scheme = schemeName;
			this.Host = hostName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.UriBuilder" /> class with the specified scheme, host, and port.</summary>
		/// <param name="scheme">An Internet access protocol.</param>
		/// <param name="host">A DNS-style domain name or IP address.</param>
		/// <param name="portNumber">An IP port number for the service.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="portNumber" /> is less than -1 or greater than 65,535.</exception>
		public UriBuilder(string scheme, string host, int portNumber) : this(scheme, host)
		{
			this.Port = portNumber;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.UriBuilder" /> class with the specified scheme, host, port number, and path.</summary>
		/// <param name="scheme">An Internet access protocol.</param>
		/// <param name="host">A DNS-style domain name or IP address.</param>
		/// <param name="port">An IP port number for the service.</param>
		/// <param name="pathValue">The path to the Internet resource.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="port" /> is less than -1 or greater than 65,535.</exception>
		public UriBuilder(string scheme, string host, int port, string pathValue) : this(scheme, host, port)
		{
			this.Path = pathValue;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.UriBuilder" /> class with the specified scheme, host, port number, path and query string or fragment identifier.</summary>
		/// <param name="scheme">An Internet access protocol.</param>
		/// <param name="host">A DNS-style domain name or IP address.</param>
		/// <param name="port">An IP port number for the service.</param>
		/// <param name="path">The path to the Internet resource.</param>
		/// <param name="extraValue">A query string or fragment identifier.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="extraValue" /> is neither <see langword="null" /> nor <see cref="F:System.String.Empty" />, nor does a valid fragment identifier begin with a number sign (#), nor a valid query string begin with a question mark (?).</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="port" /> is less than -1 or greater than 65,535.</exception>
		public UriBuilder(string scheme, string host, int port, string path, string extraValue) : this(scheme, host, port, path)
		{
			try
			{
				this.Extra = extraValue;
			}
			catch (Exception ex)
			{
				if (ex is OutOfMemoryException)
				{
					throw;
				}
				throw new ArgumentException("Extra portion of URI not valid.", "extraValue");
			}
		}

		private string Extra
		{
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (value.Length <= 0)
				{
					this.Fragment = string.Empty;
					this.Query = string.Empty;
					return;
				}
				if (value[0] == '#')
				{
					this.Fragment = value.Substring(1);
					return;
				}
				if (value[0] == '?')
				{
					int num = value.IndexOf('#');
					if (num == -1)
					{
						num = value.Length;
					}
					else
					{
						this.Fragment = value.Substring(num + 1);
					}
					this.Query = value.Substring(1, num - 1);
					return;
				}
				throw new ArgumentException("Extra portion of URI not valid.", "value");
			}
		}

		/// <summary>Gets or sets the fragment portion of the URI.</summary>
		/// <returns>The fragment portion of the URI. The fragment identifier ("#") is added to the beginning of the fragment.</returns>
		public string Fragment
		{
			get
			{
				return this._fragment;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (value.Length > 0 && value[0] != '#')
				{
					value = "#" + value;
				}
				this._fragment = value;
				this._changed = true;
			}
		}

		/// <summary>Gets or sets the Domain Name System (DNS) host name or IP address of a server.</summary>
		/// <returns>The DNS host name or IP address of the server.</returns>
		public string Host
		{
			get
			{
				return this._host;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				this._host = value;
				if (this._host.IndexOf(':') >= 0 && this._host[0] != '[')
				{
					this._host = "[" + this._host + "]";
				}
				this._changed = true;
			}
		}

		/// <summary>Gets or sets the password associated with the user that accesses the URI.</summary>
		/// <returns>The password of the user that accesses the URI.</returns>
		public string Password
		{
			get
			{
				return this._password;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				this._password = value;
				this._changed = true;
			}
		}

		/// <summary>Gets or sets the path to the resource referenced by the URI.</summary>
		/// <returns>The path to the resource referenced by the URI.</returns>
		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					value = "/";
				}
				this._path = Uri.InternalEscapeString(value.Replace('\\', '/'));
				this._changed = true;
			}
		}

		/// <summary>Gets or sets the port number of the URI.</summary>
		/// <returns>The port number of the URI.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port cannot be set to a value less than -1 or greater than 65,535.</exception>
		public int Port
		{
			get
			{
				return this._port;
			}
			set
			{
				if (value < -1 || value > 65535)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._port = value;
				this._changed = true;
			}
		}

		/// <summary>Gets or sets any query information included in the URI.</summary>
		/// <returns>The query information included in the URI.</returns>
		public string Query
		{
			get
			{
				return this._query;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (value.Length > 0 && value[0] != '?')
				{
					value = "?" + value;
				}
				this._query = value;
				this._changed = true;
			}
		}

		/// <summary>Gets or sets the scheme name of the URI.</summary>
		/// <returns>The scheme of the URI.</returns>
		/// <exception cref="T:System.ArgumentException">The scheme cannot be set to an invalid scheme name.</exception>
		public string Scheme
		{
			get
			{
				return this._scheme;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				int num = value.IndexOf(':');
				if (num != -1)
				{
					value = value.Substring(0, num);
				}
				if (value.Length != 0)
				{
					if (!Uri.CheckSchemeName(value))
					{
						throw new ArgumentException("Invalid URI: The URI scheme is not valid.", "value");
					}
					value = value.ToLowerInvariant();
				}
				this._scheme = value;
				this._changed = true;
			}
		}

		/// <summary>Gets the <see cref="T:System.Uri" /> instance constructed by the specified <see cref="T:System.UriBuilder" /> instance.</summary>
		/// <returns>A <see cref="T:System.Uri" /> that contains the URI constructed by the <see cref="T:System.UriBuilder" />.</returns>
		/// <exception cref="T:System.UriFormatException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.FormatException" />, instead.  
		///
		///
		///
		///
		///  The URI constructed by the <see cref="T:System.UriBuilder" /> properties is invalid.</exception>
		public Uri Uri
		{
			get
			{
				if (this._changed)
				{
					this._uri = new Uri(this.ToString());
					this.SetFieldsFromUri(this._uri);
					this._changed = false;
				}
				return this._uri;
			}
		}

		/// <summary>The user name associated with the user that accesses the URI.</summary>
		/// <returns>The user name of the user that accesses the URI.</returns>
		public string UserName
		{
			get
			{
				return this._username;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				this._username = value;
				this._changed = true;
			}
		}

		/// <summary>Compares an existing <see cref="T:System.Uri" /> instance with the contents of the <see cref="T:System.UriBuilder" /> for equality.</summary>
		/// <param name="rparam">The object to compare with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="rparam" /> represents the same <see cref="T:System.Uri" /> as the <see cref="T:System.Uri" /> constructed by this <see cref="T:System.UriBuilder" /> instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object rparam)
		{
			return rparam != null && this.Uri.Equals(rparam.ToString());
		}

		/// <summary>Returns the hash code for the URI.</summary>
		/// <returns>The hash code generated for the URI.</returns>
		public override int GetHashCode()
		{
			return this.Uri.GetHashCode();
		}

		private void SetFieldsFromUri(Uri uri)
		{
			this._fragment = uri.Fragment;
			this._query = uri.Query;
			this._host = uri.Host;
			this._path = uri.AbsolutePath;
			this._port = uri.Port;
			this._scheme = uri.Scheme;
			this._schemeDelimiter = (uri.HasAuthority ? Uri.SchemeDelimiter : ":");
			string userInfo = uri.UserInfo;
			if (userInfo.Length > 0)
			{
				int num = userInfo.IndexOf(':');
				if (num != -1)
				{
					this._password = userInfo.Substring(num + 1);
					this._username = userInfo.Substring(0, num);
					return;
				}
				this._username = userInfo;
			}
		}

		/// <summary>Returns the display string for the specified <see cref="T:System.UriBuilder" /> instance.</summary>
		/// <returns>The string that contains the unescaped display string of the <see cref="T:System.UriBuilder" />.</returns>
		/// <exception cref="T:System.UriFormatException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.FormatException" />, instead.  
		///
		///
		///
		///
		///  The <see cref="T:System.UriBuilder" /> instance has a bad password.</exception>
		public override string ToString()
		{
			if (this._username.Length == 0 && this._password.Length > 0)
			{
				throw new UriFormatException("Invalid URI: The username:password construct is badly formed.");
			}
			if (this._scheme.Length != 0)
			{
				UriParser syntax = UriParser.GetSyntax(this._scheme);
				if (syntax != null)
				{
					this._schemeDelimiter = ((syntax.InFact(UriSyntaxFlags.MustHaveAuthority) || (this._host.Length != 0 && syntax.NotAny(UriSyntaxFlags.MailToLikeUri) && syntax.InFact(UriSyntaxFlags.OptionalAuthority))) ? Uri.SchemeDelimiter : ":");
				}
				else
				{
					this._schemeDelimiter = ((this._host.Length != 0) ? Uri.SchemeDelimiter : ":");
				}
			}
			string text = (this._scheme.Length != 0) ? (this._scheme + this._schemeDelimiter) : string.Empty;
			return string.Concat(new string[]
			{
				text,
				this._username,
				(this._password.Length > 0) ? (":" + this._password) : string.Empty,
				(this._username.Length > 0) ? "@" : string.Empty,
				this._host,
				(this._port != -1 && this._host.Length > 0) ? (":" + this._port.ToString()) : string.Empty,
				(this._host.Length > 0 && this._path.Length != 0 && this._path[0] != '/') ? "/" : string.Empty,
				this._path,
				this._query,
				this._fragment
			});
		}

		private bool _changed = true;

		private string _fragment = string.Empty;

		private string _host = "localhost";

		private string _password = string.Empty;

		private string _path = "/";

		private int _port = -1;

		private string _query = string.Empty;

		private string _scheme = "http";

		private string _schemeDelimiter = Uri.SchemeDelimiter;

		private Uri _uri;

		private string _username = string.Empty;
	}
}
