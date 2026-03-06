using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-wide-motion-mode/")]
public class OVRHandTrackingWideMotionModeSample : MonoBehaviour
{
	private void OnEnable()
	{
		this.fusionToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnFusionToggleChanged));
	}

	private void OnDisable()
	{
		this.fusionToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnFusionToggleChanged));
	}

	private void Update()
	{
		this.UpdateLineRenderer();
	}

	private void UpdateLineRenderer()
	{
		this.leftLinePointer.enabled = false;
		this.rightLinePointer.enabled = false;
		this.UpdateLineRendererForHand(false);
		this.UpdateLineRendererForHand(true);
	}

	private void UpdateLineRendererForHand(bool isLeft)
	{
		Transform transform;
		if (isLeft)
		{
			if (this.handLeft != null && this.handLeft.IsPointerPoseValid)
			{
				transform = this.handLeft.PointerPose;
				if (this.handLeft.GetFingerIsPinching(OVRHand.HandFinger.Index))
				{
					this.inputModule.rayTransform = transform;
				}
			}
			else
			{
				transform = null;
			}
		}
		else
		{
			if (this.handRight != null && this.handRight.IsPointerPoseValid)
			{
				transform = this.handRight.PointerPose;
				if (this.handRight.GetFingerIsPinching(OVRHand.HandFinger.Index))
				{
					this.inputModule.rayTransform = transform;
				}
			}
			transform = ((this.handRight != null && this.handRight.IsPointerPoseValid) ? this.handRight.PointerPose : null);
		}
		if (transform == null)
		{
			return;
		}
		Vector3 position = transform.position;
		object obj = isLeft ? this.leftLinePointer : this.rightLinePointer;
		Ray ray = new Ray(position, transform.rotation * Vector3.forward);
		object obj2 = obj;
		obj2.enabled = true;
		obj2.SetPosition(0, transform.position + ray.direction * 0.05f);
		obj2.SetPosition(1, position + ray.direction * 2.5f);
	}

	private void OnFusionToggleChanged(bool newValue)
	{
		OVRManager.instance.wideMotionModeHandPosesEnabled = newValue;
	}

	[SerializeField]
	public Toggle fusionToggle;

	[SerializeField]
	private LineRenderer leftLinePointer;

	[SerializeField]
	private LineRenderer rightLinePointer;

	[SerializeField]
	private OVRHand handLeft;

	[SerializeField]
	private OVRHand handRight;

	[SerializeField]
	private OVRInputModule inputModule;
}
