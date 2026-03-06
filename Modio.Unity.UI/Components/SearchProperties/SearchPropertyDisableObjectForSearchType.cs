using System;
using System.Linq;
using Modio.Unity.UI.Search;
using UnityEngine;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyDisableObjectForSearchType : ISearchProperty
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			bool flag = (this._hideOnCustomSearch && search.HasCustomSearch()) || this._hideForSearchTypes.Contains(search.LastSearchPreset);
			GameObject[] array = this._gameObjectsToHide;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(!flag);
			}
			array = this._gameObjectsToShow;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
		}

		[SerializeField]
		private GameObject[] _gameObjectsToHide;

		[SerializeField]
		private GameObject[] _gameObjectsToShow;

		[SerializeField]
		private bool _hideOnCustomSearch;

		[SerializeField]
		private SpecialSearchType[] _hideForSearchTypes;
	}
}
