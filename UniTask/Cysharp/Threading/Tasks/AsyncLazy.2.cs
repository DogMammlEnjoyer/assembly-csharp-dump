using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public class AsyncLazy<T>
	{
		public AsyncLazy(Func<UniTask<T>> taskFactory)
		{
			this.taskFactory = taskFactory;
			this.completionSource = new UniTaskCompletionSource<T>();
			this.syncLock = new object();
			this.initialized = false;
		}

		internal AsyncLazy(UniTask<T> task)
		{
			this.taskFactory = null;
			this.completionSource = new UniTaskCompletionSource<T>();
			this.syncLock = null;
			this.initialized = true;
			UniTask<T>.Awaiter awaiter = task.GetAwaiter();
			if (awaiter.IsCompleted)
			{
				this.SetCompletionSource(awaiter);
				return;
			}
			this.awaiter = awaiter;
			awaiter.SourceOnCompleted(AsyncLazy<T>.continuation, this);
		}

		public UniTask<T> Task
		{
			get
			{
				this.EnsureInitialized();
				return this.completionSource.Task;
			}
		}

		public UniTask<T>.Awaiter GetAwaiter()
		{
			return this.Task.GetAwaiter();
		}

		private void EnsureInitialized()
		{
			if (Volatile.Read(ref this.initialized))
			{
				return;
			}
			this.EnsureInitializedCore();
		}

		private void EnsureInitializedCore()
		{
			object obj = this.syncLock;
			lock (obj)
			{
				if (!Volatile.Read(ref this.initialized))
				{
					Func<UniTask<T>> func = Interlocked.Exchange<Func<UniTask<T>>>(ref this.taskFactory, null);
					if (func != null)
					{
						UniTask<T>.Awaiter awaiter = func().GetAwaiter();
						if (awaiter.IsCompleted)
						{
							this.SetCompletionSource(awaiter);
						}
						else
						{
							this.awaiter = awaiter;
							awaiter.SourceOnCompleted(AsyncLazy<T>.continuation, this);
						}
						Volatile.Write(ref this.initialized, true);
					}
				}
			}
		}

		private void SetCompletionSource(in UniTask<T>.Awaiter awaiter)
		{
			try
			{
				T result = awaiter.GetResult();
				this.completionSource.TrySetResult(result);
			}
			catch (Exception exception)
			{
				this.completionSource.TrySetException(exception);
			}
		}

		private static void SetCompletionSource(object state)
		{
			AsyncLazy<T> asyncLazy = (AsyncLazy<T>)state;
			try
			{
				T result = asyncLazy.awaiter.GetResult();
				asyncLazy.completionSource.TrySetResult(result);
			}
			catch (Exception exception)
			{
				asyncLazy.completionSource.TrySetException(exception);
			}
			finally
			{
				asyncLazy.awaiter = default(UniTask<T>.Awaiter);
			}
		}

		private static Action<object> continuation = new Action<object>(AsyncLazy<T>.SetCompletionSource);

		private Func<UniTask<T>> taskFactory;

		private UniTaskCompletionSource<T> completionSource;

		private UniTask<T>.Awaiter awaiter;

		private object syncLock;

		private bool initialized;
	}
}
