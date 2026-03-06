using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public abstract class SkeletonJointsCache
	{
		public int LocalDataVersion { get; private set; } = -1;

		protected abstract bool TryGetParent(int joint, out int parent);

		public SkeletonJointsCache(int numJoints)
		{
			this.LocalDataVersion = -1;
			this._numJoints = numJoints;
			this._originalPoses = new Pose[numJoints];
			this._posesFromRoot = new Pose[numJoints];
			this._localPoses = new Pose[numJoints];
			this._worldPoses = new Pose[numJoints];
			this._dirtyArraySize = 1 + numJoints / 64;
			this._dirtyJointsFromRoot = new ulong[this._dirtyArraySize];
			this._dirtyLocalJoints = new ulong[this._dirtyArraySize];
			this._dirtyWorldJoints = new ulong[this._dirtyArraySize];
		}

		public void Update(int dataVersion, Pose rootPose, Pose[] jointPoses, float scale, Transform trackingSpace = null)
		{
			this.LocalDataVersion = dataVersion;
			for (int i = 0; i < this._dirtyArraySize; i++)
			{
				this._dirtyJointsFromRoot[i] = ulong.MaxValue;
				this._dirtyLocalJoints[i] = ulong.MaxValue;
				this._dirtyWorldJoints[i] = ulong.MaxValue;
			}
			this._scale = Matrix4x4.Scale(Vector3.one * scale);
			this._rootPose = rootPose;
			this._worldRoot = this._rootPose;
			if (trackingSpace != null)
			{
				this._scale *= Matrix4x4.Scale(trackingSpace.lossyScale);
				this._worldRoot.position = trackingSpace.TransformPoint(this._rootPose.position);
				this._worldRoot.rotation = trackingSpace.rotation * this._rootPose.rotation;
			}
			Array.Copy(jointPoses, this._originalPoses, this._numJoints);
		}

		public Pose GetLocalJointPose(int jointId)
		{
			this.UpdateLocalJointPose(jointId);
			return this._localPoses[jointId];
		}

		public Pose GetJointPoseFromRoot(int jointId)
		{
			this.UpdateJointPoseFromRoot(jointId);
			return this._posesFromRoot[jointId];
		}

		public Pose GetWorldJointPose(int jointId)
		{
			this.UpdateWorldJointPose(jointId);
			return this._worldPoses[jointId];
		}

		public Pose GetWorldRootPose()
		{
			return this._worldRoot;
		}

		private void UpdateJointPoseFromRoot(int jointId)
		{
			if (!this.CheckJointDirty(jointId, this._dirtyJointsFromRoot))
			{
				return;
			}
			this._posesFromRoot[jointId] = this._originalPoses[jointId];
			this.SetJointClean(jointId, this._dirtyJointsFromRoot);
		}

		private void UpdateLocalJointPose(int jointId)
		{
			if (!this.CheckJointDirty(jointId, this._dirtyLocalJoints))
			{
				return;
			}
			int num;
			if (this.TryGetParent(jointId, out num))
			{
				Pose pose = this._originalPoses[jointId];
				Pose pose2 = this._originalPoses[num];
				Vector3 position = Quaternion.Inverse(pose2.rotation) * (pose.position - pose2.position);
				Quaternion rotation = Quaternion.Inverse(pose2.rotation) * pose.rotation;
				this._localPoses[jointId] = new Pose(position, rotation);
			}
			else
			{
				this._localPoses[jointId] = Pose.identity;
			}
			this.SetJointClean(jointId, this._dirtyLocalJoints);
		}

		private void UpdateWorldJointPose(int jointId)
		{
			if (!this.CheckJointDirty(jointId, this._dirtyWorldJoints))
			{
				return;
			}
			Pose jointPoseFromRoot = this.GetJointPoseFromRoot(jointId);
			jointPoseFromRoot.position = this._scale * jointPoseFromRoot.position;
			Pose worldRootPose = this.GetWorldRootPose();
			ref jointPoseFromRoot.Postmultiply(worldRootPose);
			this._worldPoses[jointId] = jointPoseFromRoot;
			this.SetJointClean(jointId, this._dirtyWorldJoints);
		}

		protected void UpdateAllWorldPoses()
		{
			for (int i = 0; i < this._numJoints; i++)
			{
				this.UpdateWorldJointPose(i);
			}
		}

		protected void UpdateAllLocalPoses()
		{
			for (int i = 0; i < this._numJoints; i++)
			{
				this.UpdateLocalJointPose(i);
			}
		}

		protected void UpdateAllPosesFromRoot()
		{
			for (int i = 0; i < this._numJoints; i++)
			{
				this.UpdateJointPoseFromRoot(i);
			}
		}

		private bool CheckJointDirty(int jointId, ulong[] dirtyFlags)
		{
			int num = jointId / 64;
			int num2 = jointId % 64;
			return (dirtyFlags[num] & 1UL << num2) > 0UL;
		}

		private void SetJointClean(int jointId, ulong[] dirtyFlags)
		{
			int num = jointId / 64;
			int num2 = jointId % 64;
			dirtyFlags[num] &= ~(1UL << num2);
		}

		private const int ULONG_BITS = 64;

		protected Pose[] _originalPoses;

		protected Pose[] _posesFromRoot;

		protected Pose[] _localPoses;

		protected Pose[] _worldPoses;

		private ulong[] _dirtyJointsFromRoot;

		private ulong[] _dirtyLocalJoints;

		private ulong[] _dirtyWorldJoints;

		private Matrix4x4 _scale;

		private Pose _rootPose;

		private Pose _worldRoot;

		private readonly int _numJoints;

		private readonly int _dirtyArraySize;
	}
}
