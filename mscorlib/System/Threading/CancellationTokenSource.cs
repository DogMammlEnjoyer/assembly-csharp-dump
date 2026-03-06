using System;
using System.Collections.Generic;

namespace System.Threading
{
	/// <summary>Signals to a <see cref="T:System.Threading.CancellationToken" /> that it should be canceled.</summary>
	public class CancellationTokenSource : IDisposable
	{
		/// <summary>Gets whether cancellation has been requested for this <see cref="T:System.Threading.CancellationTokenSource" />.</summary>
		/// <returns>
		///   <see langword="true" /> if cancellation has been requested for this <see cref="T:System.Threading.CancellationTokenSource" />; otherwise, <see langword="false" />.</returns>
		public bool IsCancellationRequested
		{
			get
			{
				return this._state >= 2;
			}
		}

		internal bool IsCancellationCompleted
		{
			get
			{
				return this._state == 3;
			}
		}

		internal bool IsDisposed
		{
			get
			{
				return this._disposed;
			}
		}

		internal int ThreadIDExecutingCallbacks
		{
			get
			{
				return this._threadIDExecutingCallbacks;
			}
			set
			{
				this._threadIDExecutingCallbacks = value;
			}
		}

		/// <summary>Gets the <see cref="T:System.Threading.CancellationToken" /> associated with this <see cref="T:System.Threading.CancellationTokenSource" />.</summary>
		/// <returns>The <see cref="T:System.Threading.CancellationToken" /> associated with this <see cref="T:System.Threading.CancellationTokenSource" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The token source has been disposed.</exception>
		public CancellationToken Token
		{
			get
			{
				this.ThrowIfDisposed();
				return new CancellationToken(this);
			}
		}

		internal bool CanBeCanceled
		{
			get
			{
				return this._state != 0;
			}
		}

		internal WaitHandle WaitHandle
		{
			get
			{
				this.ThrowIfDisposed();
				if (this._kernelEvent != null)
				{
					return this._kernelEvent;
				}
				ManualResetEvent manualResetEvent = new ManualResetEvent(false);
				if (Interlocked.CompareExchange<ManualResetEvent>(ref this._kernelEvent, manualResetEvent, null) != null)
				{
					manualResetEvent.Dispose();
				}
				if (this.IsCancellationRequested)
				{
					this._kernelEvent.Set();
				}
				return this._kernelEvent;
			}
		}

		internal CancellationCallbackInfo ExecutingCallback
		{
			get
			{
				return this._executingCallback;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.CancellationTokenSource" /> class.</summary>
		public CancellationTokenSource()
		{
			this._state = 1;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.CancellationTokenSource" /> class that will be canceled after the specified time span.</summary>
		/// <param name="delay">The time interval to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="delay" />.<see cref="P:System.TimeSpan.TotalMilliseconds" /> is less than -1 or greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		public CancellationTokenSource(TimeSpan delay)
		{
			long num = (long)delay.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("delay");
			}
			this.InitializeWithTimer((int)num);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.CancellationTokenSource" /> class that will be canceled after the specified delay in milliseconds.</summary>
		/// <param name="millisecondsDelay">The time interval in milliseconds to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsDelay" /> is less than -1.</exception>
		public CancellationTokenSource(int millisecondsDelay)
		{
			if (millisecondsDelay < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsDelay");
			}
			this.InitializeWithTimer(millisecondsDelay);
		}

		private void InitializeWithTimer(int millisecondsDelay)
		{
			this._state = 1;
			this._timer = new Timer(CancellationTokenSource.s_timerCallback, this, millisecondsDelay, -1);
		}

		/// <summary>Communicates a request for cancellation.</summary>
		/// <exception cref="T:System.ObjectDisposedException">This <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.AggregateException">An aggregate exception containing all the exceptions thrown by the registered callbacks on the associated <see cref="T:System.Threading.CancellationToken" />.</exception>
		public void Cancel()
		{
			this.Cancel(false);
		}

		/// <summary>Communicates a request for cancellation, and specifies whether remaining callbacks and cancelable operations should be processed if an exception occurs.</summary>
		/// <param name="throwOnFirstException">
		///   <see langword="true" /> if exceptions should immediately propagate; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ObjectDisposedException">This <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.AggregateException">An aggregate exception containing all the exceptions thrown by the registered callbacks on the associated <see cref="T:System.Threading.CancellationToken" />.</exception>
		public void Cancel(bool throwOnFirstException)
		{
			this.ThrowIfDisposed();
			this.NotifyCancellation(throwOnFirstException);
		}

		/// <summary>Schedules a cancel operation on this <see cref="T:System.Threading.CancellationTokenSource" /> after the specified time span.</summary>
		/// <param name="delay">The time span to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource" />.</param>
		/// <exception cref="T:System.ObjectDisposedException">The exception thrown when this <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The exception that is thrown when <paramref name="delay" /> is less than -1 or greater than Int32.MaxValue.</exception>
		public void CancelAfter(TimeSpan delay)
		{
			long num = (long)delay.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("delay");
			}
			this.CancelAfter((int)num);
		}

		/// <summary>Schedules a cancel operation on this <see cref="T:System.Threading.CancellationTokenSource" /> after the specified number of milliseconds.</summary>
		/// <param name="millisecondsDelay">The time span to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource" />.</param>
		/// <exception cref="T:System.ObjectDisposedException">The exception thrown when this <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The exception thrown when <paramref name="millisecondsDelay" /> is less than -1.</exception>
		public void CancelAfter(int millisecondsDelay)
		{
			this.ThrowIfDisposed();
			if (millisecondsDelay < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsDelay");
			}
			if (this.IsCancellationRequested)
			{
				return;
			}
			if (this._timer == null)
			{
				Timer timer = new Timer(CancellationTokenSource.s_timerCallback, this, -1, -1);
				if (Interlocked.CompareExchange<Timer>(ref this._timer, timer, null) != null)
				{
					timer.Dispose();
				}
			}
			try
			{
				this._timer.Change(millisecondsDelay, -1);
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private static void TimerCallbackLogic(object obj)
		{
			CancellationTokenSource cancellationTokenSource = (CancellationTokenSource)obj;
			if (!cancellationTokenSource.IsDisposed)
			{
				try
				{
					cancellationTokenSource.Cancel();
				}
				catch (ObjectDisposedException)
				{
					if (!cancellationTokenSource.IsDisposed)
					{
						throw;
					}
				}
			}
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Threading.CancellationTokenSource" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Threading.CancellationTokenSource" /> class and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this._disposed)
			{
				Timer timer = this._timer;
				if (timer != null)
				{
					timer.Dispose();
				}
				this._registeredCallbacksLists = null;
				if (this._kernelEvent != null)
				{
					ManualResetEvent manualResetEvent = Interlocked.Exchange<ManualResetEvent>(ref this._kernelEvent, null);
					if (manualResetEvent != null && this._state != 2)
					{
						manualResetEvent.Dispose();
					}
				}
				this._disposed = true;
			}
		}

		internal void ThrowIfDisposed()
		{
			if (this._disposed)
			{
				CancellationTokenSource.ThrowObjectDisposedException();
			}
		}

		private static void ThrowObjectDisposedException()
		{
			throw new ObjectDisposedException(null, "The CancellationTokenSource has been disposed.");
		}

		internal CancellationTokenRegistration InternalRegister(Action<object> callback, object stateForCallback, SynchronizationContext targetSyncContext, ExecutionContext executionContext)
		{
			if (!this.IsCancellationRequested)
			{
				if (this._disposed)
				{
					return default(CancellationTokenRegistration);
				}
				int num = Environment.CurrentManagedThreadId % CancellationTokenSource.s_nLists;
				CancellationCallbackInfo cancellationCallbackInfo = (targetSyncContext != null) ? new CancellationCallbackInfo.WithSyncContext(callback, stateForCallback, executionContext, this, targetSyncContext) : new CancellationCallbackInfo(callback, stateForCallback, executionContext, this);
				SparselyPopulatedArray<CancellationCallbackInfo>[] array = this._registeredCallbacksLists;
				if (array == null)
				{
					SparselyPopulatedArray<CancellationCallbackInfo>[] array2 = new SparselyPopulatedArray<CancellationCallbackInfo>[CancellationTokenSource.s_nLists];
					array = Interlocked.CompareExchange<SparselyPopulatedArray<CancellationCallbackInfo>[]>(ref this._registeredCallbacksLists, array2, null);
					if (array == null)
					{
						array = array2;
					}
				}
				SparselyPopulatedArray<CancellationCallbackInfo> sparselyPopulatedArray = Volatile.Read<SparselyPopulatedArray<CancellationCallbackInfo>>(ref array[num]);
				if (sparselyPopulatedArray == null)
				{
					SparselyPopulatedArray<CancellationCallbackInfo> value = new SparselyPopulatedArray<CancellationCallbackInfo>(4);
					Interlocked.CompareExchange<SparselyPopulatedArray<CancellationCallbackInfo>>(ref array[num], value, null);
					sparselyPopulatedArray = array[num];
				}
				SparselyPopulatedArrayAddInfo<CancellationCallbackInfo> registrationInfo = sparselyPopulatedArray.Add(cancellationCallbackInfo);
				CancellationTokenRegistration result = new CancellationTokenRegistration(cancellationCallbackInfo, registrationInfo);
				if (!this.IsCancellationRequested)
				{
					return result;
				}
				if (!result.Unregister())
				{
					return result;
				}
			}
			callback(stateForCallback);
			return default(CancellationTokenRegistration);
		}

		private void NotifyCancellation(bool throwOnFirstException)
		{
			if (!this.IsCancellationRequested && Interlocked.CompareExchange(ref this._state, 2, 1) == 1)
			{
				Timer timer = this._timer;
				if (timer != null)
				{
					timer.Dispose();
				}
				this.ThreadIDExecutingCallbacks = Environment.CurrentManagedThreadId;
				ManualResetEvent kernelEvent = this._kernelEvent;
				if (kernelEvent != null)
				{
					kernelEvent.Set();
				}
				this.ExecuteCallbackHandlers(throwOnFirstException);
			}
		}

		private void ExecuteCallbackHandlers(bool throwOnFirstException)
		{
			LowLevelListWithIList<Exception> lowLevelListWithIList = null;
			SparselyPopulatedArray<CancellationCallbackInfo>[] registeredCallbacksLists = this._registeredCallbacksLists;
			if (registeredCallbacksLists == null)
			{
				Interlocked.Exchange(ref this._state, 3);
				return;
			}
			try
			{
				for (int i = 0; i < registeredCallbacksLists.Length; i++)
				{
					SparselyPopulatedArray<CancellationCallbackInfo> sparselyPopulatedArray = Volatile.Read<SparselyPopulatedArray<CancellationCallbackInfo>>(ref registeredCallbacksLists[i]);
					if (sparselyPopulatedArray != null)
					{
						for (SparselyPopulatedArrayFragment<CancellationCallbackInfo> sparselyPopulatedArrayFragment = sparselyPopulatedArray.Tail; sparselyPopulatedArrayFragment != null; sparselyPopulatedArrayFragment = sparselyPopulatedArrayFragment.Prev)
						{
							for (int j = sparselyPopulatedArrayFragment.Length - 1; j >= 0; j--)
							{
								this._executingCallback = sparselyPopulatedArrayFragment[j];
								if (this._executingCallback != null)
								{
									CancellationCallbackCoreWorkArguments cancellationCallbackCoreWorkArguments = new CancellationCallbackCoreWorkArguments(sparselyPopulatedArrayFragment, j);
									try
									{
										CancellationCallbackInfo.WithSyncContext withSyncContext = this._executingCallback as CancellationCallbackInfo.WithSyncContext;
										if (withSyncContext != null)
										{
											withSyncContext.TargetSyncContext.Send(new SendOrPostCallback(this.CancellationCallbackCoreWork_OnSyncContext), cancellationCallbackCoreWorkArguments);
											this.ThreadIDExecutingCallbacks = Environment.CurrentManagedThreadId;
										}
										else
										{
											this.CancellationCallbackCoreWork(cancellationCallbackCoreWorkArguments);
										}
									}
									catch (Exception item)
									{
										if (throwOnFirstException)
										{
											throw;
										}
										if (lowLevelListWithIList == null)
										{
											lowLevelListWithIList = new LowLevelListWithIList<Exception>();
										}
										lowLevelListWithIList.Add(item);
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				this._state = 3;
				this._executingCallback = null;
				Interlocked.MemoryBarrier();
			}
			if (lowLevelListWithIList != null)
			{
				throw new AggregateException(lowLevelListWithIList);
			}
		}

		private void CancellationCallbackCoreWork_OnSyncContext(object obj)
		{
			this.CancellationCallbackCoreWork((CancellationCallbackCoreWorkArguments)obj);
		}

		private void CancellationCallbackCoreWork(CancellationCallbackCoreWorkArguments args)
		{
			CancellationCallbackInfo cancellationCallbackInfo = args._currArrayFragment.SafeAtomicRemove(args._currArrayIndex, this._executingCallback);
			if (cancellationCallbackInfo == this._executingCallback)
			{
				cancellationCallbackInfo.CancellationTokenSource.ThreadIDExecutingCallbacks = Environment.CurrentManagedThreadId;
				cancellationCallbackInfo.ExecuteCallback();
			}
		}

		/// <summary>Creates a <see cref="T:System.Threading.CancellationTokenSource" /> that will be in the canceled state when any of the source tokens are in the canceled state.</summary>
		/// <param name="token1">The first cancellation token to observe.</param>
		/// <param name="token2">The second cancellation token to observe.</param>
		/// <returns>A <see cref="T:System.Threading.CancellationTokenSource" /> that is linked to the source tokens.</returns>
		/// <exception cref="T:System.ObjectDisposedException">A <see cref="T:System.Threading.CancellationTokenSource" /> associated with one of the source tokens has been disposed.</exception>
		public static CancellationTokenSource CreateLinkedTokenSource(CancellationToken token1, CancellationToken token2)
		{
			if (!token1.CanBeCanceled)
			{
				return CancellationTokenSource.CreateLinkedTokenSource(token2);
			}
			if (!token2.CanBeCanceled)
			{
				return new CancellationTokenSource.Linked1CancellationTokenSource(token1);
			}
			return new CancellationTokenSource.Linked2CancellationTokenSource(token1, token2);
		}

		internal static CancellationTokenSource CreateLinkedTokenSource(CancellationToken token)
		{
			if (!token.CanBeCanceled)
			{
				return new CancellationTokenSource();
			}
			return new CancellationTokenSource.Linked1CancellationTokenSource(token);
		}

		/// <summary>Creates a <see cref="T:System.Threading.CancellationTokenSource" /> that will be in the canceled state when any of the source tokens in the specified array are in the canceled state.</summary>
		/// <param name="tokens">An array that contains the cancellation token instances to observe.</param>
		/// <returns>A <see cref="T:System.Threading.CancellationTokenSource" /> that is linked to the source tokens.</returns>
		/// <exception cref="T:System.ObjectDisposedException">A <see cref="T:System.Threading.CancellationTokenSource" /> associated with one of the source tokens has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="tokens" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="tokens" /> is empty.</exception>
		public static CancellationTokenSource CreateLinkedTokenSource(params CancellationToken[] tokens)
		{
			if (tokens == null)
			{
				throw new ArgumentNullException("tokens");
			}
			switch (tokens.Length)
			{
			case 0:
				throw new ArgumentException("No tokens were supplied.");
			case 1:
				return CancellationTokenSource.CreateLinkedTokenSource(tokens[0]);
			case 2:
				return CancellationTokenSource.CreateLinkedTokenSource(tokens[0], tokens[1]);
			default:
				return new CancellationTokenSource.LinkedNCancellationTokenSource(tokens);
			}
		}

		internal void WaitForCallbackToComplete(CancellationCallbackInfo callbackInfo)
		{
			SpinWait spinWait = default(SpinWait);
			while (this.ExecutingCallback == callbackInfo)
			{
				spinWait.SpinOnce();
			}
		}

		internal static readonly CancellationTokenSource s_canceledSource = new CancellationTokenSource
		{
			_state = 3
		};

		internal static readonly CancellationTokenSource s_neverCanceledSource = new CancellationTokenSource
		{
			_state = 0
		};

		private static readonly int s_nLists = (PlatformHelper.ProcessorCount > 24) ? 24 : PlatformHelper.ProcessorCount;

		private volatile ManualResetEvent _kernelEvent;

		private volatile SparselyPopulatedArray<CancellationCallbackInfo>[] _registeredCallbacksLists;

		private const int CannotBeCanceled = 0;

		private const int NotCanceledState = 1;

		private const int NotifyingState = 2;

		private const int NotifyingCompleteState = 3;

		private volatile int _state;

		private volatile int _threadIDExecutingCallbacks = -1;

		private bool _disposed;

		private volatile CancellationCallbackInfo _executingCallback;

		private volatile Timer _timer;

		private static readonly TimerCallback s_timerCallback = new TimerCallback(CancellationTokenSource.TimerCallbackLogic);

		private sealed class Linked1CancellationTokenSource : CancellationTokenSource
		{
			internal Linked1CancellationTokenSource(CancellationToken token1)
			{
				this._reg1 = token1.InternalRegisterWithoutEC(CancellationTokenSource.LinkedNCancellationTokenSource.s_linkedTokenCancelDelegate, this);
			}

			protected override void Dispose(bool disposing)
			{
				if (!disposing || this._disposed)
				{
					return;
				}
				this._reg1.Dispose();
				base.Dispose(disposing);
			}

			private readonly CancellationTokenRegistration _reg1;
		}

		private sealed class Linked2CancellationTokenSource : CancellationTokenSource
		{
			internal Linked2CancellationTokenSource(CancellationToken token1, CancellationToken token2)
			{
				this._reg1 = token1.InternalRegisterWithoutEC(CancellationTokenSource.LinkedNCancellationTokenSource.s_linkedTokenCancelDelegate, this);
				this._reg2 = token2.InternalRegisterWithoutEC(CancellationTokenSource.LinkedNCancellationTokenSource.s_linkedTokenCancelDelegate, this);
			}

			protected override void Dispose(bool disposing)
			{
				if (!disposing || this._disposed)
				{
					return;
				}
				this._reg1.Dispose();
				this._reg2.Dispose();
				base.Dispose(disposing);
			}

			private readonly CancellationTokenRegistration _reg1;

			private readonly CancellationTokenRegistration _reg2;
		}

		private sealed class LinkedNCancellationTokenSource : CancellationTokenSource
		{
			internal LinkedNCancellationTokenSource(params CancellationToken[] tokens)
			{
				this._linkingRegistrations = new CancellationTokenRegistration[tokens.Length];
				for (int i = 0; i < tokens.Length; i++)
				{
					if (tokens[i].CanBeCanceled)
					{
						this._linkingRegistrations[i] = tokens[i].InternalRegisterWithoutEC(CancellationTokenSource.LinkedNCancellationTokenSource.s_linkedTokenCancelDelegate, this);
					}
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (!disposing || this._disposed)
				{
					return;
				}
				CancellationTokenRegistration[] linkingRegistrations = this._linkingRegistrations;
				if (linkingRegistrations != null)
				{
					this._linkingRegistrations = null;
					for (int i = 0; i < linkingRegistrations.Length; i++)
					{
						linkingRegistrations[i].Dispose();
					}
				}
				base.Dispose(disposing);
			}

			internal static readonly Action<object> s_linkedTokenCancelDelegate = delegate(object s)
			{
				((CancellationTokenSource)s).NotifyCancellation(false);
			};

			private CancellationTokenRegistration[] _linkingRegistrations;
		}
	}
}
