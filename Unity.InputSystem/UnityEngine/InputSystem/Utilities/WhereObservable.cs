using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal class WhereObservable<TValue> : IObservable<TValue>
	{
		public WhereObservable(IObservable<TValue> source, Func<TValue, bool> predicate)
		{
			this.m_Source = source;
			this.m_Predicate = predicate;
		}

		public IDisposable Subscribe(IObserver<TValue> observer)
		{
			return this.m_Source.Subscribe(new WhereObservable<TValue>.Where(this, observer));
		}

		private readonly IObservable<TValue> m_Source;

		private readonly Func<TValue, bool> m_Predicate;

		private class Where : IObserver<TValue>
		{
			public Where(WhereObservable<TValue> observable, IObserver<TValue> observer)
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

			public void OnNext(TValue evt)
			{
				if (this.m_Observable.m_Predicate(evt))
				{
					this.m_Observer.OnNext(evt);
				}
			}

			private WhereObservable<TValue> m_Observable;

			private readonly IObserver<TValue> m_Observer;
		}
	}
}
