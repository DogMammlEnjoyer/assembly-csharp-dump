using System;
using System.Collections.Generic;
using UnityEngine.Localization.Tables;

namespace UnityEngine.Localization.Metadata
{
	public abstract class SharedTableEntryMetadata : IMetadata, ISerializationCallbackReceiver
	{
		internal int Count
		{
			get
			{
				return this.m_EntriesLookup.Count;
			}
		}

		internal bool IsRegistered(TableEntry entry)
		{
			return this.m_EntriesLookup.Contains(entry.Data.Id);
		}

		internal void Register(TableEntry entry)
		{
			this.m_EntriesLookup.Add(entry.Data.Id);
		}

		internal void Unregister(TableEntry entry)
		{
			this.m_EntriesLookup.Remove(entry.Data.Id);
		}

		public void OnBeforeSerialize()
		{
			this.m_Entries = null;
			this.m_SharedEntries.Clear();
			foreach (long id in this.m_EntriesLookup)
			{
				this.m_SharedEntries.Add(new SharedTableEntryMetadata.Entry
				{
					id = id
				});
			}
		}

		public void OnAfterDeserialize()
		{
			if (this.m_EntriesLookup == null)
			{
				this.m_EntriesLookup = new HashSet<long>();
			}
			else
			{
				this.m_EntriesLookup.Clear();
			}
			if (this.m_Entries != null && this.m_Entries.Count > 0)
			{
				foreach (long item in this.m_Entries)
				{
					this.m_EntriesLookup.Add(item);
				}
				this.m_Entries = null;
			}
			if (this.m_SharedEntries != null && this.m_SharedEntries.Count > 0)
			{
				foreach (SharedTableEntryMetadata.Entry entry in this.m_SharedEntries)
				{
					this.m_EntriesLookup.Add(entry.id);
				}
			}
		}

		[SerializeField]
		private List<long> m_Entries;

		[SerializeField]
		private List<SharedTableEntryMetadata.Entry> m_SharedEntries = new List<SharedTableEntryMetadata.Entry>();

		private HashSet<long> m_EntriesLookup = new HashSet<long>();

		[Serializable]
		private struct Entry
		{
			public long id;
		}
	}
}
