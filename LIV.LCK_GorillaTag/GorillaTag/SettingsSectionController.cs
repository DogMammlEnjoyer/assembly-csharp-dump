using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class SettingsSectionController : MonoBehaviour
	{
		public void EvaluateMode(CameraMode mode)
		{
			bool active = mode == this._mode;
			this._ui.SetActive(active);
		}

		[SerializeField]
		private CameraMode _mode;

		[SerializeField]
		private GameObject _ui;
	}
}
