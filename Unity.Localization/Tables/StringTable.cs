using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat;

namespace UnityEngine.Localization.Tables
{
	public class StringTable : DetailedLocalizationTable<StringTableEntry>
	{
		public string GenerateCharacterSet()
		{
			return string.Concat<char>(from c in this.CollectLiteralCharacters().Distinct<char>()
			orderby c
			select c);
		}

		internal IEnumerable<char> CollectLiteralCharacters()
		{
			IEnumerable<char> enumerable = "";
			LocalizedStringDatabase stringDatabase = LocalizationSettings.StringDatabase;
			SmartFormatterLiteralCharacterExtractor smartFormatterLiteralCharacterExtractor = new SmartFormatterLiteralCharacterExtractor((stringDatabase != null) ? stringDatabase.SmartFormatter : null);
			foreach (StringTableEntry stringTableEntry in base.Values)
			{
				if (stringTableEntry.IsSmart)
				{
					enumerable = enumerable.Concat(smartFormatterLiteralCharacterExtractor.ExtractLiteralsCharacters(stringTableEntry.LocalizedValue));
				}
				else
				{
					enumerable = enumerable.Concat(stringTableEntry.LocalizedValue.AsEnumerable<char>());
				}
			}
			return enumerable;
		}

		public override StringTableEntry CreateTableEntry()
		{
			return new StringTableEntry
			{
				Table = this,
				Data = new TableEntryData()
			};
		}
	}
}
