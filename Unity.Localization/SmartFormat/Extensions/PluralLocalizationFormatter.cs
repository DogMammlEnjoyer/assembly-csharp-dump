using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class PluralLocalizationFormatter : FormatterBase, IFormatterLiteralExtractor
	{
		public string DefaultTwoLetterISOLanguageName
		{
			get
			{
				return this.m_DefaultTwoLetterISOLanguageName;
			}
			set
			{
				this.m_DefaultTwoLetterISOLanguageName = value;
				this.m_DefaultPluralRule = PluralRules.GetPluralRule(value);
			}
		}

		public PluralLocalizationFormatter()
		{
			base.Names = this.DefaultNames;
		}

		public override string[] DefaultNames
		{
			get
			{
				return new string[]
				{
					"plural",
					"p",
					""
				};
			}
		}

		public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			Format format = formattingInfo.Format;
			object currentValue = formattingInfo.CurrentValue;
			if (format == null || format.baseString[format.startIndex] == ':')
			{
				return false;
			}
			IList<Format> list = format.Split('|');
			if (list.Count == 1)
			{
				return false;
			}
			IConvertible convertible = currentValue as IConvertible;
			decimal value;
			if (convertible != null && !(currentValue is DateTime) && !(currentValue is string) && !(currentValue is bool) && !(currentValue is Enum))
			{
				value = convertible.ToDecimal(null);
			}
			else
			{
				IEnumerable<object> enumerable = currentValue as IEnumerable<object>;
				if (enumerable == null)
				{
					return false;
				}
				value = enumerable.Count<object>();
			}
			PluralRules.PluralRuleDelegate pluralRule = this.GetPluralRule(formattingInfo);
			if (pluralRule == null)
			{
				return false;
			}
			int count = list.Count;
			int num = pluralRule(value, count);
			if (num < 0 || list.Count <= num)
			{
				throw new FormattingException(format, "Invalid number of plural parameters", list.Last<Format>().endIndex);
			}
			Format format2 = list[num];
			formattingInfo.Write(format2, currentValue);
			return true;
		}

		protected virtual PluralRules.PluralRuleDelegate GetPluralRule(IFormattingInfo formattingInfo)
		{
			string formatterOptions = formattingInfo.FormatterOptions;
			if (formatterOptions.Length != 0)
			{
				return PluralRules.GetPluralRule(formatterOptions);
			}
			IFormatProvider formatProvider = formattingInfo.FormatDetails.Provider;
			CustomPluralRuleProvider customPluralRuleProvider = (CustomPluralRuleProvider)((formatProvider != null) ? formatProvider.GetFormat(typeof(CustomPluralRuleProvider)) : null);
			if (customPluralRuleProvider != null)
			{
				return customPluralRuleProvider.GetPluralRule();
			}
			Locale locale = formatProvider as Locale;
			if (locale != null)
			{
				formatProvider = locale.Identifier.CultureInfo;
			}
			CultureInfo cultureInfo = formatProvider as CultureInfo;
			if (cultureInfo != null)
			{
				return PluralRules.GetPluralRule(cultureInfo.TwoLetterISOLanguageName);
			}
			Locale locale2 = null;
			AsyncOperationHandle<Locale> selectedLocaleAsync = LocalizationSettings.SelectedLocaleAsync;
			if (selectedLocaleAsync.IsValid() && selectedLocaleAsync.IsDone)
			{
				locale2 = selectedLocaleAsync.Result;
			}
			if (locale2 != null)
			{
				CultureInfo cultureInfo2 = locale2.Identifier.CultureInfo;
				string twoLetterIsoLanguageName;
				if (cultureInfo2 != null)
				{
					twoLetterIsoLanguageName = cultureInfo2.TwoLetterISOLanguageName;
				}
				else
				{
					twoLetterIsoLanguageName = locale2.Identifier.Code;
					if (locale2.Identifier.Code.Length > 2)
					{
						twoLetterIsoLanguageName = locale2.Identifier.Code.Substring(0, 2);
					}
				}
				return PluralRules.GetPluralRule(twoLetterIsoLanguageName);
			}
			PluralRules.PluralRuleDelegate result;
			if ((result = this.m_DefaultPluralRule) == null)
			{
				result = (this.m_DefaultPluralRule = PluralRules.GetPluralRule(this.DefaultTwoLetterISOLanguageName));
			}
			return result;
		}

		public void WriteAllLiterals(IFormattingInfo formattingInfo)
		{
			Format format = formattingInfo.Format;
			if (format == null || format.baseString[format.startIndex] == ':')
			{
				return;
			}
			IList<Format> list = format.Split('|');
			if (list.Count == 1)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				formattingInfo.Write(list[i], null);
			}
		}

		[SerializeField]
		private string m_DefaultTwoLetterISOLanguageName = "en";

		private PluralRules.PluralRuleDelegate m_DefaultPluralRule;
	}
}
