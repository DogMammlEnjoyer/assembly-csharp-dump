using System;

namespace System.Threading.Tasks
{
	/// <summary>Stores options that configure the operation of methods on the <see cref="T:System.Threading.Tasks.Parallel" /> class.</summary>
	public class ParallelOptions
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.ParallelOptions" /> class.</summary>
		public ParallelOptions()
		{
			this._scheduler = TaskScheduler.Default;
			this._maxDegreeOfParallelism = -1;
			this._cancellationToken = CancellationToken.None;
		}

		/// <summary>Gets or sets the <see cref="T:System.Threading.Tasks.TaskScheduler" /> associated with this <see cref="T:System.Threading.Tasks.ParallelOptions" /> instance. Setting this property to null indicates that the current scheduler should be used.</summary>
		/// <returns>The task scheduler that is associated with this instance.</returns>
		public TaskScheduler TaskScheduler
		{
			get
			{
				return this._scheduler;
			}
			set
			{
				this._scheduler = value;
			}
		}

		internal TaskScheduler EffectiveTaskScheduler
		{
			get
			{
				if (this._scheduler == null)
				{
					return TaskScheduler.Current;
				}
				return this._scheduler;
			}
		}

		/// <summary>Gets or sets the maximum number of concurrent tasks enabled by this <see cref="T:System.Threading.Tasks.ParallelOptions" /> instance.</summary>
		/// <returns>An integer that represents the maximum degree of parallelism.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The property is being set to zero or to a value that is less than -1.</exception>
		public int MaxDegreeOfParallelism
		{
			get
			{
				return this._maxDegreeOfParallelism;
			}
			set
			{
				if (value == 0 || value < -1)
				{
					throw new ArgumentOutOfRangeException("MaxDegreeOfParallelism");
				}
				this._maxDegreeOfParallelism = value;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Threading.CancellationToken" /> associated with this <see cref="T:System.Threading.Tasks.ParallelOptions" /> instance.</summary>
		/// <returns>The token that is associated with this instance.</returns>
		public CancellationToken CancellationToken
		{
			get
			{
				return this._cancellationToken;
			}
			set
			{
				this._cancellationToken = value;
			}
		}

		internal int EffectiveMaxConcurrencyLevel
		{
			get
			{
				int num = this.MaxDegreeOfParallelism;
				int maximumConcurrencyLevel = this.EffectiveTaskScheduler.MaximumConcurrencyLevel;
				if (maximumConcurrencyLevel > 0 && maximumConcurrencyLevel != 2147483647)
				{
					num = ((num == -1) ? maximumConcurrencyLevel : Math.Min(maximumConcurrencyLevel, num));
				}
				return num;
			}
		}

		private TaskScheduler _scheduler;

		private int _maxDegreeOfParallelism;

		private CancellationToken _cancellationToken;
	}
}
