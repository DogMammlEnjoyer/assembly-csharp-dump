using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal class LoadAllTablesOperation<TTable, TEntry> : WaitForCurrentOperationAsyncOperationBase<IList<TTable>> where TTable : DetailedLocalizationTable<TEntry> where TEntry : TableEntry
	{
		public LoadAllTablesOperation()
		{
			this.m_LoadingCompletedAction = new Action<AsyncOperationHandle<IList<TTable>>>(this.LoadingCompleted);
		}

		public void Init(LocalizedDatabase<TTable, TEntry> database, Locale locale)
		{
			this.m_Database = database;
			this.m_SelectedLocale = locale;
		}

		protected override void Execute()
		{
			if (this.m_SelectedLocale == null)
			{
				this.m_SelectedLocale = LocalizationSettings.SelectedLocale;
				if (this.m_SelectedLocale == null)
				{
					base.Complete(null, false, "SelectedLocale is null. Could not load table.");
					return;
				}
			}
			string label = (this.m_SelectedLocale != null) ? AddressHelper.FormatAssetLabel(this.m_SelectedLocale.Identifier) : AddressHelper.FormatAssetLabel(LocalizationSettings.SelectedLocaleAsync.Result.Identifier);
			this.m_AllTablesOperation = AddressablesInterface.LoadAssetsWithLabel<TTable>(label, null);
			if (this.m_AllTablesOperation.IsDone)
			{
				this.LoadingCompleted(this.m_AllTablesOperation);
				return;
			}
			this.m_AllTablesOperation.Completed += this.m_LoadingCompletedAction;
			base.CurrentOperation = this.m_AllTablesOperation;
		}

		private void LoadingCompleted(AsyncOperationHandle<IList<TTable>> obj)
		{
			if (obj.Result != null)
			{
				foreach (TTable ttable in obj.Result)
				{
					if (!(ttable == null))
					{
						this.m_Database.GetTableAsync(ttable.TableCollectionName, this.m_SelectedLocale);
					}
				}
			}
			IList<TTable> result = obj.Result;
			bool success = obj.Status == AsyncOperationStatus.Succeeded;
			Exception operationException = obj.OperationException;
			base.Complete(result, success, (operationException != null) ? operationException.Message : null);
		}

		protected override void Destroy()
		{
			base.Destroy();
			LoadAllTablesOperation<TTable, TEntry>.Pool.Release(this);
			AddressablesInterface.ReleaseAndReset<IList<TTable>>(ref this.m_AllTablesOperation);
		}

		private readonly Action<AsyncOperationHandle<IList<TTable>>> m_LoadingCompletedAction;

		private AsyncOperationHandle<IList<TTable>> m_AllTablesOperation;

		private LocalizedDatabase<TTable, TEntry> m_Database;

		private Locale m_SelectedLocale;

		public static readonly ObjectPool<LoadAllTablesOperation<TTable, TEntry>> Pool = new ObjectPool<LoadAllTablesOperation<TTable, TEntry>>(() => new LoadAllTablesOperation<TTable, TEntry>(), null, null, null, false, 10, 10000);
	}
}
