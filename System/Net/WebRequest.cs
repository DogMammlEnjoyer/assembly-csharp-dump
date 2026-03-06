using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net.Cache;
using System.Net.Configuration;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	/// <summary>Makes a request to a Uniform Resource Identifier (URI). This is an <see langword="abstract" /> class.</summary>
	[Serializable]
	public abstract class WebRequest : MarshalByRefObject, ISerializable
	{
		/// <summary>When overridden in a descendant class, gets the factory object derived from the <see cref="T:System.Net.IWebRequestCreate" /> class used to create the <see cref="T:System.Net.WebRequest" /> instantiated for making the request to the specified URI.</summary>
		/// <returns>The derived <see cref="T:System.Net.WebRequest" /> type returned by the <see cref="M:System.Net.IWebRequestCreate.Create(System.Uri)" /> method.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		public virtual IWebRequestCreate CreatorInstance
		{
			get
			{
				return WebRequest.webRequestCreate;
			}
		}

		/// <summary>Register an <see cref="T:System.Net.IWebRequestCreate" /> object.</summary>
		/// <param name="creator">The <see cref="T:System.Net.IWebRequestCreate" /> object to register.</param>
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void RegisterPortableWebRequestCreator(IWebRequestCreate creator)
		{
		}

		private static object InternalSyncObject
		{
			get
			{
				if (WebRequest.s_InternalSyncObject == null)
				{
					object value = new object();
					Interlocked.CompareExchange(ref WebRequest.s_InternalSyncObject, value, null);
				}
				return WebRequest.s_InternalSyncObject;
			}
		}

		internal static TimerThread.Queue DefaultTimerQueue
		{
			get
			{
				return WebRequest.s_DefaultTimerQueue;
			}
		}

		private static WebRequest Create(Uri requestUri, bool useUriBase)
		{
			bool on = Logging.On;
			WebRequestPrefixElement webRequestPrefixElement = null;
			bool flag = false;
			string text;
			if (!useUriBase)
			{
				text = requestUri.AbsoluteUri;
			}
			else
			{
				text = requestUri.Scheme + ":";
			}
			int length = text.Length;
			ArrayList prefixList = WebRequest.PrefixList;
			for (int i = 0; i < prefixList.Count; i++)
			{
				webRequestPrefixElement = (WebRequestPrefixElement)prefixList[i];
				if (length >= webRequestPrefixElement.Prefix.Length && string.Compare(webRequestPrefixElement.Prefix, 0, text, 0, webRequestPrefixElement.Prefix.Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				WebRequest result = webRequestPrefixElement.Creator.Create(requestUri);
				bool on2 = Logging.On;
				return result;
			}
			bool on3 = Logging.On;
			throw new NotSupportedException(SR.GetString("The URI prefix is not recognized."));
		}

		/// <summary>Initializes a new <see cref="T:System.Net.WebRequest" /> instance for the specified URI scheme.</summary>
		/// <param name="requestUriString">The URI that identifies the Internet resource.</param>
		/// <returns>A <see cref="T:System.Net.WebRequest" /> descendant for the specific URI scheme.</returns>
		/// <exception cref="T:System.NotSupportedException">The request scheme specified in <paramref name="requestUriString" /> has not been registered.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="requestUriString" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have <see cref="T:System.Net.WebPermissionAttribute" /> permission to connect to the requested URI or a URI that the request is redirected to.</exception>
		/// <exception cref="T:System.UriFormatException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.FormatException" />, instead.  
		///
		///
		///
		///
		///  The URI specified in <paramref name="requestUriString" /> is not a valid URI.</exception>
		public static WebRequest Create(string requestUriString)
		{
			if (requestUriString == null)
			{
				throw new ArgumentNullException("requestUriString");
			}
			return WebRequest.Create(new Uri(requestUriString), false);
		}

		/// <summary>Initializes a new <see cref="T:System.Net.WebRequest" /> instance for the specified URI scheme.</summary>
		/// <param name="requestUri">A <see cref="T:System.Uri" /> containing the URI of the requested resource.</param>
		/// <returns>A <see cref="T:System.Net.WebRequest" /> descendant for the specified URI scheme.</returns>
		/// <exception cref="T:System.NotSupportedException">The request scheme specified in <paramref name="requestUri" /> is not registered.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have <see cref="T:System.Net.WebPermissionAttribute" /> permission to connect to the requested URI or a URI that the request is redirected to.</exception>
		public static WebRequest Create(Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			return WebRequest.Create(requestUri, false);
		}

		/// <summary>Initializes a new <see cref="T:System.Net.WebRequest" /> instance for the specified URI scheme.</summary>
		/// <param name="requestUri">A <see cref="T:System.Uri" /> containing the URI of the requested resource.</param>
		/// <returns>A <see cref="T:System.Net.WebRequest" /> descendant for the specified URI scheme.</returns>
		/// <exception cref="T:System.NotSupportedException">The request scheme specified in <paramref name="requestUri" /> is not registered.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have <see cref="T:System.Net.WebPermissionAttribute" /> permission to connect to the requested URI or a URI that the request is redirected to.</exception>
		public static WebRequest CreateDefault(Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			return WebRequest.Create(requestUri, true);
		}

		/// <summary>Initializes a new <see cref="T:System.Net.HttpWebRequest" /> instance for the specified URI string.</summary>
		/// <param name="requestUriString">A URI string that identifies the Internet resource.</param>
		/// <returns>An <see cref="T:System.Net.HttpWebRequest" /> instance for the specific URI string.</returns>
		/// <exception cref="T:System.NotSupportedException">The request scheme specified in <paramref name="requestUriString" /> is the http or https scheme.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="requestUriString" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have <see cref="T:System.Net.WebPermissionAttribute" /> permission to connect to the requested URI or a URI that the request is redirected to.</exception>
		/// <exception cref="T:System.UriFormatException">The URI specified in <paramref name="requestUriString" /> is not a valid URI.</exception>
		public static HttpWebRequest CreateHttp(string requestUriString)
		{
			if (requestUriString == null)
			{
				throw new ArgumentNullException("requestUriString");
			}
			return WebRequest.CreateHttp(new Uri(requestUriString));
		}

		/// <summary>Initializes a new <see cref="T:System.Net.HttpWebRequest" /> instance for the specified URI.</summary>
		/// <param name="requestUri">A URI that identifies the Internet resource.</param>
		/// <returns>An <see cref="T:System.Net.HttpWebRequest" /> instance for the specific URI string.</returns>
		/// <exception cref="T:System.NotSupportedException">The request scheme specified in <paramref name="requestUri" /> is the http or https scheme.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have <see cref="T:System.Net.WebPermissionAttribute" /> permission to connect to the requested URI or a URI that the request is redirected to.</exception>
		/// <exception cref="T:System.UriFormatException">The URI specified in <paramref name="requestUri" /> is not a valid URI.</exception>
		public static HttpWebRequest CreateHttp(Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			if (requestUri.Scheme != Uri.UriSchemeHttp && requestUri.Scheme != Uri.UriSchemeHttps)
			{
				throw new NotSupportedException(SR.GetString("The URI prefix is not recognized."));
			}
			return (HttpWebRequest)WebRequest.CreateDefault(requestUri);
		}

		/// <summary>Registers a <see cref="T:System.Net.WebRequest" /> descendant for the specified URI.</summary>
		/// <param name="prefix">The complete URI or URI prefix that the <see cref="T:System.Net.WebRequest" /> descendant services.</param>
		/// <param name="creator">The create method that the <see cref="T:System.Net.WebRequest" /> calls to create the <see cref="T:System.Net.WebRequest" /> descendant.</param>
		/// <returns>
		///   <see langword="true" /> if registration is successful; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="prefix" /> is <see langword="null" />  
		/// -or-  
		/// <paramref name="creator" /> is <see langword="null" />.</exception>
		public static bool RegisterPrefix(string prefix, IWebRequestCreate creator)
		{
			bool flag = false;
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			object internalSyncObject = WebRequest.InternalSyncObject;
			lock (internalSyncObject)
			{
				ArrayList arrayList = (ArrayList)WebRequest.PrefixList.Clone();
				Uri uri;
				if (Uri.TryCreate(prefix, UriKind.Absolute, out uri))
				{
					string text = uri.AbsoluteUri;
					if (!prefix.EndsWith("/", StringComparison.Ordinal) && uri.GetComponents(UriComponents.Path | UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped).Equals("/"))
					{
						text = text.Substring(0, text.Length - 1);
					}
					prefix = text;
				}
				int i;
				for (i = 0; i < arrayList.Count; i++)
				{
					WebRequestPrefixElement webRequestPrefixElement = (WebRequestPrefixElement)arrayList[i];
					if (prefix.Length > webRequestPrefixElement.Prefix.Length)
					{
						break;
					}
					if (prefix.Length == webRequestPrefixElement.Prefix.Length && string.Compare(webRequestPrefixElement.Prefix, prefix, StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					arrayList.Insert(i, new WebRequestPrefixElement(prefix, creator));
					WebRequest.PrefixList = arrayList;
				}
			}
			return !flag;
		}

		internal static ArrayList PrefixList
		{
			get
			{
				if (WebRequest.s_PrefixList == null)
				{
					object internalSyncObject = WebRequest.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (WebRequest.s_PrefixList == null)
						{
							WebRequest.s_PrefixList = WebRequest.PopulatePrefixList();
						}
					}
				}
				return WebRequest.s_PrefixList;
			}
			set
			{
				WebRequest.s_PrefixList = value;
			}
		}

		private static ArrayList PopulatePrefixList()
		{
			ArrayList arrayList = new ArrayList();
			if (Console.IsRunningOnAndroid)
			{
				IWebRequestCreate c = new HttpRequestCreator();
				arrayList.Add(new WebRequestPrefixElement("http", c));
				arrayList.Add(new WebRequestPrefixElement("https", c));
				arrayList.Add(new WebRequestPrefixElement("file", new FileWebRequestCreator()));
				arrayList.Add(new WebRequestPrefixElement("ftp", new FtpWebRequestCreator()));
			}
			else
			{
				WebRequestModulesSection webRequestModulesSection = ConfigurationManager.GetSection("system.net/webRequestModules") as WebRequestModulesSection;
				if (webRequestModulesSection != null)
				{
					foreach (object obj in webRequestModulesSection.WebRequestModules)
					{
						WebRequestModuleElement webRequestModuleElement = (WebRequestModuleElement)obj;
						arrayList.Add(new WebRequestPrefixElement(webRequestModuleElement.Prefix, webRequestModuleElement.Type));
					}
				}
			}
			return arrayList;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.WebRequest" /> class.</summary>
		protected WebRequest()
		{
			this.m_ImpersonationLevel = TokenImpersonationLevel.Delegation;
			this.m_AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.WebRequest" /> class from the specified instances of the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" /> classes.</summary>
		/// <param name="serializationInfo">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that contains the information required to serialize the new <see cref="T:System.Net.WebRequest" /> instance.</param>
		/// <param name="streamingContext">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that indicates the source of the serialized stream associated with the new <see cref="T:System.Net.WebRequest" /> instance.</param>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the constructor, when the constructor is not overridden in a descendant class.</exception>
		protected WebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
		}

		/// <summary>When overridden in a descendant class, populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> instance with the data needed to serialize the <see cref="T:System.Net.WebRequest" />.</summary>
		/// <param name="serializationInfo">A <see cref="T:System.Runtime.Serialization.SerializationInfo" />, which holds the serialized data for the <see cref="T:System.Net.WebRequest" />.</param>
		/// <param name="streamingContext">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains the destination of the serialized stream associated with the new <see cref="T:System.Net.WebRequest" />.</param>
		/// <exception cref="T:System.NotImplementedException">An attempt is made to serialize the object, when the interface is not overridden in a descendant class.</exception>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.</summary>
		/// <param name="serializationInfo">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="streamingContext">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that specifies the destination for this serialization.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
		}

		/// <summary>Gets or sets the default cache policy for this request.</summary>
		/// <returns>A <see cref="T:System.Net.Cache.HttpRequestCachePolicy" /> that specifies the cache policy in effect for this request when no other policy is applicable.</returns>
		public static RequestCachePolicy DefaultCachePolicy
		{
			get
			{
				return RequestCacheManager.GetBinding(string.Empty).Policy;
			}
			set
			{
				RequestCacheBinding binding = RequestCacheManager.GetBinding(string.Empty);
				RequestCacheManager.SetBinding(string.Empty, new RequestCacheBinding(binding.Cache, binding.Validator, value));
			}
		}

		/// <summary>Gets or sets the cache policy for this request.</summary>
		/// <returns>A <see cref="T:System.Net.Cache.RequestCachePolicy" /> object that defines a cache policy.</returns>
		public virtual RequestCachePolicy CachePolicy
		{
			get
			{
				return this.m_CachePolicy;
			}
			set
			{
				this.InternalSetCachePolicy(value);
			}
		}

		private void InternalSetCachePolicy(RequestCachePolicy policy)
		{
			if (this.m_CacheBinding != null && this.m_CacheBinding.Cache != null && this.m_CacheBinding.Validator != null && this.CacheProtocol == null && policy != null && policy.Level != RequestCacheLevel.BypassCache)
			{
				this.CacheProtocol = new RequestCacheProtocol(this.m_CacheBinding.Cache, this.m_CacheBinding.Validator.CreateValidator());
			}
			this.m_CachePolicy = policy;
		}

		/// <summary>When overridden in a descendant class, gets or sets the protocol method to use in this request.</summary>
		/// <returns>The protocol method to use in this request.</returns>
		/// <exception cref="T:System.NotImplementedException">If the property is not overridden in a descendant class, any attempt is made to get or set the property.</exception>
		public virtual string Method
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, gets the URI of the Internet resource associated with the request.</summary>
		/// <returns>A <see cref="T:System.Uri" /> representing the resource associated with the request</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual Uri RequestUri
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, gets or sets the name of the connection group for the request.</summary>
		/// <returns>The name of the connection group for the request.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual string ConnectionGroupName
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, gets or sets the collection of header name/value pairs associated with the request.</summary>
		/// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> containing the header name/value pairs associated with this request.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual WebHeaderCollection Headers
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, gets or sets the content length of the request data being sent.</summary>
		/// <returns>The number of bytes of request data being sent.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual long ContentLength
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, gets or sets the content type of the request data being sent.</summary>
		/// <returns>The content type of the request data.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual string ContentType
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, gets or sets the network credentials used for authenticating the request with the Internet resource.</summary>
		/// <returns>An <see cref="T:System.Net.ICredentials" /> containing the authentication credentials associated with the request. The default is <see langword="null" />.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual ICredentials Credentials
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, gets or sets a <see cref="T:System.Boolean" /> value that controls whether <see cref="P:System.Net.CredentialCache.DefaultCredentials" /> are sent with requests.</summary>
		/// <returns>
		///   <see langword="true" /> if the default credentials are used; otherwise <see langword="false" />. The default value is <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">You attempted to set this property after the request was sent.</exception>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the property, when the property is not overridden in a descendant class.</exception>
		public virtual bool UseDefaultCredentials
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, gets or sets the network proxy to use to access this Internet resource.</summary>
		/// <returns>The <see cref="T:System.Net.IWebProxy" /> to use to access the Internet resource.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual IWebProxy Proxy
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, indicates whether to pre-authenticate the request.</summary>
		/// <returns>
		///   <see langword="true" /> to pre-authenticate; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual bool PreAuthenticate
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>Gets or sets the length of time, in milliseconds, before the request times out.</summary>
		/// <returns>The length of time, in milliseconds, until the request times out, or the value <see cref="F:System.Threading.Timeout.Infinite" /> to indicate that the request does not time out. The default value is defined by the descendant class.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class.</exception>
		public virtual int Timeout
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>When overridden in a descendant class, returns a <see cref="T:System.IO.Stream" /> for writing data to the Internet resource.</summary>
		/// <returns>A <see cref="T:System.IO.Stream" /> for writing data to the Internet resource.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method, when the method is not overridden in a descendant class.</exception>
		public virtual Stream GetRequestStream()
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>When overridden in a descendant class, returns a response to an Internet request.</summary>
		/// <returns>A <see cref="T:System.Net.WebResponse" /> containing the response to the Internet request.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method, when the method is not overridden in a descendant class.</exception>
		public virtual WebResponse GetResponse()
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>When overridden in a descendant class, begins an asynchronous request for an Internet resource.</summary>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object containing state information for this asynchronous request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous request.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method, when the method is not overridden in a descendant class.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>When overridden in a descendant class, returns a <see cref="T:System.Net.WebResponse" />.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that references a pending request for a response.</param>
		/// <returns>A <see cref="T:System.Net.WebResponse" /> that contains a response to the Internet request.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method, when the method is not overridden in a descendant class.</exception>
		public virtual WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>When overridden in a descendant class, provides an asynchronous version of the <see cref="M:System.Net.WebRequest.GetRequestStream" /> method.</summary>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object containing state information for this asynchronous request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous request.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method, when the method is not overridden in a descendant class.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>When overridden in a descendant class, returns a <see cref="T:System.IO.Stream" /> for writing data to the Internet resource.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that references a pending request for a stream.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> to write data to.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method, when the method is not overridden in a descendant class.</exception>
		public virtual Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>When overridden in a descendant class, returns a <see cref="T:System.IO.Stream" /> for writing data to the Internet resource as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task<Stream> GetRequestStreamAsync()
		{
			IWebProxy webProxy = null;
			try
			{
				webProxy = this.Proxy;
			}
			catch (NotImplementedException)
			{
			}
			if (ExecutionContext.IsFlowSuppressed() && (this.UseDefaultCredentials || this.Credentials != null || (webProxy != null && webProxy.Credentials != null)))
			{
				WindowsIdentity currentUser = this.SafeCaptureIdenity();
				return Task.Run<Stream>(delegate()
				{
					Task<Stream> result;
					using (WindowsIdentity currentUser = currentUser)
					{
						using (currentUser.Impersonate())
						{
							result = Task<Stream>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetRequestStream), new Func<IAsyncResult, Stream>(this.EndGetRequestStream), null);
						}
					}
					return result;
				});
			}
			return Task.Run<Stream>(() => Task<Stream>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetRequestStream), new Func<IAsyncResult, Stream>(this.EndGetRequestStream), null));
		}

		/// <summary>When overridden in a descendant class, returns a response to an Internet request as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task<WebResponse> GetResponseAsync()
		{
			IWebProxy webProxy = null;
			try
			{
				webProxy = this.Proxy;
			}
			catch (NotImplementedException)
			{
			}
			if (ExecutionContext.IsFlowSuppressed() && (this.UseDefaultCredentials || this.Credentials != null || (webProxy != null && webProxy.Credentials != null)))
			{
				WindowsIdentity currentUser = this.SafeCaptureIdenity();
				return Task.Run<WebResponse>(delegate()
				{
					Task<WebResponse> result;
					using (WindowsIdentity currentUser = currentUser)
					{
						using (currentUser.Impersonate())
						{
							result = Task<WebResponse>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetResponse), new Func<IAsyncResult, WebResponse>(this.EndGetResponse), null);
						}
					}
					return result;
				});
			}
			return Task.Run<WebResponse>(() => Task<WebResponse>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetResponse), new Func<IAsyncResult, WebResponse>(this.EndGetResponse), null));
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.ControlPrincipal)]
		private WindowsIdentity SafeCaptureIdenity()
		{
			return WindowsIdentity.GetCurrent();
		}

		/// <summary>Aborts the request.</summary>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method, when the method is not overridden in a descendant class.</exception>
		public virtual void Abort()
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		internal RequestCacheProtocol CacheProtocol
		{
			get
			{
				return this.m_CacheProtocol;
			}
			set
			{
				this.m_CacheProtocol = value;
			}
		}

		/// <summary>Gets or sets values indicating the level of authentication and impersonation used for this request.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Net.Security.AuthenticationLevel" /> values. The default value is <see cref="F:System.Net.Security.AuthenticationLevel.MutualAuthRequested" />.  
		///  In mutual authentication, both the client and server present credentials to establish their identity. The <see cref="F:System.Net.Security.AuthenticationLevel.MutualAuthRequired" /> and <see cref="F:System.Net.Security.AuthenticationLevel.MutualAuthRequested" /> values are relevant for Kerberos authentication. Kerberos authentication can be supported directly, or can be used if the Negotiate security protocol is used to select the actual security protocol. For more information about authentication protocols, see Internet Authentication.  
		///  To determine whether mutual authentication occurred, check the <see cref="P:System.Net.WebResponse.IsMutuallyAuthenticated" /> property.  
		///  If you specify the <see cref="F:System.Net.Security.AuthenticationLevel.MutualAuthRequired" /> authentication flag value and mutual authentication does not occur, your application will receive an <see cref="T:System.IO.IOException" /> with a <see cref="T:System.Net.ProtocolViolationException" /> inner exception indicating that mutual authentication failed.</returns>
		public AuthenticationLevel AuthenticationLevel
		{
			get
			{
				return this.m_AuthenticationLevel;
			}
			set
			{
				this.m_AuthenticationLevel = value;
			}
		}

		/// <summary>Gets or sets the impersonation level for the current request.</summary>
		/// <returns>A <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> value.</returns>
		public TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				return this.m_ImpersonationLevel;
			}
			set
			{
				this.m_ImpersonationLevel = value;
			}
		}

		internal static IWebProxy InternalDefaultWebProxy
		{
			get
			{
				if (!WebRequest.s_DefaultWebProxyInitialized)
				{
					object internalSyncObject = WebRequest.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (!WebRequest.s_DefaultWebProxyInitialized)
						{
							DefaultProxySectionInternal section = DefaultProxySectionInternal.GetSection();
							if (section != null)
							{
								WebRequest.s_DefaultWebProxy = section.WebProxy;
							}
							WebRequest.s_DefaultWebProxyInitialized = true;
						}
					}
				}
				return WebRequest.s_DefaultWebProxy;
			}
			set
			{
				if (!WebRequest.s_DefaultWebProxyInitialized)
				{
					object internalSyncObject = WebRequest.InternalSyncObject;
					lock (internalSyncObject)
					{
						WebRequest.s_DefaultWebProxy = value;
						WebRequest.s_DefaultWebProxyInitialized = true;
						return;
					}
				}
				WebRequest.s_DefaultWebProxy = value;
			}
		}

		/// <summary>Gets or sets the global HTTP proxy.</summary>
		/// <returns>An <see cref="T:System.Net.IWebProxy" /> used by every call to instances of <see cref="T:System.Net.WebRequest" />.</returns>
		public static IWebProxy DefaultWebProxy
		{
			get
			{
				return WebRequest.InternalDefaultWebProxy;
			}
			set
			{
				WebRequest.InternalDefaultWebProxy = value;
			}
		}

		/// <summary>Returns a proxy configured with the Internet Explorer settings of the currently impersonated user.</summary>
		/// <returns>An <see cref="T:System.Net.IWebProxy" /> used by every call to instances of <see cref="T:System.Net.WebRequest" />.</returns>
		public static IWebProxy GetSystemWebProxy()
		{
			return WebRequest.InternalGetSystemWebProxy();
		}

		internal static IWebProxy InternalGetSystemWebProxy()
		{
			return WebProxy.CreateDefaultProxy();
		}

		internal void SetupCacheProtocol(Uri uri)
		{
			this.m_CacheBinding = RequestCacheManager.GetBinding(uri.Scheme);
			this.InternalSetCachePolicy(this.m_CacheBinding.Policy);
			if (this.m_CachePolicy == null)
			{
				this.InternalSetCachePolicy(WebRequest.DefaultCachePolicy);
			}
		}

		internal const int DefaultTimeout = 100000;

		private static volatile ArrayList s_PrefixList;

		private static object s_InternalSyncObject;

		private static TimerThread.Queue s_DefaultTimerQueue = TimerThread.CreateQueue(100000);

		private AuthenticationLevel m_AuthenticationLevel;

		private TokenImpersonationLevel m_ImpersonationLevel;

		private RequestCachePolicy m_CachePolicy;

		private RequestCacheProtocol m_CacheProtocol;

		private RequestCacheBinding m_CacheBinding;

		private static WebRequest.DesignerWebRequestCreate webRequestCreate = new WebRequest.DesignerWebRequestCreate();

		private static volatile IWebProxy s_DefaultWebProxy;

		private static volatile bool s_DefaultWebProxyInitialized;

		internal class DesignerWebRequestCreate : IWebRequestCreate
		{
			public WebRequest Create(Uri uri)
			{
				return WebRequest.Create(uri);
			}
		}

		internal class WebProxyWrapperOpaque : IAutoWebProxy, IWebProxy
		{
			internal WebProxyWrapperOpaque(WebProxy webProxy)
			{
				this.webProxy = webProxy;
			}

			public Uri GetProxy(Uri destination)
			{
				return this.webProxy.GetProxy(destination);
			}

			public bool IsBypassed(Uri host)
			{
				return this.webProxy.IsBypassed(host);
			}

			public ICredentials Credentials
			{
				get
				{
					return this.webProxy.Credentials;
				}
				set
				{
					this.webProxy.Credentials = value;
				}
			}

			public ProxyChain GetProxies(Uri destination)
			{
				return ((IAutoWebProxy)this.webProxy).GetProxies(destination);
			}

			protected readonly WebProxy webProxy;
		}

		internal class WebProxyWrapper : WebRequest.WebProxyWrapperOpaque
		{
			internal WebProxyWrapper(WebProxy webProxy) : base(webProxy)
			{
			}

			internal WebProxy WebProxy
			{
				get
				{
					return this.webProxy;
				}
			}
		}
	}
}
