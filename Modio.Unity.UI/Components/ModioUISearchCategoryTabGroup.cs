using System;
using System.Collections.Generic;
using Modio.Unity.Settings;
using Modio.Unity.UI.Components.Localization;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUISearchCategoryTabGroup : MonoBehaviour
	{
		public void ClearCategory()
		{
			if (this._setCategoryOnFrame != Time.frameCount)
			{
				this.SetCategory(null);
			}
		}

		public void SetCategory(ModioUISearchCategory category)
		{
			if (this._disableIfNoCategory != null)
			{
				this._disableIfNoCategory.SetActive(category != null);
			}
			if (category == null)
			{
				return;
			}
			this._setCategoryOnFrame = Time.frameCount;
			this.SetTabs(category.Tabs);
			if (ModioUISearch.Default != null)
			{
				if (category.CustomSearchBase != null)
				{
					category.CustomSearchBase.SetAsCustomSearchBase(ModioUISearch.Default);
				}
				else
				{
					ModioUISearch.Default.SetCustomSearchBase(null, (SpecialSearchType)0);
				}
			}
			if (this._categoryName != null)
			{
				this._categoryName.text = category.CategoryLabel;
			}
			if (this._categoryNameLocalized != null)
			{
				this._categoryNameLocalized.SetKey(category.CategoryLabelLocalized);
			}
		}

		private void Start()
		{
			if (this._tabs.Count > 0)
			{
				this._tabs[0].SetSelected(true);
			}
			else if (this._disableIfNoCategory != null)
			{
				this._disableIfNoCategory.SetActive(false);
			}
			if (this._activeTabCount == 1)
			{
				this._tabs[0].gameObject.SetActive(false);
			}
			this._hasRunStart = true;
		}

		public void SetTabs(IEnumerable<ModioUISearchSettings> tabSearches)
		{
			int i = 0;
			ModioComponentUISettings platformSettings = ModioClient.Settings.GetPlatformSettings<ModioComponentUISettings>();
			bool flag = platformSettings != null && platformSettings.ShowMonetizationUI;
			foreach (ModioUISearchSettings modioUISearchSettings in tabSearches)
			{
				if (flag || !modioUISearchSettings.HiddenIfMonetizationDisabled)
				{
					ModioUISearchCategoryTab modioUISearchCategoryTab;
					if (i < this._tabs.Count)
					{
						modioUISearchCategoryTab = this._tabs[i];
						modioUISearchCategoryTab.gameObject.SetActive(true);
					}
					else if (i == 0)
					{
						this._tabs.Add(this._firstTab);
						modioUISearchCategoryTab = this._firstTab;
					}
					else
					{
						modioUISearchCategoryTab = Object.Instantiate<ModioUISearchCategoryTab>(this._firstTab, this._firstTab.transform.parent);
						modioUISearchCategoryTab.transform.SetSiblingIndex(this._firstTab.transform.GetSiblingIndex() + i);
						this._tabs.Add(modioUISearchCategoryTab);
					}
					modioUISearchCategoryTab.SetSearch(modioUISearchSettings);
					i++;
				}
			}
			this._activeTabCount = i;
			while (i < this._tabs.Count)
			{
				this._tabs[i].gameObject.SetActive(false);
				i++;
			}
			if (this._hasRunStart)
			{
				for (int j = 0; j < this._tabs.Count; j++)
				{
					this._tabs[j].SetSelected(j == 0);
				}
				if (this._activeTabCount == 1)
				{
					this._tabs[0].gameObject.SetActive(false);
				}
			}
			if (this._disableIfNoCategory != null && this._activeTabCount <= 1)
			{
				this._disableIfNoCategory.SetActive(false);
			}
		}

		[SerializeField]
		private ModioUISearchCategoryTab _firstTab;

		[SerializeField]
		private GameObject _disableIfNoCategory;

		[SerializeField]
		private TMP_Text _categoryName;

		[SerializeField]
		private ModioUILocalizedText _categoryNameLocalized;

		private readonly List<ModioUISearchCategoryTab> _tabs = new List<ModioUISearchCategoryTab>();

		private bool _hasRunStart;

		private int _activeTabCount;

		private int _setCategoryOnFrame;
	}
}
