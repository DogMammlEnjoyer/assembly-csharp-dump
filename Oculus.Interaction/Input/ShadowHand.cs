using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class ShadowHand
	{
		public ShadowHand()
		{
			for (int i = 0; i < this._localJointMap.Length; i++)
			{
				this._localJointMap[i] = Pose.identity;
				this._worldJointMap[i] = Pose.identity;
			}
			this._rootPose = Pose.identity;
			this._rootScale = 1f;
			this._dirtyMap = 0UL;
		}

		public Pose GetLocalPose(HandJointId handJointId)
		{
			return this._localJointMap[(int)handJointId];
		}

		public void SetLocalPose(HandJointId jointId, Pose pose)
		{
			this._localJointMap[(int)jointId] = pose;
			this.MarkDirty(jointId);
		}

		public Pose GetWorldPose(HandJointId jointId)
		{
			this.UpdateDirty(jointId);
			return this._worldJointMap[(int)jointId];
		}

		public Pose[] GetWorldPoses()
		{
			this.UpdateDirty(HandJointId.HandWristRoot);
			return this._worldJointMap;
		}

		public Pose GetRoot()
		{
			return this._rootPose;
		}

		public void SetRoot(Pose rootPose)
		{
			this._rootPose = rootPose;
			this.MarkDirty(HandJointId.HandStart);
		}

		public float GetRootScale()
		{
			return this._rootScale;
		}

		public void SetRootScale(float scale)
		{
			this._rootScale = scale;
			this.MarkDirty(HandJointId.HandStart);
		}

		private bool CheckDirtyBit(int i)
		{
			return (this._dirtyMap >> i & 1UL) == 1UL;
		}

		private void SetDirtyBit(int i)
		{
			this._dirtyMap |= 1UL << i;
		}

		private void ClearDirtyBit(int i)
		{
			this._dirtyMap &= ~(1UL << i);
		}

		private void MarkDirty(HandJointId jointId)
		{
			if (this.CheckDirtyBit((int)jointId))
			{
				return;
			}
			this.SetDirtyBit((int)jointId);
			foreach (HandJointId jointId2 in HandJointUtils.JointChildrenList[(int)jointId])
			{
				this.MarkDirty(jointId2);
			}
		}

		private void UpdateDirty(HandJointId jointId)
		{
			if (!this.CheckDirtyBit((int)jointId))
			{
				return;
			}
			HandJointId handJointId = HandJointUtils.JointParentList[(int)jointId];
			if (handJointId != HandJointId.Invalid)
			{
				this.UpdateDirty(handJointId);
			}
			this.ClearDirtyBit((int)jointId);
			Pose pose = (handJointId != HandJointId.Invalid) ? this.GetWorldPose(handJointId) : this._rootPose;
			Pose pose2 = this._localJointMap[(int)jointId];
			pose2.position *= this._rootScale;
			PoseUtils.Multiply(pose, pose2, ref this._worldJointMap[(int)jointId]);
		}

		public void Copy(ShadowHand hand)
		{
			this.SetRoot(hand.GetRoot());
			this.SetRootScale(hand.GetRootScale());
			for (int i = 0; i < 26; i++)
			{
				HandJointId handJointId = (HandJointId)i;
				this.SetLocalPose(handJointId, hand.GetLocalPose(handJointId));
			}
		}

		private readonly Pose[] _localJointMap = new Pose[26];

		private readonly Pose[] _worldJointMap = new Pose[26];

		private Pose _rootPose;

		private float _rootScale;

		private ulong _dirtyMap;
	}
}
