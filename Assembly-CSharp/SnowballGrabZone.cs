using System;
using UnityEngine;

public class SnowballGrabZone : HoldableObject
{
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	public override void DropItemCleanup()
	{
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (flag ? EquipmentInteractor.instance.disableLeftGrab : EquipmentInteractor.instance.disableRightGrab)
		{
			return;
		}
		SnowballThrowable snowballThrowable;
		(flag ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
	}

	[GorillaSoundLookup]
	public int materialIndex;
}
