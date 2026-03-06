using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.Localization.Operations
{
	internal class LoadTableOperation<TTable, TEntry> : WaitForCurrentOperationAsyncOperationBase<TTable> where TTable : DetailedLocalizationTable<TEntry> where TEntry : TableEntry
	{
		public Action<AsyncOperationHandle<TTable>> RegisterTableOperation { get; private set; }

		public LoadTableOperation()
		{
			this.m_LoadTableByGuidAction = new Action<AsyncOperationHandle<SharedTableData>>(this.LoadTableByGuid);
			this.m_LoadTableResourceAction = new Action<AsyncOperationHandle<IList<IResourceLocation>>>(this.LoadTableResource);
			this.m_TableLoadedAction = new Action<AsyncOperationHandle<TTable>>(this.TableLoaded);
			this.m_CustomTableLoadedAction = new Action<AsyncOperationHandle<TTable>>(this.CustomTableLoaded);
		}

		public void Init(LocalizedDatabase<TTable, TEntry> database, TableReference tableReference, Locale locale)
		{
			this.m_Database = database;
			this.m_TableReference = tableReference;
			this.m_SelectedLocale = locale;
		}

		protected override void Execute()
		{
			if (this.m_SelectedLocale == null)
			{
				this.m_SelectedLocale = LocalizationSettings.SelectedLocale;
				if (this.m_SelectedLocale == null)
				{
					base.Complete(default(TTable), false, "SelectedLocale is null. Could not load table.");
					return;
				}
			}
			if (this.m_TableReference.ReferenceType != TableReference.Type.Guid)
			{
				this.FindTableByName(this.m_TableReference.TableCollectionName);
				return;
			}
			AsyncOperationHandle<SharedTableData> sharedTableData = this.m_Database.GetSharedTableData(this.m_TableReference.TableCollectionNameGuid);
			if (sharedTableData.IsDone)
			{
				this.LoadTableByGuid(sharedTableData);
				return;
			}
			base.CurrentOperation = sharedTableData;
			sharedTableData.Completed += this.m_LoadTableByGuidAction;
		}

		private void LoadTableByGuid(AsyncOperationHandle<SharedTableData> operationHandle)
		{
			if (operationHandle.Status == AsyncOperationStatus.Succeeded)
			{
				this.FindTableByName(operationHandle.Result.TableCollectionName);
				return;
			}
			base.Complete(default(TTable), false, string.Format("Failed to extract table name from shared table data {0}. Load Shared Table data operation failed.", this.m_TableReference));
		}

		private void FindTableByName(string collectionName)
		{
			this.m_CollectionName = collectionName;
			if (!this.TryLoadWithTableProvider())
			{
				this.DefaultLoadTableByName();
			}
		}

		private bool TryLoadWithTableProvider()
		{
			if (this.m_Database.TableProvider != null)
			{
				this.m_LoadTableOperation = this.m_Database.TableProvider.ProvideTableAsync<TTable>(this.m_CollectionName, this.m_SelectedLocale);
				if (this.m_LoadTableOperation.IsValid())
				{
					if (this.m_LoadTableOperation.IsDone)
					{
						this.CustomTableLoaded(this.m_LoadTableOperation);
					}
					else
					{
						this.m_LoadTableOperation.Completed += this.m_CustomTableLoadedAction;
						base.CurrentOperation = this.m_LoadTableOperation;
					}
					return true;
				}
			}
			return false;
		}

		private void DefaultLoadTableByName()
		{
			AsyncOperationHandle<IList<IResourceLocation>> asyncOperationHandle = AddressablesInterface.LoadTableLocationsAsync(this.m_CollectionName, this.m_SelectedLocale.Identifier, typeof(TTable));
			if (asyncOperationHandle.IsDone)
			{
				this.LoadTableResource(asyncOperationHandle);
				return;
			}
			base.CurrentOperation = asyncOperationHandle;
			asyncOperationHandle.Completed += this.m_LoadTableResourceAction;
		}

		private void LoadTableResource(AsyncOperationHandle<IList<IResourceLocation>> operationHandle)
		{
			if (operationHandle.Status != AsyncOperationStatus.Succeeded || operationHandle.Result.Count == 0)
			{
				AddressablesInterface.Release(operationHandle);
				base.Complete(default(TTable), true, string.Format("Could not find a {0} table with the name '{1}`", this.m_SelectedLocale, this.m_CollectionName));
				return;
			}
			this.m_LoadTableOperation = AddressablesInterface.LoadTableFromLocation<TTable>(operationHandle.Result[0]);
			if (this.m_LoadTableOperation.IsDone)
			{
				this.TableLoaded(this.m_LoadTableOperation);
			}
			else
			{
				base.CurrentOperation = this.m_LoadTableOperation;
				this.m_LoadTableOperation.Completed += this.m_TableLoadedAction;
			}
			AddressablesInterface.Release(operationHandle);
		}

		private void CustomTableLoaded(AsyncOperationHandle<TTable> operationHandle)
		{
			if (operationHandle.Status == AsyncOperationStatus.Succeeded && operationHandle.Result != null)
			{
				base.Complete(operationHandle.Result, true, null);
				return;
			}
			this.DefaultLoadTableByName();
		}

		private void TableLoaded(AsyncOperationHandle<TTable> operationHandle)
		{
			base.Complete(operationHandle.Result, operationHandle.Status == AsyncOperationStatus.Succeeded, null);
		}

		protected override void Destroy()
		{
			base.Destroy();
			AddressablesInterface.ReleaseAndReset<TTable>(ref this.m_LoadTableOperation);
			LoadTableOperation<TTable, TEntry>.Pool.Release(this);
		}

		public override string ToString()
		{
			return string.Format("{0}, Selected Locale: {1}, Table: {2}", base.GetType().Name, this.m_SelectedLocale, this.m_TableReference);
		}

		private readonly Action<AsyncOperationHandle<SharedTableData>> m_LoadTableByGuidAction;

		private readonly Action<AsyncOperationHandle<IList<IResourceLocation>>> m_LoadTableResourceAction;

		private readonly Action<AsyncOperationHandle<TTable>> m_TableLoadedAction;

		private readonly Action<AsyncOperationHandle<TTable>> m_CustomTableLoadedAction;

		private LocalizedDatabase<TTable, TEntry> m_Database;

		private TableReference m_TableReference;

		private AsyncOperationHandle<TTable> m_LoadTableOperation;

		private Locale m_SelectedLocale;

		private string m_CollectionName;

		public static readonly ObjectPool<LoadTableOperation<TTable, TEntry>> Pool = new ObjectPool<LoadTableOperation<TTable, TEntry>>(() => new LoadTableOperation<TTable, TEntry>(), null, null, null, false, 10, 10000);
	}
}
