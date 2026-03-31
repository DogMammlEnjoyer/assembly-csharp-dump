using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Internal;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Linq
{
	public static class UniTaskAsyncEnumerable
	{
		public static UniTask<TSource> AggregateAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TSource, TSource> accumulator, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TSource, TSource>>(accumulator, "accumulator");
			return Aggregate.AggregateAsync<TSource>(source, accumulator, cancellationToken);
		}

		public static UniTask<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, TAccumulate>>(accumulator, "accumulator");
			return Aggregate.AggregateAsync<TSource, TAccumulate>(source, seed, accumulator, cancellationToken);
		}

		public static UniTask<TResult> AggregateAsync<TSource, TAccumulate, TResult>(this IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, TAccumulate>>(accumulator, "accumulator");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, TAccumulate>>(accumulator, "resultSelector");
			return Aggregate.AggregateAsync<TSource, TAccumulate, TResult>(source, seed, accumulator, resultSelector, cancellationToken);
		}

		public static UniTask<TSource> AggregateAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TSource, UniTask<TSource>> accumulator, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TSource, UniTask<TSource>>>(accumulator, "accumulator");
			return Aggregate.AggregateAwaitAsync<TSource>(source, accumulator, cancellationToken);
		}

		public static UniTask<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(this IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, UniTask<TAccumulate>> accumulator, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, UniTask<TAccumulate>>>(accumulator, "accumulator");
			return Aggregate.AggregateAwaitAsync<TSource, TAccumulate>(source, seed, accumulator, cancellationToken);
		}

		public static UniTask<TResult> AggregateAwaitAsync<TSource, TAccumulate, TResult>(this IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, UniTask<TAccumulate>> accumulator, Func<TAccumulate, UniTask<TResult>> resultSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, UniTask<TAccumulate>>>(accumulator, "accumulator");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, UniTask<TAccumulate>>>(accumulator, "resultSelector");
			return Aggregate.AggregateAwaitAsync<TSource, TAccumulate, TResult>(source, seed, accumulator, resultSelector, cancellationToken);
		}

		public static UniTask<TSource> AggregateAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TSource, CancellationToken, UniTask<TSource>> accumulator, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TSource, CancellationToken, UniTask<TSource>>>(accumulator, "accumulator");
			return Aggregate.AggregateAwaitWithCancellationAsync<TSource>(source, accumulator, cancellationToken);
		}

		public static UniTask<TAccumulate> AggregateAwaitWithCancellationAsync<TSource, TAccumulate>(this IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, UniTask<TAccumulate>> accumulator, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, CancellationToken, UniTask<TAccumulate>>>(accumulator, "accumulator");
			return Aggregate.AggregateAwaitWithCancellationAsync<TSource, TAccumulate>(source, seed, accumulator, cancellationToken);
		}

		public static UniTask<TResult> AggregateAwaitWithCancellationAsync<TSource, TAccumulate, TResult>(this IUniTaskAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, UniTask<TAccumulate>> accumulator, Func<TAccumulate, CancellationToken, UniTask<TResult>> resultSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, CancellationToken, UniTask<TAccumulate>>>(accumulator, "accumulator");
			Error.ThrowArgumentNullException<Func<TAccumulate, TSource, CancellationToken, UniTask<TAccumulate>>>(accumulator, "resultSelector");
			return Aggregate.AggregateAwaitWithCancellationAsync<TSource, TAccumulate, TResult>(source, seed, accumulator, resultSelector, cancellationToken);
		}

		public static UniTask<bool> AllAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return All.AllAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<bool> AllAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return All.AllAwaitAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<bool> AllAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return All.AllAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<bool> AnyAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return Any.AnyAsync<TSource>(source, cancellationToken);
		}

		public static UniTask<bool> AnyAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return Any.AnyAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<bool> AnyAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return Any.AnyAwaitAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<bool> AnyAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return Any.AnyAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken);
		}

		public static IUniTaskAsyncEnumerable<TSource> Append<TSource>(this IUniTaskAsyncEnumerable<TSource> source, TSource element)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new AppendPrepend<TSource>(source, element, true);
		}

		public static IUniTaskAsyncEnumerable<TSource> Prepend<TSource>(this IUniTaskAsyncEnumerable<TSource> source, TSource element)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new AppendPrepend<TSource>(source, element, false);
		}

		public static IUniTaskAsyncEnumerable<TSource> AsUniTaskAsyncEnumerable<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			return source;
		}

		public static UniTask<double> AverageAsync(this IUniTaskAsyncEnumerable<int> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<int>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<double> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> AverageAsync(this IUniTaskAsyncEnumerable<long> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<long>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<double> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> AverageAsync(this IUniTaskAsyncEnumerable<float> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<float>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<float> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> AverageAsync(this IUniTaskAsyncEnumerable<double> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<double>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<double> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> AverageAsync(this IUniTaskAsyncEnumerable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<decimal>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<decimal> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAsync(this IUniTaskAsyncEnumerable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<int?>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<double?> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAsync(this IUniTaskAsyncEnumerable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<long?>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<double?> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> AverageAsync(this IUniTaskAsyncEnumerable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<float?>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<float?> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAsync(this IUniTaskAsyncEnumerable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<double?>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<double?> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> AverageAsync(this IUniTaskAsyncEnumerable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<decimal?>>(source, "source");
			return Average.AverageAsync(source, cancellationToken);
		}

		public static UniTask<decimal?> AverageAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> AverageAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> AverageAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Average.AverageAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static IUniTaskAsyncEnumerable<IList<TSource>> Buffer<TSource>(this IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			if (count <= 0)
			{
				throw Error.ArgumentOutOfRange("count");
			}
			return new Buffer<TSource>(source, count);
		}

		public static IUniTaskAsyncEnumerable<IList<TSource>> Buffer<TSource>(this IUniTaskAsyncEnumerable<TSource> source, int count, int skip)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			if (count <= 0)
			{
				throw Error.ArgumentOutOfRange("count");
			}
			if (skip <= 0)
			{
				throw Error.ArgumentOutOfRange("skip");
			}
			return new BufferSkip<TSource>(source, count, skip);
		}

		public static IUniTaskAsyncEnumerable<TResult> Cast<TResult>(this IUniTaskAsyncEnumerable<object> source)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<object>>(source, "source");
			return new Cast<TResult>(source);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, Func<T1, T2, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<Func<T1, T2, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, TResult>(source1, source2, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, Func<T1, T2, T3, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, TResult>(source1, source2, source3, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, Func<T1, T2, T3, T4, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, TResult>(source1, source2, source3, source4, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, Func<T1, T2, T3, T4, T5, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, TResult>(source1, source2, source3, source4, source5, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, Func<T1, T2, T3, T4, T5, T6, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, TResult>(source1, source2, source3, source4, source5, source6, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, Func<T1, T2, T3, T4, T5, T6, T7, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, TResult>(source1, source2, source3, source4, source5, source6, source7, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T8>>(source8, "source8");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(source1, source2, source3, source4, source5, source6, source7, source8, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T8>>(source8, "source8");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T9>>(source9, "source9");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(source1, source2, source3, source4, source5, source6, source7, source8, source9, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, IUniTaskAsyncEnumerable<T10> source10, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T8>>(source8, "source8");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T9>>(source9, "source9");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T10>>(source10, "source10");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(source1, source2, source3, source4, source5, source6, source7, source8, source9, source10, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, IUniTaskAsyncEnumerable<T10> source10, IUniTaskAsyncEnumerable<T11> source11, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T8>>(source8, "source8");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T9>>(source9, "source9");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T10>>(source10, "source10");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T11>>(source11, "source11");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(source1, source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, IUniTaskAsyncEnumerable<T10> source10, IUniTaskAsyncEnumerable<T11> source11, IUniTaskAsyncEnumerable<T12> source12, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T8>>(source8, "source8");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T9>>(source9, "source9");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T10>>(source10, "source10");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T11>>(source11, "source11");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T12>>(source12, "source12");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(source1, source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, IUniTaskAsyncEnumerable<T10> source10, IUniTaskAsyncEnumerable<T11> source11, IUniTaskAsyncEnumerable<T12> source12, IUniTaskAsyncEnumerable<T13> source13, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T8>>(source8, "source8");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T9>>(source9, "source9");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T10>>(source10, "source10");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T11>>(source11, "source11");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T12>>(source12, "source12");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T13>>(source13, "source13");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(source1, source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, IUniTaskAsyncEnumerable<T10> source10, IUniTaskAsyncEnumerable<T11> source11, IUniTaskAsyncEnumerable<T12> source12, IUniTaskAsyncEnumerable<T13> source13, IUniTaskAsyncEnumerable<T14> source14, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T8>>(source8, "source8");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T9>>(source9, "source9");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T10>>(source10, "source10");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T11>>(source11, "source11");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T12>>(source12, "source12");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T13>>(source13, "source13");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T14>>(source14, "source14");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(source1, source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, source14, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this IUniTaskAsyncEnumerable<T1> source1, IUniTaskAsyncEnumerable<T2> source2, IUniTaskAsyncEnumerable<T3> source3, IUniTaskAsyncEnumerable<T4> source4, IUniTaskAsyncEnumerable<T5> source5, IUniTaskAsyncEnumerable<T6> source6, IUniTaskAsyncEnumerable<T7> source7, IUniTaskAsyncEnumerable<T8> source8, IUniTaskAsyncEnumerable<T9> source9, IUniTaskAsyncEnumerable<T10> source10, IUniTaskAsyncEnumerable<T11> source11, IUniTaskAsyncEnumerable<T12> source12, IUniTaskAsyncEnumerable<T13> source13, IUniTaskAsyncEnumerable<T14> source14, IUniTaskAsyncEnumerable<T15> source15, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T1>>(source1, "source1");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T2>>(source2, "source2");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T3>>(source3, "source3");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T4>>(source4, "source4");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T5>>(source5, "source5");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T6>>(source6, "source6");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T7>>(source7, "source7");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T8>>(source8, "source8");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T9>>(source9, "source9");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T10>>(source10, "source10");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T11>>(source11, "source11");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T12>>(source12, "source12");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T13>>(source13, "source13");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T14>>(source14, "source14");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<T15>>(source15, "source15");
			Error.ThrowArgumentNullException<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>(resultSelector, "resultSelector");
			return new CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(source1, source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, source14, source15, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TSource> Concat<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(second, "second");
			return new Concat<TSource>(first, second);
		}

		public static UniTask<bool> ContainsAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, TSource value, CancellationToken cancellationToken = default(CancellationToken))
		{
			return source.ContainsAsync(value, EqualityComparer<TSource>.Default, cancellationToken);
		}

		public static UniTask<bool> ContainsAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IEqualityComparer<TSource>>(comparer, "comparer");
			return Contains.ContainsAsync<TSource>(source, value, comparer, cancellationToken);
		}

		public static UniTask<int> CountAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return Count.CountAsync<TSource>(source, cancellationToken);
		}

		public static UniTask<int> CountAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return Count.CountAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<int> CountAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return Count.CountAwaitAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<int> CountAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return Count.CountAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken);
		}

		public static IUniTaskAsyncEnumerable<T> Create<T>(Func<IAsyncWriter<T>, CancellationToken, UniTask> create)
		{
			Error.ThrowArgumentNullException<Func<IAsyncWriter<T>, CancellationToken, UniTask>>(create, "create");
			return new Create<T>(create);
		}

		public static IUniTaskAsyncEnumerable<TSource> DefaultIfEmpty<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new DefaultIfEmpty<TSource>(source, default(TSource));
		}

		public static IUniTaskAsyncEnumerable<TSource> DefaultIfEmpty<TSource>(this IUniTaskAsyncEnumerable<TSource> source, TSource defaultValue)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new DefaultIfEmpty<TSource>(source, defaultValue);
		}

		public static IUniTaskAsyncEnumerable<TSource> Distinct<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			return source.Distinct(EqualityComparer<TSource>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> Distinct<TSource>(this IUniTaskAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IEqualityComparer<TSource>>(comparer, "comparer");
			return new Distinct<TSource>(source, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> Distinct<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.Distinct(keySelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> Distinct<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new Distinct<TSource, TKey>(source, keySelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector)
		{
			return source.DistinctAwait(keySelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new DistinctAwait<TSource, TKey>(source, keySelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector)
		{
			return source.DistinctAwaitWithCancellation(keySelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new DistinctAwaitWithCancellation<TSource, TKey>(source, keySelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctUntilChanged<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			return source.DistinctUntilChanged(EqualityComparer<TSource>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctUntilChanged<TSource>(this IUniTaskAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IEqualityComparer<TSource>>(comparer, "comparer");
			return new DistinctUntilChanged<TSource>(source, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctUntilChanged<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.DistinctUntilChanged(keySelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctUntilChanged<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new DistinctUntilChanged<TSource, TKey>(source, keySelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctUntilChangedAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector)
		{
			return source.DistinctUntilChangedAwait(keySelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctUntilChangedAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new DistinctUntilChangedAwait<TSource, TKey>(source, keySelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctUntilChangedAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector)
		{
			return source.DistinctUntilChangedAwaitWithCancellation(keySelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> DistinctUntilChangedAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new DistinctUntilChangedAwaitWithCancellation<TSource, TKey>(source, keySelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> Do<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return source.Do(onNext, null, null);
		}

		public static IUniTaskAsyncEnumerable<TSource> Do<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return source.Do(onNext, onError, null);
		}

		public static IUniTaskAsyncEnumerable<TSource> Do<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action onCompleted)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return source.Do(onNext, null, onCompleted);
		}

		public static IUniTaskAsyncEnumerable<TSource> Do<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new Do<TSource>(source, onNext, onError, onCompleted);
		}

		public static IUniTaskAsyncEnumerable<TSource> Do<TSource>(this IUniTaskAsyncEnumerable<TSource> source, IObserver<TSource> observer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IObserver<TSource>>(observer, "observer");
			return source.Do(new Action<TSource>(observer.OnNext), new Action<Exception>(observer.OnError), new Action(observer.OnCompleted));
		}

		public static UniTask<TSource> ElementAtAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return ElementAt.ElementAtAsync<TSource>(source, index, cancellationToken, false);
		}

		public static UniTask<TSource> ElementAtOrDefaultAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return ElementAt.ElementAtAsync<TSource>(source, index, cancellationToken, true);
		}

		public static IUniTaskAsyncEnumerable<T> Empty<T>()
		{
			return Empty<T>.Instance;
		}

		public static IUniTaskAsyncEnumerable<TSource> Except<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(second, "second");
			return new Except<TSource>(first, second, EqualityComparer<TSource>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> Except<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(second, "second");
			Error.ThrowArgumentNullException<IEqualityComparer<TSource>>(comparer, "comparer");
			return new Except<TSource>(first, second, comparer);
		}

		public static UniTask<TSource> FirstAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return First.FirstAsync<TSource>(source, cancellationToken, false);
		}

		public static UniTask<TSource> FirstAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return First.FirstAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> FirstAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return First.FirstAwaitAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> FirstAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return First.FirstAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> FirstOrDefaultAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return First.FirstAsync<TSource>(source, cancellationToken, true);
		}

		public static UniTask<TSource> FirstOrDefaultAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return First.FirstAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static UniTask<TSource> FirstOrDefaultAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return First.FirstAwaitAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static UniTask<TSource> FirstOrDefaultAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return First.FirstAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static UniTask ForEachAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Action<TSource>>(action, "action");
			return ForEach.ForEachAsync<TSource>(source, action, cancellationToken);
		}

		public static UniTask ForEachAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource, int> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Action<TSource, int>>(action, "action");
			return ForEach.ForEachAsync<TSource>(source, action, cancellationToken);
		}

		[Obsolete("Use ForEachAwaitAsync instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static UniTask ForEachAsync<T>(this IUniTaskAsyncEnumerable<T> source, Func<T, UniTask> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException("Use ForEachAwaitAsync instead.");
		}

		[Obsolete("Use ForEachAwaitAsync instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static UniTask ForEachAsync<T>(this IUniTaskAsyncEnumerable<T> source, Func<T, int, UniTask> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException("Use ForEachAwaitAsync instead.");
		}

		public static UniTask ForEachAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask>>(action, "action");
			return ForEach.ForEachAwaitAsync<TSource>(source, action, cancellationToken);
		}

		public static UniTask ForEachAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, UniTask>>(action, "action");
			return ForEach.ForEachAwaitAsync<TSource>(source, action, cancellationToken);
		}

		public static UniTask ForEachAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask>>(action, "action");
			return ForEach.ForEachAwaitWithCancellationAsync<TSource>(source, action, cancellationToken);
		}

		public static UniTask ForEachAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, CancellationToken, UniTask>>(action, "action");
			return ForEach.ForEachAwaitWithCancellationAsync<TSource>(source, action, cancellationToken);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			return new GroupBy<TSource, TKey, TSource>(source, keySelector, (TSource x) => x, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupBy<TSource, TKey, TSource>(source, keySelector, (TSource x) => x, comparer);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, TElement>>(elementSelector, "elementSelector");
			return new GroupBy<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, TElement>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupBy<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TSource>, TResult>>(resultSelector, "resultSelector");
			return new GroupBy<TSource, TKey, TSource, TResult>(source, keySelector, (TSource x) => x, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TSource>, TResult>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupBy<TSource, TKey, TSource, TResult>(source, keySelector, (TSource x) => x, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, TElement>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TElement>, TResult>>(resultSelector, "resultSelector");
			return new GroupBy<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, TElement>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TElement>, TResult>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupBy<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TSource>> GroupByAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			return new GroupByAwait<TSource, TKey, TSource>(source, keySelector, (TSource x) => UniTask.FromResult<TSource>(x), EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TSource>> GroupByAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupByAwait<TSource, TKey, TSource>(source, keySelector, (TSource x) => UniTask.FromResult<TSource>(x), comparer);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TElement>> GroupByAwait<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TElement>>>(elementSelector, "elementSelector");
			return new GroupByAwait<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TElement>> GroupByAwait<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupByAwait<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupByAwait<TSource, TKey, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TSource>, UniTask<TResult>>>(resultSelector, "resultSelector");
			return new GroupByAwait<TSource, TKey, TSource, TResult>(source, keySelector, (TSource x) => UniTask.FromResult<TSource>(x), resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupByAwait<TSource, TKey, TElement, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TElement>, UniTask<TResult>>>(resultSelector, "resultSelector");
			return new GroupByAwait<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupByAwait<TSource, TKey, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TSource>, UniTask<TResult>>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupByAwait<TSource, TKey, TSource, TResult>(source, keySelector, (TSource x) => UniTask.FromResult<TSource>(x), resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupByAwait<TSource, TKey, TElement, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TElement>, UniTask<TResult>>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupByAwait<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TSource>> GroupByAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			return new GroupByAwaitWithCancellation<TSource, TKey, TSource>(source, keySelector, (TSource x, CancellationToken _) => UniTask.FromResult<TSource>(x), EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TSource>> GroupByAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupByAwaitWithCancellation<TSource, TKey, TSource>(source, keySelector, (TSource x, CancellationToken _) => UniTask.FromResult<TSource>(x), comparer);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TElement>> GroupByAwaitWithCancellation<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TElement>>>(elementSelector, "elementSelector");
			return new GroupByAwaitWithCancellation<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<IGrouping<TKey, TElement>> GroupByAwaitWithCancellation<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupByAwaitWithCancellation<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupByAwaitWithCancellation<TSource, TKey, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, CancellationToken, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TSource>, CancellationToken, UniTask<TResult>>>(resultSelector, "resultSelector");
			return new GroupByAwaitWithCancellation<TSource, TKey, TSource, TResult>(source, keySelector, (TSource x, CancellationToken _) => UniTask.FromResult<TSource>(x), resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, CancellationToken, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TElement>, CancellationToken, UniTask<TResult>>>(resultSelector, "resultSelector");
			return new GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupByAwaitWithCancellation<TSource, TKey, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TSource>, CancellationToken, UniTask<TResult>>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupByAwaitWithCancellation<TSource, TKey, TSource, TResult>(source, keySelector, (TSource x, CancellationToken _) => UniTask.FromResult<TSource>(x), resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<Func<TKey, IEnumerable<TElement>, CancellationToken, UniTask<TResult>>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, TKey>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, TKey>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, IEnumerable<TInner>, TResult>>(resultSelector, "resultSelector");
			return new GroupJoin<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, TKey>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, TKey>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, IEnumerable<TInner>, TResult>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupJoin<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupJoinAwait<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, UniTask<TKey>> outerKeySelector, Func<TInner, UniTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, UniTask<TKey>>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, UniTask<TKey>>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, IEnumerable<TInner>, UniTask<TResult>>>(resultSelector, "resultSelector");
			return new GroupJoinAwait<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupJoinAwait<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, UniTask<TKey>> outerKeySelector, Func<TInner, UniTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, UniTask<TKey>>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, UniTask<TKey>>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, IEnumerable<TInner>, UniTask<TResult>>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupJoinAwait<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, CancellationToken, UniTask<TKey>>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, CancellationToken, UniTask<TKey>>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, IEnumerable<TInner>, CancellationToken, UniTask<TResult>>>(resultSelector, "resultSelector");
			return new GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, CancellationToken, UniTask<TKey>>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, CancellationToken, UniTask<TKey>>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, IEnumerable<TInner>, CancellationToken, UniTask<TResult>>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new GroupJoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TSource> Intersect<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(second, "second");
			return new Intersect<TSource>(first, second, EqualityComparer<TSource>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> Intersect<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(second, "second");
			Error.ThrowArgumentNullException<IEqualityComparer<TSource>>(comparer, "comparer");
			return new Intersect<TSource>(first, second, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, TKey>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, TKey>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, TInner, TResult>>(resultSelector, "resultSelector");
			return new Join<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, TKey>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, TKey>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, TInner, TResult>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new Join<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> JoinAwait<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, UniTask<TKey>> outerKeySelector, Func<TInner, UniTask<TKey>> innerKeySelector, Func<TOuter, TInner, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, UniTask<TKey>>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, UniTask<TKey>>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, TInner, UniTask<TResult>>>(resultSelector, "resultSelector");
			return new JoinAwait<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> JoinAwait<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, UniTask<TKey>> outerKeySelector, Func<TInner, UniTask<TKey>> innerKeySelector, Func<TOuter, TInner, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, UniTask<TKey>>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, UniTask<TKey>>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, TInner, UniTask<TResult>>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new JoinAwait<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		public static IUniTaskAsyncEnumerable<TResult> JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector, Func<TOuter, TInner, CancellationToken, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, CancellationToken, UniTask<TKey>>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, CancellationToken, UniTask<TKey>>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, TInner, CancellationToken, UniTask<TResult>>>(resultSelector, "resultSelector");
			return new JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
		}

		public static IUniTaskAsyncEnumerable<TResult> JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(this IUniTaskAsyncEnumerable<TOuter> outer, IUniTaskAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, UniTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, UniTask<TKey>> innerKeySelector, Func<TOuter, TInner, CancellationToken, UniTask<TResult>> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TOuter>>(outer, "outer");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TInner>>(inner, "inner");
			Error.ThrowArgumentNullException<Func<TOuter, CancellationToken, UniTask<TKey>>>(outerKeySelector, "outerKeySelector");
			Error.ThrowArgumentNullException<Func<TInner, CancellationToken, UniTask<TKey>>>(innerKeySelector, "innerKeySelector");
			Error.ThrowArgumentNullException<Func<TOuter, TInner, CancellationToken, UniTask<TResult>>>(resultSelector, "resultSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return new JoinAwaitWithCancellation<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		public static UniTask<TSource> LastAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return Last.LastAsync<TSource>(source, cancellationToken, false);
		}

		public static UniTask<TSource> LastAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return Last.LastAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> LastAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return Last.LastAwaitAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> LastAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return Last.LastAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> LastOrDefaultAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return Last.LastAsync<TSource>(source, cancellationToken, true);
		}

		public static UniTask<TSource> LastOrDefaultAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return Last.LastAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static UniTask<TSource> LastOrDefaultAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return Last.LastAwaitAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static UniTask<TSource> LastOrDefaultAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return Last.LastAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static UniTask<long> LongCountAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return LongCount.LongCountAsync<TSource>(source, cancellationToken);
		}

		public static UniTask<long> LongCountAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return LongCount.LongCountAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<long> LongCountAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return LongCount.LongCountAwaitAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<long> LongCountAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return LongCount.LongCountAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken);
		}

		public static UniTask<TSource> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return Max.MaxAsync<TSource>(source, cancellationToken);
		}

		public static UniTask<TResult> MaxAsync<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource, TResult>(source, selector, cancellationToken);
		}

		public static UniTask<TResult> MaxAwaitAsync<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource, TResult>(source, selector, cancellationToken);
		}

		public static UniTask<TResult> MaxAwaitWithCancellationAsync<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource, TResult>(source, selector, cancellationToken);
		}

		public static UniTask<TSource> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return Min.MinAsync<TSource>(source, cancellationToken);
		}

		public static UniTask<TResult> MinAsync<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TResult> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource, TResult>(source, selector, cancellationToken);
		}

		public static UniTask<TResult> MinAwaitAsync<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource, TResult>(source, selector, cancellationToken);
		}

		public static UniTask<TResult> MinAwaitWithCancellationAsync<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource, TResult>(source, selector, cancellationToken);
		}

		public static UniTask<int> MinAsync(this IUniTaskAsyncEnumerable<int> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<int>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<int> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> MinAsync(this IUniTaskAsyncEnumerable<long> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<long>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<long> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> MinAsync(this IUniTaskAsyncEnumerable<float> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<float>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<float> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> MinAsync(this IUniTaskAsyncEnumerable<double> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<double>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<double> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> MinAsync(this IUniTaskAsyncEnumerable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<decimal>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<decimal> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> MinAsync(this IUniTaskAsyncEnumerable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<int?>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<int?> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> MinAsync(this IUniTaskAsyncEnumerable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<long?>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<long?> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> MinAsync(this IUniTaskAsyncEnumerable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<float?>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<float?> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> MinAsync(this IUniTaskAsyncEnumerable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<double?>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<double?> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> MinAsync(this IUniTaskAsyncEnumerable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<decimal?>>(source, "source");
			return Min.MinAsync(source, cancellationToken);
		}

		public static UniTask<decimal?> MinAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> MinAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> MinAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Min.MinAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int> MaxAsync(this IUniTaskAsyncEnumerable<int> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<int>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<int> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> MaxAsync(this IUniTaskAsyncEnumerable<long> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<long>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<long> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> MaxAsync(this IUniTaskAsyncEnumerable<float> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<float>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<float> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> MaxAsync(this IUniTaskAsyncEnumerable<double> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<double>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<double> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> MaxAsync(this IUniTaskAsyncEnumerable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<decimal>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<decimal> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> MaxAsync(this IUniTaskAsyncEnumerable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<int?>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<int?> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> MaxAsync(this IUniTaskAsyncEnumerable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<long?>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<long?> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> MaxAsync(this IUniTaskAsyncEnumerable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<float?>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<float?> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> MaxAsync(this IUniTaskAsyncEnumerable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<double?>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<double?> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> MaxAsync(this IUniTaskAsyncEnumerable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<decimal?>>(source, "source");
			return Max.MaxAsync(source, cancellationToken);
		}

		public static UniTask<decimal?> MaxAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> MaxAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> MaxAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Max.MaxAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static IUniTaskAsyncEnumerable<T> Never<T>()
		{
			return Never<T>.Instance;
		}

		public static IUniTaskAsyncEnumerable<TResult> OfType<TResult>(this IUniTaskAsyncEnumerable<object> source)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<object>>(source, "source");
			return new OfType<TResult>(source);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderBy<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			return new OrderedAsyncEnumerable<TSource, TKey>(source, keySelector, Comparer<TKey>.Default, false, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderBy<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return new OrderedAsyncEnumerable<TSource, TKey>(source, keySelector, comparer, false, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			return new OrderedAsyncEnumerableAwait<TSource, TKey>(source, keySelector, Comparer<TKey>.Default, false, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return new OrderedAsyncEnumerableAwait<TSource, TKey>(source, keySelector, comparer, false, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			return new OrderedAsyncEnumerableAwaitWithCancellation<TSource, TKey>(source, keySelector, Comparer<TKey>.Default, false, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return new OrderedAsyncEnumerableAwaitWithCancellation<TSource, TKey>(source, keySelector, comparer, false, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByDescending<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			return new OrderedAsyncEnumerable<TSource, TKey>(source, keySelector, Comparer<TKey>.Default, true, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByDescending<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return new OrderedAsyncEnumerable<TSource, TKey>(source, keySelector, comparer, true, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByDescendingAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			return new OrderedAsyncEnumerableAwait<TSource, TKey>(source, keySelector, Comparer<TKey>.Default, true, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByDescendingAwait<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return new OrderedAsyncEnumerableAwait<TSource, TKey>(source, keySelector, comparer, true, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByDescendingAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			return new OrderedAsyncEnumerableAwaitWithCancellation<TSource, TKey>(source, keySelector, Comparer<TKey>.Default, true, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> OrderByDescendingAwaitWithCancellation<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return new OrderedAsyncEnumerableAwaitWithCancellation<TSource, TKey>(source, keySelector, comparer, true, null);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenBy<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			return source.CreateOrderedEnumerable<TKey>(keySelector, Comparer<TKey>.Default, false);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenBy<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, false);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByAwait<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			return source.CreateOrderedEnumerable<TKey>(keySelector, Comparer<TKey>.Default, false);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByAwait<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, false);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByAwaitWithCancellation<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			return source.CreateOrderedEnumerable<TKey>(keySelector, Comparer<TKey>.Default, false);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByAwaitWithCancellation<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, false);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByDescending<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			return source.CreateOrderedEnumerable<TKey>(keySelector, Comparer<TKey>.Default, true);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByDescending<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, true);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByDescendingAwait<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			return source.CreateOrderedEnumerable<TKey>(keySelector, Comparer<TKey>.Default, true);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByDescendingAwait<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, true);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByDescendingAwaitWithCancellation<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			return source.CreateOrderedEnumerable<TKey>(keySelector, Comparer<TKey>.Default, true);
		}

		public static IUniTaskOrderedAsyncEnumerable<TSource> ThenByDescendingAwaitWithCancellation<TSource, TKey>(this IUniTaskOrderedAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IComparer<TKey> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskOrderedAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IComparer<TKey>>(comparer, "comparer");
			return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, true);
		}

		public static IUniTaskAsyncEnumerable<ValueTuple<TSource, TSource>> Pairwise<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new Pairwise<TSource>(source);
		}

		public static IConnectableUniTaskAsyncEnumerable<TSource> Publish<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new Publish<TSource>(source);
		}

		public static IUniTaskAsyncEnumerable<TSource> Queue<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			return new QueueOperator<TSource>(source);
		}

		public static IUniTaskAsyncEnumerable<int> Range(int start, int count)
		{
			if (count < 0)
			{
				throw Error.ArgumentOutOfRange("count");
			}
			if ((long)start + (long)count - 1L > 2147483647L)
			{
				throw Error.ArgumentOutOfRange("count");
			}
			if (count == 0)
			{
				UniTaskAsyncEnumerable.Empty<int>();
			}
			return new Range(start, count);
		}

		public static IUniTaskAsyncEnumerable<TElement> Repeat<TElement>(TElement element, int count)
		{
			if (count < 0)
			{
				throw Error.ArgumentOutOfRange("count");
			}
			return new Repeat<TElement>(element, count);
		}

		public static IUniTaskAsyncEnumerable<TValue> Return<TValue>(TValue value)
		{
			return new Return<TValue>(value);
		}

		public static IUniTaskAsyncEnumerable<TSource> Reverse<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new Reverse<TSource>(source);
		}

		public static IUniTaskAsyncEnumerable<TResult> Select<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TResult>>(selector, "selector");
			return new Select<TSource, TResult>(source, selector);
		}

		public static IUniTaskAsyncEnumerable<TResult> Select<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, TResult>>(selector, "selector");
			return new SelectInt<TSource, TResult>(source, selector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TResult>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TResult>>>(selector, "selector");
			return new SelectAwait<TSource, TResult>(source, selector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<TResult>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, UniTask<TResult>>>(selector, "selector");
			return new SelectIntAwait<TSource, TResult>(source, selector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectAwaitWithCancellation<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TResult>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TResult>>>(selector, "selector");
			return new SelectAwaitWithCancellation<TSource, TResult>(source, selector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectAwaitWithCancellation<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<TResult>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, CancellationToken, UniTask<TResult>>>(selector, "selector");
			return new SelectIntAwaitWithCancellation<TSource, TResult>(source, selector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, IUniTaskAsyncEnumerable<TResult>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, IUniTaskAsyncEnumerable<TResult>>>(selector, "selector");
			return new SelectMany<TSource, TResult, TResult>(source, selector, (TSource x, TResult y) => y);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, IUniTaskAsyncEnumerable<TResult>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, IUniTaskAsyncEnumerable<TResult>>>(selector, "selector");
			return new SelectMany<TSource, TResult, TResult>(source, selector, (TSource x, TResult y) => y);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, IUniTaskAsyncEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, IUniTaskAsyncEnumerable<TCollection>>>(collectionSelector, "collectionSelector");
			return new SelectMany<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, IUniTaskAsyncEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, IUniTaskAsyncEnumerable<TCollection>>>(collectionSelector, "collectionSelector");
			return new SelectMany<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<IUniTaskAsyncEnumerable<TResult>>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<IUniTaskAsyncEnumerable<TResult>>>>(selector, "selector");
			return new SelectManyAwait<TSource, TResult, TResult>(source, selector, (TSource x, TResult y) => UniTask.FromResult<TResult>(y));
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectManyAwait<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TResult>>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TResult>>>>(selector, "selector");
			return new SelectManyAwait<TSource, TResult, TResult>(source, selector, (TSource x, TResult y) => UniTask.FromResult<TResult>(y));
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectManyAwait<TSource, TCollection, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<IUniTaskAsyncEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<IUniTaskAsyncEnumerable<TCollection>>>>(collectionSelector, "collectionSelector");
			return new SelectManyAwait<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectManyAwait<TSource, TCollection, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, UniTask<IUniTaskAsyncEnumerable<TCollection>>>>(collectionSelector, "collectionSelector");
			return new SelectManyAwait<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectManyAwaitWithCancellation<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TResult>>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TResult>>>>(selector, "selector");
			return new SelectManyAwaitWithCancellation<TSource, TResult, TResult>(source, selector, (TSource x, TResult y, CancellationToken c) => UniTask.FromResult<TResult>(y));
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectManyAwaitWithCancellation<TSource, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TResult>>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TResult>>>>(selector, "selector");
			return new SelectManyAwaitWithCancellation<TSource, TResult, TResult>(source, selector, (TSource x, TResult y, CancellationToken c) => UniTask.FromResult<TResult>(y));
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectManyAwaitWithCancellation<TSource, TCollection, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, CancellationToken, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>>>(collectionSelector, "collectionSelector");
			return new SelectManyAwaitWithCancellation<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> SelectManyAwaitWithCancellation<TSource, TCollection, TResult>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, CancellationToken, UniTask<TResult>> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, CancellationToken, UniTask<IUniTaskAsyncEnumerable<TCollection>>>>(collectionSelector, "collectionSelector");
			return new SelectManyAwaitWithCancellation<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}

		public static UniTask<bool> SequenceEqualAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, CancellationToken cancellationToken = default(CancellationToken))
		{
			return first.SequenceEqualAsync(second, EqualityComparer<TSource>.Default, cancellationToken);
		}

		public static UniTask<bool> SequenceEqualAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(second, "second");
			Error.ThrowArgumentNullException<IEqualityComparer<TSource>>(comparer, "comparer");
			return SequenceEqual.SequenceEqualAsync<TSource>(first, second, comparer, cancellationToken);
		}

		public static UniTask<TSource> SingleAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return SingleOperator.SingleAsync<TSource>(source, cancellationToken, false);
		}

		public static UniTask<TSource> SingleAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return SingleOperator.SingleAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> SingleAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return SingleOperator.SingleAwaitAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> SingleAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return SingleOperator.SingleAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken, false);
		}

		public static UniTask<TSource> SingleOrDefaultAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return SingleOperator.SingleAsync<TSource>(source, cancellationToken, true);
		}

		public static UniTask<TSource> SingleOrDefaultAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return SingleOperator.SingleAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static UniTask<TSource> SingleOrDefaultAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return SingleOperator.SingleAwaitAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static UniTask<TSource> SingleOrDefaultAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return SingleOperator.SingleAwaitWithCancellationAsync<TSource>(source, predicate, cancellationToken, true);
		}

		public static IUniTaskAsyncEnumerable<TSource> Skip<TSource>(this IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new Skip<TSource>(source, count);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipLast<TSource>(this IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			if (count <= 0)
			{
				return source;
			}
			return new SkipLast<TSource>(source, count);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipUntil<TSource>(this IUniTaskAsyncEnumerable<TSource> source, UniTask other)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new SkipUntil<TSource>(source, other, null);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipUntil<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<CancellationToken, UniTask> other)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "other");
			return new SkipUntil<TSource>(source, default(UniTask), other);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipUntilCanceled<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new SkipUntilCanceled<TSource>(source, cancellationToken);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipWhile<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return new SkipWhile<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipWhile<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, bool>>(predicate, "predicate");
			return new SkipWhileInt<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipWhileAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return new SkipWhileAwait<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipWhileAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, UniTask<bool>>>(predicate, "predicate");
			return new SkipWhileIntAwait<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipWhileAwaitWithCancellation<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return new SkipWhileAwaitWithCancellation<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> SkipWhileAwaitWithCancellation<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return new SkipWhileIntAwaitWithCancellation<TSource>(source, predicate);
		}

		public static IDisposable Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> action)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Action<TSource>>(action, "action");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, action, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static IDisposable Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTaskVoid> action)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTaskVoid>>(action, "action");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, action, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static IDisposable Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTaskVoid> action)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTaskVoid>>(action, "action");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, action, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> action, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Action<TSource>>(action, "action");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, action, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static void Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTaskVoid> action, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTaskVoid>>(action, "action");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, action, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static void Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTaskVoid> action, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTaskVoid>>(action, "action");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, action, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static IDisposable SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask> onNext)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask>>(onNext, "onNext");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask> onNext, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask>>(onNext, "onNext");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static IDisposable SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask> onNext)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask>>(onNext, "onNext");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask> onNext, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask>>(onNext, "onNext");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static IDisposable Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Action<TSource>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action<Exception>>(onError, "onError");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, onNext, onError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static IDisposable Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTaskVoid> onNext, Action<Exception> onError)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTaskVoid>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action<Exception>>(onError, "onError");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, onNext, onError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Action<TSource>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action<Exception>>(onError, "onError");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, onNext, onError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static void Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTaskVoid> onNext, Action<Exception> onError, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTaskVoid>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action<Exception>>(onError, "onError");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, onNext, onError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static IDisposable SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask> onNext, Action<Exception> onError)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action<Exception>>(onError, "onError");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, onError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask> onNext, Action<Exception> onError, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action<Exception>>(onError, "onError");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, onError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static IDisposable SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask> onNext, Action<Exception> onError)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action<Exception>>(onError, "onError");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, onError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask> onNext, Action<Exception> onError, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action<Exception>>(onError, "onError");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, onError, Cysharp.Threading.Tasks.Linq.Subscribe.NopCompleted, cancellationToken).Forget();
		}

		public static IDisposable Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action onCompleted)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Action<TSource>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action>(onCompleted, "onCompleted");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, onCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static IDisposable Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTaskVoid> onNext, Action onCompleted)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTaskVoid>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action>(onCompleted, "onCompleted");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, onCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action onCompleted, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Action<TSource>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action>(onCompleted, "onCompleted");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, onCompleted, cancellationToken).Forget();
		}

		public static void Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTaskVoid> onNext, Action onCompleted, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTaskVoid>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action>(onCompleted, "onCompleted");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, onCompleted, cancellationToken).Forget();
		}

		public static IDisposable SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask> onNext, Action onCompleted)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action>(onCompleted, "onCompleted");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, onCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask> onNext, Action onCompleted, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action>(onCompleted, "onCompleted");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, onCompleted, cancellationToken).Forget();
		}

		public static IDisposable SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask> onNext, Action onCompleted)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action>(onCompleted, "onCompleted");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, onCompleted, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void SubscribeAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask> onNext, Action onCompleted, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask>>(onNext, "onNext");
			Error.ThrowArgumentNullException<Action>(onCompleted, "onCompleted");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeAwaitCore<TSource>(source, onNext, Cysharp.Threading.Tasks.Linq.Subscribe.NopError, onCompleted, cancellationToken).Forget();
		}

		public static IDisposable Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, IObserver<TSource> observer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IObserver<TSource>>(observer, "observer");
			CancellationTokenDisposable cancellationTokenDisposable = new CancellationTokenDisposable();
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, observer, cancellationTokenDisposable.Token).Forget();
			return cancellationTokenDisposable;
		}

		public static void Subscribe<TSource>(this IUniTaskAsyncEnumerable<TSource> source, IObserver<TSource> observer, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IObserver<TSource>>(observer, "observer");
			Cysharp.Threading.Tasks.Linq.Subscribe.SubscribeCore<TSource>(source, observer, cancellationToken).Forget();
		}

		public static UniTask<int> SumAsync(this IUniTaskAsyncEnumerable<int> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<int>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<int> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> SumAsync(this IUniTaskAsyncEnumerable<long> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<long>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<long> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> SumAsync(this IUniTaskAsyncEnumerable<float> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<float>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<float> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> SumAsync(this IUniTaskAsyncEnumerable<double> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<double>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<double> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> SumAsync(this IUniTaskAsyncEnumerable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<decimal>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<decimal> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> SumAsync(this IUniTaskAsyncEnumerable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<int?>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<int?> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<int?> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> SumAsync(this IUniTaskAsyncEnumerable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<long?>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<long?> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<long?> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> SumAsync(this IUniTaskAsyncEnumerable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<float?>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<float?> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<float?> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> SumAsync(this IUniTaskAsyncEnumerable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<double?>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<double?> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<double?> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> SumAsync(this IUniTaskAsyncEnumerable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<decimal?>>(source, "source");
			return Sum.SumAsync(source, cancellationToken);
		}

		public static UniTask<decimal?> SumAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> SumAwaitAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitAsync<TSource>(source, selector, cancellationToken);
		}

		public static UniTask<decimal?> SumAwaitWithCancellationAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "selector");
			return Sum.SumAwaitWithCancellationAsync<TSource>(source, selector, cancellationToken);
		}

		public static IUniTaskAsyncEnumerable<TSource> Take<TSource>(this IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new Take<TSource>(source, count);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeLast<TSource>(this IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			if (count <= 0)
			{
				return UniTaskAsyncEnumerable.Empty<TSource>();
			}
			return new TakeLast<TSource>(source, count);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeUntil<TSource>(this IUniTaskAsyncEnumerable<TSource> source, UniTask other)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new TakeUntil<TSource>(source, other, null);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeUntil<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<CancellationToken, UniTask> other)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "other");
			return new TakeUntil<TSource>(source, default(UniTask), other);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeUntilCanceled<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new TakeUntilCanceled<TSource>(source, cancellationToken);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeWhile<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return new TakeWhile<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeWhile<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, bool>>(predicate, "predicate");
			return new TakeWhileInt<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeWhileAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return new TakeWhileAwait<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeWhileAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, UniTask<bool>>>(predicate, "predicate");
			return new TakeWhileIntAwait<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeWhileAwaitWithCancellation<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return new TakeWhileAwaitWithCancellation<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> TakeWhileAwaitWithCancellation<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return new TakeWhileIntAwaitWithCancellation<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TValue> Throw<TValue>(Exception exception)
		{
			return new Throw<TValue>(exception);
		}

		public static UniTask<TSource[]> ToArrayAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return ToArray.ToArrayAsync<TSource>(source, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			return ToDictionary.ToDictionaryAsync<TSource, TKey>(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToDictionary.ToDictionaryAsync<TSource, TKey>(source, keySelector, comparer, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, TElement>>(elementSelector, "elementSelector");
			return ToDictionary.ToDictionaryAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, TElement>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToDictionary.ToDictionaryAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TSource>> ToDictionaryAwaitAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			return ToDictionary.ToDictionaryAwaitAsync<TSource, TKey>(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TSource>> ToDictionaryAwaitAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToDictionary.ToDictionaryAwaitAsync<TSource, TKey>(source, keySelector, comparer, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TElement>> ToDictionaryAwaitAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TElement>>>(elementSelector, "elementSelector");
			return ToDictionary.ToDictionaryAwaitAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TElement>> ToDictionaryAwaitAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToDictionary.ToDictionaryAwaitAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TSource>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			return ToDictionary.ToDictionaryAwaitWithCancellationAsync<TSource, TKey>(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TSource>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToDictionary.ToDictionaryAwaitWithCancellationAsync<TSource, TKey>(source, keySelector, comparer, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TElement>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TElement>>>(elementSelector, "elementSelector");
			return ToDictionary.ToDictionaryAwaitWithCancellationAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<Dictionary<TKey, TElement>> ToDictionaryAwaitWithCancellationAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToDictionary.ToDictionaryAwaitWithCancellationAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer, cancellationToken);
		}

		public static UniTask<HashSet<TSource>> ToHashSetAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return ToHashSet.ToHashSetAsync<TSource>(source, EqualityComparer<TSource>.Default, cancellationToken);
		}

		public static UniTask<HashSet<TSource>> ToHashSetAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<IEqualityComparer<TSource>>(comparer, "comparer");
			return ToHashSet.ToHashSetAsync<TSource>(source, comparer, cancellationToken);
		}

		public static UniTask<List<TSource>> ToListAsync<TSource>(this IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return ToList.ToListAsync<TSource>(source, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			return ToLookup.ToLookupAsync<TSource, TKey>(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToLookup.ToLookupAsync<TSource, TKey>(source, keySelector, comparer, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, TElement>>(elementSelector, "elementSelector");
			return ToLookup.ToLookupAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, TKey>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, TElement>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToLookup.ToLookupAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TSource>> ToLookupAwaitAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			return ToLookup.ToLookupAwaitAsync<TSource, TKey>(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TSource>> ToLookupAwaitAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToLookup.ToLookupAwaitAsync<TSource, TKey>(source, keySelector, comparer, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TElement>> ToLookupAwaitAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TElement>>>(elementSelector, "elementSelector");
			return ToLookup.ToLookupAwaitAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TElement>> ToLookupAwaitAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToLookup.ToLookupAwaitAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TSource>> ToLookupAwaitWithCancellationAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			return ToLookup.ToLookupAwaitWithCancellationAsync<TSource, TKey>(source, keySelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TSource>> ToLookupAwaitWithCancellationAsync<TSource, TKey>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToLookup.ToLookupAwaitWithCancellationAsync<TSource, TKey>(source, keySelector, comparer, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TElement>> ToLookupAwaitWithCancellationAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TElement>>>(elementSelector, "elementSelector");
			return ToLookup.ToLookupAwaitWithCancellationAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default, cancellationToken);
		}

		public static UniTask<ILookup<TKey, TElement>> ToLookupAwaitWithCancellationAsync<TSource, TKey, TElement>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TKey>>>(keySelector, "keySelector");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<TElement>>>(elementSelector, "elementSelector");
			Error.ThrowArgumentNullException<IEqualityComparer<TKey>>(comparer, "comparer");
			return ToLookup.ToLookupAwaitWithCancellationAsync<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer, cancellationToken);
		}

		public static IObservable<TSource> ToObservable<TSource>(this IUniTaskAsyncEnumerable<TSource> source)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			return new ToObservable<TSource>(source);
		}

		public static IUniTaskAsyncEnumerable<TSource> ToUniTaskAsyncEnumerable<TSource>(this IEnumerable<TSource> source)
		{
			Error.ThrowArgumentNullException<IEnumerable<TSource>>(source, "source");
			return new ToUniTaskAsyncEnumerable<TSource>(source);
		}

		public static IUniTaskAsyncEnumerable<TSource> ToUniTaskAsyncEnumerable<TSource>(this Task<TSource> source)
		{
			Error.ThrowArgumentNullException<Task<TSource>>(source, "source");
			return new ToUniTaskAsyncEnumerableTask<TSource>(source);
		}

		public static IUniTaskAsyncEnumerable<TSource> ToUniTaskAsyncEnumerable<TSource>(this UniTask<TSource> source)
		{
			return new ToUniTaskAsyncEnumerableUniTask<TSource>(source);
		}

		public static IUniTaskAsyncEnumerable<TSource> ToUniTaskAsyncEnumerable<TSource>(this IObservable<TSource> source)
		{
			Error.ThrowArgumentNullException<IObservable<TSource>>(source, "source");
			return new ToUniTaskAsyncEnumerableObservable<TSource>(source);
		}

		public static IUniTaskAsyncEnumerable<TSource> Union<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(second, "second");
			return first.Union(second, EqualityComparer<TSource>.Default);
		}

		public static IUniTaskAsyncEnumerable<TSource> Union<TSource>(this IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(second, "second");
			Error.ThrowArgumentNullException<IEqualityComparer<TSource>>(comparer, "comparer");
			return first.Concat(second).Distinct(comparer);
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> EveryUpdate(PlayerLoopTiming updateTiming = PlayerLoopTiming.Update)
		{
			return new EveryUpdate(updateTiming);
		}

		public static IUniTaskAsyncEnumerable<TProperty> EveryValueChanged<TTarget, TProperty>(TTarget target, Func<TTarget, TProperty> propertySelector, PlayerLoopTiming monitorTiming = PlayerLoopTiming.Update, IEqualityComparer<TProperty> equalityComparer = null) where TTarget : class
		{
			if (target is Object)
			{
				return new EveryValueChangedUnityObject<TTarget, TProperty>(target, propertySelector, equalityComparer ?? UnityEqualityComparer.GetDefault<TProperty>(), monitorTiming);
			}
			return new EveryValueChangedStandardObject<TTarget, TProperty>(target, propertySelector, equalityComparer ?? UnityEqualityComparer.GetDefault<TProperty>(), monitorTiming);
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> Timer(TimeSpan dueTime, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool ignoreTimeScale = false)
		{
			return new Timer(dueTime, null, updateTiming, ignoreTimeScale);
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> Timer(TimeSpan dueTime, TimeSpan period, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool ignoreTimeScale = false)
		{
			return new Timer(dueTime, new TimeSpan?(period), updateTiming, ignoreTimeScale);
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> Interval(TimeSpan period, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update, bool ignoreTimeScale = false)
		{
			return new Timer(period, new TimeSpan?(period), updateTiming, ignoreTimeScale);
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> TimerFrame(int dueTimeFrameCount, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update)
		{
			if (dueTimeFrameCount < 0)
			{
				throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. dueTimeFrameCount:" + dueTimeFrameCount.ToString());
			}
			return new TimerFrame(dueTimeFrameCount, null, updateTiming);
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> TimerFrame(int dueTimeFrameCount, int periodFrameCount, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update)
		{
			if (dueTimeFrameCount < 0)
			{
				throw new ArgumentOutOfRangeException("Delay does not allow minus delayFrameCount. dueTimeFrameCount:" + dueTimeFrameCount.ToString());
			}
			if (periodFrameCount < 0)
			{
				throw new ArgumentOutOfRangeException("Delay does not allow minus periodFrameCount. periodFrameCount:" + dueTimeFrameCount.ToString());
			}
			return new TimerFrame(dueTimeFrameCount, new int?(periodFrameCount), updateTiming);
		}

		public static IUniTaskAsyncEnumerable<AsyncUnit> IntervalFrame(int intervalFrameCount, PlayerLoopTiming updateTiming = PlayerLoopTiming.Update)
		{
			if (intervalFrameCount < 0)
			{
				throw new ArgumentOutOfRangeException("Delay does not allow minus intervalFrameCount. intervalFrameCount:" + intervalFrameCount.ToString());
			}
			return new TimerFrame(intervalFrameCount, new int?(intervalFrameCount), updateTiming);
		}

		public static IUniTaskAsyncEnumerable<TSource> Where<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, bool>>(predicate, "predicate");
			return new Where<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> Where<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, bool>>(predicate, "predicate");
			return new WhereInt<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> WhereAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, UniTask<bool>>>(predicate, "predicate");
			return new WhereAwait<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> WhereAwait<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, UniTask<bool>>>(predicate, "predicate");
			return new WhereIntAwait<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> WhereAwaitWithCancellation<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return new WhereAwaitWithCancellation<TSource>(source, predicate);
		}

		public static IUniTaskAsyncEnumerable<TSource> WhereAwaitWithCancellation<TSource>(this IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<bool>> predicate)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSource>>(source, "source");
			Error.ThrowArgumentNullException<Func<TSource, int, CancellationToken, UniTask<bool>>>(predicate, "predicate");
			return new WhereIntAwaitWithCancellation<TSource>(source, predicate);
		}

		[return: TupleElementNames(new string[]
		{
			"First",
			"Second"
		})]
		public static IUniTaskAsyncEnumerable<ValueTuple<TFirst, TSecond>> Zip<TFirst, TSecond>(this IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TFirst>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSecond>>(second, "second");
			return first.Zip(second, (TFirst x, TSecond y) => new ValueTuple<TFirst, TSecond>(x, y));
		}

		public static IUniTaskAsyncEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TFirst>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSecond>>(second, "second");
			Error.ThrowArgumentNullException<Func<TFirst, TSecond, TResult>>(resultSelector, "resultSelector");
			return new Zip<TFirst, TSecond, TResult>(first, second, resultSelector);
		}

		public static IUniTaskAsyncEnumerable<TResult> ZipAwait<TFirst, TSecond, TResult>(this IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, UniTask<TResult>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TFirst>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSecond>>(second, "second");
			Error.ThrowArgumentNullException<Func<TFirst, TSecond, UniTask<TResult>>>(selector, "selector");
			return new ZipAwait<TFirst, TSecond, TResult>(first, second, selector);
		}

		public static IUniTaskAsyncEnumerable<TResult> ZipAwaitWithCancellation<TFirst, TSecond, TResult>(this IUniTaskAsyncEnumerable<TFirst> first, IUniTaskAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, CancellationToken, UniTask<TResult>> selector)
		{
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TFirst>>(first, "first");
			Error.ThrowArgumentNullException<IUniTaskAsyncEnumerable<TSecond>>(second, "second");
			Error.ThrowArgumentNullException<Func<TFirst, TSecond, CancellationToken, UniTask<TResult>>>(selector, "selector");
			return new ZipAwaitWithCancellation<TFirst, TSecond, TResult>(first, second, selector);
		}
	}
}
