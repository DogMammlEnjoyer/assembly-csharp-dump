using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
	public class InteractorUnityEventWrapper : MonoBehaviour
	{
		public UnityEvent WhenDisabled
		{
			get
			{
				return this._whenDisabled;
			}
		}

		public UnityEvent WhenEnabled
		{
			get
			{
				return this._whenEnabled;
			}
		}

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

		public UnityEvent WhenPreprocessed
		{
			get
			{
				return this._whenPreprocessed;
			}
		}

		public UnityEvent WhenProcessed
		{
			get
			{
				return this._whenProcessed;
			}
		}

		public UnityEvent WhenPostprocessed
		{
			get
			{
				return this._whenPostprocessed;
			}
		}

		protected virtual void Awake()
		{
			this.InteractorView = (this._interactorView as IInteractorView);
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
				this.InteractorView.WhenStateChanged += this.HandleStateChanged;
				this.InteractorView.WhenPreprocessed += this.HandlePreprocessed;
				this.InteractorView.WhenProcessed += this.HandleProcessed;
				this.InteractorView.WhenPostprocessed += this.HandlePostprocessed;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.InteractorView.WhenStateChanged -= this.HandleStateChanged;
				this.InteractorView.WhenPreprocessed -= this.HandlePreprocessed;
				this.InteractorView.WhenProcessed -= this.HandleProcessed;
				this.InteractorView.WhenPostprocessed -= this.HandlePostprocessed;
			}
		}

		private void HandleStateChanged(InteractorStateChangeArgs args)
		{
			switch (args.NewState)
			{
			case InteractorState.Normal:
				if (args.PreviousState == InteractorState.Hover)
				{
					this._whenUnhover.Invoke();
					return;
				}
				if (args.PreviousState == InteractorState.Disabled)
				{
					this._whenEnabled.Invoke();
					return;
				}
				break;
			case InteractorState.Hover:
				if (args.PreviousState == InteractorState.Normal)
				{
					this._whenHover.Invoke();
					return;
				}
				if (args.PreviousState == InteractorState.Select)
				{
					this._whenUnselect.Invoke();
					return;
				}
				break;
			case InteractorState.Select:
				if (args.PreviousState == InteractorState.Hover)
				{
					this._whenSelect.Invoke();
				}
				break;
			case InteractorState.Disabled:
				this._whenDisabled.Invoke();
				return;
			default:
				return;
			}
		}

		private void HandlePreprocessed()
		{
			this._whenPreprocessed.Invoke();
		}

		private void HandleProcessed()
		{
			this._whenProcessed.Invoke();
		}

		private void HandlePostprocessed()
		{
			this._whenPostprocessed.Invoke();
		}

		public void InjectAllInteractorUnityEventWrapper(IInteractorView interactorView)
		{
			this.InjectInteractorView(interactorView);
		}

		public void InjectInteractorView(IInteractorView interactorView)
		{
			this._interactorView = (interactorView as Object);
			this.InteractorView = interactorView;
		}

		[Tooltip("The IInteractorView (Interactor) component to wrap.")]
		[SerializeField]
		[Interface(typeof(IInteractorView), new Type[]
		{

		})]
		private Object _interactorView;

		private IInteractorView InteractorView;

		[Tooltip("Raised when the Interactor is enabled.")]
		[SerializeField]
		private UnityEvent _whenEnabled;

		[Tooltip("Raised when the Interactor is disabled.")]
		[SerializeField]
		private UnityEvent _whenDisabled;

		[Tooltip("Raised when the Interactor is hovering over an Interactable.")]
		[SerializeField]
		private UnityEvent _whenHover;

		[Tooltip("Raised when the stops hovering over an Interactable.")]
		[SerializeField]
		private UnityEvent _whenUnhover;

		[Tooltip("Raised when the Interactor selects an Interactable.")]
		[SerializeField]
		private UnityEvent _whenSelect;

		[Tooltip("Raised when the Interactor stops selecting an Interactable.")]
		[SerializeField]
		private UnityEvent _whenUnselect;

		[Space]
		[Tooltip("Raised when the Interactor preprocesses.")]
		[SerializeField]
		private UnityEvent _whenPreprocessed;

		[Tooltip("Raised when the Interactor processes.")]
		[SerializeField]
		private UnityEvent _whenProcessed;

		[Tooltip("Raised when the Interactor processes.")]
		[SerializeField]
		private UnityEvent _whenPostprocessed;

		protected bool _started;
	}
}
