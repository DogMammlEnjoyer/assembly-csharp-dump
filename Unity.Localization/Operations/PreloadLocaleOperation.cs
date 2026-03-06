using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.Localization.Operations
{
	internal class PreloadLocaleOperation<TTable, TEntry> : WaitForCurrentOperationAsyncOperationBase<LocalizedDatabase<TTable, TEntry>> where TTable : DetailedLocalizationTable<TEntry> where TEntry : TableEntry
	{
		protected override float Progress
		{
			get
			{
				return this.m_Progress;
			}
		}

		protected override string DebugName
		{
			get
			{
				return string.Format("Preload ({0}) {1}", this.m_Locale, this.m_Database.GetType());
			}
		}

		public PreloadLocaleOperation()
		{
			this.m_LoadTablesAction = new Action<AsyncOperationHandle<IList<IResourceLocation>>>(this.LoadTables);
			this.m_LoadTableContentsAction = new Action<AsyncOperationHandle<TTable>>(this.LoadTableContents);
			this.m_FinishPreloadingAction = new Action<AsyncOperationHandle>(this.FinishPreloading);
			this.m_PreloadTablesCompletedAction = new Action<AsyncOperationHandle<IList<AsyncOperationHandle>>>(this.PreloadTablesCompleted);
		}

		public void Init(LocalizedDatabase<TTable, TEntry> database, Locale locale)
		{
			this.m_Database = database;
			this.m_Locale = locale;
			this.m_LoadTablesOperations.Clear();
			this.m_PreloadTableContentsOperations.Clear();
		}

		protected override void Execute()
		{
			this.BeginPreloading();
		}

		private void BeginPreloading()
		{
			this.m_Progress = 0f;
			string item = AddressHelper.FormatAssetLabel(this.m_Locale.Identifier);
			this.m_ResourceLabels.Clear();
			this.m_ResourceLabels.Add(item);
			this.m_ResourceLabels.Add("Preload");
			this.m_LoadResourcesOperation = AddressablesInterface.LoadResourceLocationsWithLabelsAsync(this.m_ResourceLabels, Addressables.MergeMode.Intersection, typeof(TTable));
			if (!this.m_LoadResourcesOperation.IsValid())
			{
				this.CompleteAndRelease(true, null);
				return;
			}
			if (this.m_LoadResourcesOperation.IsDone)
			{
				this.LoadTables(this.m_LoadResourcesOperation);
				return;
			}
			base.CurrentOperation = this.m_LoadResourcesOperation;
			this.m_LoadResourcesOperation.Completed += this.m_LoadTablesAction;
		}

		private void LoadTables(AsyncOperationHandle<IList<IResourceLocation>> loadResourcesOperation)
		{
			if (loadResourcesOperation.Status != AsyncOperationStatus.Succeeded)
			{
				bool success = false;
				string str = "Failed to locate preload tables for ";
				Locale locale = this.m_Locale;
				this.CompleteAndRelease(success, str + ((locale != null) ? locale.ToString() : null));
				return;
			}
			if (loadResourcesOperation.Result.Count == 0)
			{
				this.m_Progress = 1f;
				this.CompleteAndRelease(true, null);
				return;
			}
			foreach (IResourceLocation location in loadResourcesOperation.Result)
			{
				AsyncOperationHandle<TTable> asyncOperationHandle = AddressablesInterface.LoadTableFromLocation<TTable>(location);
				this.m_LoadTablesOperations.Add(asyncOperationHandle);
				if (asyncOperationHandle.IsDone)
				{
					this.LoadTableContents(asyncOperationHandle);
				}
				else
				{
					asyncOperationHandle.Completed += this.m_LoadTableContentsAction;
				}
			}
			this.m_LoadTablesGroupOperation = AddressablesInterface.CreateGroupOperation(this.m_LoadTablesOperations);
			if (this.m_LoadTablesGroupOperation.IsDone)
			{
				this.PreloadTablesCompleted(this.m_LoadTablesGroupOperation);
				return;
			}
			base.CurrentOperation = this.m_LoadTablesGroupOperation;
			this.m_LoadTablesGroupOperation.Completed += this.m_PreloadTablesCompletedAction;
		}

		private void LoadTableContents(AsyncOperationHandle<TTable> operation)
		{
			this.m_Progress += 1f / (float)this.m_LoadTablesOperations.Count;
			if (operation.Result == null)
			{
				return;
			}
			TTable result = operation.Result;
			string tableCollectionName = result.TableCollectionName;
			AsyncOperationHandle<TTable> asyncOperationHandle;
			if (this.m_Database.TableOperations.TryGetValue(new ValueTuple<LocaleIdentifier, string>(result.LocaleIdentifier, tableCollectionName), out asyncOperationHandle))
			{
				LocalizationBehaviour.ReleaseNextFrame(operation);
				if (asyncOperationHandle.IsDone && asyncOperationHandle.Result != result)
				{
					Debug.LogError(string.Format("A table with the same key `{0}` already exists. Something went wrong during preloading of {1}. Table {2} does not match {3}.", new object[]
					{
						tableCollectionName,
						this.m_Locale,
						result,
						asyncOperationHandle.Result
					}));
					return;
				}
			}
			else
			{
				this.m_Database.RegisterCompletedTableOperation(operation);
			}
			IPreloadRequired preloadRequired = result as IPreloadRequired;
			if (preloadRequired != null)
			{
				AsyncOperationHandle preloadOperation = preloadRequired.PreloadOperation;
				if (!preloadOperation.IsDone)
				{
					this.m_PreloadTableContentsOperations.Add(preloadOperation);
				}
			}
		}

		private void PreloadTablesCompleted(AsyncOperationHandle<IList<AsyncOperationHandle>> obj)
		{
			if (this.m_PreloadTableContentsOperations.Count == 0)
			{
				this.CompleteAndRelease(true, null);
				return;
			}
			this.m_LoadTableContentsOperation = AddressablesInterface.CreateGroupOperation(this.m_PreloadTableContentsOperations);
			if (this.m_LoadTableContentsOperation.IsDone)
			{
				this.FinishPreloading(this.m_LoadTableContentsOperation);
				return;
			}
			base.CurrentOperation = this.m_LoadTableContentsOperation;
			this.m_LoadTableContentsOperation.CompletedTypeless += this.m_FinishPreloadingAction;
		}

		private void FinishPreloading(AsyncOperationHandle op)
		{
			this.m_Progress = 1f;
			this.CompleteAndRelease(op.Status == AsyncOperationStatus.Succeeded, null);
		}

		private void CompleteAndRelease(bool success, string errorMsg)
		{
			AddressablesInterface.ReleaseAndReset<IList<IResourceLocation>>(ref this.m_LoadResourcesOperation);
			AddressablesInterface.ReleaseAndReset<IList<AsyncOperationHandle>>(ref this.m_LoadTablesGroupOperation);
			AddressablesInterface.ReleaseAndReset<IList<AsyncOperationHandle>>(ref this.m_LoadTableContentsOperation);
			base.Complete(this.m_Database, success, errorMsg);
		}

		protected override void Destroy()
		{
			base.Destroy();
			PreloadLocaleOperation<TTable, TEntry>.Pool.Release(this);
		}

		private readonly Action<AsyncOperationHandle<IList<IResourceLocation>>> m_LoadTablesAction;

		private readonly Action<AsyncOperationHandle<TTable>> m_LoadTableContentsAction;

		private readonly Action<AsyncOperationHandle> m_FinishPreloadingAction;

		private readonly Action<AsyncOperationHandle<IList<AsyncOperationHandle>>> m_PreloadTablesCompletedAction;

		private LocalizedDatabase<TTable, TEntry> m_Database;

		private Locale m_Locale;

		private AsyncOperationHandle<IList<IResourceLocation>> m_LoadResourcesOperation;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_LoadTablesGroupOperation;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_LoadTableContentsOperation;

		private readonly List<AsyncOperationHandle> m_LoadTablesOperations = new List<AsyncOperationHandle>();

		private readonly List<AsyncOperationHandle> m_PreloadTableContentsOperations = new List<AsyncOperationHandle>();

		private readonly List<string> m_ResourceLabels = new List<string>();

		private float m_Progress;

		public static readonly ObjectPool<PreloadLocaleOperation<TTable, TEntry>> Pool = new ObjectPool<PreloadLocaleOperation<TTable, TEntry>>(() => new PreloadLocaleOperation<TTable, TEntry>(), null, null, null, false, 10, 10000);
	}
}
