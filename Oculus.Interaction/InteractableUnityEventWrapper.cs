using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
	public class InteractableUnityEventWrapper : MonoBehaviour
	{
		public UnityEvent WhenHover
		{
			get
			{
				return this._whenHover;
			}
		}

		public UnityEvent WhenUnhover
		{
			get
			{
				return this._whenUnhover;
			}
		}

		public UnityEvent WhenSelect
		{
			get
			{
				return this._whenSelect;
			}
		}

		public UnityEvent WhenUnselect
		{
			get
			{
				return this._whenUnselect;
			}
		}

		public UnityEvent WhenInteractorViewAdded
		{
			get
			{
				return this._whenInteractorViewAdded;
			}
		}

		public UnityEvent WhenInteractorViewRemoved
		{
			get
			{
				return this._whenInteractorViewRemoved;
			}
		}

		public UnityEvent WhenSelectingInteractorViewAdded
		{
			get
			{
				return this._whenSelectingInteractorViewAdded;
			}
		}

		public UnityEvent WhenSelectingInteractorViewRemoved
		{
			get
			{
				return this._whenSelectingInteractorViewRemoved;
			}
		}

		protected virtual void Awake()
		{
			this.InteractableView = (this._interactableView as IInteractableView);
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
				this.InteractableView.WhenStateChanged += this.HandleStateChanged;
				this.InteractableView.WhenInteractorViewAdded += this.HandleInteractorViewAdded;
				this.InteractableView.WhenInteractorViewRemoved += this.HandleInteractorViewRemoved;
				this.InteractableView.WhenSelectingInteractorViewAdded += this.HandleSelectingInteractorViewAdded;
				this.InteractableView.WhenSelectingInteractorViewRemoved += this.HandleSelectingInteractorViewRemoved;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.InteractableView.WhenStateChanged -= this.HandleStateChanged;
				this.InteractableView.WhenInteractorViewAdded -= this.HandleInteractorViewAdded;
				this.InteractableView.WhenInteractorViewRemoved -= this.HandleInteractorViewRemoved;
				this.InteractableView.WhenSelectingInteractorViewAdded -= this.HandleSelectingInteractorViewAdded;
				this.InteractableView.WhenSelectingInteractorViewRemoved -= this.HandleSelectingInteractorViewRemoved;
			}
		}

		private void HandleStateChanged(InteractableStateChangeArgs args)
		{
			switch (args.NewState)
			{
			case InteractableState.Normal:
				if (args.PreviousState == InteractableState.Hover)
				{
					this._whenUnhover.Invoke();
					return;
				}
				break;
			case InteractableState.Hover:
				if (args.PreviousState == InteractableState.Normal)
				{
					this._whenHover.Invoke();
					return;
				}
				if (args.PreviousState == InteractableState.Select)
				{
					this._whenUnselect.Invoke();
					return;
				}
				break;
			case InteractableState.Select:
				if (args.PreviousState == InteractableState.Hover)
				{
					this._whenSelect.Invoke();
				}
				break;
			default:
				return;
			}
		}

		private void HandleInteractorViewAdded(IInteractorView interactorView)
		{
			this.WhenInteractorViewAdded.Invoke();
		}

		private void HandleInteractorViewRemoved(IInteractorView interactorView)
		{
			this.WhenInteractorViewRemoved.Invoke();
		}

		private void HandleSelectingInteractorViewAdded(IInteractorView interactorView)
		{
			this.WhenSelectingInteractorViewAdded.Invoke();
		}

		private void HandleSelectingInteractorViewRemoved(IInteractorView interactorView)
		{
			this.WhenSelectingInteractorViewRemoved.Invoke();
		}

		public void InjectAllInteractableUnityEventWrapper(IInteractableView interactableView)
		{
			this.InjectInteractableView(interactableView);
		}

		public void InjectInteractableView(IInteractableView interactableView)
		{
			this._interactableView = (interactableView as Object);
			this.InteractableView = interactableView;
		}

		[Tooltip("The IInteractableView (Interactable) component to wrap.")]
		[SerializeField]
		[Interface(typeof(IInteractableView), new Type[]
		{

		})]
		private Object _interactableView;

		private IInteractableView InteractableView;

		[Tooltip("Raised when an Interactor hovers over the Interactable.")]
		[SerializeField]
		private UnityEvent _whenHover;

		[Tooltip("Raised when the Interactable was being hovered but now it isn't.")]
		[SerializeField]
		private UnityEvent _whenUnhover;

		[Tooltip("Raised when an Interactor selects the Interactable.")]
		[SerializeField]
		private UnityEvent _whenSelect;

		[Tooltip("Raised when the Interactable was being selected but now it isn't.")]
		[SerializeField]
		private UnityEvent _whenUnselect;

		[Tooltip("Raised each time an Interactor hovers over the Interactable, even if the Interactable is already being hovered by a different Interactor.")]
		[SerializeField]
		private UnityEvent _whenInteractorViewAdded;

		[Tooltip("Raised each time an Interactor stops hovering over the Interactable, even if the Interactable is still being hovered by a different Interactor.")]
		[SerializeField]
		private UnityEvent _whenInteractorViewRemoved;

		[Tooltip("Raised each time an Interactor selects the Interactable, even if the Interactable is already being selected by a different Interactor.")]
		[SerializeField]
		private UnityEvent _whenSelectingInteractorViewAdded;

		[Tooltip("Raised each time an Interactor stops selecting the Interactable, even if the Interactable is still being selected by a different Interactor.")]
		[SerializeField]
		private UnityEvent _whenSelectingInteractorViewRemoved;

		protected bool _started;
	}
}
