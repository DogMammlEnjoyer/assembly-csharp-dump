using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class Hand : DataModifier<HandDataAsset>, IHand
	{
		public Handedness Handedness
		{
			get
			{
				return base.GetData().Config.Handedness;
			}
		}

		public ITrackingToWorldTransformer TrackingToWorldTransformer
		{
			get
			{
				return base.GetData().Config.TrackingToWorldTransformer;
			}
		}

		public HandSkeleton HandSkeleton
		{
			get
			{
				return base.GetData().Config.HandSkeleton;
			}
		}

		public event Action WhenHandUpdated = delegate()
		{
		};

		public bool IsConnected
		{
			get
			{
				return base.GetData().IsDataValidAndConnected;
			}
		}

		public bool IsHighConfidence
		{
			get
			{
				return base.GetData().IsHighConfidence;
			}
		}

		public bool IsDominantHand
		{
			get
			{
				return base.GetData().IsDominantHand;
			}
		}

		public float Scale
		{
			get
			{
				return ((this.TrackingToWorldTransformer != null) ? this.TrackingToWorldTransformer.Transform.lossyScale.x : 1f) * base.GetData().HandScale;
			}
		}

		protected override void Apply(HandDataAsset data)
		{
		}

		public override void MarkInputDataRequiresUpdate()
		{
			base.MarkInputDataRequiresUpdate();
			if (base.Started)
			{
				this.InitializeJointPosesCache();
				this.WhenHandUpdated();
			}
		}

		private void InitializeJointPosesCache()
		{
			if (this._jointPosesCache == null && base.GetData().IsDataValidAndConnected)
			{
				this._jointPosesCache = new HandJointCache();
			}
		}

		private void CheckJointPosesCacheUpdate()
		{
			if (this._jointPosesCache != null && this.CurrentDataVersion != this._jointPosesCache.LocalDataVersion)
			{
				HandJointCache jointPosesCache = this._jointPosesCache;
				HandDataAsset data = base.GetData();
				int currentDataVersion = this.CurrentDataVersion;
				ITrackingToWorldTransformer trackingToWorldTransformer = this.TrackingToWorldTransformer;
				jointPosesCache.Update(data, currentDataVersion, (trackingToWorldTransformer != null) ? trackingToWorldTransformer.Transform : null);
			}
		}

		public bool GetFingerIsPinching(HandFinger finger)
		{
			HandDataAsset data = base.GetData();
			return data.IsConnected && data.IsFingerPinching[(int)finger];
		}

		public bool GetIndexFingerIsPinching()
		{
			return this.GetFingerIsPinching(HandFinger.Index);
		}

		public bool IsPointerPoseValid
		{
			get
			{
				return this.IsPoseOriginAllowed(base.GetData().PointerPoseOrigin);
			}
		}

		public bool GetPointerPose(out Pose pose)
		{
			HandDataAsset data = base.GetData();
			return this.ValidatePose(data.PointerPose, data.PointerPoseOrigin, out pose);
		}

		public bool GetJointPose(HandJointId handJointId, out Pose pose)
		{
			pose = Pose.identity;
			Pose pose2;
			if (!this.IsTrackedDataValid || this._jointPosesCache == null || !this.GetRootPose(out pose2))
			{
				return false;
			}
			this.CheckJointPosesCacheUpdate();
			pose = this._jointPosesCache.GetWorldJointPose(handJointId);
			return true;
		}

		public bool GetJointPoseLocal(HandJointId handJointId, out Pose pose)
		{
			pose = Pose.identity;
			ReadOnlyHandJointPoses readOnlyHandJointPoses;
			if (!this.GetJointPosesLocal(out readOnlyHandJointPoses))
			{
				return false;
			}
			pose = readOnlyHandJointPoses[(int)handJointId];
			return true;
		}

		public bool GetJointPosesLocal(out ReadOnlyHandJointPoses localJointPoses)
		{
			if (!this.IsTrackedDataValid || this._jointPosesCache == null)
			{
				localJointPoses = ReadOnlyHandJointPoses.Empty;
				return false;
			}
			this.CheckJointPosesCacheUpdate();
			return this._jointPosesCache.GetAllLocalPoses(out localJointPoses);
		}

		public bool GetJointPoseFromWrist(HandJointId handJointId, out Pose pose)
		{
			pose = Pose.identity;
			ReadOnlyHandJointPoses readOnlyHandJointPoses;
			if (!this.GetJointPosesFromWrist(out readOnlyHandJointPoses))
			{
				return false;
			}
			pose = readOnlyHandJointPoses[(int)handJointId];
			return true;
		}

		public bool GetJointPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist)
		{
			if (!this.IsTrackedDataValid || this._jointPosesCache == null)
			{
				jointPosesFromWrist = ReadOnlyHandJointPoses.Empty;
				return false;
			}
			this.CheckJointPosesCacheUpdate();
			return this._jointPosesCache.GetAllPosesFromWrist(out jointPosesFromWrist);
		}

		public bool GetPalmPoseLocal(out Pose pose)
		{
			Quaternion identity = Quaternion.identity;
			Vector3 a = Hand.PALM_LOCAL_OFFSET;
			if (this.Handedness == Handedness.Left)
			{
				a = -a;
			}
			pose = new Pose(a * this.Scale, identity);
			return true;
		}

		public bool GetFingerIsHighConfidence(HandFinger finger)
		{
			return base.GetData().IsFingerHighConfidence[(int)finger];
		}

		public float GetFingerPinchStrength(HandFinger finger)
		{
			return base.GetData().FingerPinchStrength[(int)finger];
		}

		public bool IsTrackedDataValid
		{
			get
			{
				return this.IsPoseOriginAllowed(base.GetData().RootPoseOrigin);
			}
		}

		public bool GetRootPose(out Pose pose)
		{
			HandDataAsset data = base.GetData();
			return this.ValidatePose(data.Root, data.RootPoseOrigin, out pose);
		}

		private bool ValidatePose(in Pose sourcePose, PoseOrigin sourcePoseOrigin, out Pose pose)
		{
			if (this.IsPoseOriginDisallowed(sourcePoseOrigin))
			{
				pose = Pose.identity;
				return false;
			}
			pose = ((this.TrackingToWorldTransformer != null) ? this.TrackingToWorldTransformer.ToWorldPose(sourcePose) : sourcePose);
			return true;
		}

		private bool IsPoseOriginAllowed(PoseOrigin poseOrigin)
		{
			return poseOrigin > PoseOrigin.None;
		}

		private bool IsPoseOriginDisallowed(PoseOrigin poseOrigin)
		{
			return poseOrigin == PoseOrigin.None;
		}

		public void InjectAllHand(DataSource<HandDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, DataModifier<HandDataAsset> modifyDataFromSource, bool applyModifier)
		{
			base.InjectAllDataModifier(updateMode, updateAfter, modifyDataFromSource, applyModifier);
		}

		private HandJointCache _jointPosesCache;

		private static readonly Vector3 PALM_LOCAL_OFFSET = new Vector3(0.08f, -0.01f, 0f);
	}
}
