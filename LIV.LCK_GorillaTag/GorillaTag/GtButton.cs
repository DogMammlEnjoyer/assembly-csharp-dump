using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtButton : MonoBehaviour
	{
		private void Awake()
		{
			if (this._initializeType == ButtonInitializeType.Awake)
			{
				this.InitSetUp();
			}
		}

		private void Start()
		{
			if (this._initializeType == ButtonInitializeType.Start)
			{
				this.InitSetUp();
			}
		}

		public void SetDisabled(bool isDisabled)
		{
			this._isDisabled = isDisabled;
			if (this._iconImage)
			{
				this._iconImage.color = (isDisabled ? this._settings.InactiveIconColor : this._settings.PrimaryIconColor);
			}
			if (this._label)
			{
				this._label.color = (isDisabled ? this._settings.DisabledTextColor : this._settings.PrimaryTextColor);
			}
		}

		public void TapStarted()
		{
			if (this._isDisabled)
			{
				return;
			}
			this._bodyRenderer.material = this._settings.SelectedBodyMaterial;
			this._visualsTrans.localPosition = this._defaultLocalPosition + Vector3.forward * -this._settings.ActiveButtonOffset;
			this.FlipVisuals();
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
			this.onTap.Invoke();
		}

		public void TapEnded()
		{
			if (this._isDisabled)
			{
				return;
			}
			this._bodyRenderer.material = this._settings.DefaultBodyMaterial;
			this._visualsTrans.localPosition = this._defaultLocalPosition;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
		}

		public void TapEndedNoAudio()
		{
			this._bodyRenderer.material = this._settings.DefaultBodyMaterial;
			this._visualsTrans.localPosition = this._defaultLocalPosition;
		}

		public void SetLabelText(string text)
		{
			this._label.text = text;
		}

		private void InitSetUp()
		{
			this._defaultLocalPosition = this._visualsTrans.localPosition;
			this._label.color = this._settings.PrimaryTextColor;
			if (this._iconImage)
			{
				this._iconImage.color = this._settings.PrimaryIconColor;
			}
			this._label.text = this._name.ToUpper();
		}

		private void FlipVisuals()
		{
			if (!this._doFlipping)
			{
				return;
			}
			this._isFlipped = !this._isFlipped;
			this._visualsTrans.localScale = new Vector3((float)(this._isFlipped ? -1 : 1), 1f, 1f);
		}

		[SerializeField]
		private GtUiSettings _settings;

		[Space(10f)]
		[Header("Settings")]
		[SerializeField]
		private string _name;

		[SerializeField]
		private bool _doFlipping = true;

		[SerializeField]
		private ButtonInitializeType _initializeType;

		[Space(10f)]
		[Header("UI Elements")]
		[SerializeField]
		private TextMeshPro _label;

		[SerializeField]
		private MeshRenderer _bodyRenderer;

		[SerializeField]
		private Transform _visualsTrans;

		[SerializeField]
		private SpriteRenderer _iconImage;

		[Space(10f)]
		[Header("Sounds")]
		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[Space(10f)]
		[Header("Events")]
		public UnityEvent onTap;

		private Vector3 _defaultLocalPosition;

		private bool _isFlipped;

		private bool _isDisabled;
	}
}
