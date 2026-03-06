using System;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class Axis1DFingerUseAPI : MonoBehaviour, IFingerUseAPI
	{
		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this.Axis = (this._axis as IAxis1D);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public float GetFingerUseStrength(HandFinger finger)
		{
			if (!this.Hand.GetFingerIsPinching(finger))
			{
				return 0f;
			}
			return this.Axis.Value();
		}

		public void InjectAllUseFingerPinchPressureApi(IHand hand, IAxis1D axis)
		{
			this.InjectHand(hand);
			this.InjectAxis(axis);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectAxis(IAxis1D pinchPressure)
		{
			this.Axis = pinchPressure;
			this._axis = (pinchPressure as Object);
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[FormerlySerializedAs("_pressureAxis")]
		[FormerlySerializedAs("_pinchPressure")]
		[SerializeField]
		[Interface(typeof(IAxis1D), new Type[]
		{

		})]
		private Object _axis;

		protected IHand Hand;

		protected IAxis1D Axis;

		protected bool _started;
	}
}
