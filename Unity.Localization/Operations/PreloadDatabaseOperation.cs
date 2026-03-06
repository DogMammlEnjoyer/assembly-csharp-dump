using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal class PreloadDatabaseOperation<TTable, TEntry> : WaitForCurrentOperationAsyncOperationBase<LocalizedDatabase<TTable, TEntry>> where TTable : DetailedLocalizationTable<TEntry> where TEntry : TableEntry
	{
		protected override float Progress
		{
			get
			{
				if (!base.CurrentOperation.IsValid())
				{
					return base.Progress;
				}
				return base.CurrentOperation.PercentComplete;
			}
		}

		protected override string DebugName
		{
			get
			{
				return string.Format("Preload {0}", this.m_Database.GetType());
			}
		}

		public PreloadDatabaseOperation()
		{
			this.m_CompleteOperation = new Action<AsyncOperationHandle>(this.CompleteOperation);
			this.m_CompleteGenericGroup = new Action<AsyncOperationHandle<IList<AsyncOperationHandle>>>(this.CompleteGenericGroup);
		}

		public void Init(LocalizedDatabase<TTable, TEntry> database)
		{
			this.m_Database = database;
		}

		protected override void Execute()
		{
			AsyncOperationHandle<Locale> selectedLocaleAsync = LocalizationSettings.SelectedLocaleAsync;
			if (selectedLocaleAsync.Result == null)
			{
				base.Complete(this.m_Database, true, null);
				return;
			}
			switch (LocalizationSettings.PreloadBehavior)
			{
			case PreloadBehavior.NoPreloading:
				base.Complete(this.m_Database, true, null);
				return;
			case PreloadBehavior.PreloadSelectedLocale:
			{
				AsyncOperationHandle asyncOperationHandle = this.PreloadLocale(selectedLocaleAsync.Result);
				if (asyncOperationHandle.IsDone)
				{
					this.m_CompleteOperation(asyncOperationHandle);
					return;
				}
				asyncOperationHandle.Completed += this.m_CompleteOperation;
				base.CurrentOperation = asyncOperationHandle;
				return;
			}
			case PreloadBehavior.PreloadSelectedLocaleAndFallbacks:
			{
				HashSet<Locale> hashSet;
				using (CollectionPool<HashSet<Locale>, Locale>.Get(out hashSet))
				{
					hashSet.Add(selectedLocaleAsync.Result);
					this.GetAllFallbackLocales(selectedLocaleAsync.Result, hashSet);
					this.PreloadLocales(hashSet);
					return;
				}
				break;
			}
			case PreloadBehavior.PreloadAllLocales:
				break;
			default:
				return;
			}
			this.PreloadLocales(LocalizationSettings.AvailableLocales.Locales);
		}

		private void GetAllFallbackLocales(Locale current, HashSet<Locale> locales)
		{
			foreach (Locale locale in current.GetFallbacks())
			{
				if (!locales.Contains(locale))
				{
					locales.Add(locale);
					this.GetAllFallbackLocales(locale, locales);
				}
			}
		}

		private AsyncOperationHandle PreloadLocale(Locale locale)
		{
			PreloadLocaleOperation<TTable, TEntry> preloadLocaleOperation = PreloadLocaleOperation<TTable, TEntry>.Pool.Get();
			preloadLocaleOperation.Init(this.m_Database, locale);
			return AddressablesInterface.ResourceManager.StartOperation<LocalizedDatabase<TTable, TEntry>>(preloadLocaleOperation, default(AsyncOperationHandle));
		}

		private void PreloadLocales(ICollection<Locale> locales)
		{
			List<AsyncOperationHandle> list;
			using (CollectionPool<List<AsyncOperationHandle>, AsyncOperationHandle>.Get(out list))
			{
				foreach (Locale locale in locales)
				{
					AsyncOperationHandle item = this.PreloadLocale(locale);
					if (!item.IsDone)
					{
						list.Add(item);
					}
				}
				if (list.Count > 0)
				{
					AsyncOperationHandle<IList<AsyncOperationHandle>> obj = AddressablesInterface.CreateGroupOperation(list);
					obj.Completed += this.m_CompleteGenericGroup;
					base.CurrentOperation = obj;
				}
				else
				{
					base.Complete(this.m_Database, true, null);
				}
			}
		}

		private void CompleteOperation(AsyncOperationHandle operationHandle)
		{
			AddressablesInterface.Release(operationHandle);
			base.Complete(this.m_Database, true, null);
		}

		private void CompleteGenericGroup(AsyncOperationHandle<IList<AsyncOperationHandle>> operationHandle)
		{
			AddressablesInterface.Release(operationHandle);
			base.Complete(this.m_Database, true, null);
		}

		protected override void Destroy()
		{
			base.Destroy();
			PreloadDatabaseOperation<TTable, TEntry>.Pool.Release(this);
		}

		private readonly Action<AsyncOperationHandle> m_CompleteOperation;

		private readonly Action<AsyncOperationHandle<IList<AsyncOperationHandle>>> m_CompleteGenericGroup;

		private LocalizedDatabase<TTable, TEntry> m_Database;

		public static readonly ObjectPool<PreloadDatabaseOperation<TTable, TEntry>> Pool = new ObjectPool<PreloadDatabaseOperation<TTable, TEntry>>(() => new PreloadDatabaseOperation<TTable, TEntry>(), null, null, null, false, 10, 10000);
	}
}
