using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal class TakeNObservable<TValue> : IObservable<TValue>
	{
		public TakeNObservable(IObservable<TValue> source, int count)
		{
			this.m_Source = source;
			this.m_Count = count;
		}

		public IDisposable Subscribe(IObserver<TValue> observer)
		{
			return this.m_Source.Subscribe(new TakeNObservable<TValue>.Take(this, observer));
		}

		private IObservable<TValue> m_Source;

		private int m_Count;

		private class Take : IObserver<TValue>
		{
			public Take(TakeNObservable<TValue> observable, IObserver<TValue> observer)
			{
				this.m_Observer = observer;
				this.m_Remaining = observable.m_Count;
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
				if (this.m_Remaining <= 0)
				{
					return;
				}
				this.m_Remaining--;
				this.m_Observer.OnNext(evt);
				if (this.m_Remaining == 0)
				{
					this.m_Observer.OnCompleted();
					this.m_Observer = null;
				}
			}

			private IObserver<TValue> m_Observer;

			private int m_Remaining;
		}
	}
}
