using System;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUISearchField : MonoBehaviour
	{
		private void Start()
		{
			this._hasRunStart = true;
			this.OnEnable();
		}

		private void OnEnable()
		{
			if (!this._hasRunStart)
			{
				return;
			}
			this.searchField.text = (this.lastSearchPhrase = "");
			ModioUISearch.Default.AppliedSearchPreset += this.OnAppliedSearchPreset;
		}

		private void OnDisable()
		{
			ModioUISearch.Default.AppliedSearchPreset -= this.OnAppliedSearchPreset;
		}

		private void OnAppliedSearchPreset()
		{
			this.searchField.text = (this.lastSearchPhrase = "");
		}

		public void FilterView()
		{
			if (this.lastSearchPhrase == this.searchField.text)
			{
				return;
			}
			this.lastSearchPhrase = this.searchField.text;
			ModioUISearch.Default.ApplySearchPhrase(this.searchField.text);
		}

		[SerializeField]
		private TMP_InputField searchField;

		private string lastSearchPhrase;

		private bool _hasRunStart;
	}
}
