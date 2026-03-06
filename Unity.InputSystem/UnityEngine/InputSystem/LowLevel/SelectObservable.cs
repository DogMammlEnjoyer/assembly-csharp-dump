using System;

namespace UnityEngine.InputSystem.LowLevel
{
	internal class SelectObservable<TSource, TResult> : IObservable<TResult>
	{
		public SelectObservable(IObservable<TSource> source, Func<TSource, TResult> filter)
		{
			this.m_Source = source;
			this.m_Filter = filter;
		}

		public IDisposable Subscribe(IObserver<TResult> observer)
		{
			return this.m_Source.Subscribe(new SelectObservable<TSource, TResult>.Select(this, observer));
		}

		private readonly IObservable<TSource> m_Source;

		private readonly Func<TSource, TResult> m_Filter;

		private class Select : IObserver<TSource>
		{
			public Select(SelectObservable<TSource, TResult> observable, IObserver<TResult> observer)
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
				TResult value = this.m_Observable.m_Filter(evt);
				this.m_Observer.OnNext(value);
			}

			private SelectObservable<TSource, TResult> m_Observable;

			private readonly IObserver<TResult> m_Observer;
		}
	}
}
