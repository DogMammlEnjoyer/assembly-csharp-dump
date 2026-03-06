using System;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
	/// <summary>Represents a particular manner of splitting a data source into multiple partitions.</summary>
	/// <typeparam name="TSource">Type of the elements in the collection.</typeparam>
	public abstract class Partitioner<TSource>
	{
		/// <summary>Partitions the underlying collection into the given number of partitions.</summary>
		/// <param name="partitionCount">The number of partitions to create.</param>
		/// <returns>A list containing <paramref name="partitionCount" /> enumerators.</returns>
		public abstract IList<IEnumerator<TSource>> GetPartitions(int partitionCount);

		/// <summary>Gets whether additional partitions can be created dynamically.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Concurrent.Partitioner`1" /> can create partitions dynamically as they are requested; <see langword="false" /> if the <see cref="T:System.Collections.Concurrent.Partitioner`1" /> can only allocate partitions statically.</returns>
		public virtual bool SupportsDynamicPartitions
		{
			get
			{
				return false;
			}
		}

		/// <summary>Creates an object that can partition the underlying collection into a variable number of partitions.</summary>
		/// <returns>An object that can create partitions over the underlying data source.</returns>
		/// <exception cref="T:System.NotSupportedException">Dynamic partitioning is not supported by the base class. You must implement it in a derived class.</exception>
		public virtual IEnumerable<TSource> GetDynamicPartitions()
		{
			throw new NotSupportedException("Dynamic partitions are not supported by this partitioner.");
		}
	}
}
