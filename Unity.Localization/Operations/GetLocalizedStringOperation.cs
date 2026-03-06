using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal class GetLocalizedStringOperation : WaitForCurrentOperationAsyncOperationBase<string>
	{
		public void Init(AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult> tableEntryOperation, Locale locale, LocalizedStringDatabase database, TableReference tableReference, TableEntryReference tableEntryReference, IList<object> arguments, IVariableGroup localVariables, bool autoRelease)
		{
			this.m_TableEntryOperation = tableEntryOperation;
			this.m_SelectedLocale = locale;
			AddressablesInterface.Acquire(this.m_TableEntryOperation);
			this.m_Database = database;
			this.m_TableReference = tableReference;
			this.m_TableEntryReference = tableEntryReference;
			this.m_Arguments = arguments;
			this.m_LocalVariables = localVariables;
			this.m_AutoRelease = autoRelease;
		}

		protected override void Execute()
		{
			if (this.m_SelectedLocale == null)
			{
				this.m_SelectedLocale = LocalizationSettings.SelectedLocaleAsync.Result;
				if (this.m_SelectedLocale == null)
				{
					this.CompleteAndRelease(null, false, "SelectedLocale is null. Could not get localized string.");
					return;
				}
			}
			if (this.m_TableEntryOperation.Status != AsyncOperationStatus.Succeeded)
			{
				this.CompleteAndRelease(null, false, "Load Table Operation Failed");
				return;
			}
			try
			{
				StringTableEntry entry = this.m_TableEntryOperation.Result.Entry;
				FormatCache formatCache = (entry != null) ? entry.GetOrCreateFormatCache() : null;
				if (formatCache != null)
				{
					formatCache.LocalVariables = this.m_LocalVariables;
				}
				string result = this.m_Database.GenerateLocalizedString(this.m_TableEntryOperation.Result.Table, entry, this.m_TableReference, this.m_TableEntryReference, this.m_SelectedLocale, this.m_Arguments);
				if (formatCache != null)
				{
					formatCache.LocalVariables = null;
				}
				this.CompleteAndRelease(result, true, null);
			}
			catch (Exception ex)
			{
				this.CompleteAndRelease(null, false, ex.Message);
			}
		}

		public void CompleteAndRelease(string result, bool success, string errorMsg)
		{
			base.Complete(result, success, errorMsg);
			AddressablesInterface.SafeRelease(this.m_TableEntryOperation);
			if (this.m_AutoRelease && LocalizationSettings.Instance.IsPlaying)
			{
				LocalizationBehaviour.ReleaseNextFrame(base.Handle);
			}
		}

		protected override void Destroy()
		{
			base.Destroy();
			GetLocalizedStringOperation.Pool.Release(this);
		}

		public override string ToString()
		{
			return string.Format("{0}, Locale: {1}, Table: {2}, Entry: {3}", new object[]
			{
				base.GetType().Name,
				this.m_SelectedLocale,
				this.m_TableReference,
				this.m_TableEntryReference
			});
		}

		private LocalizedStringDatabase m_Database;

		private AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult> m_TableEntryOperation;

		private TableReference m_TableReference;

		private TableEntryReference m_TableEntryReference;

		private Locale m_SelectedLocale;

		private IList<object> m_Arguments;

		private IVariableGroup m_LocalVariables;

		private bool m_AutoRelease;

		public static readonly ObjectPool<GetLocalizedStringOperation> Pool = new ObjectPool<GetLocalizedStringOperation>(() => new GetLocalizedStringOperation(), null, null, null, false, 10, 10000);
	}
}
