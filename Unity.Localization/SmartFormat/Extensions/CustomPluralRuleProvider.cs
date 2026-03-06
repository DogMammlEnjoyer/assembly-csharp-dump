using System;
using UnityEngine.Localization.SmartFormat.Utilities;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	public class CustomPluralRuleProvider : IFormatProvider
	{
		public CustomPluralRuleProvider(PluralRules.PluralRuleDelegate pluralRule)
		{
			this._pluralRule = pluralRule;
		}

		public object GetFormat(Type formatType)
		{
			if (!(formatType == typeof(CustomPluralRuleProvider)))
			{
				return null;
			}
			return this;
		}

		public PluralRules.PluralRuleDelegate GetPluralRule()
		{
			return this._pluralRule;
		}

		private readonly PluralRules.PluralRuleDelegate _pluralRule;
	}
}
