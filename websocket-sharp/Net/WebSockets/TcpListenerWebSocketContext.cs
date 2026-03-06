using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;

namespace WebSocketSharp.Net.WebSockets
{
	internal class TcpListenerWebSocketContext : WebSocketContext
	{
		internal TcpListenerWebSocketContext(TcpClient tcpClient, string protocol, bool secure, ServerSslConfiguration sslConfig, Logger log)
		{
			this._tcpClient = tcpClient;
			this._secure = secure;
			this._log = log;
			NetworkStream stream = tcpClient.GetStream();
			if (secure)
			{
				SslStream sslStream = new SslStream(stream, false, sslConfig.ClientCertificateValidationCallback);
				sslStream.AuthenticateAsServer(sslConfig.ServerCertificate, sslConfig.ClientCertificateRequired, sslConfig.EnabledSslProtocols, sslConfig.CheckCertificateRevocation);
				this._stream = sslStream;
			}
			else
			{
				this._stream = stream;
			}
			Socket client = tcpClient.Client;
			this._serverEndPoint = client.LocalEndPoint;
			this._userEndPoint = client.RemoteEndPoint;
			this._request = HttpRequest.Read(this._stream, 90000);
			this._websocket = new WebSocket(this, protocol);
		}

		internal Logger Log
		{
			get
			{
				return this._log;
			}
		}

		internal Stream Stream
		{
			get
			{
				return this._stream;
			}
		}

		public override CookieCollection CookieCollection
		{
			get
			{
				return this._request.Cookies;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return this._request.Headers;
			}
		}

		public override string Host
		{
			get
			{
				return this._request.Headers["Host"];
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this._user != null;
			}
		}

		public override bool IsLocal
		{
			get
			{
				return this.UserEndPoint.Address.IsLocal();
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return this._secure;
			}
		}

		public override bool IsWebSocketRequest
		{
			get
			{
				return this._request.IsWebSocketRequest;
			}
		}

		public override string Origin
		{
			get
			{
				return this._request.Headers["Origin"];
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				bool flag = this._queryString == null;
				if (flag)
				{
					Uri requestUri = this.RequestUri;
					this._queryString = QueryStringCollection.Parse((requestUri != null) ? requestUri.Query : null, Encoding.UTF8);
				}
				return this._queryString;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				bool flag = this._requestUri == null;
				if (flag)
				{
					this._requestUri = HttpUtility.CreateRequestUrl(this._request.RequestUri, this._request.Headers["Host"], this._request.IsWebSocketRequest, this._secure);
				}
				return this._requestUri;
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return this._request.Headers["Sec-WebSocket-Key"];
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				string val = this._request.Headers["Sec-WebSocket-Protocol"];
				bool flag = val == null || val.Length == 0;
				if (flag)
				{
					yield break;
				}
				foreach (string elm in val.Split(new char[]
				{
					','
				}))
				{
					string protocol = elm.Trim();
					bool flag2 = protocol.Length == 0;
					if (!flag2)
					{
						yield return protocol;
						protocol = null;
						elm = null;
					}
				}
				string[] array = null;
				yield break;
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return this._request.Headers["Sec-WebSocket-Version"];
			}
		}

		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return (IPEndPoint)this._serverEndPoint;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return this._user;
			}
		}

		public override IPEndPoint UserEndPoint
		{
			get
			{
				return (IPEndPoint)this._userEndPoint;
			}
		}

		public override WebSocket WebSocket
		{
			get
			{
				return this._websocket;
			}
		}

		private HttpRequest sendAuthenticationChallenge(string challenge)
		{
			HttpResponse httpResponse = HttpResponse.CreateUnauthorizedResponse(challenge);
			byte[] array = httpResponse.ToByteArray();
			this._stream.Write(array, 0, array.Length);
			return HttpRequest.Read(this._stream, 15000);
		}

		internal bool Authenticate(AuthenticationSchemes scheme, string realm, Func<IIdentity, NetworkCredential> credentialsFinder)
		{
			string chal = new AuthenticationChallenge(scheme, realm).ToString();
			int retry = -1;
			Func<bool> auth = null;
			auth = delegate()
			{
				int retry = retry;
				retry++;
				bool flag = retry > 99;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					IPrincipal principal = HttpUtility.CreateUser(this._request.Headers["Authorization"], scheme, realm, this._request.HttpMethod, credentialsFinder);
					bool flag2 = principal != null && principal.Identity.IsAuthenticated;
					if (flag2)
					{
						this._user = principal;
						result = true;
					}
					else
					{
						this._request = this.sendAuthenticationChallenge(chal);
						result = auth();
					}
				}
				return result;
			};
			return auth();
		}

		internal void Close()
		{
			this._stream.Close();
			this._tcpClient.Close();
		}

		internal void Close(HttpStatusCode code)
		{
			HttpResponse httpResponse = HttpResponse.CreateCloseResponse(code);
			byte[] array = httpResponse.ToByteArray();
			this._stream.Write(array, 0, array.Length);
			this._stream.Close();
			this._tcpClient.Close();
		}

		public override string ToString()
		{
			return this._request.ToString();
		}

		private Logger _log;

		private NameValueCollection _queryString;

		private HttpRequest _request;

		private Uri _requestUri;

		private bool _secure;

		private EndPoint _serverEndPoint;

		private Stream _stream;

		private TcpClient _tcpClient;

		private IPrincipal _user;

		private EndPoint _userEndPoint;

		private WebSocket _websocket;
	}
}
