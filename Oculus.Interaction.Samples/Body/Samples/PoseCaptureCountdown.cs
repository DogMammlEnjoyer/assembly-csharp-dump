using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Body.Samples
{
	public class PoseCaptureCountdown : MonoBehaviour
	{
		public void Restart()
		{
			this._timer = this.duration;
			this._timerStart.Invoke();
			if (this._renderer != null)
			{
				this._renderer.material.color = this._resetColor;
			}
		}

		private void Update()
		{
			bool flag = this._timer > 0f;
			if (flag)
			{
				int num = (int)this._timer;
				this._timer -= Time.unscaledDeltaTime;
				if ((int)this._timer < num)
				{
					this._timerSecondTick.Invoke();
				}
			}
			bool flag2 = this._timer > 0f;
			if (flag && !flag2)
			{
				this._timer = 0f;
				this._timeUp.Invoke();
				this._countdownText.text = this._poseText;
				return;
			}
			if (flag2)
			{
				this._countdownText.text = this._timer.ToString("#0.0");
			}
		}

		[SerializeField]
		private UnityEvent _timerStart = new UnityEvent();

		[SerializeField]
		private UnityEvent _timerSecondTick = new UnityEvent();

		[SerializeField]
		private UnityEvent _timeUp = new UnityEvent();

		[SerializeField]
		private TextMeshProUGUI _countdownText;

		[SerializeField]
		private string _poseText = "Capture Pose";

		[SerializeField]
		private float duration = 10f;

		[SerializeField]
		[Optional]
		private Renderer _renderer;

		[SerializeField]
		[Optional]
		private Color _resetColor;

		private float _timer;
	}
}
