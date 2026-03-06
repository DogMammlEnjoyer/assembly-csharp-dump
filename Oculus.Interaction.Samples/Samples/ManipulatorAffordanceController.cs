using System;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class ManipulatorAffordanceController : MonoBehaviour
	{
		private void Start()
		{
			int animatorState = this.GetAnimatorState();
			Animator[] animators = this._animators;
			for (int i = 0; i < animators.Length; i++)
			{
				animators[i].SetInteger("state", animatorState);
			}
			this._grabInteractable.WhenStateChanged += this.HandleInteractableStateChanged;
			this._handGrabInteractable.WhenStateChanged += this.HandleInteractableStateChanged;
			if (this._rayInteractable != null)
			{
				this._rayInteractable.WhenStateChanged += this.HandleInteractableStateChanged;
			}
			this._stateSignaler.WhenStateChanged += this.HandleStateChanged;
			if (this._panelHoverState != null)
			{
				this._panelHoverState.WhenStateChanged += this.PanelHoverStateChanged;
			}
		}

		private void OnDestroy()
		{
			this._grabInteractable.WhenStateChanged -= this.HandleInteractableStateChanged;
			this._handGrabInteractable.WhenStateChanged -= this.HandleInteractableStateChanged;
			if (this._rayInteractable != null)
			{
				this._rayInteractable.WhenStateChanged -= this.HandleInteractableStateChanged;
			}
			this._stateSignaler.WhenStateChanged -= this.HandleStateChanged;
			if (this._panelHoverState != null)
			{
				this._panelHoverState.WhenStateChanged -= this.PanelHoverStateChanged;
			}
		}

		private int GetAnimatorStateFromInteractable(IInteractableView view)
		{
			int result = 0;
			switch (view.State)
			{
			case InteractableState.Normal:
				result = 1;
				break;
			case InteractableState.Hover:
				result = 2;
				break;
			case InteractableState.Select:
				result = 3;
				break;
			}
			return result;
		}

		private int GetAnimatorState()
		{
			int result = 0;
			if (this._panelHoverState != null && !this._panelHoverState.Hovered)
			{
				return result;
			}
			int num = (this._rayInteractable != null) ? this.GetAnimatorStateFromInteractable(this._rayInteractable) : 0;
			return Mathf.Max(new int[]
			{
				this.GetAnimatorStateFromInteractable(this._grabInteractable),
				this.GetAnimatorStateFromInteractable(this._handGrabInteractable),
				num
			});
		}

		private void HandleInteractableStateChanged(InteractableStateChangeArgs args)
		{
			if (args.NewState == InteractableState.Select)
			{
				this._stateSignaler.CurrentState = PanelWithManipulatorsStateSignaler.State.Selected;
			}
			else if (args.PreviousState == InteractableState.Select)
			{
				this._stateSignaler.CurrentState = PanelWithManipulatorsStateSignaler.State.Default;
			}
			int animatorState = this.GetAnimatorState();
			Animator[] animators = this._animators;
			for (int i = 0; i < animators.Length; i++)
			{
				animators[i].SetInteger("state", animatorState);
			}
		}

		private void PanelHoverStateChanged(bool newState)
		{
			int animatorState = this.GetAnimatorState();
			Animator[] animators = this._animators;
			for (int i = 0; i < animators.Length; i++)
			{
				animators[i].SetInteger("state", animatorState);
			}
		}

		private void HandleStateChanged(PanelWithManipulatorsStateSignaler.State state)
		{
			if (state != PanelWithManipulatorsStateSignaler.State.Default)
			{
				bool flag = !(this._rayInteractable != null) || this._rayInteractable.State != InteractableState.Select;
				if (this._grabInteractable.State != InteractableState.Select && this._handGrabInteractable.State != InteractableState.Select && flag)
				{
					this._grabInteractable.enabled = false;
					this._handGrabInteractable.enabled = false;
					if (this._rayInteractable != null)
					{
						this._rayInteractable.enabled = false;
						return;
					}
				}
			}
			else
			{
				this._grabInteractable.enabled = true;
				this._handGrabInteractable.enabled = true;
				if (this._rayInteractable != null)
				{
					this._rayInteractable.enabled = true;
				}
			}
		}

		[SerializeField]
		[Tooltip("The grab interactable for the slate itself (as opposed to the surrounding affordances)")]
		private GrabInteractable _grabInteractable;

		[SerializeField]
		[Tooltip("The hand grab interactable for the slate itself (as opposed to the surrounding affordances)")]
		private HandGrabInteractable _handGrabInteractable;

		[SerializeField]
		[Optional]
		[Tooltip("The ray interactable for the slate itself (as opposed to the surrounding affordances)")]
		private RayInteractable _rayInteractable;

		[SerializeField]
		[Tooltip("The state signaler for the SlateWithManipulators prefab")]
		private PanelWithManipulatorsStateSignaler _stateSignaler;

		[SerializeField]
		[Tooltip("The animators (canonically geometry and opacity) whose 'state' variables should be controlled by this affordance")]
		private Animator[] _animators;

		[SerializeField]
		[Optional]
		[Tooltip("Holds the panel hover state")]
		private PanelHoverState _panelHoverState;
	}
}
