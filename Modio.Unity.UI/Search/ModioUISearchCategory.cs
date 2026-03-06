using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modio.Unity.UI.Search
{
	public class ModioUISearchCategory : MonoBehaviour
	{
		public string CategoryLabel
		{
			get
			{
				return this._categoryLabel;
			}
		}

		public string CategoryLabelLocalized
		{
			get
			{
				return this._categoryLabelLocalized;
			}
		}

		public IEnumerable<ModioUISearchSettings> Tabs
		{
			get
			{
				return this._tabs;
			}
		}

		public ModioUISearchSettings CustomSearchBase
		{
			get
			{
				return this._customSearchBase;
			}
		}

		[SerializeField]
		private string _categoryLabel;

		[SerializeField]
		private string _categoryLabelLocalized;

		[SerializeField]
		private List<ModioUISearchSettings> _tabs;

		[SerializeField]
		private ModioUISearchSettings _customSearchBase;
	}
}
