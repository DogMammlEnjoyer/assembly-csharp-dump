using System;
using System.Collections.Generic;
using UnityEngine;

public class SIExclusionZone : MonoBehaviour
{
	private void OnDisable()
	{
		foreach (SIGadget sigadget in this.gadgetsInZone)
		{
			if (sigadget != null)
			{
				sigadget.LeaveExclusionZone(this);
			}
		}
		this.gadgetsInZone.Clear();
		foreach (SIPlayer siplayer in this.playersInZone)
		{
			if (siplayer != null)
			{
				siplayer.exclusionZoneCount--;
			}
		}
		this.playersInZone.Clear();
	}

	private void OnTriggerEnter(Collider other)
	{
		SIGadget componentInParent = other.GetComponentInParent<SIGadget>();
		if (componentInParent != null)
		{
			if (!this.gadgetsInZone.Contains(componentInParent))
			{
				this.gadgetsInZone.Add(componentInParent);
			}
			componentInParent.ApplyExclusionZone(this);
		}
		SIPlayer componentInParent2 = other.GetComponentInParent<SIPlayer>();
		if (componentInParent2 != null && !this.playersInZone.Contains(componentInParent2))
		{
			this.playersInZone.Add(componentInParent2);
			componentInParent2.exclusionZoneCount++;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		SIGadget componentInParent = other.GetComponentInParent<SIGadget>();
		if (componentInParent != null && this.gadgetsInZone.Contains(componentInParent))
		{
			componentInParent.LeaveExclusionZone(this);
			this.gadgetsInZone.Remove(componentInParent);
		}
		SIPlayer componentInParent2 = other.GetComponentInParent<SIPlayer>();
		if (componentInParent2 != null && this.playersInZone.Contains(componentInParent2))
		{
			this.playersInZone.Remove(componentInParent2);
			componentInParent2.exclusionZoneCount--;
		}
	}

	public void ClearGadget(SIGadget gadget)
	{
		this.gadgetsInZone.Remove(gadget);
	}

	private List<SIGadget> gadgetsInZone = new List<SIGadget>();

	private List<SIPlayer> playersInZone = new List<SIPlayer>();
}
