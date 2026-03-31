using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class EveryValueChangedUnityObject<TTarget, TProperty> : IUniTaskAsyncEnumerable<TProperty>
	{
		public EveryValueChangedUnityObject(TTarget target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming)
		{
			this.target = target;
			this.propertySelector = propertySelector;
			this.equalityComparer = equalityComparer;
			this.monitorTiming = monitorTiming;
		}

		public IUniTaskAsyncEnumerator<TProperty> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new EveryValueChangedUnityObject<TTarget, TProperty>._EveryValueChanged(this.target, this.propertySelector, this.equalityComparer, this.monitorTiming, cancellationToken);
		}

		private readonly TTarget target;

		private readonly Func<TTarget, TProperty> propertySelector;

		private readonly IEqualityComparer<TProperty> equalityComparer;

		private readonly PlayerLoopTiming monitorTiming;

		private sealed class _EveryValueChanged : MoveNextSource, IUniTaskAsyncEnumerator<TProperty>, IUniTaskAsyncDisposable, IPlayerLoopItem
		{
			public _EveryValueChanged(TTarget target, Func<TTarget, TProperty> propertySelector, IEqualityComparer<TProperty> equalityComparer, PlayerLoopTiming monitorTiming, CancellationToken cancellationToken)
			{
				this.target = target;
				this.targetAsUnityObject = (target as Object);
				this.propertySelector = propertySelector;
				this.equalityComparer = equalityComparer;
				this.cancellationToken = cancellationToken;
				this.first = true;
				PlayerLoopHelper.AddAction(monitorTiming, this);
			}

			public TProperty Current
			{
				get
				{
					return this.currentValue;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				if (this.disposed || this.cancellationToken.IsCancellationRequested)
				{
					return CompletedTasks.False;
				}
				if (!this.first)
				{
					this.completionSource.Reset();
					return new UniTask<bool>(this, this.completionSource.Version);
				}
				this.first = false;
				if (this.targetAsUnityObject == null)
				{
					return CompletedTasks.False;
				}
				this.currentValue = this.propertySelector(this.target);
				return CompletedTasks.True;
			}

			public UniTask DisposeAsync()
			{
				if (!this.disposed)
				{
					this.disposed = true;
				}
				return default(UniTask);
			}

			public bool MoveNext()
			{
				if (this.disposed || this.cancellationToken.IsCancellationRequested || this.targetAsUnityObject == null)
				{
					this.completionSource.TrySetResult(false);
					this.DisposeAsync().Forget();
					return false;
				}
				TProperty y = default(TProperty);
				try
				{
					y = this.propertySelector(this.target);
					if (this.equalityComparer.Equals(this.currentValue, y))
					{
						return true;
					}
				}
				catch (Exception error)
				{
					this.completionSource.TrySetException(error);
					this.DisposeAsync().Forget();
					return false;
				}
				this.currentValue = y;
				this.completionSource.TrySetResult(true);
				return true;
			}

			private readonly TTarget target;

			private readonly Object targetAsUnityObject;

			private readonly IEqualityComparer<TProperty> equalityComparer;

			private readonly Func<TTarget, TProperty> propertySelector;

			private CancellationToken cancellationToken;

			private bool first;

			private TProperty currentValue;

			private bool disposed;
		}
	}
}
