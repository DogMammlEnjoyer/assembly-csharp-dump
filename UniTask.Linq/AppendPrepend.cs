using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class AppendPrepend<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public AppendPrepend(IUniTaskAsyncEnumerable<TSource> source, TSource element, bool append)
		{
			this.source = source;
			this.element = element;
			this.append = append;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new AppendPrepend<TSource>._AppendPrepend(this.source, this.element, this.append, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly TSource element;

		private readonly bool append;

		private sealed class _AppendPrepend : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _AppendPrepend(IUniTaskAsyncEnumerable<TSource> source, TSource element, bool append, CancellationToken cancellationToken)
			{
				this.source = source;
				this.element = element;
				this.state = (append ? AppendPrepend<TSource>._AppendPrepend.State.RequireAppend : AppendPrepend<TSource>._AppendPrepend.State.RequirePrepend);
				this.cancellationToken = cancellationToken;
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				this.completionSource.Reset();
				if (this.enumerator == null)
				{
					if (this.state == AppendPrepend<TSource>._AppendPrepend.State.RequirePrepend)
					{
						this.Current = this.element;
						this.state = AppendPrepend<TSource>._AppendPrepend.State.None;
						return CompletedTasks.True;
					}
					this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
				}
				if (this.state == AppendPrepend<TSource>._AppendPrepend.State.Completed)
				{
					return CompletedTasks.False;
				}
				this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
				if (this.awaiter.IsCompleted)
				{
					AppendPrepend<TSource>._AppendPrepend.MoveNextCoreDelegate(this);
				}
				else
				{
					this.awaiter.SourceOnCompleted(AppendPrepend<TSource>._AppendPrepend.MoveNextCoreDelegate, this);
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private static void MoveNextCore(object state)
			{
				AppendPrepend<TSource>._AppendPrepend appendPrepend = (AppendPrepend<TSource>._AppendPrepend)state;
				bool flag;
				if (appendPrepend.TryGetResult<bool>(appendPrepend.awaiter, out flag))
				{
					if (flag)
					{
						appendPrepend.Current = appendPrepend.enumerator.Current;
						appendPrepend.completionSource.TrySetResult(true);
						return;
					}
					if (appendPrepend.state == AppendPrepend<TSource>._AppendPrepend.State.RequireAppend)
					{
						appendPrepend.state = AppendPrepend<TSource>._AppendPrepend.State.Completed;
						appendPrepend.Current = appendPrepend.element;
						appendPrepend.completionSource.TrySetResult(true);
						return;
					}
					appendPrepend.state = AppendPrepend<TSource>._AppendPrepend.State.Completed;
					appendPrepend.completionSource.TrySetResult(false);
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

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(AppendPrepend<TSource>._AppendPrepend.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly TSource element;

			private CancellationToken cancellationToken;

			private AppendPrepend<TSource>._AppendPrepend.State state;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private enum State : byte
			{
				None,
				RequirePrepend,
				RequireAppend,
				Completed
			}
		}
	}
}
