using System;
using System.IO;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.Threading.Tasks;

namespace System.Net.Security
{
	/// <summary>Provides a stream that uses the Negotiate security protocol to authenticate the client, and optionally the server, in client-server communication.</summary>
	public class NegotiateStream : AuthenticatedStream
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Security.NegotiateStream" /> class using the specified <see cref="T:System.IO.Stream" />.</summary>
		/// <param name="innerStream">A <see cref="T:System.IO.Stream" /> object used by the <see cref="T:System.Net.Security.NegotiateStream" /> for sending and receiving data.</param>
		[MonoTODO]
		public NegotiateStream(Stream innerStream) : base(innerStream, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Security.NegotiateStream" /> class using the specified <see cref="T:System.IO.Stream" /> and stream closure behavior.</summary>
		/// <param name="innerStream">A <see cref="T:System.IO.Stream" /> object used by the <see cref="T:System.Net.Security.NegotiateStream" /> for sending and receiving data.</param>
		/// <param name="leaveInnerStreamOpen">
		///   <see langword="true" /> to indicate that closing this <see cref="T:System.Net.Security.NegotiateStream" /> has no effect on <paramref name="innerStream" />; <see langword="false" /> to indicate that closing this <see cref="T:System.Net.Security.NegotiateStream" /> also closes <paramref name="innerStream" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="innerStream" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="innerStream" /> is equal to <see cref="F:System.IO.Stream.Null" />.</exception>
		[MonoTODO]
		public NegotiateStream(Stream innerStream, bool leaveInnerStreamOpen) : base(innerStream, leaveInnerStreamOpen)
		{
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the underlying stream is readable.</summary>
		/// <returns>
		///   <see langword="true" /> if authentication has occurred and the underlying stream is readable; otherwise, <see langword="false" />.</returns>
		public override bool CanRead
		{
			get
			{
				return base.InnerStream.CanRead;
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the underlying stream is seekable.</summary>
		/// <returns>This property always returns <see langword="false" />.</returns>
		public override bool CanSeek
		{
			get
			{
				return base.InnerStream.CanSeek;
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the underlying stream supports time-outs.</summary>
		/// <returns>
		///   <see langword="true" /> if the underlying stream supports time-outs; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public override bool CanTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the underlying stream is writable.</summary>
		/// <returns>
		///   <see langword="true" /> if authentication has occurred and the underlying stream is writable; otherwise, <see langword="false" />.</returns>
		public override bool CanWrite
		{
			get
			{
				return base.InnerStream.CanWrite;
			}
		}

		/// <summary>Gets a value that indicates how the server can use the client's credentials.</summary>
		/// <returns>One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values.</returns>
		/// <exception cref="T:System.InvalidOperationException">Authentication failed or has not occurred.</exception>
		[MonoTODO]
		public virtual TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether authentication was successful.</summary>
		/// <returns>
		///   <see langword="true" /> if successful authentication occurred; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public override bool IsAuthenticated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether this <see cref="T:System.Net.Security.NegotiateStream" /> uses data encryption.</summary>
		/// <returns>
		///   <see langword="true" /> if data is encrypted before being transmitted over the network and decrypted when it reaches the remote endpoint; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public override bool IsEncrypted
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether both the server and the client have been authenticated.</summary>
		/// <returns>
		///   <see langword="true" /> if the server has been authenticated; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public override bool IsMutuallyAuthenticated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the local side of the connection used by this <see cref="T:System.Net.Security.NegotiateStream" /> was authenticated as the server.</summary>
		/// <returns>
		///   <see langword="true" /> if the local endpoint was successfully authenticated as the server side of the authenticated connection; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public override bool IsServer
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the data sent using this stream is signed.</summary>
		/// <returns>
		///   <see langword="true" /> if the data is signed before being transmitted; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public override bool IsSigned
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the length of the underlying stream.</summary>
		/// <returns>A <see cref="T:System.Int64" /> that specifies the length of the underlying stream.</returns>
		/// <exception cref="T:System.NotSupportedException">Getting the value of this property is not supported when the underlying stream is a <see cref="T:System.Net.Sockets.NetworkStream" />.</exception>
		public override long Length
		{
			get
			{
				return base.InnerStream.Length;
			}
		}

		/// <summary>Gets or sets the current position in the underlying stream.</summary>
		/// <returns>A <see cref="T:System.Int64" /> that specifies the current position in the underlying stream.</returns>
		/// <exception cref="T:System.NotSupportedException">Setting this property is not supported.  
		/// -or-
		///  Getting the value of this property is not supported when the underlying stream is a <see cref="T:System.Net.Sockets.NetworkStream" />.</exception>
		public override long Position
		{
			get
			{
				return base.InnerStream.Position;
			}
			set
			{
				base.InnerStream.Position = value;
			}
		}

		/// <summary>Gets or sets the amount of time a read operation blocks waiting for data.</summary>
		/// <returns>A <see cref="T:System.Int32" /> that specifies the amount of time that will elapse before a read operation fails.</returns>
		public override int ReadTimeout
		{
			get
			{
				return this.readTimeout;
			}
			set
			{
				this.readTimeout = value;
			}
		}

		/// <summary>Gets information about the identity of the remote party sharing this authenticated stream.</summary>
		/// <returns>An <see cref="T:System.Security.Principal.IIdentity" /> object that describes the identity of the remote endpoint.</returns>
		/// <exception cref="T:System.InvalidOperationException">Authentication failed or has not occurred.</exception>
		[MonoTODO]
		public virtual IIdentity RemoteIdentity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the amount of time a write operation blocks waiting for data.</summary>
		/// <returns>A <see cref="T:System.Int32" /> that specifies the amount of time that will elapse before a write operation fails.</returns>
		public override int WriteTimeout
		{
			get
			{
				return this.writeTimeout;
			}
			set
			{
				this.writeTimeout = value;
			}
		}

		/// <summary>Called by clients to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. This method does not block.</summary>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsClient(AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified credentials and channel binding. This method does not block.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="binding">The <see cref="T:System.Security.Authentication.ExtendedProtection.ChannelBinding" /> that is used for extended protection.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the write operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="targetName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsClient(NetworkCredential credential, ChannelBinding binding, string targetName, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified credentials. This method does not block.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the write operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="targetName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsClient(NetworkCredential credential, string targetName, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified credentials and authentication options. This method does not block.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="allowedImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the write operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="targetName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsClient(NetworkCredential credential, string targetName, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel allowedImpersonationLevel, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified credentials, authentication options, and channel binding. This method does not block.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="binding">The <see cref="T:System.Security.Authentication.ExtendedProtection.ChannelBinding" /> that is used for extended protection.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="allowedImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the write operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="targetName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsClient(NetworkCredential credential, ChannelBinding binding, string targetName, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel allowedImpersonationLevel, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Begins an asynchronous read operation that reads data from the stream and stores it in the specified array.</summary>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that receives the bytes read from the stream.</param>
		/// <param name="offset">The zero-based location in <paramref name="buffer" /> at which to begin storing the data read from this stream.</param>
		/// <param name="count">The maximum number of bytes to read from the stream.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the read operation is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the read operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-
		///  <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-
		///  <paramref name="offset" /> plus <paramref name="count" /> is greater than the length of <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.IO.IOException">The read operation failed.  
		/// -or-
		///  Encryption is in use, but the data could not be decrypted.</exception>
		/// <exception cref="T:System.NotSupportedException">There is already a read operation in progress.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has not occurred.</exception>
		[MonoTODO]
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. This method does not block.</summary>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsServer(AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified server credentials, authentication options, and extended protection policy. This method does not block.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="policy">The <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> that is used for extended protection.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="requiredImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the write operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomChannelBinding" /> and <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomServiceNames" /> on the extended protection policy passed in the <paramref name="policy" /> parameter are both <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="requiredImpersonationLevel" /> must be <see cref="F:System.Security.Principal.TokenImpersonationLevel.Identification" />, <see cref="F:System.Security.Principal.TokenImpersonationLevel.Impersonation" />, or <see cref="F:System.Security.Principal.TokenImpersonationLevel.Delegation" />,</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the client. You cannot use the stream to retry authentication as the server.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The <paramref name="policy" /> parameter was set to <see cref="F:System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Always" /> on a platform that does not support extended protection.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsServer(NetworkCredential credential, ExtendedProtectionPolicy policy, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified server credentials and authentication options. This method does not block.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="requiredImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="requiredImpersonationLevel" /> must be <see cref="F:System.Security.Principal.TokenImpersonationLevel.Identification" />, <see cref="F:System.Security.Principal.TokenImpersonationLevel.Impersonation" />, or <see cref="F:System.Security.Principal.TokenImpersonationLevel.Delegation" />,</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the client. You cannot use the stream to retry authentication as the server.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsServer(NetworkCredential credential, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to begin an asynchronous operation to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified extended protection policy. This method does not block.</summary>
		/// <param name="policy">The <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> that is used for extended protection.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the authentication is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the write operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomChannelBinding" /> and <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomServiceNames" /> on the extended protection policy passed in the <paramref name="policy" /> parameter are both <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The <paramref name="policy" /> parameter was set to <see cref="F:System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Always" /> on a platform that does not support extended protection.</exception>
		[MonoTODO]
		public virtual IAsyncResult BeginAuthenticateAsServer(ExtendedProtectionPolicy policy, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Begins an asynchronous write operation that writes <see cref="T:System.Byte" />s from the specified buffer to the stream.</summary>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that supplies the bytes to be written to the stream.</param>
		/// <param name="offset">The zero-based location in <paramref name="buffer" /> at which to begin reading bytes to be written to the stream.</param>
		/// <param name="count">An <see cref="T:System.Int32" /> value that specifies the number of bytes to read from <paramref name="buffer" />.</param>
		/// <param name="asyncCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the write operation is complete.</param>
		/// <param name="asyncState">A user-defined object containing information about the write operation. This object is passed to the <paramref name="asyncCallback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object indicating the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset is less than 0" />.  
		/// -or-
		///  <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-
		///  <paramref name="offset" /> plus count is greater than the length of <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.IO.IOException">The write operation failed.  
		/// -or-
		///  Encryption is in use, but the data could not be encrypted.</exception>
		/// <exception cref="T:System.NotSupportedException">There is already a write operation in progress.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has not occurred.</exception>
		[MonoTODO]
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection.</summary>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsClient()
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified client credential.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetName" /> is <see langword="null" />.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsClient(NetworkCredential credential, string targetName)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified client credential and the channel binding.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="binding">The <see cref="T:System.Security.Authentication.ExtendedProtection.ChannelBinding" /> that is used for extended protection.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetName" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsClient(NetworkCredential credential, ChannelBinding binding, string targetName)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified credential, authentication options, and channel binding.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="binding">The <see cref="T:System.Security.Authentication.ExtendedProtection.ChannelBinding" /> that is used for extended protection.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="allowedImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetName" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="allowedImpersonationLevel" /> is not a valid value.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsClient(NetworkCredential credential, ChannelBinding binding, string targetName, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel allowedImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified credentials and authentication options.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="allowedImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="allowedImpersonationLevel" /> is not a valid value.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetName" /> is null.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsClient(NetworkCredential credential, string targetName, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel allowedImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to authenticate the client, and optionally the server, in a client-server connection.</summary>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsServer()
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified extended protection policy.</summary>
		/// <param name="policy">The <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> that is used for extended protection.</param>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomChannelBinding" /> and <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomServiceNames" /> on the extended protection policy passed in the <paramref name="policy" /> parameter are both <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The <paramref name="policy" /> parameter was set to <see cref="F:System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Always" /> on a platform that does not support extended protection.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsServer(ExtendedProtectionPolicy policy)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified server credentials, authentication options, and extended protection policy.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="policy">The <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> that is used for extended protection.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="requiredImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomChannelBinding" /> and <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomServiceNames" /> on the extended protection policy passed in the <paramref name="policy" /> parameter are both <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="requiredImpersonationLevel" /> must be <see cref="F:System.Security.Principal.TokenImpersonationLevel.Identification" />, <see cref="F:System.Security.Principal.TokenImpersonationLevel.Impersonation" />, or <see cref="F:System.Security.Principal.TokenImpersonationLevel.Delegation" />,</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to try to r-authenticate.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the client. You cannot use the stream to retry authentication as the server.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The <paramref name="policy" /> parameter was set to <see cref="F:System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Always" /> on a platform that does not support extended protection.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsServer(NetworkCredential credential, ExtendedProtectionPolicy policy, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to authenticate the client, and optionally the server, in a client-server connection. The authentication process uses the specified server credentials and authentication options.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the server.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="requiredImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="requiredImpersonationLevel" /> must be <see cref="F:System.Security.Principal.TokenImpersonationLevel.Identification" />, <see cref="F:System.Security.Principal.TokenImpersonationLevel.Impersonation" />, or <see cref="F:System.Security.Principal.TokenImpersonationLevel.Delegation" />,</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to try to r-authenticate.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the client. You cannot use the stream to retry authentication as the server.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		[MonoTODO]
		public virtual void AuthenticateAsServer(NetworkCredential credential, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Security.NegotiateStream" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		[MonoTODO]
		protected override void Dispose(bool disposing)
		{
		}

		/// <summary>Ends a pending asynchronous client authentication operation that was started with a call to <see cref="Overload:System.Net.Security.NegotiateStream.BeginAuthenticateAsClient" />.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> instance returned by a call to <see cref="Overload:System.Net.Security.NegotiateStream.BeginAuthenticateAsClient" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not created by a call to <see cref="Overload:System.Net.Security.NegotiateStream.BeginAuthenticateAsClient" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">There is no pending client authentication to complete.</exception>
		[MonoTODO]
		public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		/// <summary>Ends an asynchronous read operation that was started with a call to <see cref="M:System.Net.Security.NegotiateStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" />.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> instance returned by a call to <see cref="M:System.Net.Security.NegotiateStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /></param>
		/// <returns>A <see cref="T:System.Int32" /> value that specifies the number of bytes read from the underlying stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The asyncResult was not created by a call to <see cref="M:System.Net.Security.NegotiateStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">There is no pending read operation to complete.
		/// -or-
		/// Authentication has not occurred.</exception>
		/// <exception cref="T:System.IO.IOException">The read operation failed.</exception>
		[MonoTODO]
		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		/// <summary>Ends a pending asynchronous client authentication operation that was started with a call to <see cref="Overload:System.Net.Security.NegotiateStream.BeginAuthenticateAsServer" />.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> instance returned by a call to <see cref="Overload:System.Net.Security.NegotiateStream.BeginAuthenticateAsServer" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not created by a call to <see cref="Overload:System.Net.Security.NegotiateStream.BeginAuthenticateAsServer" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">There is no pending authentication to complete.</exception>
		[MonoTODO]
		public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		/// <summary>Ends an asynchronous write operation that was started with a call to <see cref="M:System.Net.Security.NegotiateStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" />.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> instance returned by a call to <see cref="M:System.Net.Security.NegotiateStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /></param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The asyncResult was not created by a call to <see cref="M:System.Net.Security.NegotiateStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">There is no pending write operation to complete.
		/// -or-
		/// Authentication has not occurred.</exception>
		/// <exception cref="T:System.IO.IOException">The write operation failed.</exception>
		[MonoTODO]
		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		/// <summary>Causes any buffered data to be written to the underlying device.</summary>
		[MonoTODO]
		public override void Flush()
		{
			base.InnerStream.Flush();
		}

		/// <summary>Reads data from this stream and stores it in the specified array.</summary>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that receives the bytes read from the stream.</param>
		/// <param name="offset">A <see cref="T:System.Int32" /> containing the zero-based location in <paramref name="buffer" /> at which to begin storing the data read from this stream.</param>
		/// <param name="count">A <see cref="T:System.Int32" /> containing the maximum number of bytes to read from the stream.</param>
		/// <returns>A <see cref="T:System.Int32" /> value that specifies the number of bytes read from the underlying stream. When there is no more data to be read, returns 0.</returns>
		/// <exception cref="T:System.IO.IOException">The read operation failed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has not occurred.</exception>
		/// <exception cref="T:System.NotSupportedException">A <see cref="M:System.Net.Security.NegotiateStream.Read(System.Byte[],System.Int32,System.Int32)" /> operation is already in progress.</exception>
		[MonoTODO]
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		/// <summary>Throws <see cref="T:System.NotSupportedException" />.</summary>
		/// <param name="offset">This value is ignored.</param>
		/// <param name="origin">This value is ignored.</param>
		/// <returns>Always throws a <see cref="T:System.NotSupportedException" />.</returns>
		/// <exception cref="T:System.NotSupportedException">Seeking is not supported on <see cref="T:System.Net.Security.NegotiateStream" />.</exception>
		[MonoTODO]
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		/// <summary>Sets the length of the underlying stream.</summary>
		/// <param name="value">An <see cref="T:System.Int64" /> value that specifies the length of the stream.</param>
		[MonoTODO]
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		/// <summary>Write the specified number of <see cref="T:System.Byte" />s to the underlying stream using the specified buffer and offset.</summary>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that supplies the bytes written to the stream.</param>
		/// <param name="offset">An <see cref="T:System.Int32" /> containing the zero-based location in <paramref name="buffer" /> at which to begin reading bytes to be written to the stream.</param>
		/// <param name="count">A <see cref="T:System.Int32" /> containing the number of bytes to read from <paramref name="buffer" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset is less than 0" />.  
		/// -or-
		///  <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-
		///  <paramref name="offset" /> plus count is greater than the length of <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.IO.IOException">The write operation failed.  
		/// -or-
		///  Encryption is in use, but the data could not be encrypted.</exception>
		/// <exception cref="T:System.NotSupportedException">There is already a write operation in progress.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has not occurred.</exception>
		[MonoTODO]
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		public virtual Task AuthenticateAsClientAsync()
		{
			return Task.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginAuthenticateAsClient), new Action<IAsyncResult>(this.EndAuthenticateAsClient), null);
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation. The authentication process uses the specified client credential.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetName" /> is <see langword="null" />.</exception>
		public virtual Task AuthenticateAsClientAsync(NetworkCredential credential, string targetName)
		{
			return Task.Factory.FromAsync<NetworkCredential, string>(new Func<NetworkCredential, string, AsyncCallback, object, IAsyncResult>(this.BeginAuthenticateAsClient), new Action<IAsyncResult>(this.EndAuthenticateAsClient), credential, targetName, null);
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation. The authentication process uses the specified credentials and authentication options.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="allowedImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="allowedImpersonationLevel" /> is not a valid value.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetName" /> is null.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		public virtual Task AuthenticateAsClientAsync(NetworkCredential credential, string targetName, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel allowedImpersonationLevel)
		{
			return Task.Factory.FromAsync((AsyncCallback callback, object state) => this.BeginAuthenticateAsClient(credential, targetName, requiredProtectionLevel, allowedImpersonationLevel, callback, state), new Action<IAsyncResult>(this.EndAuthenticateAsClient), null);
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation. The authentication process uses the specified client credential and the channel binding.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="binding">The <see cref="T:System.Security.Authentication.ExtendedProtection.ChannelBinding" /> that is used for extended protection.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetName" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		public virtual Task AuthenticateAsClientAsync(NetworkCredential credential, ChannelBinding binding, string targetName)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by clients to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation. The authentication process uses the specified credential, authentication options, and channel binding.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="binding">The <see cref="T:System.Security.Authentication.ExtendedProtection.ChannelBinding" /> that is used for extended protection.</param>
		/// <param name="targetName">The Service Principal Name (SPN) that uniquely identifies the server to authenticate.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="allowedImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetName" /> is <see langword="null" />.  
		/// -or-
		///  <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="allowedImpersonationLevel" /> is not a valid value.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the server. You cannot use the stream to retry authentication as the client.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		public virtual Task AuthenticateAsClientAsync(NetworkCredential credential, ChannelBinding binding, string targetName, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel allowedImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		public virtual Task AuthenticateAsServerAsync()
		{
			return Task.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginAuthenticateAsServer), new Action<IAsyncResult>(this.EndAuthenticateAsServer), null);
		}

		/// <summary>Called by servers to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation. The authentication process uses the specified extended protection policy.</summary>
		/// <param name="policy">The <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> that is used for extended protection.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomChannelBinding" /> and <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomServiceNames" /> on the extended protection policy passed in the <paramref name="policy" /> parameter are both <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The <paramref name="policy" /> parameter was set to <see cref="F:System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Always" /> on a platform that does not support extended protection.</exception>
		public virtual Task AuthenticateAsServerAsync(ExtendedProtectionPolicy policy)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation. The authentication process uses the specified server credentials and authentication options.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the server.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="requiredImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="requiredImpersonationLevel" /> must be <see cref="F:System.Security.Principal.TokenImpersonationLevel.Identification" />, <see cref="F:System.Security.Principal.TokenImpersonationLevel.Impersonation" />, or <see cref="F:System.Security.Principal.TokenImpersonationLevel.Delegation" />,</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to try to r-authenticate.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the client. You cannot use the stream to retry authentication as the server.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		public virtual Task AuthenticateAsServerAsync(NetworkCredential credential, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		/// <summary>Called by servers to authenticate the client, and optionally the server, in a client-server connection as an asynchronous operation. The authentication process uses the specified server credentials, authentication options, and extended protection policy.</summary>
		/// <param name="credential">The <see cref="T:System.Net.NetworkCredential" /> that is used to establish the identity of the client.</param>
		/// <param name="policy">The <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> that is used for extended protection.</param>
		/// <param name="requiredProtectionLevel">One of the <see cref="T:System.Net.Security.ProtectionLevel" /> values, indicating the security services for the stream.</param>
		/// <param name="requiredImpersonationLevel">One of the <see cref="T:System.Security.Principal.TokenImpersonationLevel" /> values, indicating how the server can use the client's credentials to access resources.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomChannelBinding" /> and <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomServiceNames" /> on the extended protection policy passed in the <paramref name="policy" /> parameter are both <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="credential" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="requiredImpersonationLevel" /> must be <see cref="F:System.Security.Principal.TokenImpersonationLevel.Identification" />, <see cref="F:System.Security.Principal.TokenImpersonationLevel.Impersonation" />, or <see cref="F:System.Security.Principal.TokenImpersonationLevel.Delegation" />,</exception>
		/// <exception cref="T:System.Security.Authentication.AuthenticationException">The authentication failed. You can use this object to try to r-authenticate.</exception>
		/// <exception cref="T:System.Security.Authentication.InvalidCredentialException">The authentication failed. You can use this object to retry the authentication.</exception>
		/// <exception cref="T:System.InvalidOperationException">Authentication has already occurred.  
		/// -or-
		///  This stream was used previously to attempt authentication as the client. You cannot use the stream to retry authentication as the server.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows 95 and Windows 98 are not supported.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The <paramref name="policy" /> parameter was set to <see cref="F:System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Always" /> on a platform that does not support extended protection.</exception>
		public virtual Task AuthenticateAsServerAsync(NetworkCredential credential, ExtendedProtectionPolicy policy, ProtectionLevel requiredProtectionLevel, TokenImpersonationLevel requiredImpersonationLevel)
		{
			throw new NotImplementedException();
		}

		private int readTimeout;

		private int writeTimeout;
	}
}
