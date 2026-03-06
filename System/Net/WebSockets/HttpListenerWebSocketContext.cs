using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using Unity;

namespace System.Net.WebSockets
{
	/// <summary>Provides access to information received by the <see cref="T:System.Net.HttpListener" /> class when accepting WebSocket connections.</summary>
	public class HttpListenerWebSocketContext : WebSocketContext
	{
		internal HttpListenerWebSocketContext(Uri requestUri, NameValueCollection headers, CookieCollection cookieCollection, IPrincipal user, bool isAuthenticated, bool isLocal, bool isSecureConnection, string origin, IEnumerable<string> secWebSocketProtocols, string secWebSocketVersion, string secWebSocketKey, WebSocket webSocket)
		{
			this._cookieCollection = new CookieCollection();
			this._cookieCollection.Add(cookieCollection);
			this._headers = new NameValueCollection(headers);
			this._user = HttpListenerWebSocketContext.CopyPrincipal(user);
			this._requestUri = requestUri;
			this._isAuthenticated = isAuthenticated;
			this._isLocal = isLocal;
			this._isSecureConnection = isSecureConnection;
			this._origin = origin;
			this._secWebSocketProtocols = secWebSocketProtocols;
			this._secWebSocketVersion = secWebSocketVersion;
			this._secWebSocketKey = secWebSocketKey;
			this._webSocket = webSocket;
		}

		/// <summary>Gets the URI requested by the WebSocket client.</summary>
		/// <returns>The URI requested by the WebSocket client.</returns>
		public override Uri RequestUri
		{
			get
			{
				return this._requestUri;
			}
		}

		/// <summary>Gets the HTTP headers received by the <see cref="T:System.Net.HttpListener" /> object in the WebSocket opening handshake.</summary>
		/// <returns>The HTTP headers received by the <see cref="T:System.Net.HttpListener" /> object.</returns>
		public override NameValueCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		/// <summary>Gets the value of the Origin HTTP header included in the WebSocket opening handshake.</summary>
		/// <returns>The value of the Origin HTTP header.</returns>
		public override string Origin
		{
			get
			{
				return this._origin;
			}
		}

		/// <summary>Gets the list of the Secure WebSocket protocols included in the WebSocket opening handshake.</summary>
		/// <returns>The list of the Secure WebSocket protocols.</returns>
		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				return this._secWebSocketProtocols;
			}
		}

		/// <summary>Gets the list of sub-protocols requested by the WebSocket client.</summary>
		/// <returns>The list of sub-protocols requested by the WebSocket client.</returns>
		public override string SecWebSocketVersion
		{
			get
			{
				return this._secWebSocketVersion;
			}
		}

		/// <summary>Gets the value of the SecWebSocketKey HTTP header included in the WebSocket opening handshake.</summary>
		/// <returns>The value of the SecWebSocketKey HTTP header.</returns>
		public override string SecWebSocketKey
		{
			get
			{
				return this._secWebSocketKey;
			}
		}

		/// <summary>Gets the cookies received by the <see cref="T:System.Net.HttpListener" /> object in the WebSocket opening handshake.</summary>
		/// <returns>The cookies received by the <see cref="T:System.Net.HttpListener" /> object.</returns>
		public override CookieCollection CookieCollection
		{
			get
			{
				return this._cookieCollection;
			}
		}

		/// <summary>Gets an object used to obtain identity, authentication information, and security roles for the WebSocket client.</summary>
		/// <returns>The identity, authentication information, and security roles for the WebSocket client.</returns>
		public override IPrincipal User
		{
			get
			{
				return this._user;
			}
		}

		/// <summary>Gets a value that indicates if the WebSocket client is authenticated.</summary>
		/// <returns>
		///   <see langword="true" /> if the WebSocket client is authenticated; otherwise, <see langword="false" />.</returns>
		public override bool IsAuthenticated
		{
			get
			{
				return this._isAuthenticated;
			}
		}

		/// <summary>Gets a value that indicates if the WebSocket client connected from the local machine.</summary>
		/// <returns>
		///   <see langword="true" /> if the WebSocket client connected from the local machine; otherwise, <see langword="false" />.</returns>
		public override bool IsLocal
		{
			get
			{
				return this._isLocal;
			}
		}

		/// <summary>Gets a value that indicates if the WebSocket connection is secured using Secure Sockets Layer (SSL).</summary>
		/// <returns>
		///   <see langword="true" /> if the WebSocket connection is secured using SSL; otherwise, <see langword="false" />.</returns>
		public override bool IsSecureConnection
		{
			get
			{
				return this._isSecureConnection;
			}
		}

		/// <summary>Gets the <see cref="T:System.Net.WebSockets.WebSocket" /> instance used to send and receive data over the <see cref="T:System.Net.WebSockets.WebSocket" /> connection.</summary>
		/// <returns>The <see cref="T:System.Net.WebSockets.WebSocket" /> instance used to send and receive data over the <see cref="T:System.Net.WebSockets.WebSocket" /> connection.</returns>
		public override WebSocket WebSocket
		{
			get
			{
				return this._webSocket;
			}
		}

		private static IPrincipal CopyPrincipal(IPrincipal user)
		{
			if (user != null)
			{
				if (user is WindowsPrincipal)
				{
					throw new PlatformNotSupportedException();
				}
				HttpListenerBasicIdentity httpListenerBasicIdentity = user.Identity as HttpListenerBasicIdentity;
				if (httpListenerBasicIdentity != null)
				{
					return new GenericPrincipal(new HttpListenerBasicIdentity(httpListenerBasicIdentity.Name, httpListenerBasicIdentity.Password), null);
				}
			}
			return null;
		}

		internal HttpListenerWebSocketContext()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly Uri _requestUri;

		private readonly NameValueCollection _headers;

		private readonly CookieCollection _cookieCollection;

		private readonly IPrincipal _user;

		private readonly bool _isAuthenticated;

		private readonly bool _isLocal;

		private readonly bool _isSecureConnection;

		private readonly string _origin;

		private readonly IEnumerable<string> _secWebSocketProtocols;

		private readonly string _secWebSocketVersion;

		private readonly string _secWebSocketKey;

		private readonly WebSocket _webSocket;
	}
}
