using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace System
{
	/// <summary>Controls the system garbage collector, a service that automatically reclaims unused memory.</summary>
	public static class GC
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCollectionCount(int generation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaxGeneration();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalCollect(int generation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RecordPressure(long bytesAllocated);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void register_ephemeron_array(Ephemeron[] array);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object get_ephemeron_tombstone();

		internal static void GetMemoryInfo(out uint highMemLoadThreshold, out ulong totalPhysicalMem, out uint lastRecordedMemLoad, out UIntPtr lastRecordedHeapSize, out UIntPtr lastRecordedFragmentation)
		{
			highMemLoadThreshold = 0U;
			totalPhysicalMem = ulong.MaxValue;
			lastRecordedMemLoad = 0U;
			lastRecordedHeapSize = UIntPtr.Zero;
			lastRecordedFragmentation = UIntPtr.Zero;
		}

		/// <summary>Gets the total number of bytes allocated to the current thread since the beginning of its lifetime.</summary>
		/// <returns>The total number of bytes allocated to the current thread since the beginning of its lifetime.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetAllocatedBytesForCurrentThread();

		/// <summary>Informs the runtime of a large allocation of unmanaged memory that should be taken into account when scheduling garbage collection.</summary>
		/// <param name="bytesAllocated">The incremental amount of unmanaged memory that has been allocated.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="bytesAllocated" /> is less than or equal to 0.  
		/// -or-  
		/// On a 32-bit computer, <paramref name="bytesAllocated" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		[SecurityCritical]
		public static void AddMemoryPressure(long bytesAllocated)
		{
			if (bytesAllocated <= 0L)
			{
				throw new ArgumentOutOfRangeException("bytesAllocated", Environment.GetResourceString("Positive number required."));
			}
			if (4 == IntPtr.Size && bytesAllocated > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("pressure", Environment.GetResourceString("Value must be non-negative and less than or equal to Int32.MaxValue."));
			}
			GC.RecordPressure(bytesAllocated);
		}

		/// <summary>Informs the runtime that unmanaged memory has been released and no longer needs to be taken into account when scheduling garbage collection.</summary>
		/// <param name="bytesAllocated">The amount of unmanaged memory that has been released.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="bytesAllocated" /> is less than or equal to 0.  
		/// -or-  
		/// On a 32-bit computer, <paramref name="bytesAllocated" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		[SecurityCritical]
		public static void RemoveMemoryPressure(long bytesAllocated)
		{
			if (bytesAllocated <= 0L)
			{
				throw new ArgumentOutOfRangeException("bytesAllocated", Environment.GetResourceString("Positive number required."));
			}
			if (4 == IntPtr.Size && bytesAllocated > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("bytesAllocated", Environment.GetResourceString("Value must be non-negative and less than or equal to Int32.MaxValue."));
			}
			GC.RecordPressure(-bytesAllocated);
		}

		/// <summary>Returns the current generation number of the specified object.</summary>
		/// <param name="obj">The object that generation information is retrieved for.</param>
		/// <returns>The current generation number of <paramref name="obj" />.</returns>
		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetGeneration(object obj);

		/// <summary>Forces an immediate garbage collection from generation 0 through a specified generation.</summary>
		/// <param name="generation">The number of the oldest generation to be garbage collected.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="generation" /> is not valid.</exception>
		public static void Collect(int generation)
		{
			GC.Collect(generation, GCCollectionMode.Default);
		}

		/// <summary>Forces an immediate garbage collection of all generations.</summary>
		[SecuritySafeCritical]
		public static void Collect()
		{
			GC.InternalCollect(GC.MaxGeneration);
		}

		/// <summary>Forces a garbage collection from generation 0 through a specified generation, at a time specified by a <see cref="T:System.GCCollectionMode" /> value.</summary>
		/// <param name="generation">The number of the oldest generation to be garbage collected.</param>
		/// <param name="mode">An enumeration value that specifies whether the garbage collection is forced (<see cref="F:System.GCCollectionMode.Default" /> or <see cref="F:System.GCCollectionMode.Forced" />) or optimized (<see cref="F:System.GCCollectionMode.Optimized" />).</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="generation" /> is not valid.  
		/// -or-  
		/// <paramref name="mode" /> is not one of the <see cref="T:System.GCCollectionMode" /> values.</exception>
		[SecuritySafeCritical]
		public static void Collect(int generation, GCCollectionMode mode)
		{
			GC.Collect(generation, mode, true);
		}

		/// <summary>Forces a garbage collection from generation 0 through a specified generation, at a time specified by a <see cref="T:System.GCCollectionMode" /> value, with a value specifying whether the collection should be blocking.</summary>
		/// <param name="generation">The number of the oldest generation to be garbage collected.</param>
		/// <param name="mode">An enumeration value that specifies whether the garbage collection is forced (<see cref="F:System.GCCollectionMode.Default" /> or <see cref="F:System.GCCollectionMode.Forced" />) or optimized (<see cref="F:System.GCCollectionMode.Optimized" />).</param>
		/// <param name="blocking">
		///   <see langword="true" /> to perform a blocking garbage collection; <see langword="false" /> to perform a background garbage collection where possible.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="generation" /> is not valid.  
		/// -or-  
		/// <paramref name="mode" /> is not one of the <see cref="T:System.GCCollectionMode" /> values.</exception>
		[SecuritySafeCritical]
		public static void Collect(int generation, GCCollectionMode mode, bool blocking)
		{
			GC.Collect(generation, mode, blocking, false);
		}

		/// <summary>Forces a garbage collection from generation 0 through a specified generation, at a time specified by a <see cref="T:System.GCCollectionMode" /> value, with values that specify whether the collection should be blocking and compacting.</summary>
		/// <param name="generation">The number of the oldest generation to be garbage collected.</param>
		/// <param name="mode">An enumeration value that specifies whether the garbage collection is forced (<see cref="F:System.GCCollectionMode.Default" /> or <see cref="F:System.GCCollectionMode.Forced" />) or optimized (<see cref="F:System.GCCollectionMode.Optimized" />).</param>
		/// <param name="blocking">
		///   <see langword="true" /> to perform a blocking garbage collection; <see langword="false" /> to perform a background garbage collection where possible.</param>
		/// <param name="compacting">
		///   <see langword="true" /> to compact the small object heap; <see langword="false" /> to sweep only.</param>
		[SecuritySafeCritical]
		public static void Collect(int generation, GCCollectionMode mode, bool blocking, bool compacting)
		{
			if (generation < 0)
			{
				throw new ArgumentOutOfRangeException("generation", Environment.GetResourceString("Value must be positive."));
			}
			if (mode < GCCollectionMode.Default || mode > GCCollectionMode.Optimized)
			{
				throw new ArgumentOutOfRangeException("mode", Environment.GetResourceString("Enum value was out of legal range."));
			}
			int num = 0;
			if (mode == GCCollectionMode.Optimized)
			{
				num |= 4;
			}
			if (compacting)
			{
				num |= 8;
			}
			if (blocking)
			{
				num |= 2;
			}
			else if (!compacting)
			{
				num |= 1;
			}
			GC.InternalCollect(generation);
		}

		/// <summary>Returns the number of times garbage collection has occurred for the specified generation of objects.</summary>
		/// <param name="generation">The generation of objects for which the garbage collection count is to be determined.</param>
		/// <returns>The number of times garbage collection has occurred for the specified generation since the process was started.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="generation" /> is less than 0.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecuritySafeCritical]
		public static int CollectionCount(int generation)
		{
			if (generation < 0)
			{
				throw new ArgumentOutOfRangeException("generation", Environment.GetResourceString("Value must be positive."));
			}
			return GC.GetCollectionCount(generation);
		}

		/// <summary>References the specified object, which makes it ineligible for garbage collection from the start of the current routine to the point where this method is called.</summary>
		/// <param name="obj">The object to reference.</param>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void KeepAlive(object obj)
		{
		}

		/// <summary>Returns the current generation number of the target of a specified weak reference.</summary>
		/// <param name="wo">A <see cref="T:System.WeakReference" /> that refers to the target object whose generation number is to be determined.</param>
		/// <returns>The current generation number of the target of <paramref name="wo" />.</returns>
		/// <exception cref="T:System.ArgumentException">Garbage collection has already been performed on <paramref name="wo" />.</exception>
		[SecuritySafeCritical]
		public static int GetGeneration(WeakReference wo)
		{
			object target = wo.Target;
			if (target == null)
			{
				throw new ArgumentException();
			}
			return GC.GetGeneration(target);
		}

		/// <summary>Gets the maximum number of generations that the system currently supports.</summary>
		/// <returns>A value that ranges from zero to the maximum number of supported generations.</returns>
		public static int MaxGeneration
		{
			[SecuritySafeCritical]
			get
			{
				return GC.GetMaxGeneration();
			}
		}

		/// <summary>Suspends the current thread until the thread that is processing the queue of finalizers has emptied that queue.</summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WaitForPendingFinalizers();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void _SuppressFinalize(object o);

		/// <summary>Requests that the common language runtime not call the finalizer for the specified object.</summary>
		/// <param name="obj">The object whose finalizer must not be executed.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="obj" /> is <see langword="null" />.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecuritySafeCritical]
		public static void SuppressFinalize(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			GC._SuppressFinalize(obj);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void _ReRegisterForFinalize(object o);

		/// <summary>Requests that the system call the finalizer for the specified object for which <see cref="M:System.GC.SuppressFinalize(System.Object)" /> has previously been called.</summary>
		/// <param name="obj">The object that a finalizer must be called for.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="obj" /> is <see langword="null" />.</exception>
		[SecuritySafeCritical]
		public static void ReRegisterForFinalize(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			GC._ReRegisterForFinalize(obj);
		}

		/// <summary>Retrieves the number of bytes currently thought to be allocated. A parameter indicates whether this method can wait a short interval before returning, to allow the system to collect garbage and finalize objects.</summary>
		/// <param name="forceFullCollection">
		///   <see langword="true" /> to indicate that this method can wait for garbage collection to occur before returning; otherwise, <see langword="false" />.</param>
		/// <returns>A number that is the best available approximation of the number of bytes currently allocated in managed memory.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalMemory(bool forceFullCollection);

		private static bool _RegisterForFullGCNotification(int maxGenerationPercentage, int largeObjectHeapPercentage)
		{
			throw new NotImplementedException();
		}

		private static bool _CancelFullGCNotification()
		{
			throw new NotImplementedException();
		}

		private static int _WaitForFullGCApproach(int millisecondsTimeout)
		{
			throw new NotImplementedException();
		}

		private static int _WaitForFullGCComplete(int millisecondsTimeout)
		{
			throw new NotImplementedException();
		}

		/// <summary>Specifies that a garbage collection notification should be raised when conditions favor full garbage collection and when the collection has been completed.</summary>
		/// <param name="maxGenerationThreshold">A number between 1 and 99 that specifies when the notification should be raised based on the objects allocated in generation 2.</param>
		/// <param name="largeObjectHeapThreshold">A number between 1 and 99 that specifies when the notification should be raised based on objects allocated in the large object heap.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="maxGenerationThreshold" /> or <paramref name="largeObjectHeapThreshold" /> is not between 1 and 99.</exception>
		/// <exception cref="T:System.InvalidOperationException">This member is not available when concurrent garbage collection is enabled. See the &lt;gcConcurrent&gt; runtime setting for information about how to disable concurrent garbage collection.</exception>
		[SecurityCritical]
		public static void RegisterForFullGCNotification(int maxGenerationThreshold, int largeObjectHeapThreshold)
		{
			if (maxGenerationThreshold <= 0 || maxGenerationThreshold >= 100)
			{
				throw new ArgumentOutOfRangeException("maxGenerationThreshold", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument must be between {0} and {1}."), 1, 99));
			}
			if (largeObjectHeapThreshold <= 0 || largeObjectHeapThreshold >= 100)
			{
				throw new ArgumentOutOfRangeException("largeObjectHeapThreshold", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument must be between {0} and {1}."), 1, 99));
			}
			if (!GC._RegisterForFullGCNotification(maxGenerationThreshold, largeObjectHeapThreshold))
			{
				throw new InvalidOperationException(Environment.GetResourceString("This API is not available when the concurrent GC is enabled."));
			}
		}

		/// <summary>Cancels the registration of a garbage collection notification.</summary>
		/// <exception cref="T:System.InvalidOperationException">This member is not available when concurrent garbage collection is enabled. See the &lt;gcConcurrent&gt; runtime setting for information about how to disable concurrent garbage collection.</exception>
		[SecurityCritical]
		public static void CancelFullGCNotification()
		{
			if (!GC._CancelFullGCNotification())
			{
				throw new InvalidOperationException(Environment.GetResourceString("This API is not available when the concurrent GC is enabled."));
			}
		}

		/// <summary>Returns the status of a registered notification for determining whether a full, blocking garbage collection by the common language runtime is imminent.</summary>
		/// <returns>The status of the registered garbage collection notification.</returns>
		[SecurityCritical]
		public static GCNotificationStatus WaitForFullGCApproach()
		{
			return (GCNotificationStatus)GC._WaitForFullGCApproach(-1);
		}

		/// <summary>Returns, in a specified time-out period, the status of a registered notification for determining whether a full, blocking garbage collection by the common language runtime is imminent.</summary>
		/// <param name="millisecondsTimeout">The length of time to wait before a notification status can be obtained. Specify -1 to wait indefinitely.</param>
		/// <returns>The status of the registered garbage collection notification.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> must be either non-negative or less than or equal to <see cref="F:System.Int32.MaxValue" /> or -1.</exception>
		[SecurityCritical]
		public static GCNotificationStatus WaitForFullGCApproach(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return (GCNotificationStatus)GC._WaitForFullGCApproach(millisecondsTimeout);
		}

		/// <summary>Returns the status of a registered notification for determining whether a full, blocking garbage collection by the common language runtime has completed.</summary>
		/// <returns>The status of the registered garbage collection notification.</returns>
		[SecurityCritical]
		public static GCNotificationStatus WaitForFullGCComplete()
		{
			return (GCNotificationStatus)GC._WaitForFullGCComplete(-1);
		}

		/// <summary>Returns, in a specified time-out period, the status of a registered notification for determining whether a full, blocking garbage collection by common language the runtime has completed.</summary>
		/// <param name="millisecondsTimeout">The length of time to wait before a notification status can be obtained. Specify -1 to wait indefinitely.</param>
		/// <returns>The status of the registered garbage collection notification.</returns>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="millisecondsTimeout" /> must be either non-negative or less than or equal to <see cref="F:System.Int32.MaxValue" /> or -1.</exception>
		[SecurityCritical]
		public static GCNotificationStatus WaitForFullGCComplete(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", Environment.GetResourceString("Number must be either non-negative and less than or equal to Int32.MaxValue or -1."));
			}
			return (GCNotificationStatus)GC._WaitForFullGCComplete(millisecondsTimeout);
		}

		[SecurityCritical]
		private static bool StartNoGCRegionWorker(long totalSize, bool hasLohSize, long lohSize, bool disallowFullBlockingGC)
		{
			throw new NotImplementedException();
		}

		/// <summary>Attempts to disallow garbage collection during the execution of a critical path if a specified amount of memory is available.</summary>
		/// <param name="totalSize">The amount of memory in bytes to allocate without triggering a garbage collection. It must be less than or equal to the size of an ephemeral segment. For information on the size of an ephemeral segment, see the "Ephemeral generations and segments" section in the Fundamentals of Garbage Collection article.</param>
		/// <returns>
		///   <see langword="true" /> if the runtime was able to commit the required amount of memory and the garbage collector is able to enter no GC region latency mode; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="totalSize" /> exceeds the ephemeral segment size.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process is already in no GC region latency mode.</exception>
		[SecurityCritical]
		public static bool TryStartNoGCRegion(long totalSize)
		{
			return GC.StartNoGCRegionWorker(totalSize, false, 0L, false);
		}

		/// <summary>Attempts to disallow garbage collection during the execution of a critical path if a specified amount of memory is available for the large object heap and the small object heap.</summary>
		/// <param name="totalSize">The amount of memory in bytes to allocate without triggering a garbage collection. <paramref name="totalSize" /> -<paramref name="lohSize" /> must be less than or equal to the size of an ephemeral segment. For information on the size of an ephemeral segment, see the "Ephemeral generations and segments" section in the Fundamentals of Garbage Collection article.</param>
		/// <param name="lohSize">The number of bytes in <paramref name="totalSize" /> to use for large object heap (LOH) allocations.</param>
		/// <returns>
		///   <see langword="true" /> if the runtime was able to commit the required amount of memory and the garbage collector is able to enter no GC region latency mode; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="totalSize" /> - <paramref name="lohSize" /> exceeds the ephemeral segment size.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process is already in no GC region latency mode.</exception>
		[SecurityCritical]
		public static bool TryStartNoGCRegion(long totalSize, long lohSize)
		{
			return GC.StartNoGCRegionWorker(totalSize, true, lohSize, false);
		}

		/// <summary>Attempts to disallow garbage collection during the execution of a critical path if a specified amount of memory is available, and controls whether the garbage collector does a full blocking garbage collection if not enough memory is initially available.</summary>
		/// <param name="totalSize">The amount of memory in bytes to allocate without triggering a garbage collection. It must be less than or equal to the size of an ephemeral segment. For information on the size of an ephemeral segment, see the "Ephemeral generations and segments" section in the Fundamentals of Garbage Collection article.</param>
		/// <param name="disallowFullBlockingGC">
		///   <see langword="true" /> to omit a full blocking garbage collection if the garbage collector is initially unable to allocate <paramref name="totalSize" /> bytes; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the runtime was able to commit the required amount of memory and the garbage collector is able to enter no GC region latency mode; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="totalSize" /> exceeds the ephemeral segment size.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process is already in no GC region latency mode.</exception>
		[SecurityCritical]
		public static bool TryStartNoGCRegion(long totalSize, bool disallowFullBlockingGC)
		{
			return GC.StartNoGCRegionWorker(totalSize, false, 0L, disallowFullBlockingGC);
		}

		/// <summary>Attempts to disallow garbage collection during the execution of a critical path if a specified amount of memory is available for the large object heap and the small object heap, and controls whether the garbage collector does a full blocking garbage collection if not enough memory is initially available.</summary>
		/// <param name="totalSize">The amount of memory in bytes to allocate without triggering a garbage collection. <paramref name="totalSize" /> -<paramref name="lohSize" /> must be less than or equal to the size of an ephemeral segment. For information on the size of an ephemeral segment, see the "Ephemeral generations and segments" section in the Fundamentals of Garbage Collection article.</param>
		/// <param name="lohSize">The number of bytes in <paramref name="totalSize" /> to use for large object heap (LOH) allocations.</param>
		/// <param name="disallowFullBlockingGC">
		///   <see langword="true" /> to omit a full blocking garbage collection if the garbage collector is initially unable to allocate the specified memory on the small object heap (SOH) and LOH; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the runtime was able to commit the required amount of memory and the garbage collector is able to enter no GC region latency mode; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="totalSize" /> - <paramref name="lohSize" /> exceeds the ephemeral segment size.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process is already in no GC region latency mode.</exception>
		[SecurityCritical]
		public static bool TryStartNoGCRegion(long totalSize, long lohSize, bool disallowFullBlockingGC)
		{
			return GC.StartNoGCRegionWorker(totalSize, true, lohSize, disallowFullBlockingGC);
		}

		[SecurityCritical]
		private static GC.EndNoGCRegionStatus EndNoGCRegionWorker()
		{
			throw new NotImplementedException();
		}

		/// <summary>Ends the no GC region latency mode.</summary>
		/// <exception cref="T:System.InvalidOperationException">The garbage collector is not in no GC region latency mode.  
		///  -or-  
		///  The no GC region latency mode was ended previously because a garbage collection was induced.  
		///  -or-  
		///  A memory allocation exceeded the amount specified in the call to the <see cref="M:System.GC.TryStartNoGCRegion(System.Int64)" /> method.</exception>
		[SecurityCritical]
		public static void EndNoGCRegion()
		{
			GC.EndNoGCRegionWorker();
		}

		internal static readonly object EPHEMERON_TOMBSTONE = GC.get_ephemeron_tombstone();

		private enum StartNoGCRegionStatus
		{
			Succeeded,
			NotEnoughMemory,
			AmountTooLarge,
			AlreadyInProgress
		}

		private enum EndNoGCRegionStatus
		{
			Succeeded,
			NotInProgress,
			GCInduced,
			AllocationExceeded
		}
	}
}
