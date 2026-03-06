using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Threading
{
	/// <summary>Provides a mutual exclusion lock primitive where a thread trying to acquire the lock waits in a loop repeatedly checking until the lock becomes available.</summary>
	[ComVisible(false)]
	[DebuggerTypeProxy(typeof(SpinLock.SystemThreading_SpinLockDebugView))]
	[DebuggerDisplay("IsHeld = {IsHeld}")]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public struct SpinLock
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.SpinLock" /> structure with the option to track thread IDs to improve debugging.</summary>
		/// <param name="enableThreadOwnerTracking">Whether to capture and use thread IDs for debugging purposes.</param>
		public SpinLock(bool enableThreadOwnerTracking)
		{
			this.m_owner = 0;
			if (!enableThreadOwnerTracking)
			{
				this.m_owner |= int.MinValue;
			}
		}

		/// <summary>Acquires the lock in a reliable manner, such that even if an exception occurs within the method call, <paramref name="lockTaken" /> can be examined reliably to determine whether the lock was acquired.</summary>
		/// <param name="lockTaken">True if the lock is acquired; otherwise, false. <paramref name="lockTaken" /> must be initialized to false prior to calling this method.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="lockTaken" /> argument must be initialized to false prior to calling Enter.</exception>
		/// <exception cref="T:System.Threading.LockRecursionException">Thread ownership tracking is enabled, and the current thread has already acquired this lock.</exception>
		public void Enter(ref bool lockTaken)
		{
			Thread.BeginCriticalRegion();
			int owner = this.m_owner;
			if (lockTaken || (owner & -2147483647) != -2147483648 || Interlocked.CompareExchange(ref this.m_owner, owner | 1, owner, ref lockTaken) != owner)
			{
				this.ContinueTryEnter(-1, ref lockTaken);
			}
		}

		/// <summary>Attempts to acquire the lock in a reliable manner, such that even if an exception occurs within the method call, <paramref name="lockTaken" /> can be examined reliably to determine whether the lock was acquired.</summary>
		/// <param name="lockTaken">True if the lock is acquired; otherwise, false. <paramref name="lockTaken" /> must be initialized to false prior to calling this method.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="lockTaken" /> argument must be initialized to false prior to calling TryEnter.</exception>
		/// <exception cref="T:System.Threading.LockRecursionException">Thread ownership tracking is enabled, and the current thread has already acquired this lock.</exception>
		public void TryEnter(ref bool lockTaken)
		{
			this.TryEnter(0, ref lockTaken);
		}

		/// <summary>Attempts to acquire the lock in a reliable manner, such that even if an exception occurs within the method call, <paramref name="lockTaken" /> can be examined reliably to determine whether the lock was acquired.</summary>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="lockTaken">True if the lock is acquired; otherwise, false. <paramref name="lockTaken" /> must be initialized to false prior to calling this method.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out -or- timeout is greater than <see cref="F:System.Int32.MaxValue" /> milliseconds.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="lockTaken" /> argument must be initialized to false prior to calling TryEnter.</exception>
		/// <exception cref="T:System.Threading.LockRecursionException">Thread ownership tracking is enabled, and the current thread has already acquired this lock.</exception>
		public void TryEnter(TimeSpan timeout, ref bool lockTaken)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", timeout, Environment.GetResourceString("The timeout must be a value between -1 and Int32.MaxValue, inclusive."));
			}
			this.TryEnter((int)timeout.TotalMilliseconds, ref lockTaken);
		}

		/// <summary>Attempts to acquire the lock in a reliable manner, such that even if an exception occurs within the method call, <paramref name="lockTaken" /> can be examined reliably to determine whether the lock was acquired.</summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <param name="lockTaken">True if the lock is acquired; otherwise, false. <paramref name="lockTaken" /> must be initialized to false prior to calling this method.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="lockTaken" /> argument must be initialized to false prior to calling TryEnter.</exception>
		/// <exception cref="T:System.Threading.LockRecursionException">Thread ownership tracking is enabled, and the current thread has already acquired this lock.</exception>
		public void TryEnter(int millisecondsTimeout, ref bool lockTaken)
		{
			Thread.BeginCriticalRegion();
			int owner = this.m_owner;
			if ((millisecondsTimeout < -1 | lockTaken) || (owner & -2147483647) != -2147483648 || Interlocked.CompareExchange(ref this.m_owner, owner | 1, owner, ref lockTaken) != owner)
			{
				this.ContinueTryEnter(millisecondsTimeout, ref lockTaken);
			}
		}

		private void ContinueTryEnter(int millisecondsTimeout, ref bool lockTaken)
		{
			Thread.EndCriticalRegion();
			if (lockTaken)
			{
				lockTaken = false;
				throw new ArgumentException(Environment.GetResourceString("The tookLock argument must be set to false before calling this method."));
			}
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", millisecondsTimeout, Environment.GetResourceString("The timeout must be a value between -1 and Int32.MaxValue, inclusive."));
			}
			uint startTime = 0U;
			if (millisecondsTimeout != -1 && millisecondsTimeout != 0)
			{
				startTime = TimeoutHelper.GetTime();
			}
			if (this.IsThreadOwnerTrackingEnabled)
			{
				this.ContinueTryEnterWithThreadTracking(millisecondsTimeout, startTime, ref lockTaken);
				return;
			}
			int num = int.MaxValue;
			int owner = this.m_owner;
			if ((owner & 1) == 0)
			{
				Thread.BeginCriticalRegion();
				if (Interlocked.CompareExchange(ref this.m_owner, owner | 1, owner, ref lockTaken) == owner)
				{
					return;
				}
				Thread.EndCriticalRegion();
			}
			else if ((owner & 2147483646) != SpinLock.MAXIMUM_WAITERS)
			{
				num = (Interlocked.Add(ref this.m_owner, 2) & 2147483646) >> 1;
			}
			if (millisecondsTimeout == 0 || (millisecondsTimeout != -1 && TimeoutHelper.UpdateTimeOut(startTime, millisecondsTimeout) <= 0))
			{
				this.DecrementWaiters();
				return;
			}
			int processorCount = PlatformHelper.ProcessorCount;
			if (num < processorCount)
			{
				int num2 = 1;
				for (int i = 1; i <= num * 100; i++)
				{
					Thread.SpinWait((num + i) * 100 * num2);
					if (num2 < processorCount)
					{
						num2++;
					}
					owner = this.m_owner;
					if ((owner & 1) == 0)
					{
						Thread.BeginCriticalRegion();
						int value = ((owner & 2147483646) == 0) ? (owner | 1) : (owner - 2 | 1);
						if (Interlocked.CompareExchange(ref this.m_owner, value, owner, ref lockTaken) == owner)
						{
							return;
						}
						Thread.EndCriticalRegion();
					}
				}
			}
			if (millisecondsTimeout != -1 && TimeoutHelper.UpdateTimeOut(startTime, millisecondsTimeout) <= 0)
			{
				this.DecrementWaiters();
				return;
			}
			int num3 = 0;
			for (;;)
			{
				owner = this.m_owner;
				if ((owner & 1) == 0)
				{
					Thread.BeginCriticalRegion();
					int value2 = ((owner & 2147483646) == 0) ? (owner | 1) : (owner - 2 | 1);
					if (Interlocked.CompareExchange(ref this.m_owner, value2, owner, ref lockTaken) == owner)
					{
						break;
					}
					Thread.EndCriticalRegion();
				}
				if (num3 % 40 == 0)
				{
					Thread.Sleep(1);
				}
				else if (num3 % 10 == 0)
				{
					Thread.Sleep(0);
				}
				else
				{
					Thread.Yield();
				}
				if (num3 % 10 == 0 && millisecondsTimeout != -1 && TimeoutHelper.UpdateTimeOut(startTime, millisecondsTimeout) <= 0)
				{
					goto Block_25;
				}
				num3++;
			}
			return;
			Block_25:
			this.DecrementWaiters();
		}

		private void DecrementWaiters()
		{
			SpinWait spinWait = default(SpinWait);
			for (;;)
			{
				int owner = this.m_owner;
				if ((owner & 2147483646) == 0)
				{
					break;
				}
				if (Interlocked.CompareExchange(ref this.m_owner, owner - 2, owner) == owner)
				{
					return;
				}
				spinWait.SpinOnce();
			}
		}

		private void ContinueTryEnterWithThreadTracking(int millisecondsTimeout, uint startTime, ref bool lockTaken)
		{
			int num = 0;
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			if (this.m_owner == managedThreadId)
			{
				throw new LockRecursionException(Environment.GetResourceString("The calling thread already holds the lock."));
			}
			SpinWait spinWait = default(SpinWait);
			for (;;)
			{
				spinWait.SpinOnce();
				if (this.m_owner == num)
				{
					Thread.BeginCriticalRegion();
					if (Interlocked.CompareExchange(ref this.m_owner, managedThreadId, num, ref lockTaken) == num)
					{
						break;
					}
					Thread.EndCriticalRegion();
				}
				if (millisecondsTimeout == 0 || (millisecondsTimeout != -1 && spinWait.NextSpinWillYield && TimeoutHelper.UpdateTimeOut(startTime, millisecondsTimeout) <= 0))
				{
					return;
				}
			}
		}

		/// <summary>Releases the lock.</summary>
		/// <exception cref="T:System.Threading.SynchronizationLockException">Thread ownership tracking is enabled, and the current thread is not the owner of this lock.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Exit()
		{
			if ((this.m_owner & -2147483648) == 0)
			{
				this.ExitSlowPath(true);
			}
			else
			{
				Interlocked.Decrement(ref this.m_owner);
			}
			Thread.EndCriticalRegion();
		}

		/// <summary>Releases the lock.</summary>
		/// <param name="useMemoryBarrier">A Boolean value that indicates whether a memory fence should be issued in order to immediately publish the exit operation to other threads.</param>
		/// <exception cref="T:System.Threading.SynchronizationLockException">Thread ownership tracking is enabled, and the current thread is not the owner of this lock.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Exit(bool useMemoryBarrier)
		{
			if ((this.m_owner & -2147483648) != 0 && !useMemoryBarrier)
			{
				int owner = this.m_owner;
				this.m_owner = (owner & -2);
			}
			else
			{
				this.ExitSlowPath(useMemoryBarrier);
			}
			Thread.EndCriticalRegion();
		}

		private void ExitSlowPath(bool useMemoryBarrier)
		{
			bool flag = (this.m_owner & int.MinValue) == 0;
			if (flag && !this.IsHeldByCurrentThread)
			{
				throw new SynchronizationLockException(Environment.GetResourceString("The calling thread does not hold the lock."));
			}
			if (useMemoryBarrier)
			{
				if (flag)
				{
					Interlocked.Exchange(ref this.m_owner, 0);
					return;
				}
				Interlocked.Decrement(ref this.m_owner);
				return;
			}
			else
			{
				if (flag)
				{
					this.m_owner = 0;
					return;
				}
				int owner = this.m_owner;
				this.m_owner = (owner & -2);
				return;
			}
		}

		/// <summary>Gets whether the lock is currently held by any thread.</summary>
		/// <returns>true if the lock is currently held by any thread; otherwise false.</returns>
		public bool IsHeld
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				if (this.IsThreadOwnerTrackingEnabled)
				{
					return this.m_owner != 0;
				}
				return (this.m_owner & 1) != 0;
			}
		}

		/// <summary>Gets whether the lock is held by the current thread.</summary>
		/// <returns>true if the lock is held by the current thread; otherwise false.</returns>
		/// <exception cref="T:System.InvalidOperationException">Thread ownership tracking is disabled.</exception>
		public bool IsHeldByCurrentThread
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				if (!this.IsThreadOwnerTrackingEnabled)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Thread tracking is disabled."));
				}
				return (this.m_owner & int.MaxValue) == Thread.CurrentThread.ManagedThreadId;
			}
		}

		/// <summary>Gets whether thread ownership tracking is enabled for this instance.</summary>
		/// <returns>true if thread ownership tracking is enabled for this instance; otherwise false.</returns>
		public bool IsThreadOwnerTrackingEnabled
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return (this.m_owner & int.MinValue) == 0;
			}
		}

		private volatile int m_owner;

		private const int SPINNING_FACTOR = 100;

		private const int SLEEP_ONE_FREQUENCY = 40;

		private const int SLEEP_ZERO_FREQUENCY = 10;

		private const int TIMEOUT_CHECK_FREQUENCY = 10;

		private const int LOCK_ID_DISABLE_MASK = -2147483648;

		private const int LOCK_ANONYMOUS_OWNED = 1;

		private const int WAITERS_MASK = 2147483646;

		private const int ID_DISABLED_AND_ANONYMOUS_OWNED = -2147483647;

		private const int LOCK_UNOWNED = 0;

		private static int MAXIMUM_WAITERS = 2147483646;

		internal class SystemThreading_SpinLockDebugView
		{
			public SystemThreading_SpinLockDebugView(SpinLock spinLock)
			{
				this.m_spinLock = spinLock;
			}

			public bool? IsHeldByCurrentThread
			{
				get
				{
					bool? result;
					try
					{
						result = new bool?(this.m_spinLock.IsHeldByCurrentThread);
					}
					catch (InvalidOperationException)
					{
						result = null;
					}
					return result;
				}
			}

			public int? OwnerThreadID
			{
				get
				{
					if (this.m_spinLock.IsThreadOwnerTrackingEnabled)
					{
						return new int?(this.m_spinLock.m_owner);
					}
					return null;
				}
			}

			public bool IsHeld
			{
				get
				{
					return this.m_spinLock.IsHeld;
				}
			}

			private SpinLock m_spinLock;
		}
	}
}
