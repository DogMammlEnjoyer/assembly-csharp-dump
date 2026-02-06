using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Pun.UtilityScripts
{
	public class TabViewManager : MonoBehaviour
	{
		private void Start()
		{
			this.Tab_lut = new Dictionary<Toggle, TabViewManager.Tab>();
			TabViewManager.Tab[] tabs = this.Tabs;
			for (int i = 0; i < tabs.Length; i++)
			{
				TabViewManager.Tab _tab = tabs[i];
				this.Tab_lut[_tab.Toggle] = _tab;
				_tab.View.gameObject.SetActive(_tab.Toggle.isOn);
				if (_tab.Toggle.isOn)
				{
					this.CurrentTab = _tab;
				}
				_tab.Toggle.onValueChanged.AddListener(delegate(bool isSelected)
				{
					if (!isSelected)
					{
						return;
					}
					this.OnTabSelected(_tab);
				});
			}
		}

		public void SelectTab(string id)
		{
			foreach (TabViewManager.Tab tab in this.Tabs)
			{
				if (tab.ID == id)
				{
					tab.Toggle.isOn = true;
					return;
				}
			}
		}

		private void OnTabSelected(TabViewManager.Tab tab)
		{
			this.CurrentTab.View.gameObject.SetActive(false);
			this.CurrentTab = this.Tab_lut[this.ToggleGroup.ActiveToggles().FirstOrDefault<Toggle>()];
			this.CurrentTab.View.gameObject.SetActive(true);
			this.OnTabChanged.Invoke(this.CurrentTab.ID);
		}

		public ToggleGroup ToggleGroup;

		public TabViewManager.Tab[] Tabs;

		public TabViewManager.TabChangeEvent OnTabChanged;

		protected TabViewManager.Tab CurrentTab;

		private Dictionary<Toggle, TabViewManager.Tab> Tab_lut;

		[Serializable]
		public class TabChangeEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class Tab
		{
			public string ID = "";

			public Toggle Toggle;

			public RectTransform View;
		}
	}
}
