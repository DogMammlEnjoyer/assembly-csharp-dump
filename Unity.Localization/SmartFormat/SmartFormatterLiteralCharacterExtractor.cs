using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat
{
	internal class SmartFormatterLiteralCharacterExtractor : SmartFormatter
	{
		public SmartFormatterLiteralCharacterExtractor(SmartFormatter parent)
		{
			base.Settings = parent.Settings;
			base.Parser = parent.Parser;
			base.SourceExtensions.AddRange(parent.SourceExtensions);
			base.FormatterExtensions.AddRange(parent.FormatterExtensions);
		}

		public IEnumerable<char> ExtractLiteralsCharacters(string value)
		{
			this.m_Characters = "";
			base.Format(value, null);
			return this.m_Characters;
		}

		public override void Format(FormattingInfo formattingInfo)
		{
			foreach (FormatItem formatItem in formattingInfo.Format.Items)
			{
				if (formatItem is LiteralText)
				{
					this.m_Characters = this.m_Characters.Concat(formatItem.ToEnumerable());
				}
				else
				{
					Placeholder placeholder = (Placeholder)formatItem;
					FormattingInfo formattingInfo2 = formattingInfo.CreateChild(placeholder);
					string formatterName = formattingInfo2.Placeholder.FormatterName;
					foreach (IFormatter formatter in base.FormatterExtensions)
					{
						IFormatterLiteralExtractor formatterLiteralExtractor = formatter as IFormatterLiteralExtractor;
						if (formatterLiteralExtractor != null && formatter.Names.Contains(formatterName))
						{
							formatterLiteralExtractor.WriteAllLiterals(formattingInfo2);
						}
					}
				}
			}
		}

		private IEnumerable<char> m_Characters;
	}
}
