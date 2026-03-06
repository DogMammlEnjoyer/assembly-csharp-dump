using System;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public struct InputEventListener : IObservable<InputEventPtr>
	{
		public static InputEventListener operator +(InputEventListener _, Action<InputEventPtr, InputDevice> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			InputManager s_Manager = InputSystem.s_Manager;
			lock (s_Manager)
			{
				InputSystem.s_Manager.onEvent += callback;
			}
			return default(InputEventListener);
		}

		public static InputEventListener operator -(InputEventListener _, Action<InputEventPtr, InputDevice> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			InputManager s_Manager = InputSystem.s_Manager;
			lock (s_Manager)
			{
				InputSystem.s_Manager.onEvent -= callback;
			}
			return default(InputEventListener);
		}

		public IDisposable Subscribe(IObserver<InputEventPtr> observer)
		{
			if (InputEventListener.s_ObserverState == null)
			{
				InputEventListener.s_ObserverState = new InputEventListener.ObserverState();
			}
			if (InputEventListener.s_ObserverState.observers.length == 0)
			{
				InputSystem.s_Manager.onEvent += InputEventListener.s_ObserverState.onEventDelegate;
			}
			InputEventListener.s_ObserverState.observers.AppendWithCapacity(observer, 10);
			return new InputEventListener.DisposableObserver
			{
				observer = observer
			};
		}

		internal static InputEventListener.ObserverState s_ObserverState;

		internal class ObserverState
		{
			public ObserverState()
			{
				this.onEventDelegate = delegate(InputEventPtr eventPtr, InputDevice device)
				{
					for (int i = this.observers.length - 1; i >= 0; i--)
					{
						this.observers[i].OnNext(eventPtr);
					}
				};
			}

			public InlinedArray<IObserver<InputEventPtr>> observers;

			public Action<InputEventPtr, InputDevice> onEventDelegate;
		}

		private class DisposableObserver : IDisposable
		{
			public void Dispose()
			{
				int num = InputEventListener.s_ObserverState.observers.IndexOfReference(this.observer);
				if (num >= 0)
				{
					InputEventListener.s_ObserverState.observers.RemoveAtWithCapacity(num);
				}
				if (InputEventListener.s_ObserverState.observers.length == 0)
				{
					InputSystem.s_Manager.onEvent -= InputEventListener.s_ObserverState.onEventDelegate;
				}
			}

			public IObserver<InputEventPtr> observer;
		}
	}
}
