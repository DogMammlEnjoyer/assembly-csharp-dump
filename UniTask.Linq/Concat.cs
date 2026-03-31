using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Concat<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public Concat(IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second)
		{
			this.first = first;
			this.second = second;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Concat<TSource>._Concat(this.first, this.second, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> first;

		private readonly IUniTaskAsyncEnumerable<TSource> second;

		private sealed class _Concat : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _Concat(IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, CancellationToken cancellationToken)
			{
				this.first = first;
				this.second = second;
				this.cancellationToken = cancellationToken;
				this.iteratingState = Concat<TSource>._Concat.IteratingState.IteratingFirst;
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.iteratingState == Concat<TSource>._Concat.IteratingState.Complete)
				{
					return CompletedTasks.False;
				}
				this.completionSource.Reset();
				this.StartIterate();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void StartIterate()
			{
				if (this.enumerator == null)
				{
					if (this.iteratingState == Concat<TSource>._Concat.IteratingState.IteratingFirst)
					{
						this.enumerator = this.first.GetAsyncEnumerator(this.cancellationToken);
					}
					else if (this.iteratingState == Concat<TSource>._Concat.IteratingState.IteratingSecond)
					{
						this.enumerator = this.second.GetAsyncEnumerator(this.cancellationToken);
					}
				}
				try
				{
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
					return;
				}
				if (this.awaiter.IsCompleted)
				{
					Concat<TSource>._Concat.MoveNextCoreDelegate(this);
					return;
				}
				this.awaiter.SourceOnCompleted(Concat<TSource>._Concat.MoveNextCoreDelegate, this);
			}

			private static void MoveNextCore(object state)
			{
				Concat<TSource>._Concat concat = (Concat<TSource>._Concat)state;
				bool flag;
				if (concat.TryGetResult<bool>(concat.awaiter, out flag))
				{
					if (flag)
					{
						concat.Current = concat.enumerator.Current;
						concat.completionSource.TrySetResult(true);
						return;
					}
					if (concat.iteratingState == Concat<TSource>._Concat.IteratingState.IteratingFirst)
					{
						concat.RunSecondAfterDisposeAsync().Forget();
						return;
					}
					concat.iteratingState = Concat<TSource>._Concat.IteratingState.Complete;
					concat.completionSource.TrySetResult(false);
				}
			}

			private UniTaskVoid RunSecondAfterDisposeAsync()
			{
				Concat<TSource>._Concat.<RunSecondAfterDisposeAsync>d__16 <RunSecondAfterDisposeAsync>d__;
				<RunSecondAfterDisposeAsync>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
				<RunSecondAfterDisposeAsync>d__.<>4__this = this;
				<RunSecondAfterDisposeAsync>d__.<>1__state = -1;
				<RunSecondAfterDisposeAsync>d__.<>t__builder.Start<Concat<TSource>._Concat.<RunSecondAfterDisposeAsync>d__16>(ref <RunSecondAfterDisposeAsync>d__);
				return <RunSecondAfterDisposeAsync>d__.<>t__builder.Task;
			}

			public UniTask DisposeAsync()
			{
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(Concat<TSource>._Concat.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> first;

			private readonly IUniTaskAsyncEnumerable<TSource> second;

			private CancellationToken cancellationToken;

			private Concat<TSource>._Concat.IteratingState iteratingState;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;

			private enum IteratingState
			{
				IteratingFirst,
				IteratingSecond,
				Complete
			}
		}
	}
}
