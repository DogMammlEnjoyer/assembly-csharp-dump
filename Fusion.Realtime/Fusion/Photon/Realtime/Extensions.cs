using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal static class Extensions
	{
		public static void Merge(this IDictionary target, IDictionary addHash)
		{
			bool flag = addHash == null || target.Equals(addHash);
			if (!flag)
			{
				foreach (object key in addHash.Keys)
				{
					target[key] = addHash[key];
				}
			}
		}

		public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
		{
			bool flag = addHash == null || target.Equals(addHash);
			if (!flag)
			{
				foreach (object obj in addHash.Keys)
				{
					bool flag2 = obj is string;
					if (flag2)
					{
						target[obj] = addHash[obj];
					}
				}
			}
		}

		public static string ToStringFull(this IDictionary origin)
		{
			return SupportClass.DictionaryToString(origin, false);
		}

		public static string ToStringFull<T>(this List<T> data)
		{
			bool flag = data == null;
			string result;
			if (flag)
			{
				result = "null";
			}
			else
			{
				string[] array = new string[data.Count];
				for (int i = 0; i < data.Count; i++)
				{
					object obj = data[i];
					array[i] = ((obj != null) ? obj.ToString() : "null");
				}
				result = string.Join(", ", array);
			}
			return result;
		}

		public static string ToStringFull(this object[] data)
		{
			bool flag = data == null;
			string result;
			if (flag)
			{
				result = "null";
			}
			else
			{
				string[] array = new string[data.Length];
				for (int i = 0; i < data.Length; i++)
				{
					object obj = data[i];
					array[i] = ((obj != null) ? obj.ToString() : "null");
				}
				result = string.Join(", ", array);
			}
			return result;
		}

		public static Hashtable StripToStringKeys(this IDictionary original)
		{
			Hashtable hashtable = new Hashtable();
			bool flag = original != null;
			if (flag)
			{
				foreach (object obj in original.Keys)
				{
					bool flag2 = obj is string;
					if (flag2)
					{
						hashtable[obj] = original[obj];
					}
				}
			}
			return hashtable;
		}

		public static Hashtable StripToStringKeys(this Hashtable original)
		{
			Hashtable hashtable = new Hashtable();
			bool flag = original != null;
			if (flag)
			{
				foreach (DictionaryEntry dictionaryEntry in original)
				{
					bool flag2 = dictionaryEntry.Key is string;
					if (flag2)
					{
						hashtable[dictionaryEntry.Key] = original[dictionaryEntry.Key];
					}
				}
			}
			return hashtable;
		}

		public static void StripKeysWithNullValues(this IDictionary original)
		{
			List<object> obj = Extensions.keysWithNullValue;
			lock (obj)
			{
				Extensions.keysWithNullValue.Clear();
				foreach (object obj2 in original)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
					bool flag2 = dictionaryEntry.Value == null;
					if (flag2)
					{
						Extensions.keysWithNullValue.Add(dictionaryEntry.Key);
					}
				}
				for (int i = 0; i < Extensions.keysWithNullValue.Count; i++)
				{
					object key = Extensions.keysWithNullValue[i];
					original.Remove(key);
				}
			}
		}

		public static void StripKeysWithNullValues(this Hashtable original)
		{
			List<object> obj = Extensions.keysWithNullValue;
			lock (obj)
			{
				Extensions.keysWithNullValue.Clear();
				foreach (DictionaryEntry dictionaryEntry in original)
				{
					bool flag2 = dictionaryEntry.Value == null;
					if (flag2)
					{
						Extensions.keysWithNullValue.Add(dictionaryEntry.Key);
					}
				}
				for (int i = 0; i < Extensions.keysWithNullValue.Count; i++)
				{
					object key = Extensions.keysWithNullValue[i];
					original.Remove(key);
				}
			}
		}

		public static bool Contains(this int[] target, int nr)
		{
			bool flag = target == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < target.Length; i++)
				{
					bool flag2 = target[i] == nr;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private static readonly List<object> keysWithNullValue = new List<object>();
	}
}
