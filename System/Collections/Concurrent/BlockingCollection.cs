using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace System.Collections.Concurrent
{
	/// <summary>Provides blocking and bounding capabilities for thread-safe collections that implement <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1" />.</summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	[DebuggerTypeProxy(typeof(BlockingCollectionDebugView<>))]
	[DebuggerDisplay("Count = {Count}, Type = {_collection}")]
	public class BlockingCollection<T> : IEnumerable<!0>, IEnumerable, ICollection, IDisposable, IReadOnlyCollection<T>
	{
		/// <summary>Gets the bounded capacity of this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</summary>
		/// <returns>The bounded capacity of this collection, or int.MaxValue if no bound was supplied.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		public int BoundedCapacity
		{
			get
			{
				this.CheckDisposed();
				return this._boundedCapacity;
			}
		}

		/// <summary>Gets whether this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been marked as complete for adding.</summary>
		/// <returns>Whether this collection has been marked as complete for adding.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		public bool IsAddingCompleted
		{
			get
			{
				this.CheckDisposed();
				return this._currentAdders == int.MinValue;
			}
		}

		/// <summary>Gets whether this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been marked as complete for adding and is empty.</summary>
		/// <returns>Whether this collection has been marked as complete for adding and is empty.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		public bool IsCompleted
		{
			get
			{
				this.CheckDisposed();
				return this.IsAddingCompleted && this._occupiedNodes.CurrentCount == 0;
			}
		}

		/// <summary>Gets the number of items contained in the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</summary>
		/// <returns>The number of items contained in the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		public int Count
		{
			get
			{
				this.CheckDisposed();
				return this._occupiedNodes.CurrentCount;
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
		/// <returns>Always returns <see langword="false" /> to indicate the access is not synchronized.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		bool ICollection.IsSynchronized
		{
			get
			{
				this.CheckDisposed();
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />. This property is not supported.</summary>
		/// <returns>returns null.</returns>
		/// <exception cref="T:System.NotSupportedException">The SyncRoot property is not supported.</exception>
		object ICollection.SyncRoot
		{
			get
			{
				throw new NotSupportedException("The SyncRoot property may not be used for the synchronization of concurrent collections.");
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> class without an upper-bound.</summary>
		public BlockingCollection() : this(new ConcurrentQueue<T>())
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> class with the specified upper-bound.</summary>
		/// <param name="boundedCapacity">The bounded size of the collection.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="boundedCapacity" /> is not a positive value.</exception>
		public BlockingCollection(int boundedCapacity) : this(new ConcurrentQueue<T>(), boundedCapacity)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> class with the specified upper-bound and using the provided <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1" /> as its underlying data store.</summary>
		/// <param name="collection">The collection to use as the underlying data store.</param>
		/// <param name="boundedCapacity">The bounded size of the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collection" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="boundedCapacity" /> is not a positive value.</exception>
		/// <exception cref="T:System.ArgumentException">The supplied <paramref name="collection" /> contains more values than is permitted by <paramref name="boundedCapacity" />.</exception>
		public BlockingCollection(IProducerConsumerCollection<T> collection, int boundedCapacity)
		{
			if (boundedCapacity < 1)
			{
				throw new ArgumentOutOfRangeException("boundedCapacity", boundedCapacity, "The boundedCapacity argument must be positive.");
			}
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			int count = collection.Count;
			if (count > boundedCapacity)
			{
				throw new ArgumentException("The collection argument contains more items than are allowed by the boundedCapacity.");
			}
			this.Initialize(collection, boundedCapacity, count);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> class without an upper-bound and using the provided <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1" /> as its underlying data store.</summary>
		/// <param name="collection">The collection to use as the underlying data store.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collection" /> argument is null.</exception>
		public BlockingCollection(IProducerConsumerCollection<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this.Initialize(collection, -1, collection.Count);
		}

		private void Initialize(IProducerConsumerCollection<T> collection, int boundedCapacity, int collectionCount)
		{
			this._collection = collection;
			this._boundedCapacity = boundedCapacity;
			this._isDisposed = false;
			this._consumersCancellationTokenSource = new CancellationTokenSource();
			this._producersCancellationTokenSource = new CancellationTokenSource();
			if (boundedCapacity == -1)
			{
				this._freeNodes = null;
			}
			else
			{
				this._freeNodes = new SemaphoreSlim(boundedCapacity - collectionCount);
			}
			this._occupiedNodes = new SemaphoreSlim(collectionCount);
		}

		/// <summary>Adds the item to the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</summary>
		/// <param name="item">The item to be added to the collection. The value can be a null reference.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been marked as complete with regards to additions.  
		///  -or-  
		///  The underlying collection didn't accept the item.</exception>
		public void Add(T item)
		{
			this.TryAddWithNoTimeValidation(item, -1, default(CancellationToken));
		}

		/// <summary>Adds the item to the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</summary>
		/// <param name="item">The item to be added to the collection. The value can be a null reference.</param>
		/// <param name="cancellationToken">A cancellation token to observe.</param>
		/// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed or the <see cref="T:System.Threading.CancellationTokenSource" /> that owns <paramref name="cancellationToken" /> has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been marked as complete with regards to additions.  
		///  -or-  
		///  The underlying collection didn't accept the item.</exception>
		public void Add(T item, CancellationToken cancellationToken)
		{
			this.TryAddWithNoTimeValidation(item, -1, cancellationToken);
		}

		/// <summary>Tries to add the specified item to the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</summary>
		/// <param name="item">The item to be added to the collection.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="item" /> could be added; otherwise, <see langword="false" />. If the item is a duplicate, and the underlying collection does not accept duplicate items, then an <see cref="T:System.InvalidOperationException" /> is thrown.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been marked as complete with regards to additions.  
		///  -or-  
		///  The underlying collection didn't accept the item.</exception>
		public bool TryAdd(T item)
		{
			return this.TryAddWithNoTimeValidation(item, 0, default(CancellationToken));
		}

		/// <summary>Tries to add the specified item to the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</summary>
		/// <param name="item">The item to be added to the collection.</param>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <returns>true if the <paramref name="item" /> could be added to the collection within the specified time span; otherwise, false.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out -or- timeout is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been marked as complete with regards to additions.  
		///  -or-  
		///  The underlying collection didn't accept the item.</exception>
		public bool TryAdd(T item, TimeSpan timeout)
		{
			BlockingCollection<T>.ValidateTimeout(timeout);
			return this.TryAddWithNoTimeValidation(item, (int)timeout.TotalMilliseconds, default(CancellationToken));
		}

		/// <summary>Tries to add the specified item to the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> within the specified time period.</summary>
		/// <param name="item">The item to be added to the collection.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <returns>true if the <paramref name="item" /> could be added to the collection within the specified time; otherwise, false. If the item is a duplicate, and the underlying collection does not accept duplicate items, then an <see cref="T:System.InvalidOperationException" /> is thrown.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been marked as complete with regards to additions.  
		///  -or-  
		///  The underlying collection didn't accept the item.</exception>
		public bool TryAdd(T item, int millisecondsTimeout)
		{
			BlockingCollection<T>.ValidateMillisecondsTimeout(millisecondsTimeout);
			return this.TryAddWithNoTimeValidation(item, millisecondsTimeout, default(CancellationToken));
		}

		/// <summary>Tries to add the specified item to the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> within the specified time period, while observing a cancellation token.</summary>
		/// <param name="item">The item to be added to the collection.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <param name="cancellationToken">A cancellation token to observe.</param>
		/// <returns>true if the <paramref name="item" /> could be added to the collection within the specified time; otherwise, false. If the item is a duplicate, and the underlying collection does not accept duplicate items, then an <see cref="T:System.InvalidOperationException" /> is thrown.</returns>
		/// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed or the underlying <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been marked as complete with regards to additions.  
		///  -or-  
		///  The underlying collection didn't accept the item.</exception>
		public bool TryAdd(T item, int millisecondsTimeout, CancellationToken cancellationToken)
		{
			BlockingCollection<T>.ValidateMillisecondsTimeout(millisecondsTimeout);
			return this.TryAddWithNoTimeValidation(item, millisecondsTimeout, cancellationToken);
		}

		private bool TryAddWithNoTimeValidation(T item, int millisecondsTimeout, CancellationToken cancellationToken)
		{
			this.CheckDisposed();
			if (cancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException("The operation was canceled.", cancellationToken);
			}
			if (this.IsAddingCompleted)
			{
				throw new InvalidOperationException("The collection has been marked as complete with regards to additions.");
			}
			bool flag = true;
			if (this._freeNodes != null)
			{
				CancellationTokenSource cancellationTokenSource = null;
				try
				{
					flag = this._freeNodes.Wait(0);
					if (!flag && millisecondsTimeout != 0)
					{
						cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._producersCancellationTokenSource.Token);
						flag = this._freeNodes.Wait(millisecondsTimeout, cancellationTokenSource.Token);
					}
				}
				catch (OperationCanceledException)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						throw new OperationCanceledException("The operation was canceled.", cancellationToken);
					}
					throw new InvalidOperationException("CompleteAdding may not be used concurrently with additions to the collection.");
				}
				finally
				{
					if (cancellationTokenSource != null)
					{
						cancellationTokenSource.Dispose();
					}
				}
			}
			if (flag)
			{
				SpinWait spinWait = default(SpinWait);
				for (;;)
				{
					int currentAdders = this._currentAdders;
					if ((currentAdders & -2147483648) != 0)
					{
						break;
					}
					if (Interlocked.CompareExchange(ref this._currentAdders, currentAdders + 1, currentAdders) == currentAdders)
					{
						goto IL_104;
					}
					spinWait.SpinOnce();
				}
				spinWait.Reset();
				while (this._currentAdders != -2147483648)
				{
					spinWait.SpinOnce();
				}
				throw new InvalidOperationException("The collection has been marked as complete with regards to additions.");
				IL_104:
				try
				{
					bool flag2 = false;
					try
					{
						cancellationToken.ThrowIfCancellationRequested();
						flag2 = this._collection.TryAdd(item);
					}
					catch
					{
						if (this._freeNodes != null)
						{
							this._freeNodes.Release();
						}
						throw;
					}
					if (!flag2)
					{
						throw new InvalidOperationException("The underlying collection didn't accept the item.");
					}
					this._occupiedNodes.Release();
				}
				finally
				{
					Interlocked.Decrement(ref this._currentAdders);
				}
			}
			return flag;
		}

		/// <summary>Removes  an item from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</summary>
		/// <returns>The item removed from the collection.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying collection was modified outside of this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance, or the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> is empty and the collection has been marked as complete for adding.</exception>
		public T Take()
		{
			T result;
			if (!this.TryTake(out result, -1, CancellationToken.None))
			{
				throw new InvalidOperationException("The collection argument is empty and has been marked as complete with regards to additions.");
			}
			return result;
		}

		/// <summary>Removes an item from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</summary>
		/// <param name="cancellationToken">Object that can be used to cancel the take operation.</param>
		/// <returns>The item removed from the collection.</returns>
		/// <exception cref="T:System.OperationCanceledException">The <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed or the <see cref="T:System.Threading.CancellationTokenSource" /> that created the token was canceled.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying collection was modified outside of this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance or the BlockingCollection is marked as complete for adding, or the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> is empty.</exception>
		public T Take(CancellationToken cancellationToken)
		{
			T result;
			if (!this.TryTake(out result, -1, cancellationToken))
			{
				throw new InvalidOperationException("The collection argument is empty and has been marked as complete with regards to additions.");
			}
			return result;
		}

		/// <summary>Tries to remove an item from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" />.</summary>
		/// <param name="item">The item to be removed from the collection.</param>
		/// <returns>
		///   <see langword="true" /> if an item could be removed; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying collection was modified outside of this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		public bool TryTake(out T item)
		{
			return this.TryTake(out item, 0, CancellationToken.None);
		}

		/// <summary>Tries to remove an item from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> in the specified time period.</summary>
		/// <param name="item">The item to be removed from the collection.</param>
		/// <param name="timeout">An object that represents the number of milliseconds to wait, or an object that represents -1 milliseconds to wait indefinitely.</param>
		/// <returns>
		///   <see langword="true" /> if an item could be removed from the collection within the specified  time; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out.  
		/// -or-  
		/// <paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying collection was modified outside of this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		public bool TryTake(out T item, TimeSpan timeout)
		{
			BlockingCollection<T>.ValidateTimeout(timeout);
			return this.TryTakeWithNoTimeValidation(out item, (int)timeout.TotalMilliseconds, CancellationToken.None, null);
		}

		/// <summary>Tries to remove an item from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> in the specified time period.</summary>
		/// <param name="item">The item to be removed from the collection.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <returns>
		///   <see langword="true" /> if an item could be removed from the collection within the specified  time; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying collection was modified outside of this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		public bool TryTake(out T item, int millisecondsTimeout)
		{
			BlockingCollection<T>.ValidateMillisecondsTimeout(millisecondsTimeout);
			return this.TryTakeWithNoTimeValidation(out item, millisecondsTimeout, CancellationToken.None, null);
		}

		/// <summary>Tries to remove an item from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> in the specified time period while observing a cancellation token.</summary>
		/// <param name="item">The item to be removed from the collection.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <param name="cancellationToken">A cancellation token to observe.</param>
		/// <returns>
		///   <see langword="true" /> if an item could be removed from the collection within the specified  time; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.OperationCanceledException">The <see cref="T:System.Threading.CancellationToken" /> has been canceled.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed or the underlying <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying collection was modified outside this <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		public bool TryTake(out T item, int millisecondsTimeout, CancellationToken cancellationToken)
		{
			BlockingCollection<T>.ValidateMillisecondsTimeout(millisecondsTimeout);
			return this.TryTakeWithNoTimeValidation(out item, millisecondsTimeout, cancellationToken, null);
		}

		private bool TryTakeWithNoTimeValidation(out T item, int millisecondsTimeout, CancellationToken cancellationToken, CancellationTokenSource combinedTokenSource)
		{
			this.CheckDisposed();
			item = default(T);
			if (cancellationToken.IsCancellationRequested)
			{
				throw new OperationCanceledException("The operation was canceled.", cancellationToken);
			}
			if (this.IsCompleted)
			{
				return false;
			}
			bool flag = false;
			CancellationTokenSource cancellationTokenSource = combinedTokenSource;
			try
			{
				flag = this._occupiedNodes.Wait(0);
				if (!flag && millisecondsTimeout != 0)
				{
					if (combinedTokenSource == null)
					{
						cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._consumersCancellationTokenSource.Token);
					}
					flag = this._occupiedNodes.Wait(millisecondsTimeout, cancellationTokenSource.Token);
				}
			}
			catch (OperationCanceledException)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					throw new OperationCanceledException("The operation was canceled.", cancellationToken);
				}
				return false;
			}
			finally
			{
				if (cancellationTokenSource != null && combinedTokenSource == null)
				{
					cancellationTokenSource.Dispose();
				}
			}
			if (flag)
			{
				bool flag2 = false;
				bool flag3 = true;
				try
				{
					cancellationToken.ThrowIfCancellationRequested();
					flag2 = this._collection.TryTake(out item);
					flag3 = false;
					if (!flag2)
					{
						throw new InvalidOperationException("The underlying collection was modified from outside of the BlockingCollection<T>.");
					}
				}
				finally
				{
					if (flag2)
					{
						if (this._freeNodes != null)
						{
							this._freeNodes.Release();
						}
					}
					else if (flag3)
					{
						this._occupiedNodes.Release();
					}
					if (this.IsCompleted)
					{
						this.CancelWaitingConsumers();
					}
				}
			}
			return flag;
		}

		/// <summary>Adds the specified item to any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item to be added to one of the collections.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array to which the item was added.</returns>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element, or at least one of collections has been marked as complete for adding.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one underlying collection didn't accept the item.</exception>
		public static int AddToAny(BlockingCollection<T>[] collections, T item)
		{
			return BlockingCollection<T>.TryAddToAny(collections, item, -1, CancellationToken.None);
		}

		/// <summary>Adds the specified item to any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item to be added to one of the collections.</param>
		/// <param name="cancellationToken">A cancellation token to observe.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array to which the item was added.</returns>
		/// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one underlying collection didn't accept the item.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element, or at least one of collections has been marked as complete for adding.</exception>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed, or the <see cref="T:System.Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has been disposed.</exception>
		public static int AddToAny(BlockingCollection<T>[] collections, T item, CancellationToken cancellationToken)
		{
			return BlockingCollection<T>.TryAddToAny(collections, item, -1, cancellationToken);
		}

		/// <summary>Tries to add the specified item to any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item to be added to one of the collections.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array to which the item was added, or -1 if the item could not be added.</returns>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element, or at least one of collections has been marked as complete for adding.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one underlying collection didn't accept the item.</exception>
		public static int TryAddToAny(BlockingCollection<T>[] collections, T item)
		{
			return BlockingCollection<T>.TryAddToAny(collections, item, 0, CancellationToken.None);
		}

		/// <summary>Tries to add the specified item to any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances while observing the specified cancellation token.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item to be added to one of the collections.</param>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array to which the item was added, or -1 if the item could not be added.</returns>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances or the <see cref="T:System.Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out -or- timeout is greater than <see cref="F:System.Int32.MaxValue" />.  
		/// -or-  
		/// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element, or at least one of collections has been marked as complete for adding.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one underlying collection didn't accept the item.</exception>
		public static int TryAddToAny(BlockingCollection<T>[] collections, T item, TimeSpan timeout)
		{
			BlockingCollection<T>.ValidateTimeout(timeout);
			return BlockingCollection<T>.TryAddToAnyCore(collections, item, (int)timeout.TotalMilliseconds, CancellationToken.None);
		}

		/// <summary>Tries to add the specified item to any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item to be added to one of the collections.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array to which the item was added, or -1 if the item could not be added.</returns>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.  
		/// -or-  
		/// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element, or at least one of collections has been marked as complete for adding.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one underlying collection didn't accept the item.</exception>
		public static int TryAddToAny(BlockingCollection<T>[] collections, T item, int millisecondsTimeout)
		{
			BlockingCollection<T>.ValidateMillisecondsTimeout(millisecondsTimeout);
			return BlockingCollection<T>.TryAddToAnyCore(collections, item, millisecondsTimeout, CancellationToken.None);
		}

		/// <summary>Tries to add the specified item to any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item to be added to one of the collections.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <param name="cancellationToken">A cancellation token to observe.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array to which the item was added, or -1 if the item could not be added.</returns>
		/// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one underlying collection didn't accept the item.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.  
		/// -or-  
		/// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element, or at least one of collections has been marked as complete for adding.</exception>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		public static int TryAddToAny(BlockingCollection<T>[] collections, T item, int millisecondsTimeout, CancellationToken cancellationToken)
		{
			BlockingCollection<T>.ValidateMillisecondsTimeout(millisecondsTimeout);
			return BlockingCollection<T>.TryAddToAnyCore(collections, item, millisecondsTimeout, cancellationToken);
		}

		private static int TryAddToAnyCore(BlockingCollection<T>[] collections, T item, int millisecondsTimeout, CancellationToken externalCancellationToken)
		{
			BlockingCollection<T>.ValidateCollectionsArray(collections, true);
			int num = millisecondsTimeout;
			uint startTime = 0U;
			if (millisecondsTimeout != -1)
			{
				startTime = (uint)Environment.TickCount;
			}
			int num2 = BlockingCollection<T>.TryAddToAnyFast(collections, item);
			if (num2 > -1)
			{
				return num2;
			}
			CancellationToken[] tokens;
			List<WaitHandle> handles = BlockingCollection<T>.GetHandles(collections, externalCancellationToken, true, out tokens);
			while (millisecondsTimeout == -1 || num >= 0)
			{
				num2 = -1;
				using (CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokens))
				{
					handles.Add(cancellationTokenSource.Token.WaitHandle);
					num2 = WaitHandle.WaitAny(handles.ToArray(), num);
					handles.RemoveAt(handles.Count - 1);
					if (cancellationTokenSource.IsCancellationRequested)
					{
						if (externalCancellationToken.IsCancellationRequested)
						{
							throw new OperationCanceledException("The operation was canceled.", externalCancellationToken);
						}
						throw new ArgumentException("At least one of the specified collections is marked as complete with regards to additions.", "collections");
					}
				}
				if (num2 == 258)
				{
					return -1;
				}
				if (collections[num2].TryAdd(item))
				{
					return num2;
				}
				if (millisecondsTimeout != -1)
				{
					num = BlockingCollection<T>.UpdateTimeOut(startTime, millisecondsTimeout);
				}
			}
			return -1;
		}

		private static int TryAddToAnyFast(BlockingCollection<T>[] collections, T item)
		{
			for (int i = 0; i < collections.Length; i++)
			{
				if (collections[i]._freeNodes == null)
				{
					collections[i].TryAdd(item);
					return i;
				}
			}
			return -1;
		}

		private static List<WaitHandle> GetHandles(BlockingCollection<T>[] collections, CancellationToken externalCancellationToken, bool isAddOperation, out CancellationToken[] cancellationTokens)
		{
			List<WaitHandle> list = new List<WaitHandle>(collections.Length + 1);
			List<CancellationToken> list2 = new List<CancellationToken>(collections.Length + 1);
			list2.Add(externalCancellationToken);
			if (isAddOperation)
			{
				for (int i = 0; i < collections.Length; i++)
				{
					if (collections[i]._freeNodes != null)
					{
						list.Add(collections[i]._freeNodes.AvailableWaitHandle);
						list2.Add(collections[i]._producersCancellationTokenSource.Token);
					}
				}
			}
			else
			{
				for (int j = 0; j < collections.Length; j++)
				{
					if (!collections[j].IsCompleted)
					{
						list.Add(collections[j]._occupiedNodes.AvailableWaitHandle);
						list2.Add(collections[j]._consumersCancellationTokenSource.Token);
					}
				}
			}
			cancellationTokens = list2.ToArray();
			return list;
		}

		private static int UpdateTimeOut(uint startTime, int originalWaitMillisecondsTimeout)
		{
			if (originalWaitMillisecondsTimeout == 0)
			{
				return 0;
			}
			uint num = (uint)(Environment.TickCount - (int)startTime);
			if (num > 2147483647U)
			{
				return 0;
			}
			int num2 = originalWaitMillisecondsTimeout - (int)num;
			if (num2 <= 0)
			{
				return 0;
			}
			return num2;
		}

		/// <summary>Takes an item from any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item removed from one of the collections.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array from which the item was removed.</returns>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element or <see cref="M:System.Collections.Concurrent.BlockingCollection`1.CompleteAdding" /> has been called on the collection.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one of the underlying collections was modified outside of its <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		public static int TakeFromAny(BlockingCollection<T>[] collections, out T item)
		{
			return BlockingCollection<T>.TakeFromAny(collections, out item, CancellationToken.None);
		}

		/// <summary>Takes an item from any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances while observing the specified cancellation token.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item removed from one of the collections.</param>
		/// <param name="cancellationToken">A cancellation token to observe.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array from which the item was removed.</returns>
		/// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one of the underlying collections was modified outside of its <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element, or <see cref="M:System.Collections.Concurrent.BlockingCollection`1.CompleteAdding" /> has been called on the collection.</exception>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		public static int TakeFromAny(BlockingCollection<T>[] collections, out T item, CancellationToken cancellationToken)
		{
			return BlockingCollection<T>.TryTakeFromAnyCore(collections, out item, -1, true, cancellationToken);
		}

		/// <summary>Tries to remove an item from any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item removed from one of the collections.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array from which the item was removed, or -1 if an item could not be removed.</returns>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one of the underlying collections was modified outside of its <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		public static int TryTakeFromAny(BlockingCollection<T>[] collections, out T item)
		{
			return BlockingCollection<T>.TryTakeFromAny(collections, out item, 0);
		}

		/// <summary>Tries to remove an item from any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item removed from one of the collections.</param>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array from which the item was removed, or -1 if an item could not be removed.</returns>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out -or- timeout is greater than <see cref="F:System.Int32.MaxValue" />.  
		/// -or-  
		/// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one of the underlying collections was modified outside of its <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		public static int TryTakeFromAny(BlockingCollection<T>[] collections, out T item, TimeSpan timeout)
		{
			BlockingCollection<T>.ValidateTimeout(timeout);
			return BlockingCollection<T>.TryTakeFromAnyCore(collections, out item, (int)timeout.TotalMilliseconds, false, CancellationToken.None);
		}

		/// <summary>Tries to remove an item from any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item removed from one of the collections.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array from which the item was removed, or -1 if an item could not be removed.</returns>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.  
		/// -or-  
		/// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one of the underlying collections was modified outside of its <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		public static int TryTakeFromAny(BlockingCollection<T>[] collections, out T item, int millisecondsTimeout)
		{
			BlockingCollection<T>.ValidateMillisecondsTimeout(millisecondsTimeout);
			return BlockingCollection<T>.TryTakeFromAnyCore(collections, out item, millisecondsTimeout, false, CancellationToken.None);
		}

		/// <summary>Tries to remove an item from any one of the specified <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances.</summary>
		/// <param name="collections">The array of collections.</param>
		/// <param name="item">The item removed from one of the collections.</param>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <param name="cancellationToken">A cancellation token to observe.</param>
		/// <returns>The index of the collection in the <paramref name="collections" /> array from which the item was removed, or -1 if an item could not be removed.</returns>
		/// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
		/// <exception cref="T:System.InvalidOperationException">At least one of the underlying collections was modified outside of its <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.  
		/// -or-  
		/// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and 63 for MTA.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="collections" /> argument is a 0-length array or contains a null element.</exception>
		/// <exception cref="T:System.ObjectDisposedException">At least one of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances has been disposed.</exception>
		public static int TryTakeFromAny(BlockingCollection<T>[] collections, out T item, int millisecondsTimeout, CancellationToken cancellationToken)
		{
			BlockingCollection<T>.ValidateMillisecondsTimeout(millisecondsTimeout);
			return BlockingCollection<T>.TryTakeFromAnyCore(collections, out item, millisecondsTimeout, false, cancellationToken);
		}

		private static int TryTakeFromAnyCore(BlockingCollection<T>[] collections, out T item, int millisecondsTimeout, bool isTakeOperation, CancellationToken externalCancellationToken)
		{
			BlockingCollection<T>.ValidateCollectionsArray(collections, false);
			for (int i = 0; i < collections.Length; i++)
			{
				if (!collections[i].IsCompleted && collections[i]._occupiedNodes.CurrentCount > 0 && collections[i].TryTake(out item))
				{
					return i;
				}
			}
			return BlockingCollection<T>.TryTakeFromAnyCoreSlow(collections, out item, millisecondsTimeout, isTakeOperation, externalCancellationToken);
		}

		private static int TryTakeFromAnyCoreSlow(BlockingCollection<T>[] collections, out T item, int millisecondsTimeout, bool isTakeOperation, CancellationToken externalCancellationToken)
		{
			int num = millisecondsTimeout;
			uint startTime = 0U;
			if (millisecondsTimeout != -1)
			{
				startTime = (uint)Environment.TickCount;
			}
			while (millisecondsTimeout == -1 || num >= 0)
			{
				CancellationToken[] tokens;
				List<WaitHandle> handles = BlockingCollection<T>.GetHandles(collections, externalCancellationToken, false, out tokens);
				if (handles.Count == 0 && isTakeOperation)
				{
					throw new ArgumentException("All collections are marked as complete with regards to additions.", "collections");
				}
				if (handles.Count != 0)
				{
					using (CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokens))
					{
						handles.Add(cancellationTokenSource.Token.WaitHandle);
						int num2 = WaitHandle.WaitAny(handles.ToArray(), num);
						if (cancellationTokenSource.IsCancellationRequested && externalCancellationToken.IsCancellationRequested)
						{
							throw new OperationCanceledException("The operation was canceled.", externalCancellationToken);
						}
						if (!cancellationTokenSource.IsCancellationRequested)
						{
							if (num2 == 258)
							{
								break;
							}
							if (collections.Length != handles.Count - 1)
							{
								for (int i = 0; i < collections.Length; i++)
								{
									if (collections[i]._occupiedNodes.AvailableWaitHandle == handles[num2])
									{
										num2 = i;
										break;
									}
								}
							}
							if (collections[num2].TryTake(out item))
							{
								return num2;
							}
						}
					}
					if (millisecondsTimeout != -1)
					{
						num = BlockingCollection<T>.UpdateTimeOut(startTime, millisecondsTimeout);
						continue;
					}
					continue;
				}
				break;
			}
			item = default(T);
			return -1;
		}

		/// <summary>Marks the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instances as not accepting any more additions.</summary>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		public void CompleteAdding()
		{
			this.CheckDisposed();
			if (this.IsAddingCompleted)
			{
				return;
			}
			SpinWait spinWait = default(SpinWait);
			for (;;)
			{
				int currentAdders = this._currentAdders;
				if ((currentAdders & -2147483648) != 0)
				{
					break;
				}
				if (Interlocked.CompareExchange(ref this._currentAdders, currentAdders | -2147483648, currentAdders) == currentAdders)
				{
					goto Block_4;
				}
				spinWait.SpinOnce();
			}
			spinWait.Reset();
			while (this._currentAdders != -2147483648)
			{
				spinWait.SpinOnce();
			}
			return;
			Block_4:
			spinWait.Reset();
			while (this._currentAdders != -2147483648)
			{
				spinWait.SpinOnce();
			}
			if (this.Count == 0)
			{
				this.CancelWaitingConsumers();
			}
			this.CancelWaitingProducers();
		}

		private void CancelWaitingConsumers()
		{
			this._consumersCancellationTokenSource.Cancel();
		}

		private void CancelWaitingProducers()
		{
			this._producersCancellationTokenSource.Cancel();
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases resources used by the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance.</summary>
		/// <param name="disposing">Whether being disposed explicitly (true) or due to a finalizer (false).</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this._isDisposed)
			{
				if (this._freeNodes != null)
				{
					this._freeNodes.Dispose();
				}
				this._occupiedNodes.Dispose();
				this._isDisposed = true;
			}
		}

		/// <summary>Copies the items from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance into a new array.</summary>
		/// <returns>An array containing copies of the elements of the collection.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		public T[] ToArray()
		{
			this.CheckDisposed();
			return this._collection.ToArray();
		}

		/// <summary>Copies all of the items in the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance to a compatible one-dimensional array, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="array" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="index" /> argument is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="index" /> argument is equal to or greater than the length of the <paramref name="array" />.  
		///  The destination array is too small to hold all of the BlockingCcollection elements.  
		///  The array rank doesn't match.  
		///  The array type is incompatible with the type of the BlockingCollection elements.</exception>
		public void CopyTo(T[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>Copies all of the items in the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance to a compatible one-dimensional array, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> instance. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="array" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="index" /> argument is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="index" /> argument is equal to or greater than the length of the <paramref name="array" />, the array is multidimensional, or the type parameter for the collection cannot be cast automatically to the type of the destination array.</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			this.CheckDisposed();
			T[] array2 = this._collection.ToArray();
			try
			{
				Array.Copy(array2, 0, array, index, array2.Length);
			}
			catch (ArgumentNullException)
			{
				throw new ArgumentNullException("array");
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new ArgumentOutOfRangeException("index", index, "The index argument must be greater than or equal zero.");
			}
			catch (ArgumentException)
			{
				throw new ArgumentException("The number of elements in the collection is greater than the available space from index to the end of the destination array.", "index");
			}
			catch (RankException)
			{
				throw new ArgumentException("The array argument is multidimensional.", "array");
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException("The array argument is of the incorrect type.", "array");
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("The array argument is of the incorrect type.", "array");
			}
		}

		/// <summary>Provides a consuming <see cref="T:System.Collections.Generic.IEnumerator`1" /> for items in the collection.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that removes and returns items from the collection.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		public IEnumerable<T> GetConsumingEnumerable()
		{
			return this.GetConsumingEnumerable(CancellationToken.None);
		}

		/// <summary>Provides a consuming <see cref="T:System.Collections.Generic.IEnumerable`1" /> for items in the collection.</summary>
		/// <param name="cancellationToken">A cancellation token to observe.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that removes and returns items from the collection.</returns>
		/// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed or the <see cref="T:System.Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has been disposed</exception>
		public IEnumerable<T> GetConsumingEnumerable(CancellationToken cancellationToken)
		{
			CancellationTokenSource linkedTokenSource = null;
			try
			{
				linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._consumersCancellationTokenSource.Token);
				while (!this.IsCompleted)
				{
					T t;
					if (this.TryTakeWithNoTimeValidation(out t, -1, cancellationToken, linkedTokenSource))
					{
						yield return t;
					}
				}
			}
			finally
			{
				if (linkedTokenSource != null)
				{
					linkedTokenSource.Dispose();
				}
			}
			yield break;
			yield break;
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			this.CheckDisposed();
			return this._collection.GetEnumerator();
		}

		/// <summary>Provides an <see cref="T:System.Collections.IEnumerator" /> for items in the collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the items in the collection.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Collections.Concurrent.BlockingCollection`1" /> has been disposed.</exception>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<!0>)this).GetEnumerator();
		}

		private static void ValidateCollectionsArray(BlockingCollection<T>[] collections, bool isAddOperation)
		{
			if (collections == null)
			{
				throw new ArgumentNullException("collections");
			}
			if (collections.Length < 1)
			{
				throw new ArgumentException("The collections argument is a zero-length array.", "collections");
			}
			if ((!BlockingCollection<T>.IsSTAThread && collections.Length > 63) || (BlockingCollection<T>.IsSTAThread && collections.Length > 62))
			{
				throw new ArgumentOutOfRangeException("collections", "The collections length is greater than the supported range for 32 bit machine.");
			}
			for (int i = 0; i < collections.Length; i++)
			{
				if (collections[i] == null)
				{
					throw new ArgumentException("The collections argument contains at least one null element.", "collections");
				}
				if (collections[i]._isDisposed)
				{
					throw new ObjectDisposedException("collections", "The collections argument contains at least one disposed element.");
				}
				if (isAddOperation && collections[i].IsAddingCompleted)
				{
					throw new ArgumentException("At least one of the specified collections is marked as complete with regards to additions.", "collections");
				}
			}
		}

		private static bool IsSTAThread
		{
			get
			{
				return false;
			}
		}

		private static void ValidateTimeout(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if ((num < 0L || num > 2147483647L) && num != -1L)
			{
				throw new ArgumentOutOfRangeException("timeout", timeout, string.Format(CultureInfo.InvariantCulture, "The specified timeout must represent a value between -1 and {0}, inclusive.", int.MaxValue));
			}
		}

		private static void ValidateMillisecondsTimeout(int millisecondsTimeout)
		{
			if (millisecondsTimeout < 0 && millisecondsTimeout != -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", millisecondsTimeout, string.Format(CultureInfo.InvariantCulture, "The specified timeout must represent a value between -1 and {0}, inclusive.", int.MaxValue));
			}
		}

		private void CheckDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException("BlockingCollection", "The collection has been disposed.");
			}
		}

		private IProducerConsumerCollection<T> _collection;

		private int _boundedCapacity;

		private const int NON_BOUNDED = -1;

		private SemaphoreSlim _freeNodes;

		private SemaphoreSlim _occupiedNodes;

		private bool _isDisposed;

		private CancellationTokenSource _consumersCancellationTokenSource;

		private CancellationTokenSource _producersCancellationTokenSource;

		private volatile int _currentAdders;

		private const int COMPLETE_ADDING_ON_MASK = -2147483648;
	}
}
