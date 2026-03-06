using System;
using System.Threading;

namespace UnityEngine
{
	public class AwaitableCompletionSource
	{
		public Awaitable Awaitable { get; private set; } = Awaitable.NewManagedAwaitable();

		public void SetResult()
		{
			bool flag = !this.TrySetResult();
			if (flag)
			{
				throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
			}
		}

		public void SetCanceled()
		{
			bool flag = !this.TrySetCanceled();
			if (flag)
			{
				throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
			}
		}

		public void SetException(Exception exception)
		{
			bool flag = !this.TrySetException(exception);
			if (flag)
			{
				throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
			}
		}

		private bool CheckAndAcquireCompletionState()
		{
			return Interlocked.CompareExchange(ref this._state, 1, 0) == 0;
		}

		public bool TrySetResult()
		{
			bool flag = !this.CheckAndAcquireCompletionState();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.Awaitable.RaiseManagedCompletion();
				result = true;
			}
			return result;
		}

		public bool TrySetCanceled()
		{
			bool flag = !this.CheckAndAcquireCompletionState();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.Awaitable.Cancel();
				result = true;
			}
			return result;
		}

		public bool TrySetException(Exception exception)
		{
			bool flag = !this.CheckAndAcquireCompletionState();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.Awaitable.RaiseManagedCompletion(exception);
				result = true;
			}
			return result;
		}

		public void Reset()
		{
			this.Awaitable = Awaitable.NewManagedAwaitable();
			this._state = 0;
		}

		private volatile int _state;
	}
}
