using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.StructWrapping
{
	public static class StructWrapperUtility
	{
		public static Type GetWrappedType(this object obj)
		{
			StructWrapper structWrapper = obj as StructWrapper;
			bool flag = structWrapper == null;
			Type result;
			if (flag)
			{
				result = obj.GetType();
			}
			else
			{
				result = structWrapper.ttype;
			}
			return result;
		}

		public static StructWrapper<T> Wrap<T>(this T value, bool persistant)
		{
			StructWrapper<T> structWrapper = StructWrapper<T>.staticPool.Acquire(value);
			if (persistant)
			{
				structWrapper.DisconnectFromPool();
			}
			return structWrapper;
		}

		public static StructWrapper<T> Wrap<T>(this T value)
		{
			return StructWrapper<T>.staticPool.Acquire(value);
		}

		public static StructWrapper<byte> Wrap(this byte value)
		{
			return StructWrapperPools.mappedByteWrappers[(int)value];
		}

		public static StructWrapper<bool> Wrap(this bool value)
		{
			return StructWrapperPools.mappedBoolWrappers[value ? 1 : 0];
		}

		public static bool IsType<T>(this object obj)
		{
			bool flag = obj is T;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = obj is StructWrapper<T>;
				result = flag2;
			}
			return result;
		}

		public static T DisconnectPooling<T>(this T table) where T : IEnumerable<object>
		{
			foreach (object obj in table)
			{
				StructWrapper structWrapper = obj as StructWrapper;
				bool flag = structWrapper == null;
				if (!flag)
				{
					structWrapper.DisconnectFromPool();
				}
			}
			return table;
		}

		public static List<object> ReleaseAllWrappers(this List<object> collection)
		{
			foreach (object obj in collection)
			{
				StructWrapper structWrapper = obj as StructWrapper;
				bool flag = structWrapper == null;
				if (!flag)
				{
					structWrapper.Dispose();
				}
			}
			return collection;
		}

		public static object[] ReleaseAllWrappers(this object[] collection)
		{
			foreach (object obj in collection)
			{
				StructWrapper structWrapper = obj as StructWrapper;
				bool flag = structWrapper == null;
				if (!flag)
				{
					structWrapper.Dispose();
				}
			}
			return collection;
		}

		public static Hashtable ReleaseAllWrappers(this Hashtable table)
		{
			foreach (object obj in table.Values)
			{
				StructWrapper structWrapper = obj as StructWrapper;
				bool flag = structWrapper == null;
				if (!flag)
				{
					structWrapper.Dispose();
				}
			}
			return table;
		}

		public static void BoxAll(this Hashtable table, bool recursive = false)
		{
			foreach (object obj in table.Values)
			{
				if (recursive)
				{
					Hashtable hashtable = obj as Hashtable;
					bool flag = hashtable != null;
					if (flag)
					{
						hashtable.BoxAll(false);
					}
				}
				StructWrapper structWrapper = obj as StructWrapper;
				bool flag2 = structWrapper == null;
				if (!flag2)
				{
					structWrapper.Box();
				}
			}
		}

		public static T Unwrap<T>(this object obj)
		{
			StructWrapper<T> structWrapper = obj as StructWrapper<T>;
			bool flag = structWrapper == null;
			T result;
			if (flag)
			{
				result = (T)((object)obj);
			}
			else
			{
				T value = structWrapper.value;
				bool flag2 = (structWrapper.pooling & Pooling.ReleaseOnUnwrap) == Pooling.ReleaseOnUnwrap;
				if (flag2)
				{
					structWrapper.Dispose();
				}
				result = structWrapper.value;
			}
			return result;
		}

		public static T Get<T>(this object obj)
		{
			StructWrapper<T> structWrapper = obj as StructWrapper<T>;
			bool flag = structWrapper == null;
			T result;
			if (flag)
			{
				result = (T)((object)obj);
			}
			else
			{
				T value = structWrapper.value;
				result = value;
			}
			return result;
		}

		public static T Unwrap<T>(this Hashtable table, object key)
		{
			object obj = table[key];
			return obj.Unwrap<T>();
		}

		public static bool TryUnwrapValue<T>(this Hashtable table, byte key, out T value) where T : new()
		{
			object obj;
			bool flag = table.TryGetValue(key, out obj);
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				value = default(T);
				result = false;
			}
			else
			{
				value = obj.Unwrap<T>();
				result = true;
			}
			return result;
		}

		public static bool TryGetValue<T>(this Hashtable table, byte key, out T value) where T : new()
		{
			object obj;
			bool flag = table.TryGetValue(key, out obj);
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				value = default(T);
				result = false;
			}
			else
			{
				value = obj.Get<T>();
				result = true;
			}
			return result;
		}

		public static bool TryGetValue<T>(this Hashtable table, object key, out T value) where T : new()
		{
			object obj;
			bool flag = table.TryGetValue(key, out obj);
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				value = default(T);
				result = false;
			}
			else
			{
				value = obj.Get<T>();
				result = true;
			}
			return result;
		}

		public static bool TryUnwrapValue<T>(this Hashtable table, object key, out T value) where T : new()
		{
			object obj;
			bool flag = table.TryGetValue(key, out obj);
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				value = default(T);
				result = false;
			}
			else
			{
				value = obj.Unwrap<T>();
				result = true;
			}
			return result;
		}

		public static T Unwrap<T>(this Hashtable table, byte key)
		{
			object obj = table[key];
			return obj.Unwrap<T>();
		}

		public static T Get<T>(this Hashtable table, byte key)
		{
			object obj = table[key];
			return obj.Get<T>();
		}
	}
}
