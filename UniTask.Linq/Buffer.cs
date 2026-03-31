using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Buffer<TSource> : IUniTaskAsyncEnumerable<IList<TSource>>
	{
		public Buffer(IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			this.source = source;
			this.count = count;
		}

		public IUniTaskAsyncEnumerator<IList<TSource>> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Buffer<TSource>._Buffer(this.source, this.count, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly int count;

		private sealed class _Buffer : MoveNextSource, IUniTaskAsyncEnumerator<IList<TSource>>, IUniTaskAsyncDisposable
		{
			public _Buffer(IUniTaskAsyncEnumerable<TSource> source, int count, CancellationToken cancellationToken)
			{
				this.source = source;
				this.count = count;
				this.cancellationToken = cancellationToken;
			}

			public IList<TSource> Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.enumerator == null)
				{
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
					this.buffer = new List<TSource>(this.count);
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
							Buffer<TSource>._Buffer.MoveNextCore(this);
							if (!this.continueNext)
							{
								goto IL_A5;
							}
							this.continueNext = false;
						}
						this.awaiter.SourceOnCompleted(Buffer<TSource>._Buffer.MoveNextCoreDelegate, this);
						IL_A5:;
					}
					catch (Exception error)
					{
						this.completionSource.TrySetException(error);
					}
					return;
				}
				if (this.buffer != null && this.buffer.Count > 0)
				{
					List<TSource> value = this.buffer;
					this.buffer = null;
					this.Current = value;
					this.completionSource.TrySetResult(true);
					return;
				}
				this.completionSource.TrySetResult(false);
			}

			private static void MoveNextCore(object state)
			{
				Buffer<TSource>._Buffer buffer = (Buffer<TSource>._Buffer)state;
				bool flag;
				if (buffer.TryGetResult<bool>(buffer.awaiter, out flag))
				{
					if (!flag)
					{
						buffer.continueNext = false;
						buffer.completed = true;
						buffer.SourceMoveNext();
						return;
					}
					buffer.buffer.Add(buffer.enumerator.Current);
					if (buffer.buffer.Count == buffer.count)
					{
						buffer.Current = buffer.buffer;
						buffer.buffer = new List<TSource>(buffer.count);
						buffer.continueNext = false;
						buffer.completionSource.TrySetResult(true);
						return;
					}
					if (!buffer.continueNext)
					{
						buffer.SourceMoveNext();
						return;
					}
				}
				else
				{
					buffer.continueNext = false;
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(Buffer<TSource>._Buffer.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly int count;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private bool continueNext;

			private bool completed;

			private List<TSource> buffer;
		}
	}
}
