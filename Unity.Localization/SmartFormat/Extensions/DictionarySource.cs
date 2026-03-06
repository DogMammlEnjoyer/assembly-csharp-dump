using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class DictionarySource : ISource
	{
		public DictionarySource(SmartFormatter formatter)
		{
			formatter.Parser.AddAlphanumericSelectors();
			formatter.Parser.AddAdditionalSelectorChars("_");
			formatter.Parser.AddOperators(".");
		}

		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
		{
			object currentValue = selectorInfo.CurrentValue;
			string selector = selectorInfo.SelectorText;
			IDictionary dictionary = currentValue as IDictionary;
			if (dictionary != null)
			{
				foreach (object obj in dictionary)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (((dictionaryEntry.Key as string) ?? dictionaryEntry.Key.ToString()).Equals(selector, selectorInfo.FormatDetails.Settings.GetCaseSensitivityComparison()))
					{
						selectorInfo.Result = dictionaryEntry.Value;
						return true;
					}
				}
			}
			IDictionary<string, object> dictionary2 = currentValue as IDictionary<string, object>;
			if (dictionary2 != null)
			{
				object value = dictionary2.FirstOrDefault((KeyValuePair<string, object> x) => x.Key.Equals(selector, selectorInfo.FormatDetails.Settings.GetCaseSensitivityComparison())).Value;
				if (value != null)
				{
					selectorInfo.Result = value;
					return true;
				}
			}
			return false;
		}
	}
}
