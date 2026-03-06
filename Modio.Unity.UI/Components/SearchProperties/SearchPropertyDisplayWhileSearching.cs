using System;
using Modio.Unity.UI.Search;
using UnityEngine;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyDisplayWhileSearching : ISearchProperty
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			bool flag;
			switch (this._additiveLoadBehaviour)
			{
			case SearchPropertyDisplayWhileSearching.AdditiveLoadBehaviour.Ignore:
				flag = true;
				break;
			case SearchPropertyDisplayWhileSearching.AdditiveLoadBehaviour.OnlyDuringAdditiveLoad:
				flag = search.IsAdditiveSearch;
				break;
			case SearchPropertyDisplayWhileSearching.AdditiveLoadBehaviour.HideDuringAdditiveLoad:
				flag = !search.IsAdditiveSearch;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			bool flag2 = flag;
			this._displayWhileSearching.SetActive(search.IsSearching && flag2);
		}

		[SerializeField]
		private GameObject _displayWhileSearching;

		[SerializeField]
		private SearchPropertyDisplayWhileSearching.AdditiveLoadBehaviour _additiveLoadBehaviour;

		private enum AdditiveLoadBehaviour
		{
			Ignore,
			OnlyDuringAdditiveLoad,
			HideDuringAdditiveLoad
		}
	}
}
