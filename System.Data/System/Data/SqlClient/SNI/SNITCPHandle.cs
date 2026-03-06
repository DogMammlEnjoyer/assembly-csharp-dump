using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
	internal class SNITCPHandle : SNIHandle
	{
		public override void Dispose()
		{
			lock (this)
			{
				if (this._sslOverTdsStream != null)
				{
					this._sslOverTdsStream.Dispose();
					this._sslOverTdsStream = null;
				}
				if (this._sslStream != null)
				{
					this._sslStream.Dispose();
					this._sslStream = null;
				}
				if (this._tcpStream != null)
				{
					this._tcpStream.Dispose();
					this._tcpStream = null;
				}
				this._stream = null;
			}
		}

		public override Guid ConnectionId
		{
			get
			{
				return this._connectionId;
			}
		}

		public override uint Status
		{
			get
			{
				return this._status;
			}
		}

		public SNITCPHandle(string serverName, int port, long timerExpire, object callbackObject, bool parallel)
		{
			this._callbackObject = callbackObject;
			this._targetServer = serverName;
			try
			{
				TimeSpan timeSpan = default(TimeSpan);
				bool flag = long.MaxValue == timerExpire;
				if (!flag)
				{
					timeSpan = DateTime.FromFileTime(timerExpire) - DateTime.Now;
					timeSpan = ((timeSpan.Ticks < 0L) ? TimeSpan.FromTicks(0L) : timeSpan);
				}
				if (parallel)
				{
					Task<IPAddress[]> hostAddressesAsync = Dns.GetHostAddressesAsync(serverName);
					hostAddressesAsync.Wait(timeSpan);
					IPAddress[] result = hostAddressesAsync.Result;
					if (result.Length > 64)
					{
						this.ReportTcpSNIError(0U, 47U, string.Empty);
						return;
					}
					Task<Socket> task = SNITCPHandle.ParallelConnectAsync(result, port);
					if (!(flag ? task.Wait(-1) : task.Wait(timeSpan)))
					{
						this.ReportTcpSNIError(0U, 40U, string.Empty);
						return;
					}
					this._socket = task.Result;
				}
				else
				{
					this._socket = SNITCPHandle.Connect(serverName, port, flag ? TimeSpan.FromMilliseconds(2147483647.0) : timeSpan);
				}
				if (this._socket == null || !this._socket.Connected)
				{
					if (this._socket != null)
					{
						this._socket.Dispose();
						this._socket = null;
					}
					this.ReportTcpSNIError(0U, 40U, string.Empty);
					return;
				}
				this._socket.NoDelay = true;
				this._tcpStream = new NetworkStream(this._socket, true);
				this._sslOverTdsStream = new SslOverTdsStream(this._tcpStream);
				this._sslStream = new SslStream(this._sslOverTdsStream, true, new RemoteCertificateValidationCallback(this.ValidateServerCertificate), null);
			}
			catch (SocketException sniException)
			{
				this.ReportTcpSNIError(sniException);
				return;
			}
			catch (Exception sniException2)
			{
				this.ReportTcpSNIError(sniException2);
				return;
			}
			this._stream = this._tcpStream;
			this._status = 0U;
		}

		private static Socket Connect(string serverName, int port, TimeSpan timeout)
		{
			SNITCPHandle.<>c__DisplayClass20_0 CS$<>8__locals1 = new SNITCPHandle.<>c__DisplayClass20_0();
			IPAddress[] array = Dns.GetHostAddresses(serverName);
			IPAddress ipaddress = null;
			IPAddress ipaddress2 = null;
			foreach (IPAddress ipaddress3 in array)
			{
				if (ipaddress3.AddressFamily == AddressFamily.InterNetwork)
				{
					ipaddress = ipaddress3;
				}
				else if (ipaddress3.AddressFamily == AddressFamily.InterNetworkV6)
				{
					ipaddress2 = ipaddress3;
				}
			}
			array = new IPAddress[]
			{
				ipaddress,
				ipaddress2
			};
			CS$<>8__locals1.sockets = new Socket[2];
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			cancellationTokenSource.CancelAfter(timeout);
			cancellationTokenSource.Token.Register(new Action(CS$<>8__locals1.<Connect>g__Cancel|0));
			Socket result = null;
			for (int j = 0; j < CS$<>8__locals1.sockets.Length; j++)
			{
				try
				{
					if (array[j] != null)
					{
						CS$<>8__locals1.sockets[j] = new Socket(array[j].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						CS$<>8__locals1.sockets[j].Connect(array[j], port);
						if (CS$<>8__locals1.sockets[j] != null)
						{
							if (CS$<>8__locals1.sockets[j].Connected)
							{
								result = CS$<>8__locals1.sockets[j];
								break;
							}
							CS$<>8__locals1.sockets[j].Dispose();
							CS$<>8__locals1.sockets[j] = null;
						}
					}
				}
				catch
				{
				}
			}
			return result;
		}

		private static Task<Socket> ParallelConnectAsync(IPAddress[] serverAddresses, int port)
		{
			if (serverAddresses == null)
			{
				throw new ArgumentNullException("serverAddresses");
			}
			if (serverAddresses.Length == 0)
			{
				throw new ArgumentOutOfRangeException("serverAddresses");
			}
			List<Socket> list = new List<Socket>(serverAddresses.Length);
			List<Task> list2 = new List<Task>(serverAddresses.Length);
			TaskCompletionSource<Socket> taskCompletionSource = new TaskCompletionSource<Socket>();
			StrongBox<Exception> lastError = new StrongBox<Exception>();
			StrongBox<int> pendingCompleteCount = new StrongBox<int>(serverAddresses.Length);
			foreach (IPAddress ipaddress in serverAddresses)
			{
				Socket socket = new Socket(ipaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				list.Add(socket);
				try
				{
					list2.Add(socket.ConnectAsync(ipaddress, port));
				}
				catch (Exception exception)
				{
					list2.Add(Task.FromException(exception));
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				SNITCPHandle.ParallelConnectHelper(list[j], list2[j], taskCompletionSource, pendingCompleteCount, lastError, list);
			}
			return taskCompletionSource.Task;
		}

		private static void ParallelConnectHelper(Socket socket, Task connectTask, TaskCompletionSource<Socket> tcs, StrongBox<int> pendingCompleteCount, StrongBox<Exception> lastError, List<Socket> sockets)
		{
			SNITCPHandle.<ParallelConnectHelper>d__22 <ParallelConnectHelper>d__;
			<ParallelConnectHelper>d__.socket = socket;
			<ParallelConnectHelper>d__.connectTask = connectTask;
			<ParallelConnectHelper>d__.tcs = tcs;
			<ParallelConnectHelper>d__.pendingCompleteCount = pendingCompleteCount;
			<ParallelConnectHelper>d__.lastError = lastError;
			<ParallelConnectHelper>d__.sockets = sockets;
			<ParallelConnectHelper>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ParallelConnectHelper>d__.<>1__state = -1;
			<ParallelConnectHelper>d__.<>t__builder.Start<SNITCPHandle.<ParallelConnectHelper>d__22>(ref <ParallelConnectHelper>d__);
		}

		public override uint EnableSsl(uint options)
		{
			this._validateCert = ((options & 1U) > 0U);
			try
			{
				this._sslStream.AuthenticateAsClient(this._targetServer);
				this._sslOverTdsStream.FinishHandshake();
			}
			catch (AuthenticationException sniException)
			{
				return this.ReportTcpSNIError(sniException);
			}
			catch (InvalidOperationException sniException2)
			{
				return this.ReportTcpSNIError(sniException2);
			}
			this._stream = this._sslStream;
			return 0U;
		}

		public override void DisableSsl()
		{
			this._sslStream.Dispose();
			this._sslStream = null;
			this._sslOverTdsStream.Dispose();
			this._sslOverTdsStream = null;
			this._stream = this._tcpStream;
		}

		private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return !this._validateCert || SNICommon.ValidateSslServerCertificate(this._targetServer, sender, cert, chain, policyErrors);
		}

		public override void SetBufferSize(int bufferSize)
		{
			this._bufferSize = bufferSize;
		}

		public override uint Send(SNIPacket packet)
		{
			uint result;
			lock (this)
			{
				try
				{
					packet.WriteToStream(this._stream);
					result = 0U;
				}
				catch (ObjectDisposedException sniException)
				{
					result = this.ReportTcpSNIError(sniException);
				}
				catch (SocketException sniException2)
				{
					result = this.ReportTcpSNIError(sniException2);
				}
				catch (IOException sniException3)
				{
					result = this.ReportTcpSNIError(sniException3);
				}
			}
			return result;
		}

		public override uint Receive(out SNIPacket packet, int timeoutInMilliseconds)
		{
			uint result;
			lock (this)
			{
				packet = null;
				try
				{
					if (timeoutInMilliseconds > 0)
					{
						this._socket.ReceiveTimeout = timeoutInMilliseconds;
					}
					else
					{
						if (timeoutInMilliseconds != -1)
						{
							this.ReportTcpSNIError(0U, 11U, string.Empty);
							return 258U;
						}
						this._socket.ReceiveTimeout = 0;
					}
					packet = new SNIPacket(this._bufferSize);
					packet.ReadFromStream(this._stream);
					if (packet.Length == 0)
					{
						Win32Exception ex = new Win32Exception();
						result = this.ReportErrorAndReleasePacket(packet, (uint)ex.NativeErrorCode, 0U, ex.Message);
					}
					else
					{
						result = 0U;
					}
				}
				catch (ObjectDisposedException sniException)
				{
					result = this.ReportErrorAndReleasePacket(packet, sniException);
				}
				catch (SocketException sniException2)
				{
					result = this.ReportErrorAndReleasePacket(packet, sniException2);
				}
				catch (IOException ex2)
				{
					uint num = this.ReportErrorAndReleasePacket(packet, ex2);
					if (ex2.InnerException is SocketException && ((SocketException)ex2.InnerException).SocketErrorCode == SocketError.TimedOut)
					{
						num = 258U;
					}
					result = num;
				}
				finally
				{
					this._socket.ReceiveTimeout = 0;
				}
			}
			return result;
		}

		public override void SetAsyncCallbacks(SNIAsyncCallback receiveCallback, SNIAsyncCallback sendCallback)
		{
			this._receiveCallback = receiveCallback;
			this._sendCallback = sendCallback;
		}

		public override uint SendAsync(SNIPacket packet, bool disposePacketAfterSendAsync, SNIAsyncCallback callback = null)
		{
			SNIAsyncCallback callback2 = callback ?? this._sendCallback;
			lock (this)
			{
				packet.WriteToStreamAsync(this._stream, callback2, SNIProviders.TCP_PROV, disposePacketAfterSendAsync);
			}
			return 997U;
		}

		public override uint ReceiveAsync(ref SNIPacket packet)
		{
			packet = new SNIPacket(this._bufferSize);
			uint result;
			try
			{
				packet.ReadFromStreamAsync(this._stream, this._receiveCallback);
				result = 997U;
			}
			catch (Exception ex) when (ex is ObjectDisposedException || ex is SocketException || ex is IOException)
			{
				result = this.ReportErrorAndReleasePacket(packet, ex);
			}
			return result;
		}

		public override uint CheckConnection()
		{
			try
			{
				if (!this._socket.Connected || this._socket.Poll(0, SelectMode.SelectError))
				{
					return 1U;
				}
			}
			catch (SocketException sniException)
			{
				return this.ReportTcpSNIError(sniException);
			}
			catch (ObjectDisposedException sniException2)
			{
				return this.ReportTcpSNIError(sniException2);
			}
			return 0U;
		}

		private uint ReportTcpSNIError(Exception sniException)
		{
			this._status = 1U;
			return SNICommon.ReportSNIError(SNIProviders.TCP_PROV, 35U, sniException);
		}

		private uint ReportTcpSNIError(uint nativeError, uint sniError, string errorMessage)
		{
			this._status = 1U;
			return SNICommon.ReportSNIError(SNIProviders.TCP_PROV, nativeError, sniError, errorMessage);
		}

		private uint ReportErrorAndReleasePacket(SNIPacket packet, Exception sniException)
		{
			if (packet != null)
			{
				packet.Release();
			}
			return this.ReportTcpSNIError(sniException);
		}

		private uint ReportErrorAndReleasePacket(SNIPacket packet, uint nativeError, uint sniError, string errorMessage)
		{
			if (packet != null)
			{
				packet.Release();
			}
			return this.ReportTcpSNIError(nativeError, sniError, errorMessage);
		}

		private readonly string _targetServer;

		private readonly object _callbackObject;

		private readonly Socket _socket;

		private NetworkStream _tcpStream;

		private Stream _stream;

		private SslStream _sslStream;

		private SslOverTdsStream _sslOverTdsStream;

		private SNIAsyncCallback _receiveCallback;

		private SNIAsyncCallback _sendCallback;

		private bool _validateCert = true;

		private int _bufferSize = 4096;

		private uint _status = uint.MaxValue;

		private Guid _connectionId = Guid.NewGuid();

		private const int MaxParallelIpAddresses = 64;
	}
}
