using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	internal class Changelog
	{
		public ReadOnlyCollection<ChangelogEntry> entries
		{
			get
			{
				return new ReadOnlyCollection<ChangelogEntry>(this.m_Entries);
			}
		}

		public Changelog(string log)
		{
			string version = string.Empty;
			StringBuilder stringBuilder = null;
			this.m_Entries = new List<ChangelogEntry>();
			ChangelogEntry item;
			foreach (string text in log.Split('\n', StringSplitOptions.None))
			{
				if (Regex.Match(text, "(##\\s\\[[0-9]+\\.[0-9]+\\.[0-9]+(\\-[a-zA-Z]+(\\.[0-9]+)*)*\\])").Success)
				{
					if ((item = this.CreateEntry(version, (stringBuilder != null) ? stringBuilder.ToString() : "")) != null)
					{
						this.m_Entries.Add(item);
					}
					version = text;
					stringBuilder = new StringBuilder();
				}
				else if (stringBuilder != null)
				{
					stringBuilder.AppendLine(text);
				}
			}
			if ((item = this.CreateEntry(version, stringBuilder.ToString())) != null)
			{
				this.m_Entries.Add(item);
			}
		}

		private ChangelogEntry CreateEntry(string version, string contents)
		{
			Match match = Regex.Match(version, "(?<=##\\s\\[).*(?=\\])");
			Match match2 = Regex.Match(version, "(?<=##\\s\\[.*\\]\\s-\\s)[0-9-]*");
			if (match.Success)
			{
				return new ChangelogEntry(new SemVer(match.Value, match2.Value), contents.Trim());
			}
			return null;
		}

		private const string k_ChangelogEntryPattern = "(##\\s\\[[0-9]+\\.[0-9]+\\.[0-9]+(\\-[a-zA-Z]+(\\.[0-9]+)*)*\\])";

		private const string k_VersionInfoPattern = "(?<=##\\s\\[).*(?=\\])";

		private const string k_VersionDatePattern = "(?<=##\\s\\[.*\\]\\s-\\s)[0-9-]*";

		[SerializeField]
		private List<ChangelogEntry> m_Entries;
	}
}
