using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public abstract class InteractorGroup : MonoBehaviour, IInteractor, IInteractorView, IUpdateDriver
	{
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

		public object Data
		{
			get
			{
				return null;
			}
		}

		public bool IsRootDriver { get; set; } = true;

		public abstract bool ShouldHover { get; }

		public abstract bool ShouldUnhover { get; }

		public abstract bool ShouldSelect { get; }

		public abstract bool ShouldUnselect { get; }

		public abstract void Hover();

		public abstract void Unhover();

		public abstract void Select();

		public abstract void Unselect();

		public abstract bool HasCandidate { get; }

		public abstract bool HasInteractable { get; }

		public abstract bool HasSelectedInteractable { get; }

		public abstract object CandidateProperties { get; }

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

		public InteractorState State
		{
			get
			{
				return this._state;
			}
			protected set
			{
				if (this._state != value)
				{
					InteractorStateChangeArgs obj = new InteractorStateChangeArgs(this._state, value);
					this._state = value;
					this.WhenStateChanged(obj);
				}
			}
		}

		public int Identifier
		{
			get
			{
				return this._identifier.ID;
			}
		}

		protected virtual void Awake()
		{
			this._identifier = UniqueIdentifier.Generate(Context.Global.GetInstance(), this);
			this.ActiveState = (this._activeState as IActiveState);
			if (this._interactors != null)
			{
				this.Interactors = this._interactors.FindAll((Object mono) => mono != null).ConvertAll<IInteractor>((Object mono) => mono as IInteractor);
			}
			this.CandidateComparer = (this._candidateComparer as ICandidateComparer);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			for (int i = 0; i < this.Interactors.Count; i++)
			{
				this.Interactors[i].IsRootDriver = false;
			}
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Disable();
			}
		}

		protected virtual void OnDestroy()
		{
			UniqueIdentifier.Release(this._identifier);
		}

		protected static int CompareStates(InteractorState a, InteractorState b)
		{
			if (a == b)
			{
				return 0;
			}
			if ((a == InteractorState.Disabled && b != InteractorState.Disabled) || (a == InteractorState.Normal && (b == InteractorState.Hover || b == InteractorState.Select)) || (a == InteractorState.Hover && b == InteractorState.Select))
			{
				return 1;
			}
			return -1;
		}

		protected bool TryGetBestCandidateIndex(InteractorGroup.InteractorPredicate predicate, out int bestCandidateIndex, int betterThan = -1, int skipIndex = -1)
		{
			bestCandidateIndex = betterThan;
			for (int i = 0; i < this.Interactors.Count; i++)
			{
				if (i != skipIndex)
				{
					IInteractor interactor = this.Interactors[i];
					if (predicate(interactor, i) && this.CompareCandidates(bestCandidateIndex, i) > 0)
					{
						bestCandidateIndex = i;
					}
				}
			}
			return bestCandidateIndex != betterThan;
		}

		protected bool AnyInteractor(InteractorGroup.InteractorPredicate predicate)
		{
			for (int i = 0; i < this.Interactors.Count; i++)
			{
				if (predicate(this.Interactors[i], i))
				{
					return true;
				}
			}
			return false;
		}

		protected int CompareCandidates(int indexA, int indexB)
		{
			if (indexA < 0 && indexB >= 0)
			{
				return 1;
			}
			if (indexA >= 0 && indexB < 0)
			{
				return -1;
			}
			if (indexA < 0 && indexB < 0)
			{
				return 0;
			}
			if (indexA == indexB)
			{
				return 0;
			}
			IInteractor interactor = this.Interactors[indexA];
			IInteractor interactor2 = this.Interactors[indexB];
			if (!interactor.HasCandidate && !interactor2.HasCandidate)
			{
				if (indexA >= indexB)
				{
					return 1;
				}
				return -1;
			}
			else if (interactor.HasCandidate && interactor2.HasCandidate)
			{
				if (this.CandidateComparer == null)
				{
					if (indexA >= indexB)
					{
						return 1;
					}
					return -1;
				}
				else
				{
					if (this.CandidateComparer.Compare(interactor.CandidateProperties, interactor2.CandidateProperties) >= 0)
					{
						return 1;
					}
					return -1;
				}
			}
			else
			{
				if (!interactor.HasCandidate)
				{
					return 1;
				}
				return -1;
			}
		}

		public virtual void Preprocess()
		{
			if (!this.UpdateActiveState())
			{
				this.Disable();
			}
			else
			{
				for (int i = 0; i < this.Interactors.Count; i++)
				{
					this.Interactors[i].Preprocess();
				}
			}
			this.WhenPreprocessed();
		}

		public virtual void Process()
		{
			int num = 0;
			while (this.Interactors != null && num < this.Interactors.Count)
			{
				this.Interactors[num].Process();
				num++;
			}
			this.WhenProcessed();
		}

		public virtual void Postprocess()
		{
			int num = 0;
			while (this.Interactors != null && num < this.Interactors.Count)
			{
				this.Interactors[num].Postprocess();
				num++;
			}
			this.WhenPostprocessed();
		}

		public virtual void ProcessCandidate()
		{
			if (!this.UpdateActiveState())
			{
				return;
			}
			for (int i = 0; i < this.Interactors.Count; i++)
			{
				IInteractor interactor = this.Interactors[i];
				if (interactor.State == InteractorState.Hover || interactor.State == InteractorState.Normal)
				{
					interactor.ProcessCandidate();
				}
			}
		}

		public virtual void Enable()
		{
			if (!this.UpdateActiveState())
			{
				return;
			}
			for (int i = 0; i < this.Interactors.Count; i++)
			{
				this.Interactors[i].Enable();
			}
			if (this.State == InteractorState.Disabled)
			{
				this.State = InteractorState.Normal;
			}
		}

		public virtual void Disable()
		{
			int num = 0;
			while (this.Interactors != null && num < this.Interactors.Count)
			{
				this.Interactors[num].Disable();
				num++;
			}
			this.State = InteractorState.Disabled;
		}

		protected void DisableAllExcept(IInteractor mainInteractor)
		{
			for (int i = 0; i < this.Interactors.Count; i++)
			{
				IInteractor interactor = this.Interactors[i];
				if (interactor != mainInteractor)
				{
					interactor.Disable();
				}
			}
		}

		protected void EnableAllExcept(IInteractor mainInteractor)
		{
			for (int i = 0; i < this.Interactors.Count; i++)
			{
				IInteractor interactor = this.Interactors[i];
				if (interactor != mainInteractor)
				{
					interactor.Enable();
				}
			}
		}

		protected bool UpdateActiveState()
		{
			bool flag = base.isActiveAndEnabled && this._started;
			if (this.ActiveState != null)
			{
				flag = (flag && this.ActiveState.Active);
			}
			return flag;
		}

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
			for (int i = 0; i < this.MaxIterationsPerFrame; i++)
			{
				if (this.State == InteractorState.Normal || this.State == InteractorState.Hover)
				{
					this.ProcessCandidate();
				}
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

		public void InjectAllInteractorGroupBase(List<IInteractor> interactors)
		{
			this.InjectInteractors(interactors);
		}

		public void InjectInteractors(List<IInteractor> interactors)
		{
			this.Interactors = interactors;
			this._interactors = interactors.ConvertAll<Object>((IInteractor i) => i as Object);
		}

		public void InjectOptionalActiveState(IActiveState activeState)
		{
			this.ActiveState = activeState;
			this._activeState = (activeState as Object);
		}

		public void InjectOptionalCandidateComparer(ICandidateComparer candidateComparer)
		{
			this.CandidateComparer = candidateComparer;
			this._candidateComparer = (candidateComparer as Object);
		}

		[SerializeField]
		[Interface(typeof(IInteractor), new Type[]
		{

		})]
		protected List<Object> _interactors;

		public IReadOnlyList<IInteractor> Interactors;

		[SerializeField]
		[Interface(typeof(IActiveState), new Type[]
		{

		})]
		[Optional]
		private Object _activeState;

		private IActiveState ActiveState;

		[SerializeField]
		[Interface(typeof(ICandidateComparer), new Type[]
		{

		})]
		[Optional]
		protected Object _candidateComparer;

		protected ICandidateComparer CandidateComparer;

		[SerializeField]
		private int _maxIterationsPerFrame = 3;

		protected static readonly InteractorGroup.InteractorPredicate TruePredicate = (IInteractor interactor, int index) => true;

		protected static readonly InteractorGroup.InteractorPredicate HasCandidatePredicate = (IInteractor interactor, int index) => interactor.HasCandidate;

		protected static readonly InteractorGroup.InteractorPredicate HasInteractablePredicate = (IInteractor interactor, int index) => interactor.HasInteractable;

		private InteractorState _state = InteractorState.Disabled;

		private UniqueIdentifier _identifier;

		protected bool _started;

		protected delegate bool InteractorPredicate(IInteractor interactor, int index);
	}
}
