using System;
using Oculus.Interaction.Body.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.PoseDetection
{
	public class BodyPoseDebugGizmos : SkeletonDebugGizmos
	{
		protected virtual void Awake()
		{
			this.BodyPose = (this._bodyPose as IBodyPose);
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			foreach (BodyJointId joint in this.BodyPose.SkeletonMapping.Joints)
			{
				base.Draw((int)joint, this.GetVisibilityFlags());
			}
		}

		private SkeletonDebugGizmos.VisibilityFlags GetVisibilityFlags()
		{
			SkeletonDebugGizmos.VisibilityFlags visibilityFlags = base.Visibility;
			if (base.HasNegativeScale)
			{
				visibilityFlags &= ~SkeletonDebugGizmos.VisibilityFlags.Axes;
			}
			return visibilityFlags;
		}

		protected override bool TryGetJointPose(int jointId, out Pose pose)
		{
			if (this.BodyPose.GetJointPoseFromRoot((BodyJointId)jointId, out pose))
			{
				pose.position = base.transform.TransformPoint(pose.position);
				pose.rotation = base.transform.rotation * pose.rotation;
				return true;
			}
			return false;
		}

		protected override bool TryGetParentJointId(int jointId, out int parent)
		{
			BodyJointId bodyJointId;
			if (this.BodyPose.SkeletonMapping.TryGetParentJointId((BodyJointId)jointId, out bodyJointId))
			{
				parent = (int)bodyJointId;
				return true;
			}
			parent = 0;
			return false;
		}

		public void InjectAllBodyJointDebugGizmos(IBodyPose bodyPose)
		{
			this.InjectBodyPose(bodyPose);
		}

		public void InjectBodyPose(IBodyPose bodyPose)
		{
			this._bodyPose = (bodyPose as Object);
			this.BodyPose = bodyPose;
		}

		[Tooltip("The IBodyPose that will drive the visuals.")]
		[SerializeField]
		[Interface(typeof(IBodyPose), new Type[]
		{

		})]
		private Object _bodyPose;

		private IBodyPose BodyPose;
	}
}
