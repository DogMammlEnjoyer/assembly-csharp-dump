using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.Input
{
	public class Body : DataModifier<BodyDataAsset>, IBody
	{
		public bool IsConnected
		{
			get
			{
				return base.GetData().IsDataValid;
			}
		}

		public bool IsHighConfidence
		{
			get
			{
				return base.GetData().IsDataHighConfidence;
			}
		}

		public float Scale
		{
			get
			{
				return base.GetData().RootScale;
			}
		}

		public ISkeletonMapping SkeletonMapping
		{
			get
			{
				return base.GetData().SkeletonMapping;
			}
		}

		public bool IsTrackedDataValid
		{
			get
			{
				return base.GetData().IsDataValid;
			}
		}

		public event Action WhenBodyUpdated = delegate()
		{
		};

		public bool GetJointPose(BodyJointId bodyJointId, out Pose pose)
		{
			pose = Pose.identity;
			if (!this.IsTrackedDataValid || !this.SkeletonMapping.Joints.Contains(bodyJointId))
			{
				return false;
			}
			this.CheckJointPosesCacheUpdate();
			pose = this._jointPosesCache.GetWorldJointPose(bodyJointId);
			return true;
		}

		public bool GetJointPoseLocal(BodyJointId bodyJointId, out Pose pose)
		{
			pose = Pose.identity;
			if (!this.IsTrackedDataValid || !this.SkeletonMapping.Joints.Contains(bodyJointId))
			{
				return false;
			}
			this.CheckJointPosesCacheUpdate();
			pose = this._jointPosesCache.GetLocalJointPose(bodyJointId);
			return true;
		}

		public bool GetJointPoseFromRoot(BodyJointId bodyJointId, out Pose pose)
		{
			pose = Pose.identity;
			if (!this.IsTrackedDataValid || !this.SkeletonMapping.Joints.Contains(bodyJointId))
			{
				return false;
			}
			this.CheckJointPosesCacheUpdate();
			pose = this._jointPosesCache.GetJointPoseFromRoot(bodyJointId);
			return true;
		}

		public bool GetRootPose(out Pose pose)
		{
			pose = Pose.identity;
			if (!this.IsTrackedDataValid)
			{
				return false;
			}
			this.CheckJointPosesCacheUpdate();
			pose = this._jointPosesCache.GetWorldRootPose();
			return true;
		}

		private void InitializeJointPosesCache()
		{
			if (this._jointPosesCache == null)
			{
				this._jointPosesCache = new BodyJointsCache(this.SkeletonMapping);
			}
		}

		private void CheckJointPosesCacheUpdate()
		{
			if (this._jointPosesCache != null && this.CurrentDataVersion != this._jointPosesCache.LocalDataVersion)
			{
				this._jointPosesCache.Update(base.GetData(), this.CurrentDataVersion, this._trackingSpace);
			}
		}

		protected override void Apply(BodyDataAsset data)
		{
		}

		public override void MarkInputDataRequiresUpdate()
		{
			base.MarkInputDataRequiresUpdate();
			if (base.Started)
			{
				this.InitializeJointPosesCache();
				this.WhenBodyUpdated();
			}
		}

		[Tooltip("If assigned, joint pose translations into world space will be performed via this transform. If unassigned, world joint poses will be returned in tracking space.")]
		[SerializeField]
		[Optional]
		private Transform _trackingSpace;

		private BodyJointsCache _jointPosesCache;
	}
}
