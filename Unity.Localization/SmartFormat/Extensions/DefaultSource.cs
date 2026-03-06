using System;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Formatting;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class DefaultSource : ISource
	{
		public DefaultSource(SmartFormatter formatter)
		{
			formatter.Parser.AddOperators(",");
			formatter.Parser.AddAdditionalSelectorChars("-");
		}

		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
		{
			object currentValue = selectorInfo.CurrentValue;
			string selectorText = selectorInfo.SelectorText;
			FormatDetails formatDetails = selectorInfo.FormatDetails;
			int num;
			if (int.TryParse(selectorText, out num))
			{
				if (selectorInfo.SelectorIndex == 0 && num < formatDetails.OriginalArgs.Count && selectorInfo.SelectorOperator == "")
				{
					selectorInfo.Result = formatDetails.OriginalArgs[num];
					return true;
				}
				if (selectorInfo.SelectorOperator == ",")
				{
					if (selectorInfo.Placeholder != null)
					{
						selectorInfo.Placeholder.Alignment = num;
					}
					selectorInfo.Result = currentValue;
					return true;
				}
			}
			return false;
		}
	}
}
