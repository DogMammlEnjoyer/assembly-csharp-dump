using System;
using UnityEngine.Localization.Metadata;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Tables
{
	public class AssetTableEntry : TableEntry
	{
		internal AsyncOperationHandle<Object[]> PreloadAsyncOperation { get; set; }

		internal AsyncOperationHandle AsyncOperation { get; set; }

		public string Address
		{
			get
			{
				return base.Data.Localized;
			}
			set
			{
				base.Data.Localized = value;
				this.m_GuidCache = null;
				this.m_SubAssetNameCache = null;
			}
		}

		public string Guid
		{
			get
			{
				if (this.m_GuidCache == null)
				{
					this.m_GuidCache = AssetAddress.GetGuid(this.Address);
				}
				return this.m_GuidCache;
			}
			set
			{
				this.Address = value;
			}
		}

		public string SubAssetName
		{
			get
			{
				if (this.m_SubAssetNameCache == null)
				{
					this.m_SubAssetNameCache = AssetAddress.GetSubAssetName(this.Address);
				}
				return this.m_SubAssetNameCache;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.Address);
			}
		}

		public bool IsSubAsset
		{
			get
			{
				return AssetAddress.IsSubAsset(this.Address);
			}
		}

		internal AssetTableEntry()
		{
		}

		public void RemoveFromTable()
		{
			AssetTable assetTable = base.Table as AssetTable;
			if (assetTable == null)
			{
				Debug.LogWarning(string.Format("Failed to remove {0} with id {1} and address `{2}` as it does not belong to a table.", "AssetTableEntry", base.KeyId, this.Address));
				return;
			}
			assetTable.Remove(base.KeyId);
		}

		internal Type GetExpectedType()
		{
			foreach (IMetadata metadata in base.Table.SharedData.Metadata.MetadataEntries)
			{
				AssetTypeMetadata assetTypeMetadata = metadata as AssetTypeMetadata;
				if (assetTypeMetadata != null && assetTypeMetadata.Contains(base.KeyId))
				{
					return assetTypeMetadata.Type;
				}
			}
			return typeof(Object);
		}

		public void SetAssetOverride<T>(T asset) where T : Object
		{
			AddressablesInterface.SafeRelease(this.AsyncOperation);
			this.AsyncOperation = AddressablesInterface.ResourceManager.CreateCompletedOperation<T>(asset, null);
		}

		private string m_GuidCache;

		private string m_SubAssetNameCache;
	}
}
