using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal abstract class MobileAuthenticatedStream : AuthenticatedStream, IMonoSslStream, IDisposable
	{
		public MobileAuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen, SslStream owner, MonoTlsSettings settings, MobileTlsProvider provider) : base(innerStream, leaveInnerStreamOpen)
		{
			this.SslStream = owner;
			this.Settings = settings;
			this.Provider = provider;
			this.readBuffer = new BufferOffsetSize2(16500);
			this.writeBuffer = new BufferOffsetSize2(16384);
			this.operation = MobileAuthenticatedStream.Operation.None;
		}

		public SslStream SslStream { get; }

		public MonoTlsSettings Settings { get; }

		public MobileTlsProvider Provider { get; }

		MonoTlsProvider IMonoSslStream.Provider
		{
			get
			{
				return this.Provider;
			}
		}

		internal bool HasContext
		{
			get
			{
				return this.xobileTlsContext != null;
			}
		}

		internal string TargetHost { get; private set; }

		internal void CheckThrow(bool authSuccessCheck, bool shutdownCheck = false)
		{
			if (this.lastException != null)
			{
				this.lastException.Throw();
			}
			if (authSuccessCheck && !this.IsAuthenticated)
			{
				throw new InvalidOperationException("This operation is only allowed using a successfully authenticated context.");
			}
			if (shutdownCheck && this.shutdown)
			{
				throw new InvalidOperationException("Write operations are not allowed after the channel was shutdown.");
			}
		}

		internal static Exception GetSSPIException(Exception e)
		{
			if (e is OperationCanceledException || e is IOException || e is ObjectDisposedException || e is AuthenticationException || e is NotSupportedException)
			{
				return e;
			}
			return new AuthenticationException("Authentication failed, see inner exception.", e);
		}

		internal static Exception GetIOException(Exception e, string message)
		{
			if (e is OperationCanceledException || e is IOException || e is ObjectDisposedException || e is AuthenticationException || e is NotSupportedException)
			{
				return e;
			}
			return new IOException(message, e);
		}

		internal static Exception GetRenegotiationException(string message)
		{
			TlsException innerException = new TlsException(AlertDescription.NoRenegotiation, message);
			return new AuthenticationException("Authentication failed, see inner exception.", innerException);
		}

		internal static Exception GetInternalError()
		{
			throw new InvalidOperationException("Internal error.");
		}

		internal static Exception GetInvalidNestedCallException()
		{
			throw new InvalidOperationException("Invalid nested call.");
		}

		internal ExceptionDispatchInfo SetException(Exception e)
		{
			ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
			return Interlocked.CompareExchange<ExceptionDispatchInfo>(ref this.lastException, exceptionDispatchInfo, null) ?? exceptionDispatchInfo;
		}

		public void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			MonoSslClientAuthenticationOptions options = new MonoSslClientAuthenticationOptions
			{
				TargetHost = targetHost,
				ClientCertificates = clientCertificates,
				EnabledSslProtocols = enabledSslProtocols,
				CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
				EncryptionPolicy = EncryptionPolicy.RequireEncryption
			};
			Task task = this.ProcessAuthentication(true, options, CancellationToken.None);
			try
			{
				task.Wait();
			}
			catch (Exception e)
			{
				throw HttpWebRequest.FlattenException(e);
			}
		}

		public void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			MonoSslServerAuthenticationOptions options = new MonoSslServerAuthenticationOptions
			{
				ServerCertificate = serverCertificate,
				ClientCertificateRequired = clientCertificateRequired,
				EnabledSslProtocols = enabledSslProtocols,
				CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
				EncryptionPolicy = EncryptionPolicy.RequireEncryption
			};
			Task task = this.ProcessAuthentication(true, options, CancellationToken.None);
			try
			{
				task.Wait();
			}
			catch (Exception e)
			{
				throw HttpWebRequest.FlattenException(e);
			}
		}

		public Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			MonoSslClientAuthenticationOptions options = new MonoSslClientAuthenticationOptions
			{
				TargetHost = targetHost,
				ClientCertificates = clientCertificates,
				EnabledSslProtocols = enabledSslProtocols,
				CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
				EncryptionPolicy = EncryptionPolicy.RequireEncryption
			};
			return this.ProcessAuthentication(false, options, CancellationToken.None);
		}

		public Task AuthenticateAsClientAsync(IMonoSslClientAuthenticationOptions sslClientAuthenticationOptions, CancellationToken cancellationToken)
		{
			return this.ProcessAuthentication(false, (MonoSslClientAuthenticationOptions)sslClientAuthenticationOptions, cancellationToken);
		}

		public Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			MonoSslServerAuthenticationOptions options = new MonoSslServerAuthenticationOptions
			{
				ServerCertificate = serverCertificate,
				ClientCertificateRequired = clientCertificateRequired,
				EnabledSslProtocols = enabledSslProtocols,
				CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
				EncryptionPolicy = EncryptionPolicy.RequireEncryption
			};
			return this.ProcessAuthentication(false, options, CancellationToken.None);
		}

		public Task AuthenticateAsServerAsync(IMonoSslServerAuthenticationOptions sslServerAuthenticationOptions, CancellationToken cancellationToken)
		{
			return this.ProcessAuthentication(false, (MonoSslServerAuthenticationOptions)sslServerAuthenticationOptions, cancellationToken);
		}

		public Task ShutdownAsync()
		{
			AsyncShutdownRequest asyncRequest = new AsyncShutdownRequest(this);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Shutdown, asyncRequest, CancellationToken.None);
		}

		public AuthenticatedStream AuthenticatedStream
		{
			get
			{
				return this;
			}
		}

		private Task ProcessAuthentication(bool runSynchronously, MonoSslAuthenticationOptions options, CancellationToken cancellationToken)
		{
			MobileAuthenticatedStream.<ProcessAuthentication>d__48 <ProcessAuthentication>d__;
			<ProcessAuthentication>d__.<>4__this = this;
			<ProcessAuthentication>d__.runSynchronously = runSynchronously;
			<ProcessAuthentication>d__.options = options;
			<ProcessAuthentication>d__.cancellationToken = cancellationToken;
			<ProcessAuthentication>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessAuthentication>d__.<>1__state = -1;
			<ProcessAuthentication>d__.<>t__builder.Start<MobileAuthenticatedStream.<ProcessAuthentication>d__48>(ref <ProcessAuthentication>d__);
			return <ProcessAuthentication>d__.<>t__builder.Task;
		}

		protected abstract MobileTlsContext CreateContext(MonoSslAuthenticationOptions options);

		public override int Read(byte[] buffer, int offset, int count)
		{
			AsyncReadRequest asyncRequest = new AsyncReadRequest(this, true, buffer, offset, count);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Read, asyncRequest, CancellationToken.None).Result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			AsyncWriteRequest asyncRequest = new AsyncWriteRequest(this, true, buffer, offset, count);
			this.StartOperation(MobileAuthenticatedStream.OperationType.Write, asyncRequest, CancellationToken.None).Wait();
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			AsyncReadRequest asyncRequest = new AsyncReadRequest(this, false, buffer, offset, count);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Read, asyncRequest, cancellationToken);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			AsyncWriteRequest asyncRequest = new AsyncWriteRequest(this, false, buffer, offset, count);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Write, asyncRequest, cancellationToken);
		}

		public bool CanRenegotiate
		{
			get
			{
				this.CheckThrow(true, false);
				return this.xobileTlsContext != null && this.xobileTlsContext.CanRenegotiate;
			}
		}

		public Task RenegotiateAsync(CancellationToken cancellationToken)
		{
			AsyncRenegotiateRequest asyncRequest = new AsyncRenegotiateRequest(this);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Renegotiate, asyncRequest, cancellationToken);
		}

		private Task<int> StartOperation(MobileAuthenticatedStream.OperationType type, AsyncProtocolRequest asyncRequest, CancellationToken cancellationToken)
		{
			MobileAuthenticatedStream.<StartOperation>d__57 <StartOperation>d__;
			<StartOperation>d__.<>4__this = this;
			<StartOperation>d__.type = type;
			<StartOperation>d__.asyncRequest = asyncRequest;
			<StartOperation>d__.cancellationToken = cancellationToken;
			<StartOperation>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<StartOperation>d__.<>1__state = -1;
			<StartOperation>d__.<>t__builder.Start<MobileAuthenticatedStream.<StartOperation>d__57>(ref <StartOperation>d__);
			return <StartOperation>d__.<>t__builder.Task;
		}

		[Conditional("MONO_TLS_DEBUG")]
		protected internal void Debug(string format, params object[] args)
		{
		}

		[Conditional("MONO_TLS_DEBUG")]
		protected internal void Debug(string message)
		{
		}

		internal int InternalRead(byte[] buffer, int offset, int size, out bool outWantMore)
		{
			int result;
			try
			{
				AsyncProtocolRequest asyncRequest = this.asyncHandshakeRequest ?? this.asyncReadRequest;
				ValueTuple<int, bool> valueTuple = this.InternalRead(asyncRequest, this.readBuffer, buffer, offset, size);
				int item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				outWantMore = item2;
				result = item;
			}
			catch (Exception e)
			{
				this.SetException(MobileAuthenticatedStream.GetIOException(e, "InternalRead() failed"));
				outWantMore = false;
				result = -1;
			}
			return result;
		}

		private ValueTuple<int, bool> InternalRead(AsyncProtocolRequest asyncRequest, BufferOffsetSize internalBuffer, byte[] buffer, int offset, int size)
		{
			if (asyncRequest == null)
			{
				throw new InvalidOperationException();
			}
			if (internalBuffer.Size == 0 && !internalBuffer.Complete)
			{
				internalBuffer.Offset = (internalBuffer.Size = 0);
				asyncRequest.RequestRead(size);
				return new ValueTuple<int, bool>(0, true);
			}
			int num = Math.Min(internalBuffer.Size, size);
			Buffer.BlockCopy(internalBuffer.Buffer, internalBuffer.Offset, buffer, offset, num);
			internalBuffer.Offset += num;
			internalBuffer.Size -= num;
			return new ValueTuple<int, bool>(num, !internalBuffer.Complete && num < size);
		}

		internal bool InternalWrite(byte[] buffer, int offset, int size)
		{
			bool result;
			try
			{
				AsyncProtocolRequest asyncProtocolRequest;
				switch (this.operation)
				{
				case MobileAuthenticatedStream.Operation.Handshake:
				case MobileAuthenticatedStream.Operation.Renegotiate:
					asyncProtocolRequest = this.asyncHandshakeRequest;
					goto IL_57;
				case MobileAuthenticatedStream.Operation.Read:
					asyncProtocolRequest = this.asyncReadRequest;
					if (this.xobileTlsContext.PendingRenegotiation())
					{
						goto IL_57;
					}
					goto IL_57;
				case MobileAuthenticatedStream.Operation.Write:
				case MobileAuthenticatedStream.Operation.Close:
					asyncProtocolRequest = this.asyncWriteRequest;
					goto IL_57;
				}
				throw MobileAuthenticatedStream.GetInternalError();
				IL_57:
				if (asyncProtocolRequest == null && this.operation != MobileAuthenticatedStream.Operation.Close)
				{
					throw MobileAuthenticatedStream.GetInternalError();
				}
				result = this.InternalWrite(asyncProtocolRequest, this.writeBuffer, buffer, offset, size);
			}
			catch (Exception e)
			{
				this.SetException(MobileAuthenticatedStream.GetIOException(e, "InternalWrite() failed"));
				result = false;
			}
			return result;
		}

		private bool InternalWrite(AsyncProtocolRequest asyncRequest, BufferOffsetSize2 internalBuffer, byte[] buffer, int offset, int size)
		{
			if (asyncRequest == null)
			{
				if (this.lastException != null)
				{
					return false;
				}
				if (Interlocked.Exchange(ref this.closeRequested, 1) == 0)
				{
					internalBuffer.Reset();
				}
				else if (internalBuffer.Remaining == 0)
				{
					throw new InvalidOperationException();
				}
			}
			internalBuffer.AppendData(buffer, offset, size);
			if (asyncRequest != null)
			{
				asyncRequest.RequestWrite();
			}
			return true;
		}

		internal Task<int> InnerRead(bool sync, int requestedSize, CancellationToken cancellationToken)
		{
			MobileAuthenticatedStream.<InnerRead>d__66 <InnerRead>d__;
			<InnerRead>d__.<>4__this = this;
			<InnerRead>d__.sync = sync;
			<InnerRead>d__.requestedSize = requestedSize;
			<InnerRead>d__.cancellationToken = cancellationToken;
			<InnerRead>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<InnerRead>d__.<>1__state = -1;
			<InnerRead>d__.<>t__builder.Start<MobileAuthenticatedStream.<InnerRead>d__66>(ref <InnerRead>d__);
			return <InnerRead>d__.<>t__builder.Task;
		}

		internal Task InnerWrite(bool sync, CancellationToken cancellationToken)
		{
			MobileAuthenticatedStream.<InnerWrite>d__67 <InnerWrite>d__;
			<InnerWrite>d__.<>4__this = this;
			<InnerWrite>d__.sync = sync;
			<InnerWrite>d__.cancellationToken = cancellationToken;
			<InnerWrite>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<InnerWrite>d__.<>1__state = -1;
			<InnerWrite>d__.<>t__builder.Start<MobileAuthenticatedStream.<InnerWrite>d__67>(ref <InnerWrite>d__);
			return <InnerWrite>d__.<>t__builder.Task;
		}

		internal AsyncOperationStatus ProcessHandshake(AsyncOperationStatus status, bool renegotiate)
		{
			object obj = this.ioLock;
			AsyncOperationStatus result;
			lock (obj)
			{
				switch (this.operation)
				{
				case MobileAuthenticatedStream.Operation.None:
					if (renegotiate)
					{
						throw MobileAuthenticatedStream.GetInternalError();
					}
					this.operation = MobileAuthenticatedStream.Operation.Handshake;
					break;
				case MobileAuthenticatedStream.Operation.Handshake:
				case MobileAuthenticatedStream.Operation.Renegotiate:
					break;
				case MobileAuthenticatedStream.Operation.Authenticated:
					if (!renegotiate)
					{
						throw MobileAuthenticatedStream.GetInternalError();
					}
					this.operation = MobileAuthenticatedStream.Operation.Renegotiate;
					break;
				default:
					throw MobileAuthenticatedStream.GetInternalError();
				}
				switch (status)
				{
				case AsyncOperationStatus.Initialize:
					if (renegotiate)
					{
						this.xobileTlsContext.Renegotiate();
					}
					else
					{
						this.xobileTlsContext.StartHandshake();
					}
					result = AsyncOperationStatus.Continue;
					break;
				case AsyncOperationStatus.Continue:
				{
					AsyncOperationStatus asyncOperationStatus = AsyncOperationStatus.Continue;
					try
					{
						if (this.xobileTlsContext.ProcessHandshake())
						{
							this.xobileTlsContext.FinishHandshake();
							this.operation = MobileAuthenticatedStream.Operation.Authenticated;
							asyncOperationStatus = AsyncOperationStatus.Complete;
						}
					}
					catch (Exception e)
					{
						this.SetException(MobileAuthenticatedStream.GetSSPIException(e));
						base.Dispose();
						throw;
					}
					if (this.lastException != null)
					{
						this.lastException.Throw();
					}
					result = asyncOperationStatus;
					break;
				}
				case AsyncOperationStatus.ReadDone:
					throw new IOException("Authentication failed because the remote party has closed the transport stream.");
				default:
					throw new InvalidOperationException();
				}
			}
			return result;
		}

		[return: TupleElementNames(new string[]
		{
			"ret",
			"wantMore"
		})]
		internal ValueTuple<int, bool> ProcessRead(BufferOffsetSize userBuffer)
		{
			object obj = this.ioLock;
			ValueTuple<int, bool> result;
			lock (obj)
			{
				if (this.operation != MobileAuthenticatedStream.Operation.Authenticated)
				{
					throw MobileAuthenticatedStream.GetInternalError();
				}
				this.operation = MobileAuthenticatedStream.Operation.Read;
				ValueTuple<int, bool> valueTuple = this.xobileTlsContext.Read(userBuffer.Buffer, userBuffer.Offset, userBuffer.Size);
				if (this.lastException != null)
				{
					this.lastException.Throw();
				}
				this.operation = MobileAuthenticatedStream.Operation.Authenticated;
				result = valueTuple;
			}
			return result;
		}

		[return: TupleElementNames(new string[]
		{
			"ret",
			"wantMore"
		})]
		internal ValueTuple<int, bool> ProcessWrite(BufferOffsetSize userBuffer)
		{
			object obj = this.ioLock;
			ValueTuple<int, bool> result;
			lock (obj)
			{
				if (this.operation != MobileAuthenticatedStream.Operation.Authenticated)
				{
					throw MobileAuthenticatedStream.GetInternalError();
				}
				this.operation = MobileAuthenticatedStream.Operation.Write;
				ValueTuple<int, bool> valueTuple = this.xobileTlsContext.Write(userBuffer.Buffer, userBuffer.Offset, userBuffer.Size);
				if (this.lastException != null)
				{
					this.lastException.Throw();
				}
				this.operation = MobileAuthenticatedStream.Operation.Authenticated;
				result = valueTuple;
			}
			return result;
		}

		internal AsyncOperationStatus ProcessShutdown(AsyncOperationStatus status)
		{
			object obj = this.ioLock;
			AsyncOperationStatus result;
			lock (obj)
			{
				if (this.operation != MobileAuthenticatedStream.Operation.Authenticated)
				{
					throw MobileAuthenticatedStream.GetInternalError();
				}
				this.operation = MobileAuthenticatedStream.Operation.Close;
				this.xobileTlsContext.Shutdown();
				this.shutdown = true;
				this.operation = MobileAuthenticatedStream.Operation.Authenticated;
				result = AsyncOperationStatus.Complete;
			}
			return result;
		}

		public override bool IsServer
		{
			get
			{
				this.CheckThrow(false, false);
				return this.xobileTlsContext != null && this.xobileTlsContext.IsServer;
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				object obj = this.ioLock;
				bool result;
				lock (obj)
				{
					result = (this.xobileTlsContext != null && this.lastException == null && this.xobileTlsContext.IsAuthenticated);
				}
				return result;
			}
		}

		public override bool IsMutuallyAuthenticated
		{
			get
			{
				object obj = this.ioLock;
				bool result;
				lock (obj)
				{
					if (!this.IsAuthenticated)
					{
						result = false;
					}
					else if ((this.xobileTlsContext.IsServer ? this.xobileTlsContext.LocalServerCertificate : this.xobileTlsContext.LocalClientCertificate) == null)
					{
						result = false;
					}
					else
					{
						result = this.xobileTlsContext.IsRemoteCertificateAvailable;
					}
				}
				return result;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				object obj = this.ioLock;
				lock (obj)
				{
					this.SetException(new ObjectDisposedException("MobileAuthenticatedStream"));
					if (this.xobileTlsContext != null)
					{
						this.xobileTlsContext.Dispose();
						this.xobileTlsContext = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void Flush()
		{
			base.InnerStream.Flush();
		}

		public SslProtocols SslProtocol
		{
			get
			{
				object obj = this.ioLock;
				SslProtocols negotiatedProtocol;
				lock (obj)
				{
					this.CheckThrow(true, false);
					negotiatedProtocol = (SslProtocols)this.xobileTlsContext.NegotiatedProtocol;
				}
				return negotiatedProtocol;
			}
		}

		public X509Certificate RemoteCertificate
		{
			get
			{
				object obj = this.ioLock;
				X509Certificate remoteCertificate;
				lock (obj)
				{
					this.CheckThrow(true, false);
					remoteCertificate = this.xobileTlsContext.RemoteCertificate;
				}
				return remoteCertificate;
			}
		}

		public X509Certificate LocalCertificate
		{
			get
			{
				object obj = this.ioLock;
				X509Certificate internalLocalCertificate;
				lock (obj)
				{
					this.CheckThrow(true, false);
					internalLocalCertificate = this.InternalLocalCertificate;
				}
				return internalLocalCertificate;
			}
		}

		public X509Certificate InternalLocalCertificate
		{
			get
			{
				object obj = this.ioLock;
				X509Certificate result;
				lock (obj)
				{
					this.CheckThrow(false, false);
					if (this.xobileTlsContext == null)
					{
						result = null;
					}
					else
					{
						result = (this.xobileTlsContext.IsServer ? this.xobileTlsContext.LocalServerCertificate : this.xobileTlsContext.LocalClientCertificate);
					}
				}
				return result;
			}
		}

		public MonoTlsConnectionInfo GetConnectionInfo()
		{
			object obj = this.ioLock;
			MonoTlsConnectionInfo connectionInfo;
			lock (obj)
			{
				this.CheckThrow(true, false);
				connectionInfo = this.xobileTlsContext.ConnectionInfo;
			}
			return connectionInfo;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			base.InnerStream.SetLength(value);
		}

		public TransportContext TransportContext
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.IsAuthenticated && base.InnerStream.CanRead;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return base.InnerStream.CanTimeout;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return (this.IsAuthenticated & base.InnerStream.CanWrite) && !this.shutdown;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return base.InnerStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return base.InnerStream.Position;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override bool IsEncrypted
		{
			get
			{
				return this.IsAuthenticated;
			}
		}

		public override bool IsSigned
		{
			get
			{
				return this.IsAuthenticated;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return base.InnerStream.ReadTimeout;
			}
			set
			{
				base.InnerStream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return base.InnerStream.WriteTimeout;
			}
			set
			{
				base.InnerStream.WriteTimeout = value;
			}
		}

		public System.Security.Authentication.CipherAlgorithmType CipherAlgorithm
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return System.Security.Authentication.CipherAlgorithmType.None;
				}
				switch (connectionInfo.CipherAlgorithmType)
				{
				case Mono.Security.Interface.CipherAlgorithmType.Aes128:
				case Mono.Security.Interface.CipherAlgorithmType.AesGcm128:
					return System.Security.Authentication.CipherAlgorithmType.Aes128;
				case Mono.Security.Interface.CipherAlgorithmType.Aes256:
				case Mono.Security.Interface.CipherAlgorithmType.AesGcm256:
					return System.Security.Authentication.CipherAlgorithmType.Aes256;
				default:
					return System.Security.Authentication.CipherAlgorithmType.None;
				}
			}
		}

		public System.Security.Authentication.HashAlgorithmType HashAlgorithm
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return System.Security.Authentication.HashAlgorithmType.None;
				}
				Mono.Security.Interface.HashAlgorithmType hashAlgorithmType = connectionInfo.HashAlgorithmType;
				if (hashAlgorithmType != Mono.Security.Interface.HashAlgorithmType.Md5)
				{
					if (hashAlgorithmType - Mono.Security.Interface.HashAlgorithmType.Sha1 <= 4)
					{
						return System.Security.Authentication.HashAlgorithmType.Sha1;
					}
					if (hashAlgorithmType != Mono.Security.Interface.HashAlgorithmType.Md5Sha1)
					{
						return System.Security.Authentication.HashAlgorithmType.None;
					}
				}
				return System.Security.Authentication.HashAlgorithmType.Md5;
			}
		}

		public System.Security.Authentication.ExchangeAlgorithmType KeyExchangeAlgorithm
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return System.Security.Authentication.ExchangeAlgorithmType.None;
				}
				switch (connectionInfo.ExchangeAlgorithmType)
				{
				case Mono.Security.Interface.ExchangeAlgorithmType.Dhe:
				case Mono.Security.Interface.ExchangeAlgorithmType.EcDhe:
					return System.Security.Authentication.ExchangeAlgorithmType.DiffieHellman;
				case Mono.Security.Interface.ExchangeAlgorithmType.Rsa:
					return System.Security.Authentication.ExchangeAlgorithmType.RsaSign;
				default:
					return System.Security.Authentication.ExchangeAlgorithmType.None;
				}
			}
		}

		public int CipherStrength
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return 0;
				}
				switch (connectionInfo.CipherAlgorithmType)
				{
				case Mono.Security.Interface.CipherAlgorithmType.None:
				case Mono.Security.Interface.CipherAlgorithmType.Aes128:
				case Mono.Security.Interface.CipherAlgorithmType.AesGcm128:
					return 128;
				case Mono.Security.Interface.CipherAlgorithmType.Aes256:
				case Mono.Security.Interface.CipherAlgorithmType.AesGcm256:
					return 256;
				default:
					throw new ArgumentOutOfRangeException("CipherAlgorithmType");
				}
			}
		}

		public int HashStrength
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return 0;
				}
				Mono.Security.Interface.HashAlgorithmType hashAlgorithmType = connectionInfo.HashAlgorithmType;
				switch (hashAlgorithmType)
				{
				case Mono.Security.Interface.HashAlgorithmType.Md5:
					break;
				case Mono.Security.Interface.HashAlgorithmType.Sha1:
					return 160;
				case Mono.Security.Interface.HashAlgorithmType.Sha224:
					return 224;
				case Mono.Security.Interface.HashAlgorithmType.Sha256:
					return 256;
				case Mono.Security.Interface.HashAlgorithmType.Sha384:
					return 384;
				case Mono.Security.Interface.HashAlgorithmType.Sha512:
					return 512;
				default:
					if (hashAlgorithmType != Mono.Security.Interface.HashAlgorithmType.Md5Sha1)
					{
						throw new ArgumentOutOfRangeException("HashAlgorithmType");
					}
					break;
				}
				return 128;
			}
		}

		public int KeyExchangeStrength
		{
			get
			{
				return 0;
			}
		}

		public bool CheckCertRevocationStatus
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private MobileTlsContext xobileTlsContext;

		private ExceptionDispatchInfo lastException;

		private AsyncProtocolRequest asyncHandshakeRequest;

		private AsyncProtocolRequest asyncReadRequest;

		private AsyncProtocolRequest asyncWriteRequest;

		private BufferOffsetSize2 readBuffer;

		private BufferOffsetSize2 writeBuffer;

		private object ioLock = new object();

		private int closeRequested;

		private bool shutdown;

		private MobileAuthenticatedStream.Operation operation;

		private static int uniqueNameInteger = 123;

		private static int nextId;

		internal readonly int ID = ++MobileAuthenticatedStream.nextId;

		private enum Operation
		{
			None,
			Handshake,
			Authenticated,
			Renegotiate,
			Read,
			Write,
			Close
		}

		private enum OperationType
		{
			Read,
			Write,
			Renegotiate,
			Shutdown
		}
	}
}
