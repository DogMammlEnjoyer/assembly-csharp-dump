using System;
using UnityEngine;

public abstract class OVRComposition
{
	protected OVRComposition(GameObject parentObject, Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration)
	{
		this.RefreshCameraRig(parentObject, mainCamera);
	}

	public abstract OVRManager.CompositionMethod CompositionMethod();

	public abstract void Update(GameObject gameObject, Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin);

	public abstract void Cleanup();

	public virtual void RecenterPose()
	{
	}

	public void RefreshCameraRig(GameObject parentObject, Camera mainCamera)
	{
		OVRCameraRig ovrcameraRig = mainCamera.GetComponentInParent<OVRCameraRig>();
		if (ovrcameraRig == null)
		{
			ovrcameraRig = parentObject.GetComponent<OVRCameraRig>();
		}
		this.cameraInTrackingSpace = (ovrcameraRig != null && ovrcameraRig.trackingSpace != null);
		this.cameraRig = ovrcameraRig;
		Debug.Log((ovrcameraRig == null) ? "[OVRComposition] CameraRig not found" : "[OVRComposition] CameraRig found");
	}

	public OVRPose ComputeCameraWorldSpacePose(OVRPlugin.CameraExtrinsics extrinsics, Camera mainCamera)
	{
		return this.ComputeCameraTrackingSpacePose(extrinsics).ToWorldSpacePose(mainCamera);
	}

	public OVRPose ComputeCameraTrackingSpacePose(OVRPlugin.CameraExtrinsics extrinsics)
	{
		OVRPose ovrpose = default(OVRPose);
		ovrpose = extrinsics.RelativePose.ToOVRPose();
		if (extrinsics.AttachedToNode != OVRPlugin.Node.None && OVRPlugin.GetNodePresent(extrinsics.AttachedToNode))
		{
			if (this.usingLastAttachedNodePose)
			{
				Debug.Log("The camera attached node get tracked");
				this.usingLastAttachedNodePose = false;
			}
			OVRPose lhs = OVRPlugin.GetNodePose(extrinsics.AttachedToNode, OVRPlugin.Step.Render).ToOVRPose();
			this.lastAttachedNodePose = lhs;
			ovrpose = lhs * ovrpose;
		}
		else if (extrinsics.AttachedToNode != OVRPlugin.Node.None)
		{
			if (!this.usingLastAttachedNodePose)
			{
				Debug.LogWarning("The camera attached node could not be tracked, using the last pose");
				this.usingLastAttachedNodePose = true;
			}
			ovrpose = this.lastAttachedNodePose * ovrpose;
		}
		return ovrpose;
	}

	public bool cameraInTrackingSpace;

	public OVRCameraRig cameraRig;

	protected bool usingLastAttachedNodePose;

	protected OVRPose lastAttachedNodePose;
}
