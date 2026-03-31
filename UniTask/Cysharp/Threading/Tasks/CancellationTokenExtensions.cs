using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks
{
	public static class CancellationTokenExtensions
	{
		public static CancellationToken ToCancellationToken(this UniTask task)
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CancellationTokenExtensions.ToCancellationTokenCore(task, cancellationTokenSource).Forget();
			return cancellationTokenSource.Token;
		}

		public static CancellationToken ToCancellationToken(this UniTask task, CancellationToken linkToken)
		{
			if (linkToken.IsCancellationRequested)
			{
				return linkToken;
			}
			if (!linkToken.CanBeCanceled)
			{
				return task.ToCancellationToken();
			}
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[]
			{
				linkToken
			});
			CancellationTokenExtensions.ToCancellationTokenCore(task, cancellationTokenSource).Forget();
			return cancellationTokenSource.Token;
		}

		public static CancellationToken ToCancellationToken<T>(this UniTask<T> task)
		{
			return task.AsUniTask().ToCancellationToken();
		}

		public static CancellationToken ToCancellationToken<T>(this UniTask<T> task, CancellationToken linkToken)
		{
			return task.AsUniTask().ToCancellationToken(linkToken);
		}

		private static UniTaskVoid ToCancellationTokenCore(UniTask task, CancellationTokenSource cts)
		{
			CancellationTokenExtensions.<ToCancellationTokenCore>d__6 <ToCancellationTokenCore>d__;
			<ToCancellationTokenCore>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<ToCancellationTokenCore>d__.task = task;
			<ToCancellationTokenCore>d__.cts = cts;
			<ToCancellationTokenCore>d__.<>1__state = -1;
			<ToCancellationTokenCore>d__.<>t__builder.Start<CancellationTokenExtensions.<ToCancellationTokenCore>d__6>(ref <ToCancellationTokenCore>d__);
			return <ToCancellationTokenCore>d__.<>t__builder.Task;
		}

		public static ValueTuple<UniTask, CancellationTokenRegistration> ToUniTask(this CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTuple<UniTask, CancellationTokenRegistration>(UniTask.FromCanceled(cancellationToken), default(CancellationTokenRegistration));
			}
			UniTaskCompletionSource uniTaskCompletionSource = new UniTaskCompletionSource();
			return new ValueTuple<UniTask, CancellationTokenRegistration>(uniTaskCompletionSource.Task, cancellationToken.RegisterWithoutCaptureExecutionContext(CancellationTokenExtensions.cancellationTokenCallback, uniTaskCompletionSource));
		}

		private static void Callback(object state)
		{
			((UniTaskCompletionSource)state).TrySetResult();
		}

		public static CancellationTokenAwaitable WaitUntilCanceled(this CancellationToken cancellationToken)
		{
			return new CancellationTokenAwaitable(cancellationToken);
		}

		public static CancellationTokenRegistration RegisterWithoutCaptureExecutionContext(this CancellationToken cancellationToken, Action callback)
		{
			bool flag = false;
			if (!ExecutionContext.IsFlowSuppressed())
			{
				ExecutionContext.SuppressFlow();
				flag = true;
			}
			CancellationTokenRegistration result;
			try
			{
				result = cancellationToken.Register(callback, false);
			}
			finally
			{
				if (flag)
				{
					ExecutionContext.RestoreFlow();
				}
			}
			return result;
		}

		public static CancellationTokenRegistration RegisterWithoutCaptureExecutionContext(this CancellationToken cancellationToken, Action<object> callback, object state)
		{
			bool flag = false;
			if (!ExecutionContext.IsFlowSuppressed())
			{
				ExecutionContext.SuppressFlow();
				flag = true;
			}
			CancellationTokenRegistration result;
			try
			{
				result = cancellationToken.Register(callback, state, false);
			}
			finally
			{
				if (flag)
				{
					ExecutionContext.RestoreFlow();
				}
			}
			return result;
		}

		public static CancellationTokenRegistration AddTo(this IDisposable disposable, CancellationToken cancellationToken)
		{
			return cancellationToken.RegisterWithoutCaptureExecutionContext(CancellationTokenExtensions.disposeCallback, disposable);
		}

		private static void DisposeCallback(object state)
		{
			((IDisposable)state).Dispose();
		}

		private static readonly Action<object> cancellationTokenCallback = new Action<object>(CancellationTokenExtensions.Callback);

		private static readonly Action<object> disposeCallback = new Action<object>(CancellationTokenExtensions.DisposeCallback);
	}
}
