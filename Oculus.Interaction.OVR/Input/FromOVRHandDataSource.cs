using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class FromOVRHandDataSource : DataSource<HandDataAsset>
	{
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

		protected override HandDataAsset DataAsset
		{
			get
			{
				return this._handDataAsset;
			}
		}

		public static Quaternion WristFixupRotation { get; } = new Quaternion(0f, 1f, 0f, 0f);

		protected virtual void Awake()
		{
			this.TrackingToWorldTransformer = (this._trackingToWorldTransformer as ITrackingToWorldTransformer);
			this.CameraRigRef = (this._cameraRigRef as IOVRCameraRigRef);
			this.HandSkeletonProvider = (this._handSkeletonProvider as IHandSkeletonProvider);
			this.UpdateConfig();
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (this._ovrHand == null)
			{
				this._ovrHand = ((this._handedness == Handedness.Left) ? this.CameraRigRef.LeftHand : this.CameraRigRef.RightHand);
			}
			this.UpdateConfig();
			OVRHandSkeletonVersion handSkeletonVersion = OVRRuntimeSettings.GetRuntimeSettings().HandSkeletonVersion;
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

		private HandDataSourceConfig Config
		{
			get
			{
				if (this._config != null)
				{
					return this._config;
				}
				this._config = new HandDataSourceConfig
				{
					Handedness = this._handedness
				};
				return this._config;
			}
		}

		private void UpdateConfig()
		{
			this.Config.Handedness = this._handedness;
			this.Config.TrackingToWorldTransformer = this.TrackingToWorldTransformer;
			this.Config.HandSkeleton = this.HandSkeletonProvider[this._handedness];
		}

		protected override void UpdateData()
		{
			this._handDataAsset.Config = this.Config;
			this._handDataAsset.IsDataValid = true;
			this._handDataAsset.IsConnected = false;
			if (this._ovrHand != null && this._ovrHand.isActiveAndEnabled && base.isActiveAndEnabled)
			{
				OVRSkeleton.SkeletonPoseData skeletonPoseData = ((OVRSkeleton.IOVRSkeletonDataProvider)this._ovrHand).GetSkeletonPoseData();
				this._handDataAsset.IsConnected = (skeletonPoseData.IsDataValid && skeletonPoseData.RootScale > 0f);
				if (!this._handDataAsset.IsConnected)
				{
					if (this._lastHandScale <= 0f)
					{
						skeletonPoseData.IsDataValid = false;
					}
					else
					{
						skeletonPoseData.RootScale = this._lastHandScale;
					}
				}
				else
				{
					this._lastHandScale = skeletonPoseData.RootScale;
				}
				if (skeletonPoseData.IsDataValid && this._handDataAsset.IsConnected)
				{
					this.UpdateDataPoses(skeletonPoseData);
					return;
				}
			}
			this._handDataAsset.IsConnected = false;
			this._handDataAsset.IsTracked = false;
			this._handDataAsset.RootPoseOrigin = PoseOrigin.None;
			this._handDataAsset.PointerPoseOrigin = PoseOrigin.None;
			this._handDataAsset.IsHighConfidence = false;
			for (int i = 0; i < 5; i++)
			{
				this._handDataAsset.IsFingerPinching[i] = false;
				this._handDataAsset.IsFingerHighConfidence[i] = false;
			}
		}

		private void UpdateDataPoses(OVRSkeleton.SkeletonPoseData poseData)
		{
			this._handDataAsset.HandScale = poseData.RootScale;
			this._handDataAsset.IsTracked = this._ovrHand.IsTracked;
			this._handDataAsset.IsHighConfidence = poseData.IsDataHighConfidence;
			this._handDataAsset.IsDominantHand = this._ovrHand.IsDominantHand;
			this._handDataAsset.RootPoseOrigin = (this._handDataAsset.IsTracked ? PoseOrigin.RawTrackedPose : PoseOrigin.None);
			for (int i = 0; i < 5; i++)
			{
				OVRHand.HandFinger finger = (OVRHand.HandFinger)i;
				bool fingerIsPinching = this._ovrHand.GetFingerIsPinching(finger);
				this._handDataAsset.IsFingerPinching[i] = fingerIsPinching;
				bool flag = this._ovrHand.GetFingerConfidence(finger) == OVRHand.TrackingConfidence.High;
				this._handDataAsset.IsFingerHighConfidence[i] = flag;
				float fingerPinchStrength = this._ovrHand.GetFingerPinchStrength(finger);
				this._handDataAsset.FingerPinchStrength[i] = fingerPinchStrength;
			}
			this._handDataAsset.Root = new Pose
			{
				position = poseData.RootPose.Position.FromFlippedZVector3f(),
				rotation = poseData.RootPose.Orientation.FromFlippedZQuatf()
			};
			if (this._ovrHand.IsPointerPoseValid)
			{
				this._handDataAsset.PointerPoseOrigin = PoseOrigin.RawTrackedPose;
				this._handDataAsset.PointerPose = new Pose(this._ovrHand.PointerPose.localPosition, this._ovrHand.PointerPose.localRotation);
			}
			else
			{
				this._handDataAsset.PointerPoseOrigin = PoseOrigin.None;
			}
			float d = (this._handDataAsset.HandScale > 0f) ? (1f / this._handDataAsset.HandScale) : 0f;
			OVRPlugin.Skeleton2 skeleton = (this._handedness == Handedness.Left) ? OVRSkeletonData.LeftSkeleton : OVRSkeletonData.RightSkeleton;
			for (int j = 0; j < 26; j++)
			{
				Pose pose = new Pose(poseData.BoneTranslations[j].FromFlippedZVector3f(), poseData.BoneRotations[j].FromFlippedZQuatf());
				Pose pose2 = PoseUtils.Delta(this._handDataAsset.Root, pose);
				pose2.position *= d;
				this._handDataAsset.JointPoses[j] = pose2;
				this._handDataAsset.JointRadii[j] = HandSkeletonOVR.GetBoneRadius(skeleton, j);
			}
			HandJointUtils.WristJointPosesToLocalRotations(this._handDataAsset.JointPoses, ref this._handDataAsset.Joints);
		}

		public void InjectAllFromOVRHandDataSource(DataSource<HandDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, Handedness handedness, ITrackingToWorldTransformer trackingToWorldTransformer, IHandSkeletonProvider handSkeletonProvider)
		{
			base.InjectAllDataSource(updateMode, updateAfter);
			this.InjectHandedness(handedness);
			this.InjectTrackingToWorldTransformer(trackingToWorldTransformer);
			this.InjectHandSkeletonProvider(handSkeletonProvider);
		}

		public void InjectHandedness(Handedness handedness)
		{
			this._handedness = handedness;
		}

		public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
		{
			this._trackingToWorldTransformer = (trackingToWorldTransformer as Object);
			this.TrackingToWorldTransformer = trackingToWorldTransformer;
		}

		public void InjectHandSkeletonProvider(IHandSkeletonProvider handSkeletonProvider)
		{
			this._handSkeletonProvider = (handSkeletonProvider as Object);
			this.HandSkeletonProvider = handSkeletonProvider;
		}

		public void InjectOptionalOVRHand(OVRHand ovrHand)
		{
			this._ovrHand = ovrHand;
		}

		[Header("OVR Data Source")]
		[SerializeField]
		[Interface(typeof(IOVRCameraRigRef), new Type[]
		{

		})]
		private Object _cameraRigRef;

		[SerializeField]
		private bool _processLateUpdates;

		[Header("Shared Configuration")]
		[SerializeField]
		private Handedness _handedness;

		[SerializeField]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		private OVRHand _ovrHand;

		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		private Object _trackingToWorldTransformer;

		private ITrackingToWorldTransformer TrackingToWorldTransformer;

		[SerializeField]
		[Interface(typeof(IHandSkeletonProvider), new Type[]
		{

		})]
		private Object _handSkeletonProvider;

		private IHandSkeletonProvider HandSkeletonProvider;

		private readonly HandDataAsset _handDataAsset = new HandDataAsset();

		private float _lastHandScale;

		private HandDataSourceConfig _config;

		private IOVRCameraRigRef CameraRigRef;
	}
}
