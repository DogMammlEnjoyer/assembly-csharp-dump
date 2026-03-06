using System;
using System.Collections.Generic;

namespace Oculus.Interaction
{
	public class BestHoverInteractorGroup : InteractorGroup
	{
		public override bool ShouldHover
		{
			get
			{
				return base.State == InteractorState.Normal && base.AnyInteractor(BestHoverInteractorGroup.IsNormalAndShouldHoverPredicate);
			}
		}

		public override bool ShouldUnhover
		{
			get
			{
				return base.State == InteractorState.Hover && this._bestInteractor != null && this._bestInteractor.ShouldUnhover;
			}
		}

		public override bool ShouldSelect
		{
			get
			{
				return base.State == InteractorState.Hover && this._bestInteractor != null && this._bestInteractor.ShouldSelect;
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
			if (this.TryHover(-1))
			{
				base.State = InteractorState.Hover;
			}
		}

		private bool TryHover(int betterThan = -1)
		{
			int num;
			if (base.TryGetBestCandidateIndex(BestHoverInteractorGroup.IsNormalAndShouldHoverPredicate, out num, betterThan, betterThan))
			{
				if (this._bestInteractor != null)
				{
					this._bestInteractor.Unhover();
				}
				if (this._bestInteractor == null || base.CompareCandidates(this._bestInteractorIndex, num) > 0)
				{
					this.HoverAtIndex(num);
					return true;
				}
			}
			return false;
		}

		private bool TryReplaceHover()
		{
			base.EnableAllExcept(this._bestInteractor);
			this.ProcessCandidate();
			if (this.TryHover(this._bestInteractorIndex))
			{
				return true;
			}
			base.DisableAllExcept(this._bestInteractor);
			return false;
		}

		private void HoverAtIndex(int interactorIndex)
		{
			this.UnsuscribeBestInteractor();
			this._bestInteractorIndex = interactorIndex;
			this._bestInteractor = this.Interactors[this._bestInteractorIndex];
			this._bestInteractor.Hover();
			this._bestInteractor.WhenStateChanged += this.HandleBestInteractorStateChanged;
			base.DisableAllExcept(this._bestInteractor);
		}

		public override void Unhover()
		{
			if (base.State != InteractorState.Hover)
			{
				return;
			}
			if (this._bestInteractor != null)
			{
				this._bestInteractor.Unhover();
				if (this._bestInteractor != null && this._bestInteractor.State == InteractorState.Hover)
				{
					return;
				}
			}
			base.State = InteractorState.Normal;
		}

		public override void Select()
		{
			if (base.State != InteractorState.Hover)
			{
				return;
			}
			this._bestInteractor.Select();
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
			if (this._bestInteractor == null && (base.State == InteractorState.Hover || base.State == InteractorState.Select))
			{
				this.ProcessCandidate();
				base.Process();
				if (this.TryHover(-1))
				{
					if (base.State == InteractorState.Select)
					{
						this._bestInteractor.Process();
						if (this.ShouldSelect)
						{
							this.Select();
							base.State = InteractorState.Select;
							return;
						}
					}
					base.State = InteractorState.Hover;
					return;
				}
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
			else if (this._bestInteractor != null && base.State == InteractorState.Select && this._bestInteractor.State == InteractorState.Hover)
			{
				base.State = InteractorState.Hover;
			}
		}

		public override void Process()
		{
			base.Process();
			if (this._bestInteractor != null && base.State == InteractorState.Hover && this.TryReplaceHover())
			{
				this._bestInteractor.Process();
			}
		}

		private void HandleBestInteractorStateChanged(InteractorStateChangeArgs stateChange)
		{
			if (stateChange.PreviousState == InteractorState.Hover && stateChange.NewState == InteractorState.Normal)
			{
				IInteractor bestInteractor = this._bestInteractor;
				this.UnsuscribeBestInteractor();
				base.EnableAllExcept(bestInteractor);
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
				this._bestInteractorIndex = -1;
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
				return this._bestInteractor != null && this._bestInteractor.HasInteractable;
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

		public void InjectAllInteractorGroupBestHover(List<IInteractor> interactors)
		{
			base.InjectAllInteractorGroupBase(interactors);
		}

		private IInteractor _bestInteractor;

		private int _bestInteractorIndex = -1;

		private static readonly InteractorGroup.InteractorPredicate IsNormalAndShouldHoverPredicate = (IInteractor interactor, int index) => interactor.State == InteractorState.Normal && interactor.ShouldHover;
	}
}
