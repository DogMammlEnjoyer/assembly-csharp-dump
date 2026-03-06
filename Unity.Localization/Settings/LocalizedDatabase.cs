using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Localization.Operations;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Settings
{
	[Serializable]
	public abstract class LocalizedDatabase<TTable, TEntry> : IPreloadRequired, IReset, IDisposable where TTable : DetailedLocalizationTable<TEntry> where TEntry : TableEntry
	{
		public AsyncOperationHandle PreloadOperation
		{
			get
			{
				if (!this.m_PreloadOperationHandle.IsValid())
				{
					PreloadDatabaseOperation<TTable, TEntry> preloadDatabaseOperation = PreloadDatabaseOperation<TTable, TEntry>.Pool.Get();
					preloadDatabaseOperation.Init(this);
					this.m_PreloadOperationHandle = AddressablesInterface.ResourceManager.StartOperation<LocalizedDatabase<TTable, TEntry>>(preloadDatabaseOperation, default(AsyncOperationHandle));
				}
				return this.m_PreloadOperationHandle;
			}
		}

		internal Action<AsyncOperationHandle> ReleaseNextFrame
		{
			get
			{
				return this.m_ReleaseNextFrame;
			}
		}

		[TupleElementNames(new string[]
		{
			"localeIdentifier",
			"tableNameOrGuid"
		})]
		internal Dictionary<ValueTuple<LocaleIdentifier, string>, AsyncOperationHandle<TTable>> TableOperations { [return: TupleElementNames(new string[]
		{
			"localeIdentifier",
			"tableNameOrGuid"
		})] get; } = new Dictionary<ValueTuple<LocaleIdentifier, string>, AsyncOperationHandle<TTable>>();

		internal Dictionary<Guid, AsyncOperationHandle<SharedTableData>> SharedTableDataOperations { get; } = new Dictionary<Guid, AsyncOperationHandle<SharedTableData>>();

		public virtual TableReference DefaultTable
		{
			get
			{
				return this.m_DefaultTableReference;
			}
			set
			{
				this.m_DefaultTableReference = value;
			}
		}

		public ITableProvider TableProvider
		{
			get
			{
				return this.m_CustomTableProvider;
			}
			set
			{
				this.m_CustomTableProvider = value;
			}
		}

		public ITablePostprocessor TablePostprocessor
		{
			get
			{
				return this.m_CustomTablePostprocessor;
			}
			set
			{
				this.m_CustomTablePostprocessor = value;
			}
		}

		public bool UseFallback
		{
			get
			{
				return this.m_UseFallback;
			}
			set
			{
				this.m_UseFallback = value;
			}
		}

		public AsynchronousBehaviour AsynchronousBehaviour
		{
			get
			{
				return this.m_AsynchronousBehaviour;
			}
			set
			{
				this.m_AsynchronousBehaviour = value;
			}
		}

		public LocalizedDatabase()
		{
			this.m_PatchTableContentsAction = new Action<AsyncOperationHandle<TTable>>(this.PatchTableContents);
			this.m_RegisterSharedTableAndGuidOperationAction = new Action<AsyncOperationHandle<TTable>>(this.RegisterSharedTableAndGuidOperation);
			this.m_RegisterCompletedTableOperationAction = new Action<AsyncOperationHandle<TTable>>(this.RegisterCompletedTableOperation);
			this.m_ReleaseNextFrame = new Action<AsyncOperationHandle>(LocalizationBehaviour.ReleaseNextFrame);
		}

		internal TableReference GetDefaultTable()
		{
			if (this.m_DefaultTableReference.ReferenceType == TableReference.Type.Empty)
			{
				throw new Exception("Trying to get the DefaultTable however the " + base.GetType().Name + " DefaultTable value has not been set. This can be configured in the Localization Settings.");
			}
			return this.m_DefaultTableReference;
		}

		internal void RegisterCompletedTableOperation(AsyncOperationHandle<TTable> tableOperation)
		{
			if (!tableOperation.IsDone)
			{
				tableOperation.Completed += this.m_RegisterCompletedTableOperationAction;
				return;
			}
			TTable result = tableOperation.Result;
			if (result == null)
			{
				return;
			}
			this.RegisterTableNameOperation(tableOperation, result.LocaleIdentifier, result.TableCollectionName);
			if (tableOperation.IsValid())
			{
				this.RegisterSharedTableAndGuidOperation(tableOperation);
			}
		}

		private void RegisterTableNameOperation(AsyncOperationHandle<TTable> tableOperation, LocaleIdentifier localeIdentifier, string tableName)
		{
			ValueTuple<LocaleIdentifier, string> key = new ValueTuple<LocaleIdentifier, string>(localeIdentifier, tableName);
			if (this.TableOperations.ContainsKey(key))
			{
				return;
			}
			this.TableOperations[key] = tableOperation;
			if (this.TablePostprocessor != null)
			{
				if (tableOperation.IsDone)
				{
					this.PatchTableContents(tableOperation);
					return;
				}
				tableOperation.Completed += this.m_PatchTableContentsAction;
			}
		}

		private void RegisterSharedTableAndGuidOperation(AsyncOperationHandle<TTable> tableOperation)
		{
			if (!tableOperation.IsDone)
			{
				tableOperation.Completed += this.m_RegisterSharedTableAndGuidOperationAction;
				return;
			}
			TTable result = tableOperation.Result;
			if (result == null)
			{
				return;
			}
			Guid tableCollectionNameGuid = result.SharedData.TableCollectionNameGuid;
			if (!this.SharedTableDataOperations.ContainsKey(tableCollectionNameGuid))
			{
				this.SharedTableDataOperations[tableCollectionNameGuid] = AddressablesInterface.ResourceManager.CreateCompletedOperation<SharedTableData>(result.SharedData, null);
			}
			ValueTuple<LocaleIdentifier, string> key = new ValueTuple<LocaleIdentifier, string>(result.LocaleIdentifier, TableReference.StringFromGuid(tableCollectionNameGuid));
			if (!this.TableOperations.ContainsKey(key))
			{
				AddressablesInterface.Acquire(tableOperation);
				this.TableOperations[key] = tableOperation;
			}
		}

		public AsyncOperationHandle<TTable> GetDefaultTableAsync()
		{
			return this.GetTableAsync(this.GetDefaultTable(), null);
		}

		public virtual AsyncOperationHandle<TTable> GetTableAsync(TableReference tableReference, Locale locale = null)
		{
			bool flag = locale != null || LocalizationSettings.SelectedLocaleAsync.IsDone;
			bool flag2 = true;
			if (flag)
			{
				if (locale == null)
				{
					if (LocalizationSettings.SelectedLocaleAsync.Result == null)
					{
						return AddressablesInterface.ResourceManager.CreateCompletedOperation<TTable>(default(TTable), "SelectedLocale is null. Database could not get table.");
					}
					locale = LocalizationSettings.SelectedLocaleAsync.Result;
				}
				flag2 = false;
			}
			tableReference.Validate();
			string text = (tableReference.ReferenceType == TableReference.Type.Guid) ? TableReference.StringFromGuid(tableReference.TableCollectionNameGuid) : tableReference.TableCollectionName;
			LocaleIdentifier localeIdentifier = flag2 ? LocalizedDatabase<TTable, TEntry>.k_SelectedLocaleId : locale.Identifier;
			AsyncOperationHandle<TTable> result;
			if (this.TableOperations.TryGetValue(new ValueTuple<LocaleIdentifier, string>(localeIdentifier, text), out result))
			{
				if (result.IsValid())
				{
					return result;
				}
				this.TableOperations.Remove(new ValueTuple<LocaleIdentifier, string>(localeIdentifier, text));
			}
			LoadTableOperation<TTable, TEntry> loadTableOperation = this.CreateLoadTableOperation();
			loadTableOperation.Init(this, tableReference, locale);
			loadTableOperation.Dependency = LocalizationSettings.InitializationOperation;
			AsyncOperationHandle<TTable> asyncOperationHandle = AddressablesInterface.ResourceManager.StartOperation<TTable>(loadTableOperation, LocalizationSettings.InitializationOperation);
			if (flag2 || tableReference.ReferenceType == TableReference.Type.Guid)
			{
				if (!flag2)
				{
					AddressablesInterface.Acquire(asyncOperationHandle);
				}
				this.TableOperations[new ValueTuple<LocaleIdentifier, string>(localeIdentifier, text)] = asyncOperationHandle;
			}
			else
			{
				this.RegisterTableNameOperation(asyncOperationHandle, localeIdentifier, text);
			}
			this.RegisterCompletedTableOperation(asyncOperationHandle);
			return asyncOperationHandle;
		}

		public virtual TTable GetTable(TableReference tableReference, Locale locale = null)
		{
			return this.GetTableAsync(tableReference, locale).WaitForCompletion();
		}

		public AsyncOperationHandle PreloadTables(TableReference tableReference, Locale locale = null)
		{
			PreloadTablesOperation<TTable, TEntry> preloadTablesOperation = this.CreatePreloadTablesOperation();
			preloadTablesOperation.Init(this, new TableReference[]
			{
				tableReference
			}, locale);
			preloadTablesOperation.Dependency = LocalizationSettings.InitializationOperation;
			AsyncOperationHandle<LocalizedDatabase<TTable, TEntry>> obj = AddressablesInterface.ResourceManager.StartOperation<LocalizedDatabase<TTable, TEntry>>(preloadTablesOperation, LocalizationSettings.InitializationOperation);
			if (LocalizationSettings.Instance.IsPlaying)
			{
				obj.CompletedTypeless += this.ReleaseNextFrame;
			}
			return obj;
		}

		public AsyncOperationHandle PreloadTables(IList<TableReference> tableReferences, Locale locale = null)
		{
			PreloadTablesOperation<TTable, TEntry> preloadTablesOperation = this.CreatePreloadTablesOperation();
			preloadTablesOperation.Init(this, tableReferences, locale);
			preloadTablesOperation.Dependency = LocalizationSettings.InitializationOperation;
			AsyncOperationHandle<LocalizedDatabase<TTable, TEntry>> obj = AddressablesInterface.ResourceManager.StartOperation<LocalizedDatabase<TTable, TEntry>>(preloadTablesOperation, LocalizationSettings.InitializationOperation);
			if (LocalizationSettings.Instance.IsPlaying)
			{
				obj.CompletedTypeless += this.ReleaseNextFrame;
			}
			return obj;
		}

		public void ReleaseAllTables(Locale locale = null)
		{
			HashSet<TTable> hashSet;
			using (CollectionPool<HashSet<TTable>, TTable>.Get(out hashSet))
			{
				foreach (AsyncOperationHandle<TTable> obj in this.TableOperations.Values)
				{
					if (obj.IsValid() && (!(locale != null) || !(obj.Result.LocaleIdentifier != locale.Identifier)))
					{
						if (obj.Result != null && !hashSet.Contains(obj.Result))
						{
							this.ReleaseTableContents(obj.Result);
							hashSet.Add(obj.Result);
						}
						AddressablesInterface.Release(obj);
					}
				}
			}
			foreach (KeyValuePair<Guid, AsyncOperationHandle<SharedTableData>> keyValuePair in this.SharedTableDataOperations)
			{
				AddressablesInterface.SafeRelease(keyValuePair.Value);
			}
			this.SharedTableDataOperations.Clear();
			if (this.m_PreloadOperationHandle.IsValid())
			{
				if (this.m_PreloadOperationHandle.IsDone)
				{
					AddressablesInterface.Release(this.m_PreloadOperationHandle);
				}
				this.m_PreloadOperationHandle = default(AsyncOperationHandle);
			}
			this.TableOperations.Clear();
		}

		public void ReleaseTable(TableReference tableReference, Locale locale = null)
		{
			tableReference.Validate();
			bool flag = locale == LocalizationSettings.SelectedLocaleAsync.Result;
			if (locale == null)
			{
				locale = LocalizationSettings.SelectedLocaleAsync.Result;
				flag = true;
				if (locale == null)
				{
					return;
				}
			}
			SharedTableData sharedTableData;
			if (tableReference.ReferenceType == TableReference.Type.Guid)
			{
				AsyncOperationHandle<SharedTableData> asyncOperationHandle;
				if (!this.SharedTableDataOperations.TryGetValue(tableReference.TableCollectionNameGuid, out asyncOperationHandle) || asyncOperationHandle.Result == null)
				{
					return;
				}
				sharedTableData = asyncOperationHandle.Result;
			}
			else
			{
				ValueTuple<LocaleIdentifier, string> key = new ValueTuple<LocaleIdentifier, string>(locale.Identifier, tableReference.TableCollectionName);
				AsyncOperationHandle<TTable> asyncOperationHandle2;
				if (!this.TableOperations.TryGetValue(key, out asyncOperationHandle2) || asyncOperationHandle2.Result == null)
				{
					return;
				}
				sharedTableData = asyncOperationHandle2.Result.SharedData;
			}
			if (sharedTableData == null)
			{
				return;
			}
			int num = 0;
			bool flag2 = false;
			List<ValueTuple<LocaleIdentifier, string>> list;
			using (CollectionPool<List<ValueTuple<LocaleIdentifier, string>>, ValueTuple<LocaleIdentifier, string>>.Get(out list))
			{
				foreach (KeyValuePair<ValueTuple<LocaleIdentifier, string>, AsyncOperationHandle<TTable>> keyValuePair in this.TableOperations)
				{
					if (keyValuePair.Value.IsValid() && !(keyValuePair.Value.Result == null) && !(keyValuePair.Value.Result.SharedData != sharedTableData))
					{
						if (keyValuePair.Key.Item1 == locale.Identifier || (flag && keyValuePair.Key.Item1 == LocalizedDatabase<TTable, TEntry>.k_SelectedLocaleId))
						{
							if (!flag2)
							{
								this.ReleaseTableContents(keyValuePair.Value.Result);
								flag2 = true;
							}
							AddressablesInterface.SafeRelease(keyValuePair.Value);
							list.Add(keyValuePair.Key);
						}
						else
						{
							num++;
						}
					}
				}
				foreach (ValueTuple<LocaleIdentifier, string> key2 in list)
				{
					this.TableOperations.Remove(key2);
				}
				AsyncOperationHandle<SharedTableData> obj;
				if (num == 0 && this.SharedTableDataOperations.TryGetValue(sharedTableData.TableCollectionNameGuid, out obj))
				{
					AddressablesInterface.SafeRelease(obj);
					this.SharedTableDataOperations.Remove(sharedTableData.TableCollectionNameGuid);
				}
			}
		}

		public virtual AsyncOperationHandle<IList<TTable>> GetAllTables(Locale locale = null)
		{
			LoadAllTablesOperation<TTable, TEntry> loadAllTablesOperation = LoadAllTablesOperation<TTable, TEntry>.Pool.Get();
			loadAllTablesOperation.Init(this, locale);
			loadAllTablesOperation.Dependency = LocalizationSettings.InitializationOperation;
			AsyncOperationHandle<IList<TTable>> result = AddressablesInterface.ResourceManager.StartOperation<IList<TTable>>(loadAllTablesOperation, LocalizationSettings.InitializationOperation);
			if (LocalizationSettings.Instance.IsPlaying)
			{
				result.CompletedTypeless += this.ReleaseNextFrame;
			}
			return result;
		}

		public virtual bool IsTableLoaded(TableReference tableReference, Locale locale = null)
		{
			string item = (tableReference.ReferenceType == TableReference.Type.Guid) ? TableReference.StringFromGuid(tableReference.TableCollectionNameGuid) : tableReference.TableCollectionName;
			ValueTuple<LocaleIdentifier, string> key = (locale != null) ? new ValueTuple<LocaleIdentifier, string>(locale.Identifier, item) : new ValueTuple<LocaleIdentifier, string>(LocalizationSettings.SelectedLocaleAsync.Result.Identifier, item);
			AsyncOperationHandle<TTable> asyncOperationHandle;
			return this.TableOperations.TryGetValue(key, out asyncOperationHandle) && asyncOperationHandle.Status == AsyncOperationStatus.Succeeded;
		}

		internal virtual LoadTableOperation<TTable, TEntry> CreateLoadTableOperation()
		{
			return LoadTableOperation<TTable, TEntry>.Pool.Get();
		}

		internal virtual PreloadTablesOperation<TTable, TEntry> CreatePreloadTablesOperation()
		{
			return PreloadTablesOperation<TTable, TEntry>.Pool.Get();
		}

		public virtual AsyncOperationHandle<LocalizedDatabase<TTable, TEntry>.TableEntryResult> GetTableEntryAsync(TableReference tableReference, TableEntryReference tableEntryReference, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings)
		{
			AsyncOperationHandle<TTable> tableAsync = this.GetTableAsync(tableReference, locale);
			GetTableEntryOperation<TTable, TEntry> getTableEntryOperation = GetTableEntryOperation<TTable, TEntry>.Pool.Get();
			bool useFallBack = (fallbackBehavior != FallbackBehavior.UseProjectSettings) ? (fallbackBehavior == FallbackBehavior.UseFallback) : this.UseFallback;
			getTableEntryOperation.Init(this, tableAsync, tableReference, tableEntryReference, locale, useFallBack, true);
			getTableEntryOperation.Dependency = tableAsync;
			return AddressablesInterface.ResourceManager.StartOperation<LocalizedDatabase<TTable, TEntry>.TableEntryResult>(getTableEntryOperation, tableAsync);
		}

		public virtual LocalizedDatabase<TTable, TEntry>.TableEntryResult GetTableEntry(TableReference tableReference, TableEntryReference tableEntryReference, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings)
		{
			return this.GetTableEntryAsync(tableReference, tableEntryReference, locale, fallbackBehavior).WaitForCompletion();
		}

		internal AsyncOperationHandle<SharedTableData> GetSharedTableData(Guid tableNameGuid)
		{
			AsyncOperationHandle<SharedTableData> asyncOperationHandle;
			if (this.SharedTableDataOperations.TryGetValue(tableNameGuid, out asyncOperationHandle))
			{
				return asyncOperationHandle;
			}
			asyncOperationHandle = AddressablesInterface.LoadAssetFromGUID<SharedTableData>(TableReference.StringFromGuid(tableNameGuid));
			this.SharedTableDataOperations[tableNameGuid] = asyncOperationHandle;
			return asyncOperationHandle;
		}

		internal virtual void ReleaseTableContents(TTable table)
		{
		}

		public virtual void OnLocaleChanged(Locale locale)
		{
			this.ReleaseAllTables(null);
		}

		private void PatchTableContents(AsyncOperationHandle<TTable> tableOperation)
		{
			if (this.TablePostprocessor != null && tableOperation.Result != null)
			{
				this.TablePostprocessor.PostprocessTable(tableOperation.Result);
			}
		}

		public void ResetState()
		{
			this.ReleaseAllTables(null);
		}

		void IDisposable.Dispose()
		{
			this.ReleaseAllTables(null);
		}

		[SerializeField]
		private TableReference m_DefaultTableReference;

		[SerializeReference]
		private ITableProvider m_CustomTableProvider;

		[SerializeReference]
		private ITablePostprocessor m_CustomTablePostprocessor;

		[SerializeField]
		private AsynchronousBehaviour m_AsynchronousBehaviour;

		[SerializeField]
		private bool m_UseFallback;

		internal AsyncOperationHandle m_PreloadOperationHandle;

		private Action<AsyncOperationHandle> m_ReleaseNextFrame;

		private readonly Action<AsyncOperationHandle<TTable>> m_PatchTableContentsAction;

		private readonly Action<AsyncOperationHandle<TTable>> m_RegisterSharedTableAndGuidOperationAction;

		private readonly Action<AsyncOperationHandle<TTable>> m_RegisterCompletedTableOperationAction;

		internal static readonly LocaleIdentifier k_SelectedLocaleId = new LocaleIdentifier("selected locale placeholder");

		public struct TableEntryResult
		{
			public readonly TEntry Entry { get; }

			public readonly TTable Table { get; }

			internal TableEntryResult(TEntry entry, TTable table)
			{
				this.Entry = entry;
				this.Table = table;
			}
		}
	}
}
