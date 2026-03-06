using System;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class UseFingerCurlAPI : MonoBehaviour, IFingerUseAPI
	{
		private IHand Hand { get; set; }

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public float GetFingerUseStrength(HandFinger finger)
		{
			if (this._lastDataVersion != this.Hand.CurrentDataVersion)
			{
				this._lastDataVersion = this.Hand.CurrentDataVersion;
				this._grabAPI.Update(this.Hand);
			}
			return this._grabAPI.GetFingerGrabScore(finger);
		}

		public void InjectAllUseFingerCurlAPI(IHand hand)
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

		private IFingerAPI _grabAPI = new PalmGrabAPI();

		private int _lastDataVersion = -1;

		protected bool _started;
	}
}
