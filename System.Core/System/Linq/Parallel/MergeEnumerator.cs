using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal abstract class MergeEnumerator<TInputOutput> : IEnumerator<!0>, IDisposable, IEnumerator
	{
		protected MergeEnumerator(QueryTaskGroupState taskGroupState)
		{
			this._taskGroupState = taskGroupState;
		}

		public abstract TInputOutput Current { get; }

		public abstract bool MoveNext();

		object IEnumerator.Current
		{
			get
			{
				return ((IEnumerator<!0>)this).Current;
			}
		}

		public virtual void Reset()
		{
		}

		public virtual void Dispose()
		{
			if (!this._taskGroupState.IsAlreadyEnded)
			{
				this._taskGroupState.QueryEnd(true);
			}
		}

		protected QueryTaskGroupState _taskGroupState;
	}
}
