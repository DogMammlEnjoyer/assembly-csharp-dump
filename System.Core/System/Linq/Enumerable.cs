using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	/// <summary>Provides a set of <see langword="static" /> (<see langword="Shared" /> in Visual Basic) methods for querying objects that implement <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
	public static class Enumerable
	{
		/// <summary>Applies an accumulator function over a sequence.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
		/// <param name="func">An accumulator function to be invoked on each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The final accumulator value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="func" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (func == null)
			{
				throw Error.ArgumentNull("func");
			}
			TSource result;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				TSource tsource = enumerator.Current;
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					tsource = func(tsource, arg);
				}
				result = tsource;
			}
			return result;
		}

		/// <summary>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
		/// <param name="seed">The initial accumulator value.</param>
		/// <param name="func">An accumulator function to be invoked on each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
		/// <returns>The final accumulator value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="func" /> is <see langword="null" />.</exception>
		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (func == null)
			{
				throw Error.ArgumentNull("func");
			}
			TAccumulate taccumulate = seed;
			foreach (TSource arg in source)
			{
				taccumulate = func(taccumulate, arg);
			}
			return taccumulate;
		}

		/// <summary>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
		/// <param name="seed">The initial accumulator value.</param>
		/// <param name="func">An accumulator function to be invoked on each element.</param>
		/// <param name="resultSelector">A function to transform the final accumulator value into the result value.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
		/// <typeparam name="TResult">The type of the resulting value.</typeparam>
		/// <returns>The transformed final accumulator value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="func" /> or <paramref name="resultSelector" /> is <see langword="null" />.</exception>
		public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (func == null)
			{
				throw Error.ArgumentNull("func");
			}
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			TAccumulate taccumulate = seed;
			foreach (TSource arg in source)
			{
				taccumulate = func(taccumulate, arg);
			}
			return resultSelector(taccumulate);
		}

		/// <summary>Determines whether a sequence contains any elements.</summary>
		/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to check for emptiness.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="true" /> if the source sequence contains any elements; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static bool Any<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			bool result;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				result = enumerator.MoveNext();
			}
			return result;
		}

		/// <summary>Determines whether any element of a sequence satisfies a condition.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to apply the predicate to.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="true" /> if any elements in the source sequence pass the test in the specified predicate; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			foreach (TSource arg in source)
			{
				if (predicate(arg))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Determines whether all elements of a sequence satisfy a condition.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements to apply the predicate to.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="true" /> if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			foreach (TSource arg in source)
			{
				if (!predicate(arg))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Appends a value to the end of the sequence.</summary>
		/// <param name="source">A sequence of values. </param>
		/// <param name="element">The value to append to <paramref name="source" />.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />. </typeparam>
		/// <returns>A new sequence that ends with <paramref name="element" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			Enumerable.AppendPrependIterator<TSource> appendPrependIterator = source as Enumerable.AppendPrependIterator<TSource>;
			if (appendPrependIterator == null)
			{
				return new Enumerable.AppendPrepend1Iterator<TSource>(source, element, true);
			}
			return appendPrependIterator.Append(element);
		}

		/// <summary>Adds a value to the beginning of the sequence.</summary>
		/// <param name="source">A sequence of values. </param>
		/// <param name="element">The value to prepend to <paramref name="source" />. </param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>A new sequence that begins with <paramref name="element" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source" /> is <see langword="null" />. </exception>
		public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			Enumerable.AppendPrependIterator<TSource> appendPrependIterator = source as Enumerable.AppendPrependIterator<TSource>;
			if (appendPrependIterator == null)
			{
				return new Enumerable.AppendPrepend1Iterator<TSource>(source, element, false);
			}
			return appendPrependIterator.Prepend(element);
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Int32" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Int32" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static double Average(this IEnumerable<int> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double result;
			using (IEnumerator<int> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				long num = (long)enumerator.Current;
				long num2 = 1L;
				checked
				{
					while (enumerator.MoveNext())
					{
						int num3 = enumerator.Current;
						num += unchecked((long)num3);
						num2 += 1L;
					}
					result = (double)num / (double)num2;
				}
			}
			return result;
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Int32" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int32" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static double? Average(this IEnumerable<int?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			foreach (int? num in source)
			{
				if (num != null)
				{
					long num2 = (long)num.GetValueOrDefault();
					long num3 = 1L;
					checked
					{
						IEnumerator<int?> enumerator;
						while (enumerator.MoveNext())
						{
							num = enumerator.Current;
							if (num != null)
							{
								num2 += unchecked((long)num.GetValueOrDefault());
								num3 += 1L;
							}
						}
						return new double?((double)num2 / (double)num3);
					}
				}
			}
			return null;
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Int64" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Int64" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static double Average(this IEnumerable<long> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			checked
			{
				double result;
				using (IEnumerator<long> enumerator = source.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						throw Error.NoElements();
					}
					long num = enumerator.Current;
					long num2 = 1L;
					while (enumerator.MoveNext())
					{
						long num3 = enumerator.Current;
						num += num3;
						num2 += 1L;
					}
					result = (double)num / (double)num2;
				}
				return result;
			}
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Int64" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int64" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static double? Average(this IEnumerable<long?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			checked
			{
				foreach (long? num in source)
				{
					if (num != null)
					{
						long num2 = num.GetValueOrDefault();
						long num3 = 1L;
						IEnumerator<long?> enumerator;
						while (enumerator.MoveNext())
						{
							num = enumerator.Current;
							if (num != null)
							{
								num2 += num.GetValueOrDefault();
								num3 += 1L;
							}
						}
						return new double?((double)num2 / (double)num3);
					}
				}
				return null;
			}
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Single" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Single" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static float Average(this IEnumerable<float> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			float result;
			using (IEnumerator<float> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				double num = (double)enumerator.Current;
				long num2 = 1L;
				while (enumerator.MoveNext())
				{
					float num3 = enumerator.Current;
					num += (double)num3;
					num2 += 1L;
				}
				result = (float)(num / (double)num2);
			}
			return result;
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Single" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Single" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static float? Average(this IEnumerable<float?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			foreach (float? num in source)
			{
				if (num != null)
				{
					double num2 = (double)num.GetValueOrDefault();
					long num3 = 1L;
					IEnumerator<float?> enumerator;
					while (enumerator.MoveNext())
					{
						num = enumerator.Current;
						if (num != null)
						{
							num2 += (double)num.GetValueOrDefault();
							checked
							{
								num3 += 1L;
							}
						}
					}
					return new float?((float)(num2 / (double)num3));
				}
			}
			return null;
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Double" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Double" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static double Average(this IEnumerable<double> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double result;
			using (IEnumerator<double> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				double num = enumerator.Current;
				long num2 = 1L;
				while (enumerator.MoveNext())
				{
					double num3 = enumerator.Current;
					num += num3;
					num2 += 1L;
				}
				result = num / (double)num2;
			}
			return result;
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Double" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Double" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static double? Average(this IEnumerable<double?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			foreach (double? num in source)
			{
				if (num != null)
				{
					double num2 = num.GetValueOrDefault();
					long num3 = 1L;
					IEnumerator<double?> enumerator;
					while (enumerator.MoveNext())
					{
						num = enumerator.Current;
						if (num != null)
						{
							num2 += num.GetValueOrDefault();
							checked
							{
								num3 += 1L;
							}
						}
					}
					return new double?(num2 / (double)num3);
				}
			}
			return null;
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Decimal" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Decimal" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static decimal Average(this IEnumerable<decimal> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			decimal result;
			using (IEnumerator<decimal> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				decimal d = enumerator.Current;
				long num = 1L;
				while (enumerator.MoveNext())
				{
					decimal d2 = enumerator.Current;
					d += d2;
					num += 1L;
				}
				result = d / num;
			}
			return result;
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Decimal" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Decimal" /> values to calculate the average of.</param>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal? Average(this IEnumerable<decimal?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			foreach (decimal? num in source)
			{
				if (num != null)
				{
					decimal d = num.GetValueOrDefault();
					long num2 = 1L;
					IEnumerator<decimal?> enumerator;
					while (enumerator.MoveNext())
					{
						num = enumerator.Current;
						if (num != null)
						{
							d += num.GetValueOrDefault();
							num2 += 1L;
						}
					}
					return new decimal?(d / num2);
				}
			}
			return null;
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Int32" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double result;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				long num = (long)selector(enumerator.Current);
				long num2 = 1L;
				checked
				{
					while (enumerator.MoveNext())
					{
						TSource arg = enumerator.Current;
						num += unchecked((long)selector(arg));
						num2 += 1L;
					}
					result = (double)num / (double)num2;
				}
			}
			return result;
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Int32" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			foreach (TSource arg in source)
			{
				int? num = selector(arg);
				if (num != null)
				{
					long num2 = (long)num.GetValueOrDefault();
					long num3 = 1L;
					checked
					{
						IEnumerator<TSource> enumerator;
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							num = selector(arg2);
							if (num != null)
							{
								num2 += unchecked((long)num.GetValueOrDefault());
								num3 += 1L;
							}
						}
						return new double?((double)num2 / (double)num3);
					}
				}
			}
			return null;
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Int64" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			checked
			{
				double result;
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						throw Error.NoElements();
					}
					long num = selector(enumerator.Current);
					long num2 = 1L;
					while (enumerator.MoveNext())
					{
						TSource arg = enumerator.Current;
						num += selector(arg);
						num2 += 1L;
					}
					result = (double)num / (double)num2;
				}
				return result;
			}
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Int64" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			checked
			{
				foreach (TSource arg in source)
				{
					long? num = selector(arg);
					if (num != null)
					{
						long num2 = num.GetValueOrDefault();
						long num3 = 1L;
						IEnumerator<TSource> enumerator;
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							num = selector(arg2);
							if (num != null)
							{
								num2 += num.GetValueOrDefault();
								num3 += 1L;
							}
						}
						return new double?((double)num2 / (double)num3);
					}
				}
				return null;
			}
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Single" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			float result;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				double num = (double)selector(enumerator.Current);
				long num2 = 1L;
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					num += (double)selector(arg);
					num2 += 1L;
				}
				result = (float)(num / (double)num2);
			}
			return result;
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Single" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			foreach (TSource arg in source)
			{
				float? num = selector(arg);
				if (num != null)
				{
					double num2 = (double)num.GetValueOrDefault();
					long num3 = 1L;
					IEnumerator<TSource> enumerator;
					while (enumerator.MoveNext())
					{
						TSource arg2 = enumerator.Current;
						num = selector(arg2);
						if (num != null)
						{
							num2 += (double)num.GetValueOrDefault();
							checked
							{
								num3 += 1L;
							}
						}
					}
					return new float?((float)(num2 / (double)num3));
				}
			}
			return null;
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Double" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double result;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				double num = selector(enumerator.Current);
				long num2 = 1L;
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					num += selector(arg);
					num2 += 1L;
				}
				result = num / (double)num2;
			}
			return result;
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Double" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			foreach (TSource arg in source)
			{
				double? num = selector(arg);
				if (num != null)
				{
					double num2 = num.GetValueOrDefault();
					long num3 = 1L;
					IEnumerator<TSource> enumerator;
					while (enumerator.MoveNext())
					{
						TSource arg2 = enumerator.Current;
						num = selector(arg2);
						if (num != null)
						{
							num2 += num.GetValueOrDefault();
							checked
							{
								num3 += 1L;
							}
						}
					}
					return new double?(num2 / (double)num3);
				}
			}
			return null;
		}

		/// <summary>Computes the average of a sequence of <see cref="T:System.Decimal" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate an average.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			decimal result;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				decimal d = selector(enumerator.Current);
				long num = 1L;
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					d += selector(arg);
					num += 1L;
				}
				result = d / num;
			}
			return result;
		}

		/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Decimal" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The average of the sequence of values, or <see langword="null" /> if the source sequence is empty or contains only values that are <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			foreach (TSource arg in source)
			{
				decimal? num = selector(arg);
				if (num != null)
				{
					decimal d = num.GetValueOrDefault();
					long num2 = 1L;
					IEnumerator<TSource> enumerator;
					while (enumerator.MoveNext())
					{
						TSource arg2 = enumerator.Current;
						num = selector(arg2);
						if (num != null)
						{
							d += num.GetValueOrDefault();
							num2 += 1L;
						}
					}
					return new decimal?(d / num2);
				}
			}
			return null;
		}

		/// <summary>Filters the elements of an <see cref="T:System.Collections.IEnumerable" /> based on a specified type.</summary>
		/// <param name="source">The <see cref="T:System.Collections.IEnumerable" /> whose elements to filter.</param>
		/// <typeparam name="TResult">The type to filter the elements of the sequence on.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence of type <paramref name="TResult" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return Enumerable.OfTypeIterator<TResult>(source);
		}

		private static IEnumerable<TResult> OfTypeIterator<TResult>(IEnumerable source)
		{
			foreach (object obj in source)
			{
				if (obj is TResult)
				{
					yield return (TResult)((object)obj);
				}
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Casts the elements of an <see cref="T:System.Collections.IEnumerable" /> to the specified type.</summary>
		/// <param name="source">The <see cref="T:System.Collections.IEnumerable" /> that contains the elements to be cast to type <paramref name="TResult" />.</param>
		/// <typeparam name="TResult">The type to cast the elements of <paramref name="source" /> to.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains each element of the source sequence cast to the specified type.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidCastException">An element in the sequence cannot be cast to type <paramref name="TResult" />.</exception>
		public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
		{
			IEnumerable<TResult> enumerable = source as IEnumerable<TResult>;
			if (enumerable != null)
			{
				return enumerable;
			}
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return Enumerable.CastIterator<TResult>(source);
		}

		private static IEnumerable<TResult> CastIterator<TResult>(IEnumerable source)
		{
			foreach (object obj in source)
			{
				yield return (TResult)((object)obj);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Concatenates two sequences.</summary>
		/// <param name="first">The first sequence to concatenate.</param>
		/// <param name="second">The sequence to concatenate to the first sequence.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the concatenated elements of the two input sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			if (first == null)
			{
				throw Error.ArgumentNull("first");
			}
			if (second == null)
			{
				throw Error.ArgumentNull("second");
			}
			Enumerable.ConcatIterator<TSource> concatIterator = first as Enumerable.ConcatIterator<TSource>;
			if (concatIterator == null)
			{
				return new Enumerable.Concat2Iterator<TSource>(first, second);
			}
			return concatIterator.Concat(second);
		}

		/// <summary>Determines whether a sequence contains a specified element by using the default equality comparer.</summary>
		/// <param name="source">A sequence in which to locate a value.</param>
		/// <param name="value">The value to locate in the sequence.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="true" /> if the source sequence contains an element that has the specified value; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
		{
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection == null)
			{
				return source.Contains(value, null);
			}
			return collection.Contains(value);
		}

		/// <summary>Determines whether a sequence contains a specified element by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
		/// <param name="source">A sequence in which to locate a value.</param>
		/// <param name="value">The value to locate in the sequence.</param>
		/// <param name="comparer">An equality comparer to compare values.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="true" /> if the source sequence contains an element that has the specified value; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (comparer == null)
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TSource x = enumerator.Current;
						if (EqualityComparer<TSource>.Default.Equals(x, value))
						{
							return true;
						}
					}
					return false;
				}
			}
			foreach (TSource x2 in source)
			{
				if (comparer.Equals(x2, value))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Returns the number of elements in a sequence.</summary>
		/// <param name="source">A sequence that contains elements to be counted.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The number of elements in the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int Count<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				return collection.Count;
			}
			IIListProvider<TSource> iilistProvider = source as IIListProvider<TSource>;
			if (iilistProvider != null)
			{
				return iilistProvider.GetCount(false);
			}
			ICollection collection2 = source as ICollection;
			if (collection2 != null)
			{
				return collection2.Count;
			}
			int num = 0;
			checked
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						num++;
					}
				}
				return num;
			}
		}

		/// <summary>Returns a number that represents how many elements in the specified sequence satisfy a condition.</summary>
		/// <param name="source">A sequence that contains elements to be tested and counted.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			int num = 0;
			checked
			{
				foreach (TSource arg in source)
				{
					if (predicate(arg))
					{
						num++;
					}
				}
				return num;
			}
		}

		/// <summary>Returns an <see cref="T:System.Int64" /> that represents the total number of elements in a sequence.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements to be counted.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The number of elements in the source sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The number of elements exceeds <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long LongCount<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			long num = 0L;
			checked
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						num += 1L;
					}
				}
				return num;
			}
		}

		/// <summary>Returns an <see cref="T:System.Int64" /> that represents how many elements in a sequence satisfy a condition.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements to be counted.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The number of matching elements exceeds <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			long num = 0L;
			checked
			{
				foreach (TSource arg in source)
				{
					if (predicate(arg))
					{
						num += 1L;
					}
				}
				return num;
			}
		}

		/// <summary>Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.</summary>
		/// <param name="source">The sequence to return a default value for if it is empty.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> object that contains the default value for the <paramref name="TSource" /> type if <paramref name="source" /> is empty; otherwise, <paramref name="source" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
		{
			return source.DefaultIfEmpty(default(TSource));
		}

		/// <summary>Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.</summary>
		/// <param name="source">The sequence to return the specified value for if it is empty.</param>
		/// <param name="defaultValue">The value to return if the sequence is empty.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <paramref name="defaultValue" /> if <paramref name="source" /> is empty; otherwise, <paramref name="source" />.</returns>
		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return new Enumerable.DefaultIfEmptyIterator<TSource>(source, defaultValue);
		}

		/// <summary>Returns distinct elements from a sequence by using the default equality comparer to compare values.</summary>
		/// <param name="source">The sequence to remove duplicate elements from.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains distinct elements from the source sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
		{
			return source.Distinct(null);
		}

		/// <summary>Returns distinct elements from a sequence by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</summary>
		/// <param name="source">The sequence to remove duplicate elements from.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains distinct elements from the source sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return new Enumerable.DistinctIterator<TSource>(source, comparer);
		}

		/// <summary>Returns the element at a specified index in a sequence.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
		/// <param name="index">The zero-based index of the element to retrieve.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The element at the specified position in the source sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="index" /> is less than 0 or greater than or equal to the number of elements in <paramref name="source" />.</exception>
		public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			IPartition<TSource> partition = source as IPartition<TSource>;
			if (partition != null)
			{
				bool flag;
				TSource result = partition.TryGetElementAt(index, out flag);
				if (flag)
				{
					return result;
				}
			}
			else
			{
				IList<TSource> list = source as IList<TSource>;
				if (list != null)
				{
					return list[index];
				}
				if (index >= 0)
				{
					using (IEnumerator<TSource> enumerator = source.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (index == 0)
							{
								return enumerator.Current;
							}
							index--;
						}
					}
				}
			}
			throw Error.ArgumentOutOfRange("index");
		}

		/// <summary>Returns the element at a specified index in a sequence or a default value if the index is out of range.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
		/// <param name="index">The zero-based index of the element to retrieve.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="default" />(<paramref name="TSource" />) if the index is outside the bounds of the source sequence; otherwise, the element at the specified position in the source sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			IPartition<TSource> partition = source as IPartition<TSource>;
			if (partition != null)
			{
				bool flag;
				return partition.TryGetElementAt(index, out flag);
			}
			if (index >= 0)
			{
				IList<TSource> list = source as IList<TSource>;
				if (list != null)
				{
					if (index < list.Count)
					{
						return list[index];
					}
				}
				else
				{
					using (IEnumerator<TSource> enumerator = source.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (index == 0)
							{
								return enumerator.Current;
							}
							index--;
						}
					}
				}
			}
			return default(TSource);
		}

		/// <summary>Returns the input typed as <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
		/// <param name="source">The sequence to type as <see cref="T:System.Collections.Generic.IEnumerable`1" />.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The input sequence typed as <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
		public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
		{
			return source;
		}

		/// <summary>Returns an empty <see cref="T:System.Collections.Generic.IEnumerable`1" /> that has the specified type argument.</summary>
		/// <typeparam name="TResult">The type to assign to the type parameter of the returned generic <see cref="T:System.Collections.Generic.IEnumerable`1" />.</typeparam>
		/// <returns>An empty <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose type argument is <paramref name="TResult" />.</returns>
		public static IEnumerable<TResult> Empty<TResult>()
		{
			return Array.Empty<TResult>();
		}

		/// <summary>Produces the set difference of two sequences by using the default equality comparer to compare values.</summary>
		/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that are not also in <paramref name="second" /> will be returned.</param>
		/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			if (first == null)
			{
				throw Error.ArgumentNull("first");
			}
			if (second == null)
			{
				throw Error.ArgumentNull("second");
			}
			return Enumerable.ExceptIterator<TSource>(first, second, null);
		}

		/// <summary>Produces the set difference of two sequences by using the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</summary>
		/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that are not also in <paramref name="second" /> will be returned.</param>
		/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if (first == null)
			{
				throw Error.ArgumentNull("first");
			}
			if (second == null)
			{
				throw Error.ArgumentNull("second");
			}
			return Enumerable.ExceptIterator<TSource>(first, second, comparer);
		}

		private static IEnumerable<TSource> ExceptIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Set<TSource> set = new Set<TSource>(comparer);
			foreach (TSource value in second)
			{
				set.Add(value);
			}
			foreach (TSource tsource in first)
			{
				if (set.Add(tsource))
				{
					yield return tsource;
				}
			}
			IEnumerator<TSource> enumerator2 = null;
			yield break;
			yield break;
		}

		/// <summary>Returns the first element of a sequence.</summary>
		/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the first element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The first element in the specified sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
		public static TSource First<TSource>(this IEnumerable<TSource> source)
		{
			bool flag;
			TSource result = source.TryGetFirst(out flag);
			if (!flag)
			{
				throw Error.NoElements();
			}
			return result;
		}

		/// <summary>Returns the first element in a sequence that satisfies a specified condition.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The first element in the sequence that passes the test in the specified predicate function.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate" />.-or-The source sequence is empty.</exception>
		public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			bool flag;
			TSource result = source.TryGetFirst(predicate, out flag);
			if (!flag)
			{
				throw Error.NoMatch();
			}
			return result;
		}

		/// <summary>Returns the first element of a sequence, or a default value if the sequence contains no elements.</summary>
		/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the first element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="default" />(<paramref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			bool flag;
			return source.TryGetFirst(out flag);
		}

		/// <summary>Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="default" />(<paramref name="TSource" />) if <paramref name="source" /> is empty or if no element passes the test specified by <paramref name="predicate" />; otherwise, the first element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			bool flag;
			return source.TryGetFirst(predicate, out flag);
		}

		private static TSource TryGetFirst<TSource>(this IEnumerable<TSource> source, out bool found)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			IPartition<TSource> partition = source as IPartition<TSource>;
			if (partition != null)
			{
				return partition.TryGetFirst(out found);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				if (list.Count > 0)
				{
					found = true;
					return list[0];
				}
			}
			else
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						found = true;
						return enumerator.Current;
					}
				}
			}
			found = false;
			return default(TSource);
		}

		private static TSource TryGetFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			OrderedEnumerable<TSource> orderedEnumerable = source as OrderedEnumerable<TSource>;
			if (orderedEnumerable != null)
			{
				return orderedEnumerable.TryGetFirst(predicate, out found);
			}
			foreach (TSource tsource in source)
			{
				if (predicate(tsource))
				{
					found = true;
					return tsource;
				}
			}
			found = false;
			return default(TSource);
		}

		/// <summary>Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.</summary>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements of type <paramref name="TResult" /> that are obtained by performing a grouped join on two sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="outer" /> or <paramref name="inner" /> or <paramref name="outerKeySelector" /> or <paramref name="innerKeySelector" /> or <paramref name="resultSelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			if (outer == null)
			{
				throw Error.ArgumentNull("outer");
			}
			if (inner == null)
			{
				throw Error.ArgumentNull("inner");
			}
			if (outerKeySelector == null)
			{
				throw Error.ArgumentNull("outerKeySelector");
			}
			if (innerKeySelector == null)
			{
				throw Error.ArgumentNull("innerKeySelector");
			}
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			return Enumerable.GroupJoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		/// <summary>Correlates the elements of two sequences based on key equality and groups the results. A specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> is used to compare keys.</summary>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to hash and compare keys.</param>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements of type <paramref name="TResult" /> that are obtained by performing a grouped join on two sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="outer" /> or <paramref name="inner" /> or <paramref name="outerKeySelector" /> or <paramref name="innerKeySelector" /> or <paramref name="resultSelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			if (outer == null)
			{
				throw Error.ArgumentNull("outer");
			}
			if (inner == null)
			{
				throw Error.ArgumentNull("inner");
			}
			if (outerKeySelector == null)
			{
				throw Error.ArgumentNull("outerKeySelector");
			}
			if (innerKeySelector == null)
			{
				throw Error.ArgumentNull("innerKeySelector");
			}
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			return Enumerable.GroupJoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		private static IEnumerable<TResult> GroupJoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			using (IEnumerator<TOuter> e = outer.GetEnumerator())
			{
				if (e.MoveNext())
				{
					Lookup<TKey, TInner> lookup = Lookup<TKey, TInner>.CreateForJoin(inner, innerKeySelector, comparer);
					do
					{
						TOuter touter = e.Current;
						yield return resultSelector(touter, lookup[outerKeySelector(touter)]);
					}
					while (e.MoveNext());
					lookup = null;
				}
			}
			IEnumerator<TOuter> e = null;
			yield break;
			yield break;
		}

		/// <summary>Groups the elements of a sequence according to a specified key selector function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An IEnumerable&lt;IGrouping&lt;TKey, TSource&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TSource)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2" /> object contains a sequence of objects and a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return new GroupedEnumerable<TSource, TKey>(source, keySelector, null);
		}

		/// <summary>Groups the elements of a sequence according to a specified key selector function and compares the keys by using a specified comparer.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An IEnumerable&lt;IGrouping&lt;TKey, TSource&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TSource)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2" /> object contains a collection of objects and a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return new GroupedEnumerable<TSource, TKey>(source, keySelector, comparer);
		}

		/// <summary>Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="elementSelector">A function to map each source element to an element in the <see cref="T:System.Linq.IGrouping`2" />.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the elements in the <see cref="T:System.Linq.IGrouping`2" />.</typeparam>
		/// <returns>An IEnumerable&lt;IGrouping&lt;TKey, TElement&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TElement)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2" /> object contains a collection of objects of type <paramref name="TElement" /> and a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector, elementSelector, null);
		}

		/// <summary>Groups the elements of a sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2" />.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the elements in the <see cref="T:System.Linq.IGrouping`2" />.</typeparam>
		/// <returns>An IEnumerable&lt;IGrouping&lt;TKey, TElement&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TElement)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2" /> object contains a collection of objects of type <paramref name="TElement" /> and a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			return new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
		}

		/// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="resultSelector">A function to create a result value from each group.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector" />.</typeparam>
		/// <returns>A collection of elements of type <paramref name="TResult" /> where each element represents a projection over a group and its key.</returns>
		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
		{
			return new GroupedResultEnumerable<TSource, TKey, TResult>(source, keySelector, resultSelector, null);
		}

		/// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2" />.</param>
		/// <param name="resultSelector">A function to create a result value from each group.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the elements in each <see cref="T:System.Linq.IGrouping`2" />.</typeparam>
		/// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector" />.</typeparam>
		/// <returns>A collection of elements of type <paramref name="TResult" /> where each element represents a projection over a group and its key.</returns>
		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			return new GroupedResultEnumerable<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, null);
		}

		/// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="resultSelector">A function to create a result value from each group.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys with.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector" />.</typeparam>
		/// <returns>A collection of elements of type <paramref name="TResult" /> where each element represents a projection over a group and its key.</returns>
		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			return new GroupedResultEnumerable<TSource, TKey, TResult>(source, keySelector, resultSelector, comparer);
		}

		/// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2" />.</param>
		/// <param name="resultSelector">A function to create a result value from each group.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys with.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the elements in each <see cref="T:System.Linq.IGrouping`2" />.</typeparam>
		/// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector" />.</typeparam>
		/// <returns>A collection of elements of type <paramref name="TResult" /> where each element represents a projection over a group and its key.</returns>
		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			return new GroupedResultEnumerable<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, comparer);
		}

		/// <summary>Produces the set intersection of two sequences by using the default equality comparer to compare values.</summary>
		/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements that also appear in <paramref name="second" /> will be returned.</param>
		/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements that also appear in the first sequence will be returned.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			if (first == null)
			{
				throw Error.ArgumentNull("first");
			}
			if (second == null)
			{
				throw Error.ArgumentNull("second");
			}
			return Enumerable.IntersectIterator<TSource>(first, second, null);
		}

		/// <summary>Produces the set intersection of two sequences by using the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</summary>
		/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements that also appear in <paramref name="second" /> will be returned.</param>
		/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements that also appear in the first sequence will be returned.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if (first == null)
			{
				throw Error.ArgumentNull("first");
			}
			if (second == null)
			{
				throw Error.ArgumentNull("second");
			}
			return Enumerable.IntersectIterator<TSource>(first, second, comparer);
		}

		private static IEnumerable<TSource> IntersectIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Set<TSource> set = new Set<TSource>(comparer);
			foreach (TSource value in second)
			{
				set.Add(value);
			}
			foreach (TSource tsource in first)
			{
				if (set.Remove(tsource))
				{
					yield return tsource;
				}
			}
			IEnumerator<TSource> enumerator2 = null;
			yield break;
			yield break;
		}

		/// <summary>Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.</summary>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two matching elements.</param>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that has elements of type <paramref name="TResult" /> that are obtained by performing an inner join on two sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="outer" /> or <paramref name="inner" /> or <paramref name="outerKeySelector" /> or <paramref name="innerKeySelector" /> or <paramref name="resultSelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			if (outer == null)
			{
				throw Error.ArgumentNull("outer");
			}
			if (inner == null)
			{
				throw Error.ArgumentNull("inner");
			}
			if (outerKeySelector == null)
			{
				throw Error.ArgumentNull("outerKeySelector");
			}
			if (innerKeySelector == null)
			{
				throw Error.ArgumentNull("innerKeySelector");
			}
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			return Enumerable.JoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		/// <summary>Correlates the elements of two sequences based on matching keys. A specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> is used to compare keys.</summary>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two matching elements.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to hash and compare keys.</param>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that has elements of type <paramref name="TResult" /> that are obtained by performing an inner join on two sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="outer" /> or <paramref name="inner" /> or <paramref name="outerKeySelector" /> or <paramref name="innerKeySelector" /> or <paramref name="resultSelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			if (outer == null)
			{
				throw Error.ArgumentNull("outer");
			}
			if (inner == null)
			{
				throw Error.ArgumentNull("inner");
			}
			if (outerKeySelector == null)
			{
				throw Error.ArgumentNull("outerKeySelector");
			}
			if (innerKeySelector == null)
			{
				throw Error.ArgumentNull("innerKeySelector");
			}
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			return Enumerable.JoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		private static IEnumerable<TResult> JoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			using (IEnumerator<TOuter> e = outer.GetEnumerator())
			{
				if (e.MoveNext())
				{
					Lookup<TKey, TInner> lookup = Lookup<TKey, TInner>.CreateForJoin(inner, innerKeySelector, comparer);
					if (lookup.Count != 0)
					{
						do
						{
							TOuter item = e.Current;
							Grouping<TKey, TInner> grouping = lookup.GetGrouping(outerKeySelector(item), false);
							if (grouping != null)
							{
								int count = grouping._count;
								TInner[] elements = grouping._elements;
								int num;
								for (int i = 0; i != count; i = num)
								{
									yield return resultSelector(item, elements[i]);
									num = i + 1;
								}
								elements = null;
							}
							item = default(TOuter);
						}
						while (e.MoveNext());
					}
					lookup = null;
				}
			}
			IEnumerator<TOuter> e = null;
			yield break;
			yield break;
		}

		/// <summary>Returns the last element of a sequence.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the last element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value at the last position in the source sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
		public static TSource Last<TSource>(this IEnumerable<TSource> source)
		{
			bool flag;
			TSource result = source.TryGetLast(out flag);
			if (!flag)
			{
				throw Error.NoElements();
			}
			return result;
		}

		/// <summary>Returns the last element of a sequence that satisfies a specified condition.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The last element in the sequence that passes the test in the specified predicate function.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate" />.-or-The source sequence is empty.</exception>
		public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			bool flag;
			TSource result = source.TryGetLast(predicate, out flag);
			if (!flag)
			{
				throw Error.NoMatch();
			}
			return result;
		}

		/// <summary>Returns the last element of a sequence, or a default value if the sequence contains no elements.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the last element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="default" />(<paramref name="TSource" />) if the source sequence is empty; otherwise, the last element in the <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			bool flag;
			return source.TryGetLast(out flag);
		}

		/// <summary>Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>
		///     <see langword="default" />(<paramref name="TSource" />) if the sequence is empty or if no elements pass the test in the predicate function; otherwise, the last element that passes the test in the predicate function.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			bool flag;
			return source.TryGetLast(predicate, out flag);
		}

		private static TSource TryGetLast<TSource>(this IEnumerable<TSource> source, out bool found)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			IPartition<TSource> partition = source as IPartition<TSource>;
			if (partition != null)
			{
				return partition.TryGetLast(out found);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				int count = list.Count;
				if (count > 0)
				{
					found = true;
					return list[count - 1];
				}
			}
			else
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						TSource result;
						do
						{
							result = enumerator.Current;
						}
						while (enumerator.MoveNext());
						found = true;
						return result;
					}
				}
			}
			found = false;
			return default(TSource);
		}

		private static TSource TryGetLast<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			OrderedEnumerable<TSource> orderedEnumerable = source as OrderedEnumerable<TSource>;
			if (orderedEnumerable != null)
			{
				return orderedEnumerable.TryGetLast(predicate, out found);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					TSource tsource = list[i];
					if (predicate(tsource))
					{
						found = true;
						return tsource;
					}
				}
			}
			else
			{
				foreach (TSource tsource2 in source)
				{
					if (predicate(tsource2))
					{
						IEnumerator<TSource> enumerator;
						while (enumerator.MoveNext())
						{
							TSource tsource3 = enumerator.Current;
							if (predicate(tsource3))
							{
								tsource2 = tsource3;
							}
						}
						found = true;
						return tsource2;
					}
				}
			}
			found = false;
			return default(TSource);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Lookup`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function.</summary>
		/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Linq.Lookup`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Linq.Lookup`2" /> that contains keys and values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToLookup(keySelector, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Lookup`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function and key comparer.</summary>
		/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Linq.Lookup`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Linq.Lookup`2" /> that contains keys and values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (keySelector == null)
			{
				throw Error.ArgumentNull("keySelector");
			}
			return Lookup<TKey, TSource>.Create(source, keySelector, comparer);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Lookup`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to specified key selector and element selector functions.</summary>
		/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Linq.Lookup`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Linq.Lookup`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is <see langword="null" />.</exception>
		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToLookup(keySelector, elementSelector, null);
		}

		/// <summary>Creates a <see cref="T:System.Linq.Lookup`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function, a comparer and an element selector function.</summary>
		/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Linq.Lookup`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Linq.Lookup`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is <see langword="null" />.</exception>
		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (keySelector == null)
			{
				throw Error.ArgumentNull("keySelector");
			}
			if (elementSelector == null)
			{
				throw Error.ArgumentNull("elementSelector");
			}
			return Lookup<TKey, TElement>.Create<TSource>(source, keySelector, elementSelector, comparer);
		}

		/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Int32" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Int32" /> values to determine the maximum value of.</param>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static int Max(this IEnumerable<int> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			int num;
			using (IEnumerator<int> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (enumerator.MoveNext())
				{
					int num2 = enumerator.Current;
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Int32" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int32" /> values to determine the maximum value of.</param>
		/// <returns>A value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the maximum value in the sequence. </returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static int? Max(this IEnumerable<int?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			int? result = null;
			using (IEnumerator<int?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						int num = result.GetValueOrDefault();
						if (num >= 0)
						{
							while (enumerator.MoveNext())
							{
								int? num2 = enumerator.Current;
								int valueOrDefault = num2.GetValueOrDefault();
								if (valueOrDefault > num)
								{
									num = valueOrDefault;
									result = num2;
								}
							}
							return result;
						}
						while (enumerator.MoveNext())
						{
							int? num3 = enumerator.Current;
							int valueOrDefault2 = num3.GetValueOrDefault();
							if (num3 != null & valueOrDefault2 > num)
							{
								num = valueOrDefault2;
								result = num3;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Int64" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Int64" /> values to determine the maximum value of.</param>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static long Max(this IEnumerable<long> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			long num;
			using (IEnumerator<long> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (enumerator.MoveNext())
				{
					long num2 = enumerator.Current;
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Int64" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int64" /> values to determine the maximum value of.</param>
		/// <returns>A value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the maximum value in the sequence. </returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static long? Max(this IEnumerable<long?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			long? result = null;
			using (IEnumerator<long?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						long num = result.GetValueOrDefault();
						if (num >= 0L)
						{
							while (enumerator.MoveNext())
							{
								long? num2 = enumerator.Current;
								long valueOrDefault = num2.GetValueOrDefault();
								if (valueOrDefault > num)
								{
									num = valueOrDefault;
									result = num2;
								}
							}
							return result;
						}
						while (enumerator.MoveNext())
						{
							long? num3 = enumerator.Current;
							long valueOrDefault2 = num3.GetValueOrDefault();
							if (num3 != null & valueOrDefault2 > num)
							{
								num = valueOrDefault2;
								result = num3;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Double" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Double" /> values to determine the maximum value of.</param>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static double Max(this IEnumerable<double> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double num;
			using (IEnumerator<double> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (double.IsNaN(num))
				{
					if (!enumerator.MoveNext())
					{
						return num;
					}
					num = enumerator.Current;
				}
				while (enumerator.MoveNext())
				{
					double num2 = enumerator.Current;
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Double" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Double" /> values to determine the maximum value of.</param>
		/// <returns>A value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static double? Max(this IEnumerable<double?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double? result = null;
			using (IEnumerator<double?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						double num = result.GetValueOrDefault();
						while (double.IsNaN(num))
						{
							if (!enumerator.MoveNext())
							{
								return result;
							}
							double? num2 = enumerator.Current;
							if (num2 != null)
							{
								double? num3;
								result = (num3 = num2);
								num = num3.GetValueOrDefault();
							}
						}
						while (enumerator.MoveNext())
						{
							double? num4 = enumerator.Current;
							double valueOrDefault = num4.GetValueOrDefault();
							if (num4 != null & valueOrDefault > num)
							{
								num = valueOrDefault;
								result = num4;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Single" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Single" /> values to determine the maximum value of.</param>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static float Max(this IEnumerable<float> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			float num;
			using (IEnumerator<float> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (float.IsNaN(num))
				{
					if (!enumerator.MoveNext())
					{
						return num;
					}
					num = enumerator.Current;
				}
				while (enumerator.MoveNext())
				{
					float num2 = enumerator.Current;
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Single" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Single" /> values to determine the maximum value of.</param>
		/// <returns>A value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static float? Max(this IEnumerable<float?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			float? result = null;
			using (IEnumerator<float?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						float num = result.GetValueOrDefault();
						while (float.IsNaN(num))
						{
							if (!enumerator.MoveNext())
							{
								return result;
							}
							float? num2 = enumerator.Current;
							if (num2 != null)
							{
								float? num3;
								result = (num3 = num2);
								num = num3.GetValueOrDefault();
							}
						}
						while (enumerator.MoveNext())
						{
							float? num4 = enumerator.Current;
							float valueOrDefault = num4.GetValueOrDefault();
							if (num4 != null & valueOrDefault > num)
							{
								num = valueOrDefault;
								result = num4;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Decimal" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Decimal" /> values to determine the maximum value of.</param>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static decimal Max(this IEnumerable<decimal> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			decimal num;
			using (IEnumerator<decimal> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (enumerator.MoveNext())
				{
					decimal num2 = enumerator.Current;
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Decimal" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Decimal" /> values to determine the maximum value of.</param>
		/// <returns>A value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the maximum value in the sequence. </returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static decimal? Max(this IEnumerable<decimal?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			decimal? result = null;
			using (IEnumerator<decimal?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						decimal d = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							decimal? num = enumerator.Current;
							decimal valueOrDefault = num.GetValueOrDefault();
							if (num != null && valueOrDefault > d)
							{
								d = valueOrDefault;
								result = num;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the maximum value in a generic sequence.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static TSource Max<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			Comparer<TSource> @default = Comparer<TSource>.Default;
			TSource tsource = default(TSource);
			if (tsource == null)
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						tsource = enumerator.Current;
						if (tsource != null)
						{
							while (enumerator.MoveNext())
							{
								TSource tsource2 = enumerator.Current;
								if (tsource2 != null && @default.Compare(tsource2, tsource) > 0)
								{
									tsource = tsource2;
								}
							}
							return tsource;
						}
					}
					return tsource;
				}
			}
			using (IEnumerator<TSource> enumerator2 = source.GetEnumerator())
			{
				if (!enumerator2.MoveNext())
				{
					throw Error.NoElements();
				}
				tsource = enumerator2.Current;
				while (enumerator2.MoveNext())
				{
					TSource tsource3 = enumerator2.Current;
					if (@default.Compare(tsource3, tsource) > 0)
					{
						tsource = tsource3;
					}
				}
			}
			return tsource;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Int32" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			int num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					int num2 = selector(arg);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Int32" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			int? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						int num = result.GetValueOrDefault();
						if (num >= 0)
						{
							while (enumerator.MoveNext())
							{
								TSource arg2 = enumerator.Current;
								int? num2 = selector(arg2);
								int valueOrDefault = num2.GetValueOrDefault();
								if (valueOrDefault > num)
								{
									num = valueOrDefault;
									result = num2;
								}
							}
							return result;
						}
						while (enumerator.MoveNext())
						{
							TSource arg3 = enumerator.Current;
							int? num3 = selector(arg3);
							int valueOrDefault2 = num3.GetValueOrDefault();
							if (num3 != null & valueOrDefault2 > num)
							{
								num = valueOrDefault2;
								result = num3;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Int64" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			long num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					long num2 = selector(arg);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Int64" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			long? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						long num = result.GetValueOrDefault();
						if (num >= 0L)
						{
							while (enumerator.MoveNext())
							{
								TSource arg2 = enumerator.Current;
								long? num2 = selector(arg2);
								long valueOrDefault = num2.GetValueOrDefault();
								if (valueOrDefault > num)
								{
									num = valueOrDefault;
									result = num2;
								}
							}
							return result;
						}
						while (enumerator.MoveNext())
						{
							TSource arg3 = enumerator.Current;
							long? num3 = selector(arg3);
							long valueOrDefault2 = num3.GetValueOrDefault();
							if (num3 != null & valueOrDefault2 > num)
							{
								num = valueOrDefault2;
								result = num3;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Single" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			float num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (float.IsNaN(num))
				{
					if (!enumerator.MoveNext())
					{
						return num;
					}
					num = selector(enumerator.Current);
				}
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					float num2 = selector(arg);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Single" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			float? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						float num = result.GetValueOrDefault();
						while (float.IsNaN(num))
						{
							if (!enumerator.MoveNext())
							{
								return result;
							}
							float? num2 = selector(enumerator.Current);
							if (num2 != null)
							{
								float? num3;
								result = (num3 = num2);
								num = num3.GetValueOrDefault();
							}
						}
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							float? num4 = selector(arg2);
							float valueOrDefault = num4.GetValueOrDefault();
							if (num4 != null & valueOrDefault > num)
							{
								num = valueOrDefault;
								result = num4;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Double" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (double.IsNaN(num))
				{
					if (!enumerator.MoveNext())
					{
						return num;
					}
					num = selector(enumerator.Current);
				}
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					double num2 = selector(arg);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Double" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						double num = result.GetValueOrDefault();
						while (double.IsNaN(num))
						{
							if (!enumerator.MoveNext())
							{
								return result;
							}
							double? num2 = selector(enumerator.Current);
							if (num2 != null)
							{
								double? num3;
								result = (num3 = num2);
								num = num3.GetValueOrDefault();
							}
						}
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							double? num4 = selector(arg2);
							double valueOrDefault = num4.GetValueOrDefault();
							if (num4 != null & valueOrDefault > num)
							{
								num = valueOrDefault;
								result = num4;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Decimal" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			decimal num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					decimal num2 = selector(arg);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Decimal" /> value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			decimal? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						decimal d = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							decimal? num = selector(arg2);
							decimal valueOrDefault = num.GetValueOrDefault();
							if (num != null && valueOrDefault > d)
							{
								d = valueOrDefault;
								result = num;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a generic sequence and returns the maximum resulting value.</summary>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
		/// <returns>The maximum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			Comparer<TResult> @default = Comparer<TResult>.Default;
			TResult tresult = default(TResult);
			if (tresult == null)
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TSource arg = enumerator.Current;
						tresult = selector(arg);
						if (tresult != null)
						{
							while (enumerator.MoveNext())
							{
								TSource arg2 = enumerator.Current;
								TResult tresult2 = selector(arg2);
								if (tresult2 != null && @default.Compare(tresult2, tresult) > 0)
								{
									tresult = tresult2;
								}
							}
							return tresult;
						}
					}
					return tresult;
				}
			}
			using (IEnumerator<TSource> enumerator2 = source.GetEnumerator())
			{
				if (!enumerator2.MoveNext())
				{
					throw Error.NoElements();
				}
				tresult = selector(enumerator2.Current);
				while (enumerator2.MoveNext())
				{
					TSource arg3 = enumerator2.Current;
					TResult tresult3 = selector(arg3);
					if (@default.Compare(tresult3, tresult) > 0)
					{
						tresult = tresult3;
					}
				}
			}
			return tresult;
		}

		/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Int32" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Int32" /> values to determine the minimum value of.</param>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static int Min(this IEnumerable<int> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			int num;
			using (IEnumerator<int> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (enumerator.MoveNext())
				{
					int num2 = enumerator.Current;
					if (num2 < num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Int32" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int32" /> values to determine the minimum value of.</param>
		/// <returns>A value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static int? Min(this IEnumerable<int?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			int? result = null;
			using (IEnumerator<int?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						int num = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							int? num2 = enumerator.Current;
							int valueOrDefault = num2.GetValueOrDefault();
							if (num2 != null & valueOrDefault < num)
							{
								num = valueOrDefault;
								result = num2;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Int64" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Int64" /> values to determine the minimum value of.</param>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static long Min(this IEnumerable<long> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			long num;
			using (IEnumerator<long> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (enumerator.MoveNext())
				{
					long num2 = enumerator.Current;
					if (num2 < num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Int64" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int64" /> values to determine the minimum value of.</param>
		/// <returns>A value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static long? Min(this IEnumerable<long?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			long? result = null;
			using (IEnumerator<long?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						long num = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							long? num2 = enumerator.Current;
							long valueOrDefault = num2.GetValueOrDefault();
							if (num2 != null & valueOrDefault < num)
							{
								num = valueOrDefault;
								result = num2;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Single" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Single" /> values to determine the minimum value of.</param>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static float Min(this IEnumerable<float> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			float num;
			using (IEnumerator<float> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (enumerator.MoveNext())
				{
					float num2 = enumerator.Current;
					if (num2 < num)
					{
						num = num2;
					}
					else if (float.IsNaN(num2))
					{
						return num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Single" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Single" /> values to determine the minimum value of.</param>
		/// <returns>A value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static float? Min(this IEnumerable<float?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			float? result = null;
			using (IEnumerator<float?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						float num = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							float? num2 = enumerator.Current;
							if (num2 != null)
							{
								float valueOrDefault = num2.GetValueOrDefault();
								if (valueOrDefault < num)
								{
									num = valueOrDefault;
									result = num2;
								}
								else if (float.IsNaN(valueOrDefault))
								{
									return num2;
								}
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Double" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Double" /> values to determine the minimum value of.</param>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static double Min(this IEnumerable<double> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double num;
			using (IEnumerator<double> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (enumerator.MoveNext())
				{
					double num2 = enumerator.Current;
					if (num2 < num)
					{
						num = num2;
					}
					else if (double.IsNaN(num2))
					{
						return num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Double" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Double" /> values to determine the minimum value of.</param>
		/// <returns>A value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static double? Min(this IEnumerable<double?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double? result = null;
			using (IEnumerator<double?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						double num = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							double? num2 = enumerator.Current;
							if (num2 != null)
							{
								double valueOrDefault = num2.GetValueOrDefault();
								if (valueOrDefault < num)
								{
									num = valueOrDefault;
									result = num2;
								}
								else if (double.IsNaN(valueOrDefault))
								{
									return num2;
								}
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Decimal" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Decimal" /> values to determine the minimum value of.</param>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static decimal Min(this IEnumerable<decimal> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			decimal num;
			using (IEnumerator<decimal> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = enumerator.Current;
				while (enumerator.MoveNext())
				{
					decimal num2 = enumerator.Current;
					if (num2 < num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Decimal" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Decimal" /> values to determine the minimum value of.</param>
		/// <returns>A value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static decimal? Min(this IEnumerable<decimal?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			decimal? result = null;
			using (IEnumerator<decimal?> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
					if (result != null)
					{
						decimal d = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							decimal? num = enumerator.Current;
							decimal valueOrDefault = num.GetValueOrDefault();
							if (num != null && valueOrDefault < d)
							{
								d = valueOrDefault;
								result = num;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Returns the minimum value in a generic sequence.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static TSource Min<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			Comparer<TSource> @default = Comparer<TSource>.Default;
			TSource tsource = default(TSource);
			if (tsource == null)
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						tsource = enumerator.Current;
						if (tsource != null)
						{
							while (enumerator.MoveNext())
							{
								TSource tsource2 = enumerator.Current;
								if (tsource2 != null && @default.Compare(tsource2, tsource) < 0)
								{
									tsource = tsource2;
								}
							}
							return tsource;
						}
					}
					return tsource;
				}
			}
			using (IEnumerator<TSource> enumerator2 = source.GetEnumerator())
			{
				if (!enumerator2.MoveNext())
				{
					throw Error.NoElements();
				}
				tsource = enumerator2.Current;
				while (enumerator2.MoveNext())
				{
					TSource tsource3 = enumerator2.Current;
					if (@default.Compare(tsource3, tsource) < 0)
					{
						tsource = tsource3;
					}
				}
			}
			return tsource;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Int32" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			int num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					int num2 = selector(arg);
					if (num2 < num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Int32" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			int? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						int num = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							int? num2 = selector(arg2);
							int valueOrDefault = num2.GetValueOrDefault();
							if (num2 != null & valueOrDefault < num)
							{
								num = valueOrDefault;
								result = num2;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Int64" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			long num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					long num2 = selector(arg);
					if (num2 < num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Int64" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			long? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						long num = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							long? num2 = selector(arg2);
							long valueOrDefault = num2.GetValueOrDefault();
							if (num2 != null & valueOrDefault < num)
							{
								num = valueOrDefault;
								result = num2;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Single" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			float num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					float num2 = selector(arg);
					if (num2 < num)
					{
						num = num2;
					}
					else if (float.IsNaN(num2))
					{
						return num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Single" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			float? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						float num = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							float? num2 = selector(arg2);
							if (num2 != null)
							{
								float valueOrDefault = num2.GetValueOrDefault();
								if (valueOrDefault < num)
								{
									num = valueOrDefault;
									result = num2;
								}
								else if (float.IsNaN(valueOrDefault))
								{
									return num2;
								}
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Double" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					double num2 = selector(arg);
					if (num2 < num)
					{
						num = num2;
					}
					else if (double.IsNaN(num2))
					{
						return num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Double" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						double num = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							double? num2 = selector(arg2);
							if (num2 != null)
							{
								double valueOrDefault = num2.GetValueOrDefault();
								if (valueOrDefault < num)
								{
									num = valueOrDefault;
									result = num2;
								}
								else if (double.IsNaN(valueOrDefault))
								{
									return num2;
								}
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Decimal" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <paramref name="source" /> contains no elements.</exception>
		public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			decimal num;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw Error.NoElements();
				}
				num = selector(enumerator.Current);
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					decimal num2 = selector(arg);
					if (num2 < num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Decimal" /> value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			decimal? result = null;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TSource arg = enumerator.Current;
					result = selector(arg);
					if (result != null)
					{
						decimal d = result.GetValueOrDefault();
						while (enumerator.MoveNext())
						{
							TSource arg2 = enumerator.Current;
							decimal? num = selector(arg2);
							decimal valueOrDefault = num.GetValueOrDefault();
							if (num != null && valueOrDefault < d)
							{
								d = valueOrDefault;
								result = num;
							}
						}
						return result;
					}
				}
				return result;
			}
			return result;
		}

		/// <summary>Invokes a transform function on each element of a generic sequence and returns the minimum resulting value.</summary>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
		/// <returns>The minimum value in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			Comparer<TResult> @default = Comparer<TResult>.Default;
			TResult tresult = default(TResult);
			if (tresult == null)
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TSource arg = enumerator.Current;
						tresult = selector(arg);
						if (tresult != null)
						{
							while (enumerator.MoveNext())
							{
								TSource arg2 = enumerator.Current;
								TResult tresult2 = selector(arg2);
								if (tresult2 != null && @default.Compare(tresult2, tresult) < 0)
								{
									tresult = tresult2;
								}
							}
							return tresult;
						}
					}
					return tresult;
				}
			}
			using (IEnumerator<TSource> enumerator2 = source.GetEnumerator())
			{
				if (!enumerator2.MoveNext())
				{
					throw Error.NoElements();
				}
				tresult = selector(enumerator2.Current);
				while (enumerator2.MoveNext())
				{
					TSource arg3 = enumerator2.Current;
					TResult tresult3 = selector(arg3);
					if (@default.Compare(tresult3, tresult) < 0)
					{
						tresult = tresult3;
					}
				}
			}
			return tresult;
		}

		/// <summary>Sorts the elements of a sequence in ascending order according to a key.</summary>
		/// <param name="source">A sequence of values to order.</param>
		/// <param name="keySelector">A function to extract a key from an element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return new OrderedEnumerable<TSource, TKey>(source, keySelector, null, false, null);
		}

		/// <summary>Sorts the elements of a sequence in ascending order by using a specified comparer.</summary>
		/// <param name="source">A sequence of values to order.</param>
		/// <param name="keySelector">A function to extract a key from an element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, false, null);
		}

		/// <summary>Sorts the elements of a sequence in descending order according to a key.</summary>
		/// <param name="source">A sequence of values to order.</param>
		/// <param name="keySelector">A function to extract a key from an element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted in descending order according to a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return new OrderedEnumerable<TSource, TKey>(source, keySelector, null, true, null);
		}

		/// <summary>Sorts the elements of a sequence in descending order by using a specified comparer.</summary>
		/// <param name="source">A sequence of values to order.</param>
		/// <param name="keySelector">A function to extract a key from an element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted in descending order according to a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, true, null);
		}

		/// <summary>Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.</summary>
		/// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1" /> that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return source.CreateOrderedEnumerable<TKey>(keySelector, null, false);
		}

		/// <summary>Performs a subsequent ordering of the elements in a sequence in ascending order by using a specified comparer.</summary>
		/// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1" /> that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, false);
		}

		/// <summary>Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.</summary>
		/// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1" /> that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted in descending order according to a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return source.CreateOrderedEnumerable<TKey>(keySelector, null, true);
		}

		/// <summary>Performs a subsequent ordering of the elements in a sequence in descending order by using a specified comparer.</summary>
		/// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1" /> that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted in descending order according to a key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.</exception>
		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, true);
		}

		/// <summary>Generates a sequence of integral numbers within a specified range.</summary>
		/// <param name="start">The value of the first integer in the sequence.</param>
		/// <param name="count">The number of sequential integers to generate.</param>
		/// <returns>An IEnumerable&lt;Int32&gt; in C# or IEnumerable(Of Int32) in Visual Basic that contains a range of sequential integral numbers.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="count" /> is less than 0.-or-
		///         <paramref name="start" /> + <paramref name="count" /> -1 is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static IEnumerable<int> Range(int start, int count)
		{
			long num = (long)start + (long)count - 1L;
			if (count < 0 || num > 2147483647L)
			{
				throw Error.ArgumentOutOfRange("count");
			}
			if (count == 0)
			{
				return EmptyPartition<int>.Instance;
			}
			return new Enumerable.RangeIterator(start, count);
		}

		/// <summary>Generates a sequence that contains one repeated value.</summary>
		/// <param name="element">The value to be repeated.</param>
		/// <param name="count">The number of times to repeat the value in the generated sequence.</param>
		/// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains a repeated value.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="count" /> is less than 0.</exception>
		public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
		{
			if (count < 0)
			{
				throw Error.ArgumentOutOfRange("count");
			}
			if (count == 0)
			{
				return EmptyPartition<TResult>.Instance;
			}
			return new Enumerable.RepeatIterator<TResult>(element, count);
		}

		/// <summary>Inverts the order of the elements in a sequence.</summary>
		/// <param name="source">A sequence of values to reverse.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>A sequence whose elements correspond to those of the input sequence in reverse order.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return new Enumerable.ReverseIterator<TSource>(source);
		}

		/// <summary>Projects each element of a sequence into a new form.</summary>
		/// <param name="source">A sequence of values to invoke a transform function on.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the transform function on each element of <paramref name="source" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			Enumerable.Iterator<TSource> iterator = source as Enumerable.Iterator<TSource>;
			if (iterator != null)
			{
				return iterator.Select<TResult>(selector);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				TSource[] array = source as TSource[];
				if (array != null)
				{
					if (array.Length != 0)
					{
						return new Enumerable.SelectArrayIterator<TSource, TResult>(array, selector);
					}
					return EmptyPartition<TResult>.Instance;
				}
				else
				{
					List<TSource> list2 = source as List<TSource>;
					if (list2 != null)
					{
						return new Enumerable.SelectListIterator<TSource, TResult>(list2, selector);
					}
					return new Enumerable.SelectIListIterator<TSource, TResult>(list, selector);
				}
			}
			else
			{
				IPartition<TSource> partition = source as IPartition<TSource>;
				if (partition == null)
				{
					return new Enumerable.SelectEnumerableIterator<TSource, TResult>(source, selector);
				}
				if (!(partition is EmptyPartition<TSource>))
				{
					return new Enumerable.SelectIPartitionIterator<TSource, TResult>(partition, selector);
				}
				return EmptyPartition<TResult>.Instance;
			}
		}

		/// <summary>Projects each element of a sequence into a new form by incorporating the element's index.</summary>
		/// <param name="source">A sequence of values to invoke a transform function on.</param>
		/// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the transform function on each element of <paramref name="source" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			return Enumerable.SelectIterator<TSource, TResult>(source, selector);
		}

		private static IEnumerable<TResult> SelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			int index = -1;
			foreach (TSource arg in source)
			{
				int num = index;
				index = checked(num + 1);
				yield return selector(arg, index);
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1" /> and flattens the resulting sequences into one sequence.</summary>
		/// <param name="source">A sequence of values to project.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the elements of the sequence returned by <paramref name="selector" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			return new Enumerable.SelectManySingleSelectorIterator<TSource, TResult>(source, selector);
		}

		/// <summary>Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1" />, and flattens the resulting sequences into one sequence. The index of each source element is used in the projected form of that element.</summary>
		/// <param name="source">A sequence of values to project.</param>
		/// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the elements of the sequence returned by <paramref name="selector" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the one-to-many transform function on each element of an input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			return Enumerable.SelectManyIterator<TSource, TResult>(source, selector);
		}

		private static IEnumerable<TResult> SelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			int index = -1;
			foreach (TSource arg in source)
			{
				int num = index;
				index = checked(num + 1);
				foreach (TResult tresult in selector(arg, index))
				{
					yield return tresult;
				}
				IEnumerator<TResult> enumerator2 = null;
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1" />, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein. The index of each source element is used in the intermediate projected form of that element.</summary>
		/// <param name="source">A sequence of values to project.</param>
		/// <param name="collectionSelector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
		/// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TCollection">The type of the intermediate elements collected by <paramref name="collectionSelector" />.</typeparam>
		/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the one-to-many transform function <paramref name="collectionSelector" /> on each element of <paramref name="source" /> and then mapping each of those sequence elements and their corresponding source element to a result element.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="collectionSelector" /> or <paramref name="resultSelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (collectionSelector == null)
			{
				throw Error.ArgumentNull("collectionSelector");
			}
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			return Enumerable.SelectManyIterator<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		private static IEnumerable<TResult> SelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			int index = -1;
			foreach (TSource element in source)
			{
				int num = index;
				index = checked(num + 1);
				foreach (TCollection arg in collectionSelector(element, index))
				{
					yield return resultSelector(element, arg);
				}
				IEnumerator<TCollection> enumerator2 = null;
				element = default(TSource);
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1" />, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.</summary>
		/// <param name="source">A sequence of values to project.</param>
		/// <param name="collectionSelector">A transform function to apply to each element of the input sequence.</param>
		/// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TCollection">The type of the intermediate elements collected by <paramref name="collectionSelector" />.</typeparam>
		/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the one-to-many transform function <paramref name="collectionSelector" /> on each element of <paramref name="source" /> and then mapping each of those sequence elements and their corresponding source element to a result element.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="collectionSelector" /> or <paramref name="resultSelector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (collectionSelector == null)
			{
				throw Error.ArgumentNull("collectionSelector");
			}
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			return Enumerable.SelectManyIterator<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		private static IEnumerable<TResult> SelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			foreach (TSource element in source)
			{
				foreach (TCollection arg in collectionSelector(element))
				{
					yield return resultSelector(element, arg);
				}
				IEnumerator<TCollection> enumerator2 = null;
				element = default(TSource);
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.</summary>
		/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to compare to <paramref name="second" />.</param>
		/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to compare to the first sequence.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>
		///     <see langword="true" /> if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.SequenceEqual(second, null);
		}

		/// <summary>Determines whether two sequences are equal by comparing their elements by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
		/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to compare to <paramref name="second" />.</param>
		/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to compare to the first sequence.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to use to compare elements.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>
		///     <see langword="true" /> if the two source sequences are of equal length and their corresponding elements compare equal according to <paramref name="comparer" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if (first == null)
			{
				throw Error.ArgumentNull("first");
			}
			if (second == null)
			{
				throw Error.ArgumentNull("second");
			}
			ICollection<TSource> collection = first as ICollection<TSource>;
			if (collection != null)
			{
				ICollection<TSource> collection2 = second as ICollection<TSource>;
				if (collection2 != null)
				{
					if (collection.Count != collection2.Count)
					{
						return false;
					}
					IList<TSource> list = collection as IList<TSource>;
					if (list != null)
					{
						IList<TSource> list2 = collection2 as IList<TSource>;
						if (list2 != null)
						{
							int count = collection.Count;
							for (int i = 0; i < count; i++)
							{
								if (!comparer.Equals(list[i], list2[i]))
								{
									return false;
								}
							}
							return true;
						}
					}
				}
			}
			bool result;
			using (IEnumerator<TSource> enumerator = first.GetEnumerator())
			{
				using (IEnumerator<TSource> enumerator2 = second.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator2.MoveNext() || !comparer.Equals(enumerator.Current, enumerator2.Current))
						{
							return false;
						}
					}
					result = !enumerator2.MoveNext();
				}
			}
			return result;
		}

		/// <summary>Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the single element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The single element of the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.-or-The input sequence is empty.</exception>
		public static TSource Single<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				int count = list.Count;
				if (count == 0)
				{
					throw Error.NoElements();
				}
				if (count == 1)
				{
					return list[0];
				}
			}
			else
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						throw Error.NoElements();
					}
					TSource result = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						return result;
					}
				}
			}
			throw Error.MoreThanOneElement();
		}

		/// <summary>Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return a single element from.</param>
		/// <param name="predicate">A function to test an element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The single element of the input sequence that satisfies a condition.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate" />.-or-More than one element satisfies the condition in <paramref name="predicate" />.-or-The source sequence is empty.</exception>
		public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			foreach (TSource tsource in source)
			{
				if (predicate(tsource))
				{
					IEnumerator<TSource> enumerator;
					while (enumerator.MoveNext())
					{
						if (predicate(enumerator.Current))
						{
							throw Error.MoreThanOneMatch();
						}
					}
					return tsource;
				}
			}
			throw Error.NoMatch();
		}

		/// <summary>Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the single element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The single element of the input sequence, or <see langword="default" />(<paramref name="TSource" />) if the sequence contains no elements.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				int count = list.Count;
				if (count == 0)
				{
					TSource result = default(TSource);
					return result;
				}
				if (count == 1)
				{
					return list[0];
				}
			}
			else
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						TSource result = default(TSource);
						return result;
					}
					TSource result2 = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						return result2;
					}
				}
			}
			throw Error.MoreThanOneElement();
		}

		/// <summary>Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return a single element from.</param>
		/// <param name="predicate">A function to test an element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The single element of the input sequence that satisfies the condition, or <see langword="default" />(<paramref name="TSource" />) if no such element is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			foreach (TSource tsource in source)
			{
				if (predicate(tsource))
				{
					IEnumerator<TSource> enumerator;
					while (enumerator.MoveNext())
					{
						if (predicate(enumerator.Current))
						{
							throw Error.MoreThanOneMatch();
						}
					}
					return tsource;
				}
			}
			return default(TSource);
		}

		/// <summary>Bypasses a specified number of elements in a sequence and then returns the remaining elements.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return elements from.</param>
		/// <param name="count">The number of elements to skip before returning the remaining elements.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements that occur after the specified index in the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (count <= 0)
			{
				if (source is Enumerable.Iterator<TSource> || source is IPartition<TSource>)
				{
					return source;
				}
				count = 0;
			}
			else
			{
				IPartition<TSource> partition = source as IPartition<TSource>;
				if (partition != null)
				{
					return partition.Skip(count);
				}
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return new Enumerable.ListPartition<TSource>(list, count, int.MaxValue);
			}
			return new Enumerable.EnumerablePartition<TSource>(source, count, -1);
		}

		/// <summary>Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return elements from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from the input sequence starting at the first element in the linear series that does not pass the test specified by <paramref name="predicate" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			return Enumerable.SkipWhileIterator<TSource>(source, predicate);
		}

		private static IEnumerable<TSource> SkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			IEnumerator<TSource> e;
			foreach (TSource tsource in source)
			{
				if (!predicate(tsource))
				{
					yield return tsource;
					while (e.MoveNext())
					{
						TSource tsource2 = e.Current;
						yield return tsource2;
					}
					yield break;
				}
			}
			e = null;
			yield break;
			yield break;
		}

		/// <summary>Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements. The element's index is used in the logic of the predicate function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return elements from.</param>
		/// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from the input sequence starting at the first element in the linear series that does not pass the test specified by <paramref name="predicate" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			return Enumerable.SkipWhileIterator<TSource>(source, predicate);
		}

		private static IEnumerable<TSource> SkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			checked
			{
				using (IEnumerator<TSource> e = source.GetEnumerator())
				{
					int num = -1;
					while (e.MoveNext())
					{
						num++;
						TSource tsource = e.Current;
						if (!predicate(tsource, num))
						{
							yield return tsource;
							while (e.MoveNext())
							{
								TSource tsource2 = e.Current;
								yield return tsource2;
							}
							yield break;
						}
					}
				}
				IEnumerator<TSource> e = null;
				yield break;
				yield break;
			}
		}

		public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (count <= 0)
			{
				return source.Skip(0);
			}
			return Enumerable.SkipLastIterator<TSource>(source, count);
		}

		private static IEnumerable<TSource> SkipLastIterator<TSource>(IEnumerable<TSource> source, int count)
		{
			Queue<TSource> queue = new Queue<TSource>();
			using (IEnumerator<TSource> e = source.GetEnumerator())
			{
				while (e.MoveNext())
				{
					if (queue.Count == count)
					{
						do
						{
							yield return queue.Dequeue();
							queue.Enqueue(e.Current);
						}
						while (e.MoveNext());
						break;
					}
					queue.Enqueue(e.Current);
				}
			}
			IEnumerator<TSource> e = null;
			yield break;
			yield break;
		}

		/// <summary>Computes the sum of a sequence of <see cref="T:System.Int32" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Int32" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int Sum(this IEnumerable<int> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			int num = 0;
			checked
			{
				foreach (int num2 in source)
				{
					num += num2;
				}
				return num;
			}
		}

		/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Int32" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int32" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int? Sum(this IEnumerable<int?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			int num = 0;
			checked
			{
				foreach (int? num2 in source)
				{
					if (num2 != null)
					{
						num += num2.GetValueOrDefault();
					}
				}
				return new int?(num);
			}
		}

		/// <summary>Computes the sum of a sequence of <see cref="T:System.Int64" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Int64" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long Sum(this IEnumerable<long> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			long num = 0L;
			checked
			{
				foreach (long num2 in source)
				{
					num += num2;
				}
				return num;
			}
		}

		/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Int64" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int64" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long? Sum(this IEnumerable<long?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			long num = 0L;
			checked
			{
				foreach (long? num2 in source)
				{
					if (num2 != null)
					{
						num += num2.GetValueOrDefault();
					}
				}
				return new long?(num);
			}
		}

		/// <summary>Computes the sum of a sequence of <see cref="T:System.Single" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Single" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static float Sum(this IEnumerable<float> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double num = 0.0;
			foreach (float num2 in source)
			{
				num += (double)num2;
			}
			return (float)num;
		}

		/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Single" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Single" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static float? Sum(this IEnumerable<float?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double num = 0.0;
			foreach (float? num2 in source)
			{
				if (num2 != null)
				{
					num += (double)num2.GetValueOrDefault();
				}
			}
			return new float?((float)num);
		}

		/// <summary>Computes the sum of a sequence of <see cref="T:System.Double" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Double" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static double Sum(this IEnumerable<double> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double num = 0.0;
			foreach (double num2 in source)
			{
				num += num2;
			}
			return num;
		}

		/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Double" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Double" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static double? Sum(this IEnumerable<double?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			double num = 0.0;
			foreach (double? num2 in source)
			{
				if (num2 != null)
				{
					num += num2.GetValueOrDefault();
				}
			}
			return new double?(num);
		}

		/// <summary>Computes the sum of a sequence of <see cref="T:System.Decimal" /> values.</summary>
		/// <param name="source">A sequence of <see cref="T:System.Decimal" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal Sum(this IEnumerable<decimal> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			decimal num = 0m;
			foreach (decimal d in source)
			{
				num += d;
			}
			return num;
		}

		/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Decimal" /> values.</summary>
		/// <param name="source">A sequence of nullable <see cref="T:System.Decimal" /> values to calculate the sum of.</param>
		/// <returns>The sum of the values in the sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal? Sum(this IEnumerable<decimal?> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			decimal num = 0m;
			foreach (decimal? num2 in source)
			{
				if (num2 != null)
				{
					num += num2.GetValueOrDefault();
				}
			}
			return new decimal?(num);
		}

		/// <summary>Computes the sum of the sequence of <see cref="T:System.Int32" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			int num = 0;
			checked
			{
				foreach (TSource arg in source)
				{
					num += selector(arg);
				}
				return num;
			}
		}

		/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Int32" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			int num = 0;
			checked
			{
				foreach (TSource arg in source)
				{
					int? num2 = selector(arg);
					if (num2 != null)
					{
						num += num2.GetValueOrDefault();
					}
				}
				return new int?(num);
			}
		}

		/// <summary>Computes the sum of the sequence of <see cref="T:System.Int64" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			long num = 0L;
			checked
			{
				foreach (TSource arg in source)
				{
					num += selector(arg);
				}
				return num;
			}
		}

		/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Int64" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			long num = 0L;
			checked
			{
				foreach (TSource arg in source)
				{
					long? num2 = selector(arg);
					if (num2 != null)
					{
						num += num2.GetValueOrDefault();
					}
				}
				return new long?(num);
			}
		}

		/// <summary>Computes the sum of the sequence of <see cref="T:System.Single" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double num = 0.0;
			foreach (TSource arg in source)
			{
				num += (double)selector(arg);
			}
			return (float)num;
		}

		/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Single" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double num = 0.0;
			foreach (TSource arg in source)
			{
				float? num2 = selector(arg);
				if (num2 != null)
				{
					num += (double)num2.GetValueOrDefault();
				}
			}
			return new float?((float)num);
		}

		/// <summary>Computes the sum of the sequence of <see cref="T:System.Double" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double num = 0.0;
			foreach (TSource arg in source)
			{
				num += selector(arg);
			}
			return num;
		}

		/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Double" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			double num = 0.0;
			foreach (TSource arg in source)
			{
				double? num2 = selector(arg);
				if (num2 != null)
				{
					num += num2.GetValueOrDefault();
				}
			}
			return new double?(num);
		}

		/// <summary>Computes the sum of the sequence of <see cref="T:System.Decimal" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			decimal num = 0m;
			foreach (TSource arg in source)
			{
				num += selector(arg);
			}
			return num;
		}

		/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Decimal" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
		/// <param name="source">A sequence of values that are used to calculate a sum.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>The sum of the projected values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (selector == null)
			{
				throw Error.ArgumentNull("selector");
			}
			decimal num = 0m;
			foreach (TSource arg in source)
			{
				decimal? num2 = selector(arg);
				if (num2 != null)
				{
					num += num2.GetValueOrDefault();
				}
			}
			return new decimal?(num);
		}

		/// <summary>Returns a specified number of contiguous elements from the start of a sequence.</summary>
		/// <param name="source">The sequence to return elements from.</param>
		/// <param name="count">The number of elements to return.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the specified number of elements from the start of the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (count <= 0)
			{
				return EmptyPartition<TSource>.Instance;
			}
			IPartition<TSource> partition = source as IPartition<TSource>;
			if (partition != null)
			{
				return partition.Take(count);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return new Enumerable.ListPartition<TSource>(list, 0, count - 1);
			}
			return new Enumerable.EnumerablePartition<TSource>(source, 0, count - 1);
		}

		/// <summary>Returns elements from a sequence as long as a specified condition is true.</summary>
		/// <param name="source">A sequence to return elements from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from the input sequence that occur before the element at which the test no longer passes.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			return Enumerable.TakeWhileIterator<TSource>(source, predicate);
		}

		private static IEnumerable<TSource> TakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			foreach (TSource tsource in source)
			{
				if (!predicate(tsource))
				{
					break;
				}
				yield return tsource;
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.</summary>
		/// <param name="source">The sequence to return elements from.</param>
		/// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence that occur before the element at which the test no longer passes.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			return Enumerable.TakeWhileIterator<TSource>(source, predicate);
		}

		private static IEnumerable<TSource> TakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int index = -1;
			foreach (TSource tsource in source)
			{
				int num = index;
				index = checked(num + 1);
				if (!predicate(tsource, index))
				{
					break;
				}
				yield return tsource;
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (count <= 0)
			{
				return EmptyPartition<TSource>.Instance;
			}
			return Enumerable.TakeLastIterator<TSource>(source, count);
		}

		private static IEnumerable<TSource> TakeLastIterator<TSource>(IEnumerable<TSource> source, int count)
		{
			Queue<TSource> queue;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					yield break;
				}
				queue = new Queue<TSource>();
				queue.Enqueue(enumerator.Current);
				while (enumerator.MoveNext())
				{
					if (queue.Count >= count)
					{
						do
						{
							queue.Dequeue();
							queue.Enqueue(enumerator.Current);
						}
						while (enumerator.MoveNext());
						break;
					}
					queue.Enqueue(enumerator.Current);
				}
			}
			do
			{
				yield return queue.Dequeue();
			}
			while (queue.Count > 0);
			yield break;
		}

		/// <summary>Creates an array from a <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create an array from.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An array that contains the elements from the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			IIListProvider<TSource> iilistProvider = source as IIListProvider<TSource>;
			if (iilistProvider == null)
			{
				return EnumerableHelpers.ToArray<TSource>(source);
			}
			return iilistProvider.ToArray();
		}

		/// <summary>Creates a <see cref="T:System.Collections.Generic.List`1" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
		/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.List`1" /> from.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>A <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> is <see langword="null" />.</exception>
		public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			IIListProvider<TSource> iilistProvider = source as IIListProvider<TSource>;
			if (iilistProvider == null)
			{
				return new List<TSource>(source);
			}
			return iilistProvider.ToList();
		}

		/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains keys and values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.-or-
		///         <paramref name="keySelector" /> produces a key that is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToDictionary(keySelector, null);
		}

		/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function and key comparer.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by <paramref name="keySelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains keys and values.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> is <see langword="null" />.-or-
		///         <paramref name="keySelector" /> produces a key that is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (keySelector == null)
			{
				throw Error.ArgumentNull("keySelector");
			}
			int num = 0;
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				num = collection.Count;
				if (num == 0)
				{
					return new Dictionary<TKey, TSource>(comparer);
				}
				TSource[] array = collection as TSource[];
				if (array != null)
				{
					return Enumerable.ToDictionary<TSource, TKey>(array, keySelector, comparer);
				}
				List<TSource> list = collection as List<TSource>;
				if (list != null)
				{
					return Enumerable.ToDictionary<TSource, TKey>(list, keySelector, comparer);
				}
			}
			Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(num, comparer);
			foreach (TSource tsource in source)
			{
				dictionary.Add(keySelector(tsource), tsource);
			}
			return dictionary;
		}

		private static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(TSource[] source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(source.Length, comparer);
			for (int i = 0; i < source.Length; i++)
			{
				dictionary.Add(keySelector(source[i]), source[i]);
			}
			return dictionary;
		}

		private static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(List<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(source.Count, comparer);
			foreach (TSource tsource in source)
			{
				dictionary.Add(keySelector(tsource), tsource);
			}
			return dictionary;
		}

		/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to specified key selector and element selector functions.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is <see langword="null" />.-or-
		///         <paramref name="keySelector" /> produces a key that is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToDictionary(keySelector, elementSelector, null);
		}

		/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function, a comparer, and an element selector function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is <see langword="null" />.-or-
		///         <paramref name="keySelector" /> produces a key that is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (keySelector == null)
			{
				throw Error.ArgumentNull("keySelector");
			}
			if (elementSelector == null)
			{
				throw Error.ArgumentNull("elementSelector");
			}
			int num = 0;
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				num = collection.Count;
				if (num == 0)
				{
					return new Dictionary<TKey, TElement>(comparer);
				}
				TSource[] array = collection as TSource[];
				if (array != null)
				{
					return Enumerable.ToDictionary<TSource, TKey, TElement>(array, keySelector, elementSelector, comparer);
				}
				List<TSource> list = collection as List<TSource>;
				if (list != null)
				{
					return Enumerable.ToDictionary<TSource, TKey, TElement>(list, keySelector, elementSelector, comparer);
				}
			}
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(num, comparer);
			foreach (TSource arg in source)
			{
				dictionary.Add(keySelector(arg), elementSelector(arg));
			}
			return dictionary;
		}

		private static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(TSource[] source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(source.Length, comparer);
			for (int i = 0; i < source.Length; i++)
			{
				dictionary.Add(keySelector(source[i]), elementSelector(source[i]));
			}
			return dictionary;
		}

		private static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(List<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(source.Count, comparer);
			foreach (TSource arg in source)
			{
				dictionary.Add(keySelector(arg), elementSelector(arg));
			}
			return dictionary;
		}

		/// <summary>Creates a <see cref="T:System.Collections.Generic.HashSet`1" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
		/// <param name="source">
		///       An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.HashSet`1" /> from.
		///     </param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>A <see cref="T:System.Collections.Generic.HashSet`1" /> that contains values of type TSource selected from the input sequence.</returns>
		public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
		{
			return source.ToHashSet(null);
		}

		/// <summary>Creates a <see cref="T:System.Collections.Generic.HashSet`1" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> using the <paramref name="comparer" /> to compare keys</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.HashSet`1" /> from.</param>
		/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" /></typeparam>
		/// <returns>A <see cref="T:System.Collections.Generic.HashSet`1" /> that contains values of type <paramref name="TSource" /> selected from the input sequence.</returns>
		public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			return new HashSet<TSource>(source, comparer);
		}

		/// <summary>Produces the set union of two sequences by using the default equality comparer.</summary>
		/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the first set for the union.</param>
		/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the second set for the union.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from both input sequences, excluding duplicates.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Union(second, null);
		}

		/// <summary>Produces the set union of two sequences by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
		/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the first set for the union.</param>
		/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the second set for the union.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from both input sequences, excluding duplicates.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if (first == null)
			{
				throw Error.ArgumentNull("first");
			}
			if (second == null)
			{
				throw Error.ArgumentNull("second");
			}
			Enumerable.UnionIterator<TSource> unionIterator = first as Enumerable.UnionIterator<TSource>;
			if (unionIterator == null || !Utilities.AreEqualityComparersEqual<TSource>(comparer, unionIterator._comparer))
			{
				return new Enumerable.UnionIterator2<TSource>(first, second, comparer);
			}
			return unionIterator.Union(second);
		}

		/// <summary>Filters a sequence of values based on a predicate.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to filter.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence that satisfy the condition.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			Enumerable.Iterator<TSource> iterator = source as Enumerable.Iterator<TSource>;
			if (iterator != null)
			{
				return iterator.Where(predicate);
			}
			TSource[] array = source as TSource[];
			if (array != null)
			{
				if (array.Length != 0)
				{
					return new Enumerable.WhereArrayIterator<TSource>(array, predicate);
				}
				return EmptyPartition<TSource>.Instance;
			}
			else
			{
				List<TSource> list = source as List<TSource>;
				if (list != null)
				{
					return new Enumerable.WhereListIterator<TSource>(list, predicate);
				}
				return new Enumerable.WhereEnumerableIterator<TSource>(source, predicate);
			}
		}

		/// <summary>Filters a sequence of values based on a predicate. Each element's index is used in the logic of the predicate function.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to filter.</param>
		/// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence that satisfy the condition.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			if (predicate == null)
			{
				throw Error.ArgumentNull("predicate");
			}
			return Enumerable.WhereIterator<TSource>(source, predicate);
		}

		private static IEnumerable<TSource> WhereIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int index = -1;
			foreach (TSource tsource in source)
			{
				int num = index;
				index = checked(num + 1);
				if (predicate(tsource, index))
				{
					yield return tsource;
				}
			}
			IEnumerator<TSource> enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.</summary>
		/// <param name="first">The first sequence to merge.</param>
		/// <param name="second">The second sequence to merge.</param>
		/// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
		/// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
		/// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
		/// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains merged elements of two input sequences.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			if (first == null)
			{
				throw Error.ArgumentNull("first");
			}
			if (second == null)
			{
				throw Error.ArgumentNull("second");
			}
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			return Enumerable.ZipIterator<TFirst, TSecond, TResult>(first, second, resultSelector);
		}

		private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			using (IEnumerator<TFirst> e = first.GetEnumerator())
			{
				using (IEnumerator<TSecond> e2 = second.GetEnumerator())
				{
					while (e.MoveNext() && e2.MoveNext())
					{
						yield return resultSelector(e.Current, e2.Current);
					}
				}
				IEnumerator<TSecond> e2 = null;
			}
			IEnumerator<TFirst> e = null;
			yield break;
			yield break;
		}

		private abstract class AppendPrependIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			protected AppendPrependIterator(IEnumerable<TSource> source)
			{
				this._source = source;
			}

			protected void GetSourceEnumerator()
			{
				this._enumerator = this._source.GetEnumerator();
			}

			public abstract Enumerable.AppendPrependIterator<TSource> Append(TSource item);

			public abstract Enumerable.AppendPrependIterator<TSource> Prepend(TSource item);

			protected bool LoadFromEnumerator()
			{
				if (this._enumerator.MoveNext())
				{
					this._current = this._enumerator.Current;
					return true;
				}
				this.Dispose();
				return false;
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			public abstract TSource[] ToArray();

			public abstract List<TSource> ToList();

			public abstract int GetCount(bool onlyIfCheap);

			protected readonly IEnumerable<TSource> _source;

			protected IEnumerator<TSource> _enumerator;
		}

		private class AppendPrepend1Iterator<TSource> : Enumerable.AppendPrependIterator<TSource>
		{
			public AppendPrepend1Iterator(IEnumerable<TSource> source, TSource item, bool appending) : base(source)
			{
				this._item = item;
				this._appending = appending;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.AppendPrepend1Iterator<TSource>(this._source, this._item, this._appending);
			}

			public override bool MoveNext()
			{
				switch (this._state)
				{
				case 1:
					this._state = 2;
					if (!this._appending)
					{
						this._current = this._item;
						return true;
					}
					break;
				case 2:
					break;
				case 3:
					goto IL_47;
				default:
					goto IL_67;
				}
				base.GetSourceEnumerator();
				this._state = 3;
				IL_47:
				if (base.LoadFromEnumerator())
				{
					return true;
				}
				if (this._appending)
				{
					this._current = this._item;
					return true;
				}
				IL_67:
				this.Dispose();
				return false;
			}

			public override Enumerable.AppendPrependIterator<TSource> Append(TSource item)
			{
				if (this._appending)
				{
					return new Enumerable.AppendPrependN<TSource>(this._source, null, new SingleLinkedNode<TSource>(this._item).Add(item), 0, 2);
				}
				return new Enumerable.AppendPrependN<TSource>(this._source, new SingleLinkedNode<TSource>(this._item), new SingleLinkedNode<TSource>(item), 1, 1);
			}

			public override Enumerable.AppendPrependIterator<TSource> Prepend(TSource item)
			{
				if (this._appending)
				{
					return new Enumerable.AppendPrependN<TSource>(this._source, new SingleLinkedNode<TSource>(item), new SingleLinkedNode<TSource>(this._item), 1, 1);
				}
				return new Enumerable.AppendPrependN<TSource>(this._source, new SingleLinkedNode<TSource>(this._item).Add(item), null, 2, 0);
			}

			private TSource[] LazyToArray()
			{
				LargeArrayBuilder<TSource> largeArrayBuilder = new LargeArrayBuilder<TSource>(true);
				if (!this._appending)
				{
					largeArrayBuilder.SlowAdd(this._item);
				}
				largeArrayBuilder.AddRange(this._source);
				if (this._appending)
				{
					largeArrayBuilder.SlowAdd(this._item);
				}
				return largeArrayBuilder.ToArray();
			}

			public override TSource[] ToArray()
			{
				int count = this.GetCount(true);
				if (count == -1)
				{
					return this.LazyToArray();
				}
				TSource[] array = new TSource[count];
				int arrayIndex;
				if (this._appending)
				{
					arrayIndex = 0;
				}
				else
				{
					array[0] = this._item;
					arrayIndex = 1;
				}
				EnumerableHelpers.Copy<TSource>(this._source, array, arrayIndex, count - 1);
				if (this._appending)
				{
					array[array.Length - 1] = this._item;
				}
				return array;
			}

			public override List<TSource> ToList()
			{
				int count = this.GetCount(true);
				List<TSource> list = (count == -1) ? new List<TSource>() : new List<TSource>(count);
				if (!this._appending)
				{
					list.Add(this._item);
				}
				list.AddRange(this._source);
				if (this._appending)
				{
					list.Add(this._item);
				}
				return list;
			}

			public override int GetCount(bool onlyIfCheap)
			{
				IIListProvider<TSource> iilistProvider = this._source as IIListProvider<TSource>;
				if (iilistProvider != null)
				{
					int count = iilistProvider.GetCount(onlyIfCheap);
					if (count != -1)
					{
						return count + 1;
					}
					return -1;
				}
				else
				{
					if (onlyIfCheap && !(this._source is ICollection<TSource>))
					{
						return -1;
					}
					return this._source.Count<TSource>() + 1;
				}
			}

			private readonly TSource _item;

			private readonly bool _appending;
		}

		private class AppendPrependN<TSource> : Enumerable.AppendPrependIterator<TSource>
		{
			public AppendPrependN(IEnumerable<TSource> source, SingleLinkedNode<TSource> prepended, SingleLinkedNode<TSource> appended, int prependCount, int appendCount) : base(source)
			{
				this._prepended = prepended;
				this._appended = appended;
				this._prependCount = prependCount;
				this._appendCount = appendCount;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.AppendPrependN<TSource>(this._source, this._prepended, this._appended, this._prependCount, this._appendCount);
			}

			public override bool MoveNext()
			{
				switch (this._state)
				{
				case 1:
					this._node = this._prepended;
					this._state = 2;
					break;
				case 2:
					break;
				case 3:
					goto IL_70;
				case 4:
					goto IL_A2;
				default:
					this.Dispose();
					return false;
				}
				if (this._node != null)
				{
					this._current = this._node.Item;
					this._node = this._node.Linked;
					return true;
				}
				base.GetSourceEnumerator();
				this._state = 3;
				IL_70:
				if (base.LoadFromEnumerator())
				{
					return true;
				}
				if (this._appended == null)
				{
					return false;
				}
				this._enumerator = this._appended.GetEnumerator(this._appendCount);
				this._state = 4;
				IL_A2:
				return base.LoadFromEnumerator();
			}

			public override Enumerable.AppendPrependIterator<TSource> Append(TSource item)
			{
				SingleLinkedNode<TSource> appended = (this._appended != null) ? this._appended.Add(item) : new SingleLinkedNode<TSource>(item);
				return new Enumerable.AppendPrependN<TSource>(this._source, this._prepended, appended, this._prependCount, this._appendCount + 1);
			}

			public override Enumerable.AppendPrependIterator<TSource> Prepend(TSource item)
			{
				SingleLinkedNode<TSource> prepended = (this._prepended != null) ? this._prepended.Add(item) : new SingleLinkedNode<TSource>(item);
				return new Enumerable.AppendPrependN<TSource>(this._source, prepended, this._appended, this._prependCount + 1, this._appendCount);
			}

			private TSource[] LazyToArray()
			{
				SparseArrayBuilder<TSource> sparseArrayBuilder = new SparseArrayBuilder<TSource>(true);
				if (this._prepended != null)
				{
					sparseArrayBuilder.Reserve(this._prependCount);
				}
				sparseArrayBuilder.AddRange(this._source);
				if (this._appended != null)
				{
					sparseArrayBuilder.Reserve(this._appendCount);
				}
				TSource[] array = sparseArrayBuilder.ToArray();
				int num = 0;
				for (SingleLinkedNode<TSource> singleLinkedNode = this._prepended; singleLinkedNode != null; singleLinkedNode = singleLinkedNode.Linked)
				{
					array[num++] = singleLinkedNode.Item;
				}
				num = array.Length - 1;
				for (SingleLinkedNode<TSource> singleLinkedNode2 = this._appended; singleLinkedNode2 != null; singleLinkedNode2 = singleLinkedNode2.Linked)
				{
					array[num--] = singleLinkedNode2.Item;
				}
				return array;
			}

			public override TSource[] ToArray()
			{
				int count = this.GetCount(true);
				if (count == -1)
				{
					return this.LazyToArray();
				}
				TSource[] array = new TSource[count];
				int num = 0;
				for (SingleLinkedNode<TSource> singleLinkedNode = this._prepended; singleLinkedNode != null; singleLinkedNode = singleLinkedNode.Linked)
				{
					array[num] = singleLinkedNode.Item;
					num++;
				}
				ICollection<TSource> collection = this._source as ICollection<TSource>;
				if (collection != null)
				{
					collection.CopyTo(array, num);
				}
				else
				{
					foreach (TSource tsource in this._source)
					{
						array[num] = tsource;
						num++;
					}
				}
				num = array.Length;
				for (SingleLinkedNode<TSource> singleLinkedNode2 = this._appended; singleLinkedNode2 != null; singleLinkedNode2 = singleLinkedNode2.Linked)
				{
					num--;
					array[num] = singleLinkedNode2.Item;
				}
				return array;
			}

			public override List<TSource> ToList()
			{
				int count = this.GetCount(true);
				List<TSource> list = (count == -1) ? new List<TSource>() : new List<TSource>(count);
				for (SingleLinkedNode<TSource> singleLinkedNode = this._prepended; singleLinkedNode != null; singleLinkedNode = singleLinkedNode.Linked)
				{
					list.Add(singleLinkedNode.Item);
				}
				list.AddRange(this._source);
				if (this._appended != null)
				{
					IEnumerator<TSource> enumerator = this._appended.GetEnumerator(this._appendCount);
					while (enumerator.MoveNext())
					{
						TSource item = enumerator.Current;
						list.Add(item);
					}
				}
				return list;
			}

			public override int GetCount(bool onlyIfCheap)
			{
				IIListProvider<TSource> iilistProvider = this._source as IIListProvider<TSource>;
				if (iilistProvider != null)
				{
					int count = iilistProvider.GetCount(onlyIfCheap);
					if (count != -1)
					{
						return count + this._appendCount + this._prependCount;
					}
					return -1;
				}
				else
				{
					if (onlyIfCheap && !(this._source is ICollection<TSource>))
					{
						return -1;
					}
					return this._source.Count<TSource>() + this._appendCount + this._prependCount;
				}
			}

			private readonly SingleLinkedNode<TSource> _prepended;

			private readonly SingleLinkedNode<TSource> _appended;

			private readonly int _prependCount;

			private readonly int _appendCount;

			private SingleLinkedNode<TSource> _node;
		}

		private sealed class Concat2Iterator<TSource> : Enumerable.ConcatIterator<TSource>
		{
			internal Concat2Iterator(IEnumerable<TSource> first, IEnumerable<TSource> second)
			{
				this._first = first;
				this._second = second;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.Concat2Iterator<TSource>(this._first, this._second);
			}

			internal override Enumerable.ConcatIterator<TSource> Concat(IEnumerable<TSource> next)
			{
				bool hasOnlyCollections = next is ICollection<TSource> && this._first is ICollection<TSource> && this._second is ICollection<TSource>;
				return new Enumerable.ConcatNIterator<TSource>(this, next, 2, hasOnlyCollections);
			}

			public override int GetCount(bool onlyIfCheap)
			{
				int num;
				if (!EnumerableHelpers.TryGetCount<TSource>(this._first, out num))
				{
					if (onlyIfCheap)
					{
						return -1;
					}
					num = this._first.Count<TSource>();
				}
				int num2;
				if (!EnumerableHelpers.TryGetCount<TSource>(this._second, out num2))
				{
					if (onlyIfCheap)
					{
						return -1;
					}
					num2 = this._second.Count<TSource>();
				}
				return checked(num + num2);
			}

			internal override IEnumerable<TSource> GetEnumerable(int index)
			{
				if (index == 0)
				{
					return this._first;
				}
				if (index != 1)
				{
					return null;
				}
				return this._second;
			}

			public override TSource[] ToArray()
			{
				SparseArrayBuilder<TSource> sparseArrayBuilder = new SparseArrayBuilder<TSource>(true);
				bool flag = sparseArrayBuilder.ReserveOrAdd(this._first);
				bool flag2 = sparseArrayBuilder.ReserveOrAdd(this._second);
				TSource[] array = sparseArrayBuilder.ToArray();
				if (flag)
				{
					Marker marker = sparseArrayBuilder.Markers.First();
					EnumerableHelpers.Copy<TSource>(this._first, array, 0, marker.Count);
				}
				if (flag2)
				{
					Marker marker2 = sparseArrayBuilder.Markers.Last();
					EnumerableHelpers.Copy<TSource>(this._second, array, marker2.Index, marker2.Count);
				}
				return array;
			}

			internal readonly IEnumerable<TSource> _first;

			internal readonly IEnumerable<TSource> _second;
		}

		private sealed class ConcatNIterator<TSource> : Enumerable.ConcatIterator<TSource>
		{
			internal ConcatNIterator(Enumerable.ConcatIterator<TSource> tail, IEnumerable<TSource> head, int headIndex, bool hasOnlyCollections)
			{
				this._tail = tail;
				this._head = head;
				this._headIndex = headIndex;
				this._hasOnlyCollections = hasOnlyCollections;
			}

			private Enumerable.ConcatNIterator<TSource> PreviousN
			{
				get
				{
					return this._tail as Enumerable.ConcatNIterator<TSource>;
				}
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.ConcatNIterator<TSource>(this._tail, this._head, this._headIndex, this._hasOnlyCollections);
			}

			internal override Enumerable.ConcatIterator<TSource> Concat(IEnumerable<TSource> next)
			{
				if (this._headIndex == 2147483645)
				{
					return new Enumerable.Concat2Iterator<TSource>(this, next);
				}
				bool hasOnlyCollections = this._hasOnlyCollections && next is ICollection<TSource>;
				return new Enumerable.ConcatNIterator<TSource>(this, next, this._headIndex + 1, hasOnlyCollections);
			}

			public override int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap && !this._hasOnlyCollections)
				{
					return -1;
				}
				int num = 0;
				Enumerable.ConcatNIterator<TSource> concatNIterator = this;
				checked
				{
					Enumerable.ConcatNIterator<TSource> concatNIterator2;
					do
					{
						concatNIterator2 = concatNIterator;
						IEnumerable<TSource> head = concatNIterator2._head;
						ICollection<TSource> collection = head as ICollection<TSource>;
						int num2 = (collection != null) ? collection.Count : head.Count<TSource>();
						num += num2;
					}
					while ((concatNIterator = concatNIterator2.PreviousN) != null);
					return num + concatNIterator2._tail.GetCount(onlyIfCheap);
				}
			}

			internal override IEnumerable<TSource> GetEnumerable(int index)
			{
				if (index > this._headIndex)
				{
					return null;
				}
				Enumerable.ConcatNIterator<TSource> concatNIterator = this;
				Enumerable.ConcatNIterator<TSource> concatNIterator2;
				for (;;)
				{
					concatNIterator2 = concatNIterator;
					if (index == concatNIterator2._headIndex)
					{
						break;
					}
					if ((concatNIterator = concatNIterator2.PreviousN) == null)
					{
						goto Block_3;
					}
				}
				return concatNIterator2._head;
				Block_3:
				return concatNIterator2._tail.GetEnumerable(index);
			}

			public override TSource[] ToArray()
			{
				if (!this._hasOnlyCollections)
				{
					return this.LazyToArray();
				}
				return this.PreallocatingToArray();
			}

			private TSource[] LazyToArray()
			{
				SparseArrayBuilder<TSource> sparseArrayBuilder = new SparseArrayBuilder<TSource>(true);
				ArrayBuilder<int> arrayBuilder = default(ArrayBuilder<int>);
				int num = 0;
				for (;;)
				{
					IEnumerable<TSource> enumerable = this.GetEnumerable(num);
					if (enumerable == null)
					{
						break;
					}
					if (sparseArrayBuilder.ReserveOrAdd(enumerable))
					{
						arrayBuilder.Add(num);
					}
					num++;
				}
				TSource[] array = sparseArrayBuilder.ToArray();
				ArrayBuilder<Marker> markers = sparseArrayBuilder.Markers;
				for (int i = 0; i < markers.Count; i++)
				{
					Marker marker = markers[i];
					EnumerableHelpers.Copy<TSource>(this.GetEnumerable(arrayBuilder[i]), array, marker.Index, marker.Count);
				}
				return array;
			}

			private TSource[] PreallocatingToArray()
			{
				int count = this.GetCount(true);
				if (count == 0)
				{
					return Array.Empty<TSource>();
				}
				TSource[] array = new TSource[count];
				int num = array.Length;
				Enumerable.ConcatNIterator<TSource> concatNIterator = this;
				checked
				{
					Enumerable.ConcatNIterator<TSource> concatNIterator2;
					do
					{
						concatNIterator2 = concatNIterator;
						ICollection<TSource> collection = (ICollection<TSource>)concatNIterator2._head;
						int count2 = collection.Count;
						if (count2 > 0)
						{
							num -= count2;
							collection.CopyTo(array, num);
						}
					}
					while ((concatNIterator = concatNIterator2.PreviousN) != null);
					Enumerable.Concat2Iterator<TSource> concat2Iterator = (Enumerable.Concat2Iterator<TSource>)concatNIterator2._tail;
					ICollection<TSource> collection2 = (ICollection<TSource>)concat2Iterator._second;
					int count3 = collection2.Count;
					if (count3 > 0)
					{
						collection2.CopyTo(array, num - count3);
					}
					if (num > count3)
					{
						((ICollection<TSource>)concat2Iterator._first).CopyTo(array, 0);
					}
					return array;
				}
			}

			private readonly Enumerable.ConcatIterator<TSource> _tail;

			private readonly IEnumerable<TSource> _head;

			private readonly int _headIndex;

			private readonly bool _hasOnlyCollections;
		}

		private abstract class ConcatIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			internal abstract IEnumerable<TSource> GetEnumerable(int index);

			internal abstract Enumerable.ConcatIterator<TSource> Concat(IEnumerable<TSource> next);

			public override bool MoveNext()
			{
				if (this._state == 1)
				{
					this._enumerator = this.GetEnumerable(0).GetEnumerator();
					this._state = 2;
				}
				if (this._state > 1)
				{
					while (!this._enumerator.MoveNext())
					{
						int state = this._state;
						this._state = state + 1;
						IEnumerable<TSource> enumerable = this.GetEnumerable(state - 1);
						if (enumerable == null)
						{
							this.Dispose();
							return false;
						}
						this._enumerator.Dispose();
						this._enumerator = enumerable.GetEnumerator();
					}
					this._current = this._enumerator.Current;
					return true;
				}
				return false;
			}

			public abstract int GetCount(bool onlyIfCheap);

			public abstract TSource[] ToArray();

			public List<TSource> ToList()
			{
				int count = this.GetCount(true);
				List<TSource> list = (count != -1) ? new List<TSource>(count) : new List<TSource>();
				int num = 0;
				for (;;)
				{
					IEnumerable<TSource> enumerable = this.GetEnumerable(num);
					if (enumerable == null)
					{
						break;
					}
					list.AddRange(enumerable);
					num++;
				}
				return list;
			}

			private IEnumerator<TSource> _enumerator;
		}

		private sealed class DefaultIfEmptyIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			public DefaultIfEmptyIterator(IEnumerable<TSource> source, TSource defaultValue)
			{
				this._source = source;
				this._default = defaultValue;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.DefaultIfEmptyIterator<TSource>(this._source, this._default);
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state == 2)
					{
						if (this._enumerator.MoveNext())
						{
							this._current = this._enumerator.Current;
							return true;
						}
					}
					this.Dispose();
					return false;
				}
				this._enumerator = this._source.GetEnumerator();
				if (this._enumerator.MoveNext())
				{
					this._current = this._enumerator.Current;
					this._state = 2;
				}
				else
				{
					this._current = this._default;
					this._state = -1;
				}
				return true;
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			public TSource[] ToArray()
			{
				TSource[] array = this._source.ToArray<TSource>();
				if (array.Length != 0)
				{
					return array;
				}
				return new TSource[]
				{
					this._default
				};
			}

			public List<TSource> ToList()
			{
				List<TSource> list = this._source.ToList<TSource>();
				if (list.Count == 0)
				{
					list.Add(this._default);
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				int num;
				if (!onlyIfCheap || this._source is ICollection<TSource> || this._source is ICollection)
				{
					num = this._source.Count<TSource>();
				}
				else
				{
					IIListProvider<TSource> iilistProvider = this._source as IIListProvider<TSource>;
					num = ((iilistProvider != null) ? iilistProvider.GetCount(true) : -1);
				}
				if (num != 0)
				{
					return num;
				}
				return 1;
			}

			private readonly IEnumerable<TSource> _source;

			private readonly TSource _default;

			private IEnumerator<TSource> _enumerator;
		}

		private sealed class DistinctIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			public DistinctIterator(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
			{
				this._source = source;
				this._comparer = comparer;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.DistinctIterator<TSource>(this._source, this._comparer);
			}

			public override bool MoveNext()
			{
				int state = this._state;
				TSource tsource;
				if (state != 1)
				{
					if (state == 2)
					{
						while (this._enumerator.MoveNext())
						{
							tsource = this._enumerator.Current;
							if (this._set.Add(tsource))
							{
								this._current = tsource;
								return true;
							}
						}
					}
					this.Dispose();
					return false;
				}
				this._enumerator = this._source.GetEnumerator();
				if (!this._enumerator.MoveNext())
				{
					this.Dispose();
					return false;
				}
				tsource = this._enumerator.Current;
				this._set = new Set<TSource>(this._comparer);
				this._set.Add(tsource);
				this._current = tsource;
				this._state = 2;
				return true;
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
					this._set = null;
				}
				base.Dispose();
			}

			private Set<TSource> FillSet()
			{
				Set<TSource> set = new Set<TSource>(this._comparer);
				set.UnionWith(this._source);
				return set;
			}

			public TSource[] ToArray()
			{
				return this.FillSet().ToArray();
			}

			public List<TSource> ToList()
			{
				return this.FillSet().ToList();
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (!onlyIfCheap)
				{
					return this.FillSet().Count;
				}
				return -1;
			}

			private readonly IEnumerable<TSource> _source;

			private readonly IEqualityComparer<TSource> _comparer;

			private Set<TSource> _set;

			private IEnumerator<TSource> _enumerator;
		}

		internal abstract class Iterator<TSource> : IEnumerable<!0>, IEnumerable, IEnumerator<!0>, IDisposable, IEnumerator
		{
			protected Iterator()
			{
				this._threadId = Environment.CurrentManagedThreadId;
			}

			public TSource Current
			{
				get
				{
					return this._current;
				}
			}

			public abstract Enumerable.Iterator<TSource> Clone();

			public virtual void Dispose()
			{
				this._current = default(TSource);
				this._state = -1;
			}

			public IEnumerator<TSource> GetEnumerator()
			{
				Enumerable.Iterator iterator = (this._state == 0 && this._threadId == Environment.CurrentManagedThreadId) ? this : this.Clone();
				iterator._state = 1;
				return iterator;
			}

			public abstract bool MoveNext();

			public virtual IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
			{
				return new Enumerable.SelectEnumerableIterator<TSource, TResult>(this, selector);
			}

			public virtual IEnumerable<TSource> Where(Func<TSource, bool> predicate)
			{
				return new Enumerable.WhereEnumerableIterator<TSource>(this, predicate);
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			void IEnumerator.Reset()
			{
				throw Error.NotSupported();
			}

			private readonly int _threadId;

			internal int _state;

			internal TSource _current;
		}

		private sealed class ListPartition<TSource> : Enumerable.Iterator<TSource>, IPartition<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			public ListPartition(IList<TSource> source, int minIndexInclusive, int maxIndexInclusive)
			{
				this._source = source;
				this._minIndexInclusive = minIndexInclusive;
				this._maxIndexInclusive = maxIndexInclusive;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.ListPartition<TSource>(this._source, this._minIndexInclusive, this._maxIndexInclusive);
			}

			public override bool MoveNext()
			{
				int num = this._state - 1;
				if (num <= this._maxIndexInclusive - this._minIndexInclusive && num < this._source.Count - this._minIndexInclusive)
				{
					this._current = this._source[this._minIndexInclusive + num];
					this._state++;
					return true;
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
			{
				return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, selector, this._minIndexInclusive, this._maxIndexInclusive);
			}

			public IPartition<TSource> Skip(int count)
			{
				int num = this._minIndexInclusive + count;
				if (num <= this._maxIndexInclusive)
				{
					return new Enumerable.ListPartition<TSource>(this._source, num, this._maxIndexInclusive);
				}
				return EmptyPartition<TSource>.Instance;
			}

			public IPartition<TSource> Take(int count)
			{
				int num = this._minIndexInclusive + count - 1;
				if (num < this._maxIndexInclusive)
				{
					return new Enumerable.ListPartition<TSource>(this._source, this._minIndexInclusive, num);
				}
				return this;
			}

			public TSource TryGetElementAt(int index, out bool found)
			{
				if (index <= this._maxIndexInclusive - this._minIndexInclusive && index < this._source.Count - this._minIndexInclusive)
				{
					found = true;
					return this._source[this._minIndexInclusive + index];
				}
				found = false;
				return default(TSource);
			}

			public TSource TryGetFirst(out bool found)
			{
				if (this._source.Count > this._minIndexInclusive)
				{
					found = true;
					return this._source[this._minIndexInclusive];
				}
				found = false;
				return default(TSource);
			}

			public TSource TryGetLast(out bool found)
			{
				int num = this._source.Count - 1;
				if (num >= this._minIndexInclusive)
				{
					found = true;
					return this._source[Math.Min(num, this._maxIndexInclusive)];
				}
				found = false;
				return default(TSource);
			}

			private int Count
			{
				get
				{
					int count = this._source.Count;
					if (count <= this._minIndexInclusive)
					{
						return 0;
					}
					return Math.Min(count - 1, this._maxIndexInclusive) - this._minIndexInclusive + 1;
				}
			}

			public TSource[] ToArray()
			{
				int count = this.Count;
				if (count == 0)
				{
					return Array.Empty<TSource>();
				}
				TSource[] array = new TSource[count];
				int num = 0;
				int num2 = this._minIndexInclusive;
				while (num != array.Length)
				{
					array[num] = this._source[num2];
					num++;
					num2++;
				}
				return array;
			}

			public List<TSource> ToList()
			{
				int count = this.Count;
				if (count == 0)
				{
					return new List<TSource>();
				}
				List<TSource> list = new List<TSource>(count);
				int num = this._minIndexInclusive + count;
				for (int num2 = this._minIndexInclusive; num2 != num; num2++)
				{
					list.Add(this._source[num2]);
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				return this.Count;
			}

			private readonly IList<TSource> _source;

			private readonly int _minIndexInclusive;

			private readonly int _maxIndexInclusive;
		}

		private sealed class EnumerablePartition<TSource> : Enumerable.Iterator<TSource>, IPartition<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			internal EnumerablePartition(IEnumerable<TSource> source, int minIndexInclusive, int maxIndexInclusive)
			{
				this._source = source;
				this._minIndexInclusive = minIndexInclusive;
				this._maxIndexInclusive = maxIndexInclusive;
			}

			private bool HasLimit
			{
				get
				{
					return this._maxIndexInclusive != -1;
				}
			}

			private int Limit
			{
				get
				{
					return this._maxIndexInclusive + 1 - this._minIndexInclusive;
				}
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.EnumerablePartition<TSource>(this._source, this._minIndexInclusive, this._maxIndexInclusive);
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				if (!this.HasLimit)
				{
					return Math.Max(this._source.Count<TSource>() - this._minIndexInclusive, 0);
				}
				int result;
				using (IEnumerator<TSource> enumerator = this._source.GetEnumerator())
				{
					result = Math.Max((int)(Enumerable.EnumerablePartition<TSource>.SkipAndCount((uint)(this._maxIndexInclusive + 1), enumerator) - (uint)this._minIndexInclusive), 0);
				}
				return result;
			}

			public override bool MoveNext()
			{
				int num = this._state - 3;
				if (num < -2)
				{
					this.Dispose();
					return false;
				}
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						goto IL_54;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				if (!this.SkipBeforeFirst(this._enumerator))
				{
					goto IL_9B;
				}
				this._state = 3;
				IL_54:
				if ((!this.HasLimit || num < this.Limit) && this._enumerator.MoveNext())
				{
					if (this.HasLimit)
					{
						this._state++;
					}
					this._current = this._enumerator.Current;
					return true;
				}
				IL_9B:
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
			{
				return new Enumerable.SelectIPartitionIterator<TSource, TResult>(this, selector);
			}

			public IPartition<TSource> Skip(int count)
			{
				int num = this._minIndexInclusive + count;
				if (!this.HasLimit)
				{
					if (num < 0)
					{
						return new Enumerable.EnumerablePartition<TSource>(this, count, -1);
					}
				}
				else if (num > this._maxIndexInclusive)
				{
					return EmptyPartition<TSource>.Instance;
				}
				return new Enumerable.EnumerablePartition<TSource>(this._source, num, this._maxIndexInclusive);
			}

			public IPartition<TSource> Take(int count)
			{
				int num = this._minIndexInclusive + count - 1;
				if (!this.HasLimit)
				{
					if (num < 0)
					{
						return new Enumerable.EnumerablePartition<TSource>(this, 0, count - 1);
					}
				}
				else if (num >= this._maxIndexInclusive)
				{
					return this;
				}
				return new Enumerable.EnumerablePartition<TSource>(this._source, this._minIndexInclusive, num);
			}

			public TSource TryGetElementAt(int index, out bool found)
			{
				if (index >= 0 && (!this.HasLimit || index < this.Limit))
				{
					using (IEnumerator<TSource> enumerator = this._source.GetEnumerator())
					{
						if (Enumerable.EnumerablePartition<TSource>.SkipBefore(this._minIndexInclusive + index, enumerator) && enumerator.MoveNext())
						{
							found = true;
							return enumerator.Current;
						}
					}
				}
				found = false;
				return default(TSource);
			}

			public TSource TryGetFirst(out bool found)
			{
				using (IEnumerator<TSource> enumerator = this._source.GetEnumerator())
				{
					if (this.SkipBeforeFirst(enumerator) && enumerator.MoveNext())
					{
						found = true;
						return enumerator.Current;
					}
				}
				found = false;
				return default(TSource);
			}

			public TSource TryGetLast(out bool found)
			{
				using (IEnumerator<TSource> enumerator = this._source.GetEnumerator())
				{
					if (this.SkipBeforeFirst(enumerator) && enumerator.MoveNext())
					{
						int num = this.Limit - 1;
						int num2 = this.HasLimit ? 0 : int.MinValue;
						TSource result;
						do
						{
							num--;
							result = enumerator.Current;
						}
						while (num >= num2 && enumerator.MoveNext());
						found = true;
						return result;
					}
				}
				found = false;
				return default(TSource);
			}

			public TSource[] ToArray()
			{
				using (IEnumerator<TSource> enumerator = this._source.GetEnumerator())
				{
					if (this.SkipBeforeFirst(enumerator) && enumerator.MoveNext())
					{
						int num = this.Limit - 1;
						int num2 = this.HasLimit ? 0 : int.MinValue;
						int maxCapacity = this.HasLimit ? this.Limit : int.MaxValue;
						LargeArrayBuilder<TSource> largeArrayBuilder = new LargeArrayBuilder<TSource>(maxCapacity);
						do
						{
							num--;
							largeArrayBuilder.Add(enumerator.Current);
						}
						while (num >= num2 && enumerator.MoveNext());
						return largeArrayBuilder.ToArray();
					}
				}
				return Array.Empty<TSource>();
			}

			public List<TSource> ToList()
			{
				List<TSource> list = new List<TSource>();
				using (IEnumerator<TSource> enumerator = this._source.GetEnumerator())
				{
					if (this.SkipBeforeFirst(enumerator) && enumerator.MoveNext())
					{
						int num = this.Limit - 1;
						int num2 = this.HasLimit ? 0 : int.MinValue;
						do
						{
							num--;
							list.Add(enumerator.Current);
						}
						while (num >= num2 && enumerator.MoveNext());
					}
				}
				return list;
			}

			private bool SkipBeforeFirst(IEnumerator<TSource> en)
			{
				return Enumerable.EnumerablePartition<TSource>.SkipBefore(this._minIndexInclusive, en);
			}

			private static bool SkipBefore(int index, IEnumerator<TSource> en)
			{
				return Enumerable.EnumerablePartition<TSource>.SkipAndCount(index, en) == index;
			}

			private static int SkipAndCount(int index, IEnumerator<TSource> en)
			{
				return (int)Enumerable.EnumerablePartition<TSource>.SkipAndCount((uint)index, en);
			}

			private static uint SkipAndCount(uint index, IEnumerator<TSource> en)
			{
				for (uint num = 0U; num < index; num += 1U)
				{
					if (!en.MoveNext())
					{
						return num;
					}
				}
				return index;
			}

			private readonly IEnumerable<TSource> _source;

			private readonly int _minIndexInclusive;

			private readonly int _maxIndexInclusive;

			private IEnumerator<TSource> _enumerator;
		}

		private sealed class RangeIterator : Enumerable.Iterator<int>, IPartition<int>, IIListProvider<int>, IEnumerable<int>, IEnumerable
		{
			public RangeIterator(int start, int count)
			{
				this._start = start;
				this._end = start + count;
			}

			public override Enumerable.Iterator<int> Clone()
			{
				return new Enumerable.RangeIterator(this._start, this._end - this._start);
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state == 2)
					{
						int num = this._current + 1;
						this._current = num;
						if (num != this._end)
						{
							return true;
						}
					}
					this._state = -1;
					return false;
				}
				this._current = this._start;
				this._state = 2;
				return true;
			}

			public override void Dispose()
			{
				this._state = -1;
			}

			public override IEnumerable<TResult> Select<TResult>(Func<int, TResult> selector)
			{
				return new Enumerable.SelectIPartitionIterator<int, TResult>(this, selector);
			}

			public int[] ToArray()
			{
				int[] array = new int[this._end - this._start];
				int num = this._start;
				for (int num2 = 0; num2 != array.Length; num2++)
				{
					array[num2] = num;
					num++;
				}
				return array;
			}

			public List<int> ToList()
			{
				List<int> list = new List<int>(this._end - this._start);
				for (int num = this._start; num != this._end; num++)
				{
					list.Add(num);
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				return this._end - this._start;
			}

			public IPartition<int> Skip(int count)
			{
				if (count >= this._end - this._start)
				{
					return EmptyPartition<int>.Instance;
				}
				return new Enumerable.RangeIterator(this._start + count, this._end - this._start - count);
			}

			public IPartition<int> Take(int count)
			{
				int num = this._end - this._start;
				if (count >= num)
				{
					return this;
				}
				return new Enumerable.RangeIterator(this._start, count);
			}

			public int TryGetElementAt(int index, out bool found)
			{
				if (index < this._end - this._start)
				{
					found = true;
					return this._start + index;
				}
				found = false;
				return 0;
			}

			public int TryGetFirst(out bool found)
			{
				found = true;
				return this._start;
			}

			public int TryGetLast(out bool found)
			{
				found = true;
				return this._end - 1;
			}

			private readonly int _start;

			private readonly int _end;
		}

		private sealed class RepeatIterator<TResult> : Enumerable.Iterator<TResult>, IPartition<TResult>, IIListProvider<TResult>, IEnumerable<!0>, IEnumerable
		{
			public RepeatIterator(TResult element, int count)
			{
				this._current = element;
				this._count = count;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.RepeatIterator<TResult>(this._current, this._count);
			}

			public override void Dispose()
			{
				this._state = -1;
			}

			public override bool MoveNext()
			{
				int num = this._state - 1;
				if (num >= 0 && num != this._count)
				{
					this._state++;
					return true;
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.SelectIPartitionIterator<TResult, TResult2>(this, selector);
			}

			public TResult[] ToArray()
			{
				TResult[] array = new TResult[this._count];
				if (this._current != null)
				{
					Array.Fill<TResult>(array, this._current);
				}
				return array;
			}

			public List<TResult> ToList()
			{
				List<TResult> list = new List<TResult>(this._count);
				for (int num = 0; num != this._count; num++)
				{
					list.Add(this._current);
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				return this._count;
			}

			public IPartition<TResult> Skip(int count)
			{
				if (count >= this._count)
				{
					return EmptyPartition<TResult>.Instance;
				}
				return new Enumerable.RepeatIterator<TResult>(this._current, this._count - count);
			}

			public IPartition<TResult> Take(int count)
			{
				if (count >= this._count)
				{
					return this;
				}
				return new Enumerable.RepeatIterator<TResult>(this._current, count);
			}

			public TResult TryGetElementAt(int index, out bool found)
			{
				if (index < this._count)
				{
					found = true;
					return this._current;
				}
				found = false;
				return default(TResult);
			}

			public TResult TryGetFirst(out bool found)
			{
				found = true;
				return this._current;
			}

			public TResult TryGetLast(out bool found)
			{
				found = true;
				return this._current;
			}

			private readonly int _count;
		}

		private sealed class ReverseIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			public ReverseIterator(IEnumerable<TSource> source)
			{
				this._source = source;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.ReverseIterator<TSource>(this._source);
			}

			public override bool MoveNext()
			{
				if (this._state - 2 <= -2)
				{
					this.Dispose();
					return false;
				}
				if (this._state == 1)
				{
					Buffer<TSource> buffer = new Buffer<TSource>(this._source);
					this._buffer = buffer._items;
					this._state = buffer._count + 2;
				}
				int num = this._state - 3;
				if (num != -1)
				{
					this._current = this._buffer[num];
					this._state--;
					return true;
				}
				this.Dispose();
				return false;
			}

			public override void Dispose()
			{
				this._buffer = null;
				base.Dispose();
			}

			public TSource[] ToArray()
			{
				TSource[] array = this._source.ToArray<TSource>();
				Array.Reverse<TSource>(array);
				return array;
			}

			public List<TSource> ToList()
			{
				List<TSource> list = this._source.ToList<TSource>();
				list.Reverse();
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (!onlyIfCheap)
				{
					return this._source.Count<TSource>();
				}
				IEnumerable<TSource> source = this._source;
				IIListProvider<TSource> iilistProvider = source as IIListProvider<TSource>;
				if (iilistProvider != null)
				{
					return iilistProvider.GetCount(true);
				}
				ICollection<TSource> collection = source as ICollection<TSource>;
				if (collection != null)
				{
					return collection.Count;
				}
				ICollection collection2 = source as ICollection;
				if (collection2 == null)
				{
					return -1;
				}
				return collection2.Count;
			}

			private readonly IEnumerable<TSource> _source;

			private TSource[] _buffer;
		}

		private sealed class SelectEnumerableIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public SelectEnumerableIterator(IEnumerable<TSource> source, Func<TSource, TResult> selector)
			{
				this._source = source;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.SelectEnumerableIterator<TSource, TResult>(this._source, this._selector);
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						return false;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				if (this._enumerator.MoveNext())
				{
					this._current = this._selector(this._enumerator.Current);
					return true;
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.SelectEnumerableIterator<TSource, TResult2>(this._source, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector));
			}

			public TResult[] ToArray()
			{
				LargeArrayBuilder<TResult> largeArrayBuilder = new LargeArrayBuilder<TResult>(true);
				foreach (TSource arg in this._source)
				{
					largeArrayBuilder.Add(this._selector(arg));
				}
				return largeArrayBuilder.ToArray();
			}

			public List<TResult> ToList()
			{
				List<TResult> list = new List<TResult>();
				foreach (TSource arg in this._source)
				{
					list.Add(this._selector(arg));
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				int num = 0;
				checked
				{
					foreach (TSource arg in this._source)
					{
						this._selector(arg);
						num++;
					}
					return num;
				}
			}

			private readonly IEnumerable<TSource> _source;

			private readonly Func<TSource, TResult> _selector;

			private IEnumerator<TSource> _enumerator;
		}

		private sealed class SelectArrayIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IPartition<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public SelectArrayIterator(TSource[] source, Func<TSource, TResult> selector)
			{
				this._source = source;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.SelectArrayIterator<TSource, TResult>(this._source, this._selector);
			}

			public override bool MoveNext()
			{
				if (this._state < 1 | this._state == this._source.Length + 1)
				{
					this.Dispose();
					return false;
				}
				int state = this._state;
				this._state = state + 1;
				int num = state - 1;
				this._current = this._selector(this._source[num]);
				return true;
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.SelectArrayIterator<TSource, TResult2>(this._source, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector));
			}

			public TResult[] ToArray()
			{
				TResult[] array = new TResult[this._source.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._selector(this._source[i]);
				}
				return array;
			}

			public List<TResult> ToList()
			{
				TSource[] source = this._source;
				List<TResult> list = new List<TResult>(source.Length);
				for (int i = 0; i < source.Length; i++)
				{
					list.Add(this._selector(source[i]));
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (!onlyIfCheap)
				{
					foreach (TSource arg in this._source)
					{
						this._selector(arg);
					}
				}
				return this._source.Length;
			}

			public IPartition<TResult> Skip(int count)
			{
				if (count >= this._source.Length)
				{
					return EmptyPartition<TResult>.Instance;
				}
				return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, count, int.MaxValue);
			}

			public IPartition<TResult> Take(int count)
			{
				if (count < this._source.Length)
				{
					return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, 0, count - 1);
				}
				return this;
			}

			public TResult TryGetElementAt(int index, out bool found)
			{
				if (index < this._source.Length)
				{
					found = true;
					return this._selector(this._source[index]);
				}
				found = false;
				return default(TResult);
			}

			public TResult TryGetFirst(out bool found)
			{
				found = true;
				return this._selector(this._source[0]);
			}

			public TResult TryGetLast(out bool found)
			{
				found = true;
				return this._selector(this._source[this._source.Length - 1]);
			}

			private readonly TSource[] _source;

			private readonly Func<TSource, TResult> _selector;
		}

		private sealed class SelectListIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IPartition<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public SelectListIterator(List<TSource> source, Func<TSource, TResult> selector)
			{
				this._source = source;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.SelectListIterator<TSource, TResult>(this._source, this._selector);
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						return false;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				if (this._enumerator.MoveNext())
				{
					this._current = this._selector(this._enumerator.Current);
					return true;
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.SelectListIterator<TSource, TResult2>(this._source, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector));
			}

			public TResult[] ToArray()
			{
				int count = this._source.Count;
				if (count == 0)
				{
					return Array.Empty<TResult>();
				}
				TResult[] array = new TResult[count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._selector(this._source[i]);
				}
				return array;
			}

			public List<TResult> ToList()
			{
				int count = this._source.Count;
				List<TResult> list = new List<TResult>(count);
				for (int i = 0; i < count; i++)
				{
					list.Add(this._selector(this._source[i]));
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				int count = this._source.Count;
				if (!onlyIfCheap)
				{
					for (int i = 0; i < count; i++)
					{
						this._selector(this._source[i]);
					}
				}
				return count;
			}

			public IPartition<TResult> Skip(int count)
			{
				return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, count, int.MaxValue);
			}

			public IPartition<TResult> Take(int count)
			{
				return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, 0, count - 1);
			}

			public TResult TryGetElementAt(int index, out bool found)
			{
				if (index < this._source.Count)
				{
					found = true;
					return this._selector(this._source[index]);
				}
				found = false;
				return default(TResult);
			}

			public TResult TryGetFirst(out bool found)
			{
				if (this._source.Count != 0)
				{
					found = true;
					return this._selector(this._source[0]);
				}
				found = false;
				return default(TResult);
			}

			public TResult TryGetLast(out bool found)
			{
				int count = this._source.Count;
				if (count != 0)
				{
					found = true;
					return this._selector(this._source[count - 1]);
				}
				found = false;
				return default(TResult);
			}

			private readonly List<TSource> _source;

			private readonly Func<TSource, TResult> _selector;

			private List<TSource>.Enumerator _enumerator;
		}

		private sealed class SelectIListIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IPartition<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public SelectIListIterator(IList<TSource> source, Func<TSource, TResult> selector)
			{
				this._source = source;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.SelectIListIterator<TSource, TResult>(this._source, this._selector);
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						return false;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				if (this._enumerator.MoveNext())
				{
					this._current = this._selector(this._enumerator.Current);
					return true;
				}
				this.Dispose();
				return false;
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.SelectIListIterator<TSource, TResult2>(this._source, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector));
			}

			public TResult[] ToArray()
			{
				int count = this._source.Count;
				if (count == 0)
				{
					return Array.Empty<TResult>();
				}
				TResult[] array = new TResult[count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._selector(this._source[i]);
				}
				return array;
			}

			public List<TResult> ToList()
			{
				int count = this._source.Count;
				List<TResult> list = new List<TResult>(count);
				for (int i = 0; i < count; i++)
				{
					list.Add(this._selector(this._source[i]));
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				int count = this._source.Count;
				if (!onlyIfCheap)
				{
					for (int i = 0; i < count; i++)
					{
						this._selector(this._source[i]);
					}
				}
				return count;
			}

			public IPartition<TResult> Skip(int count)
			{
				return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, count, int.MaxValue);
			}

			public IPartition<TResult> Take(int count)
			{
				return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, 0, count - 1);
			}

			public TResult TryGetElementAt(int index, out bool found)
			{
				if (index < this._source.Count)
				{
					found = true;
					return this._selector(this._source[index]);
				}
				found = false;
				return default(TResult);
			}

			public TResult TryGetFirst(out bool found)
			{
				if (this._source.Count != 0)
				{
					found = true;
					return this._selector(this._source[0]);
				}
				found = false;
				return default(TResult);
			}

			public TResult TryGetLast(out bool found)
			{
				int count = this._source.Count;
				if (count != 0)
				{
					found = true;
					return this._selector(this._source[count - 1]);
				}
				found = false;
				return default(TResult);
			}

			private readonly IList<TSource> _source;

			private readonly Func<TSource, TResult> _selector;

			private IEnumerator<TSource> _enumerator;
		}

		private sealed class SelectIPartitionIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IPartition<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public SelectIPartitionIterator(IPartition<TSource> source, Func<TSource, TResult> selector)
			{
				this._source = source;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.SelectIPartitionIterator<TSource, TResult>(this._source, this._selector);
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						return false;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				if (this._enumerator.MoveNext())
				{
					this._current = this._selector(this._enumerator.Current);
					return true;
				}
				this.Dispose();
				return false;
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.SelectIPartitionIterator<TSource, TResult2>(this._source, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector));
			}

			public IPartition<TResult> Skip(int count)
			{
				return new Enumerable.SelectIPartitionIterator<TSource, TResult>(this._source.Skip(count), this._selector);
			}

			public IPartition<TResult> Take(int count)
			{
				return new Enumerable.SelectIPartitionIterator<TSource, TResult>(this._source.Take(count), this._selector);
			}

			public TResult TryGetElementAt(int index, out bool found)
			{
				bool flag;
				TSource arg = this._source.TryGetElementAt(index, out flag);
				found = flag;
				if (!flag)
				{
					return default(TResult);
				}
				return this._selector(arg);
			}

			public TResult TryGetFirst(out bool found)
			{
				bool flag;
				TSource arg = this._source.TryGetFirst(out flag);
				found = flag;
				if (!flag)
				{
					return default(TResult);
				}
				return this._selector(arg);
			}

			public TResult TryGetLast(out bool found)
			{
				bool flag;
				TSource arg = this._source.TryGetLast(out flag);
				found = flag;
				if (!flag)
				{
					return default(TResult);
				}
				return this._selector(arg);
			}

			private TResult[] LazyToArray()
			{
				LargeArrayBuilder<TResult> largeArrayBuilder = new LargeArrayBuilder<TResult>(true);
				foreach (TSource arg in this._source)
				{
					largeArrayBuilder.Add(this._selector(arg));
				}
				return largeArrayBuilder.ToArray();
			}

			private TResult[] PreallocatingToArray(int count)
			{
				TResult[] array = new TResult[count];
				int num = 0;
				foreach (TSource arg in this._source)
				{
					array[num] = this._selector(arg);
					num++;
				}
				return array;
			}

			public TResult[] ToArray()
			{
				int count = this._source.GetCount(true);
				if (count == -1)
				{
					return this.LazyToArray();
				}
				if (count != 0)
				{
					return this.PreallocatingToArray(count);
				}
				return Array.Empty<TResult>();
			}

			public List<TResult> ToList()
			{
				int count = this._source.GetCount(true);
				List<TResult> list;
				if (count != -1)
				{
					if (count == 0)
					{
						return new List<TResult>();
					}
					list = new List<TResult>(count);
				}
				else
				{
					list = new List<TResult>();
				}
				foreach (TSource arg in this._source)
				{
					list.Add(this._selector(arg));
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (!onlyIfCheap)
				{
					foreach (TSource arg in this._source)
					{
						this._selector(arg);
					}
				}
				return this._source.GetCount(onlyIfCheap);
			}

			private readonly IPartition<TSource> _source;

			private readonly Func<TSource, TResult> _selector;

			private IEnumerator<TSource> _enumerator;
		}

		private sealed class SelectListPartitionIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IPartition<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public SelectListPartitionIterator(IList<TSource> source, Func<TSource, TResult> selector, int minIndexInclusive, int maxIndexInclusive)
			{
				this._source = source;
				this._selector = selector;
				this._minIndexInclusive = minIndexInclusive;
				this._maxIndexInclusive = maxIndexInclusive;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, this._minIndexInclusive, this._maxIndexInclusive);
			}

			public override bool MoveNext()
			{
				int num = this._state - 1;
				if (num <= this._maxIndexInclusive - this._minIndexInclusive && num < this._source.Count - this._minIndexInclusive)
				{
					this._current = this._selector(this._source[this._minIndexInclusive + num]);
					this._state++;
					return true;
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.SelectListPartitionIterator<TSource, TResult2>(this._source, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector), this._minIndexInclusive, this._maxIndexInclusive);
			}

			public IPartition<TResult> Skip(int count)
			{
				int num = this._minIndexInclusive + count;
				if (num <= this._maxIndexInclusive)
				{
					return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, num, this._maxIndexInclusive);
				}
				return EmptyPartition<TResult>.Instance;
			}

			public IPartition<TResult> Take(int count)
			{
				int num = this._minIndexInclusive + count - 1;
				if (num < this._maxIndexInclusive)
				{
					return new Enumerable.SelectListPartitionIterator<TSource, TResult>(this._source, this._selector, this._minIndexInclusive, num);
				}
				return this;
			}

			public TResult TryGetElementAt(int index, out bool found)
			{
				if (index <= this._maxIndexInclusive - this._minIndexInclusive && index < this._source.Count - this._minIndexInclusive)
				{
					found = true;
					return this._selector(this._source[this._minIndexInclusive + index]);
				}
				found = false;
				return default(TResult);
			}

			public TResult TryGetFirst(out bool found)
			{
				if (this._source.Count > this._minIndexInclusive)
				{
					found = true;
					return this._selector(this._source[this._minIndexInclusive]);
				}
				found = false;
				return default(TResult);
			}

			public TResult TryGetLast(out bool found)
			{
				int num = this._source.Count - 1;
				if (num >= this._minIndexInclusive)
				{
					found = true;
					return this._selector(this._source[Math.Min(num, this._maxIndexInclusive)]);
				}
				found = false;
				return default(TResult);
			}

			private int Count
			{
				get
				{
					int count = this._source.Count;
					if (count <= this._minIndexInclusive)
					{
						return 0;
					}
					return Math.Min(count - 1, this._maxIndexInclusive) - this._minIndexInclusive + 1;
				}
			}

			public TResult[] ToArray()
			{
				int count = this.Count;
				if (count == 0)
				{
					return Array.Empty<TResult>();
				}
				TResult[] array = new TResult[count];
				int num = 0;
				int num2 = this._minIndexInclusive;
				while (num != array.Length)
				{
					array[num] = this._selector(this._source[num2]);
					num++;
					num2++;
				}
				return array;
			}

			public List<TResult> ToList()
			{
				int count = this.Count;
				if (count == 0)
				{
					return new List<TResult>();
				}
				List<TResult> list = new List<TResult>(count);
				int num = this._minIndexInclusive + count;
				for (int num2 = this._minIndexInclusive; num2 != num; num2++)
				{
					list.Add(this._selector(this._source[num2]));
				}
				return list;
			}

			public int GetCount(bool onlyIfCheap)
			{
				int count = this.Count;
				if (!onlyIfCheap)
				{
					int num = this._minIndexInclusive + count;
					for (int num2 = this._minIndexInclusive; num2 != num; num2++)
					{
						this._selector(this._source[num2]);
					}
				}
				return count;
			}

			private readonly IList<TSource> _source;

			private readonly Func<TSource, TResult> _selector;

			private readonly int _minIndexInclusive;

			private readonly int _maxIndexInclusive;
		}

		private sealed class SelectManySingleSelectorIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			internal SelectManySingleSelectorIterator(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
			{
				this._source = source;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.SelectManySingleSelectorIterator<TSource, TResult>(this._source, this._selector);
			}

			public override void Dispose()
			{
				if (this._subEnumerator != null)
				{
					this._subEnumerator.Dispose();
					this._subEnumerator = null;
				}
				if (this._sourceEnumerator != null)
				{
					this._sourceEnumerator.Dispose();
					this._sourceEnumerator = null;
				}
				base.Dispose();
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				int num = 0;
				checked
				{
					foreach (TSource arg in this._source)
					{
						num += this._selector(arg).Count<TResult>();
					}
					return num;
				}
			}

			public override bool MoveNext()
			{
				switch (this._state)
				{
				case 1:
					this._sourceEnumerator = this._source.GetEnumerator();
					this._state = 2;
					break;
				case 2:
					break;
				case 3:
					goto IL_6F;
				default:
					goto IL_AA;
				}
				IL_38:
				if (!this._sourceEnumerator.MoveNext())
				{
					goto IL_AA;
				}
				TSource arg = this._sourceEnumerator.Current;
				this._subEnumerator = this._selector(arg).GetEnumerator();
				this._state = 3;
				IL_6F:
				if (!this._subEnumerator.MoveNext())
				{
					this._subEnumerator.Dispose();
					this._subEnumerator = null;
					this._state = 2;
					goto IL_38;
				}
				this._current = this._subEnumerator.Current;
				return true;
				IL_AA:
				this.Dispose();
				return false;
			}

			public TResult[] ToArray()
			{
				SparseArrayBuilder<TResult> sparseArrayBuilder = new SparseArrayBuilder<TResult>(true);
				ArrayBuilder<IEnumerable<TResult>> arrayBuilder = default(ArrayBuilder<IEnumerable<TResult>>);
				foreach (TSource arg in this._source)
				{
					IEnumerable<TResult> enumerable = this._selector(arg);
					if (sparseArrayBuilder.ReserveOrAdd(enumerable))
					{
						arrayBuilder.Add(enumerable);
					}
				}
				TResult[] array = sparseArrayBuilder.ToArray();
				ArrayBuilder<Marker> markers = sparseArrayBuilder.Markers;
				for (int i = 0; i < markers.Count; i++)
				{
					Marker marker = markers[i];
					EnumerableHelpers.Copy<TResult>(arrayBuilder[i], array, marker.Index, marker.Count);
				}
				return array;
			}

			public List<TResult> ToList()
			{
				List<TResult> list = new List<TResult>();
				foreach (TSource arg in this._source)
				{
					list.AddRange(this._selector(arg));
				}
				return list;
			}

			private readonly IEnumerable<TSource> _source;

			private readonly Func<TSource, IEnumerable<TResult>> _selector;

			private IEnumerator<TSource> _sourceEnumerator;

			private IEnumerator<TResult> _subEnumerator;
		}

		private abstract class UnionIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			protected UnionIterator(IEqualityComparer<TSource> comparer)
			{
				this._comparer = comparer;
			}

			public sealed override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
					this._set = null;
				}
				base.Dispose();
			}

			internal abstract IEnumerable<TSource> GetEnumerable(int index);

			internal abstract Enumerable.UnionIterator<TSource> Union(IEnumerable<TSource> next);

			private void SetEnumerator(IEnumerator<TSource> enumerator)
			{
				IEnumerator<TSource> enumerator2 = this._enumerator;
				if (enumerator2 != null)
				{
					enumerator2.Dispose();
				}
				this._enumerator = enumerator;
			}

			private void StoreFirst()
			{
				Set<TSource> set = new Set<TSource>(this._comparer);
				TSource tsource = this._enumerator.Current;
				set.Add(tsource);
				this._current = tsource;
				this._set = set;
			}

			private bool GetNext()
			{
				Set<TSource> set = this._set;
				while (this._enumerator.MoveNext())
				{
					TSource tsource = this._enumerator.Current;
					if (set.Add(tsource))
					{
						this._current = tsource;
						return true;
					}
				}
				return false;
			}

			public sealed override bool MoveNext()
			{
				if (this._state == 1)
				{
					for (IEnumerable<TSource> enumerable = this.GetEnumerable(0); enumerable != null; enumerable = this.GetEnumerable(this._state - 1))
					{
						IEnumerator<TSource> enumerator = enumerable.GetEnumerator();
						this._state++;
						if (enumerator.MoveNext())
						{
							this.SetEnumerator(enumerator);
							this.StoreFirst();
							return true;
						}
					}
				}
				else if (this._state > 0)
				{
					while (!this.GetNext())
					{
						IEnumerable<TSource> enumerable2 = this.GetEnumerable(this._state - 1);
						if (enumerable2 == null)
						{
							goto IL_94;
						}
						this.SetEnumerator(enumerable2.GetEnumerator());
						this._state++;
					}
					return true;
				}
				IL_94:
				this.Dispose();
				return false;
			}

			private Set<TSource> FillSet()
			{
				Set<TSource> set = new Set<TSource>(this._comparer);
				int num = 0;
				for (;;)
				{
					IEnumerable<TSource> enumerable = this.GetEnumerable(num);
					if (enumerable == null)
					{
						break;
					}
					set.UnionWith(enumerable);
					num++;
				}
				return set;
			}

			public TSource[] ToArray()
			{
				return this.FillSet().ToArray();
			}

			public List<TSource> ToList()
			{
				return this.FillSet().ToList();
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (!onlyIfCheap)
				{
					return this.FillSet().Count;
				}
				return -1;
			}

			internal readonly IEqualityComparer<TSource> _comparer;

			private IEnumerator<TSource> _enumerator;

			private Set<TSource> _set;
		}

		private sealed class UnionIterator2<TSource> : Enumerable.UnionIterator<TSource>
		{
			public UnionIterator2(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) : base(comparer)
			{
				this._first = first;
				this._second = second;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.UnionIterator2<TSource>(this._first, this._second, this._comparer);
			}

			internal override IEnumerable<TSource> GetEnumerable(int index)
			{
				if (index == 0)
				{
					return this._first;
				}
				if (index != 1)
				{
					return null;
				}
				return this._second;
			}

			internal override Enumerable.UnionIterator<TSource> Union(IEnumerable<TSource> next)
			{
				return new Enumerable.UnionIteratorN<TSource>(new SingleLinkedNode<IEnumerable<TSource>>(this._first).Add(this._second).Add(next), 2, this._comparer);
			}

			private readonly IEnumerable<TSource> _first;

			private readonly IEnumerable<TSource> _second;
		}

		private sealed class UnionIteratorN<TSource> : Enumerable.UnionIterator<TSource>
		{
			public UnionIteratorN(SingleLinkedNode<IEnumerable<TSource>> sources, int headIndex, IEqualityComparer<TSource> comparer) : base(comparer)
			{
				this._sources = sources;
				this._headIndex = headIndex;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.UnionIteratorN<TSource>(this._sources, this._headIndex, this._comparer);
			}

			internal override IEnumerable<TSource> GetEnumerable(int index)
			{
				if (index <= this._headIndex)
				{
					return this._sources.GetNode(this._headIndex - index).Item;
				}
				return null;
			}

			internal override Enumerable.UnionIterator<TSource> Union(IEnumerable<TSource> next)
			{
				if (this._headIndex == 2147483645)
				{
					return new Enumerable.UnionIterator2<TSource>(this, next, this._comparer);
				}
				return new Enumerable.UnionIteratorN<TSource>(this._sources.Add(next), this._headIndex + 1, this._comparer);
			}

			private readonly SingleLinkedNode<IEnumerable<TSource>> _sources;

			private readonly int _headIndex;
		}

		private sealed class WhereEnumerableIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			public WhereEnumerableIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate)
			{
				this._source = source;
				this._predicate = predicate;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.WhereEnumerableIterator<TSource>(this._source, this._predicate);
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				int num = 0;
				checked
				{
					foreach (TSource arg in this._source)
					{
						if (this._predicate(arg))
						{
							num++;
						}
					}
					return num;
				}
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						return false;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				while (this._enumerator.MoveNext())
				{
					TSource tsource = this._enumerator.Current;
					if (this._predicate(tsource))
					{
						this._current = tsource;
						return true;
					}
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
			{
				return new Enumerable.WhereSelectEnumerableIterator<TSource, TResult>(this._source, this._predicate, selector);
			}

			public TSource[] ToArray()
			{
				LargeArrayBuilder<TSource> largeArrayBuilder = new LargeArrayBuilder<TSource>(true);
				foreach (TSource tsource in this._source)
				{
					if (this._predicate(tsource))
					{
						largeArrayBuilder.Add(tsource);
					}
				}
				return largeArrayBuilder.ToArray();
			}

			public List<TSource> ToList()
			{
				List<TSource> list = new List<TSource>();
				foreach (TSource tsource in this._source)
				{
					if (this._predicate(tsource))
					{
						list.Add(tsource);
					}
				}
				return list;
			}

			public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
			{
				return new Enumerable.WhereEnumerableIterator<TSource>(this._source, Utilities.CombinePredicates<TSource>(this._predicate, predicate));
			}

			private readonly IEnumerable<TSource> _source;

			private readonly Func<TSource, bool> _predicate;

			private IEnumerator<TSource> _enumerator;
		}

		internal sealed class WhereArrayIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			public WhereArrayIterator(TSource[] source, Func<TSource, bool> predicate)
			{
				this._source = source;
				this._predicate = predicate;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.WhereArrayIterator<TSource>(this._source, this._predicate);
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				int num = 0;
				checked
				{
					foreach (TSource arg in this._source)
					{
						if (this._predicate(arg))
						{
							num++;
						}
					}
					return num;
				}
			}

			public override bool MoveNext()
			{
				int i = this._state - 1;
				TSource[] source = this._source;
				while (i < source.Length)
				{
					TSource tsource = source[i];
					int state = this._state;
					this._state = state + 1;
					i = state;
					if (this._predicate(tsource))
					{
						this._current = tsource;
						return true;
					}
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
			{
				return new Enumerable.WhereSelectArrayIterator<TSource, TResult>(this._source, this._predicate, selector);
			}

			public TSource[] ToArray()
			{
				LargeArrayBuilder<TSource> largeArrayBuilder = new LargeArrayBuilder<TSource>(this._source.Length);
				foreach (TSource tsource in this._source)
				{
					if (this._predicate(tsource))
					{
						largeArrayBuilder.Add(tsource);
					}
				}
				return largeArrayBuilder.ToArray();
			}

			public List<TSource> ToList()
			{
				List<TSource> list = new List<TSource>();
				foreach (TSource tsource in this._source)
				{
					if (this._predicate(tsource))
					{
						list.Add(tsource);
					}
				}
				return list;
			}

			public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
			{
				return new Enumerable.WhereArrayIterator<TSource>(this._source, Utilities.CombinePredicates<TSource>(this._predicate, predicate));
			}

			private readonly TSource[] _source;

			private readonly Func<TSource, bool> _predicate;
		}

		private sealed class WhereListIterator<TSource> : Enumerable.Iterator<TSource>, IIListProvider<TSource>, IEnumerable<!0>, IEnumerable
		{
			public WhereListIterator(List<TSource> source, Func<TSource, bool> predicate)
			{
				this._source = source;
				this._predicate = predicate;
			}

			public override Enumerable.Iterator<TSource> Clone()
			{
				return new Enumerable.WhereListIterator<TSource>(this._source, this._predicate);
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				int num = 0;
				for (int i = 0; i < this._source.Count; i++)
				{
					TSource arg = this._source[i];
					checked
					{
						if (this._predicate(arg))
						{
							num++;
						}
					}
				}
				return num;
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						return false;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				while (this._enumerator.MoveNext())
				{
					TSource tsource = this._enumerator.Current;
					if (this._predicate(tsource))
					{
						this._current = tsource;
						return true;
					}
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
			{
				return new Enumerable.WhereSelectListIterator<TSource, TResult>(this._source, this._predicate, selector);
			}

			public TSource[] ToArray()
			{
				LargeArrayBuilder<TSource> largeArrayBuilder = new LargeArrayBuilder<TSource>(this._source.Count);
				for (int i = 0; i < this._source.Count; i++)
				{
					TSource tsource = this._source[i];
					if (this._predicate(tsource))
					{
						largeArrayBuilder.Add(tsource);
					}
				}
				return largeArrayBuilder.ToArray();
			}

			public List<TSource> ToList()
			{
				List<TSource> list = new List<TSource>();
				for (int i = 0; i < this._source.Count; i++)
				{
					TSource tsource = this._source[i];
					if (this._predicate(tsource))
					{
						list.Add(tsource);
					}
				}
				return list;
			}

			public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
			{
				return new Enumerable.WhereListIterator<TSource>(this._source, Utilities.CombinePredicates<TSource>(this._predicate, predicate));
			}

			private readonly List<TSource> _source;

			private readonly Func<TSource, bool> _predicate;

			private List<TSource>.Enumerator _enumerator;
		}

		private sealed class WhereSelectArrayIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public WhereSelectArrayIterator(TSource[] source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
			{
				this._source = source;
				this._predicate = predicate;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.WhereSelectArrayIterator<TSource, TResult>(this._source, this._predicate, this._selector);
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				int num = 0;
				checked
				{
					foreach (TSource arg in this._source)
					{
						if (this._predicate(arg))
						{
							this._selector(arg);
							num++;
						}
					}
					return num;
				}
			}

			public override bool MoveNext()
			{
				int i = this._state - 1;
				TSource[] source = this._source;
				while (i < source.Length)
				{
					TSource arg = source[i];
					int state = this._state;
					this._state = state + 1;
					i = state;
					if (this._predicate(arg))
					{
						this._current = this._selector(arg);
						return true;
					}
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.WhereSelectArrayIterator<TSource, TResult2>(this._source, this._predicate, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector));
			}

			public TResult[] ToArray()
			{
				LargeArrayBuilder<TResult> largeArrayBuilder = new LargeArrayBuilder<TResult>(this._source.Length);
				foreach (TSource arg in this._source)
				{
					if (this._predicate(arg))
					{
						largeArrayBuilder.Add(this._selector(arg));
					}
				}
				return largeArrayBuilder.ToArray();
			}

			public List<TResult> ToList()
			{
				List<TResult> list = new List<TResult>();
				foreach (TSource arg in this._source)
				{
					if (this._predicate(arg))
					{
						list.Add(this._selector(arg));
					}
				}
				return list;
			}

			private readonly TSource[] _source;

			private readonly Func<TSource, bool> _predicate;

			private readonly Func<TSource, TResult> _selector;
		}

		private sealed class WhereSelectListIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public WhereSelectListIterator(List<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
			{
				this._source = source;
				this._predicate = predicate;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.WhereSelectListIterator<TSource, TResult>(this._source, this._predicate, this._selector);
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				int num = 0;
				for (int i = 0; i < this._source.Count; i++)
				{
					TSource arg = this._source[i];
					checked
					{
						if (this._predicate(arg))
						{
							this._selector(arg);
							num++;
						}
					}
				}
				return num;
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						return false;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				while (this._enumerator.MoveNext())
				{
					TSource arg = this._enumerator.Current;
					if (this._predicate(arg))
					{
						this._current = this._selector(arg);
						return true;
					}
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.WhereSelectListIterator<TSource, TResult2>(this._source, this._predicate, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector));
			}

			public TResult[] ToArray()
			{
				LargeArrayBuilder<TResult> largeArrayBuilder = new LargeArrayBuilder<TResult>(this._source.Count);
				for (int i = 0; i < this._source.Count; i++)
				{
					TSource arg = this._source[i];
					if (this._predicate(arg))
					{
						largeArrayBuilder.Add(this._selector(arg));
					}
				}
				return largeArrayBuilder.ToArray();
			}

			public List<TResult> ToList()
			{
				List<TResult> list = new List<TResult>();
				for (int i = 0; i < this._source.Count; i++)
				{
					TSource arg = this._source[i];
					if (this._predicate(arg))
					{
						list.Add(this._selector(arg));
					}
				}
				return list;
			}

			private readonly List<TSource> _source;

			private readonly Func<TSource, bool> _predicate;

			private readonly Func<TSource, TResult> _selector;

			private List<TSource>.Enumerator _enumerator;
		}

		private sealed class WhereSelectEnumerableIterator<TSource, TResult> : Enumerable.Iterator<TResult>, IIListProvider<TResult>, IEnumerable<!1>, IEnumerable
		{
			public WhereSelectEnumerableIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
			{
				this._source = source;
				this._predicate = predicate;
				this._selector = selector;
			}

			public override Enumerable.Iterator<TResult> Clone()
			{
				return new Enumerable.WhereSelectEnumerableIterator<TSource, TResult>(this._source, this._predicate, this._selector);
			}

			public override void Dispose()
			{
				if (this._enumerator != null)
				{
					this._enumerator.Dispose();
					this._enumerator = null;
				}
				base.Dispose();
			}

			public int GetCount(bool onlyIfCheap)
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				int num = 0;
				checked
				{
					foreach (TSource arg in this._source)
					{
						if (this._predicate(arg))
						{
							this._selector(arg);
							num++;
						}
					}
					return num;
				}
			}

			public override bool MoveNext()
			{
				int state = this._state;
				if (state != 1)
				{
					if (state != 2)
					{
						return false;
					}
				}
				else
				{
					this._enumerator = this._source.GetEnumerator();
					this._state = 2;
				}
				while (this._enumerator.MoveNext())
				{
					TSource arg = this._enumerator.Current;
					if (this._predicate(arg))
					{
						this._current = this._selector(arg);
						return true;
					}
				}
				this.Dispose();
				return false;
			}

			public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
			{
				return new Enumerable.WhereSelectEnumerableIterator<TSource, TResult2>(this._source, this._predicate, Utilities.CombineSelectors<TSource, TResult, TResult2>(this._selector, selector));
			}

			public TResult[] ToArray()
			{
				LargeArrayBuilder<TResult> largeArrayBuilder = new LargeArrayBuilder<TResult>(true);
				foreach (TSource arg in this._source)
				{
					if (this._predicate(arg))
					{
						largeArrayBuilder.Add(this._selector(arg));
					}
				}
				return largeArrayBuilder.ToArray();
			}

			public List<TResult> ToList()
			{
				List<TResult> list = new List<TResult>();
				foreach (TSource arg in this._source)
				{
					if (this._predicate(arg))
					{
						list.Add(this._selector(arg));
					}
				}
				return list;
			}

			private readonly IEnumerable<TSource> _source;

			private readonly Func<TSource, bool> _predicate;

			private readonly Func<TSource, TResult> _selector;

			private IEnumerator<TSource> _enumerator;
		}
	}
}
