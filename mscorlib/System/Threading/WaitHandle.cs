using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Contexts;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
	/// <summary>Encapsulates operating system-specific objects that wait for exclusive access to shared resources.</summary>
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class WaitHandle : MarshalByRefObject, IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.WaitHandle" /> class.</summary>
		protected WaitHandle()
		{
			this.Init();
		}

		[SecuritySafeCritical]
		private void Init()
		{
			this.safeWaitHandle = null;
			this.waitHandle = WaitHandle.InvalidHandle;
			this.hasThreadAffinity = false;
		}

		/// <summary>Gets or sets the native operating system handle.</summary>
		/// <returns>An <see langword="IntPtr" /> representing the native operating system handle. The default is the value of the <see cref="F:System.Threading.WaitHandle.InvalidHandle" /> field.</returns>
		[Obsolete("Use the SafeWaitHandle property instead.")]
		public virtual IntPtr Handle
		{
			[SecuritySafeCritical]
			get
			{
				if (this.safeWaitHandle != null)
				{
					return this.safeWaitHandle.DangerousGetHandle();
				}
				return WaitHandle.InvalidHandle;
			}
			[SecurityCritical]
			set
			{
				if (value == WaitHandle.InvalidHandle)
				{
					if (this.safeWaitHandle != null)
					{
						this.safeWaitHandle.SetHandleAsInvalid();
						this.safeWaitHandle = null;
					}
				}
				else
				{
					this.safeWaitHandle = new SafeWaitHandle(value, true);
				}
				this.waitHandle = value;
			}
		}

		/// <summary>Gets or sets the native operating system handle.</summary>
		/// <returns>A <see cref="T:Microsoft.Win32.SafeHandles.SafeWaitHandle" /> representing the native operating system handle.</returns>
		public SafeWaitHandle SafeWaitHandle
		{
			[SecurityCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				if (this.safeWaitHandle == null)
				{
					this.safeWaitHandle = new SafeWaitHandle(WaitHandle.InvalidHandle, false);
				}
				return this.safeWaitHandle;
			}
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[SecurityCritical]
			set
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					if (value == null)
					{
						this.safeWaitHandle = null;
						this.waitHandle = WaitHandle.InvalidHandle;
					}
					else
					{
						this.safeWaitHandle = value;
						this.waitHandle = this.safeWaitHandle.DangerousGetHandle();
					}
				}
			}
		}

		[SecurityCritical]
		internal void SetHandleInternal(SafeWaitHandle handle)
		{
			this.safeWaitHandle = handle;
			this.waitHandle = handle.DangerousGetHandle();
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.WaitHandle" /> receives a signal, using a 32-bit signed integer to specify the time interval and specifying whether to exit the synchronization domain before the wait.</summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <param name="exitContext">
		///   <see langword="true" /> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the current instance receives a signal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current instance is a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public virtual bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return this.WaitOne((long)millisecondsTimeout, exitContext);
		}

		/// <summary>Blocks the current thread until the current instance receives a signal, using a <see cref="T:System.TimeSpan" /> to specify the time interval and specifying whether to exit the synchronization domain before the wait.</summary>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="exitContext">
		///   <see langword="true" /> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the current instance receives a signal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current instance is a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public virtual bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (-1L > num || 2147483647L < num)
			{
				throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return this.WaitOne(num, exitContext);
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.WaitHandle" /> receives a signal.</summary>
		/// <returns>
		///   <see langword="true" /> if the current instance receives a signal. If the current instance is never signaled, <see cref="M:System.Threading.WaitHandle.WaitOne(System.Int32,System.Boolean)" /> never returns.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has already been disposed.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current instance is a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public virtual bool WaitOne()
		{
			return this.WaitOne(-1, false);
		}

		/// <summary>Blocks the current thread until the current <see cref="T:System.Threading.WaitHandle" /> receives a signal, using a 32-bit signed integer to specify the time interval in milliseconds.</summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <returns>
		///   <see langword="true" /> if the current instance receives a signal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current instance is a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public virtual bool WaitOne(int millisecondsTimeout)
		{
			return this.WaitOne(millisecondsTimeout, false);
		}

		/// <summary>Blocks the current thread until the current instance receives a signal, using a <see cref="T:System.TimeSpan" /> to specify the time interval.</summary>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <returns>
		///   <see langword="true" /> if the current instance receives a signal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current instance is a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public virtual bool WaitOne(TimeSpan timeout)
		{
			return this.WaitOne(timeout, false);
		}

		[SecuritySafeCritical]
		private bool WaitOne(long timeout, bool exitContext)
		{
			return WaitHandle.InternalWaitOne(this.safeWaitHandle, timeout, this.hasThreadAffinity, exitContext);
		}

		[SecurityCritical]
		internal static bool InternalWaitOne(SafeHandle waitableSafeHandle, long millisecondsTimeout, bool hasThreadAffinity, bool exitContext)
		{
			if (waitableSafeHandle == null)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("Cannot access a disposed object."));
			}
			int num = WaitHandle.WaitOneNative(waitableSafeHandle, (uint)millisecondsTimeout, hasThreadAffinity, exitContext);
			if (num == 128)
			{
				WaitHandle.ThrowAbandonedMutexException();
			}
			return num != 258 && num != int.MaxValue;
		}

		[SecurityCritical]
		internal bool WaitOneWithoutFAS()
		{
			if (this.safeWaitHandle == null)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("Cannot access a disposed object."));
			}
			long num = -1L;
			int num2 = WaitHandle.WaitOneNative(this.safeWaitHandle, (uint)num, this.hasThreadAffinity, false);
			if (num2 == 128)
			{
				WaitHandle.ThrowAbandonedMutexException();
			}
			return num2 != 258 && num2 != int.MaxValue;
		}

		/// <summary>Waits for all the elements in the specified array to receive a signal, using an <see cref="T:System.Int32" /> value to specify the time interval and specifying whether to exit the synchronization domain before the wait.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait. This array cannot contain multiple references to the same object (duplicates).</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <param name="exitContext">
		///   <see langword="true" /> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> when every element in <paramref name="waitHandles" /> has received a signal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.  
		///  -or-  
		///  <paramref name="waitHandles" /> is an array with no elements and the .NET Framework version is 2.0 or later.</exception>
		/// <exception cref="T:System.DuplicateWaitObjectException">The <paramref name="waitHandles" /> array contains elements that are duplicates.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.  
		///  -or-  
		///  The <see cref="T:System.STAThreadAttribute" /> attribute is applied to the thread procedure for the current thread, and <paramref name="waitHandles" /> contains more than one element.</exception>
		/// <exception cref="T:System.ApplicationException">
		///   <paramref name="waitHandles" /> is an array with no elements and the .NET Framework version is 1.0 or 1.1.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		[SecuritySafeCritical]
		public static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
		{
			if (waitHandles == null)
			{
				throw new ArgumentNullException(Environment.GetResourceString("The waitHandles parameter cannot be null."));
			}
			if (waitHandles.Length == 0)
			{
				throw new ArgumentNullException(Environment.GetResourceString("Waithandle array may not be empty."));
			}
			if (waitHandles.Length > 64)
			{
				throw new NotSupportedException(Environment.GetResourceString("The number of WaitHandles must be less than or equal to 64."));
			}
			if (-1 > millisecondsTimeout)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			WaitHandle[] array = new WaitHandle[waitHandles.Length];
			for (int i = 0; i < waitHandles.Length; i++)
			{
				WaitHandle waitHandle = waitHandles[i];
				if (waitHandle == null)
				{
					throw new ArgumentNullException(Environment.GetResourceString("At least one element in the specified array was null."));
				}
				if (RemotingServices.IsTransparentProxy(waitHandle))
				{
					throw new InvalidOperationException(Environment.GetResourceString("Cannot wait on a transparent proxy."));
				}
				array[i] = waitHandle;
			}
			int num = WaitHandle.WaitMultiple(array, millisecondsTimeout, exitContext, true);
			if (128 <= num && 128 + array.Length > num)
			{
				WaitHandle.ThrowAbandonedMutexException();
			}
			GC.KeepAlive(array);
			return num != 258 && num != int.MaxValue;
		}

		/// <summary>Waits for all the elements in the specified array to receive a signal, using a <see cref="T:System.TimeSpan" /> value to specify the time interval, and specifying whether to exit the synchronization domain before the wait.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait. This array cannot contain multiple references to the same object.</param>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds, to wait indefinitely.</param>
		/// <param name="exitContext">
		///   <see langword="true" /> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> when every element in <paramref name="waitHandles" /> has received a signal; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.  
		///  -or-  
		///  <paramref name="waitHandles" /> is an array with no elements and the .NET Framework version is 2.0 or later.</exception>
		/// <exception cref="T:System.DuplicateWaitObjectException">The <paramref name="waitHandles" /> array contains elements that are duplicates.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.  
		///  -or-  
		///  The <see cref="T:System.STAThreadAttribute" /> attribute is applied to the thread procedure for the current thread, and <paramref name="waitHandles" /> contains more than one element.</exception>
		/// <exception cref="T:System.ApplicationException">
		///   <paramref name="waitHandles" /> is an array with no elements and the .NET Framework version is 1.0 or 1.1.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait terminated because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (-1L > num || 2147483647L < num)
			{
				throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return WaitHandle.WaitAll(waitHandles, (int)num, exitContext);
		}

		/// <summary>Waits for all the elements in the specified array to receive a signal.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait. This array cannot contain multiple references to the same object.</param>
		/// <returns>
		///   <see langword="true" /> when every element in <paramref name="waitHandles" /> has received a signal; otherwise the method never returns.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />. -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array are <see langword="null" />.  
		///  -or-  
		///  <paramref name="waitHandles" /> is an array with no elements and the .NET Framework version is 2.0 or later.</exception>
		/// <exception cref="T:System.DuplicateWaitObjectException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.ArgumentException" />, instead.  
		///
		///
		///
		///
		///  The <paramref name="waitHandles" /> array contains elements that are duplicates.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.  
		///  -or-  
		///  The <see cref="T:System.STAThreadAttribute" /> attribute is applied to the thread procedure for the current thread, and <paramref name="waitHandles" /> contains more than one element.</exception>
		/// <exception cref="T:System.ApplicationException">
		///   <paramref name="waitHandles" /> is an array with no elements and the .NET Framework version is 1.0 or 1.1.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait terminated because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public static bool WaitAll(WaitHandle[] waitHandles)
		{
			return WaitHandle.WaitAll(waitHandles, -1, true);
		}

		/// <summary>Waits for all the elements in the specified array to receive a signal, using an <see cref="T:System.Int32" /> value to specify the time interval.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait. This array cannot contain multiple references to the same object (duplicates).</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <returns>
		///   <see langword="true" /> when every element in <paramref name="waitHandles" /> has received a signal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.  
		///  -or-  
		///  <paramref name="waitHandles" /> is an array with no elements.</exception>
		/// <exception cref="T:System.DuplicateWaitObjectException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.ArgumentException" />, instead.  
		///
		///
		///
		///
		///  The <paramref name="waitHandles" /> array contains elements that are duplicates.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.  
		///  -or-  
		///  The <see cref="T:System.STAThreadAttribute" /> attribute is applied to the thread procedure for the current thread, and <paramref name="waitHandles" /> contains more than one element.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout)
		{
			return WaitHandle.WaitAll(waitHandles, millisecondsTimeout, true);
		}

		/// <summary>Waits for all the elements in the specified array to receive a signal, using a <see cref="T:System.TimeSpan" /> value to specify the time interval.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait. This array cannot contain multiple references to the same object.</param>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds, to wait indefinitely.</param>
		/// <returns>
		///   <see langword="true" /> when every element in <paramref name="waitHandles" /> has received a signal; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.  
		///  -or-  
		///  <paramref name="waitHandles" /> is an array with no elements.</exception>
		/// <exception cref="T:System.DuplicateWaitObjectException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.ArgumentException" />, instead.  
		///
		///
		///
		///
		///  The <paramref name="waitHandles" /> array contains elements that are duplicates.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.  
		///  -or-  
		///  The <see cref="T:System.STAThreadAttribute" /> attribute is applied to the thread procedure for the current thread, and <paramref name="waitHandles" /> contains more than one element.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait terminated because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout)
		{
			return WaitHandle.WaitAll(waitHandles, timeout, true);
		}

		/// <summary>Waits for any of the elements in the specified array to receive a signal, using a 32-bit signed integer to specify the time interval, and specifying whether to exit the synchronization domain before the wait.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <param name="exitContext">
		///   <see langword="true" /> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <see langword="false" />.</param>
		/// <returns>The array index of the object that satisfied the wait, or <see cref="F:System.Threading.WaitHandle.WaitTimeout" /> if no object satisfied the wait and a time interval equivalent to <paramref name="millisecondsTimeout" /> has passed.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.</exception>
		/// <exception cref="T:System.ApplicationException">
		///   <paramref name="waitHandles" /> is an array with no elements, and the .NET Framework version is 1.0 or 1.1.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="waitHandles" /> is an array with no elements, and the .NET Framework version is 2.0 or later.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[SecuritySafeCritical]
		public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
		{
			if (waitHandles == null)
			{
				throw new ArgumentNullException(Environment.GetResourceString("The waitHandles parameter cannot be null."));
			}
			if (waitHandles.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Waithandle array may not be empty."));
			}
			if (64 < waitHandles.Length)
			{
				throw new NotSupportedException(Environment.GetResourceString("The number of WaitHandles must be less than or equal to 64."));
			}
			if (-1 > millisecondsTimeout)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			WaitHandle[] array = new WaitHandle[waitHandles.Length];
			for (int i = 0; i < waitHandles.Length; i++)
			{
				WaitHandle waitHandle = waitHandles[i];
				if (waitHandle == null)
				{
					throw new ArgumentNullException(Environment.GetResourceString("At least one element in the specified array was null."));
				}
				if (RemotingServices.IsTransparentProxy(waitHandle))
				{
					throw new InvalidOperationException(Environment.GetResourceString("Cannot wait on a transparent proxy."));
				}
				array[i] = waitHandle;
			}
			int num = WaitHandle.WaitMultiple(array, millisecondsTimeout, exitContext, false);
			if (128 <= num && 128 + array.Length > num)
			{
				int num2 = num - 128;
				if (0 <= num2 && num2 < array.Length)
				{
					WaitHandle.ThrowAbandonedMutexException(num2, array[num2]);
				}
				else
				{
					WaitHandle.ThrowAbandonedMutexException();
				}
			}
			GC.KeepAlive(array);
			return num;
		}

		/// <summary>Waits for any of the elements in the specified array to receive a signal, using a <see cref="T:System.TimeSpan" /> to specify the time interval and specifying whether to exit the synchronization domain before the wait.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait.</param>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="exitContext">
		///   <see langword="true" /> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <see langword="false" />.</param>
		/// <returns>The array index of the object that satisfied the wait, or <see cref="F:System.Threading.WaitHandle.WaitTimeout" /> if no object satisfied the wait and a time interval equivalent to <paramref name="timeout" /> has passed.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.</exception>
		/// <exception cref="T:System.ApplicationException">
		///   <paramref name="waitHandles" /> is an array with no elements, and the .NET Framework version is 1.0 or 1.1.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="waitHandles" /> is an array with no elements, and the .NET Framework version is 2.0 or later.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (-1L > num || 2147483647L < num)
			{
				throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return WaitHandle.WaitAny(waitHandles, (int)num, exitContext);
		}

		/// <summary>Waits for any of the elements in the specified array to receive a signal, using a <see cref="T:System.TimeSpan" /> to specify the time interval.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait.</param>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <returns>The array index of the object that satisfied the wait, or <see cref="F:System.Threading.WaitHandle.WaitTimeout" /> if no object satisfied the wait and a time interval equivalent to <paramref name="timeout" /> has passed.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="waitHandles" /> is an array with no elements.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout)
		{
			return WaitHandle.WaitAny(waitHandles, timeout, true);
		}

		/// <summary>Waits for any of the elements in the specified array to receive a signal.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait.</param>
		/// <returns>The array index of the object that satisfied the wait.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.</exception>
		/// <exception cref="T:System.ApplicationException">
		///   <paramref name="waitHandles" /> is an array with no elements, and the .NET Framework version is 1.0 or 1.1.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="waitHandles" /> is an array with no elements, and the .NET Framework version is 2.0 or later.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles)
		{
			return WaitHandle.WaitAny(waitHandles, -1, true);
		}

		/// <summary>Waits for any of the elements in the specified array to receive a signal, using a 32-bit signed integer to specify the time interval.</summary>
		/// <param name="waitHandles">A <see langword="WaitHandle" /> array containing the objects for which the current instance will wait.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <returns>The array index of the object that satisfied the wait, or <see cref="F:System.Threading.WaitHandle.WaitTimeout" /> if no object satisfied the wait and a time interval equivalent to <paramref name="millisecondsTimeout" /> has passed.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="waitHandles" /> parameter is <see langword="null" />.  
		///  -or-  
		///  One or more of the objects in the <paramref name="waitHandles" /> array is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The number of objects in <paramref name="waitHandles" /> is greater than the system permits.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="waitHandles" /> is an array with no elements.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="waitHandles" /> array contains a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout)
		{
			return WaitHandle.WaitAny(waitHandles, millisecondsTimeout, true);
		}

		/// <summary>Signals one <see cref="T:System.Threading.WaitHandle" /> and waits on another.</summary>
		/// <param name="toSignal">The <see cref="T:System.Threading.WaitHandle" /> to signal.</param>
		/// <param name="toWaitOn">The <see cref="T:System.Threading.WaitHandle" /> to wait on.</param>
		/// <returns>
		///   <see langword="true" /> if both the signal and the wait complete successfully; if the wait does not complete, the method does not return.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="toSignal" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="toWaitOn" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The method was called on a thread that has <see cref="T:System.STAThreadAttribute" />.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">This method is not supported on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="toSignal" /> is a semaphore, and it already has a full count.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn)
		{
			return WaitHandle.SignalAndWait(toSignal, toWaitOn, -1, false);
		}

		/// <summary>Signals one <see cref="T:System.Threading.WaitHandle" /> and waits on another, specifying the time-out interval as a <see cref="T:System.TimeSpan" /> and specifying whether to exit the synchronization domain for the context before entering the wait.</summary>
		/// <param name="toSignal">The <see cref="T:System.Threading.WaitHandle" /> to signal.</param>
		/// <param name="toWaitOn">The <see cref="T:System.Threading.WaitHandle" /> to wait on.</param>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the interval to wait. If the value is -1, the wait is infinite.</param>
		/// <param name="exitContext">
		///   <see langword="true" /> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if both the signal and the wait completed successfully, or <see langword="false" /> if the signal completed but the wait timed out.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="toSignal" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="toWaitOn" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The method was called on a thread that has <see cref="T:System.STAThreadAttribute" />.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">This method is not supported on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="toSignal" /> is a semaphore, and it already has a full count.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> evaluates to a negative number of milliseconds other than -1.  
		/// -or-  
		/// <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, TimeSpan timeout, bool exitContext)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (-1L > num || 2147483647L < num)
			{
				throw new ArgumentOutOfRangeException("timeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return WaitHandle.SignalAndWait(toSignal, toWaitOn, (int)num, exitContext);
		}

		/// <summary>Signals one <see cref="T:System.Threading.WaitHandle" /> and waits on another, specifying a time-out interval as a 32-bit signed integer and specifying whether to exit the synchronization domain for the context before entering the wait.</summary>
		/// <param name="toSignal">The <see cref="T:System.Threading.WaitHandle" /> to signal.</param>
		/// <param name="toWaitOn">The <see cref="T:System.Threading.WaitHandle" /> to wait on.</param>
		/// <param name="millisecondsTimeout">An integer that represents the interval to wait. If the value is <see cref="F:System.Threading.Timeout.Infinite" />, that is, -1, the wait is infinite.</param>
		/// <param name="exitContext">
		///   <see langword="true" /> to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if both the signal and the wait completed successfully, or <see langword="false" /> if the signal completed but the wait timed out.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="toSignal" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="toWaitOn" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The method is called on a thread that has <see cref="T:System.STAThreadAttribute" />.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">This method is not supported on Windows 98 or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Threading.WaitHandle" /> cannot be signaled because it would exceed its maximum count.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.Threading.AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
		[SecuritySafeCritical]
		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, int millisecondsTimeout, bool exitContext)
		{
			if (toSignal == null)
			{
				throw new ArgumentNullException("toSignal");
			}
			if (toWaitOn == null)
			{
				throw new ArgumentNullException("toWaitOn");
			}
			if (-1 > millisecondsTimeout)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			int num = WaitHandle.SignalAndWaitOne(toSignal.safeWaitHandle, toWaitOn.safeWaitHandle, millisecondsTimeout, toWaitOn.hasThreadAffinity, exitContext);
			if (2147483647 != num && toSignal.hasThreadAffinity)
			{
				Thread.EndCriticalRegion();
				Thread.EndThreadAffinity();
			}
			if (128 == num)
			{
				WaitHandle.ThrowAbandonedMutexException();
			}
			if (298 == num)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The WaitHandle cannot be signaled because it would exceed its maximum count."));
			}
			if (299 == num)
			{
				throw new ApplicationException("Attempt to release mutex not owned by caller");
			}
			return num == 0;
		}

		private static void ThrowAbandonedMutexException()
		{
			throw new AbandonedMutexException();
		}

		private static void ThrowAbandonedMutexException(int location, WaitHandle handle)
		{
			throw new AbandonedMutexException(location, handle);
		}

		/// <summary>Releases all resources held by the current <see cref="T:System.Threading.WaitHandle" />.</summary>
		public virtual void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>When overridden in a derived class, releases the unmanaged resources used by the <see cref="T:System.Threading.WaitHandle" />, and optionally releases the managed resources.</summary>
		/// <param name="explicitDisposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		[SecuritySafeCritical]
		protected virtual void Dispose(bool explicitDisposing)
		{
			if (this.safeWaitHandle != null)
			{
				this.safeWaitHandle.Close();
			}
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Threading.WaitHandle" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private unsafe static int WaitOneNative(SafeHandle waitableSafeHandle, uint millisecondsTimeout, bool hasThreadAffinity, bool exitContext)
		{
			bool flag = false;
			SynchronizationContext synchronizationContext = SynchronizationContext.Current;
			int result;
			try
			{
				waitableSafeHandle.DangerousAddRef(ref flag);
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				if (synchronizationContext != null && synchronizationContext.IsWaitNotificationRequired())
				{
					result = synchronizationContext.Wait(new IntPtr[]
					{
						waitableSafeHandle.DangerousGetHandle()
					}, false, (int)millisecondsTimeout);
				}
				else
				{
					IntPtr intPtr = waitableSafeHandle.DangerousGetHandle();
					result = WaitHandle.Wait_internal(&intPtr, 1, false, (int)millisecondsTimeout);
				}
			}
			finally
			{
				if (flag)
				{
					waitableSafeHandle.DangerousRelease();
				}
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
			return result;
		}

		private unsafe static int WaitMultiple(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext, bool WaitAll)
		{
			if (waitHandles.Length > 64)
			{
				return int.MaxValue;
			}
			int num = -1;
			SynchronizationContext synchronizationContext = SynchronizationContext.Current;
			int result;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				for (int i = 0; i < waitHandles.Length; i++)
				{
					try
					{
					}
					finally
					{
						bool flag = false;
						waitHandles[i].SafeWaitHandle.DangerousAddRef(ref flag);
						num = i;
					}
				}
				if (synchronizationContext != null && synchronizationContext.IsWaitNotificationRequired())
				{
					IntPtr[] array = new IntPtr[waitHandles.Length];
					for (int j = 0; j < waitHandles.Length; j++)
					{
						array[j] = waitHandles[j].SafeWaitHandle.DangerousGetHandle();
					}
					result = synchronizationContext.Wait(array, false, millisecondsTimeout);
				}
				else
				{
					IntPtr* ptr = stackalloc IntPtr[checked(unchecked((UIntPtr)waitHandles.Length) * (UIntPtr)sizeof(IntPtr))];
					for (int k = 0; k < waitHandles.Length; k++)
					{
						ptr[k] = waitHandles[k].SafeWaitHandle.DangerousGetHandle();
					}
					result = WaitHandle.Wait_internal(ptr, waitHandles.Length, WaitAll, millisecondsTimeout);
				}
			}
			finally
			{
				for (int l = num; l >= 0; l--)
				{
					waitHandles[l].SafeWaitHandle.DangerousRelease();
				}
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern int Wait_internal(IntPtr* handles, int numHandles, bool waitAll, int ms);

		private static int SignalAndWaitOne(SafeWaitHandle waitHandleToSignal, SafeWaitHandle waitHandleToWaitOn, int millisecondsTimeout, bool hasThreadAffinity, bool exitContext)
		{
			bool flag = false;
			bool flag2 = false;
			int result;
			try
			{
				waitHandleToSignal.DangerousAddRef(ref flag);
				waitHandleToWaitOn.DangerousAddRef(ref flag2);
				result = WaitHandle.SignalAndWait_Internal(waitHandleToSignal.DangerousGetHandle(), waitHandleToWaitOn.DangerousGetHandle(), millisecondsTimeout);
			}
			finally
			{
				if (flag)
				{
					waitHandleToSignal.DangerousRelease();
				}
				if (flag2)
				{
					waitHandleToWaitOn.DangerousRelease();
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SignalAndWait_Internal(IntPtr toSignal, IntPtr toWaitOn, int ms);

		internal static int ToTimeoutMilliseconds(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", "Number must be either non-negative and less than or equal to Int32.MaxValue or -1.");
			}
			return (int)num;
		}

		/// <summary>Indicates that a <see cref="M:System.Threading.WaitHandle.WaitAny(System.Threading.WaitHandle[],System.Int32,System.Boolean)" /> operation timed out before any of the wait handles were signaled. This field is constant.</summary>
		public const int WaitTimeout = 258;

		private const int MAX_WAITHANDLES = 64;

		private IntPtr waitHandle;

		[SecurityCritical]
		internal volatile SafeWaitHandle safeWaitHandle;

		internal bool hasThreadAffinity;

		private const int WAIT_OBJECT_0 = 0;

		private const int WAIT_ABANDONED = 128;

		private const int WAIT_FAILED = 2147483647;

		private const int ERROR_TOO_MANY_POSTS = 298;

		private const int ERROR_NOT_OWNED_BY_CALLER = 299;

		/// <summary>Represents an invalid native operating system handle. This field is read-only.</summary>
		protected static readonly IntPtr InvalidHandle = (IntPtr)(-1);

		internal const int MaxWaitHandles = 64;

		internal enum OpenExistingResult
		{
			Success,
			NameNotFound,
			PathNotFound,
			NameInvalid
		}
	}
}
