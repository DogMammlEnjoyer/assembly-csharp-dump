using System;
using UnityEngine.Experimental.XR.Interaction;

namespace UnityEngine.SpatialTracking
{
	[DefaultExecutionOrder(-30000)]
	[AddComponentMenu("XR/Tracked Pose Driver")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.legacyinputhelpers@2.1/manual/index.html")]
	[Serializable]
	public class TrackedPoseDriver : MonoBehaviour
	{
		public TrackedPoseDriver.DeviceType deviceType
		{
			get
			{
				return this.m_Device;
			}
			internal set
			{
				this.m_Device = value;
			}
		}

		public TrackedPoseDriver.TrackedPose poseSource
		{
			get
			{
				return this.m_PoseSource;
			}
			internal set
			{
				this.m_PoseSource = value;
			}
		}

		public bool SetPoseSource(TrackedPoseDriver.DeviceType deviceType, TrackedPoseDriver.TrackedPose pose)
		{
			if (deviceType < (TrackedPoseDriver.DeviceType)TrackedPoseDriverDataDescription.DeviceData.Count)
			{
				TrackedPoseDriverDataDescription.PoseData poseData = TrackedPoseDriverDataDescription.DeviceData[(int)deviceType];
				for (int i = 0; i < poseData.Poses.Count; i++)
				{
					if (poseData.Poses[i] == pose)
					{
						this.deviceType = deviceType;
						this.poseSource = pose;
						return true;
					}
				}
			}
			return false;
		}

		public BasePoseProvider poseProviderComponent
		{
			get
			{
				return this.m_PoseProviderComponent;
			}
			set
			{
				this.m_PoseProviderComponent = value;
			}
		}

		private PoseDataFlags GetPoseData(TrackedPoseDriver.DeviceType device, TrackedPoseDriver.TrackedPose poseSource, out Pose resultPose)
		{
			if (!(this.m_PoseProviderComponent != null))
			{
				return PoseDataSource.GetDataFromSource(poseSource, out resultPose);
			}
			return this.m_PoseProviderComponent.GetPoseFromProvider(out resultPose);
		}

		public TrackedPoseDriver.TrackingType trackingType
		{
			get
			{
				return this.m_TrackingType;
			}
			set
			{
				this.m_TrackingType = value;
			}
		}

		public TrackedPoseDriver.UpdateType updateType
		{
			get
			{
				return this.m_UpdateType;
			}
			set
			{
				this.m_UpdateType = value;
			}
		}

		public bool UseRelativeTransform
		{
			get
			{
				return this.m_UseRelativeTransform;
			}
			set
			{
				this.m_UseRelativeTransform = value;
			}
		}

		public Pose originPose
		{
			get
			{
				return this.m_OriginPose;
			}
			set
			{
				this.m_OriginPose = value;
			}
		}

		private void CacheLocalPosition()
		{
			this.m_OriginPose.position = base.transform.localPosition;
			this.m_OriginPose.rotation = base.transform.localRotation;
		}

		private void ResetToCachedLocalPosition()
		{
			this.SetLocalTransform(this.m_OriginPose.position, this.m_OriginPose.rotation, PoseDataFlags.Position | PoseDataFlags.Rotation);
		}

		protected virtual void Awake()
		{
			this.CacheLocalPosition();
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnEnable()
		{
			Application.onBeforeRender += this.OnBeforeRender;
		}

		protected virtual void OnDisable()
		{
			this.ResetToCachedLocalPosition();
			Application.onBeforeRender -= this.OnBeforeRender;
		}

		protected virtual void FixedUpdate()
		{
			if (this.m_UpdateType == TrackedPoseDriver.UpdateType.Update || this.m_UpdateType == TrackedPoseDriver.UpdateType.UpdateAndBeforeRender)
			{
				this.PerformUpdate();
			}
		}

		protected virtual void Update()
		{
			if (this.m_UpdateType == TrackedPoseDriver.UpdateType.Update || this.m_UpdateType == TrackedPoseDriver.UpdateType.UpdateAndBeforeRender)
			{
				this.PerformUpdate();
			}
		}

		[BeforeRenderOrder(-30000)]
		protected virtual void OnBeforeRender()
		{
			if (this.m_UpdateType == TrackedPoseDriver.UpdateType.BeforeRender || this.m_UpdateType == TrackedPoseDriver.UpdateType.UpdateAndBeforeRender)
			{
				this.PerformUpdate();
			}
		}

		protected virtual void SetLocalTransform(Vector3 newPosition, Quaternion newRotation, PoseDataFlags poseFlags)
		{
			if ((this.m_TrackingType == TrackedPoseDriver.TrackingType.RotationAndPosition || this.m_TrackingType == TrackedPoseDriver.TrackingType.RotationOnly) && (poseFlags & PoseDataFlags.Rotation) > PoseDataFlags.NoData)
			{
				base.transform.localRotation = newRotation;
			}
			if ((this.m_TrackingType == TrackedPoseDriver.TrackingType.RotationAndPosition || this.m_TrackingType == TrackedPoseDriver.TrackingType.PositionOnly) && (poseFlags & PoseDataFlags.Position) > PoseDataFlags.NoData)
			{
				base.transform.localPosition = newPosition;
			}
		}

		protected Pose TransformPoseByOriginIfNeeded(Pose pose)
		{
			if (this.m_UseRelativeTransform)
			{
				return pose.GetTransformedBy(this.m_OriginPose);
			}
			return pose;
		}

		private bool HasStereoCamera()
		{
			Camera component = base.GetComponent<Camera>();
			return component != null && component.stereoEnabled;
		}

		protected virtual void PerformUpdate()
		{
			if (!base.enabled)
			{
				return;
			}
			Pose pose;
			PoseDataFlags poseData = this.GetPoseData(this.m_Device, this.m_PoseSource, out pose);
			if (poseData != PoseDataFlags.NoData)
			{
				Pose pose2 = this.TransformPoseByOriginIfNeeded(pose);
				this.SetLocalTransform(pose2.position, pose2.rotation, poseData);
			}
		}

		[SerializeField]
		private TrackedPoseDriver.DeviceType m_Device;

		[SerializeField]
		private TrackedPoseDriver.TrackedPose m_PoseSource = TrackedPoseDriver.TrackedPose.Center;

		[SerializeField]
		private BasePoseProvider m_PoseProviderComponent;

		[SerializeField]
		private TrackedPoseDriver.TrackingType m_TrackingType;

		[SerializeField]
		private TrackedPoseDriver.UpdateType m_UpdateType;

		[SerializeField]
		private bool m_UseRelativeTransform;

		protected Pose m_OriginPose;

		public enum DeviceType
		{
			GenericXRDevice,
			GenericXRController,
			GenericXRRemote
		}

		public enum TrackedPose
		{
			LeftEye,
			RightEye,
			Center,
			Head,
			LeftPose,
			RightPose,
			ColorCamera,
			DepthCameraDeprecated,
			FisheyeCameraDeprected,
			DeviceDeprecated,
			RemotePose
		}

		public enum TrackingType
		{
			RotationAndPosition,
			RotationOnly,
			PositionOnly
		}

		public enum UpdateType
		{
			UpdateAndBeforeRender,
			Update,
			BeforeRender
		}
	}
}
