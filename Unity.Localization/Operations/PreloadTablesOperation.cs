using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal class PreloadTablesOperation<TTable, TEntry> : WaitForCurrentOperationAsyncOperationBase<LocalizedDatabase<TTable, TEntry>> where TTable : DetailedLocalizationTable<TEntry> where TEntry : TableEntry
	{
		public PreloadTablesOperation()
		{
			this.m_LoadTableContentsAction = delegate(AsyncOperationHandle<IList<AsyncOperationHandle>> a)
			{
				this.LoadTableContents();
				AddressablesInterface.Release(a);
			};
			this.m_FinishPreloadingAction = new Action<AsyncOperationHandle>(this.FinishPreloading);
		}

		public void Init(LocalizedDatabase<TTable, TEntry> database, IList<TableReference> tableReference, Locale locale = null)
		{
			this.m_Database = database;
			this.m_TableReferences = tableReference;
			this.m_SelectedLocale = locale;
		}

		protected override void Execute()
		{
			this.BeginPreloadingTables();
		}

		private void BeginPreloadingTables()
		{
			foreach (TableReference tableReference in this.m_TableReferences)
			{
				AsyncOperationHandle<TTable> tableAsync = this.m_Database.GetTableAsync(tableReference, this.m_SelectedLocale);
				this.m_LoadTables.Add(tableAsync);
				if (!tableAsync.IsDone)
				{
					this.m_LoadTablesOperation.Add(tableAsync);
				}
			}
			if (this.m_LoadTablesOperation.Count > 0)
			{
				this.m_LoadTablesOperationHandle = AddressablesInterface.CreateGroupOperation(this.m_LoadTablesOperation);
				if (!this.m_LoadTablesOperationHandle.IsDone)
				{
					base.CurrentOperation = this.m_LoadTablesOperationHandle;
					this.m_LoadTablesOperationHandle.Completed += this.m_LoadTableContentsAction;
					return;
				}
			}
			this.LoadTableContents();
		}

		private void LoadTableContents()
		{
			foreach (AsyncOperationHandle<TTable> asyncOperationHandle in this.m_LoadTables)
			{
				if (asyncOperationHandle.Result == null)
				{
					base.Complete(null, false, "Table is null.");
					return;
				}
				IPreloadRequired preloadRequired = asyncOperationHandle.Result as IPreloadRequired;
				if (preloadRequired != null)
				{
					this.m_PreloadTablesOperations.Add(preloadRequired.PreloadOperation);
				}
			}
			if (this.m_PreloadTablesOperations.Count == 0)
			{
				base.Complete(this.m_Database, true, null);
				return;
			}
			this.m_PreloadTablesContentsHandle = AddressablesInterface.CreateGroupOperation(this.m_PreloadTablesOperations);
			if (!this.m_PreloadTablesContentsHandle.IsDone)
			{
				base.CurrentOperation = this.m_PreloadTablesContentsHandle;
				this.m_PreloadTablesContentsHandle.CompletedTypeless += this.m_FinishPreloadingAction;
				return;
			}
			this.FinishPreloading(this.m_PreloadTablesContentsHandle);
		}

		private void FinishPreloading(AsyncOperationHandle op)
		{
			base.Complete(this.m_Database, op.Status == AsyncOperationStatus.Succeeded, null);
		}

		protected override void Destroy()
		{
			base.Destroy();
			AddressablesInterface.ReleaseAndReset<IList<AsyncOperationHandle>>(ref this.m_LoadTablesOperationHandle);
			AddressablesInterface.ReleaseAndReset<IList<AsyncOperationHandle>>(ref this.m_PreloadTablesContentsHandle);
			this.m_LoadTables.Clear();
			this.m_LoadTablesOperation.Clear();
			this.m_PreloadTablesOperations.Clear();
			this.m_TableReferences = null;
			PreloadTablesOperation<TTable, TEntry>.Pool.Release(this);
		}

		private LocalizedDatabase<TTable, TEntry> m_Database;

		private readonly List<AsyncOperationHandle<TTable>> m_LoadTables = new List<AsyncOperationHandle<TTable>>();

		private readonly List<AsyncOperationHandle> m_LoadTablesOperation = new List<AsyncOperationHandle>();

		private readonly List<AsyncOperationHandle> m_PreloadTablesOperations = new List<AsyncOperationHandle>();

		private readonly Action<AsyncOperationHandle<IList<AsyncOperationHandle>>> m_LoadTableContentsAction;

		private readonly Action<AsyncOperationHandle> m_FinishPreloadingAction;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_LoadTablesOperationHandle;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_PreloadTablesContentsHandle;

		private IList<TableReference> m_TableReferences;

		private Locale m_SelectedLocale;

		public static readonly ObjectPool<PreloadTablesOperation<TTable, TEntry>> Pool = new ObjectPool<PreloadTablesOperation<TTable, TEntry>>(() => new PreloadTablesOperation<TTable, TEntry>(), null, null, null, false, 10, 10000);
	}
}
