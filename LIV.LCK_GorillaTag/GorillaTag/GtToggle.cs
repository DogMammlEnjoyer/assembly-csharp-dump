using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtToggle : MonoBehaviour
	{
		public bool IsFirstSelected
		{
			get
			{
				return this._isFirstSelected;
			}
			set
			{
				this._isFirstSelected = value;
				this.UpdateToggle(this._isFirstSelected);
				this.onValueChanged.Invoke(this._isFirstSelected);
			}
		}

		private void OnValidate()
		{
			this.SetUp();
		}

		private void Start()
		{
			this.SetUp();
		}

		public void FirstButtonPressed()
		{
			this.IsFirstSelected = true;
			this._visualsTrans.localRotation = Quaternion.Euler(0f, this._settings.CounterAngleOffset, 0f);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void SecondButtonPressed()
		{
			this.IsFirstSelected = false;
			this._visualsTrans.localRotation = Quaternion.Euler(0f, -this._settings.CounterAngleOffset, 0f);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void TapEnded()
		{
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
		}

		public void Reset()
		{
			this._visualsTrans.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}

		private void UpdateToggle(bool isFirstSelected)
		{
			this._firstButtonRenderer.color = (isFirstSelected ? this._settings.PrimaryCounterButtonActiveColor : this._settings.PrimaryCounterButtonDefaultColor);
			this._secondButtonRenderer.color = (isFirstSelected ? this._settings.PrimaryCounterButtonDefaultColor : this._settings.PrimaryCounterButtonActiveColor);
		}

		private void SetUp()
		{
			this._label.text = this._name.ToUpper();
			this.UpdateToggle(this._isFirstSelected);
			this._firstButtonLabel.text = this._firstLabelValue.ToUpper();
			this._secondButtonLabel.text = this._secondLabelValue.ToUpper();
		}

		[SerializeField]
		private bool _isFirstSelected = true;

		[SerializeField]
		private string _name;

		[SerializeField]
		private string _firstLabelValue;

		[SerializeField]
		private string _secondLabelValue;

		[SerializeField]
		private GtUiSettings _settings;

		[Space(10f)]
		[Header("Elements")]
		[SerializeField]
		private TextMeshPro _label;

		[SerializeField]
		private SpriteRenderer _firstButtonRenderer;

		[SerializeField]
		private SpriteRenderer _secondButtonRenderer;

		[SerializeField]
		private TextMeshPro _firstButtonLabel;

		[SerializeField]
		private TextMeshPro _secondButtonLabel;

		[SerializeField]
		private Transform _visualsTrans;

		[Space(10f)]
		[Header("Sounds")]
		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[Space(10f)]
		[Header("Events")]
		public UnityEvent<bool> onValueChanged = new UnityEvent<bool>();
	}
}
