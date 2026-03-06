using System;
using System.Collections.Generic;
using Oculus.Interaction.Body.Input;
using Oculus.Interaction.Body.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction.Body.Samples
{
	public class LockedBodyPose : MonoBehaviour, IBodyPose
	{
		public event Action WhenBodyPoseUpdated = delegate()
		{
		};

		public ISkeletonMapping SkeletonMapping
		{
			get
			{
				return this.Pose.SkeletonMapping;
			}
		}

		public bool GetJointPoseLocal(BodyJointId bodyJointId, out Pose pose)
		{
			return this.Pose.GetJointPoseLocal(bodyJointId, out pose);
		}

		public bool GetJointPoseFromRoot(BodyJointId bodyJointId, out Pose pose)
		{
			return this._lockedPoses.TryGetValue(bodyJointId, out pose);
		}

		private void UpdateLockedBodyPose()
		{
			this._lockedPoses.Clear();
			for (int i = 0; i < 84; i++)
			{
				BodyJointId bodyJointId = (BodyJointId)i;
				Pose pose;
				Pose value;
				if (this.Pose.GetJointPoseFromRoot(this._referenceJoint, out pose) && this.Pose.GetJointPoseFromRoot(bodyJointId, out value))
				{
					ref pose.Invert();
					PoseUtils.Multiply(pose, value, ref value);
					PoseUtils.Multiply(this._referenceOffset, value, ref value);
					this._lockedPoses[bodyJointId] = value;
				}
			}
			this.WhenBodyPoseUpdated();
		}

		protected virtual void Awake()
		{
			this._lockedPoses = new Dictionary<BodyJointId, Pose>();
			this.Pose = (this._pose as IBodyPose);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.UpdateLockedBodyPose();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Pose.WhenBodyPoseUpdated += this.UpdateLockedBodyPose;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Pose.WhenBodyPoseUpdated -= this.UpdateLockedBodyPose;
			}
		}

		private static readonly Pose HIP_OFFSET = new Pose
		{
			position = new Vector3(0f, 0.923987f, 0f),
			rotation = Quaternion.Euler(0f, 270f, 270f)
		};

		[Tooltip("The body pose to be locked")]
		[SerializeField]
		[Interface(typeof(IBodyPose), new Type[]
		{

		})]
		private Object _pose;

		private IBodyPose Pose;

		[Tooltip("The body pose will be locked relative to this joint at the specified offset.")]
		[SerializeField]
		private BodyJointId _referenceJoint = BodyJointId.Body_Hips;

		[Tooltip("The reference joint will be placed at this offset from the root.")]
		[SerializeField]
		private Pose _referenceOffset = LockedBodyPose.HIP_OFFSET;

		protected bool _started;

		private Dictionary<BodyJointId, Pose> _lockedPoses;
	}
}
