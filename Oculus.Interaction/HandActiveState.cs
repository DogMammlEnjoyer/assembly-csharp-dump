using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandActiveState : MonoBehaviour, IActiveState
	{
		public bool Active
		{
			get
			{
				return this.Hand.IsConnected;
			}
		}

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
		}

		public void InjectAllHandActiveState(IHand hand)
		{
			this.InjectHand(hand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[Tooltip("ActiveState will be true while this hand is connected.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private IHand Hand;
	}
}
