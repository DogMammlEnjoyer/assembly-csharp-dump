using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oculus.Interaction
{
	public class InteractableGroupView : MonoBehaviour, IInteractableView
	{
		public object Data { get; protected set; }

		public int InteractorsCount
		{
			get
			{
				int num = 0;
				foreach (IInteractableView interactableView in this.Interactables)
				{
					num += interactableView.InteractorViews.Count<IInteractorView>();
				}
				return num;
			}
		}

		public int SelectingInteractorsCount
		{
			get
			{
				int num = 0;
				foreach (IInteractableView interactableView in this.Interactables)
				{
					num += interactableView.SelectingInteractorViews.Count<IInteractorView>();
				}
				return num;
			}
		}

		public IEnumerable<IInteractorView> InteractorViews
		{
			get
			{
				return this.Interactables.SelectMany((IInteractableView interactable) => interactable.InteractorViews).ToList<IInteractorView>();
			}
		}

		public IEnumerable<IInteractorView> SelectingInteractorViews
		{
			get
			{
				return this.Interactables.SelectMany((IInteractableView interactable) => interactable.SelectingInteractorViews).ToList<IInteractorView>();
			}
		}

		public event Action<IInteractorView> WhenInteractorViewAdded = delegate(IInteractorView <p0>)
		{
		};

		public event Action<IInteractorView> WhenInteractorViewRemoved = delegate(IInteractorView <p0>)
		{
		};

		public event Action<IInteractorView> WhenSelectingInteractorViewAdded = delegate(IInteractorView <p0>)
		{
		};

		public event Action<IInteractorView> WhenSelectingInteractorViewRemoved = delegate(IInteractorView <p0>)
		{
		};

		public int MaxInteractors
		{
			get
			{
				int num = 0;
				foreach (IInteractableView interactableView in this.Interactables)
				{
					num = Mathf.Max(interactableView.MaxInteractors, num);
				}
				return num;
			}
		}

		public int MaxSelectingInteractors
		{
			get
			{
				int num = 0;
				foreach (IInteractableView interactableView in this.Interactables)
				{
					num = Mathf.Max(interactableView.MaxSelectingInteractors, num);
				}
				return num;
			}
		}

		public event Action<InteractableStateChangeArgs> WhenStateChanged = delegate(InteractableStateChangeArgs <p0>)
		{
		};

		public InteractableState State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state == value)
				{
					return;
				}
				InteractableState state = this._state;
				this._state = value;
				this.WhenStateChanged(new InteractableStateChangeArgs(state, this._state));
			}
		}

		private void UpdateState()
		{
			if (this.SelectingInteractorsCount > 0)
			{
				this.State = InteractableState.Select;
				return;
			}
			if (this.InteractorsCount > 0)
			{
				this.State = InteractableState.Hover;
				return;
			}
			this.State = InteractableState.Normal;
		}

		protected virtual void Awake()
		{
			if (this._interactables != null)
			{
				this.Interactables = this._interactables.ConvertAll<IInteractableView>((Object mono) => mono as IInteractableView);
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (this.Data == null)
			{
				this._data = this;
				this.Data = this._data;
			}
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				foreach (IInteractableView interactableView in this.Interactables)
				{
					interactableView.WhenStateChanged += this.HandleStateChange;
					interactableView.WhenInteractorViewAdded += this.HandleInteractorViewAdded;
					interactableView.WhenInteractorViewRemoved += this.HandleInteractorViewRemoved;
					interactableView.WhenSelectingInteractorViewAdded += this.HandleSelectingInteractorViewAdded;
					interactableView.WhenSelectingInteractorViewRemoved += this.HandleSelectingInteractorViewRemoved;
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				foreach (IInteractableView interactableView in this.Interactables)
				{
					interactableView.WhenStateChanged -= this.HandleStateChange;
					interactableView.WhenInteractorViewAdded -= this.HandleInteractorViewAdded;
					interactableView.WhenInteractorViewRemoved -= this.HandleInteractorViewRemoved;
					interactableView.WhenSelectingInteractorViewAdded -= this.HandleSelectingInteractorViewAdded;
					interactableView.WhenSelectingInteractorViewRemoved -= this.HandleSelectingInteractorViewRemoved;
				}
			}
		}

		private void HandleStateChange(InteractableStateChangeArgs args)
		{
			this.UpdateState();
		}

		private void HandleInteractorViewAdded(IInteractorView obj)
		{
			this.WhenInteractorViewAdded(obj);
		}

		private void HandleInteractorViewRemoved(IInteractorView obj)
		{
			this.WhenInteractorViewRemoved(obj);
		}

		private void HandleSelectingInteractorViewAdded(IInteractorView obj)
		{
			this.WhenSelectingInteractorViewAdded(obj);
		}

		private void HandleSelectingInteractorViewRemoved(IInteractorView obj)
		{
			this.WhenSelectingInteractorViewRemoved(obj);
		}

		public void InjectAllInteractableGroupView(List<IInteractableView> interactables)
		{
			this.InjectInteractables(interactables);
		}

		public void InjectInteractables(List<IInteractableView> interactables)
		{
			this.Interactables = interactables;
			this._interactables = this.Interactables.ConvertAll<Object>((IInteractableView interactable) => interactable as Object);
		}

		public void InjectOptionalData(object data)
		{
			this._data = (data as Object);
			this.Data = data;
		}

		[SerializeField]
		[Interface(typeof(IInteractableView), new Type[]
		{

		})]
		private List<Object> _interactables;

		private List<IInteractableView> Interactables;

		[SerializeField]
		[Optional]
		private Object _data;

		private InteractableState _state;

		protected bool _started;
	}
}
