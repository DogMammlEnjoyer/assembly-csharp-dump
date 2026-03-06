using System;
using System.Collections.Generic;
using Modio.API;
using Modio.Mods;
using Modio.Unity.UI.Components.Localization;
using Modio.Unity.UI.Search;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyInfoText : ISearchProperty
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			ModSearchFilter lastSearchFilter = search.LastSearchFilter;
			IList<string> searchPhrase = lastSearchFilter.GetSearchPhrase(Filtering.Like);
			bool flag = searchPhrase != null && searchPhrase.Count > 0;
			if (this._searchText != null)
			{
				this._searchText.enabled = flag;
				if (flag)
				{
					this._searchText.text = (string.Join(" ", searchPhrase) ?? "");
				}
				if (SpecialSearchType.SearchForTag == search.LastSearchPreset)
				{
					this._searchText.enabled = true;
					IReadOnlyList<string> tags = lastSearchFilter.GetTags();
					this._searchText.text = (string.Join(" ", tags) ?? "");
				}
			}
			IReadOnlyList<UserProfile> users = lastSearchFilter.GetUsers();
			bool flag2 = false;
			if (users.Count > 0)
			{
				UserProfile userProfile = users[0];
				flag2 = (userProfile != null);
				if (this._searchText != null)
				{
					this._searchText.enabled = flag2;
					if (flag2)
					{
						this._searchText.text = (userProfile.Username ?? "");
					}
				}
			}
			if (this._disableWhileShowingCustomText != null)
			{
				this._disableWhileShowingCustomText.SetActive(!flag && !flag2);
			}
			if (this._showWhileShowingCustomText != null)
			{
				this._showWhileShowingCustomText.SetActive(flag || flag2);
			}
			if (search.LastSearchSettingsFrom != null)
			{
				if (this._searchCategoryName != null)
				{
					this._searchCategoryName.text = search.LastSearchSettingsFrom.DisplayAs;
				}
				if (this._searchCategoryNameLocalized != null)
				{
					this._searchCategoryNameLocalized.SetKey(search.LastSearchSettingsFrom.DisplayAsLocalisedKey);
				}
				if (this._searchCategoryIcon != null)
				{
					this._searchCategoryIcon.sprite = search.LastSearchSettingsFrom.Icon;
					this._searchCategoryIcon.enabled = (search.LastSearchSettingsFrom.Icon != null);
				}
			}
		}

		[SerializeField]
		private TMP_Text _searchText;

		[SerializeField]
		private GameObject _disableWhileShowingCustomText;

		[SerializeField]
		private GameObject _showWhileShowingCustomText;

		[SerializeField]
		private TMP_Text _searchCategoryName;

		[SerializeField]
		private ModioUILocalizedText _searchCategoryNameLocalized;

		[SerializeField]
		private Image _searchCategoryIcon;
	}
}
