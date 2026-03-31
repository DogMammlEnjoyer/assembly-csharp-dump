using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Create<T> : IUniTaskAsyncEnumerable<T>
	{
		public Create(Func<IAsyncWriter<T>, CancellationToken, UniTask> create)
		{
			this.create = create;
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Create<T>._Create(this.create, cancellationToken);
		}

		private readonly Func<IAsyncWriter<T>, CancellationToken, UniTask> create;

		private sealed class _Create : MoveNextSource, IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable
		{
			public _Create(Func<IAsyncWriter<T>, CancellationToken, UniTask> create, CancellationToken cancellationToken)
			{
				this.create = create;
				this.cancellationToken = cancellationToken;
			}

			public T Current { get; private set; }

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			public UniTask<bool> MoveNextAsync()
			{
				if (this.state == -2)
				{
					return default(UniTask<bool>);
				}
				this.completionSource.Reset();
				this.MoveNext();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void MoveNext()
			{
				try
				{
					int num = this.state;
					if (num != -1)
					{
						if (num == 0)
						{
							this.writer.SignalWriter();
							return;
						}
					}
					else
					{
						this.writer = new Create<T>.AsyncWriter(this);
						this.RunWriterTask(this.create(this.writer, this.cancellationToken)).Forget();
						if (Volatile.Read(ref this.state) == -2)
						{
							return;
						}
						this.state = 0;
						return;
					}
				}
				catch (Exception error)
				{
					this.state = -2;
					this.completionSource.TrySetException(error);
					return;
				}
				this.state = -2;
				this.completionSource.TrySetResult(false);
			}

			private UniTaskVoid RunWriterTask(UniTask task)
			{
				Create<T>._Create.<RunWriterTask>d__12 <RunWriterTask>d__;
				<RunWriterTask>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<RunWriterTask>d__.<>4__this = this;
				<RunWriterTask>d__.task = task;
				<RunWriterTask>d__.<>1__state = -1;
				<RunWriterTask>d__.<>t__builder.Start<Create<T>._Create.<RunWriterTask>d__12>(ref <RunWriterTask>d__);
				return <RunWriterTask>d__.<>t__builder.Task;
			}

			public void SetResult(T value)
			{
				this.Current = value;
				this.completionSource.TrySetResult(true);
			}

			private readonly Func<IAsyncWriter<T>, CancellationToken, UniTask> create;

			private readonly CancellationToken cancellationToken;

			private int state = -1;

			private Create<T>.AsyncWriter writer;
		}

		private sealed class AsyncWriter : IUniTaskSource, IAsyncWriter<T>
		{
			public AsyncWriter(Create<T>._Create enumerator)
			{
				this.enumerator = enumerator;
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

			public UniTask YieldAsync(T value)
			{
				this.core.Reset();
				this.enumerator.SetResult(value);
				return new UniTask(this, this.core.Version);
			}

			public void SignalWriter()
			{
				this.core.TrySetResult(AsyncUnit.Default);
			}

			private readonly Create<T>._Create enumerator;

			private UniTaskCompletionSourceCore<AsyncUnit> core;
		}
	}
}
