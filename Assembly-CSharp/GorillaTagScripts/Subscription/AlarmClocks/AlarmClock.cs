using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks
{
	[RequireComponent(typeof(Collider))]
	public sealed class AlarmClock : MonoBehaviour
	{
		public string Key
		{
			get
			{
				return this._key;
			}
		}

		public bool Initialized { get; private set; }

		private void Awake()
		{
			AlarmClock.<Awake>d__14 <Awake>d__;
			<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Awake>d__.<>4__this = this;
			<Awake>d__.<>1__state = -1;
			<Awake>d__.<>t__builder.Start<AlarmClock.<Awake>d__14>(ref <Awake>d__);
		}

		private void OnEnable()
		{
			this.OnActivate.AddListener(new UnityAction(this.OnActivateCallback));
			this.OnDeactivate.AddListener(new UnityAction(this.OnDeactivateCallback));
			base.StartCoroutine(this.Blink());
		}

		private void OnDisable()
		{
			this.OnActivate.RemoveListener(new UnityAction(this.OnActivateCallback));
			this.OnDeactivate.RemoveListener(new UnityAction(this.OnDeactivateCallback));
			base.StopAllCoroutines();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!this.Initialized)
			{
				return;
			}
			if (Time.time < this._lastTouchTime + 1f)
			{
				return;
			}
			if (other.GetComponentInParent<GTPlayer>() == null)
			{
				return;
			}
			if (!SubscriptionManager.IsLocalSubscribed())
			{
				return;
			}
			this._lastTouchTime = Time.time;
			AlarmClockManager.ToggleAlarmClock(this);
		}

		private void OnActivateCallback()
		{
			this._readout.text = "SET";
			this._readout.color = Color.green;
		}

		private void OnDeactivateCallback()
		{
			this._readout.text = "88:88";
			this._readout.color = Color.red;
		}

		private IEnumerator Blink()
		{
			for (;;)
			{
				this._readout.enabled = true;
				yield return new WaitForSeconds(this._onTime);
				this._readout.enabled = false;
				yield return new WaitForSeconds(this._offTime);
			}
			yield break;
		}

		private const float TouchDebouncePeriod = 1f;

		[SerializeField]
		private string _key;

		[SerializeField]
		private TextMeshPro _readout;

		[SerializeField]
		private float _onTime = 1f;

		[SerializeField]
		private float _offTime = 0.2f;

		public UnityEvent OnActivate;

		public UnityEvent OnDeactivate;

		private float _lastTouchTime = float.MinValue;
	}
}
