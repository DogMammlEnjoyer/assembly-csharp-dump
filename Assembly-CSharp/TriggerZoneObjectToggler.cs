using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZoneObjectToggler : MonoBehaviour
{
	private void Awake()
	{
		this.ToggleObject.SetActive(false);
	}

	private void OnDisable()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			this.HandleExit();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.IsMatchingTrigger(other))
		{
			this.HandleEnter();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (this.IsMatchingTrigger(other))
		{
			this.HandleExit();
		}
	}

	private void HandleEnter()
	{
		if (this._inTriggerZone)
		{
			return;
		}
		this._inTriggerZone = true;
		this.ToggleObject.SetActive(true);
		UnityEvent onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter.Invoke();
	}

	private void HandleExit()
	{
		if (!this._inTriggerZone)
		{
			return;
		}
		this._inTriggerZone = false;
		this.ToggleObject.SetActive(false);
		UnityEvent onExit = this.OnExit;
		if (onExit == null)
		{
			return;
		}
		onExit.Invoke();
	}

	private bool IsMatchingTrigger(Collider other)
	{
		NamedTriggerZone component = other.GetComponent<NamedTriggerZone>();
		return component != null && component.TriggerName == this.TriggerName;
	}

	public string TriggerName = "Trigger";

	public GameObject ToggleObject;

	public UnityEvent OnEnter;

	public UnityEvent OnExit;

	private bool _inTriggerZone;
}
