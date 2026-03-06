using System;
using System.Collections.Generic;
using Modio.API;
using Modio.Mods;
using UnityEngine;

namespace Modio.Unity.UI.Search
{
	public class ModioUISearchSettings : MonoBehaviour
	{
		public ModSearchFilter GetSearchFilter(int paginationSize)
		{
			ModSearchFilter modSearchFilter = new ModSearchFilter(0, paginationSize);
			modSearchFilter.SortBy = this.sortModsBy;
			modSearchFilter.ShowMatureContent = this.showMatureContent;
			modSearchFilter.IsSortAscending = this.isAscending;
			modSearchFilter.RevenueType = this.filterRevenueType;
			modSearchFilter.AddTags(this.searchTags);
			modSearchFilter.AddSearchPhrase(this.searchPhrase, Filtering.Like);
			return modSearchFilter;
		}

		public void Search(ModioUISearch searchWith)
		{
			if (searchWith == null)
			{
				searchWith = ModioUISearch.Default;
			}
			ModSearchFilter searchFilter = this.GetSearchFilter(searchWith.DefaultPageSize);
			searchWith.SetSearch(searchFilter, this.searchType, true, this.shareFilterSettingsWith, this);
		}

		public void SetAsCustomSearchBase(ModioUISearch searchWith)
		{
			if (searchWith == null)
			{
				searchWith = ModioUISearch.Default;
			}
			ModSearchFilter searchFilter = this.GetSearchFilter(searchWith.DefaultPageSize);
			searchWith.SetCustomSearchBase(searchFilter, this.searchType);
		}

		public string DisplayAs;

		public string DisplayAsLocalisedKey;

		public Sprite Icon;

		public bool HiddenIfMonetizationDisabled;

		public SpecialSearchType searchType = SpecialSearchType.Nothing;

		public string searchPhrase;

		public List<string> searchTags;

		public SortModsBy sortModsBy;

		public bool showMatureContent;

		public bool isAscending;

		public RevenueType filterRevenueType;

		public Object shareFilterSettingsWith;
	}
}
