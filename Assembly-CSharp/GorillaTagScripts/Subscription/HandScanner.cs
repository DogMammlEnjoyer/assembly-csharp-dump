using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	public class HandScanner : ObservableBehavior
	{
		protected override void ObservableSliceUpdate()
		{
			if (this.scanningRig == null)
			{
				return;
			}
			if (Time.time - this.scanStart > this.scanTime)
			{
				UnityEvent<NetPlayer> unityEvent = this.onHandScanSuccess;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.scanningRig.creator);
				}
				this.scanningRig = null;
			}
		}

		protected override void OnBecameObservable()
		{
			UnityEvent unityEvent = this.onHandScanInRange;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		protected override void OnLostObservable()
		{
			UnityEvent unityEvent = this.onHandScanOutOfRange;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		private void OnTriggerEnter(Collider other)
		{
			SIScannableHand component = other.GetComponent<SIScannableHand>();
			if (component == null)
			{
				return;
			}
			VRRig componentInParent = component.GetComponentInParent<VRRig>();
			if (componentInParent != null && componentInParent.isLocal)
			{
				this.scanningRig = componentInParent;
				this.scanStart = Time.time;
				UnityEvent<NetPlayer> unityEvent = this.onHandScanStart;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.scanningRig.creator);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			SIScannableHand component = other.GetComponent<SIScannableHand>();
			if (component == null)
			{
				return;
			}
			VRRig componentInParent = component.GetComponentInParent<VRRig>();
			if (componentInParent != null && componentInParent == this.scanningRig && componentInParent.isLocal)
			{
				UnityEvent<NetPlayer> unityEvent = this.onHandScanAbort;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.scanningRig.creator);
				}
				this.scanningRig = null;
			}
		}

		[SerializeField]
		private float scanTime = 1f;

		public UnityEvent<NetPlayer> onHandScanStart;

		public UnityEvent<NetPlayer> onHandScanAbort;

		public UnityEvent<NetPlayer> onHandScanSuccess;

		public UnityEvent onHandScanInRange;

		public UnityEvent onHandScanOutOfRange;

		private VRRig scanningRig;

		private float scanStart;
	}
}
