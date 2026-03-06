using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;

namespace WebSocketSharp.Net
{
	public sealed class HttpListener : IDisposable
	{
		public HttpListener()
		{
			this._authSchemes = AuthenticationSchemes.Anonymous;
			this._contextQueue = new Queue<HttpListenerContext>();
			this._contextRegistry = new LinkedList<HttpListenerContext>();
			this._contextRegistrySync = ((ICollection)this._contextRegistry).SyncRoot;
			this._log = new Logger();
			this._objectName = base.GetType().ToString();
			this._prefixes = new HttpListenerPrefixCollection(this);
			this._waitQueue = new Queue<HttpListenerAsyncResult>();
		}

		internal bool ReuseAddress
		{
			get
			{
				return this._reuseAddress;
			}
			set
			{
				this._reuseAddress = value;
			}
		}

		public AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				return this._authSchemes;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				this._authSchemes = value;
			}
		}

		public Func<HttpListenerRequest, AuthenticationSchemes> AuthenticationSchemeSelector
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				return this._authSchemeSelector;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				this._authSchemeSelector = value;
			}
		}

		public string CertificateFolderPath
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				return this._certFolderPath;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				this._certFolderPath = value;
			}
		}

		public bool IgnoreWriteExceptions
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				return this._ignoreWriteExceptions;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				this._ignoreWriteExceptions = value;
			}
		}

		public bool IsListening
		{
			get
			{
				return this._listening;
			}
		}

		public static bool IsSupported
		{
			get
			{
				return true;
			}
		}

		public Logger Log
		{
			get
			{
				return this._log;
			}
		}

		public HttpListenerPrefixCollection Prefixes
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				return this._prefixes;
			}
		}

		public string Realm
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				return this._realm;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				this._realm = value;
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				bool flag = this._sslConfig == null;
				if (flag)
				{
					this._sslConfig = new ServerSslConfiguration();
				}
				return this._sslConfig;
			}
		}

		public bool UnsafeConnectionNtlmAuthentication
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public Func<IIdentity, NetworkCredential> UserCredentialsFinder
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				return this._userCredFinder;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				this._userCredFinder = value;
			}
		}

		private bool authenticateContext(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;
			AuthenticationSchemes authenticationSchemes = this.selectAuthenticationScheme(request);
			bool flag = authenticationSchemes == AuthenticationSchemes.Anonymous;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = authenticationSchemes == AuthenticationSchemes.None;
				if (flag2)
				{
					string message = "Authentication not allowed";
					context.SendError(403, message);
					result = false;
				}
				else
				{
					string realm = this.getRealm();
					IPrincipal principal = HttpUtility.CreateUser(request.Headers["Authorization"], authenticationSchemes, realm, request.HttpMethod, this._userCredFinder);
					bool flag3 = principal != null && principal.Identity.IsAuthenticated;
					bool flag4 = !flag3;
					if (flag4)
					{
						context.SendAuthenticationChallenge(authenticationSchemes, realm);
						result = false;
					}
					else
					{
						context.User = principal;
						result = true;
					}
				}
			}
			return result;
		}

		private HttpListenerAsyncResult beginGetContext(AsyncCallback callback, object state)
		{
			object contextRegistrySync = this._contextRegistrySync;
			HttpListenerAsyncResult result;
			lock (contextRegistrySync)
			{
				bool flag = !this._listening;
				if (flag)
				{
					string message = this._disposed ? "The listener is closed." : "The listener is stopped.";
					throw new HttpListenerException(995, message);
				}
				HttpListenerAsyncResult httpListenerAsyncResult = new HttpListenerAsyncResult(callback, state);
				bool flag2 = this._contextQueue.Count == 0;
				if (flag2)
				{
					this._waitQueue.Enqueue(httpListenerAsyncResult);
					result = httpListenerAsyncResult;
				}
				else
				{
					HttpListenerContext context = this._contextQueue.Dequeue();
					httpListenerAsyncResult.Complete(context, true);
					result = httpListenerAsyncResult;
				}
			}
			return result;
		}

		private void cleanupContextQueue(bool force)
		{
			bool flag = this._contextQueue.Count == 0;
			if (!flag)
			{
				if (force)
				{
					this._contextQueue.Clear();
				}
				else
				{
					HttpListenerContext[] array = this._contextQueue.ToArray();
					this._contextQueue.Clear();
					foreach (HttpListenerContext httpListenerContext in array)
					{
						httpListenerContext.SendError(503);
					}
				}
			}
		}

		private void cleanupContextRegistry()
		{
			int count = this._contextRegistry.Count;
			bool flag = count == 0;
			if (!flag)
			{
				HttpListenerContext[] array = new HttpListenerContext[count];
				this._contextRegistry.CopyTo(array, 0);
				this._contextRegistry.Clear();
				foreach (HttpListenerContext httpListenerContext in array)
				{
					httpListenerContext.Connection.Close(true);
				}
			}
		}

		private void cleanupWaitQueue(string message)
		{
			bool flag = this._waitQueue.Count == 0;
			if (!flag)
			{
				HttpListenerAsyncResult[] array = this._waitQueue.ToArray();
				this._waitQueue.Clear();
				foreach (HttpListenerAsyncResult httpListenerAsyncResult in array)
				{
					HttpListenerException exception = new HttpListenerException(995, message);
					httpListenerAsyncResult.Complete(exception);
				}
			}
		}

		private void close(bool force)
		{
			bool flag = !this._listening;
			if (flag)
			{
				this._disposed = true;
			}
			else
			{
				this._listening = false;
				this.cleanupContextQueue(force);
				this.cleanupContextRegistry();
				string message = "The listener is closed.";
				this.cleanupWaitQueue(message);
				EndPointManager.RemoveListener(this);
				this._disposed = true;
			}
		}

		private string getRealm()
		{
			string realm = this._realm;
			return (realm != null && realm.Length > 0) ? realm : HttpListener._defaultRealm;
		}

		private bool registerContext(HttpListenerContext context)
		{
			object contextRegistrySync = this._contextRegistrySync;
			bool result;
			lock (contextRegistrySync)
			{
				bool flag = !this._listening;
				if (flag)
				{
					result = false;
				}
				else
				{
					context.Listener = this;
					this._contextRegistry.AddLast(context);
					bool flag2 = this._waitQueue.Count == 0;
					if (flag2)
					{
						this._contextQueue.Enqueue(context);
						result = true;
					}
					else
					{
						HttpListenerAsyncResult httpListenerAsyncResult = this._waitQueue.Dequeue();
						httpListenerAsyncResult.Complete(context, false);
						result = true;
					}
				}
			}
			return result;
		}

		private AuthenticationSchemes selectAuthenticationScheme(HttpListenerRequest request)
		{
			Func<HttpListenerRequest, AuthenticationSchemes> authSchemeSelector = this._authSchemeSelector;
			bool flag = authSchemeSelector == null;
			AuthenticationSchemes result;
			if (flag)
			{
				result = this._authSchemes;
			}
			else
			{
				try
				{
					result = authSchemeSelector(request);
				}
				catch
				{
					result = AuthenticationSchemes.None;
				}
			}
			return result;
		}

		internal void CheckDisposed()
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(this._objectName);
			}
		}

		internal bool RegisterContext(HttpListenerContext context)
		{
			bool flag = !this.authenticateContext(context);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.registerContext(context);
				if (flag2)
				{
					context.SendError(503);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		internal void UnregisterContext(HttpListenerContext context)
		{
			object contextRegistrySync = this._contextRegistrySync;
			lock (contextRegistrySync)
			{
				this._contextRegistry.Remove(context);
			}
		}

		public void Abort()
		{
			bool disposed = this._disposed;
			if (!disposed)
			{
				object contextRegistrySync = this._contextRegistrySync;
				lock (contextRegistrySync)
				{
					bool disposed2 = this._disposed;
					if (!disposed2)
					{
						this.close(true);
					}
				}
			}
		}

		public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(this._objectName);
			}
			bool flag = !this._listening;
			if (flag)
			{
				string message = "The listener has not been started.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = this._prefixes.Count == 0;
			if (flag2)
			{
				string message2 = "The listener has no URI prefix on which listens.";
				throw new InvalidOperationException(message2);
			}
			return this.beginGetContext(callback, state);
		}

		public void Close()
		{
			bool disposed = this._disposed;
			if (!disposed)
			{
				object contextRegistrySync = this._contextRegistrySync;
				lock (contextRegistrySync)
				{
					bool disposed2 = this._disposed;
					if (!disposed2)
					{
						this.close(false);
					}
				}
			}
		}

		public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(this._objectName);
			}
			bool flag = !this._listening;
			if (flag)
			{
				string message = "The listener has not been started.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = asyncResult == null;
			if (flag2)
			{
				throw new ArgumentNullException("asyncResult");
			}
			HttpListenerAsyncResult httpListenerAsyncResult = asyncResult as HttpListenerAsyncResult;
			bool flag3 = httpListenerAsyncResult == null;
			if (flag3)
			{
				string message2 = "A wrong IAsyncResult instance.";
				throw new ArgumentException(message2, "asyncResult");
			}
			object syncRoot = httpListenerAsyncResult.SyncRoot;
			lock (syncRoot)
			{
				bool endCalled = httpListenerAsyncResult.EndCalled;
				if (endCalled)
				{
					string message3 = "This IAsyncResult instance cannot be reused.";
					throw new InvalidOperationException(message3);
				}
				httpListenerAsyncResult.EndCalled = true;
			}
			bool flag4 = !httpListenerAsyncResult.IsCompleted;
			if (flag4)
			{
				httpListenerAsyncResult.AsyncWaitHandle.WaitOne();
			}
			return httpListenerAsyncResult.Context;
		}

		public HttpListenerContext GetContext()
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(this._objectName);
			}
			bool flag = !this._listening;
			if (flag)
			{
				string message = "The listener has not been started.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = this._prefixes.Count == 0;
			if (flag2)
			{
				string message2 = "The listener has no URI prefix on which listens.";
				throw new InvalidOperationException(message2);
			}
			HttpListenerAsyncResult httpListenerAsyncResult = this.beginGetContext(null, null);
			httpListenerAsyncResult.EndCalled = true;
			bool flag3 = !httpListenerAsyncResult.IsCompleted;
			if (flag3)
			{
				httpListenerAsyncResult.AsyncWaitHandle.WaitOne();
			}
			return httpListenerAsyncResult.Context;
		}

		public void Start()
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(this._objectName);
			}
			object contextRegistrySync = this._contextRegistrySync;
			lock (contextRegistrySync)
			{
				bool disposed2 = this._disposed;
				if (disposed2)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				bool listening = this._listening;
				if (!listening)
				{
					EndPointManager.AddListener(this);
					this._listening = true;
				}
			}
		}

		public void Stop()
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(this._objectName);
			}
			object contextRegistrySync = this._contextRegistrySync;
			lock (contextRegistrySync)
			{
				bool disposed2 = this._disposed;
				if (disposed2)
				{
					throw new ObjectDisposedException(this._objectName);
				}
				bool flag = !this._listening;
				if (!flag)
				{
					this._listening = false;
					this.cleanupContextQueue(false);
					this.cleanupContextRegistry();
					string message = "The listener is stopped.";
					this.cleanupWaitQueue(message);
					EndPointManager.RemoveListener(this);
				}
			}
		}

		void IDisposable.Dispose()
		{
			bool disposed = this._disposed;
			if (!disposed)
			{
				object contextRegistrySync = this._contextRegistrySync;
				lock (contextRegistrySync)
				{
					bool disposed2 = this._disposed;
					if (!disposed2)
					{
						this.close(true);
					}
				}
			}
		}

		private AuthenticationSchemes _authSchemes;

		private Func<HttpListenerRequest, AuthenticationSchemes> _authSchemeSelector;

		private string _certFolderPath;

		private Queue<HttpListenerContext> _contextQueue;

		private LinkedList<HttpListenerContext> _contextRegistry;

		private object _contextRegistrySync;

		private static readonly string _defaultRealm = "SECRET AREA";

		private bool _disposed;

		private bool _ignoreWriteExceptions;

		private volatile bool _listening;

		private Logger _log;

		private string _objectName;

		private HttpListenerPrefixCollection _prefixes;

		private string _realm;

		private bool _reuseAddress;

		private ServerSslConfiguration _sslConfig;

		private Func<IIdentity, NetworkCredential> _userCredFinder;

		private Queue<HttpListenerAsyncResult> _waitQueue;
	}
}
