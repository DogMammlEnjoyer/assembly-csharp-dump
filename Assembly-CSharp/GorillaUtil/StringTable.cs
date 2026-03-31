using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaUtil
{
	[CreateAssetMenu(fileName = "StringTable", menuName = "Scriptable Objects/StringTable")]
	public class StringTable : ScriptableObject
	{
		public bool ContainsKey(string key)
		{
			if (this.dict == null)
			{
				this.dict = new Dictionary<string, string>();
				for (int i = 0; i < this.entries.Length; i++)
				{
					this.dict.Add(this.entries[i].Key, this.entries[i].Value);
				}
			}
			return this.dict.ContainsKey(key);
		}

		public string FetchValue(string key)
		{
			if (this.ContainsKey(key))
			{
				return this.dict[key];
			}
			return null;
		}

		[SerializeField]
		private StringTable.StringPair[] entries;

		private Dictionary<string, string> dict;

		[Serializable]
		private struct StringPair
		{
			public string Key;

			public string Value;
		}
	}
}
