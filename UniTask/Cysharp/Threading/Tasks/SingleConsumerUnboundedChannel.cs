using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	internal class SingleConsumerUnboundedChannel<T> : Channel<T>
	{
		public SingleConsumerUnboundedChannel()
		{
			this.items = new Queue<T>();
			base.Writer = new SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelWriter(this);
			this.readerSource = new SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader(this);
			base.Reader = this.readerSource;
		}

		private readonly Queue<T> items;

		private readonly SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader readerSource;

		private UniTaskCompletionSource completedTaskSource;

		private UniTask completedTask;

		private Exception completionError;

		private bool closed;

		private sealed class SingleConsumerUnboundedChannelWriter : ChannelWriter<T>
		{
			public SingleConsumerUnboundedChannelWriter(SingleConsumerUnboundedChannel<T> parent)
			{
				this.parent = parent;
			}

			public override bool TryWrite(T item)
			{
				Queue<T> items = this.parent.items;
				bool isWaiting;
				lock (items)
				{
					if (this.parent.closed)
					{
						return false;
					}
					this.parent.items.Enqueue(item);
					isWaiting = this.parent.readerSource.isWaiting;
				}
				if (isWaiting)
				{
					this.parent.readerSource.SingalContinuation();
				}
				return true;
			}

			public override bool TryComplete(Exception error = null)
			{
				Queue<T> items = this.parent.items;
				lock (items)
				{
					if (this.parent.closed)
					{
						return false;
					}
					this.parent.closed = true;
					bool isWaiting = this.parent.readerSource.isWaiting;
					if (this.parent.items.Count == 0)
					{
						if (error == null)
						{
							if (this.parent.completedTaskSource != null)
							{
								this.parent.completedTaskSource.TrySetResult();
							}
							else
							{
								this.parent.completedTask = UniTask.CompletedTask;
							}
						}
						else if (this.parent.completedTaskSource != null)
						{
							this.parent.completedTaskSource.TrySetException(error);
						}
						else
						{
							this.parent.completedTask = UniTask.FromException(error);
						}
						if (isWaiting)
						{
							this.parent.readerSource.SingalCompleted(error);
						}
					}
					this.parent.completionError = error;
				}
				return true;
			}

			private readonly SingleConsumerUnboundedChannel<T> parent;
		}

		private sealed class SingleConsumerUnboundedChannelReader : ChannelReader<T>, IUniTaskSource<bool>, IUniTaskSource
		{
			public SingleConsumerUnboundedChannelReader(SingleConsumerUnboundedChannel<T> parent)
			{
				this.parent = parent;
			}

			public override UniTask Completion
			{
				get
				{
					if (this.parent.completedTaskSource != null)
					{
						return this.parent.completedTaskSource.Task;
					}
					if (this.parent.closed)
					{
						return this.parent.completedTask;
					}
					this.parent.completedTaskSource = new UniTaskCompletionSource();
					return this.parent.completedTaskSource.Task;
				}
			}

			public override bool TryRead(out T item)
			{
				Queue<T> items = this.parent.items;
				lock (items)
				{
					if (this.parent.items.Count == 0)
					{
						item = default(T);
						return false;
					}
					item = this.parent.items.Dequeue();
					if (this.parent.closed && this.parent.items.Count == 0)
					{
						if (this.parent.completionError != null)
						{
							if (this.parent.completedTaskSource != null)
							{
								this.parent.completedTaskSource.TrySetException(this.parent.completionError);
							}
							else
							{
								this.parent.completedTask = UniTask.FromException(this.parent.completionError);
							}
						}
						else if (this.parent.completedTaskSource != null)
						{
							this.parent.completedTaskSource.TrySetResult();
						}
						else
						{
							this.parent.completedTask = UniTask.CompletedTask;
						}
					}
				}
				return true;
			}

			public override UniTask<bool> WaitToReadAsync(CancellationToken cancellationToken)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return UniTask.FromCanceled<bool>(cancellationToken);
				}
				Queue<T> items = this.parent.items;
				UniTask<bool> result;
				lock (items)
				{
					if (this.parent.items.Count != 0)
					{
						result = CompletedTasks.True;
					}
					else if (this.parent.closed)
					{
						if (this.parent.completionError == null)
						{
							result = CompletedTasks.False;
						}
						else
						{
							result = UniTask.FromException<bool>(this.parent.completionError);
						}
					}
					else
					{
						this.cancellationTokenRegistration.Dispose();
						this.core.Reset();
						this.isWaiting = true;
						this.cancellationToken = cancellationToken;
						if (this.cancellationToken.CanBeCanceled)
						{
							this.cancellationTokenRegistration = this.cancellationToken.RegisterWithoutCaptureExecutionContext(this.CancellationCallbackDelegate, this);
						}
						result = new UniTask<bool>(this, this.core.Version);
					}
				}
				return result;
			}

			public void SingalContinuation()
			{
				this.core.TrySetResult(true);
			}

			public void SingalCancellation(CancellationToken cancellationToken)
			{
				this.core.TrySetCanceled(cancellationToken);
			}

			public void SingalCompleted(Exception error)
			{
				if (error != null)
				{
					this.core.TrySetException(error);
					return;
				}
				this.core.TrySetResult(false);
			}

			public override IUniTaskAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default(CancellationToken))
			{
				return new SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader.ReadAllAsyncEnumerable(this, cancellationToken);
			}

			bool IUniTaskSource<bool>.GetResult(short token)
			{
				return this.core.GetResult(token);
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.core.GetResult(token);
			}

			UniTaskStatus IUniTaskSource.GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			UniTaskStatus IUniTaskSource.UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			private static void CancellationCallback(object state)
			{
				SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader singleConsumerUnboundedChannelReader = (SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader)state;
				singleConsumerUnboundedChannelReader.SingalCancellation(singleConsumerUnboundedChannelReader.cancellationToken);
			}

			private readonly Action<object> CancellationCallbackDelegate = new Action<object>(SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader.CancellationCallback);

			private readonly SingleConsumerUnboundedChannel<T> parent;

			private CancellationToken cancellationToken;

			private CancellationTokenRegistration cancellationTokenRegistration;

			private UniTaskCompletionSourceCore<bool> core;

			internal bool isWaiting;

			private sealed class ReadAllAsyncEnumerable : IUniTaskAsyncEnumerable<T>, IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable
			{
				public ReadAllAsyncEnumerable(SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader parent, CancellationToken cancellationToken)
				{
					this.parent = parent;
					this.cancellationToken1 = cancellationToken;
				}

				public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
				{
					if (this.running)
					{
						throw new InvalidOperationException("Enumerator is already running, does not allow call GetAsyncEnumerator twice.");
					}
					if (this.cancellationToken1 != cancellationToken)
					{
						this.cancellationToken2 = cancellationToken;
					}
					if (this.cancellationToken1.CanBeCanceled)
					{
						this.cancellationTokenRegistration1 = this.cancellationToken1.RegisterWithoutCaptureExecutionContext(this.CancellationCallback1Delegate, this);
					}
					if (this.cancellationToken2.CanBeCanceled)
					{
						this.cancellationTokenRegistration2 = this.cancellationToken2.RegisterWithoutCaptureExecutionContext(this.CancellationCallback2Delegate, this);
					}
					this.running = true;
					return this;
				}

				public T Current
				{
					get
					{
						if (this.cacheValue)
						{
							return this.current;
						}
						this.parent.TryRead(out this.current);
						return this.current;
					}
				}

				public UniTask<bool> MoveNextAsync()
				{
					this.cacheValue = false;
					return this.parent.WaitToReadAsync(CancellationToken.None);
				}

				public UniTask DisposeAsync()
				{
					this.cancellationTokenRegistration1.Dispose();
					this.cancellationTokenRegistration2.Dispose();
					return default(UniTask);
				}

				private static void CancellationCallback1(object state)
				{
					SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader.ReadAllAsyncEnumerable readAllAsyncEnumerable = (SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader.ReadAllAsyncEnumerable)state;
					readAllAsyncEnumerable.parent.SingalCancellation(readAllAsyncEnumerable.cancellationToken1);
				}

				private static void CancellationCallback2(object state)
				{
					SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader.ReadAllAsyncEnumerable readAllAsyncEnumerable = (SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader.ReadAllAsyncEnumerable)state;
					readAllAsyncEnumerable.parent.SingalCancellation(readAllAsyncEnumerable.cancellationToken2);
				}

				private readonly Action<object> CancellationCallback1Delegate = new Action<object>(SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader.ReadAllAsyncEnumerable.CancellationCallback1);

				private readonly Action<object> CancellationCallback2Delegate = new Action<object>(SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader.ReadAllAsyncEnumerable.CancellationCallback2);

				private readonly SingleConsumerUnboundedChannel<T>.SingleConsumerUnboundedChannelReader parent;

				private CancellationToken cancellationToken1;

				private CancellationToken cancellationToken2;

				private CancellationTokenRegistration cancellationTokenRegistration1;

				private CancellationTokenRegistration cancellationTokenRegistration2;

				private T current;

				private bool cacheValue;

				private bool running;
			}
		}
	}
}
