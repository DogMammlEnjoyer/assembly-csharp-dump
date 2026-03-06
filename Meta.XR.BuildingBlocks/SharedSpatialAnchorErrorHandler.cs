using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

[Feature(Feature.Anchors)]
public class SharedSpatialAnchorErrorHandler : MonoBehaviour
{
	private void Awake()
	{
		if (this.AlertViewHUDPrefab)
		{
			Object.Instantiate<GameObject>(this.AlertViewHUDPrefab);
		}
	}

	public void OnAnchorCreate(OVRSpatialAnchor _, OVRSpatialAnchor.OperationResult result)
	{
		if (result == OVRSpatialAnchor.OperationResult.Failure_SpaceCloudStorageDisabled)
		{
			this.LogWarning(this.cloudPermissionMsg);
			return;
		}
		if (result != OVRSpatialAnchor.OperationResult.Success)
		{
			this.LogWarning("Failed to create the spatial anchor.");
		}
	}

	public void OnAnchorShare(List<OVRSpatialAnchor> _, OVRSpatialAnchor.OperationResult result)
	{
		if (result == OVRSpatialAnchor.OperationResult.Failure_SpaceCloudStorageDisabled)
		{
			this.LogWarning(this.cloudPermissionMsg);
			return;
		}
		if (result != OVRSpatialAnchor.OperationResult.Success)
		{
			this.LogWarning("Failed to share the spatial anchor.");
		}
	}

	public void OnSharedSpatialAnchorLoad(List<OVRSpatialAnchor> loadedAnchors, OVRSpatialAnchor.OperationResult result)
	{
		if (result == OVRSpatialAnchor.OperationResult.Failure_SpaceCloudStorageDisabled)
		{
			this.LogWarning(this.cloudPermissionMsg);
			return;
		}
		if (loadedAnchors == null || loadedAnchors.Count == 0)
		{
			this.LogWarning("Failed to load the spatial anchor(s).");
		}
	}

	public void OnAnchorEraseAll(OVRSpatialAnchor.OperationResult result)
	{
		if (result == OVRSpatialAnchor.OperationResult.Failure)
		{
			this.LogWarning("Failed to erase the spatial anchor(s).");
		}
	}

	public void OnAnchorErase(OVRSpatialAnchor anchor, OVRSpatialAnchor.OperationResult result)
	{
		if (result == OVRSpatialAnchor.OperationResult.Failure)
		{
			this.LogWarning(string.Format("Failed to erase the spatial anchor with uuid: {0}", anchor));
		}
	}

	private void LogWarning(string msg)
	{
		if (!this.DisableRuntimeGUIAlerts)
		{
			AlertViewHUD.PostMessage(msg, AlertViewHUD.MessageType.Error);
		}
		Debug.LogWarning("[SharedSpatialAnchorErrorHandler] " + msg);
	}

	[Tooltip("Disables the message alerts in headset.")]
	public bool DisableRuntimeGUIAlerts;

	[SerializeField]
	private GameObject AlertViewHUDPrefab;

	private string cloudPermissionMsg = "Your headset uses on-device point cloud data to determine its position within your room. To expand your headset’s capabilities and enable features like local multiplayer, you’ll need to share point cloud data with Meta. You can turn off point cloud sharing anytime in Settings.\n\nSettings > Privacy > Device Permissions > Turn on \"Share Point Cloud Data\"";
}
