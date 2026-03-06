using System;
using Liv.Lck.UI;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class LckTopButtonsHelper : MonoBehaviour, ILckTopButtons
	{
		public void HideButtons()
		{
			this._cameraToggle.SetDisabledState();
			this._streamToggle.SetDisabledState();
		}

		public void ShowButtons()
		{
			this._cameraToggle.RestoreToggleState();
			this._streamToggle.RestoreToggleState();
		}

		[SerializeField]
		private LckToggle _cameraToggle;

		[SerializeField]
		private LckToggle _streamToggle;
	}
}
