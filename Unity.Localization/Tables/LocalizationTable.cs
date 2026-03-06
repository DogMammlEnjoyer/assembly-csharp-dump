using System;
using System.Collections.Generic;
using UnityEngine.Localization.Metadata;
using UnityEngine.Serialization;

namespace UnityEngine.Localization.Tables
{
	public abstract class LocalizationTable : ScriptableObject, IMetadataCollection, IComparable<LocalizationTable>
	{
		public LocaleIdentifier LocaleIdentifier
		{
			get
			{
				return this.m_LocaleId;
			}
			set
			{
				this.m_LocaleId = value;
			}
		}

		public string TableCollectionName
		{
			get
			{
				this.VerifySharedTableDataIsNotNull();
				return this.SharedData.TableCollectionName;
			}
		}

		public SharedTableData SharedData
		{
			get
			{
				return this.m_SharedData;
			}
			set
			{
				this.m_SharedData = value;
			}
		}

		internal List<TableEntryData> TableData
		{
			get
			{
				return this.m_TableData;
			}
		}

		public IList<IMetadata> MetadataEntries
		{
			get
			{
				return this.m_Metadata.MetadataEntries;
			}
		}

		public TObject GetMetadata<TObject>() where TObject : IMetadata
		{
			return this.m_Metadata.GetMetadata<TObject>();
		}

		public void GetMetadatas<TObject>(IList<TObject> foundItems) where TObject : IMetadata
		{
			this.m_Metadata.GetMetadatas<TObject>(foundItems);
		}

		public IList<TObject> GetMetadatas<TObject>() where TObject : IMetadata
		{
			return this.m_Metadata.GetMetadatas<TObject>();
		}

		public void AddMetadata(IMetadata md)
		{
			this.m_Metadata.AddMetadata(md);
		}

		public bool RemoveMetadata(IMetadata md)
		{
			return this.m_Metadata.RemoveMetadata(md);
		}

		public bool Contains(IMetadata md)
		{
			return this.m_Metadata.Contains(md);
		}

		public abstract void CreateEmpty(TableEntryReference entryReference);

		protected long FindKeyId(string key, bool addKey)
		{
			this.VerifySharedTableDataIsNotNull();
			return this.SharedData.GetId(key, addKey);
		}

		private void VerifySharedTableDataIsNotNull()
		{
			if (this.SharedData == null)
			{
				throw new NullReferenceException("The Table \"" + base.name + "\" does not have a SharedTableData.");
			}
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", this.TableCollectionName, this.LocaleIdentifier);
		}

		public int CompareTo(LocalizationTable other)
		{
			if (other == null)
			{
				return 1;
			}
			return this.LocaleIdentifier.CompareTo(other.LocaleIdentifier);
		}

		[SerializeField]
		private LocaleIdentifier m_LocaleId;

		[FormerlySerializedAs("m_KeyDatabase")]
		[SerializeField]
		[HideInInspector]
		private SharedTableData m_SharedData;

		[SerializeField]
		private MetadataCollection m_Metadata = new MetadataCollection();

		[SerializeField]
		private List<TableEntryData> m_TableData = new List<TableEntryData>();
	}
}
