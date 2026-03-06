using System;
using System.Collections.Generic;

namespace UnityEngine.Localization.Metadata
{
	[Serializable]
	public class MetadataCollection : IMetadataCollection
	{
		public IList<IMetadata> MetadataEntries
		{
			get
			{
				return this.m_Items;
			}
		}

		public bool HasData
		{
			get
			{
				return this.MetadataEntries != null && this.MetadataEntries.Count > 0;
			}
		}

		public bool HasMetadata<TObject>() where TObject : IMetadata
		{
			return this.GetMetadata<TObject>() != null;
		}

		public TObject GetMetadata<TObject>() where TObject : IMetadata
		{
			foreach (IMetadata metadata in this.m_Items)
			{
				if (metadata is TObject)
				{
					return (TObject)((object)metadata);
				}
			}
			return default(TObject);
		}

		public void GetMetadatas<TObject>(IList<TObject> foundItems) where TObject : IMetadata
		{
			foundItems.Clear();
			foreach (IMetadata metadata in this.m_Items)
			{
				if (metadata is TObject)
				{
					TObject item = (TObject)((object)metadata);
					foundItems.Add(item);
				}
			}
		}

		public IList<TObject> GetMetadatas<TObject>() where TObject : IMetadata
		{
			List<TObject> list = new List<TObject>();
			this.GetMetadatas<TObject>(list);
			return list;
		}

		public void AddMetadata(IMetadata md)
		{
			this.MetadataEntries.Add(md);
		}

		public bool RemoveMetadata(IMetadata md)
		{
			return this.m_Items.Remove(md);
		}

		public bool Contains(IMetadata md)
		{
			return this.m_Items.Contains(md);
		}

		[SerializeReference]
		private List<IMetadata> m_Items = new List<IMetadata>();
	}
}
