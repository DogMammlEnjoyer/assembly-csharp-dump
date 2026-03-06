using System;
using Modio.Unity.UI.Search;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyLoadMoreResults : ISearchProperty, IPropertyMonoBehaviourEvents
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			this._search = search;
			GameObject[] displayWhenMoreResults = this._displayWhenMoreResults;
			for (int i = 0; i < displayWhenMoreResults.Length; i++)
			{
				displayWhenMoreResults[i].SetActive(!search.IsSearching && search.CanGetMoreMods);
			}
		}

		public void Start()
		{
		}

		public void OnDestroy()
		{
		}

		public void OnEnable()
		{
			if (this._loadMoreResultsButton != null)
			{
				this._loadMoreResultsButton.onClick.AddListener(new UnityAction(this.LoadMoreClicked));
			}
		}

		public void OnDisable()
		{
			if (this._loadMoreResultsButton != null)
			{
				this._loadMoreResultsButton.onClick.RemoveListener(new UnityAction(this.LoadMoreClicked));
			}
		}

		private void LoadMoreClicked()
		{
			this._search.GetNextPageAdditivelyForLastSearch();
		}

		[SerializeField]
		private GameObject[] _displayWhenMoreResults;

		[SerializeField]
		private Button _loadMoreResultsButton;

		private ModioUISearch _search;
	}
}
