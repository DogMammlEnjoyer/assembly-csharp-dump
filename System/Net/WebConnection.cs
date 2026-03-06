using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mono.Net.Security;

namespace System.Net
{
	internal class WebConnection : IDisposable
	{
		public ServicePoint ServicePoint { get; }

		public WebConnection(ServicePoint sPoint)
		{
			this.ServicePoint = sPoint;
		}

		[Conditional("MONO_WEB_DEBUG")]
		internal static void Debug(string message, params object[] args)
		{
		}

		[Conditional("MONO_WEB_DEBUG")]
		internal static void Debug(string message)
		{
		}

		private bool CanReuse()
		{
			return !this.socket.Poll(0, SelectMode.SelectRead);
		}

		private bool CheckReusable()
		{
			if (this.socket != null && this.socket.Connected)
			{
				try
				{
					if (this.CanReuse())
					{
						return true;
					}
				}
				catch
				{
				}
				return false;
			}
			return false;
		}

		private Task Connect(WebOperation operation, CancellationToken cancellationToken)
		{
			WebConnection.<Connect>d__16 <Connect>d__;
			<Connect>d__.<>4__this = this;
			<Connect>d__.operation = operation;
			<Connect>d__.cancellationToken = cancellationToken;
			<Connect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Connect>d__.<>1__state = -1;
			<Connect>d__.<>t__builder.Start<WebConnection.<Connect>d__16>(ref <Connect>d__);
			return <Connect>d__.<>t__builder.Task;
		}

		private Task<bool> CreateStream(WebOperation operation, bool reused, CancellationToken cancellationToken)
		{
			WebConnection.<CreateStream>d__18 <CreateStream>d__;
			<CreateStream>d__.<>4__this = this;
			<CreateStream>d__.operation = operation;
			<CreateStream>d__.reused = reused;
			<CreateStream>d__.cancellationToken = cancellationToken;
			<CreateStream>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<CreateStream>d__.<>1__state = -1;
			<CreateStream>d__.<>t__builder.Start<WebConnection.<CreateStream>d__18>(ref <CreateStream>d__);
			return <CreateStream>d__.<>t__builder.Task;
		}

		internal Task<WebRequestStream> InitConnection(WebOperation operation, CancellationToken cancellationToken)
		{
			WebConnection.<InitConnection>d__19 <InitConnection>d__;
			<InitConnection>d__.<>4__this = this;
			<InitConnection>d__.operation = operation;
			<InitConnection>d__.cancellationToken = cancellationToken;
			<InitConnection>d__.<>t__builder = AsyncTaskMethodBuilder<WebRequestStream>.Create();
			<InitConnection>d__.<>1__state = -1;
			<InitConnection>d__.<>t__builder.Start<WebConnection.<InitConnection>d__19>(ref <InitConnection>d__);
			return <InitConnection>d__.<>t__builder.Task;
		}

		internal static WebException GetException(WebExceptionStatus status, Exception error)
		{
			if (error == null)
			{
				return new WebException(string.Format("Error: {0}", status), status);
			}
			WebException ex = error as WebException;
			if (ex != null)
			{
				return ex;
			}
			return new WebException(string.Format("Error: {0} ({1})", status, error.Message), status, WebExceptionInternalStatus.RequestFatal, error);
		}

		internal static bool ReadLine(byte[] buffer, ref int start, int max, ref string output)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (start < max)
			{
				int num2 = start;
				start = num2 + 1;
				num = (int)buffer[num2];
				if (num == 10)
				{
					if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\r')
					{
						StringBuilder stringBuilder2 = stringBuilder;
						num2 = stringBuilder2.Length;
						stringBuilder2.Length = num2 - 1;
					}
					flag = false;
					break;
				}
				if (flag)
				{
					StringBuilder stringBuilder3 = stringBuilder;
					num2 = stringBuilder3.Length;
					stringBuilder3.Length = num2 - 1;
					break;
				}
				if (num == 13)
				{
					flag = true;
				}
				stringBuilder.Append((char)num);
			}
			if (num != 10 && num != 13)
			{
				return false;
			}
			if (stringBuilder.Length == 0)
			{
				output = null;
				return num == 10 || num == 13;
			}
			if (flag)
			{
				StringBuilder stringBuilder4 = stringBuilder;
				int num2 = stringBuilder4.Length;
				stringBuilder4.Length = num2 - 1;
			}
			output = stringBuilder.ToString();
			return true;
		}

		internal bool CanReuseConnection(WebOperation operation)
		{
			bool result;
			lock (this)
			{
				if (this.Closed || this.currentOperation != null)
				{
					result = false;
				}
				else if (!this.NtlmAuthenticated)
				{
					result = true;
				}
				else
				{
					NetworkCredential ntlmCredential = this.NtlmCredential;
					HttpWebRequest request = operation.Request;
					ICredentials credentials = (request.Proxy == null || request.Proxy.IsBypassed(request.RequestUri)) ? request.Credentials : request.Proxy.Credentials;
					NetworkCredential networkCredential = (credentials != null) ? credentials.GetCredential(request.RequestUri, "NTLM") : null;
					if (ntlmCredential == null || networkCredential == null || ntlmCredential.Domain != networkCredential.Domain || ntlmCredential.UserName != networkCredential.UserName || ntlmCredential.Password != networkCredential.Password)
					{
						result = false;
					}
					else
					{
						bool unsafeAuthenticatedConnectionSharing = request.UnsafeAuthenticatedConnectionSharing;
						bool unsafeAuthenticatedConnectionSharing2 = this.UnsafeAuthenticatedConnectionSharing;
						result = (unsafeAuthenticatedConnectionSharing && unsafeAuthenticatedConnectionSharing == unsafeAuthenticatedConnectionSharing2);
					}
				}
			}
			return result;
		}

		private bool PrepareSharingNtlm(WebOperation operation)
		{
			if (operation == null || !this.NtlmAuthenticated)
			{
				return true;
			}
			bool flag = false;
			NetworkCredential ntlmCredential = this.NtlmCredential;
			HttpWebRequest request = operation.Request;
			ICredentials credentials = (request.Proxy == null || request.Proxy.IsBypassed(request.RequestUri)) ? request.Credentials : request.Proxy.Credentials;
			NetworkCredential networkCredential = (credentials != null) ? credentials.GetCredential(request.RequestUri, "NTLM") : null;
			if (ntlmCredential == null || networkCredential == null || ntlmCredential.Domain != networkCredential.Domain || ntlmCredential.UserName != networkCredential.UserName || ntlmCredential.Password != networkCredential.Password)
			{
				flag = true;
			}
			if (!flag)
			{
				bool unsafeAuthenticatedConnectionSharing = request.UnsafeAuthenticatedConnectionSharing;
				bool unsafeAuthenticatedConnectionSharing2 = this.UnsafeAuthenticatedConnectionSharing;
				flag = (!unsafeAuthenticatedConnectionSharing || unsafeAuthenticatedConnectionSharing != unsafeAuthenticatedConnectionSharing2);
			}
			return flag;
		}

		private void Reset()
		{
			lock (this)
			{
				this.tunnel = null;
				this.ResetNtlm();
			}
		}

		private void Close(bool reset)
		{
			lock (this)
			{
				this.CloseSocket();
				if (reset)
				{
					this.Reset();
				}
			}
		}

		private void CloseSocket()
		{
			lock (this)
			{
				if (this.networkStream != null)
				{
					try
					{
						this.networkStream.Dispose();
					}
					catch
					{
					}
					this.networkStream = null;
				}
				if (this.monoTlsStream != null)
				{
					try
					{
						this.monoTlsStream.Dispose();
					}
					catch
					{
					}
					this.monoTlsStream = null;
				}
				if (this.socket != null)
				{
					try
					{
						this.socket.Dispose();
					}
					catch
					{
					}
					this.socket = null;
				}
				this.monoTlsStream = null;
			}
		}

		public bool Closed
		{
			get
			{
				return this.disposed != 0;
			}
		}

		public bool Busy
		{
			get
			{
				return this.currentOperation != null;
			}
		}

		public DateTime IdleSince
		{
			get
			{
				return this.idleSince;
			}
		}

		public bool StartOperation(WebOperation operation, bool reused)
		{
			lock (this)
			{
				if (this.Closed)
				{
					return false;
				}
				if (Interlocked.CompareExchange<WebOperation>(ref this.currentOperation, operation, null) != null)
				{
					return false;
				}
				this.idleSince = DateTime.UtcNow + TimeSpan.FromDays(3650.0);
				if (reused && !this.PrepareSharingNtlm(operation))
				{
					this.Close(true);
				}
				operation.RegisterRequest(this.ServicePoint, this);
			}
			operation.Run();
			return true;
		}

		public bool Continue(WebOperation next)
		{
			lock (this)
			{
				if (this.Closed)
				{
					return false;
				}
				if (this.socket == null || !this.socket.Connected || !this.PrepareSharingNtlm(next))
				{
					this.Close(true);
					return false;
				}
				this.currentOperation = next;
				if (next == null)
				{
					return true;
				}
				next.RegisterRequest(this.ServicePoint, this);
			}
			next.Run();
			return true;
		}

		private void Dispose(bool disposing)
		{
			if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
			{
				return;
			}
			this.Close(true);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void ResetNtlm()
		{
			this.ntlm_authenticated = false;
			this.ntlm_credentials = null;
			this.unsafe_sharing = false;
		}

		internal bool NtlmAuthenticated
		{
			get
			{
				return this.ntlm_authenticated;
			}
			set
			{
				this.ntlm_authenticated = value;
			}
		}

		internal NetworkCredential NtlmCredential
		{
			get
			{
				return this.ntlm_credentials;
			}
			set
			{
				this.ntlm_credentials = value;
			}
		}

		internal bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return this.unsafe_sharing;
			}
			set
			{
				this.unsafe_sharing = value;
			}
		}

		private NetworkCredential ntlm_credentials;

		private bool ntlm_authenticated;

		private bool unsafe_sharing;

		private Stream networkStream;

		private Socket socket;

		private MonoTlsStream monoTlsStream;

		private WebConnectionTunnel tunnel;

		private int disposed;

		internal readonly int ID;

		private DateTime idleSince;

		private WebOperation currentOperation;
	}
}
