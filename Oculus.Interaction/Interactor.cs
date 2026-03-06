using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public abstract class Interactor<TInteractor, TInteractable> : MonoBehaviour, IInteractor, IInteractorView, IUpdateDriver where TInteractor : Interactor<TInteractor, TInteractable> where TInteractable : Interactable<TInteractor, TInteractable>
	{
		protected virtual void DoPreprocess()
		{
		}

		protected virtual void DoNormalUpdate()
		{
		}

		protected virtual void DoHoverUpdate()
		{
		}

		protected virtual void DoSelectUpdate()
		{
		}

		protected virtual void DoPostprocess()
		{
		}

		public virtual bool ShouldHover
		{
			get
			{
				return this.State == InteractorState.Normal && (this.HasCandidate || this.ComputeShouldSelect());
			}
		}

		public virtual bool ShouldUnhover
		{
			get
			{
				return this.State == InteractorState.Hover && (this._interactable != this._candidate || this._candidate == null);
			}
		}

		public bool ShouldSelect
		{
			get
			{
				if (this.State != InteractorState.Hover)
				{
					return false;
				}
				if (this._computeShouldSelectOverride != null)
				{
					return this._computeShouldSelectOverride();
				}
				return this._candidate == this._interactable && this.ComputeShouldSelect();
			}
		}

		public bool ShouldUnselect
		{
			get
			{
				if (this.State != InteractorState.Select)
				{
					return false;
				}
				if (this._computeShouldUnselectOverride != null)
				{
					return this._computeShouldUnselectOverride();
				}
				return this.ComputeShouldUnselect();
			}
		}

		protected virtual bool ComputeShouldSelect()
		{
			return this.QueuedSelect;
		}

		protected virtual bool ComputeShouldUnselect()
		{
			return this.QueuedUnselect;
		}

		public event Action<InteractorStateChangeArgs> WhenStateChanged = delegate(InteractorStateChangeArgs <p0>)
		{
		};

		public event Action WhenPreprocessed = delegate()
		{
		};

		public event Action WhenProcessed = delegate()
		{
		};

		public event Action WhenPostprocessed = delegate()
		{
		};

		public int MaxIterationsPerFrame
		{
			get
			{
				return this._maxIterationsPerFrame;
			}
			set
			{
				this._maxIterationsPerFrame = value;
			}
		}

		protected ISelector Selector
		{
			get
			{
				return this._selector;
			}
			set
			{
				if (value != this._selector && this._selector != null && this._started)
				{
					this._selector.WhenSelected -= this.HandleSelected;
					this._selector.WhenUnselected -= this.HandleUnselected;
				}
				this._selector = value;
				if (this._selector != null && this._started)
				{
					this._selector.WhenSelected += this.HandleSelected;
					this._selector.WhenUnselected += this.HandleUnselected;
				}
			}
		}

		private bool QueuedSelect
		{
			get
			{
				return this._selectorQueue.Count > 0 && this._selectorQueue.Peek();
			}
		}

		private bool QueuedUnselect
		{
			get
			{
				return this._selectorQueue.Count > 0 && !this._selectorQueue.Peek();
			}
		}

		public InteractorState State
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
				InteractorState state = this._state;
				this._state = value;
				this.WhenStateChanged(new InteractorStateChangeArgs(state, this._state));
				if (this._nativeId != 5282254251404903456UL && this._state == InteractorState.Select)
				{
					NativeMethods.isdk_NativeComponent_Activate(this._nativeId);
				}
			}
		}

		public virtual object CandidateProperties
		{
			get
			{
				return null;
			}
		}

		public TInteractable Candidate
		{
			get
			{
				return this._candidate;
			}
		}

		public TInteractable Interactable
		{
			get
			{
				return this._interactable;
			}
		}

		public TInteractable SelectedInteractable
		{
			get
			{
				return this._selectedInteractable;
			}
		}

		public bool HasCandidate
		{
			get
			{
				return this._candidate != null;
			}
		}

		public bool HasInteractable
		{
			get
			{
				return this._interactable != null;
			}
		}

		public bool HasSelectedInteractable
		{
			get
			{
				return this._selectedInteractable != null;
			}
		}

		public MAction<TInteractable> WhenInteractableSet
		{
			get
			{
				return this._whenInteractableSet;
			}
		}

		public MAction<TInteractable> WhenInteractableUnset
		{
			get
			{
				return this._whenInteractableUnset;
			}
		}

		public MAction<TInteractable> WhenInteractableSelected
		{
			get
			{
				return this._whenInteractableSelected;
			}
		}

		public MAction<TInteractable> WhenInteractableUnselected
		{
			get
			{
				return this._whenInteractableUnselected;
			}
		}

		protected virtual void InteractableSet(TInteractable interactable)
		{
			this._whenInteractableSet.Invoke(interactable);
		}

		protected virtual void InteractableUnset(TInteractable interactable)
		{
			this._whenInteractableUnset.Invoke(interactable);
		}

		protected virtual void InteractableSelected(TInteractable interactable)
		{
			this._whenInteractableSelected.Invoke(interactable);
		}

		protected virtual void InteractableUnselected(TInteractable interactable)
		{
			this._whenInteractableUnselected.Invoke(interactable);
		}

		public int Identifier
		{
			get
			{
				return this._identifier.ID;
			}
		}

		public object Data { get; protected set; }

		protected virtual void Awake()
		{
			this._identifier = UniqueIdentifier.Generate(Context.Global.GetInstance(), this);
			this.ActiveState = (this._activeState as IActiveState);
			this.CandidateTiebreaker = (this._candidateTiebreaker as IComparer<TInteractable>);
			this.InteractableFilters = this._interactableFilters.ConvertAll<IGameObjectFilter>((Object mono) => mono as IGameObjectFilter);
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
			if (this._started && this._selector != null)
			{
				this._selectorQueue.Clear();
				this._selector.WhenSelected += this.HandleSelected;
				this._selector.WhenUnselected += this.HandleUnselected;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				if (this._selector != null)
				{
					this._selector.WhenSelected -= this.HandleSelected;
					this._selector.WhenUnselected -= this.HandleUnselected;
				}
				this.Disable();
			}
		}

		protected virtual void OnDestroy()
		{
			UniqueIdentifier.Release(this._identifier);
		}

		public virtual void SetComputeCandidateOverride(Func<TInteractable> computeCandidate, bool shouldClearOverrideOnSelect = true)
		{
			this._computeCandidateOverride = computeCandidate;
			this._clearComputeCandidateOverrideOnSelect = shouldClearOverrideOnSelect;
		}

		public virtual void ClearComputeCandidateOverride()
		{
			this._computeCandidateOverride = null;
			this._clearComputeCandidateOverrideOnSelect = false;
		}

		public virtual void SetComputeShouldSelectOverride(Func<bool> computeShouldSelect, bool clearOverrideOnSelect = true)
		{
			this._computeShouldSelectOverride = computeShouldSelect;
			this._clearComputeShouldSelectOverrideOnSelect = clearOverrideOnSelect;
		}

		public virtual void ClearComputeShouldSelectOverride()
		{
			this._computeShouldSelectOverride = null;
			this._clearComputeShouldSelectOverrideOnSelect = false;
		}

		public virtual void SetComputeShouldUnselectOverride(Func<bool> computeShouldUnselect, bool clearOverrideOnUnselect = true)
		{
			this._computeShouldUnselectOverride = computeShouldUnselect;
			this._clearComputeShouldUnselectOverrideOnUnselect = clearOverrideOnUnselect;
		}

		public virtual void ClearComputeShouldUnselectOverride()
		{
			this._computeShouldUnselectOverride = null;
			this._clearComputeShouldUnselectOverrideOnUnselect = false;
		}

		public void Preprocess()
		{
			if (this._started)
			{
				this.DoPreprocess();
			}
			if (!this.UpdateActiveState())
			{
				this.Disable();
			}
			this.WhenPreprocessed();
		}

		public void Process()
		{
			switch (this.State)
			{
			case InteractorState.Normal:
				this.DoNormalUpdate();
				break;
			case InteractorState.Hover:
				this.DoHoverUpdate();
				break;
			case InteractorState.Select:
				this.DoSelectUpdate();
				break;
			}
			this.WhenProcessed();
		}

		public void Postprocess()
		{
			this._selectorQueue.Clear();
			if (this._started)
			{
				this.DoPostprocess();
			}
			this.WhenPostprocessed();
		}

		public virtual void ProcessCandidate()
		{
			this._candidate = default(TInteractable);
			if (!this.UpdateActiveState())
			{
				return;
			}
			if (this._computeCandidateOverride != null)
			{
				this._candidate = this._computeCandidateOverride();
				return;
			}
			this._candidate = this.ComputeCandidate();
		}

		public void InteractableChangesUpdate()
		{
			if (this._selectedInteractable != null && !this._selectedInteractable.HasSelectingInteractor(this as TInteractor))
			{
				this.UnselectInteractable();
			}
			if (this._interactable != null && !this._interactable.HasInteractor(this as TInteractor))
			{
				this.UnsetInteractable();
			}
		}

		public void Hover()
		{
			if (this.State != InteractorState.Normal)
			{
				return;
			}
			this.SetInteractable(this._candidate);
			this.State = InteractorState.Hover;
		}

		public void Unhover()
		{
			if (this.State != InteractorState.Hover)
			{
				return;
			}
			this.UnsetInteractable();
			this.State = InteractorState.Normal;
		}

		public virtual void Select()
		{
			if (this.State != InteractorState.Hover)
			{
				return;
			}
			if (this._clearComputeCandidateOverrideOnSelect)
			{
				this.ClearComputeCandidateOverride();
			}
			if (this._clearComputeShouldSelectOverrideOnSelect)
			{
				this.ClearComputeShouldSelectOverride();
			}
			while (this.QueuedSelect)
			{
				this._selectorQueue.Dequeue();
			}
			if (this.Interactable != null)
			{
				this.SelectInteractable(this.Interactable);
			}
			this.State = InteractorState.Select;
		}

		public virtual void Unselect()
		{
			if (this.State != InteractorState.Select)
			{
				return;
			}
			if (this._clearComputeShouldUnselectOverrideOnUnselect)
			{
				this.ClearComputeShouldUnselectOverride();
			}
			while (this.QueuedUnselect)
			{
				this._selectorQueue.Dequeue();
			}
			this.UnselectInteractable();
			this.State = InteractorState.Hover;
		}

		protected abstract TInteractable ComputeCandidate();

		protected virtual int ComputeCandidateTiebreaker(TInteractable a, TInteractable b)
		{
			if (this.CandidateTiebreaker == null)
			{
				return 0;
			}
			return this.CandidateTiebreaker.Compare(a, b);
		}

		public virtual bool CanSelect(TInteractable interactable)
		{
			if (this.InteractableFilters == null)
			{
				return true;
			}
			using (List<IGameObjectFilter>.Enumerator enumerator = this.InteractableFilters.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Filter(interactable.gameObject))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void SetInteractable(TInteractable interactable)
		{
			if (this._interactable == interactable)
			{
				return;
			}
			this.UnsetInteractable();
			this._interactable = interactable;
			interactable.AddInteractor(this as TInteractor);
			this.InteractableSet(interactable);
		}

		private void UnsetInteractable()
		{
			TInteractable interactable = this._interactable;
			if (interactable == null)
			{
				return;
			}
			this._interactable = default(TInteractable);
			interactable.RemoveInteractor(this as TInteractor);
			this.InteractableUnset(interactable);
		}

		private void SelectInteractable(TInteractable interactable)
		{
			this.Unselect();
			this._selectedInteractable = interactable;
			interactable.AddSelectingInteractor(this as TInteractor);
			this.InteractableSelected(interactable);
		}

		private void UnselectInteractable()
		{
			TInteractable selectedInteractable = this._selectedInteractable;
			if (selectedInteractable == null)
			{
				return;
			}
			this._selectedInteractable = default(TInteractable);
			selectedInteractable.RemoveSelectingInteractor(this as TInteractor);
			this.InteractableUnselected(selectedInteractable);
		}

		public void Enable()
		{
			if (!this.UpdateActiveState())
			{
				return;
			}
			if (this.State == InteractorState.Disabled)
			{
				this.State = InteractorState.Normal;
				this.HandleEnabled();
			}
		}

		public void Disable()
		{
			if (this.State == InteractorState.Disabled)
			{
				return;
			}
			this.HandleDisabled();
			if (this.State == InteractorState.Select)
			{
				this.UnselectInteractable();
				this.State = InteractorState.Hover;
			}
			if (this.State == InteractorState.Hover)
			{
				this.UnsetInteractable();
				this.State = InteractorState.Normal;
			}
			if (this.State == InteractorState.Normal)
			{
				this.State = InteractorState.Disabled;
			}
		}

		protected virtual void HandleEnabled()
		{
		}

		protected virtual void HandleDisabled()
		{
		}

		protected virtual void HandleSelected()
		{
			this._selectorQueue.Enqueue(true);
		}

		protected virtual void HandleUnselected()
		{
			this._selectorQueue.Enqueue(false);
		}

		private bool UpdateActiveState()
		{
			bool flag = base.isActiveAndEnabled && this._started;
			if (this.ActiveState != null)
			{
				flag = (flag && this.ActiveState.Active);
			}
			return flag;
		}

		public bool IsRootDriver { get; set; } = true;

		protected virtual void Update()
		{
			if (!this.IsRootDriver)
			{
				return;
			}
			this.Drive();
		}

		public virtual void Drive()
		{
			this.Preprocess();
			if (!this.UpdateActiveState())
			{
				this.Disable();
				this.Postprocess();
				return;
			}
			this.Enable();
			InteractorState state = this.State;
			for (int i = 0; i < this.MaxIterationsPerFrame; i++)
			{
				if (this.State == InteractorState.Normal || (this.State == InteractorState.Hover && state != InteractorState.Normal))
				{
					this.ProcessCandidate();
				}
				state = this.State;
				this.Process();
				if (this.State == InteractorState.Disabled)
				{
					break;
				}
				if (this.State == InteractorState.Normal)
				{
					if (!this.ShouldHover)
					{
						break;
					}
					this.Hover();
				}
				else if (this.State == InteractorState.Hover)
				{
					if (this.ShouldSelect)
					{
						this.Select();
					}
					else
					{
						if (!this.ShouldUnhover)
						{
							break;
						}
						this.Unhover();
					}
				}
				else if (this.State == InteractorState.Select)
				{
					if (!this.ShouldUnselect)
					{
						break;
					}
					this.Unselect();
				}
			}
			this.Postprocess();
		}

		public void InjectOptionalActiveState(IActiveState activeState)
		{
			this._activeState = (activeState as Object);
			this.ActiveState = activeState;
		}

		public void InjectOptionalInteractableFilters(List<IGameObjectFilter> interactableFilters)
		{
			this.InteractableFilters = interactableFilters;
			this._interactableFilters = interactableFilters.ConvertAll<Object>((IGameObjectFilter interactableFilter) => interactableFilter as Object);
		}

		public void InjectOptionalCandidateTiebreaker(IComparer<TInteractable> candidateTiebreaker)
		{
			this._candidateTiebreaker = (candidateTiebreaker as Object);
			this.CandidateTiebreaker = candidateTiebreaker;
		}

		public void InjectOptionalData(object data)
		{
			this._data = (data as Object);
			this.Data = data;
		}

		private const ulong DefaultNativeId = 5282254251404903456UL;

		protected ulong _nativeId = 5282254251404903456UL;

		[Tooltip("An ActiveState whose value determines if the interactor is enabled or disabled.")]
		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		[Optional]
		private Object _activeState;

		private IActiveState ActiveState;

		[Tooltip("The interactables this interactor can or can't use. Is determined by comparing this interactor's TagSetFilter component(s) to the TagSet component on the interactables.")]
		[SerializeField]
		[Interface(typeof(IGameObjectFilter), new Type[]
		{

		})]
		[Optional]
		private List<Object> _interactableFilters = new List<Object>();

		private List<IGameObjectFilter> InteractableFilters;

		[Tooltip("Custom logic used to determine the best interactable candidate.")]
		[SerializeField]
		[Interface("CandidateTiebreaker")]
		[Optional]
		private Object _candidateTiebreaker;

		private IComparer<TInteractable> CandidateTiebreaker;

		private Func<TInteractable> _computeCandidateOverride;

		private bool _clearComputeCandidateOverrideOnSelect;

		private Func<bool> _computeShouldSelectOverride;

		private bool _clearComputeShouldSelectOverrideOnSelect;

		private Func<bool> _computeShouldUnselectOverride;

		private bool _clearComputeShouldUnselectOverrideOnUnselect;

		private InteractorState _state = InteractorState.Disabled;

		private ISelector _selector;

		[Tooltip("The maximum number of state changes that can occur per frame. For example, the interactor switching from normal to hover or vice-versa counts as one state change.")]
		[SerializeField]
		private int _maxIterationsPerFrame = 3;

		private Queue<bool> _selectorQueue = new Queue<bool>();

		protected TInteractable _candidate;

		protected TInteractable _interactable;

		protected TInteractable _selectedInteractable;

		private MultiAction<TInteractable> _whenInteractableSet = new MultiAction<TInteractable>();

		private MultiAction<TInteractable> _whenInteractableUnset = new MultiAction<TInteractable>();

		private MultiAction<TInteractable> _whenInteractableSelected = new MultiAction<TInteractable>();

		private MultiAction<TInteractable> _whenInteractableUnselected = new MultiAction<TInteractable>();

		private UniqueIdentifier _identifier;

		[Tooltip("Can supply additional data (ex. data from an Interactable about a given Interactor, or vice-versa), or pass data along with events like PointerEvent (ex. the associated Interactor generating the event).")]
		[SerializeField]
		[Optional]
		private Object _data;

		protected bool _started;
	}
}
