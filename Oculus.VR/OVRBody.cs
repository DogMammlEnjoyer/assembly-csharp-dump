using System;
using Meta.XR.Util;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/move-body-tracking/")]
[Feature(Feature.BodyTracking)]
public class OVRBody : MonoBehaviour, OVRSkeleton.IOVRSkeletonDataProvider, OVRSkeletonRenderer.IOVRSkeletonRendererDataProvider
{
	public OVRPlugin.BodyJointSet ProvidedSkeletonType
	{
		get
		{
			return this._providedSkeletonType;
		}
		set
		{
			this._providedSkeletonType = value;
		}
	}

	public OVRPlugin.BodyState? BodyState
	{
		get
		{
			if (!this._hasData)
			{
				return null;
			}
			return new OVRPlugin.BodyState?(this._bodyState);
		}
	}

	private void Awake()
	{
		this._onPermissionGranted = new Action<string>(this.OnPermissionGranted);
	}

	private void OnEnable()
	{
		this._dataChangedSinceLastQuery = false;
		this._hasData = false;
		OVRManager ovrmanager = Object.FindAnyObjectByType<OVRManager>();
		if (ovrmanager != null && ovrmanager.SimultaneousHandsAndControllersEnabled)
		{
			Debug.LogWarning("Currently, Body API and simultaneous hands and controllers cannot be enabled at the same time", this);
			base.enabled = false;
			return;
		}
		if (this._providedSkeletonType == OVRPlugin.BodyJointSet.FullBody && OVRRuntimeSettings.GetRuntimeSettings().BodyTrackingJointSet == OVRPlugin.BodyJointSet.UpperBody)
		{
			Debug.LogWarning("[OVRBody] Full body skeleton is used, but Full body tracking is disabled. Check settings in OVRManager.");
		}
		OVRBody._trackingInstanceCount++;
		if (!OVRBody.StartBodyTracking())
		{
			base.enabled = false;
			return;
		}
		if (OVRPlugin.nativeXrApi == OVRPlugin.XrApi.OpenXR)
		{
			this.GetBodyState(OVRPlugin.Step.Render);
			return;
		}
		base.enabled = false;
		Debug.LogWarning("[OVRBody] Body tracking is only supported by OpenXR and is unavailable.");
	}

	private void OnPermissionGranted(string permissionId)
	{
		if (permissionId == OVRPermissionsRequester.GetPermissionId(OVRPermissionsRequester.Permission.BodyTracking))
		{
			OVRPermissionsRequester.PermissionGranted -= this._onPermissionGranted;
			base.enabled = true;
		}
	}

	private static bool StartBodyTracking()
	{
		OVRPlugin.BodyJointSet bodyTrackingJointSet = OVRRuntimeSettings.GetRuntimeSettings().BodyTrackingJointSet;
		if (!OVRPlugin.StartBodyTracking2(bodyTrackingJointSet))
		{
			Debug.LogWarning(string.Format("[{0}] Failed to start body tracking with joint set {1}.", "OVRBody", bodyTrackingJointSet));
			return false;
		}
		OVRPlugin.BodyTrackingFidelity2 bodyTrackingFidelity = OVRRuntimeSettings.GetRuntimeSettings().BodyTrackingFidelity;
		if (!OVRPlugin.RequestBodyTrackingFidelity(bodyTrackingFidelity))
		{
			Debug.LogWarning(string.Format("[{0}] Failed to set Body Tracking fidelity to: {1}", "OVRBody", bodyTrackingFidelity));
		}
		return true;
	}

	private void OnDisable()
	{
		if (--OVRBody._trackingInstanceCount == 0)
		{
			OVRPlugin.StopBodyTracking();
		}
	}

	private void OnDestroy()
	{
		OVRPermissionsRequester.PermissionGranted -= this._onPermissionGranted;
	}

	private void Update()
	{
		this.GetBodyState(OVRPlugin.Step.Render);
	}

	public static bool SetRequestedJointSet(OVRPlugin.BodyJointSet jointSet)
	{
		OVRPlugin.BodyJointSet bodyTrackingJointSet = OVRRuntimeSettings.GetRuntimeSettings().BodyTrackingJointSet;
		if (jointSet != bodyTrackingJointSet)
		{
			OVRRuntimeSettings.GetRuntimeSettings().BodyTrackingJointSet = jointSet;
			if (OVRBody._trackingInstanceCount > 0)
			{
				OVRPlugin.StopBodyTracking();
				return OVRBody.StartBodyTracking();
			}
		}
		return true;
	}

	public static bool SuggestBodyTrackingCalibrationOverride(float height)
	{
		return OVRPlugin.SuggestBodyTrackingCalibrationOverride(new OVRPlugin.BodyTrackingCalibrationInfo
		{
			BodyHeight = height
		});
	}

	public static bool ResetBodyTrackingCalibration()
	{
		return OVRPlugin.ResetBodyTrackingCalibration();
	}

	public OVRPlugin.BodyTrackingCalibrationState GetBodyTrackingCalibrationStatus()
	{
		if (!this._hasData)
		{
			return OVRPlugin.BodyTrackingCalibrationState.Invalid;
		}
		return this._bodyState.CalibrationStatus;
	}

	public OVRPlugin.BodyTrackingFidelity2 GetBodyTrackingFidelityStatus()
	{
		return this._bodyState.Fidelity;
	}

	private void GetBodyState(OVRPlugin.Step step)
	{
		if (OVRPlugin.GetBodyState4(step, this._providedSkeletonType, ref this._bodyState))
		{
			this._hasData = true;
			this._dataChangedSinceLastQuery = true;
			return;
		}
		this._hasData = false;
	}

	OVRSkeleton.SkeletonType OVRSkeleton.IOVRSkeletonDataProvider.GetSkeletonType()
	{
		OVRPlugin.BodyJointSet providedSkeletonType = this._providedSkeletonType;
		OVRSkeleton.SkeletonType result;
		if (providedSkeletonType != OVRPlugin.BodyJointSet.UpperBody)
		{
			if (providedSkeletonType != OVRPlugin.BodyJointSet.FullBody)
			{
				result = OVRSkeleton.SkeletonType.None;
			}
			else
			{
				result = OVRSkeleton.SkeletonType.FullBody;
			}
		}
		else
		{
			result = OVRSkeleton.SkeletonType.Body;
		}
		return result;
	}

	OVRSkeleton.SkeletonPoseData OVRSkeleton.IOVRSkeletonDataProvider.GetSkeletonPoseData()
	{
		if (!this._hasData)
		{
			return default(OVRSkeleton.SkeletonPoseData);
		}
		if (this._dataChangedSinceLastQuery)
		{
			Array.Resize<OVRPlugin.Quatf>(ref this._boneRotations, this._bodyState.JointLocations.Length);
			Array.Resize<OVRPlugin.Vector3f>(ref this._boneTranslations, this._bodyState.JointLocations.Length);
			for (int i = 0; i < this._bodyState.JointLocations.Length; i++)
			{
				OVRPlugin.BodyJointLocation bodyJointLocation = this._bodyState.JointLocations[i];
				if (bodyJointLocation.OrientationValid)
				{
					this._boneRotations[i] = bodyJointLocation.Pose.Orientation;
				}
				if (bodyJointLocation.PositionValid)
				{
					this._boneTranslations[i] = bodyJointLocation.Pose.Position;
				}
			}
			this._dataChangedSinceLastQuery = false;
		}
		return new OVRSkeleton.SkeletonPoseData
		{
			IsDataValid = true,
			IsDataHighConfidence = (this._bodyState.Confidence > 0.5f),
			RootPose = this._bodyState.JointLocations[0].Pose,
			RootScale = 1f,
			BoneRotations = this._boneRotations,
			BoneTranslations = this._boneTranslations,
			SkeletonChangedCount = (int)this._bodyState.SkeletonChangedCount
		};
	}

	OVRSkeletonRenderer.SkeletonRendererData OVRSkeletonRenderer.IOVRSkeletonRendererDataProvider.GetSkeletonRendererData()
	{
		if (!this._hasData)
		{
			return default(OVRSkeletonRenderer.SkeletonRendererData);
		}
		return new OVRSkeletonRenderer.SkeletonRendererData
		{
			RootScale = 1f,
			IsDataValid = true,
			IsDataHighConfidence = true,
			ShouldUseSystemGestureMaterial = false
		};
	}

	public static OVRPlugin.BodyTrackingFidelity2 Fidelity
	{
		get
		{
			return OVRRuntimeSettings.GetRuntimeSettings().BodyTrackingFidelity;
		}
		set
		{
			OVRRuntimeSettings.GetRuntimeSettings().BodyTrackingFidelity = value;
			OVRPlugin.RequestBodyTrackingFidelity(value);
		}
	}

	bool OVRSkeleton.IOVRSkeletonDataProvider.get_enabled()
	{
		return base.enabled;
	}

	private OVRPlugin.BodyState _bodyState;

	private OVRPlugin.Quatf[] _boneRotations;

	private OVRPlugin.Vector3f[] _boneTranslations;

	private bool _dataChangedSinceLastQuery;

	private bool _hasData;

	private const OVRPermissionsRequester.Permission BodyTrackingPermission = OVRPermissionsRequester.Permission.BodyTracking;

	private Action<string> _onPermissionGranted;

	[SerializeField]
	[Tooltip("The skeleton data type to be provided. Should be sync with OVRSkeleton. For selecting the tracking mode on the device, check settings in OVRManager.")]
	private OVRPlugin.BodyJointSet _providedSkeletonType;

	private static int _trackingInstanceCount;
}
