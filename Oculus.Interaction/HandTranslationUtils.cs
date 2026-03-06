using System;
using Oculus.Interaction.Input;
using Oculus.Interaction.Input.Compatibility.OVR;
using UnityEngine;

namespace Oculus.Interaction
{
	public static class HandTranslationUtils
	{
		public static GUIStyle FixButtonStyle
		{
			get
			{
				return new GUIStyle(GUI.skin.button)
				{
					stretchWidth = true,
					stretchHeight = true,
					fixedWidth = 60f
				};
			}
		}

		public static int OpenXRHandJointToOVR(int openXRJointId)
		{
			Oculus.Interaction.Input.Compatibility.OVR.HandJointId result;
			switch (openXRJointId)
			{
			case 0:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.Invalid;
				break;
			case 1:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandStart;
				break;
			case 2:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandThumb1;
				break;
			case 3:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandThumb2;
				break;
			case 4:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandThumb3;
				break;
			case 5:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandMaxSkinnable;
				break;
			case 6:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.Invalid;
				break;
			case 7:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandIndex1;
				break;
			case 8:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandIndex2;
				break;
			case 9:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandIndex3;
				break;
			case 10:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandIndexTip;
				break;
			case 11:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.Invalid;
				break;
			case 12:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandMiddle1;
				break;
			case 13:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandMiddle2;
				break;
			case 14:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandMiddle3;
				break;
			case 15:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandMiddleTip;
				break;
			case 16:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.Invalid;
				break;
			case 17:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandRing1;
				break;
			case 18:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandRing2;
				break;
			case 19:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandRing3;
				break;
			case 20:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandRingTip;
				break;
			case 21:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandPinky0;
				break;
			case 22:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandPinky1;
				break;
			case 23:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandPinky2;
				break;
			case 24:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandPinky3;
				break;
			case 25:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandPinkyTip;
				break;
			default:
				result = Oculus.Interaction.Input.Compatibility.OVR.HandJointId.Invalid;
				break;
			}
			return (int)result;
		}

		public static bool OVRHandRotationsToOpenXRPoses(Quaternion[] ovrJointRotations, Oculus.Interaction.Input.Handedness handedness, ref Pose[] targetPoses)
		{
			if (ovrJointRotations.Length < 24 || targetPoses.Length < 26)
			{
				return false;
			}
			Oculus.Interaction.Input.Compatibility.OVR.HandSkeleton handSkeleton = (handedness == Oculus.Interaction.Input.Handedness.Left) ? Oculus.Interaction.Input.Compatibility.OVR.HandSkeleton.DefaultLeftSkeleton : Oculus.Interaction.Input.Compatibility.OVR.HandSkeleton.DefaultRightSkeleton;
			Oculus.Interaction.Input.HandSkeleton handSkeleton2 = (handedness == Oculus.Interaction.Input.Handedness.Left) ? Oculus.Interaction.Input.HandSkeleton.DefaultLeftSkeleton : Oculus.Interaction.Input.HandSkeleton.DefaultRightSkeleton;
			targetPoses[1] = handSkeleton2.Joints[1].pose;
			for (int i = 0; i < 26; i++)
			{
				Pose pose = handSkeleton2[i].pose;
				int num = (int)Oculus.Interaction.Input.HandJointUtils.JointParentList[i];
				if (num >= 0)
				{
					Oculus.Interaction.Input.Compatibility.OVR.HandJointId handJointId = (Oculus.Interaction.Input.Compatibility.OVR.HandJointId)HandTranslationUtils.OpenXRHandJointToOVR(i);
					if (Oculus.Interaction.Input.HandJointUtils.JointToFingerList[i] == Oculus.Interaction.Input.HandFinger.Thumb)
					{
						Pose identity = Pose.identity;
						for (int j = (int)handJointId; j >= 0; j = (int)Oculus.Interaction.Input.Compatibility.OVR.HandJointUtils.JointParentList[j])
						{
							Pose pose2 = handSkeleton.Joints[j].pose;
							pose2.rotation = ovrJointRotations[j];
							ref identity.Postmultiply(pose2);
						}
						identity.rotation = identity.rotation.normalized;
						Pose[] array = targetPoses;
						int num2 = i;
						HandMirroring.HandSpace handSpace = HandTranslationUtils.ovrHands[handedness];
						HandMirroring.HandSpace handSpace2 = HandTranslationUtils.openXRHands[handedness];
						array[num2] = HandMirroring.TransformPose(identity, handSpace, handSpace2);
					}
					else
					{
						if (handJointId != Oculus.Interaction.Input.Compatibility.OVR.HandJointId.Invalid && handJointId < Oculus.Interaction.Input.Compatibility.OVR.HandJointId.HandMaxSkinnable)
						{
							int num3 = (int)handJointId;
							HandMirroring.HandSpace handSpace = HandTranslationUtils.ovrHands[handedness];
							HandMirroring.HandSpace handSpace2 = HandTranslationUtils.openXRHands[handedness];
							pose.rotation = HandMirroring.TransformRotation(ovrJointRotations[num3], handSpace, handSpace2);
						}
						PoseUtils.Multiply(targetPoses[num], pose, ref targetPoses[i]);
					}
				}
			}
			return true;
		}

		public static Oculus.Interaction.Input.HandJointId OVRHandJointToOpenXR(int ovrJointId)
		{
			Oculus.Interaction.Input.HandJointId result;
			switch (ovrJointId)
			{
			case 0:
				result = Oculus.Interaction.Input.HandJointId.HandWristRoot;
				break;
			case 1:
				result = Oculus.Interaction.Input.HandJointId.Invalid;
				break;
			case 2:
				result = Oculus.Interaction.Input.HandJointId.Invalid;
				break;
			case 3:
				result = Oculus.Interaction.Input.HandJointId.HandThumb1;
				break;
			case 4:
				result = Oculus.Interaction.Input.HandJointId.HandThumb2;
				break;
			case 5:
				result = Oculus.Interaction.Input.HandJointId.HandThumb3;
				break;
			case 6:
				result = Oculus.Interaction.Input.HandJointId.HandIndex1;
				break;
			case 7:
				result = Oculus.Interaction.Input.HandJointId.HandIndex2;
				break;
			case 8:
				result = Oculus.Interaction.Input.HandJointId.HandIndex3;
				break;
			case 9:
				result = Oculus.Interaction.Input.HandJointId.HandMiddle1;
				break;
			case 10:
				result = Oculus.Interaction.Input.HandJointId.HandMiddle2;
				break;
			case 11:
				result = Oculus.Interaction.Input.HandJointId.HandMiddle3;
				break;
			case 12:
				result = Oculus.Interaction.Input.HandJointId.HandRing1;
				break;
			case 13:
				result = Oculus.Interaction.Input.HandJointId.HandRing2;
				break;
			case 14:
				result = Oculus.Interaction.Input.HandJointId.HandRing3;
				break;
			case 15:
				result = Oculus.Interaction.Input.HandJointId.HandPinky0;
				break;
			case 16:
				result = Oculus.Interaction.Input.HandJointId.HandPinky1;
				break;
			case 17:
				result = Oculus.Interaction.Input.HandJointId.HandPinky2;
				break;
			case 18:
				result = Oculus.Interaction.Input.HandJointId.HandPinky3;
				break;
			case 19:
				result = Oculus.Interaction.Input.HandJointId.HandThumbTip;
				break;
			case 20:
				result = Oculus.Interaction.Input.HandJointId.HandIndexTip;
				break;
			case 21:
				result = Oculus.Interaction.Input.HandJointId.HandMiddleTip;
				break;
			case 22:
				result = Oculus.Interaction.Input.HandJointId.HandRingTip;
				break;
			case 23:
				result = Oculus.Interaction.Input.HandJointId.HandPinkyTip;
				break;
			default:
				result = Oculus.Interaction.Input.HandJointId.Invalid;
				break;
			}
			return result;
		}

		public static Vector3 TransformOVRToOpenXRPosition(Vector3 position, Oculus.Interaction.Input.Handedness handedness)
		{
			HandMirroring.HandSpace handSpace = HandTranslationUtils.ovrHands[handedness];
			HandMirroring.HandSpace handSpace2 = HandTranslationUtils.openXRHands[handedness];
			return HandMirroring.TransformPosition(position, handSpace, handSpace2);
		}

		public static Quaternion TransformOVRToOpenXRRotation(Quaternion rotation, Oculus.Interaction.Input.Handedness handedness)
		{
			HandMirroring.HandSpace handSpace = HandTranslationUtils.ovrHands[handedness];
			HandMirroring.HandSpace handSpace2 = HandTranslationUtils.openXRHands[handedness];
			return HandMirroring.TransformRotation(rotation, handSpace, handSpace2);
		}

		public const string UpgradeRequiredMessage = "Some fields do not contain the expected values of converting to OpenXR from the previous serialized data. Convert the values?";

		public const string UpgradeRequiredButton = "Convert";

		private static readonly HandMirroring.HandSpace _openXRLeft = new HandMirroring.HandSpace(Oculus.Interaction.Input.Constants.LeftDistal, Oculus.Interaction.Input.Constants.LeftDorsal, Oculus.Interaction.Input.Constants.LeftThumbSide);

		private static readonly HandMirroring.HandSpace _openXRRight = new HandMirroring.HandSpace(Oculus.Interaction.Input.Constants.RightDistal, Oculus.Interaction.Input.Constants.RightDorsal, Oculus.Interaction.Input.Constants.RightThumbSide);

		private static readonly HandMirroring.HandSpace _ovrLeft = new HandMirroring.HandSpace(Oculus.Interaction.Input.Compatibility.OVR.Constants.LeftDistal, Oculus.Interaction.Input.Compatibility.OVR.Constants.LeftDorsal, Oculus.Interaction.Input.Compatibility.OVR.Constants.LeftThumbSide);

		private static readonly HandMirroring.HandSpace _ovrRight = new HandMirroring.HandSpace(Oculus.Interaction.Input.Compatibility.OVR.Constants.RightDistal, Oculus.Interaction.Input.Compatibility.OVR.Constants.RightDorsal, Oculus.Interaction.Input.Compatibility.OVR.Constants.RightThumbSide);

		public static readonly HandMirroring.HandsSpace openXRHands = new HandMirroring.HandsSpace(HandTranslationUtils._openXRLeft, HandTranslationUtils._openXRRight);

		public static readonly HandMirroring.HandsSpace ovrHands = new HandMirroring.HandsSpace(HandTranslationUtils._ovrLeft, HandTranslationUtils._ovrRight);

		public static int[] HAND_JOINT_IDS_OpenXRtoOVR = new int[]
		{
			1,
			2,
			3,
			-1,
			4,
			5,
			6,
			-1,
			7,
			8,
			9,
			-1,
			10,
			11,
			12,
			13,
			14,
			15,
			16
		};
	}
}
