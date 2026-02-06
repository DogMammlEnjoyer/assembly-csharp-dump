using System;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class VRMap
{
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	public virtual void Initialize()
	{
	}

	public void MapOther(float lerpValue)
	{
		Vector3 a;
		Quaternion a2;
		this.rigTarget.GetLocalPositionAndRotation(out a, out a2);
		this.rigTarget.SetLocalPositionAndRotation(Vector3.Lerp(a, this.syncPos, lerpValue), Quaternion.Lerp(a2, this.syncRotation, lerpValue));
	}

	public void MapMine(float ratio, Transform playerOffsetTransform)
	{
		Vector3 current;
		Quaternion rotation;
		this.rigTarget.GetPositionAndRotation(out current, out rotation);
		if (this.overrideTarget != null)
		{
			Vector3 a;
			Quaternion lhs;
			this.overrideTarget.GetPositionAndRotation(out a, out lhs);
			this.rigTarget.SetPositionAndRotation(a + rotation * this.trackingPositionOffset * ratio, lhs * Quaternion.Euler(this.trackingRotationOffset));
		}
		else
		{
			if (!this.hasInputDevice && ConnectedControllerHandler.Instance.GetValidForXRNode(this.vrTargetNode))
			{
				this.myInputDevice = InputDevices.GetDeviceAtXRNode(this.vrTargetNode);
				this.hasInputDevice = true;
				if (this.vrTargetNode != XRNode.LeftHand && this.vrTargetNode != XRNode.RightHand)
				{
					this.hasInputDevice = this.myInputDevice.isValid;
				}
			}
			Quaternion lhs2;
			Vector3 a2;
			if (this.hasInputDevice && this.myInputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out lhs2) && this.myInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out a2))
			{
				this.rigTarget.SetPositionAndRotation(a2 + rotation * this.trackingPositionOffset * ratio + playerOffsetTransform.position, lhs2 * Quaternion.Euler(this.trackingRotationOffset));
				this.rigTarget.RotateAround(playerOffsetTransform.position, Vector3.up, playerOffsetTransform.eulerAngles.y);
			}
		}
		if (this.handholdOverrideTarget != null)
		{
			this.rigTarget.position = Vector3.MoveTowards(current, this.handholdOverrideTarget.position - this.handholdOverrideTargetOffset + rotation * this.trackingPositionOffset * ratio, Time.deltaTime * 2f);
		}
	}

	public Vector3 GetExtrapolatedControllerPosition()
	{
		Vector3 a;
		Quaternion rotation;
		this.rigTarget.GetPositionAndRotation(out a, out rotation);
		return a - rotation * this.trackingPositionOffset * this.rigTarget.lossyScale.x;
	}

	public virtual void MapOtherFinger(float handSync, float lerpValue)
	{
		this.calcT = handSync;
		this.LerpFinger(lerpValue, true);
	}

	public virtual void MapMyFinger(float lerpValue)
	{
	}

	public virtual void LerpFinger(float lerpValue, bool isOther)
	{
	}

	public XRNode vrTargetNode;

	public Transform overrideTarget;

	public Transform rigTarget;

	public Vector3 trackingPositionOffset;

	public Vector3 trackingRotationOffset;

	internal NetworkVector3 netSyncPos = new NetworkVector3();

	public Quaternion syncRotation;

	public float calcT;

	private InputDevice myInputDevice;

	private bool hasInputDevice;

	public Transform handholdOverrideTarget;

	public Vector3 handholdOverrideTargetOffset;
}
