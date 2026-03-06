using System;
using UnityEngine;
using UnityEngine.Events;

public class OVRMicrogestureEventSource : MonoBehaviour
{
	public OVRHand Hand
	{
		get
		{
			return this._hand;
		}
		set
		{
			this._hand = value;
		}
	}

	private void Update()
	{
		OVRHand.MicrogestureType microgestureType = this._hand.GetMicrogestureType();
		if (microgestureType != OVRHand.MicrogestureType.Invalid && microgestureType != OVRHand.MicrogestureType.NoGesture)
		{
			this.RaiseGestureRecognized(microgestureType);
		}
	}

	private void RaiseGestureRecognized(OVRHand.MicrogestureType gesture)
	{
		UnityEvent<OVRHand.MicrogestureType> gestureRecognizedEvent = this.GestureRecognizedEvent;
		if (gestureRecognizedEvent != null)
		{
			gestureRecognizedEvent.Invoke(gesture);
		}
		Action<OVRHand.MicrogestureType> whenGestureRecognized = this.WhenGestureRecognized;
		if (whenGestureRecognized == null)
		{
			return;
		}
		whenGestureRecognized(gesture);
	}

	[SerializeField]
	private OVRHand _hand;

	public UnityEvent<OVRHand.MicrogestureType> GestureRecognizedEvent;

	public Action<OVRHand.MicrogestureType> WhenGestureRecognized = delegate(OVRHand.MicrogestureType <p0>)
	{
	};
}
