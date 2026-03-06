using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Threading
{
	/// <summary>Provides a mechanism for executing a method on a thread pool thread at specified intervals. This class cannot be inherited.</summary>
	[ComVisible(true)]
	public sealed class Timer : MarshalByRefObject, IDisposable, IAsyncDisposable
	{
		private static Timer.Scheduler scheduler
		{
			get
			{
				return Timer.Scheduler.Instance;
			}
		}

		/// <summary>Initializes a new instance of the <see langword="Timer" /> class, using a 32-bit signed integer to specify the time interval.</summary>
		/// <param name="callback">A <see cref="T:System.Threading.TimerCallback" /> delegate representing a method to be executed.</param>
		/// <param name="state">An object containing information to be used by the callback method, or <see langword="null" />.</param>
		/// <param name="dueTime">The amount of time to delay before <paramref name="callback" /> is invoked, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to prevent the timer from starting. Specify zero (0) to start the timer immediately.</param>
		/// <param name="period">The time interval between invocations of <paramref name="callback" />, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to disable periodic signaling.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="callback" /> parameter is <see langword="null" />.</exception>
		public Timer(TimerCallback callback, object state, int dueTime, int period)
		{
			this.Init(callback, state, (long)dueTime, (long)period);
		}

		/// <summary>Initializes a new instance of the <see langword="Timer" /> class, using 64-bit signed integers to measure time intervals.</summary>
		/// <param name="callback">A <see cref="T:System.Threading.TimerCallback" /> delegate representing a method to be executed.</param>
		/// <param name="state">An object containing information to be used by the callback method, or <see langword="null" />.</param>
		/// <param name="dueTime">The amount of time to delay before <paramref name="callback" /> is invoked, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to prevent the timer from starting. Specify zero (0) to start the timer immediately.</param>
		/// <param name="period">The time interval between invocations of <paramref name="callback" />, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to disable periodic signaling.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter is greater than 4294967294.</exception>
		public Timer(TimerCallback callback, object state, long dueTime, long period)
		{
			this.Init(callback, state, dueTime, period);
		}

		/// <summary>Initializes a new instance of the <see langword="Timer" /> class, using <see cref="T:System.TimeSpan" /> values to measure time intervals.</summary>
		/// <param name="callback">A delegate representing a method to be executed.</param>
		/// <param name="state">An object containing information to be used by the callback method, or <see langword="null" />.</param>
		/// <param name="dueTime">The amount of time to delay before the <paramref name="callback" /> parameter invokes its methods. Specify negative one (-1) milliseconds to prevent the timer from starting. Specify zero (0) to start the timer immediately.</param>
		/// <param name="period">The time interval between invocations of the methods referenced by <paramref name="callback" />. Specify negative one (-1) milliseconds to disable periodic signaling.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The number of milliseconds in the value of <paramref name="dueTime" /> or <paramref name="period" /> is negative and not equal to <see cref="F:System.Threading.Timeout.Infinite" />, or is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="callback" /> parameter is <see langword="null" />.</exception>
		public Timer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
		{
			this.Init(callback, state, (long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds);
		}

		/// <summary>Initializes a new instance of the <see langword="Timer" /> class, using 32-bit unsigned integers to measure time intervals.</summary>
		/// <param name="callback">A delegate representing a method to be executed.</param>
		/// <param name="state">An object containing information to be used by the callback method, or <see langword="null" />.</param>
		/// <param name="dueTime">The amount of time to delay before <paramref name="callback" /> is invoked, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to prevent the timer from starting. Specify zero (0) to start the timer immediately.</param>
		/// <param name="period">The time interval between invocations of <paramref name="callback" />, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to disable periodic signaling.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="callback" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public Timer(TimerCallback callback, object state, uint dueTime, uint period)
		{
			long dueTime2 = (long)((dueTime == uint.MaxValue) ? ulong.MaxValue : ((ulong)dueTime));
			long period2 = (long)((period == uint.MaxValue) ? ulong.MaxValue : ((ulong)period));
			this.Init(callback, state, dueTime2, period2);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Timer" /> class with an infinite period and an infinite due time, using the newly created <see cref="T:System.Threading.Timer" /> object as the state object.</summary>
		/// <param name="callback">A <see cref="T:System.Threading.TimerCallback" /> delegate representing a method to be executed.</param>
		public Timer(TimerCallback callback)
		{
			this.Init(callback, this, -1L, -1L);
		}

		private void Init(TimerCallback callback, object state, long dueTime, long period)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			this.callback = callback;
			this.state = state;
			this.is_dead = false;
			this.is_added = false;
			this.Change(dueTime, period, true);
		}

		/// <summary>Changes the start time and the interval between method invocations for a timer, using 32-bit signed integers to measure time intervals.</summary>
		/// <param name="dueTime">The amount of time to delay before the invoking the callback method specified when the <see cref="T:System.Threading.Timer" /> was constructed, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
		/// <param name="period">The time interval between invocations of the callback method specified when the <see cref="T:System.Threading.Timer" /> was constructed, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to disable periodic signaling.</param>
		/// <returns>
		///   <see langword="true" /> if the timer was successfully updated; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Threading.Timer" /> has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite" />.</exception>
		public bool Change(int dueTime, int period)
		{
			return this.Change((long)dueTime, (long)period, false);
		}

		/// <summary>Changes the start time and the interval between method invocations for a timer, using <see cref="T:System.TimeSpan" /> values to measure time intervals.</summary>
		/// <param name="dueTime">A <see cref="T:System.TimeSpan" /> representing the amount of time to delay before invoking the callback method specified when the <see cref="T:System.Threading.Timer" /> was constructed. Specify negative one (-1) milliseconds to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
		/// <param name="period">The time interval between invocations of the callback method specified when the <see cref="T:System.Threading.Timer" /> was constructed. Specify negative one (-1) milliseconds to disable periodic signaling.</param>
		/// <returns>
		///   <see langword="true" /> if the timer was successfully updated; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Threading.Timer" /> has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter, in milliseconds, is less than -1.</exception>
		/// <exception cref="T:System.NotSupportedException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter, in milliseconds, is greater than 4294967294.</exception>
		public bool Change(TimeSpan dueTime, TimeSpan period)
		{
			return this.Change((long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds, false);
		}

		/// <summary>Changes the start time and the interval between method invocations for a timer, using 32-bit unsigned integers to measure time intervals.</summary>
		/// <param name="dueTime">The amount of time to delay before the invoking the callback method specified when the <see cref="T:System.Threading.Timer" /> was constructed, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
		/// <param name="period">The time interval between invocations of the callback method specified when the <see cref="T:System.Threading.Timer" /> was constructed, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to disable periodic signaling.</param>
		/// <returns>
		///   <see langword="true" /> if the timer was successfully updated; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Threading.Timer" /> has already been disposed.</exception>
		[CLSCompliant(false)]
		public bool Change(uint dueTime, uint period)
		{
			long dueTime2 = (long)((dueTime == uint.MaxValue) ? ulong.MaxValue : ((ulong)dueTime));
			long period2 = (long)((period == uint.MaxValue) ? ulong.MaxValue : ((ulong)period));
			return this.Change(dueTime2, period2, false);
		}

		/// <summary>Releases all resources used by the current instance of <see cref="T:System.Threading.Timer" />.</summary>
		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			Timer.scheduler.Remove(this);
		}

		/// <summary>Changes the start time and the interval between method invocations for a timer, using 64-bit signed integers to measure time intervals.</summary>
		/// <param name="dueTime">The amount of time to delay before the invoking the callback method specified when the <see cref="T:System.Threading.Timer" /> was constructed, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
		/// <param name="period">The time interval between invocations of the callback method specified when the <see cref="T:System.Threading.Timer" /> was constructed, in milliseconds. Specify <see cref="F:System.Threading.Timeout.Infinite" /> to disable periodic signaling.</param>
		/// <returns>
		///   <see langword="true" /> if the timer was successfully updated; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Threading.Timer" /> has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter is less than -1.</exception>
		/// <exception cref="T:System.NotSupportedException">The <paramref name="dueTime" /> or <paramref name="period" /> parameter is greater than 4294967294.</exception>
		public bool Change(long dueTime, long period)
		{
			return this.Change(dueTime, period, false);
		}

		private bool Change(long dueTime, long period, bool first)
		{
			if (dueTime > (long)((ulong)-2))
			{
				throw new ArgumentOutOfRangeException("dueTime", "Due time too large");
			}
			if (period > (long)((ulong)-2))
			{
				throw new ArgumentOutOfRangeException("period", "Period too large");
			}
			if (dueTime < -1L)
			{
				throw new ArgumentOutOfRangeException("dueTime");
			}
			if (period < -1L)
			{
				throw new ArgumentOutOfRangeException("period");
			}
			if (this.disposed)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("Cannot access a disposed object."));
			}
			this.due_time_ms = dueTime;
			this.period_ms = period;
			long new_next_run;
			if (dueTime == 0L)
			{
				new_next_run = 0L;
			}
			else if (dueTime < 0L)
			{
				new_next_run = long.MaxValue;
				if (first)
				{
					this.next_run = new_next_run;
					return true;
				}
			}
			else
			{
				new_next_run = dueTime * 10000L + Timer.GetTimeMonotonic();
			}
			Timer.scheduler.Change(this, new_next_run);
			return true;
		}

		/// <summary>Releases all resources used by the current instance of <see cref="T:System.Threading.Timer" /> and signals when the timer has been disposed of.</summary>
		/// <param name="notifyObject">The <see cref="T:System.Threading.WaitHandle" /> to be signaled when the <see langword="Timer" /> has been disposed of.</param>
		/// <returns>
		///   <see langword="true" /> if the function succeeds; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="notifyObject" /> parameter is <see langword="null" />.</exception>
		public bool Dispose(WaitHandle notifyObject)
		{
			if (notifyObject == null)
			{
				throw new ArgumentNullException("notifyObject");
			}
			this.Dispose();
			NativeEventCalls.SetEvent(notifyObject.SafeWaitHandle);
			return true;
		}

		public ValueTask DisposeAsync()
		{
			this.Dispose();
			return new ValueTask(Task.FromResult<object>(null));
		}

		internal void KeepRootedWhileScheduled()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetTimeMonotonic();

		private TimerCallback callback;

		private object state;

		private long due_time_ms;

		private long period_ms;

		private long next_run;

		private bool disposed;

		private bool is_dead;

		private bool is_added;

		private const long MaxValue = 4294967294L;

		private struct TimerComparer : IComparer, IComparer<Timer>
		{
			int IComparer.Compare(object x, object y)
			{
				if (x == y)
				{
					return 0;
				}
				Timer timer = x as Timer;
				if (timer == null)
				{
					return -1;
				}
				Timer timer2 = y as Timer;
				if (timer2 == null)
				{
					return 1;
				}
				return this.Compare(timer, timer2);
			}

			public int Compare(Timer tx, Timer ty)
			{
				return Math.Sign(tx.next_run - ty.next_run);
			}
		}

		private sealed class Scheduler
		{
			private void InitScheduler()
			{
				this.changed = new ManualResetEvent(false);
				new Thread(new ThreadStart(this.SchedulerThread))
				{
					IsBackground = true
				}.Start();
			}

			private void WakeupScheduler()
			{
				this.changed.Set();
			}

			private void SchedulerThread()
			{
				Thread.CurrentThread.Name = "Timer-Scheduler";
				for (;;)
				{
					int millisecondsTimeout = -1;
					lock (this)
					{
						this.changed.Reset();
						millisecondsTimeout = this.RunSchedulerLoop();
					}
					this.changed.WaitOne(millisecondsTimeout);
				}
			}

			public static Timer.Scheduler Instance
			{
				get
				{
					return Timer.Scheduler.instance;
				}
			}

			private Scheduler()
			{
				this.list = new List<Timer>(1024);
				this.InitScheduler();
			}

			public void Remove(Timer timer)
			{
				lock (this)
				{
					this.InternalRemove(timer);
				}
			}

			public void Change(Timer timer, long new_next_run)
			{
				if (timer.is_dead)
				{
					timer.is_dead = false;
				}
				bool flag = false;
				lock (this)
				{
					this.needReSort = true;
					if (!timer.is_added)
					{
						timer.next_run = new_next_run;
						this.Add(timer);
						flag = (this.current_next_run > new_next_run);
					}
					else
					{
						if (new_next_run == 9223372036854775807L)
						{
							timer.next_run = new_next_run;
							this.InternalRemove(timer);
							return;
						}
						if (!timer.disposed)
						{
							timer.next_run = new_next_run;
							flag = (this.current_next_run > new_next_run);
						}
					}
				}
				if (flag)
				{
					this.WakeupScheduler();
				}
			}

			private void Add(Timer timer)
			{
				timer.is_added = true;
				this.needReSort = true;
				this.list.Add(timer);
				if (this.list.Count == 1)
				{
					this.WakeupScheduler();
				}
			}

			private void InternalRemove(Timer timer)
			{
				timer.is_dead = true;
				this.needReSort = true;
			}

			private static void TimerCB(object o)
			{
				Timer timer = (Timer)o;
				timer.callback(timer.state);
			}

			private void FireTimer(Timer timer)
			{
				long period_ms = timer.period_ms;
				long due_time_ms = timer.due_time_ms;
				if (period_ms == -1L || ((period_ms == 0L || period_ms == -1L) && due_time_ms != -1L))
				{
					timer.next_run = long.MaxValue;
					timer.is_dead = true;
				}
				else
				{
					timer.next_run = Timer.GetTimeMonotonic() + 10000L * timer.period_ms;
					timer.is_dead = false;
				}
				ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(Timer.Scheduler.TimerCB), timer);
			}

			private int RunSchedulerLoop()
			{
				long timeMonotonic = Timer.GetTimeMonotonic();
				Timer.TimerComparer timerComparer = default(Timer.TimerComparer);
				if (this.needReSort)
				{
					this.list.Sort(new Comparison<Timer>(timerComparer.Compare));
					this.needReSort = false;
				}
				long num = long.MaxValue;
				for (int i = 0; i < this.list.Count; i++)
				{
					Timer timer = this.list[i];
					if (!timer.is_dead)
					{
						if (timer.next_run <= timeMonotonic)
						{
							this.FireTimer(timer);
						}
						num = Math.Min(num, timer.next_run);
						if (timer.next_run > timeMonotonic && timer.next_run < 9223372036854775807L)
						{
							timer.is_dead = false;
						}
					}
				}
				for (int i = 0; i < this.list.Count; i++)
				{
					Timer timer2 = this.list[i];
					if (timer2.is_dead)
					{
						timer2.is_added = false;
						this.needReSort = true;
						this.list[i] = this.list[this.list.Count - 1];
						i--;
						this.list.RemoveAt(this.list.Count - 1);
						if (this.list.Count == 0)
						{
							break;
						}
					}
				}
				if (this.needReSort)
				{
					this.list.Sort(new Comparison<Timer>(timerComparer.Compare));
					this.needReSort = false;
				}
				int num2 = -1;
				this.current_next_run = num;
				if (num != 9223372036854775807L)
				{
					long num3 = (num - Timer.GetTimeMonotonic()) / 10000L;
					if (num3 > 2147483647L)
					{
						num2 = 2147483646;
					}
					else
					{
						num2 = (int)num3;
						if (num2 < 0)
						{
							num2 = 0;
						}
					}
				}
				return num2;
			}

			private static readonly Timer.Scheduler instance = new Timer.Scheduler();

			private volatile bool needReSort = true;

			private List<Timer> list;

			private long current_next_run = long.MaxValue;

			private ManualResetEvent changed;
		}
	}
}
