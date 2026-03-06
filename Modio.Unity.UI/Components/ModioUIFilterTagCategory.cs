using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUIFilterTagCategory : MonoBehaviour
	{
		public int CurrentFilterCount { get; private set; }

		private void Reset()
		{
			this._categoryTitle = base.GetComponentInChildren<TMP_Text>();
		}

		public void Setup(GameTagCategory category)
		{
			this._categoryTitle.text = category.Name;
			this.SetFilterCount(0);
		}

		public void SetFilterCount(int filterCount)
		{
			this.CurrentFilterCount = filterCount;
			if (this._filterCount != null)
			{
				this._filterCount.text = filterCount.ToString();
			}
			if (this._filterCountBackground != null)
			{
				this._filterCountBackground.SetActive(filterCount > 0);
			}
		}

		[SerializeField]
		private TMP_Text _categoryTitle;

		[SerializeField]
		private TMP_Text _filterCount;

		[SerializeField]
		private GameObject _filterCountBackground;
	}
}
