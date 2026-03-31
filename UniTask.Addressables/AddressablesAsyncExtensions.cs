using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cysharp.Threading.Tasks
{
	public static class AddressablesAsyncExtensions
	{
		public static UniTask.Awaiter GetAwaiter(this AsyncOperationHandle handle)
		{
			return handle.ToUniTask(null, PlayerLoopTiming.Update, default(CancellationToken)).GetAwaiter();
		}

		public static UniTask WithCancellation(this AsyncOperationHandle handle, CancellationToken cancellationToken)
		{
			return handle.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask ToUniTask(this AsyncOperationHandle handle, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled(cancellationToken);
			}
			if (!handle.IsValid())
			{
				return UniTask.CompletedTask;
			}
			if (!handle.IsDone)
			{
				short token;
				return new UniTask(AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource.Create(handle, timing, progress, cancellationToken, out token), token);
			}
			if (handle.Status == AsyncOperationStatus.Failed)
			{
				return UniTask.FromException(handle.OperationException);
			}
			return UniTask.CompletedTask;
		}

		public static UniTask<T>.Awaiter GetAwaiter<T>(this AsyncOperationHandle<T> handle)
		{
			return handle.ToUniTask(null, PlayerLoopTiming.Update, default(CancellationToken)).GetAwaiter();
		}

		public static UniTask<T> WithCancellation<T>(this AsyncOperationHandle<T> handle, CancellationToken cancellationToken)
		{
			return handle.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
		}

		public static UniTask<T> ToUniTask<T>(this AsyncOperationHandle<T> handle, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return UniTask.FromCanceled<T>(cancellationToken);
			}
			if (!handle.IsValid())
			{
				throw new Exception("Attempting to use an invalid operation handle");
			}
			if (!handle.IsDone)
			{
				short token;
				return new UniTask<T>(AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T>.Create(handle, timing, progress, cancellationToken, out token), token);
			}
			if (handle.Status == AsyncOperationStatus.Failed)
			{
				return UniTask.FromException<T>(handle.OperationException);
			}
			return UniTask.FromResult<T>(handle.Result);
		}

		public struct AsyncOperationHandleAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public AsyncOperationHandleAwaiter(AsyncOperationHandle handle)
			{
				this.handle = handle;
				this.continuationAction = null;
			}

			public bool IsCompleted
			{
				get
				{
					return this.handle.IsDone;
				}
			}

			public void GetResult()
			{
				if (this.continuationAction != null)
				{
					this.handle.Completed -= this.continuationAction;
					this.continuationAction = null;
				}
				if (this.handle.Status == AsyncOperationStatus.Failed)
				{
					Exception operationException = this.handle.OperationException;
					this.handle = default(AsyncOperationHandle);
					ExceptionDispatchInfo.Capture(operationException).Throw();
				}
				object result = this.handle.Result;
				this.handle = default(AsyncOperationHandle);
			}

			public void OnCompleted(Action continuation)
			{
				this.UnsafeOnCompleted(continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				Error.ThrowWhenContinuationIsAlreadyRegistered<Action<AsyncOperationHandle>>(this.continuationAction);
				this.continuationAction = PooledDelegate<AsyncOperationHandle>.Create(continuation);
				this.handle.Completed += this.continuationAction;
			}

			private AsyncOperationHandle handle;

			private Action<AsyncOperationHandle> continuationAction;
		}

		private sealed class AsyncOperationHandleConfiguredSource : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource>
		{
			public ref AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static AsyncOperationHandleConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource), () => AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource.pool.Size);
			}

			private AsyncOperationHandleConfiguredSource()
			{
				this.continuationAction = new Action<AsyncOperationHandle>(this.Continuation);
			}

			public static IUniTaskSource Create(AsyncOperationHandle handle, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
				}
				AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource asyncOperationHandleConfiguredSource;
				if (!AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource.pool.TryPop(out asyncOperationHandleConfiguredSource))
				{
					asyncOperationHandleConfiguredSource = new AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource();
				}
				asyncOperationHandleConfiguredSource.handle = handle;
				asyncOperationHandleConfiguredSource.progress = progress;
				asyncOperationHandleConfiguredSource.cancellationToken = cancellationToken;
				asyncOperationHandleConfiguredSource.completed = false;
				PlayerLoopHelper.AddAction(timing, asyncOperationHandleConfiguredSource);
				handle.Completed += asyncOperationHandleConfiguredSource.continuationAction;
				token = asyncOperationHandleConfiguredSource.core.Version;
				return asyncOperationHandleConfiguredSource;
			}

			private void Continuation(AsyncOperationHandle _)
			{
				this.handle.Completed -= this.continuationAction;
				if (this.completed)
				{
					this.TryReturn();
					return;
				}
				this.completed = true;
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return;
				}
				if (this.handle.Status == AsyncOperationStatus.Failed)
				{
					this.core.TrySetException(this.handle.OperationException);
					return;
				}
				this.core.TrySetResult(AsyncUnit.Default);
			}

			public void GetResult(short token)
			{
				this.core.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public bool MoveNext()
			{
				if (this.completed)
				{
					this.TryReturn();
					return false;
				}
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.completed = true;
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.progress != null && this.handle.IsValid())
				{
					this.progress.Report(this.handle.PercentComplete);
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.handle = default(AsyncOperationHandle);
				this.progress = null;
				this.cancellationToken = default(CancellationToken);
				return AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource.pool.TryPush(this);
			}

			private static TaskPool<AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource> pool;

			private AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource nextNode;

			private readonly Action<AsyncOperationHandle> continuationAction;

			private AsyncOperationHandle handle;

			private CancellationToken cancellationToken;

			private IProgress<float> progress;

			private bool completed;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}

		private sealed class AsyncOperationHandleConfiguredSource<T> : IUniTaskSource<T>, IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T>>
		{
			public ref AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T> NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static AsyncOperationHandleConfiguredSource()
			{
				TaskPool.RegisterSizeGetter(typeof(AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T>), () => AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T>.pool.Size);
			}

			private AsyncOperationHandleConfiguredSource()
			{
				this.continuationAction = new Action<AsyncOperationHandle<T>>(this.Continuation);
			}

			public static IUniTaskSource<T> Create(AsyncOperationHandle<T> handle, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
				}
				AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T> asyncOperationHandleConfiguredSource;
				if (!AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T>.pool.TryPop(out asyncOperationHandleConfiguredSource))
				{
					asyncOperationHandleConfiguredSource = new AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T>();
				}
				asyncOperationHandleConfiguredSource.handle = handle;
				asyncOperationHandleConfiguredSource.cancellationToken = cancellationToken;
				asyncOperationHandleConfiguredSource.completed = false;
				asyncOperationHandleConfiguredSource.progress = progress;
				PlayerLoopHelper.AddAction(timing, asyncOperationHandleConfiguredSource);
				handle.Completed += asyncOperationHandleConfiguredSource.continuationAction;
				token = asyncOperationHandleConfiguredSource.core.Version;
				return asyncOperationHandleConfiguredSource;
			}

			private void Continuation(AsyncOperationHandle<T> argHandle)
			{
				this.handle.Completed -= this.continuationAction;
				if (this.completed)
				{
					this.TryReturn();
					return;
				}
				this.completed = true;
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.core.TrySetCanceled(this.cancellationToken);
					return;
				}
				if (argHandle.Status == AsyncOperationStatus.Failed)
				{
					this.core.TrySetException(argHandle.OperationException);
					return;
				}
				this.core.TrySetResult(argHandle.Result);
			}

			public T GetResult(short token)
			{
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public bool MoveNext()
			{
				if (this.completed)
				{
					this.TryReturn();
					return false;
				}
				if (this.cancellationToken.IsCancellationRequested)
				{
					this.completed = true;
					this.core.TrySetCanceled(this.cancellationToken);
					return false;
				}
				if (this.progress != null && this.handle.IsValid())
				{
					this.progress.Report(this.handle.PercentComplete);
				}
				return true;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.handle = default(AsyncOperationHandle<T>);
				this.progress = null;
				this.cancellationToken = default(CancellationToken);
				return AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T>.pool.TryPush(this);
			}

			private static TaskPool<AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T>> pool;

			private AddressablesAsyncExtensions.AsyncOperationHandleConfiguredSource<T> nextNode;

			private readonly Action<AsyncOperationHandle<T>> continuationAction;

			private AsyncOperationHandle<T> handle;

			private CancellationToken cancellationToken;

			private IProgress<float> progress;

			private bool completed;

			private UniTaskCompletionSourceCore<T> core;
		}
	}
}
