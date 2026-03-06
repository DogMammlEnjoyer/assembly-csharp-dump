using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.Net.WebSockets
{
	/// <summary>Options to use with a  <see cref="T:System.Net.WebSockets.ClientWebSocket" /> object.</summary>
	public sealed class ClientWebSocketOptions
	{
		internal ClientWebSocketOptions()
		{
			this._requestedSubProtocols = new List<string>();
			this._requestHeaders = new WebHeaderCollection();
		}

		/// <summary>Creates a HTTP request header and its value.</summary>
		/// <param name="headerName">The name of the HTTP header.</param>
		/// <param name="headerValue">The value of the HTTP header.</param>
		public void SetRequestHeader(string headerName, string headerValue)
		{
			this.ThrowIfReadOnly();
			this._requestHeaders.Set(headerName, headerValue);
		}

		internal WebHeaderCollection RequestHeaders
		{
			get
			{
				return this._requestHeaders;
			}
		}

		internal List<string> RequestedSubProtocols
		{
			get
			{
				return this._requestedSubProtocols;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that indicates if default credentials should be used during WebSocket handshake.</summary>
		/// <returns>
		///   <see langword="true" /> if default credentials should be used during WebSocket handshake; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool UseDefaultCredentials
		{
			get
			{
				return this._useDefaultCredentials;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._useDefaultCredentials = value;
			}
		}

		/// <summary>Gets or sets the credential information for the client.</summary>
		/// <returns>The credential information for the client.</returns>
		public ICredentials Credentials
		{
			get
			{
				return this._credentials;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._credentials = value;
			}
		}

		/// <summary>Gets or sets the proxy for WebSocket requests.</summary>
		/// <returns>The proxy for WebSocket requests.</returns>
		public IWebProxy Proxy
		{
			get
			{
				return this._proxy;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._proxy = value;
			}
		}

		/// <summary>Gets or sets a collection of client side certificates.</summary>
		/// <returns>A collection of client side certificates.</returns>
		public X509CertificateCollection ClientCertificates
		{
			get
			{
				if (this._clientCertificates == null)
				{
					this._clientCertificates = new X509CertificateCollection();
				}
				return this._clientCertificates;
			}
			set
			{
				this.ThrowIfReadOnly();
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._clientCertificates = value;
			}
		}

		public RemoteCertificateValidationCallback RemoteCertificateValidationCallback
		{
			get
			{
				return this._remoteCertificateValidationCallback;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._remoteCertificateValidationCallback = value;
			}
		}

		/// <summary>Gets or sets the cookies associated with the request.</summary>
		/// <returns>The cookies associated with the request.</returns>
		public CookieContainer Cookies
		{
			get
			{
				return this._cookies;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._cookies = value;
			}
		}

		/// <summary>Adds a sub-protocol to be negotiated during the WebSocket connection handshake.</summary>
		/// <param name="subProtocol">The WebSocket sub-protocol to add.</param>
		public void AddSubProtocol(string subProtocol)
		{
			this.ThrowIfReadOnly();
			WebSocketValidate.ValidateSubprotocol(subProtocol);
			using (List<string>.Enumerator enumerator = this._requestedSubProtocols.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (string.Equals(enumerator.Current, subProtocol, StringComparison.OrdinalIgnoreCase))
					{
						throw new ArgumentException(SR.Format("Duplicate protocols are not allowed: '{0}'.", subProtocol), "subProtocol");
					}
				}
			}
			this._requestedSubProtocols.Add(subProtocol);
		}

		/// <summary>Gets or sets the WebSocket protocol keep-alive interval.</summary>
		/// <returns>The WebSocket protocol keep-alive interval.</returns>
		public TimeSpan KeepAliveInterval
		{
			get
			{
				return this._keepAliveInterval;
			}
			set
			{
				this.ThrowIfReadOnly();
				if (value != Timeout.InfiniteTimeSpan && value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", value, SR.Format("The argument must be a value greater than {0}.", Timeout.InfiniteTimeSpan.ToString()));
				}
				this._keepAliveInterval = value;
			}
		}

		internal int ReceiveBufferSize
		{
			get
			{
				return this._receiveBufferSize;
			}
		}

		internal int SendBufferSize
		{
			get
			{
				return this._sendBufferSize;
			}
		}

		internal ArraySegment<byte>? Buffer
		{
			get
			{
				return this._buffer;
			}
		}

		/// <summary>Sets the client buffer parameters.</summary>
		/// <param name="receiveBufferSize">The size, in bytes, of the client receive buffer.</param>
		/// <param name="sendBufferSize">The size, in bytes, of the client send buffer.</param>
		public void SetBuffer(int receiveBufferSize, int sendBufferSize)
		{
			this.ThrowIfReadOnly();
			if (receiveBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("receiveBufferSize", receiveBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			if (sendBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("sendBufferSize", sendBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			this._receiveBufferSize = receiveBufferSize;
			this._sendBufferSize = sendBufferSize;
			this._buffer = null;
		}

		/// <summary>Sets client buffer parameters.</summary>
		/// <param name="receiveBufferSize">The size, in bytes, of the client receive buffer.</param>
		/// <param name="sendBufferSize">The size, in bytes, of the client send buffer.</param>
		/// <param name="buffer">The receive buffer to use.</param>
		public void SetBuffer(int receiveBufferSize, int sendBufferSize, ArraySegment<byte> buffer)
		{
			this.ThrowIfReadOnly();
			if (receiveBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("receiveBufferSize", receiveBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			if (sendBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("sendBufferSize", sendBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			WebSocketValidate.ValidateArraySegment(buffer, "buffer");
			if (buffer.Count == 0)
			{
				throw new ArgumentOutOfRangeException("buffer");
			}
			this._receiveBufferSize = receiveBufferSize;
			this._sendBufferSize = sendBufferSize;
			this._buffer = new ArraySegment<byte>?(buffer);
		}

		internal void SetToReadOnly()
		{
			this._isReadOnly = true;
		}

		private void ThrowIfReadOnly()
		{
			if (this._isReadOnly)
			{
				throw new InvalidOperationException("The WebSocket has already been started.");
			}
		}

		private bool _isReadOnly;

		private readonly List<string> _requestedSubProtocols;

		private readonly WebHeaderCollection _requestHeaders;

		private TimeSpan _keepAliveInterval = WebSocket.DefaultKeepAliveInterval;

		private bool _useDefaultCredentials;

		private ICredentials _credentials;

		private IWebProxy _proxy;

		private X509CertificateCollection _clientCertificates;

		private CookieContainer _cookies;

		private int _receiveBufferSize = 4096;

		private int _sendBufferSize = 4096;

		private ArraySegment<byte>? _buffer;

		private RemoteCertificateValidationCallback _remoteCertificateValidationCallback;
	}
}
