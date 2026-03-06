using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab.Visuals
{
	[Serializable]
	public class JointCollection
	{
		public JointCollection(List<HandJointMap> joints)
		{
			this._jointMaps = joints;
			for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; i++)
			{
				HandJointId boneId = FingersMetadata.HAND_JOINT_IDS[i];
				this._jointIndices[i] = joints.FindIndex((HandJointMap bone) => bone.id == boneId);
			}
		}

		public HandJointMap this[int jointIndex]
		{
			get
			{
				int num = this._jointIndices[jointIndex];
				if (num >= 0)
				{
					return this._jointMaps[num];
				}
				return null;
			}
		}

		[SerializeField]
		[HideInInspector]
		private int[] _jointIndices = new int[FingersMetadata.HAND_JOINT_IDS.Length];

		[SerializeField]
		[HideInInspector]
		private List<HandJointMap> _jointMaps;
	}
}
