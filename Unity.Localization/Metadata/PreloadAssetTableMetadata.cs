using System;

namespace UnityEngine.Localization.Metadata
{
	[Metadata(AllowedTypes = (MetadataType.SharedTableData | MetadataType.AssetTable), MenuItem = "Preload Assets")]
	[Serializable]
	public class PreloadAssetTableMetadata : IMetadata
	{
		public PreloadAssetTableMetadata.PreloadBehaviour Behaviour
		{
			get
			{
				return this.m_PreloadBehaviour;
			}
			set
			{
				this.m_PreloadBehaviour = value;
			}
		}

		[SerializeField]
		private PreloadAssetTableMetadata.PreloadBehaviour m_PreloadBehaviour;

		public enum PreloadBehaviour
		{
			NoPreload,
			PreloadAll
		}
	}
}
