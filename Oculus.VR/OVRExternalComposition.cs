using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OVRExternalComposition : OVRComposition
{
	public override OVRManager.CompositionMethod CompositionMethod()
	{
		return OVRManager.CompositionMethod.External;
	}

	public OVRExternalComposition(GameObject parentObject, Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration) : base(parentObject, mainCamera, configuration)
	{
		this.RefreshCameraObjects(parentObject, mainCamera, configuration);
	}

	private void RefreshCameraObjects(GameObject parentObject, Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration)
	{
		if (mainCamera.gameObject != this.previousMainCameraObject)
		{
			Debug.LogFormat("[OVRExternalComposition] Camera refreshed. Rebind camera to {0}", new object[]
			{
				mainCamera.gameObject.name
			});
			OVRCompositionUtil.SafeDestroy(ref this.backgroundCameraGameObject);
			this.backgroundCamera = null;
			OVRCompositionUtil.SafeDestroy(ref this.foregroundCameraGameObject);
			this.foregroundCamera = null;
			base.RefreshCameraRig(parentObject, mainCamera);
			if (configuration.instantiateMixedRealityCameraGameObject != null)
			{
				this.backgroundCameraGameObject = configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject, OVRManager.MrcCameraType.Background);
			}
			else
			{
				this.backgroundCameraGameObject = Object.Instantiate<GameObject>(mainCamera.gameObject);
			}
			this.backgroundCameraGameObject.name = "OculusMRC_BackgroundCamera";
			this.backgroundCameraGameObject.transform.parent = (this.cameraInTrackingSpace ? this.cameraRig.trackingSpace : parentObject.transform);
			if (this.backgroundCameraGameObject.GetComponent<AudioListener>())
			{
				Object.Destroy(this.backgroundCameraGameObject.GetComponent<AudioListener>());
			}
			if (this.backgroundCameraGameObject.GetComponent<OVRManager>())
			{
				Object.Destroy(this.backgroundCameraGameObject.GetComponent<OVRManager>());
			}
			this.backgroundCamera = this.backgroundCameraGameObject.GetComponent<Camera>();
			this.backgroundCamera.tag = "Untagged";
			UniversalAdditionalCameraData universalAdditionalCameraData = this.backgroundCamera.GetUniversalAdditionalCameraData();
			if (universalAdditionalCameraData != null)
			{
				universalAdditionalCameraData.allowXRRendering = false;
			}
			this.backgroundCamera.depth = 99990f;
			this.backgroundCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
			this.backgroundCamera.cullingMask = ((this.backgroundCamera.cullingMask & ~configuration.extraHiddenLayers) | configuration.extraVisibleLayers);
			if (configuration.instantiateMixedRealityCameraGameObject != null)
			{
				this.foregroundCameraGameObject = configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject, OVRManager.MrcCameraType.Foreground);
			}
			else
			{
				this.foregroundCameraGameObject = Object.Instantiate<GameObject>(mainCamera.gameObject);
			}
			this.foregroundCameraGameObject.name = "OculusMRC_ForgroundCamera";
			this.foregroundCameraGameObject.transform.parent = (this.cameraInTrackingSpace ? this.cameraRig.trackingSpace : parentObject.transform);
			if (this.foregroundCameraGameObject.GetComponent<AudioListener>())
			{
				Object.Destroy(this.foregroundCameraGameObject.GetComponent<AudioListener>());
			}
			if (this.foregroundCameraGameObject.GetComponent<OVRManager>())
			{
				Object.Destroy(this.foregroundCameraGameObject.GetComponent<OVRManager>());
			}
			this.foregroundCamera = this.foregroundCameraGameObject.GetComponent<Camera>();
			this.foregroundCamera.tag = "Untagged";
			UniversalAdditionalCameraData universalAdditionalCameraData2 = this.foregroundCamera.GetUniversalAdditionalCameraData();
			if (universalAdditionalCameraData2 != null)
			{
				universalAdditionalCameraData2.allowXRRendering = false;
			}
			this.foregroundCamera.depth = this.backgroundCamera.depth + 1f;
			this.foregroundCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
			this.foregroundCamera.clearFlags = CameraClearFlags.Color;
			this.foregroundCamera.backgroundColor = configuration.externalCompositionBackdropColorRift;
			this.foregroundCamera.cullingMask = ((this.foregroundCamera.cullingMask & ~configuration.extraHiddenLayers) | configuration.extraVisibleLayers);
			this.previousMainCameraObject = mainCamera.gameObject;
		}
	}

	public override void Update(GameObject gameObject, Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
	{
		this.RefreshCameraObjects(gameObject, mainCamera, configuration);
		OVRPlugin.SetHandNodePoseStateLatency(0.0);
		OVRPose lhs = OVRPlugin.GetTrackingTransformRelativePose(OVRPlugin.TrackingOrigin.Stage).ToOVRPose().Inverse();
		OVRPose ovrpose = lhs * OVRPlugin.GetNodePose(OVRPlugin.Node.Head, OVRPlugin.Step.Render).ToOVRPose();
		OVRPose ovrpose2 = lhs * OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, OVRPlugin.Step.Render).ToOVRPose();
		OVRPose ovrpose3 = lhs * OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, OVRPlugin.Step.Render).ToOVRPose();
		OVRPlugin.Media.SetMrcHeadsetControllerPose(ovrpose.ToPosef(), ovrpose2.ToPosef(), ovrpose3.ToPosef());
		this.backgroundCamera.clearFlags = mainCamera.clearFlags;
		this.backgroundCamera.backgroundColor = mainCamera.backgroundColor;
		if (configuration.dynamicCullingMask)
		{
			this.backgroundCamera.cullingMask = ((mainCamera.cullingMask & ~configuration.extraHiddenLayers) | configuration.extraVisibleLayers);
		}
		this.backgroundCamera.nearClipPlane = mainCamera.nearClipPlane;
		this.backgroundCamera.farClipPlane = mainCamera.farClipPlane;
		if (configuration.dynamicCullingMask)
		{
			this.foregroundCamera.cullingMask = ((mainCamera.cullingMask & ~configuration.extraHiddenLayers) | configuration.extraVisibleLayers);
		}
		this.foregroundCamera.nearClipPlane = mainCamera.nearClipPlane;
		this.foregroundCamera.farClipPlane = mainCamera.farClipPlane;
		if (OVRMixedReality.useFakeExternalCamera || OVRPlugin.GetExternalCameraCount() == 0)
		{
			OVRPose pose = default(OVRPose);
			OVRPose ovrpose4 = new OVRPose
			{
				position = ((trackingOrigin == OVRManager.TrackingOrigin.EyeLevel) ? OVRMixedReality.fakeCameraEyeLevelPosition : OVRMixedReality.fakeCameraFloorLevelPosition),
				orientation = OVRMixedReality.fakeCameraRotation
			};
			pose = ovrpose4.ToWorldSpacePose(mainCamera);
			this.backgroundCamera.fieldOfView = OVRMixedReality.fakeCameraFov;
			this.backgroundCamera.aspect = OVRMixedReality.fakeCameraAspect;
			this.foregroundCamera.fieldOfView = OVRMixedReality.fakeCameraFov;
			this.foregroundCamera.aspect = OVRMixedReality.fakeCameraAspect;
			if (this.cameraInTrackingSpace)
			{
				this.backgroundCamera.transform.FromOVRPose(ovrpose4, true);
				this.foregroundCamera.transform.FromOVRPose(ovrpose4, true);
			}
			else
			{
				this.backgroundCamera.transform.FromOVRPose(pose, false);
				this.foregroundCamera.transform.FromOVRPose(pose, false);
			}
		}
		else
		{
			OVRPlugin.CameraExtrinsics extrinsics;
			OVRPlugin.CameraIntrinsics cameraIntrinsics;
			if (!OVRPlugin.GetMixedRealityCameraInfo(0, out extrinsics, out cameraIntrinsics))
			{
				Debug.LogError("Failed to get external camera information");
				return;
			}
			float fieldOfView = Mathf.Atan(cameraIntrinsics.FOVPort.UpTan) * 57.29578f * 2f;
			float aspect = cameraIntrinsics.FOVPort.LeftTan / cameraIntrinsics.FOVPort.UpTan;
			this.backgroundCamera.fieldOfView = fieldOfView;
			this.backgroundCamera.aspect = aspect;
			this.foregroundCamera.fieldOfView = fieldOfView;
			this.foregroundCamera.aspect = cameraIntrinsics.FOVPort.LeftTan / cameraIntrinsics.FOVPort.UpTan;
			if (this.cameraInTrackingSpace)
			{
				OVRPose pose2 = base.ComputeCameraTrackingSpacePose(extrinsics);
				this.backgroundCamera.transform.FromOVRPose(pose2, true);
				this.foregroundCamera.transform.FromOVRPose(pose2, true);
			}
			else
			{
				OVRPose pose3 = base.ComputeCameraWorldSpacePose(extrinsics, mainCamera);
				this.backgroundCamera.transform.FromOVRPose(pose3, false);
				this.foregroundCamera.transform.FromOVRPose(pose3, false);
			}
		}
		float b = Vector3.Dot(mainCamera.transform.position - this.foregroundCamera.transform.position, this.foregroundCamera.transform.forward);
		this.foregroundCamera.farClipPlane = Mathf.Max(this.foregroundCamera.nearClipPlane + 0.001f, b);
	}

	public override void Cleanup()
	{
		OVRCompositionUtil.SafeDestroy(ref this.backgroundCameraGameObject);
		this.backgroundCamera = null;
		OVRCompositionUtil.SafeDestroy(ref this.foregroundCameraGameObject);
		this.foregroundCamera = null;
		Debug.Log("ExternalComposition deactivated");
	}

	public void CacheAudioData(float[] data, int channels)
	{
		object obj = this.audioDataLock;
		lock (obj)
		{
			if (channels != this.cachedChannels)
			{
				this.cachedAudioData.Clear();
			}
			this.cachedChannels = channels;
			this.cachedAudioData.AddRange(data);
		}
	}

	public void GetAndResetAudioData(ref float[] audioData, out int audioFrames, out int channels)
	{
		object obj = this.audioDataLock;
		lock (obj)
		{
			if (audioData == null || audioData.Length < this.cachedAudioData.Count)
			{
				audioData = new float[this.cachedAudioData.Capacity];
			}
			this.cachedAudioData.CopyTo(audioData);
			audioFrames = this.cachedAudioData.Count;
			channels = this.cachedChannels;
			this.cachedAudioData.Clear();
		}
	}

	private GameObject previousMainCameraObject;

	public GameObject foregroundCameraGameObject;

	public Camera foregroundCamera;

	public GameObject backgroundCameraGameObject;

	public Camera backgroundCamera;

	private readonly object audioDataLock = new object();

	private List<float> cachedAudioData = new List<float>(16384);

	private int cachedChannels;
}
