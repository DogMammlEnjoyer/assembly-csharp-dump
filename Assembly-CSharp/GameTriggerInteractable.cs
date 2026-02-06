using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameEntity))]
public class GameTriggerInteractable : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		if (this.interactableWhileGrabbed)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartHolding));
			GameEntity gameEntity2 = this.gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.StopHolding));
		}
		if (this.interactableWhileSnapped)
		{
			GameEntity gameEntity3 = this.gameEntity;
			gameEntity3.OnSnapped = (Action)Delegate.Combine(gameEntity3.OnSnapped, new Action(this.StartHolding));
			GameEntity gameEntity4 = this.gameEntity;
			gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.StopHolding));
		}
	}

	public void StartHolding()
	{
		GameTriggerInteractable.LocalInteractableTriggers.AddIfNew(this);
	}

	public void StopHolding()
	{
		GameTriggerInteractable.LocalInteractableTriggers.RemoveIfContains(this);
	}

	public bool PointWithinInteractableArea(Vector3 point)
	{
		return (this.interactableCenter.position - point).magnitude < this.interactableRadius;
	}

	public void BeginTriggerInteraction(int _handIndex)
	{
		this.triggerInteractionActive = true;
		this.handIndex = _handIndex;
	}

	public void EndTriggerInteraction()
	{
		this.triggerInteractionActive = false;
		this.handIndex = -1;
	}

	public GameEntity gameEntity;

	public Transform interactableCenter;

	public float interactableRadius;

	public bool interactableWhileGrabbed;

	public bool interactableWhileSnapped;

	public bool interactablePermanently;

	public bool interactableOnOthers;

	public bool triggerInteractionActive;

	public int handIndex = -1;

	public static List<GameTriggerInteractable> LocalInteractableTriggers = new List<GameTriggerInteractable>();
}
