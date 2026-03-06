using System;
using Modio.Unity.UI.Input;
using Modio.Unity.UI.Panels;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyPagedSearch : ISearchProperty, IPropertyMonoBehaviourEvents
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			this._search = search;
			if (this._pageCountText != null)
			{
				int num = search.LastSearchFilter.PageIndex + 1;
				int lastSearchResultPageCount = search.LastSearchResultPageCount;
				this._pageCountText.text = string.Format(this._pageCountString, num, lastSearchResultPageCount);
			}
		}

		public void Start()
		{
			if (this._prevPage != null)
			{
				this._prevPage.onClick.AddListener(new UnityAction(this.OnPrevPageClicked));
			}
			if (this._nextPage != null)
			{
				this._nextPage.onClick.AddListener(new UnityAction(this.OnNextPageClicked));
			}
		}

		private void OnPrevPageClicked()
		{
			if (this._whenPanelFocused != null && !this._whenPanelFocused.HasFocus)
			{
				return;
			}
			int pageIndex = this._search.LastSearchFilter.PageIndex;
			if (pageIndex > 0)
			{
				this._search.SetPageForCurrentSearch(pageIndex - 1);
			}
		}

		private void OnNextPageClicked()
		{
			if (this._whenPanelFocused != null && !this._whenPanelFocused.HasFocus)
			{
				return;
			}
			int pageIndex = this._search.LastSearchFilter.PageIndex;
			if (pageIndex + 1 < this._search.LastSearchResultPageCount)
			{
				this._search.SetPageForCurrentSearch(pageIndex + 1);
			}
		}

		public void OnDestroy()
		{
		}

		public void OnEnable()
		{
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.SearchPageLeft, new Action(this.OnPrevPageClicked));
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.SearchPageRight, new Action(this.OnNextPageClicked));
		}

		public void OnDisable()
		{
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.SearchPageLeft, new Action(this.OnPrevPageClicked));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.SearchPageRight, new Action(this.OnNextPageClicked));
		}

		[SerializeField]
		private TMP_Text _pageCountText;

		[SerializeField]
		private string _pageCountString = "Page {0} of {1}";

		[SerializeField]
		private Button _prevPage;

		[SerializeField]
		private Button _nextPage;

		[SerializeField]
		private ModioPanelBase _whenPanelFocused;

		private ModioUISearch _search;
	}
}
