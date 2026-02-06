using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

public class FreeHoverboardHandle : HoldableObject
{
	private void Awake()
	{
		this.hasParentBoard = (this.parentFreeBoard != null);
	}

	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		if (Time.frameCount > this.noHapticsUntilFrame)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.noHapticsUntilFrame = Time.frameCount + 1;
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (this.hasParentBoard)
		{
			FreeHoverboardManager.instance.SendGrabBoardRPC(this.parentFreeBoard);
			Transform transform = flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget;
			Quaternion rot = transform.InverseTransformRotation(base.transform.rotation);
			Vector3 pos = transform.InverseTransformPoint(base.transform.position);
			GTPlayer.Instance.GrabPersonalHoverboard(flag, pos, rot, this.parentFreeBoard.boardColor);
			return;
		}
		Quaternion rot2 = flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight;
		Vector3 pos2 = flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight;
		GTPlayer.Instance.GrabPersonalHoverboard(flag, pos2, rot2, VRRig.LocalRig.playerColor);
	}

	public override void DropItemCleanup()
	{
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		throw new NotImplementedException();
	}

	[SerializeField]
	private FreeHoverboardInstance parentFreeBoard;

	private bool hasParentBoard;

	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	[SerializeField]
	private Vector3 defaultHoldPosRight;

	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	private int noHapticsUntilFrame = -1;
}
