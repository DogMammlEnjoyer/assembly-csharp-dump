using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.WitAi.Data.Info
{
	[Serializable]
	public struct WitEntityKeywordInfo
	{
		public WitEntityKeywordInfo(string keyword, List<string> synonyms = null)
		{
			this.keyword = keyword;
			this.synonyms = (synonyms ?? new List<string>());
		}

		public override bool Equals(object obj)
		{
			if (obj is WitEntityKeywordInfo)
			{
				WitEntityKeywordInfo other = (WitEntityKeywordInfo)obj;
				return this.Equals(other);
			}
			return false;
		}

		public bool Equals(WitEntityKeywordInfo other)
		{
			return this.keyword == other.keyword && this.synonyms.Equivalent(other.synonyms);
		}

		public override int GetHashCode()
		{
			return (17 * 31 + this.keyword.GetHashCode()) * 31 + this.synonyms.GetHashCode();
		}

		public string keyword;

		[NonReorderable]
		public List<string> synonyms;
	}
}
