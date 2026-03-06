using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace UnityEditor.XR.LegacyInputHelpers
{
	[AddComponentMenu("XR/Camera Offset")]
	public class CameraOffset : MonoBehaviour
	{
		public GameObject cameraFloorOffsetObject
		{
			get
			{
				return this.m_CameraFloorOffsetObject;
			}
			set
			{
				this.m_CameraFloorOffsetObject = value;
				this.UpdateTrackingOrigin(this.m_TrackingOriginMode);
			}
		}

		public UserRequestedTrackingMode requestedTrackingMode
		{
			get
			{
				return this.m_RequestedTrackingMode;
			}
			set
			{
				this.m_RequestedTrackingMode = value;
				this.TryInitializeCamera();
			}
		}

		public TrackingOriginModeFlags TrackingOriginMode
		{
			get
			{
				return this.m_TrackingOriginMode;
			}
			set
			{
				this.m_TrackingOriginMode = value;
				this.TryInitializeCamera();
			}
		}

		[Obsolete("CameraOffset.trackingSpace is obsolete.  Please use CameraOffset.trackingOriginMode.")]
		public TrackingSpaceType trackingSpace
		{
			get
			{
				return this.m_TrackingSpace;
			}
			set
			{
				this.m_TrackingSpace = value;
				this.TryInitializeCamera();
			}
		}

		public float cameraYOffset
		{
			get
			{
				return this.m_CameraYOffset;
			}
			set
			{
				this.m_CameraYOffset = value;
				this.UpdateTrackingOrigin(this.m_TrackingOriginMode);
			}
		}

		private void UpgradeTrackingSpaceToTrackingOriginMode()
		{
			if (this.m_TrackingOriginMode == TrackingOriginModeFlags.Unknown && this.m_TrackingSpace <= TrackingSpaceType.RoomScale)
			{
				TrackingSpaceType trackingSpace = this.m_TrackingSpace;
				if (trackingSpace != TrackingSpaceType.Stationary)
				{
					if (trackingSpace == TrackingSpaceType.RoomScale)
					{
						this.m_TrackingOriginMode = TrackingOriginModeFlags.Floor;
					}
				}
				else
				{
					this.m_TrackingOriginMode = TrackingOriginModeFlags.Device;
				}
				this.m_TrackingSpace = (TrackingSpaceType)3;
			}
		}

		private void Awake()
		{
			if (!this.m_CameraFloorOffsetObject)
			{
				Debug.LogWarning("No camera container specified for XR Rig, using attached GameObject");
				this.m_CameraFloorOffsetObject = base.gameObject;
			}
		}

		private void Start()
		{
			this.TryInitializeCamera();
		}

		private void OnValidate()
		{
			this.UpgradeTrackingSpaceToTrackingOriginMode();
			this.TryInitializeCamera();
		}

		private void TryInitializeCamera()
		{
			this.m_CameraInitialized = this.SetupCamera();
			if (!this.m_CameraInitialized & !this.m_CameraInitializing)
			{
				base.StartCoroutine(this.RepeatInitializeCamera());
			}
		}

		private IEnumerator RepeatInitializeCamera()
		{
			this.m_CameraInitializing = true;
			yield return null;
			while (!this.m_CameraInitialized)
			{
				this.m_CameraInitialized = this.SetupCamera();
				yield return null;
			}
			this.m_CameraInitializing = false;
			yield break;
		}

		private bool SetupCamera()
		{
			SubsystemManager.GetInstances<XRInputSubsystem>(CameraOffset.s_InputSubsystems);
			bool flag = true;
			if (CameraOffset.s_InputSubsystems.Count != 0)
			{
				for (int i = 0; i < CameraOffset.s_InputSubsystems.Count; i++)
				{
					bool flag2 = this.SetupCamera(CameraOffset.s_InputSubsystems[i]);
					if (flag2)
					{
						CameraOffset.s_InputSubsystems[i].trackingOriginUpdated -= this.OnTrackingOriginUpdated;
						CameraOffset.s_InputSubsystems[i].trackingOriginUpdated += this.OnTrackingOriginUpdated;
					}
					flag = (flag && flag2);
				}
			}
			else if (this.m_RequestedTrackingMode == UserRequestedTrackingMode.Floor)
			{
				this.SetupCameraLegacy(TrackingSpaceType.RoomScale);
			}
			else
			{
				this.SetupCameraLegacy(TrackingSpaceType.Stationary);
			}
			return flag;
		}

		private bool SetupCamera(XRInputSubsystem subsystem)
		{
			if (subsystem == null)
			{
				return false;
			}
			bool flag = false;
			TrackingOriginModeFlags trackingOriginMode = subsystem.GetTrackingOriginMode();
			TrackingOriginModeFlags supportedTrackingOriginModes = subsystem.GetSupportedTrackingOriginModes();
			TrackingOriginModeFlags trackingOriginModeFlags = TrackingOriginModeFlags.Unknown;
			if (this.m_RequestedTrackingMode == UserRequestedTrackingMode.Default)
			{
				trackingOriginModeFlags = trackingOriginMode;
			}
			else if (this.m_RequestedTrackingMode == UserRequestedTrackingMode.Device)
			{
				trackingOriginModeFlags = TrackingOriginModeFlags.Device;
			}
			else if (this.m_RequestedTrackingMode == UserRequestedTrackingMode.Floor)
			{
				trackingOriginModeFlags = TrackingOriginModeFlags.Floor;
			}
			else
			{
				Debug.LogWarning("Unknown Requested Tracking Mode");
			}
			if (trackingOriginModeFlags == TrackingOriginModeFlags.Floor)
			{
				if ((supportedTrackingOriginModes & TrackingOriginModeFlags.Floor) == TrackingOriginModeFlags.Unknown)
				{
					Debug.LogWarning("CameraOffset.SetupCamera: Attempting to set the tracking space to Floor, but that is not supported by the SDK.");
				}
				else
				{
					flag = subsystem.TrySetTrackingOriginMode(trackingOriginModeFlags);
				}
			}
			else if (trackingOriginModeFlags == TrackingOriginModeFlags.Device)
			{
				if ((supportedTrackingOriginModes & TrackingOriginModeFlags.Device) == TrackingOriginModeFlags.Unknown)
				{
					Debug.LogWarning("CameraOffset.SetupCamera: Attempting to set the tracking space to Device, but that is not supported by the SDK.");
				}
				else
				{
					flag = (subsystem.TrySetTrackingOriginMode(trackingOriginModeFlags) && subsystem.TryRecenter());
				}
			}
			if (flag)
			{
				this.UpdateTrackingOrigin(subsystem.GetTrackingOriginMode());
			}
			return flag;
		}

		private void UpdateTrackingOrigin(TrackingOriginModeFlags trackingOriginModeFlags)
		{
			this.m_TrackingOriginMode = trackingOriginModeFlags;
			if (this.m_CameraFloorOffsetObject != null)
			{
				this.m_CameraFloorOffsetObject.transform.localPosition = new Vector3(this.m_CameraFloorOffsetObject.transform.localPosition.x, (this.m_TrackingOriginMode == TrackingOriginModeFlags.Device) ? this.cameraYOffset : 0f, this.m_CameraFloorOffsetObject.transform.localPosition.z);
			}
		}

		private void OnTrackingOriginUpdated(XRInputSubsystem subsystem)
		{
			this.UpdateTrackingOrigin(subsystem.GetTrackingOriginMode());
		}

		private void OnDestroy()
		{
			SubsystemManager.GetInstances<XRInputSubsystem>(CameraOffset.s_InputSubsystems);
			foreach (XRInputSubsystem xrinputSubsystem in CameraOffset.s_InputSubsystems)
			{
				xrinputSubsystem.trackingOriginUpdated -= this.OnTrackingOriginUpdated;
			}
		}

		private void SetupCameraLegacy(TrackingSpaceType trackingSpace)
		{
			float y = this.m_CameraYOffset;
			XRDevice.SetTrackingSpaceType(trackingSpace);
			if (trackingSpace == TrackingSpaceType.Stationary)
			{
				InputTracking.Recenter();
			}
			else if (trackingSpace == TrackingSpaceType.RoomScale)
			{
				y = 0f;
			}
			this.m_TrackingSpace = trackingSpace;
			if (this.m_CameraFloorOffsetObject)
			{
				this.m_CameraFloorOffsetObject.transform.localPosition = new Vector3(this.m_CameraFloorOffsetObject.transform.localPosition.x, y, this.m_CameraFloorOffsetObject.transform.localPosition.z);
			}
		}

		private const float k_DefaultCameraYOffset = 1.36144f;

		[SerializeField]
		[Tooltip("GameObject to move to desired height off the floor (defaults to this object if none provided).")]
		private GameObject m_CameraFloorOffsetObject;

		[SerializeField]
		[Tooltip("What the user wants the tracking origin mode to be")]
		private UserRequestedTrackingMode m_RequestedTrackingMode;

		[SerializeField]
		[Tooltip("Sets the type of tracking origin to use for this Rig. Tracking origins identify where 0,0,0 is in the world of tracking.")]
		private TrackingOriginModeFlags m_TrackingOriginMode;

		[SerializeField]
		[Tooltip("Set if the XR experience is Room Scale or Stationary.")]
		private TrackingSpaceType m_TrackingSpace;

		[SerializeField]
		[Tooltip("Camera Height to be used when in Device tracking space.")]
		private float m_CameraYOffset = 1.36144f;

		private bool m_CameraInitialized;

		private bool m_CameraInitializing;

		private static List<XRInputSubsystem> s_InputSubsystems = new List<XRInputSubsystem>();
	}
}
