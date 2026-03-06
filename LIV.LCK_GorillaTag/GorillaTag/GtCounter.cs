using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtCounter : MonoBehaviour
	{
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = ((value <= this._minValue) ? this._minValue : value);
				this._value = ((this._value >= this._maxValue) ? this._maxValue : this._value);
				this.UpdateCounter(this._value);
				this.onValueChanged.Invoke(this._value);
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

		private void SetUp()
		{
			this._label.text = this._name.ToUpper();
			this._label.color = this._settings.PrimaryTextColor;
			this._valueLabel.color = this._settings.PrimaryTextColor;
			this.Value = this._value;
			this._decrementButtonRenderer.color = this._settings.PrimaryCounterButtonDefaultColor;
			this._incrementButtonRenderer.color = this._settings.PrimaryCounterButtonDefaultColor;
			this.UpdateCounter(this._value);
		}

		public void Increase()
		{
			this._visualsTrans.localRotation = Quaternion.Euler(0f, -this._settings.CounterAngleOffset, 0f);
			this._incrementButtonRenderer.color = this._settings.PrimaryCounterButtonActiveColor;
			this.Value += this._step;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void Decrease()
		{
			this._visualsTrans.localRotation = Quaternion.Euler(0f, this._settings.CounterAngleOffset, 0f);
			this._decrementButtonRenderer.color = ((this.Value <= 0) ? this._settings.PrimaryCounterButtonDefaultColor : this._settings.PrimaryCounterButtonActiveColor);
			this.Value -= this._step;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void TapEnded()
		{
			this._visualsTrans.localRotation = Quaternion.identity;
			this._incrementButtonRenderer.color = this._settings.PrimaryCounterButtonDefaultColor;
			this._decrementButtonRenderer.color = this._settings.PrimaryCounterButtonDefaultColor;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
		}

		private void UpdateCounter(int num)
		{
			if (num <= this._minValue)
			{
				this._valueLabel.text = (this._showOffInsteadOfZero ? "OFF" : this._minValue.ToString());
				this._minusRenderer.color = this._settings.InactiveIconColor;
				this._plusRenderer.color = this._settings.PrimaryIconColor;
				return;
			}
			if (num >= this._maxValue)
			{
				this._valueLabel.text = (this._showMaxInsteadOfNumber ? "MAX" : this._maxValue.ToString());
				this._plusRenderer.color = this._settings.InactiveIconColor;
				this._minusRenderer.color = this._settings.PrimaryIconColor;
				return;
			}
			this._valueLabel.text = num.ToString();
			this._minusRenderer.color = this._settings.PrimaryIconColor;
			this._plusRenderer.color = this._settings.PrimaryIconColor;
		}

		[SerializeField]
		private GtUiSettings _settings;

		[Space(10f)]
		[SerializeField]
		private string _name;

		[SerializeField]
		private int _value;

		[SerializeField]
		private int _step;

		[SerializeField]
		private int _minValue;

		[SerializeField]
		private int _maxValue;

		[SerializeField]
		private bool _showOffInsteadOfZero;

		[SerializeField]
		private bool _showMaxInsteadOfNumber;

		[SerializeField]
		private TextMeshPro _label;

		[SerializeField]
		private TextMeshPro _valueLabel;

		[SerializeField]
		private SpriteRenderer _decrementButtonRenderer;

		[SerializeField]
		private SpriteRenderer _incrementButtonRenderer;

		[SerializeField]
		private SpriteRenderer _minusRenderer;

		[SerializeField]
		private SpriteRenderer _plusRenderer;

		[SerializeField]
		private Transform _visualsTrans;

		[SerializeField]
		private LckDiscreetAudioController _audioController;

		public UnityEvent<int> onValueChanged = new UnityEvent<int>();
	}
}
