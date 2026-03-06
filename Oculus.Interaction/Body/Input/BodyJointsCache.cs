using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.Input
{
	public class BodyJointsCache : SkeletonJointsCache
	{
		protected override bool TryGetParent(int joint, out int parent)
		{
			BodyJointId bodyJointId;
			if (this._mapping.TryGetParentJointId((BodyJointId)joint, out bodyJointId))
			{
				parent = (int)bodyJointId;
				return true;
			}
			parent = -1;
			return false;
		}

		public BodyJointsCache(ISkeletonMapping mapping) : base(84)
		{
			this._mapping = mapping;
			this._localPosesCollection = new ReadOnlyBodyJointPoses(this._localPoses);
			this._worldPosesCollection = new ReadOnlyBodyJointPoses(this._worldPoses);
			this._posesFromRootCollection = new ReadOnlyBodyJointPoses(this._posesFromRoot);
		}

		public void Update(BodyDataAsset data, int dataVersion, Transform trackingSpace = null)
		{
			if (!data.IsDataValid)
			{
				return;
			}
			base.Update(dataVersion, data.Root, data.JointPoses, data.RootScale, trackingSpace);
		}

		public Pose GetLocalJointPose(BodyJointId jointId)
		{
			return base.GetLocalJointPose((int)jointId);
		}

		public Pose GetJointPoseFromRoot(BodyJointId jointId)
		{
			return base.GetJointPoseFromRoot((int)jointId);
		}

		public Pose GetWorldJointPose(BodyJointId jointId)
		{
			return base.GetWorldJointPose((int)jointId);
		}

		[Obsolete]
		public bool GetAllLocalPoses(out ReadOnlyBodyJointPoses localJointPoses)
		{
			base.UpdateAllLocalPoses();
			localJointPoses = this._localPosesCollection;
			return this._localPosesCollection.Count > 0;
		}

		[Obsolete]
		public bool GetAllPosesFromRoot(out ReadOnlyBodyJointPoses posesFromRoot)
		{
			base.UpdateAllPosesFromRoot();
			posesFromRoot = this._posesFromRootCollection;
			return this._posesFromRootCollection.Count > 0;
		}

		[Obsolete]
		public bool GetAllWorldPoses(out ReadOnlyBodyJointPoses worldJointPoses)
		{
			base.UpdateAllWorldPoses();
			worldJointPoses = this._worldPosesCollection;
			return this._worldPosesCollection.Count > 0;
		}

		private ReadOnlyBodyJointPoses _posesFromRootCollection;

		private ReadOnlyBodyJointPoses _worldPosesCollection;

		private ReadOnlyBodyJointPoses _localPosesCollection;

		private readonly ISkeletonMapping _mapping;
	}
}
