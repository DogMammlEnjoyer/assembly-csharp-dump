using System;
using Meta.XR.Util;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/move-eye-tracking/")]
[Feature(Feature.EyeTracking)]
public class OVREyeGaze : MonoBehaviour
{
	public bool EyeTrackingEnabled
	{
		get
		{
			return OVRPlugin.eyeTrackingEnabled;
		}
	}

	public float Confidence { get; private set; }

	private void Awake()
	{
		this._onPermissionGranted = new Action<string>(this.OnPermissionGranted);
	}

	private void Start()
	{
		this.PrepareHeadDirection();
	}

	private void OnEnable()
	{
		OVREyeGaze._trackingInstanceCount++;
		if (!this.StartEyeTracking())
		{
			base.enabled = false;
		}
	}

	private void OnPermissionGranted(string permissionId)
	{
		if (permissionId == OVRPermissionsRequester.GetPermissionId(OVRPermissionsRequester.Permission.EyeTracking))
		{
			OVRPermissionsRequester.PermissionGranted -= this._onPermissionGranted;
			base.enabled = true;
		}
	}

	private bool StartEyeTracking()
	{
		if (!OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.EyeTracking))
		{
			OVRPermissionsRequester.PermissionGranted -= this._onPermissionGranted;
			OVRPermissionsRequester.PermissionGranted += this._onPermissionGranted;
			return false;
		}
		if (!OVRPlugin.StartEyeTracking())
		{
			Debug.LogWarning("[OVREyeGaze] Failed to start eye tracking.");
			return false;
		}
		return true;
	}

	private void OnDisable()
	{
		if (--OVREyeGaze._trackingInstanceCount == 0)
		{
			OVRPlugin.StopEyeTracking();
		}
	}

	private void OnDestroy()
	{
		OVRPermissionsRequester.PermissionGranted -= this._onPermissionGranted;
	}

	private void Update()
	{
		if (!OVRPlugin.GetEyeGazesState(OVRPlugin.Step.Render, -1, ref this._currentEyeGazesState))
		{
			return;
		}
		OVRPlugin.EyeGazeState eyeGazeState = this._currentEyeGazesState.EyeGazes[(int)this.Eye];
		if (!eyeGazeState.IsValid)
		{
			return;
		}
		this.Confidence = eyeGazeState.Confidence;
		if (this.Confidence < this.ConfidenceThreshold)
		{
			return;
		}
		OVRPose ovrpose = eyeGazeState.Pose.ToOVRPose();
		OVREyeGaze.EyeTrackingMode trackingMode = this.TrackingMode;
		if (trackingMode != OVREyeGaze.EyeTrackingMode.HeadSpace)
		{
			if (trackingMode == OVREyeGaze.EyeTrackingMode.WorldSpace)
			{
				ovrpose = ovrpose.ToWorldSpacePose(Camera.main);
			}
		}
		else
		{
			ovrpose = ovrpose.ToHeadSpacePose();
		}
		if (this.ApplyPosition)
		{
			base.transform.position = ovrpose.position;
		}
		if (this.ApplyRotation)
		{
			base.transform.rotation = this.CalculateEyeRotation(ovrpose.orientation);
		}
	}

	private Quaternion CalculateEyeRotation(Quaternion eyeRotation)
	{
		return Quaternion.LookRotation(this._viewTransform.rotation * eyeRotation * Vector3.forward, this._viewTransform.up) * this._initialRotationOffset;
	}

	private void PrepareHeadDirection()
	{
		string name = "HeadLookAtDirection";
		this._viewTransform = new GameObject(name).transform;
		if (this.ReferenceFrame)
		{
			this._viewTransform.SetPositionAndRotation(this.ReferenceFrame.position, this.ReferenceFrame.rotation);
		}
		else
		{
			this._viewTransform.SetPositionAndRotation(base.transform.position, Quaternion.identity);
		}
		this._viewTransform.parent = base.transform.parent;
		this._initialRotationOffset = Quaternion.Inverse(this._viewTransform.rotation) * base.transform.rotation;
	}

	public OVREyeGaze.EyeId Eye;

	[Range(0f, 1f)]
	public float ConfidenceThreshold = 0.5f;

	public bool ApplyPosition = true;

	public bool ApplyRotation = true;

	private OVRPlugin.EyeGazesState _currentEyeGazesState;

	[Tooltip("Reference frame for eye. Reference frame should be set in the forward direction of the eye. It is there to calculate the initial offset of the eye GameObject. If it's null, then world reference frame will be used.")]
	public Transform ReferenceFrame;

	[Tooltip("HeadSpace: Tracking mode will convert the eye pose from tracking space to local space which is relative to the VR camera rig. For example, we can use this setting to correctly show the eye movement of a character which is facing in another direction than the source.\nWorldSpace: Tracking mode will convert the eye pose from tracking space to world space.\nTrackingSpace: Track eye is relative to OVRCameraRig. This is raw pose information from VR tracking space.")]
	public OVREyeGaze.EyeTrackingMode TrackingMode;

	private Quaternion _initialRotationOffset;

	private Transform _viewTransform;

	private const OVRPermissionsRequester.Permission EyeTrackingPermission = OVRPermissionsRequester.Permission.EyeTracking;

	private Action<string> _onPermissionGranted;

	private static int _trackingInstanceCount;

	public enum EyeId
	{
		Left,
		Right
	}

	public enum EyeTrackingMode
	{
		HeadSpace,
		WorldSpace,
		TrackingSpace
	}
}
