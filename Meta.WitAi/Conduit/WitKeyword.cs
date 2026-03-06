using System;
using System.Collections.Generic;
using System.Linq;
using Meta.WitAi.Data.Info;
using UnityEngine.Scripting;

namespace Meta.Conduit
{
	public class WitKeyword
	{
		[Preserve]
		public WitKeyword() : this("", null)
		{
		}

		public WitKeyword(string keyword, List<string> synonyms = null)
		{
			this.keyword = keyword;
			this.synonyms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (synonyms == null)
			{
				return;
			}
			foreach (string item in synonyms)
			{
				if (!this.synonyms.Contains(item))
				{
					this.synonyms.Add(item);
				}
			}
		}

		public WitKeyword(WitEntityKeywordInfo witEntityKeywordInfo) : this(witEntityKeywordInfo.keyword, witEntityKeywordInfo.synonyms)
		{
		}

		public WitEntityKeywordInfo GetAsInfo()
		{
			return new WitEntityKeywordInfo
			{
				keyword = this.keyword,
				synonyms = this.synonyms.ToList<string>()
			};
		}

		public override bool Equals(object obj)
		{
			WitKeyword witKeyword = obj as WitKeyword;
			return witKeyword != null && this.Equals(witKeyword);
		}

		private bool Equals(WitKeyword other)
		{
			return this.keyword.Equals(other.keyword) && this.synonyms.SequenceEqual(other.synonyms);
		}

		public override int GetHashCode()
		{
			return (17 * 31 + this.keyword.GetHashCode()) * 31 + this.synonyms.GetHashCode();
		}

		public readonly string keyword;

		public readonly HashSet<string> synonyms;
	}
}
