using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SelectManyAwaitWithCancellation<TSource, TCollection, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public SelectManyAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector, Func<TSource, TCollection, CancellationToken, UniTask<TResult>> resultSelector)
		{
			this.source = source;
			this.selector1 = selector;
			this.selector2 = null;
			this.resultSelector = resultSelector;
		}

		public SelectManyAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector, Func<TSource, TCollection, CancellationToken, UniTask<TResult>> resultSelector)
		{
			this.source = source;
			this.selector1 = null;
			this.selector2 = selector;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation(this.source, this.selector1, this.selector2, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector1;

		private readonly Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector2;

		private readonly Func<TSource, TCollection, CancellationToken, UniTask<TResult>> resultSelector;

		private sealed class _SelectManyAwaitWithCancellation : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _SelectManyAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector1, Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector2, Func<TSource, TCollection, CancellationToken, UniTask<TResult>> resultSelector, CancellationToken cancellationToken)
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
					SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.SourceMoveNextCore(this);
					return;
				}
				this.sourceAwaiter.SourceOnCompleted(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.sourceMoveNextCoreDelegate, this);
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
					SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.SeletedSourceMoveNextCore(this);
					return;
				}
				this.selectedAwaiter.SourceOnCompleted(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.selectedSourceMoveNextCoreDelegate, this);
			}

			private static void SourceMoveNextCore(object state)
			{
				SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation selectManyAwaitWithCancellation = (SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation)state;
				bool flag;
				if (selectManyAwaitWithCancellation.TryGetResult<bool>(selectManyAwaitWithCancellation.sourceAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							selectManyAwaitWithCancellation.sourceCurrent = selectManyAwaitWithCancellation.sourceEnumerator.Current;
							if (selectManyAwaitWithCancellation.selector1 != null)
							{
								selectManyAwaitWithCancellation.collectionSelectorAwaiter = selectManyAwaitWithCancellation.selector1(selectManyAwaitWithCancellation.sourceCurrent, selectManyAwaitWithCancellation.cancellationToken).GetAwaiter();
							}
							else
							{
								SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation selectManyAwaitWithCancellation2 = selectManyAwaitWithCancellation;
								Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> func = selectManyAwaitWithCancellation.selector2;
								TSource arg = selectManyAwaitWithCancellation.sourceCurrent;
								SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation selectManyAwaitWithCancellation3 = selectManyAwaitWithCancellation;
								int num = selectManyAwaitWithCancellation3.sourceIndex;
								selectManyAwaitWithCancellation3.sourceIndex = checked(num + 1);
								selectManyAwaitWithCancellation2.collectionSelectorAwaiter = func(arg, num, selectManyAwaitWithCancellation.cancellationToken).GetAwaiter();
							}
							if (selectManyAwaitWithCancellation.collectionSelectorAwaiter.IsCompleted)
							{
								SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.SelectorAwaitCore(selectManyAwaitWithCancellation);
							}
							else
							{
								selectManyAwaitWithCancellation.collectionSelectorAwaiter.SourceOnCompleted(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.selectorAwaitCoreDelegate, selectManyAwaitWithCancellation);
							}
							return;
						}
						catch (Exception error)
						{
							selectManyAwaitWithCancellation.completionSource.TrySetException(error);
							return;
						}
					}
					selectManyAwaitWithCancellation.completionSource.TrySetResult(false);
				}
			}

			private static void SeletedSourceMoveNextCore(object state)
			{
				SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation selectManyAwaitWithCancellation = (SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation)state;
				bool flag;
				if (selectManyAwaitWithCancellation.TryGetResult<bool>(selectManyAwaitWithCancellation.selectedAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							selectManyAwaitWithCancellation.resultSelectorAwaiter = selectManyAwaitWithCancellation.resultSelector(selectManyAwaitWithCancellation.sourceCurrent, selectManyAwaitWithCancellation.selectedEnumerator.Current, selectManyAwaitWithCancellation.cancellationToken).GetAwaiter();
							if (selectManyAwaitWithCancellation.resultSelectorAwaiter.IsCompleted)
							{
								SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.ResultSelectorAwaitCore(selectManyAwaitWithCancellation);
							}
							else
							{
								selectManyAwaitWithCancellation.resultSelectorAwaiter.SourceOnCompleted(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.resultSelectorAwaitCoreDelegate, selectManyAwaitWithCancellation);
							}
							return;
						}
						catch (Exception error)
						{
							selectManyAwaitWithCancellation.completionSource.TrySetException(error);
							return;
						}
					}
					try
					{
						selectManyAwaitWithCancellation.selectedDisposeAsyncAwaiter = selectManyAwaitWithCancellation.selectedEnumerator.DisposeAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						selectManyAwaitWithCancellation.completionSource.TrySetException(error2);
						return;
					}
					if (selectManyAwaitWithCancellation.selectedDisposeAsyncAwaiter.IsCompleted)
					{
						SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.SelectedEnumeratorDisposeAsyncCore(selectManyAwaitWithCancellation);
						return;
					}
					selectManyAwaitWithCancellation.selectedDisposeAsyncAwaiter.SourceOnCompleted(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.selectedEnumeratorDisposeAsyncCoreDelegate, selectManyAwaitWithCancellation);
				}
			}

			private static void SelectedEnumeratorDisposeAsyncCore(object state)
			{
				SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation selectManyAwaitWithCancellation = (SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation)state;
				if (selectManyAwaitWithCancellation.TryGetResult(selectManyAwaitWithCancellation.selectedDisposeAsyncAwaiter))
				{
					selectManyAwaitWithCancellation.selectedEnumerator = null;
					selectManyAwaitWithCancellation.selectedAwaiter = default(UniTask<bool>.Awaiter);
					selectManyAwaitWithCancellation.MoveNextSource();
				}
			}

			private static void SelectorAwaitCore(object state)
			{
				SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation selectManyAwaitWithCancellation = (SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation)state;
				IUniTaskAsyncEnumerable<TCollection> uniTaskAsyncEnumerable;
				if (selectManyAwaitWithCancellation.TryGetResult<IUniTaskAsyncEnumerable<TCollection>>(selectManyAwaitWithCancellation.collectionSelectorAwaiter, out uniTaskAsyncEnumerable))
				{
					selectManyAwaitWithCancellation.selectedEnumerator = uniTaskAsyncEnumerable.GetAsyncEnumerator(selectManyAwaitWithCancellation.cancellationToken);
					selectManyAwaitWithCancellation.MoveNextSelected();
				}
			}

			private static void ResultSelectorAwaitCore(object state)
			{
				SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation selectManyAwaitWithCancellation = (SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation)state;
				TResult value;
				if (selectManyAwaitWithCancellation.TryGetResult<TResult>(selectManyAwaitWithCancellation.resultSelectorAwaiter, out value))
				{
					selectManyAwaitWithCancellation.Current = value;
					selectManyAwaitWithCancellation.completionSource.TrySetResult(true);
				}
			}

			public UniTask DisposeAsync()
			{
				SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.<DisposeAsync>d__32 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.<DisposeAsync>d__32>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> sourceMoveNextCoreDelegate = new Action<object>(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.SourceMoveNextCore);

			private static readonly Action<object> selectedSourceMoveNextCoreDelegate = new Action<object>(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.SeletedSourceMoveNextCore);

			private static readonly Action<object> selectedEnumeratorDisposeAsyncCoreDelegate = new Action<object>(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.SelectedEnumeratorDisposeAsyncCore);

			private static readonly Action<object> selectorAwaitCoreDelegate = new Action<object>(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.SelectorAwaitCore);

			private static readonly Action<object> resultSelectorAwaitCoreDelegate = new Action<object>(SelectManyAwaitWithCancellation<TSource, TCollection, TResult>._SelectManyAwaitWithCancellation.ResultSelectorAwaitCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector1;

			private readonly Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector2;

			private readonly Func<TSource, TCollection, CancellationToken, UniTask<TResult>> resultSelector;

			private CancellationToken cancellationToken;

			private TSource sourceCurrent;

			private int sourceIndex;

			private IUniTaskAsyncEnumerator<TSource> sourceEnumerator;

			private IUniTaskAsyncEnumerator<TCollection> selectedEnumerator;

			private UniTask<bool>.Awaiter sourceAwaiter;

			private UniTask<bool>.Awaiter selectedAwaiter;

			private UniTask.Awaiter selectedDisposeAsyncAwaiter;

			private UniTask<IUniTaskAsyncEnumerable<TCollection>>.Awaiter collectionSelectorAwaiter;

			private UniTask<TResult>.Awaiter resultSelectorAwaiter;
		}
	}
}
