using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Photon.Realtime
{
	public static class Extensions
	{
		public static void Merge(this IDictionary target, IDictionary addHash)
		{
			if (addHash == null || target.Equals(addHash))
			{
				return;
			}
			foreach (object key in addHash.Keys)
			{
				target[key] = addHash[key];
			}
		}

		public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
		{
			if (addHash == null || target.Equals(addHash))
			{
				return;
			}
			foreach (object obj in addHash.Keys)
			{
				if (obj is string)
				{
					target[obj] = addHash[obj];
				}
			}
		}

		public static string ToStringFull(this IDictionary origin)
		{
			return SupportClass.DictionaryToString(origin, false);
		}

		public static string ToStringFull<T>(this List<T> data)
		{
			if (data == null)
			{
				return "null";
			}
			string[] array = new string[data.Count];
			for (int i = 0; i < data.Count; i++)
			{
				object obj = data[i];
				array[i] = ((obj != null) ? obj.ToString() : "null");
			}
			return string.Join(", ", array);
		}

		public static string ToStringFull(this object[] data)
		{
			if (data == null)
			{
				return "null";
			}
			string[] array = new string[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				object obj = data[i];
				array[i] = ((obj != null) ? obj.ToString() : "null");
			}
			return string.Join(", ", array);
		}

		public static Hashtable StripToStringKeys(this IDictionary original)
		{
			Hashtable hashtable = new Hashtable();
			if (original != null)
			{
				foreach (object obj in original.Keys)
				{
					if (obj is string)
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
			if (original != null)
			{
				foreach (DictionaryEntry dictionaryEntry in original)
				{
					if (dictionaryEntry.Key is string)
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
					if (dictionaryEntry.Value == null)
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
					if (dictionaryEntry.Value == null)
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
			if (target == null)
			{
				return false;
			}
			for (int i = 0; i < target.Length; i++)
			{
				if (target[i] == nr)
				{
					return true;
				}
			}
			return false;
		}

		private static readonly List<object> keysWithNullValue = new List<object>();
	}
}
