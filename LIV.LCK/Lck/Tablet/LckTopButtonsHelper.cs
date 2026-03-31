using System;
using Liv.Lck.UI;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class LckTopButtonsHelper : MonoBehaviour, ILckTopButtons
	{
		public void HideButtons()
		{
			this._cameraToggle.SetDisabledState(true);
			this._streamToggle.SetDisabledState(true);
		}

		public void ShowButtons()
		{
			this._cameraToggle.RestoreToggleState();
			this._streamToggle.RestoreToggleState();
		}

		public void SetCameraPageVisualsManually()
		{
			this._cameraToggle.SetToggleVisualsOn();
			this._streamToggle.SetToggleVisualsOff();
		}

		[SerializeField]
		private LckToggle _cameraToggle;

		[SerializeField]
		private LckToggle _streamToggle;
	}
}
