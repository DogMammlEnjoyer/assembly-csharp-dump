using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class InteractorActiveState : MonoBehaviour, IActiveState
	{
		public InteractorActiveState.InteractorProperty Property
		{
			get
			{
				return this._property;
			}
			set
			{
				this._property = value;
			}
		}

		public bool Active
		{
			get
			{
				return base.isActiveAndEnabled && (((this._property & InteractorActiveState.InteractorProperty.HasCandidate) != (InteractorActiveState.InteractorProperty)0 && this.Interactor.HasCandidate) || ((this._property & InteractorActiveState.InteractorProperty.HasInteractable) != (InteractorActiveState.InteractorProperty)0 && this.Interactor.HasInteractable) || ((this._property & InteractorActiveState.InteractorProperty.IsSelecting) != (InteractorActiveState.InteractorProperty)0 && this.Interactor.State == InteractorState.Select) || ((this._property & InteractorActiveState.InteractorProperty.HasSelectedInteractable) != (InteractorActiveState.InteractorProperty)0 && this.Interactor.HasSelectedInteractable) || ((this._property & InteractorActiveState.InteractorProperty.IsNormal) != (InteractorActiveState.InteractorProperty)0 && this.Interactor.State == InteractorState.Normal) || ((this._property & InteractorActiveState.InteractorProperty.IsHovering) != (InteractorActiveState.InteractorProperty)0 && this.Interactor.State == InteractorState.Hover) || ((this._property & InteractorActiveState.InteractorProperty.IsDisabled) != (InteractorActiveState.InteractorProperty)0 && this.Interactor.State == InteractorState.Disabled));
			}
		}

		protected virtual void Awake()
		{
			this.Interactor = (this._interactor as IInteractor);
		}

		protected virtual void Start()
		{
		}

		public void InjectAllInteractorActiveState(IInteractor interactor)
		{
			this.InjectInteractor(interactor);
		}

		public void InjectInteractor(IInteractor interactor)
		{
			this._interactor = (interactor as Object);
			this.Interactor = interactor;
		}

		[SerializeField]
		[Interface(typeof(IInteractor), new Type[]
		{

		})]
		private Object _interactor;

		private IInteractor Interactor;

		[SerializeField]
		private InteractorActiveState.InteractorProperty _property;

		[Flags]
		public enum InteractorProperty
		{
			HasCandidate = 1,
			HasInteractable = 2,
			IsSelecting = 4,
			HasSelectedInteractable = 8,
			IsNormal = 16,
			IsHovering = 32,
			IsDisabled = 64
		}
	}
}
