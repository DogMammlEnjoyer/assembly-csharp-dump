using System;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.XR;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class FromOVRHmdDataSource : DataSource<HmdDataAsset>
	{
		public IOVRCameraRigRef CameraRigRef { get; private set; }

		public bool ProcessLateUpdates
		{
			get
			{
				return this._processLateUpdates;
			}
			set
			{
				this._processLateUpdates = value;
			}
		}

		protected void Awake()
		{
			this.CameraRigRef = (this._cameraRigRef as IOVRCameraRigRef);
			this.TrackingToWorldTransformer = (this._trackingToWorldTransformer as ITrackingToWorldTransformer);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._started)
			{
				this.CameraRigRef.WhenInputDataDirtied += this.HandleInputDataDirtied;
			}
		}

		protected override void OnDisable()
		{
			if (this._started)
			{
				this.CameraRigRef.WhenInputDataDirtied -= this.HandleInputDataDirtied;
			}
			base.OnDisable();
			this.MarkInputDataRequiresUpdate();
		}

		private void HandleInputDataDirtied(bool isLateUpdate)
		{
			if (isLateUpdate && !this._processLateUpdates)
			{
				return;
			}
			this.MarkInputDataRequiresUpdate();
		}

		private HmdDataSourceConfig Config
		{
			get
			{
				if (this._config != null)
				{
					return this._config;
				}
				this._config = new HmdDataSourceConfig
				{
					TrackingToWorldTransformer = this.TrackingToWorldTransformer
				};
				return this._config;
			}
		}

		protected override void UpdateData()
		{
			this._hmdDataAsset.Config = this.Config;
			bool flag = OVRNodeStateProperties.IsHmdPresent() && base.isActiveAndEnabled;
			ref Pose ptr = ref this._hmdDataAsset.Root;
			if (this._useOvrManagerEmulatedPose)
			{
				Quaternion rotation = Quaternion.Euler(-OVRManager.instance.headPoseRelativeOffsetRotation.x, -OVRManager.instance.headPoseRelativeOffsetRotation.y, OVRManager.instance.headPoseRelativeOffsetRotation.z);
				ptr.rotation = rotation;
				ptr.position = OVRManager.instance.headPoseRelativeOffsetTranslation;
				flag = true;
			}
			else
			{
				Pose pose = Pose.identity;
				if (this._hmdDataAsset.IsTracked)
				{
					pose = this._hmdDataAsset.Root;
				}
				if (flag)
				{
					if (!OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.CenterEye, NodeStatePropertyType.Position, OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render, out ptr.position))
					{
						ptr.position = pose.position;
					}
					if (!OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.CenterEye, NodeStatePropertyType.Orientation, OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render, out ptr.rotation))
					{
						ptr.rotation = pose.rotation;
					}
				}
				else
				{
					ptr = pose;
				}
			}
			this._hmdDataAsset.IsTracked = flag;
			this._hmdDataAsset.FrameId = Time.frameCount;
		}

		protected override HmdDataAsset DataAsset
		{
			get
			{
				return this._hmdDataAsset;
			}
		}

		public void InjectAllFromOVRHmdDataSource(DataSource<HmdDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, bool useOvrManagerEmulatedPose, ITrackingToWorldTransformer trackingToWorldTransformer)
		{
			base.InjectAllDataSource(updateMode, updateAfter);
			this.InjectUseOvrManagerEmulatedPose(useOvrManagerEmulatedPose);
			this.InjectTrackingToWorldTransformer(trackingToWorldTransformer);
		}

		public void InjectUseOvrManagerEmulatedPose(bool useOvrManagerEmulatedPose)
		{
			this._useOvrManagerEmulatedPose = useOvrManagerEmulatedPose;
		}

		public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
		{
			this._trackingToWorldTransformer = (trackingToWorldTransformer as Object);
			this.TrackingToWorldTransformer = trackingToWorldTransformer;
		}

		[Header("OVR Data Source")]
		[SerializeField]
		[Interface(typeof(IOVRCameraRigRef), new Type[]
		{

		})]
		private Object _cameraRigRef;

		[SerializeField]
		private bool _processLateUpdates;

		[SerializeField]
		[Tooltip("If true, uses OVRManager.headPoseRelativeOffset rather than sensor data for HMD pose.")]
		private bool _useOvrManagerEmulatedPose;

		[Header("Shared Configuration")]
		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		private Object _trackingToWorldTransformer;

		private ITrackingToWorldTransformer TrackingToWorldTransformer;

		private HmdDataAsset _hmdDataAsset = new HmdDataAsset();

		private HmdDataSourceConfig _config;
	}
}
