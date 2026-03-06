using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal static class OVRObjectPool
{
	public static T Get<T>() where T : class, new()
	{
		T orCreate = OVRObjectPool.Storage<T>.GetOrCreate();
		IList list = orCreate as IList;
		if (list != null)
		{
			list.Clear();
		}
		else
		{
			IDictionary dictionary = orCreate as IDictionary;
			if (dictionary != null)
			{
				dictionary.Clear();
			}
		}
		OVRObjectPool.IPoolObject poolObject = orCreate as OVRObjectPool.IPoolObject;
		if (poolObject != null)
		{
			poolObject.OnGet();
		}
		return orCreate;
	}

	public static List<T> List<T>()
	{
		return OVRObjectPool.Get<List<T>>();
	}

	public static List<T> List<T>(IEnumerable<T> source)
	{
		List<T> list = OVRObjectPool.Get<List<T>>();
		foreach (T item in source.ToNonAlloc<T>())
		{
			list.Add(item);
		}
		return list;
	}

	public static Dictionary<TKey, TValue> Dictionary<TKey, TValue>()
	{
		return OVRObjectPool.Get<Dictionary<TKey, TValue>>();
	}

	public static HashSet<T> HashSet<T>()
	{
		HashSet<T> hashSet = OVRObjectPool.Get<HashSet<T>>();
		hashSet.Clear();
		return hashSet;
	}

	public static Stack<T> Stack<T>()
	{
		Stack<T> stack = OVRObjectPool.Get<Stack<T>>();
		stack.Clear();
		return stack;
	}

	public static Queue<T> Queue<T>()
	{
		Queue<T> queue = OVRObjectPool.Get<Queue<T>>();
		queue.Clear();
		return queue;
	}

	public static void Return<T>(T obj) where T : class, new()
	{
		if (obj != null)
		{
			IList list = obj as IList;
			if (list == null)
			{
				IDictionary dictionary = obj as IDictionary;
				if (dictionary == null)
				{
					OVRObjectPool.IPoolObject poolObject = obj as OVRObjectPool.IPoolObject;
					if (poolObject != null)
					{
						poolObject.OnReturn();
					}
				}
				else
				{
					dictionary.Clear();
				}
			}
			else
			{
				list.Clear();
			}
			OVRObjectPool.Storage<T>.Add(obj);
			return;
		}
	}

	public static void Return<T>(HashSet<T> set)
	{
		if (set != null)
		{
			set.Clear();
		}
		OVRObjectPool.Return<HashSet<T>>(set);
	}

	public static void Return<T>(Stack<T> stack)
	{
		if (stack != null)
		{
			stack.Clear();
		}
		OVRObjectPool.Return<Stack<T>>(stack);
	}

	public static void Return<T>(Queue<T> queue)
	{
		if (queue != null)
		{
			queue.Clear();
		}
		OVRObjectPool.Return<Queue<T>>(queue);
	}

	public interface IPoolObject
	{
		void OnGet();

		void OnReturn();
	}

	private static class Storage<T> where T : class, new()
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remove(T item)
		{
			return OVRObjectPool.Storage<T>.s_hashSet.Remove(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Add(T item)
		{
			return OVRObjectPool.Storage<T>.s_hashSet.Add(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetOrCreate()
		{
			T result;
			using (HashSet<T>.Enumerator enumerator = OVRObjectPool.Storage<T>.s_hashSet.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					T t = enumerator.Current;
					OVRObjectPool.Storage<T>.Remove(t);
					result = t;
				}
				else
				{
					result = Activator.CreateInstance<T>();
				}
			}
			return result;
		}

		private static readonly HashSet<T> s_hashSet = new HashSet<T>();

		public static readonly Action Clear = delegate()
		{
			OVRObjectPool.Storage<T>.s_hashSet.Clear();
		};
	}

	public struct ListScope<T> : IDisposable
	{
		public ListScope(out List<T> list)
		{
			List<T> list2;
			list = (list2 = OVRObjectPool.List<T>());
			this._list = list2;
		}

		public ListScope(IEnumerable<T> source, out List<T> list)
		{
			List<T> list2;
			list = (list2 = OVRObjectPool.List<T>(source));
			this._list = list2;
		}

		public void Dispose()
		{
			OVRObjectPool.Return<List<T>>(this._list);
		}

		private List<T> _list;
	}

	public struct TaskScope<T> : IDisposable
	{
		public TaskScope(out List<OVRTask<T>> tasks, out List<T> results)
		{
			this._tasks = new OVRObjectPool.ListScope<OVRTask<T>>(ref tasks);
			this._results = new OVRObjectPool.ListScope<T>(ref results);
		}

		public void Dispose()
		{
			this._tasks.Dispose();
			this._results.Dispose();
		}

		private OVRObjectPool.ListScope<OVRTask<T>> _tasks;

		private OVRObjectPool.ListScope<T> _results;
	}

	public readonly struct DictionaryScope<TKey, TValue> : IDisposable
	{
		public DictionaryScope(out Dictionary<TKey, TValue> dictionary)
		{
			Dictionary<TKey, TValue> dictionary2;
			dictionary = (dictionary2 = OVRObjectPool.Dictionary<TKey, TValue>());
			this._dictionary = dictionary2;
		}

		public void Dispose()
		{
			OVRObjectPool.Return<Dictionary<TKey, TValue>>(this._dictionary);
		}

		private readonly Dictionary<TKey, TValue> _dictionary;
	}

	public readonly struct HashSetScope<T> : IDisposable
	{
		public HashSetScope(out HashSet<T> set)
		{
			HashSet<T> set2;
			set = (set2 = OVRObjectPool.HashSet<T>());
			this._set = set2;
		}

		public void Dispose()
		{
			OVRObjectPool.Return<T>(this._set);
		}

		private readonly HashSet<T> _set;
	}

	public readonly struct StackScope<T> : IDisposable
	{
		public StackScope(out Stack<T> stack)
		{
			Stack<T> stack2;
			stack = (stack2 = OVRObjectPool.Stack<T>());
			this._stack = stack2;
		}

		public void Dispose()
		{
			OVRObjectPool.Return<T>(this._stack);
		}

		private readonly Stack<T> _stack;
	}

	public readonly struct QueueScope<T> : IDisposable
	{
		public QueueScope(out Queue<T> queue)
		{
			Queue<T> queue2;
			queue = (queue2 = OVRObjectPool.Queue<T>());
			this._queue = queue2;
		}

		public void Dispose()
		{
			OVRObjectPool.Return<T>(this._queue);
		}

		private readonly Queue<T> _queue;
	}

	public readonly struct ItemScope<T> : IDisposable where T : class, new()
	{
		public ItemScope(out T item)
		{
			this._item = (item = OVRObjectPool.Get<T>());
		}

		public void Dispose()
		{
			OVRObjectPool.Return<T>(this._item);
		}

		private readonly T _item;
	}
}
