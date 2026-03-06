using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public abstract class PointerInteractor<TInteractor, TInteractable> : Interactor<TInteractor, TInteractable> where TInteractor : Interactor<TInteractor, TInteractable> where TInteractable : PointerInteractable<TInteractor, TInteractable>
	{
		protected void GeneratePointerEvent(PointerEventType pointerEventType, TInteractable interactable)
		{
			Pose pose = this.ComputePointerPose();
			if (interactable == null)
			{
				return;
			}
			if (interactable.PointableElement != null)
			{
				if (pointerEventType == PointerEventType.Hover)
				{
					interactable.PointableElement.WhenPointerEventRaised += this.HandlePointerEventRaised;
				}
				else if (pointerEventType == PointerEventType.Unhover)
				{
					interactable.PointableElement.WhenPointerEventRaised -= this.HandlePointerEventRaised;
				}
			}
			interactable.PublishPointerEvent(new PointerEvent(base.Identifier, pointerEventType, pose, base.Data));
		}

		protected virtual void HandlePointerEventRaised(PointerEvent evt)
		{
			if (evt.Identifier == base.Identifier && evt.Type == PointerEventType.Cancel && base.Interactable != null)
			{
				TInteractable interactable = base.Interactable;
				interactable.RemoveInteractorByIdentifier(base.Identifier);
				interactable.PointableElement.WhenPointerEventRaised -= this.HandlePointerEventRaised;
			}
		}

		protected override void InteractableSet(TInteractable interactable)
		{
			base.InteractableSet(interactable);
			this.GeneratePointerEvent(PointerEventType.Hover, interactable);
		}

		protected override void InteractableUnset(TInteractable interactable)
		{
			this.GeneratePointerEvent(PointerEventType.Unhover, interactable);
			base.InteractableUnset(interactable);
		}

		protected override void InteractableSelected(TInteractable interactable)
		{
			base.InteractableSelected(interactable);
			this.GeneratePointerEvent(PointerEventType.Select, interactable);
		}

		protected override void InteractableUnselected(TInteractable interactable)
		{
			this.GeneratePointerEvent(PointerEventType.Unselect, interactable);
			base.InteractableUnselected(interactable);
		}

		protected override void DoPostprocess()
		{
			base.DoPostprocess();
			if (this._interactable != null)
			{
				this.GeneratePointerEvent(PointerEventType.Move, this._interactable);
			}
		}

		protected abstract Pose ComputePointerPose();
	}
}
