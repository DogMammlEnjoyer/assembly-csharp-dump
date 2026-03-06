using System;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtHdSdToggle : MonoBehaviour
	{
		public GtButton Button
		{
			get
			{
				return this._hdSdButton;
			}
		}

		public void SetIsHdNoNotify(bool value)
		{
			this._isHd = value;
			this.UpdateUi();
		}

		private void OnEnable()
		{
			this._hdSdButton.onTap.AddListener(new UnityAction(this.ProcessHdSdToggle));
		}

		private void OnDisable()
		{
			this._hdSdButton.onTap.RemoveListener(new UnityAction(this.ProcessHdSdToggle));
		}

		private void Start()
		{
			this.UpdateUi();
		}

		private void UpdateUi()
		{
			string labelText = this._isHd ? this._hdLabelText : this._sdLabelText;
			this._hdSdButton.SetLabelText(labelText);
		}

		private void ProcessHdSdToggle()
		{
			this._isHd = !this._isHd;
			this.UpdateUi();
			this.OnHdModeChanged.Invoke(this._isHd);
		}

		[Header("Settings")]
		[SerializeField]
		private string _hdLabelText;

		[SerializeField]
		private string _sdLabelText;

		[SerializeField]
		private bool _isHd;

		[Space(10f)]
		[Header("Elements")]
		[SerializeField]
		private GtButton _hdSdButton;

		[Space(10f)]
		[Header("Events")]
		public UnityEvent<bool> OnHdModeChanged;
	}
}
