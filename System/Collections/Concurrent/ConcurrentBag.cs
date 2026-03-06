using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.Collections.Concurrent
{
	/// <summary>Represents a thread-safe, unordered collection of objects.</summary>
	/// <typeparam name="T">The type of the elements to be stored in the collection.</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(IProducerConsumerCollectionDebugView<>))]
	[Serializable]
	public class ConcurrentBag<T> : IProducerConsumerCollection<T>, IEnumerable<!0>, IEnumerable, ICollection, IReadOnlyCollection<T>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> class.</summary>
		public ConcurrentBag()
		{
			this._locals = new ThreadLocal<ConcurrentBag<T>.WorkStealingQueue>();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> class that contains elements copied from the specified collection.</summary>
		/// <param name="collection">The collection whose elements are copied to the new <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="collection" /> is a null reference (Nothing in Visual Basic).</exception>
		public ConcurrentBag(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection", "The collection argument is null.");
			}
			this._locals = new ThreadLocal<ConcurrentBag<T>.WorkStealingQueue>();
			ConcurrentBag<T>.WorkStealingQueue currentThreadWorkStealingQueue = this.GetCurrentThreadWorkStealingQueue(true);
			foreach (T item in collection)
			{
				currentThreadWorkStealingQueue.LocalPush(item, ref this._emptyToNonEmptyListTransitionCount);
			}
		}

		/// <summary>Adds an object to the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</summary>
		/// <param name="item">The object to be added to the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />. The value can be a null reference (Nothing in Visual Basic) for reference types.</param>
		public void Add(T item)
		{
			this.GetCurrentThreadWorkStealingQueue(true).LocalPush(item, ref this._emptyToNonEmptyListTransitionCount);
		}

		bool IProducerConsumerCollection<!0>.TryAdd(T item)
		{
			this.Add(item);
			return true;
		}

		/// <summary>Attempts to remove and return an object from the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</summary>
		/// <param name="result">When this method returns, <paramref name="result" /> contains the object removed from the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> or the default value of <typeparamref name="T" /> if the bag is empty.</param>
		/// <returns>true if an object was removed successfully; otherwise, false.</returns>
		public bool TryTake(out T result)
		{
			ConcurrentBag<T>.WorkStealingQueue currentThreadWorkStealingQueue = this.GetCurrentThreadWorkStealingQueue(false);
			return (currentThreadWorkStealingQueue != null && currentThreadWorkStealingQueue.TryLocalPop(out result)) || this.TrySteal(out result, true);
		}

		/// <summary>Attempts to return an object from the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> without removing it.</summary>
		/// <param name="result">When this method returns, <paramref name="result" /> contains an object from the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> or the default value of <typeparamref name="T" /> if the operation failed.</param>
		/// <returns>true if an object was returned successfully; otherwise, false.</returns>
		public bool TryPeek(out T result)
		{
			ConcurrentBag<T>.WorkStealingQueue currentThreadWorkStealingQueue = this.GetCurrentThreadWorkStealingQueue(false);
			return (currentThreadWorkStealingQueue != null && currentThreadWorkStealingQueue.TryLocalPeek(out result)) || this.TrySteal(out result, false);
		}

		private ConcurrentBag<T>.WorkStealingQueue GetCurrentThreadWorkStealingQueue(bool forceCreate)
		{
			ConcurrentBag<T>.WorkStealingQueue result;
			if ((result = this._locals.Value) == null)
			{
				if (!forceCreate)
				{
					return null;
				}
				result = this.CreateWorkStealingQueueForCurrentThread();
			}
			return result;
		}

		private ConcurrentBag<T>.WorkStealingQueue CreateWorkStealingQueueForCurrentThread()
		{
			object globalQueuesLock = this.GlobalQueuesLock;
			ConcurrentBag<T>.WorkStealingQueue result;
			lock (globalQueuesLock)
			{
				ConcurrentBag<T>.WorkStealingQueue workStealingQueues = this._workStealingQueues;
				ConcurrentBag<T>.WorkStealingQueue workStealingQueue = (workStealingQueues != null) ? this.GetUnownedWorkStealingQueue() : null;
				if (workStealingQueue == null)
				{
					workStealingQueue = (this._workStealingQueues = new ConcurrentBag<T>.WorkStealingQueue(workStealingQueues));
				}
				this._locals.Value = workStealingQueue;
				result = workStealingQueue;
			}
			return result;
		}

		private ConcurrentBag<T>.WorkStealingQueue GetUnownedWorkStealingQueue()
		{
			int currentManagedThreadId = Environment.CurrentManagedThreadId;
			for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue = this._workStealingQueues; workStealingQueue != null; workStealingQueue = workStealingQueue._nextQueue)
			{
				if (workStealingQueue._ownerThreadId == currentManagedThreadId)
				{
					return workStealingQueue;
				}
			}
			return null;
		}

		private bool TrySteal(out T result, bool take)
		{
			if (take)
			{
				CDSCollectionETWBCLProvider.Log.ConcurrentBag_TryTakeSteals();
			}
			else
			{
				CDSCollectionETWBCLProvider.Log.ConcurrentBag_TryPeekSteals();
			}
			for (;;)
			{
				long num = Interlocked.Read(ref this._emptyToNonEmptyListTransitionCount);
				ConcurrentBag<T>.WorkStealingQueue currentThreadWorkStealingQueue = this.GetCurrentThreadWorkStealingQueue(false);
				if ((currentThreadWorkStealingQueue == null) ? this.TryStealFromTo(this._workStealingQueues, null, out result, take) : (this.TryStealFromTo(currentThreadWorkStealingQueue._nextQueue, null, out result, take) || this.TryStealFromTo(this._workStealingQueues, currentThreadWorkStealingQueue, out result, take)))
				{
					break;
				}
				if (Interlocked.Read(ref this._emptyToNonEmptyListTransitionCount) == num)
				{
					return false;
				}
			}
			return true;
		}

		private bool TryStealFromTo(ConcurrentBag<T>.WorkStealingQueue startInclusive, ConcurrentBag<T>.WorkStealingQueue endExclusive, out T result, bool take)
		{
			for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue = startInclusive; workStealingQueue != endExclusive; workStealingQueue = workStealingQueue._nextQueue)
			{
				if (workStealingQueue.TrySteal(out result, take))
				{
					return true;
				}
			}
			result = default(T);
			return false;
		}

		/// <summary>Copies the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> elements to an existing one-dimensional <see cref="T:System.Array" />, starting at the specified array index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="index" /> is equal to or greater than the length of the <paramref name="array" /> -or- the number of elements in the source <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		public void CopyTo(T[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "The array argument is null.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "The index argument must be greater than or equal zero.");
			}
			if (this._workStealingQueues == null)
			{
				return;
			}
			bool lockTaken = false;
			try
			{
				this.FreezeBag(ref lockTaken);
				int dangerousCount = this.DangerousCount;
				if (index > array.Length - dangerousCount)
				{
					throw new ArgumentException("The number of elements in the collection is greater than the available space from index to the end of the destination array.", "index");
				}
				try
				{
					this.CopyFromEachQueueToArray(array, index);
				}
				catch (ArrayTypeMismatchException ex)
				{
					throw new InvalidCastException(ex.Message, ex);
				}
			}
			finally
			{
				this.UnfreezeBag(lockTaken);
			}
		}

		private int CopyFromEachQueueToArray(T[] array, int index)
		{
			int num = index;
			for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue = this._workStealingQueues; workStealingQueue != null; workStealingQueue = workStealingQueue._nextQueue)
			{
				num += workStealingQueue.DangerousCopyTo(array, num);
			}
			return num - index;
		}

		/// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional. -or- <paramref name="array" /> does not have zero-based indexing. -or- <paramref name="index" /> is equal to or greater than the length of the <paramref name="array" /> -or- The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />. -or- The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			T[] array2 = array as T[];
			if (array2 != null)
			{
				this.CopyTo(array2, index);
				return;
			}
			if (array == null)
			{
				throw new ArgumentNullException("array", "The array argument is null.");
			}
			this.ToArray().CopyTo(array, index);
		}

		/// <summary>Copies the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> elements to a new array.</summary>
		/// <returns>A new array containing a snapshot of elements copied from the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</returns>
		public T[] ToArray()
		{
			if (this._workStealingQueues != null)
			{
				bool lockTaken = false;
				try
				{
					this.FreezeBag(ref lockTaken);
					int dangerousCount = this.DangerousCount;
					if (dangerousCount > 0)
					{
						T[] array = new T[dangerousCount];
						this.CopyFromEachQueueToArray(array, 0);
						return array;
					}
				}
				finally
				{
					this.UnfreezeBag(lockTaken);
				}
			}
			return Array.Empty<T>();
		}

		public void Clear()
		{
			if (this._workStealingQueues == null)
			{
				return;
			}
			ConcurrentBag<T>.WorkStealingQueue currentThreadWorkStealingQueue = this.GetCurrentThreadWorkStealingQueue(false);
			if (currentThreadWorkStealingQueue != null)
			{
				currentThreadWorkStealingQueue.LocalClear();
				if (currentThreadWorkStealingQueue._nextQueue == null && currentThreadWorkStealingQueue == this._workStealingQueues)
				{
					return;
				}
			}
			bool lockTaken = false;
			try
			{
				this.FreezeBag(ref lockTaken);
				for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue = this._workStealingQueues; workStealingQueue != null; workStealingQueue = workStealingQueue._nextQueue)
				{
					T t;
					while (workStealingQueue.TrySteal(out t, true))
					{
					}
				}
			}
			finally
			{
				this.UnfreezeBag(lockTaken);
			}
		}

		/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</summary>
		/// <returns>An enumerator for the contents of the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return new ConcurrentBag<T>.Enumerator(this.ToArray());
		}

		/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</summary>
		/// <returns>An enumerator for the contents of the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</summary>
		/// <returns>The number of elements contained in the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</returns>
		public int Count
		{
			get
			{
				if (this._workStealingQueues == null)
				{
					return 0;
				}
				bool lockTaken = false;
				int dangerousCount;
				try
				{
					this.FreezeBag(ref lockTaken);
					dangerousCount = this.DangerousCount;
				}
				finally
				{
					this.UnfreezeBag(lockTaken);
				}
				return dangerousCount;
			}
		}

		private int DangerousCount
		{
			get
			{
				int num = 0;
				checked
				{
					for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue = this._workStealingQueues; workStealingQueue != null; workStealingQueue = workStealingQueue._nextQueue)
					{
						num += workStealingQueue.DangerousCount;
					}
					return num;
				}
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> is empty.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" /> is empty; otherwise, <see langword="false" />.</returns>
		public bool IsEmpty
		{
			get
			{
				ConcurrentBag<T>.WorkStealingQueue currentThreadWorkStealingQueue = this.GetCurrentThreadWorkStealingQueue(false);
				if (currentThreadWorkStealingQueue != null)
				{
					if (!currentThreadWorkStealingQueue.IsEmpty)
					{
						return false;
					}
					if (currentThreadWorkStealingQueue._nextQueue == null && currentThreadWorkStealingQueue == this._workStealingQueues)
					{
						return true;
					}
				}
				bool lockTaken = false;
				try
				{
					this.FreezeBag(ref lockTaken);
					for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue = this._workStealingQueues; workStealingQueue != null; workStealingQueue = workStealingQueue._nextQueue)
					{
						if (!workStealingQueue.IsEmpty)
						{
							return false;
						}
					}
				}
				finally
				{
					this.UnfreezeBag(lockTaken);
				}
				return true;
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized with the SyncRoot.</summary>
		/// <returns>Always returns <see langword="false" /> to indicate access is not synchronized.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />. This property is not supported.</summary>
		/// <returns>Returns null  (Nothing in Visual Basic).</returns>
		/// <exception cref="T:System.NotSupportedException">The SyncRoot property is not supported.</exception>
		object ICollection.SyncRoot
		{
			get
			{
				throw new NotSupportedException("The SyncRoot property may not be used for the synchronization of concurrent collections.");
			}
		}

		private object GlobalQueuesLock
		{
			get
			{
				return this._locals;
			}
		}

		private void FreezeBag(ref bool lockTaken)
		{
			Monitor.Enter(this.GlobalQueuesLock, ref lockTaken);
			ConcurrentBag<T>.WorkStealingQueue workStealingQueues = this._workStealingQueues;
			for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue = workStealingQueues; workStealingQueue != null; workStealingQueue = workStealingQueue._nextQueue)
			{
				Monitor.Enter(workStealingQueue, ref workStealingQueue._frozen);
			}
			Interlocked.MemoryBarrier();
			for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue2 = workStealingQueues; workStealingQueue2 != null; workStealingQueue2 = workStealingQueue2._nextQueue)
			{
				if (workStealingQueue2._currentOp != 0)
				{
					SpinWait spinWait = default(SpinWait);
					do
					{
						spinWait.SpinOnce();
					}
					while (workStealingQueue2._currentOp != 0);
				}
			}
		}

		private void UnfreezeBag(bool lockTaken)
		{
			if (lockTaken)
			{
				for (ConcurrentBag<T>.WorkStealingQueue workStealingQueue = this._workStealingQueues; workStealingQueue != null; workStealingQueue = workStealingQueue._nextQueue)
				{
					if (workStealingQueue._frozen)
					{
						workStealingQueue._frozen = false;
						Monitor.Exit(workStealingQueue);
					}
				}
				Monitor.Exit(this.GlobalQueuesLock);
			}
		}

		private readonly ThreadLocal<ConcurrentBag<T>.WorkStealingQueue> _locals;

		private volatile ConcurrentBag<T>.WorkStealingQueue _workStealingQueues;

		private long _emptyToNonEmptyListTransitionCount;

		private sealed class WorkStealingQueue
		{
			internal WorkStealingQueue(ConcurrentBag<T>.WorkStealingQueue nextQueue)
			{
				this._ownerThreadId = Environment.CurrentManagedThreadId;
				this._nextQueue = nextQueue;
			}

			internal bool IsEmpty
			{
				get
				{
					return this._headIndex >= this._tailIndex;
				}
			}

			internal void LocalPush(T item, ref long emptyToNonEmptyListTransitionCount)
			{
				bool flag = false;
				try
				{
					Interlocked.Exchange(ref this._currentOp, 1);
					int num = this._tailIndex;
					if (num == 2147483647)
					{
						this._currentOp = 0;
						lock (this)
						{
							this._headIndex &= this._mask;
							num = (this._tailIndex = (num & this._mask));
							Interlocked.Exchange(ref this._currentOp, 1);
						}
					}
					int headIndex = this._headIndex;
					if (!this._frozen && (headIndex < num - 1 & num < headIndex + this._mask))
					{
						this._array[num & this._mask] = item;
						this._tailIndex = num + 1;
					}
					else
					{
						this._currentOp = 0;
						Monitor.Enter(this, ref flag);
						headIndex = this._headIndex;
						int num2 = num - headIndex;
						if (num2 >= this._mask)
						{
							T[] array = new T[this._array.Length << 1];
							int num3 = headIndex & this._mask;
							if (num3 == 0)
							{
								Array.Copy(this._array, 0, array, 0, this._array.Length);
							}
							else
							{
								Array.Copy(this._array, num3, array, 0, this._array.Length - num3);
								Array.Copy(this._array, 0, array, this._array.Length - num3, num3);
							}
							this._array = array;
							this._headIndex = 0;
							num = (this._tailIndex = num2);
							this._mask = (this._mask << 1 | 1);
						}
						this._array[num & this._mask] = item;
						this._tailIndex = num + 1;
						if (num2 == 0)
						{
							Interlocked.Increment(ref emptyToNonEmptyListTransitionCount);
						}
						this._addTakeCount -= this._stealCount;
						this._stealCount = 0;
					}
					checked
					{
						this._addTakeCount++;
					}
				}
				finally
				{
					this._currentOp = 0;
					if (flag)
					{
						Monitor.Exit(this);
					}
				}
			}

			internal void LocalClear()
			{
				lock (this)
				{
					if (this._headIndex < this._tailIndex)
					{
						this._headIndex = (this._tailIndex = 0);
						this._addTakeCount = (this._stealCount = 0);
						Array.Clear(this._array, 0, this._array.Length);
					}
				}
			}

			internal bool TryLocalPop(out T result)
			{
				int num = this._tailIndex;
				if (this._headIndex >= num)
				{
					result = default(T);
					return false;
				}
				bool flag = false;
				bool result2;
				try
				{
					this._currentOp = 2;
					Interlocked.Exchange(ref this._tailIndex, --num);
					if (!this._frozen && this._headIndex < num)
					{
						int num2 = num & this._mask;
						result = this._array[num2];
						this._array[num2] = default(T);
						this._addTakeCount--;
						result2 = true;
					}
					else
					{
						this._currentOp = 0;
						Monitor.Enter(this, ref flag);
						if (this._headIndex <= num)
						{
							int num3 = num & this._mask;
							result = this._array[num3];
							this._array[num3] = default(T);
							this._addTakeCount--;
							result2 = true;
						}
						else
						{
							this._tailIndex = num + 1;
							result = default(T);
							result2 = false;
						}
					}
				}
				finally
				{
					this._currentOp = 0;
					if (flag)
					{
						Monitor.Exit(this);
					}
				}
				return result2;
			}

			internal bool TryLocalPeek(out T result)
			{
				int tailIndex = this._tailIndex;
				if (this._headIndex < tailIndex)
				{
					lock (this)
					{
						if (this._headIndex < tailIndex)
						{
							result = this._array[tailIndex - 1 & this._mask];
							return true;
						}
					}
				}
				result = default(T);
				return false;
			}

			internal bool TrySteal(out T result, bool take)
			{
				lock (this)
				{
					int headIndex = this._headIndex;
					if (take)
					{
						if (headIndex < this._tailIndex - 1 && this._currentOp != 1)
						{
							SpinWait spinWait = default(SpinWait);
							do
							{
								spinWait.SpinOnce();
							}
							while (this._currentOp == 1);
						}
						Interlocked.Exchange(ref this._headIndex, headIndex + 1);
						if (headIndex < this._tailIndex)
						{
							int num = headIndex & this._mask;
							result = this._array[num];
							this._array[num] = default(T);
							this._stealCount++;
							return true;
						}
						this._headIndex = headIndex;
					}
					else if (headIndex < this._tailIndex)
					{
						result = this._array[headIndex & this._mask];
						return true;
					}
				}
				result = default(T);
				return false;
			}

			internal int DangerousCopyTo(T[] array, int arrayIndex)
			{
				int headIndex = this._headIndex;
				int dangerousCount = this.DangerousCount;
				for (int i = arrayIndex + dangerousCount - 1; i >= arrayIndex; i--)
				{
					array[i] = this._array[headIndex++ & this._mask];
				}
				return dangerousCount;
			}

			internal int DangerousCount
			{
				get
				{
					return this._addTakeCount - this._stealCount;
				}
			}

			private const int InitialSize = 32;

			private const int StartIndex = 0;

			private volatile int _headIndex;

			private volatile int _tailIndex;

			private volatile T[] _array = new T[32];

			private volatile int _mask = 31;

			private int _addTakeCount;

			private int _stealCount;

			internal volatile int _currentOp;

			internal bool _frozen;

			internal readonly ConcurrentBag<T>.WorkStealingQueue _nextQueue;

			internal readonly int _ownerThreadId;
		}

		internal enum Operation
		{
			None,
			Add,
			Take
		}

		[Serializable]
		private sealed class Enumerator : IEnumerator<!0>, IDisposable, IEnumerator
		{
			public Enumerator(T[] array)
			{
				this._array = array;
			}

			public bool MoveNext()
			{
				if (this._index < this._array.Length)
				{
					T[] array = this._array;
					int index = this._index;
					this._index = index + 1;
					this._current = array[index];
					return true;
				}
				this._index = this._array.Length + 1;
				return false;
			}

			public T Current
			{
				get
				{
					return this._current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (this._index == 0 || this._index == this._array.Length + 1)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this.Current;
				}
			}

			public void Reset()
			{
				this._index = 0;
				this._current = default(T);
			}

			public void Dispose()
			{
			}

			private readonly T[] _array;

			private T _current;

			private int _index;
		}
	}
}
