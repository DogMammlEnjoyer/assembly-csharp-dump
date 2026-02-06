using System;
using UnityEngine;

public class GorillaCameraClipPlaneOverrideTrigger : GorillaTriggerBox
{
	private void Awake()
	{
		this.mainCamera = Camera.main;
	}

	public override void OnBoxTriggered()
	{
		this.mainCamera.farClipPlane = this.clipPlaneFarDistanceOverride;
	}

	private Camera mainCamera;

	public float clipPlaneFarDistanceOverride;
}
