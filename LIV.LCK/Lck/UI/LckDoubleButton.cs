using System;
using TMPro;
using UnityEngine;

namespace Liv.Lck.UI
{
	public class LckDoubleButton : MonoBehaviour
	{
		public event Action<float> OnValueChanged;

		public float Value
		{
			get
			{
				return (float)this._currentValue;
			}
		}

		private void OnEnable()
		{
			this.SetMinMaxVisuals();
			this._increase.OnEnter += this.OnEnter;
			this._increase.OnDown += this.OnPressDown;
			this._increase.OnUp += this.OnPressUp;
			this._increase.OnExit += this.OnExit;
			this._decrease.OnEnter += this.OnEnter;
			this._decrease.OnDown += this.OnPressDown;
			this._decrease.OnUp += this.OnPressUp;
			this._decrease.OnExit += this.OnExit;
		}

		private void OnDisable()
		{
			this._increase.OnEnter -= this.OnEnter;
			this._increase.OnDown -= this.OnPressDown;
			this._increase.OnUp -= this.OnPressUp;
			this._increase.OnExit -= this.OnExit;
			this._decrease.OnEnter -= this.OnEnter;
			this._decrease.OnDown -= this.OnPressDown;
			this._decrease.OnUp -= this.OnPressUp;
			this._decrease.OnExit -= this.OnExit;
		}

		public void OnEnter(bool isIncrease)
		{
			if (this.CheckIfValueIsMinOrMax(isIncrease))
			{
				return;
			}
			if (isIncrease)
			{
				this._increase.SetBackgroundColor(this._colors.HighlightedColor);
			}
			else
			{
				this._decrease.SetBackgroundColor(this._colors.HighlightedColor);
			}
			LckDiscreetAudioController audioController = this._audioController;
			if (audioController == null)
			{
				return;
			}
			audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.HoverSound);
		}

		public void OnPressDown(bool isIncrease)
		{
			if (this.CheckIfValueIsMinOrMax(isIncrease))
			{
				return;
			}
			if (isIncrease)
			{
				this._increase.SetBackgroundColor(this._colors.PressedColor);
				if (this._currentValue != this._maxValue)
				{
					this._currentValue += this._increment;
					if (this._currentValue == this._maxValue)
					{
						this.UpdateValueText("MAX");
						this._increase.SetIconColor(this._colors.DisabledColor);
					}
					else
					{
						this.UpdateValueText(this._currentValue.ToString());
					}
				}
				this._visuals.localRotation = Quaternion.Euler(0f, -8f, 0f);
			}
			else
			{
				this._decrease.SetBackgroundColor(this._colors.PressedColor);
				if (this._currentValue != this._minValue)
				{
					this._currentValue -= this._increment;
					if (this._currentValue == this._minValue)
					{
						this._decrease.SetIconColor(this._colors.DisabledColor);
					}
					this.UpdateValueText(this._currentValue.ToString());
				}
				this._visuals.localRotation = Quaternion.Euler(0f, 8f, 0f);
			}
			LckDiscreetAudioController audioController = this._audioController;
			if (audioController != null)
			{
				audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
			}
			Action<float> onValueChanged = this.OnValueChanged;
			if (onValueChanged == null)
			{
				return;
			}
			onValueChanged((float)this._currentValue);
		}

		public void OnPressUp(bool isIncrease, bool usingCollider = false)
		{
			if (this.CheckIfValueIsMinOrMax(isIncrease))
			{
				this._visuals.localRotation = Quaternion.Euler(0f, 0f, 0f);
				this.SetMinMaxVisuals();
				return;
			}
			if (this._currentValue != this._maxValue)
			{
				this._increase.SetIconColor(Color.white);
			}
			if (this._currentValue != this._minValue)
			{
				this._decrease.SetIconColor(Color.white);
			}
			if (isIncrease)
			{
				if (usingCollider)
				{
					this._increase.SetBackgroundColor(this._colors.NormalColor);
				}
				else
				{
					this._increase.SetBackgroundColor(this._colors.HighlightedColor);
				}
			}
			else if (usingCollider)
			{
				this._decrease.SetBackgroundColor(this._colors.NormalColor);
			}
			else
			{
				this._decrease.SetBackgroundColor(this._colors.HighlightedColor);
			}
			LckDiscreetAudioController audioController = this._audioController;
			if (audioController != null)
			{
				audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
			}
			this._visuals.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}

		public void OnExit(bool isIncrease)
		{
			if (this.CheckIfValueIsMinOrMax(isIncrease))
			{
				return;
			}
			if (isIncrease)
			{
				this._increase.SetBackgroundColor(this._colors.NormalColor);
			}
			else
			{
				this._decrease.SetBackgroundColor(this._colors.NormalColor);
			}
			this._visuals.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				this._increase.SetBackgroundColor(this._colors.NormalColor);
				this._decrease.SetBackgroundColor(this._colors.NormalColor);
				this._visuals.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}

		private void UpdateValueText(string text)
		{
			this._valueText.text = text;
		}

		private bool CheckIfValueIsMinOrMax(bool isIncrease)
		{
			return (isIncrease && this._currentValue == this._maxValue) || (!isIncrease && this._currentValue == this._minValue);
		}

		private void SetMinMaxVisuals()
		{
			if (this._currentValue == this._maxValue)
			{
				this._increase.SetIconColor(this._colors.DisabledColor);
				this._increase.SetBackgroundColor(this._colors.NormalColor);
			}
			if (this._currentValue == this._minValue)
			{
				this._decrease.SetIconColor(this._colors.DisabledColor);
				this._decrease.SetBackgroundColor(this._colors.NormalColor);
			}
		}

		private void OnValidate()
		{
			if (this._valueText)
			{
				this.UpdateValueText(this._currentValue.ToString());
			}
		}

		[Header("Settings")]
		[SerializeField]
		private LckButtonColors _colors;

		[SerializeField]
		private int _maxValue;

		[SerializeField]
		private int _minValue;

		[SerializeField]
		private int _currentValue;

		[SerializeField]
		private int _increment;

		[Header("References")]
		[SerializeField]
		private LckDoubleButtonTrigger _increase;

		[SerializeField]
		private LckDoubleButtonTrigger _decrease;

		[SerializeField]
		private Transform _visuals;

		[SerializeField]
		private TMP_Text _valueText;

		[Header("Audio")]
		[SerializeField]
		private LckDiscreetAudioController _audioController;
	}
}
