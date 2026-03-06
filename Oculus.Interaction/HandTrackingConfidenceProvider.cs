using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandTrackingConfidenceProvider : MonoBehaviour
	{
		private IHand Hand { get; set; }

		protected virtual void Reset()
		{
			this._interactor = (base.GetComponent<IInteractor>() as Object);
			this._hand = (base.GetComponent<IHand>() as Object);
		}

		protected virtual void Awake()
		{
			if (HandTrackingConfidenceProvider._interactorTrackingConfidence == null)
			{
				HandTrackingConfidenceProvider._interactorTrackingConfidence = new Dictionary<int, HandTrackingConfidenceProvider>();
			}
			this.Interactor = (this._interactor as IInteractor);
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
				int identifier = this.Interactor.Identifier;
				if (HandTrackingConfidenceProvider._interactorTrackingConfidence != null && !HandTrackingConfidenceProvider._interactorTrackingConfidence.ContainsKey(identifier))
				{
					HandTrackingConfidenceProvider._interactorTrackingConfidence.Add(identifier, this);
					return;
				}
				Debug.LogError("This interactor was already added to HandTrackingConfidenceProvider. Ensure each interactor is paired just once");
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				int identifier = this.Interactor.Identifier;
				if (HandTrackingConfidenceProvider._interactorTrackingConfidence != null && HandTrackingConfidenceProvider._interactorTrackingConfidence.ContainsKey(identifier))
				{
					HandTrackingConfidenceProvider._interactorTrackingConfidence.Remove(this.Interactor.Identifier);
				}
			}
		}

		public static bool TryGetTrackingConfidence(int key, out bool isTrackingHighConfidence)
		{
			if (HandTrackingConfidenceProvider._interactorTrackingConfidence != null && HandTrackingConfidenceProvider._interactorTrackingConfidence.ContainsKey(key))
			{
				isTrackingHighConfidence = HandTrackingConfidenceProvider._interactorTrackingConfidence[key].Hand.IsHighConfidence;
				return true;
			}
			isTrackingHighConfidence = true;
			return false;
		}

		public void InjectAllHandTrackingConfidenceProvider(IInteractor interactor, IHand hand)
		{
			this.InjectInteractor(interactor);
			this.InjectHand(hand);
		}

		public void InjectInteractor(IInteractor interactor)
		{
			this._interactor = (interactor as Object);
			this.Interactor = interactor;
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[SerializeField]
		[Interface(typeof(IInteractor), new Type[]
		{

		})]
		private Object _interactor;

		private IInteractor Interactor;

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private static Dictionary<int, HandTrackingConfidenceProvider> _interactorTrackingConfidence;

		protected bool _started;
	}
}
