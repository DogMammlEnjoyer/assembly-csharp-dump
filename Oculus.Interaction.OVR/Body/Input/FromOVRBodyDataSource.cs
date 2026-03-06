using System;
using Meta.XR.Util;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.Input
{
	[Feature(Feature.Interaction)]
	public class FromOVRBodyDataSource : DataSource<BodyDataAsset>
	{
		protected override BodyDataAsset DataAsset
		{
			get
			{
				return this._bodyDataAsset;
			}
		}

		private static OVRPlugin.BodyJointSet GetJointSet(OVRSkeleton.IOVRSkeletonDataProvider provider)
		{
			OVRSkeleton.SkeletonType skeletonType = provider.GetSkeletonType();
			OVRPlugin.BodyJointSet result;
			if (skeletonType != OVRSkeleton.SkeletonType.Body)
			{
				if (skeletonType != OVRSkeleton.SkeletonType.FullBody)
				{
					result = OVRPlugin.BodyJointSet.None;
				}
				else
				{
					result = OVRPlugin.BodyJointSet.FullBody;
				}
			}
			else
			{
				result = OVRPlugin.BodyJointSet.UpperBody;
			}
			return result;
		}

		protected void Awake()
		{
			this.CameraRigRef = (this._cameraRigRef as IOVRCameraRigRef);
			this.DataProvider = (this._dataProvider as OVRSkeleton.IOVRSkeletonDataProvider);
		}

		protected override void Start()
		{
			base.Start();
			this._mapping = new OVRSkeletonMapping(FromOVRBodyDataSource.GetJointSet(this.DataProvider));
			this._bodyDataAsset.SkeletonMapping = this._mapping;
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
		}

		private void HandleInputDataDirtied(bool isLateUpdate)
		{
			if (isLateUpdate && !this._processLateUpdates)
			{
				return;
			}
			this.MarkInputDataRequiresUpdate();
		}

		protected override void UpdateData()
		{
			OVRSkeleton.SkeletonPoseData skeletonPoseData = this.DataProvider.GetSkeletonPoseData();
			if (!skeletonPoseData.IsDataValid)
			{
				return;
			}
			this._bodyDataAsset.SkeletonMapping = this._mapping;
			this._bodyDataAsset.IsDataHighConfidence = skeletonPoseData.IsDataHighConfidence;
			this._bodyDataAsset.IsDataValid = skeletonPoseData.IsDataValid;
			this._bodyDataAsset.SkeletonChangedCount = skeletonPoseData.SkeletonChangedCount;
			this._bodyDataAsset.RootScale = skeletonPoseData.RootScale;
			BodyDataAsset bodyDataAsset = this._bodyDataAsset;
			Pose pose = new Pose
			{
				position = skeletonPoseData.RootPose.Position.FromFlippedZVector3f(),
				rotation = skeletonPoseData.RootPose.Orientation.FromFlippedZQuatf()
			};
			bodyDataAsset.Root = pose;
			foreach (BodyJointId bodyJointId in this._mapping.Joints)
			{
				Pose pose2 = default(Pose);
				OVRPlugin.BoneId boneId;
				if (this._mapping.TryGetSourceJointId(bodyJointId, out boneId))
				{
					int num = (int)boneId;
					pose = new Pose
					{
						rotation = (float.IsNaN(skeletonPoseData.BoneRotations[num].w) ? default(Quaternion) : skeletonPoseData.BoneRotations[num].FromFlippedZQuatf()),
						position = skeletonPoseData.BoneTranslations[num].FromFlippedZVector3f()
					};
					pose2 = pose;
				}
				Pose[] jointPoses = this._bodyDataAsset.JointPoses;
				int num2 = (int)bodyJointId;
				pose = this._bodyDataAsset.Root;
				jointPoses[num2] = PoseUtils.Delta(pose, pose2);
			}
		}

		[Header("OVR Data Source")]
		[SerializeField]
		[Interface(typeof(OVRSkeleton.IOVRSkeletonDataProvider), new Type[]
		{

		})]
		private Object _dataProvider;

		private OVRSkeleton.IOVRSkeletonDataProvider DataProvider;

		[SerializeField]
		[Interface(typeof(IOVRCameraRigRef), new Type[]
		{

		})]
		private Object _cameraRigRef;

		private IOVRCameraRigRef CameraRigRef;

		[SerializeField]
		private bool _processLateUpdates;

		private readonly BodyDataAsset _bodyDataAsset = new BodyDataAsset();

		private OVRSkeletonMapping _mapping;
	}
}
