using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Samples
{
	public class CountdownTimer : MonoBehaviour
	{
		public bool CountdownOn
		{
			get
			{
				return this._countdownOn;
			}
			set
			{
				if (value && !this._countdownOn)
				{
					this._countdownTimer = this._countdownTime;
				}
				this._countdownOn = value;
			}
		}

		private void Awake()
		{
		}

		private void Update()
		{
			if (!this._countdownOn || this._countdownTimer < 0f)
			{
				this._progressCallback.Invoke(0f);
				return;
			}
			this._countdownTimer -= Time.deltaTime;
			if (this._countdownTimer < 0f)
			{
				this._countdownTimer = -1f;
				this._callback.Invoke();
				this._progressCallback.Invoke(1f);
				return;
			}
			this._progressCallback.Invoke(1f - this._countdownTimer / this._countdownTime);
		}

		[SerializeField]
		[Min(0f)]
		private float _countdownTime = 1f;

		[SerializeField]
		private bool _countdownOn;

		[SerializeField]
		private UnityEvent _callback;

		[SerializeField]
		private UnityEvent<float> _progressCallback;

		private float _countdownTimer;
	}
}
