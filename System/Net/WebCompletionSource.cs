using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class WebCompletionSource<T>
	{
		public WebCompletionSource(bool runAsync = true)
		{
			this.completion = new TaskCompletionSource<WebCompletionSource<T>.Result>(runAsync ? TaskCreationOptions.RunContinuationsAsynchronously : TaskCreationOptions.None);
		}

		internal WebCompletionSource<T>.Result CurrentResult
		{
			get
			{
				return this.currentResult;
			}
		}

		internal WebCompletionSource<T>.Status CurrentStatus
		{
			get
			{
				WebCompletionSource<T>.Result result = this.currentResult;
				if (result == null)
				{
					return WebCompletionSource<T>.Status.Running;
				}
				return result.Status;
			}
		}

		internal Task Task
		{
			get
			{
				return this.completion.Task;
			}
		}

		public bool TrySetCompleted(T argument)
		{
			WebCompletionSource<T>.Result result = new WebCompletionSource<T>.Result(argument);
			return Interlocked.CompareExchange<WebCompletionSource<T>.Result>(ref this.currentResult, result, null) == null && this.completion.TrySetResult(result);
		}

		public bool TrySetCompleted()
		{
			WebCompletionSource<T>.Result result = new WebCompletionSource<T>.Result(WebCompletionSource<T>.Status.Completed, null);
			return Interlocked.CompareExchange<WebCompletionSource<T>.Result>(ref this.currentResult, result, null) == null && this.completion.TrySetResult(result);
		}

		public bool TrySetCanceled()
		{
			return this.TrySetCanceled(new OperationCanceledException());
		}

		public bool TrySetCanceled(OperationCanceledException error)
		{
			WebCompletionSource<T>.Result result = new WebCompletionSource<T>.Result(WebCompletionSource<T>.Status.Canceled, ExceptionDispatchInfo.Capture(error));
			return Interlocked.CompareExchange<WebCompletionSource<T>.Result>(ref this.currentResult, result, null) == null && this.completion.TrySetResult(result);
		}

		public bool TrySetException(Exception error)
		{
			WebCompletionSource<T>.Result result = new WebCompletionSource<T>.Result(WebCompletionSource<T>.Status.Faulted, ExceptionDispatchInfo.Capture(error));
			return Interlocked.CompareExchange<WebCompletionSource<T>.Result>(ref this.currentResult, result, null) == null && this.completion.TrySetResult(result);
		}

		public void ThrowOnError()
		{
			if (!this.completion.Task.IsCompleted)
			{
				return;
			}
			ExceptionDispatchInfo error = this.completion.Task.Result.Error;
			if (error == null)
			{
				return;
			}
			error.Throw();
		}

		public Task<T> WaitForCompletion()
		{
			WebCompletionSource<T>.<WaitForCompletion>d__15 <WaitForCompletion>d__;
			<WaitForCompletion>d__.<>4__this = this;
			<WaitForCompletion>d__.<>t__builder = AsyncTaskMethodBuilder<T>.Create();
			<WaitForCompletion>d__.<>1__state = -1;
			<WaitForCompletion>d__.<>t__builder.Start<WebCompletionSource<T>.<WaitForCompletion>d__15>(ref <WaitForCompletion>d__);
			return <WaitForCompletion>d__.<>t__builder.Task;
		}

		private TaskCompletionSource<WebCompletionSource<T>.Result> completion;

		private WebCompletionSource<T>.Result currentResult;

		internal enum Status
		{
			Running,
			Completed,
			Canceled,
			Faulted
		}

		internal class Result
		{
			public WebCompletionSource<T>.Status Status { get; }

			public bool Success
			{
				get
				{
					return this.Status == WebCompletionSource<T>.Status.Completed;
				}
			}

			public ExceptionDispatchInfo Error { get; }

			public T Argument { get; }

			public Result(T argument)
			{
				this.Status = 1;
				this.Argument = argument;
			}

			public Result(WebCompletionSource<T>.Status state, ExceptionDispatchInfo error)
			{
				this.Status = state;
				this.Error = error;
			}
		}
	}
}
