using System;
using UnityEngine;

public class OVRMixedRealityCaptureTest : MonoBehaviour
{
	private void Start()
	{
		if (!this.defaultExternalCamera)
		{
			Debug.LogWarning("defaultExternalCamera undefined");
		}
		if (!OVRManager.instance.enableMixedReality)
		{
			OVRManager.instance.enableMixedReality = true;
		}
	}

	private void Initialize()
	{
		if (this.inited)
		{
			return;
		}
		if (!OVRPlugin.IsMixedRealityInitialized())
		{
			return;
		}
		OVRPlugin.ResetDefaultExternalCamera();
		Debug.LogFormat("GetExternalCameraCount before adding manual external camera {0}", new object[]
		{
			OVRPlugin.GetExternalCameraCount()
		});
		this.UpdateDefaultExternalCamera();
		Debug.LogFormat("GetExternalCameraCount after adding manual external camera {0}", new object[]
		{
			OVRPlugin.GetExternalCameraCount()
		});
		OVRPlugin.CameraExtrinsics cameraExtrinsics;
		OVRPlugin.CameraIntrinsics cameraIntrinsics;
		OVRPlugin.GetMixedRealityCameraInfo(0, out cameraExtrinsics, out cameraIntrinsics);
		this.defaultFov = cameraIntrinsics.FOVPort;
		this.inited = true;
	}

	private void UpdateDefaultExternalCamera()
	{
		string cameraName = "UnityExternalCamera";
		OVRPlugin.CameraIntrinsics cameraIntrinsics = default(OVRPlugin.CameraIntrinsics);
		OVRPlugin.CameraExtrinsics cameraExtrinsics = default(OVRPlugin.CameraExtrinsics);
		cameraIntrinsics.IsValid = OVRPlugin.Bool.True;
		cameraIntrinsics.LastChangedTimeSeconds = (double)Time.time;
		float num = this.defaultExternalCamera.fieldOfView * 0.017453292f;
		float num2 = Mathf.Atan(Mathf.Tan(num * 0.5f) * 1.7777778f) * 2f;
		OVRPlugin.Fovf fovport = default(OVRPlugin.Fovf);
		fovport.UpTan = (fovport.DownTan = Mathf.Tan(num * 0.5f));
		fovport.LeftTan = (fovport.RightTan = Mathf.Tan(num2 * 0.5f));
		cameraIntrinsics.FOVPort = fovport;
		cameraIntrinsics.VirtualNearPlaneDistanceMeters = this.defaultExternalCamera.nearClipPlane;
		cameraIntrinsics.VirtualFarPlaneDistanceMeters = this.defaultExternalCamera.farClipPlane;
		cameraIntrinsics.ImageSensorPixelResolution.w = 1920;
		cameraIntrinsics.ImageSensorPixelResolution.h = 1080;
		cameraExtrinsics.IsValid = OVRPlugin.Bool.True;
		cameraExtrinsics.LastChangedTimeSeconds = (double)Time.time;
		cameraExtrinsics.CameraStatusData = OVRPlugin.CameraStatus.CameraStatus_Calibrated;
		cameraExtrinsics.AttachedToNode = OVRPlugin.Node.None;
		OVRCameraRig componentInParent = Camera.main.GetComponentInParent<OVRCameraRig>();
		if (componentInParent)
		{
			OVRPose ovrpose = componentInParent.trackingSpace.ToOVRPose(false);
			OVRPose rhs = this.defaultExternalCamera.transform.ToOVRPose(false);
			cameraExtrinsics.RelativePose = (ovrpose.Inverse() * rhs).ToPosef();
		}
		else
		{
			cameraExtrinsics.RelativePose = OVRPlugin.Posef.identity;
		}
		if (!OVRPlugin.SetDefaultExternalCamera(cameraName, ref cameraIntrinsics, ref cameraExtrinsics))
		{
			Debug.LogError("SetDefaultExternalCamera() failed");
		}
	}

	private void Update()
	{
		if (!this.inited)
		{
			this.Initialize();
			return;
		}
		if (!this.defaultExternalCamera)
		{
			return;
		}
		if (!OVRPlugin.IsMixedRealityInitialized())
		{
			return;
		}
		if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			if (this.currentMode == OVRMixedRealityCaptureTest.CameraMode.ThirdPerson)
			{
				this.currentMode = OVRMixedRealityCaptureTest.CameraMode.Normal;
			}
			else
			{
				this.currentMode++;
			}
			Debug.LogFormat("Camera mode change to {0}", new object[]
			{
				this.currentMode
			});
		}
		if (this.currentMode == OVRMixedRealityCaptureTest.CameraMode.Normal)
		{
			this.UpdateDefaultExternalCamera();
			OVRPlugin.OverrideExternalCameraFov(0, false, default(OVRPlugin.Fovf));
			OVRPlugin.OverrideExternalCameraStaticPose(0, false, OVRPlugin.Posef.identity);
			return;
		}
		if (this.currentMode == OVRMixedRealityCaptureTest.CameraMode.OverrideFov)
		{
			OVRPlugin.Fovf fovf = this.defaultFov;
			OVRPlugin.OverrideExternalCameraFov(0, true, new OVRPlugin.Fovf
			{
				LeftTan = fovf.LeftTan * 2f,
				RightTan = fovf.RightTan * 2f,
				UpTan = fovf.UpTan * 2f,
				DownTan = fovf.DownTan * 2f
			});
			OVRPlugin.OverrideExternalCameraStaticPose(0, false, OVRPlugin.Posef.identity);
			if (!OVRPlugin.GetUseOverriddenExternalCameraFov(0))
			{
				Debug.LogWarning("FOV not overridden");
				return;
			}
		}
		else if (this.currentMode == OVRMixedRealityCaptureTest.CameraMode.ThirdPerson)
		{
			Camera component = base.GetComponent<Camera>();
			if (component == null)
			{
				return;
			}
			float num = component.fieldOfView * 0.017453292f;
			float num2 = Mathf.Atan(Mathf.Tan(num * 0.5f) * component.aspect) * 2f;
			OVRPlugin.Fovf fov = default(OVRPlugin.Fovf);
			fov.UpTan = (fov.DownTan = Mathf.Tan(num * 0.5f));
			fov.LeftTan = (fov.RightTan = Mathf.Tan(num2 * 0.5f));
			OVRPlugin.OverrideExternalCameraFov(0, true, fov);
			OVRCameraRig componentInParent = Camera.main.GetComponentInParent<OVRCameraRig>();
			if (componentInParent)
			{
				OVRPose ovrpose = componentInParent.trackingSpace.ToOVRPose(false);
				OVRPose rhs = base.transform.ToOVRPose(false);
				OVRPose rhs2 = ovrpose.Inverse() * rhs;
				OVRPlugin.Posef poseInStageOrigin = (OVRPlugin.GetTrackingTransformRelativePose(OVRPlugin.TrackingOrigin.Stage).ToOVRPose().Inverse() * rhs2).ToPosef();
				OVRPlugin.OverrideExternalCameraStaticPose(0, true, poseInStageOrigin);
			}
			else
			{
				OVRPlugin.OverrideExternalCameraStaticPose(0, false, OVRPlugin.Posef.identity);
			}
			if (!OVRPlugin.GetUseOverriddenExternalCameraFov(0))
			{
				Debug.LogWarning("FOV not overridden");
			}
			if (!OVRPlugin.GetUseOverriddenExternalCameraStaticPose(0))
			{
				Debug.LogWarning("StaticPose not overridden");
			}
		}
	}

	private bool inited;

	private OVRMixedRealityCaptureTest.CameraMode currentMode;

	public Camera defaultExternalCamera;

	private OVRPlugin.Fovf defaultFov;

	private enum CameraMode
	{
		Normal,
		OverrideFov,
		ThirdPerson
	}
}
