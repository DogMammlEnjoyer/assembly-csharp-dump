using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class ForAllOperator<TInput> : UnaryQueryOperator<TInput, TInput>
	{
		internal ForAllOperator(IEnumerable<TInput> child, Action<TInput> elementAction) : base(child)
		{
			this._elementAction = elementAction;
		}

		internal void RunSynchronously()
		{
			Shared<bool> topLevelDisposedFlag = new Shared<bool>(false);
			CancellationTokenSource topLevelCancellationTokenSource = new CancellationTokenSource();
			QuerySettings querySettings = base.SpecifiedQuerySettings.WithPerExecutionSettings(topLevelCancellationTokenSource, topLevelDisposedFlag).WithDefaults();
			QueryLifecycle.LogicalQueryExecutionBegin(querySettings.QueryId);
			base.GetOpenedEnumerator(new ParallelMergeOptions?(ParallelMergeOptions.FullyBuffered), true, true, querySettings);
			querySettings.CleanStateAtQueryEnd();
			QueryLifecycle.LogicalQueryExecutionEnd(querySettings.QueryId);
		}

		internal override QueryResults<TInput> Open(QuerySettings settings, bool preferStriping)
		{
			return new UnaryQueryOperator<TInput, TInput>.UnaryQueryOperatorResults(base.Child.Open(settings, preferStriping), this, settings, preferStriping);
		}

		internal override void WrapPartitionedStream<TKey>(PartitionedStream<TInput, TKey> inputStream, IPartitionedStreamRecipient<TInput> recipient, bool preferStriping, QuerySettings settings)
		{
			int partitionCount = inputStream.PartitionCount;
			PartitionedStream<TInput, int> partitionedStream = new PartitionedStream<TInput, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Correct);
			for (int i = 0; i < partitionCount; i++)
			{
				partitionedStream[i] = new ForAllOperator<TInput>.ForAllEnumerator<TKey>(inputStream[i], this._elementAction, settings.CancellationState.MergedCancellationToken);
			}
			recipient.Receive<int>(partitionedStream);
		}

		[ExcludeFromCodeCoverage]
		internal override IEnumerable<TInput> AsSequentialQuery(CancellationToken token)
		{
			throw new InvalidOperationException();
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly Action<TInput> _elementAction;

		private class ForAllEnumerator<TKey> : QueryOperatorEnumerator<TInput, int>
		{
			internal ForAllEnumerator(QueryOperatorEnumerator<TInput, TKey> source, Action<TInput> elementAction, CancellationToken cancellationToken)
			{
				this._source = source;
				this._elementAction = elementAction;
				this._cancellationToken = cancellationToken;
			}

			internal override bool MoveNext(ref TInput currentElement, ref int currentKey)
			{
				TInput obj = default(TInput);
				TKey tkey = default(TKey);
				int num = 0;
				while (this._source.MoveNext(ref obj, ref tkey))
				{
					if ((num++ & 63) == 0)
					{
						CancellationState.ThrowIfCanceled(this._cancellationToken);
					}
					this._elementAction(obj);
				}
				return false;
			}

			protected override void Dispose(bool disposing)
			{
				this._source.Dispose();
			}

			private readonly QueryOperatorEnumerator<TInput, TKey> _source;

			private readonly Action<TInput> _elementAction;

			private CancellationToken _cancellationToken;
		}
	}
}
