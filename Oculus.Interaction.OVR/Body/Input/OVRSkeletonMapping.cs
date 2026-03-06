using System;
using System.Collections.Generic;
using Meta.XR.Util;

namespace Oculus.Interaction.Body.Input
{
	[Feature(Feature.Interaction)]
	public class OVRSkeletonMapping : BodySkeletonMapping<OVRPlugin.BoneId>, ISkeletonMapping
	{
		[Obsolete("Use the parameterized constructor instead", true)]
		public OVRSkeletonMapping() : base(OVRPlugin.BoneId.Hand_Start, OVRSkeletonMapping._upperBodyJoints)
		{
		}

		public OVRSkeletonMapping(OVRPlugin.BodyJointSet skeletonType) : base(OVRSkeletonMapping.GetRoot(), OVRSkeletonMapping.GetJointMapping(skeletonType))
		{
		}

		private static IReadOnlyDictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo> GetJointMapping(OVRPlugin.BodyJointSet jointSet)
		{
			Dictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo> dictionary = new Dictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo>();
			foreach (KeyValuePair<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo> keyValuePair in OVRSkeletonMapping._upperBodyJoints)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			if (jointSet == OVRPlugin.BodyJointSet.FullBody)
			{
				foreach (KeyValuePair<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo> keyValuePair2 in OVRSkeletonMapping._lowerBodyJoints)
				{
					dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
			}
			return dictionary;
		}

		private static OVRPlugin.BoneId GetRoot()
		{
			return OVRPlugin.BoneId.Hand_Start;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static OVRSkeletonMapping()
		{
			Dictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo> dictionary = new Dictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo>();
			dictionary[BodyJointId.Body_Start] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Start, OVRPlugin.BoneId.Hand_Start);
			dictionary[BodyJointId.Body_Hips] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_ForearmStub, OVRPlugin.BoneId.Hand_Start);
			dictionary[BodyJointId.Body_SpineLower] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Thumb0, OVRPlugin.BoneId.Hand_ForearmStub);
			dictionary[BodyJointId.Body_SpineMiddle] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Thumb1, OVRPlugin.BoneId.Hand_Thumb0);
			dictionary[BodyJointId.Body_SpineUpper] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Thumb2, OVRPlugin.BoneId.Hand_Thumb1);
			dictionary[BodyJointId.Body_Chest] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Thumb3, OVRPlugin.BoneId.Hand_Thumb2);
			dictionary[BodyJointId.Body_Neck] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Index1, OVRPlugin.BoneId.Hand_Thumb3);
			dictionary[BodyJointId.Body_Head] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Index2, OVRPlugin.BoneId.Hand_Index1);
			dictionary[BodyJointId.Body_LeftShoulder] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Index3, OVRPlugin.BoneId.Hand_Thumb3);
			dictionary[BodyJointId.Body_LeftScapula] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Middle1, OVRPlugin.BoneId.Hand_Index3);
			dictionary[BodyJointId.Body_LeftArmUpper] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Middle2, OVRPlugin.BoneId.Hand_Middle1);
			dictionary[BodyJointId.Body_LeftArmLower] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Middle3, OVRPlugin.BoneId.Hand_Middle2);
			dictionary[BodyJointId.Body_LeftHandWristTwist] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Ring1, OVRPlugin.BoneId.Hand_Middle3);
			dictionary[BodyJointId.Body_RightShoulder] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Ring2, OVRPlugin.BoneId.Hand_Thumb3);
			dictionary[BodyJointId.Body_RightScapula] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Ring3, OVRPlugin.BoneId.Hand_Ring2);
			dictionary[BodyJointId.Body_RightArmUpper] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Pinky0, OVRPlugin.BoneId.Hand_Ring3);
			dictionary[BodyJointId.Body_RightArmLower] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Pinky1, OVRPlugin.BoneId.Hand_Pinky0);
			dictionary[BodyJointId.Body_RightHandWristTwist] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Pinky2, OVRPlugin.BoneId.Hand_Pinky1);
			dictionary[BodyJointId.Body_LeftHandPalm] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_Pinky3, OVRPlugin.BoneId.Hand_MaxSkinnable);
			dictionary[BodyJointId.Body_LeftHandWrist] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_MaxSkinnable, OVRPlugin.BoneId.Hand_Middle3);
			dictionary[BodyJointId.Body_LeftHandThumbMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_IndexTip, OVRPlugin.BoneId.Hand_MaxSkinnable);
			dictionary[BodyJointId.Body_LeftHandThumbProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_MiddleTip, OVRPlugin.BoneId.Hand_IndexTip);
			dictionary[BodyJointId.Body_LeftHandThumbDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_RingTip, OVRPlugin.BoneId.Hand_MiddleTip);
			dictionary[BodyJointId.Body_LeftHandThumbTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_PinkyTip, OVRPlugin.BoneId.Hand_RingTip);
			dictionary[BodyJointId.Body_LeftHandIndexMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Hand_End, OVRPlugin.BoneId.Hand_MaxSkinnable);
			dictionary[BodyJointId.Body_LeftHandIndexProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.XRHand_LittleTip, OVRPlugin.BoneId.Hand_End);
			dictionary[BodyJointId.Body_LeftHandIndexIntermediate] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.XRHand_Max, OVRPlugin.BoneId.XRHand_LittleTip);
			dictionary[BodyJointId.Body_LeftHandIndexDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandIndexDistal, OVRPlugin.BoneId.XRHand_Max);
			dictionary[BodyJointId.Body_LeftHandIndexTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandIndexTip, OVRPlugin.BoneId.Body_LeftHandIndexDistal);
			dictionary[BodyJointId.Body_LeftHandMiddleMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandMiddleMetacarpal, OVRPlugin.BoneId.Hand_MaxSkinnable);
			dictionary[BodyJointId.Body_LeftHandMiddleProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandMiddleProximal, OVRPlugin.BoneId.Body_LeftHandMiddleMetacarpal);
			dictionary[BodyJointId.Body_LeftHandMiddleIntermediate] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandMiddleIntermediate, OVRPlugin.BoneId.Body_LeftHandMiddleProximal);
			dictionary[BodyJointId.Body_LeftHandMiddleDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandMiddleDistal, OVRPlugin.BoneId.Body_LeftHandMiddleIntermediate);
			dictionary[BodyJointId.Body_LeftHandMiddleTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandMiddleTip, OVRPlugin.BoneId.Body_LeftHandMiddleDistal);
			dictionary[BodyJointId.Body_LeftHandRingMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandRingMetacarpal, OVRPlugin.BoneId.Hand_MaxSkinnable);
			dictionary[BodyJointId.Body_LeftHandRingProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandRingProximal, OVRPlugin.BoneId.Body_LeftHandRingMetacarpal);
			dictionary[BodyJointId.Body_LeftHandRingIntermediate] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandRingIntermediate, OVRPlugin.BoneId.Body_LeftHandRingProximal);
			dictionary[BodyJointId.Body_LeftHandRingDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandRingDistal, OVRPlugin.BoneId.Body_LeftHandRingIntermediate);
			dictionary[BodyJointId.Body_LeftHandRingTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandRingTip, OVRPlugin.BoneId.Body_LeftHandRingDistal);
			dictionary[BodyJointId.Body_LeftHandLittleMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandLittleMetacarpal, OVRPlugin.BoneId.Hand_MaxSkinnable);
			dictionary[BodyJointId.Body_LeftHandLittleProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandLittleProximal, OVRPlugin.BoneId.Body_LeftHandLittleMetacarpal);
			dictionary[BodyJointId.Body_LeftHandLittleIntermediate] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandLittleIntermediate, OVRPlugin.BoneId.Body_LeftHandLittleProximal);
			dictionary[BodyJointId.Body_LeftHandLittleDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandLittleDistal, OVRPlugin.BoneId.Body_LeftHandLittleIntermediate);
			dictionary[BodyJointId.Body_LeftHandLittleTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_LeftHandLittleTip, OVRPlugin.BoneId.Body_LeftHandLittleDistal);
			dictionary[BodyJointId.Body_RightHandPalm] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandPalm, OVRPlugin.BoneId.Body_RightHandWrist);
			dictionary[BodyJointId.Body_RightHandWrist] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandWrist, OVRPlugin.BoneId.Hand_Pinky1);
			dictionary[BodyJointId.Body_RightHandThumbMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandThumbMetacarpal, OVRPlugin.BoneId.Body_RightHandWrist);
			dictionary[BodyJointId.Body_RightHandThumbProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandThumbProximal, OVRPlugin.BoneId.Body_RightHandThumbMetacarpal);
			dictionary[BodyJointId.Body_RightHandThumbDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandThumbDistal, OVRPlugin.BoneId.Body_RightHandThumbProximal);
			dictionary[BodyJointId.Body_RightHandThumbTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandThumbTip, OVRPlugin.BoneId.Body_RightHandThumbDistal);
			dictionary[BodyJointId.Body_RightHandIndexMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandIndexMetacarpal, OVRPlugin.BoneId.Body_RightHandWrist);
			dictionary[BodyJointId.Body_RightHandIndexProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandIndexProximal, OVRPlugin.BoneId.Body_RightHandIndexMetacarpal);
			dictionary[BodyJointId.Body_RightHandIndexIntermediate] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandIndexIntermediate, OVRPlugin.BoneId.Body_RightHandIndexProximal);
			dictionary[BodyJointId.Body_RightHandIndexDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandIndexDistal, OVRPlugin.BoneId.Body_RightHandIndexIntermediate);
			dictionary[BodyJointId.Body_RightHandIndexTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandIndexTip, OVRPlugin.BoneId.Body_RightHandIndexDistal);
			dictionary[BodyJointId.Body_RightHandMiddleMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandMiddleMetacarpal, OVRPlugin.BoneId.Body_RightHandWrist);
			dictionary[BodyJointId.Body_RightHandMiddleProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandMiddleProximal, OVRPlugin.BoneId.Body_RightHandMiddleMetacarpal);
			dictionary[BodyJointId.Body_RightHandMiddleIntermediate] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandMiddleIntermediate, OVRPlugin.BoneId.Body_RightHandMiddleProximal);
			dictionary[BodyJointId.Body_RightHandMiddleDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandMiddleDistal, OVRPlugin.BoneId.Body_RightHandMiddleIntermediate);
			dictionary[BodyJointId.Body_RightHandMiddleTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandMiddleTip, OVRPlugin.BoneId.Body_RightHandMiddleDistal);
			dictionary[BodyJointId.Body_RightHandRingMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandRingMetacarpal, OVRPlugin.BoneId.Body_RightHandWrist);
			dictionary[BodyJointId.Body_RightHandRingProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandRingProximal, OVRPlugin.BoneId.Body_RightHandRingMetacarpal);
			dictionary[BodyJointId.Body_RightHandRingIntermediate] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandRingIntermediate, OVRPlugin.BoneId.Body_RightHandRingProximal);
			dictionary[BodyJointId.Body_RightHandRingDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandRingDistal, OVRPlugin.BoneId.Body_RightHandRingIntermediate);
			dictionary[BodyJointId.Body_RightHandRingTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandRingTip, OVRPlugin.BoneId.Body_RightHandRingDistal);
			dictionary[BodyJointId.Body_RightHandLittleMetacarpal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandLittleMetacarpal, OVRPlugin.BoneId.Body_RightHandWrist);
			dictionary[BodyJointId.Body_RightHandLittleProximal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandLittleProximal, OVRPlugin.BoneId.Body_RightHandLittleMetacarpal);
			dictionary[BodyJointId.Body_RightHandLittleIntermediate] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandLittleIntermediate, OVRPlugin.BoneId.Body_RightHandLittleProximal);
			dictionary[BodyJointId.Body_RightHandLittleDistal] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandLittleDistal, OVRPlugin.BoneId.Body_RightHandLittleIntermediate);
			dictionary[BodyJointId.Body_RightHandLittleTip] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_RightHandLittleTip, OVRPlugin.BoneId.Body_RightHandLittleDistal);
			OVRSkeletonMapping._upperBodyJoints = dictionary;
			Dictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo> dictionary2 = new Dictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo>();
			dictionary2[BodyJointId.Body_LeftLegUpper] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.Body_End, OVRPlugin.BoneId.Hand_ForearmStub);
			dictionary2[BodyJointId.Body_LeftLegLower] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_LeftLowerLeg, OVRPlugin.BoneId.Body_End);
			dictionary2[BodyJointId.Body_LeftFootAnkleTwist] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_LeftFootAnkleTwist, OVRPlugin.BoneId.FullBody_LeftLowerLeg);
			dictionary2[BodyJointId.Body_LeftFootAnkle] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_LeftFootAnkle, OVRPlugin.BoneId.FullBody_LeftFootAnkleTwist);
			dictionary2[BodyJointId.Body_LeftFootSubtalar] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_LeftFootSubtalar, OVRPlugin.BoneId.FullBody_LeftFootAnkle);
			dictionary2[BodyJointId.Body_LeftFootTransverse] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_LeftFootTransverse, OVRPlugin.BoneId.FullBody_LeftFootSubtalar);
			dictionary2[BodyJointId.Body_LeftFootBall] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_LeftFootBall, OVRPlugin.BoneId.FullBody_LeftFootTransverse);
			dictionary2[BodyJointId.Body_RightLegUpper] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_RightUpperLeg, OVRPlugin.BoneId.Hand_ForearmStub);
			dictionary2[BodyJointId.Body_RightLegLower] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_RightLowerLeg, OVRPlugin.BoneId.FullBody_RightUpperLeg);
			dictionary2[BodyJointId.Body_RightFootAnkleTwist] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_RightFootAnkleTwist, OVRPlugin.BoneId.FullBody_RightLowerLeg);
			dictionary2[BodyJointId.Body_RightFootAnkle] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_RightFootAnkle, OVRPlugin.BoneId.FullBody_RightFootAnkleTwist);
			dictionary2[BodyJointId.Body_RightFootSubtalar] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_RightFootSubtalar, OVRPlugin.BoneId.FullBody_RightFootAnkle);
			dictionary2[BodyJointId.Body_RightFootTransverse] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_RightFootTransverse, OVRPlugin.BoneId.FullBody_RightFootSubtalar);
			dictionary2[BodyJointId.Body_RightFootBall] = new BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo(OVRPlugin.BoneId.FullBody_RightFootBall, OVRPlugin.BoneId.FullBody_RightFootTransverse);
			OVRSkeletonMapping._lowerBodyJoints = dictionary2;
		}

		private static readonly Dictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo> _upperBodyJoints;

		private static readonly Dictionary<BodyJointId, BodySkeletonMapping<OVRPlugin.BoneId>.JointInfo> _lowerBodyJoints;
	}
}
