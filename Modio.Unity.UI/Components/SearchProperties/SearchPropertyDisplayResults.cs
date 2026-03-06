using System;
using Modio.Errors;
using Modio.Unity.UI.Search;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyDisplayResults : ISearchProperty
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			if (!search.IsSearching)
			{
				if (this._modGroup != null)
				{
					this._modGroup.SetMods(search.LastSearchResultMods, search.LastSearchSelectionIndex);
				}
				bool flag = this._displayWhenOffline != null && search.LastSearchError.Code == ErrorCode.CANNOT_OPEN_CONNECTION;
				if (search.LastSearchError && !flag)
				{
					this._errorHandler.Invoke(search.LastSearchError);
				}
				if (this._displayWhenOffline != null)
				{
					this._displayWhenOffline.SetActive(flag && search.LastSearchResultMods.Count == 0);
				}
				if (this._displayWhenNoResults != null)
				{
					this._displayWhenNoResults.SetActive(!flag && search.LastSearchResultMods.Count == 0);
					return;
				}
			}
			else
			{
				if (this._displayWhenOffline != null)
				{
					this._displayWhenOffline.SetActive(false);
				}
				if (this._displayWhenNoResults != null)
				{
					this._displayWhenNoResults.SetActive(false);
				}
			}
		}

		[SerializeField]
		private ModioUIGroup _modGroup;

		[SerializeField]
		[Tooltip("(Optional) Enable this gameObject when there are zero results")]
		private GameObject _displayWhenNoResults;

		[SerializeField]
		[Tooltip("(Optional) Enable this gameObject when there are network issues and there's no results")]
		private GameObject _displayWhenOffline;

		[SerializeField]
		private UnityEvent<Error> _errorHandler;
	}
}
