using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal struct QuerySettings
	{
		internal CancellationState CancellationState
		{
			get
			{
				return this._cancellationState;
			}
			set
			{
				this._cancellationState = value;
			}
		}

		internal TaskScheduler TaskScheduler
		{
			get
			{
				return this._taskScheduler;
			}
			set
			{
				this._taskScheduler = value;
			}
		}

		internal int? DegreeOfParallelism
		{
			get
			{
				return this._degreeOfParallelism;
			}
			set
			{
				this._degreeOfParallelism = value;
			}
		}

		internal ParallelExecutionMode? ExecutionMode
		{
			get
			{
				return this._executionMode;
			}
			set
			{
				this._executionMode = value;
			}
		}

		internal ParallelMergeOptions? MergeOptions
		{
			get
			{
				return this._mergeOptions;
			}
			set
			{
				this._mergeOptions = value;
			}
		}

		internal int QueryId
		{
			get
			{
				return this._queryId;
			}
		}

		internal QuerySettings(TaskScheduler taskScheduler, int? degreeOfParallelism, CancellationToken externalCancellationToken, ParallelExecutionMode? executionMode, ParallelMergeOptions? mergeOptions)
		{
			this._taskScheduler = taskScheduler;
			this._degreeOfParallelism = degreeOfParallelism;
			this._cancellationState = new CancellationState(externalCancellationToken);
			this._executionMode = executionMode;
			this._mergeOptions = mergeOptions;
			this._queryId = -1;
		}

		internal QuerySettings Merge(QuerySettings settings2)
		{
			if (this.TaskScheduler != null && settings2.TaskScheduler != null)
			{
				throw new InvalidOperationException("The WithTaskScheduler operator may be used at most once in a query.");
			}
			if (this.DegreeOfParallelism != null && settings2.DegreeOfParallelism != null)
			{
				throw new InvalidOperationException("The WithDegreeOfParallelism operator may be used at most once in a query.");
			}
			if (this.CancellationState.ExternalCancellationToken.CanBeCanceled && settings2.CancellationState.ExternalCancellationToken.CanBeCanceled)
			{
				throw new InvalidOperationException("The WithCancellation operator may by used at most once in a query.");
			}
			if (this.ExecutionMode != null && settings2.ExecutionMode != null)
			{
				throw new InvalidOperationException("The WithExecutionMode operator may be used at most once in a query.");
			}
			if (this.MergeOptions != null && settings2.MergeOptions != null)
			{
				throw new InvalidOperationException("The WithMergeOptions operator may be used at most once in a query.");
			}
			TaskScheduler taskScheduler = (this.TaskScheduler == null) ? settings2.TaskScheduler : this.TaskScheduler;
			int? degreeOfParallelism = (this.DegreeOfParallelism != null) ? this.DegreeOfParallelism : settings2.DegreeOfParallelism;
			CancellationToken externalCancellationToken = this.CancellationState.ExternalCancellationToken.CanBeCanceled ? this.CancellationState.ExternalCancellationToken : settings2.CancellationState.ExternalCancellationToken;
			ParallelExecutionMode? executionMode = (this.ExecutionMode != null) ? this.ExecutionMode : settings2.ExecutionMode;
			ParallelMergeOptions? mergeOptions = (this.MergeOptions != null) ? this.MergeOptions : settings2.MergeOptions;
			return new QuerySettings(taskScheduler, degreeOfParallelism, externalCancellationToken, executionMode, mergeOptions);
		}

		internal QuerySettings WithPerExecutionSettings()
		{
			return this.WithPerExecutionSettings(new CancellationTokenSource(), new Shared<bool>(false));
		}

		internal QuerySettings WithPerExecutionSettings(CancellationTokenSource topLevelCancellationTokenSource, Shared<bool> topLevelDisposedFlag)
		{
			QuerySettings result = new QuerySettings(this.TaskScheduler, this.DegreeOfParallelism, this.CancellationState.ExternalCancellationToken, this.ExecutionMode, this.MergeOptions);
			result.CancellationState.InternalCancellationTokenSource = topLevelCancellationTokenSource;
			result.CancellationState.MergedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(result.CancellationState.InternalCancellationTokenSource.Token, result.CancellationState.ExternalCancellationToken);
			result.CancellationState.TopLevelDisposedFlag = topLevelDisposedFlag;
			result._queryId = PlinqEtwProvider.NextQueryId();
			return result;
		}

		internal QuerySettings WithDefaults()
		{
			QuerySettings result = this;
			if (result.TaskScheduler == null)
			{
				result.TaskScheduler = TaskScheduler.Default;
			}
			if (result.DegreeOfParallelism == null)
			{
				result.DegreeOfParallelism = new int?(Scheduling.GetDefaultDegreeOfParallelism());
			}
			if (result.ExecutionMode == null)
			{
				result.ExecutionMode = new ParallelExecutionMode?(ParallelExecutionMode.Default);
			}
			if (result.MergeOptions == null)
			{
				result.MergeOptions = new ParallelMergeOptions?(ParallelMergeOptions.Default);
			}
			ParallelMergeOptions? mergeOptions = result.MergeOptions;
			ParallelMergeOptions parallelMergeOptions = ParallelMergeOptions.Default;
			if (mergeOptions.GetValueOrDefault() == parallelMergeOptions & mergeOptions != null)
			{
				result.MergeOptions = new ParallelMergeOptions?(ParallelMergeOptions.AutoBuffered);
			}
			return result;
		}

		internal static QuerySettings Empty
		{
			get
			{
				return new QuerySettings(null, null, default(CancellationToken), null, null);
			}
		}

		public void CleanStateAtQueryEnd()
		{
			this._cancellationState.MergedCancellationTokenSource.Dispose();
		}

		private TaskScheduler _taskScheduler;

		private int? _degreeOfParallelism;

		private CancellationState _cancellationState;

		private ParallelExecutionMode? _executionMode;

		private ParallelMergeOptions? _mergeOptions;

		private int _queryId;
	}
}
