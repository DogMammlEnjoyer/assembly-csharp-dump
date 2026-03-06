using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.Interaction.HandGrab.Recorder
{
	public class TimerUIControl : MonoBehaviour
	{
		public int DelaySeconds
		{
			get
			{
				return this._delaySeconds;
			}
			set
			{
				this._delaySeconds = Mathf.Clamp(value, 0, this._maxSeconds);
				this.UpdateDisplay(value);
			}
		}

		private void OnEnable()
		{
			this._moreButton.onClick.AddListener(new UnityAction(this.IncreaseTime));
			this._lessButton.onClick.AddListener(new UnityAction(this.DecreaseTime));
		}

		private void OnDisable()
		{
			this._moreButton.onClick.RemoveListener(new UnityAction(this.IncreaseTime));
			this._lessButton.onClick.RemoveListener(new UnityAction(this.DecreaseTime));
		}

		private void Start()
		{
			this.UpdateDisplay(this.DelaySeconds);
		}

		private void IncreaseTime()
		{
			int delaySeconds = this.DelaySeconds;
			this.DelaySeconds = delaySeconds + 1;
		}

		private void DecreaseTime()
		{
			int delaySeconds = this.DelaySeconds;
			this.DelaySeconds = delaySeconds - 1;
		}

		private void UpdateDisplay(int seconds)
		{
			this._timerLabel.text = string.Format("{0}\nseconds", seconds);
			this._lessButton.interactable = (seconds > 0);
			this._moreButton.interactable = (seconds < this._maxSeconds);
		}

		[SerializeField]
		private TextMeshProUGUI _timerLabel;

		[SerializeField]
		private int _delaySeconds = 3;

		[SerializeField]
		private int _maxSeconds = 10;

		[SerializeField]
		private Button _moreButton;

		[SerializeField]
		private Button _lessButton;
	}
}
