using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class HandJointCache : SkeletonJointsCache
	{
		protected override bool TryGetParent(int joint, out int parent)
		{
			parent = (int)HandJointUtils.JointParentList[joint];
			return parent >= 0;
		}

		public HandJointCache() : base(26)
		{
			this._posesFromWristCollection = new ReadOnlyHandJointPoses(this._posesFromRoot);
			this._localPosesCollection = new ReadOnlyHandJointPoses(this._localPoses);
		}

		public void Update(HandDataAsset data, int dataVersion, Transform trackingSpace = null)
		{
			if (!data.IsDataValidAndConnected)
			{
				return;
			}
			base.Update(dataVersion, data.Root, data.JointPoses, data.HandScale, trackingSpace);
		}

		public bool GetAllLocalPoses(out ReadOnlyHandJointPoses localJointPoses)
		{
			base.UpdateAllLocalPoses();
			localJointPoses = this._localPosesCollection;
			return this._posesFromWristCollection.Count > 0;
		}

		public bool GetAllPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist)
		{
			base.UpdateAllPosesFromRoot();
			jointPosesFromWrist = this._posesFromWristCollection;
			return this._posesFromWristCollection.Count > 0;
		}

		public Pose GetLocalJointPose(HandJointId jointId)
		{
			return base.GetLocalJointPose((int)jointId);
		}

		public Pose GetJointPoseFromRoot(HandJointId jointId)
		{
			return base.GetJointPoseFromRoot((int)jointId);
		}

		public Pose GetWorldJointPose(HandJointId jointId)
		{
			return base.GetWorldJointPose((int)jointId);
		}

		[Obsolete("Use GetLocalJointPose instead")]
		public Pose LocalJointPose(HandJointId jointid)
		{
			return base.GetLocalJointPose((int)jointid);
		}

		[Obsolete("Use GetJointPoseFromRoot instead")]
		public Pose PoseFromWrist(HandJointId jointid)
		{
			return base.GetJointPoseFromRoot((int)jointid);
		}

		[Obsolete("Use GetWorldJointPose instead")]
		public Pose WorldJointPose(HandJointId jointid, Pose rootPose, float handScale)
		{
			return base.GetWorldJointPose((int)jointid);
		}

		private ReadOnlyHandJointPoses _posesFromWristCollection;

		private ReadOnlyHandJointPoses _localPosesCollection;
	}
}
