using System;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyTotalResults : ISearchProperty
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			this._foundResultsText.gameObject.SetActive(search.LastSearchResultModCount > 0);
			this._foundResultsText.text = string.Format(this._foundResultsString, search.LastSearchResultModCount);
		}

		[SerializeField]
		private TMP_Text _foundResultsText;

		[SerializeField]
		private string _foundResultsString = "Found {0} results";
	}
}
