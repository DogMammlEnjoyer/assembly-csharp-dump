using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mono.Net.Security
{
	internal abstract class AsyncProtocolRequest
	{
		public MobileAuthenticatedStream Parent { get; }

		public bool RunSynchronously { get; }

		public int ID
		{
			get
			{
				return ++AsyncProtocolRequest.next_id;
			}
		}

		public string Name
		{
			get
			{
				return base.GetType().Name;
			}
		}

		public int UserResult { get; protected set; }

		public AsyncProtocolRequest(MobileAuthenticatedStream parent, bool sync)
		{
			this.Parent = parent;
			this.RunSynchronously = sync;
		}

		[Conditional("MONO_TLS_DEBUG")]
		protected void Debug(string message, params object[] args)
		{
		}

		internal void RequestRead(int size)
		{
			object obj = this.locker;
			lock (obj)
			{
				this.RequestedSize += size;
			}
		}

		internal void RequestWrite()
		{
			this.WriteRequested = 1;
		}

		internal Task<AsyncProtocolResult> StartOperation(CancellationToken cancellationToken)
		{
			AsyncProtocolRequest.<StartOperation>d__23 <StartOperation>d__;
			<StartOperation>d__.<>4__this = this;
			<StartOperation>d__.cancellationToken = cancellationToken;
			<StartOperation>d__.<>t__builder = AsyncTaskMethodBuilder<AsyncProtocolResult>.Create();
			<StartOperation>d__.<>1__state = -1;
			<StartOperation>d__.<>t__builder.Start<AsyncProtocolRequest.<StartOperation>d__23>(ref <StartOperation>d__);
			return <StartOperation>d__.<>t__builder.Task;
		}

		private Task ProcessOperation(CancellationToken cancellationToken)
		{
			AsyncProtocolRequest.<ProcessOperation>d__24 <ProcessOperation>d__;
			<ProcessOperation>d__.<>4__this = this;
			<ProcessOperation>d__.cancellationToken = cancellationToken;
			<ProcessOperation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessOperation>d__.<>1__state = -1;
			<ProcessOperation>d__.<>t__builder.Start<AsyncProtocolRequest.<ProcessOperation>d__24>(ref <ProcessOperation>d__);
			return <ProcessOperation>d__.<>t__builder.Task;
		}

		private Task<int?> InnerRead(CancellationToken cancellationToken)
		{
			AsyncProtocolRequest.<InnerRead>d__25 <InnerRead>d__;
			<InnerRead>d__.<>4__this = this;
			<InnerRead>d__.cancellationToken = cancellationToken;
			<InnerRead>d__.<>t__builder = AsyncTaskMethodBuilder<int?>.Create();
			<InnerRead>d__.<>1__state = -1;
			<InnerRead>d__.<>t__builder.Start<AsyncProtocolRequest.<InnerRead>d__25>(ref <InnerRead>d__);
			return <InnerRead>d__.<>t__builder.Task;
		}

		protected abstract AsyncOperationStatus Run(AsyncOperationStatus status);

		public override string ToString()
		{
			return string.Format("[{0}]", this.Name);
		}

		private int Started;

		private int RequestedSize;

		private int WriteRequested;

		private readonly object locker = new object();

		private static int next_id;
	}
}
