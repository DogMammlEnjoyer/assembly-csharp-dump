using System;
using UnityEngine;

public class OVRPointerVisualizer : MonoBehaviour
{
	private void Update()
	{
		this.linePointer.enabled = (OVRInput.GetActiveController() == OVRInput.Controller.Touch);
		Ray ray = new Ray(this.rayTransform.position, this.rayTransform.forward);
		this.linePointer.SetPosition(0, ray.origin);
		this.linePointer.SetPosition(1, ray.origin + ray.direction * this.rayDrawDistance);
	}

	[Tooltip("Object which points with Z axis. E.g. CentreEyeAnchor from OVRCameraRig")]
	public Transform rayTransform;

	[Header("Visual Elements")]
	[Tooltip("Line Renderer used to draw selection ray.")]
	public LineRenderer linePointer;

	[Tooltip("Visually, how far out should the ray be drawn.")]
	public float rayDrawDistance = 2.5f;
}
