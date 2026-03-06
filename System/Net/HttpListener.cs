using System;
using System.Collections;
using System.IO;
using System.Net.Security;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Mono.Net.Security.Private;
using Mono.Security.Authenticode;
using Mono.Security.Interface;

namespace System.Net
{
	/// <summary>Provides a simple, programmatically controlled HTTP protocol listener. This class cannot be inherited.</summary>
	public sealed class HttpListener : IDisposable
	{
		internal HttpListener(X509Certificate certificate, MonoTlsProvider tlsProvider, MonoTlsSettings tlsSettings) : this()
		{
			this.certificate = certificate;
			this.tlsProvider = tlsProvider;
			this.tlsSettings = tlsSettings;
		}

		internal X509Certificate LoadCertificateAndKey(IPAddress addr, int port)
		{
			object internalLock = this._internalLock;
			X509Certificate result;
			lock (internalLock)
			{
				if (this.certificate != null)
				{
					result = this.certificate;
				}
				else
				{
					try
					{
						string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".mono");
						path = Path.Combine(path, "httplistener");
						string text = Path.Combine(path, string.Format("{0}.cer", port));
						if (!File.Exists(text))
						{
							result = null;
						}
						else
						{
							string text2 = Path.Combine(path, string.Format("{0}.pvk", port));
							if (!File.Exists(text2))
							{
								result = null;
							}
							else
							{
								X509Certificate2 x509Certificate = new X509Certificate2(text);
								RSA rsa = PrivateKey.CreateFromFile(text2).RSA;
								this.certificate = new X509Certificate2((X509Certificate2Impl)x509Certificate.Impl.CopyWithPrivateKey(rsa));
								result = this.certificate;
							}
						}
					}
					catch
					{
						this.certificate = null;
						result = null;
					}
				}
			}
			return result;
		}

		internal SslStream CreateSslStream(Stream innerStream, bool ownsStream, RemoteCertificateValidationCallback callback)
		{
			object internalLock = this._internalLock;
			SslStream result;
			lock (internalLock)
			{
				if (this.tlsProvider == null)
				{
					this.tlsProvider = MonoTlsProviderFactory.GetProvider();
				}
				MonoTlsSettings monoTlsSettings = (this.tlsSettings ?? MonoTlsSettings.DefaultSettings).Clone();
				monoTlsSettings.RemoteCertificateValidationCallback = CallbackHelpers.PublicToMono(callback);
				result = new SslStream(innerStream, ownsStream, this.tlsProvider, monoTlsSettings);
			}
			return result;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.HttpListener" /> class.</summary>
		/// <exception cref="T:System.PlatformNotSupportedException">This class cannot be used on the current operating system. Windows Server 2003 or Windows XP SP2 is required to use instances of this class.</exception>
		public HttpListener()
		{
			this._internalLock = new object();
			this.prefixes = new HttpListenerPrefixCollection(this);
			this.registry = new Hashtable();
			this.connections = Hashtable.Synchronized(new Hashtable());
			this.ctx_queue = new ArrayList();
			this.wait_queue = new ArrayList();
			this.auth_schemes = AuthenticationSchemes.Anonymous;
			this.defaultServiceNames = new ServiceNameStore();
			this.extendedProtectionPolicy = new ExtendedProtectionPolicy(PolicyEnforcement.Never);
		}

		/// <summary>Gets or sets the scheme used to authenticate clients.</summary>
		/// <returns>A bitwise combination of <see cref="T:System.Net.AuthenticationSchemes" /> enumeration values that indicates how clients are to be authenticated. The default value is <see cref="F:System.Net.AuthenticationSchemes.Anonymous" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		public AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return this.auth_schemes;
			}
			set
			{
				this.CheckDisposed();
				this.auth_schemes = value;
			}
		}

		/// <summary>Gets or sets the delegate called to determine the protocol used to authenticate clients.</summary>
		/// <returns>An <see cref="T:System.Net.AuthenticationSchemeSelector" /> delegate that invokes the method used to select an authentication protocol. The default value is <see langword="null" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		public AuthenticationSchemeSelector AuthenticationSchemeSelectorDelegate
		{
			get
			{
				return this.auth_selector;
			}
			set
			{
				this.CheckDisposed();
				this.auth_selector = value;
			}
		}

		/// <summary>Gets or sets the delegate called to determine the <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> to use for each request.</summary>
		/// <returns>A <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> that specifies the policy to use for extended protection.</returns>
		/// <exception cref="T:System.ArgumentException">An attempt was made to set the <see cref="P:System.Net.HttpListener.ExtendedProtectionSelectorDelegate" /> property, but the <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomChannelBinding" /> property must be <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">An attempt was made to set the <see cref="P:System.Net.HttpListener.ExtendedProtectionSelectorDelegate" /> property to <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to set the <see cref="P:System.Net.HttpListener.ExtendedProtectionSelectorDelegate" /> property after the <see cref="M:System.Net.HttpListener.Start" /> method was already called.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object is closed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">An attempt was made to set the <see cref="P:System.Net.HttpListener.ExtendedProtectionSelectorDelegate" /> property on a platform that does not support extended protection.</exception>
		public HttpListener.ExtendedProtectionSelector ExtendedProtectionSelectorDelegate
		{
			get
			{
				return this.extendedProtectionSelectorDelegate;
			}
			set
			{
				this.CheckDisposed();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (!AuthenticationManager.OSSupportsExtendedProtection)
				{
					throw new PlatformNotSupportedException(SR.GetString("This operation requires OS support for extended protection."));
				}
				this.extendedProtectionSelectorDelegate = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies whether your application receives exceptions that occur when an <see cref="T:System.Net.HttpListener" /> sends the response to the client.</summary>
		/// <returns>
		///   <see langword="true" /> if this <see cref="T:System.Net.HttpListener" /> should not return exceptions that occur when sending the response to the client; otherwise, <see langword="false" />. The default value is <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		public bool IgnoreWriteExceptions
		{
			get
			{
				return this.ignore_write_exceptions;
			}
			set
			{
				this.CheckDisposed();
				this.ignore_write_exceptions = value;
			}
		}

		/// <summary>Gets a value that indicates whether <see cref="T:System.Net.HttpListener" /> has been started.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.HttpListener" /> was started; otherwise, <see langword="false" />.</returns>
		public bool IsListening
		{
			get
			{
				return this.listening;
			}
		}

		/// <summary>Gets a value that indicates whether <see cref="T:System.Net.HttpListener" /> can be used with the current operating system.</summary>
		/// <returns>
		///   <see langword="true" /> if <see cref="T:System.Net.HttpListener" /> is supported; otherwise, <see langword="false" />.</returns>
		public static bool IsSupported
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets the Uniform Resource Identifier (URI) prefixes handled by this <see cref="T:System.Net.HttpListener" /> object.</summary>
		/// <returns>An <see cref="T:System.Net.HttpListenerPrefixCollection" /> that contains the URI prefixes that this <see cref="T:System.Net.HttpListener" /> object is configured to handle.</returns>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		public HttpListenerPrefixCollection Prefixes
		{
			get
			{
				this.CheckDisposed();
				return this.prefixes;
			}
		}

		/// <summary>The timeout manager for this <see cref="T:System.Net.HttpListener" /> instance.</summary>
		/// <returns>The timeout manager for this <see cref="T:System.Net.HttpListener" /> instance.</returns>
		[MonoTODO]
		public HttpListenerTimeoutManager TimeoutManager
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> to use for extended protection for a session.</summary>
		/// <returns>A <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> that specifies the policy to use for extended protection.</returns>
		/// <exception cref="T:System.ArgumentException">An attempt was made to set the <see cref="P:System.Net.HttpListener.ExtendedProtectionPolicy" /> property, but the <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.CustomChannelBinding" /> property was not <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">An attempt was made to set the <see cref="P:System.Net.HttpListener.ExtendedProtectionPolicy" /> property to <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to set the <see cref="P:System.Net.HttpListener.ExtendedProtectionPolicy" /> property after the <see cref="M:System.Net.HttpListener.Start" /> method was already called.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object is closed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The <see cref="P:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.PolicyEnforcement" /> property was set to <see cref="F:System.Security.Authentication.ExtendedProtection.PolicyEnforcement.Always" /> on a platform that does not support extended protection.</exception>
		[MonoTODO("not used anywhere in the implementation")]
		public ExtendedProtectionPolicy ExtendedProtectionPolicy
		{
			get
			{
				return this.extendedProtectionPolicy;
			}
			set
			{
				this.CheckDisposed();
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!AuthenticationManager.OSSupportsExtendedProtection && value.PolicyEnforcement == PolicyEnforcement.Always)
				{
					throw new PlatformNotSupportedException(SR.GetString("This operation requires OS support for extended protection."));
				}
				if (value.CustomChannelBinding != null)
				{
					throw new ArgumentException(SR.GetString("Custom channel bindings are not supported."), "CustomChannelBinding");
				}
				this.extendedProtectionPolicy = value;
			}
		}

		/// <summary>Gets a default list of Service Provider Names (SPNs) as determined by registered prefixes.</summary>
		/// <returns>A <see cref="T:System.Security.Authentication.ExtendedProtection.ServiceNameCollection" /> that contains a list of SPNs.</returns>
		public ServiceNameCollection DefaultServiceNames
		{
			get
			{
				return this.defaultServiceNames.ServiceNames;
			}
		}

		/// <summary>Gets or sets the realm, or resource partition, associated with this <see cref="T:System.Net.HttpListener" /> object.</summary>
		/// <returns>A <see cref="T:System.String" /> value that contains the name of the realm associated with the <see cref="T:System.Net.HttpListener" /> object.</returns>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		public string Realm
		{
			get
			{
				return this.realm;
			}
			set
			{
				this.CheckDisposed();
				this.realm = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that controls whether, when NTLM is used, additional requests using the same Transmission Control Protocol (TCP) connection are required to authenticate.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.Principal.IIdentity" /> of the first request will be used for subsequent requests on the same connection; otherwise, <see langword="false" />. The default value is <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		[MonoTODO("Support for NTLM needs some loving.")]
		public bool UnsafeConnectionNtlmAuthentication
		{
			get
			{
				return this.unsafe_ntlm_auth;
			}
			set
			{
				this.CheckDisposed();
				this.unsafe_ntlm_auth = value;
			}
		}

		/// <summary>Shuts down the <see cref="T:System.Net.HttpListener" /> object immediately, discarding all currently queued requests.</summary>
		public void Abort()
		{
			if (this.disposed)
			{
				return;
			}
			if (!this.listening)
			{
				return;
			}
			this.Close(true);
		}

		/// <summary>Shuts down the <see cref="T:System.Net.HttpListener" />.</summary>
		public void Close()
		{
			if (this.disposed)
			{
				return;
			}
			if (!this.listening)
			{
				this.disposed = true;
				return;
			}
			this.Close(true);
			this.disposed = true;
		}

		private void Close(bool force)
		{
			this.CheckDisposed();
			EndPointManager.RemoveListener(this);
			this.Cleanup(force);
		}

		private void Cleanup(bool close_existing)
		{
			object internalLock = this._internalLock;
			lock (internalLock)
			{
				if (close_existing)
				{
					ICollection keys = this.registry.Keys;
					HttpListenerContext[] array = new HttpListenerContext[keys.Count];
					keys.CopyTo(array, 0);
					this.registry.Clear();
					for (int i = array.Length - 1; i >= 0; i--)
					{
						array[i].Connection.Close(true);
					}
				}
				object syncRoot = this.connections.SyncRoot;
				lock (syncRoot)
				{
					ICollection keys2 = this.connections.Keys;
					HttpConnection[] array2 = new HttpConnection[keys2.Count];
					keys2.CopyTo(array2, 0);
					this.connections.Clear();
					for (int j = array2.Length - 1; j >= 0; j--)
					{
						array2[j].Close(true);
					}
				}
				ArrayList obj = this.ctx_queue;
				lock (obj)
				{
					HttpListenerContext[] array3 = (HttpListenerContext[])this.ctx_queue.ToArray(typeof(HttpListenerContext));
					this.ctx_queue.Clear();
					for (int k = array3.Length - 1; k >= 0; k--)
					{
						array3[k].Connection.Close(true);
					}
				}
				obj = this.wait_queue;
				lock (obj)
				{
					Exception exc = new ObjectDisposedException("listener");
					foreach (object obj2 in this.wait_queue)
					{
						((ListenerAsyncResult)obj2).Complete(exc);
					}
					this.wait_queue.Clear();
				}
			}
		}

		/// <summary>Begins asynchronously retrieving an incoming request.</summary>
		/// <param name="callback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when a client request is available.</param>
		/// <param name="state">A user-defined object that contains information about the operation. This object is passed to the <paramref name="callback" /> delegate when the operation completes.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that indicates the status of the asynchronous operation.</returns>
		/// <exception cref="T:System.Net.HttpListenerException">A Win32 function call failed. Check the exception's <see cref="P:System.Net.HttpListenerException.ErrorCode" /> property to determine the cause of the exception.</exception>
		/// <exception cref="T:System.InvalidOperationException">This object has not been started or is currently stopped.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object is closed.</exception>
		public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
		{
			this.CheckDisposed();
			if (!this.listening)
			{
				throw new InvalidOperationException("Please, call Start before using this method.");
			}
			ListenerAsyncResult listenerAsyncResult = new ListenerAsyncResult(callback, state);
			ArrayList obj = this.wait_queue;
			lock (obj)
			{
				ArrayList obj2 = this.ctx_queue;
				lock (obj2)
				{
					HttpListenerContext contextFromQueue = this.GetContextFromQueue();
					if (contextFromQueue != null)
					{
						listenerAsyncResult.Complete(contextFromQueue, true);
						return listenerAsyncResult;
					}
				}
				this.wait_queue.Add(listenerAsyncResult);
			}
			return listenerAsyncResult;
		}

		/// <summary>Completes an asynchronous operation to retrieve an incoming client request.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> object that was obtained when the asynchronous operation was started.</param>
		/// <returns>An <see cref="T:System.Net.HttpListenerContext" /> object that represents the client request.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not obtained by calling the <see cref="M:System.Net.HttpListener.BeginGetContext(System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.HttpListenerException">A Win32 function call failed. Check the exception's <see cref="P:System.Net.HttpListenerException.ErrorCode" /> property to determine the cause of the exception.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="M:System.Net.HttpListener.EndGetContext(System.IAsyncResult)" /> method was already called for the specified <paramref name="asyncResult" /> object.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object is closed.</exception>
		public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
		{
			this.CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			ListenerAsyncResult listenerAsyncResult = asyncResult as ListenerAsyncResult;
			if (listenerAsyncResult == null)
			{
				throw new ArgumentException("Wrong IAsyncResult.", "asyncResult");
			}
			if (listenerAsyncResult.EndCalled)
			{
				throw new ArgumentException("Cannot reuse this IAsyncResult");
			}
			listenerAsyncResult.EndCalled = true;
			if (!listenerAsyncResult.IsCompleted)
			{
				listenerAsyncResult.AsyncWaitHandle.WaitOne();
			}
			ArrayList obj = this.wait_queue;
			lock (obj)
			{
				int num = this.wait_queue.IndexOf(listenerAsyncResult);
				if (num >= 0)
				{
					this.wait_queue.RemoveAt(num);
				}
			}
			HttpListenerContext context = listenerAsyncResult.GetContext();
			context.ParseAuthentication(this.SelectAuthenticationScheme(context));
			return context;
		}

		internal AuthenticationSchemes SelectAuthenticationScheme(HttpListenerContext context)
		{
			if (this.AuthenticationSchemeSelectorDelegate != null)
			{
				return this.AuthenticationSchemeSelectorDelegate(context.Request);
			}
			return this.auth_schemes;
		}

		/// <summary>Waits for an incoming request and returns when one is received.</summary>
		/// <returns>An <see cref="T:System.Net.HttpListenerContext" /> object that represents a client request.</returns>
		/// <exception cref="T:System.Net.HttpListenerException">A Win32 function call failed. Check the exception's <see cref="P:System.Net.HttpListenerException.ErrorCode" /> property to determine the cause of the exception.</exception>
		/// <exception cref="T:System.InvalidOperationException">This object has not been started or is currently stopped.  
		///  -or-  
		///  The <see cref="T:System.Net.HttpListener" /> does not have any Uniform Resource Identifier (URI) prefixes to respond to.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object is closed.</exception>
		public HttpListenerContext GetContext()
		{
			if (this.prefixes.Count == 0)
			{
				throw new InvalidOperationException("Please, call AddPrefix before using this method.");
			}
			ListenerAsyncResult listenerAsyncResult = (ListenerAsyncResult)this.BeginGetContext(null, null);
			listenerAsyncResult.InGet = true;
			return this.EndGetContext(listenerAsyncResult);
		}

		/// <summary>Allows this instance to receive incoming requests.</summary>
		/// <exception cref="T:System.Net.HttpListenerException">A Win32 function call failed. Check the exception's <see cref="P:System.Net.HttpListenerException.ErrorCode" /> property to determine the cause of the exception.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object is closed.</exception>
		public void Start()
		{
			this.CheckDisposed();
			if (this.listening)
			{
				return;
			}
			EndPointManager.AddListener(this);
			this.listening = true;
		}

		/// <summary>Causes this instance to stop receiving incoming requests.</summary>
		/// <exception cref="T:System.ObjectDisposedException">This object has been closed.</exception>
		public void Stop()
		{
			this.CheckDisposed();
			this.listening = false;
			this.Close(false);
		}

		/// <summary>Releases the resources held by this <see cref="T:System.Net.HttpListener" /> object.</summary>
		void IDisposable.Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.Close(true);
			this.disposed = true;
		}

		/// <summary>Waits for an incoming request as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns an <see cref="T:System.Net.HttpListenerContext" /> object that represents a client request.</returns>
		public Task<HttpListenerContext> GetContextAsync()
		{
			return Task<HttpListenerContext>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetContext), new Func<IAsyncResult, HttpListenerContext>(this.EndGetContext), null);
		}

		internal void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private HttpListenerContext GetContextFromQueue()
		{
			if (this.ctx_queue.Count == 0)
			{
				return null;
			}
			HttpListenerContext result = (HttpListenerContext)this.ctx_queue[0];
			this.ctx_queue.RemoveAt(0);
			return result;
		}

		internal void RegisterContext(HttpListenerContext context)
		{
			object internalLock = this._internalLock;
			lock (internalLock)
			{
				this.registry[context] = context;
			}
			ListenerAsyncResult listenerAsyncResult = null;
			ArrayList obj = this.wait_queue;
			lock (obj)
			{
				if (this.wait_queue.Count == 0)
				{
					ArrayList obj2 = this.ctx_queue;
					lock (obj2)
					{
						this.ctx_queue.Add(context);
						goto IL_A3;
					}
				}
				listenerAsyncResult = (ListenerAsyncResult)this.wait_queue[0];
				this.wait_queue.RemoveAt(0);
			}
			IL_A3:
			if (listenerAsyncResult != null)
			{
				listenerAsyncResult.Complete(context);
			}
		}

		internal void UnregisterContext(HttpListenerContext context)
		{
			object internalLock = this._internalLock;
			lock (internalLock)
			{
				this.registry.Remove(context);
			}
			ArrayList obj = this.ctx_queue;
			lock (obj)
			{
				int num = this.ctx_queue.IndexOf(context);
				if (num >= 0)
				{
					this.ctx_queue.RemoveAt(num);
				}
			}
		}

		internal void AddConnection(HttpConnection cnc)
		{
			this.connections[cnc] = cnc;
		}

		internal void RemoveConnection(HttpConnection cnc)
		{
			this.connections.Remove(cnc);
		}

		private MonoTlsProvider tlsProvider;

		private MonoTlsSettings tlsSettings;

		private X509Certificate certificate;

		private AuthenticationSchemes auth_schemes;

		private HttpListenerPrefixCollection prefixes;

		private AuthenticationSchemeSelector auth_selector;

		private string realm;

		private bool ignore_write_exceptions;

		private bool unsafe_ntlm_auth;

		private bool listening;

		private bool disposed;

		private readonly object _internalLock;

		private Hashtable registry;

		private ArrayList ctx_queue;

		private ArrayList wait_queue;

		private Hashtable connections;

		private ServiceNameStore defaultServiceNames;

		private ExtendedProtectionPolicy extendedProtectionPolicy;

		private HttpListener.ExtendedProtectionSelector extendedProtectionSelectorDelegate;

		/// <summary>A delegate called to determine the <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> to use for each <see cref="T:System.Net.HttpListener" /> request.</summary>
		/// <param name="request">The <see cref="T:System.Net.HttpListenerRequest" /> to determine the extended protection policy that the <see cref="T:System.Net.HttpListener" /> instance will use to provide extended protection.</param>
		/// <returns>An <see cref="T:System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy" /> object that specifies the extended protection policy to use for this request.</returns>
		public delegate ExtendedProtectionPolicy ExtendedProtectionSelector(HttpListenerRequest request);
	}
}
