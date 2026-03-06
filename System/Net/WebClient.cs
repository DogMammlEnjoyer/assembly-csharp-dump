using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	/// <summary>Provides common methods for sending data to and receiving data from a resource identified by a URI.</summary>
	public class WebClient : Component
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.WebClient" /> class.</summary>
		public WebClient()
		{
			if (base.GetType() == typeof(WebClient))
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>Occurs when an asynchronous resource-download operation completes.</summary>
		public event DownloadStringCompletedEventHandler DownloadStringCompleted;

		/// <summary>Occurs when an asynchronous data download operation completes.</summary>
		public event DownloadDataCompletedEventHandler DownloadDataCompleted;

		/// <summary>Occurs when an asynchronous file download operation completes.</summary>
		public event AsyncCompletedEventHandler DownloadFileCompleted;

		/// <summary>Occurs when an asynchronous string-upload operation completes.</summary>
		public event UploadStringCompletedEventHandler UploadStringCompleted;

		/// <summary>Occurs when an asynchronous data-upload operation completes.</summary>
		public event UploadDataCompletedEventHandler UploadDataCompleted;

		/// <summary>Occurs when an asynchronous file-upload operation completes.</summary>
		public event UploadFileCompletedEventHandler UploadFileCompleted;

		/// <summary>Occurs when an asynchronous upload of a name/value collection completes.</summary>
		public event UploadValuesCompletedEventHandler UploadValuesCompleted;

		/// <summary>Occurs when an asynchronous operation to open a stream containing a resource completes.</summary>
		public event OpenReadCompletedEventHandler OpenReadCompleted;

		/// <summary>Occurs when an asynchronous operation to open a stream to write data to a resource completes.</summary>
		public event OpenWriteCompletedEventHandler OpenWriteCompleted;

		/// <summary>Occurs when an asynchronous download operation successfully transfers some or all of the data.</summary>
		public event DownloadProgressChangedEventHandler DownloadProgressChanged;

		/// <summary>Occurs when an asynchronous upload operation successfully transfers some or all of the data.</summary>
		public event UploadProgressChangedEventHandler UploadProgressChanged;

		/// <summary>Raises the <see cref="E:System.Net.WebClient.DownloadStringCompleted" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.DownloadStringCompletedEventArgs" /> object containing event data.</param>
		protected virtual void OnDownloadStringCompleted(DownloadStringCompletedEventArgs e)
		{
			DownloadStringCompletedEventHandler downloadStringCompleted = this.DownloadStringCompleted;
			if (downloadStringCompleted == null)
			{
				return;
			}
			downloadStringCompleted(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.DownloadDataCompleted" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.DownloadDataCompletedEventArgs" /> object that contains event data.</param>
		protected virtual void OnDownloadDataCompleted(DownloadDataCompletedEventArgs e)
		{
			DownloadDataCompletedEventHandler downloadDataCompleted = this.DownloadDataCompleted;
			if (downloadDataCompleted == null)
			{
				return;
			}
			downloadDataCompleted(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.DownloadFileCompleted" /> event.</summary>
		/// <param name="e">An <see cref="T:System.ComponentModel.AsyncCompletedEventArgs" /> object containing event data.</param>
		protected virtual void OnDownloadFileCompleted(AsyncCompletedEventArgs e)
		{
			AsyncCompletedEventHandler downloadFileCompleted = this.DownloadFileCompleted;
			if (downloadFileCompleted == null)
			{
				return;
			}
			downloadFileCompleted(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.DownloadProgressChanged" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.DownloadProgressChangedEventArgs" /> object containing event data.</param>
		protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
		{
			DownloadProgressChangedEventHandler downloadProgressChanged = this.DownloadProgressChanged;
			if (downloadProgressChanged == null)
			{
				return;
			}
			downloadProgressChanged(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.UploadStringCompleted" /> event.</summary>
		/// <param name="e">An <see cref="T:System.Net.UploadStringCompletedEventArgs" /> object containing event data.</param>
		protected virtual void OnUploadStringCompleted(UploadStringCompletedEventArgs e)
		{
			UploadStringCompletedEventHandler uploadStringCompleted = this.UploadStringCompleted;
			if (uploadStringCompleted == null)
			{
				return;
			}
			uploadStringCompleted(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.UploadDataCompleted" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.UploadDataCompletedEventArgs" /> object containing event data.</param>
		protected virtual void OnUploadDataCompleted(UploadDataCompletedEventArgs e)
		{
			UploadDataCompletedEventHandler uploadDataCompleted = this.UploadDataCompleted;
			if (uploadDataCompleted == null)
			{
				return;
			}
			uploadDataCompleted(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.UploadFileCompleted" /> event.</summary>
		/// <param name="e">An <see cref="T:System.Net.UploadFileCompletedEventArgs" /> object containing event data.</param>
		protected virtual void OnUploadFileCompleted(UploadFileCompletedEventArgs e)
		{
			UploadFileCompletedEventHandler uploadFileCompleted = this.UploadFileCompleted;
			if (uploadFileCompleted == null)
			{
				return;
			}
			uploadFileCompleted(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.UploadValuesCompleted" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.UploadValuesCompletedEventArgs" /> object containing event data.</param>
		protected virtual void OnUploadValuesCompleted(UploadValuesCompletedEventArgs e)
		{
			UploadValuesCompletedEventHandler uploadValuesCompleted = this.UploadValuesCompleted;
			if (uploadValuesCompleted == null)
			{
				return;
			}
			uploadValuesCompleted(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.UploadProgressChanged" /> event.</summary>
		/// <param name="e">An <see cref="T:System.Net.UploadProgressChangedEventArgs" /> object containing event data.</param>
		protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
		{
			UploadProgressChangedEventHandler uploadProgressChanged = this.UploadProgressChanged;
			if (uploadProgressChanged == null)
			{
				return;
			}
			uploadProgressChanged(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.OpenReadCompleted" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.OpenReadCompletedEventArgs" /> object containing event data.</param>
		protected virtual void OnOpenReadCompleted(OpenReadCompletedEventArgs e)
		{
			OpenReadCompletedEventHandler openReadCompleted = this.OpenReadCompleted;
			if (openReadCompleted == null)
			{
				return;
			}
			openReadCompleted(this, e);
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.OpenWriteCompleted" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.OpenWriteCompletedEventArgs" /> object containing event data.</param>
		protected virtual void OnOpenWriteCompleted(OpenWriteCompletedEventArgs e)
		{
			OpenWriteCompletedEventHandler openWriteCompleted = this.OpenWriteCompleted;
			if (openWriteCompleted == null)
			{
				return;
			}
			openWriteCompleted(this, e);
		}

		private void StartOperation()
		{
			if (Interlocked.Increment(ref this._callNesting) > 1)
			{
				this.EndOperation();
				throw new NotSupportedException("WebClient does not support concurrent I/O operations.");
			}
			this._contentLength = -1L;
			this._webResponse = null;
			this._webRequest = null;
			this._method = null;
			this._canceled = false;
			WebClient.ProgressData progress = this._progress;
			if (progress == null)
			{
				return;
			}
			progress.Reset();
		}

		private AsyncOperation StartAsyncOperation(object userToken)
		{
			if (!this._initWebClientAsync)
			{
				this._openReadOperationCompleted = delegate(object arg)
				{
					this.OnOpenReadCompleted((OpenReadCompletedEventArgs)arg);
				};
				this._openWriteOperationCompleted = delegate(object arg)
				{
					this.OnOpenWriteCompleted((OpenWriteCompletedEventArgs)arg);
				};
				this._downloadStringOperationCompleted = delegate(object arg)
				{
					this.OnDownloadStringCompleted((DownloadStringCompletedEventArgs)arg);
				};
				this._downloadDataOperationCompleted = delegate(object arg)
				{
					this.OnDownloadDataCompleted((DownloadDataCompletedEventArgs)arg);
				};
				this._downloadFileOperationCompleted = delegate(object arg)
				{
					this.OnDownloadFileCompleted((AsyncCompletedEventArgs)arg);
				};
				this._uploadStringOperationCompleted = delegate(object arg)
				{
					this.OnUploadStringCompleted((UploadStringCompletedEventArgs)arg);
				};
				this._uploadDataOperationCompleted = delegate(object arg)
				{
					this.OnUploadDataCompleted((UploadDataCompletedEventArgs)arg);
				};
				this._uploadFileOperationCompleted = delegate(object arg)
				{
					this.OnUploadFileCompleted((UploadFileCompletedEventArgs)arg);
				};
				this._uploadValuesOperationCompleted = delegate(object arg)
				{
					this.OnUploadValuesCompleted((UploadValuesCompletedEventArgs)arg);
				};
				this._reportDownloadProgressChanged = delegate(object arg)
				{
					this.OnDownloadProgressChanged((DownloadProgressChangedEventArgs)arg);
				};
				this._reportUploadProgressChanged = delegate(object arg)
				{
					this.OnUploadProgressChanged((UploadProgressChangedEventArgs)arg);
				};
				this._progress = new WebClient.ProgressData();
				this._initWebClientAsync = true;
			}
			AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(userToken);
			this.StartOperation();
			this._asyncOp = asyncOperation;
			return asyncOperation;
		}

		private void EndOperation()
		{
			Interlocked.Decrement(ref this._callNesting);
		}

		/// <summary>Gets or sets the <see cref="T:System.Text.Encoding" /> used to upload and download strings.</summary>
		/// <returns>A <see cref="T:System.Text.Encoding" /> that is used to encode strings. The default value of this property is the encoding returned by <see cref="P:System.Text.Encoding.Default" />.</returns>
		public Encoding Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				WebClient.ThrowIfNull(value, "Encoding");
				this._encoding = value;
			}
		}

		/// <summary>Gets or sets the base URI for requests made by a <see cref="T:System.Net.WebClient" />.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the base URI for requests made by a <see cref="T:System.Net.WebClient" /> or <see cref="F:System.String.Empty" /> if no base address has been specified.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <see cref="P:System.Net.WebClient.BaseAddress" /> is set to an invalid URI. The inner exception may contain information that will help you locate the error.</exception>
		public string BaseAddress
		{
			get
			{
				if (!(this._baseAddress != null))
				{
					return string.Empty;
				}
				return this._baseAddress.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this._baseAddress = null;
					return;
				}
				try
				{
					this._baseAddress = new Uri(value);
				}
				catch (UriFormatException innerException)
				{
					throw new ArgumentException("The specified value is not a valid base address.", "value", innerException);
				}
			}
		}

		/// <summary>Gets or sets the network credentials that are sent to the host and used to authenticate the request.</summary>
		/// <returns>An <see cref="T:System.Net.ICredentials" /> containing the authentication credentials for the request. The default is <see langword="null" />.</returns>
		public ICredentials Credentials
		{
			get
			{
				return this._credentials;
			}
			set
			{
				this._credentials = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that controls whether the <see cref="P:System.Net.CredentialCache.DefaultCredentials" /> are sent with requests.</summary>
		/// <returns>
		///   <see langword="true" /> if the default credentials are used; otherwise <see langword="false" />. The default value is <see langword="false" />.</returns>
		public bool UseDefaultCredentials
		{
			get
			{
				return this._credentials == CredentialCache.DefaultCredentials;
			}
			set
			{
				this._credentials = (value ? CredentialCache.DefaultCredentials : null);
			}
		}

		/// <summary>Gets or sets a collection of header name/value pairs associated with the request.</summary>
		/// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> containing header name/value pairs associated with this request.</returns>
		public WebHeaderCollection Headers
		{
			get
			{
				WebHeaderCollection result;
				if ((result = this._headers) == null)
				{
					result = (this._headers = new WebHeaderCollection());
				}
				return result;
			}
			set
			{
				this._headers = value;
			}
		}

		/// <summary>Gets or sets a collection of query name/value pairs associated with the request.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.NameValueCollection" /> that contains query name/value pairs associated with the request. If no pairs are associated with the request, the value is an empty <see cref="T:System.Collections.Specialized.NameValueCollection" />.</returns>
		public NameValueCollection QueryString
		{
			get
			{
				NameValueCollection result;
				if ((result = this._requestParameters) == null)
				{
					result = (this._requestParameters = new NameValueCollection());
				}
				return result;
			}
			set
			{
				this._requestParameters = value;
			}
		}

		/// <summary>Gets a collection of header name/value pairs associated with the response.</summary>
		/// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> containing header name/value pairs associated with the response, or <see langword="null" /> if no response has been received.</returns>
		public WebHeaderCollection ResponseHeaders
		{
			get
			{
				WebResponse webResponse = this._webResponse;
				if (webResponse == null)
				{
					return null;
				}
				return webResponse.Headers;
			}
		}

		/// <summary>Gets or sets the proxy used by this <see cref="T:System.Net.WebClient" /> object.</summary>
		/// <returns>An <see cref="T:System.Net.IWebProxy" /> instance used to send requests.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <see cref="P:System.Net.WebClient.Proxy" /> is set to <see langword="null" />.</exception>
		public IWebProxy Proxy
		{
			get
			{
				if (!this._proxySet)
				{
					return WebRequest.DefaultWebProxy;
				}
				return this._proxy;
			}
			set
			{
				this._proxy = value;
				this._proxySet = true;
			}
		}

		/// <summary>Gets or sets the application's cache policy for any resources obtained by this WebClient instance using <see cref="T:System.Net.WebRequest" /> objects.</summary>
		/// <returns>A <see cref="T:System.Net.Cache.RequestCachePolicy" /> object that represents the application's caching requirements.</returns>
		public RequestCachePolicy CachePolicy { get; set; }

		/// <summary>Gets whether a Web request is in progress.</summary>
		/// <returns>
		///   <see langword="true" /> if the Web request is still in progress; otherwise <see langword="false" />.</returns>
		public bool IsBusy
		{
			get
			{
				return this._asyncOp != null;
			}
		}

		/// <summary>Returns a <see cref="T:System.Net.WebRequest" /> object for the specified resource.</summary>
		/// <param name="address">A <see cref="T:System.Uri" /> that identifies the resource to request.</param>
		/// <returns>A new <see cref="T:System.Net.WebRequest" /> object for the specified resource.</returns>
		protected virtual WebRequest GetWebRequest(Uri address)
		{
			WebRequest webRequest = WebRequest.Create(address);
			this.CopyHeadersTo(webRequest);
			if (this.Credentials != null)
			{
				webRequest.Credentials = this.Credentials;
			}
			if (this._method != null)
			{
				webRequest.Method = this._method;
			}
			if (this._contentLength != -1L)
			{
				webRequest.ContentLength = this._contentLength;
			}
			if (this._proxySet)
			{
				webRequest.Proxy = this._proxy;
			}
			if (this.CachePolicy != null)
			{
				webRequest.CachePolicy = this.CachePolicy;
			}
			return webRequest;
		}

		/// <summary>Returns the <see cref="T:System.Net.WebResponse" /> for the specified <see cref="T:System.Net.WebRequest" />.</summary>
		/// <param name="request">A <see cref="T:System.Net.WebRequest" /> that is used to obtain the response.</param>
		/// <returns>A <see cref="T:System.Net.WebResponse" /> containing the response for the specified <see cref="T:System.Net.WebRequest" />.</returns>
		protected virtual WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse response = request.GetResponse();
			this._webResponse = response;
			return response;
		}

		/// <summary>Returns the <see cref="T:System.Net.WebResponse" /> for the specified <see cref="T:System.Net.WebRequest" /> using the specified <see cref="T:System.IAsyncResult" />.</summary>
		/// <param name="request">A <see cref="T:System.Net.WebRequest" /> that is used to obtain the response.</param>
		/// <param name="result">An <see cref="T:System.IAsyncResult" /> object obtained from a previous call to <see cref="M:System.Net.WebRequest.BeginGetResponse(System.AsyncCallback,System.Object)" /> .</param>
		/// <returns>A <see cref="T:System.Net.WebResponse" /> containing the response for the specified <see cref="T:System.Net.WebRequest" />.</returns>
		protected virtual WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
		{
			WebResponse webResponse = request.EndGetResponse(result);
			this._webResponse = webResponse;
			return webResponse;
		}

		private Task<WebResponse> GetWebResponseTaskAsync(WebRequest request)
		{
			WebClient.<GetWebResponseTaskAsync>d__112 <GetWebResponseTaskAsync>d__;
			<GetWebResponseTaskAsync>d__.<>4__this = this;
			<GetWebResponseTaskAsync>d__.request = request;
			<GetWebResponseTaskAsync>d__.<>t__builder = AsyncTaskMethodBuilder<WebResponse>.Create();
			<GetWebResponseTaskAsync>d__.<>1__state = -1;
			<GetWebResponseTaskAsync>d__.<>t__builder.Start<WebClient.<GetWebResponseTaskAsync>d__112>(ref <GetWebResponseTaskAsync>d__);
			return <GetWebResponseTaskAsync>d__.<>t__builder.Task;
		}

		/// <summary>Downloads the resource as a <see cref="T:System.Byte" /> array from the URI specified.</summary>
		/// <param name="address">The URI from which to download data.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the downloaded resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading data.</exception>
		/// <exception cref="T:System.NotSupportedException">The method has been called simultaneously on multiple threads.</exception>
		public byte[] DownloadData(string address)
		{
			return this.DownloadData(this.GetUri(address));
		}

		/// <summary>Downloads the resource as a <see cref="T:System.Byte" /> array from the URI specified.</summary>
		/// <param name="address">The URI represented by the <see cref="T:System.Uri" /> object, from which to download data.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the downloaded resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		public byte[] DownloadData(Uri address)
		{
			WebClient.ThrowIfNull(address, "address");
			this.StartOperation();
			byte[] result;
			try
			{
				WebRequest webRequest;
				result = this.DownloadDataInternal(address, out webRequest);
			}
			finally
			{
				this.EndOperation();
			}
			return result;
		}

		private byte[] DownloadDataInternal(Uri address, out WebRequest request)
		{
			request = null;
			byte[] result;
			try
			{
				request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				result = this.DownloadBits(request, new ChunkedMemoryStream());
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			return result;
		}

		/// <summary>Downloads the resource with the specified URI to a local file.</summary>
		/// <param name="address">The URI from which to download data.</param>
		/// <param name="fileName">The name of the local file that is to receive the data.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="filename" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.  
		///  -or-  
		///  The file does not exist.  
		///  -or- An error occurred while downloading data.</exception>
		/// <exception cref="T:System.NotSupportedException">The method has been called simultaneously on multiple threads.</exception>
		public void DownloadFile(string address, string fileName)
		{
			this.DownloadFile(this.GetUri(address), fileName);
		}

		/// <summary>Downloads the resource with the specified URI to a local file.</summary>
		/// <param name="address">The URI specified as a <see cref="T:System.String" />, from which to download data.</param>
		/// <param name="fileName">The name of the local file that is to receive the data.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="filename" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.  
		///  -or-  
		///  The file does not exist.  
		///  -or-  
		///  An error occurred while downloading data.</exception>
		/// <exception cref="T:System.NotSupportedException">The method has been called simultaneously on multiple threads.</exception>
		public void DownloadFile(Uri address, string fileName)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(fileName, "fileName");
			WebRequest request = null;
			FileStream fileStream = null;
			bool flag = false;
			this.StartOperation();
			try
			{
				fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
				request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				this.DownloadBits(request, fileStream);
				flag = true;
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
					if (!flag)
					{
						File.Delete(fileName);
					}
				}
				this.EndOperation();
			}
		}

		/// <summary>Opens a readable stream for the data downloaded from a resource with the URI specified as a <see cref="T:System.String" />.</summary>
		/// <param name="address">The URI specified as a <see cref="T:System.String" /> from which to download data.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> used to read data from a resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading data.</exception>
		public Stream OpenRead(string address)
		{
			return this.OpenRead(this.GetUri(address));
		}

		/// <summary>Opens a readable stream for the data downloaded from a resource with the URI specified as a <see cref="T:System.Uri" /></summary>
		/// <param name="address">The URI specified as a <see cref="T:System.Uri" /> from which to download data.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> used to read data from a resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading data.</exception>
		public Stream OpenRead(Uri address)
		{
			WebClient.ThrowIfNull(address, "address");
			WebRequest request = null;
			this.StartOperation();
			Stream responseStream;
			try
			{
				request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				responseStream = (this._webResponse = this.GetWebResponse(request)).GetResponseStream();
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				this.EndOperation();
			}
			return responseStream;
		}

		/// <summary>Opens a stream for writing data to the specified resource.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> used to write data to the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Stream OpenWrite(string address)
		{
			return this.OpenWrite(this.GetUri(address), null);
		}

		/// <summary>Opens a stream for writing data to the specified resource.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> used to write data to the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Stream OpenWrite(Uri address)
		{
			return this.OpenWrite(address, null);
		}

		/// <summary>Opens a stream for writing data to the specified resource, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> used to write data to the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Stream OpenWrite(string address, string method)
		{
			return this.OpenWrite(this.GetUri(address), method);
		}

		/// <summary>Opens a stream for writing data to the specified resource, by using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <returns>A <see cref="T:System.IO.Stream" /> used to write data to the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Stream OpenWrite(Uri address, string method)
		{
			WebClient.ThrowIfNull(address, "address");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			WebRequest webRequest = null;
			this.StartOperation();
			Stream result;
			try
			{
				this._method = method;
				webRequest = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				result = new WebClient.WebClientWriteStream(webRequest.GetRequestStream(), webRequest, this);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(webRequest);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				this.EndOperation();
			}
			return result;
		}

		/// <summary>Uploads a data buffer to a resource identified by a URI.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="data" /> is <see langword="null" />.  
		///  -or-  
		///  An error occurred while sending the data.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public byte[] UploadData(string address, byte[] data)
		{
			return this.UploadData(this.GetUri(address), null, data);
		}

		/// <summary>Uploads a data buffer to a resource identified by a URI.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="data" /> is <see langword="null" />.  
		///  -or-  
		///  An error occurred while sending the data.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public byte[] UploadData(Uri address, byte[] data)
		{
			return this.UploadData(address, null, data);
		}

		/// <summary>Uploads a data buffer to the specified resource, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The HTTP method used to send the data to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="data" /> is <see langword="null" />.  
		///  -or-  
		///  An error occurred while uploading the data.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public byte[] UploadData(string address, string method, byte[] data)
		{
			return this.UploadData(this.GetUri(address), method, data);
		}

		/// <summary>Uploads a data buffer to the specified resource, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The HTTP method used to send the data to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="data" /> is <see langword="null" />.  
		///  -or-  
		///  An error occurred while uploading the data.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public byte[] UploadData(Uri address, string method, byte[] data)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			this.StartOperation();
			byte[] result;
			try
			{
				WebRequest webRequest;
				result = this.UploadDataInternal(address, method, data, out webRequest);
			}
			finally
			{
				this.EndOperation();
			}
			return result;
		}

		private byte[] UploadDataInternal(Uri address, string method, byte[] data, out WebRequest request)
		{
			request = null;
			byte[] result;
			try
			{
				this._method = method;
				this._contentLength = (long)data.Length;
				request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				result = this.UploadBits(request, null, data, 0, null, null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			return result;
		}

		private void OpenFileInternal(bool needsHeaderAndBoundary, string fileName, ref FileStream fs, ref byte[] buffer, ref byte[] formHeaderBytes, ref byte[] boundaryBytes)
		{
			fileName = Path.GetFullPath(fileName);
			WebHeaderCollection headers = this.Headers;
			string text = headers["Content-Type"];
			if (text == null)
			{
				text = "application/octet-stream";
			}
			else if (text.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase))
			{
				throw new WebException("The Content-Type header cannot be set to a multipart type for this request.");
			}
			fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			int num = 8192;
			this._contentLength = -1L;
			if (string.Equals(this._method, "POST", StringComparison.Ordinal))
			{
				if (needsHeaderAndBoundary)
				{
					string text2 = "---------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
					headers["Content-Type"] = "multipart/form-data; boundary=" + text2;
					string s = string.Concat(new string[]
					{
						"--",
						text2,
						"\r\nContent-Disposition: form-data; name=\"file\"; filename=\"",
						Path.GetFileName(fileName),
						"\"\r\nContent-Type: ",
						text,
						"\r\n\r\n"
					});
					formHeaderBytes = Encoding.UTF8.GetBytes(s);
					boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + text2 + "--\r\n");
				}
				else
				{
					formHeaderBytes = Array.Empty<byte>();
					boundaryBytes = Array.Empty<byte>();
				}
				if (fs.CanSeek)
				{
					this._contentLength = fs.Length + (long)formHeaderBytes.Length + (long)boundaryBytes.Length;
					num = (int)Math.Min(8192L, fs.Length);
				}
			}
			else
			{
				headers["Content-Type"] = text;
				formHeaderBytes = null;
				boundaryBytes = null;
				if (fs.CanSeek)
				{
					this._contentLength = fs.Length;
					num = (int)Math.Min(8192L, fs.Length);
				}
			}
			buffer = new byte[num];
		}

		/// <summary>Uploads the specified local file to a resource with the specified URI.</summary>
		/// <param name="address">The URI of the resource to receive the file. For example, ftp://localhost/samplefile.txt.</param>
		/// <param name="fileName">The file to send to the resource. For example, "samplefile.txt".</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid characters, or does not exist.  
		///  -or-  
		///  An error occurred while uploading the file.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public byte[] UploadFile(string address, string fileName)
		{
			return this.UploadFile(this.GetUri(address), fileName);
		}

		/// <summary>Uploads the specified local file to a resource with the specified URI.</summary>
		/// <param name="address">The URI of the resource to receive the file. For example, ftp://localhost/samplefile.txt.</param>
		/// <param name="fileName">The file to send to the resource. For example, "samplefile.txt".</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid characters, or does not exist.  
		///  -or-  
		///  An error occurred while uploading the file.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public byte[] UploadFile(Uri address, string fileName)
		{
			return this.UploadFile(address, null, fileName);
		}

		/// <summary>Uploads the specified local file to the specified resource, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the file.</param>
		/// <param name="method">The method used to send the file to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="fileName">The file to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid characters, or does not exist.  
		///  -or-  
		///  An error occurred while uploading the file.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public byte[] UploadFile(string address, string method, string fileName)
		{
			return this.UploadFile(this.GetUri(address), method, fileName);
		}

		/// <summary>Uploads the specified local file to the specified resource, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the file.</param>
		/// <param name="method">The method used to send the file to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="fileName">The file to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid characters, or does not exist.  
		///  -or-  
		///  An error occurred while uploading the file.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public byte[] UploadFile(Uri address, string method, string fileName)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(fileName, "fileName");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			FileStream fileStream = null;
			WebRequest request = null;
			this.StartOperation();
			byte[] result;
			try
			{
				this._method = method;
				byte[] header = null;
				byte[] footer = null;
				byte[] buffer = null;
				Uri uri = this.GetUri(address);
				bool needsHeaderAndBoundary = uri.Scheme != Uri.UriSchemeFile;
				this.OpenFileInternal(needsHeaderAndBoundary, fileName, ref fileStream, ref buffer, ref header, ref footer);
				request = (this._webRequest = this.GetWebRequest(uri));
				result = this.UploadBits(request, fileStream, buffer, 0, header, footer);
			}
			catch (Exception ex)
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				if (ex is OutOfMemoryException)
				{
					throw;
				}
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				this.EndOperation();
			}
			return result;
		}

		private byte[] GetValuesToUpload(NameValueCollection data)
		{
			WebHeaderCollection headers = this.Headers;
			string text = headers["Content-Type"];
			if (text != null && !string.Equals(text, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
			{
				throw new WebException("The Content-Type header cannot be changed from its default value for this request.");
			}
			headers["Content-Type"] = "application/x-www-form-urlencoded";
			string value = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text2 in data.AllKeys)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(WebClient.UrlEncode(text2));
				stringBuilder.Append('=');
				stringBuilder.Append(WebClient.UrlEncode(data[text2]));
				value = "&";
			}
			byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			this._contentLength = (long)bytes.Length;
			return bytes;
		}

		/// <summary>Uploads the specified name/value collection to the resource identified by the specified URI.</summary>
		/// <param name="address">The URI of the resource to receive the collection.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="data" /> is <see langword="null" />.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  The <see langword="Content-type" /> header is not <see langword="null" /> or "application/x-www-form-urlencoded".</exception>
		public byte[] UploadValues(string address, NameValueCollection data)
		{
			return this.UploadValues(this.GetUri(address), null, data);
		}

		/// <summary>Uploads the specified name/value collection to the resource identified by the specified URI.</summary>
		/// <param name="address">The URI of the resource to receive the collection.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="data" /> is <see langword="null" />.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  The <see langword="Content-type" /> header is not <see langword="null" /> or "application/x-www-form-urlencoded".</exception>
		public byte[] UploadValues(Uri address, NameValueCollection data)
		{
			return this.UploadValues(address, null, data);
		}

		/// <summary>Uploads the specified name/value collection to the resource identified by the specified URI, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the collection.</param>
		/// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="data" /> is <see langword="null" />.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header value is not <see langword="null" /> and is not <see langword="application/x-www-form-urlencoded" />.</exception>
		public byte[] UploadValues(string address, string method, NameValueCollection data)
		{
			return this.UploadValues(this.GetUri(address), method, data);
		}

		/// <summary>Uploads the specified name/value collection to the resource identified by the specified URI, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the collection.</param>
		/// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <returns>A <see cref="T:System.Byte" /> array containing the body of the response from the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="data" /> is <see langword="null" />.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header value is not <see langword="null" /> and is not <see langword="application/x-www-form-urlencoded" />.</exception>
		public byte[] UploadValues(Uri address, string method, NameValueCollection data)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			WebRequest request = null;
			this.StartOperation();
			byte[] result;
			try
			{
				byte[] valuesToUpload = this.GetValuesToUpload(data);
				this._method = method;
				request = (this._webRequest = this.GetWebRequest(this.GetUri(address)));
				result = this.UploadBits(request, null, valuesToUpload, 0, null, null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			finally
			{
				this.EndOperation();
			}
			return result;
		}

		/// <summary>Uploads the specified string to the specified resource, using the POST method.</summary>
		/// <param name="address">The URI of the resource to receive the string. For Http resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <returns>A <see cref="T:System.String" /> containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public string UploadString(string address, string data)
		{
			return this.UploadString(this.GetUri(address), null, data);
		}

		/// <summary>Uploads the specified string to the specified resource, using the POST method.</summary>
		/// <param name="address">The URI of the resource to receive the string. For Http resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <returns>A <see cref="T:System.String" /> containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public string UploadString(Uri address, string data)
		{
			return this.UploadString(address, null, data);
		}

		/// <summary>Uploads the specified string to the specified resource, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the string. This URI must identify a resource that can accept a request sent with the <paramref name="method" /> method.</param>
		/// <param name="method">The HTTP method used to send the string to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <returns>A <see cref="T:System.String" /> containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.</exception>
		public string UploadString(string address, string method, string data)
		{
			return this.UploadString(this.GetUri(address), method, data);
		}

		/// <summary>Uploads the specified string to the specified resource, using the specified method.</summary>
		/// <param name="address">The URI of the resource to receive the string. This URI must identify a resource that can accept a request sent with the <paramref name="method" /> method.</param>
		/// <param name="method">The HTTP method used to send the string to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <returns>A <see cref="T:System.String" /> containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.</exception>
		public string UploadString(Uri address, string method, string data)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			this.StartOperation();
			string stringUsingEncoding;
			try
			{
				byte[] bytes = this.Encoding.GetBytes(data);
				WebRequest request;
				byte[] data2 = this.UploadDataInternal(address, method, bytes, out request);
				stringUsingEncoding = this.GetStringUsingEncoding(request, data2);
			}
			finally
			{
				this.EndOperation();
			}
			return stringUsingEncoding;
		}

		/// <summary>Downloads the requested resource as a <see cref="T:System.String" />. The resource to download is specified as a <see cref="T:System.String" /> containing the URI.</summary>
		/// <param name="address">A <see cref="T:System.String" /> containing the URI to download.</param>
		/// <returns>A <see cref="T:System.String" /> containing the requested resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		/// <exception cref="T:System.NotSupportedException">The method has been called simultaneously on multiple threads.</exception>
		public string DownloadString(string address)
		{
			return this.DownloadString(this.GetUri(address));
		}

		/// <summary>Downloads the requested resource as a <see cref="T:System.String" />. The resource to download is specified as a <see cref="T:System.Uri" />.</summary>
		/// <param name="address">A <see cref="T:System.Uri" /> object containing the URI to download.</param>
		/// <returns>A <see cref="T:System.String" /> containing the requested resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		/// <exception cref="T:System.NotSupportedException">The method has been called simultaneously on multiple threads.</exception>
		public string DownloadString(Uri address)
		{
			WebClient.ThrowIfNull(address, "address");
			this.StartOperation();
			string stringUsingEncoding;
			try
			{
				WebRequest request;
				byte[] data = this.DownloadDataInternal(address, out request);
				stringUsingEncoding = this.GetStringUsingEncoding(request, data);
			}
			finally
			{
				this.EndOperation();
			}
			return stringUsingEncoding;
		}

		private static void AbortRequest(WebRequest request)
		{
			try
			{
				if (request != null)
				{
					request.Abort();
				}
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
			}
		}

		private void CopyHeadersTo(WebRequest request)
		{
			if (this._headers == null)
			{
				return;
			}
			HttpWebRequest httpWebRequest = request as HttpWebRequest;
			if (httpWebRequest == null)
			{
				return;
			}
			string text = this._headers["Accept"];
			string text2 = this._headers["Connection"];
			string text3 = this._headers["Content-Type"];
			string text4 = this._headers["Expect"];
			string text5 = this._headers["Referer"];
			string text6 = this._headers["User-Agent"];
			string text7 = this._headers["Host"];
			this._headers.Remove("Accept");
			this._headers.Remove("Connection");
			this._headers.Remove("Content-Type");
			this._headers.Remove("Expect");
			this._headers.Remove("Referer");
			this._headers.Remove("User-Agent");
			this._headers.Remove("Host");
			request.Headers = this._headers;
			if (!string.IsNullOrEmpty(text))
			{
				httpWebRequest.Accept = text;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				httpWebRequest.Connection = text2;
			}
			if (!string.IsNullOrEmpty(text3))
			{
				httpWebRequest.ContentType = text3;
			}
			if (!string.IsNullOrEmpty(text4))
			{
				httpWebRequest.Expect = text4;
			}
			if (!string.IsNullOrEmpty(text5))
			{
				httpWebRequest.Referer = text5;
			}
			if (!string.IsNullOrEmpty(text6))
			{
				httpWebRequest.UserAgent = text6;
			}
			if (!string.IsNullOrEmpty(text7))
			{
				httpWebRequest.Host = text7;
			}
		}

		private Uri GetUri(string address)
		{
			WebClient.ThrowIfNull(address, "address");
			Uri address2;
			if (this._baseAddress != null)
			{
				if (!Uri.TryCreate(this._baseAddress, address, out address2))
				{
					return new Uri(Path.GetFullPath(address));
				}
			}
			else if (!Uri.TryCreate(address, UriKind.Absolute, out address2))
			{
				return new Uri(Path.GetFullPath(address));
			}
			return this.GetUri(address2);
		}

		private Uri GetUri(Uri address)
		{
			WebClient.ThrowIfNull(address, "address");
			Uri uri = address;
			if (!address.IsAbsoluteUri && this._baseAddress != null && !Uri.TryCreate(this._baseAddress, address, out uri))
			{
				return address;
			}
			if (string.IsNullOrEmpty(uri.Query) && this._requestParameters != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string value = string.Empty;
				for (int i = 0; i < this._requestParameters.Count; i++)
				{
					stringBuilder.Append(value).Append(this._requestParameters.AllKeys[i]).Append('=').Append(this._requestParameters[i]);
					value = "&";
				}
				uri = new UriBuilder(uri)
				{
					Query = stringBuilder.ToString()
				}.Uri;
			}
			return uri;
		}

		private byte[] DownloadBits(WebRequest request, Stream writeStream)
		{
			byte[] result;
			try
			{
				WebResponse webResponse = this._webResponse = this.GetWebResponse(request);
				long contentLength = webResponse.ContentLength;
				byte[] array = new byte[(contentLength == -1L || contentLength > 65536L) ? 65536L : contentLength];
				if (writeStream is ChunkedMemoryStream)
				{
					if (contentLength > 2147483647L)
					{
						throw new WebException("The message length limit was exceeded", WebExceptionStatus.MessageLengthLimitExceeded);
					}
					writeStream.SetLength((long)array.Length);
				}
				using (Stream responseStream = webResponse.GetResponseStream())
				{
					if (responseStream != null)
					{
						int count;
						while ((count = responseStream.Read(array, 0, array.Length)) != 0)
						{
							writeStream.Write(array, 0, count);
						}
					}
				}
				ChunkedMemoryStream chunkedMemoryStream = writeStream as ChunkedMemoryStream;
				result = ((chunkedMemoryStream != null) ? chunkedMemoryStream.ToArray() : null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				if (writeStream != null)
				{
					writeStream.Close();
				}
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			return result;
		}

		private void DownloadBitsAsync(WebRequest request, Stream writeStream, AsyncOperation asyncOp, Action<byte[], Exception, AsyncOperation> completionDelegate)
		{
			WebClient.<DownloadBitsAsync>d__150 <DownloadBitsAsync>d__;
			<DownloadBitsAsync>d__.<>4__this = this;
			<DownloadBitsAsync>d__.request = request;
			<DownloadBitsAsync>d__.writeStream = writeStream;
			<DownloadBitsAsync>d__.asyncOp = asyncOp;
			<DownloadBitsAsync>d__.completionDelegate = completionDelegate;
			<DownloadBitsAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DownloadBitsAsync>d__.<>1__state = -1;
			<DownloadBitsAsync>d__.<>t__builder.Start<WebClient.<DownloadBitsAsync>d__150>(ref <DownloadBitsAsync>d__);
		}

		private byte[] UploadBits(WebRequest request, Stream readStream, byte[] buffer, int chunkSize, byte[] header, byte[] footer)
		{
			byte[] result;
			try
			{
				if (request.RequestUri.Scheme == Uri.UriSchemeFile)
				{
					footer = (header = null);
				}
				using (Stream requestStream = request.GetRequestStream())
				{
					if (header != null)
					{
						requestStream.Write(header, 0, header.Length);
					}
					if (readStream != null)
					{
						try
						{
							for (;;)
							{
								int num = readStream.Read(buffer, 0, buffer.Length);
								if (num <= 0)
								{
									break;
								}
								requestStream.Write(buffer, 0, num);
							}
							goto IL_8F;
						}
						finally
						{
							if (readStream != null)
							{
								((IDisposable)readStream).Dispose();
							}
						}
					}
					int num2;
					for (int i = 0; i < buffer.Length; i += num2)
					{
						num2 = buffer.Length - i;
						if (chunkSize != 0 && num2 > chunkSize)
						{
							num2 = chunkSize;
						}
						requestStream.Write(buffer, i, num2);
					}
					IL_8F:
					if (footer != null)
					{
						requestStream.Write(footer, 0, footer.Length);
					}
				}
				result = this.DownloadBits(request, new ChunkedMemoryStream());
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				WebClient.AbortRequest(request);
				if (ex is WebException || ex is SecurityException)
				{
					throw;
				}
				throw new WebException("An exception occurred during a WebClient request.", ex);
			}
			return result;
		}

		private void UploadBitsAsync(WebRequest request, Stream readStream, byte[] buffer, int chunkSize, byte[] header, byte[] footer, AsyncOperation asyncOp, Action<byte[], Exception, AsyncOperation> completionDelegate)
		{
			WebClient.<UploadBitsAsync>d__152 <UploadBitsAsync>d__;
			<UploadBitsAsync>d__.<>4__this = this;
			<UploadBitsAsync>d__.request = request;
			<UploadBitsAsync>d__.readStream = readStream;
			<UploadBitsAsync>d__.buffer = buffer;
			<UploadBitsAsync>d__.chunkSize = chunkSize;
			<UploadBitsAsync>d__.header = header;
			<UploadBitsAsync>d__.footer = footer;
			<UploadBitsAsync>d__.asyncOp = asyncOp;
			<UploadBitsAsync>d__.completionDelegate = completionDelegate;
			<UploadBitsAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UploadBitsAsync>d__.<>1__state = -1;
			<UploadBitsAsync>d__.<>t__builder.Start<WebClient.<UploadBitsAsync>d__152>(ref <UploadBitsAsync>d__);
		}

		private static bool ByteArrayHasPrefix(byte[] prefix, byte[] byteArray)
		{
			if (prefix == null || byteArray == null || prefix.Length > byteArray.Length)
			{
				return false;
			}
			for (int i = 0; i < prefix.Length; i++)
			{
				if (prefix[i] != byteArray[i])
				{
					return false;
				}
			}
			return true;
		}

		private string GetStringUsingEncoding(WebRequest request, byte[] data)
		{
			Encoding encoding = null;
			int num = -1;
			string text;
			try
			{
				text = request.ContentType;
			}
			catch (Exception ex) when (ex is NotImplementedException || ex is NotSupportedException)
			{
				text = null;
			}
			if (text != null)
			{
				text = text.ToLower(CultureInfo.InvariantCulture);
				string[] array = text.Split(WebClient.s_parseContentTypeSeparators);
				bool flag = false;
				foreach (string text2 in array)
				{
					if (text2 == "charset")
					{
						flag = true;
					}
					else if (flag)
					{
						try
						{
							encoding = Encoding.GetEncoding(text2);
						}
						catch (ArgumentException)
						{
							break;
						}
					}
				}
			}
			if (encoding == null)
			{
				Encoding[] array3 = WebClient.s_knownEncodings;
				for (int j = 0; j < array3.Length; j++)
				{
					byte[] preamble = array3[j].GetPreamble();
					if (WebClient.ByteArrayHasPrefix(preamble, data))
					{
						encoding = array3[j];
						num = preamble.Length;
						break;
					}
				}
			}
			if (encoding == null)
			{
				encoding = this.Encoding;
			}
			if (num == -1)
			{
				byte[] preamble2 = encoding.GetPreamble();
				num = (WebClient.ByteArrayHasPrefix(preamble2, data) ? preamble2.Length : 0);
			}
			return encoding.GetString(data, num, data.Length - num);
		}

		private string MapToDefaultMethod(Uri address)
		{
			if (!string.Equals(((!address.IsAbsoluteUri && this._baseAddress != null) ? new Uri(this._baseAddress, address) : address).Scheme, Uri.UriSchemeFtp, StringComparison.Ordinal))
			{
				return "POST";
			}
			return "STOR";
		}

		private static string UrlEncode(string str)
		{
			if (str == null)
			{
				return null;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return Encoding.ASCII.GetString(WebClient.UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false));
		}

		private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				char c = (char)bytes[offset + i];
				if (c == ' ')
				{
					num++;
				}
				else if (!WebClient.IsSafe(c))
				{
					num2++;
				}
			}
			if (!alwaysCreateReturnValue && num == 0 && num2 == 0)
			{
				return bytes;
			}
			byte[] array = new byte[count + num2 * 2];
			int num3 = 0;
			for (int j = 0; j < count; j++)
			{
				byte b = bytes[offset + j];
				char c2 = (char)b;
				if (WebClient.IsSafe(c2))
				{
					array[num3++] = b;
				}
				else if (c2 == ' ')
				{
					array[num3++] = 43;
				}
				else
				{
					array[num3++] = 37;
					array[num3++] = (byte)WebClient.IntToHex(b >> 4 & 15);
					array[num3++] = (byte)WebClient.IntToHex((int)(b & 15));
				}
			}
			return array;
		}

		private static char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 97);
		}

		private static bool IsSafe(char ch)
		{
			if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
			{
				return true;
			}
			if (ch != '!')
			{
				switch (ch)
				{
				case '\'':
				case '(':
				case ')':
				case '*':
				case '-':
				case '.':
					return true;
				case '+':
				case ',':
					break;
				default:
					if (ch == '_')
					{
						return true;
					}
					break;
				}
				return false;
			}
			return true;
		}

		private void InvokeOperationCompleted(AsyncOperation asyncOp, SendOrPostCallback callback, AsyncCompletedEventArgs eventArgs)
		{
			if (Interlocked.CompareExchange<AsyncOperation>(ref this._asyncOp, null, asyncOp) == asyncOp)
			{
				this.EndOperation();
				asyncOp.PostOperationCompleted(callback, eventArgs);
			}
		}

		/// <summary>Opens a readable stream containing the specified resource. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to retrieve.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and address is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public void OpenReadAsync(Uri address)
		{
			this.OpenReadAsync(address, null);
		}

		/// <summary>Opens a readable stream containing the specified resource. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to retrieve.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and address is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public void OpenReadAsync(Uri address, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				WebRequest request = this._webRequest = this.GetWebRequest(this.GetUri(address));
				request.BeginGetResponse(delegate(IAsyncResult iar)
				{
					Stream result = null;
					Exception exception = null;
					try
					{
						result = (this._webResponse = this.GetWebResponse(request, iar)).GetResponseStream();
					}
					catch (Exception ex2) when (!(ex2 is OutOfMemoryException))
					{
						exception = WebClient.GetExceptionToPropagate(ex2);
					}
					this.InvokeOperationCompleted(asyncOp, this._openReadOperationCompleted, new OpenReadCompletedEventArgs(result, exception, this._canceled, asyncOp.UserSuppliedState));
				}, null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				this.InvokeOperationCompleted(asyncOp, this._openReadOperationCompleted, new OpenReadCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState));
			}
		}

		/// <summary>Opens a stream for writing data to the specified resource. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		public void OpenWriteAsync(Uri address)
		{
			this.OpenWriteAsync(address, null, null);
		}

		/// <summary>Opens a stream for writing data to the specified resource. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		public void OpenWriteAsync(Uri address, string method)
		{
			this.OpenWriteAsync(address, method, null);
		}

		/// <summary>Opens a stream for writing data to the specified resource, using the specified method. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public void OpenWriteAsync(Uri address, string method, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				this._method = method;
				WebRequest request = this._webRequest = this.GetWebRequest(this.GetUri(address));
				request.BeginGetRequestStream(delegate(IAsyncResult iar)
				{
					WebClient.WebClientWriteStream result = null;
					Exception exception = null;
					try
					{
						result = new WebClient.WebClientWriteStream(request.EndGetRequestStream(iar), request, this);
					}
					catch (Exception ex2) when (!(ex2 is OutOfMemoryException))
					{
						exception = WebClient.GetExceptionToPropagate(ex2);
					}
					this.InvokeOperationCompleted(asyncOp, this._openWriteOperationCompleted, new OpenWriteCompletedEventArgs(result, exception, this._canceled, asyncOp.UserSuppliedState));
				}, null);
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				OpenWriteCompletedEventArgs eventArgs = new OpenWriteCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOp, this._openWriteOperationCompleted, eventArgs);
			}
		}

		private void DownloadStringAsyncCallback(byte[] returnBytes, Exception exception, object state)
		{
			AsyncOperation asyncOperation = (AsyncOperation)state;
			string result = null;
			try
			{
				if (returnBytes != null)
				{
					result = this.GetStringUsingEncoding(this._webRequest, returnBytes);
				}
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				exception = WebClient.GetExceptionToPropagate(ex);
			}
			DownloadStringCompletedEventArgs eventArgs = new DownloadStringCompletedEventArgs(result, exception, this._canceled, asyncOperation.UserSuppliedState);
			this.InvokeOperationCompleted(asyncOperation, this._downloadStringOperationCompleted, eventArgs);
		}

		/// <summary>Downloads the resource specified as a <see cref="T:System.Uri" />. This method does not block the calling thread.</summary>
		/// <param name="address">A <see cref="T:System.Uri" /> containing the URI to download.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		public void DownloadStringAsync(Uri address)
		{
			this.DownloadStringAsync(address, null);
		}

		/// <summary>Downloads the specified string to the specified resource. This method does not block the calling thread.</summary>
		/// <param name="address">A <see cref="T:System.Uri" /> containing the URI to download.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		public void DownloadStringAsync(Uri address, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			AsyncOperation asyncOperation = this.StartAsyncOperation(userToken);
			try
			{
				WebRequest request = this._webRequest = this.GetWebRequest(this.GetUri(address));
				this.DownloadBitsAsync(request, new ChunkedMemoryStream(), asyncOperation, new Action<byte[], Exception, AsyncOperation>(this.DownloadStringAsyncCallback));
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				this.DownloadStringAsyncCallback(null, WebClient.GetExceptionToPropagate(ex), asyncOperation);
			}
		}

		private void DownloadDataAsyncCallback(byte[] returnBytes, Exception exception, object state)
		{
			AsyncOperation asyncOperation = (AsyncOperation)state;
			DownloadDataCompletedEventArgs eventArgs = new DownloadDataCompletedEventArgs(returnBytes, exception, this._canceled, asyncOperation.UserSuppliedState);
			this.InvokeOperationCompleted(asyncOperation, this._downloadDataOperationCompleted, eventArgs);
		}

		/// <summary>Downloads the resource as a <see cref="T:System.Byte" /> array from the URI specified as an asynchronous operation.</summary>
		/// <param name="address">A <see cref="T:System.Uri" /> containing the URI to download.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		public void DownloadDataAsync(Uri address)
		{
			this.DownloadDataAsync(address, null);
		}

		/// <summary>Downloads the resource as a <see cref="T:System.Byte" /> array from the URI specified as an asynchronous operation.</summary>
		/// <param name="address">A <see cref="T:System.Uri" /> containing the URI to download.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		public void DownloadDataAsync(Uri address, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			AsyncOperation asyncOperation = this.StartAsyncOperation(userToken);
			try
			{
				WebRequest request = this._webRequest = this.GetWebRequest(this.GetUri(address));
				this.DownloadBitsAsync(request, new ChunkedMemoryStream(), asyncOperation, new Action<byte[], Exception, AsyncOperation>(this.DownloadDataAsyncCallback));
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				this.DownloadDataAsyncCallback(null, WebClient.GetExceptionToPropagate(ex), asyncOperation);
			}
		}

		private void DownloadFileAsyncCallback(byte[] returnBytes, Exception exception, object state)
		{
			AsyncOperation asyncOperation = (AsyncOperation)state;
			AsyncCompletedEventArgs eventArgs = new AsyncCompletedEventArgs(exception, this._canceled, asyncOperation.UserSuppliedState);
			this.InvokeOperationCompleted(asyncOperation, this._downloadFileOperationCompleted, eventArgs);
		}

		/// <summary>Downloads, to a local file, the resource with the specified URI. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to download.</param>
		/// <param name="fileName">The name of the file to be placed on the local computer.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		/// <exception cref="T:System.InvalidOperationException">The local file specified by <paramref name="fileName" /> is in use by another thread.</exception>
		public void DownloadFileAsync(Uri address, string fileName)
		{
			this.DownloadFileAsync(address, fileName, null);
		}

		/// <summary>Downloads, to a local file, the resource with the specified URI. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to download.</param>
		/// <param name="fileName">The name of the file to be placed on the local computer.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		/// <exception cref="T:System.InvalidOperationException">The local file specified by <paramref name="fileName" /> is in use by another thread.</exception>
		public void DownloadFileAsync(Uri address, string fileName, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(fileName, "fileName");
			FileStream fileStream = null;
			AsyncOperation asyncOperation = this.StartAsyncOperation(userToken);
			try
			{
				fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
				WebRequest request = this._webRequest = this.GetWebRequest(this.GetUri(address));
				this.DownloadBitsAsync(request, fileStream, asyncOperation, new Action<byte[], Exception, AsyncOperation>(this.DownloadFileAsyncCallback));
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				this.DownloadFileAsyncCallback(null, WebClient.GetExceptionToPropagate(ex), asyncOperation);
			}
		}

		/// <summary>Uploads the specified string to the specified resource. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the string. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public void UploadStringAsync(Uri address, string data)
		{
			this.UploadStringAsync(address, null, data, null);
		}

		/// <summary>Uploads the specified string to the specified resource. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the string. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public void UploadStringAsync(Uri address, string method, string data)
		{
			this.UploadStringAsync(address, method, data, null);
		}

		/// <summary>Uploads the specified string to the specified resource. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the string. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public void UploadStringAsync(Uri address, string method, string data, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			AsyncOperation asyncOperation = this.StartAsyncOperation(userToken);
			try
			{
				byte[] bytes = this.Encoding.GetBytes(data);
				this._method = method;
				this._contentLength = (long)bytes.Length;
				WebRequest request = this._webRequest = this.GetWebRequest(this.GetUri(address));
				this.UploadBitsAsync(request, null, bytes, 0, null, null, asyncOperation, delegate(byte[] bytesResult, Exception error, AsyncOperation uploadAsyncOp)
				{
					string result = null;
					if (error == null && bytesResult != null)
					{
						try
						{
							result = this.GetStringUsingEncoding(this._webRequest, bytesResult);
						}
						catch (Exception ex2) when (!(ex2 is OutOfMemoryException))
						{
							error = WebClient.GetExceptionToPropagate(ex2);
						}
					}
					this.InvokeOperationCompleted(uploadAsyncOp, this._uploadStringOperationCompleted, new UploadStringCompletedEventArgs(result, error, this._canceled, uploadAsyncOp.UserSuppliedState));
				});
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				UploadStringCompletedEventArgs eventArgs = new UploadStringCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOperation.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOperation, this._uploadStringOperationCompleted, eventArgs);
			}
		}

		/// <summary>Uploads a data buffer to a resource identified by a URI, using the POST method. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public void UploadDataAsync(Uri address, byte[] data)
		{
			this.UploadDataAsync(address, null, data, null);
		}

		/// <summary>Uploads a data buffer to a resource identified by a URI, using the specified method. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public void UploadDataAsync(Uri address, string method, byte[] data)
		{
			this.UploadDataAsync(address, method, data, null);
		}

		/// <summary>Uploads a data buffer to a resource identified by a URI, using the specified method and identifying token.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public void UploadDataAsync(Uri address, string method, byte[] data, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				this._method = method;
				this._contentLength = (long)data.Length;
				WebRequest request = this._webRequest = this.GetWebRequest(this.GetUri(address));
				int chunkSize = 0;
				if (this.UploadProgressChanged != null)
				{
					chunkSize = (int)Math.Min(8192L, (long)data.Length);
				}
				this.UploadBitsAsync(request, null, data, chunkSize, null, null, asyncOp, delegate(byte[] result, Exception error, AsyncOperation uploadAsyncOp)
				{
					this.InvokeOperationCompleted(asyncOp, this._uploadDataOperationCompleted, new UploadDataCompletedEventArgs(result, error, this._canceled, uploadAsyncOp.UserSuppliedState));
				});
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				UploadDataCompletedEventArgs eventArgs = new UploadDataCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOp, this._uploadDataOperationCompleted, eventArgs);
			}
		}

		/// <summary>Uploads the specified local file to the specified resource, using the POST method. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the file. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="fileName">The file to send to the resource.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid character, or the specified path to the file does not exist.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public void UploadFileAsync(Uri address, string fileName)
		{
			this.UploadFileAsync(address, null, fileName, null);
		}

		/// <summary>Uploads the specified local file to the specified resource, using the POST method. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the file. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="method">The method used to send the data to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="fileName">The file to send to the resource.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid character, or the specified path to the file does not exist.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public void UploadFileAsync(Uri address, string method, string fileName)
		{
			this.UploadFileAsync(address, method, fileName, null);
		}

		/// <summary>Uploads the specified local file to the specified resource, using the POST method. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the file. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="method">The method used to send the data to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="fileName">The file to send to the resource.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid character, or the specified path to the file does not exist.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public void UploadFileAsync(Uri address, string method, string fileName, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(fileName, "fileName");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			FileStream fileStream = null;
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				this._method = method;
				byte[] header = null;
				byte[] footer = null;
				byte[] buffer = null;
				Uri uri = this.GetUri(address);
				bool needsHeaderAndBoundary = uri.Scheme != Uri.UriSchemeFile;
				this.OpenFileInternal(needsHeaderAndBoundary, fileName, ref fileStream, ref buffer, ref header, ref footer);
				WebRequest request = this._webRequest = this.GetWebRequest(uri);
				this.UploadBitsAsync(request, fileStream, buffer, 0, header, footer, asyncOp, delegate(byte[] result, Exception error, AsyncOperation uploadAsyncOp)
				{
					this.InvokeOperationCompleted(asyncOp, this._uploadFileOperationCompleted, new UploadFileCompletedEventArgs(result, error, this._canceled, uploadAsyncOp.UserSuppliedState));
				});
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				UploadFileCompletedEventArgs eventArgs = new UploadFileCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOp, this._uploadFileOperationCompleted, eventArgs);
			}
		}

		/// <summary>Uploads the data in the specified name/value collection to the resource identified by the specified URI. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the collection. This URI must identify a resource that can accept a request sent with the default method.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public void UploadValuesAsync(Uri address, NameValueCollection data)
		{
			this.UploadValuesAsync(address, null, data, null);
		}

		/// <summary>Uploads the data in the specified name/value collection to the resource identified by the specified URI, using the specified method. This method does not block the calling thread.</summary>
		/// <param name="address">The URI of the resource to receive the collection. This URI must identify a resource that can accept a request sent with the <paramref name="method" /> method.</param>
		/// <param name="method">The method used to send the string to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.</exception>
		public void UploadValuesAsync(Uri address, string method, NameValueCollection data)
		{
			this.UploadValuesAsync(address, method, data, null);
		}

		/// <summary>Uploads the data in the specified name/value collection to the resource identified by the specified URI, using the specified method. This method does not block the calling thread, and allows the caller to pass an object to the method that is invoked when the operation completes.</summary>
		/// <param name="address">The URI of the resource to receive the collection. This URI must identify a resource that can accept a request sent with the <paramref name="method" /> method.</param>
		/// <param name="method">The HTTP method used to send the string to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.</exception>
		public void UploadValuesAsync(Uri address, string method, NameValueCollection data, object userToken)
		{
			WebClient.ThrowIfNull(address, "address");
			WebClient.ThrowIfNull(data, "data");
			if (method == null)
			{
				method = this.MapToDefaultMethod(address);
			}
			AsyncOperation asyncOp = this.StartAsyncOperation(userToken);
			try
			{
				byte[] valuesToUpload = this.GetValuesToUpload(data);
				this._method = method;
				WebRequest request = this._webRequest = this.GetWebRequest(this.GetUri(address));
				int chunkSize = 0;
				if (this.UploadProgressChanged != null)
				{
					chunkSize = (int)Math.Min(8192L, (long)valuesToUpload.Length);
				}
				this.UploadBitsAsync(request, null, valuesToUpload, chunkSize, null, null, asyncOp, delegate(byte[] result, Exception error, AsyncOperation uploadAsyncOp)
				{
					this.InvokeOperationCompleted(asyncOp, this._uploadValuesOperationCompleted, new UploadValuesCompletedEventArgs(result, error, this._canceled, uploadAsyncOp.UserSuppliedState));
				});
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException))
			{
				UploadValuesCompletedEventArgs eventArgs = new UploadValuesCompletedEventArgs(null, WebClient.GetExceptionToPropagate(ex), this._canceled, asyncOp.UserSuppliedState);
				this.InvokeOperationCompleted(asyncOp, this._uploadValuesOperationCompleted, eventArgs);
			}
		}

		private static Exception GetExceptionToPropagate(Exception e)
		{
			if (!(e is WebException) && !(e is SecurityException))
			{
				return new WebException("An exception occurred during a WebClient request.", e);
			}
			return e;
		}

		/// <summary>Cancels a pending asynchronous operation.</summary>
		public void CancelAsync()
		{
			WebRequest webRequest = this._webRequest;
			this._canceled = true;
			WebClient.AbortRequest(webRequest);
		}

		/// <summary>Downloads the resource as a <see cref="T:System.String" /> from the URI specified as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to download.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the downloaded resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		public Task<string> DownloadStringTaskAsync(string address)
		{
			return this.DownloadStringTaskAsync(this.GetUri(address));
		}

		/// <summary>Downloads the resource as a <see cref="T:System.String" /> from the URI specified as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to download.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the downloaded resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		public Task<string> DownloadStringTaskAsync(Uri address)
		{
			TaskCompletionSource<string> tcs = new TaskCompletionSource<string>(address);
			DownloadStringCompletedEventHandler handler = null;
			handler = delegate(object sender, DownloadStringCompletedEventArgs e)
			{
				this.HandleCompletion<DownloadStringCompletedEventArgs, DownloadStringCompletedEventHandler, string>(tcs, e, (DownloadStringCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, DownloadStringCompletedEventHandler completion)
				{
					webClient.DownloadStringCompleted -= completion;
				});
			};
			this.DownloadStringCompleted += handler;
			try
			{
				this.DownloadStringAsync(address, tcs);
			}
			catch
			{
				this.DownloadStringCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		/// <summary>Opens a readable stream containing the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to retrieve.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.IO.Stream" /> used to read data from a resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and address is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Task<Stream> OpenReadTaskAsync(string address)
		{
			return this.OpenReadTaskAsync(this.GetUri(address));
		}

		/// <summary>Opens a readable stream containing the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to retrieve.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.IO.Stream" /> used to read data from a resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and address is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Task<Stream> OpenReadTaskAsync(Uri address)
		{
			TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>(address);
			OpenReadCompletedEventHandler handler = null;
			handler = delegate(object sender, OpenReadCompletedEventArgs e)
			{
				this.HandleCompletion<OpenReadCompletedEventArgs, OpenReadCompletedEventHandler, Stream>(tcs, e, (OpenReadCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, OpenReadCompletedEventHandler completion)
				{
					webClient.OpenReadCompleted -= completion;
				});
			};
			this.OpenReadCompleted += handler;
			try
			{
				this.OpenReadAsync(address, tcs);
			}
			catch
			{
				this.OpenReadCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		/// <summary>Opens a stream for writing data to the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.IO.Stream" /> used to write data to the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Task<Stream> OpenWriteTaskAsync(string address)
		{
			return this.OpenWriteTaskAsync(this.GetUri(address), null);
		}

		/// <summary>Opens a stream for writing data to the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.IO.Stream" /> used to write data to the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Task<Stream> OpenWriteTaskAsync(Uri address)
		{
			return this.OpenWriteTaskAsync(address, null);
		}

		/// <summary>Opens a stream for writing data to the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.IO.Stream" /> used to write data to the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Task<Stream> OpenWriteTaskAsync(string address, string method)
		{
			return this.OpenWriteTaskAsync(this.GetUri(address), method);
		}

		/// <summary>Opens a stream for writing data to the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.IO.Stream" /> used to write data to the resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.</exception>
		public Task<Stream> OpenWriteTaskAsync(Uri address, string method)
		{
			TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>(address);
			OpenWriteCompletedEventHandler handler = null;
			handler = delegate(object sender, OpenWriteCompletedEventArgs e)
			{
				this.HandleCompletion<OpenWriteCompletedEventArgs, OpenWriteCompletedEventHandler, Stream>(tcs, e, (OpenWriteCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, OpenWriteCompletedEventHandler completion)
				{
					webClient.OpenWriteCompleted -= completion;
				});
			};
			this.OpenWriteCompleted += handler;
			try
			{
				this.OpenWriteAsync(address, method, tcs);
			}
			catch
			{
				this.OpenWriteCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		/// <summary>Uploads the specified string to the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the string. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.String" /> containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public Task<string> UploadStringTaskAsync(string address, string data)
		{
			return this.UploadStringTaskAsync(address, null, data);
		}

		/// <summary>Uploads the specified string to the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the string. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.String" /> containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public Task<string> UploadStringTaskAsync(Uri address, string data)
		{
			return this.UploadStringTaskAsync(address, null, data);
		}

		/// <summary>Uploads the specified string to the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the string. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.String" /> containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public Task<string> UploadStringTaskAsync(string address, string method, string data)
		{
			return this.UploadStringTaskAsync(this.GetUri(address), method, data);
		}

		/// <summary>Uploads the specified string to the specified resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the string. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="method">The HTTP method used to send the file to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The string to be uploaded.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.String" /> containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public Task<string> UploadStringTaskAsync(Uri address, string method, string data)
		{
			TaskCompletionSource<string> tcs = new TaskCompletionSource<string>(address);
			UploadStringCompletedEventHandler handler = null;
			handler = delegate(object sender, UploadStringCompletedEventArgs e)
			{
				this.HandleCompletion<UploadStringCompletedEventArgs, UploadStringCompletedEventHandler, string>(tcs, e, (UploadStringCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, UploadStringCompletedEventHandler completion)
				{
					webClient.UploadStringCompleted -= completion;
				});
			};
			this.UploadStringCompleted += handler;
			try
			{
				this.UploadStringAsync(address, method, data, tcs);
			}
			catch
			{
				this.UploadStringCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		/// <summary>Downloads the resource as a <see cref="T:System.Byte" /> array from the URI specified as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to download.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the downloaded resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		public Task<byte[]> DownloadDataTaskAsync(string address)
		{
			return this.DownloadDataTaskAsync(this.GetUri(address));
		}

		/// <summary>Downloads the resource as a <see cref="T:System.Byte" /> array from the URI specified as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to download.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the downloaded resource.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		public Task<byte[]> DownloadDataTaskAsync(Uri address)
		{
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
			DownloadDataCompletedEventHandler handler = null;
			handler = delegate(object sender, DownloadDataCompletedEventArgs e)
			{
				this.HandleCompletion<DownloadDataCompletedEventArgs, DownloadDataCompletedEventHandler, byte[]>(tcs, e, (DownloadDataCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, DownloadDataCompletedEventHandler completion)
				{
					webClient.DownloadDataCompleted -= completion;
				});
			};
			this.DownloadDataCompleted += handler;
			try
			{
				this.DownloadDataAsync(address, tcs);
			}
			catch
			{
				this.DownloadDataCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		/// <summary>Downloads the specified resource to a local file as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to download.</param>
		/// <param name="fileName">The name of the file to be placed on the local computer.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		/// <exception cref="T:System.InvalidOperationException">The local file specified by <paramref name="fileName" /> is in use by another thread.</exception>
		public Task DownloadFileTaskAsync(string address, string fileName)
		{
			return this.DownloadFileTaskAsync(this.GetUri(address), fileName);
		}

		/// <summary>Downloads the specified resource to a local file as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to download.</param>
		/// <param name="fileName">The name of the file to be placed on the local computer.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while downloading the resource.</exception>
		/// <exception cref="T:System.InvalidOperationException">The local file specified by <paramref name="fileName" /> is in use by another thread.</exception>
		public Task DownloadFileTaskAsync(Uri address, string fileName)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(address);
			AsyncCompletedEventHandler handler = null;
			handler = delegate(object sender, AsyncCompletedEventArgs e)
			{
				this.HandleCompletion<AsyncCompletedEventArgs, AsyncCompletedEventHandler, object>(tcs, e, (AsyncCompletedEventArgs args) => null, handler, delegate(WebClient webClient, AsyncCompletedEventHandler completion)
				{
					webClient.DownloadFileCompleted -= completion;
				});
			};
			this.DownloadFileCompleted += handler;
			try
			{
				this.DownloadFileAsync(address, fileName, tcs);
			}
			catch
			{
				this.DownloadFileCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		/// <summary>Uploads a data buffer that contains a <see cref="T:System.Byte" /> array to the URI specified as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the body of the response received from the resource when the data buffer was uploaded.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public Task<byte[]> UploadDataTaskAsync(string address, byte[] data)
		{
			return this.UploadDataTaskAsync(this.GetUri(address), null, data);
		}

		/// <summary>Uploads a data buffer that contains a <see cref="T:System.Byte" /> array to the URI specified as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the body of the response received from the resource when the data buffer was uploaded.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public Task<byte[]> UploadDataTaskAsync(Uri address, byte[] data)
		{
			return this.UploadDataTaskAsync(address, null, data);
		}

		/// <summary>Uploads a data buffer that contains a <see cref="T:System.Byte" /> array to the URI specified as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the body of the response received from the resource when the data buffer was uploaded.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public Task<byte[]> UploadDataTaskAsync(string address, string method, byte[] data)
		{
			return this.UploadDataTaskAsync(this.GetUri(address), method, data);
		}

		/// <summary>Uploads a data buffer that contains a <see cref="T:System.Byte" /> array to the URI specified as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the data.</param>
		/// <param name="method">The method used to send the data to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The data buffer to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the body of the response received from the resource when the data buffer was uploaded.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.</exception>
		public Task<byte[]> UploadDataTaskAsync(Uri address, string method, byte[] data)
		{
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
			UploadDataCompletedEventHandler handler = null;
			handler = delegate(object sender, UploadDataCompletedEventArgs e)
			{
				this.HandleCompletion<UploadDataCompletedEventArgs, UploadDataCompletedEventHandler, byte[]>(tcs, e, (UploadDataCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, UploadDataCompletedEventHandler completion)
				{
					webClient.UploadDataCompleted -= completion;
				});
			};
			this.UploadDataCompleted += handler;
			try
			{
				this.UploadDataAsync(address, method, data, tcs);
			}
			catch
			{
				this.UploadDataCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		/// <summary>Uploads the specified local file to a resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the file. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="fileName">The local file to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the body of the response received from the resource when the file was uploaded.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid character, or the specified path to the file does not exist.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public Task<byte[]> UploadFileTaskAsync(string address, string fileName)
		{
			return this.UploadFileTaskAsync(this.GetUri(address), null, fileName);
		}

		/// <summary>Uploads the specified local file to a resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the file. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="fileName">The local file to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the body of the response received from the resource when the file was uploaded.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid character, or the specified path to the file does not exist.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public Task<byte[]> UploadFileTaskAsync(Uri address, string fileName)
		{
			return this.UploadFileTaskAsync(address, null, fileName);
		}

		/// <summary>Uploads the specified local file to a resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the file. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="method">The method used to send the data to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="fileName">The local file to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the body of the response received from the resource when the file was uploaded.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid character, or the specified path to the file does not exist.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public Task<byte[]> UploadFileTaskAsync(string address, string method, string fileName)
		{
			return this.UploadFileTaskAsync(this.GetUri(address), method, fileName);
		}

		/// <summary>Uploads the specified local file to a resource as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the file. For HTTP resources, this URI must identify a resource that can accept a request sent with the POST method, such as a script or ASP page.</param>
		/// <param name="method">The method used to send the data to the resource. If <see langword="null" />, the default is POST for http and STOR for ftp.</param>
		/// <param name="fileName">The local file to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the body of the response received from the resource when the file was uploaded.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="fileName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" /> and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="fileName" /> is <see langword="null" />, is <see cref="F:System.String.Empty" />, contains invalid character, or the specified path to the file does not exist.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header begins with <see langword="multipart" />.</exception>
		public Task<byte[]> UploadFileTaskAsync(Uri address, string method, string fileName)
		{
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
			UploadFileCompletedEventHandler handler = null;
			handler = delegate(object sender, UploadFileCompletedEventArgs e)
			{
				this.HandleCompletion<UploadFileCompletedEventArgs, UploadFileCompletedEventHandler, byte[]>(tcs, e, (UploadFileCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, UploadFileCompletedEventHandler completion)
				{
					webClient.UploadFileCompleted -= completion;
				});
			};
			this.UploadFileCompleted += handler;
			try
			{
				this.UploadFileAsync(address, method, fileName, tcs);
			}
			catch
			{
				this.UploadFileCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		/// <summary>Uploads the specified name/value collection to the resource identified by the specified URI as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the collection.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  The <see langword="Content-type" /> header is not <see langword="null" /> or "application/x-www-form-urlencoded".</exception>
		public Task<byte[]> UploadValuesTaskAsync(string address, NameValueCollection data)
		{
			return this.UploadValuesTaskAsync(this.GetUri(address), null, data);
		}

		/// <summary>Uploads the specified name/value collection to the resource identified by the specified URI as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the collection.</param>
		/// <param name="method">The HTTP method used to send the collection to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  The <see langword="Content-type" /> header is not <see langword="null" /> or "application/x-www-form-urlencoded".</exception>
		public Task<byte[]> UploadValuesTaskAsync(string address, string method, NameValueCollection data)
		{
			return this.UploadValuesTaskAsync(this.GetUri(address), method, data);
		}

		/// <summary>Uploads the specified name/value collection to the resource identified by the specified URI as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the collection.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  The <see langword="Content-type" /> header value is not <see langword="null" /> and is not <see langword="application/x-www-form-urlencoded" />.</exception>
		public Task<byte[]> UploadValuesTaskAsync(Uri address, NameValueCollection data)
		{
			return this.UploadValuesTaskAsync(address, null, data);
		}

		/// <summary>Uploads the specified name/value collection to the resource identified by the specified URI as an asynchronous operation using a task object.</summary>
		/// <param name="address">The URI of the resource to receive the collection.</param>
		/// <param name="method">The HTTP method used to send the collection to the resource. If null, the default is POST for http and STOR for ftp.</param>
		/// <param name="data">The <see cref="T:System.Collections.Specialized.NameValueCollection" /> to send to the resource.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the response sent by the server.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.WebException">The URI formed by combining <see cref="P:System.Net.WebClient.BaseAddress" />, and <paramref name="address" /> is invalid.  
		///  -or-  
		///  <paramref name="method" /> cannot be used to send content.  
		///  -or-  
		///  There was no response from the server hosting the resource.  
		///  -or-  
		///  An error occurred while opening the stream.  
		///  -or-  
		///  The <see langword="Content-type" /> header is not <see langword="null" /> or "application/x-www-form-urlencoded".</exception>
		public Task<byte[]> UploadValuesTaskAsync(Uri address, string method, NameValueCollection data)
		{
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(address);
			UploadValuesCompletedEventHandler handler = null;
			handler = delegate(object sender, UploadValuesCompletedEventArgs e)
			{
				this.HandleCompletion<UploadValuesCompletedEventArgs, UploadValuesCompletedEventHandler, byte[]>(tcs, e, (UploadValuesCompletedEventArgs args) => args.Result, handler, delegate(WebClient webClient, UploadValuesCompletedEventHandler completion)
				{
					webClient.UploadValuesCompleted -= completion;
				});
			};
			this.UploadValuesCompleted += handler;
			try
			{
				this.UploadValuesAsync(address, method, data, tcs);
			}
			catch
			{
				this.UploadValuesCompleted -= handler;
				throw;
			}
			return tcs.Task;
		}

		private void HandleCompletion<TAsyncCompletedEventArgs, TCompletionDelegate, T>(TaskCompletionSource<T> tcs, TAsyncCompletedEventArgs e, Func<TAsyncCompletedEventArgs, T> getResult, TCompletionDelegate handler, Action<WebClient, TCompletionDelegate> unregisterHandler) where TAsyncCompletedEventArgs : AsyncCompletedEventArgs
		{
			if (e.UserState == tcs)
			{
				try
				{
					unregisterHandler(this, handler);
				}
				finally
				{
					if (e.Error != null)
					{
						tcs.TrySetException(e.Error);
					}
					else if (e.Cancelled)
					{
						tcs.TrySetCanceled();
					}
					else
					{
						tcs.TrySetResult(getResult(e));
					}
				}
			}
		}

		private void PostProgressChanged(AsyncOperation asyncOp, WebClient.ProgressData progress)
		{
			if (asyncOp != null && (progress.BytesSent > 0L || progress.BytesReceived > 0L))
			{
				if (progress.HasUploadPhase)
				{
					if (this.UploadProgressChanged != null)
					{
						int progressPercentage = (progress.TotalBytesToReceive < 0L && progress.BytesReceived == 0L) ? ((progress.TotalBytesToSend < 0L) ? 0 : ((progress.TotalBytesToSend == 0L) ? 50 : ((int)(50L * progress.BytesSent / progress.TotalBytesToSend)))) : ((progress.TotalBytesToSend < 0L) ? 50 : ((progress.TotalBytesToReceive == 0L) ? 100 : ((int)(50L * progress.BytesReceived / progress.TotalBytesToReceive + 50L))));
						asyncOp.Post(this._reportUploadProgressChanged, new UploadProgressChangedEventArgs(progressPercentage, asyncOp.UserSuppliedState, progress.BytesSent, progress.TotalBytesToSend, progress.BytesReceived, progress.TotalBytesToReceive));
						return;
					}
				}
				else if (this.DownloadProgressChanged != null)
				{
					int progressPercentage = (progress.TotalBytesToReceive < 0L) ? 0 : ((progress.TotalBytesToReceive == 0L) ? 100 : ((int)(100L * progress.BytesReceived / progress.TotalBytesToReceive)));
					asyncOp.Post(this._reportDownloadProgressChanged, new DownloadProgressChangedEventArgs(progressPercentage, asyncOp.UserSuppliedState, progress.BytesReceived, progress.TotalBytesToReceive));
				}
			}
		}

		private static void ThrowIfNull(object argument, string parameterName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		/// <summary>Gets or sets a value that indicates whether to buffer the data read from the Internet resource for a <see cref="T:System.Net.WebClient" /> instance.</summary>
		/// <returns>
		///   <see langword="true" /> to enable buffering of the data received from the Internet resource; <see langword="false" /> to disable buffering. The default is <see langword="true" />.</returns>
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool AllowReadStreamBuffering { get; set; }

		/// <summary>Gets or sets a value that indicates whether to buffer the data written to the Internet resource for a <see cref="T:System.Net.WebClient" /> instance.</summary>
		/// <returns>
		///   <see langword="true" /> to enable buffering of the data written to the Internet resource; <see langword="false" /> to disable buffering. The default is <see langword="true" />.</returns>
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool AllowWriteStreamBuffering { get; set; }

		/// <summary>Occurs when an asynchronous operation to write data to a resource using a write stream is closed.</summary>
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event WriteStreamClosedEventHandler WriteStreamClosed
		{
			add
			{
			}
			remove
			{
			}
		}

		/// <summary>Raises the <see cref="E:System.Net.WebClient.WriteStreamClosed" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.WriteStreamClosedEventArgs" /> object containing event data.</param>
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnWriteStreamClosed(WriteStreamClosedEventArgs e)
		{
		}

		private const int DefaultCopyBufferLength = 8192;

		private const int DefaultDownloadBufferLength = 65536;

		private const string DefaultUploadFileContentType = "application/octet-stream";

		private const string UploadFileContentType = "multipart/form-data";

		private const string UploadValuesContentType = "application/x-www-form-urlencoded";

		private Uri _baseAddress;

		private ICredentials _credentials;

		private WebHeaderCollection _headers;

		private NameValueCollection _requestParameters;

		private WebResponse _webResponse;

		private WebRequest _webRequest;

		private Encoding _encoding = Encoding.Default;

		private string _method;

		private long _contentLength = -1L;

		private bool _initWebClientAsync;

		private bool _canceled;

		private WebClient.ProgressData _progress;

		private IWebProxy _proxy;

		private bool _proxySet;

		private int _callNesting;

		private AsyncOperation _asyncOp;

		private SendOrPostCallback _downloadDataOperationCompleted;

		private SendOrPostCallback _openReadOperationCompleted;

		private SendOrPostCallback _openWriteOperationCompleted;

		private SendOrPostCallback _downloadStringOperationCompleted;

		private SendOrPostCallback _downloadFileOperationCompleted;

		private SendOrPostCallback _uploadStringOperationCompleted;

		private SendOrPostCallback _uploadDataOperationCompleted;

		private SendOrPostCallback _uploadFileOperationCompleted;

		private SendOrPostCallback _uploadValuesOperationCompleted;

		private SendOrPostCallback _reportDownloadProgressChanged;

		private SendOrPostCallback _reportUploadProgressChanged;

		private static readonly char[] s_parseContentTypeSeparators = new char[]
		{
			';',
			'=',
			' '
		};

		private static readonly Encoding[] s_knownEncodings = new Encoding[]
		{
			Encoding.UTF8,
			Encoding.UTF32,
			Encoding.Unicode,
			Encoding.BigEndianUnicode
		};

		private sealed class ProgressData
		{
			internal void Reset()
			{
				this.BytesSent = 0L;
				this.TotalBytesToSend = -1L;
				this.BytesReceived = 0L;
				this.TotalBytesToReceive = -1L;
				this.HasUploadPhase = false;
			}

			internal long BytesSent;

			internal long TotalBytesToSend = -1L;

			internal long BytesReceived;

			internal long TotalBytesToReceive = -1L;

			internal bool HasUploadPhase;
		}

		private sealed class WebClientWriteStream : DelegatingStream
		{
			public WebClientWriteStream(Stream stream, WebRequest request, WebClient webClient) : base(stream)
			{
				this._request = request;
				this._webClient = webClient;
			}

			protected override void Dispose(bool disposing)
			{
				try
				{
					if (disposing)
					{
						this._webClient.GetWebResponse(this._request).Dispose();
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}

			private readonly WebRequest _request;

			private readonly WebClient _webClient;
		}
	}
}
