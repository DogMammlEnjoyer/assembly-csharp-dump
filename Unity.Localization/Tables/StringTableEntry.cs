using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Pseudo;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.Localization.SmartFormat.Core.Formatting;

namespace UnityEngine.Localization.Tables
{
	public class StringTableEntry : TableEntry
	{
		public FormatCache FormatCache
		{
			get
			{
				return this.m_FormatCache;
			}
			set
			{
				this.m_FormatCache = value;
			}
		}

		public string Value
		{
			get
			{
				return base.Data.Localized;
			}
			set
			{
				base.Data.Localized = value;
				if (this.m_FormatCache != null)
				{
					FormatCachePool.Release(this.m_FormatCache);
					this.m_FormatCache = null;
				}
			}
		}

		public bool IsSmart
		{
			get
			{
				return base.HasTagMetadata<SmartFormatTag>() || base.Data.Metadata.GetMetadata<SmartFormatTag>() != null;
			}
			set
			{
				if (value)
				{
					if (this.m_FormatCache != null)
					{
						FormatCachePool.Release(this.m_FormatCache);
						this.m_FormatCache = null;
					}
					base.AddTagMetadata<SmartFormatTag>();
					return;
				}
				base.RemoveTagMetadata<SmartFormatTag>();
			}
		}

		internal StringTableEntry()
		{
		}

		public void RemoveFromTable()
		{
			StringTable stringTable = base.Table as StringTable;
			if (stringTable == null)
			{
				Debug.LogWarning(string.Format("Failed to remove {0} with id {1} and value `{2}` as it does not belong to a table.", "StringTableEntry", base.KeyId, this.Value));
				return;
			}
			stringTable.Remove(base.KeyId);
		}

		internal FormatCache GetOrCreateFormatCache()
		{
			if (!this.IsSmart)
			{
				return null;
			}
			if (this.m_FormatCache == null && !string.IsNullOrEmpty(base.Data.Localized))
			{
				this.m_FormatCache = FormatCachePool.Get(LocalizationSettings.StringDatabase.SmartFormatter.Parser.ParseFormat(base.Data.Localized, LocalizationSettings.StringDatabase.SmartFormatter.GetNotEmptyFormatterExtensionNames()));
				this.m_FormatCache.Table = base.Table;
			}
			return this.m_FormatCache;
		}

		public string GetLocalizedString()
		{
			return this.GetLocalizedString(null, null, LocalizationSettings.SelectedLocaleAsync.Result as PseudoLocale);
		}

		public string GetLocalizedString(params object[] args)
		{
			return this.GetLocalizedString(null, args, LocalizationSettings.SelectedLocaleAsync.Result as PseudoLocale);
		}

		public string GetLocalizedString(IList<object> args)
		{
			return this.GetLocalizedString(null, args, LocalizationSettings.SelectedLocaleAsync.Result as PseudoLocale);
		}

		public string GetLocalizedString(IFormatProvider formatProvider, IList<object> args)
		{
			return this.GetLocalizedString(formatProvider, args, LocalizationSettings.SelectedLocaleAsync.Result as PseudoLocale);
		}

		public string GetLocalizedString(IFormatProvider formatProvider, IList<object> args, PseudoLocale pseudoLocale)
		{
			if (formatProvider == null)
			{
				ILocalesProvider availableLocales = LocalizationSettings.AvailableLocales;
				formatProvider = ((availableLocales != null) ? availableLocales.GetLocale(base.Table.LocaleIdentifier) : null);
			}
			string text = null;
			if (this.IsSmart)
			{
				if (this.m_FormatCache == null)
				{
					this.m_FormatCache = this.GetOrCreateFormatCache();
				}
				text = LocalizationSettings.StringDatabase.SmartFormatter.FormatWithCache(ref this.m_FormatCache, base.Data.Localized, formatProvider, args);
			}
			else if (!string.IsNullOrEmpty(base.Data.Localized))
			{
				if (args != null && args.Count > 0)
				{
					try
					{
						text = ((formatProvider == null) ? string.Format(base.Data.Localized, (args as object[]) ?? args.ToArray<object>()) : string.Format(formatProvider, base.Data.Localized, (args as object[]) ?? args.ToArray<object>()));
						goto IL_F8;
					}
					catch (FormatException ex)
					{
						throw new FormatException(string.Format("Input string was not in the correct format for String.Format. Ensure that the string is marked as Smart if you intended to use Smart Format.\n`{0}`\n{1}", base.Data.Localized, ex), ex);
					}
				}
				text = base.Data.Localized;
			}
			IL_F8:
			if (pseudoLocale != null && !string.IsNullOrEmpty(text))
			{
				text = pseudoLocale.GetPseudoString(text);
			}
			return text;
		}

		private FormatCache m_FormatCache;
	}
}
