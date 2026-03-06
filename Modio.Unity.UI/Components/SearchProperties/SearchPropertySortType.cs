using System;
using Modio.Mods;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertySortType : ISearchProperty
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			if (search.SortByOverriden)
			{
				SortModsBy sortBy = search.LastSearchFilter.SortBy;
				string str = (sortBy == SortModsBy.DateSubmitted) ? "Date Submitted" : sortBy.ToString();
				this._searchText.text = "<b>SORT:</b> " + str;
				return;
			}
			this._searchText.text = "SORT";
		}

		[SerializeField]
		private TMP_Text _searchText;
	}
}
