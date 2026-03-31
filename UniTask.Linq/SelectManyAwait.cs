using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SelectManyAwait<TSource, TCollection, TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public SelectManyAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector, Func<TSource, TCollection, UniTask<TResult>> resultSelector)
		{
			this.source = source;
			this.selector1 = selector;
			this.selector2 = null;
			this.resultSelector = resultSelector;
		}

		public SelectManyAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector, Func<TSource, TCollection, UniTask<TResult>> resultSelector)
		{
			this.source = source;
			this.selector1 = null;
			this.selector2 = selector;
			this.resultSelector = resultSelector;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait(this.source, this.selector1, this.selector2, this.resultSelector, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector1;

		private readonly Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector2;

		private readonly Func<TSource, TCollection, UniTask<TResult>> resultSelector;

		private sealed class _SelectManyAwait : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
		{
			public _SelectManyAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector1, Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector2, Func<TSource, TCollection, UniTask<TResult>> resultSelector, CancellationToken cancellationToken)
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
					SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.SourceMoveNextCore(this);
					return;
				}
				this.sourceAwaiter.SourceOnCompleted(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.sourceMoveNextCoreDelegate, this);
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
					SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.SeletedSourceMoveNextCore(this);
					return;
				}
				this.selectedAwaiter.SourceOnCompleted(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.selectedSourceMoveNextCoreDelegate, this);
			}

			private static void SourceMoveNextCore(object state)
			{
				SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait selectManyAwait = (SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait)state;
				bool flag;
				if (selectManyAwait.TryGetResult<bool>(selectManyAwait.sourceAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							selectManyAwait.sourceCurrent = selectManyAwait.sourceEnumerator.Current;
							if (selectManyAwait.selector1 != null)
							{
								selectManyAwait.collectionSelectorAwaiter = selectManyAwait.selector1(selectManyAwait.sourceCurrent).GetAwaiter();
							}
							else
							{
								SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait selectManyAwait2 = selectManyAwait;
								Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TCollection>>> func = selectManyAwait.selector2;
								TSource arg = selectManyAwait.sourceCurrent;
								SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait selectManyAwait3 = selectManyAwait;
								int num = selectManyAwait3.sourceIndex;
								selectManyAwait3.sourceIndex = checked(num + 1);
								selectManyAwait2.collectionSelectorAwaiter = func(arg, num).GetAwaiter();
							}
							if (selectManyAwait.collectionSelectorAwaiter.IsCompleted)
							{
								SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.SelectorAwaitCore(selectManyAwait);
							}
							else
							{
								selectManyAwait.collectionSelectorAwaiter.SourceOnCompleted(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.selectorAwaitCoreDelegate, selectManyAwait);
							}
							return;
						}
						catch (Exception error)
						{
							selectManyAwait.completionSource.TrySetException(error);
							return;
						}
					}
					selectManyAwait.completionSource.TrySetResult(false);
				}
			}

			private static void SeletedSourceMoveNextCore(object state)
			{
				SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait selectManyAwait = (SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait)state;
				bool flag;
				if (selectManyAwait.TryGetResult<bool>(selectManyAwait.selectedAwaiter, out flag))
				{
					if (flag)
					{
						try
						{
							selectManyAwait.resultSelectorAwaiter = selectManyAwait.resultSelector(selectManyAwait.sourceCurrent, selectManyAwait.selectedEnumerator.Current).GetAwaiter();
							if (selectManyAwait.resultSelectorAwaiter.IsCompleted)
							{
								SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.ResultSelectorAwaitCore(selectManyAwait);
							}
							else
							{
								selectManyAwait.resultSelectorAwaiter.SourceOnCompleted(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.resultSelectorAwaitCoreDelegate, selectManyAwait);
							}
							return;
						}
						catch (Exception error)
						{
							selectManyAwait.completionSource.TrySetException(error);
							return;
						}
					}
					try
					{
						selectManyAwait.selectedDisposeAsyncAwaiter = selectManyAwait.selectedEnumerator.DisposeAsync().GetAwaiter();
					}
					catch (Exception error2)
					{
						selectManyAwait.completionSource.TrySetException(error2);
						return;
					}
					if (selectManyAwait.selectedDisposeAsyncAwaiter.IsCompleted)
					{
						SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.SelectedEnumeratorDisposeAsyncCore(selectManyAwait);
						return;
					}
					selectManyAwait.selectedDisposeAsyncAwaiter.SourceOnCompleted(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.selectedEnumeratorDisposeAsyncCoreDelegate, selectManyAwait);
				}
			}

			private static void SelectedEnumeratorDisposeAsyncCore(object state)
			{
				SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait selectManyAwait = (SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait)state;
				if (selectManyAwait.TryGetResult(selectManyAwait.selectedDisposeAsyncAwaiter))
				{
					selectManyAwait.selectedEnumerator = null;
					selectManyAwait.selectedAwaiter = default(UniTask<bool>.Awaiter);
					selectManyAwait.MoveNextSource();
				}
			}

			private static void SelectorAwaitCore(object state)
			{
				SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait selectManyAwait = (SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait)state;
				IUniTaskAsyncEnumerable<TCollection> uniTaskAsyncEnumerable;
				if (selectManyAwait.TryGetResult<IUniTaskAsyncEnumerable<TCollection>>(selectManyAwait.collectionSelectorAwaiter, out uniTaskAsyncEnumerable))
				{
					selectManyAwait.selectedEnumerator = uniTaskAsyncEnumerable.GetAsyncEnumerator(selectManyAwait.cancellationToken);
					selectManyAwait.MoveNextSelected();
				}
			}

			private static void ResultSelectorAwaitCore(object state)
			{
				SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait selectManyAwait = (SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait)state;
				TResult value;
				if (selectManyAwait.TryGetResult<TResult>(selectManyAwait.resultSelectorAwaiter, out value))
				{
					selectManyAwait.Current = value;
					selectManyAwait.completionSource.TrySetResult(true);
				}
			}

			public UniTask DisposeAsync()
			{
				SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.<DisposeAsync>d__32 <DisposeAsync>d__;
				<DisposeAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
				<DisposeAsync>d__.<>4__this = this;
				<DisposeAsync>d__.<>1__state = -1;
				<DisposeAsync>d__.<>t__builder.Start<SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.<DisposeAsync>d__32>(ref <DisposeAsync>d__);
				return <DisposeAsync>d__.<>t__builder.Task;
			}

			private static readonly Action<object> sourceMoveNextCoreDelegate = new Action<object>(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.SourceMoveNextCore);

			private static readonly Action<object> selectedSourceMoveNextCoreDelegate = new Action<object>(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.SeletedSourceMoveNextCore);

			private static readonly Action<object> selectedEnumeratorDisposeAsyncCoreDelegate = new Action<object>(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.SelectedEnumeratorDisposeAsyncCore);

			private static readonly Action<object> selectorAwaitCoreDelegate = new Action<object>(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.SelectorAwaitCore);

			private static readonly Action<object> resultSelectorAwaitCoreDelegate = new Action<object>(SelectManyAwait<TSource, TCollection, TResult>._SelectManyAwait.ResultSelectorAwaitCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Func<TSource, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector1;

			private readonly Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TCollection>>> selector2;

			private readonly Func<TSource, TCollection, UniTask<TResult>> resultSelector;

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
