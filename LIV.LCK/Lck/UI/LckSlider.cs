using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Liv.Lck.UI
{
	public class LckSlider : MonoBehaviour
	{
		public event Action<float> OnValueChanged;

		public float Value
		{
			get
			{
				return this.GetValue();
			}
		}

		private void Start()
		{
			this._slider.value = this._defaultValue;
			this._slider.minValue = this._minValue;
			this._slider.maxValue = this._maxValue;
			this._slider.wholeNumbers = this._isInt;
			this._slider.onValueChanged.AddListener(new UnityAction<float>(this.ChangeValue));
			this._valueText.text = this._slider.value.ToString();
			this._typeText.text = this._name;
		}

		private void OnValidate()
		{
			if (this._typeText)
			{
				this._typeText.text = this._name;
			}
			if (this._slider)
			{
				this._slider.minValue = this._minValue;
				this._slider.maxValue = this._maxValue;
				this._slider.wholeNumbers = this._isInt;
				this._slider.value = this._defaultValue;
			}
			this.UpdateValueText();
		}

		private void UpdateValueText()
		{
			if (this._slider && this._valueText)
			{
				if (this._isInt)
				{
					this._valueText.text = ((int)this.GetValue()).ToString();
					return;
				}
				this._valueText.text = this.GetValue().ToString(string.Format("N{0}", this._precision));
			}
		}

		private float GetValue()
		{
			return this._slider.value * this._valueMultiplier;
		}

		public void ChangeValue(float value)
		{
			this.UpdateValueText();
			Action<float> onValueChanged = this.OnValueChanged;
			if (onValueChanged == null)
			{
				return;
			}
			onValueChanged(value);
		}

		[SerializeField]
		private string _name;

		[SerializeField]
		private Slider _slider;

		[SerializeField]
		private float _defaultValue;

		[SerializeField]
		private float _minValue;

		[SerializeField]
		private float _maxValue = 1f;

		[SerializeField]
		private bool _isInt;

		[SerializeField]
		private int _precision = 2;

		[SerializeField]
		private float _valueMultiplier = 1f;

		[SerializeField]
		private TextMeshProUGUI _valueText;

		[SerializeField]
		private TextMeshProUGUI _typeText;
	}
}
