using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[HelpURL("https://developer.oculus.com/documentation/unity/move-body-tracking/#appendix-b-isdk-integration")]
public class OVRControllerDrivenHandPosesSample : MonoBehaviour
{
	private void Awake()
	{
		switch (OVRManager.instance.controllerDrivenHandPosesType)
		{
		case OVRManager.ControllerDrivenHandPosesType.None:
			this.SetControllerDrivenHandPosesTypeToNone();
			return;
		case OVRManager.ControllerDrivenHandPosesType.ConformingToController:
			this.SetControllerDrivenHandPosesTypeToControllerConforming();
			return;
		case OVRManager.ControllerDrivenHandPosesType.Natural:
			this.SetControllerDrivenHandPosesTypeToNatural();
			return;
		default:
			return;
		}
	}

	public void SetControllerDrivenHandPosesTypeToNone()
	{
		OVRManager.instance.controllerDrivenHandPosesType = OVRManager.ControllerDrivenHandPosesType.None;
		this.buttonOff.interactable = false;
		this.buttonConforming.interactable = true;
		this.buttonNatural.interactable = true;
	}

	public void SetControllerDrivenHandPosesTypeToControllerConforming()
	{
		OVRManager.instance.controllerDrivenHandPosesType = OVRManager.ControllerDrivenHandPosesType.ConformingToController;
		this.buttonOff.interactable = true;
		this.buttonConforming.interactable = false;
		this.buttonNatural.interactable = true;
	}

	public void SetControllerDrivenHandPosesTypeToNatural()
	{
		OVRManager.instance.controllerDrivenHandPosesType = OVRManager.ControllerDrivenHandPosesType.Natural;
		this.buttonOff.interactable = true;
		this.buttonConforming.interactable = true;
		this.buttonNatural.interactable = false;
	}

	[SerializeField]
	private Button buttonOff;

	[SerializeField]
	private Button buttonConforming;

	[SerializeField]
	private Button buttonNatural;

	public OVRCameraRig cameraRig;
}
