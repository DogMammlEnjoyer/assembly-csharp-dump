using System;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class HideHandVisualOnGrab : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.HandVisual = (this._handVisual as IHandVisual);
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			GameObject gameObject = null;
			if (this._handGrabInteractor.State == InteractorState.Select)
			{
				HandGrabInteractable selectedInteractable = this._handGrabInteractor.SelectedInteractable;
				gameObject = ((selectedInteractable != null) ? selectedInteractable.gameObject : null);
			}
			if (gameObject)
			{
				ShouldHideHandOnGrab shouldHideHandOnGrab;
				if (gameObject.TryGetComponent<ShouldHideHandOnGrab>(out shouldHideHandOnGrab))
				{
					this.HandVisual.ForceOffVisibility = true;
					return;
				}
			}
			else
			{
				this.HandVisual.ForceOffVisibility = false;
			}
		}

		public void InjectAll(HandGrabInteractor handGrabInteractor, IHandVisual handVisual)
		{
			this.InjectHandGrabInteractor(handGrabInteractor);
			this.InjectHandVisual(handVisual);
		}

		private void InjectHandGrabInteractor(HandGrabInteractor handGrabInteractor)
		{
			this._handGrabInteractor = handGrabInteractor;
		}

		private void InjectHandVisual(IHandVisual handVisual)
		{
			this._handVisual = (handVisual as Object);
			this.HandVisual = handVisual;
		}

		[SerializeField]
		private HandGrabInteractor _handGrabInteractor;

		[SerializeField]
		[Interface(typeof(IHandVisual), new Type[]
		{

		})]
		private Object _handVisual;

		private IHandVisual HandVisual;
	}
}
