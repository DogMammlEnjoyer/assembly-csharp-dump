using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class IsMatchFormatter : FormatterBase, IFormatterLiteralExtractor
	{
		public IsMatchFormatter()
		{
			base.Names = this.DefaultNames;
		}

		public override string[] DefaultNames
		{
			get
			{
				return new string[]
				{
					"ismatch"
				};
			}
		}

		public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			string formatterOptions = formattingInfo.FormatterOptions;
			IList<Format> list = formattingInfo.Format.Split('|');
			if (list.Count == 0)
			{
				return true;
			}
			if (list.Count != 2)
			{
				throw new FormatException("Exactly 2 format options are required.");
			}
			if (new Regex(formatterOptions, this.RegexOptions).IsMatch(formattingInfo.CurrentValue.ToString()))
			{
				formattingInfo.Write(list[0], formattingInfo.CurrentValue);
			}
			else if (list.Count == 2)
			{
				formattingInfo.Write(list[1], formattingInfo.CurrentValue);
			}
			return true;
		}

		public void WriteAllLiterals(IFormattingInfo formattingInfo)
		{
			IList<Format> list = formattingInfo.Format.Split('|');
			if (list.Count == 0)
			{
				return;
			}
			if (list.Count != 2)
			{
				return;
			}
			formattingInfo.Write(list[0], formattingInfo.CurrentValue);
			formattingInfo.Write(list[1], formattingInfo.CurrentValue);
		}

		public RegexOptions RegexOptions { get; set; }
	}
}
