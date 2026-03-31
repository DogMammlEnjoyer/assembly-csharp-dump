using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class ToLookup
	{
		internal static UniTask<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			ToLookup.<ToLookupAsync>d__0<TSource, TKey> <ToLookupAsync>d__;
			<ToLookupAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ILookup<TKey, TSource>>.Create();
			<ToLookupAsync>d__.source = source;
			<ToLookupAsync>d__.keySelector = keySelector;
			<ToLookupAsync>d__.comparer = comparer;
			<ToLookupAsync>d__.cancellationToken = cancellationToken;
			<ToLookupAsync>d__.<>1__state = -1;
			<ToLookupAsync>d__.<>t__builder.Start<ToLookup.<ToLookupAsync>d__0<TSource, TKey>>(ref <ToLookupAsync>d__);
			return <ToLookupAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			ToLookup.<ToLookupAsync>d__1<TSource, TKey, TElement> <ToLookupAsync>d__;
			<ToLookupAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ILookup<TKey, TElement>>.Create();
			<ToLookupAsync>d__.source = source;
			<ToLookupAsync>d__.keySelector = keySelector;
			<ToLookupAsync>d__.elementSelector = elementSelector;
			<ToLookupAsync>d__.comparer = comparer;
			<ToLookupAsync>d__.cancellationToken = cancellationToken;
			<ToLookupAsync>d__.<>1__state = -1;
			<ToLookupAsync>d__.<>t__builder.Start<ToLookup.<ToLookupAsync>d__1<TSource, TKey, TElement>>(ref <ToLookupAsync>d__);
			return <ToLookupAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<ILookup<TKey, TSource>> ToLookupAwaitAsync<TSource, TKey>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			ToLookup.<ToLookupAwaitAsync>d__2<TSource, TKey> <ToLookupAwaitAsync>d__;
			<ToLookupAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ILookup<TKey, TSource>>.Create();
			<ToLookupAwaitAsync>d__.source = source;
			<ToLookupAwaitAsync>d__.keySelector = keySelector;
			<ToLookupAwaitAsync>d__.comparer = comparer;
			<ToLookupAwaitAsync>d__.cancellationToken = cancellationToken;
			<ToLookupAwaitAsync>d__.<>1__state = -1;
			<ToLookupAwaitAsync>d__.<>t__builder.Start<ToLookup.<ToLookupAwaitAsync>d__2<TSource, TKey>>(ref <ToLookupAwaitAsync>d__);
			return <ToLookupAwaitAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<ILookup<TKey, TElement>> ToLookupAwaitAsync<TSource, TKey, TElement>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			ToLookup.<ToLookupAwaitAsync>d__3<TSource, TKey, TElement> <ToLookupAwaitAsync>d__;
			<ToLookupAwaitAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ILookup<TKey, TElement>>.Create();
			<ToLookupAwaitAsync>d__.source = source;
			<ToLookupAwaitAsync>d__.keySelector = keySelector;
			<ToLookupAwaitAsync>d__.elementSelector = elementSelector;
			<ToLookupAwaitAsync>d__.comparer = comparer;
			<ToLookupAwaitAsync>d__.cancellationToken = cancellationToken;
			<ToLookupAwaitAsync>d__.<>1__state = -1;
			<ToLookupAwaitAsync>d__.<>t__builder.Start<ToLookup.<ToLookupAwaitAsync>d__3<TSource, TKey, TElement>>(ref <ToLookupAwaitAsync>d__);
			return <ToLookupAwaitAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<ILookup<TKey, TSource>> ToLookupAwaitWithCancellationAsync<TSource, TKey>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			ToLookup.<ToLookupAwaitWithCancellationAsync>d__4<TSource, TKey> <ToLookupAwaitWithCancellationAsync>d__;
			<ToLookupAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ILookup<TKey, TSource>>.Create();
			<ToLookupAwaitWithCancellationAsync>d__.source = source;
			<ToLookupAwaitWithCancellationAsync>d__.keySelector = keySelector;
			<ToLookupAwaitWithCancellationAsync>d__.comparer = comparer;
			<ToLookupAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<ToLookupAwaitWithCancellationAsync>d__.<>1__state = -1;
			<ToLookupAwaitWithCancellationAsync>d__.<>t__builder.Start<ToLookup.<ToLookupAwaitWithCancellationAsync>d__4<TSource, TKey>>(ref <ToLookupAwaitWithCancellationAsync>d__);
			return <ToLookupAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		internal static UniTask<ILookup<TKey, TElement>> ToLookupAwaitWithCancellationAsync<TSource, TKey, TElement>(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			ToLookup.<ToLookupAwaitWithCancellationAsync>d__5<TSource, TKey, TElement> <ToLookupAwaitWithCancellationAsync>d__;
			<ToLookupAwaitWithCancellationAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ILookup<TKey, TElement>>.Create();
			<ToLookupAwaitWithCancellationAsync>d__.source = source;
			<ToLookupAwaitWithCancellationAsync>d__.keySelector = keySelector;
			<ToLookupAwaitWithCancellationAsync>d__.elementSelector = elementSelector;
			<ToLookupAwaitWithCancellationAsync>d__.comparer = comparer;
			<ToLookupAwaitWithCancellationAsync>d__.cancellationToken = cancellationToken;
			<ToLookupAwaitWithCancellationAsync>d__.<>1__state = -1;
			<ToLookupAwaitWithCancellationAsync>d__.<>t__builder.Start<ToLookup.<ToLookupAwaitWithCancellationAsync>d__5<TSource, TKey, TElement>>(ref <ToLookupAwaitWithCancellationAsync>d__);
			return <ToLookupAwaitWithCancellationAsync>d__.<>t__builder.Task;
		}

		private class Lookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable<IGrouping<TKey, TElement>>, IEnumerable
		{
			private Lookup(Dictionary<TKey, ToLookup.Grouping<TKey, TElement>> dict)
			{
				this.dict = dict;
			}

			public static ToLookup.Lookup<TKey, TElement> CreateEmpty()
			{
				return ToLookup.Lookup<TKey, TElement>.empty;
			}

			public static ToLookup.Lookup<TKey, TElement> Create(ArraySegment<TElement> source, Func<TElement, TKey> keySelector, IEqualityComparer<TKey> comparer)
			{
				Dictionary<TKey, ToLookup.Grouping<TKey, TElement>> dictionary = new Dictionary<TKey, ToLookup.Grouping<TKey, TElement>>(comparer);
				TElement[] array = source.Array;
				int count = source.Count;
				for (int i = source.Offset; i < count; i++)
				{
					TKey key = keySelector(array[i]);
					ToLookup.Grouping<TKey, TElement> grouping;
					if (!dictionary.TryGetValue(key, out grouping))
					{
						grouping = new ToLookup.Grouping<TKey, TElement>(key);
						dictionary[key] = grouping;
					}
					grouping.Add(array[i]);
				}
				return new ToLookup.Lookup<TKey, TElement>(dictionary);
			}

			public static ToLookup.Lookup<TKey, TElement> Create<TSource>(ArraySegment<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
			{
				Dictionary<TKey, ToLookup.Grouping<TKey, TElement>> dictionary = new Dictionary<TKey, ToLookup.Grouping<TKey, TElement>>(comparer);
				TSource[] array = source.Array;
				int count = source.Count;
				for (int i = source.Offset; i < count; i++)
				{
					TKey key = keySelector(array[i]);
					TElement value = elementSelector(array[i]);
					ToLookup.Grouping<TKey, TElement> grouping;
					if (!dictionary.TryGetValue(key, out grouping))
					{
						grouping = new ToLookup.Grouping<TKey, TElement>(key);
						dictionary[key] = grouping;
					}
					grouping.Add(value);
				}
				return new ToLookup.Lookup<TKey, TElement>(dictionary);
			}

			public static UniTask<ToLookup.Lookup<TKey, TElement>> CreateAsync(ArraySegment<TElement> source, Func<TElement, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer)
			{
				ToLookup.Lookup<TKey, TElement>.<CreateAsync>d__6 <CreateAsync>d__;
				<CreateAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ToLookup.Lookup<TKey, TElement>>.Create();
				<CreateAsync>d__.source = source;
				<CreateAsync>d__.keySelector = keySelector;
				<CreateAsync>d__.comparer = comparer;
				<CreateAsync>d__.<>1__state = -1;
				<CreateAsync>d__.<>t__builder.Start<ToLookup.Lookup<TKey, TElement>.<CreateAsync>d__6>(ref <CreateAsync>d__);
				return <CreateAsync>d__.<>t__builder.Task;
			}

			public static UniTask<ToLookup.Lookup<TKey, TElement>> CreateAsync<TSource>(ArraySegment<TSource> source, Func<TSource, UniTask<TKey>> keySelector, Func<TSource, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer)
			{
				ToLookup.Lookup<TKey, TElement>.<CreateAsync>d__7<TSource> <CreateAsync>d__;
				<CreateAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ToLookup.Lookup<TKey, TElement>>.Create();
				<CreateAsync>d__.source = source;
				<CreateAsync>d__.keySelector = keySelector;
				<CreateAsync>d__.elementSelector = elementSelector;
				<CreateAsync>d__.comparer = comparer;
				<CreateAsync>d__.<>1__state = -1;
				<CreateAsync>d__.<>t__builder.Start<ToLookup.Lookup<TKey, TElement>.<CreateAsync>d__7<TSource>>(ref <CreateAsync>d__);
				return <CreateAsync>d__.<>t__builder.Task;
			}

			public static UniTask<ToLookup.Lookup<TKey, TElement>> CreateAsync(ArraySegment<TElement> source, Func<TElement, CancellationToken, UniTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			{
				ToLookup.Lookup<TKey, TElement>.<CreateAsync>d__8 <CreateAsync>d__;
				<CreateAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ToLookup.Lookup<TKey, TElement>>.Create();
				<CreateAsync>d__.source = source;
				<CreateAsync>d__.keySelector = keySelector;
				<CreateAsync>d__.comparer = comparer;
				<CreateAsync>d__.cancellationToken = cancellationToken;
				<CreateAsync>d__.<>1__state = -1;
				<CreateAsync>d__.<>t__builder.Start<ToLookup.Lookup<TKey, TElement>.<CreateAsync>d__8>(ref <CreateAsync>d__);
				return <CreateAsync>d__.<>t__builder.Task;
			}

			public static UniTask<ToLookup.Lookup<TKey, TElement>> CreateAsync<TSource>(ArraySegment<TSource> source, Func<TSource, CancellationToken, UniTask<TKey>> keySelector, Func<TSource, CancellationToken, UniTask<TElement>> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			{
				ToLookup.Lookup<TKey, TElement>.<CreateAsync>d__9<TSource> <CreateAsync>d__;
				<CreateAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<ToLookup.Lookup<TKey, TElement>>.Create();
				<CreateAsync>d__.source = source;
				<CreateAsync>d__.keySelector = keySelector;
				<CreateAsync>d__.elementSelector = elementSelector;
				<CreateAsync>d__.comparer = comparer;
				<CreateAsync>d__.cancellationToken = cancellationToken;
				<CreateAsync>d__.<>1__state = -1;
				<CreateAsync>d__.<>t__builder.Start<ToLookup.Lookup<TKey, TElement>.<CreateAsync>d__9<TSource>>(ref <CreateAsync>d__);
				return <CreateAsync>d__.<>t__builder.Task;
			}

			public IEnumerable<TElement> this[TKey key]
			{
				get
				{
					ToLookup.Grouping<TKey, TElement> result;
					if (!this.dict.TryGetValue(key, out result))
					{
						return Enumerable.Empty<TElement>();
					}
					return result;
				}
			}

			public int Count
			{
				get
				{
					return this.dict.Count;
				}
			}

			public bool Contains(TKey key)
			{
				return this.dict.ContainsKey(key);
			}

			public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
			{
				return this.dict.Values.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.dict.Values.GetEnumerator();
			}

			private static readonly ToLookup.Lookup<TKey, TElement> empty = new ToLookup.Lookup<TKey, TElement>(new Dictionary<TKey, ToLookup.Grouping<TKey, TElement>>());

			private readonly Dictionary<TKey, ToLookup.Grouping<TKey, TElement>> dict;
		}

		private class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IEnumerable<TElement>, IEnumerable
		{
			public TKey Key { get; private set; }

			public Grouping(TKey key)
			{
				this.Key = key;
				this.elements = new List<TElement>();
			}

			public void Add(TElement value)
			{
				this.elements.Add(value);
			}

			public IEnumerator<TElement> GetEnumerator()
			{
				return this.elements.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.elements.GetEnumerator();
			}

			public IUniTaskAsyncEnumerator<TElement> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
			{
				return this.ToUniTaskAsyncEnumerable<TElement>().GetAsyncEnumerator(cancellationToken);
			}

			public override string ToString()
			{
				string str = "Key: ";
				TKey key = this.Key;
				return str + ((key != null) ? key.ToString() : null) + ", Count: " + this.elements.Count.ToString();
			}

			private readonly List<TElement> elements;
		}
	}
}
