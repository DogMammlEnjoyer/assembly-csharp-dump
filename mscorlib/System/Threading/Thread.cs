using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Principal;
using Internal.Runtime.Augments;

namespace System.Threading
{
	/// <summary>Creates and controls a thread, sets its priority, and gets its status.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Thread : CriticalFinalizerObject, _Thread
	{
		private static void AsyncLocalSetCurrentCulture(AsyncLocalValueChangedArgs<CultureInfo> args)
		{
			Thread.m_CurrentCulture = args.CurrentValue;
		}

		private static void AsyncLocalSetCurrentUICulture(AsyncLocalValueChangedArgs<CultureInfo> args)
		{
			Thread.m_CurrentUICulture = args.CurrentValue;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Thread" /> class.</summary>
		/// <param name="start">A <see cref="T:System.Threading.ThreadStart" /> delegate that represents the methods to be invoked when this thread begins executing.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="start" /> parameter is <see langword="null" />.</exception>
		[SecuritySafeCritical]
		public Thread(ThreadStart start)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			this.SetStartHelper(start, 0);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Thread" /> class, specifying the maximum stack size for the thread.</summary>
		/// <param name="start">A <see cref="T:System.Threading.ThreadStart" /> delegate that represents the methods to be invoked when this thread begins executing.</param>
		/// <param name="maxStackSize">The maximum stack size, in bytes, to be used by the thread, or 0 to use the default maximum stack size specified in the header for the executable.  
		///  Important   For partially trusted code, <paramref name="maxStackSize" /> is ignored if it is greater than the default stack size. No exception is thrown.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="start" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="maxStackSize" /> is less than zero.</exception>
		[SecuritySafeCritical]
		public Thread(ThreadStart start, int maxStackSize)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			if (0 > maxStackSize)
			{
				throw new ArgumentOutOfRangeException("maxStackSize", Environment.GetResourceString("Non-negative number required."));
			}
			this.SetStartHelper(start, maxStackSize);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Thread" /> class, specifying a delegate that allows an object to be passed to the thread when the thread is started.</summary>
		/// <param name="start">A delegate that represents the methods to be invoked when this thread begins executing.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="start" /> is <see langword="null" />.</exception>
		[SecuritySafeCritical]
		public Thread(ParameterizedThreadStart start)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			this.SetStartHelper(start, 0);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Thread" /> class, specifying a delegate that allows an object to be passed to the thread when the thread is started and specifying the maximum stack size for the thread.</summary>
		/// <param name="start">A <see cref="T:System.Threading.ParameterizedThreadStart" /> delegate that represents the methods to be invoked when this thread begins executing.</param>
		/// <param name="maxStackSize">The maximum stack size, in bytes, to be used by the thread, or 0 to use the default maximum stack size specified in the header for the executable.  
		///  Important   For partially trusted code, <paramref name="maxStackSize" /> is ignored if it is greater than the default stack size. No exception is thrown.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="start" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="maxStackSize" /> is less than zero.</exception>
		[SecuritySafeCritical]
		public Thread(ParameterizedThreadStart start, int maxStackSize)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			if (0 > maxStackSize)
			{
				throw new ArgumentOutOfRangeException("maxStackSize", Environment.GetResourceString("Non-negative number required."));
			}
			this.SetStartHelper(start, maxStackSize);
		}

		/// <summary>Causes the operating system to change the state of the current instance to <see cref="F:System.Threading.ThreadState.Running" />.</summary>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread has already been started.</exception>
		/// <exception cref="T:System.OutOfMemoryException">There is not enough memory available to start this thread.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Start()
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			this.Start(ref stackCrawlMark);
		}

		/// <summary>Causes the operating system to change the state of the current instance to <see cref="F:System.Threading.ThreadState.Running" />, and optionally supplies an object containing data to be used by the method the thread executes.</summary>
		/// <param name="parameter">An object that contains data to be used by the method the thread executes.</param>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread has already been started.</exception>
		/// <exception cref="T:System.OutOfMemoryException">There is not enough memory available to start this thread.</exception>
		/// <exception cref="T:System.InvalidOperationException">This thread was created using a <see cref="T:System.Threading.ThreadStart" /> delegate instead of a <see cref="T:System.Threading.ParameterizedThreadStart" /> delegate.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Start(object parameter)
		{
			if (this.m_Delegate is ThreadStart)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The thread was created with a ThreadStart delegate that does not accept a parameter."));
			}
			this.m_ThreadStartArg = parameter;
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			this.Start(ref stackCrawlMark);
		}

		[SecuritySafeCritical]
		private void Start(ref StackCrawlMark stackMark)
		{
			if (this.m_Delegate != null)
			{
				ThreadHelper threadHelper = (ThreadHelper)this.m_Delegate.Target;
				ExecutionContext executionContextHelper = ExecutionContext.Capture(ref stackMark, ExecutionContext.CaptureOptions.IgnoreSyncCtx);
				threadHelper.SetExecutionContextHelper(executionContextHelper);
			}
			object obj = null;
			this.StartInternal(obj, ref stackMark);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal ExecutionContext.Reader GetExecutionContextReader()
		{
			return new ExecutionContext.Reader(this.m_ExecutionContext);
		}

		internal bool ExecutionContextBelongsToCurrentScope
		{
			get
			{
				return !this.m_ExecutionContextBelongsToOuterScope;
			}
			set
			{
				this.m_ExecutionContextBelongsToOuterScope = !value;
			}
		}

		/// <summary>Gets an <see cref="T:System.Threading.ExecutionContext" /> object that contains information about the various contexts of the current thread.</summary>
		/// <returns>An <see cref="T:System.Threading.ExecutionContext" /> object that consolidates context information for the current thread.</returns>
		public ExecutionContext ExecutionContext
		{
			[SecuritySafeCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				ExecutionContext result;
				if (this == Thread.CurrentThread)
				{
					result = this.GetMutableExecutionContext();
				}
				else
				{
					result = this.m_ExecutionContext;
				}
				return result;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[SecurityCritical]
		internal ExecutionContext GetMutableExecutionContext()
		{
			if (this.m_ExecutionContext == null)
			{
				this.m_ExecutionContext = new ExecutionContext();
			}
			else if (!this.ExecutionContextBelongsToCurrentScope)
			{
				ExecutionContext executionContext = this.m_ExecutionContext.CreateMutableCopy();
				this.m_ExecutionContext = executionContext;
			}
			this.ExecutionContextBelongsToCurrentScope = true;
			return this.m_ExecutionContext;
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal void SetExecutionContext(ExecutionContext value, bool belongsToCurrentScope)
		{
			this.m_ExecutionContext = value;
			this.ExecutionContextBelongsToCurrentScope = belongsToCurrentScope;
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal void SetExecutionContext(ExecutionContext.Reader value, bool belongsToCurrentScope)
		{
			this.m_ExecutionContext = value.DangerousGetRawExecutionContext();
			this.ExecutionContextBelongsToCurrentScope = belongsToCurrentScope;
		}

		/// <summary>Applies a captured <see cref="T:System.Threading.CompressedStack" /> to the current thread.</summary>
		/// <param name="stack">The <see cref="T:System.Threading.CompressedStack" /> object to be applied to the current thread.</param>
		/// <exception cref="T:System.InvalidOperationException">In all cases.</exception>
		[Obsolete("Thread.SetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class")]
		public void SetCompressedStack(CompressedStack stack)
		{
			throw new InvalidOperationException(Environment.GetResourceString("Use CompressedStack.(Capture/Run) or ExecutionContext.(Capture/Run) APIs instead."));
		}

		/// <summary>Returns a <see cref="T:System.Threading.CompressedStack" /> object that can be used to capture the stack for the current thread.</summary>
		/// <returns>None.</returns>
		/// <exception cref="T:System.InvalidOperationException">In all cases.</exception>
		[SecurityCritical]
		[Obsolete("Thread.GetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class")]
		public CompressedStack GetCompressedStack()
		{
			throw new InvalidOperationException(Environment.GetResourceString("Use CompressedStack.(Capture/Run) or ExecutionContext.(Capture/Run) APIs instead."));
		}

		/// <summary>Cancels an <see cref="M:System.Threading.Thread.Abort(System.Object)" /> requested for the current thread.</summary>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Core only: This member is not supported.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">
		///   <see langword="Abort" /> was not invoked on the current thread.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required security permission for the current thread.</exception>
		public static void ResetAbort()
		{
			Thread currentThread = Thread.CurrentThread;
			if ((currentThread.ThreadState & ThreadState.AbortRequested) == ThreadState.Running)
			{
				throw new ThreadStateException(Environment.GetResourceString("Unable to reset abort because no abort was requested."));
			}
			currentThread.ResetAbortNative();
			currentThread.ClearAbortReason();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResetAbortNative();

		/// <summary>Either suspends the thread, or if the thread is already suspended, has no effect.</summary>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Core only: This member is not supported.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread has not been started or is dead.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the appropriate <see cref="T:System.Security.Permissions.SecurityPermission" />.</exception>
		[Obsolete("Thread.Suspend has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202", false)]
		[SecuritySafeCritical]
		public void Suspend()
		{
			this.SuspendInternal();
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SuspendInternal();

		/// <summary>Resumes a thread that has been suspended.</summary>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Core only: This member is not supported.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread has not been started, is dead, or is not in the suspended state.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the appropriate <see cref="T:System.Security.Permissions.SecurityPermission" />.</exception>
		[SecuritySafeCritical]
		[Obsolete("Thread.Resume has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202", false)]
		public void Resume()
		{
			this.ResumeInternal();
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResumeInternal();

		/// <summary>Interrupts a thread that is in the <see langword="WaitSleepJoin" /> thread state.</summary>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the appropriate <see cref="T:System.Security.Permissions.SecurityPermission" />.</exception>
		public void Interrupt()
		{
			this.InterruptInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InterruptInternal();

		/// <summary>Gets or sets a value indicating the scheduling priority of a thread.</summary>
		/// <returns>One of the <see cref="T:System.Threading.ThreadPriority" /> values. The default value is <see cref="F:System.Threading.ThreadPriority.Normal" />.</returns>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread has reached a final state, such as <see cref="F:System.Threading.ThreadState.Aborted" />.</exception>
		/// <exception cref="T:System.ArgumentException">The value specified for a set operation is not a valid <see cref="T:System.Threading.ThreadPriority" /> value.</exception>
		public ThreadPriority Priority
		{
			[SecuritySafeCritical]
			get
			{
				return (ThreadPriority)this.GetPriorityNative();
			}
			set
			{
				this.SetPriorityNative((int)value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetPriorityNative();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPriorityNative(int priority);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool JoinInternal(int millisecondsTimeout);

		/// <summary>Blocks the calling thread until the thread represented by this instance terminates, while continuing to perform standard COM and <see langword="SendMessage" /> pumping.</summary>
		/// <exception cref="T:System.Threading.ThreadStateException">The caller attempted to join a thread that is in the <see cref="F:System.Threading.ThreadState.Unstarted" /> state.</exception>
		/// <exception cref="T:System.Threading.ThreadInterruptedException">The thread is interrupted while waiting.</exception>
		public void Join()
		{
			this.JoinInternal(-1);
		}

		/// <summary>Blocks the calling thread until the thread represented by this instance terminates or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.</summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait for the thread to terminate.</param>
		/// <returns>
		///   <see langword="true" /> if the thread has terminated; <see langword="false" /> if the thread has not terminated after the amount of time specified by the <paramref name="millisecondsTimeout" /> parameter has elapsed.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value of <paramref name="millisecondsTimeout" /> is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite" /> in milliseconds.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread has not been started.</exception>
		public bool Join(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return this.JoinInternal(millisecondsTimeout);
		}

		/// <summary>Blocks the calling thread until the thread represented by this instance terminates or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.</summary>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> set to the amount of time to wait for the thread to terminate.</param>
		/// <returns>
		///   <see langword="true" /> if the thread terminated; <see langword="false" /> if the thread has not terminated after the amount of time specified by the <paramref name="timeout" /> parameter has elapsed.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value of <paramref name="timeout" /> is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite" /> in milliseconds, or is greater than <see cref="F:System.Int32.MaxValue" /> milliseconds.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">The caller attempted to join a thread that is in the <see cref="F:System.Threading.ThreadState.Unstarted" /> state.</exception>
		public bool Join(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return this.Join((int)num);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SleepInternal(int millisecondsTimeout);

		/// <summary>Suspends the current thread for the specified number of milliseconds.</summary>
		/// <param name="millisecondsTimeout">The number of milliseconds for which the thread is suspended. If the value of the <paramref name="millisecondsTimeout" /> argument is zero, the thread relinquishes the remainder of its time slice to any thread of equal priority that is ready to run. If there are no other threads of equal priority that are ready to run, execution of the current thread is not suspended.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The time-out value is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite" />.</exception>
		[SecuritySafeCritical]
		public static void Sleep(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			Thread.SleepInternal(millisecondsTimeout);
		}

		/// <summary>Suspends the current thread for the specified amount of time.</summary>
		/// <param name="timeout">The amount of time for which the thread is suspended. If the value of the <paramref name="millisecondsTimeout" /> argument is <see cref="F:System.TimeSpan.Zero" />, the thread relinquishes the remainder of its time slice to any thread of equal priority that is ready to run. If there are no other threads of equal priority that are ready to run, execution of the current thread is not suspended.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value of <paramref name="timeout" /> is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite" /> in milliseconds, or is greater than <see cref="F:System.Int32.MaxValue" /> milliseconds.</exception>
		public static void Sleep(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			Thread.Sleep((int)num);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool YieldInternal();

		/// <summary>Causes the calling thread to yield execution to another thread that is ready to run on the current processor. The operating system selects the thread to yield to.</summary>
		/// <returns>
		///   <see langword="true" /> if the operating system switched execution to another thread; otherwise, <see langword="false" />.</returns>
		public static bool Yield()
		{
			return Thread.YieldInternal();
		}

		[SecurityCritical]
		private void SetStartHelper(Delegate start, int maxStackSize)
		{
			maxStackSize = Thread.GetProcessDefaultStackSize(maxStackSize);
			ThreadHelper @object = new ThreadHelper(start);
			if (start is ThreadStart)
			{
				this.SetStart(new ThreadStart(@object.ThreadStart), maxStackSize);
				return;
			}
			this.SetStart(new ParameterizedThreadStart(@object.ThreadStart), maxStackSize);
		}

		/// <summary>Allocates an unnamed data slot on all the threads. For better performance, use fields that are marked with the <see cref="T:System.ThreadStaticAttribute" /> attribute instead.</summary>
		/// <returns>The allocated named data slot on all threads.</returns>
		public static LocalDataStoreSlot AllocateDataSlot()
		{
			return Thread.LocalDataStoreManager.AllocateDataSlot();
		}

		/// <summary>Allocates a named data slot on all threads. For better performance, use fields that are marked with the <see cref="T:System.ThreadStaticAttribute" /> attribute instead.</summary>
		/// <param name="name">The name of the data slot to be allocated.</param>
		/// <returns>The allocated named data slot on all threads.</returns>
		/// <exception cref="T:System.ArgumentException">A named data slot with the specified name already exists.</exception>
		public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
		{
			return Thread.LocalDataStoreManager.AllocateNamedDataSlot(name);
		}

		/// <summary>Looks up a named data slot. For better performance, use fields that are marked with the <see cref="T:System.ThreadStaticAttribute" /> attribute instead.</summary>
		/// <param name="name">The name of the local data slot.</param>
		/// <returns>A <see cref="T:System.LocalDataStoreSlot" /> allocated for this thread.</returns>
		public static LocalDataStoreSlot GetNamedDataSlot(string name)
		{
			return Thread.LocalDataStoreManager.GetNamedDataSlot(name);
		}

		/// <summary>Eliminates the association between a name and a slot, for all threads in the process. For better performance, use fields that are marked with the <see cref="T:System.ThreadStaticAttribute" /> attribute instead.</summary>
		/// <param name="name">The name of the data slot to be freed.</param>
		public static void FreeNamedDataSlot(string name)
		{
			Thread.LocalDataStoreManager.FreeNamedDataSlot(name);
		}

		/// <summary>Retrieves the value from the specified slot on the current thread, within the current thread's current domain. For better performance, use fields that are marked with the <see cref="T:System.ThreadStaticAttribute" /> attribute instead.</summary>
		/// <param name="slot">The <see cref="T:System.LocalDataStoreSlot" /> from which to get the value.</param>
		/// <returns>The retrieved value.</returns>
		public static object GetData(LocalDataStoreSlot slot)
		{
			LocalDataStoreHolder localDataStoreHolder = Thread.s_LocalDataStore;
			if (localDataStoreHolder == null)
			{
				Thread.LocalDataStoreManager.ValidateSlot(slot);
				return null;
			}
			return localDataStoreHolder.Store.GetData(slot);
		}

		/// <summary>Sets the data in the specified slot on the currently running thread, for that thread's current domain. For better performance, use fields marked with the <see cref="T:System.ThreadStaticAttribute" /> attribute instead.</summary>
		/// <param name="slot">The <see cref="T:System.LocalDataStoreSlot" /> in which to set the value.</param>
		/// <param name="data">The value to be set.</param>
		public static void SetData(LocalDataStoreSlot slot, object data)
		{
			LocalDataStoreHolder localDataStoreHolder = Thread.s_LocalDataStore;
			if (localDataStoreHolder == null)
			{
				localDataStoreHolder = Thread.LocalDataStoreManager.CreateLocalDataStore();
				Thread.s_LocalDataStore = localDataStoreHolder;
			}
			localDataStoreHolder.Store.SetData(slot, data);
		}

		/// <summary>Gets or sets the current culture used by the Resource Manager to look up culture-specific resources at run time.</summary>
		/// <returns>An object that represents the current culture.</returns>
		/// <exception cref="T:System.ArgumentNullException">The property is set to <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The property is set to a culture name that cannot be used to locate a resource file. Resource filenames must include only letters, numbers, hyphens or underscores.</exception>
		/// <exception cref="T:System.InvalidOperationException">.NET Core only: Reading or writing the culture of a thread from another thread is not supported.</exception>
		public CultureInfo CurrentUICulture
		{
			get
			{
				if (AppDomain.IsAppXModel())
				{
					return CultureInfo.GetCultureInfoForUserPreferredLanguageInAppX() ?? this.GetCurrentUICultureNoAppX();
				}
				return this.GetCurrentUICultureNoAppX();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				CultureInfo.VerifyCultureName(value, true);
				if (AppDomain.IsAppXModel())
				{
					CultureInfo.SetCultureInfoForUserPreferredLanguageInAppX(value);
					return;
				}
				if (Thread.m_CurrentUICulture == null && Thread.m_CurrentCulture == null)
				{
					Thread.nativeInitCultureAccessors();
				}
				if (!AppContextSwitches.NoAsyncCurrentCulture)
				{
					if (Thread.s_asyncLocalCurrentUICulture == null)
					{
						Interlocked.CompareExchange<AsyncLocal<CultureInfo>>(ref Thread.s_asyncLocalCurrentUICulture, new AsyncLocal<CultureInfo>(new Action<AsyncLocalValueChangedArgs<CultureInfo>>(Thread.AsyncLocalSetCurrentUICulture)), null);
					}
					Thread.s_asyncLocalCurrentUICulture.Value = value;
					return;
				}
				Thread.m_CurrentUICulture = value;
			}
		}

		internal CultureInfo GetCurrentUICultureNoAppX()
		{
			if (Thread.m_CurrentUICulture != null)
			{
				return Thread.m_CurrentUICulture;
			}
			CultureInfo defaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;
			if (defaultThreadCurrentUICulture == null)
			{
				return CultureInfo.UserDefaultUICulture;
			}
			return defaultThreadCurrentUICulture;
		}

		/// <summary>Gets or sets the culture for the current thread.</summary>
		/// <returns>An object that represents the culture for the current thread.</returns>
		/// <exception cref="T:System.ArgumentNullException">The property is set to <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">.NET Core only: Reading or writing the culture of a thread from another thread is not supported.</exception>
		public CultureInfo CurrentCulture
		{
			get
			{
				if (AppDomain.IsAppXModel())
				{
					return CultureInfo.GetCultureInfoForUserPreferredLanguageInAppX() ?? this.GetCurrentCultureNoAppX();
				}
				return this.GetCurrentCultureNoAppX();
			}
			[SecuritySafeCritical]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (AppDomain.IsAppXModel())
				{
					CultureInfo.SetCultureInfoForUserPreferredLanguageInAppX(value);
					return;
				}
				if (Thread.m_CurrentCulture == null && Thread.m_CurrentUICulture == null)
				{
					Thread.nativeInitCultureAccessors();
				}
				if (!AppContextSwitches.NoAsyncCurrentCulture)
				{
					if (Thread.s_asyncLocalCurrentCulture == null)
					{
						Interlocked.CompareExchange<AsyncLocal<CultureInfo>>(ref Thread.s_asyncLocalCurrentCulture, new AsyncLocal<CultureInfo>(new Action<AsyncLocalValueChangedArgs<CultureInfo>>(Thread.AsyncLocalSetCurrentCulture)), null);
					}
					Thread.s_asyncLocalCurrentCulture.Value = value;
					return;
				}
				Thread.m_CurrentCulture = value;
			}
		}

		private CultureInfo GetCurrentCultureNoAppX()
		{
			if (Thread.m_CurrentCulture != null)
			{
				return Thread.m_CurrentCulture;
			}
			CultureInfo defaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
			if (defaultThreadCurrentCulture == null)
			{
				return CultureInfo.UserDefaultCulture;
			}
			return defaultThreadCurrentCulture;
		}

		private static void nativeInitCultureAccessors()
		{
			Thread.m_CurrentCulture = CultureInfo.ConstructCurrentCulture();
			Thread.m_CurrentUICulture = CultureInfo.ConstructCurrentUICulture();
		}

		/// <summary>Synchronizes memory access as follows: The processor executing the current thread cannot reorder instructions in such a way that memory accesses prior to the call to <see cref="M:System.Threading.Thread.MemoryBarrier" /> execute after memory accesses that follow the call to <see cref="M:System.Threading.Thread.MemoryBarrier" />.</summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MemoryBarrier();

		private static LocalDataStoreMgr LocalDataStoreManager
		{
			get
			{
				if (Thread.s_LocalDataStoreMgr == null)
				{
					Interlocked.CompareExchange<LocalDataStoreMgr>(ref Thread.s_LocalDataStoreMgr, new LocalDataStoreMgr(), null);
				}
				return Thread.s_LocalDataStoreMgr;
			}
		}

		/// <summary>Retrieves the number of type information interfaces that an object provides (either 0 or 1).</summary>
		/// <param name="pcTInfo">Points to a location that receives the number of type information interfaces provided by the object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Thread.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>Retrieves the type information for an object, which can then be used to get the type information for an interface.</summary>
		/// <param name="iTInfo">The type information to return.</param>
		/// <param name="lcid">The locale identifier for the type information.</param>
		/// <param name="ppTInfo">Receives a pointer to the requested type information object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Thread.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>Maps a set of names to a corresponding set of dispatch identifiers.</summary>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="rgszNames">Passed-in array of names to be mapped.</param>
		/// <param name="cNames">Count of the names to be mapped.</param>
		/// <param name="lcid">The locale context in which to interpret the names.</param>
		/// <param name="rgDispId">Caller-allocated array which receives the IDs corresponding to the names.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Thread.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		/// <summary>Provides access to properties and methods exposed by an object.</summary>
		/// <param name="dispIdMember">Identifies the member.</param>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="lcid">The locale context in which to interpret arguments.</param>
		/// <param name="wFlags">Flags describing the context of the call.</param>
		/// <param name="pDispParams">Pointer to a structure containing an array of arguments, an array of argument DISPIDs for named arguments, and counts for the number of elements in the arrays.</param>
		/// <param name="pVarResult">Pointer to the location where the result is to be stored.</param>
		/// <param name="pExcepInfo">Pointer to a structure that contains exception information.</param>
		/// <param name="puArgErr">The index of the first argument that has an error.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Thread.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ConstructInternalThread();

		private InternalThread Internal
		{
			get
			{
				if (this.internal_thread == null)
				{
					this.ConstructInternalThread();
				}
				return this.internal_thread;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] ByteArrayToRootDomain(byte[] arr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] ByteArrayToCurrentDomain(byte[] arr);

		/// <summary>Gets the current context in which the thread is executing.</summary>
		/// <returns>A <see cref="T:System.Runtime.Remoting.Contexts.Context" /> representing the current thread context.</returns>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public static Context CurrentContext
		{
			get
			{
				return AppDomain.InternalGetContext();
			}
		}

		private static void DeserializePrincipal(Thread th)
		{
			MemoryStream memoryStream = new MemoryStream(Thread.ByteArrayToCurrentDomain(th.Internal._serialized_principal));
			int num = memoryStream.ReadByte();
			if (num == 0)
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				th.principal = (IPrincipal)binaryFormatter.Deserialize(memoryStream);
				th.principal_version = th.Internal._serialized_principal_version;
				return;
			}
			if (num == 1)
			{
				BinaryReader binaryReader = new BinaryReader(memoryStream);
				string name = binaryReader.ReadString();
				string type = binaryReader.ReadString();
				int num2 = binaryReader.ReadInt32();
				string[] array = null;
				if (num2 >= 0)
				{
					array = new string[num2];
					for (int i = 0; i < num2; i++)
					{
						array[i] = binaryReader.ReadString();
					}
				}
				th.principal = new GenericPrincipal(new GenericIdentity(name, type), array);
				return;
			}
			if (num == 2 || num == 3)
			{
				string[] roles = (num == 2) ? null : new string[0];
				th.principal = new GenericPrincipal(new GenericIdentity("", ""), roles);
			}
		}

		private static void SerializePrincipal(Thread th, IPrincipal value)
		{
			MemoryStream memoryStream = new MemoryStream();
			bool flag = false;
			if (value.GetType() == typeof(GenericPrincipal))
			{
				GenericPrincipal genericPrincipal = (GenericPrincipal)value;
				if (genericPrincipal.Identity != null && genericPrincipal.Identity.GetType() == typeof(GenericIdentity))
				{
					GenericIdentity genericIdentity = (GenericIdentity)genericPrincipal.Identity;
					if (genericIdentity.Name == "" && genericIdentity.AuthenticationType == "")
					{
						if (genericPrincipal.Roles == null)
						{
							memoryStream.WriteByte(2);
							flag = true;
						}
						else if (genericPrincipal.Roles.Length == 0)
						{
							memoryStream.WriteByte(3);
							flag = true;
						}
					}
					else
					{
						memoryStream.WriteByte(1);
						BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
						binaryWriter.Write(genericPrincipal.Identity.Name);
						binaryWriter.Write(genericPrincipal.Identity.AuthenticationType);
						string[] roles = genericPrincipal.Roles;
						if (roles == null)
						{
							binaryWriter.Write(-1);
						}
						else
						{
							binaryWriter.Write(roles.Length);
							foreach (string value2 in roles)
							{
								binaryWriter.Write(value2);
							}
						}
						binaryWriter.Flush();
						flag = true;
					}
				}
			}
			if (!flag)
			{
				memoryStream.WriteByte(0);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				try
				{
					binaryFormatter.Serialize(memoryStream, value);
				}
				catch
				{
				}
			}
			th.Internal._serialized_principal = Thread.ByteArrayToRootDomain(memoryStream.ToArray());
		}

		/// <summary>Gets or sets the thread's current principal (for role-based security).</summary>
		/// <returns>An <see cref="T:System.Security.Principal.IPrincipal" /> value representing the security context.</returns>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the permission required to set the principal.</exception>
		public static IPrincipal CurrentPrincipal
		{
			get
			{
				Thread currentThread = Thread.CurrentThread;
				IPrincipal principal = currentThread.GetExecutionContextReader().LogicalCallContext.Principal;
				if (principal != null)
				{
					return principal;
				}
				if (currentThread.principal_version != currentThread.Internal._serialized_principal_version)
				{
					currentThread.principal = null;
				}
				if (currentThread.principal != null)
				{
					return currentThread.principal;
				}
				if (currentThread.Internal._serialized_principal != null)
				{
					try
					{
						Thread.DeserializePrincipal(currentThread);
						return currentThread.principal;
					}
					catch
					{
					}
				}
				currentThread.principal = Thread.GetDomain().DefaultPrincipal;
				currentThread.principal_version = currentThread.Internal._serialized_principal_version;
				return currentThread.principal;
			}
			set
			{
				Thread currentThread = Thread.CurrentThread;
				currentThread.GetMutableExecutionContext().LogicalCallContext.Principal = value;
				if (value != Thread.GetDomain().DefaultPrincipal)
				{
					currentThread.Internal._serialized_principal_version++;
					try
					{
						Thread.SerializePrincipal(currentThread, value);
					}
					catch (Exception)
					{
						currentThread.Internal._serialized_principal = null;
					}
					currentThread.principal_version = currentThread.Internal._serialized_principal_version;
				}
				else
				{
					currentThread.Internal._serialized_principal = null;
				}
				currentThread.principal = value;
			}
		}

		/// <summary>Returns the current domain in which the current thread is running.</summary>
		/// <returns>An <see cref="T:System.AppDomain" /> representing the current application domain of the running thread.</returns>
		public static AppDomain GetDomain()
		{
			return AppDomain.CurrentDomain;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCurrentThread_icall(ref Thread thread);

		private static Thread GetCurrentThread()
		{
			Thread result = null;
			Thread.GetCurrentThread_icall(ref result);
			return result;
		}

		/// <summary>Gets the currently running thread.</summary>
		/// <returns>A <see cref="T:System.Threading.Thread" /> that is the representation of the currently running thread.</returns>
		public static Thread CurrentThread
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				Thread thread = Thread.current_thread;
				if (thread != null)
				{
					return thread;
				}
				return Thread.GetCurrentThread();
			}
		}

		internal static int CurrentThreadId
		{
			get
			{
				return (int)Thread.CurrentThread.internal_thread.thread_id;
			}
		}

		/// <summary>Returns a unique application domain identifier.</summary>
		/// <returns>A 32-bit signed integer uniquely identifying the application domain.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetDomainID();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Thread_internal(MulticastDelegate start);

		private Thread(InternalThread it)
		{
			this.internal_thread = it;
		}

		/// <summary>Ensures that resources are freed and other cleanup operations are performed when the garbage collector reclaims the <see cref="T:System.Threading.Thread" /> object.</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		~Thread()
		{
		}

		/// <summary>Gets or sets the apartment state of this thread.</summary>
		/// <returns>One of the <see cref="T:System.Threading.ApartmentState" /> values. The initial value is <see langword="Unknown" />.</returns>
		/// <exception cref="T:System.ArgumentException">An attempt is made to set this property to a state that is not a valid apartment state (a state other than single-threaded apartment (<see langword="STA" />) or multithreaded apartment (<see langword="MTA" />)).</exception>
		[Obsolete("Deprecated in favor of GetApartmentState, SetApartmentState and TrySetApartmentState.")]
		public ApartmentState ApartmentState
		{
			get
			{
				this.ValidateThreadState();
				return (ApartmentState)this.Internal.apartment_state;
			}
			set
			{
				this.ValidateThreadState();
				this.TrySetApartmentState(value);
			}
		}

		/// <summary>Gets a value indicating whether or not a thread belongs to the managed thread pool.</summary>
		/// <returns>
		///   <see langword="true" /> if this thread belongs to the managed thread pool; otherwise, <see langword="false" />.</returns>
		public bool IsThreadPoolThread
		{
			get
			{
				return this.IsThreadPoolThreadInternal;
			}
		}

		internal bool IsThreadPoolThreadInternal
		{
			get
			{
				return this.Internal.threadpool_thread;
			}
			set
			{
				this.Internal.threadpool_thread = value;
			}
		}

		/// <summary>Gets a value indicating the execution status of the current thread.</summary>
		/// <returns>
		///   <see langword="true" /> if this thread has been started and has not terminated normally or aborted; otherwise, <see langword="false" />.</returns>
		public bool IsAlive
		{
			get
			{
				ThreadState state = Thread.GetState(this.Internal);
				return (state & ThreadState.Aborted) == ThreadState.Running && (state & ThreadState.Stopped) == ThreadState.Running && (state & ThreadState.Unstarted) == ThreadState.Running;
			}
		}

		/// <summary>Gets or sets a value indicating whether or not a thread is a background thread.</summary>
		/// <returns>
		///   <see langword="true" /> if this thread is or is to become a background thread; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread is dead.</exception>
		public bool IsBackground
		{
			get
			{
				return (this.ValidateThreadState() & ThreadState.Background) > ThreadState.Running;
			}
			set
			{
				this.ValidateThreadState();
				if (value)
				{
					Thread.SetState(this.Internal, ThreadState.Background);
					return;
				}
				Thread.ClrState(this.Internal, ThreadState.Background);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetName_internal(InternalThread thread);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetName_icall(InternalThread thread, char* name, int nameLength);

		private unsafe static void SetName_internal(InternalThread thread, string name)
		{
			fixed (string text = name)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				Thread.SetName_icall(thread, ptr, (name != null) ? name.Length : 0);
			}
		}

		/// <summary>Gets or sets the name of the thread.</summary>
		/// <returns>A string containing the name of the thread, or <see langword="null" /> if no name was set.</returns>
		/// <exception cref="T:System.InvalidOperationException">A set operation was requested, but the <see langword="Name" /> property has already been set.</exception>
		public string Name
		{
			get
			{
				return Thread.GetName_internal(this.Internal);
			}
			set
			{
				Thread.SetName_internal(this.Internal, value);
			}
		}

		/// <summary>Gets a value containing the states of the current thread.</summary>
		/// <returns>One of the <see cref="T:System.Threading.ThreadState" /> values indicating the state of the current thread. The initial value is <see langword="Unstarted" />.</returns>
		public ThreadState ThreadState
		{
			get
			{
				return Thread.GetState(this.Internal);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Abort_internal(InternalThread thread, object stateInfo);

		/// <summary>Raises a <see cref="T:System.Threading.ThreadAbortException" /> in the thread on which it is invoked, to begin the process of terminating the thread. Calling this method usually terminates the thread.</summary>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Core only: This member is not supported.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread that is being aborted is currently suspended.</exception>
		public void Abort()
		{
			Thread.Abort_internal(this.Internal, null);
		}

		/// <summary>Raises a <see cref="T:System.Threading.ThreadAbortException" /> in the thread on which it is invoked, to begin the process of terminating the thread while also providing exception information about the thread termination. Calling this method usually terminates the thread.</summary>
		/// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted.</param>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Core only: This member is not supported.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread that is being aborted is currently suspended.</exception>
		public void Abort(object stateInfo)
		{
			Thread.Abort_internal(this.Internal, stateInfo);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetAbortExceptionState();

		internal object AbortReason
		{
			get
			{
				return this.GetAbortExceptionState();
			}
		}

		private void ClearAbortReason()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SpinWait_nop();

		/// <summary>Causes a thread to wait the number of times defined by the <paramref name="iterations" /> parameter.</summary>
		/// <param name="iterations">A 32-bit signed integer that defines how long a thread is to wait.</param>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void SpinWait(int iterations)
		{
			if (iterations < 0)
			{
				return;
			}
			while (iterations-- > 0)
			{
				Thread.SpinWait_nop();
			}
		}

		private void StartInternal(object principal, ref StackCrawlMark stackMark)
		{
			this.Internal._serialized_principal = Thread.CurrentThread.Internal._serialized_principal;
			if (!this.Thread_internal(this.m_Delegate))
			{
				throw new SystemException("Thread creation failed.");
			}
			this.m_ThreadStartArg = null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetState(InternalThread thread, ThreadState set);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClrState(InternalThread thread, ThreadState clr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ThreadState GetState(InternalThread thread);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte VolatileRead(ref byte address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double VolatileRead(ref double address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern short VolatileRead(ref short address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int VolatileRead(ref int address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long VolatileRead(ref long address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr VolatileRead(ref IntPtr address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object VolatileRead(ref object address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern sbyte VolatileRead(ref sbyte address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float VolatileRead(ref float address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ushort VolatileRead(ref ushort address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint VolatileRead(ref uint address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong VolatileRead(ref ulong address);

		/// <summary>Reads the value of a field. The value is the latest written by any processor in a computer, regardless of the number of processors or the state of processor cache.</summary>
		/// <param name="address">The field to be read.</param>
		/// <returns>The latest value written to the field by any processor.</returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UIntPtr VolatileRead(ref UIntPtr address);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref byte address, byte value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref double address, double value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref short address, short value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref int address, int value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref long address, long value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref IntPtr address, IntPtr value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref object address, object value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref sbyte address, sbyte value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref float address, float value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref ushort address, ushort value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref uint address, uint value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref ulong address, ulong value);

		/// <summary>Writes a value to a field immediately, so that the value is visible to all processors in the computer.</summary>
		/// <param name="address">The field to which the value is to be written.</param>
		/// <param name="value">The value to be written.</param>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref UIntPtr address, UIntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SystemMaxStackStize();

		private static int GetProcessDefaultStackSize(int maxStackSize)
		{
			if (maxStackSize == 0)
			{
				return 0;
			}
			if (maxStackSize < 131072)
			{
				return 131072;
			}
			int pageSize = Environment.GetPageSize();
			if (maxStackSize % pageSize != 0)
			{
				maxStackSize = maxStackSize / (pageSize - 1) * pageSize;
			}
			return Math.Min(maxStackSize, Thread.SystemMaxStackStize());
		}

		private void SetStart(MulticastDelegate start, int maxStackSize)
		{
			this.m_Delegate = start;
			this.Internal.stack_size = maxStackSize;
		}

		/// <summary>Gets a unique identifier for the current managed thread.</summary>
		/// <returns>An integer that represents a unique identifier for this managed thread.</returns>
		public int ManagedThreadId
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this.Internal.managed_id;
			}
		}

		/// <summary>Notifies a host that execution is about to enter a region of code in which the effects of a thread abort or unhandled exception might jeopardize other tasks in the application domain.</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void BeginCriticalRegion()
		{
			Thread.CurrentThread.Internal.critical_region_level++;
		}

		/// <summary>Notifies a host that execution is about to enter a region of code in which the effects of a thread abort or unhandled exception are limited to the current task.</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void EndCriticalRegion()
		{
			Thread.CurrentThread.Internal.critical_region_level--;
		}

		/// <summary>Notifies a host that managed code is about to execute instructions that depend on the identity of the current physical operating system thread.</summary>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void BeginThreadAffinity()
		{
		}

		/// <summary>Notifies a host that managed code has finished executing instructions that depend on the identity of the current physical operating system thread.</summary>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void EndThreadAffinity()
		{
		}

		/// <summary>Returns an <see cref="T:System.Threading.ApartmentState" /> value indicating the apartment state.</summary>
		/// <returns>One of the <see cref="T:System.Threading.ApartmentState" /> values indicating the apartment state of the managed thread. The default is <see cref="F:System.Threading.ApartmentState.Unknown" />.</returns>
		public ApartmentState GetApartmentState()
		{
			this.ValidateThreadState();
			return (ApartmentState)this.Internal.apartment_state;
		}

		/// <summary>Sets the apartment state of a thread before it is started.</summary>
		/// <param name="state">The new apartment state.</param>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Core only: This member is not supported on the macOS and Linux platforms.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="state" /> is not a valid apartment state.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread has already been started.</exception>
		/// <exception cref="T:System.InvalidOperationException">The apartment state has already been initialized.</exception>
		public void SetApartmentState(ApartmentState state)
		{
			if (!this.TrySetApartmentState(state))
			{
				throw new InvalidOperationException("Failed to set the specified COM apartment state.");
			}
		}

		/// <summary>Sets the apartment state of a thread before it is started.</summary>
		/// <param name="state">The new apartment state.</param>
		/// <returns>
		///   <see langword="true" /> if the apartment state is set; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="state" /> is not a valid apartment state.</exception>
		/// <exception cref="T:System.Threading.ThreadStateException">The thread has already been started.</exception>
		public bool TrySetApartmentState(ApartmentState state)
		{
			if ((this.ThreadState & ThreadState.Unstarted) == ThreadState.Running)
			{
				throw new ThreadStateException("Thread was in an invalid state for the operation being executed.");
			}
			if (this.Internal.apartment_state != 2 && (ApartmentState)this.Internal.apartment_state != state)
			{
				return false;
			}
			this.Internal.apartment_state = (byte)state;
			return true;
		}

		/// <summary>Returns a hash code for the current thread.</summary>
		/// <returns>An integer hash code value.</returns>
		[ComVisible(false)]
		public override int GetHashCode()
		{
			return this.ManagedThreadId;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetStackTraces(out Thread[] threads, out object[] stack_frames);

		internal static Dictionary<Thread, StackTrace> Mono_GetStackTraces()
		{
			Thread[] array;
			object[] array2;
			Thread.GetStackTraces(out array, out array2);
			Dictionary<Thread, StackTrace> dictionary = new Dictionary<Thread, StackTrace>();
			for (int i = 0; i < array.Length; i++)
			{
				dictionary[array[i]] = new StackTrace((StackFrame[])array2[i]);
			}
			return dictionary;
		}

		/// <summary>Turns off automatic cleanup of runtime callable wrappers (RCW) for the current thread.</summary>
		public void DisableComObjectEagerCleanup()
		{
			throw new PlatformNotSupportedException();
		}

		private ThreadState ValidateThreadState()
		{
			ThreadState state = Thread.GetState(this.Internal);
			if ((state & ThreadState.Stopped) != ThreadState.Running)
			{
				throw new ThreadStateException("Thread is dead; state can not be accessed.");
			}
			return state;
		}

		public static int GetCurrentProcessorId()
		{
			return RuntimeThread.GetCurrentProcessorId();
		}

		private static LocalDataStoreMgr s_LocalDataStoreMgr;

		[ThreadStatic]
		private static LocalDataStoreHolder s_LocalDataStore;

		[ThreadStatic]
		internal static CultureInfo m_CurrentCulture;

		[ThreadStatic]
		internal static CultureInfo m_CurrentUICulture;

		private static AsyncLocal<CultureInfo> s_asyncLocalCurrentCulture;

		private static AsyncLocal<CultureInfo> s_asyncLocalCurrentUICulture;

		private InternalThread internal_thread;

		private object m_ThreadStartArg;

		private object pending_exception;

		[ThreadStatic]
		private static Thread current_thread;

		private MulticastDelegate m_Delegate;

		private ExecutionContext m_ExecutionContext;

		private bool m_ExecutionContextBelongsToOuterScope;

		private IPrincipal principal;

		private int principal_version;
	}
}
