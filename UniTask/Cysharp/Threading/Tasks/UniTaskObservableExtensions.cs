using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks
{
	public static class UniTaskObservableExtensions
	{
		public static UniTask<T> ToUniTask<T>(this IObservable<T> source, bool useFirstValue = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			UniTaskCompletionSource<T> uniTaskCompletionSource = new UniTaskCompletionSource<T>();
			SingleAssignmentDisposable singleAssignmentDisposable = new SingleAssignmentDisposable();
			IObserver<T> observer2;
			if (!useFirstValue)
			{
				IObserver<T> observer = new UniTaskObservableExtensions.ToUniTaskObserver<T>(uniTaskCompletionSource, singleAssignmentDisposable, cancellationToken);
				observer2 = observer;
			}
			else
			{
				IObserver<T> observer = new UniTaskObservableExtensions.FirstValueToUniTaskObserver<T>(uniTaskCompletionSource, singleAssignmentDisposable, cancellationToken);
				observer2 = observer;
			}
			IObserver<T> observer3 = observer2;
			try
			{
				singleAssignmentDisposable.Disposable = source.Subscribe(observer3);
			}
			catch (Exception exception)
			{
				uniTaskCompletionSource.TrySetException(exception);
			}
			return uniTaskCompletionSource.Task;
		}

		public static IObservable<T> ToObservable<T>(this UniTask<T> task)
		{
			if (task.Status.IsCompleted())
			{
				try
				{
					return new UniTaskObservableExtensions.ReturnObservable<T>(task.GetAwaiter().GetResult());
				}
				catch (Exception value)
				{
					return new UniTaskObservableExtensions.ThrowObservable<T>(value);
				}
			}
			AsyncSubject<T> asyncSubject = new AsyncSubject<T>();
			UniTaskObservableExtensions.Fire<T>(asyncSubject, task).Forget();
			return asyncSubject;
		}

		public static IObservable<AsyncUnit> ToObservable(this UniTask task)
		{
			if (task.Status.IsCompleted())
			{
				try
				{
					task.GetAwaiter().GetResult();
					return new UniTaskObservableExtensions.ReturnObservable<AsyncUnit>(AsyncUnit.Default);
				}
				catch (Exception value)
				{
					return new UniTaskObservableExtensions.ThrowObservable<AsyncUnit>(value);
				}
			}
			AsyncSubject<AsyncUnit> asyncSubject = new AsyncSubject<AsyncUnit>();
			UniTaskObservableExtensions.Fire(asyncSubject, task).Forget();
			return asyncSubject;
		}

		private static UniTaskVoid Fire<T>(AsyncSubject<T> subject, UniTask<T> task)
		{
			UniTaskObservableExtensions.<Fire>d__3<T> <Fire>d__;
			<Fire>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<Fire>d__.subject = subject;
			<Fire>d__.task = task;
			<Fire>d__.<>1__state = -1;
			<Fire>d__.<>t__builder.Start<UniTaskObservableExtensions.<Fire>d__3<T>>(ref <Fire>d__);
			return <Fire>d__.<>t__builder.Task;
		}

		private static UniTaskVoid Fire(AsyncSubject<AsyncUnit> subject, UniTask task)
		{
			UniTaskObservableExtensions.<Fire>d__4 <Fire>d__;
			<Fire>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<Fire>d__.subject = subject;
			<Fire>d__.task = task;
			<Fire>d__.<>1__state = -1;
			<Fire>d__.<>t__builder.Start<UniTaskObservableExtensions.<Fire>d__4>(ref <Fire>d__);
			return <Fire>d__.<>t__builder.Task;
		}

		private class ToUniTaskObserver<T> : IObserver<T>
		{
			public ToUniTaskObserver(UniTaskCompletionSource<T> promise, SingleAssignmentDisposable disposable, CancellationToken cancellationToken)
			{
				this.promise = promise;
				this.disposable = disposable;
				this.cancellationToken = cancellationToken;
				if (this.cancellationToken.CanBeCanceled)
				{
					this.registration = this.cancellationToken.RegisterWithoutCaptureExecutionContext(UniTaskObservableExtensions.ToUniTaskObserver<T>.callback, this);
				}
			}

			private static void OnCanceled(object state)
			{
				UniTaskObservableExtensions.ToUniTaskObserver<T> toUniTaskObserver = (UniTaskObservableExtensions.ToUniTaskObserver<T>)state;
				toUniTaskObserver.disposable.Dispose();
				toUniTaskObserver.promise.TrySetCanceled(toUniTaskObserver.cancellationToken);
			}

			public void OnNext(T value)
			{
				this.hasValue = true;
				this.latestValue = value;
			}

			public void OnError(Exception error)
			{
				try
				{
					this.promise.TrySetException(error);
				}
				finally
				{
					this.registration.Dispose();
					this.disposable.Dispose();
				}
			}

			public void OnCompleted()
			{
				try
				{
					if (this.hasValue)
					{
						this.promise.TrySetResult(this.latestValue);
					}
					else
					{
						this.promise.TrySetException(new InvalidOperationException("Sequence has no elements"));
					}
				}
				finally
				{
					this.registration.Dispose();
					this.disposable.Dispose();
				}
			}

			private static readonly Action<object> callback = new Action<object>(UniTaskObservableExtensions.ToUniTaskObserver<T>.OnCanceled);

			private readonly UniTaskCompletionSource<T> promise;

			private readonly SingleAssignmentDisposable disposable;

			private readonly CancellationToken cancellationToken;

			private readonly CancellationTokenRegistration registration;

			private bool hasValue;

			private T latestValue;
		}

		private class FirstValueToUniTaskObserver<T> : IObserver<T>
		{
			public FirstValueToUniTaskObserver(UniTaskCompletionSource<T> promise, SingleAssignmentDisposable disposable, CancellationToken cancellationToken)
			{
				this.promise = promise;
				this.disposable = disposable;
				this.cancellationToken = cancellationToken;
				if (this.cancellationToken.CanBeCanceled)
				{
					this.registration = this.cancellationToken.RegisterWithoutCaptureExecutionContext(UniTaskObservableExtensions.FirstValueToUniTaskObserver<T>.callback, this);
				}
			}

			private static void OnCanceled(object state)
			{
				UniTaskObservableExtensions.FirstValueToUniTaskObserver<T> firstValueToUniTaskObserver = (UniTaskObservableExtensions.FirstValueToUniTaskObserver<T>)state;
				firstValueToUniTaskObserver.disposable.Dispose();
				firstValueToUniTaskObserver.promise.TrySetCanceled(firstValueToUniTaskObserver.cancellationToken);
			}

			public void OnNext(T value)
			{
				this.hasValue = true;
				try
				{
					this.promise.TrySetResult(value);
				}
				finally
				{
					this.registration.Dispose();
					this.disposable.Dispose();
				}
			}

			public void OnError(Exception error)
			{
				try
				{
					this.promise.TrySetException(error);
				}
				finally
				{
					this.registration.Dispose();
					this.disposable.Dispose();
				}
			}

			public void OnCompleted()
			{
				try
				{
					if (!this.hasValue)
					{
						this.promise.TrySetException(new InvalidOperationException("Sequence has no elements"));
					}
				}
				finally
				{
					this.registration.Dispose();
					this.disposable.Dispose();
				}
			}

			private static readonly Action<object> callback = new Action<object>(UniTaskObservableExtensions.FirstValueToUniTaskObserver<T>.OnCanceled);

			private readonly UniTaskCompletionSource<T> promise;

			private readonly SingleAssignmentDisposable disposable;

			private readonly CancellationToken cancellationToken;

			private readonly CancellationTokenRegistration registration;

			private bool hasValue;
		}

		private class ReturnObservable<T> : IObservable<T>
		{
			public ReturnObservable(T value)
			{
				this.value = value;
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				observer.OnNext(this.value);
				observer.OnCompleted();
				return EmptyDisposable.Instance;
			}

			private readonly T value;
		}

		private class ThrowObservable<T> : IObservable<T>
		{
			public ThrowObservable(Exception value)
			{
				this.value = value;
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				observer.OnError(this.value);
				return EmptyDisposable.Instance;
			}

			private readonly Exception value;
		}
	}
}
