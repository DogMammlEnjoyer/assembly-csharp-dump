using System;
using UnityEngine;

public class OVRRuntimeSettings : OVRRuntimeAssetsBase
{
	public OVRHandSkeletonVersion HandSkeletonVersion
	{
		get
		{
			return this.handSkeletonVersion;
		}
		set
		{
			this.handSkeletonVersion = value;
		}
	}

	public static OVRRuntimeSettings Instance
	{
		get
		{
			if (OVRRuntimeSettings._instance == null)
			{
				OVRRuntimeSettings._instance = OVRRuntimeSettings.GetRuntimeSettings();
			}
			return OVRRuntimeSettings._instance;
		}
	}

	public bool RequestsVisualFaceTracking
	{
		get
		{
			return this.requestsVisualFaceTracking;
		}
		set
		{
			this.requestsVisualFaceTracking = value;
		}
	}

	public bool RequestsAudioFaceTracking
	{
		get
		{
			return this.requestsAudioFaceTracking;
		}
		set
		{
			this.requestsAudioFaceTracking = value;
		}
	}

	public bool EnableFaceTrackingVisemesOutput
	{
		get
		{
			return this.enableFaceTrackingVisemesOutput;
		}
		set
		{
			this.enableFaceTrackingVisemesOutput = value;
			OVRPlugin.SetFaceTrackingVisemesEnabled(this.enableFaceTrackingVisemesOutput);
		}
	}

	internal string TelemetryProjectGuid
	{
		get
		{
			if (string.IsNullOrEmpty(this.telemetryProjectGuid))
			{
				this.telemetryProjectGuid = Guid.NewGuid().ToString();
			}
			return this.telemetryProjectGuid;
		}
	}

	public OVRPlugin.BodyTrackingFidelity2 BodyTrackingFidelity
	{
		get
		{
			return this.bodyTrackingFidelity;
		}
		set
		{
			this.bodyTrackingFidelity = value;
		}
	}

	public OVRPlugin.BodyJointSet BodyTrackingJointSet
	{
		get
		{
			return this.bodyTrackingJointSet;
		}
		set
		{
			this.bodyTrackingJointSet = value;
		}
	}

	public bool VisibilityMesh
	{
		get
		{
			return this.allowVisibilityMesh;
		}
		set
		{
			this.allowVisibilityMesh = value;
		}
	}

	public static OVRRuntimeSettings GetRuntimeSettings()
	{
		OVRRuntimeSettings ovrruntimeSettings;
		OVRRuntimeAssetsBase.LoadAsset<OVRRuntimeSettings>(out ovrruntimeSettings, "OculusRuntimeSettings", new Action<OVRRuntimeSettings>(OVRRuntimeSettings.HandleSettingsCreated));
		if (ovrruntimeSettings == null)
		{
			Debug.LogWarning("Failed to load runtime settings. Using default runtime settings instead.");
			ovrruntimeSettings = ScriptableObject.CreateInstance<OVRRuntimeSettings>();
			OVRRuntimeSettings.HandleSettingsCreated(ovrruntimeSettings);
		}
		return ovrruntimeSettings;
	}

	private static void HandleSettingsCreated(OVRRuntimeSettings settings)
	{
	}

	private const string _assetName = "OculusRuntimeSettings";

	private static OVRRuntimeSettings _instance;

	private static readonly OVRHandSkeletonVersion NewProjectDefaultSkeletonVersion = OVRHandSkeletonVersion.OpenXR;

	[SerializeField]
	private OVRHandSkeletonVersion handSkeletonVersion = OVRRuntimeSettings.NewProjectDefaultSkeletonVersion;

	public OVRManager.ColorSpace colorSpace = OVRManager.ColorSpace.P3;

	[SerializeField]
	private bool requestsVisualFaceTracking = true;

	[SerializeField]
	private bool requestsAudioFaceTracking = true;

	[SerializeField]
	private bool enableFaceTrackingVisemesOutput;

	[SerializeField]
	private string telemetryProjectGuid;

	[SerializeField]
	private OVRPlugin.BodyTrackingFidelity2 bodyTrackingFidelity = OVRPlugin.BodyTrackingFidelity2.Low;

	[SerializeField]
	private OVRPlugin.BodyJointSet bodyTrackingJointSet;

	[SerializeField]
	private bool allowVisibilityMesh;

	public bool QuestVisibilityMeshOverriden;
}
