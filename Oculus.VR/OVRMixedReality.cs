using System;
using UnityEngine;

internal static class OVRMixedReality
{
	public static void Update(GameObject parentObject, Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
	{
		if (!OVRPlugin.initialized)
		{
			Debug.LogError("OVRPlugin not initialized");
			return;
		}
		if (!OVRPlugin.IsMixedRealityInitialized())
		{
			OVRPlugin.InitializeMixedReality();
			if (!OVRPlugin.IsMixedRealityInitialized())
			{
				Debug.LogError("Unable to initialize OVRPlugin_MixedReality");
				return;
			}
			Debug.Log("OVRPlugin_MixedReality initialized");
		}
		if (!OVRPlugin.IsMixedRealityInitialized())
		{
			return;
		}
		OVRPlugin.UpdateExternalCamera();
		OVRPlugin.UpdateCameraDevices();
		if (OVRMixedReality.currentComposition != null && OVRMixedReality.currentComposition.CompositionMethod() != configuration.compositionMethod)
		{
			OVRMixedReality.currentComposition.Cleanup();
			OVRMixedReality.currentComposition = null;
		}
		if (configuration.compositionMethod == OVRManager.CompositionMethod.External)
		{
			if (OVRMixedReality.currentComposition == null)
			{
				OVRMixedReality.currentComposition = new OVRExternalComposition(parentObject, mainCamera, configuration);
			}
			OVRMixedReality.currentComposition.Update(parentObject, mainCamera, configuration, trackingOrigin);
			return;
		}
		Debug.LogError("Unknown/Unsupported CompositionMethod : " + configuration.compositionMethod.ToString());
	}

	public static void Cleanup()
	{
		if (OVRMixedReality.currentComposition != null)
		{
			OVRMixedReality.currentComposition.Cleanup();
			OVRMixedReality.currentComposition = null;
		}
		if (OVRPlugin.IsMixedRealityInitialized())
		{
			OVRPlugin.ShutdownMixedReality();
		}
	}

	public static void RecenterPose()
	{
		if (OVRMixedReality.currentComposition != null)
		{
			OVRMixedReality.currentComposition.RecenterPose();
		}
	}

	public static bool useFakeExternalCamera = false;

	public static Vector3 fakeCameraFloorLevelPosition = new Vector3(0f, 2f, -0.5f);

	public static Vector3 fakeCameraEyeLevelPosition = OVRMixedReality.fakeCameraFloorLevelPosition - new Vector3(0f, 1.8f, 0f);

	public static Quaternion fakeCameraRotation = Quaternion.LookRotation((new Vector3(0f, OVRMixedReality.fakeCameraFloorLevelPosition.y, 0f) - OVRMixedReality.fakeCameraFloorLevelPosition).normalized, Vector3.up);

	public static float fakeCameraFov = 60f;

	public static float fakeCameraAspect = 1.7777778f;

	public static OVRComposition currentComposition = null;
}
