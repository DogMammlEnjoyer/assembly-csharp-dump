using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fusion.Async
{
	internal class AsyncOperationHandler<T>
	{
		public Task<T> Task
		{
			get
			{
				return this._result.Task;
			}
		}

		public AsyncOperationHandler(CancellationToken externalCancellationToken = default(CancellationToken), float operationTimeout = 30f, string customTimeoutMsg = null)
		{
			this._result = new TaskCompletionSource<T>();
			this._customTimeoutMsg = customTimeoutMsg;
			this._cancellation = new CancellationTokenSource(TimeSpan.FromSeconds((double)operationTimeout));
			this._cancellation.Token.Register(new Action(this.Expire));
			bool flag = externalCancellationToken != default(CancellationToken);
			if (flag)
			{
				externalCancellationToken.Register(new Action(this.Cancel));
			}
		}

		public void SetResult(T result)
		{
			bool flag = this._result.TrySetResult(result);
			if (flag)
			{
				bool flag2 = !this._cancellation.IsCancellationRequested;
				if (flag2)
				{
					this._cancellation.Cancel();
				}
				this._cancellation.Dispose();
			}
		}

		public void SetException(Exception e)
		{
			bool flag = this._result.TrySetException(e);
			if (flag)
			{
				bool flag2 = !this._cancellation.IsCancellationRequested;
				if (flag2)
				{
					this._cancellation.Cancel();
				}
				this._cancellation.Dispose();
			}
		}

		private void Expire()
		{
			this.SetException(new TimeoutException("Operation timed out. " + this._customTimeoutMsg));
		}

		private void Cancel()
		{
			this.SetException(new OperationCanceledException("Operation cancelled."));
		}

		private const float OperationTimeoutSec = 30f;

		private readonly TaskCompletionSource<T> _result;

		private readonly CancellationTokenSource _cancellation;

		private readonly string _customTimeoutMsg;
	}
}
