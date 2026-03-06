using System;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyFilterCount : ISearchProperty
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			int count = search.LastSearchFilter.GetTags().Count;
			if (this._filterCount != null)
			{
				this._filterCount.text = count.ToString();
			}
			if (this._filterCountBackground != null)
			{
				this._filterCountBackground.SetActive(count > 0);
			}
		}

		[SerializeField]
		private TMP_Text _filterCount;

		[SerializeField]
		private GameObject _filterCountBackground;
	}
}
