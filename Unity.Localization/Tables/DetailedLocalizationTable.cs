using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.Metadata;

namespace UnityEngine.Localization.Tables
{
	public abstract class DetailedLocalizationTable<TEntry> : LocalizationTable, IDictionary<long, TEntry>, ICollection<KeyValuePair<long, TEntry>>, IEnumerable<KeyValuePair<long, TEntry>>, IEnumerable, ISerializationCallbackReceiver where TEntry : TableEntry
	{
		ICollection<long> IDictionary<long, !0>.Keys
		{
			get
			{
				return this.m_TableEntries.Keys;
			}
		}

		public ICollection<TEntry> Values
		{
			get
			{
				return this.m_TableEntries.Values;
			}
		}

		public int Count
		{
			get
			{
				return this.m_TableEntries.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public TEntry this[long key]
		{
			get
			{
				return this.m_TableEntries[key];
			}
			set
			{
				if (key == 0L)
				{
					throw new ArgumentException("Key Id value 0, is not valid. All Key Id's must be non-zero.");
				}
				if (value.Table != this)
				{
					throw new ArgumentException("Table entry does not belong to this table. Table entries can not be shared across tables.");
				}
				this.RemoveEntry(value.Data.Id);
				value.Data.Id = key;
				this.m_TableEntries[key] = value;
			}
		}

		public TEntry this[string keyName]
		{
			get
			{
				return this.GetEntry(keyName);
			}
			set
			{
				if (value.Table != this)
				{
					throw new ArgumentException("Table entry does not belong to this table. Table entries can not be shared across tables.");
				}
				long key = base.FindKeyId(keyName, true);
				this[key] = value;
			}
		}

		public abstract TEntry CreateTableEntry();

		internal TEntry CreateTableEntry(TableEntryData data)
		{
			TEntry tentry = this.CreateTableEntry();
			tentry.Data = data;
			return tentry;
		}

		public override void CreateEmpty(TableEntryReference entryReference)
		{
			this.AddEntryFromReference(entryReference, string.Empty);
		}

		public TEntry AddEntry(string key, string localized)
		{
			long num = base.FindKeyId(key, true);
			if (num != 0L)
			{
				return this.AddEntry(num, localized);
			}
			return default(TEntry);
		}

		public virtual TEntry AddEntry(long keyId, string localized)
		{
			if (keyId == 0L)
			{
				throw new ArgumentException(string.Format("Key Id value {0}({1}), is not valid. All Key Id's must be non-zero.", "EmptyId", 0L), "keyId");
			}
			TEntry tentry;
			if (!this.m_TableEntries.TryGetValue(keyId, out tentry))
			{
				tentry = this.CreateTableEntry();
				tentry.Data = new TableEntryData(keyId);
				this.m_TableEntries[keyId] = tentry;
			}
			tentry.Data.Localized = localized;
			return tentry;
		}

		public TEntry AddEntryFromReference(TableEntryReference entryReference, string localized)
		{
			if (entryReference.ReferenceType == TableEntryReference.Type.Id)
			{
				return this.AddEntry(entryReference.KeyId, localized);
			}
			if (entryReference.ReferenceType == TableEntryReference.Type.Name)
			{
				return this.AddEntry(entryReference.Key, localized);
			}
			throw new ArgumentException("TableEntryReference should not be Empty", "entryReference");
		}

		public bool RemoveEntry(string key)
		{
			long num = base.FindKeyId(key, false);
			return num != 0L && this.RemoveEntry(num);
		}

		public virtual bool RemoveEntry(long keyId)
		{
			TEntry tentry;
			if (this.m_TableEntries.TryGetValue(keyId, out tentry))
			{
				for (int i = 0; i < base.MetadataEntries.Count; i++)
				{
					SharedTableEntryMetadata sharedTableEntryMetadata = base.MetadataEntries[i] as SharedTableEntryMetadata;
					if (sharedTableEntryMetadata != null)
					{
						sharedTableEntryMetadata.Unregister(tentry);
						if (sharedTableEntryMetadata.Count == 0)
						{
							base.MetadataEntries.RemoveAt(i);
							i--;
						}
					}
				}
				int num = 0;
				for (;;)
				{
					int num2 = num;
					SharedTableData sharedData = base.SharedData;
					int? num3 = (sharedData != null) ? new int?(sharedData.Metadata.MetadataEntries.Count) : null;
					if (!(num2 < num3.GetValueOrDefault() & num3 != null))
					{
						break;
					}
					SharedTableCollectionMetadata sharedTableCollectionMetadata = base.SharedData.Metadata.MetadataEntries[num] as SharedTableCollectionMetadata;
					if (sharedTableCollectionMetadata != null)
					{
						sharedTableCollectionMetadata.RemoveEntry(keyId, base.LocaleIdentifier.Code);
						if (sharedTableCollectionMetadata.IsEmpty)
						{
							base.SharedData.Metadata.MetadataEntries.RemoveAt(num);
							num--;
						}
					}
					num++;
				}
				tentry.Data.Id = 0L;
				tentry.Table = null;
				return this.m_TableEntries.Remove(keyId);
			}
			return false;
		}

		public TEntry GetEntryFromReference(TableEntryReference entryReference)
		{
			if (entryReference.ReferenceType == TableEntryReference.Type.Id)
			{
				return this.GetEntry(entryReference.KeyId);
			}
			if (entryReference.ReferenceType == TableEntryReference.Type.Name)
			{
				return this.GetEntry(entryReference.Key);
			}
			return default(TEntry);
		}

		public TEntry GetEntry(string key)
		{
			long num = base.FindKeyId(key, false);
			if (num != 0L)
			{
				return this.GetEntry(num);
			}
			return default(TEntry);
		}

		public virtual TEntry GetEntry(long keyId)
		{
			TEntry result;
			this.m_TableEntries.TryGetValue(keyId, out result);
			return result;
		}

		public void Add(long keyId, TEntry value)
		{
			this[keyId] = value;
		}

		public void Add(KeyValuePair<long, TEntry> item)
		{
			this[item.Key] = item.Value;
		}

		public bool ContainsKey(long keyId)
		{
			return this.m_TableEntries.ContainsKey(keyId);
		}

		public bool ContainsValue(string localized)
		{
			using (Dictionary<long, TEntry>.ValueCollection.Enumerator enumerator = this.m_TableEntries.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Data.Localized == localized)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool Contains(KeyValuePair<long, TEntry> item)
		{
			return this.m_TableEntries.Contains(item);
		}

		public bool Remove(long keyId)
		{
			return this.RemoveEntry(keyId);
		}

		public bool Remove(KeyValuePair<long, TEntry> item)
		{
			if (this.Contains(item))
			{
				this.RemoveEntry(item.Key);
				return true;
			}
			return false;
		}

		public IList<TEntry> CheckForMissingSharedTableDataEntries(MissingEntryAction action = MissingEntryAction.Nothing)
		{
			TEntry[] array = (from e in this.m_TableEntries
			where !base.SharedData.Contains(e.Key)
			select e.Value).ToArray<TEntry>();
			if (array.Length == 0)
			{
				return array;
			}
			if (action == MissingEntryAction.AddEntriesToSharedData)
			{
				for (int i = 0; i < array.Length; i++)
				{
					SharedTableData.SharedTableEntry sharedTableEntry = base.SharedData.AddKey(null);
					base.SharedData.RemapId(sharedTableEntry.Id, array[i].KeyId);
				}
			}
			else if (action == MissingEntryAction.RemoveEntriesFromTable)
			{
				for (int j = 0; j < array.Length; j++)
				{
					this.RemoveEntry(array[j].KeyId);
				}
			}
			return array;
		}

		public bool TryGetValue(long keyId, out TEntry value)
		{
			return this.m_TableEntries.TryGetValue(keyId, out value);
		}

		public void Clear()
		{
			base.TableData.Clear();
			this.m_TableEntries.Clear();
		}

		public void CopyTo(KeyValuePair<long, TEntry>[] array, int arrayIndex)
		{
			foreach (KeyValuePair<long, TEntry> keyValuePair in this.m_TableEntries)
			{
				array[arrayIndex++] = keyValuePair;
			}
		}

		public IEnumerator<KeyValuePair<long, TEntry>> GetEnumerator()
		{
			return this.m_TableEntries.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.m_TableEntries.GetEnumerator();
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", base.TableCollectionName, base.LocaleIdentifier);
		}

		public void OnBeforeSerialize()
		{
			base.TableData.Clear();
			foreach (KeyValuePair<long, TEntry> keyValuePair in this)
			{
				keyValuePair.Value.Data.Id = keyValuePair.Key;
				base.TableData.Add(keyValuePair.Value.Data);
			}
		}

		public void OnAfterDeserialize()
		{
			try
			{
				this.m_TableEntries = base.TableData.ToDictionary((TableEntryData o) => o.Id, new Func<TableEntryData, TEntry>(this.CreateTableEntry));
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error Deserializing Table Data \"{0}({1})\".\n{2}\n{3}", new object[]
				{
					base.TableCollectionName,
					base.LocaleIdentifier,
					ex.Message,
					ex.InnerException
				}), this);
			}
		}

		private Dictionary<long, TEntry> m_TableEntries = new Dictionary<long, TEntry>();
	}
}
