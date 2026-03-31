using System;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
	/// <summary>Represents a particular manner of splitting an orderable data source into multiple partitions.</summary>
	/// <typeparam name="TSource">Type of the elements in the collection.</typeparam>
	public abstract class OrderablePartitioner<TSource> : Partitioner<TSource>
	{
		/// <summary>Called from constructors in derived classes to initialize the <see cref="T:System.Collections.Concurrent.OrderablePartitioner`1" /> class with the specified constraints on the index keys.</summary>
		/// <param name="keysOrderedInEachPartition">Indicates whether the elements in each partition are yielded in the order of increasing keys.</param>
		/// <param name="keysOrderedAcrossPartitions">Indicates whether elements in an earlier partition always come before elements in a later partition. If true, each element in partition 0 has a smaller order key than any element in partition 1, each element in partition 1 has a smaller order key than any element in partition 2, and so on.</param>
		/// <param name="keysNormalized">Indicates whether keys are normalized. If true, all order keys are distinct integers in the range [0 .. numberOfElements-1]. If false, order keys must still be distinct, but only their relative order is considered, not their absolute values.</param>
		protected OrderablePartitioner(bool keysOrderedInEachPartition, bool keysOrderedAcrossPartitions, bool keysNormalized)
		{
			this.KeysOrderedInEachPartition = keysOrderedInEachPartition;
			this.KeysOrderedAcrossPartitions = keysOrderedAcrossPartitions;
			this.KeysNormalized = keysNormalized;
		}

		/// <summary>Partitions the underlying collection into the specified number of orderable partitions.</summary>
		/// <param name="partitionCount">The number of partitions to create.</param>
		/// <returns>A list containing <paramref name="partitionCount" /> enumerators.</returns>
		public abstract IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount);

		/// <summary>Creates an object that can partition the underlying collection into a variable number of partitions.</summary>
		/// <returns>An object that can create partitions over the underlying data source.</returns>
		/// <exception cref="T:System.NotSupportedException">Dynamic partitioning is not supported by this partitioner.</exception>
		public virtual IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions()
		{
			throw new NotSupportedException("Dynamic partitions are not supported by this partitioner.");
		}

		/// <summary>Gets whether elements in each partition are yielded in the order of increasing keys.</summary>
		/// <returns>
		///   <see langword="true" /> if the elements in each partition are yielded in the order of increasing keys; otherwise, <see langword="false" />.</returns>
		public bool KeysOrderedInEachPartition { get; private set; }

		/// <summary>Gets whether elements in an earlier partition always come before elements in a later partition.</summary>
		/// <returns>
		///   <see langword="true" /> if the elements in an earlier partition always come before elements in a later partition; otherwise, <see langword="false" />.</returns>
		public bool KeysOrderedAcrossPartitions { get; private set; }

		/// <summary>Gets whether order keys are normalized.</summary>
		/// <returns>
		///   <see langword="true" /> if the keys are normalized; otherwise, <see langword="false" />.</returns>
		public bool KeysNormalized { get; private set; }

		/// <summary>Partitions the underlying collection into the given number of ordered partitions.</summary>
		/// <param name="partitionCount">The number of partitions to create.</param>
		/// <returns>A list containing <paramref name="partitionCount" /> enumerators.</returns>
		public override IList<IEnumerator<TSource>> GetPartitions(int partitionCount)
		{
			IList<IEnumerator<KeyValuePair<long, TSource>>> orderablePartitions = this.GetOrderablePartitions(partitionCount);
			if (orderablePartitions.Count != partitionCount)
			{
				throw new InvalidOperationException("GetPartitions returned an incorrect number of partitions.");
			}
			IEnumerator<TSource>[] array = new IEnumerator<!0>[partitionCount];
			for (int i = 0; i < partitionCount; i++)
			{
				array[i] = new OrderablePartitioner<TSource>.EnumeratorDropIndices(orderablePartitions[i]);
			}
			return array;
		}

		/// <summary>Creates an object that can partition the underlying collection into a variable number of partitions.</summary>
		/// <returns>An object that can create partitions over the underlying data source.</returns>
		/// <exception cref="T:System.NotSupportedException">Dynamic partitioning is not supported by the base class. It must be implemented in derived classes.</exception>
		public override IEnumerable<TSource> GetDynamicPartitions()
		{
			return new OrderablePartitioner<TSource>.EnumerableDropIndices(this.GetOrderableDynamicPartitions());
		}

		private class EnumerableDropIndices : IEnumerable<!0>, IEnumerable, IDisposable
		{
			public EnumerableDropIndices(IEnumerable<KeyValuePair<long, TSource>> source)
			{
				this._source = source;
			}

			public IEnumerator<TSource> GetEnumerator()
			{
				return new OrderablePartitioner<TSource>.EnumeratorDropIndices(this._source.GetEnumerator());
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public void Dispose()
			{
				IDisposable disposable = this._source as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}

			private readonly IEnumerable<KeyValuePair<long, TSource>> _source;
		}

		private class EnumeratorDropIndices : IEnumerator<!0>, IDisposable, IEnumerator
		{
			public EnumeratorDropIndices(IEnumerator<KeyValuePair<long, TSource>> source)
			{
				this._source = source;
			}

			public bool MoveNext()
			{
				return this._source.MoveNext();
			}

			public TSource Current
			{
				get
				{
					KeyValuePair<long, TSource> keyValuePair = this._source.Current;
					return keyValuePair.Value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
				this._source.Dispose();
			}

			public void Reset()
			{
				this._source.Reset();
			}

			private readonly IEnumerator<KeyValuePair<long, TSource>> _source;
		}
	}
}
