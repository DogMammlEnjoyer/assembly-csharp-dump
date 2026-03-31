using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SelectMany<TSource, TCollection, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public SelectMany(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, IUniTaskAsyncEnumerable<TCollection>> selector, Func<TSource, TCollection, TResult> resultSelector)
		{
			this.source = source;
			this.selector1 = selector;
			this.selector2 = null;
			this.resultSelector = resultSelector;
		}

		public SelectMany(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, IUniTaskAsyncEnumerable<TCollection>> selector, Func<TSource, TCollection, TResult> resultSelector)
		{
			this.source = source;
			this.selector1 = null;
			this.selector2 = selector;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SelectMany<TSource, TCollection, TResult>._SelectMany(this.source, this.selector1, this.selector2, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, IUniTaskAsyncEnumerable<TCollection>> selector1;

		private readonly Func<TSource, int, IUniTaskAsyncEnumerable<TCollection>> selector2;

		private readonly Func<TSource, TCollection, TResult> resultSelector;

		private sealed class _SelectMany : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _SelectMany(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, IUniTaskAsyncEnumerable<TCollection>> selector1, Func<TSource, int, IUniTaskAsyncEnumerable<TCollection>> selector2, Func<TSource, TCollection, TResult> resultSelector, CancellationToken cancellationToken)
			{
				this.source = source;
				this.selector1 = selector1;
				this.selector2 = selector2;
				this.resultSelector = resultSelector;
				this.cancellationToken = cancellationToken;
			}

			public TResult Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.completionSource.Reset();
				if (this.selectedEnumerator != null)
				{
					this.MoveNextSelected();
				}
				else
				{
					if (this.sourceEnumerator == null)
					{
						this.sourceEnumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
					}
					this.MoveNextSource();
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void MoveNextSource()
			{
				try
				{
					this.sourceAwaiter = this.sourceEnumerator.MoveNextAsync().GetAwaiter();
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
					return;
				}
				if (this.sourceAwaiter.IsCompleted)
				{
					SelectMany<TSource, TCollection, TResult>._SelectMany.SourceMoveNextCore(this);
					return;
				}
				this.sourceAwaiter.SourceOnCompleted(SelectMany<TSource, TCollection, TResult>._SelectMany.sourceMoveNextCoreDelegate, this);
			}

			private void MoveNextSelected()
			{
				try
				{
					this.selectedAwaiter = this.selectedEnumerator.MoveNextAsync().GetAwaiter();
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
					return;
				}
				if (this.selectedAwaiter.IsCompleted)
				{
					SelectMany<TSource, TCollection, TResult>._SelectMany.SeletedSourceMoveNextCore(this);
					return;
				}
				this.selectedAwaiter.SourceOnCompleted(SelectMany<TSource, TCollection, TResult>._SelectMany.selectedSourceMoveNextCoreDelegate, this);
			}

			private static void SourceMoveNextCore(object state)
			{
				SelectMany<TSource, TCollection, TResult>._SelectMany selectMany = (SelectMany<TSource, TCollection, TResult>._SelectMany)state;
				bool flag;
				if (selectMany.TryGetResult<bool>(selectMany.sourceAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							selectMany.sourceCurrent = selectMany.sourceEnumerator.Current;
							if (selectMany.selector1 != null)
							{
								selectMany.selectedEnumerator = selectMany.selector1(selectMany.sourceCurrent).GetAsyncEnumerator(selectMany.cancellationToken);
							}
							else
							{
								SelectMany<TSource, TCollection, TResult>._SelectMany selectMany2 = selectMany;
								Func<TSource, int, IUniTaskAsyncEnumerable<TCollection>> func = selectMany.selector2;
								TSource arg = selectMany.sourceCurrent;
								SelectMany<TSource, TCollection, TResult>._SelectMany selectMany3 = selectMany;
								int num = selectMany3.sourceIndex;
								selectMany3.sourceIndex = checked(num + 1);
								selectMany2.selectedEnumerator = func(arg, num).GetAsyncEnumerator(selectMany.cancellationToken);
							}
						}
						catch (Exception error)
						{
							selectMany.completionSource.TrySetException(error);
							return;
						}
						selectMany.MoveNextSelected();
						return;
					}
					selectMany.completionSource.TrySetResult(false);
				}
			}

			private static void SeletedSourceMoveNextCore(object state)
			{
				SelectMany<TSource, TCollection, TResult>._SelectMany selectMany = (SelectMany<TSource, TCollection, TResult>._SelectMany)state;
				bool flag;
				if (selectMany.TryGetResult<bool>(selectMany.selectedAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							selectMany.Current = selectMany.resultSelector(selectMany.sourceCurrent, selectMany.selectedEnumerator.Current);
						}
						catch (Exception error)
						{
							selectMany.completionSource.TrySetException(error);
							return;
						}
						selectMany.completionSource.TrySetResult(true);
						return;
					}
					try
					{
						selectMany.selectedDisposeAsyncAwaiter = selectMany.selectedEnumerator.DisposeAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						selectMany.completionSource.TrySetException(error2);
						return;
					}
					if (selectMany.selectedDisposeAsyncAwaiter.IsCompleted)
					{
						SelectMany<TSource, TCollection, TResult>._SelectMany.SelectedEnumeratorDisposeAsyncCore(selectMany);
						return;
					}
					selectMany.selectedDisposeAsyncAwaiter.SourceOnCompleted(SelectMany<TSource, TCollection, TResult>._SelectMany.selectedEnumeratorDisposeAsyncCoreDelegate, selectMany);
				}
			}

			private static void SelectedEnumeratorDisposeAsyncCore(object state)
			{
				SelectMany<TSource, TCollection, TResult>._SelectMany selectMany = (SelectMany<TSource, TCollection, TResult>._SelectMany)state;
				if (selectMany.TryGetResult(selectMany.selectedDisposeAsyncAwaiter))
				{
					selectMany.selectedEnumerator = null;
					selectMany.selectedAwaiter = default(UniTask<bool>.Awaiter);
					selectMany.MoveNextSource();
				}
			}

			public UniTask DisposeAsync()
			{
				SelectMany<TSource, TCollection, TResult>._SelectMany.<DisposeAsync>d__26 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<SelectMany<TSource, TCollection, TResult>._SelectMany.<DisposeAsync>d__26>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> sourceMoveNextCoreDelegate = new Action<object>(SelectMany<TSource, TCollection, TResult>._SelectMany.SourceMoveNextCore);

			private static readonly Action<object> selectedSourceMoveNextCoreDelegate = new Action<object>(SelectMany<TSource, TCollection, TResult>._SelectMany.SeletedSourceMoveNextCore);

			private static readonly Action<object> selectedEnumeratorDisposeAsyncCoreDelegate = new Action<object>(SelectMany<TSource, TCollection, TResult>._SelectMany.SelectedEnumeratorDisposeAsyncCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, IUniTaskAsyncEnumerable<TCollection>> selector1;

			private readonly Func<TSource, int, IUniTaskAsyncEnumerable<TCollection>> selector2;

			private readonly Func<TSource, TCollection, TResult> resultSelector;

			private CancellationToken cancellationToken;

			private TSource sourceCurrent;

			private int sourceIndex;

			private IUniTaskAsyncEnumerator<TSource> sourceEnumerator;

			private IUniTaskAsyncEnumerator<TCollection> selectedEnumerator;

			private UniTask<bool>.Awaiter sourceAwaiter;

			private UniTask<bool>.Awaiter selectedAwaiter;

			private UniTask.Awaiter selectedDisposeAsyncAwaiter;
		}
	}
}
