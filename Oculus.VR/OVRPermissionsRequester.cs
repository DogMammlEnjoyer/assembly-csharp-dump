using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public static class OVRPermissionsRequester
{
	public static event Action<string> PermissionGranted;

	public static string GetPermissionId(OVRPermissionsRequester.Permission permission)
	{
		string result;
		switch (permission)
		{
		case OVRPermissionsRequester.Permission.FaceTracking:
			result = "com.oculus.permission.FACE_TRACKING";
			break;
		case OVRPermissionsRequester.Permission.BodyTracking:
			result = "com.oculus.permission.BODY_TRACKING";
			break;
		case OVRPermissionsRequester.Permission.EyeTracking:
			result = "com.oculus.permission.EYE_TRACKING";
			break;
		case OVRPermissionsRequester.Permission.Scene:
			result = "com.oculus.permission.USE_SCENE";
			break;
		case OVRPermissionsRequester.Permission.RecordAudio:
			result = "android.permission.RECORD_AUDIO";
			break;
		default:
			throw new ArgumentOutOfRangeException("permission", permission, null);
		}
		return result;
	}

	private static bool IsPermissionSupportedByPlatform(OVRPermissionsRequester.Permission permission)
	{
		bool result;
		switch (permission)
		{
		case OVRPermissionsRequester.Permission.FaceTracking:
			result = (OVRPlugin.faceTrackingSupported || OVRPlugin.faceTracking2Supported);
			break;
		case OVRPermissionsRequester.Permission.BodyTracking:
			result = OVRPlugin.bodyTrackingSupported;
			break;
		case OVRPermissionsRequester.Permission.EyeTracking:
			result = OVRPlugin.eyeTrackingSupported;
			break;
		case OVRPermissionsRequester.Permission.Scene:
			result = true;
			break;
		case OVRPermissionsRequester.Permission.RecordAudio:
			result = true;
			break;
		default:
			throw new ArgumentOutOfRangeException("permission", permission, null);
		}
		return result;
	}

	public static bool IsPermissionGranted(OVRPermissionsRequester.Permission permission)
	{
		return true;
	}

	public static void Request(IEnumerable<OVRPermissionsRequester.Permission> permissions)
	{
	}

	private static void RequestPermissions(IEnumerable<OVRPermissionsRequester.Permission> permissions)
	{
		List<string> list = new List<string>();
		foreach (OVRPermissionsRequester.Permission permission in permissions)
		{
			if (OVRPermissionsRequester.ShouldRequestPermission(permission))
			{
				list.Add(OVRPermissionsRequester.GetPermissionId(permission));
			}
		}
		if (list.Count > 0)
		{
			UnityEngine.Android.Permission.RequestUserPermissions(list.ToArray(), OVRPermissionsRequester.BuildPermissionCallbacks());
		}
	}

	private static bool ShouldRequestPermission(OVRPermissionsRequester.Permission permission)
	{
		if (!OVRPermissionsRequester.IsPermissionSupportedByPlatform(permission))
		{
			Debug.LogWarning(string.Format("[[{0}] Permission {1} is not supported by the platform and can't be requested.", "OVRPermissionsRequester", permission));
			return false;
		}
		return !OVRPermissionsRequester.IsPermissionGranted(permission);
	}

	private static PermissionCallbacks BuildPermissionCallbacks()
	{
		PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
		permissionCallbacks.PermissionDenied += delegate(string permissionId)
		{
			Debug.LogWarning("[OVRPermissionsRequester] Permission " + permissionId + " was denied.");
		};
		permissionCallbacks.PermissionGranted += delegate(string permissionId)
		{
			Debug.Log("[OVRPermissionsRequester] Permission " + permissionId + " was granted.");
			Action<string> permissionGranted = OVRPermissionsRequester.PermissionGranted;
			if (permissionGranted == null)
			{
				return;
			}
			permissionGranted(permissionId);
		};
		return permissionCallbacks;
	}

	public const string FaceTrackingPermission = "com.oculus.permission.FACE_TRACKING";

	public const string EyeTrackingPermission = "com.oculus.permission.EYE_TRACKING";

	public const string BodyTrackingPermission = "com.oculus.permission.BODY_TRACKING";

	public const string ScenePermission = "com.oculus.permission.USE_SCENE";

	public const string RecordAudioPermission = "android.permission.RECORD_AUDIO";

	public enum Permission
	{
		FaceTracking,
		BodyTracking,
		EyeTracking,
		Scene,
		RecordAudio
	}
}
