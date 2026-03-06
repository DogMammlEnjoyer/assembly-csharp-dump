using System;
using System.Collections.Generic;
using UnityEngine.Localization.Metadata;
using UnityEngine.Serialization;

namespace UnityEngine.Localization.Tables
{
	public class SharedTableData : ScriptableObject, ISerializationCallbackReceiver
	{
		public List<SharedTableData.SharedTableEntry> Entries
		{
			get
			{
				return this.m_Entries;
			}
			set
			{
				this.m_Entries = value;
				this.m_IdDictionary.Clear();
				this.m_KeyDictionary.Clear();
			}
		}

		public void Clear()
		{
			this.m_Entries.Clear();
			this.m_IdDictionary.Clear();
			this.m_KeyDictionary.Clear();
		}

		public string TableCollectionName
		{
			get
			{
				return this.m_TableCollectionName;
			}
			set
			{
				this.m_TableCollectionName = value;
			}
		}

		public Guid TableCollectionNameGuid
		{
			get
			{
				return this.m_TableCollectionNameGuid;
			}
			internal set
			{
				this.m_TableCollectionNameGuid = value;
			}
		}

		public MetadataCollection Metadata
		{
			get
			{
				return this.m_Metadata;
			}
			set
			{
				this.m_Metadata = value;
			}
		}

		public IKeyGenerator KeyGenerator
		{
			get
			{
				return this.m_KeyGenerator;
			}
			set
			{
				this.m_KeyGenerator = value;
			}
		}

		public string GetKey(long id)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = this.FindWithId(id);
			if (sharedTableEntry == null)
			{
				return null;
			}
			return sharedTableEntry.Key;
		}

		public long GetId(string key)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = this.FindWithKey(key);
			if (sharedTableEntry == null)
			{
				return 0L;
			}
			return sharedTableEntry.Id;
		}

		public long GetId(string key, bool addNewKey)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = this.FindWithKey(key);
			long result = 0L;
			if (sharedTableEntry != null)
			{
				result = sharedTableEntry.Id;
			}
			else if (addNewKey)
			{
				result = this.AddKeyInternal(key).Id;
			}
			return result;
		}

		public SharedTableData.SharedTableEntry GetEntryFromReference(TableEntryReference tableEntryReference)
		{
			if (tableEntryReference.ReferenceType == TableEntryReference.Type.Name)
			{
				return this.GetEntry(tableEntryReference.Key);
			}
			return this.GetEntry(tableEntryReference.KeyId);
		}

		public SharedTableData.SharedTableEntry GetEntry(long id)
		{
			return this.FindWithId(id);
		}

		public SharedTableData.SharedTableEntry GetEntry(string key)
		{
			return this.FindWithKey(key);
		}

		public bool Contains(long id)
		{
			return this.FindWithId(id) != null;
		}

		public bool Contains(string key)
		{
			return this.FindWithKey(key) != null;
		}

		public SharedTableData.SharedTableEntry AddKey(string key, long id)
		{
			if (this.Contains(id))
			{
				return null;
			}
			return this.AddKeyInternal(key, id);
		}

		public SharedTableData.SharedTableEntry AddKey(string key = null)
		{
			string text = string.IsNullOrEmpty(key) ? "New Entry" : key;
			SharedTableData.SharedTableEntry sharedTableEntry = null;
			int num = 1;
			string key2 = text;
			while (sharedTableEntry == null)
			{
				if (this.Contains(key2))
				{
					key2 = string.Format("{0} {1}", text, num++);
				}
				else
				{
					sharedTableEntry = this.AddKeyInternal(key2);
				}
			}
			return sharedTableEntry;
		}

		public void RemoveKey(long id)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = this.FindWithId(id);
			if (sharedTableEntry != null)
			{
				this.RemoveKeyInternal(sharedTableEntry);
			}
		}

		public void RemoveKey(string key)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = this.FindWithKey(key);
			if (sharedTableEntry != null)
			{
				this.RemoveKeyInternal(sharedTableEntry);
			}
		}

		public void RenameKey(long id, string newValue)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = this.FindWithId(id);
			if (sharedTableEntry != null)
			{
				this.RenameKeyInternal(sharedTableEntry, newValue);
			}
		}

		public void RenameKey(string oldValue, string newValue)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = this.FindWithKey(oldValue);
			if (sharedTableEntry != null)
			{
				this.RenameKeyInternal(sharedTableEntry, newValue);
			}
		}

		public bool RemapId(long currentId, long newId)
		{
			if (this.FindWithId(newId) != null)
			{
				return false;
			}
			SharedTableData.SharedTableEntry sharedTableEntry = this.FindWithId(currentId);
			if (sharedTableEntry == null)
			{
				return false;
			}
			sharedTableEntry.Id = newId;
			this.m_IdDictionary.Remove(currentId);
			this.m_IdDictionary[newId] = sharedTableEntry;
			return true;
		}

		[Obsolete("FindSimilarKey will be removed in the future, please use Unity Search. See TableEntrySearchData class for further details.")]
		public SharedTableData.SharedTableEntry FindSimilarKey(string text, out int distance)
		{
			SharedTableData.SharedTableEntry result = null;
			distance = int.MaxValue;
			foreach (SharedTableData.SharedTableEntry sharedTableEntry in this.Entries)
			{
				int num = SharedTableData.ComputeLevenshteinDistance(text.ToLower(), sharedTableEntry.Key.ToLower());
				if (num < distance)
				{
					result = sharedTableEntry;
					distance = num;
				}
			}
			return result;
		}

		private static int ComputeLevenshteinDistance(string a, string b)
		{
			int length = a.Length;
			int length2 = b.Length;
			int[,] array = new int[length + 1, length2 + 1];
			if (length == 0)
			{
				return length2;
			}
			if (length2 == 0)
			{
				return length;
			}
			int i = 0;
			while (i <= length)
			{
				array[i, 0] = i++;
			}
			int j = 0;
			while (j <= length2)
			{
				array[0, j] = j++;
			}
			for (int k = 1; k <= length; k++)
			{
				for (int l = 1; l <= length2; l++)
				{
					int num = (b[l - 1] == a[k - 1]) ? 0 : 1;
					array[k, l] = Mathf.Min(Mathf.Min(array[k - 1, l] + 1, array[k, l - 1] + 1), array[k - 1, l - 1] + num);
				}
			}
			return array[length, length2];
		}

		private SharedTableData.SharedTableEntry AddKeyInternal(string key)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = new SharedTableData.SharedTableEntry
			{
				Id = this.m_KeyGenerator.GetNextKey(),
				Key = key
			};
			while (this.FindWithId(sharedTableEntry.Id) != null)
			{
				sharedTableEntry.Id = this.m_KeyGenerator.GetNextKey();
			}
			this.Entries.Add(sharedTableEntry);
			if (this.m_IdDictionary.Count > 0)
			{
				this.m_IdDictionary[sharedTableEntry.Id] = sharedTableEntry;
			}
			if (this.m_KeyDictionary.Count > 0)
			{
				this.m_KeyDictionary[key] = sharedTableEntry;
			}
			return sharedTableEntry;
		}

		private SharedTableData.SharedTableEntry AddKeyInternal(string key, long id)
		{
			SharedTableData.SharedTableEntry sharedTableEntry = new SharedTableData.SharedTableEntry
			{
				Id = id,
				Key = key
			};
			this.Entries.Add(sharedTableEntry);
			if (this.m_IdDictionary.Count > 0)
			{
				this.m_IdDictionary[sharedTableEntry.Id] = sharedTableEntry;
			}
			if (this.m_KeyDictionary.Count > 0)
			{
				this.m_KeyDictionary[key] = sharedTableEntry;
			}
			return sharedTableEntry;
		}

		private void RenameKeyInternal(SharedTableData.SharedTableEntry entry, string newValue)
		{
			if (this.m_KeyDictionary.Count > 0)
			{
				this.m_KeyDictionary.Remove(entry.Key);
				this.m_KeyDictionary[newValue] = entry;
			}
			entry.Key = newValue;
		}

		private void RemoveKeyInternal(SharedTableData.SharedTableEntry entry)
		{
			if (this.m_KeyDictionary.Count > 0)
			{
				this.m_KeyDictionary.Remove(entry.Key);
			}
			if (this.m_IdDictionary.Count > 0)
			{
				this.m_IdDictionary.Remove(entry.Id);
			}
			this.Entries.Remove(entry);
		}

		private SharedTableData.SharedTableEntry FindWithId(long id)
		{
			if (id == 0L)
			{
				return null;
			}
			if (this.m_IdDictionary.Count == 0)
			{
				foreach (SharedTableData.SharedTableEntry sharedTableEntry in this.m_Entries)
				{
					this.m_IdDictionary[sharedTableEntry.Id] = sharedTableEntry;
				}
			}
			SharedTableData.SharedTableEntry result;
			this.m_IdDictionary.TryGetValue(id, out result);
			return result;
		}

		private SharedTableData.SharedTableEntry FindWithKey(string key)
		{
			if (this.m_KeyDictionary.Count == 0)
			{
				foreach (SharedTableData.SharedTableEntry sharedTableEntry in this.m_Entries)
				{
					this.m_KeyDictionary[sharedTableEntry.Key] = sharedTableEntry;
				}
			}
			SharedTableData.SharedTableEntry result;
			this.m_KeyDictionary.TryGetValue(key, out result);
			return result;
		}

		public override string ToString()
		{
			return this.TableCollectionName + "(Shared Table Data)";
		}

		public void OnBeforeSerialize()
		{
			this.m_TableCollectionNameGuidString = TableReference.StringFromGuid(this.m_TableCollectionNameGuid);
		}

		public void OnAfterDeserialize()
		{
			this.m_IdDictionary.Clear();
			this.m_KeyDictionary.Clear();
			this.m_TableCollectionNameGuid = (string.IsNullOrEmpty(this.m_TableCollectionNameGuidString) ? Guid.Empty : Guid.Parse(this.m_TableCollectionNameGuidString));
		}

		public const long EmptyId = 0L;

		internal const string NewEntryKey = "New Entry";

		[FormerlySerializedAs("m_TableName")]
		[SerializeField]
		private string m_TableCollectionName;

		[FormerlySerializedAs("m_TableNameGuidString")]
		[SerializeField]
		private string m_TableCollectionNameGuidString;

		[SerializeField]
		private List<SharedTableData.SharedTableEntry> m_Entries = new List<SharedTableData.SharedTableEntry>();

		[SerializeField]
		[MetadataType(MetadataType.SharedTableData)]
		private MetadataCollection m_Metadata = new MetadataCollection();

		[SerializeReference]
		private IKeyGenerator m_KeyGenerator = new DistributedUIDGenerator();

		private Guid m_TableCollectionNameGuid;

		private Dictionary<long, SharedTableData.SharedTableEntry> m_IdDictionary = new Dictionary<long, SharedTableData.SharedTableEntry>();

		private Dictionary<string, SharedTableData.SharedTableEntry> m_KeyDictionary = new Dictionary<string, SharedTableData.SharedTableEntry>();

		[Serializable]
		public class SharedTableEntry
		{
			public long Id
			{
				get
				{
					return this.m_Id;
				}
				internal set
				{
					this.m_Id = value;
				}
			}

			public string Key
			{
				get
				{
					return this.m_Key;
				}
				internal set
				{
					this.m_Key = value;
				}
			}

			public MetadataCollection Metadata
			{
				get
				{
					return this.m_Metadata;
				}
				set
				{
					this.m_Metadata = value;
				}
			}

			public override string ToString()
			{
				return string.Format("{0} - {1}", this.Id, this.Key);
			}

			[SerializeField]
			private long m_Id;

			[SerializeField]
			private string m_Key;

			[SerializeField]
			private MetadataCollection m_Metadata = new MetadataCollection();
		}
	}
}
