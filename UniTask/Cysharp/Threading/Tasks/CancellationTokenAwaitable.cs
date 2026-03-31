using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public struct CancellationTokenAwaitable
	{
		public CancellationTokenAwaitable(CancellationToken cancellationToken)
		{
			this.cancellationToken = cancellationToken;
		}

		public CancellationTokenAwaitable.Awaiter GetAwaiter()
		{
			return new CancellationTokenAwaitable.Awaiter(this.cancellationToken);
		}

		private CancellationToken cancellationToken;

		public struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public Awaiter(CancellationToken cancellationToken)
			{
				this.cancellationToken = cancellationToken;
			}

			public bool IsCompleted
			{
				get
				{
					return !this.cancellationToken.CanBeCanceled || this.cancellationToken.IsCancellationRequested;
				}
			}

			public void GetResult()
			{
			}

			public void OnCompleted(Action continuation)
			{
				this.UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				this.cancellationToken.RegisterWithoutCaptureExecutionContext(continuation);
			}

			private CancellationToken cancellationToken;
		}
	}
}
