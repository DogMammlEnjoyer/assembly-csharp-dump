using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Localization.Metadata
{
	[Serializable]
	public abstract class SharedTableCollectionMetadata : IMetadata, ISerializationCallbackReceiver
	{
		public Dictionary<long, HashSet<string>> EntriesLookup { get; set; } = new Dictionary<long, HashSet<string>>();

		public bool IsEmpty
		{
			get
			{
				return this.EntriesLookup.Count == 0;
			}
		}

		public bool Contains(long keyId)
		{
			return this.EntriesLookup.ContainsKey(keyId);
		}

		public bool Contains(long keyId, string code)
		{
			HashSet<string> hashSet;
			return this.EntriesLookup.TryGetValue(keyId, out hashSet) && hashSet.Contains(code);
		}

		public void AddEntry(long keyId, string code)
		{
			HashSet<string> hashSet;
			this.EntriesLookup.TryGetValue(keyId, out hashSet);
			if (hashSet == null)
			{
				hashSet = new HashSet<string>();
				this.EntriesLookup[keyId] = hashSet;
			}
			hashSet.Add(code);
		}

		public void RemoveEntry(long keyId, string code)
		{
			HashSet<string> hashSet;
			if (this.EntriesLookup.TryGetValue(keyId, out hashSet))
			{
				hashSet.Remove(code);
				if (hashSet.Count == 0)
				{
					this.EntriesLookup.Remove(keyId);
				}
			}
		}

		public virtual void OnBeforeSerialize()
		{
			this.m_Entries.Clear();
			foreach (KeyValuePair<long, HashSet<string>> keyValuePair in this.EntriesLookup)
			{
				this.m_Entries.Add(new SharedTableCollectionMetadata.Item
				{
					KeyId = keyValuePair.Key,
					Tables = keyValuePair.Value.ToList<string>()
				});
			}
		}

		public virtual void OnAfterDeserialize()
		{
			this.EntriesLookup = new Dictionary<long, HashSet<string>>();
			foreach (SharedTableCollectionMetadata.Item item in this.m_Entries)
			{
				this.EntriesLookup[item.KeyId] = new HashSet<string>(item.Tables);
			}
		}

		[SerializeField]
		[HideInInspector]
		private List<SharedTableCollectionMetadata.Item> m_Entries = new List<SharedTableCollectionMetadata.Item>();

		[Serializable]
		private class Item
		{
			public long KeyId
			{
				get
				{
					return this.m_KeyId;
				}
				set
				{
					this.m_KeyId = value;
				}
			}

			public List<string> Tables
			{
				get
				{
					return this.m_TableCodes;
				}
				set
				{
					this.m_TableCodes = value;
				}
			}

			[SerializeField]
			private long m_KeyId;

			[SerializeField]
			private List<string> m_TableCodes = new List<string>();
		}
	}
}
