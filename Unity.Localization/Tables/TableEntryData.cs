using System;
using UnityEngine.Localization.Metadata;

namespace UnityEngine.Localization.Tables
{
	[Serializable]
	internal class TableEntryData
	{
		public long Id
		{
			get
			{
				return this.m_Id;
			}
			set
			{
				this.m_Id = value;
			}
		}

		public string Localized
		{
			get
			{
				return this.m_Localized;
			}
			set
			{
				this.m_Localized = value;
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

		public TableEntryData()
		{
		}

		public TableEntryData(long id)
		{
			this.Id = id;
		}

		public TableEntryData(long id, string localized) : this(id)
		{
			this.Localized = localized;
		}

		[SerializeField]
		private long m_Id;

		[SerializeField]
		private string m_Localized;

		[SerializeField]
		private MetadataCollection m_Metadata = new MetadataCollection();
	}
}
