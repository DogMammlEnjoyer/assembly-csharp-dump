using System;
using System.Collections.Generic;
using UnityEngine.Localization.Operations;
using UnityEngine.Localization.Pseudo;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Settings
{
	[Serializable]
	public class LocalizedStringDatabase : LocalizedDatabase<StringTable, StringTableEntry>
	{
		public event LocalizedStringDatabase.MissingTranslation TranslationNotFound;

		public string NoTranslationFoundMessage
		{
			get
			{
				return this.m_NoTranslationFoundMessage;
			}
			set
			{
				this.m_NoTranslationFoundMessage = value;
			}
		}

		public MissingTranslationBehavior MissingTranslationState
		{
			get
			{
				return this.m_MissingTranslationState;
			}
			set
			{
				this.m_MissingTranslationState = value;
			}
		}

		public SmartFormatter SmartFormatter
		{
			get
			{
				return this.m_SmartFormat;
			}
			set
			{
				this.m_SmartFormat = value;
			}
		}

		public AsyncOperationHandle<string> GetLocalizedStringAsync(TableEntryReference tableEntryReference, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings, params object[] arguments)
		{
			return this.GetLocalizedStringAsyncInternal(base.GetDefaultTable(), tableEntryReference, arguments, locale, fallbackBehavior, null, true);
		}

		public string GetLocalizedString(TableEntryReference tableEntryReference, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings, params object[] arguments)
		{
			return this.GetLocalizedString(base.GetDefaultTable(), tableEntryReference, arguments, locale, fallbackBehavior);
		}

		public AsyncOperationHandle<string> GetLocalizedStringAsync(TableEntryReference tableEntryReference, IList<object> arguments, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings)
		{
			return this.GetLocalizedStringAsyncInternal(base.GetDefaultTable(), tableEntryReference, arguments, locale, fallbackBehavior, null, true);
		}

		public string GetLocalizedString(TableEntryReference tableEntryReference, IList<object> arguments, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings)
		{
			return this.GetLocalizedString(base.GetDefaultTable(), tableEntryReference, arguments, locale, fallbackBehavior);
		}

		public virtual AsyncOperationHandle<string> GetLocalizedStringAsync(TableReference tableReference, TableEntryReference tableEntryReference, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings, params object[] arguments)
		{
			return this.GetLocalizedStringAsyncInternal(tableReference, tableEntryReference, arguments, locale, fallbackBehavior, null, true);
		}

		public virtual string GetLocalizedString(TableReference tableReference, TableEntryReference tableEntryReference, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings, params object[] arguments)
		{
			return this.GetLocalizedString(tableReference, tableEntryReference, arguments, locale, fallbackBehavior);
		}

		public virtual AsyncOperationHandle<string> GetLocalizedStringAsync(TableReference tableReference, TableEntryReference tableEntryReference, IList<object> arguments, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings, IVariableGroup localVariables = null)
		{
			return this.GetLocalizedStringAsyncInternal(tableReference, tableEntryReference, arguments, locale, fallbackBehavior, localVariables, true);
		}

		internal virtual AsyncOperationHandle<string> GetLocalizedStringAsyncInternal(TableReference tableReference, TableEntryReference tableEntryReference, IList<object> arguments, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings, IVariableGroup localVariables = null, bool autoRelease = true)
		{
			AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult> tableEntryAsync = this.GetTableEntryAsync(tableReference, tableEntryReference, locale, fallbackBehavior);
			GetLocalizedStringOperation getLocalizedStringOperation = GetLocalizedStringOperation.Pool.Get();
			getLocalizedStringOperation.Dependency = tableEntryAsync;
			getLocalizedStringOperation.Init(tableEntryAsync, locale, this, tableReference, tableEntryReference, arguments, localVariables, autoRelease);
			return AddressablesInterface.ResourceManager.StartOperation<string>(getLocalizedStringOperation, tableEntryAsync);
		}

		public virtual string GetLocalizedString(TableReference tableReference, TableEntryReference tableEntryReference, IList<object> arguments, Locale locale = null, FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings)
		{
			AsyncOperationHandle<string> localizedStringAsyncInternal = this.GetLocalizedStringAsyncInternal(tableReference, tableEntryReference, arguments, locale, fallbackBehavior, null, false);
			string result = localizedStringAsyncInternal.WaitForCompletion();
			AddressablesInterface.Release(localizedStringAsyncInternal);
			return result;
		}

		protected internal virtual string GenerateLocalizedString(StringTable table, StringTableEntry entry, TableReference tableReference, TableEntryReference tableEntryReference, Locale locale, IList<object> arguments)
		{
			string text = (entry != null) ? entry.GetLocalizedString(locale, arguments, locale as PseudoLocale) : null;
			if (string.IsNullOrEmpty(text))
			{
				SharedTableData sharedTableData = (table != null) ? table.SharedData : null;
				if (sharedTableData == null && tableReference.ReferenceType == TableReference.Type.Guid)
				{
					AsyncOperationHandle<SharedTableData> sharedTableData2 = base.GetSharedTableData(tableReference.TableCollectionNameGuid);
					if (sharedTableData2.IsDone)
					{
						sharedTableData = sharedTableData2.Result;
					}
				}
				string key = tableEntryReference.ResolveKeyName(sharedTableData);
				return this.ProcessUntranslatedText(key, tableEntryReference.KeyId, tableReference, table, locale);
			}
			return text;
		}

		private StringTable GetUntranslatedTextTempTable(TableReference tableReference)
		{
			if (this.m_MissingTranslationTable == null)
			{
				this.m_MissingTranslationTable = ScriptableObject.CreateInstance<StringTable>();
				this.m_MissingTranslationTable.SharedData = ScriptableObject.CreateInstance<SharedTableData>();
			}
			if (tableReference.ReferenceType == TableReference.Type.Guid)
			{
				this.m_MissingTranslationTable.SharedData.TableCollectionNameGuid = tableReference;
				AsyncOperationHandle<SharedTableData> sharedTableData = base.GetSharedTableData(tableReference.TableCollectionNameGuid);
				if (sharedTableData.IsDone && sharedTableData.Result != null)
				{
					this.m_MissingTranslationTable.SharedData.TableCollectionName = sharedTableData.Result.TableCollectionName;
				}
				else
				{
					this.m_MissingTranslationTable.SharedData.TableCollectionName = tableReference.TableCollectionNameGuid.ToString();
				}
			}
			else if (tableReference.ReferenceType == TableReference.Type.Name)
			{
				this.m_MissingTranslationTable.SharedData.TableCollectionName = tableReference.TableCollectionName;
				this.m_MissingTranslationTable.SharedData.TableCollectionNameGuid = Guid.Empty;
			}
			return this.m_MissingTranslationTable;
		}

		internal string ProcessUntranslatedText(string key, long keyId, TableReference tableReference, StringTable table, Locale locale)
		{
			if (table == null)
			{
				table = this.GetUntranslatedTextTempTable(tableReference);
			}
			if (this.MissingTranslationState != (MissingTranslationBehavior)0 || this.TranslationNotFound != null)
			{
				Dictionary<string, object> dictionary;
				using (CollectionPool<Dictionary<string, object>, KeyValuePair<string, object>>.Get(out dictionary))
				{
					dictionary["key"] = key;
					dictionary["keyId"] = keyId;
					dictionary["table"] = table;
					dictionary["locale"] = locale;
					string text = this.m_SmartFormat.Format(string.IsNullOrEmpty(this.NoTranslationFoundMessage) ? "No translation found for '{key}' in {table.TableCollectionName}" : this.NoTranslationFoundMessage, new object[]
					{
						dictionary
					});
					LocalizedStringDatabase.MissingTranslation translationNotFound = this.TranslationNotFound;
					if (translationNotFound != null)
					{
						translationNotFound(key, keyId, tableReference, table, locale, text);
					}
					if (this.MissingTranslationState.HasFlag(MissingTranslationBehavior.PrintWarning))
					{
						Debug.LogWarning(text);
					}
					if (this.MissingTranslationState.HasFlag(MissingTranslationBehavior.ShowMissingTranslationMessage))
					{
						return text;
					}
				}
			}
			return string.Empty;
		}

		[SerializeField]
		private MissingTranslationBehavior m_MissingTranslationState = MissingTranslationBehavior.ShowMissingTranslationMessage;

		private const string k_DefaultNoTranslationMessage = "No translation found for '{key}' in {table.TableCollectionName}";

		[SerializeField]
		[Tooltip("The string that will be used when a localized value is missing. This is a Smart String which has access to the following placeholders:\n\t{key}: The name of the key\n\t{keyId}: The numeric Id of the key\n\t{table}: The table object, this can be further queried, for example {table.TableCollectionName}\n\t{locale}: The locale asset, this can be further queried, for example {locale.name}")]
		private string m_NoTranslationFoundMessage = "No translation found for '{key}' in {table.TableCollectionName}";

		[SerializeReference]
		private SmartFormatter m_SmartFormat = Smart.CreateDefaultSmartFormat();

		private StringTable m_MissingTranslationTable;

		public delegate void MissingTranslation(string key, long keyId, TableReference tableReference, StringTable table, Locale locale, string noTranslationFoundMessage);
	}
}
