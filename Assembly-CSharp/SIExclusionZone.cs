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
		if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
		{
			foreach (SIPlayer siplayer in this.playersInZone)
			{
				if (siplayer != null)
				{
					siplayer.exclusionZoneCount--;
				}
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
			if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
			{
				componentInParent2.exclusionZoneCount++;
			}
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
			if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
			{
				componentInParent2.exclusionZoneCount--;
			}
		}
	}

	public void ClearGadget(SIGadget gadget)
	{
		this.gadgetsInZone.Remove(gadget);
	}

	public SIExclusionType exclusionType = SIExclusionType.AffectsOthers;

	private List<SIGadget> gadgetsInZone = new List<SIGadget>();

	private List<SIPlayer> playersInZone = new List<SIPlayer>();
}
