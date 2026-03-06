using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Serialization;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

namespace Unity.XR.CoreUtils
{
	[AddComponentMenu("XR/XR Origin")]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.core-utils@2.0/api/Unity.XR.CoreUtils.XROrigin.html")]
	public class XROrigin : MonoBehaviour
	{
		public Camera Camera
		{
			get
			{
				return this.m_Camera;
			}
			set
			{
				this.m_Camera = value;
			}
		}

		public Transform TrackablesParent { get; private set; }

		public event Action<ARTrackablesParentTransformChangedEventArgs> TrackablesParentTransformChanged;

		public GameObject Origin
		{
			get
			{
				return this.m_OriginBaseGameObject;
			}
			set
			{
				this.m_OriginBaseGameObject = value;
			}
		}

		public GameObject CameraFloorOffsetObject
		{
			get
			{
				return this.m_CameraFloorOffsetObject;
			}
			set
			{
				this.m_CameraFloorOffsetObject = value;
				this.MoveOffsetHeight();
			}
		}

		public XROrigin.TrackingOriginMode RequestedTrackingOriginMode
		{
			get
			{
				return this.m_RequestedTrackingOriginMode;
			}
			set
			{
				this.m_RequestedTrackingOriginMode = value;
				this.TryInitializeCamera();
			}
		}

		public float CameraYOffset
		{
			get
			{
				return this.m_CameraYOffset;
			}
			set
			{
				this.m_CameraYOffset = value;
				this.MoveOffsetHeight();
			}
		}

		public TrackingOriginModeFlags CurrentTrackingOriginMode { get; private set; }

		public Vector3 OriginInCameraSpacePos
		{
			get
			{
				return this.m_Camera.transform.InverseTransformPoint(this.m_OriginBaseGameObject.transform.position);
			}
		}

		public Vector3 CameraInOriginSpacePos
		{
			get
			{
				return this.m_OriginBaseGameObject.transform.InverseTransformPoint(this.m_Camera.transform.position);
			}
		}

		public float CameraInOriginSpaceHeight
		{
			get
			{
				return this.CameraInOriginSpacePos.y;
			}
		}

		private void MoveOffsetHeight()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			TrackingOriginModeFlags currentTrackingOriginMode = this.CurrentTrackingOriginMode;
			if (currentTrackingOriginMode != TrackingOriginModeFlags.Device)
			{
				if (currentTrackingOriginMode == TrackingOriginModeFlags.Floor)
				{
					this.MoveOffsetHeight(0f);
					return;
				}
				if (currentTrackingOriginMode != TrackingOriginModeFlags.Unbounded)
				{
					return;
				}
			}
			this.MoveOffsetHeight(this.m_CameraYOffset);
		}

		private void MoveOffsetHeight(float y)
		{
			if (this.m_CameraFloorOffsetObject != null)
			{
				Transform transform = this.m_CameraFloorOffsetObject.transform;
				Vector3 localPosition = transform.localPosition;
				localPosition.y = y;
				transform.localPosition = localPosition;
			}
		}

		private void TryInitializeCamera()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.m_CameraInitialized = this.SetupCamera();
			if (!this.m_CameraInitialized & !this.m_CameraInitializing)
			{
				base.StartCoroutine(this.RepeatInitializeCamera());
			}
		}

		private bool SetupCamera()
		{
			bool result = true;
			SubsystemManager.GetSubsystems<XRInputSubsystem>(XROrigin.s_InputSubsystems);
			if (XROrigin.s_InputSubsystems.Count > 0)
			{
				foreach (XRInputSubsystem xrinputSubsystem in XROrigin.s_InputSubsystems)
				{
					if (this.SetupCamera(xrinputSubsystem))
					{
						xrinputSubsystem.trackingOriginUpdated -= this.OnInputSubsystemTrackingOriginUpdated;
						xrinputSubsystem.trackingOriginUpdated += this.OnInputSubsystemTrackingOriginUpdated;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		private bool SetupCamera(XRInputSubsystem inputSubsystem)
		{
			if (inputSubsystem == null)
			{
				return false;
			}
			bool flag = true;
			XROrigin.TrackingOriginMode requestedTrackingOriginMode = this.m_RequestedTrackingOriginMode;
			if (requestedTrackingOriginMode != XROrigin.TrackingOriginMode.NotSpecified)
			{
				if (requestedTrackingOriginMode - XROrigin.TrackingOriginMode.Device > 2)
				{
					Debug.LogError(string.Format("Unhandled {0}={1}", "TrackingOriginMode", this.m_RequestedTrackingOriginMode));
					return false;
				}
				TrackingOriginModeFlags supportedTrackingOriginModes = inputSubsystem.GetSupportedTrackingOriginModes();
				if (supportedTrackingOriginModes == TrackingOriginModeFlags.Unknown)
				{
					return false;
				}
				TrackingOriginModeFlags trackingOriginModeFlags = XROrigin.ConvertTrackingOriginModeToFlag(this.m_RequestedTrackingOriginMode);
				if ((supportedTrackingOriginModes & trackingOriginModeFlags) == TrackingOriginModeFlags.Unknown)
				{
					this.m_RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.NotSpecified;
					this.CurrentTrackingOriginMode = inputSubsystem.GetTrackingOriginMode();
					Debug.LogWarning(string.Format("Attempting to set the tracking origin mode to {0}, but that is not supported by the SDK.", trackingOriginModeFlags) + string.Format(" Supported types: {0:F}. Using the current mode of {1} instead.", supportedTrackingOriginModes, this.CurrentTrackingOriginMode), this);
				}
				else
				{
					flag = inputSubsystem.TrySetTrackingOriginMode(trackingOriginModeFlags);
				}
			}
			else
			{
				this.CurrentTrackingOriginMode = inputSubsystem.GetTrackingOriginMode();
			}
			if (flag)
			{
				this.MoveOffsetHeight();
			}
			if (this.CurrentTrackingOriginMode == TrackingOriginModeFlags.Device || this.m_RequestedTrackingOriginMode == XROrigin.TrackingOriginMode.Device || this.CurrentTrackingOriginMode == TrackingOriginModeFlags.Unbounded || this.m_RequestedTrackingOriginMode == XROrigin.TrackingOriginMode.Unbounded)
			{
				flag = inputSubsystem.TryRecenter();
			}
			return flag;
		}

		private void OnInputSubsystemTrackingOriginUpdated(XRInputSubsystem inputSubsystem)
		{
			this.CurrentTrackingOriginMode = inputSubsystem.GetTrackingOriginMode();
			this.MoveOffsetHeight();
		}

		private IEnumerator RepeatInitializeCamera()
		{
			this.m_CameraInitializing = true;
			while (!this.m_CameraInitialized)
			{
				yield return null;
				if (!this.m_CameraInitialized)
				{
					this.m_CameraInitialized = this.SetupCamera();
				}
			}
			this.m_CameraInitializing = false;
			yield break;
		}

		public bool RotateAroundCameraUsingOriginUp(float angleDegrees)
		{
			return this.RotateAroundCameraPosition(this.m_OriginBaseGameObject.transform.up, angleDegrees);
		}

		public bool RotateAroundCameraPosition(Vector3 vector, float angleDegrees)
		{
			if (this.m_Camera == null || this.m_OriginBaseGameObject == null)
			{
				return false;
			}
			this.m_OriginBaseGameObject.transform.RotateAround(this.m_Camera.transform.position, vector, angleDegrees);
			return true;
		}

		public bool MatchOriginUp(Vector3 destinationUp)
		{
			if (this.m_OriginBaseGameObject == null)
			{
				return false;
			}
			if (this.m_OriginBaseGameObject.transform.up == destinationUp)
			{
				return true;
			}
			Quaternion lhs = Quaternion.FromToRotation(this.m_OriginBaseGameObject.transform.up, destinationUp);
			this.m_OriginBaseGameObject.transform.rotation = lhs * base.transform.rotation;
			return true;
		}

		public bool MatchOriginUpCameraForward(Vector3 destinationUp, Vector3 destinationForward)
		{
			if (this.m_Camera != null && this.MatchOriginUp(destinationUp))
			{
				float angleDegrees = Vector3.SignedAngle(Vector3.ProjectOnPlane(this.m_Camera.transform.forward, destinationUp).normalized, destinationForward, destinationUp);
				this.RotateAroundCameraPosition(destinationUp, angleDegrees);
				return true;
			}
			return false;
		}

		public bool MatchOriginUpOriginForward(Vector3 destinationUp, Vector3 destinationForward)
		{
			if (this.m_OriginBaseGameObject != null && this.MatchOriginUp(destinationUp))
			{
				float angleDegrees = Vector3.SignedAngle(this.m_OriginBaseGameObject.transform.forward, destinationForward, destinationUp);
				this.RotateAroundCameraPosition(destinationUp, angleDegrees);
				return true;
			}
			return false;
		}

		public bool MoveCameraToWorldLocation(Vector3 desiredWorldLocation)
		{
			if (this.m_Camera == null)
			{
				return false;
			}
			Vector3 a = Matrix4x4.Rotate(this.m_Camera.transform.rotation).MultiplyPoint3x4(this.OriginInCameraSpacePos);
			this.m_OriginBaseGameObject.transform.position = a + desiredWorldLocation;
			return true;
		}

		protected void Awake()
		{
			if (this.m_CameraFloorOffsetObject == null)
			{
				Debug.LogWarning("No Camera Floor Offset GameObject specified for XR Origin, using attached GameObject.", this);
				this.m_CameraFloorOffsetObject = base.gameObject;
			}
			if (this.m_Camera == null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					this.m_Camera = main;
				}
				else
				{
					Debug.LogWarning("No Main Camera is found for XR Origin, please assign the Camera field manually.", this);
				}
			}
			this.TrackablesParent = new GameObject("Trackables").transform;
			this.TrackablesParent.SetParent(base.transform, false);
			this.TrackablesParent.SetLocalPose(Pose.identity);
			this.TrackablesParent.localScale = Vector3.one;
			if (this.m_Camera)
			{
				Object component = this.m_Camera.GetComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
				UnityEngine.SpatialTracking.TrackedPoseDriver component2 = this.m_Camera.GetComponent<UnityEngine.SpatialTracking.TrackedPoseDriver>();
				if (component == null && component2 == null)
				{
					Debug.LogWarning("Camera \"" + this.m_Camera.name + "\" does not use a Tracked Pose Driver (Input System), so its transform will not be updated by an XR device.  In order for this to be updated, please add a Tracked Pose Driver (Input System) with bindings for position and rotation of the center eye.", this);
				}
			}
		}

		private Pose GetCameraOriginPose()
		{
			Pose identity = Pose.identity;
			Transform parent = this.m_Camera.transform.parent;
			if (!parent)
			{
				return identity;
			}
			return parent.TransformPose(identity);
		}

		protected void OnEnable()
		{
			Application.onBeforeRender += this.OnBeforeRender;
		}

		protected void OnDisable()
		{
			Application.onBeforeRender -= this.OnBeforeRender;
		}

		private void OnBeforeRender()
		{
			if (this.m_Camera)
			{
				Pose cameraOriginPose = this.GetCameraOriginPose();
				this.TrackablesParent.position = cameraOriginPose.position;
				this.TrackablesParent.rotation = cameraOriginPose.rotation;
			}
			if (this.TrackablesParent.hasChanged)
			{
				Action<ARTrackablesParentTransformChangedEventArgs> trackablesParentTransformChanged = this.TrackablesParentTransformChanged;
				if (trackablesParentTransformChanged != null)
				{
					trackablesParentTransformChanged(new ARTrackablesParentTransformChangedEventArgs(this, this.TrackablesParent));
				}
				this.TrackablesParent.hasChanged = false;
			}
		}

		protected void OnValidate()
		{
			if (this.m_OriginBaseGameObject == null)
			{
				this.m_OriginBaseGameObject = base.gameObject;
			}
			if (Application.isPlaying && base.isActiveAndEnabled)
			{
				if (this.<OnValidate>g__IsModeStale|60_0())
				{
					this.TryInitializeCamera();
					return;
				}
				this.MoveOffsetHeight();
			}
		}

		private static TrackingOriginModeFlags ConvertTrackingOriginModeToFlag(XROrigin.TrackingOriginMode mode)
		{
			switch (mode)
			{
			case XROrigin.TrackingOriginMode.NotSpecified:
				return TrackingOriginModeFlags.Unknown;
			case XROrigin.TrackingOriginMode.Device:
				return TrackingOriginModeFlags.Device;
			case XROrigin.TrackingOriginMode.Floor:
				return TrackingOriginModeFlags.Floor;
			case XROrigin.TrackingOriginMode.Unbounded:
				return TrackingOriginModeFlags.Unbounded;
			default:
				return TrackingOriginModeFlags.Unknown;
			}
		}

		protected void Start()
		{
			this.TryInitializeCamera();
		}

		protected void OnDestroy()
		{
			foreach (XRInputSubsystem xrinputSubsystem in XROrigin.s_InputSubsystems)
			{
				if (xrinputSubsystem != null)
				{
					xrinputSubsystem.trackingOriginUpdated -= this.OnInputSubsystemTrackingOriginUpdated;
				}
			}
		}

		[CompilerGenerated]
		private bool <OnValidate>g__IsModeStale|60_0()
		{
			if (XROrigin.s_InputSubsystems.Count > 0)
			{
				foreach (XRInputSubsystem xrinputSubsystem in XROrigin.s_InputSubsystems)
				{
					TrackingOriginModeFlags trackingOriginModeFlags = XROrigin.ConvertTrackingOriginModeToFlag(this.m_RequestedTrackingOriginMode);
					if (trackingOriginModeFlags == TrackingOriginModeFlags.Unknown)
					{
						return false;
					}
					if (xrinputSubsystem != null && xrinputSubsystem.GetTrackingOriginMode() != trackingOriginModeFlags)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		[SerializeField]
		[Tooltip("The Camera to associate with the XR device.")]
		private Camera m_Camera;

		private const float k_DefaultCameraYOffset = 1.1176f;

		[SerializeField]
		[FormerlySerializedAs("m_RigBaseGameObject")]
		private GameObject m_OriginBaseGameObject;

		[SerializeField]
		private GameObject m_CameraFloorOffsetObject;

		[SerializeField]
		private XROrigin.TrackingOriginMode m_RequestedTrackingOriginMode;

		[SerializeField]
		private float m_CameraYOffset = 1.1176f;

		private static readonly List<XRInputSubsystem> s_InputSubsystems = new List<XRInputSubsystem>();

		private bool m_CameraInitialized;

		private bool m_CameraInitializing;

		public enum TrackingOriginMode
		{
			NotSpecified,
			Device,
			Floor,
			Unbounded
		}
	}
}
