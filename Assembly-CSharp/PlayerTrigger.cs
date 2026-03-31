using System;
using GorillaLocomotion;
using UnityEngine;

public abstract class PlayerTrigger : MonoBehaviour
{
	protected virtual void Awake()
	{
		this.triggerCollisionEvents.CompositeTriggerEnter += this.OnCompositeTriggerEnter;
		this.triggerCollisionEvents.CompositeTriggerExit += this.OnCompositeTriggerExit;
	}

	private void OnCompositeTriggerEnter(Collider collider)
	{
		if (!this.isPlayerCollided && collider == GTPlayer.Instance.bodyCollider)
		{
			this.playerCollider = collider;
			this.PlayerEnter();
		}
	}

	private void OnCompositeTriggerExit(Collider collider)
	{
		if (this.isPlayerCollided && collider == this.playerCollider)
		{
			this.PlayerExit();
		}
	}

	protected virtual void PlayerEnter()
	{
		this.isPlayerCollided = true;
	}

	protected virtual void PlayerExit()
	{
		this.playerCollider = null;
		this.isPlayerCollided = false;
	}

	protected bool isPlayerCollided;

	protected Collider playerCollider;

	[SerializeField]
	private CompositeTriggerEvents triggerCollisionEvents;
}
