using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Skeleton_Pose_Hand
	{
		public SteamVR_Skeleton_FingerExtensionTypes GetFingerExtensionType(int finger)
		{
			if (finger == 0)
			{
				return this.thumbFingerMovementType;
			}
			if (finger == 1)
			{
				return this.indexFingerMovementType;
			}
			if (finger == 2)
			{
				return this.middleFingerMovementType;
			}
			if (finger == 3)
			{
				return this.ringFingerMovementType;
			}
			if (finger == 4)
			{
				return this.pinkyFingerMovementType;
			}
			Debug.LogWarning("Finger not in range!");
			return SteamVR_Skeleton_FingerExtensionTypes.Static;
		}

		public SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources source)
		{
			this.inputSource = source;
		}

		public SteamVR_Skeleton_FingerExtensionTypes GetMovementTypeForBone(int boneIndex)
		{
			switch (SteamVR_Skeleton_JointIndexes.GetFingerForBone(boneIndex))
			{
			case 0:
				return this.thumbFingerMovementType;
			case 1:
				return this.indexFingerMovementType;
			case 2:
				return this.middleFingerMovementType;
			case 3:
				return this.ringFingerMovementType;
			case 4:
				return this.pinkyFingerMovementType;
			default:
				return SteamVR_Skeleton_FingerExtensionTypes.Static;
			}
		}

		public SteamVR_Input_Sources inputSource;

		public SteamVR_Skeleton_FingerExtensionTypes thumbFingerMovementType;

		public SteamVR_Skeleton_FingerExtensionTypes indexFingerMovementType;

		public SteamVR_Skeleton_FingerExtensionTypes middleFingerMovementType;

		public SteamVR_Skeleton_FingerExtensionTypes ringFingerMovementType;

		public SteamVR_Skeleton_FingerExtensionTypes pinkyFingerMovementType;

		public bool ignoreRootPoseData = true;

		public bool ignoreWristPoseData = true;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3[] bonePositions;

		public Quaternion[] boneRotations;
	}
}
