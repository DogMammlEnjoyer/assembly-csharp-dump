using System;
using UnityEngine;

public class NamedTriggerZone : MonoBehaviour
{
	private void Reset()
	{
		this.ConfigureCollider();
	}

	private void ConfigureCollider()
	{
		Collider collider = base.GetComponent<Collider>();
		if (!collider)
		{
			collider = base.gameObject.AddComponent<BoxCollider>();
		}
		collider.isTrigger = true;
		base.gameObject.layer = LayerMask.NameToLayer("Gorilla Trigger");
	}

	public string TriggerName = "Trigger";
}
