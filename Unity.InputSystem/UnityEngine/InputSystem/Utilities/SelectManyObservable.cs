using System;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.Utilities
{
	internal class SelectManyObservable<TSource, TResult> : IObservable<TResult>
	{
		public SelectManyObservable(IObservable<TSource> source, Func<TSource, IEnumerable<TResult>> filter)
		{
			this.m_Source = source;
			this.m_Filter = filter;
		}

		public IDisposable Subscribe(IObserver<TResult> observer)
		{
			return this.m_Source.Subscribe(new SelectManyObservable<TSource, TResult>.Select(this, observer));
		}

		private readonly IObservable<TSource> m_Source;

		private readonly Func<TSource, IEnumerable<TResult>> m_Filter;

		private class Select : IObserver<TSource>
		{
			public Select(SelectManyObservable<TSource, TResult> observable, IObserver<TResult> observer)
			{
				this.m_Observable = observable;
				this.m_Observer = observer;
			}

			public void OnCompleted()
			{
			}

			public void OnError(Exception error)
			{
				Debug.LogException(error);
			}

			public void OnNext(TSource evt)
			{
				foreach (TResult value in this.m_Observable.m_Filter(evt))
				{
					this.m_Observer.OnNext(value);
				}
			}

			private SelectManyObservable<TSource, TResult> m_Observable;

			private readonly IObserver<TResult> m_Observer;
		}
	}
}
