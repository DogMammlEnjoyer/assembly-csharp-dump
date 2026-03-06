using System;
using Liv.Lck.GorillaTag;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class GtTopButtonsHelper : MonoBehaviour, ILckTopButtons
	{
		public void HideButtons()
		{
			this._cameraButton.SetDisabledState();
			this._streamButton.SetDisabledState();
		}

		public void ShowButtons()
		{
			this._cameraButton.RestoreButtonState();
			this._streamButton.RestoreButtonState();
		}

		[SerializeField]
		private GtTopButton _cameraButton;

		[SerializeField]
		private GtTopButton _streamButton;
	}
}
