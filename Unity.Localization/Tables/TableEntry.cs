using System;
using System.Collections.Generic;
using UnityEngine.Localization.Metadata;

namespace UnityEngine.Localization.Tables
{
	public class TableEntry : IMetadataCollection
	{
		public LocalizationTable Table { get; internal set; }

		internal TableEntryData Data { get; set; }

		public SharedTableData.SharedTableEntry SharedEntry
		{
			get
			{
				if (this.m_SharedTableEntry == null)
				{
					this.m_SharedTableEntry = this.Table.SharedData.GetEntry(this.KeyId);
				}
				return this.m_SharedTableEntry;
			}
		}

		public string Key
		{
			get
			{
				SharedTableData.SharedTableEntry sharedEntry = this.SharedEntry;
				if (sharedEntry == null)
				{
					return null;
				}
				return sharedEntry.Key;
			}
			set
			{
				this.Table.SharedData.RenameKey(this.KeyId, value);
			}
		}

		public long KeyId
		{
			get
			{
				return this.Data.Id;
			}
		}

		public string LocalizedValue
		{
			get
			{
				return this.Data.Localized;
			}
		}

		public IList<IMetadata> MetadataEntries
		{
			get
			{
				return this.Data.Metadata.MetadataEntries;
			}
		}

		public TObject GetMetadata<TObject>() where TObject : IMetadata
		{
			return this.Data.Metadata.GetMetadata<TObject>();
		}

		public void GetMetadatas<TObject>(IList<TObject> foundItems) where TObject : IMetadata
		{
			this.Data.Metadata.GetMetadatas<TObject>(foundItems);
		}

		public IList<TObject> GetMetadatas<TObject>() where TObject : IMetadata
		{
			return this.Data.Metadata.GetMetadatas<TObject>();
		}

		public bool HasTagMetadata<TShared>() where TShared : SharedTableEntryMetadata
		{
			TShared tshared = this.Table.GetMetadata<TShared>();
			return tshared != null && tshared.IsRegistered(this);
		}

		public void AddTagMetadata<TShared>() where TShared : SharedTableEntryMetadata, new()
		{
			TShared tshared = default(TShared);
			foreach (IMetadata metadata in this.Table.MetadataEntries)
			{
				TShared tshared2 = metadata as TShared;
				if (tshared2 != null)
				{
					tshared = tshared2;
					if (tshared.IsRegistered(this))
					{
						return;
					}
					break;
				}
			}
			if (tshared == null)
			{
				tshared = Activator.CreateInstance<TShared>();
				this.Table.AddMetadata(tshared);
			}
			tshared.Register(this);
			this.AddMetadata(tshared);
		}

		public void AddSharedMetadata(SharedTableEntryMetadata md)
		{
			if (!this.Table.Contains(md))
			{
				this.Table.AddMetadata(md);
			}
			if (md.IsRegistered(this))
			{
				return;
			}
			md.Register(this);
			this.AddMetadata(md);
		}

		public void AddSharedMetadata(SharedTableCollectionMetadata md)
		{
			if (!this.Table.SharedData.Metadata.Contains(md))
			{
				this.Table.SharedData.Metadata.AddMetadata(md);
			}
			md.AddEntry(this.Data.Id, this.Table.LocaleIdentifier.Code);
		}

		public void AddMetadata(IMetadata md)
		{
			this.Data.Metadata.AddMetadata(md);
		}

		public void RemoveTagMetadata<TShared>() where TShared : SharedTableEntryMetadata
		{
			IList<IMetadata> metadataEntries = this.Table.MetadataEntries;
			IList<IMetadata> metadataEntries2 = this.Data.Metadata.MetadataEntries;
			for (int i = metadataEntries2.Count - 1; i >= 0; i--)
			{
				TShared tshared = metadataEntries2[i] as TShared;
				if (tshared != null)
				{
					tshared.Unregister(this);
					metadataEntries2.RemoveAt(i);
				}
			}
			for (int j = metadataEntries.Count - 1; j >= 0; j--)
			{
				TShared tshared2 = metadataEntries[j] as TShared;
				if (tshared2 != null)
				{
					tshared2.Unregister(this);
					if (tshared2.Count == 0)
					{
						metadataEntries.RemoveAt(j);
					}
				}
			}
		}

		public void RemoveSharedMetadata(SharedTableEntryMetadata md)
		{
			md.Unregister(this);
			this.RemoveMetadata(md);
			if (md.Count == 0 && this.Table.Contains(md))
			{
				this.Table.RemoveMetadata(md);
			}
		}

		public void RemoveSharedMetadata(SharedTableCollectionMetadata md)
		{
			md.RemoveEntry(this.Data.Id, this.Table.LocaleIdentifier.Code);
			if (md.IsEmpty)
			{
				this.Table.SharedData.Metadata.RemoveMetadata(md);
			}
		}

		public bool RemoveMetadata(IMetadata md)
		{
			return this.Data.Metadata.RemoveMetadata(md);
		}

		public bool Contains(IMetadata md)
		{
			return this.Data.Metadata.Contains(md);
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", this.KeyId, this.LocalizedValue);
		}

		private SharedTableData.SharedTableEntry m_SharedTableEntry;
	}
}
