using System;
using System.Collections.Generic;
using Oculus.Interaction.Body.Input;
using Oculus.Interaction.Collections;
using UnityEngine;

namespace Oculus.Interaction.Body.PoseDetection
{
	[CreateAssetMenu(menuName = "Meta/Interaction/SDK/Pose Detection/Body Pose")]
	public class BodyPoseData : ScriptableObject, IBodyPose, ISerializationCallbackReceiver
	{
		public event Action WhenBodyPoseUpdated = delegate()
		{
		};

		public bool GetJointPoseFromRoot(BodyJointId bodyJointId, out Pose pose)
		{
			return this._posesFromRoot.TryGetValue(bodyJointId, out pose);
		}

		public bool GetJointPoseLocal(BodyJointId bodyJointId, out Pose pose)
		{
			return this._localPoses.TryGetValue(bodyJointId, out pose);
		}

		public ISkeletonMapping SkeletonMapping
		{
			get
			{
				return this._mapping;
			}
		}

		public void SetBodyPose(IBody body)
		{
			this._jointData.Clear();
			foreach (BodyJointId bodyJointId in body.SkeletonMapping.Joints)
			{
				Pose localPose;
				Pose poseFromRoot;
				BodyJointId parentId;
				if (body.GetJointPoseLocal(bodyJointId, out localPose) && body.GetJointPoseFromRoot(bodyJointId, out poseFromRoot) && body.SkeletonMapping.TryGetParentJointId(bodyJointId, out parentId))
				{
					this._jointData.Add(new BodyPoseData.JointData
					{
						JointId = bodyJointId,
						ParentId = parentId,
						PoseFromRoot = poseFromRoot,
						LocalPose = localPose
					});
				}
			}
			this._serializedVersion = 1;
			this.Rebuild();
			this.WhenBodyPoseUpdated();
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			this.Rebuild();
		}

		private void Rebuild()
		{
			this._localPoses.Clear();
			this._posesFromRoot.Clear();
			this._mapping.Joints.Clear();
			this._mapping.JointToParent.Clear();
			for (int i = 0; i < this._jointData.Count; i++)
			{
				this._localPoses[this._jointData[i].JointId] = this._jointData[i].LocalPose;
				this._posesFromRoot[this._jointData[i].JointId] = this._jointData[i].PoseFromRoot;
				this._mapping.Joints.Add(this._jointData[i].JointId);
				this._mapping.JointToParent.Add(this._jointData[i].JointId, this._jointData[i].ParentId);
			}
		}

		internal const int DATA_VERSION = 1;

		[SerializeField]
		[HideInInspector]
		private int _serializedVersion;

		[SerializeField]
		[HideInInspector]
		private List<BodyPoseData.JointData> _jointData = new List<BodyPoseData.JointData>();

		private Dictionary<BodyJointId, Pose> _posesFromRoot = new Dictionary<BodyJointId, Pose>();

		private Dictionary<BodyJointId, Pose> _localPoses = new Dictionary<BodyJointId, Pose>();

		private BodyPoseData.Mapping _mapping = new BodyPoseData.Mapping();

		[Serializable]
		internal struct JointData
		{
			public BodyJointId JointId;

			public BodyJointId ParentId;

			public Pose PoseFromRoot;

			public Pose LocalPose;
		}

		private class Mapping : ISkeletonMapping
		{
			IEnumerableHashSet<BodyJointId> ISkeletonMapping.Joints
			{
				get
				{
					return this.Joints;
				}
			}

			bool ISkeletonMapping.TryGetParentJointId(BodyJointId jointId, out BodyJointId parent)
			{
				return this.JointToParent.TryGetValue(jointId, out parent);
			}

			public EnumerableHashSet<BodyJointId> Joints = new EnumerableHashSet<BodyJointId>();

			public Dictionary<BodyJointId, BodyJointId> JointToParent = new Dictionary<BodyJointId, BodyJointId>();
		}
	}
}
