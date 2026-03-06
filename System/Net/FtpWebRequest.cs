using System;
using System.IO;
using System.Net.Cache;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Unity;

namespace System.Net
{
	/// <summary>Implements a File Transfer Protocol (FTP) client.</summary>
	public sealed class FtpWebRequest : WebRequest
	{
		internal FtpMethodInfo MethodInfo
		{
			get
			{
				return this._methodInfo;
			}
		}

		/// <summary>Defines the default cache policy for all FTP requests.</summary>
		/// <returns>A <see cref="T:System.Net.Cache.RequestCachePolicy" /> that defines the cache policy for FTP requests.</returns>
		/// <exception cref="T:System.ArgumentNullException">The caller tried to set this property to <see langword="null" />.</exception>
		public new static RequestCachePolicy DefaultCachePolicy
		{
			get
			{
				return WebRequest.DefaultCachePolicy;
			}
			set
			{
			}
		}

		/// <summary>Gets or sets the command to send to the FTP server.</summary>
		/// <returns>A <see cref="T:System.String" /> value that contains the FTP command to send to the server. The default value is <see cref="F:System.Net.WebRequestMethods.Ftp.DownloadFile" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		/// <exception cref="T:System.ArgumentException">The method is invalid.  
		/// -or-
		///  The method is not supported.  
		/// -or-
		///  Multiple methods were specified.</exception>
		public override string Method
		{
			get
			{
				return this._methodInfo.Method;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("FTP Method names cannot be null or empty.", "value");
				}
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				try
				{
					this._methodInfo = FtpMethodInfo.GetMethodInfo(value);
				}
				catch (ArgumentException)
				{
					throw new ArgumentException("This method is not supported.", "value");
				}
			}
		}

		/// <summary>Gets or sets the new name of a file being renamed.</summary>
		/// <returns>The new name of the file being renamed.</returns>
		/// <exception cref="T:System.ArgumentException">The value specified for a set operation is <see langword="null" /> or an empty string.</exception>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		public string RenameTo
		{
			get
			{
				return this._renameTo;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("The RenameTo filename cannot be null or empty.", "value");
				}
				this._renameTo = value;
			}
		}

		/// <summary>Gets or sets the credentials used to communicate with the FTP server.</summary>
		/// <returns>An <see cref="T:System.Net.ICredentials" /> instance; otherwise, <see langword="null" /> if the property has not been set.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value specified for a set operation is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">An <see cref="T:System.Net.ICredentials" /> of a type other than <see cref="T:System.Net.NetworkCredential" /> was specified for a set operation.</exception>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		public override ICredentials Credentials
		{
			get
			{
				return this._authInfo;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value == CredentialCache.DefaultNetworkCredentials)
				{
					throw new ArgumentException("Default credentials are not supported on an FTP request.", "value");
				}
				this._authInfo = value;
			}
		}

		/// <summary>Gets the URI requested by this instance.</summary>
		/// <returns>A <see cref="T:System.Uri" /> instance that identifies a resource that is accessed using the File Transfer Protocol.</returns>
		public override Uri RequestUri
		{
			get
			{
				return this._uri;
			}
		}

		/// <summary>Gets or sets the number of milliseconds to wait for a request.</summary>
		/// <returns>An <see cref="T:System.Int32" /> value that contains the number of milliseconds to wait before a request times out. The default value is <see cref="F:System.Threading.Timeout.Infinite" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value specified is less than zero and is not <see cref="F:System.Threading.Timeout.Infinite" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		public override int Timeout
		{
			get
			{
				return this._timeout;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				if (value < 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value", "Timeout can be only be set to 'System.Threading.Timeout.Infinite' or a value >= 0.");
				}
				if (this._timeout != value)
				{
					this._timeout = value;
					this._timerQueue = null;
				}
			}
		}

		internal int RemainingTimeout
		{
			get
			{
				return this._remainingTimeout;
			}
		}

		/// <summary>Gets or sets a time-out when reading from or writing to a stream.</summary>
		/// <returns>The number of milliseconds before the reading or writing times out. The default value is 300,000 milliseconds (5 minutes).</returns>
		/// <exception cref="T:System.InvalidOperationException">The request has already been sent.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than or equal to zero and is not equal to <see cref="F:System.Threading.Timeout.Infinite" />.</exception>
		public int ReadWriteTimeout
		{
			get
			{
				return this._readWriteTimeout;
			}
			set
			{
				if (this._getResponseStarted)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				if (value <= 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value", "Timeout can be only be set to 'System.Threading.Timeout.Infinite' or a value > 0.");
				}
				this._readWriteTimeout = value;
			}
		}

		/// <summary>Gets or sets a byte offset into the file being downloaded by this request.</summary>
		/// <returns>An <see cref="T:System.Int64" /> instance that specifies the file offset, in bytes. The default value is zero.</returns>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for this property is less than zero.</exception>
		public long ContentOffset
		{
			get
			{
				return this._contentOffset;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._contentOffset = value;
			}
		}

		/// <summary>Gets or sets a value that is ignored by the <see cref="T:System.Net.FtpWebRequest" /> class.</summary>
		/// <returns>An <see cref="T:System.Int64" /> value that should be ignored.</returns>
		public override long ContentLength
		{
			get
			{
				return this._contentLength;
			}
			set
			{
				this._contentLength = value;
			}
		}

		/// <summary>Gets or sets the proxy used to communicate with the FTP server.</summary>
		/// <returns>An <see cref="T:System.Net.IWebProxy" /> instance responsible for communicating with the FTP server. On .NET Core, its value is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">This property cannot be set to <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		public override IWebProxy Proxy
		{
			get
			{
				return null;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
			}
		}

		/// <summary>Gets or sets the name of the connection group that contains the service point used to send the current request.</summary>
		/// <returns>A <see cref="T:System.String" /> value that contains a connection group name.</returns>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		public override string ConnectionGroupName
		{
			get
			{
				return this._connectionGroupName;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				this._connectionGroupName = value;
			}
		}

		/// <summary>Gets the <see cref="T:System.Net.ServicePoint" /> object used to connect to the FTP server.</summary>
		/// <returns>A <see cref="T:System.Net.ServicePoint" /> object that can be used to customize connection behavior.</returns>
		public ServicePoint ServicePoint
		{
			get
			{
				if (this._servicePoint == null)
				{
					this._servicePoint = ServicePointManager.FindServicePoint(this._uri);
				}
				return this._servicePoint;
			}
		}

		internal bool Aborted
		{
			get
			{
				return this._aborted;
			}
		}

		internal FtpWebRequest(Uri uri)
		{
			this._timeout = 100000;
			this._passive = true;
			this._binary = true;
			this._timerQueue = FtpWebRequest.s_DefaultTimerQueue;
			this._readWriteTimeout = 300000;
			base..ctor();
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, uri, ".ctor");
			}
			if (uri.Scheme != Uri.UriSchemeFtp)
			{
				throw new ArgumentOutOfRangeException("uri");
			}
			this._timerCallback = new TimerThread.Callback(this.TimerCallback);
			this._syncObject = new object();
			NetworkCredential networkCredential = null;
			this._uri = uri;
			this._methodInfo = FtpMethodInfo.GetMethodInfo("RETR");
			if (this._uri.UserInfo != null && this._uri.UserInfo.Length != 0)
			{
				string userInfo = this._uri.UserInfo;
				string userName = userInfo;
				string password = "";
				int num = userInfo.IndexOf(':');
				if (num != -1)
				{
					userName = Uri.UnescapeDataString(userInfo.Substring(0, num));
					num++;
					password = Uri.UnescapeDataString(userInfo.Substring(num, userInfo.Length - num));
				}
				networkCredential = new NetworkCredential(userName, password);
			}
			if (networkCredential == null)
			{
				networkCredential = FtpWebRequest.s_defaultFtpNetworkCredential;
			}
			this._authInfo = networkCredential;
		}

		/// <summary>Returns the FTP server response.</summary>
		/// <returns>A <see cref="T:System.Net.WebResponse" /> reference that contains an <see cref="T:System.Net.FtpWebResponse" /> instance. This object contains the FTP server's response to the request.</returns>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.FtpWebRequest.GetResponse" /> or <see cref="M:System.Net.FtpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)" /> has already been called for this instance.  
		/// -or-
		///  An HTTP proxy is enabled, and you attempted to use an FTP command other than <see cref="F:System.Net.WebRequestMethods.Ftp.DownloadFile" />, <see cref="F:System.Net.WebRequestMethods.Ftp.ListDirectory" />, or <see cref="F:System.Net.WebRequestMethods.Ftp.ListDirectoryDetails" />.</exception>
		/// <exception cref="T:System.Net.WebException">
		///   <see cref="P:System.Net.FtpWebRequest.EnableSsl" /> is set to <see langword="true" />, but the server does not support this feature.  
		/// -or-
		///  A <see cref="P:System.Net.FtpWebRequest.Timeout" /> was specified and the timeout has expired.</exception>
		public override WebResponse GetResponse()
		{
			if (NetEventSource.IsEnabled)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Enter(this, null, "GetResponse");
				}
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("Method: {0}", new object[]
					{
						this._methodInfo.Method
					}), "GetResponse");
				}
			}
			try
			{
				this.CheckError();
				if (this._ftpWebResponse != null)
				{
					return this._ftpWebResponse;
				}
				if (this._getResponseStarted)
				{
					throw new InvalidOperationException("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress.");
				}
				this._getResponseStarted = true;
				this._startTime = DateTime.UtcNow;
				this._remainingTimeout = this.Timeout;
				if (this.Timeout != -1)
				{
					this._remainingTimeout = this.Timeout - (int)(DateTime.UtcNow - this._startTime).TotalMilliseconds;
					if (this._remainingTimeout <= 0)
					{
						throw ExceptionHelper.TimeoutException;
					}
				}
				FtpWebRequest.RequestStage requestStage = this.FinishRequestStage(FtpWebRequest.RequestStage.RequestStarted);
				if (requestStage >= FtpWebRequest.RequestStage.RequestStarted)
				{
					if (requestStage < FtpWebRequest.RequestStage.ReadReady)
					{
						object syncObject = this._syncObject;
						lock (syncObject)
						{
							if (this._requestStage < FtpWebRequest.RequestStage.ReadReady)
							{
								this._readAsyncResult = new LazyAsyncResult(null, null, null);
							}
						}
						if (this._readAsyncResult != null)
						{
							this._readAsyncResult.InternalWaitForCompletion();
						}
						this.CheckError();
					}
				}
				else
				{
					this.SubmitRequest(false);
					if (this._methodInfo.IsUpload)
					{
						this.FinishRequestStage(FtpWebRequest.RequestStage.WriteReady);
					}
					else
					{
						this.FinishRequestStage(FtpWebRequest.RequestStage.ReadReady);
					}
					this.CheckError();
					this.EnsureFtpWebResponse(null);
				}
			}
			catch (Exception ex)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(this, ex, "GetResponse");
				}
				if (this._exception == null)
				{
					if (NetEventSource.IsEnabled)
					{
						NetEventSource.Error(this, ex, "GetResponse");
					}
					this.SetException(ex);
					this.FinishRequestStage(FtpWebRequest.RequestStage.CheckForError);
				}
				throw;
			}
			finally
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, this._ftpWebResponse, "GetResponse");
				}
			}
			return this._ftpWebResponse;
		}

		/// <summary>Begins sending a request and receiving a response from an FTP server asynchronously.</summary>
		/// <param name="callback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the operation. This object is passed to the <paramref name="callback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> instance that indicates the status of the operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.FtpWebRequest.GetResponse" /> or <see cref="M:System.Net.FtpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)" /> has already been called for this instance.</exception>
		public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, null, "BeginGetResponse");
				NetEventSource.Info(this, FormattableStringFactory.Create("Method: {0}", new object[]
				{
					this._methodInfo.Method
				}), "BeginGetResponse");
			}
			ContextAwareResult contextAwareResult;
			try
			{
				if (this._ftpWebResponse != null)
				{
					contextAwareResult = new ContextAwareResult(this, state, callback);
					contextAwareResult.InvokeCallback(this._ftpWebResponse);
					return contextAwareResult;
				}
				if (this._getResponseStarted)
				{
					throw new InvalidOperationException("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress.");
				}
				this._getResponseStarted = true;
				this.CheckError();
				FtpWebRequest.RequestStage requestStage = this.FinishRequestStage(FtpWebRequest.RequestStage.RequestStarted);
				contextAwareResult = new ContextAwareResult(true, true, this, state, callback);
				this._readAsyncResult = contextAwareResult;
				if (requestStage >= FtpWebRequest.RequestStage.RequestStarted)
				{
					contextAwareResult.StartPostingAsyncOp();
					contextAwareResult.FinishPostingAsyncOp();
					if (requestStage >= FtpWebRequest.RequestStage.ReadReady)
					{
						contextAwareResult = null;
					}
					else
					{
						object obj = this._syncObject;
						lock (obj)
						{
							if (this._requestStage >= FtpWebRequest.RequestStage.ReadReady)
							{
								contextAwareResult = null;
							}
						}
					}
					if (contextAwareResult == null)
					{
						contextAwareResult = (ContextAwareResult)this._readAsyncResult;
						if (!contextAwareResult.InternalPeekCompleted)
						{
							contextAwareResult.InvokeCallback();
						}
					}
				}
				else
				{
					object obj = contextAwareResult.StartPostingAsyncOp();
					lock (obj)
					{
						this.SubmitRequest(true);
						contextAwareResult.FinishPostingAsyncOp();
					}
					this.FinishRequestStage(FtpWebRequest.RequestStage.CheckForError);
				}
			}
			catch (Exception message)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(this, message, "BeginGetResponse");
				}
				throw;
			}
			finally
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, null, "BeginGetResponse");
				}
			}
			return contextAwareResult;
		}

		/// <summary>Ends a pending asynchronous operation started with <see cref="M:System.Net.FtpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)" />.</summary>
		/// <param name="asyncResult">The <see cref="T:System.IAsyncResult" /> that was returned when the operation started.</param>
		/// <returns>A <see cref="T:System.Net.WebResponse" /> reference that contains an <see cref="T:System.Net.FtpWebResponse" /> instance. This object contains the FTP server's response to the request.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not obtained by calling <see cref="M:System.Net.FtpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">This method was already called for the operation identified by <paramref name="asyncResult" />.</exception>
		/// <exception cref="T:System.Net.WebException">An error occurred using an HTTP proxy.</exception>
		public override WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, null, "EndGetResponse");
			}
			try
			{
				if (asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
				if (lazyAsyncResult == null)
				{
					throw new ArgumentException("The IAsyncResult object was not returned from the corresponding asynchronous method on this class.", "asyncResult");
				}
				if (lazyAsyncResult.EndCalled)
				{
					throw new InvalidOperationException(SR.Format("{0} can only be called once for each asynchronous operation.", "EndGetResponse"));
				}
				lazyAsyncResult.InternalWaitForCompletion();
				lazyAsyncResult.EndCalled = true;
				this.CheckError();
			}
			catch (Exception message)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(this, message, "EndGetResponse");
				}
				throw;
			}
			finally
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, null, "EndGetResponse");
				}
			}
			return this._ftpWebResponse;
		}

		/// <summary>Retrieves the stream used to upload data to an FTP server.</summary>
		/// <returns>A writable <see cref="T:System.IO.Stream" /> instance used to store data to be sent to the server by the current request.</returns>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.FtpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)" /> has been called and has not completed.  
		/// -or-
		///  An HTTP proxy is enabled, and you attempted to use an FTP command other than <see cref="F:System.Net.WebRequestMethods.Ftp.DownloadFile" />, <see cref="F:System.Net.WebRequestMethods.Ftp.ListDirectory" />, or <see cref="F:System.Net.WebRequestMethods.Ftp.ListDirectoryDetails" />.</exception>
		/// <exception cref="T:System.Net.WebException">A connection to the FTP server could not be established.</exception>
		/// <exception cref="T:System.Net.ProtocolViolationException">The <see cref="P:System.Net.FtpWebRequest.Method" /> property is not set to <see cref="F:System.Net.WebRequestMethods.Ftp.UploadFile" /> or <see cref="F:System.Net.WebRequestMethods.Ftp.AppendFile" />.</exception>
		public override Stream GetRequestStream()
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, null, "GetRequestStream");
				NetEventSource.Info(this, FormattableStringFactory.Create("Method: {0}", new object[]
				{
					this._methodInfo.Method
				}), "GetRequestStream");
			}
			try
			{
				if (this._getRequestStreamStarted)
				{
					throw new InvalidOperationException("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress.");
				}
				this._getRequestStreamStarted = true;
				if (!this._methodInfo.IsUpload)
				{
					throw new ProtocolViolationException("Cannot send a content-body with this verb-type.");
				}
				this.CheckError();
				this._startTime = DateTime.UtcNow;
				this._remainingTimeout = this.Timeout;
				if (this.Timeout != -1)
				{
					this._remainingTimeout = this.Timeout - (int)(DateTime.UtcNow - this._startTime).TotalMilliseconds;
					if (this._remainingTimeout <= 0)
					{
						throw ExceptionHelper.TimeoutException;
					}
				}
				this.FinishRequestStage(FtpWebRequest.RequestStage.RequestStarted);
				this.SubmitRequest(false);
				this.FinishRequestStage(FtpWebRequest.RequestStage.WriteReady);
				this.CheckError();
				if (this._stream.CanTimeout)
				{
					this._stream.WriteTimeout = this.ReadWriteTimeout;
					this._stream.ReadTimeout = this.ReadWriteTimeout;
				}
			}
			catch (Exception message)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(this, message, "GetRequestStream");
				}
				throw;
			}
			finally
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, null, "GetRequestStream");
				}
			}
			return this._stream;
		}

		/// <summary>Begins asynchronously opening a request's content stream for writing.</summary>
		/// <param name="callback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the operation. This object is passed to the <paramref name="callback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> instance that indicates the status of the operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">A previous call to this method or <see cref="M:System.Net.FtpWebRequest.GetRequestStream" /> has not yet completed.</exception>
		/// <exception cref="T:System.Net.WebException">A connection to the FTP server could not be established.</exception>
		/// <exception cref="T:System.Net.ProtocolViolationException">The <see cref="P:System.Net.FtpWebRequest.Method" /> property is not set to <see cref="F:System.Net.WebRequestMethods.Ftp.UploadFile" />.</exception>
		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, null, "BeginGetRequestStream");
				NetEventSource.Info(this, FormattableStringFactory.Create("Method: {0}", new object[]
				{
					this._methodInfo.Method
				}), "BeginGetRequestStream");
			}
			ContextAwareResult contextAwareResult = null;
			try
			{
				if (this._getRequestStreamStarted)
				{
					throw new InvalidOperationException("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress.");
				}
				this._getRequestStreamStarted = true;
				if (!this._methodInfo.IsUpload)
				{
					throw new ProtocolViolationException("Cannot send a content-body with this verb-type.");
				}
				this.CheckError();
				this.FinishRequestStage(FtpWebRequest.RequestStage.RequestStarted);
				contextAwareResult = new ContextAwareResult(true, true, this, state, callback);
				object obj = contextAwareResult.StartPostingAsyncOp();
				lock (obj)
				{
					this._writeAsyncResult = contextAwareResult;
					this.SubmitRequest(true);
					contextAwareResult.FinishPostingAsyncOp();
					this.FinishRequestStage(FtpWebRequest.RequestStage.CheckForError);
				}
			}
			catch (Exception message)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(this, message, "BeginGetRequestStream");
				}
				throw;
			}
			finally
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, null, "BeginGetRequestStream");
				}
			}
			return contextAwareResult;
		}

		/// <summary>Ends a pending asynchronous operation started with <see cref="M:System.Net.FtpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)" />.</summary>
		/// <param name="asyncResult">The <see cref="T:System.IAsyncResult" /> object that was returned when the operation started.</param>
		/// <returns>A writable <see cref="T:System.IO.Stream" /> instance associated with this instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not obtained by calling <see cref="M:System.Net.FtpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">This method was already called for the operation identified by <paramref name="asyncResult" />.</exception>
		public override Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, null, "EndGetRequestStream");
			}
			Stream stream = null;
			try
			{
				if (asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
				if (lazyAsyncResult == null)
				{
					throw new ArgumentException("The IAsyncResult object was not returned from the corresponding asynchronous method on this class.", "asyncResult");
				}
				if (lazyAsyncResult.EndCalled)
				{
					throw new InvalidOperationException(SR.Format("{0} can only be called once for each asynchronous operation.", "EndGetResponse"));
				}
				lazyAsyncResult.InternalWaitForCompletion();
				lazyAsyncResult.EndCalled = true;
				this.CheckError();
				stream = this._stream;
				lazyAsyncResult.EndCalled = true;
				if (stream.CanTimeout)
				{
					stream.WriteTimeout = this.ReadWriteTimeout;
					stream.ReadTimeout = this.ReadWriteTimeout;
				}
			}
			catch (Exception message)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(this, message, "EndGetRequestStream");
				}
				throw;
			}
			finally
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, null, "EndGetRequestStream");
				}
			}
			return stream;
		}

		private void SubmitRequest(bool isAsync)
		{
			try
			{
				this._async = isAsync;
				for (;;)
				{
					FtpControlStream ftpControlStream = this._connection;
					if (ftpControlStream == null)
					{
						if (isAsync)
						{
							break;
						}
						ftpControlStream = this.CreateConnection();
						this._connection = ftpControlStream;
					}
					if (!isAsync && this.Timeout != -1)
					{
						this._remainingTimeout = this.Timeout - (int)(DateTime.UtcNow - this._startTime).TotalMilliseconds;
						if (this._remainingTimeout <= 0)
						{
							goto Block_6;
						}
					}
					if (NetEventSource.IsEnabled)
					{
						NetEventSource.Info(this, "Request being submitted", "SubmitRequest");
					}
					ftpControlStream.SetSocketTimeoutOption(this.RemainingTimeout);
					try
					{
						this.TimedSubmitRequestHelper(isAsync);
					}
					catch (Exception e)
					{
						if (this.AttemptedRecovery(e))
						{
							if (!isAsync && this.Timeout != -1)
							{
								this._remainingTimeout = this.Timeout - (int)(DateTime.UtcNow - this._startTime).TotalMilliseconds;
								if (this._remainingTimeout <= 0)
								{
									throw;
								}
							}
							continue;
						}
						throw;
					}
					goto IL_E9;
				}
				this.CreateConnectionAsync();
				return;
				Block_6:
				throw ExceptionHelper.TimeoutException;
				IL_E9:;
			}
			catch (WebException ex)
			{
				IOException ex2 = ex.InnerException as IOException;
				if (ex2 != null)
				{
					SocketException ex3 = ex2.InnerException as SocketException;
					if (ex3 != null && ex3.SocketErrorCode == SocketError.TimedOut)
					{
						this.SetException(new WebException("The operation has timed out.", WebExceptionStatus.Timeout));
					}
				}
				this.SetException(ex);
			}
			catch (Exception exception)
			{
				this.SetException(exception);
			}
		}

		private Exception TranslateConnectException(Exception e)
		{
			SocketException ex = e as SocketException;
			if (ex == null)
			{
				return e;
			}
			if (ex.SocketErrorCode == SocketError.HostNotFound)
			{
				return new WebException("The remote name could not be resolved", WebExceptionStatus.NameResolutionFailure);
			}
			return new WebException("Unable to connect to the remote server", WebExceptionStatus.ConnectFailure);
		}

		private void CreateConnectionAsync()
		{
			FtpWebRequest.<CreateConnectionAsync>d__86 <CreateConnectionAsync>d__;
			<CreateConnectionAsync>d__.<>4__this = this;
			<CreateConnectionAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CreateConnectionAsync>d__.<>1__state = -1;
			<CreateConnectionAsync>d__.<>t__builder.Start<FtpWebRequest.<CreateConnectionAsync>d__86>(ref <CreateConnectionAsync>d__);
		}

		private FtpControlStream CreateConnection()
		{
			string host = this._uri.Host;
			int port = this._uri.Port;
			TcpClient tcpClient = new TcpClient();
			try
			{
				tcpClient.Connect(host, port);
			}
			catch (Exception e)
			{
				throw this.TranslateConnectException(e);
			}
			return new FtpControlStream(tcpClient);
		}

		private Stream TimedSubmitRequestHelper(bool isAsync)
		{
			if (isAsync)
			{
				if (this._requestCompleteAsyncResult == null)
				{
					this._requestCompleteAsyncResult = new LazyAsyncResult(null, null, null);
				}
				return this._connection.SubmitRequest(this, true, true);
			}
			Stream stream = null;
			bool flag = false;
			TimerThread.Timer timer = this.TimerQueue.CreateTimer(this._timerCallback, null);
			try
			{
				stream = this._connection.SubmitRequest(this, false, true);
			}
			catch (Exception ex)
			{
				if ((!(ex is SocketException) && !(ex is ObjectDisposedException)) || !timer.HasExpired)
				{
					timer.Cancel();
					throw;
				}
				flag = true;
			}
			if (flag || !timer.Cancel())
			{
				this._timedOut = true;
				throw ExceptionHelper.TimeoutException;
			}
			if (stream != null)
			{
				object syncObject = this._syncObject;
				lock (syncObject)
				{
					if (this._aborted)
					{
						((ICloseEx)stream).CloseEx(CloseExState.Abort | CloseExState.Silent);
						this.CheckError();
						throw new InternalException();
					}
					this._stream = stream;
				}
			}
			return stream;
		}

		private void TimerCallback(TimerThread.Timer timer, int timeNoticed, object context)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, null, "TimerCallback");
			}
			FtpControlStream connection = this._connection;
			if (connection != null)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, "aborting connection", "TimerCallback");
				}
				connection.AbortConnect();
			}
		}

		private TimerThread.Queue TimerQueue
		{
			get
			{
				if (this._timerQueue == null)
				{
					this._timerQueue = TimerThread.GetOrCreateQueue(this.RemainingTimeout);
				}
				return this._timerQueue;
			}
		}

		private bool AttemptedRecovery(Exception e)
		{
			if (e is OutOfMemoryException || this._onceFailed || this._aborted || this._timedOut || this._connection == null || !this._connection.RecoverableFailure)
			{
				return false;
			}
			this._onceFailed = true;
			object syncObject = this._syncObject;
			lock (syncObject)
			{
				if (this._connection == null)
				{
					return false;
				}
				this._connection.CloseSocket();
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("Releasing connection: {0}", new object[]
					{
						this._connection
					}), "AttemptedRecovery");
				}
				this._connection = null;
			}
			return true;
		}

		private void SetException(Exception exception)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, null, "SetException");
			}
			if (exception is OutOfMemoryException)
			{
				this._exception = exception;
				throw exception;
			}
			FtpControlStream connection = this._connection;
			if (this._exception == null)
			{
				if (exception is WebException)
				{
					this.EnsureFtpWebResponse(exception);
					this._exception = new WebException(exception.Message, null, ((WebException)exception).Status, this._ftpWebResponse);
				}
				else if (exception is AuthenticationException || exception is SecurityException)
				{
					this._exception = exception;
				}
				else if (connection != null && connection.StatusCode != FtpStatusCode.Undefined)
				{
					this.EnsureFtpWebResponse(exception);
					this._exception = new WebException(SR.Format("The remote server returned an error: {0}.", connection.StatusLine), exception, WebExceptionStatus.ProtocolError, this._ftpWebResponse);
				}
				else
				{
					this._exception = new WebException(exception.Message, exception);
				}
				if (connection != null && this._ftpWebResponse != null)
				{
					this._ftpWebResponse.UpdateStatus(connection.StatusCode, connection.StatusLine, connection.ExitMessage);
				}
			}
		}

		private void CheckError()
		{
			if (this._exception != null)
			{
				ExceptionDispatchInfo.Throw(this._exception);
			}
		}

		internal void RequestCallback(object obj)
		{
			if (this._async)
			{
				this.AsyncRequestCallback(obj);
				return;
			}
			this.SyncRequestCallback(obj);
		}

		private void SyncRequestCallback(object obj)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, obj, "SyncRequestCallback");
			}
			FtpWebRequest.RequestStage stage = FtpWebRequest.RequestStage.CheckForError;
			try
			{
				bool flag = obj == null;
				Exception ex = obj as Exception;
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("exp:{0} completedRequest:{1}", new object[]
					{
						ex,
						flag
					}), "SyncRequestCallback");
				}
				if (ex != null)
				{
					this.SetException(ex);
				}
				else
				{
					if (!flag)
					{
						throw new InternalException();
					}
					FtpControlStream connection = this._connection;
					if (connection != null)
					{
						this.EnsureFtpWebResponse(null);
						this._ftpWebResponse.UpdateStatus(connection.StatusCode, connection.StatusLine, connection.ExitMessage);
					}
					stage = FtpWebRequest.RequestStage.ReleaseConnection;
				}
			}
			catch (Exception exception)
			{
				this.SetException(exception);
			}
			finally
			{
				this.FinishRequestStage(stage);
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, null, "SyncRequestCallback");
				}
				this.CheckError();
			}
		}

		private void AsyncRequestCallback(object obj)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, obj, "AsyncRequestCallback");
			}
			FtpWebRequest.RequestStage stage = FtpWebRequest.RequestStage.CheckForError;
			try
			{
				FtpControlStream ftpControlStream = obj as FtpControlStream;
				FtpDataStream ftpDataStream = obj as FtpDataStream;
				Exception ex = obj as Exception;
				bool flag = obj == null;
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("stream:{0} conn:{1} exp:{2} completedRequest:{3}", new object[]
					{
						ftpDataStream,
						ftpControlStream,
						ex,
						flag
					}), "AsyncRequestCallback");
				}
				for (;;)
				{
					if (ex != null)
					{
						if (this.AttemptedRecovery(ex))
						{
							ftpControlStream = this.CreateConnection();
							if (ftpControlStream == null)
							{
								break;
							}
							ex = null;
						}
						if (ex != null)
						{
							goto Block_9;
						}
					}
					if (ftpControlStream != null)
					{
						object syncObject = this._syncObject;
						lock (syncObject)
						{
							if (this._aborted)
							{
								if (NetEventSource.IsEnabled)
								{
									NetEventSource.Info(this, FormattableStringFactory.Create("Releasing connect:{0}", new object[]
									{
										ftpControlStream
									}), "AsyncRequestCallback");
								}
								ftpControlStream.CloseSocket();
								break;
							}
							this._connection = ftpControlStream;
							if (NetEventSource.IsEnabled)
							{
								NetEventSource.Associate(this, this._connection, "AsyncRequestCallback");
							}
						}
						try
						{
							ftpDataStream = (FtpDataStream)this.TimedSubmitRequestHelper(true);
						}
						catch (Exception ex)
						{
							continue;
						}
						break;
					}
					goto IL_12F;
				}
				return;
				Block_9:
				this.SetException(ex);
				return;
				IL_12F:
				if (ftpDataStream != null)
				{
					object syncObject = this._syncObject;
					lock (syncObject)
					{
						if (this._aborted)
						{
							((ICloseEx)ftpDataStream).CloseEx(CloseExState.Abort | CloseExState.Silent);
							goto IL_1CA;
						}
						this._stream = ftpDataStream;
					}
					ftpDataStream.SetSocketTimeoutOption(this.Timeout);
					this.EnsureFtpWebResponse(null);
					stage = (ftpDataStream.CanRead ? FtpWebRequest.RequestStage.ReadReady : FtpWebRequest.RequestStage.WriteReady);
				}
				else
				{
					if (!flag)
					{
						throw new InternalException();
					}
					ftpControlStream = this._connection;
					if (ftpControlStream != null)
					{
						this.EnsureFtpWebResponse(null);
						this._ftpWebResponse.UpdateStatus(ftpControlStream.StatusCode, ftpControlStream.StatusLine, ftpControlStream.ExitMessage);
					}
					stage = FtpWebRequest.RequestStage.ReleaseConnection;
				}
				IL_1CA:;
			}
			catch (Exception exception)
			{
				this.SetException(exception);
			}
			finally
			{
				this.FinishRequestStage(stage);
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, null, "AsyncRequestCallback");
				}
			}
		}

		private FtpWebRequest.RequestStage FinishRequestStage(FtpWebRequest.RequestStage stage)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("state:{0}", new object[]
				{
					stage
				}), "FinishRequestStage");
			}
			if (this._exception != null)
			{
				stage = FtpWebRequest.RequestStage.ReleaseConnection;
			}
			object syncObject = this._syncObject;
			FtpWebRequest.RequestStage requestStage;
			LazyAsyncResult writeAsyncResult;
			LazyAsyncResult readAsyncResult;
			FtpControlStream connection;
			lock (syncObject)
			{
				requestStage = this._requestStage;
				if (stage == FtpWebRequest.RequestStage.CheckForError)
				{
					return requestStage;
				}
				if (requestStage == FtpWebRequest.RequestStage.ReleaseConnection && stage == FtpWebRequest.RequestStage.ReleaseConnection)
				{
					return FtpWebRequest.RequestStage.ReleaseConnection;
				}
				if (stage > requestStage)
				{
					this._requestStage = stage;
				}
				if (stage <= FtpWebRequest.RequestStage.RequestStarted)
				{
					return requestStage;
				}
				writeAsyncResult = this._writeAsyncResult;
				readAsyncResult = this._readAsyncResult;
				connection = this._connection;
				if (stage == FtpWebRequest.RequestStage.ReleaseConnection)
				{
					if (this._exception == null && !this._aborted && requestStage != FtpWebRequest.RequestStage.ReadReady && this._methodInfo.IsDownload && !this._ftpWebResponse.IsFromCache)
					{
						return requestStage;
					}
					this._connection = null;
				}
			}
			FtpWebRequest.RequestStage result;
			try
			{
				if ((stage == FtpWebRequest.RequestStage.ReleaseConnection || requestStage == FtpWebRequest.RequestStage.ReleaseConnection) && connection != null)
				{
					try
					{
						if (this._exception != null)
						{
							connection.Abort(this._exception);
						}
					}
					finally
					{
						if (NetEventSource.IsEnabled)
						{
							NetEventSource.Info(this, FormattableStringFactory.Create("Releasing connection: {0}", new object[]
							{
								connection
							}), "FinishRequestStage");
						}
						connection.CloseSocket();
						if (this._async && this._requestCompleteAsyncResult != null)
						{
							this._requestCompleteAsyncResult.InvokeCallback();
						}
					}
				}
				result = requestStage;
			}
			finally
			{
				try
				{
					if (stage >= FtpWebRequest.RequestStage.WriteReady)
					{
						if (this._methodInfo.IsUpload && !this._getRequestStreamStarted)
						{
							if (this._stream != null)
							{
								this._stream.Close();
							}
						}
						else if (writeAsyncResult != null && !writeAsyncResult.InternalPeekCompleted)
						{
							writeAsyncResult.InvokeCallback();
						}
					}
				}
				finally
				{
					if (stage >= FtpWebRequest.RequestStage.ReadReady && readAsyncResult != null && !readAsyncResult.InternalPeekCompleted)
					{
						readAsyncResult.InvokeCallback();
					}
				}
			}
			return result;
		}

		/// <summary>Terminates an asynchronous FTP operation.</summary>
		public override void Abort()
		{
			if (this._aborted)
			{
				return;
			}
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, null, "Abort");
			}
			try
			{
				object syncObject = this._syncObject;
				Stream stream;
				FtpControlStream connection;
				lock (syncObject)
				{
					if (this._requestStage >= FtpWebRequest.RequestStage.ReleaseConnection)
					{
						return;
					}
					this._aborted = true;
					stream = this._stream;
					connection = this._connection;
					this._exception = ExceptionHelper.RequestAbortedException;
				}
				if (stream != null)
				{
					if (!(stream is ICloseEx))
					{
						NetEventSource.Fail(this, "The _stream member is not CloseEx hence the risk of connection been orphaned.", "Abort");
					}
					((ICloseEx)stream).CloseEx(CloseExState.Abort | CloseExState.Silent);
				}
				if (connection != null)
				{
					connection.Abort(ExceptionHelper.RequestAbortedException);
				}
			}
			catch (Exception message)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(this, message, "Abort");
				}
				throw;
			}
			finally
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Exit(this, null, "Abort");
				}
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies whether the control connection to the FTP server is closed after the request completes.</summary>
		/// <returns>
		///   <see langword="true" /> if the connection to the server should not be destroyed; otherwise, <see langword="false" />. The default value is <see langword="true" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		public bool KeepAlive
		{
			get
			{
				return true;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
			}
		}

		public override RequestCachePolicy CachePolicy
		{
			get
			{
				return FtpWebRequest.DefaultCachePolicy;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies the data type for file transfers.</summary>
		/// <returns>
		///   <see langword="true" /> to indicate to the server that the data to be transferred is binary; <see langword="false" /> to indicate that the data is text. The default value is <see langword="true" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		public bool UseBinary
		{
			get
			{
				return this._binary;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				this._binary = value;
			}
		}

		/// <summary>Gets or sets the behavior of a client application's data transfer process.</summary>
		/// <returns>
		///   <see langword="false" /> if the client application's data transfer process listens for a connection on the data port; otherwise, <see langword="true" /> if the client should initiate a connection on the data port. The default value is <see langword="true" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">A new value was specified for this property for a request that is already in progress.</exception>
		public bool UsePassive
		{
			get
			{
				return this._passive;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				this._passive = value;
			}
		}

		/// <summary>Gets or sets the certificates used for establishing an encrypted connection to the FTP server.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.X509Certificates.X509CertificateCollection" /> object that contains the client certificates.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value specified for a set operation is <see langword="null" />.</exception>
		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return LazyInitializer.EnsureInitialized<X509CertificateCollection>(ref this._clientCertificates, ref this._syncObject, () => new X509CertificateCollection());
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._clientCertificates = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> that specifies that an SSL connection should be used.</summary>
		/// <returns>
		///   <see langword="true" /> if control and data transmissions are encrypted; otherwise, <see langword="false" />. The default value is <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The connection to the FTP server has already been established.</exception>
		public bool EnableSsl
		{
			get
			{
				return this._enableSsl;
			}
			set
			{
				if (this.InUse)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				this._enableSsl = value;
			}
		}

		/// <summary>Gets an empty <see cref="T:System.Net.WebHeaderCollection" /> object.</summary>
		/// <returns>An empty <see cref="T:System.Net.WebHeaderCollection" /> object.</returns>
		public override WebHeaderCollection Headers
		{
			get
			{
				if (this._ftpRequestHeaders == null)
				{
					this._ftpRequestHeaders = new WebHeaderCollection();
				}
				return this._ftpRequestHeaders;
			}
			set
			{
				this._ftpRequestHeaders = value;
			}
		}

		/// <summary>Always throws a <see cref="T:System.NotSupportedException" />.</summary>
		/// <returns>Always throws a <see cref="T:System.NotSupportedException" />.</returns>
		/// <exception cref="T:System.NotSupportedException">Content type information is not supported for FTP.</exception>
		public override string ContentType
		{
			get
			{
				throw ExceptionHelper.PropertyNotSupportedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotSupportedException;
			}
		}

		/// <summary>Always throws a <see cref="T:System.NotSupportedException" />.</summary>
		/// <returns>Always throws a <see cref="T:System.NotSupportedException" />.</returns>
		/// <exception cref="T:System.NotSupportedException">Default credentials are not supported for FTP.</exception>
		public override bool UseDefaultCredentials
		{
			get
			{
				throw ExceptionHelper.PropertyNotSupportedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotSupportedException;
			}
		}

		/// <summary>Always throws a <see cref="T:System.NotSupportedException" />.</summary>
		/// <returns>Always throws a <see cref="T:System.NotSupportedException" />.</returns>
		/// <exception cref="T:System.NotSupportedException">Preauthentication is not supported for FTP.</exception>
		public override bool PreAuthenticate
		{
			get
			{
				throw ExceptionHelper.PropertyNotSupportedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotSupportedException;
			}
		}

		private bool InUse
		{
			get
			{
				return this._getRequestStreamStarted || this._getResponseStarted;
			}
		}

		private void EnsureFtpWebResponse(Exception exception)
		{
			if (this._ftpWebResponse == null || (this._ftpWebResponse.GetResponseStream() is FtpWebResponse.EmptyStream && this._stream != null))
			{
				object syncObject = this._syncObject;
				lock (syncObject)
				{
					if (this._ftpWebResponse == null || (this._ftpWebResponse.GetResponseStream() is FtpWebResponse.EmptyStream && this._stream != null))
					{
						Stream stream = this._stream;
						if (this._methodInfo.IsUpload)
						{
							stream = null;
						}
						if (this._stream != null && this._stream.CanRead && this._stream.CanTimeout)
						{
							this._stream.ReadTimeout = this.ReadWriteTimeout;
							this._stream.WriteTimeout = this.ReadWriteTimeout;
						}
						FtpControlStream connection = this._connection;
						long num = (connection != null) ? connection.ContentLength : -1L;
						if (stream == null && num < 0L)
						{
							num = 0L;
						}
						if (this._ftpWebResponse != null)
						{
							this._ftpWebResponse.SetResponseStream(stream);
						}
						else if (connection != null)
						{
							this._ftpWebResponse = new FtpWebResponse(stream, num, connection.ResponseUri, connection.StatusCode, connection.StatusLine, connection.LastModified, connection.BannerMessage, connection.WelcomeMessage, connection.ExitMessage);
						}
						else
						{
							this._ftpWebResponse = new FtpWebResponse(stream, -1L, this._uri, FtpStatusCode.Undefined, null, DateTime.Now, null, null, null);
						}
					}
				}
			}
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("Returns {0} with stream {1}", new object[]
				{
					this._ftpWebResponse,
					this._ftpWebResponse._responseStream
				}), "EnsureFtpWebResponse");
			}
		}

		internal void DataStreamClosed(CloseExState closeState)
		{
			if ((closeState & CloseExState.Abort) == CloseExState.Normal)
			{
				if (this._async)
				{
					this._requestCompleteAsyncResult.InternalWaitForCompletion();
					this.CheckError();
					return;
				}
				if (this._connection != null)
				{
					this._connection.CheckContinuePipeline();
					return;
				}
			}
			else
			{
				FtpControlStream connection = this._connection;
				if (connection != null)
				{
					connection.Abort(ExceptionHelper.RequestAbortedException);
				}
			}
		}

		internal FtpWebRequest()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private object _syncObject;

		private ICredentials _authInfo;

		private readonly Uri _uri;

		private FtpMethodInfo _methodInfo;

		private string _renameTo;

		private bool _getRequestStreamStarted;

		private bool _getResponseStarted;

		private DateTime _startTime;

		private int _timeout;

		private int _remainingTimeout;

		private long _contentLength;

		private long _contentOffset;

		private X509CertificateCollection _clientCertificates;

		private bool _passive;

		private bool _binary;

		private string _connectionGroupName;

		private ServicePoint _servicePoint;

		private bool _async;

		private bool _aborted;

		private bool _timedOut;

		private Exception _exception;

		private TimerThread.Queue _timerQueue;

		private TimerThread.Callback _timerCallback;

		private bool _enableSsl;

		private FtpControlStream _connection;

		private Stream _stream;

		private FtpWebRequest.RequestStage _requestStage;

		private bool _onceFailed;

		private WebHeaderCollection _ftpRequestHeaders;

		private FtpWebResponse _ftpWebResponse;

		private int _readWriteTimeout;

		private ContextAwareResult _writeAsyncResult;

		private LazyAsyncResult _readAsyncResult;

		private LazyAsyncResult _requestCompleteAsyncResult;

		private static readonly NetworkCredential s_defaultFtpNetworkCredential = new NetworkCredential("anonymous", "anonymous@", string.Empty);

		private const int s_DefaultTimeout = 100000;

		private static readonly TimerThread.Queue s_DefaultTimerQueue = TimerThread.GetOrCreateQueue(100000);

		private enum RequestStage
		{
			CheckForError,
			RequestStarted,
			WriteReady,
			ReadReady,
			ReleaseConnection
		}
	}
}
