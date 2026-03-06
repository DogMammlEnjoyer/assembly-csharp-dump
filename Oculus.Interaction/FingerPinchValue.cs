using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class FingerPinchValue : MonoBehaviour, IAxis1D
	{
		public IHand Hand { get; private set; }

		public HandFinger Finger
		{
			get
			{
				return this._finger;
			}
			set
			{
				this._finger = value;
			}
		}

		public float ChangeRate
		{
			get
			{
				return this._changeRate;
			}
			private set
			{
				this._changeRate = value;
			}
		}

		public AnimationCurve Curve
		{
			get
			{
				return this._curve;
			}
			set
			{
				this._curve = value;
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
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
				this._firstCall = true;
				this.Hand.WhenHandUpdated += this.HandleHandUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandleHandUpdated;
			}
		}

		public float Value()
		{
			return this._value;
		}

		private void HandleHandUpdated()
		{
			float num = this.Hand.GetFingerPinchStrength(this.Finger);
			num = this.Curve.Evaluate(num);
			if (this._firstCall)
			{
				this._firstCall = false;
				this._value = num;
				return;
			}
			this._value = Mathf.Lerp(this._value, num, this._changeRate);
		}

		public void InjectAllFingerPinchValue(IHand hand)
		{
			this.InjectHand(hand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		private HandFinger _finger = HandFinger.Index;

		[SerializeField]
		[Range(0f, 1f)]
		private float _changeRate = 1f;

		[SerializeField]
		private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		private float _value;

		protected bool _started;

		private bool _firstCall;
	}
}
