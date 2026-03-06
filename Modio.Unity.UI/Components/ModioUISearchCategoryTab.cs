using System;
using Modio.Unity.UI.Components.Localization;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components
{
	public class ModioUISearchCategoryTab : MonoBehaviour
	{
		private void Awake()
		{
			this._toggle = base.GetComponent<Toggle>();
			this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
		}

		private void Start()
		{
			if (this._toggle.isOn)
			{
				this.OnToggleValueChanged(true);
			}
		}

		private void OnToggleValueChanged(bool newValue)
		{
			if (newValue && ModioUISearch.Default != null && this._search != null)
			{
				this._search.SetAsCustomSearchBase(ModioUISearch.Default);
				this._search.Search(ModioUISearch.Default);
			}
		}

		public void SetSearch(ModioUISearchSettings searchSettings)
		{
			this._search = searchSettings;
			if (this._label != null)
			{
				this._label.text = searchSettings.DisplayAs;
			}
			if (this._labelLocalised != null)
			{
				this._labelLocalised.SetKey(searchSettings.DisplayAsLocalisedKey);
			}
		}

		public void SetSelected(bool selected = true)
		{
			if (this._toggle == null)
			{
				this._toggle = base.GetComponent<Toggle>();
				this._toggle.isOn = selected;
				this.OnToggleValueChanged(selected);
				return;
			}
			if (this._toggle.isOn != selected)
			{
				this._toggle.isOn = selected;
				return;
			}
			this.OnToggleValueChanged(selected);
		}

		[SerializeField]
		private ModioUISearchSettings _search;

		[SerializeField]
		private TMP_Text _label;

		[SerializeField]
		private ModioUILocalizedText _labelLocalised;

		[SerializeField]
		private bool _selectOnEnable;

		private Toggle _toggle;
	}
}
