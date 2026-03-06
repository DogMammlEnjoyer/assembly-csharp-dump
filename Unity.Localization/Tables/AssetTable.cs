using System;
using System.Collections.Generic;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Operations;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Tables
{
	public class AssetTable : DetailedLocalizationTable<AssetTableEntry>, IPreloadRequired
	{
		private ResourceManager ResourceManager
		{
			get
			{
				return AddressablesInterface.ResourceManager;
			}
		}

		public virtual AsyncOperationHandle PreloadOperation
		{
			get
			{
				if (!this.m_PreloadOperationHandle.IsValid())
				{
					this.m_PreloadOperationHandle = this.PreloadAssets();
				}
				return this.m_PreloadOperationHandle;
			}
		}

		private AsyncOperationHandle PreloadAssets()
		{
			PreloadAssetTableMetadata preloadAssetTableMetadata = base.GetMetadata<PreloadAssetTableMetadata>() ?? base.SharedData.Metadata.GetMetadata<PreloadAssetTableMetadata>();
			if (preloadAssetTableMetadata == null || preloadAssetTableMetadata.Behaviour > PreloadAssetTableMetadata.PreloadBehaviour.NoPreload)
			{
				List<AsyncOperationHandle> list = CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Get();
				foreach (AssetTableEntry assetTableEntry in base.Values)
				{
					if (!assetTableEntry.IsEmpty && !assetTableEntry.PreloadAsyncOperation.IsValid())
					{
						assetTableEntry.PreloadAsyncOperation = AddressablesInterface.LoadAssetFromGUID<Object[]>(assetTableEntry.Guid);
						list.Add(assetTableEntry.PreloadAsyncOperation);
					}
				}
				if (list.Count > 0)
				{
					return AddressablesInterface.CreateGroupOperation(list);
				}
				CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Release(list);
			}
			return this.ResourceManager.CreateCompletedOperation<AssetTable>(this, null);
		}

		public AsyncOperationHandle<TObject> GetAssetAsync<TObject>(TableEntryReference entryReference) where TObject : Object
		{
			AssetTableEntry entryFromReference = base.GetEntryFromReference(entryReference);
			if (entryFromReference == null)
			{
				string str = entryReference.ResolveKeyName(base.SharedData);
				return this.ResourceManager.CreateCompletedOperation<TObject>(default(TObject), "Could not find asset with key \"" + str + "\"");
			}
			return this.GetAssetAsync<TObject>(entryFromReference);
		}

		internal AsyncOperationHandle<TObject> GetAssetAsync<TObject>(AssetTableEntry entry) where TObject : Object
		{
			if (entry.AsyncOperation.IsValid())
			{
				try
				{
					return entry.AsyncOperation.Convert<TObject>();
				}
				catch (InvalidCastException)
				{
					AddressablesInterface.Release(entry.AsyncOperation);
					entry.AsyncOperation = default(AsyncOperationHandle);
				}
			}
			if (entry.IsEmpty)
			{
				AsyncOperationHandle<TObject> asyncOperationHandle = this.ResourceManager.CreateCompletedOperation<TObject>(default(TObject), null);
				entry.AsyncOperation = asyncOperationHandle;
				return asyncOperationHandle;
			}
			LoadSubAssetOperation<TObject> loadSubAssetOperation = LoadSubAssetOperation<TObject>.Pool.Get();
			loadSubAssetOperation.Init(entry.PreloadAsyncOperation, entry.Address, entry.IsSubAsset, entry.SubAssetName);
			AsyncOperationHandle<TObject> asyncOperationHandle2 = this.ResourceManager.StartOperation<TObject>(loadSubAssetOperation, entry.PreloadAsyncOperation);
			entry.AsyncOperation = asyncOperationHandle2;
			return asyncOperationHandle2;
		}

		public void ReleaseAssets()
		{
			if (this.m_PreloadOperationHandle.IsValid())
			{
				AddressablesInterface.Release(this.m_PreloadOperationHandle);
				this.m_PreloadOperationHandle = default(AsyncOperationHandle);
			}
			foreach (AssetTableEntry entry in base.Values)
			{
				this.ReleaseAsset(entry);
			}
		}

		public void ReleaseAsset(AssetTableEntry entry)
		{
			if (entry == null)
			{
				return;
			}
			if (entry.PreloadAsyncOperation.IsValid())
			{
				AddressablesInterface.Release(entry.PreloadAsyncOperation);
				entry.PreloadAsyncOperation = default(AsyncOperationHandle<Object[]>);
			}
			if (entry.AsyncOperation.IsValid())
			{
				AddressablesInterface.Release(entry.AsyncOperation);
				entry.AsyncOperation = default(AsyncOperationHandle);
			}
		}

		public void ReleaseAsset(TableEntryReference entry)
		{
			this.ReleaseAsset(base.GetEntryFromReference(entry));
		}

		public override AssetTableEntry CreateTableEntry()
		{
			return new AssetTableEntry
			{
				Table = this,
				Data = new TableEntryData()
			};
		}

		private AsyncOperationHandle m_PreloadOperationHandle;
	}
}
