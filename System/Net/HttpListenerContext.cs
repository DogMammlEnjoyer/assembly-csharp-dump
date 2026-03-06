using System;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace System.Net
{
	/// <summary>Provides access to the request and response objects used by the <see cref="T:System.Net.HttpListener" /> class. This class cannot be inherited.</summary>
	public sealed class HttpListenerContext
	{
		internal HttpListenerContext(HttpConnection cnc)
		{
			this.err_status = 400;
			base..ctor();
			this.cnc = cnc;
			this.request = new HttpListenerRequest(this);
			this.response = new HttpListenerResponse(this);
		}

		internal int ErrorStatus
		{
			get
			{
				return this.err_status;
			}
			set
			{
				this.err_status = value;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return this.error;
			}
			set
			{
				this.error = value;
			}
		}

		internal bool HaveError
		{
			get
			{
				return this.error != null;
			}
		}

		internal HttpConnection Connection
		{
			get
			{
				return this.cnc;
			}
		}

		/// <summary>Gets the <see cref="T:System.Net.HttpListenerRequest" /> that represents a client's request for a resource.</summary>
		/// <returns>An <see cref="T:System.Net.HttpListenerRequest" /> object that represents the client request.</returns>
		public HttpListenerRequest Request
		{
			get
			{
				return this.request;
			}
		}

		/// <summary>Gets the <see cref="T:System.Net.HttpListenerResponse" /> object that will be sent to the client in response to the client's request.</summary>
		/// <returns>An <see cref="T:System.Net.HttpListenerResponse" /> object used to send a response back to the client.</returns>
		public HttpListenerResponse Response
		{
			get
			{
				return this.response;
			}
		}

		/// <summary>Gets an object used to obtain identity, authentication information, and security roles for the client whose request is represented by this <see cref="T:System.Net.HttpListenerContext" /> object.</summary>
		/// <returns>An <see cref="T:System.Security.Principal.IPrincipal" /> object that describes the client, or <see langword="null" /> if the <see cref="T:System.Net.HttpListener" /> that supplied this <see cref="T:System.Net.HttpListenerContext" /> does not require authentication.</returns>
		public IPrincipal User
		{
			get
			{
				return this.user;
			}
		}

		internal void ParseAuthentication(AuthenticationSchemes expectedSchemes)
		{
			if (expectedSchemes == AuthenticationSchemes.Anonymous)
			{
				return;
			}
			string text = this.request.Headers["Authorization"];
			if (text == null || text.Length < 2)
			{
				return;
			}
			string[] array = text.Split(new char[]
			{
				' '
			}, 2);
			if (string.Compare(array[0], "basic", true) == 0)
			{
				this.user = this.ParseBasicAuthentication(array[1]);
			}
		}

		internal IPrincipal ParseBasicAuthentication(string authData)
		{
			IPrincipal result;
			try
			{
				string text = Encoding.Default.GetString(Convert.FromBase64String(authData));
				int num = text.IndexOf(':');
				string password = text.Substring(num + 1);
				text = text.Substring(0, num);
				num = text.IndexOf('\\');
				string username;
				if (num > 0)
				{
					username = text.Substring(num);
				}
				else
				{
					username = text;
				}
				result = new GenericPrincipal(new HttpListenerBasicIdentity(username, password), new string[0]);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		/// <summary>Accept a WebSocket connection as an asynchronous operation.</summary>
		/// <param name="subProtocol">The supported WebSocket sub-protocol.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns an <see cref="T:System.Net.WebSockets.HttpListenerWebSocketContext" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="subProtocol" /> is an empty string  
		/// -or-  
		/// <paramref name="subProtocol" /> contains illegal characters.</exception>
		/// <exception cref="T:System.Net.WebSockets.WebSocketException">An error occurred when sending the response to complete the WebSocket handshake.</exception>
		[MonoTODO]
		public Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(string subProtocol)
		{
			throw new NotImplementedException();
		}

		/// <summary>Accept a WebSocket connection specifying the supported WebSocket sub-protocol  and WebSocket keep-alive interval as an asynchronous operation.</summary>
		/// <param name="subProtocol">The supported WebSocket sub-protocol.</param>
		/// <param name="keepAliveInterval">The WebSocket protocol keep-alive interval in milliseconds.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns an <see cref="T:System.Net.WebSockets.HttpListenerWebSocketContext" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="subProtocol" /> is an empty string  
		/// -or-  
		/// <paramref name="subProtocol" /> contains illegal characters.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="keepAliveInterval" /> is too small.</exception>
		/// <exception cref="T:System.Net.WebSockets.WebSocketException">An error occurred when sending the response to complete the WebSocket handshake.</exception>
		[MonoTODO]
		public Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(string subProtocol, TimeSpan keepAliveInterval)
		{
			throw new NotImplementedException();
		}

		/// <summary>Accept a WebSocket connection specifying the supported WebSocket sub-protocol, receive buffer size, and WebSocket keep-alive interval as an asynchronous operation.</summary>
		/// <param name="subProtocol">The supported WebSocket sub-protocol.</param>
		/// <param name="receiveBufferSize">The receive buffer size in bytes.</param>
		/// <param name="keepAliveInterval">The WebSocket protocol keep-alive interval in milliseconds.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns an <see cref="T:System.Net.WebSockets.HttpListenerWebSocketContext" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="subProtocol" /> is an empty string  
		/// -or-  
		/// <paramref name="subProtocol" /> contains illegal characters.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="keepAliveInterval" /> is too small.  
		/// -or-  
		/// <paramref name="receiveBufferSize" /> is less than 16 bytes  
		/// -or-  
		/// <paramref name="receiveBufferSize" /> is greater than 64K bytes.</exception>
		/// <exception cref="T:System.Net.WebSockets.WebSocketException">An error occurred when sending the response to complete the WebSocket handshake.</exception>
		[MonoTODO]
		public Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(string subProtocol, int receiveBufferSize, TimeSpan keepAliveInterval)
		{
			throw new NotImplementedException();
		}

		/// <summary>Accept a WebSocket connection specifying the supported WebSocket sub-protocol, receive buffer size, WebSocket keep-alive interval, and the internal buffer as an asynchronous operation.</summary>
		/// <param name="subProtocol">The supported WebSocket sub-protocol.</param>
		/// <param name="receiveBufferSize">The receive buffer size in bytes.</param>
		/// <param name="keepAliveInterval">The WebSocket protocol keep-alive interval in milliseconds.</param>
		/// <param name="internalBuffer">An internal buffer to use for this operation.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns an <see cref="T:System.Net.WebSockets.HttpListenerWebSocketContext" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="subProtocol" /> is an empty string  
		/// -or-  
		/// <paramref name="subProtocol" /> contains illegal characters.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="keepAliveInterval" /> is too small.  
		/// -or-  
		/// <paramref name="receiveBufferSize" /> is less than 16 bytes  
		/// -or-  
		/// <paramref name="receiveBufferSize" /> is greater than 64K bytes.</exception>
		/// <exception cref="T:System.Net.WebSockets.WebSocketException">An error occurred when sending the response to complete the WebSocket handshake.</exception>
		[MonoTODO]
		public Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(string subProtocol, int receiveBufferSize, TimeSpan keepAliveInterval, ArraySegment<byte> internalBuffer)
		{
			throw new NotImplementedException();
		}

		internal HttpListenerContext()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private HttpListenerRequest request;

		private HttpListenerResponse response;

		private IPrincipal user;

		private HttpConnection cnc;

		private string error;

		private int err_status;

		internal HttpListener Listener;
	}
}
