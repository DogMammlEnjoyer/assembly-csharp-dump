using System;
using Modio.Unity.UI.Panels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.Selectables
{
	public class ModioUIToggleDeactivateWhenPanelLostFocus : MonoBehaviour
	{
		private void Awake()
		{
			this._toggle = base.GetComponent<Toggle>();
			this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
		}

		private void OnValueChanged(bool isOn)
		{
			if (this._panel == null)
			{
				return;
			}
			this._panel.OnHasFocusChanged -= this.PanelChangedFocus;
			if (isOn)
			{
				this._panel.OnHasFocusChanged += this.PanelChangedFocus;
			}
		}

		private void OnDestroy()
		{
			if (this._panel != null)
			{
				this._panel.OnHasFocusChanged -= this.PanelChangedFocus;
			}
		}

		private void PanelChangedFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				this._toggle.isOn = false;
			}
		}

		[SerializeField]
		private ModioPanelBase _panel;

		private Toggle _toggle;
	}
}
