using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class BufferSkip<TSource> : IUniTaskAsyncEnumerable<IList<TSource>>
	{
		public BufferSkip(IUniTaskAsyncEnumerable<TSource> source, int count, int skip)
		{
			this.source = source;
			this.count = count;
			this.skip = skip;
		}

		public IUniTaskAsyncEnumerator<IList<TSource>> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new BufferSkip<TSource>._BufferSkip(this.source, this.count, this.skip, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly int count;

		private readonly int skip;

		private sealed class _BufferSkip : MoveNextSource, IUniTaskAsyncEnumerator<IList<TSource>>, IUniTaskAsyncDisposable
		{
			public _BufferSkip(IUniTaskAsyncEnumerable<TSource> source, int count, int skip, CancellationToken cancellationToken)
			{
				this.source = source;
				this.count = count;
				this.skip = skip;
				this.cancellationToken = cancellationToken;
			}

			public IList<TSource> Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.enumerator == null)
				{
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
					this.buffers = new Queue<List<TSource>>();
				}
				this.completionSource.Reset();
				this.SourceMoveNext();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void SourceMoveNext()
			{
				if (!this.completed)
				{
					try
					{
						for (;;)
						{
							this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
							if (!this.awaiter.IsCompleted)
							{
								break;
							}
							this.continueNext = true;
							BufferSkip<TSource>._BufferSkip.MoveNextCore(this);
							if (!this.continueNext)
							{
								goto IL_99;
							}
							this.continueNext = false;
						}
						this.awaiter.SourceOnCompleted(BufferSkip<TSource>._BufferSkip.MoveNextCoreDelegate, this);
						IL_99:;
					}
					catch (Exception error)
					{
						this.completionSource.TrySetException(error);
					}
					return;
				}
				if (this.buffers.Count > 0)
				{
					this.Current = this.buffers.Dequeue();
					this.completionSource.TrySetResult(true);
					return;
				}
				this.completionSource.TrySetResult(false);
			}

			private static void MoveNextCore(object state)
			{
				BufferSkip<TSource>._BufferSkip bufferSkip = (BufferSkip<TSource>._BufferSkip)state;
				bool flag;
				if (bufferSkip.TryGetResult<bool>(bufferSkip.awaiter, out flag))
				{
					if (!flag)
					{
						bufferSkip.continueNext = false;
						bufferSkip.completed = true;
						bufferSkip.SourceMoveNext();
						return;
					}
					BufferSkip<TSource>._BufferSkip bufferSkip2 = bufferSkip;
					int num = bufferSkip2.index;
					bufferSkip2.index = num + 1;
					if (num % bufferSkip.skip == 0)
					{
						bufferSkip.buffers.Enqueue(new List<TSource>(bufferSkip.count));
					}
					TSource item = bufferSkip.enumerator.Current;
					foreach (List<TSource> list in bufferSkip.buffers)
					{
						list.Add(item);
					}
					if (bufferSkip.buffers.Count > 0 && bufferSkip.buffers.Peek().Count == bufferSkip.count)
					{
						bufferSkip.Current = bufferSkip.buffers.Dequeue();
						bufferSkip.continueNext = false;
						bufferSkip.completionSource.TrySetResult(true);
						return;
					}
					if (!bufferSkip.continueNext)
					{
						bufferSkip.SourceMoveNext();
						return;
					}
				}
				else
				{
					bufferSkip.continueNext = false;
				}
			}

			public UniTask DisposeAsync()
			{
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(BufferSkip<TSource>._BufferSkip.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly int count;

			private readonly int skip;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private bool continueNext;

			private bool completed;

			private Queue<List<TSource>> buffers;

			private int index;
		}
	}
}
