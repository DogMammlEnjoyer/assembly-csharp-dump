using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WebSocketSharp.Net
{
	internal sealed class HttpConnection
	{
		internal HttpConnection(Socket socket, EndPointListener listener)
		{
			this._socket = socket;
			this._listener = listener;
			NetworkStream networkStream = new NetworkStream(socket, false);
			bool isSecure = listener.IsSecure;
			if (isSecure)
			{
				ServerSslConfiguration sslConfiguration = listener.SslConfiguration;
				SslStream sslStream = new SslStream(networkStream, false, sslConfiguration.ClientCertificateValidationCallback);
				sslStream.AuthenticateAsServer(sslConfiguration.ServerCertificate, sslConfiguration.ClientCertificateRequired, sslConfiguration.EnabledSslProtocols, sslConfiguration.CheckCertificateRevocation);
				this._secure = true;
				this._stream = sslStream;
			}
			else
			{
				this._stream = networkStream;
			}
			this._buffer = new byte[HttpConnection._bufferLength];
			this._localEndPoint = socket.LocalEndPoint;
			this._remoteEndPoint = socket.RemoteEndPoint;
			this._sync = new object();
			this._timeoutCanceled = new Dictionary<int, bool>();
			this._timer = new Timer(new TimerCallback(HttpConnection.onTimeout), this, -1, -1);
			this.init(new MemoryStream(), 90000);
		}

		public bool IsClosed
		{
			get
			{
				return this._socket == null;
			}
		}

		public bool IsLocal
		{
			get
			{
				return ((IPEndPoint)this._remoteEndPoint).Address.IsLocal();
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return (IPEndPoint)this._localEndPoint;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return (IPEndPoint)this._remoteEndPoint;
			}
		}

		public int Reuses
		{
			get
			{
				return this._reuses;
			}
		}

		public Stream Stream
		{
			get
			{
				return this._stream;
			}
		}

		private void close()
		{
			object sync = this._sync;
			lock (sync)
			{
				bool flag = this._socket == null;
				if (flag)
				{
					return;
				}
				this.disposeTimer();
				this.disposeRequestBuffer();
				this.disposeStream();
				this.closeSocket();
			}
			this._context.Unregister();
			this._listener.RemoveConnection(this);
		}

		private void closeSocket()
		{
			try
			{
				this._socket.Shutdown(SocketShutdown.Both);
			}
			catch
			{
			}
			this._socket.Close();
			this._socket = null;
		}

		private static MemoryStream createRequestBuffer(RequestStream inputStream)
		{
			MemoryStream memoryStream = new MemoryStream();
			bool flag = inputStream is ChunkedRequestStream;
			MemoryStream result;
			if (flag)
			{
				ChunkedRequestStream chunkedRequestStream = (ChunkedRequestStream)inputStream;
				bool hasRemainingBuffer = chunkedRequestStream.HasRemainingBuffer;
				if (hasRemainingBuffer)
				{
					byte[] remainingBuffer = chunkedRequestStream.RemainingBuffer;
					memoryStream.Write(remainingBuffer, 0, remainingBuffer.Length);
				}
				result = memoryStream;
			}
			else
			{
				int count = inputStream.Count;
				bool flag2 = count > 0;
				if (flag2)
				{
					memoryStream.Write(inputStream.InitialBuffer, inputStream.Offset, count);
				}
				result = memoryStream;
			}
			return result;
		}

		private void disposeRequestBuffer()
		{
			bool flag = this._requestBuffer == null;
			if (!flag)
			{
				this._requestBuffer.Dispose();
				this._requestBuffer = null;
			}
		}

		private void disposeStream()
		{
			bool flag = this._stream == null;
			if (!flag)
			{
				this._stream.Dispose();
				this._stream = null;
			}
		}

		private void disposeTimer()
		{
			bool flag = this._timer == null;
			if (!flag)
			{
				try
				{
					this._timer.Change(-1, -1);
				}
				catch
				{
				}
				this._timer.Dispose();
				this._timer = null;
			}
		}

		private void init(MemoryStream requestBuffer, int timeout)
		{
			this._requestBuffer = requestBuffer;
			this._timeout = timeout;
			this._context = new HttpListenerContext(this);
			this._currentLine = new StringBuilder(64);
			this._inputState = InputState.RequestLine;
			this._inputStream = null;
			this._lineState = LineState.None;
			this._outputStream = null;
			this._position = 0;
		}

		private static void onRead(IAsyncResult asyncResult)
		{
			HttpConnection httpConnection = (HttpConnection)asyncResult.AsyncState;
			int attempts = httpConnection._attempts;
			bool flag = httpConnection._socket == null;
			if (!flag)
			{
				object sync = httpConnection._sync;
				lock (sync)
				{
					bool flag2 = httpConnection._socket == null;
					if (!flag2)
					{
						httpConnection._timer.Change(-1, -1);
						httpConnection._timeoutCanceled[attempts] = true;
						int num = 0;
						try
						{
							num = httpConnection._stream.EndRead(asyncResult);
						}
						catch (Exception)
						{
							httpConnection.close();
							return;
						}
						bool flag3 = num <= 0;
						if (flag3)
						{
							httpConnection.close();
						}
						else
						{
							httpConnection._requestBuffer.Write(httpConnection._buffer, 0, num);
							bool flag4 = httpConnection.processRequestBuffer();
							if (!flag4)
							{
								httpConnection.BeginReadRequest();
							}
						}
					}
				}
			}
		}

		private static void onTimeout(object state)
		{
			HttpConnection httpConnection = (HttpConnection)state;
			int attempts = httpConnection._attempts;
			bool flag = httpConnection._socket == null;
			if (!flag)
			{
				object sync = httpConnection._sync;
				lock (sync)
				{
					bool flag2 = httpConnection._socket == null;
					if (!flag2)
					{
						bool flag3 = httpConnection._timeoutCanceled[attempts];
						if (!flag3)
						{
							httpConnection._context.SendError(408);
						}
					}
				}
			}
		}

		private bool processInput(byte[] data, int length)
		{
			try
			{
				for (;;)
				{
					int num;
					string text = this.readLineFrom(data, this._position, length, out num);
					this._position += num;
					bool flag = text == null;
					if (flag)
					{
						break;
					}
					bool flag2 = text.Length == 0;
					if (flag2)
					{
						bool flag3 = this._inputState == InputState.RequestLine;
						if (!flag3)
						{
							goto IL_56;
						}
					}
					else
					{
						bool flag4 = this._inputState == InputState.RequestLine;
						if (flag4)
						{
							this._context.Request.SetRequestLine(text);
							this._inputState = InputState.Headers;
						}
						else
						{
							this._context.Request.AddHeader(text);
						}
						bool hasErrorMessage = this._context.HasErrorMessage;
						if (hasErrorMessage)
						{
							goto Block_8;
						}
					}
				}
				goto IL_FF;
				IL_56:
				bool flag5 = this._position > HttpConnection._maxInputLength;
				if (flag5)
				{
					this._context.ErrorMessage = "Headers too long";
				}
				return true;
				Block_8:
				return true;
			}
			catch (Exception ex)
			{
				this._context.ErrorMessage = ex.Message;
				return true;
			}
			IL_FF:
			bool flag6 = this._position >= HttpConnection._maxInputLength;
			bool result;
			if (flag6)
			{
				this._context.ErrorMessage = "Headers too long";
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool processRequestBuffer()
		{
			byte[] buffer = this._requestBuffer.GetBuffer();
			int length = (int)this._requestBuffer.Length;
			bool flag = !this.processInput(buffer, length);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this._context.HasErrorMessage;
				if (flag2)
				{
					this._context.Request.FinishInitialization();
				}
				bool hasErrorMessage = this._context.HasErrorMessage;
				if (hasErrorMessage)
				{
					this._context.SendError();
					result = true;
				}
				else
				{
					Uri url = this._context.Request.Url;
					HttpListener httpListener;
					bool flag3 = !this._listener.TrySearchHttpListener(url, out httpListener);
					if (flag3)
					{
						this._context.SendError(404);
						result = true;
					}
					else
					{
						httpListener.RegisterContext(this._context);
						result = true;
					}
				}
			}
			return result;
		}

		private string readLineFrom(byte[] buffer, int offset, int length, out int nread)
		{
			nread = 0;
			for (int i = offset; i < length; i++)
			{
				nread++;
				byte b = buffer[i];
				bool flag = b == 13;
				if (flag)
				{
					this._lineState = LineState.Cr;
				}
				else
				{
					bool flag2 = b == 10;
					if (flag2)
					{
						this._lineState = LineState.Lf;
						break;
					}
					this._currentLine.Append((char)b);
				}
			}
			bool flag3 = this._lineState != LineState.Lf;
			string result;
			if (flag3)
			{
				result = null;
			}
			else
			{
				string text = this._currentLine.ToString();
				this._currentLine.Length = 0;
				this._lineState = LineState.None;
				result = text;
			}
			return result;
		}

		private MemoryStream takeOverRequestBuffer()
		{
			bool flag = this._inputStream != null;
			MemoryStream result;
			if (flag)
			{
				result = HttpConnection.createRequestBuffer(this._inputStream);
			}
			else
			{
				MemoryStream memoryStream = new MemoryStream();
				byte[] buffer = this._requestBuffer.GetBuffer();
				int num = (int)this._requestBuffer.Length;
				int num2 = num - this._position;
				bool flag2 = num2 > 0;
				if (flag2)
				{
					memoryStream.Write(buffer, this._position, num2);
				}
				this.disposeRequestBuffer();
				result = memoryStream;
			}
			return result;
		}

		internal void BeginReadRequest()
		{
			this._attempts++;
			this._timeoutCanceled.Add(this._attempts, false);
			this._timer.Change(this._timeout, -1);
			try
			{
				this._stream.BeginRead(this._buffer, 0, HttpConnection._bufferLength, new AsyncCallback(HttpConnection.onRead), this);
			}
			catch (Exception)
			{
				this.close();
			}
		}

		internal void Close(bool force)
		{
			bool flag = this._socket == null;
			if (!flag)
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag2 = this._socket == null;
					if (!flag2)
					{
						if (force)
						{
							bool flag3 = this._outputStream != null;
							if (flag3)
							{
								this._outputStream.Close(true);
							}
							this.close();
						}
						else
						{
							this.GetResponseStream().Close(false);
							bool closeConnection = this._context.Response.CloseConnection;
							if (closeConnection)
							{
								this.close();
							}
							else
							{
								bool flag4 = !this._context.Request.FlushInput();
								if (flag4)
								{
									this.close();
								}
								else
								{
									this._context.Unregister();
									this._reuses++;
									MemoryStream memoryStream = this.takeOverRequestBuffer();
									long length = memoryStream.Length;
									this.init(memoryStream, 15000);
									bool flag5 = length > 0L;
									if (flag5)
									{
										bool flag6 = this.processRequestBuffer();
										if (flag6)
										{
											return;
										}
									}
									this.BeginReadRequest();
								}
							}
						}
					}
				}
			}
		}

		public void Close()
		{
			this.Close(false);
		}

		public RequestStream GetRequestStream(long contentLength, bool chunked)
		{
			object sync = this._sync;
			RequestStream result;
			lock (sync)
			{
				bool flag = this._socket == null;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this._inputStream != null;
					if (flag2)
					{
						result = this._inputStream;
					}
					else
					{
						byte[] buffer = this._requestBuffer.GetBuffer();
						int num = (int)this._requestBuffer.Length;
						int count = num - this._position;
						this._inputStream = (chunked ? new ChunkedRequestStream(this._stream, buffer, this._position, count, this._context) : new RequestStream(this._stream, buffer, this._position, count, contentLength));
						this.disposeRequestBuffer();
						result = this._inputStream;
					}
				}
			}
			return result;
		}

		public ResponseStream GetResponseStream()
		{
			object sync = this._sync;
			ResponseStream result;
			lock (sync)
			{
				bool flag = this._socket == null;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this._outputStream != null;
					if (flag2)
					{
						result = this._outputStream;
					}
					else
					{
						HttpListener listener = this._context.Listener;
						bool ignoreWriteExceptions = listener == null || listener.IgnoreWriteExceptions;
						this._outputStream = new ResponseStream(this._stream, this._context.Response, ignoreWriteExceptions);
						result = this._outputStream;
					}
				}
			}
			return result;
		}

		private int _attempts;

		private byte[] _buffer;

		private static readonly int _bufferLength = 8192;

		private HttpListenerContext _context;

		private StringBuilder _currentLine;

		private InputState _inputState;

		private RequestStream _inputStream;

		private LineState _lineState;

		private EndPointListener _listener;

		private EndPoint _localEndPoint;

		private static readonly int _maxInputLength = 32768;

		private ResponseStream _outputStream;

		private int _position;

		private EndPoint _remoteEndPoint;

		private MemoryStream _requestBuffer;

		private int _reuses;

		private bool _secure;

		private Socket _socket;

		private Stream _stream;

		private object _sync;

		private int _timeout;

		private Dictionary<int, bool> _timeoutCanceled;

		private Timer _timer;
	}
}
