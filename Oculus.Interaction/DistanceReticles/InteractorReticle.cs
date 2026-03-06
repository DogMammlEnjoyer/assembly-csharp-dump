using System;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public abstract class InteractorReticle<TReticleData> : MonoBehaviour where TReticleData : class, IReticleData
	{
		public bool VisibleDuringSelect
		{
			get
			{
				return this._visibleDuringSelect;
			}
			set
			{
				this._visibleDuringSelect = value;
			}
		}

		protected abstract IInteractorView Interactor { get; set; }

		protected abstract Component InteractableComponent { get; }

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.Hide();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Interactor.WhenStateChanged += this.HandleStateChanged;
				this.Interactor.WhenPostprocessed += this.HandlePostProcessed;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Interactor.WhenStateChanged -= this.HandleStateChanged;
				this.Interactor.WhenPostprocessed -= this.HandlePostProcessed;
			}
		}

		private void HandleStateChanged(InteractorStateChangeArgs args)
		{
			if (args.NewState == InteractorState.Normal || args.NewState == InteractorState.Disabled)
			{
				this.InteractableUnset();
				return;
			}
			if (args.NewState == InteractorState.Hover && args.PreviousState != InteractorState.Select)
			{
				this.InteractableSet(this.InteractableComponent);
			}
		}

		private void HandlePostProcessed()
		{
			if (this._targetData != null && (this.Interactor.State == InteractorState.Hover || (this.Interactor.State == InteractorState.Select && this._visibleDuringSelect)))
			{
				if (!this._drawn)
				{
					this._drawn = true;
					this.Draw(this._targetData);
				}
				this.Align(this._targetData);
				return;
			}
			if (this._drawn)
			{
				this._drawn = false;
				this.Hide();
			}
		}

		private void InteractableSet(Component interactable)
		{
			if (interactable != null && interactable.TryGetComponent<TReticleData>(out this._targetData))
			{
				this._drawn = false;
				return;
			}
			this._targetData = default(TReticleData);
		}

		private void InteractableUnset()
		{
			if (this._drawn)
			{
				this._drawn = false;
				this.Hide();
			}
			this._targetData = default(TReticleData);
		}

		protected abstract void Draw(TReticleData data);

		protected abstract void Align(TReticleData data);

		protected abstract void Hide();

		[Tooltip("Should the reticle be visible when you're selecting an object?")]
		[SerializeField]
		private bool _visibleDuringSelect;

		protected bool _started;

		protected TReticleData _targetData;

		private bool _drawn;
	}
}
