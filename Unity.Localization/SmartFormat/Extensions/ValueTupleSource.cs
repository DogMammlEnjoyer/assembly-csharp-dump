using System;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.Utilities;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class ValueTupleSource : ISource
	{
		public ValueTupleSource(SmartFormatter formatter)
		{
			this.m_Formatter = formatter;
		}

		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
		{
			FormattingInfo formattingInfo = selectorInfo as FormattingInfo;
			if (formattingInfo == null)
			{
				return false;
			}
			if (formattingInfo.CurrentValue == null || !formattingInfo.CurrentValue.IsValueTuple())
			{
				return false;
			}
			object currentValue = formattingInfo.CurrentValue;
			foreach (object currentValue2 in formattingInfo.CurrentValue.GetValueTupleItemObjectsFlattened())
			{
				if (this.m_Formatter == null)
				{
					SmartFormatter smartFormatter = LocalizationSettings.StringDatabase.SmartFormatter;
				}
				foreach (ISource source in this.m_Formatter.SourceExtensions)
				{
					formattingInfo.CurrentValue = currentValue2;
					if (source.TryEvaluateSelector(formattingInfo))
					{
						formattingInfo.CurrentValue = currentValue;
						return true;
					}
				}
			}
			formattingInfo.CurrentValue = currentValue;
			return false;
		}

		private SmartFormatter m_Formatter;
	}
}
