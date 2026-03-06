using System;
using System.Collections.Generic;
using System.Linq;
using Modio.API;
using Modio.Users;

namespace Modio.Mods
{
	[Serializable]
	public class ModSearchFilter
	{
		public int PageIndex
		{
			get
			{
				return this._pageIndex;
			}
			set
			{
				this._pageIndex = ((value < 0) ? 0 : value);
			}
		}

		public int PageSize
		{
			get
			{
				return this._pageSize;
			}
			set
			{
				this._pageSize = ((value < 1) ? 1 : ((value > 100) ? 100 : value));
			}
		}

		public bool ShowMatureContent { get; set; }

		public SearchFilterPlatformStatus PlatformStatus { get; set; }

		public SortModsBy SortBy { get; set; } = SortModsBy.DateSubmitted;

		public bool IsSortAscending { get; set; } = true;

		public RevenueType RevenueType { get; set; }

		public ModSearchFilter(int pageIndex = 0, int pageSize = 100)
		{
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
		}

		public void AddSearchPhrase(string phrase, Filtering filtering = Filtering.Like)
		{
			if (string.IsNullOrEmpty(phrase))
			{
				return;
			}
			if (this._searchPhrases == null)
			{
				this._searchPhrases = new Dictionary<Filtering, List<string>>();
			}
			List<string> list;
			if (!this._searchPhrases.TryGetValue(filtering, out list))
			{
				list = new List<string>();
				this._searchPhrases.Add(filtering, list);
			}
			list.Add(phrase);
		}

		public void AddSearchPhrases(ICollection<string> phrase, Filtering filtering = Filtering.Like)
		{
			if (phrase == null || phrase.Count == 0)
			{
				return;
			}
			if (this._searchPhrases == null)
			{
				this._searchPhrases = new Dictionary<Filtering, List<string>>();
			}
			List<string> list;
			if (!this._searchPhrases.TryGetValue(filtering, out list))
			{
				list = new List<string>();
				this._searchPhrases.Add(filtering, list);
			}
			list.AddRange(phrase);
		}

		public void ClearSearchPhrases()
		{
			Dictionary<Filtering, List<string>> searchPhrases = this._searchPhrases;
			if (searchPhrases == null)
			{
				return;
			}
			searchPhrases.Clear();
		}

		public void ClearSearchPhrases(Filtering filtering)
		{
			Dictionary<Filtering, List<string>> searchPhrases = this._searchPhrases;
			if (searchPhrases == null)
			{
				return;
			}
			searchPhrases.Remove(filtering);
		}

		public IList<string> GetSearchPhrase(Filtering filtering)
		{
			List<string> result;
			if (this._searchPhrases != null && this._searchPhrases.TryGetValue(filtering, out result))
			{
				return result;
			}
			return Array.Empty<string>();
		}

		public void AddTag(string tag)
		{
			if (this._tags == null)
			{
				this._tags = new List<string>();
			}
			this._tags.Add(tag);
		}

		public void AddTags(IEnumerable<string> tags)
		{
			if (this._tags == null)
			{
				this._tags = new List<string>();
			}
			this._tags.AddRange(tags);
		}

		public void ClearTags()
		{
			List<string> tags = this._tags;
			if (tags == null)
			{
				return;
			}
			tags.Clear();
		}

		public IReadOnlyList<string> GetTags()
		{
			IReadOnlyList<string> tags = this._tags;
			return tags ?? Array.Empty<string>();
		}

		public void AddUser(UserProfile user)
		{
			if (this._users == null)
			{
				this._users = new List<UserProfile>();
			}
			this._users.Add(user);
		}

		public IReadOnlyList<UserProfile> GetUsers()
		{
			IReadOnlyList<UserProfile> users = this._users;
			return users ?? Array.Empty<UserProfile>();
		}

		public ModioAPI.Mods.GetModsFilter GetModsFilter()
		{
			ModioAPI.Mods.GetModsFilter getModsFilter = ModioAPI.Mods.FilterGetMods(this.PageIndex, this.PageSize);
			for (Filtering filtering = Filtering.None; filtering <= Filtering.BitwiseAnd; filtering++)
			{
				IList<string> searchPhrase = this.GetSearchPhrase(filtering);
				if (searchPhrase.Count > 0)
				{
					getModsFilter.Name("*" + searchPhrase[0] + "*", filtering);
				}
			}
			if (this._tags != null && this._tags.Count > 0)
			{
				getModsFilter.Tags(this._tags, Filtering.None);
			}
			if (this._users != null && this._users.Count > 0)
			{
				getModsFilter.SubmittedBy((from u in this._users
				select u.UserId).ToArray<long>(), Filtering.None);
			}
			getModsFilter.MaturityOption(this.ShowMatureContent ? 15L : 0L, this.ShowMatureContent ? Filtering.BitwiseAnd : Filtering.None);
			string text;
			switch (this.PlatformStatus)
			{
			case SearchFilterPlatformStatus.None:
				text = null;
				break;
			case SearchFilterPlatformStatus.PendingOnly:
				text = "pending_only";
				break;
			case SearchFilterPlatformStatus.LiveAndPending:
				text = "live_and_pending";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			string text2 = text;
			if (text2 != null)
			{
				getModsFilter.PlatformStatus(text2, Filtering.None);
			}
			switch (this.SortBy)
			{
			case SortModsBy.Name:
				text = "name";
				break;
			case SortModsBy.Price:
				text = "price";
				break;
			case SortModsBy.Rating:
				text = "ratings_weighted_aggregate";
				break;
			case SortModsBy.Popular:
				text = "downloads_today";
				break;
			case SortModsBy.Downloads:
				text = "downloads_total";
				break;
			case SortModsBy.Subscribers:
				text = "subscribers_total";
				break;
			case SortModsBy.DateSubmitted:
				text = "id";
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			string key = text;
			getModsFilter.SortByStringType(key, this.IsSortAscending);
			getModsFilter.RevenueType((long)this.RevenueType, Filtering.None);
			return getModsFilter;
		}

		private Dictionary<Filtering, List<string>> _searchPhrases;

		private List<string> _tags;

		private List<UserProfile> _users;

		private int _pageSize;

		private int _pageIndex;
	}
}
