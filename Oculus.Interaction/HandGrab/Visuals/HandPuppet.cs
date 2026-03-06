using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab.Visuals
{
	public class HandPuppet : MonoBehaviour
	{
		public List<HandJointMap> JointMaps
		{
			get
			{
				return this._jointMaps;
			}
		}

		public float Scale
		{
			get
			{
				return base.transform.localScale.x;
			}
			set
			{
				base.transform.localScale = Vector3.one * value;
			}
		}

		private JointCollection JointsCache
		{
			get
			{
				if (this._jointsCache == null)
				{
					this._jointsCache = new JointCollection(this._jointMaps);
				}
				return this._jointsCache;
			}
		}

		public void SetJointRotations(in Quaternion[] jointRotations)
		{
			int num = 0;
			while (num < FingersMetadata.HAND_JOINT_IDS.Length && num < jointRotations.Length)
			{
				HandJointMap handJointMap = this.JointsCache[num];
				if (handJointMap != null)
				{
					Transform transform = handJointMap.transform;
					Quaternion localRotation = handJointMap.RotationOffset * jointRotations[num];
					transform.localRotation = localRotation;
				}
				num++;
			}
		}

		public void SetRootPose(in Pose rootPose)
		{
			base.transform.SetPose(rootPose, Space.World);
		}

		public void CopyCachedJoints(ref HandPose result)
		{
			for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; i++)
			{
				HandJointMap handJointMap = this.JointsCache[i];
				if (handJointMap != null)
				{
					result.JointRotations[i] = handJointMap.TrackedRotation;
				}
			}
		}

		[SerializeField]
		private List<HandJointMap> _jointMaps = new List<HandJointMap>(FingersMetadata.HAND_JOINT_IDS.Length);

		private JointCollection _jointsCache;
	}
}
