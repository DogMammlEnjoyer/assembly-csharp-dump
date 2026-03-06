using System;
using UnityEngine.Localization.Operations;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Settings
{
	[Serializable]
	public class LocalizedAssetDatabase : LocalizedDatabase<AssetTable, AssetTableEntry>
	{
		public AsyncOperationHandle<TObject> GetLocalizedAssetAsync<TObject>(TableEntryReference tableEntryReference, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings) where TObject : Object
		{
			return this.GetLocalizedAssetAsync<TObject>(base.GetDefaultTable(), tableEntryReference, locale, fallbackBehavior);
		}

		public TObject GetLocalizedAsset<TObject>(TableEntryReference tableEntryReference, Locale locale = null) where TObject : Object
		{
			return this.GetLocalizedAssetAsync<TObject>(base.GetDefaultTable(), tableEntryReference, locale, FallbackBehavior.UseProjectSettings).WaitForCompletion();
		}

		public virtual AsyncOperationHandle<TObject> GetLocalizedAssetAsync<TObject>(TableReference tableReference, TableEntryReference tableEntryReference, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings) where TObject : Object
		{
			return this.GetLocalizedAssetAsyncInternal<TObject>(tableReference, tableEntryReference, locale, fallbackBehavior);
		}

		public virtual TObject GetLocalizedAsset<TObject>(TableReference tableReference, TableEntryReference tableEntryReference, Locale locale = null) where TObject : Object
		{
			return this.GetLocalizedAssetAsyncInternal<TObject>(tableReference, tableEntryReference, locale, FallbackBehavior.UseProjectSettings).WaitForCompletion();
		}

		protected virtual AsyncOperationHandle<TObject> GetLocalizedAssetAsyncInternal<TObject>(TableReference tableReference, TableEntryReference tableEntryReference, Locale locale, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings) where TObject : Object
		{
			AsyncOperationHandle<LocalizedDatabase<AssetTable, AssetTableEntry>.TableEntryResult> tableEntryAsync = this.GetTableEntryAsync(tableReference, tableEntryReference, locale, fallbackBehavior);
			LoadAssetOperation<TObject> loadAssetOperation = LoadAssetOperation<TObject>.Pool.Get();
			loadAssetOperation.Init(tableEntryAsync, true);
			loadAssetOperation.Dependency = tableEntryAsync;
			return AddressablesInterface.ResourceManager.StartOperation<TObject>(loadAssetOperation, tableEntryAsync);
		}

		internal override void ReleaseTableContents(AssetTable table)
		{
			table.ReleaseAssets();
		}
	}
}
