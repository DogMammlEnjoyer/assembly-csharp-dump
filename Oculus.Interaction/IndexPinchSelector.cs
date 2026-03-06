using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class IndexPinchSelector : MonoBehaviour, ISelector
	{
		public IHand Hand { get; private set; }

		public event Action WhenSelected = delegate()
		{
		};

		public event Action WhenUnselected = delegate()
		{
		};

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

		private void HandleHandUpdated()
		{
			bool isIndexFingerPinching = this._isIndexFingerPinching;
			this._isIndexFingerPinching = this.Hand.GetIndexFingerIsPinching();
			if (isIndexFingerPinching != this._isIndexFingerPinching)
			{
				if (this._isIndexFingerPinching)
				{
					this.WhenSelected();
					return;
				}
				this.WhenUnselected();
			}
		}

		public void InjectAllIndexPinchSelector(IHand hand)
		{
			this.InjectHand(hand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[Tooltip("The hand to check.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private bool _isIndexFingerPinching;

		protected bool _started;
	}
}
