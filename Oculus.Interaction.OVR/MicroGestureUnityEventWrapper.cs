using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
	internal class MicroGestureUnityEventWrapper : MonoBehaviour
	{
		public UnityEvent WhenTapCenter
		{
			get
			{
				return this._whenTapCenter;
			}
		}

		public UnityEvent WhenSwipeUp
		{
			get
			{
				return this._whenSwipeUp;
			}
		}

		public UnityEvent WhenSwipeDown
		{
			get
			{
				return this._whenSwipeDown;
			}
		}

		public UnityEvent WhenSwipeLeft
		{
			get
			{
				return this._whenSwipeLeft;
			}
		}

		public UnityEvent WhenSwipeRight
		{
			get
			{
				return this._whenSwipeRight;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				OVRMicrogestureEventSource ovrMicrogestureEventSource = this._ovrMicrogestureEventSource;
				ovrMicrogestureEventSource.WhenGestureRecognized = (Action<OVRHand.MicrogestureType>)Delegate.Combine(ovrMicrogestureEventSource.WhenGestureRecognized, new Action<OVRHand.MicrogestureType>(this.HandleGesture));
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				OVRMicrogestureEventSource ovrMicrogestureEventSource = this._ovrMicrogestureEventSource;
				ovrMicrogestureEventSource.WhenGestureRecognized = (Action<OVRHand.MicrogestureType>)Delegate.Remove(ovrMicrogestureEventSource.WhenGestureRecognized, new Action<OVRHand.MicrogestureType>(this.HandleGesture));
			}
		}

		private void HandleGesture(OVRHand.MicrogestureType gesture)
		{
			if (gesture == OVRHand.MicrogestureType.SwipeRight)
			{
				this._whenSwipeRight.Invoke();
				return;
			}
			if (gesture == OVRHand.MicrogestureType.SwipeLeft)
			{
				this._whenSwipeLeft.Invoke();
				return;
			}
			if (gesture == OVRHand.MicrogestureType.SwipeForward)
			{
				this._whenSwipeUp.Invoke();
				return;
			}
			if (gesture == OVRHand.MicrogestureType.SwipeBackward)
			{
				this._whenSwipeDown.Invoke();
				return;
			}
			if (gesture == OVRHand.MicrogestureType.ThumbTap)
			{
				this._whenTapCenter.Invoke();
			}
		}

		public void InjectAllMicroGestureUnityEventWrapper(OVRMicrogestureEventSource ovrMicrogestureEventSource)
		{
			this.InjectOvrMicrogestureEventSource(ovrMicrogestureEventSource);
		}

		public void InjectOvrMicrogestureEventSource(OVRMicrogestureEventSource ovrMicrogestureEventSource)
		{
			this._ovrMicrogestureEventSource = ovrMicrogestureEventSource;
		}

		[SerializeField]
		private OVRMicrogestureEventSource _ovrMicrogestureEventSource;

		[SerializeField]
		private UnityEvent _whenTapCenter;

		[SerializeField]
		private UnityEvent _whenSwipeUp;

		[SerializeField]
		private UnityEvent _whenSwipeDown;

		[SerializeField]
		private UnityEvent _whenSwipeLeft;

		[SerializeField]
		private UnityEvent _whenSwipeRight;

		private bool _started;
	}
}
