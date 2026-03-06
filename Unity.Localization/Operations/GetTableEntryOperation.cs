using System;
using System.Collections.Generic;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal class GetTableEntryOperation<TTable, TEntry> : WaitForCurrentOperationAsyncOperationBase<LocalizedDatabase<TTable, TEntry>.TableEntryResult> where TTable : DetailedLocalizationTable<TEntry> where TEntry : TableEntry
	{
		public GetTableEntryOperation()
		{
			this.m_ExtractEntryFromTableAction = new Action<AsyncOperationHandle<TTable>>(this.ExtractEntryFromTable);
		}

		public void Init(LocalizedDatabase<TTable, TEntry> database, AsyncOperationHandle<TTable> loadTableOperation, TableReference tableReference, TableEntryReference tableEntryReference, Locale selectedLoale, bool UseFallBack, bool autoRelease)
		{
			this.m_Database = database;
			this.m_LoadTableOperation = loadTableOperation;
			AddressablesInterface.Acquire(this.m_LoadTableOperation);
			this.m_TableReference = tableReference;
			this.m_TableEntryReference = tableEntryReference;
			this.m_SelectedLocale = selectedLoale;
			this.m_UseFallback = UseFallBack;
			this.m_AutoRelease = autoRelease;
		}

		protected override void Execute()
		{
			AsyncOperationHandle<TTable> loadTableOperation = this.m_LoadTableOperation;
			this.m_LoadTableOperation = default(AsyncOperationHandle<TTable>);
			if (this.m_SelectedLocale == null)
			{
				this.m_SelectedLocale = LocalizationSettings.SelectedLocaleAsync.Result;
				if (this.m_SelectedLocale == null)
				{
					this.CompleteAndRelease(default(LocalizedDatabase<TTable, TEntry>.TableEntryResult), false, "SelectedLocale is null. Could not get table entry.");
					AddressablesInterface.SafeRelease(loadTableOperation);
					return;
				}
			}
			this.m_CurrentLocale = this.m_SelectedLocale;
			this.ExtractEntryFromTable(loadTableOperation);
		}

		private void ExtractEntryFromTable(AsyncOperationHandle<TTable> asyncOperation)
		{
			TTable ttable = asyncOperation.Result;
			TEntry entry = (ttable != null) ? ttable.GetEntryFromReference(this.m_TableEntryReference) : default(TEntry);
			if (this.HandleEntryOverride(asyncOperation, entry) || this.HandleFallback(asyncOperation, entry))
			{
				return;
			}
			this.m_LoadTableOperation = asyncOperation;
			this.CompleteAndRelease(new LocalizedDatabase<TTable, TEntry>.TableEntryResult(entry, asyncOperation.Result), true, null);
		}

		private bool HandleEntryOverride(AsyncOperationHandle<TTable> asyncOperation, TEntry entry)
		{
			if (entry != null)
			{
				for (int i = 0; i < entry.MetadataEntries.Count; i++)
				{
					IEntryOverride entryOverride = entry.MetadataEntries[i] as IEntryOverride;
					if (entryOverride != null && this.ApplyEntryOverride(entryOverride, asyncOperation, entry))
					{
						return true;
					}
				}
			}
			TEntry tentry = entry;
			SharedTableData.SharedTableEntry sharedTableEntry;
			if ((sharedTableEntry = ((tentry != null) ? tentry.SharedEntry : null)) == null)
			{
				TTable ttable = asyncOperation.Result;
				sharedTableEntry = ((ttable != null) ? ttable.SharedData.GetEntryFromReference(this.m_TableEntryReference) : null);
			}
			SharedTableData.SharedTableEntry sharedTableEntry2 = sharedTableEntry;
			if (sharedTableEntry2 != null)
			{
				for (int j = 0; j < sharedTableEntry2.Metadata.MetadataEntries.Count; j++)
				{
					IEntryOverride entryOverride2 = sharedTableEntry2.Metadata.MetadataEntries[j] as IEntryOverride;
					if (entryOverride2 != null && this.ApplyEntryOverride(entryOverride2, asyncOperation, entry))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool ApplyEntryOverride(IEntryOverride entryOverride, AsyncOperationHandle<TTable> asyncOperation, TEntry entry)
		{
			if (entryOverride == null)
			{
				return false;
			}
			TableReference tableReference;
			TableEntryReference tableEntryReference;
			EntryOverrideType @override = entryOverride.GetOverride(out tableReference, out tableEntryReference);
			if (@override == EntryOverrideType.None)
			{
				return false;
			}
			if (@override == EntryOverrideType.Entry)
			{
				this.m_TableEntryReference = tableEntryReference;
				this.ExtractEntryFromTable(asyncOperation);
				return true;
			}
			if (@override == EntryOverrideType.Table)
			{
				TEntry tentry = entry;
				SharedTableData.SharedTableEntry sharedTableEntry;
				if ((sharedTableEntry = ((tentry != null) ? tentry.SharedEntry : null)) == null)
				{
					TTable ttable = asyncOperation.Result;
					sharedTableEntry = ((ttable != null) ? ttable.SharedData.GetEntryFromReference(this.m_TableEntryReference) : null);
				}
				SharedTableData.SharedTableEntry sharedTableEntry2 = sharedTableEntry;
				this.m_TableEntryReference = sharedTableEntry2.Key;
			}
			else if (@override == EntryOverrideType.TableAndEntry)
			{
				this.m_TableEntryReference = tableEntryReference;
			}
			AddressablesInterface.Release(asyncOperation);
			asyncOperation = this.m_Database.GetTableAsync(tableReference, this.m_CurrentLocale);
			AddressablesInterface.Acquire(asyncOperation);
			if (asyncOperation.IsDone)
			{
				this.ExtractEntryFromTable(asyncOperation);
			}
			else
			{
				base.CurrentOperation = asyncOperation;
				asyncOperation.Completed += this.m_ExtractEntryFromTableAction;
			}
			return true;
		}

		private Locale GetNextFallback(Locale currentLocale)
		{
			if (this.m_FallbackQueue == null)
			{
				this.m_FallbackQueue = CollectionPool<List<Locale>, Locale>.Get();
				this.m_HandledFallbacks = CollectionPool<HashSet<Locale>, Locale>.Get();
			}
			if (!this.m_HandledFallbacks.Contains(currentLocale))
			{
				this.m_HandledFallbacks.Add(currentLocale);
			}
			IEnumerable<Locale> fallbacks = currentLocale.GetFallbacks();
			if (fallbacks != null)
			{
				foreach (Locale item in fallbacks)
				{
					if (!this.m_HandledFallbacks.Contains(item))
					{
						this.m_HandledFallbacks.Add(item);
						this.m_FallbackQueue.Add(item);
					}
				}
			}
			if (this.m_FallbackQueue.Count == 0)
			{
				return null;
			}
			Locale result = this.m_FallbackQueue[0];
			this.m_FallbackQueue.RemoveAt(0);
			return result;
		}

		private bool HandleFallback(AsyncOperationHandle<TTable> asyncOperation, TEntry entry)
		{
			if ((entry == null || string.IsNullOrEmpty(entry.Data.Localized)) && this.m_UseFallback)
			{
				Locale nextFallback = this.GetNextFallback(this.m_CurrentLocale);
				if (nextFallback != null)
				{
					this.m_CurrentLocale = nextFallback;
					AddressablesInterface.Release(asyncOperation);
					asyncOperation = this.m_Database.GetTableAsync(this.m_TableReference, this.m_CurrentLocale);
					AddressablesInterface.Acquire(asyncOperation);
					if (asyncOperation.IsDone)
					{
						this.ExtractEntryFromTable(asyncOperation);
					}
					else
					{
						base.CurrentOperation = asyncOperation;
						asyncOperation.Completed += this.m_ExtractEntryFromTableAction;
					}
					return true;
				}
			}
			return false;
		}

		private void CompleteAndRelease(LocalizedDatabase<TTable, TEntry>.TableEntryResult result, bool success, string errorMsg)
		{
			base.Complete(result, success, errorMsg);
			if (this.m_AutoRelease && LocalizationSettings.Instance.IsPlaying)
			{
				LocalizationBehaviour.ReleaseNextFrame(base.Handle);
			}
		}

		protected override void Destroy()
		{
			AddressablesInterface.SafeRelease(this.m_LoadTableOperation);
			this.m_LoadTableOperation = default(AsyncOperationHandle<TTable>);
			base.Destroy();
			GetTableEntryOperation<TTable, TEntry>.Pool.Release(this);
			if (this.m_FallbackQueue != null)
			{
				CollectionPool<List<Locale>, Locale>.Release(this.m_FallbackQueue);
				CollectionPool<HashSet<Locale>, Locale>.Release(this.m_HandledFallbacks);
				this.m_FallbackQueue = null;
				this.m_HandledFallbacks = null;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}, Current Locale: {1}, Selected Locale: {2}, Table: {3}, Entry: {4}, Fallback: {5}", new object[]
			{
				base.GetType().Name,
				this.m_CurrentLocale,
				this.m_SelectedLocale,
				this.m_TableReference,
				this.m_TableEntryReference,
				this.m_UseFallback
			});
		}

		private readonly Action<AsyncOperationHandle<TTable>> m_ExtractEntryFromTableAction;

		private AsyncOperationHandle<TTable> m_LoadTableOperation;

		private TableReference m_TableReference;

		private TableEntryReference m_TableEntryReference;

		private LocalizedDatabase<TTable, TEntry> m_Database;

		private Locale m_SelectedLocale;

		private Locale m_CurrentLocale;

		private HashSet<Locale> m_HandledFallbacks;

		private List<Locale> m_FallbackQueue;

		private bool m_UseFallback;

		private bool m_AutoRelease;

		public static readonly ObjectPool<GetTableEntryOperation<TTable, TEntry>> Pool = new ObjectPool<GetTableEntryOperation<TTable, TEntry>>(() => new GetTableEntryOperation<TTable, TEntry>(), null, null, null, false, 10, 10000);
	}
}
