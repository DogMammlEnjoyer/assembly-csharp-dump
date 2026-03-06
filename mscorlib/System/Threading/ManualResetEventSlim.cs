using System;
using System.Diagnostics;

namespace System.Threading
{
	/// <summary>Provides a slimmed down version of <see cref="T:System.Threading.ManualResetEvent" />.</summary>
	[DebuggerDisplay("Set = {IsSet}")]
	public class ManualResetEventSlim : IDisposable
	{
		/// <summary>Gets the underlying <see cref="T:System.Threading.WaitHandle" /> object for this <see cref="T:System.Threading.ManualResetEventSlim" />.</summary>
		/// <returns>The underlying <see cref="T:System.Threading.WaitHandle" /> event object fore this <see cref="T:System.Threading.ManualResetEventSlim" />.</returns>
		public WaitHandle WaitHandle
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.m_eventObj == null)
				{
					this.LazyInitializeEvent();
				}
				return this.m_eventObj;
			}
		}

		/// <summary>Gets whether the event is set.</summary>
		/// <returns>true if the event has is set; otherwise, false.</returns>
		public bool IsSet
		{
			get
			{
				return ManualResetEventSlim.ExtractStatePortion(this.m_combinedState, int.MinValue) != 0;
			}
			private set
			{
				this.UpdateStateAtomically((value ? 1 : 0) << 31, int.MinValue);
			}
		}

		/// <summary>Gets the number of spin waits that will occur before falling back to a kernel-based wait operation.</summary>
		/// <returns>Returns the number of spin waits that will occur before falling back to a kernel-based wait operation.</returns>
		public int SpinCount
		{
			get
			{
				return ManualResetEventSlim.ExtractStatePortionAndShiftRight(this.m_combinedState, 1073217536, 19);
			}
			private set
			{
				this.m_combinedState = ((this.m_combinedState & -1073217537) | value << 19);
			}
		}

		private int Waiters
		{
			get
			{
				return ManualResetEventSlim.ExtractStatePortionAndShiftRight(this.m_combinedState, 524287, 0);
			}
			set
			{
				if (value >= 524287)
				{
					throw new InvalidOperationException(string.Format("There are too many threads currently waiting on the event. A maximum of {0} waiting threads are supported.", 524287));
				}
				this.UpdateStateAtomically(value, 524287);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.ManualResetEventSlim" /> class with an initial state of nonsignaled.</summary>
		public ManualResetEventSlim() : this(false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.ManualResetEventSlim" /> class with a Boolean value indicating whether to set the intial state to signaled.</summary>
		/// <param name="initialState">true to set the initial state signaled; false to set the initial state to nonsignaled.</param>
		public ManualResetEventSlim(bool initialState)
		{
			this.Initialize(initialState, SpinWait.SpinCountforSpinBeforeWait);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.ManualResetEventSlim" /> class with a Boolean value indicating whether to set the intial state to signaled and a specified spin count.</summary>
		/// <param name="initialState">true to set the initial state to signaled; false to set the initial state to nonsignaled.</param>
		/// <param name="spinCount">The number of spin waits that will occur before falling back to a kernel-based wait operation.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="spinCount" /> is less than 0 or greater than the maximum allowed value.</exception>
		public ManualResetEventSlim(bool initialState, int spinCount)
		{
			if (spinCount < 0)
			{
				throw new ArgumentOutOfRangeException("spinCount");
			}
			if (spinCount > 2047)
			{
				throw new ArgumentOutOfRangeException("spinCount", string.Format("The spinCount argument must be in the range 0 to {0}, inclusive.", 2047));
			}
			this.Initialize(initialState, spinCount);
		}

		private void Initialize(bool initialState, int spinCount)
		{
			this.m_combinedState = (initialState ? int.MinValue : 0);
			this.SpinCount = (PlatformHelper.IsSingleProcessor ? 1 : spinCount);
		}

		private void EnsureLockObjectCreated()
		{
			if (this.m_lock != null)
			{
				return;
			}
			object value = new object();
			Interlocked.CompareExchange(ref this.m_lock, value, null);
		}

		private bool LazyInitializeEvent()
		{
			bool isSet = this.IsSet;
			ManualResetEvent manualResetEvent = new ManualResetEvent(isSet);
			if (Interlocked.CompareExchange<ManualResetEvent>(ref this.m_eventObj, manualResetEvent, null) != null)
			{
				manualResetEvent.Dispose();
				return false;
			}
			if (this.IsSet != isSet)
			{
				ManualResetEvent obj = manualResetEvent;
				lock (obj)
				{
					if (this.m_eventObj == manualResetEvent)
					{
						manualResetEvent.Set();
					}
				}
			}
			return true;
		}

		/// <summary>Sets the state of the event to signaled, which allows one or more threads waiting on the event to proceed.</summary>
		public void Set()
		{
			this.Set(false);
		}

		private void Set(bool duringCancellation)
		{
			this.IsSet = true;
			if (this.Waiters > 0)
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					Monitor.PulseAll(this.m_lock);
				}
			}
			ManualResetEvent eventObj = this.m_eventObj;
			if (eventObj != null && !duringCancellation)
			{
				ManualResetEvent obj = eventObj;
				lock (obj)
				{
					if (this.m_eventObj != null)
					{
						this.m_eventObj.Set();
					}
				}
			}
		}

		/// <summary>Sets the state of the event to nonsignaled, which causes threads to block.</summary>
		/// <exception cref="T:System.ObjectDisposedException">The object has already been disposed.</exception>
		public void Reset()
		{
			this.ThrowIfDisposed();
			if (this.m_eventObj != null)
			{
				this.m_eventObj.Reset();
			}
			this.IsSet = false;
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.ManualResetEventSlim" /> is set.</summary>
		/// <exception cref="T:System.InvalidOperationException">The maximum number of waiters has been exceeded.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The object has already been disposed.</exception>
		public void Wait()
		{
			this.Wait(-1, default(CancellationToken));
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.ManualResetEventSlim" /> receives a signal, while observing a <see cref="T:System.Threading.CancellationToken" />.</summary>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> to observe.</param>
		/// <exception cref="T:System.InvalidOperationException">The maximum number of waiters has been exceeded.</exception>
		/// <exception cref="T:System.OperationCanceledException">
		///   <paramref name="cancellationToken" /> was canceled.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The object has already been disposed or the <see cref="T:System.Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has been disposed.</exception>
		public void Wait(CancellationToken cancellationToken)
		{
			this.Wait(-1, cancellationToken);
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.ManualResetEventSlim" /> is set, using a <see cref="T:System.TimeSpan" /> to measure the time interval.</summary>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Threading.ManualResetEventSlim" /> was set; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// The number of milliseconds in <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The maximum number of waiters has been exceeded.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The object has already been disposed.</exception>
		public bool Wait(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.Wait((int)num, default(CancellationToken));
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.ManualResetEventSlim" /> is set, using a <see cref="T:System.TimeSpan" /> to measure the time interval, while observing a <see cref="T:System.Threading.CancellationToken" />.</summary>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> to observe.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Threading.ManualResetEventSlim" /> was set; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.OperationCanceledException">
		///   <paramref name="cancellationToken" /> was canceled.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// The number of milliseconds in <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The maximum number of waiters has been exceeded.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The object has already been disposed or the <see cref="T:System.Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has been disposed.</exception>
		public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.Wait((int)num, cancellationToken);
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.ManualResetEventSlim" /> is set, using a 32-bit signed integer to measure the time interval.</summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" />(-1) to wait indefinitely.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Threading.ManualResetEventSlim" /> was set; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.InvalidOperationException">The maximum number of waiters has been exceeded.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The object has already been disposed.</exception>
		public bool Wait(int millisecondsTimeout)
		{
			return this.Wait(millisecondsTimeout, default(CancellationToken));
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.ManualResetEventSlim" /> is set, using a 32-bit signed integer to measure the time interval, while observing a <see cref="T:System.Threading.CancellationToken" />.</summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" />(-1) to wait indefinitely.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> to observe.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Threading.ManualResetEventSlim" /> was set; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.OperationCanceledException">
		///   <paramref name="cancellationToken" /> was canceled.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.InvalidOperationException">The maximum number of waiters has been exceeded.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The object has already been disposed or the <see cref="T:System.Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has been disposed.</exception>
		public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
		{
			this.ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			if (!this.IsSet)
			{
				if (millisecondsTimeout == 0)
				{
					return false;
				}
				uint startTime = 0U;
				bool flag = false;
				int num = millisecondsTimeout;
				if (millisecondsTimeout != -1)
				{
					startTime = TimeoutHelper.GetTime();
					flag = true;
				}
				int spinCount = this.SpinCount;
				SpinWait spinWait = default(SpinWait);
				while (spinWait.Count < spinCount)
				{
					spinWait.SpinOnce(40);
					if (this.IsSet)
					{
						return true;
					}
					if (spinWait.Count >= 100 && spinWait.Count % 10 == 0)
					{
						cancellationToken.ThrowIfCancellationRequested();
					}
				}
				this.EnsureLockObjectCreated();
				using (cancellationToken.InternalRegisterWithoutEC(ManualResetEventSlim.s_cancellationTokenCallback, this))
				{
					object @lock = this.m_lock;
					lock (@lock)
					{
						while (!this.IsSet)
						{
							cancellationToken.ThrowIfCancellationRequested();
							if (flag)
							{
								num = TimeoutHelper.UpdateTimeOut(startTime, millisecondsTimeout);
								if (num <= 0)
								{
									return false;
								}
							}
							this.Waiters++;
							if (this.IsSet)
							{
								int waiters = this.Waiters;
								this.Waiters = waiters - 1;
								return true;
							}
							try
							{
								if (!Monitor.Wait(this.m_lock, num))
								{
									return false;
								}
							}
							finally
							{
								this.Waiters--;
							}
						}
					}
				}
				return true;
			}
			return true;
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Threading.ManualResetEventSlim" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Threading.ManualResetEventSlim" />, and optionally releases the managed resources.</summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if ((this.m_combinedState & 1073741824) != 0)
			{
				return;
			}
			this.m_combinedState |= 1073741824;
			if (disposing)
			{
				ManualResetEvent eventObj = this.m_eventObj;
				if (eventObj != null)
				{
					ManualResetEvent obj = eventObj;
					lock (obj)
					{
						eventObj.Dispose();
						this.m_eventObj = null;
					}
				}
			}
		}

		private void ThrowIfDisposed()
		{
			if ((this.m_combinedState & 1073741824) != 0)
			{
				throw new ObjectDisposedException("The event has been disposed.");
			}
		}

		private static void CancellationTokenCallback(object obj)
		{
			ManualResetEventSlim manualResetEventSlim = obj as ManualResetEventSlim;
			object @lock = manualResetEventSlim.m_lock;
			lock (@lock)
			{
				Monitor.PulseAll(manualResetEventSlim.m_lock);
			}
		}

		private void UpdateStateAtomically(int newBits, int updateBitsMask)
		{
			SpinWait spinWait = default(SpinWait);
			for (;;)
			{
				int combinedState = this.m_combinedState;
				int value = (combinedState & ~updateBitsMask) | newBits;
				if (Interlocked.CompareExchange(ref this.m_combinedState, value, combinedState) == combinedState)
				{
					break;
				}
				spinWait.SpinOnce();
			}
		}

		private static int ExtractStatePortionAndShiftRight(int state, int mask, int rightBitShiftCount)
		{
			return (int)((uint)(state & mask) >> rightBitShiftCount);
		}

		private static int ExtractStatePortion(int state, int mask)
		{
			return state & mask;
		}

		private const int DEFAULT_SPIN_SP = 1;

		private volatile object m_lock;

		private volatile ManualResetEvent m_eventObj;

		private volatile int m_combinedState;

		private const int SignalledState_BitMask = -2147483648;

		private const int SignalledState_ShiftCount = 31;

		private const int Dispose_BitMask = 1073741824;

		private const int SpinCountState_BitMask = 1073217536;

		private const int SpinCountState_ShiftCount = 19;

		private const int SpinCountState_MaxValue = 2047;

		private const int NumWaitersState_BitMask = 524287;

		private const int NumWaitersState_ShiftCount = 0;

		private const int NumWaitersState_MaxValue = 524287;

		private static Action<object> s_cancellationTokenCallback = new Action<object>(ManualResetEventSlim.CancellationTokenCallback);
	}
}
