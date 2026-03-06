using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.Selectables
{
	public class ModioUIToggleOutputSplitter : MonoBehaviour
	{
		private void Awake()
		{
			this._toggle = base.GetComponent<Toggle>();
			this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleValueChanged));
		}

		private void Start()
		{
			if (this._toggle.isOn && !this._hasFiredEvent)
			{
				this.ToggleValueChanged(true);
			}
		}

		private void ToggleValueChanged(bool isOn)
		{
			this._hasFiredEvent = true;
			if (isOn)
			{
				this.onToggleOn.Invoke(true);
				return;
			}
			this.onToggleOff.Invoke(false);
		}

		public Toggle.ToggleEvent onToggleOn = new Toggle.ToggleEvent();

		public Toggle.ToggleEvent onToggleOff = new Toggle.ToggleEvent();

		private Toggle _toggle;

		private bool _hasFiredEvent;
	}
}
