using System;
using System.Runtime.InteropServices;
using Unity;

namespace System.Threading
{
	/// <summary>Represents a handle that has been registered when calling <see cref="M:System.Threading.ThreadPool.RegisterWaitForSingleObject(System.Threading.WaitHandle,System.Threading.WaitOrTimerCallback,System.Object,System.UInt32,System.Boolean)" />. This class cannot be inherited.</summary>
	[ComVisible(true)]
	public sealed class RegisteredWaitHandle : MarshalByRefObject
	{
		internal RegisteredWaitHandle(WaitHandle waitObject, WaitOrTimerCallback callback, object state, TimeSpan timeout, bool executeOnlyOnce)
		{
			this._waitObject = waitObject;
			this._callback = callback;
			this._state = state;
			this._timeout = timeout;
			this._executeOnlyOnce = executeOnlyOnce;
			this._finalEvent = null;
			this._cancelEvent = new ManualResetEvent(false);
			this._callsInProcess = 0;
			this._unregistered = false;
		}

		internal void Wait(object state)
		{
			bool flag = false;
			try
			{
				this._waitObject.SafeWaitHandle.DangerousAddRef(ref flag);
				RegisteredWaitHandle obj;
				try
				{
					WaitHandle[] waitHandles = new WaitHandle[]
					{
						this._waitObject,
						this._cancelEvent
					};
					do
					{
						int num = WaitHandle.WaitAny(waitHandles, this._timeout, false);
						if (!this._unregistered)
						{
							obj = this;
							lock (obj)
							{
								this._callsInProcess++;
							}
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.DoCallBack), num == 258);
						}
					}
					while (!this._unregistered && !this._executeOnlyOnce);
				}
				catch
				{
				}
				obj = this;
				lock (obj)
				{
					this._unregistered = true;
					if (this._callsInProcess == 0 && this._finalEvent != null)
					{
						NativeEventCalls.SetEvent(this._finalEvent.SafeWaitHandle);
						this._finalEvent = null;
					}
				}
			}
			catch (ObjectDisposedException)
			{
				if (flag)
				{
					throw;
				}
			}
			finally
			{
				if (flag)
				{
					this._waitObject.SafeWaitHandle.DangerousRelease();
				}
			}
		}

		private void DoCallBack(object timedOut)
		{
			try
			{
				if (this._callback != null)
				{
					this._callback(this._state, (bool)timedOut);
				}
			}
			finally
			{
				lock (this)
				{
					this._callsInProcess--;
					if (this._unregistered && this._callsInProcess == 0 && this._finalEvent != null)
					{
						NativeEventCalls.SetEvent(this._finalEvent.SafeWaitHandle);
						this._finalEvent = null;
					}
				}
			}
		}

		/// <summary>Cancels a registered wait operation issued by the <see cref="M:System.Threading.ThreadPool.RegisterWaitForSingleObject(System.Threading.WaitHandle,System.Threading.WaitOrTimerCallback,System.Object,System.UInt32,System.Boolean)" /> method.</summary>
		/// <param name="waitObject">The <see cref="T:System.Threading.WaitHandle" /> to be signaled.</param>
		/// <returns>
		///   <see langword="true" /> if the function succeeds; otherwise, <see langword="false" />.</returns>
		[ComVisible(true)]
		public bool Unregister(WaitHandle waitObject)
		{
			bool result;
			lock (this)
			{
				if (this._unregistered)
				{
					result = false;
				}
				else
				{
					this._finalEvent = waitObject;
					this._unregistered = true;
					this._cancelEvent.Set();
					result = true;
				}
			}
			return result;
		}

		internal RegisteredWaitHandle()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private WaitHandle _waitObject;

		private WaitOrTimerCallback _callback;

		private object _state;

		private WaitHandle _finalEvent;

		private ManualResetEvent _cancelEvent;

		private TimeSpan _timeout;

		private int _callsInProcess;

		private bool _executeOnlyOnce;

		private bool _unregistered;
	}
}
