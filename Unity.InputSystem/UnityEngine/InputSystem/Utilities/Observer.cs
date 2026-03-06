using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal class Observer<TValue> : IObserver<TValue>
	{
		public Observer(Action<TValue> onNext, Action onCompleted = null)
		{
			this.m_OnNext = onNext;
			this.m_OnCompleted = onCompleted;
		}

		public void OnCompleted()
		{
			Action onCompleted = this.m_OnCompleted;
			if (onCompleted == null)
			{
				return;
			}
			onCompleted();
		}

		public void OnError(Exception error)
		{
			Debug.LogException(error);
		}

		public void OnNext(TValue evt)
		{
			Action<TValue> onNext = this.m_OnNext;
			if (onNext == null)
			{
				return;
			}
			onNext(evt);
		}

		private Action<TValue> m_OnNext;

		private Action m_OnCompleted;
	}
}
