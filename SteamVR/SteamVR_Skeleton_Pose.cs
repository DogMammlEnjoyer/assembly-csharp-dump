using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Skeleton_Pose : ScriptableObject
	{
		public SteamVR_Skeleton_Pose_Hand GetHand(int hand)
		{
			if (hand == 1)
			{
				return this.leftHand;
			}
			if (hand == 2)
			{
				return this.rightHand;
			}
			return null;
		}

		public SteamVR_Skeleton_Pose_Hand GetHand(SteamVR_Input_Sources hand)
		{
			if (hand == SteamVR_Input_Sources.LeftHand)
			{
				return this.leftHand;
			}
			if (hand == SteamVR_Input_Sources.RightHand)
			{
				return this.rightHand;
			}
			return null;
		}

		public SteamVR_Skeleton_Pose_Hand leftHand = new SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources.LeftHand);

		public SteamVR_Skeleton_Pose_Hand rightHand = new SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources.RightHand);

		protected const int leftHandInputSource = 1;

		protected const int rightHandInputSource = 2;

		public bool applyToSkeletonRoot = true;
	}
}
