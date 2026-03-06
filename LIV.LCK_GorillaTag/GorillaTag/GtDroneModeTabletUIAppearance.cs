using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtDroneModeTabletUIAppearance : MonoBehaviour
	{
		public bool IsDroneModeActive
		{
			get
			{
				return this._isDroneModeActive;
			}
			set
			{
				this._isDroneModeActive = value;
				this.EvaluateMode(this._isDroneModeActive);
			}
		}

		private void EvaluateMode(bool isDroneMode)
		{
			this._selectorsGroup.SetActive(!isDroneMode);
			this._settingsGroup.SetActive(!isDroneMode);
			if (isDroneMode)
			{
				this._display.Maximize();
				return;
			}
			this._display.Minimize();
		}

		[Header("Tablet UI Elements")]
		[SerializeField]
		private GameObject _selectorsGroup;

		[SerializeField]
		private GameObject _orientationButton;

		[SerializeField]
		private GameObject _settingsGroup;

		[SerializeField]
		private GtDisplay _display;

		private bool _isDroneModeActive;
	}
}
