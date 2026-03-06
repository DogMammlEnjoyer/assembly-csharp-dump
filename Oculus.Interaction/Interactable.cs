using System;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.Collections;
using UnityEngine;

namespace Oculus.Interaction
{
	public abstract class Interactable<TInteractor, TInteractable> : MonoBehaviour, IInteractable, IInteractableView where TInteractor : Interactor<TInteractor, TInteractable> where TInteractable : Interactable<TInteractor, TInteractable>
	{
		public object Data { get; protected set; }

		public int MaxInteractors
		{
			get
			{
				return this._maxInteractors;
			}
			set
			{
				this._maxInteractors = value;
			}
		}

		public int MaxSelectingInteractors
		{
			get
			{
				return this._maxSelectingInteractors;
			}
			set
			{
				this._maxSelectingInteractors = value;
			}
		}

		public IEnumerable<IInteractorView> InteractorViews
		{
			get
			{
				return this._interactors.Cast<IInteractorView>();
			}
		}

		public IEnumerable<IInteractorView> SelectingInteractorViews
		{
			get
			{
				return this._selectingInteractors.Cast<IInteractorView>();
			}
		}

		public event Action<InteractableStateChangeArgs> WhenStateChanged = delegate(InteractableStateChangeArgs <p0>)
		{
		};

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

		public MAction<TInteractor> WhenInteractorAdded
		{
			get
			{
				return this._whenInteractorAdded;
			}
		}

		public MAction<TInteractor> WhenInteractorRemoved
		{
			get
			{
				return this._whenInteractorRemoved;
			}
		}

		public MAction<TInteractor> WhenSelectingInteractorAdded
		{
			get
			{
				return this._whenSelectingInteractorAdded;
			}
		}

		public MAction<TInteractor> WhenSelectingInteractorRemoved
		{
			get
			{
				return this._whenSelectingInteractorRemoved;
			}
		}

		public InteractableState State
		{
			get
			{
				return this._state;
			}
			private set
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

		public static InteractableRegistry<TInteractor, TInteractable> Registry
		{
			get
			{
				return Interactable<TInteractor, TInteractable>._registry;
			}
		}

		protected virtual void InteractorAdded(TInteractor interactor)
		{
			this.WhenInteractorViewAdded(interactor);
			this._whenInteractorAdded.Invoke(interactor);
		}

		protected virtual void InteractorRemoved(TInteractor interactor)
		{
			this.WhenInteractorViewRemoved(interactor);
			this._whenInteractorRemoved.Invoke(interactor);
		}

		protected virtual void SelectingInteractorAdded(TInteractor interactor)
		{
			this.WhenSelectingInteractorViewAdded(interactor);
			this._whenSelectingInteractorAdded.Invoke(interactor);
		}

		protected virtual void SelectingInteractorRemoved(TInteractor interactor)
		{
			this.WhenSelectingInteractorViewRemoved(interactor);
			this._whenSelectingInteractorRemoved.Invoke(interactor);
		}

		public IEnumerableHashSet<TInteractor> Interactors
		{
			get
			{
				return this._interactors;
			}
		}

		public IEnumerableHashSet<TInteractor> SelectingInteractors
		{
			get
			{
				return this._selectingInteractors;
			}
		}

		public void AddInteractor(TInteractor interactor)
		{
			this._interactors.Add(interactor);
			this.InteractorAdded(interactor);
			this.UpdateInteractableState();
		}

		public void RemoveInteractor(TInteractor interactor)
		{
			if (!this._interactors.Remove(interactor))
			{
				return;
			}
			interactor.InteractableChangesUpdate();
			this.InteractorRemoved(interactor);
			this.UpdateInteractableState();
		}

		public void AddSelectingInteractor(TInteractor interactor)
		{
			this._selectingInteractors.Add(interactor);
			this.SelectingInteractorAdded(interactor);
			this.UpdateInteractableState();
		}

		public void RemoveSelectingInteractor(TInteractor interactor)
		{
			if (!this._selectingInteractors.Remove(interactor))
			{
				return;
			}
			interactor.InteractableChangesUpdate();
			this.SelectingInteractorRemoved(interactor);
			this.UpdateInteractableState();
		}

		private void UpdateInteractableState()
		{
			if (this.State == InteractableState.Disabled)
			{
				return;
			}
			if (this._selectingInteractors.Count > 0)
			{
				this.State = InteractableState.Select;
				return;
			}
			if (this._interactors.Count > 0)
			{
				this.State = InteractableState.Hover;
				return;
			}
			this.State = InteractableState.Normal;
		}

		public bool CanBeSelectedBy(TInteractor interactor)
		{
			if (this.State == InteractableState.Disabled)
			{
				return false;
			}
			if (this.MaxSelectingInteractors >= 0 && this._selectingInteractors.Count == this.MaxSelectingInteractors)
			{
				return false;
			}
			if (this.MaxInteractors >= 0 && this._interactors.Count == this.MaxInteractors && !this._interactors.Contains(interactor))
			{
				return false;
			}
			if (this.InteractorFilters == null)
			{
				return true;
			}
			using (List<IGameObjectFilter>.Enumerator enumerator = this.InteractorFilters.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Filter(interactor.gameObject))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool HasInteractor(TInteractor interactor)
		{
			return this._interactors.Contains(interactor);
		}

		public bool HasSelectingInteractor(TInteractor interactor)
		{
			return this._selectingInteractors.Contains(interactor);
		}

		public void Enable()
		{
			if (this.State != InteractableState.Disabled)
			{
				return;
			}
			if (this._started)
			{
				Interactable<TInteractor, TInteractable>._registry.Register((TInteractable)((object)this));
				this.State = InteractableState.Normal;
			}
		}

		public void Disable()
		{
			if (this.State == InteractableState.Disabled)
			{
				return;
			}
			if (this._started)
			{
				foreach (TInteractor interactor in new List<TInteractor>(this._selectingInteractors))
				{
					this.RemoveSelectingInteractor(interactor);
				}
				foreach (TInteractor interactor2 in new List<TInteractor>(this._interactors))
				{
					this.RemoveInteractor(interactor2);
				}
				Interactable<TInteractor, TInteractable>._registry.Unregister((TInteractable)((object)this));
				this.State = InteractableState.Disabled;
			}
		}

		public void RemoveInteractorByIdentifier(int id)
		{
			TInteractor tinteractor = default(TInteractor);
			foreach (TInteractor tinteractor2 in this._selectingInteractors)
			{
				if (tinteractor2.Identifier == id)
				{
					tinteractor = tinteractor2;
					break;
				}
			}
			if (tinteractor != null)
			{
				this.RemoveSelectingInteractor(tinteractor);
			}
			tinteractor = default(TInteractor);
			foreach (TInteractor tinteractor3 in this._interactors)
			{
				if (tinteractor3.Identifier == id)
				{
					tinteractor = tinteractor3;
					break;
				}
			}
			if (tinteractor == null)
			{
				return;
			}
			this.RemoveInteractor(tinteractor);
		}

		protected virtual void Awake()
		{
			this.InteractorFilters = this._interactorFilters.ConvertAll<IGameObjectFilter>((Object mono) => mono as IGameObjectFilter);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (this.Data == null)
			{
				if (this._data == null)
				{
					this._data = this;
				}
				this.Data = this._data;
			}
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			this.Enable();
		}

		protected virtual void OnDisable()
		{
			this.Disable();
		}

		protected virtual void SetRegistry(InteractableRegistry<TInteractor, TInteractable> registry)
		{
			if (registry == Interactable<TInteractor, TInteractable>._registry)
			{
				return;
			}
			foreach (TInteractable interactable in Interactable<TInteractor, TInteractable>._registry.List())
			{
				registry.Register(interactable);
				Interactable<TInteractor, TInteractable>._registry.Unregister(interactable);
			}
			Interactable<TInteractor, TInteractable>._registry = registry;
		}

		public void InjectOptionalInteractorFilters(List<IGameObjectFilter> interactorFilters)
		{
			this.InteractorFilters = interactorFilters;
			this._interactorFilters = interactorFilters.ConvertAll<Object>((IGameObjectFilter interactorFilter) => interactorFilter as Object);
		}

		public void InjectOptionalData(object data)
		{
			this._data = (data as Object);
			this.Data = data;
		}

		[SerializeField]
		[Interface(typeof(IGameObjectFilter), new Type[]
		{

		})]
		[Optional]
		private List<Object> _interactorFilters = new List<Object>();

		private List<IGameObjectFilter> InteractorFilters;

		[SerializeField]
		private int _maxInteractors = -1;

		[SerializeField]
		private int _maxSelectingInteractors = -1;

		[SerializeField]
		[Optional]
		private Object _data;

		protected bool _started;

		private EnumerableHashSet<TInteractor> _interactors = new EnumerableHashSet<TInteractor>();

		private EnumerableHashSet<TInteractor> _selectingInteractors = new EnumerableHashSet<TInteractor>();

		private InteractableState _state = InteractableState.Disabled;

		private MultiAction<TInteractor> _whenInteractorAdded = new MultiAction<TInteractor>();

		private MultiAction<TInteractor> _whenInteractorRemoved = new MultiAction<TInteractor>();

		private MultiAction<TInteractor> _whenSelectingInteractorAdded = new MultiAction<TInteractor>();

		private MultiAction<TInteractor> _whenSelectingInteractorRemoved = new MultiAction<TInteractor>();

		private static InteractableRegistry<TInteractor, TInteractable> _registry = new InteractableRegistry<TInteractor, TInteractable>();
	}
}
