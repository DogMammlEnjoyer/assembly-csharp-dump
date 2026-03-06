using System;
using UnityEngine;

namespace Oculus.Interaction.Body.Input
{
	public enum BodyJointId
	{
		Invalid = -1,
		[InspectorName("Body Start")]
		Body_Start,
		[InspectorName("Root")]
		Body_Root = 0,
		[InspectorName("Hips")]
		Body_Hips,
		[InspectorName("Spine Lower")]
		Body_SpineLower,
		[InspectorName("Spine Middle")]
		Body_SpineMiddle,
		[InspectorName("Spine Upper")]
		Body_SpineUpper,
		[InspectorName("Chest")]
		Body_Chest,
		[InspectorName("Neck")]
		Body_Neck,
		[InspectorName("Head")]
		Body_Head,
		[InspectorName("Left Arm/Left Shoulder")]
		Body_LeftShoulder,
		[InspectorName("Left Arm/Left Scapula")]
		Body_LeftScapula,
		[InspectorName("Left Arm/Left Arm Upper")]
		Body_LeftArmUpper,
		[InspectorName("Left Arm/Left Arm Lower")]
		Body_LeftArmLower,
		[InspectorName("Left Arm/Left Hand Wrist Twist")]
		Body_LeftHandWristTwist,
		[InspectorName("Right Arm/Right Shoulder")]
		Body_RightShoulder,
		[InspectorName("Right Arm/Right Scapula")]
		Body_RightScapula,
		[InspectorName("Right Arm/Right Arm Upper")]
		Body_RightArmUpper,
		[InspectorName("Right Arm/Right Arm Lower")]
		Body_RightArmLower,
		[InspectorName("Right Arm/Right Hand Wrist Twist")]
		Body_RightHandWristTwist,
		[InspectorName("Left Hand/Left Hand Palm")]
		Body_LeftHandPalm,
		[InspectorName("Left Hand/Left Hand Wrist")]
		Body_LeftHandWrist,
		[InspectorName("Left Hand/Left Hand Thumb Metacarpal")]
		Body_LeftHandThumbMetacarpal,
		[InspectorName("Left Hand/Left Hand Thumb Proximal")]
		Body_LeftHandThumbProximal,
		[InspectorName("Left Hand/Left Hand Thumb Distal")]
		Body_LeftHandThumbDistal,
		[InspectorName("Left Hand/Left Hand Thumb Tip")]
		Body_LeftHandThumbTip,
		[InspectorName("Left Hand/Left Hand Index Metacarpal")]
		Body_LeftHandIndexMetacarpal,
		[InspectorName("Left Hand/Left Hand Index Proximal")]
		Body_LeftHandIndexProximal,
		[InspectorName("Left Hand/Left Hand Index Intermediate")]
		Body_LeftHandIndexIntermediate,
		[InspectorName("Left Hand/Left Hand Index Distal")]
		Body_LeftHandIndexDistal,
		[InspectorName("Left Hand/Left Hand Index Tip")]
		Body_LeftHandIndexTip,
		[InspectorName("Left Hand/Left Hand Middle Metacarpal")]
		Body_LeftHandMiddleMetacarpal,
		[InspectorName("Left Hand/Left Hand Middle Proximal")]
		Body_LeftHandMiddleProximal,
		[InspectorName("Left Hand/Left Hand Middle Intermediate")]
		Body_LeftHandMiddleIntermediate,
		[InspectorName("Left Hand/Left Hand Middle Distal")]
		Body_LeftHandMiddleDistal,
		[InspectorName("Left Hand/Left Hand Middle Tip")]
		Body_LeftHandMiddleTip,
		[InspectorName("Left Hand/Left Hand Ring Metacarpal")]
		Body_LeftHandRingMetacarpal,
		[InspectorName("Left Hand/Left Hand Ring Proximal")]
		Body_LeftHandRingProximal,
		[InspectorName("Left Hand/Left Hand Ring Intermediate")]
		Body_LeftHandRingIntermediate,
		[InspectorName("Left Hand/Left Hand Ring Distal")]
		Body_LeftHandRingDistal,
		[InspectorName("Left Hand/Left Hand Ring Tip")]
		Body_LeftHandRingTip,
		[InspectorName("Left Hand/Left Hand Little Metacarpal")]
		Body_LeftHandLittleMetacarpal,
		[InspectorName("Left Hand/Left Hand Little Proximal")]
		Body_LeftHandLittleProximal,
		[InspectorName("Left Hand/Left Hand Little Intermediate")]
		Body_LeftHandLittleIntermediate,
		[InspectorName("Left Hand/Left Hand Little Distal")]
		Body_LeftHandLittleDistal,
		[InspectorName("Left Hand/Left Hand Little Tip")]
		Body_LeftHandLittleTip,
		[InspectorName("Right Hand/Right Hand Palm")]
		Body_RightHandPalm,
		[InspectorName("Right Hand/Right Hand Wrist")]
		Body_RightHandWrist,
		[InspectorName("Right Hand/Right Hand Thumb Metacarpal")]
		Body_RightHandThumbMetacarpal,
		[InspectorName("Right Hand/Right Hand Thumb Proximal")]
		Body_RightHandThumbProximal,
		[InspectorName("Right Hand/Right Hand Thumb Distal")]
		Body_RightHandThumbDistal,
		[InspectorName("Right Hand/Right Hand Thumb Tip")]
		Body_RightHandThumbTip,
		[InspectorName("Right Hand/Right Hand Index Metacarpal")]
		Body_RightHandIndexMetacarpal,
		[InspectorName("Right Hand/Right Hand Index Proximal")]
		Body_RightHandIndexProximal,
		[InspectorName("Right Hand/Right Hand Index Intermediate")]
		Body_RightHandIndexIntermediate,
		[InspectorName("Right Hand/Right Hand Index Distal")]
		Body_RightHandIndexDistal,
		[InspectorName("Right Hand/Right Hand Index Tip")]
		Body_RightHandIndexTip,
		[InspectorName("Right Hand/Right Hand Middle Metacarpal")]
		Body_RightHandMiddleMetacarpal,
		[InspectorName("Right Hand/Right Hand Middle Proximal")]
		Body_RightHandMiddleProximal,
		[InspectorName("Right Hand/Right Hand Middle Intermediate")]
		Body_RightHandMiddleIntermediate,
		[InspectorName("Right Hand/Right Hand Middle Distal")]
		Body_RightHandMiddleDistal,
		[InspectorName("Right Hand/Right Hand Middle Tip")]
		Body_RightHandMiddleTip,
		[InspectorName("Right Hand/Right Hand Ring Metacarpal")]
		Body_RightHandRingMetacarpal,
		[InspectorName("Right Hand/Right Hand Ring Proximal")]
		Body_RightHandRingProximal,
		[InspectorName("Right Hand/Right Hand Ring Intermediate")]
		Body_RightHandRingIntermediate,
		[InspectorName("Right Hand/Right Hand Ring Distal")]
		Body_RightHandRingDistal,
		[InspectorName("Right Hand/Right Hand Ring Tip")]
		Body_RightHandRingTip,
		[InspectorName("Right Hand/Right Hand Little Metacarpal")]
		Body_RightHandLittleMetacarpal,
		[InspectorName("Right Hand/Right Hand Little Proximal")]
		Body_RightHandLittleProximal,
		[InspectorName("Right Hand/Right Hand Little Intermediate")]
		Body_RightHandLittleIntermediate,
		[InspectorName("Right Hand/Right Hand Little Distal")]
		Body_RightHandLittleDistal,
		[InspectorName("Right Hand/Right Hand Little Tip")]
		Body_RightHandLittleTip,
		[InspectorName("Left Leg/Left Leg Upper")]
		Body_LeftLegUpper,
		[InspectorName("Left Leg/Left Leg Lower")]
		Body_LeftLegLower,
		[InspectorName("Left Foot/Left Foot Ankle Twist")]
		Body_LeftFootAnkleTwist,
		[InspectorName("Left Foot/Left Foot Ankle")]
		Body_LeftFootAnkle,
		[InspectorName("Left Foot/Left Foot Subtalar")]
		Body_LeftFootSubtalar,
		[InspectorName("Left Foot/Left Foot Transverse")]
		Body_LeftFootTransverse,
		[InspectorName("Left Foot/Left Foot Ball")]
		Body_LeftFootBall,
		[InspectorName("Right Leg/Right Leg Upper")]
		Body_RightLegUpper,
		[InspectorName("Right Leg/Right Leg Lower")]
		Body_RightLegLower,
		[InspectorName("Right Foot/Right Foot Ankle Twist")]
		Body_RightFootAnkleTwist,
		[InspectorName("Right Foot/Right Foot Ankle")]
		Body_RightFootAnkle,
		[InspectorName("Right Foot/Right Foot Subtalar")]
		Body_RightFootSubtalar,
		[InspectorName("Right Foot/Right Foot Transverse")]
		Body_RightFootTransverse,
		[InspectorName("Right Foot/Right Foot Ball")]
		Body_RightFootBall,
		[InspectorName("Body End")]
		Body_End
	}
}
