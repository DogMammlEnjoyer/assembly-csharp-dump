using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks
{
	public static class UniTaskExtensions
	{
		public static UniTask<T> AsUniTask<T>(this Task<T> task, bool useCurrentSynchronizationContext = true)
		{
			UniTaskCompletionSource<T> uniTaskCompletionSource = new UniTaskCompletionSource<T>();
			task.ContinueWith(delegate(Task<T> x, object state)
			{
				UniTaskCompletionSource<T> uniTaskCompletionSource2 = (UniTaskCompletionSource<T>)state;
				switch (x.Status)
				{
				case TaskStatus.RanToCompletion:
					uniTaskCompletionSource2.TrySetResult(x.Result);
					return;
				case TaskStatus.Canceled:
					uniTaskCompletionSource2.TrySetCanceled(default(CancellationToken));
					return;
				case TaskStatus.Faulted:
					uniTaskCompletionSource2.TrySetException(x.Exception);
					return;
				default:
					throw new NotSupportedException();
				}
			}, uniTaskCompletionSource, useCurrentSynchronizationContext ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Current);
			return uniTaskCompletionSource.Task;
		}

		public static UniTask AsUniTask(this Task task, bool useCurrentSynchronizationContext = true)
		{
			UniTaskCompletionSource uniTaskCompletionSource = new UniTaskCompletionSource();
			task.ContinueWith(delegate(Task x, object state)
			{
				UniTaskCompletionSource uniTaskCompletionSource2 = (UniTaskCompletionSource)state;
				switch (x.Status)
				{
				case TaskStatus.RanToCompletion:
					uniTaskCompletionSource2.TrySetResult();
					return;
				case TaskStatus.Canceled:
					uniTaskCompletionSource2.TrySetCanceled(default(CancellationToken));
					return;
				case TaskStatus.Faulted:
					uniTaskCompletionSource2.TrySetException(x.Exception);
					return;
				default:
					throw new NotSupportedException();
				}
			}, uniTaskCompletionSource, useCurrentSynchronizationContext ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Current);
			return uniTaskCompletionSource.Task;
		}

		public static Task<T> AsTask<T>(this UniTask<T> task)
		{
			Task<T> result;
			try
			{
				UniTask<T>.Awaiter awaiter;
				try
				{
					awaiter = task.GetAwaiter();
				}
				catch (Exception exception)
				{
					return Task.FromException<T>(exception);
				}
				if (awaiter.IsCompleted)
				{
					try
					{
						return Task.FromResult<T>(awaiter.GetResult());
					}
					catch (Exception exception2)
					{
						return Task.FromException<T>(exception2);
					}
				}
				TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
				awaiter.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<TaskCompletionSource<T>, UniTask<T>.Awaiter> stateTuple = (StateTuple<TaskCompletionSource<T>, UniTask<T>.Awaiter>)state)
					{
						TaskCompletionSource<T> taskCompletionSource2;
						UniTask<T>.Awaiter awaiter2;
						stateTuple.Deconstruct(out taskCompletionSource2, out awaiter2);
						TaskCompletionSource<T> taskCompletionSource3 = taskCompletionSource2;
						UniTask<T>.Awaiter awaiter3 = awaiter2;
						try
						{
							T result2 = awaiter3.GetResult();
							taskCompletionSource3.SetResult(result2);
						}
						catch (Exception exception4)
						{
							taskCompletionSource3.SetException(exception4);
						}
					}
				}, StateTuple.Create<TaskCompletionSource<T>, UniTask<T>.Awaiter>(taskCompletionSource, awaiter));
				result = taskCompletionSource.Task;
			}
			catch (Exception exception3)
			{
				result = Task.FromException<T>(exception3);
			}
			return result;
		}

		public static Task AsTask(this UniTask task)
		{
			Task result;
			try
			{
				UniTask.Awaiter awaiter;
				try
				{
					awaiter = task.GetAwaiter();
				}
				catch (Exception exception)
				{
					return Task.FromException(exception);
				}
				if (awaiter.IsCompleted)
				{
					try
					{
						awaiter.GetResult();
						return Task.CompletedTask;
					}
					catch (Exception exception2)
					{
						return Task.FromException(exception2);
					}
				}
				TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
				awaiter.SourceOnCompleted(delegate(object state)
				{
					using (StateTuple<TaskCompletionSource<object>, UniTask.Awaiter> stateTuple = (StateTuple<TaskCompletionSource<object>, UniTask.Awaiter>)state)
					{
						TaskCompletionSource<object> taskCompletionSource2;
						UniTask.Awaiter awaiter2;
						stateTuple.Deconstruct(out taskCompletionSource2, out awaiter2);
						TaskCompletionSource<object> taskCompletionSource3 = taskCompletionSource2;
						UniTask.Awaiter awaiter3 = awaiter2;
						try
						{
							awaiter3.GetResult();
							taskCompletionSource3.SetResult(null);
						}
						catch (Exception exception4)
						{
							taskCompletionSource3.SetException(exception4);
						}
					}
				}, StateTuple.Create<TaskCompletionSource<object>, UniTask.Awaiter>(taskCompletionSource, awaiter));
				result = taskCompletionSource.Task;
			}
			catch (Exception exception3)
			{
				result = Task.FromException(exception3);
			}
			return result;
		}

		public static AsyncLazy ToAsyncLazy(this UniTask task)
		{
			return new AsyncLazy(task);
		}

		public static AsyncLazy<T> ToAsyncLazy<T>(this UniTask<T> task)
		{
			return new AsyncLazy<T>(task);
		}

		public static UniTask AttachExternalCancellation(this UniTask task, CancellationToken cancellationToken)
		{
			if (!cancellationToken.CanBeCanceled)
			{
				return task;
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled(cancellationToken);
			}
			if (task.Status.IsCompleted())
			{
				return task;
			}
			return new UniTask(new UniTaskExtensions.AttachExternalCancellationSource(task, cancellationToken), 0);
		}

		public static UniTask<T> AttachExternalCancellation<T>(this UniTask<T> task, CancellationToken cancellationToken)
		{
			if (!cancellationToken.CanBeCanceled)
			{
				return task;
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled<T>(cancellationToken);
			}
			if (task.Status.IsCompleted())
			{
				return task;
			}
			return new UniTask<T>(new UniTaskExtensions.AttachExternalCancellationSource<T>(task, cancellationToken), 0);
		}

		public static IEnumerator ToCoroutine<T>(this UniTask<T> task, Action<T> resultHandler = null, Action<Exception> exceptionHandler = null)
		{
			return new UniTaskExtensions.ToCoroutineEnumerator<T>(task, resultHandler, exceptionHandler);
		}

		public static IEnumerator ToCoroutine(this UniTask task, Action<Exception> exceptionHandler = null)
		{
			return new UniTaskExtensions.ToCoroutineEnumerator(task, exceptionHandler);
		}

		public static UniTask Timeout(this UniTask task, TimeSpan timeout, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
		{
			UniTaskExtensions.<Timeout>d__12 <Timeout>d__;
			<Timeout>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<Timeout>d__.task = task;
			<Timeout>d__.timeout = timeout;
			<Timeout>d__.delayType = delayType;
			<Timeout>d__.timeoutCheckTiming = timeoutCheckTiming;
			<Timeout>d__.taskCancellationTokenSource = taskCancellationTokenSource;
			<Timeout>d__.<>1__state = -1;
			<Timeout>d__.<>t__builder.Start<UniTaskExtensions.<Timeout>d__12>(ref <Timeout>d__);
			return <Timeout>d__.<>t__builder.Task;
		}

		public static UniTask<T> Timeout<T>(this UniTask<T> task, TimeSpan timeout, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
		{
			UniTaskExtensions.<Timeout>d__13<T> <Timeout>d__;
			<Timeout>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<Timeout>d__.task = task;
			<Timeout>d__.timeout = timeout;
			<Timeout>d__.delayType = delayType;
			<Timeout>d__.timeoutCheckTiming = timeoutCheckTiming;
			<Timeout>d__.taskCancellationTokenSource = taskCancellationTokenSource;
			<Timeout>d__.<>1__state = -1;
			<Timeout>d__.<>t__builder.Start<UniTaskExtensions.<Timeout>d__13<T>>(ref <Timeout>d__);
			return <Timeout>d__.<>t__builder.Task;
		}

		public static UniTask<bool> TimeoutWithoutException(this UniTask task, TimeSpan timeout, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
		{
			UniTaskExtensions.<TimeoutWithoutException>d__14 <TimeoutWithoutException>d__;
			<TimeoutWithoutException>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
			<TimeoutWithoutException>d__.task = task;
			<TimeoutWithoutException>d__.timeout = timeout;
			<TimeoutWithoutException>d__.delayType = delayType;
			<TimeoutWithoutException>d__.timeoutCheckTiming = timeoutCheckTiming;
			<TimeoutWithoutException>d__.taskCancellationTokenSource = taskCancellationTokenSource;
			<TimeoutWithoutException>d__.<>1__state = -1;
			<TimeoutWithoutException>d__.<>t__builder.Start<UniTaskExtensions.<TimeoutWithoutException>d__14>(ref <TimeoutWithoutException>d__);
			return <TimeoutWithoutException>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"IsTimeout",
			"Result"
		})]
		public static UniTask<ValueTuple<bool, T>> TimeoutWithoutException<T>(this UniTask<T> task, TimeSpan timeout, DelayType delayType = DelayType.DeltaTime, PlayerLoopTiming timeoutCheckTiming = PlayerLoopTiming.Update, CancellationTokenSource taskCancellationTokenSource = null)
		{
			UniTaskExtensions.<TimeoutWithoutException>d__15<T> <TimeoutWithoutException>d__;
			<TimeoutWithoutException>d__.<>t__builder = AsyncUniTaskMethodBuilder<ValueTuple<bool, T>>.Create();
			<TimeoutWithoutException>d__.task = task;
			<TimeoutWithoutException>d__.timeout = timeout;
			<TimeoutWithoutException>d__.delayType = delayType;
			<TimeoutWithoutException>d__.timeoutCheckTiming = timeoutCheckTiming;
			<TimeoutWithoutException>d__.taskCancellationTokenSource = taskCancellationTokenSource;
			<TimeoutWithoutException>d__.<>1__state = -1;
			<TimeoutWithoutException>d__.<>t__builder.Start<UniTaskExtensions.<TimeoutWithoutException>d__15<T>>(ref <TimeoutWithoutException>d__);
			return <TimeoutWithoutException>d__.<>t__builder.Task;
		}

		public static void Forget(this UniTask task)
		{
			UniTask.Awaiter awaiter = task.GetAwaiter();
			if (awaiter.IsCompleted)
			{
				try
				{
					awaiter.GetResult();
					return;
				}
				catch (Exception ex)
				{
					UniTaskScheduler.PublishUnobservedTaskException(ex);
					return;
				}
			}
			awaiter.SourceOnCompleted(delegate(object state)
			{
				using (StateTuple<UniTask.Awaiter> stateTuple = (StateTuple<UniTask.Awaiter>)state)
				{
					try
					{
						stateTuple.Item1.GetResult();
					}
					catch (Exception ex2)
					{
						UniTaskScheduler.PublishUnobservedTaskException(ex2);
					}
				}
			}, StateTuple.Create<UniTask.Awaiter>(awaiter));
		}

		public static void Forget(this UniTask task, Action<Exception> exceptionHandler, bool handleExceptionOnMainThread = true)
		{
			if (exceptionHandler == null)
			{
				task.Forget();
				return;
			}
			UniTaskExtensions.ForgetCoreWithCatch(task, exceptionHandler, handleExceptionOnMainThread).Forget();
		}

		private static UniTaskVoid ForgetCoreWithCatch(UniTask task, Action<Exception> exceptionHandler, bool handleExceptionOnMainThread)
		{
			UniTaskExtensions.<ForgetCoreWithCatch>d__18 <ForgetCoreWithCatch>d__;
			<ForgetCoreWithCatch>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<ForgetCoreWithCatch>d__.task = task;
			<ForgetCoreWithCatch>d__.exceptionHandler = exceptionHandler;
			<ForgetCoreWithCatch>d__.handleExceptionOnMainThread = handleExceptionOnMainThread;
			<ForgetCoreWithCatch>d__.<>1__state = -1;
			<ForgetCoreWithCatch>d__.<>t__builder.Start<UniTaskExtensions.<ForgetCoreWithCatch>d__18>(ref <ForgetCoreWithCatch>d__);
			return <ForgetCoreWithCatch>d__.<>t__builder.Task;
		}

		public static void Forget<T>(this UniTask<T> task)
		{
			UniTask<T>.Awaiter awaiter = task.GetAwaiter();
			if (awaiter.IsCompleted)
			{
				try
				{
					awaiter.GetResult();
					return;
				}
				catch (Exception ex)
				{
					UniTaskScheduler.PublishUnobservedTaskException(ex);
					return;
				}
			}
			awaiter.SourceOnCompleted(delegate(object state)
			{
				using (StateTuple<UniTask<T>.Awaiter> stateTuple = (StateTuple<UniTask<T>.Awaiter>)state)
				{
					try
					{
						stateTuple.Item1.GetResult();
					}
					catch (Exception ex2)
					{
						UniTaskScheduler.PublishUnobservedTaskException(ex2);
					}
				}
			}, StateTuple.Create<UniTask<T>.Awaiter>(awaiter));
		}

		public static void Forget<T>(this UniTask<T> task, Action<Exception> exceptionHandler, bool handleExceptionOnMainThread = true)
		{
			if (exceptionHandler == null)
			{
				task.Forget<T>();
				return;
			}
			UniTaskExtensions.ForgetCoreWithCatch<T>(task, exceptionHandler, handleExceptionOnMainThread).Forget();
		}

		private static UniTaskVoid ForgetCoreWithCatch<T>(UniTask<T> task, Action<Exception> exceptionHandler, bool handleExceptionOnMainThread)
		{
			UniTaskExtensions.<ForgetCoreWithCatch>d__21<T> <ForgetCoreWithCatch>d__;
			<ForgetCoreWithCatch>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<ForgetCoreWithCatch>d__.task = task;
			<ForgetCoreWithCatch>d__.exceptionHandler = exceptionHandler;
			<ForgetCoreWithCatch>d__.handleExceptionOnMainThread = handleExceptionOnMainThread;
			<ForgetCoreWithCatch>d__.<>1__state = -1;
			<ForgetCoreWithCatch>d__.<>t__builder.Start<UniTaskExtensions.<ForgetCoreWithCatch>d__21<T>>(ref <ForgetCoreWithCatch>d__);
			return <ForgetCoreWithCatch>d__.<>t__builder.Task;
		}

		public static UniTask ContinueWith<T>(this UniTask<T> task, Action<T> continuationFunction)
		{
			UniTaskExtensions.<ContinueWith>d__22<T> <ContinueWith>d__;
			<ContinueWith>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ContinueWith>d__.task = task;
			<ContinueWith>d__.continuationFunction = continuationFunction;
			<ContinueWith>d__.<>1__state = -1;
			<ContinueWith>d__.<>t__builder.Start<UniTaskExtensions.<ContinueWith>d__22<T>>(ref <ContinueWith>d__);
			return <ContinueWith>d__.<>t__builder.Task;
		}

		public static UniTask ContinueWith<T>(this UniTask<T> task, Func<T, UniTask> continuationFunction)
		{
			UniTaskExtensions.<ContinueWith>d__23<T> <ContinueWith>d__;
			<ContinueWith>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ContinueWith>d__.task = task;
			<ContinueWith>d__.continuationFunction = continuationFunction;
			<ContinueWith>d__.<>1__state = -1;
			<ContinueWith>d__.<>t__builder.Start<UniTaskExtensions.<ContinueWith>d__23<T>>(ref <ContinueWith>d__);
			return <ContinueWith>d__.<>t__builder.Task;
		}

		public static UniTask<TR> ContinueWith<T, TR>(this UniTask<T> task, Func<T, TR> continuationFunction)
		{
			UniTaskExtensions.<ContinueWith>d__24<T, TR> <ContinueWith>d__;
			<ContinueWith>d__.<>t__builder = AsyncUniTaskMethodBuilder<TR>.Create();
			<ContinueWith>d__.task = task;
			<ContinueWith>d__.continuationFunction = continuationFunction;
			<ContinueWith>d__.<>1__state = -1;
			<ContinueWith>d__.<>t__builder.Start<UniTaskExtensions.<ContinueWith>d__24<T, TR>>(ref <ContinueWith>d__);
			return <ContinueWith>d__.<>t__builder.Task;
		}

		public static UniTask<TR> ContinueWith<T, TR>(this UniTask<T> task, Func<T, UniTask<TR>> continuationFunction)
		{
			UniTaskExtensions.<ContinueWith>d__25<T, TR> <ContinueWith>d__;
			<ContinueWith>d__.<>t__builder = AsyncUniTaskMethodBuilder<TR>.Create();
			<ContinueWith>d__.task = task;
			<ContinueWith>d__.continuationFunction = continuationFunction;
			<ContinueWith>d__.<>1__state = -1;
			<ContinueWith>d__.<>t__builder.Start<UniTaskExtensions.<ContinueWith>d__25<T, TR>>(ref <ContinueWith>d__);
			return <ContinueWith>d__.<>t__builder.Task;
		}

		public static UniTask ContinueWith(this UniTask task, Action continuationFunction)
		{
			UniTaskExtensions.<ContinueWith>d__26 <ContinueWith>d__;
			<ContinueWith>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ContinueWith>d__.task = task;
			<ContinueWith>d__.continuationFunction = continuationFunction;
			<ContinueWith>d__.<>1__state = -1;
			<ContinueWith>d__.<>t__builder.Start<UniTaskExtensions.<ContinueWith>d__26>(ref <ContinueWith>d__);
			return <ContinueWith>d__.<>t__builder.Task;
		}

		public static UniTask ContinueWith(this UniTask task, Func<UniTask> continuationFunction)
		{
			UniTaskExtensions.<ContinueWith>d__27 <ContinueWith>d__;
			<ContinueWith>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<ContinueWith>d__.task = task;
			<ContinueWith>d__.continuationFunction = continuationFunction;
			<ContinueWith>d__.<>1__state = -1;
			<ContinueWith>d__.<>t__builder.Start<UniTaskExtensions.<ContinueWith>d__27>(ref <ContinueWith>d__);
			return <ContinueWith>d__.<>t__builder.Task;
		}

		public static UniTask<T> ContinueWith<T>(this UniTask task, Func<T> continuationFunction)
		{
			UniTaskExtensions.<ContinueWith>d__28<T> <ContinueWith>d__;
			<ContinueWith>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<ContinueWith>d__.task = task;
			<ContinueWith>d__.continuationFunction = continuationFunction;
			<ContinueWith>d__.<>1__state = -1;
			<ContinueWith>d__.<>t__builder.Start<UniTaskExtensions.<ContinueWith>d__28<T>>(ref <ContinueWith>d__);
			return <ContinueWith>d__.<>t__builder.Task;
		}

		public static UniTask<T> ContinueWith<T>(this UniTask task, Func<UniTask<T>> continuationFunction)
		{
			UniTaskExtensions.<ContinueWith>d__29<T> <ContinueWith>d__;
			<ContinueWith>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<ContinueWith>d__.task = task;
			<ContinueWith>d__.continuationFunction = continuationFunction;
			<ContinueWith>d__.<>1__state = -1;
			<ContinueWith>d__.<>t__builder.Start<UniTaskExtensions.<ContinueWith>d__29<T>>(ref <ContinueWith>d__);
			return <ContinueWith>d__.<>t__builder.Task;
		}

		public static UniTask<T> Unwrap<T>(this UniTask<UniTask<T>> task)
		{
			UniTaskExtensions.<Unwrap>d__30<T> <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__30<T>>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask Unwrap(this UniTask<UniTask> task)
		{
			UniTaskExtensions.<Unwrap>d__31 <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__31>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask<T> Unwrap<T>(this Task<UniTask<T>> task)
		{
			UniTaskExtensions.<Unwrap>d__32<T> <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__32<T>>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask<T> Unwrap<T>(this Task<UniTask<T>> task, bool continueOnCapturedContext)
		{
			UniTaskExtensions.<Unwrap>d__33<T> <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.continueOnCapturedContext = continueOnCapturedContext;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__33<T>>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask Unwrap(this Task<UniTask> task)
		{
			UniTaskExtensions.<Unwrap>d__34 <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__34>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask Unwrap(this Task<UniTask> task, bool continueOnCapturedContext)
		{
			UniTaskExtensions.<Unwrap>d__35 <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.continueOnCapturedContext = continueOnCapturedContext;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__35>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask<T> Unwrap<T>(this UniTask<Task<T>> task)
		{
			UniTaskExtensions.<Unwrap>d__36<T> <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__36<T>>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask<T> Unwrap<T>(this UniTask<Task<T>> task, bool continueOnCapturedContext)
		{
			UniTaskExtensions.<Unwrap>d__37<T> <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder<T>.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.continueOnCapturedContext = continueOnCapturedContext;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__37<T>>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask Unwrap(this UniTask<Task> task)
		{
			UniTaskExtensions.<Unwrap>d__38 <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__38>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask Unwrap(this UniTask<Task> task, bool continueOnCapturedContext)
		{
			UniTaskExtensions.<Unwrap>d__39 <Unwrap>d__;
			<Unwrap>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
			<Unwrap>d__.task = task;
			<Unwrap>d__.continueOnCapturedContext = continueOnCapturedContext;
			<Unwrap>d__.<>1__state = -1;
			<Unwrap>d__.<>t__builder.Start<UniTaskExtensions.<Unwrap>d__39>(ref <Unwrap>d__);
			return <Unwrap>d__.<>t__builder.Task;
		}

		public static UniTask.Awaiter GetAwaiter(this UniTask[] tasks)
		{
			return UniTask.WhenAll(tasks).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter(this IEnumerable<UniTask> tasks)
		{
			return UniTask.WhenAll(tasks).GetAwaiter();
		}

		public static UniTask<T[]>.Awaiter GetAwaiter<T>(this UniTask<T>[] tasks)
		{
			return UniTask.WhenAll<T>(tasks).GetAwaiter();
		}

		public static UniTask<T[]>.Awaiter GetAwaiter<T>(this IEnumerable<UniTask<T>> tasks)
		{
			return UniTask.WhenAll<T>(tasks).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2>>.Awaiter GetAwaiter<T1, T2>([TupleElementNames(new string[]
		{
			"task1",
			"task2"
		})] this ValueTuple<UniTask<T1>, UniTask<T2>> tasks)
		{
			return UniTask.WhenAll<T1, T2>(tasks.Item1, tasks.Item2).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3>>.Awaiter GetAwaiter<T1, T2, T3>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3"
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3>(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4>>.Awaiter GetAwaiter<T1, T2, T3, T4>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4"
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5"
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6"
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7"
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			null
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>, ValueTuple<UniTask<T8>>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7, T8>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Rest.Item1).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9>>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			null,
			null
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>, ValueTuple<UniTask<T8>, UniTask<T9>>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Rest.Item1, tasks.Rest.Item2).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10>>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			null,
			null,
			null
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>, ValueTuple<UniTask<T8>, UniTask<T9>, UniTask<T10>>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Rest.Item1, tasks.Rest.Item2, tasks.Rest.Item3).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11>>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>, ValueTuple<UniTask<T8>, UniTask<T9>, UniTask<T10>, UniTask<T11>>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Rest.Item1, tasks.Rest.Item2, tasks.Rest.Item3, tasks.Rest.Item4).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12>>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			"task12",
			null,
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>, ValueTuple<UniTask<T8>, UniTask<T9>, UniTask<T10>, UniTask<T11>, UniTask<T12>>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Rest.Item1, tasks.Rest.Item2, tasks.Rest.Item3, tasks.Rest.Item4, tasks.Rest.Item5).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13>>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			"task12",
			"task13",
			null,
			null,
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>, ValueTuple<UniTask<T8>, UniTask<T9>, UniTask<T10>, UniTask<T11>, UniTask<T12>, UniTask<T13>>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Rest.Item1, tasks.Rest.Item2, tasks.Rest.Item3, tasks.Rest.Item4, tasks.Rest.Item5, tasks.Rest.Item6).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14>>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			"task12",
			"task13",
			"task14",
			null,
			null,
			null,
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>, ValueTuple<UniTask<T8>, UniTask<T9>, UniTask<T10>, UniTask<T11>, UniTask<T12>, UniTask<T13>, UniTask<T14>>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Rest.Item1, tasks.Rest.Item2, tasks.Rest.Item3, tasks.Rest.Item4, tasks.Rest.Item5, tasks.Rest.Item6, tasks.Rest.Item7).GetAwaiter();
		}

		public static UniTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8, T9, T10, T11, T12, T13, T14, ValueTuple<T15>>>>.Awaiter GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			"task12",
			"task13",
			"task14",
			"task15",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask<T1>, UniTask<T2>, UniTask<T3>, UniTask<T4>, UniTask<T5>, UniTask<T6>, UniTask<T7>, ValueTuple<UniTask<T8>, UniTask<T9>, UniTask<T10>, UniTask<T11>, UniTask<T12>, UniTask<T13>, UniTask<T14>, ValueTuple<UniTask<T15>>>> tasks)
		{
			return UniTask.WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Rest.Item1, tasks.Rest.Item2, tasks.Rest.Item3, tasks.Rest.Item4, tasks.Rest.Item5, tasks.Rest.Item6, tasks.Rest.Item7, tasks.Rest.Rest.Item1).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2"
		})] this ValueTuple<UniTask, UniTask> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3"
		})] this ValueTuple<UniTask, UniTask, UniTask> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4"
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5"
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6"
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7"
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			null
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask>> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7,
				tasks.Rest.Item1
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			null,
			null
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask, UniTask>> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7,
				tasks.Rest.Item1,
				tasks.Rest.Item2
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			null,
			null,
			null
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask, UniTask, UniTask>> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7,
				tasks.Rest.Item1,
				tasks.Rest.Item2,
				tasks.Rest.Item3
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask, UniTask, UniTask, UniTask>> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7,
				tasks.Rest.Item1,
				tasks.Rest.Item2,
				tasks.Rest.Item3,
				tasks.Rest.Item4
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			"task12",
			null,
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask>> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7,
				tasks.Rest.Item1,
				tasks.Rest.Item2,
				tasks.Rest.Item3,
				tasks.Rest.Item4,
				tasks.Rest.Item5
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			"task12",
			"task13",
			null,
			null,
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask>> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7,
				tasks.Rest.Item1,
				tasks.Rest.Item2,
				tasks.Rest.Item3,
				tasks.Rest.Item4,
				tasks.Rest.Item5,
				tasks.Rest.Item6
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			"task12",
			"task13",
			"task14",
			null,
			null,
			null,
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask>> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7,
				tasks.Rest.Item1,
				tasks.Rest.Item2,
				tasks.Rest.Item3,
				tasks.Rest.Item4,
				tasks.Rest.Item5,
				tasks.Rest.Item6,
				tasks.Rest.Item7
			}).GetAwaiter();
		}

		public static UniTask.Awaiter GetAwaiter([TupleElementNames(new string[]
		{
			"task1",
			"task2",
			"task3",
			"task4",
			"task5",
			"task6",
			"task7",
			"task8",
			"task9",
			"task10",
			"task11",
			"task12",
			"task13",
			"task14",
			"task15",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		})] this ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, UniTask, ValueTuple<UniTask>>> tasks)
		{
			return UniTask.WhenAll(new UniTask[]
			{
				tasks.Item1,
				tasks.Item2,
				tasks.Item3,
				tasks.Item4,
				tasks.Item5,
				tasks.Item6,
				tasks.Item7,
				tasks.Rest.Item1,
				tasks.Rest.Item2,
				tasks.Rest.Item3,
				tasks.Rest.Item4,
				tasks.Rest.Item5,
				tasks.Rest.Item6,
				tasks.Rest.Item7,
				tasks.Rest.Rest.Item1
			}).GetAwaiter();
		}

		private sealed class AttachExternalCancellationSource : IUniTaskSource
		{
			public AttachExternalCancellationSource(UniTask task, CancellationToken cancellationToken)
			{
				this.cancellationToken = cancellationToken;
				this.tokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(UniTaskExtensions.AttachExternalCancellationSource.cancellationCallbackDelegate, this);
				this.RunTask(task).Forget();
			}

			private UniTaskVoid RunTask(UniTask task)
			{
				UniTaskExtensions.AttachExternalCancellationSource.<RunTask>d__5 <RunTask>d__;
				<RunTask>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<RunTask>d__.<>4__this = this;
				<RunTask>d__.task = task;
				<RunTask>d__.<>1__state = -1;
				<RunTask>d__.<>t__builder.Start<UniTaskExtensions.AttachExternalCancellationSource.<RunTask>d__5>(ref <RunTask>d__);
				return <RunTask>d__.<>t__builder.Task;
			}

			private static void CancellationCallback(object state)
			{
				UniTaskExtensions.AttachExternalCancellationSource attachExternalCancellationSource = (UniTaskExtensions.AttachExternalCancellationSource)state;
				attachExternalCancellationSource.core.TrySetCanceled(attachExternalCancellationSource.cancellationToken);
			}

			public void GetResult(short token)
			{
				this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			private static readonly Action<object> cancellationCallbackDelegate = new Action<object>(UniTaskExtensions.AttachExternalCancellationSource.CancellationCallback);

			private CancellationToken cancellationToken;

			private CancellationTokenRegistration tokenRegistration;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}

		private sealed class AttachExternalCancellationSource<T> : IUniTaskSource<!0>, IUniTaskSource
		{
			public AttachExternalCancellationSource(UniTask<T> task, CancellationToken cancellationToken)
			{
				this.cancellationToken = cancellationToken;
				this.tokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(UniTaskExtensions.AttachExternalCancellationSource<T>.cancellationCallbackDelegate, this);
				this.RunTask(task).Forget();
			}

			private UniTaskVoid RunTask(UniTask<T> task)
			{
				UniTaskExtensions.AttachExternalCancellationSource<T>.<RunTask>d__5 <RunTask>d__;
				<RunTask>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<RunTask>d__.<>4__this = this;
				<RunTask>d__.task = task;
				<RunTask>d__.<>1__state = -1;
				<RunTask>d__.<>t__builder.Start<UniTaskExtensions.AttachExternalCancellationSource<T>.<RunTask>d__5>(ref <RunTask>d__);
				return <RunTask>d__.<>t__builder.Task;
			}

			private static void CancellationCallback(object state)
			{
				UniTaskExtensions.AttachExternalCancellationSource<T> attachExternalCancellationSource = (UniTaskExtensions.AttachExternalCancellationSource<T>)state;
				attachExternalCancellationSource.core.TrySetCanceled(attachExternalCancellationSource.cancellationToken);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.core.GetResult(token);
			}

			public T GetResult(short token)
			{
				return this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			private static readonly Action<object> cancellationCallbackDelegate = new Action<object>(UniTaskExtensions.AttachExternalCancellationSource<T>.CancellationCallback);

			private CancellationToken cancellationToken;

			private CancellationTokenRegistration tokenRegistration;

			private UniTaskCompletionSourceCore<T> core;
		}

		private sealed class ToCoroutineEnumerator : IEnumerator
		{
			public ToCoroutineEnumerator(UniTask task, Action<Exception> exceptionHandler)
			{
				this.completed = false;
				this.exceptionHandler = exceptionHandler;
				this.task = task;
			}

			private UniTaskVoid RunTask(UniTask task)
			{
				UniTaskExtensions.ToCoroutineEnumerator.<RunTask>d__6 <RunTask>d__;
				<RunTask>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<RunTask>d__.<>4__this = this;
				<RunTask>d__.task = task;
				<RunTask>d__.<>1__state = -1;
				<RunTask>d__.<>t__builder.Start<UniTaskExtensions.ToCoroutineEnumerator.<RunTask>d__6>(ref <RunTask>d__);
				return <RunTask>d__.<>t__builder.Task;
			}

			public object Current
			{
				get
				{
					return null;
				}
			}

			public bool MoveNext()
			{
				if (!this.isStarted)
				{
					this.isStarted = true;
					this.RunTask(this.task).Forget();
				}
				if (this.exception != null)
				{
					this.exception.Throw();
					return false;
				}
				return !this.completed;
			}

			void IEnumerator.Reset()
			{
			}

			private bool completed;

			private UniTask task;

			private Action<Exception> exceptionHandler;

			private bool isStarted;

			private ExceptionDispatchInfo exception;
		}

		private sealed class ToCoroutineEnumerator<T> : IEnumerator
		{
			public ToCoroutineEnumerator(UniTask<T> task, Action<T> resultHandler, Action<Exception> exceptionHandler)
			{
				this.completed = false;
				this.task = task;
				this.resultHandler = resultHandler;
				this.exceptionHandler = exceptionHandler;
			}

			private UniTaskVoid RunTask(UniTask<T> task)
			{
				UniTaskExtensions.ToCoroutineEnumerator<T>.<RunTask>d__8 <RunTask>d__;
				<RunTask>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<RunTask>d__.<>4__this = this;
				<RunTask>d__.task = task;
				<RunTask>d__.<>1__state = -1;
				<RunTask>d__.<>t__builder.Start<UniTaskExtensions.ToCoroutineEnumerator<T>.<RunTask>d__8>(ref <RunTask>d__);
				return <RunTask>d__.<>t__builder.Task;
			}

			public object Current
			{
				get
				{
					return this.current;
				}
			}

			public bool MoveNext()
			{
				if (!this.isStarted)
				{
					this.isStarted = true;
					this.RunTask(this.task).Forget();
				}
				if (this.exception != null)
				{
					this.exception.Throw();
					return false;
				}
				return !this.completed;
			}

			void IEnumerator.Reset()
			{
			}

			private bool completed;

			private Action<T> resultHandler;

			private Action<Exception> exceptionHandler;

			private bool isStarted;

			private UniTask<T> task;

			private object current;

			private ExceptionDispatchInfo exception;
		}
	}
}
