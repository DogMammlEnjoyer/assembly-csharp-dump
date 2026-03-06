using System;
using System.Collections.Generic;

namespace Oculus.Interaction
{
	public class BestSelectInteractorGroup : InteractorGroup
	{
		public override bool ShouldHover
		{
			get
			{
				return base.State == InteractorState.Normal && base.AnyInteractor(BestSelectInteractorGroup.IsNormalAndShouldHoverPredicate);
			}
		}

		public override bool ShouldUnhover
		{
			get
			{
				return base.State == InteractorState.Hover && (base.AnyInteractor(BestSelectInteractorGroup.IsHoverAndShouldUnhoverPredicate) || !base.AnyInteractor(BestSelectInteractorGroup.IsHover));
			}
		}

		public override bool ShouldSelect
		{
			get
			{
				return base.State == InteractorState.Hover && base.AnyInteractor(BestSelectInteractorGroup.IsHoverAndShouldSelectPredicate);
			}
		}

		public override bool ShouldUnselect
		{
			get
			{
				return base.State == InteractorState.Select && this._bestInteractor != null && this._bestInteractor.ShouldUnselect;
			}
		}

		public override void Hover()
		{
			if (this.TryHover(null))
			{
				base.State = InteractorState.Hover;
			}
		}

		private bool TryHover(Action<IInteractor> whenHover = null)
		{
			bool result = false;
			int index;
			while (base.TryGetBestCandidateIndex(BestSelectInteractorGroup.IsNormalAndShouldHoverPredicate, out index, -1, -1))
			{
				this.Interactors[index].Hover();
				if (whenHover != null)
				{
					whenHover(this.Interactors[index]);
				}
				result = true;
			}
			return result;
		}

		public override void Unhover()
		{
			if (base.State != InteractorState.Hover)
			{
				return;
			}
			int index;
			while (base.TryGetBestCandidateIndex(BestSelectInteractorGroup.IsHoverAndShouldUnhoverPredicate, out index, -1, -1))
			{
				this.Interactors[index].Unhover();
			}
			if (!base.AnyInteractor(BestSelectInteractorGroup.IsHover))
			{
				base.State = InteractorState.Normal;
			}
		}

		public override void Select()
		{
			int index;
			if (base.TryGetBestCandidateIndex(BestSelectInteractorGroup.IsHoverAndShouldSelectPredicate, out index, -1, -1))
			{
				this._bestInteractor = this.Interactors[index];
				this._bestInteractor.Select();
				this._bestInteractor.WhenStateChanged += this.HandleBestInteractorStateChanged;
				base.DisableAllExcept(this._bestInteractor);
			}
			base.State = InteractorState.Select;
		}

		public override void Unselect()
		{
			if (base.State != InteractorState.Select)
			{
				return;
			}
			if (this._bestInteractor != null)
			{
				this._bestInteractor.Unselect();
				if (this._bestInteractor != null && this._bestInteractor.State == InteractorState.Select)
				{
					return;
				}
			}
			base.State = InteractorState.Hover;
		}

		public override void Preprocess()
		{
			base.Preprocess();
			if (this._bestInteractor == null && base.State == InteractorState.Select)
			{
				this.ProcessCandidate();
				base.Process();
				if (this.TryHover(delegate(IInteractor interactor)
				{
					interactor.Process();
				}))
				{
					if (this.ShouldSelect)
					{
						this.Select();
						base.State = InteractorState.Select;
						return;
					}
					base.State = InteractorState.Hover;
					return;
				}
				else
				{
					if (base.State == InteractorState.Select)
					{
						base.State = InteractorState.Hover;
					}
					if (base.State == InteractorState.Hover)
					{
						base.State = InteractorState.Normal;
						return;
					}
				}
			}
			else if (this._bestInteractor != null && base.State == InteractorState.Select && this._bestInteractor.State == InteractorState.Hover)
			{
				base.State = InteractorState.Hover;
			}
		}

		public override void Process()
		{
			base.Process();
			if (base.State == InteractorState.Hover && base.AnyInteractor(BestSelectInteractorGroup.IsNormalAndShouldHoverPredicate))
			{
				if (this.TryHover(delegate(IInteractor interactor)
				{
					interactor.Process();
				}))
				{
					base.State = InteractorState.Hover;
				}
			}
			if (base.State == InteractorState.Hover && base.AnyInteractor(BestSelectInteractorGroup.IsHoverAndShouldUnhoverPredicate))
			{
				int index;
				while (base.TryGetBestCandidateIndex(BestSelectInteractorGroup.IsHoverAndShouldUnhoverPredicate, out index, -1, -1))
				{
					IInteractor interactor2 = this.Interactors[index];
					interactor2.Unhover();
					if (interactor2.State != InteractorState.Hover)
					{
						interactor2.Process();
					}
				}
			}
		}

		public override void Enable()
		{
			if (this._bestInteractor != null)
			{
				this._bestInteractor.Enable();
				return;
			}
			base.Enable();
		}

		public override void Disable()
		{
			this.UnsuscribeBestInteractor();
			base.Disable();
		}

		private void UnsuscribeBestInteractor()
		{
			if (this._bestInteractor != null)
			{
				this._bestInteractor.WhenStateChanged -= this.HandleBestInteractorStateChanged;
				this._bestInteractor = null;
			}
		}

		private void HandleBestInteractorStateChanged(InteractorStateChangeArgs stateChange)
		{
			if (stateChange.PreviousState == InteractorState.Select && stateChange.NewState == InteractorState.Hover)
			{
				IInteractor bestInteractor = this._bestInteractor;
				this.UnsuscribeBestInteractor();
				base.EnableAllExcept(bestInteractor);
			}
		}

		public override bool HasCandidate
		{
			get
			{
				return (this._bestInteractor != null && this._bestInteractor.HasCandidate) || base.AnyInteractor(InteractorGroup.HasCandidatePredicate);
			}
		}

		public override bool HasInteractable
		{
			get
			{
				if (this._bestInteractor != null)
				{
					return this._bestInteractor.HasInteractable;
				}
				return base.AnyInteractor(InteractorGroup.HasInteractablePredicate);
			}
		}

		public override bool HasSelectedInteractable
		{
			get
			{
				return this._bestInteractor != null && this._bestInteractor.HasSelectedInteractable;
			}
		}

		public override object CandidateProperties
		{
			get
			{
				if (this._bestInteractor != null && this._bestInteractor.HasCandidate)
				{
					return this._bestInteractor.CandidateProperties;
				}
				int index;
				if (base.TryGetBestCandidateIndex(InteractorGroup.TruePredicate, out index, -1, -1))
				{
					return this.Interactors[index].CandidateProperties;
				}
				return null;
			}
		}

		public void InjectAllInteractorGroupBestSelect(List<IInteractor> interactors)
		{
			base.InjectAllInteractorGroupBase(interactors);
		}

		private IInteractor _bestInteractor;

		private static readonly InteractorGroup.InteractorPredicate IsNormalAndShouldHoverPredicate = (IInteractor interactor, int index) => interactor.State == InteractorState.Normal && interactor.ShouldHover;

		private static readonly InteractorGroup.InteractorPredicate IsHoverAndShouldUnhoverPredicate = (IInteractor interactor, int index) => interactor.State == InteractorState.Hover && interactor.ShouldUnhover;

		private static readonly InteractorGroup.InteractorPredicate IsHoverAndShouldSelectPredicate = (IInteractor interactor, int index) => interactor.State == InteractorState.Hover && interactor.ShouldSelect;

		private static readonly InteractorGroup.InteractorPredicate IsHover = (IInteractor interactor, int index) => interactor.State == InteractorState.Hover;
	}
}
