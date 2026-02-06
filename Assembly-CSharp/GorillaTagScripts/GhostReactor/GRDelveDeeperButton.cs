using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	[RequireComponent(typeof(GorillaPressableButton))]
	public sealed class GRDelveDeeperButton : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this._shiftManager == null)
			{
				throw new Exception("_shiftManager unset for GREndShiftButton.");
			}
			this._button = base.GetComponent<GorillaPressableButton>();
			this.UpdateButton();
		}

		private void LateUpdate()
		{
			if (this._lastAuthorizedToDelveDeeper != this._shiftManager.authorizedToDelveDeeper)
			{
				this.UpdateButton();
			}
		}

		private void UpdateButton()
		{
			this._lastAuthorizedToDelveDeeper = this._shiftManager.authorizedToDelveDeeper;
			if (this._lastAuthorizedToDelveDeeper)
			{
				this._button.enabled = true;
				this._text.text = "DELVE\nNOW";
				return;
			}
			this._button.enabled = false;
			this._text.text = "DISABLED";
		}

		public void DelveDeeper()
		{
			this._shiftManager.EndShift();
		}

		[SerializeField]
		private GhostReactorShiftManager _shiftManager;

		[SerializeField]
		private TextMeshPro _text;

		private bool _lastAuthorizedToDelveDeeper;

		private GorillaPressableButton _button;
	}
}
