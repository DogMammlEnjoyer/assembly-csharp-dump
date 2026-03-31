using System;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public class AsyncLazy
	{
		public AsyncLazy(Func<UniTask> taskFactory)
		{
			this.taskFactory = taskFactory;
			this.completionSource = new UniTaskCompletionSource();
			this.syncLock = new object();
			this.initialized = false;
		}

		internal AsyncLazy(UniTask task)
		{
			this.taskFactory = null;
			this.completionSource = new UniTaskCompletionSource();
			this.syncLock = null;
			this.initialized = true;
			UniTask.Awaiter awaiter = task.GetAwaiter();
			if (awaiter.IsCompleted)
			{
				this.SetCompletionSource(awaiter);
				return;
			}
			this.awaiter = awaiter;
			awaiter.SourceOnCompleted(AsyncLazy.continuation, this);
		}

		public UniTask Task
		{
			get
			{
				this.EnsureInitialized();
				return this.completionSource.Task;
			}
		}

		public UniTask.Awaiter GetAwaiter()
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
					Func<UniTask> func = Interlocked.Exchange<Func<UniTask>>(ref this.taskFactory, null);
					if (func != null)
					{
						UniTask.Awaiter awaiter = func().GetAwaiter();
						if (awaiter.IsCompleted)
						{
							this.SetCompletionSource(awaiter);
						}
						else
						{
							this.awaiter = awaiter;
							awaiter.SourceOnCompleted(AsyncLazy.continuation, this);
						}
						Volatile.Write(ref this.initialized, true);
					}
				}
			}
		}

		private void SetCompletionSource(in UniTask.Awaiter awaiter)
		{
			try
			{
				awaiter.GetResult();
				this.completionSource.TrySetResult();
			}
			catch (Exception exception)
			{
				this.completionSource.TrySetException(exception);
			}
		}

		private static void SetCompletionSource(object state)
		{
			AsyncLazy asyncLazy = (AsyncLazy)state;
			try
			{
				asyncLazy.awaiter.GetResult();
				asyncLazy.completionSource.TrySetResult();
			}
			catch (Exception exception)
			{
				asyncLazy.completionSource.TrySetException(exception);
			}
			finally
			{
				asyncLazy.awaiter = default(UniTask.Awaiter);
			}
		}

		private static Action<object> continuation = new Action<object>(AsyncLazy.SetCompletionSource);

		private Func<UniTask> taskFactory;

		private UniTaskCompletionSource completionSource;

		private UniTask.Awaiter awaiter;

		private object syncLock;

		private bool initialized;
	}
}
